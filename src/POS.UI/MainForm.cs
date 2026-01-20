using System;
using System.Windows.Forms;
using POS.UI.Controls;

namespace POS.UI
{
    /// <summary>
    /// Main application form for the Cashier POS system.
    /// Provides navigation to different sections: New Order, Order Queue.
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
            ApplyTheme();
            // Default to New Order view
            ShowNewOrder();
        }

        private void ApplyTheme()
        {
            var theme = ThemeManager.Instance;
            
            // Main Container
            this.BackColor = theme.BackgroundColor;
            
            // Sidebar
            this.sidebarPanel.BackColor = theme.PrimaryColor; // Slightly different if needed, or keeping dark
            this.logoPanel.BackColor = theme.PrimaryColor;
            this.lblLogo.ForeColor = theme.TextColor;
            this.lblLogo.Font = theme.HeaderFont;

            // Content
            this.contentPanel.BackColor = theme.BackgroundColor;

            // Buttons are auto-themed but we can force refresh if needed
            // ThemeManager.Instance.ApplyTheme(this); // Optional specific override
        }

        private void btnNewOrder_Click(object sender, EventArgs e)
        {
            ShowNewOrder();
        }

        private void btnOrderQueue_Click(object sender, EventArgs e)
        {
            ShowOrderQueue();
        }

        private void ShowNewOrder()
        {
            contentPanel.Controls.Clear();
            var control = new POS.UI.Controls.NewOrderControl();
            control.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(control);
            SetActiveButton(btnNewOrder);
        }

        private void ShowOrderQueue()
        {
            // Placeholder for now
            contentPanel.Controls.Clear();
            Label lbl = new Label();
            lbl.Text = "Order Queue (Coming Soon)";
            lbl.ForeColor = Color.White;
            lbl.AutoSize = true;
            lbl.Location = new Point(20, 20);
            contentPanel.Controls.Add(lbl);
            
            SetActiveButton(btnOrderQueue);
        }

        private void LoadContent(string title)
        {
             // Deprecated
        }

        private void SetActiveButton(FlatButton activeBtn)
        {
            // Reset all sidebar buttons
            btnNewOrder.BackColor = ThemeManager.Instance.PrimaryColor;
            btnOrderQueue.BackColor = ThemeManager.Instance.PrimaryColor;

            // Highlight active
            activeBtn.BackColor = ThemeManager.Instance.SecondaryColor;
        }
    }
}
