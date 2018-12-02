using System;

namespace ConnectionSwitcher
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var core = new Core();
            core.ApplicationLoop();
            Console.ReadKey();
        }
    }
}