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
        private int[] history = new int[120];
        private int currentPos = 0;

        const int LeftTrigger = 65536;
        const int RightTrigger = 131072;

        Pen linePen = new Pen(Color.LightGray, 1);
        Pen onPen = new Pen(Color.Gray, 4.5F);
        Pen onPen2 = new Pen(Color.DarkGray, 4.5F);
        Font drawFont = new Font("MS Serif", 11F, GraphicsUnit.Pixel);
        string[] btnNames = new string[] { "↑", "↗", "→", "↘", "↓", "↙", "←", "↖", "X", "Y", "A", "B", "LB", "RB", "LT", "RT" };
        int[] btnFlags = new int[] {
            (int)GamepadButtons.DPadUp, (int)GamepadButtons.DPadUp | (int)GamepadButtons.DPadRight,
            (int)GamepadButtons.DPadRight, (int)GamepadButtons.DPadRight | (int)GamepadButtons.DPadDown,
            (int)GamepadButtons.DPadDown, (int)GamepadButtons.DPadDown | (int)GamepadButtons.DPadLeft,
            (int)GamepadButtons.DPadLeft, (int)GamepadButtons.DPadLeft | (int)GamepadButtons.DPadUp,
            (int)GamepadButtons.X, (int)GamepadButtons.Y,
            (int)GamepadButtons.A, (int)GamepadButtons.B,
            (int)GamepadButtons.LeftShoulder, (int)GamepadButtons.RightShoulder,
            LeftTrigger, RightTrigger
        };
        Pen[] btnPen;
        int dirMask = (int)GamepadButtons.DPadUp | (int)GamepadButtons.DPadRight | (int)GamepadButtons.DPadDown | (int)GamepadButtons.DPadLeft;

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

            btnPen = new Pen[] {
                onPen, onPen2, onPen, onPen2, onPen, onPen2, onPen, onPen2,
                new Pen(Color.Blue, 4.5F), new Pen(Color.FromArgb(255, 242, 0), 4.5F),
                new Pen(Color.Green, 4.5F), new Pen(Color.Red, 4.5F),
                onPen, onPen, onPen, onPen,
            };
        }

        private void comboBoxJoystick_SelectedIndexChanged(object sender, EventArgs e)
        {
            input.Index = ((JoystickItem)comboBoxJoystick.Items[comboBoxJoystick.SelectedIndex]).Index;
            comboBoxJoystick.Enabled = false;
            comboBoxJoystick.Enabled = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            int state = (int)input.keystate.Gamepad.Buttons;
            if (0 < input.keystate.Gamepad.LeftTrigger)
                state |= LeftTrigger;
            if (0 < input.keystate.Gamepad.RightTrigger)
                state |= RightTrigger;
            history[currentPos++] = state;
            if (currentPos == history.Length)
                currentPos = 0;

            pictureBoxStick.Refresh();
            pictureBoxButton1.Refresh();
            pictureBoxButton2.Refresh();
            pictureBoxButton3.Refresh();
            pictureBoxButton4.Refresh();
            pictureBoxButton5.Refresh();
            pictureBoxButton6.Refresh();
            pictureBoxButton7.Refresh();
            pictureBoxButton8.Refresh();
            pictureBoxTimeLine.Refresh();
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
            for (int i = 0; i < period; i++)
            {
                float x2 = 113.5F + x[i] * 100.0F;
                float y2 = 113.5F + y[i] * 100.0F;
                float width = i + 1;
                g.FillEllipse(Brushes.Red, x2 - width / 2, y2 - width / 2, width, width);
            }
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

        private void drawOn(Graphics g, int k, int x2, int y2)
        {
            g.DrawLine(btnPen[k], new Point(x2, y2 - 4), new Point(x2, y2 + 4));
        }

        private void pictureBoxTimeLine_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            for (int i = 0; i < btnNames.Length; i++)
            {
                int y2 = 10 + i * 12;
                g.DrawString(btnNames[i], drawFont, Brushes.Black, new Point(0, y2 - 7));
                g.DrawLine(linePen, new Point(16, y2), new Point(pictureBoxTimeLine.Width, y2));
            }

            for (int j = 0; j < 35; j++)
            {
                int x2 = pictureBoxTimeLine.Width + 160 - currentPos % 40 * 4 - j * 20;
                if (x2 > 16)
                {
                    g.DrawLine(linePen, new Point(x2, 0), new Point(x2, pictureBoxTimeLine.Height));
                }
            }

            int pos = currentPos;
            int num = (pictureBoxTimeLine.Width - 16) / 4;
            Span<int> offCounts = stackalloc int[btnFlags.Length];
            for (int j = 0; j < num; j++)
            {
                pos--;
                if (pos < 0) pos += history.Length;

                int x2 = pictureBoxTimeLine.Width - j * 4 - 4;

                for (int k = 0; k < btnFlags.Length; k++)
                {
                    int y2 = 10 + k * 12;

                    // direction
                    if (k < 8)
                    {
                        if ((history[pos] & dirMask) == btnFlags[k])
                        {
                            drawOn(g, k, x2, y2);
                            offCounts[k] = 1;
                        }
                        else if (offCounts[k] > 0)
                            offCounts[k]++;
                    }
                    else
                    {
                        if ((history[pos] & btnFlags[k]) > 0)
                        {
                            drawOn(g, k, x2, y2);
                            offCounts[k] = 1;
                        }
                        else if (offCounts[k] > 0)
                            offCounts[k]++;
                    }

                    if (offCounts[k] == 6)
                        g.DrawString(btnNames[k], drawFont, Brushes.Black, new Point(x2, y2 - 7));
                }
            }
        }
    }
}

