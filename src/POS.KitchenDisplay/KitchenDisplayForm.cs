using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using POS.Bridge;
using POS.Bridge.DataTransferObjects;

namespace POS.KitchenDisplay
{
    public partial class KitchenDisplayForm : Form
    {
        private OrderServiceWrapper _orderService;
        private FlowLayoutPanel flowOrders;
        private System.Windows.Forms.Timer refreshTimer;
        private Label lblTitle;
        private Panel pnlTop;
        private Button btnRefresh;
        
        // Simple Theme State
        private bool _isDarkMode = true;
        private Color _bgColor = Color.FromArgb(30, 30, 30);
        private Color _cardColor = Color.FromArgb(45, 45, 48);
        private Color _textColor = Color.White;

        public KitchenDisplayForm()
        {
            InitializeComponent();
            _orderService = new OrderServiceWrapper();
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Restaurant POS - Kitchen Display";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = _bgColor;

            // Top Panel
            pnlTop = new Panel();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 60;
            pnlTop.Padding = new Padding(20);
            pnlTop.BackColor = _bgColor;

            lblTitle = new Label();
            lblTitle.Text = "KITCHEN DISPLAY SYSTEM";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = _textColor;
            lblTitle.AutoSize = true;
            lblTitle.Dock = DockStyle.Left;
            pnlTop.Controls.Add(lblTitle);
            
            // Manual Refresh / Theme Button (Right Aligned)
            btnRefresh = new Button();
            btnRefresh.Text = "Toggle Theme";
            btnRefresh.Dock = DockStyle.Right;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.ForeColor = _textColor;
            btnRefresh.AutoSize = true; 
            btnRefresh.Click += (s, e) => {
                _isDarkMode = !_isDarkMode;
                ApplyTheme();
                LoadOrders(); 
            };
            pnlTop.Controls.Add(btnRefresh);

            // Flow Panel
            flowOrders = new FlowLayoutPanel();
            flowOrders.Dock = DockStyle.Fill;
            flowOrders.AutoScroll = true;
            flowOrders.Padding = new Padding(10);
            flowOrders.BackColor = _bgColor;

            this.Controls.Add(flowOrders);
            this.Controls.Add(pnlTop);

            // Timer
            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Tick += RefreshTimer_Tick;
            
            // Read Env for Interval
            int interval = 5000;
            try {
                string envPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\.env");
                if (System.IO.File.Exists(envPath)) {
                    foreach (var line in System.IO.File.ReadAllLines(envPath)) {
                        if (line.StartsWith("REFRESH_INTERVAL_MS=")) {
                            if (int.TryParse(line.Split('=')[1].Trim(), out int val)) interval = val;
                        }
                    }
                }
            } catch {}
            
            refreshTimer.Interval = interval;
            refreshTimer.Start();
        }

        private void ApplyTheme()
        {
            if (_isDarkMode)
            {
                _bgColor = Color.FromArgb(30, 30, 30);
                _cardColor = Color.FromArgb(45, 45, 48);
                _textColor = Color.White;
            }
            else
            {
                _bgColor = Color.WhiteSmoke;
                _cardColor = Color.White;
                _textColor = Color.Black;
            }
            
            this.BackColor = _bgColor;
            pnlTop.BackColor = _bgColor;
            flowOrders.BackColor = _bgColor;
            lblTitle.ForeColor = _textColor;
            btnRefresh.ForeColor = _textColor;
        }

        private void KitchenDisplayForm_Load(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                if (flowOrders.InvokeRequired)
                {
                    flowOrders.Invoke(new Action(LoadOrders));
                    return;
                }

                // Fetch Ordered and Processing
                var ordered = _orderService.GetOrdersByStatus(OrderStatusDTO.Ordered);
                var processing = _orderService.GetOrdersByStatus(OrderStatusDTO.Processing);
                
                var all = new List<OrderDTO>();
                if (ordered != null) all.AddRange(ordered);
                if (processing != null) all.AddRange(processing);
                
                all.Sort((a, b) => a.Id.CompareTo(b.Id));
                
                RenderOrders(all);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading orders: {ex.Message}");
            }
        }

        private void RenderOrders(List<OrderDTO> orders)
        {
            flowOrders.SuspendLayout();
            flowOrders.Controls.Clear();

            if (orders.Count == 0)
            {
                var lbl = new Label();
                lbl.Text = "No active orders";
                lbl.Font = new Font("Segoe UI", 16);
                lbl.ForeColor = Color.Gray;
                lbl.AutoSize = true;
                lbl.Margin = new Padding(20);
                flowOrders.Controls.Add(lbl);
            }
            else
            {
                foreach (var order in orders)
                {
                    flowOrders.Controls.Add(CreateOrderCard(order));
                }
            }

            flowOrders.ResumeLayout();
        }

        private Control CreateOrderCard(OrderDTO order)
        {
            var card = new Panel();
            card.Size = new Size(300, 420); // Taller for items
            card.Margin = new Padding(15);
            card.BackColor = _cardColor;
            
            // 1. Status Strip (Color Bar)
            Color statusColor = (order.Status == OrderStatusDTO.Ordered) ? Color.Gold : Color.DodgerBlue;
            Panel pnlStatus = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = statusColor };
            card.Controls.Add(pnlStatus);

            // 2. Header
            var lblHeader = new Label();
            string tableTxt = order.TableNumber > 0 ? $"T-{order.TableNumber}" : "Parcel";
            lblHeader.Text = $"#{order.Id} | {tableTxt}";
            lblHeader.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblHeader.ForeColor = _textColor;
            lblHeader.Dock = DockStyle.Top;
            lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            lblHeader.Height = 50;
            card.Controls.Add(lblHeader);

            // 3. Action Button (Bottom)
            var btnAction = new Button();
            btnAction.Dock = DockStyle.Bottom;
            btnAction.Height = 50;
            btnAction.FlatStyle = FlatStyle.Flat;
            btnAction.Cursor = Cursors.Hand;
            btnAction.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            if (order.Status == OrderStatusDTO.Ordered)
            {
                btnAction.Text = "START";
                btnAction.BackColor = Color.Orange;
                btnAction.ForeColor = Color.White;
                btnAction.Click += (s, e) => UpdateStatus(order.Id, OrderStatusDTO.Processing);
            }
            else
            {
                btnAction.Text = "READY";
                btnAction.BackColor = Color.SeaGreen;
                btnAction.ForeColor = Color.White;
                btnAction.Click += (s, e) => UpdateStatus(order.Id, OrderStatusDTO.Done);
            }
            card.Controls.Add(btnAction);

            // 4. Items List (Fill - Scrollable)
            var pnlItems = new Panel();
            pnlItems.Dock = DockStyle.Fill;
            pnlItems.AutoScroll = true; // Fixes cut-off
            pnlItems.Padding = new Padding(10);
            
            var lblItems = new Label();
            lblItems.ForeColor = _textColor;
            lblItems.Font = new Font("Consolas", 12);
            lblItems.AutoSize = true; // Grows with text, forcing scroll on parent panel
             
            string msg = "";
            if (order.Items != null)
            {
                foreach(var item in order.Items)
                {
                    msg += $"{item.Quantity} x {item.MenuName}\n";
                }
            }
            lblItems.Text = msg;
            
            pnlItems.Controls.Add(lblItems);
            card.Controls.Add(pnlItems);
            
            // DOCKING ORDER FIX
            // To ensure correct layout:
            // Top (Status) -> Top (Header) -> Bottom (Button) -> Fill (Items)
            // Reverse Z-Order:
            // 1. Status (SendToBack)
            // 2. Header (SendToBack)
            // 3. Button (SendToBack)
            // 4. Items (BringToFront)
            
            pnlStatus.SendToBack();
            lblHeader.SendToBack();  // Will be under Status
            btnAction.SendToBack();  // Will be at Bottom
            pnlItems.BringToFront(); // Fills the middle
            
            return card;
        }

        private void UpdateStatus(long orderId, OrderStatusDTO newStatus)
        {
            try
            {
                _orderService.UpdateOrderStatus((int)orderId, newStatus);
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            KitchenDisplayForm_Load(this, e);
        }
    }
}
