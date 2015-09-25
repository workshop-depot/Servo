using System;
using System.ServiceProcess;
using NLog;

namespace ServoCS.Servo
{
    class SvcController
    {
        readonly ServiceController _c = new ServiceController(Config.ServiceName);

        public void Start()
        {
            try
            {
                ClassLogger.Info("Starting Service...");
                var timeout = Config.StartTimeout;
                _c.Start();

                const ServiceControllerStatus targetStatus = ServiceControllerStatus.Running;
                _c.WaitForStatus(targetStatus, TimeSpan.FromSeconds(timeout));
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error in starting service: {0}", ex);
                ClassLogger.Error(msg);
            }
        }

        public void Stop()
        {
            try
            {
                ClassLogger.Info("Stopping Service...");
                var timeout = Config.StopTimeout;
                _c.Stop();

                const ServiceControllerStatus targetStatus = ServiceControllerStatus.Stopped;
                _c.WaitForStatus(targetStatus, TimeSpan.FromSeconds(timeout));
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error in stopping service: {0}", ex);
                ClassLogger.Error(msg);
            }
        }

        static readonly Logger ClassLogger = LogManager.GetCurrentClassLogger();
    }
}