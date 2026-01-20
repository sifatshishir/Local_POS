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
        private FlatButton btnTheme;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "Restaurant POS - Cashier";
            this.WindowState = FormWindowState.Maximized;
            
            InitializeThemeButton();
            ThemeManager.Instance.ThemeChanged += Instance_ThemeChanged;
        }

        private void InitializeThemeButton()
        {
            btnTheme = new FlatButton();
            btnTheme.Text = "Theme: Dark";
            btnTheme.Dock = DockStyle.Bottom;
            btnTheme.Height = 50;
            btnTheme.Cursor = Cursors.Hand;
            btnTheme.Click += BtnTheme_Click;
            
            // Add to sidebar
            sidebarPanel.Controls.Add(btnTheme);
        }

        private void Instance_ThemeChanged(object? sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void BtnTheme_Click(object? sender, EventArgs e)
        {
            var current = ThemeManager.Instance.CurrentMode;
            ThemeMode next = ThemeMode.Light;

            switch (current)
            {
                case ThemeMode.Dark: next = ThemeMode.Auto; break;
                case ThemeMode.Auto: next = ThemeMode.Light; break;
                case ThemeMode.Light: next = ThemeMode.Dark; break;
            }

            ThemeManager.Instance.SetTheme(next);
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
            this.sidebarPanel.BackColor = theme.PrimaryColor; 
            this.logoPanel.BackColor = theme.PrimaryColor;
            this.lblLogo.ForeColor = theme.TextColor;
            this.lblLogo.Font = theme.HeaderFont;

            // Content
            this.contentPanel.BackColor = theme.BackgroundColor;

            // Update Theme Button Text
            if (btnTheme != null)
            {
                btnTheme.Text = $"Theme: {theme.CurrentMode}";
                btnTheme.BackColor = theme.ButtonHoverColor; // Stand out slightly or same?
                btnTheme.ForeColor = theme.TextColor;
            }
            
            // Re-apply to sidebar buttons manually if needed
            btnNewOrder.BackColor = theme.PrimaryColor;
            btnNewOrder.ForeColor = theme.TextColor;
            
            btnOrderQueue.BackColor = theme.PrimaryColor;
            btnOrderQueue.ForeColor = theme.TextColor;
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
            contentPanel.Controls.Clear();
            var control = new POS.UI.Controls.OrderQueueControl();
            control.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(control);
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
