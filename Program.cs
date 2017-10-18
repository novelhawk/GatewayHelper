using System;
using System.Windows.Forms;

namespace ConnectionSwitcher
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            new Core();
            Application.Run();
        }
    }
}
