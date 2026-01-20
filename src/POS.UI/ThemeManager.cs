using System.Drawing;
using System.Windows.Forms;

namespace POS.UI
{
    public class ThemeManager
    {
        private static ThemeManager? _instance;
        public static ThemeManager Instance => _instance ??= new ThemeManager();

        public Color PrimaryColor { get; set; } = Color.FromArgb(45, 45, 48); // Dark Grey
        public Color SecondaryColor { get; set; } = Color.FromArgb(0, 122, 204); // Blue Accent
        public Color BackgroundColor { get; set; } = Color.FromArgb(30, 30, 30); // Darker Grey
        public Color TextColor { get; set; } = Color.White;
        public Color ButtonHoverColor { get; set; } = Color.FromArgb(60, 60, 60);

        public Font HeaderFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font BodyFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font ButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);

        private ThemeManager()
        {
            // Load theme from config if needed
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
