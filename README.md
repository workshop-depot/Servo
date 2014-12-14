Servo
=====

A Project Template for developing Windows Service easily.

## V 0.7

RunInService is now in App.config so it can be easily changed if one forgot to set it to true even after deployment; so there is no difference between development assemblies and deployment assemblies other than the flag in App.config.

## V 0.6

F# VSIX added. Both project templates will be accessible via Visual Studio Gallery.

## V 0.5

It is now a Windows Application, so the app can handle Windows Messages. The service has just to implement ```Servo.IService``` and the ```Main``` method would just call ```new XServiceService().Run();``` when developing, and ```new XServiceService().Run(true);``` when we release.

## V 0.1

(Structurally changed, not applicable)

_NLog_ is used for logging as a NuGet.
### TODO:

- Update Readme.md, include a quick tutorial
- Move boolean flag from Run to App.config
- Remove $safeprojectname$ from service name
