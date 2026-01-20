using System;
using System.Windows.Forms;

namespace POS.UI
{
    /// <summary>
    /// Main entry point for the Cashier POS application.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}