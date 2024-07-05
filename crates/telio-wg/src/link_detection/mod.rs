use enhanced_detection::EnhancedDetection;
use std::{
    collections::HashMap,
    default,
    net::IpAddr,
    sync::{Arc, Mutex},
    time::Duration,
};
use tokio::time::Instant;

use telio_crypto::PublicKey;
use telio_model::{
    features::FeatureLinkDetection,
    mesh::{LinkState, NodeState},
};
use telio_task::io::{chan, Chan};
use telio_utils::{
    telio_err_with_log, telio_log_debug, telio_log_error, telio_log_trace, telio_log_warn,
};

use crate::wg::{BytesAndTimestamps, WG_KEEPALIVE};

mod enhanced_detection;

pub struct LinkDetection {
    cfg_max_allowed_rtt: Duration,
    enhanced_detection: Option<EnhancedDetection>,
    ping_channel: chan::Tx<Vec<IpAddr>>,
    peers: HashMap<PublicKey, State>,
}

impl LinkDetection {
    pub fn new(cfg: FeatureLinkDetection) -> Self {
        let ping_channel = Chan::default();
        let enhanced_detection = if cfg.no_of_pings != 0 {
            EnhancedDetection::start_with(ping_channel.rx, cfg.no_of_pings).ok()
        } else {
            None
        };

        LinkDetection {
            cfg_max_allowed_rtt: Duration::from_secs(cfg.rtt_seconds),
            enhanced_detection,
            ping_channel: ping_channel.tx,
            peers: HashMap::default(),
        }
    }

    pub fn insert(&mut self, public_key: &PublicKey, stats: Arc<Mutex<BytesAndTimestamps>>) {
        self.peers
            .insert(*public_key, State::new(stats, self.cfg_max_allowed_rtt));
    }

    pub async fn update(
        &mut self,
        public_key: &PublicKey,
        node_addresses: Vec<IpAddr>,
        push: bool,
    ) -> LinkDetectionUpdateResult {
        // We want to update info only on pull
        if push {
            return self.push_update(public_key);
        }

        if let Some(state) = self.peers.get_mut(public_key) {
            let ping_enabled = self.enhanced_detection.is_some();
            let result = state.update(self.cfg_max_allowed_rtt, ping_enabled);

            if ping_enabled && result.should_ping {
                if let Err(e) = self.ping_channel.send(node_addresses).await {
                    telio_log_warn!("Failed to trigger ping to {:?} {:?}", public_key, e);
                }
            }

            result.link_detection_update_result
        } else {
            LinkDetectionUpdateResult {
                should_notify: false,
                link_state: Some(LinkState::Down),
            }
        }
    }

    pub fn remove(&mut self, public_key: &PublicKey) {
        self.peers.remove(public_key);
    }

    pub async fn stop(self) {
        if let Some(ed) = self.enhanced_detection {
            ed.stop().await;
        }
    }

    fn push_update(&self, public_key: &PublicKey) -> LinkDetectionUpdateResult {
        LinkDetectionUpdateResult {
            should_notify: false,
            link_state: Some(
                self.peers
                    .get(public_key)
                    .map(|p| p.current_link_state())
                    .unwrap_or(LinkState::Down),
            ),
        }
    }
}

#[derive(Default)]
pub struct LinkDetectionUpdateResult {
    pub should_notify: bool,
    pub link_state: Option<LinkState>,
}

// +--------------+
// |      Down    |---------------------+
// +--------------+                     |
//         |                +----------------------+
//         |                |     PossibleDown     |
//         |                +----------------------+
//         |                            |
//  +--------------+                    |
//  |     Up       |--------------------+
//  +--------------+
// NodeState != Connected should act as a reset "button" on the fsm
// Whenever it is detected, it will reset the fsm into StateVariant::Down
// Transitions to StateVariant::Up are straight forward, when the is_link_up condition is true
// Transition from StateVariant::Up to StateVariant::Down is made through an additional state
// StateVariant::PossibleDown which introduces a little delay of 3 seconds until we report link state Down
enum StateVariant {
    Down,
    PossibleDown { deadline: Instant },
    Up,
}

struct StateUpdateResult {
    should_ping: bool,
    link_detection_update_result: LinkDetectionUpdateResult,
}

enum StateDecision {
    NoAction,
    Notify,
    Ping,
}

impl StateDecision {
    fn should_notify(&self) -> bool {
        matches!(self, StateDecision::Notify)
    }

    fn should_ping(&self) -> bool {
        matches!(self, StateDecision::Ping)
    }
}

struct State {
    stats: Arc<Mutex<BytesAndTimestamps>>,
    variant: StateVariant,
}

impl State {
    const POSSIBLE_DOWN_DELAY: Duration = Duration::from_secs(3);

    fn new(stats: Arc<Mutex<BytesAndTimestamps>>, cfg_max_allowed_rtt: Duration) -> Self {
        State {
            stats: stats.clone(),
            variant: match stats.lock() {
                Ok(s) => {
                    if s.is_link_up(cfg_max_allowed_rtt) {
                        StateVariant::Up
                    } else {
                        StateVariant::Down
                    }
                }
                Err(e) => {
                    telio_log_error!("poisoned lock - {}", e);
                    StateVariant::Down
                }
            },
        }
    }

    fn update(&mut self, cfg_max_allowed_rtt: Duration, ping_enabled: bool) -> StateUpdateResult {
        let is_link_up = match self.stats.lock() {
            Ok(s) => s.is_link_up(cfg_max_allowed_rtt),
            Err(e) => {
                telio_log_error!("poisoned lock - {}", e);
                false
            }
        };

        match &mut self.variant {
            StateVariant::Down => {
                if is_link_up {
                    // Transition to Up
                    // Report link state Up
                    self.variant = StateVariant::Up;
                    Self::build_result(StateDecision::Notify, LinkState::Up)
                } else {
                    // Stay in Down
                    // No notify
                    Self::build_result(StateDecision::NoAction, LinkState::Down)
                }
            }

            StateVariant::PossibleDown { deadline } => {
                if is_link_up {
                    // Transition to Up
                    // No notify
                    self.variant = StateVariant::Up;
                    Self::build_result(StateDecision::NoAction, LinkState::Up)
                } else if Instant::now() >= *deadline {
                    // Transition to Down
                    // Report link state Down
                    self.variant = StateVariant::Down;
                    Self::build_result(StateDecision::Notify, LinkState::Down)
                } else {
                    Self::build_result(StateDecision::NoAction, LinkState::Up)
                }
            }

            StateVariant::Up => {
                if !is_link_up {
                    // Transition to PossibleDown
                    // No notify for now

                    let delay = if ping_enabled {
                        // If enhanced detection is enabled we will issue an ICMP echo request
                        // The maximum waiting time for a response can be WG_KEEPALIVE + rtt
                        // We will receive either an ICMP response or a Passive Keepalive message
                        WG_KEEPALIVE
                            .checked_add(cfg_max_allowed_rtt)
                            .unwrap_or(WG_KEEPALIVE)
                    } else {
                        Self::POSSIBLE_DOWN_DELAY
                    };

                    self.variant = StateVariant::PossibleDown {
                        deadline: Instant::now()
                            .checked_add(delay)
                            .unwrap_or_else(Instant::now),
                    };
                    Self::build_result(StateDecision::Ping, LinkState::Up)
                } else {
                    // Current link_state is Up
                    Self::build_result(StateDecision::NoAction, LinkState::Up)
                }
            }
        }
    }

    fn current_link_state(&self) -> LinkState {
        match self.variant {
            StateVariant::Up | StateVariant::PossibleDown { .. } => LinkState::Up,
            StateVariant::Down => LinkState::Down,
        }
    }

    fn build_result(state_decision: StateDecision, link_state: LinkState) -> StateUpdateResult {
        StateUpdateResult {
            should_ping: state_decision.should_ping(),
            link_detection_update_result: LinkDetectionUpdateResult {
                should_notify: state_decision.should_notify(),
                link_state: Some(link_state),
            },
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::link_detection;
    use telio_model::{features::FeatureLinkDetection, mesh::Node};
    use tokio::time;

    const ONE_SECOND: Duration = Duration::from_secs(1);

    #[test]
    fn test_state_build() {
        let stats_down = Arc::new(Mutex::new(BytesAndTimestamps::new(None, None)));
        let stats_up = Arc::new(Mutex::new(BytesAndTimestamps::new(Some(100), Some(100))));
        let rtt = Duration::from_secs(0);

        let down = State::new(stats_down, rtt);
        let up = State::new(stats_up, rtt);

        assert!(matches!(up.variant, StateVariant::Up));
        assert!(matches!(down.variant, StateVariant::Down));
    }

    #[tokio::test(start_paused = true)]
    async fn test_state_transitions_from_down() {
        let mut state = State {
            stats: Arc::new(Mutex::new(BytesAndTimestamps::new(None, None))),
            variant: StateVariant::Down,
        };
        assert!(matches!(state.variant, StateVariant::Down));

        time::advance(ONE_SECOND).await;

        state.stats.lock().unwrap().update(0, 1);
        assert!(matches!(state.variant, StateVariant::Down));

        time::advance(ONE_SECOND).await;

        state.stats.lock().unwrap().update(0, 1);
        state.update(Duration::from_secs(0), false);
        assert!(matches!(state.variant, StateVariant::Down));

        assert!(matches!(state.variant, StateVariant::Down));

        time::advance(ONE_SECOND).await;

        state.stats.lock().unwrap().update(1, 1);
        assert!(matches!(state.variant, StateVariant::Down));

        state.update(Duration::from_secs(0), false);
        assert!(matches!(state.variant, StateVariant::Up));
    }

    #[tokio::test(start_paused = true)]
    async fn test_state_transition_from_up() {
        let mut state = State {
            stats: Arc::new(Mutex::new(BytesAndTimestamps::new(Some(0), Some(0)))),
            variant: StateVariant::Up,
        };

        time::advance(ONE_SECOND).await;

        state.stats.lock().unwrap().update(0, 1);
        assert!(matches!(state.variant, StateVariant::Up));

        state.stats.lock().unwrap().update(0, 2);
        state.update(Duration::from_secs(0), false);
        assert!(matches!(state.variant, StateVariant::PossibleDown { .. }));
    }

    #[tokio::test(start_paused = true)]
    async fn test_state_transition_from_possible_down_to_down() {
        let mut state = State {
            stats: Arc::new(Mutex::new(BytesAndTimestamps::new(None, None))),
            variant: StateVariant::PossibleDown {
                deadline: Instant::now().checked_add(Duration::from_secs(3)).unwrap(),
            },
        };
        assert!(matches!(state.variant, StateVariant::PossibleDown { .. }));

        time::advance(Duration::from_secs(3)).await;
        state.update(Duration::from_secs(0), false);
        assert!(matches!(state.variant, StateVariant::Down));
    }

    #[tokio::test(start_paused = true)]
    async fn test_state_transition_from_possible_down_to_up() {
        let mut state = State {
            stats: Arc::new(Mutex::new(BytesAndTimestamps::new(None, None))),
            variant: StateVariant::PossibleDown {
                deadline: Instant::now().checked_add(Duration::from_secs(3)).unwrap(),
            },
        };
        assert!(matches!(state.variant, StateVariant::PossibleDown { .. }));

        state.stats.lock().unwrap().update(1, 1);
        state.update(ONE_SECOND, false);
        assert!(matches!(state.variant, StateVariant::Up));
    }

    #[test]
    fn test_state_current_link_state() {
        let down = State {
            stats: Arc::new(Mutex::new(BytesAndTimestamps::new(None, None))),
            variant: StateVariant::Down,
        };
        let possible_down = State {
            stats: Arc::new(Mutex::new(BytesAndTimestamps::new(None, None))),
            variant: StateVariant::PossibleDown {
                deadline: Instant::now(),
            },
        };
        let up = State {
            stats: Arc::new(Mutex::new(BytesAndTimestamps::new(None, None))),
            variant: StateVariant::Up,
        };

        assert_eq!(down.current_link_state(), LinkState::Down);
        assert_eq!(possible_down.current_link_state(), LinkState::Up);
        assert_eq!(up.current_link_state(), LinkState::Up);
    }
}
