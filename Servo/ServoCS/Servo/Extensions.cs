using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServoCS.Servo
{
    static class Extensions
    {
        public static void Run(this IService svc)
        {
            var flags = Environment.GetCommandLineArgs();
            var f = flags.Skip(1).FirstOrDefault() ?? string.Empty;

            if (f == "-h" || f == "-help")
            {
                ShowHelp(Config.RunInService);

                return;
            }

            if (Config.RunInService)
            {
                switch (f)
                {
                    case "-i":
                    case "-install":
                        SvcInstaller.Install(false, flags);
                        break;
                    case "-u":
                    case "-uninstall":
                        SvcInstaller.Install(true, flags);
                        break;
                    case "-start":
                        new SvcController().Start();
                        break;
                    case "-stop":
                        new SvcController().Stop();
                        break;
                    case "-a":
                    case "-app":
                        new InApp(svc).Run();
                        break;
                    default:
                        new InService(svc).Run();
                        break;
                }
            }
            else
            {
                new InApp(svc).Run();
            }
        }

        static void ShowHelp(bool runInService)
        {
            var h = new StringBuilder();
            if (!runInService) h = h.AppendLine("<< IS NOT RUNNING IN SERVICE MODE >>\r\nChange value of Servo/Conf/RunInService in App.config to true.\r\n");
            h = h
                .AppendLine("-h or -help\tshows this help")
                .AppendLine("-i or -install\tinstalls this windows service")
                .AppendLine("-u or -uninstall\tuninstalls this windows service")
                .AppendLine("-start\t\tstarts this windows service")
                .AppendLine("-stop\t\tstops this windows service")
                .AppendLine("-a or -app\t\truns this windows service as an app");

            MessageBox.Show(h.ToString(), "ServoCS");
        }
    }
}
