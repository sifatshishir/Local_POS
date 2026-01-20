using System;
using System.Drawing;
using System.Windows.Forms;
using POS.Bridge;
using POS.Bridge.DataTransferObjects;
using System.Collections.Generic;

namespace POS.UI.Controls
{
    public partial class OrderQueueControl : UserControl
    {
        private OrderServiceWrapper _orderService;
        private int _currentPage = 1;
        private const int PAGE_SIZE = 15;
        private int _totalPages = 1;

        public OrderQueueControl()
        {
            InitializeComponent();
            _orderService = new OrderServiceWrapper();
            ThemeManager.Instance.ThemeChanged += Instance_ThemeChanged;
            
            // Enable Double Buffering on both DataGridViews
            EnableDoubleBuffering(dgvActive);
            EnableDoubleBuffering(dgvHistory);
            
            // Initialize timer (interval will be set from .env in Load event)
            refreshTimer.Tick += refreshTimer_Tick;
            
            // Handle visibility changes to restart timer
            this.VisibleChanged += OrderQueueControl_VisibleChanged;
        }

        private void OrderQueueControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                // Restart timer when control becomes visible
                LoadCurrentTabData();
                refreshTimer.Start();
            }
            else
            {
                // Stop timer when hidden to save resources
                refreshTimer.Stop();
            }
        }

        private void EnableDoubleBuffering(DataGridView dgv)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.SetProperty, 
                null, dgv, new object[] { true });
        }

        private void Instance_ThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
            LoadCurrentTabData();
        }

        private void OrderQueueControl_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            SetupDataGridViews();
            
            // Read refresh interval from .env
            int interval = 5000;
            try
            {
                string envPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\.env");
                if (System.IO.File.Exists(envPath))
                {
                    foreach (var line in System.IO.File.ReadAllLines(envPath))
                    {
                        if (line.StartsWith("REFRESH_INTERVAL_MS="))
                        {
                            if (int.TryParse(line.Split('=')[1].Trim(), out int val)) interval = val;
                        }
                    }
                }
            }
            catch {}

            refreshTimer.Interval = interval;
            
            // Load initial data and start timer
            LoadCurrentTabData();
            refreshTimer.Start();
        }

        private void ApplyTheme()
        {
            var theme = ThemeManager.Instance;
            this.BackColor = theme.BackgroundColor;
            pnlTop.BackColor = theme.BackgroundColor;
            lblTitle.ForeColor = theme.TextColor;
            
            // Tab Control
            tabControl.BackColor = theme.BackgroundColor;
            tabActive.BackColor = theme.BackgroundColor;
            tabHistory.BackColor = theme.BackgroundColor;
            pnlPagination.BackColor = theme.BackgroundColor;
            lblPageInfo.ForeColor = theme.TextColor;
            
            ApplyGridTheme(dgvActive);
            ApplyGridTheme(dgvHistory);
            
            // Buttons
            btnRefresh.BackColor = theme.PrimaryColor;
            btnRefresh.ForeColor = theme.TextColor;
            btnPrevPage.BackColor = theme.PrimaryColor;
            btnPrevPage.ForeColor = theme.TextColor;
            btnNextPage.BackColor = theme.PrimaryColor;
            btnNextPage.ForeColor = theme.TextColor;
        }

        private void ApplyGridTheme(DataGridView dgv)
        {
            var theme = ThemeManager.Instance;
            dgv.BackgroundColor = theme.BackgroundColor;
            dgv.ForeColor = theme.TextColor;
            dgv.GridColor = theme.BorderColor;
            
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = theme.PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = theme.TextColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = theme.PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            
            dgv.DefaultCellStyle.BackColor = theme.IsDarkMode ? Color.FromArgb(40, 40, 40) : Color.WhiteSmoke;
            dgv.DefaultCellStyle.ForeColor = theme.TextColor;
            dgv.DefaultCellStyle.SelectionBackColor = theme.SecondaryColor;
            dgv.DefaultCellStyle.SelectionForeColor = theme.TextColor;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgv.RowTemplate.Height = 40;
        }

        private void SetupDataGridViews()
        {
            SetupGrid(dgvActive);
            SetupGrid(dgvHistory);
        }

        private void SetupGrid(DataGridView dgv)
        {
            dgv.Columns.Clear();
            dgv.Columns.Add("OrderID", "Order #");
            dgv.Columns.Add("Type", "Type");
            dgv.Columns.Add("Table", "Table");
            dgv.Columns.Add("Status", "Status");
            dgv.Columns.Add("Time", "Time");
            dgv.Columns.Add("Total", "Total");
            
            dgv.Columns["OrderID"].Width = 80;
            dgv.Columns["Type"].Width = 120;
            dgv.Columns["Table"].Width = 80;
            dgv.Columns["Status"].Width = 120;
            dgv.Columns["Time"].Width = 150;
            dgv.Columns["Total"].Width = 100;
        }

        private void LoadCurrentTabData()
        {
            if (tabControl.SelectedTab == tabActive)
            {
                LoadActiveOrders();
            }
            else if (tabControl.SelectedTab == tabHistory)
            {
                LoadHistoryOrders();
            }
        }

        private void LoadActiveOrders()
        {
            try
            {
                if (dgvActive.InvokeRequired)
                {
                    dgvActive.Invoke(new Action(LoadActiveOrders));
                    return;
                }

                var orderedList = _orderService.GetOrdersByStatus(OrderStatusDTO.Ordered);
                var processingList = _orderService.GetOrdersByStatus(OrderStatusDTO.Processing);

                var allOrders = new List<KeyValuePair<OrderDTO, OrderStatusDTO>>();
                if (orderedList != null) foreach (var o in orderedList) allOrders.Add(new KeyValuePair<OrderDTO, OrderStatusDTO>(o, OrderStatusDTO.Ordered));
                if (processingList != null) foreach (var o in processingList) allOrders.Add(new KeyValuePair<OrderDTO, OrderStatusDTO>(o, OrderStatusDTO.Processing));

                // Diff Update with Dictionary
                var rowMap = new Dictionary<long, DataGridViewRow>();
                
                // Only build map if there are existing rows
                if (dgvActive.Rows.Count > 0)
                {
                    foreach (DataGridViewRow r in dgvActive.Rows)
                    {
                        try
                        {
                            if (r.Cells["OrderID"].Value != null)
                            {
                                long orderId = Convert.ToInt64(r.Cells["OrderID"].Value);
                                rowMap[orderId] = r;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error building row map:\nCell Value: {r.Cells["OrderID"].Value}\nType: {r.Cells["OrderID"].Value?.GetType().Name}\nError: {ex.Message}", "Cast Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            throw;
                        }
                    }
                }

                var activeIds = new HashSet<long>();
                foreach(var pair in allOrders) activeIds.Add(pair.Key.Id);

                // Remove stale rows
                for (int i = dgvActive.Rows.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        if (dgvActive.Rows[i].Cells["OrderID"].Value != null)
                        {
                            long rowId = Convert.ToInt64(dgvActive.Rows[i].Cells["OrderID"].Value);
                            if (!activeIds.Contains(rowId))
                            {
                                dgvActive.Rows.RemoveAt(i);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error removing stale row at index {i}:\nCell Value: {dgvActive.Rows[i].Cells["OrderID"].Value}\nType: {dgvActive.Rows[i].Cells["OrderID"].Value?.GetType().Name}\nError: {ex.Message}", "Cast Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
                    }
                }

                // Update or add rows
                foreach (var pair in allOrders)
                {
                    if (rowMap.TryGetValue(pair.Key.Id, out DataGridViewRow existingRow))
                    {
                        UpdateRowData(existingRow, pair.Key, pair.Value);
                    }
                    else
                    {
                        int idx = dgvActive.Rows.Add();
                        UpdateRowData(dgvActive.Rows[idx], pair.Key, pair.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading active orders: {ex.Message}\n\nStack: {ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Error loading active orders: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void LoadHistoryOrders()
        {
            try
            {
                if (dgvHistory.InvokeRequired)
                {
                    dgvHistory.Invoke(new Action(LoadHistoryOrders));
                    return;
                }

                // Get total count for pagination
                int totalCount = _orderService.GetOrderCountByStatus(OrderStatusDTO.Done);
                _totalPages = (int)Math.Ceiling((double)totalCount / PAGE_SIZE);
                if (_totalPages < 1) _totalPages = 1;

                // Ensure current page is valid
                if (_currentPage > _totalPages) _currentPage = _totalPages;
                if (_currentPage < 1) _currentPage = 1;

                // Fetch paginated data
                var doneOrders = _orderService.GetOrdersByStatusPaginated(OrderStatusDTO.Done, _currentPage, PAGE_SIZE);

                dgvHistory.Rows.Clear();
                if (doneOrders != null)
                {
                    foreach (var order in doneOrders)
                    {
                        int idx = dgvHistory.Rows.Add();
                        UpdateRowData(dgvHistory.Rows[idx], order, OrderStatusDTO.Done);
                    }
                }

                // Update pagination UI
                UpdatePaginationControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading history orders: {ex.Message}\n\nStack: {ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Error loading history orders: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void UpdatePaginationControls()
        {
            lblPageInfo.Text = $"Page {_currentPage} of {_totalPages}";
            btnPrevPage.Enabled = _currentPage > 1;
            btnNextPage.Enabled = _currentPage < _totalPages;
        }

        private void UpdateRowData(DataGridViewRow row, OrderDTO order, OrderStatusDTO status)
        {
            row.Cells["OrderID"].Value = order.Id;
            row.Cells["Type"].Value = order.Type.ToString();
            row.Cells["Table"].Value = order.TableNumber.ToString() ?? "-";
            row.Cells["Status"].Value = status.ToString();
            row.Cells["Time"].Value = DateTime.Now.ToString("HH:mm:ss");
            row.Cells["Total"].Value = $"${order.TotalAmount:F2}";

            Color statusColor;
            switch (status)
            {
                case OrderStatusDTO.Ordered: statusColor = Color.Gold; break;
                case OrderStatusDTO.Processing: statusColor = Color.DodgerBlue; break;
                case OrderStatusDTO.Done: statusColor = Color.LimeGreen; break;
                default: statusColor = Color.Gray; break;
            }

            var theme = ThemeManager.Instance;
            if (theme.IsDarkMode)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(30, statusColor.R / 3, statusColor.G / 3, statusColor.B / 3);
            }
            else
            {
                 row.DefaultCellStyle.BackColor = ControlPaint.Light(statusColor, 1.5f);
            }
            
            row.Cells["Status"].Style.ForeColor = statusColor;
            if (!theme.IsDarkMode && (statusColor == Color.Gold || statusColor == Color.LimeGreen))
            {
                 row.Cells["Status"].Style.ForeColor = ControlPaint.Dark(statusColor);
            }
            row.Cells["Status"].Style.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCurrentTabData();
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            // Auto-refresh the currently selected tab
            LoadCurrentTabData();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCurrentTabData();
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadHistoryOrders();
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                LoadHistoryOrders();
            }
        }
    }
}
