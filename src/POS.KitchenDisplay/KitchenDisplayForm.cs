using System;
using System.Windows.Forms;

namespace POS.KitchenDisplay
{
    /// <summary>
    /// Kitchen Display application form.
    /// Shows active orders with status management (Ordered -> Processing -> Done).
    /// Auto-refreshes to display new orders in real-time.
    /// </summary>
    public partial class KitchenDisplayForm : Form
    {
        public KitchenDisplayForm()
        {
            InitializeComponent();
            this.Text = "Restaurant POS - Kitchen Display";
            this.WindowState = FormWindowState.Maximized;
            // this.FormBorderStyle = FormBorderStyle.None; // Fullscreen for kitchen
        }

        private void KitchenDisplayForm_Load(object sender, EventArgs e)
        {
            // TODO: Load active orders and setup auto-refresh timer
        }
    }
}
