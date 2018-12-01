using System;

namespace ConnectionSwitcher
{
    /// <summary>
    /// Native functions from user32
    /// </summary>
    public interface NativeFunctions
    {
        bool RegisterHotkeys(IntPtr hWnd, int id, uint identifiers, uint vk);
        
        
    }
}