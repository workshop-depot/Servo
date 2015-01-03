namespace Servo

open NLog
open System
open System.Collections
open System.Collections.Generic
open System.ComponentModel
open System.Configuration
open System.Configuration.Install
open System.Linq
open System.ServiceProcess
open System.Text
open System.Text.RegularExpressions
open System.Threading.Tasks
open System.Windows.Forms

module Helpers =
    let readConf (key: string) = ConfigurationManager.AppSettings.[key]

    let asEnum s :'a option when 'a:enum<'b> =
        match System.Enum.TryParse s with
        | true, v -> Some v
        | _ -> None

open Helpers

type Config() =
    member this.ServiceName with get() = let key = "WinSvc/Conf/ServiceName"
                                         let v = readConf key
                                         if String.IsNullOrWhiteSpace(v) then raise <| SettingsPropertyNotFoundException("app setting " + key + " is not set")
                                         v

    member this.DisplayName with get() = let key = "WinSvc/Conf/DisplayName"
                                         let v = readConf key
                                         if String.IsNullOrWhiteSpace(v) then raise <| SettingsPropertyNotFoundException("app setting " + key + " is not set")
                                         v

    member this.ServiceStartMode with get() = let key = "WinSvc/Conf/ServiceStartMode"
                                              let v = readConf key
                                              if String.IsNullOrWhiteSpace(v) then raise <| SettingsPropertyNotFoundException("app setting " + key + " is not set")
                                              
                                              let mode = (asEnum v : ServiceStartMode option)
                                              match mode with
                                              | Some(m) -> m
                                              | _ -> raise <| SettingsPropertyNotFoundException("app setting " + key + " has a wrong value")

    member this.Description with get() = let key = "WinSvc/Conf/Description"
                                         let v = readConf key
                                         if String.IsNullOrWhiteSpace(v) then raise <| SettingsPropertyNotFoundException("app setting " + key + " is not set")
                                         v

    member this.ServicesDependedOn with get() = let key = "WinSvc/Conf/ServicesDependedOn"
                                                let v = readConf key
                                                if String.IsNullOrWhiteSpace(v) then 
                                                    [||]
                                                else
                                                    v.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)

    member this.DelayedAutoStart with get() = let key = "WinSvc/Conf/DelayedAutoStart"
                                              let v = readConf key
                                              if String.IsNullOrWhiteSpace(v) then raise <| SettingsPropertyNotFoundException("app setting " + key + " is not set")

                                              let ok, mode = Boolean.TryParse(v)
                                              if not ok then raise <| SettingsPropertyNotFoundException("app setting " + key + " has a wrong value")
                                              
                                              mode

    member this.RunInService with get() = let key = "Servo/Conf/RunInService"
                                          let v = readConf key
                                          if String.IsNullOrWhiteSpace(v) then raise <| SettingsPropertyNotFoundException("app setting " + key + " is not set")

                                          let ok, runInService = Boolean.TryParse(v)
                                          if not ok then raise <| SettingsPropertyNotFoundException("app setting " + key + " has a wrong value")
                                          
                                          runInService

    override this.ToString() =
        let depended = String.Join(", ", this.ServicesDependedOn)
        (("ServiceName", this.ServiceName),
         ("DisplayName", this.DisplayName),
         ("ServiceStartMode", this.ServiceStartMode),
         ("Description", this.Description),
         ("ServicesDependedOn", depended),
         ("DelayedAutoStart", this.DelayedAutoStart),
         ("RunInService", this.RunInService)).ToString()

type IService = 
    abstract member OnContinue      : unit -> unit
    abstract member OnCustomCommand : int -> unit
    abstract member OnPause         : unit -> unit
    abstract member OnPowerEvent    : PowerBroadcastStatus -> bool
    abstract member OnSessionChange : SessionChangeDescription -> unit
    abstract member OnShutdown      : unit -> unit
    abstract member OnStart         : string[] -> unit
    abstract member OnStop          : unit -> unit

type InService(scaffold: IService) =
    inherit ServiceBase()

    static let ClassLogger = LogManager.GetCurrentClassLogger()

    do
        let conf = Config()
        base.ServiceName <- conf.ServiceName

        base.CanHandlePowerEvent <- false
        base.CanHandleSessionChangeEvent <- false
        base.CanPauseAndContinue <- false
        base.CanShutdown <- false
        base.CanStop <- true
        base.AutoLog <- true

    override this.OnContinue() =
        try
            scaffold.OnContinue()
        with _ as NotImplementedException ->
            base.OnContinue()

    override this.OnCustomCommand(command: int) =
        try
            scaffold.OnCustomCommand(command)
        with _ as NotImplementedException ->
            base.OnCustomCommand(command)

    override this.OnPause() =
        try
            scaffold.OnPause()
        with _ as NotImplementedException ->
            base.OnPause()

    override this.OnPowerEvent(powerStatus: PowerBroadcastStatus) =
        try
            scaffold.OnPowerEvent(powerStatus)
        with _ as NotImplementedException ->
            base.OnPowerEvent(powerStatus)

    override this.OnSessionChange(changeDescription: SessionChangeDescription) =
        try
            scaffold.OnSessionChange(changeDescription)
        with _ as NotImplementedException ->
            base.OnSessionChange(changeDescription)

    override this.OnShutdown() =
        try
            scaffold.OnShutdown()
        with _ as NotImplementedException ->
            base.OnShutdown()

    override this.OnStart(args: string[]) =
        try
            scaffold.OnStart(args)
        with _ as NotImplementedException ->
            base.OnStart(args)

    override this.OnStop() =
        try
            scaffold.OnStop()
        with _ as NotImplementedException ->
            base.OnStop()

    member this.Run() = 
        ServiceBase.Run(this)

type MainForm(svc: IService) as thisObj =
    inherit Form()
    
    let mutable components : System.ComponentModel.IContainer = null
    let mutable stopBtn    : System.Windows.Forms.Button      = null

    do
        thisObj.InitializeComponent()
        thisObj.Start()

    member this.Start() =
        stopBtn.Enabled <- false
        Task.Factory.StartNew(fun () -> svc.OnStart(Environment.GetCommandLineArgs()), TaskCreationOptions.LongRunning).Wait()
        stopBtn.Enabled <- true

    member this.Stop() =
        stopBtn.Enabled <- false
        Task.Factory.StartNew(fun () -> svc.OnStop(), TaskCreationOptions.LongRunning).Wait()
        this.Close()

    member this.stopBtn_Click(sender: obj, e: EventArgs) =
        this.Stop()

    member this.InitializeComponent() =
        stopBtn <- new System.Windows.Forms.Button()
        this.SuspendLayout()
        // 
        // stopBtn
        // 
        stopBtn.FlatStyle <- System.Windows.Forms.FlatStyle.Flat
        stopBtn.Font <- new System.Drawing.Font("Courier New", 12.0f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0uy)
        stopBtn.Location <- new System.Drawing.Point(12, 12)
        stopBtn.Name <- "stopBtn"
        stopBtn.Size <- new System.Drawing.Size(218, 72)
        stopBtn.TabIndex <- 0
        stopBtn.Text <- "Stop"
        stopBtn.UseVisualStyleBackColor <- true
        stopBtn.Click.AddHandler(new System.EventHandler(fun s e -> this.stopBtn_Click(s, e)))
        // 
        // MainForm
        // 
        this.AutoScaleDimensions <- new System.Drawing.SizeF(6.0f, 13.0f)
        this.AutoScaleMode <- System.Windows.Forms.AutoScaleMode.Font
        this.ClientSize <- new System.Drawing.Size(242, 96)
        this.Controls.Add(stopBtn)
        this.FormBorderStyle <- System.Windows.Forms.FormBorderStyle.None
        this.MaximizeBox <- false
        this.MinimizeBox <- false
        this.Name <- "MainForm"
        this.ShowIcon <- false
        this.StartPosition <- System.Windows.Forms.FormStartPosition.CenterScreen
        this.Text <- "WinSvc App"
        this.ResumeLayout(false)

    override this.Dispose(disposing: bool) =
        if disposing && (components <> null) then
            components.Dispose()

        base.Dispose(disposing)

type InApp(scaffold: IService) =
    let form = new MainForm(scaffold)

    member this.Run() = 
        Application.Run(form)

type SvcController() =
    let _c = new ServiceController(Config().ServiceName)
    static let ClassLogger = LogManager.GetCurrentClassLogger()

    member this.Start() = 
        try
            ClassLogger.Info("Starting Service...")
            
            let timeout = 7.0 * 60.0
            _c.Start()

            let targetStatus = ServiceControllerStatus.Running
            _c.WaitForStatus(targetStatus, TimeSpan.FromSeconds(timeout))
        with ex ->
            ClassLogger.Error(String.Format("Error in starting service: {0}", ex))

    member this.Stop() = 
        try
            ClassLogger.Info("Stopping Service...")

            let timeout = 7.0 * 60.0
            _c.Stop()

            let targetStatus = ServiceControllerStatus.Stopped
            _c.WaitForStatus(targetStatus, TimeSpan.FromSeconds(timeout))
        with ex ->
            ClassLogger.Error("Error in stopping service: {0}", ex)

[<RunInstaller(true)>]
type public SvcInstaller() as thisObj =
    inherit Installer()

    static let ClassLogger = LogManager.GetCurrentClassLogger()

    do
        let processInstaller = new ServiceProcessInstaller()
        let serviceInstaller = new ServiceInstaller()

        processInstaller.Account <- ServiceAccount.LocalSystem 

        let conf = Config()
        serviceInstaller.ServiceName <- conf.ServiceName    
        serviceInstaller.DisplayName <- conf.DisplayName    
        serviceInstaller.StartType   <- conf.ServiceStartMode 
        serviceInstaller.Description <- conf.Description

        try
            serviceInstaller.ServicesDependedOn <- conf.ServicesDependedOn
        with ex ->
            ClassLogger.Error(ex)

        try
            serviceInstaller.DelayedAutoStart <- conf.DelayedAutoStart
        with ex ->
            ClassLogger.Error(ex)

        thisObj.Installers.Add(serviceInstaller) |> ignore
        thisObj.Installers.Add(processInstaller) |> ignore
        
    static member Install (undo: bool) (args: String[]) = 
        try
            ClassLogger.Info(if undo then "uninstalling" else "installing")

            if undo then
                ManagedInstallerClass.InstallHelper([| "/u"; System.Reflection.Assembly.GetAssembly(typeof<SvcInstaller>).Location |])
            else
                ManagedInstallerClass.InstallHelper([| System.Reflection.Assembly.GetAssembly(typeof<SvcInstaller>).Location |])
        with ex ->
            ClassLogger.Error(ex)

module Toolbox =
    let ShowHelp(runInService : bool) =
        let mutable h = StringBuilder()
        if (not runInService) then h <- h.AppendLine("<< IS NOT RUNNING IN SERVICE MODE >>\r\nChange value of Servo/Conf/RunInService in App.config to true.\r\n")
        h <- h
            .AppendLine("-h or -help\tshows this help")
            .AppendLine("-i or -install\tinstalls this windows service")
            .AppendLine("-u or -uninstall\tuninstalls this windows service")
            .AppendLine("-start\t\tstarts this windows service")
            .AppendLine("-stop\t\tstops this windows service")
            .AppendLine("-a or -app\t\truns this windows service as an app")

        MessageBox.Show(h.ToString(), "XService") |> ignore

    type IService with
        member this.Run () =
            let conf = new Config()
            let flags = Environment.GetCommandLineArgs()
            let mutable f = flags.Skip(1).FirstOrDefault()
            if String.IsNullOrWhiteSpace(f) then f <- String.Empty

            match conf.RunInService, f with
            | _, "-h" | _, "-help" -> ShowHelp(conf.RunInService)
            | true, _ ->
                match f with
                | "-i" | "-install" -> SvcInstaller.Install false flags
                | "-u" | "-uninstall" -> SvcInstaller.Install true flags
                | "-start" -> (new SvcController()).Start()
                | "-stop" -> (new SvcController()).Stop()
                | "-a" | "-app" -> (new InApp(this)).Run()
                | _ -> (new InService(this)).Run()
            | _ -> (new InApp(this)).Run()
                
