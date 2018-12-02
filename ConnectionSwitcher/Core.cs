using System;
using System.Runtime.InteropServices;
using ConnectionSwitcher.Keyboard;
using ConnectionSwitcher.NativeLibraries;

namespace ConnectionSwitcher
{
    public class Core
    {
        private KeyboardHook _keyboard;
        private Switcher _switcher;
        
        public Core()
        {
            InitializeComponents();
            ApplicationLoop();
        }

        private void InitializeComponents()
        {
            _switcher = new Switcher();
            _switcher.LoadGatewaysFromFile(null);
            _switcher.LoadCurrentGateway();
            
            _keyboard = new KeyboardHook();
            _keyboard.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.F11 when e.ControlDown:
                    _switcher.ChangeGateway();
                    break;
                case Keys.F12 when e.ControlDown:
                    Console.WriteLine("[INFO] Current gateway: {0}", _switcher.Gateway);
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