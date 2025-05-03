using System;
using System.ServiceProcess;

namespace sergiye.Common {

  internal class ServiceHelper {

    private readonly Logger logger;

    internal ServiceHelper(Logger logger) {
      this.logger = logger;
    }

    internal void StopService(string serviceName, TimeSpan timeout) {
      using ServiceController service = new(serviceName);
      if (service.Status == ServiceControllerStatus.Stopped) return;
      logger.Log($"Terminating {serviceName}...");
      service.Stop();
      service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
      logger.Log(" Done", Logger.StateKind.Info, false);
    }

    internal void StartService(string serviceName, TimeSpan timeout) {
      using ServiceController service = new(serviceName);
      if (service.Status == ServiceControllerStatus.Running) return;
      logger.Log($"Starting {serviceName}...");
      service.Start();
      service.WaitForStatus(ServiceControllerStatus.Running, timeout);
      logger.Log(" Done", Logger.StateKind.Info, false);
    }

    internal ServiceControllerStatus? GetServiceState(string serviceName) {
      try {
        using var service = new ServiceController(serviceName);
        return service.Status;
      }
      catch (Exception ex) {
        logger.Log($"Error getting {serviceName} state: {ex.Message}", Logger.StateKind.Error);
        return null;
      }
    }
  }
}
