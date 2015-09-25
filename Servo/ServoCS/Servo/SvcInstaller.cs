using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using NLog;

namespace ServoCS.Servo
{
    [RunInstaller(true)]
    public class SvcInstaller : Installer
    {
        public SvcInstaller()
        {
            var processInstaller = new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem };

            var serviceInstaller = new ServiceInstaller
            {
                ServiceName = Config.ServiceName,
                DisplayName = Config.DisplayName,
                StartType = Config.ServiceStartMode,
                Description = Config.Description
            };

            try
            {
                serviceInstaller.ServicesDependedOn = Config.ServicesDependedOn;
            }
            catch (Exception ex) { ClassLogger.Error(ex); }

            try
            {
                serviceInstaller.DelayedAutoStart = Config.DelayedAutoStart;
            }
            catch (Exception ex) { ClassLogger.Error(ex); }

            Installers.AddRange(new Installer[] { serviceInstaller, processInstaller });
        }

        internal static void Install(bool undo, string[] args)
        {
            try
            {
                Console.WriteLine(undo ? "uninstalling" : "installing");

                if (undo)
                    ManagedInstallerClass.InstallHelper(new string[] { "/u", System.Reflection.Assembly.GetAssembly(typeof(SvcInstaller)).Location });
                else
                    ManagedInstallerClass.InstallHelper(new string[] { System.Reflection.Assembly.GetAssembly(typeof(SvcInstaller)).Location });


            }
            catch (Exception ex) { ClassLogger.Error(ex); }
        }

        static readonly Logger SlotClassLogger = LogManager.GetCurrentClassLogger();
        static Logger ClassLogger { get { return SlotClassLogger; } }
    }
}