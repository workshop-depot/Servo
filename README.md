Servo
=====

A Project Template for developing Windows Service easily.

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
