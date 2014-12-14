Servo
=====

A Project Template for developing Windows Service easily.

## V 0.6

F# VSIX added. Both project templates will be accessible via Visual Studio Gallery.

## V 0.5

It is now a Windows Application, so the app can handle Windows Messages. The service has just to implement ```Servo.IService``` and the ```Main``` method would just call ```new XServiceService().Run();``` when developing, and ```new XServiceService().Run(true);``` when we release.

## V 0.1

Template base projects created; a single file _Servo.cs_, which is the Windows Service scaffold.

Currently, for using this we just need add:

```
  <appSettings>
    <add key="WinSvc/Conf/ServiceName" value="SampleService" />
    <add key="WinSvc/Conf/DisplayName" value="Sample Service" />
    <!-- System.ServiceProcess.ServiceStartMode -->
    <add key="WinSvc/Conf/ServiceStartMode" value="Automatic" />
    <add key="WinSvc/Conf/Description" value="Sample Service" />
    <!-- csv -->
    <add key="WinSvc/Conf/ServicesDependedOn" value=",," />
    <!-- bool -->
    <add key="WinSvc/Conf/DelayedAutoStart" value="true" />
  </appSettings>
```

To _App.Config_ and necessary assemblies.

_NLog_ is used for logging as a NuGet.
### TODO:

Create Visual Studio Project Template