using System;
using System.Runtime.InteropServices;
using GatewayChanger.Test.Keyboard;
using GatewayChanger.Test.Native;
using GatewayChanger.Test.Native.Enums;
using GatewayChanger.Test.Native.Structures;

namespace GatewayChanger.Test
{
    public class Core
    {
        private KeyboardHook _keyboard;
        private GatewayManager _manager;
        
        public Core()
        {
            InitializeComponents();
            ApplicationLoop();
        }

        private void InitializeComponents()
        {
            _manager = new GatewayManager();
            _manager.LoadGatewaysFromFile(null);
            _manager.EnsureCorrectGateway();
            
            _keyboard = new KeyboardHook();
            _keyboard.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.F11 when e.ControlDown:
                    _manager.UseNext();
                    break;
                case Keys.F12 when e.ControlDown:
                    Console.WriteLine("[INFO] Current gateway: {0}", _manager.Gateway);
                    break;
            }
        }

        private static void ApplicationLoop()
        {
            int ret;
            while ((ret = NativeLibrary.User32.GetMessageA(out MSG msg, IntPtr.Zero, 0, 0)) != 0)
            {
                if (ret == -1)
                {
                    Console.WriteLine("An error occurred in the main loop. (Win32Error: {0})", Marshal.GetLastWin32Error());
                    return;
                }
                
                NativeLibrary.User32.TranslateMessage(ref msg);
                NativeLibrary.User32.DispatchMessageA(ref msg);
            }
        }
    }
}