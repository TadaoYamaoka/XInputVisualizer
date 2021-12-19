using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vortice.XInput;

namespace XInputVisualizer
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    public class Input
    {
        public State keystate;
        public int Index { get; set; } = 0;

        private System.Threading.Timer Timer { get; set; }
        public int Interval { get; set; } = 33;

        protected object Lock { get; } = new object();

        public void Start(Object state)
        {
            Timer = new System.Threading.Timer(Elapsed, state, 0, Interval);
        }

        public void Stop()
        {
            Timer?.Dispose();
            Timer = null;
        }

        private void Elapsed(Object state)
        {
            if (XInput.GetState(Index, out keystate))
            {
                Form1 form1 = (Form1)state;
                form1.Invalidate();
            }
        }
    }

    public class JoystickItem
    {
        public JoystickItem(string name, int index)
        {
            Name = name;
            Index = index;
        }

        public string Name { get; set; }
        public int Index { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
