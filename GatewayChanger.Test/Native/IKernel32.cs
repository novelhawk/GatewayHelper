using System;

namespace GatewayChanger.Test.Native
{
    public interface IKernel32
    {
        IntPtr GetModuleHandleA(string lpModuleName);
    }
}