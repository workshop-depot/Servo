using NLog;
using Servo;
using System;

namespace XService
{
    class XServiceService : IService
    {
        #region IService Members
        public void OnContinue()
        {
            throw new NotImplementedException();
        }

        public void OnCustomCommand(int command)
        {
            throw new NotImplementedException();
        }

        public void OnPause()
        {
            throw new NotImplementedException();
        }

        public bool OnPowerEvent(System.ServiceProcess.PowerBroadcastStatus powerStatus)
        {
            throw new NotImplementedException();
        }

        public void OnSessionChange(System.ServiceProcess.SessionChangeDescription changeDescription)
        {
            throw new NotImplementedException();
        }

        public void OnShutdown()
        {
            throw new NotImplementedException();
        }

        public void OnStart(string[] args)
        {
            ClassLogger.Info("Service Started - XService");
        }

        public void OnStop()
        {
            ClassLogger.Info("Service Stopped - XService");
        }
        #endregion

        #region Utilities
        static readonly Logger ClassLogger = LogManager.GetCurrentClassLogger();
        #endregion
    }
}
