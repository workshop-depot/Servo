using System;
using System.ServiceProcess;

namespace ServoCS.Servo
{
    class InService : ServiceBase
    {
        readonly IService _scaffold;
        public InService(IService scaffold)
        {
            _scaffold = scaffold;

            ServiceName = Config.ServiceName;

            CanHandlePowerEvent = Config.CanHandlePowerEvent;
            CanHandleSessionChangeEvent = Config.CanHandleSessionChangeEvent;
            CanPauseAndContinue = Config.CanPauseAndContinue;
            CanShutdown = Config.CanShutdown;
            CanStop = Config.CanStop;
            AutoLog = Config.AutoLog;
        }

        protected override void OnContinue()
        {
            try { _scaffold.OnContinue(); }
            catch (NotImplementedException) { base.OnContinue(); }
        }

        protected override void OnCustomCommand(int command)
        {
            try { _scaffold.OnCustomCommand(command); }
            catch (NotImplementedException) { base.OnCustomCommand(command); }
        }

        protected override void OnPause()
        {
            try { _scaffold.OnPause(); }
            catch (NotImplementedException) { base.OnPause(); }
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            try { return _scaffold.OnPowerEvent(powerStatus); }
            catch (NotImplementedException) { return base.OnPowerEvent(powerStatus); }
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            try { _scaffold.OnSessionChange(changeDescription); }
            catch (NotImplementedException) { base.OnSessionChange(changeDescription); }
        }

        protected override void OnShutdown()
        {
            try { _scaffold.OnShutdown(); }
            catch (NotImplementedException) { base.OnShutdown(); }
        }

        protected override void OnStart(string[] args)
        {
            try { _scaffold.OnStart(args); }
            catch (NotImplementedException) { base.OnStart(args); }
        }

        protected override void OnStop()
        {
            try { _scaffold.OnStop(); }
            catch (NotImplementedException) { base.OnStop(); }
        }

        public void Run()
        {
            Run(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try { _scaffold.Dispose(); }
                catch (NotImplementedException) { }
            }

            base.Dispose(disposing);

            try
            {
                NLog.LogManager.Flush();
            }
            catch
            {
                //TODO:
            }
        }
    }
}