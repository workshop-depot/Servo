using System.Windows.Forms;

namespace ServoCS.Servo
{
    class InApp
    {
        readonly MainForm _form;
        public InApp(IService scaffold)
        {
            _form = new MainForm(scaffold);
        }

        public void Run()
        {
            Application.Run(_form);
        }
    }
}