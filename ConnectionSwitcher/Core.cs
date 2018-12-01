using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AdvancedDLSupport;

namespace ConnectionSwitcher
{
    public class Core
    {
        private Switcher _switcher;
        private IUser32 _user32;
        
        public Core()
        {
            InitializeComponents();
            LoadNativeLibraries();
            RegisterHotkeys();
        }

        private void InitializeComponents()
        {
            _switcher = new Switcher();
            _switcher.LoadGatewaysFromFile(null);
        }

        private void LoadNativeLibraries()
        {
            var activator = new NativeLibraryBuilder();
            
            _user32 = activator.ActivateInterface<IUser32>("user32.dll");
        }

        private void RegisterHotkeys()
        {
            _user32.RegisterHotKey(IntPtr.Zero, 0, KeyModifiers.Control, Keys.F11);
            _user32.RegisterHotKey(IntPtr.Zero, 1, KeyModifiers.Control, Keys.F12);
        }

        private void HandleHotkey(uint id)
        {
            Console.WriteLine("[DEBUG] Received hoykey {0}", id);
            switch (id)
            {
                case 0:
                    _switcher.ChangeGateway();
                    break;
                case 1:
                    Console.WriteLine("[INFO] Current gateway: {0}", _switcher.Gateway);
                    break;
                default:
                    Console.WriteLine("[ERROR] Received invalid hotkey id. (ID: {0})", id);
                    break;
            }
        }

        public void ApplicationLoop()
        {
            const uint WM_HOTKEY = 0x312;
            
            int ret;
            while ((ret = _user32.GetMessageA(out MSG msg, IntPtr.Zero, 0, 0)) != 0)
            {
                if (ret == -1)
                {
                    Console.WriteLine("An error occurred in the main loop. (HRESULT: {0})", Marshal.GetLastWin32Error());
                    return;
                }

                if (msg.message == WM_HOTKEY)
                    HandleHotkey(msg.wParam.ToUInt32());
                
                _user32.TranslateMessage(ref msg);
                _user32.DispatchMessageA(ref msg);
            }
        }
    }
}