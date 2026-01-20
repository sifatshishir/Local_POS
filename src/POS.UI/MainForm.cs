using System;
using System.Windows.Forms;

namespace POS.UI
{
    /// <summary>
    /// Main application form for the Cashier POS system.
    /// Provides navigation to different sections: New Order, Order Queue, Reports.
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Text = "Restaurant POS - Cashier";
            this.WindowState = FormWindowState.Maximized;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // TODO: Initialize services and load initial data
        }
    }
}
