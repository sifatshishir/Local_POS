using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace POS.UI
{
    public enum ThemeMode
    {
        Light,
        Dark,
        Auto
    }

    public class ThemeManager
    {
        private static ThemeManager? _instance;
        public static ThemeManager Instance => _instance ??= new ThemeManager();

        public ThemeMode CurrentMode { get; private set; } = ThemeMode.Dark;
        public bool IsDarkMode { get; private set; } = true;

        public event EventHandler? ThemeChanged;

        public Color PrimaryColor { get; private set; }
        public Color SecondaryColor { get; private set; }
        public Color BackgroundColor { get; private set; }
        public Color TextColor { get; private set; }
        public Color ButtonHoverColor { get; private set; }
        public Color BorderColor { get; private set; }

        public Font HeaderFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font BodyFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font ButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);

        private ThemeManager()
        {
            SetTheme(ThemeMode.Dark); // Default
        }

        public void SetTheme(ThemeMode mode)
        {
            CurrentMode = mode;
            IsDarkMode = false;

            if (mode == ThemeMode.Auto)
            {
                IsDarkMode = IsSystemDarkTheme();
            }
            else
            {
                IsDarkMode = (mode == ThemeMode.Dark);
            }

            if (IsDarkMode)
            {
                // Dark Theme
                PrimaryColor = Color.FromArgb(45, 45, 48);
                SecondaryColor = Color.FromArgb(0, 122, 204);
                BackgroundColor = Color.FromArgb(30, 30, 30);
                TextColor = Color.White;
                ButtonHoverColor = Color.FromArgb(60, 60, 60);
                BorderColor = Color.FromArgb(80, 80, 80);
            }
            else
            {
                // Light Theme - Calmer Colors
                PrimaryColor = Color.FromArgb(245, 245, 245); // WhiteSmoke
                SecondaryColor = Color.FromArgb(0, 120, 215);
                BackgroundColor = Color.FromArgb(250, 250, 250); // Very light grey, not pure white
                TextColor = Color.FromArgb(30, 30, 30); // Soft black
                ButtonHoverColor = Color.FromArgb(225, 225, 225); // Light Gray
                BorderColor = Color.FromArgb(210, 210, 210);
            }

            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool IsSystemDarkTheme()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        object? val = key.GetValue("AppsUseLightTheme");
                        if (val is int i)
                        {
                            return i == 0;
                        }
                    }
                }
            }
            catch { }
            return false; 
        }

        public void ApplyTheme(Control control)
        {
            if (control is Form form)
            {
                form.BackColor = BackgroundColor;
                form.ForeColor = TextColor;
            }
            else if (control is Button button)
            {
                button.BackColor = PrimaryColor;
                button.ForeColor = TextColor;
                button.Font = ButtonFont;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
            }
            else if (control is Label label)
            {
                label.ForeColor = TextColor;
                label.Font = BodyFont;
            }
            else if (control is Panel panel)
            {
                panel.BackColor = BackgroundColor;
                panel.ForeColor = TextColor;
            }

            foreach (Control child in control.Controls)
            {
                ApplyTheme(child);
            }
        }
    }
}
