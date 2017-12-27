using System;

namespace ConnectionSwitcher
{
    [Flags]
    public enum Modifiers : uint
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        OSKey = 8,
        NoRepeat = 16384
    }
}
