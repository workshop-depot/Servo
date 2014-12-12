using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servo;
using NLog;

namespace ServeCS
{
    class Program
    {
        static void Main(string[] args)
        {
            new SampleService().Run();
        }
    }

    class SampleService : IService
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
            ClassLogger.Info("Service Started");
        }

        public void OnStop()
        {
            ClassLogger.Info("Service Stopped");
        }
        #endregion

        #region Utilities
        static readonly Logger ClassLogger = LogManager.GetCurrentClassLogger();
        #endregion
    }
}
