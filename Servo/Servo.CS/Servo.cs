using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
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
                const string key = "WinSvc/Conf/ServiceName";

                var val = ReadConf(key);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                return val;
            }
        }
        public string DisplayName
        {
            get
            {
                const string key = "WinSvc/Conf/DisplayName";

                var val = ReadConf(key);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                return val;
            }
        }
        public ServiceStartMode ServiceStartMode
        {
            get
            {
                const string key = "WinSvc/Conf/ServiceStartMode";

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
                const string key = "WinSvc/Conf/Description";

                var val = ReadConf(key);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                return val;
            }
        }
        public string[] ServicesDependedOn
        {
            get
            {
                const string key = "WinSvc/Conf/ServicesDependedOn";

                var val = ReadConf(key);
                if (string.IsNullOrWhiteSpace(val)) return new string[] { };

                return val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        public bool DelayedAutoStart
        {
            get
            {
                const string key = "WinSvc/Conf/DelayedAutoStart";

                var val = ReadConf(key);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                bool auto;
                if (!bool.TryParse(val, out auto)) throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value"); ;

                return auto;
            }
        }

        public override string ToString()
        {
            var depended = string.Join(", ", ServicesDependedOn);
            return new { ServiceName, DisplayName, ServiceStartMode, Description, ServicesDependedOn = depended, DelayedAutoStart }.ToString();
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
            this.Text = "WinSvc App";
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
                using (var assinst = new AssemblyInstaller(typeof(SvcInstaller).Assembly, args))
                {
                    IDictionary state = new Hashtable();
                    assinst.UseNewContext = true;

                    try
                    {
                        if (undo)
                        {
                            assinst.Uninstall(state);
                        }
                        else
                        {
                            assinst.Install(state);
                            assinst.Commit(state);
                        }
                    }
                    catch
                    {
                        try
                        {
                            assinst.Rollback(state);
                        }
                        catch { }

                        throw;
                    }
                }
            }
            catch (Exception ex) { ClassLogger.Error(ex); }
        }

        static readonly Logger SlotClassLogger = LogManager.GetCurrentClassLogger();
        static Logger ClassLogger { get { return SlotClassLogger; } }
    }
    #endregion

    #region CommandLineArgs
    class CommandLineArgs
    {
        const string ArgTemplate = "(?<parameter>(?<prefix>^-{1,2}|^/)(?<switch>[^=:]+)(?<splitter>[=:]{0,1})(?<value>.*$))|(?<argument>.*)";
        readonly List<Arg> _args;

        public CommandLineArgs() { _args = Parser(Environment.GetCommandLineArgs()); }
        public CommandLineArgs(string[] args) { _args = Parser(args); }

        public string this[string @switch]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(@switch)) throw new ArgumentNullException("@switch");

                var @param = (from a in _args where a.IsParameter && string.Compare(a.Switch, @switch, true) == 0 select a).FirstOrDefault();
                if (@param != null)
                {
                    if (!string.IsNullOrWhiteSpace(@param.Value) || !string.IsNullOrWhiteSpace(@param.Splitter)) return @param.Value;
                    return @param.Switch;
                }

                @param = (from a in _args where !a.IsParameter && string.Compare(a.Value, @switch, true) == 0 select a).FirstOrDefault();
                if (@param != null) return @param.Value;

                return null;
            }
        }
        public string[] this[string @switch, int maxNumberOfArguments]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(@switch)) throw new ArgumentNullException("@switch");

                maxNumberOfArguments = maxNumberOfArguments == -1 ? int.MaxValue - 1 : maxNumberOfArguments;

                Arg @param = null;
                var args = new List<Arg>();

                var part1 = _args.SkipWhile(p => string.Compare(p.Switch, @switch, true) != 0).ToList();
                @param = part1.Take(1).FirstOrDefault();
                if (@param != null)
                {
                    args = part1.Skip(1).TakeWhile(a => !a.IsParameter).ToList();
                    args = args.Take(maxNumberOfArguments).ToList();
                }

                return @param != null ? (from a in args select a.Value).ToArray() : null;
            }
        }

        static List<Arg> Parser(string[] args)
        {
            if (args == null || args.Length == 0) return new List<Arg>();

            Func<Capture, string> getVal = c => { return c == null ? null : c.Value; };

            var result = new List<Arg>();

            foreach (var a in args)
            {
                var m = Regex.Match(a, ArgTemplate);
                if (m.Groups["parameter"] != null && m.Groups["parameter"].Success)
                {
                    result.Add(new Arg(
                        getVal(m.Groups["prefix"]),
                        getVal(m.Groups["switch"]),
                        getVal(m.Groups["splitter"]),
                        getVal(m.Groups["value"])));
                }
                else
                {
                    result.Add(new Arg(getVal(m.Groups["argument"])));
                }
            }

            return result;
        }

        class Arg
        {
            /// <summary>
            /// is a parameter or an argument
            /// </summary>
            public bool IsParameter { get; private set; }
            public string Prefix { get; private set; }
            public string Switch { get; private set; }
            public string Splitter { get; private set; }
            public string Value { get; private set; }

            private Arg() { }
            public Arg(
                string prefix,
                string switch_,
                string splitter,
                string val)
            {
                Prefix = prefix;
                Switch = switch_;
                Splitter = splitter;
                Value = val;
                IsParameter = true;
            }
            public Arg(
                string val)
            {
                Value = val;
                IsParameter = false;
            }

            public override string ToString()
            {
                return IsParameter ?
                    string.Format("{0}{1}{2}{3}",
                        Prefix ?? string.Empty,
                        Switch ?? string.Empty,
                        Splitter ?? string.Empty,
                        Value ?? string.Empty) :
                    string.Format("{0}", Value ?? string.Empty);
            }
        }
    }
    #endregion

    #region Extensions
    static class Extensions
    {
        public static void Run(this IService svc, bool runInService = false)
        {
            var flags = new CommandLineArgs();

            if (flags["h"].IsSupplied() || flags["help"].IsSupplied())
            {
                ShowHelp();

                return;
            }

            if (runInService)
            {
                var args = Environment.GetCommandLineArgs();

                if (flags["i"].IsSupplied() || flags["install"].IsSupplied()) SvcInstaller.Install(false, args);
                else if (flags["u"].IsSupplied() || flags["uninstall"].IsSupplied()) SvcInstaller.Install(true, args);
                else if (flags["start"].IsSupplied()) new SvcController().Start();
                else if (flags["stop"].IsSupplied()) new SvcController().Stop();
                else if (flags["a"].IsSupplied() || flags["app"].IsSupplied()) new InApp(svc).Run();
                else new InService(svc).Run();
            }
            else
            {
                new InApp(svc).Run();
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("-h or -help          shows this help");
            Console.WriteLine("-i or -install       installs this windows service");
            Console.WriteLine("-u or -uninstall     uninstalls this windows service");
            Console.WriteLine("-start               starts this windows service");
            Console.WriteLine("-stop                stops this windows service");
            Console.WriteLine("-a or -app           runs this windows service as an app");
        }

        internal static bool IsSupplied(this string arg) { return !string.IsNullOrWhiteSpace(arg); }
    }
    #endregion
}
