using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using GatewayChanger.Test.Native;
using GatewayChanger.Test.Native.Enums;
using GatewayChanger.Test.Native.Structures;

namespace GatewayChanger.Test.Keyboard
{
    public sealed class KeyboardHook
    {
        // Keep alive the callback by keeping a reference in managed
        private static HookProc _hook;
        
        private bool _isControlDown;

        public KeyboardHook()
        {
            SetHook();
        }

        private void SetHook()
        {
            _hook = LowLevelKeyboardProc;
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                var handle = NativeLibrary.Kernel32.GetModuleHandleA(curModule.ModuleName);
                NativeLibrary.User32.SetWindowsHookExA(HookType.WH_KEYBOARD_LL, _hook, handle, 0);
            }
        }
        
        private IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KeyInfo keyInfo = (KeyInfo) Marshal.PtrToStructure(lParam, typeof(KeyInfo));
                switch (wParam.ToInt32())
                {
                    case (int) WM.KEYDOWN:
                        switch (keyInfo.KeyCode)
                        {
                            case Keys.ControlKey:
                            case Keys.LControlKey:
                            case Keys.RControlKey:
                                _isControlDown = true;
                                break;
                        }
                        OnKeyDown(new KeyboardEventArgs(keyInfo.KeyCode, _isControlDown));
                        break;
                    
                    case (int) WM.KEYUP:
                        switch (keyInfo.KeyCode)
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
            return NativeLibrary.User32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        public event EventHandler<KeyboardEventArgs> KeyDown;
        private void OnKeyDown(KeyboardEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }
    }
}