using System;
using ServoCS.Servo;

namespace ServoCS
{
    static class App
    {
        static void Main(string[] args)
        {
            new ServoCSService().Run();
        }
    }
}
