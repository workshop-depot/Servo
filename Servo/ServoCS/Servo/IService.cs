using System;
using System.ServiceProcess;

namespace ServoCS.Servo
{
    interface IService : IDisposable
    {
        void OnContinue();

        void OnCustomCommand(int command);

        void OnPause();

        bool OnPowerEvent(PowerBroadcastStatus powerStatus);

        void OnSessionChange(SessionChangeDescription changeDescription);

        void OnShutdown();

        void OnStart(string[] args);

        void OnStop();
    }
}