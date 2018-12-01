using System;
using System.Runtime.InteropServices;
// ReSharper disable All

namespace ConnectionSwitcher
{
    /// <summary>
    /// Native functions from user32.dll
    /// </summary>
    public interface IUser32
    {
        int GetMessageA(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
        bool TranslateMessage(ref MSG lpMsg);
        IntPtr DispatchMessageA(ref MSG lpMsg);
        bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers identifiers, Keys vk);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public UIntPtr wParam;
        public IntPtr lParam;
        public int time;
        public POINT pt;
#if _MAC
        public int lPrivate;
#endif
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
    
    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }

    public enum Keys
    {
        KeyCode = 65535, 
        Modifiers = -65536, 
        None = 0,
        LButton = 1,
        RButton = 2,
        Cancel = 3, 
        MButton = 4,
        XButton1 = 5, 
        XButton2 = 6, 
        Back = 8,
        Tab = 9, 
        LineFeed = 10, 
        Clear = 12, 
        Return = 13, 
        Enter = Return, 
        ShiftKey = 16, 
        ControlKey = 17, 
        Menu = 18, 
        Pause = 19, 
        Capital = 20, 
        CapsLock = Capital, 
        KanaMode = 21, 
        HanguelMode = KanaMode, 
        HangulMode = HanguelMode, 
        JunjaMode = 23, 
        FinalMode = 24, 
        HanjaMode = 25, 
        KanjiMode = HanjaMode, 
        Escape = 27, 
        IMEConvert = 28, 
        IMENonconvert = 29, 
        IMEAccept = 30, 
        IMEAceept = IMEAccept, 
        IMEModeChange = 31, 
        Space = 32, 
        Prior = 33, 
        PageUp = Prior, 
        Next = 34, 
        PageDown = Next, 
        End = 35, 
        Home = 36, 
        Left = 37, 
        Up = 38, 
        Right = 39, 
        Down = 40, 
        Select = 41, 
        Print = 42, 
        Execute = 43, 
        Snapshot = 44, 
        PrintScreen = Snapshot, 
        Insert = 45, 
        Delete = 46, 
        Help = 47, 
        D0 = 48, 
        D1 = 49, 
        D2 = 50, 
        D3 = 51, 
        D4 = 52, 
        D5 = 53, 
        D6 = 54, 
        D7 = 55, 
        D8 = 56, 
        D9 = 57, 
        A = 65, 
        B = 66, 
        C = 67, 
        D = 68, 
        E = 69, 
        F = 70, 
        G = 71, 
        H = 72, 
        I = 73, 
        J = 74, 
        K = 75, 
        L = 76, 
        M = 77, 
        N = 78, 
        O = 79, 
        P = 80, 
        Q = 81, 
        R = 82, 
        S = 83, 
        T = 84, 
        U = 85, 
        V = 86, 
        W = 87, 
        X = 88, 
        Y = 89, 
        Z = 90, 
        LWin = 91, 
        RWin = 92, 
        Apps = 93, 
        Sleep = 95, 
        NumPad0 = 96, 
        NumPad1 = 97, 
        NumPad2 = 98, 
        NumPad3 = 99, 
        NumPad4 = 100, 
        NumPad5 = 101, 
        NumPad6 = 102, 
        NumPad7 = 103, 
        NumPad8 = 104, 
        NumPad9 = 105, 
        Multiply = 106, 
        Add = 107, 
        Separator = 108, 
        Subtract = 109, 
        Decimal = 110, 
        Divide = 111, 
        F1 = 112, 
        F2 = 113, 
        F3 = 114, 
        F4 = 115, 
        F5 = 116, 
        F6 = 117, 
        F7 = 118, 
        F8 = 119, 
        F9 = 120, 
        F10 = 121, 
        F11 = 122, 
        F12 = 123, 
        F13 = 124, 
        F14 = 125, 
        F15 = 126, 
        F16 = 127, 
        F17 = 128, 
        F18 = 129, 
        F19 = 130, 
        F20 = 131, 
        F21 = 132, 
        F22 = 133, 
        F23 = 134, 
        F24 = 135, 
        NumLock = 144, 
        Scroll = 145, 
        LShiftKey = 160, 
        RShiftKey = 161, 
        LControlKey = 162, 
        RControlKey = 163, 
        LMenu = 164, 
        RMenu = 165, 
        BrowserBack = 166, 
        BrowserForward = 167, 
        BrowserRefresh = 168, 
        BrowserStop = 169, 
        BrowserSearch = 170, 
        BrowserFavorites = 171, 
        BrowserHome = 172, 
        VolumeMute = 173, 
        VolumeDown = 174, 
        VolumeUp = 175, 
        MediaNextTrack = 176, 
        MediaPreviousTrack = 177, 
        MediaStop = 178, 
        MediaPlayPause = 179, 
        LaunchMail = 180, 
        SelectMedia = 181, 
        LaunchApplication1 = 182, 
        LaunchApplication2 = 183, 
        OemSemicolon = 186, 
        Oem1 = OemSemicolon, 
        Oemplus = 187, 
        Oemcomma = 188, 
        OemMinus = 189, 
        OemPeriod = 190, 
        OemQuestion = 191, 
        Oem2 = OemQuestion, 
        Oemtilde = 192, 
        Oem3 = Oemtilde, 
        OemOpenBrackets = Oem3 | Escape, 
        Oem4 = OemOpenBrackets, 
        OemPipe = Oem3 | IMEConvert, 
        Oem5 = OemPipe, 
        OemCloseBrackets = 221, 
        Oem6 = OemCloseBrackets, 
        OemQuotes = 222, 
        Oem7 = OemQuotes, 
        Oem8 = 223, 
        OemBackslash = 226, 
        Oem102 = OemBackslash, 
        ProcessKey = 229, 
        Packet = 231, 
        Attn = 246, 
        Crsel = 247, 
        Exsel = 248, 
        EraseEof = 249, 
        Play = 250, 
        Zoom = 251, 
        NoName = 252, 
        Pa1 = 253, 
        OemClear = 254, 
        Shift = 65536, 
        Control = 131072, 
        Alt = 262144, 
    }
}