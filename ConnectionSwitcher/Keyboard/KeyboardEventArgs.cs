using ConnectionSwitcher.NativeLibraries;

namespace ConnectionSwitcher.Keyboard
{
    public class KeyboardEventArgs
    {
        public bool ControlDown { get; }
        public Keys Key { get; }

        public KeyboardEventArgs(Keys key, bool controlDown)
        {
            ControlDown = controlDown;
            Key = key;
        }
    }
}