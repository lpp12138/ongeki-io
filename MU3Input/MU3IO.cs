﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MU3Input
{
    public static class Mu3IO
    {
        internal static IO IO;
        private static IOTest _test;

        static Mu3IO()
        {
            var io = new MixedIO();
            foreach (var ioConfig in Config.Instance.IO)
            {
                io.Add(io.CreateIO(ioConfig.Type, ioConfig.Param), ioConfig.Part);
            }
            IO = io;
            _test = new IOTest(io);

            //Task.Run(() => _test.ShowDialog());
        }

        [DllExport(ExportName = "mu3_io_get_api_version")]
        public static ushort GetVersion()
        {
            return 0x0102;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_init")]
        public static uint Init()
        {
            if (Process.GetCurrentProcess().ProcessName != "amdaemon" &&
                  Process.GetCurrentProcess().ProcessName != "Debug" &&
                  Process.GetCurrentProcess().ProcessName != "TestSharp" &&
                  Process.GetCurrentProcess().ProcessName != "Test")
                return 1;
            else return 0;

        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_poll")]
        public static uint Poll()
        {
            if (IO == null)
                return 0;

            if (!IO.IsConnected)
            {
                IO.Reconnect();
            }

            _test.UpdateData();

            return 0;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_get_opbtns")]
        public static void GetOpButtons(out byte opbtn)
        {
            if (IO == null || !IO.IsConnected)
            {
                opbtn = 0;
                return;
            }

            opbtn = (byte)IO.OptButtonsStatus;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_get_gamebtns")]
        public static void GetGameButtons(out byte left, out byte right)
        {
            if (IO == null || !IO.IsConnected)
            {
                left = 0;
                right = 0;
                return;
            }

            left = IO.LeftButton;
            right = IO.RightButton;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_get_lever")]
        public static void GetLever(out short pos)
        {
            pos = 0;
            if (IO == null || !IO.IsConnected)
            {
                pos = 0;
                return;
            }

            pos = IO.Lever;
        }

        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_set_led")]
        public static void SetLed(uint data)
        {
            IO.SetLed(data);
            _test.SetColor(data);
        }


    }
}
