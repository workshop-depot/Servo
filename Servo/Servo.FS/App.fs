open Service
open Servo.Toolbox

[<EntryPoint>]
let main argv = 
    (new XServiceService()).Run(false)
    0 
