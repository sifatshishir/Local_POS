using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using POS.Bridge;
using POS.Bridge.DataTransferObjects;
using POS.UI.Services;
using System.IO;

namespace POS.UI.Forms
{
    public partial class CheckoutDialog : Form
    {
        private readonly List<OrderItemDTO> _orderItems;
        private readonly OrderTypeDTO _orderType;
        private readonly int _tableNumber;
        private double _subtotal;
        private double _tax;
        private double _total;

        // Dynamic Controls for Parcel Provider
        private Label lblProvider;
        private ComboBox cmbProvider;

        public CheckoutDialog(List<OrderItemDTO> orderItems, OrderTypeDTO orderType, int tableNumber)
        {
            InitializeComponent();
            _orderItems = orderItems;
            _orderType = orderType;
            _tableNumber = tableNumber;
            
            InitializeProviderControls();
            LoadOrderSummary();
            ApplyTheme();
        }

        private void InitializeProviderControls()
        {
            // Only needed for Parcel orders
            if (_orderType != OrderTypeDTO.Parcel) return;

            // Expand Form and Payment Panel to accommodate new controls
            this.Height += 60;
            pnlPayment.Height += 60;
            
            // Move buttons down to avoid overlap
            btnConfirm.Top += 60;
            btnCancel.Top += 60;

            lblProvider = new Label();
            lblProvider.Text = "Provider:";
            lblProvider.Font = new Font("Segoe UI", 11F);
            lblProvider.AutoSize = true;
            lblProvider.ForeColor = ThemeManager.Instance.TextColor;
            lblProvider.Location = new Point(0, 100); // Below Change
            
            cmbProvider = new ComboBox();
            cmbProvider.Font = new Font("Segoe UI", 11F);
            cmbProvider.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProvider.Size = new Size(150, 30);
            cmbProvider.Location = new Point(150, 98);
            cmbProvider.BackColor = ThemeManager.Instance.ButtonHoverColor;
            cmbProvider.ForeColor = ThemeManager.Instance.TextColor;
            
            cmbProvider.Items.Add("Self");
            cmbProvider.Items.Add("FoodPanda");
            cmbProvider.SelectedIndex = 0; // Default to Self

            pnlPayment.Controls.Add(lblProvider);
            pnlPayment.Controls.Add(cmbProvider);
        }

        private void ApplyTheme()
        {
            var theme = ThemeManager.Instance;
            this.BackColor = theme.BackgroundColor;
            pnlMain.BackColor = theme.BackgroundColor;
            pnlPayment.BackColor = theme.BackgroundColor;
            
            lblTitle.ForeColor = theme.TextColor;
            lblSubtotal.ForeColor = theme.TextColor;
            lblTax.ForeColor = theme.TextColor;
            lblCashReceived.ForeColor = theme.TextColor;
            lblChange.ForeColor = theme.TextColor;
            
            if (lblProvider != null) lblProvider.ForeColor = theme.TextColor;
            
            // Fix: Use correct list background based on active theme, not hardcoded
            lvOrderSummary.BackColor = theme.IsDarkMode ? Color.FromArgb(50, 50, 50) : Color.WhiteSmoke;
            lvOrderSummary.ForeColor = theme.TextColor;
            
            txtCashReceived.BackColor = theme.ButtonHoverColor;
            txtCashReceived.ForeColor = theme.TextColor;

            if (cmbProvider != null)
            {
                cmbProvider.BackColor = theme.ButtonHoverColor;
                cmbProvider.ForeColor = theme.TextColor;
                // Fix: ComboBox flat style sometimes hides arrow in dark mode, but color change is main fix
            }
        }

        private void LoadOrderSummary()
        {
            lvOrderSummary.Items.Clear();
            _subtotal = 0;

            foreach (var item in _orderItems)
            {
                var lvi = new ListViewItem(item.MenuName);
                lvi.SubItems.Add(item.Quantity.ToString());
                lvi.SubItems.Add($"${item.Subtotal:F2}");
                lvOrderSummary.Items.Add(lvi);
                
                _subtotal += item.Subtotal;
            }

            _tax = 0; // No tax for now, can be configured later
            _total = _subtotal + _tax;

            lblSubtotal.Text = $"Subtotal: ${_subtotal:F2}";
            lblTax.Text = $"Tax (0%): ${_tax:F2}";
            lblTotal.Text = $"Total: ${_total:F2}";
        }

        private void txtCashReceived_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(txtCashReceived.Text, out double cashReceived))
            {
                double change = cashReceived - _total;
                lblChangeAmount.Text = $"${change:F2}";
                lblChangeAmount.ForeColor = change >= 0 ? Color.LimeGreen : Color.Red;
            }
            else
            {
                lblChangeAmount.Text = "$0.00";
                lblChangeAmount.ForeColor = Color.White;
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            // Validate cash received
            if (!double.TryParse(txtCashReceived.Text, out double cashReceived))
            {
                MessageBox.Show("Please enter a valid cash amount.", "Invalid Input", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cashReceived < _total)
            {
                MessageBox.Show($"Insufficient cash. Total is ${_total:F2}", "Insufficient Payment", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Create order via Bridge
                OrderServiceWrapper orderService = null;
                try
                {
                    orderService = new OrderServiceWrapper();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating OrderServiceWrapper: {ex.Message}\n\nStack: {ex.StackTrace}", 
                        "Service Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                // Create OrderDTO (Items list is already initialized in constructor)
                OrderDTO orderDto = null;
                try
                {
                    orderDto = new OrderDTO();
                    orderDto.Type = _orderType;
                    orderDto.TableNumber = _tableNumber;
                    orderDto.Status = OrderStatusDTO.Ordered;
                    orderDto.TotalAmount = _total;
                    
                    // Determine Provider
                    if (_orderType == OrderTypeDTO.Parcel)
                    {
                        if (cmbProvider != null && cmbProvider.SelectedItem != null)
                        {
                            string selected = cmbProvider.SelectedItem.ToString();
                            if (selected == "Self") orderDto.Provider = ParcelProviderDTO.Self;
                            else if (selected == "FoodPanda") orderDto.Provider = ParcelProviderDTO.FoodPanda;
                            else orderDto.Provider = ParcelProviderDTO.None;
                        }
                        else
                        {
                            // Default fallback if UI fails
                             orderDto.Provider = ParcelProviderDTO.Self;
                        }
                    }
                    else
                    {
                        orderDto.Provider = ParcelProviderDTO.None;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating OrderDTO: {ex.Message}\n\nStack: {ex.StackTrace}", 
                        "DTO Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                // Add items to the pre-initialized Items list
                try
                {
                    foreach (var item in _orderItems)
                    {
                        orderDto.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding items to OrderDTO: {ex.Message}\n\nStack: {ex.StackTrace}", 
                        "Items Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int orderId = 0;
                try
                {
                    orderId = orderService.CreateOrder(orderDto);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error calling CreateOrder: {ex.Message}\n\nInner: {ex.InnerException?.Message}\n\nStack: {ex.StackTrace}", 
                        "CreateOrder Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (orderId > 0)
                {
                    // Print receipts (future: integrate with PrintServiceWrapper)
                    // For now, just show success
                    
                    double change = cashReceived - _total;

                    // Generate Receipt
                    try
                    {
                        var receiptService = new ReceiptService();
                        // Save to Downloads folder
                        string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                        string receiptDir = Path.Combine(downloadsPath, "POS_Receipts");
                        Directory.CreateDirectory(receiptDir);
                        
                        string fileName = $"Receipt_{orderId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                        string fullPath = Path.Combine(receiptDir, fileName);

                        // Update OrderDTO with ID and Date for the receipt
                        orderDto.Id = orderId;

                        receiptService.GenerateReceipt(orderDto, cashReceived, change, fullPath);

                        // Open the PDF automatically
                        try
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(fullPath) { UseShellExecute = true });
                        }
                        catch { /* Ignore if fails to open */ }

                        MessageBox.Show(
                            $"Order #{orderId} Created Successfully!\n" +
                            $"Receipt saved to:\n{fileName}\n\n" +
                            $"Total: ${_total:F2}\n" +
                            $"Cash: ${cashReceived:F2}\n" +
                            $"Change: ${change:F2}", 
                            "Payment Complete", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Order created, but failed to generate receipt: {ex.Message}\n\n" +
                            $"Total: ${_total:F2}\n" +
                            $"Cash: ${cashReceived:F2}\n" +
                            $"Change: ${change:F2}", 
                            "Payment Complete (Receipt Error)", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Warning);
                    }
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to create order. Please try again.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}\n\nStack: {ex.StackTrace}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
