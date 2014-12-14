open XService
open Servo.Toolbox

[<EntryPoint>]
let main argv = 
    (new SampleService()).Run()
     
    0 
