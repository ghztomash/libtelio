//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.2
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace NordSec.Telio {

public enum TelioAdapterType {
  AdapterBoringTun,
  AdapterLinuxNativeTun,
  AdapterWireguardGoTun,
  AdapterWindowsNativeTun
}

}
namespace NordSec.Telio {

public enum TelioLogLevel {
  LogCritical = 1,
  LogError = 2,
  LogWarning = 3,
  LogInfo = 4,
  LogDebug = 5,
  LogTrace = 6
}

}
namespace NordSec.Telio {

public enum TelioResult {
  ResOk = 0,
  ResError = 1,
  ResInvalidKey = 2,
  ResBadConfig = 3,
  ResLockError = 4,
  ResInvalidString = 5,
  ResAlreadyStarted = 6
}

}
namespace NordSec.Telio {

public class Telio : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal Telio(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(Telio obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~Telio() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          libtelioPINVOKE.delete_Telio(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public delegate void EventDelegate(string message);
  public delegate void LoggerDelegate(TelioLogLevel level, string message);

  public Telio(string features, EventDelegate events, TelioLogLevel level, LoggerDelegate logger) : this(libtelioPINVOKE.new_Telio(features, events, (int)level, logger), true) {
    if (libtelioPINVOKE.SWIGPendingException.Pending) throw libtelioPINVOKE.SWIGPendingException.Retrieve();
  }

  public static TelioAdapterType GetDefaultAdapter() {
    TelioAdapterType ret = (TelioAdapterType)libtelioPINVOKE.Telio_GetDefaultAdapter();
    return ret;
  }

  public TelioResult Start(string privateKey, TelioAdapterType adapter) {
    TelioResult ret = (TelioResult)libtelioPINVOKE.Telio_Start(swigCPtr, privateKey, (int)adapter);
    return ret;
  }

  public TelioResult StartNamed(string privateKey, TelioAdapterType adapter, string name) {
    TelioResult ret = (TelioResult)libtelioPINVOKE.Telio_StartNamed(swigCPtr, privateKey, (int)adapter, name);
    return ret;
  }

  public TelioResult EnableMagicDns(string forwardServers) {
    TelioResult ret = (TelioResult)libtelioPINVOKE.Telio_EnableMagicDns(swigCPtr, forwardServers);
    return ret;
  }

  public TelioResult DisableMagicDns() {
    TelioResult ret = (TelioResult)libtelioPINVOKE.Telio_DisableMagicDns(swigCPtr);
    return ret;
  }

  public TelioResult Stop() {
    TelioResult ret = (TelioResult)libtelioPINVOKE.Telio_Stop(swigCPtr);
    return ret;
  }

  public ulong GetAdapterLuid() {
    ulong ret = libtelioPINVOKE.Telio_GetAdapterLuid(swigCPtr);
    return ret;
  }

  public TelioResult SetPrivateKey(string privateKey) {
    TelioResult ret = (TelioResult)libtelioPINVOKE.Telio_SetPrivateKey(swigCPtr, privateKey);
    return ret;
  }

  public string GetPrivateKey() {
    string ret = libtelioPINVOKE.Telio_GetPrivateKey(swigCPtr);
    return ret;
  }

  public TelioResult NotifyNetworkChange(string notifyInfo) {
    TelioResult ret = (TelioResult)libtelioPINVOKE.Telio_NotifyNetworkChange(swigCPtr, notifyInfo);
    return ret;
  }

  public TelioResult ConnectToExitNode(string publicKey, string allowedIps, string endpoint) {
    TelioResult ret = (TelioResult)libtelioPINVOKE.Telio_ConnectToExitNode(swigCPtr, publicKey, allowedIps, endpoint);
    return ret;
  }

  public TelioResult DisconnectFromExitNode(string publicKey) {
    TelioResult ret = (TelioResult)libtelioPINVOKE.Telio_DisconnectFromExitNode(swigCPtr, publicKey);
    return ret;
  }

  public TelioResult DisconnectFromExitNodes() {
    TelioResult ret = (TelioResult)libtelioPINVOKE.Telio_DisconnectFromExitNodes(swigCPtr);
    return ret;
  }

  public TelioResult SetMeshnet(string cfg) {
    TelioResult ret = (TelioResult)libtelioPINVOKE.Telio_SetMeshnet(swigCPtr, cfg);
    return ret;
  }

  public TelioResult SetMeshnetOff() {
    TelioResult ret = (TelioResult)libtelioPINVOKE.Telio_SetMeshnetOff(swigCPtr);
    return ret;
  }

  public string GenerateSecretKey() {
    string ret = libtelioPINVOKE.Telio_GenerateSecretKey(swigCPtr);
    return ret;
  }

  public string GeneratePublicKey(string secretKey) {
    string ret = libtelioPINVOKE.Telio_GeneratePublicKey(swigCPtr, secretKey);
    return ret;
  }

  public string GetStatusMap() {
    string ret = libtelioPINVOKE.Telio_GetStatusMap(swigCPtr);
    return ret;
  }

  public string GetLastError() {
    string ret = libtelioPINVOKE.Telio_GetLastError(swigCPtr);
    return ret;
  }

  public static string GetVersionTag() {
    string ret = libtelioPINVOKE.Telio_GetVersionTag();
    return ret;
  }

  public static string GetCommitSha() {
    string ret = libtelioPINVOKE.Telio_GetCommitSha();
    return ret;
  }

}

}
namespace NordSec.Telio {

class libtelioPINVOKE {

  protected class SWIGExceptionHelper {

    public delegate void ExceptionDelegate(string message);
    public delegate void ExceptionArgumentDelegate(string message, string paramName);

    static ExceptionDelegate applicationDelegate = new ExceptionDelegate(SetPendingApplicationException);
    static ExceptionDelegate arithmeticDelegate = new ExceptionDelegate(SetPendingArithmeticException);
    static ExceptionDelegate divideByZeroDelegate = new ExceptionDelegate(SetPendingDivideByZeroException);
    static ExceptionDelegate indexOutOfRangeDelegate = new ExceptionDelegate(SetPendingIndexOutOfRangeException);
    static ExceptionDelegate invalidCastDelegate = new ExceptionDelegate(SetPendingInvalidCastException);
    static ExceptionDelegate invalidOperationDelegate = new ExceptionDelegate(SetPendingInvalidOperationException);
    static ExceptionDelegate ioDelegate = new ExceptionDelegate(SetPendingIOException);
    static ExceptionDelegate nullReferenceDelegate = new ExceptionDelegate(SetPendingNullReferenceException);
    static ExceptionDelegate outOfMemoryDelegate = new ExceptionDelegate(SetPendingOutOfMemoryException);
    static ExceptionDelegate overflowDelegate = new ExceptionDelegate(SetPendingOverflowException);
    static ExceptionDelegate systemDelegate = new ExceptionDelegate(SetPendingSystemException);

    static ExceptionArgumentDelegate argumentDelegate = new ExceptionArgumentDelegate(SetPendingArgumentException);
    static ExceptionArgumentDelegate argumentNullDelegate = new ExceptionArgumentDelegate(SetPendingArgumentNullException);
    static ExceptionArgumentDelegate argumentOutOfRangeDelegate = new ExceptionArgumentDelegate(SetPendingArgumentOutOfRangeException);

    [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="SWIGRegisterExceptionCallbacks_libtelio")]
    public static extern void SWIGRegisterExceptionCallbacks_libtelio(
                                ExceptionDelegate applicationDelegate,
                                ExceptionDelegate arithmeticDelegate,
                                ExceptionDelegate divideByZeroDelegate, 
                                ExceptionDelegate indexOutOfRangeDelegate, 
                                ExceptionDelegate invalidCastDelegate,
                                ExceptionDelegate invalidOperationDelegate,
                                ExceptionDelegate ioDelegate,
                                ExceptionDelegate nullReferenceDelegate,
                                ExceptionDelegate outOfMemoryDelegate, 
                                ExceptionDelegate overflowDelegate, 
                                ExceptionDelegate systemExceptionDelegate);

    [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="SWIGRegisterExceptionArgumentCallbacks_libtelio")]
    public static extern void SWIGRegisterExceptionCallbacksArgument_libtelio(
                                ExceptionArgumentDelegate argumentDelegate,
                                ExceptionArgumentDelegate argumentNullDelegate,
                                ExceptionArgumentDelegate argumentOutOfRangeDelegate);

    static void SetPendingApplicationException(string message) {
      SWIGPendingException.Set(new global::System.ApplicationException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingArithmeticException(string message) {
      SWIGPendingException.Set(new global::System.ArithmeticException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingDivideByZeroException(string message) {
      SWIGPendingException.Set(new global::System.DivideByZeroException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingIndexOutOfRangeException(string message) {
      SWIGPendingException.Set(new global::System.IndexOutOfRangeException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingInvalidCastException(string message) {
      SWIGPendingException.Set(new global::System.InvalidCastException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingInvalidOperationException(string message) {
      SWIGPendingException.Set(new global::System.InvalidOperationException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingIOException(string message) {
      SWIGPendingException.Set(new global::System.IO.IOException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingNullReferenceException(string message) {
      SWIGPendingException.Set(new global::System.NullReferenceException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingOutOfMemoryException(string message) {
      SWIGPendingException.Set(new global::System.OutOfMemoryException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingOverflowException(string message) {
      SWIGPendingException.Set(new global::System.OverflowException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingSystemException(string message) {
      SWIGPendingException.Set(new global::System.SystemException(message, SWIGPendingException.Retrieve()));
    }

    static void SetPendingArgumentException(string message, string paramName) {
      SWIGPendingException.Set(new global::System.ArgumentException(message, paramName, SWIGPendingException.Retrieve()));
    }
    static void SetPendingArgumentNullException(string message, string paramName) {
      global::System.Exception e = SWIGPendingException.Retrieve();
      if (e != null) message = message + " Inner Exception: " + e.Message;
      SWIGPendingException.Set(new global::System.ArgumentNullException(paramName, message));
    }
    static void SetPendingArgumentOutOfRangeException(string message, string paramName) {
      global::System.Exception e = SWIGPendingException.Retrieve();
      if (e != null) message = message + " Inner Exception: " + e.Message;
      SWIGPendingException.Set(new global::System.ArgumentOutOfRangeException(paramName, message));
    }

    static SWIGExceptionHelper() {
      SWIGRegisterExceptionCallbacks_libtelio(
                                applicationDelegate,
                                arithmeticDelegate,
                                divideByZeroDelegate,
                                indexOutOfRangeDelegate,
                                invalidCastDelegate,
                                invalidOperationDelegate,
                                ioDelegate,
                                nullReferenceDelegate,
                                outOfMemoryDelegate,
                                overflowDelegate,
                                systemDelegate);

      SWIGRegisterExceptionCallbacksArgument_libtelio(
                                argumentDelegate,
                                argumentNullDelegate,
                                argumentOutOfRangeDelegate);
    }
  }

  protected static SWIGExceptionHelper swigExceptionHelper = new SWIGExceptionHelper();

  public class SWIGPendingException {
    [global::System.ThreadStatic]
    private static global::System.Exception pendingException = null;
    private static int numExceptionsPending = 0;
    private static global::System.Object exceptionsLock = null;

    public static bool Pending {
      get {
        bool pending = false;
        if (numExceptionsPending > 0)
          if (pendingException != null)
            pending = true;
        return pending;
      } 
    }

    public static void Set(global::System.Exception e) {
      if (pendingException != null)
        throw new global::System.ApplicationException("FATAL: An earlier pending exception from unmanaged code was missed and thus not thrown (" + pendingException.ToString() + ")", e);
      pendingException = e;
      lock(exceptionsLock) {
        numExceptionsPending++;
      }
    }

    public static global::System.Exception Retrieve() {
      global::System.Exception e = null;
      if (numExceptionsPending > 0) {
        if (pendingException != null) {
          e = pendingException;
          pendingException = null;
          lock(exceptionsLock) {
            numExceptionsPending--;
          }
        }
      }
      return e;
    }

    static SWIGPendingException() {
      exceptionsLock = new global::System.Object();
    }
  }


  protected class SWIGStringHelper {

    public delegate string SWIGStringDelegate(string message);
    static SWIGStringDelegate stringDelegate = new SWIGStringDelegate(CreateString);

    [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="SWIGRegisterStringCallback_libtelio")]
    public static extern void SWIGRegisterStringCallback_libtelio(SWIGStringDelegate stringDelegate);

    static string CreateString(string cString) {
      return cString;
    }

    static SWIGStringHelper() {
      SWIGRegisterStringCallback_libtelio(stringDelegate);
    }
  }

  static protected SWIGStringHelper swigStringHelper = new SWIGStringHelper();


  static libtelioPINVOKE() {
  }


  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_new_Telio___")]
  public static extern global::System.IntPtr new_Telio(string jarg1, Telio.EventDelegate jarg2, int jarg3, Telio.LoggerDelegate jarg4);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_GetDefaultAdapter___")]
  public static extern int Telio_GetDefaultAdapter();

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_delete_Telio___")]
  public static extern void delete_Telio(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_Start___")]
  public static extern int Telio_Start(global::System.Runtime.InteropServices.HandleRef jarg1, string jarg2, int jarg3);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_StartNamed___")]
  public static extern int Telio_StartNamed(global::System.Runtime.InteropServices.HandleRef jarg1, string jarg2, int jarg3, string jarg4);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_EnableMagicDns___")]
  public static extern int Telio_EnableMagicDns(global::System.Runtime.InteropServices.HandleRef jarg1, string jarg2);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_DisableMagicDns___")]
  public static extern int Telio_DisableMagicDns(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_Stop___")]
  public static extern int Telio_Stop(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_GetAdapterLuid___")]
  public static extern ulong Telio_GetAdapterLuid(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_SetPrivateKey___")]
  public static extern int Telio_SetPrivateKey(global::System.Runtime.InteropServices.HandleRef jarg1, string jarg2);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_GetPrivateKey___")]
  public static extern string Telio_GetPrivateKey(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_NotifyNetworkChange___")]
  public static extern int Telio_NotifyNetworkChange(global::System.Runtime.InteropServices.HandleRef jarg1, string jarg2);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_ConnectToExitNode___")]
  public static extern int Telio_ConnectToExitNode(global::System.Runtime.InteropServices.HandleRef jarg1, string jarg2, string jarg3, string jarg4);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_DisconnectFromExitNode___")]
  public static extern int Telio_DisconnectFromExitNode(global::System.Runtime.InteropServices.HandleRef jarg1, string jarg2);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_DisconnectFromExitNodes___")]
  public static extern int Telio_DisconnectFromExitNodes(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_SetMeshnet___")]
  public static extern int Telio_SetMeshnet(global::System.Runtime.InteropServices.HandleRef jarg1, string jarg2);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_SetMeshnetOff___")]
  public static extern int Telio_SetMeshnetOff(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_GenerateSecretKey___")]
  public static extern string Telio_GenerateSecretKey(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_GeneratePublicKey___")]
  public static extern string Telio_GeneratePublicKey(global::System.Runtime.InteropServices.HandleRef jarg1, string jarg2);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_GetStatusMap___")]
  public static extern string Telio_GetStatusMap(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_GetLastError___")]
  public static extern string Telio_GetLastError(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_GetVersionTag___")]
  public static extern string Telio_GetVersionTag();

  [global::System.Runtime.InteropServices.DllImport("telio", EntryPoint="CSharp_NordSecfTelio_Telio_GetCommitSha___")]
  public static extern string Telio_GetCommitSha();
}

}
namespace NordSec.Telio {

public class libtelio {
}

}
