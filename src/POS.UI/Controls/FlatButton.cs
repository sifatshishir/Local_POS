using System;
using System.Drawing;
using System.Windows.Forms;

namespace POS.UI.Controls
{
    public class FlatButton : Button
    {
        public FlatButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Cursor = Cursors.Hand;
            
            // Initial styling from ThemeManager
            ApplyTheme();
        }

        public void ApplyTheme()
        {
            var theme = ThemeManager.Instance;
            this.BackColor = theme.PrimaryColor;
            this.ForeColor = theme.TextColor;
            this.Font = theme.ButtonFont;
            this.FlatAppearance.MouseOverBackColor = theme.ButtonHoverColor;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            // Custom painting can be added here if needed
        }
    }
}
