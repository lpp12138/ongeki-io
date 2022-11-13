﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MU3Input
{
    internal class KeyboardIO : IO
    {
        private KeyboardIOConfig config;
        public override OutputData Data => GetData();

        public override bool IsConnected => true;

        public KeyboardIO(KeyboardIOConfig param)
        {
            config = param;
        }

        public override void Reconnect() { }

        public override void SetLed(uint data) { }


        private OutputData GetData()
        {
            byte[] buttons = new byte[] {
                Pressed(config.L1),
                Pressed(config.L2),
                Pressed(config.L3),
                Pressed(config.LSide),
                Pressed(config.LMenu),
                Pressed(config.R1),
                Pressed(config.R2),
                Pressed(config.R3),
                Pressed(config.RSide),
                Pressed(config.RMenu),
            };
            short lever = 0;
            OptButtons optButtons = (OptButtons)(Pressed(config.Test) << 0 | Pressed(config.Service) << 1);
            Aime aime = new Aime()
            {
                Scan = Pressed(config.Scan)
            };
            if (aime.Scan == 1)
            {
                byte[] bytes = Utils.ReadOrCreateAimeTxt();
                aime.Mifare = Mifare.Create(bytes);
            }
            return new OutputData
            {
                Buttons = buttons,
                Lever = lever,
                OptButtons = optButtons,
                Aime = aime
            };
        }
        private byte Pressed(Keys key)
        {
            return User32.GetAsyncKeyState(key) == 0 ? (byte)0 : (byte)1;
        }
        public class KeyboardIOConfig
        {
            public Keys L1 { get; set; }
            public Keys L2 { get; set; }
            public Keys L3 { get; set; }
            public Keys LSide { get; set; }
            public Keys LMenu { get; set; }
            public Keys R1 { get; set; }
            public Keys R2 { get; set; }
            public Keys R3 { get; set; }
            public Keys RSide { get; set; }
            public Keys RMenu { get; set; }
            public Keys Test { get; set; }
            public Keys Service { get; set; }
            public Keys Scan { get; set; }
        }
    }
}
