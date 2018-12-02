using System;

namespace ConnectionSwitcher
{
    public interface IKernel32
    {
        IntPtr GetModuleHandleA(string lpModuleName);
    }
}