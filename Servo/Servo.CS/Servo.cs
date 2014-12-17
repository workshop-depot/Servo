using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Servo
{
    #region Config
    class Config
    {
        public string ServiceName
        {
            get
            {
                const string key = "Servo/Conf/ServiceName";

                var val = ReadConf(key);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                return val;
            }
        }
        public string DisplayName
        {
            get
            {
                const string key = "Servo/Conf/DisplayName";

                var val = ReadConf(key);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                return val;
            }
        }
        public ServiceStartMode ServiceStartMode
        {
            get
            {
                const string key = "Servo/Conf/ServiceStartMode";

                var val = ReadConf(key);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                ServiceStartMode mode;
                if (!System.Enum.TryParse(val, out mode)) throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value");

                return mode;
            }
        }
        public string Description
        {
            get
            {
                const string key = "Servo/Conf/Description";

                var val = ReadConf(key);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                return val;
            }
        }
        public string[] ServicesDependedOn
        {
            get
            {
                const string key = "Servo/Conf/ServicesDependedOn";

                var val = ReadConf(key);
                if (string.IsNullOrWhiteSpace(val)) return new string[] { };

                return val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        public bool DelayedAutoStart
        {
            get
            {
                const string key = "Servo/Conf/DelayedAutoStart";

                var val = ReadConf(key);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                bool auto;
                if (!bool.TryParse(val, out auto)) throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value"); ;

                return auto;
            }
        }
        public bool RunInService
        {
            get
            {
                const string key = "Servo/Conf/RunInService";

                var val = ReadConf(key);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                bool runInService;
                if (!bool.TryParse(val, out runInService)) throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value"); ;

                return runInService;
            }
        }

        public override string ToString()
        {
            var depended = string.Join(", ", ServicesDependedOn);
            return new { ServiceName, DisplayName, ServiceStartMode, Description, ServicesDependedOn = depended, DelayedAutoStart, RunInService }.ToString();
        }

        static string ReadConf(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
    #endregion

    #region IService
    interface IService
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
    #endregion

    #region InService
    class InService : ServiceBase
    {
        IService _scaffold;
        public InService(IService scaffold)
        {
            _scaffold = scaffold;

            var conf = new Config();
            ServiceName = conf.ServiceName;

            CanHandlePowerEvent = false;
            CanHandleSessionChangeEvent = false;
            CanPauseAndContinue = false;
            CanShutdown = false;
            CanStop = true;
            AutoLog = true;
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
    }
    #endregion

    #region InApp
    class InApp
    {
        IService _scaffold;
        MainForm _form;
        public InApp(IService scaffold)
        {
            _scaffold = scaffold;
            _form = new MainForm(_scaffold);
        }

        public void Run()
        {
            Application.Run(_form);
        }
    }
    #endregion

    #region MainForm
    class MainForm : Form
    {
        IService _svc;

        public MainForm(IService svc)
        {
            InitializeComponent();

            _svc = svc;
            Start();
        }

        private async void stopBtn_Click(object sender, EventArgs e)
        {
            await Stop();
        }

        async void Start()
        {
            stopBtn.Enabled = false;
            await Task.Factory.StartNew(() => _svc.OnStart(Environment.GetCommandLineArgs()), TaskCreationOptions.LongRunning);
            stopBtn.Enabled = true;
        }

        async Task Stop()
        {
            await Task.Factory.StartNew(() => _svc.OnStop(), TaskCreationOptions.LongRunning);
            this.Close();
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.stopBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // stopBtn
            // 
            this.stopBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stopBtn.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopBtn.Location = new System.Drawing.Point(12, 12);
            this.stopBtn.Name = "stopBtn";
            this.stopBtn.Size = new System.Drawing.Size(218, 72);
            this.stopBtn.TabIndex = 0;
            this.stopBtn.Text = "Stop";
            this.stopBtn.UseVisualStyleBackColor = true;
            this.stopBtn.Click += new System.EventHandler(this.stopBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(242, 96);
            this.Controls.Add(this.stopBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Servo App";
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button stopBtn;
    }
    #endregion

    #region SvcController
    class SvcController
    {
        readonly ServiceController _c = new ServiceController(new Config().ServiceName);

        public void Start()
        {
            try
            {
                ClassLogger.Info("Starting Service...");
                const int timeout = 7 * 60;
                _c.Start();

                const ServiceControllerStatus targetStatus = ServiceControllerStatus.Running;
                _c.WaitForStatus(targetStatus, TimeSpan.FromSeconds(timeout));
            }
            catch (Exception ex)
            {
                ClassLogger.Error("Error in starting service: {0}", ex);
            }
        }

        public void Stop()
        {
            try
            {
                ClassLogger.Info("Stopping Service...");
                const int timeout = 7 * 60;
                _c.Stop();

                const ServiceControllerStatus targetStatus = ServiceControllerStatus.Stopped;
                _c.WaitForStatus(targetStatus, TimeSpan.FromSeconds(timeout));
            }
            catch (Exception ex)
            {
                ClassLogger.Error("Error in stopping service: {0}", ex);
            }
        }

        static readonly Logger ClassLogger = LogManager.GetCurrentClassLogger();
    }
    #endregion

    #region SvcInstaller
    [RunInstaller(true)]
    public partial class SvcInstaller : Installer
    {
        public SvcInstaller()
        {
            var processInstaller = new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem };

            var conf = new Config();
            var serviceInstaller = new ServiceInstaller
            {
                ServiceName = conf.ServiceName,
                DisplayName = conf.DisplayName,
                StartType = conf.ServiceStartMode,
                Description = conf.Description
            };

            try
            {
                serviceInstaller.ServicesDependedOn = conf.ServicesDependedOn;
            }
            catch (Exception ex) { ClassLogger.Error(ex); }

            try
            {
                serviceInstaller.DelayedAutoStart = conf.DelayedAutoStart;
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
    #endregion

    #region Extensions
    static class Extensions
    {
        public static void Run(this IService svc)
        {
            var conf = new Config();
            var flags = Environment.GetCommandLineArgs();
            var f = flags.Skip(1).FirstOrDefault() ?? string.Empty;

            if (f == "-h" || f == "-help")
            {
                ShowHelp(conf.RunInService);

                return;
            }

            if (conf.RunInService)
            {
                if (f == "-i" || f == "-install") SvcInstaller.Install(false, flags);
                else if (f == "-u" || f == "-uninstall") SvcInstaller.Install(true, flags);
                else if (f == "-start") new SvcController().Start();
                else if (f == "-stop") new SvcController().Stop();
                else if (f == "-a" || f == "-app") new InApp(svc).Run();
                else new InService(svc).Run();
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

            MessageBox.Show(h.ToString(), "XService");
        }
    }
    #endregion
}
