using System;
using System.Drawing;
using System.Windows.Forms;
using POS.Bridge;
using POS.Bridge.DataTransferObjects;

namespace POS.UI.Controls
{
    public partial class OrderQueueControl : UserControl
    {
        private OrderServiceWrapper _orderService;

        public OrderQueueControl()
        {
            InitializeComponent();
            _orderService = new OrderServiceWrapper();
            ThemeManager.Instance.ThemeChanged += Instance_ThemeChanged; // Subscribe
        }

        private void Instance_ThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
            LoadOrders(); // Reload to apply row styles
        }

        private void OrderQueueControl_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            SetupDataGridView();
            LoadOrders();
            refreshTimer.Start();
        }

        private void ApplyTheme()
        {
            var theme = ThemeManager.Instance;
            this.BackColor = theme.BackgroundColor;
            pnlTop.BackColor = theme.BackgroundColor;
            lblTitle.ForeColor = theme.TextColor;
            
            dgvOrders.BackgroundColor = theme.BackgroundColor;
            dgvOrders.ForeColor = theme.TextColor;
            dgvOrders.GridColor = theme.BorderColor;
            
            // Header Style
            dgvOrders.EnableHeadersVisualStyles = false; // Required for custom header color
            dgvOrders.ColumnHeadersDefaultCellStyle.BackColor = theme.PrimaryColor;
            dgvOrders.ColumnHeadersDefaultCellStyle.ForeColor = theme.TextColor;
            dgvOrders.ColumnHeadersDefaultCellStyle.SelectionBackColor = theme.PrimaryColor; // Keep same
            dgvOrders.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            
            // Row Style
            dgvOrders.DefaultCellStyle.BackColor = theme.IsDarkMode ? Color.FromArgb(40, 40, 40) : Color.WhiteSmoke;
            dgvOrders.DefaultCellStyle.ForeColor = theme.TextColor;
            dgvOrders.DefaultCellStyle.SelectionBackColor = theme.SecondaryColor;
            dgvOrders.DefaultCellStyle.SelectionForeColor = theme.TextColor;
            dgvOrders.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvOrders.RowTemplate.Height = 40;
            
            // Button
            if (btnRefresh != null)
            {
                btnRefresh.BackColor = theme.PrimaryColor;
                btnRefresh.ForeColor = theme.TextColor;
            }
        }

        private void SetupDataGridView()
        {
            dgvOrders.Columns.Clear();
            dgvOrders.Columns.Add("OrderID", "Order #");
            dgvOrders.Columns.Add("Type", "Type");
            dgvOrders.Columns.Add("Table", "Table");
            dgvOrders.Columns.Add("Status", "Status");
            dgvOrders.Columns.Add("Time", "Time");
            dgvOrders.Columns.Add("Total", "Total");
            
            dgvOrders.Columns["OrderID"].Width = 80;
            dgvOrders.Columns["Type"].Width = 120;
            dgvOrders.Columns["Table"].Width = 80;
            dgvOrders.Columns["Status"].Width = 120;
            dgvOrders.Columns["Time"].Width = 150;
            dgvOrders.Columns["Total"].Width = 100;
        }

        private void LoadOrders()
        {
            try
            {
                // Suspend layout to prevent flickering/overlap during update
                dgvOrders.SuspendLayout(); 
                dgvOrders.Rows.Clear();
                
                // Get orders with different statuses
                var orderedList = _orderService.GetOrdersByStatus(OrderStatusDTO.Ordered);
                var processingList = _orderService.GetOrdersByStatus(OrderStatusDTO.Processing);
                var doneList = _orderService.GetOrdersByStatus(OrderStatusDTO.Done);
                
                // Add Ordered orders
                foreach (var order in orderedList) AddOrderRow(order, OrderStatusDTO.Ordered);
                
                // Add Processing orders
                foreach (var order in processingList) AddOrderRow(order, OrderStatusDTO.Processing);
                
                // Add Done orders (last 10 only)
                int doneCount = 0;
                foreach (var order in doneList)
                {
                    if (doneCount >= 10) break;
                    AddOrderRow(order, OrderStatusDTO.Done);
                    doneCount++;
                }
            }
            catch (Exception ex)
            {
                // Suppress timer errors to avoid spam, but log/show on manual refresh?
                // For now, simple logging/swallowing on timer, showing on manual?
                // Just write to debug or status bar.
                System.Diagnostics.Debug.WriteLine($"Error loading orders: {ex.Message}");
            }
            finally
            {
                dgvOrders.ResumeLayout();
            }
        }

        private void AddOrderRow(OrderDTO order, OrderStatusDTO status)
        {
             // ... logic same ...
            int rowIndex = dgvOrders.Rows.Add();
            var row = dgvOrders.Rows[rowIndex];
            
            row.Cells["OrderID"].Value = order.Id;
            row.Cells["Type"].Value = order.Type.ToString();
            row.Cells["Table"].Value = order.TableNumber.ToString() ?? "-";
            row.Cells["Status"].Value = status.ToString();
            row.Cells["Time"].Value = DateTime.Now.ToString("HH:mm:ss"); 
            row.Cells["Total"].Value = $"${order.TotalAmount:F2}";
            
            // Color code by status (keep text color, but adapt background?)
            Color statusColor;
            switch (status)
            {
                case OrderStatusDTO.Ordered: statusColor = Color.Gold; break;
                case OrderStatusDTO.Processing: statusColor = Color.DodgerBlue; break;
                case OrderStatusDTO.Done: statusColor = Color.LimeGreen; break;
                default: statusColor = Color.Gray; break;
            }
            
            // For light mode, status text needs to be darker or background lighter.
            // Current approach: Darken background tint.
            // But if Theme is Light (White), tinting with RGB/3 makes it very dark.
            
            if (ThemeManager.Instance.IsDarkMode)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(30, statusColor.R / 3, statusColor.G / 3, statusColor.B / 3);
            }
            else
            {
                 // Light Mode: Use a light tint of the status color
                 // e.g. statusColor mixed with White
                 row.DefaultCellStyle.BackColor = ControlPaint.Light(statusColor, 1.5f);
            }
            
            // Ensure text is readable
            row.Cells["Status"].Style.ForeColor = statusColor;
            if (!ThemeManager.Instance.IsDarkMode && (statusColor == Color.Gold || statusColor == Color.LimeGreen))
            {
                 // Darken text for light mode
                 row.Cells["Status"].Style.ForeColor = ControlPaint.Dark(statusColor);
            }
            
            row.Cells["Status"].Style.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            LoadOrders();
        }
    }
}
