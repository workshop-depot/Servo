using System.Runtime.CompilerServices;
using System.ServiceProcess;
using NLog;
using ServoCS.Servo;

namespace ServoCS
{
    class ServoCSService : IService
    {
        #region IService Members
        public void OnContinue()
        {
            Trace();
        }

        public void OnCustomCommand(int command)
        {
            Trace();
        }

        public void OnPause()
        {
            Trace();
        }

        public bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            Trace();
            return true;
        }

        public void OnSessionChange(SessionChangeDescription changeDescription)
        {
            Trace();
        }

        public void OnShutdown()
        {
            Trace();
        }

        public void OnStart(string[] args)
        {
            ClassLogger.Info("Service Started - ServoCS");
        }

        public void OnStop()
        {
            ClassLogger.Info("Service Stopped - ServoCS");
        }
        #endregion

        #region Utilities
        static readonly Logger ClassLogger = LogManager.GetCurrentClassLogger();
        #endregion

        public void Dispose()
        {
            Trace();
        }

        static void Trace([CallerMemberName]string name = null) { ClassLogger.Info(name); }
    }
}
