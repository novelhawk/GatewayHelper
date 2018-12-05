using System;
using GatewayChanger.Test.Native.Enums;
using GatewayChanger.Test.Native.Structures;

namespace GatewayChanger.Test.Native
{
    public interface IUser32
    {
        int GetMessageA(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
        bool TranslateMessage(ref MSG lpMsg);
        IntPtr DispatchMessageA(ref MSG lpMsg);
        
        IntPtr SetWindowsHookExA(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);
        IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    }
    
    public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);
}