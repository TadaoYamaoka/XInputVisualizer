using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vortice.XInput;

namespace XInputVisualizer
{
    public partial class Form1 : Form
    {
        Input input = new Input();
        private const int period = 30;
        private float[] x = new float[period];
        private float[] y = new float[period];
        private float[] buttons = new float[8];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                if (XInput.GetState(i, out var keystate))
                {
                    comboBoxJoystick.Items.Add(new JoystickItem("Joystick #" + i.ToString(), i));
                }
            }

            if (comboBoxJoystick.Items.Count > 0)
            {
                comboBoxJoystick.SelectedIndex = 0;
                input.Index = ((JoystickItem)comboBoxJoystick.Items[0]).Index;
                input.Start(this);
            }
        }

        private void pictureBoxStick_Paint(object sender, PaintEventArgs e)
        {
            if (0 < (input.keystate.Gamepad.Buttons & GamepadButtons.DPadLeft))
                x[period - 1] = -1.0F;
            else if (0 < (input.keystate.Gamepad.Buttons & GamepadButtons.DPadRight))
                x[period - 1] = 1.0F;
            else
                x[period - 1] = 0.0F;
            if (0 < (input.keystate.Gamepad.Buttons & GamepadButtons.DPadUp))
                y[period - 1] = -1.0F;
            else if (0 < (input.keystate.Gamepad.Buttons & GamepadButtons.DPadDown))
                y[period - 1] = 1.0F;
            else
                y[period - 1] = 0.0F;

            for (int i = period - 2; i >= 0; i--)
            {
                x[i] = x[i + 1] - (x[i + 1] - x[i]) / 4;
                y[i] = y[i + 1] - (y[i + 1] - y[i]) / 4;
            }

            var g = e.Graphics;
            g.Clear(Color.White);
            for (int i = 0; i < period; i++)
            {
                float x2 = 113.5F + x[i] * 100.0F;
                float y2 = 113.5F + y[i] * 100.0F;
                float width = i + 1;
                g.FillEllipse(Brushes.Red, x2 - width / 2, y2 - width / 2, width, width);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            pictureBoxStick.Refresh();
            pictureBoxButton1.Refresh();
            pictureBoxButton2.Refresh();
            pictureBoxButton3.Refresh();
            pictureBoxButton4.Refresh();
            pictureBoxButton5.Refresh();
            pictureBoxButton6.Refresh();
            pictureBoxButton7.Refresh();
            pictureBoxButton8.Refresh();
        }

        private void pictureBoxButton_Paint(PaintEventArgs e, int index, bool on)
        {
            if (on)
            {
                buttons[index] = 1.0F;
            }
            else
            {
                buttons[index] = Math.Max(0, buttons[index] - 0.1F);
            }

            var g = e.Graphics;
            g.Clear(Color.White);
            for (int i = 0; i < period; i++)
            {
                float width = buttons[index] * 28;
                g.FillEllipse(Brushes.Red, 13.5F - width / 2, 13.5F - width / 2, width, width);
            }
        }

        private void pictureBoxButton1_Paint(object sender, PaintEventArgs e)
        {
            pictureBoxButton_Paint(e, 0, 0 < (input.keystate.Gamepad.Buttons & GamepadButtons.A));
        }

        private void pictureBoxButton2_Paint(object sender, PaintEventArgs e)
        {
            pictureBoxButton_Paint(e, 1, 0 < (input.keystate.Gamepad.Buttons & GamepadButtons.B));
        }

        private void pictureBoxButton3_Paint(object sender, PaintEventArgs e)
        {
            pictureBoxButton_Paint(e, 2, 0 < (input.keystate.Gamepad.Buttons & GamepadButtons.X));
        }

        private void pictureBoxButton4_Paint(object sender, PaintEventArgs e)
        {
            pictureBoxButton_Paint(e, 3, 0 < (input.keystate.Gamepad.Buttons & GamepadButtons.Y));
        }

        private void pictureBoxButton5_Paint(object sender, PaintEventArgs e)
        {
            pictureBoxButton_Paint(e, 4, 0 < (input.keystate.Gamepad.Buttons & GamepadButtons.LeftShoulder));
        }

        private void pictureBoxButton6_Paint(object sender, PaintEventArgs e)
        {
            pictureBoxButton_Paint(e, 5, 0 < (input.keystate.Gamepad.Buttons & GamepadButtons.RightShoulder));
        }

        private void pictureBoxButton7_Paint(object sender, PaintEventArgs e)
        {
            pictureBoxButton_Paint(e, 6, 0 < (input.keystate.Gamepad.RightTrigger));
        }

        private void pictureBoxButton8_Paint(object sender, PaintEventArgs e)
        {
            pictureBoxButton_Paint(e, 7, 0 < (input.keystate.Gamepad.LeftTrigger));
        }

        private void comboBoxJoystick_SelectedIndexChanged(object sender, EventArgs e)
        {
            input.Index = ((JoystickItem)comboBoxJoystick.Items[comboBoxJoystick.SelectedIndex]).Index;
            comboBoxJoystick.Enabled = false;
            comboBoxJoystick.Enabled = true;
        }
    }
}

