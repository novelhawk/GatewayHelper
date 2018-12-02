using System;

namespace ConnectionSwitcher.NativeLibraries
{
    public interface IKernel32
    {
        IntPtr GetModuleHandleA(string lpModuleName);
    }
}