using System;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

namespace rdpWrapper {

  internal class ServiceHelper {

    private const int SC_MANAGER_CONNECT = 0x0001;
    private const int SERVICE_QUERY_STATUS = 0x0004;
    private const int SC_STATUS_PROCESS_INFO = 0;
    private const int SC_MANAGER_ALL_ACCESS = 0xF003F;
    private const int SERVICE_STOP = 0x0020;
    private const int SERVICE_START = 0x0010;
    private const int SERVICE_ALL_ACCESS = 0xF01FF;

    internal const short SERVICE_CONTINUE_PENDING = 0x00000005; //The service continue is pending.
    internal const short SERVICE_PAUSE_PENDING = 0x00000006; //The service pause is pending.
    internal const short SERVICE_PAUSED = 0x00000007; //The service is paused.
    internal const short SERVICE_RUNNING = 0x00000004; //The service is running.
    internal const short SERVICE_START_PENDING = 0x00000002; //The service is starting.
    internal const short SERVICE_STOP_PENDING = 0x00000003; //The service is stopping.
    internal const short SERVICE_STOPPED = 0x00000001; //The service is not running.

    [StructLayout(LayoutKind.Sequential)]
    internal struct SERVICE_STATUS_PROCESS {
      public int dwServiceType;
      public int dwCurrentState;
      public int dwControlsAccepted;
      public int dwWin32ExitCode;
      public int dwServiceSpecificExitCode;
      public int dwCheckPoint;
      public int dwWaitHint;
      public int dwProcessId;
      public int dwServiceFlags;
    }

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern IntPtr OpenSCManager(string lpMachineName, string lpDatabaseName, int dwDesiredAccess);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, int dwDesiredAccess);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool CloseServiceHandle(IntPtr hSCObject);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool ControlService(IntPtr hService, int dwControl, out SERVICE_STATUS_PROCESS lpServiceStatus);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool QueryServiceStatusEx(IntPtr hService, int InfoLevel, IntPtr lpBuffer, int cbBufSize, out int pcbBytesNeeded);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool StartService(IntPtr hService, int dwNumServiceArgs, string[] lpServiceArgVectors);

    private static bool WaitForServiceStatus(IntPtr service, int desiredStatus, TimeSpan timeout) {

      var start = DateTime.Now;
      var buf = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SERVICE_STATUS_PROCESS)));
      try {
        while ((DateTime.Now - start) < timeout) {
          if (!QueryServiceStatusEx(service, SC_STATUS_PROCESS_INFO, buf, Marshal.SizeOf(typeof(SERVICE_STATUS_PROCESS)), out int bytesNeeded))
            return false;

          var ssp = Marshal.PtrToStructure<SERVICE_STATUS_PROCESS>(buf);
          if (ssp.dwCurrentState == desiredStatus)
            return true;

          Thread.Sleep(500);
        }
      }
      finally {
        Marshal.FreeHGlobal(buf);
      }

      return false;
    }

    public static bool RestartServiceNative(string svcName = "TermService") {
      IntPtr scm = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
      if (scm == IntPtr.Zero)
        return false;

      IntPtr service = OpenService(scm, svcName, SERVICE_ALL_ACCESS);
      if (service == IntPtr.Zero) {
        CloseServiceHandle(scm);
        return false;
      }

      try {
        // Stop the service
        ControlService(service, 1 /* SERVICE_CONTROL_STOP */, out SERVICE_STATUS_PROCESS status);

        // Wait until it's stopped
        if (!WaitForServiceStatus(service, SERVICE_STOPPED, TimeSpan.FromSeconds(10)))
          return false;

        // Start the service again
        if (!StartService(service, 0, null))
          return false;

        return WaitForServiceStatus(service, SERVICE_RUNNING, TimeSpan.FromSeconds(10));
      }
      finally {
        CloseServiceHandle(service);
        CloseServiceHandle(scm);
      }
    }

    internal static void RestartService(string serviceName, int timeoutMilliseconds) {
      ServiceController service = new ServiceController(serviceName);
      int ticks = Environment.TickCount;
      var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
      if (service.Status != ServiceControllerStatus.Stopped) {
        service.Stop();
        service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
      }
      timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (Environment.TickCount - ticks));
      service.Start();
      service.WaitForStatus(ServiceControllerStatus.Running, timeout);
    }

    internal static short GetServiceState(string svcName = "TermService") {
      var hSC = OpenSCManager(null, null, SC_MANAGER_CONNECT);
      if (hSC == IntPtr.Zero)
        return -1;

      var hSvc = OpenService(hSC, svcName, SERVICE_QUERY_STATUS);
      if (hSvc == IntPtr.Zero) {
        CloseServiceHandle(hSC);
        return -1;
      }

      QueryServiceStatusEx(hSvc, SC_STATUS_PROCESS_INFO, IntPtr.Zero, 0, out int bytesNeeded);

      var buffer = Marshal.AllocHGlobal(bytesNeeded);
      try {
        if (!QueryServiceStatusEx(hSvc, SC_STATUS_PROCESS_INFO, buffer, bytesNeeded, out _))
          return -1;
        var ssp = Marshal.PtrToStructure<SERVICE_STATUS_PROCESS>(buffer);
        return (short)ssp.dwCurrentState;
      }
      finally {
        Marshal.FreeHGlobal(buffer);
        CloseServiceHandle(hSvc);
        CloseServiceHandle(hSC);
      }
    }

  }
}
