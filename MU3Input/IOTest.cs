﻿using System;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace MU3Input
{
    public partial class IOTest : Form
    {
        private IO _io;

        private CheckBox[] _left;
        private CheckBox[] _right;

        public IOTest(IO io)
        {
            InitializeComponent();

            _left = new[] {
                lA,
                lB,
                lC,
                lS,
                lM,
            };

            _right = new[] {
                rA,
                rB,
                rC,
                rS,
                rM,
            };

            _io = io;
        }

        public static byte[] StringToByteArray(string hex)
        {
            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        internal void UpdateData()
        {
            if (!Enabled && Handle == IntPtr.Zero) return;

            try
            {
                BeginInvoke(new Action(() =>
                {

                    if (!_io.IsConnected) return;

                    for (var i = 0; i < 5; i++)
                    {
                        _left[i].Checked = Convert.ToBoolean(_io.Data.Buttons[i]);
                        _right[i].Checked = Convert.ToBoolean(_io.Data.Buttons[i + 5]);
                    }

                    trackBar1.Value = _io.Lever;

                    if (_io.Aime.Scan == 0)
                    {
                        label1.Text = "Aime";
                        textAimiId.Text = "None";
                    }
                    else if (_io.Aime.Scan == 1)
                    {
                        label1.Text = "AimeID";
                        textAimiId.Text = BitConverter.ToString(_io.Aime.ID).Replace("-", "");
                    }
                    else if (_io.Aime.Scan == 2)
                    {
                        label1.Text = "IDm";
                        textAimiId.Text = "0x" + BitConverter.ToUInt64(BitConverter.GetBytes(_io.Aime.IDm).Reverse().ToArray(), 0).ToString("X16");
                    }

                    lS.BackColor = Color.FromArgb(Mu3IO.LedData[0], Mu3IO.LedData[1], Mu3IO.LedData[2]);
                    rS.BackColor = Color.FromArgb(Mu3IO.LedData[3], Mu3IO.LedData[4], Mu3IO.LedData[5]);
                }));
            }
            catch
            {
                // ignored
            }
        }

        public void SetColor(uint data)
        {
            try
            {
                BeginInvoke(new Action(() =>
                {
                    _left[0].BackColor = Color.FromArgb(
                        (int)((data >> 23) & 1) * 255,
                        (int)((data >> 19) & 1) * 255,
                        (int)((data >> 22) & 1) * 255
                    );
                    _left[1].BackColor = Color.FromArgb(
                        (int)((data >> 20) & 1) * 255,
                        (int)((data >> 21) & 1) * 255,
                        (int)((data >> 18) & 1) * 255
                    );
                    _left[2].BackColor = Color.FromArgb(
                        (int)((data >> 17) & 1) * 255,
                        (int)((data >> 16) & 1) * 255,
                        (int)((data >> 15) & 1) * 255
                    );
                    _right[0].BackColor = Color.FromArgb(
                        (int)((data >> 14) & 1) * 255,
                        (int)((data >> 13) & 1) * 255,
                        (int)((data >> 12) & 1) * 255
                    );
                    _right[1].BackColor = Color.FromArgb(
                        (int)((data >> 11) & 1) * 255,
                        (int)((data >> 10) & 1) * 255,
                        (int)((data >> 9) & 1) * 255
                    );
                    _right[2].BackColor = Color.FromArgb(
                        (int)((data >> 8) & 1) * 255,
                        (int)((data >> 7) & 1) * 255,
                        (int)((data >> 6) & 1) * 255
                    );
                }));
            }
            catch
            {
                // ignored
            }
        }

        private void btnSetOption_Click(object sender, EventArgs e)
        {
            byte[] aimiId;

            try
            {
                aimiId = StringToByteArray(textAimiId.Text);
            }
            catch
            {
                MessageBox.Show("Invaild id, Id need to be a hex dump of 10 byte data.", "Error");
                return;
            }

            if (aimiId.Length != 10)
            {
                MessageBox.Show("Invaild id, Id need to be a hex dump of 10 byte data.");
                return;
            }

            //_io.SetAimiId(aimiId);
        }
    }
}
