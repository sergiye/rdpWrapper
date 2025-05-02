using System;
using System.ServiceProcess;

namespace rdpWrapper {

  internal static class ServiceHelper {

    internal static void StopService(string serviceName, TimeSpan timeout) {
      using ServiceController service = new(serviceName);
      if (service.Status == ServiceControllerStatus.Stopped) return;
      service.Stop();
      service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
    }

    internal static void StartService(string serviceName, TimeSpan timeout) {
      using ServiceController service = new(serviceName);
      if (service.Status == ServiceControllerStatus.Running) return;
      service.Start();
      service.WaitForStatus(ServiceControllerStatus.Running, timeout);
    }

    internal static void RestartService(string serviceName, int timeoutMilliseconds) {
      using var service = new ServiceController(serviceName);
      var ticks = Environment.TickCount;
      var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
      if (service.Status != ServiceControllerStatus.Stopped) {
        service.Stop();
        service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
      }
      timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (Environment.TickCount - ticks));
      service.Start();
      service.WaitForStatus(ServiceControllerStatus.Running, timeout);
    }

    internal static ServiceControllerStatus? GetServiceState(string serviceName = "TermService") {
      try {
        using var service = new ServiceController(serviceName);
        return service.Status;
      }
      catch (Exception ex) {
        Console.WriteLine(ex.Message);
        return null;
      }
    }
  }
}
