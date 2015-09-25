Servo
=====

A Project Template for developing Windows Service easily (Available on [Visual Studio Gallery](https://visualstudiogallery.msdn.microsoft.com/3014ea0a-7959-4e04-8681-f68feeee9ef7)).

For developing a Windows Service:

- Implement `IService` like `SampleService`.

This service will run in `Main` method just using this code: `new SampleService().Run();`.

After you are done with development of your service, just set `Servo/Conf/RunInService` in `App.config` to `true`. After that, in the command line, pass `-i` to install the service.

You can also set things like service name and description in `App.config`.

## V 0.9

Some settings added and project structure changed; keeping in mind to not breaking things as much as possible.

## V 0.8

Help for command line shows a dialog now.

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
