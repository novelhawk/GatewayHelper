using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace ConnectionSwitcher
{
    public class Core
    {
        private HookProc _hook;
        private Switcher _switcher;
        private IUser32 _user32;
        private IKernel32 _kernel32;
        
        public Core()
        {
            InitializeComponents();
            LoadNativeLibraries();
            RegisterKeyboardHook();
        }

        private void RegisterKeyboardHook()
        {
            _hook = LowLevelKeyboardProc;
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                var handle = _kernel32.GetModuleHandleA(curModule.ModuleName);
                _user32.SetWindowsHookExA(HookType.WH_KEYBOARD_LL, _hook, handle, 0);
            }
        }

        private bool _isControlDown;
        private IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT kbd = (KBDLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                switch (wParam.ToInt32())
                {
                    case (int) WM.KEYDOWN:
                        switch (kbd.vkCode)
                        {
                            case Keys.ControlKey:
                            case Keys.LControlKey:
                            case Keys.RControlKey:
                                _isControlDown = true;
                                break;
                            case Keys.F11 when _isControlDown:
                                _switcher.ChangeGateway();
                                break;
                            case Keys.F12 when _isControlDown:
                                Console.WriteLine("[INFO] Current gateway: {0}", _switcher.Gateway);
                                break;
                        }
                        break;
                    
                    case (int) WM.KEYUP:
                        switch (kbd.vkCode)
                        {
                            case Keys.ControlKey:
                            case Keys.LControlKey:
                            case Keys.RControlKey:
                                _isControlDown = false;
                                break;
                        }
                        break;
                }
            }
            return _user32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
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
            _kernel32 = activator.ActivateInterface<IKernel32>("kernel32.dll");
        }

        public void ApplicationLoop()
        {
            int ret;
            while ((ret = _user32.GetMessageA(out MSG msg, IntPtr.Zero, 0, 0)) != 0)
            {
                if (ret == -1)
                {
                    Console.WriteLine("An error occurred in the main loop. (HRESULT: {0})", Marshal.GetLastWin32Error());
                    return;
                }
                
                _user32.TranslateMessage(ref msg);
                _user32.DispatchMessageA(ref msg);
            }
        }
    }
}