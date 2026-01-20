using System;
using System.Windows.Forms;

namespace POS.KitchenDisplay
{
    /// <summary>
    /// Main entry point for the Kitchen Display application.
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
            Application.Run(new KitchenDisplayForm());
        }
    }
}