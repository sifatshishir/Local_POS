using System.Drawing;
using System.Windows.Forms;

namespace POS.UI.Controls
{
    // Replicating OrderStatus enum here or referencing from Bridge
    // For now assuming we use integer or string, but optimally should reference common types.
    // However, since we don't have direct reference to Bridge types in UI plain code yet without the project ref, 
    // we will stick to basic implementation and refine later.
    public enum UIOrderStatus
    {
        Ordered = 0,
        Processing = 1,
        Done = 2
    }

    public class StatusLabel : Label
    {
        private UIOrderStatus _status;
        public UIOrderStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                UpdateStyle();
            }
        }

        public StatusLabel()
        {
            this.AutoSize = false;
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.Size = new Size(100, 25);
            this.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            UpdateStyle();
        }

        private void UpdateStyle()
        {
            switch (_status)
            {
                case UIOrderStatus.Ordered:
                    this.BackColor = Color.Gold;
                    this.ForeColor = Color.Black;
                    this.Text = "Ordered";
                    break;
                case UIOrderStatus.Processing:
                    this.BackColor = Color.DodgerBlue;
                    this.ForeColor = Color.White;
                    this.Text = "Processing";
                    break;
                case UIOrderStatus.Done:
                    this.BackColor = Color.LimeGreen;
                    this.ForeColor = Color.White;
                    this.Text = "Done";
                    break;
            }
        }
    }
}
