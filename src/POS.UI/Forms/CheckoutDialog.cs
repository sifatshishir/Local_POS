using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using POS.Bridge;
using POS.Bridge.DataTransferObjects;

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

        public CheckoutDialog(List<OrderItemDTO> orderItems, OrderTypeDTO orderType, int tableNumber)
        {
            InitializeComponent();
            _orderItems = orderItems;
            _orderType = orderType;
            _tableNumber = tableNumber;
            
            LoadOrderSummary();
            ApplyTheme();
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
            
            lvOrderSummary.BackColor = Color.FromArgb(50, 50, 50);
            lvOrderSummary.ForeColor = theme.TextColor;
            
            txtCashReceived.BackColor = theme.ButtonHoverColor;
            txtCashReceived.ForeColor = theme.TextColor;
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
                var orderService = new OrderServiceWrapper();
                
                var orderDto = new OrderDTO();
                orderDto.Type = _orderType;
                orderDto.TableNumber = _tableNumber;
                orderDto.Status = OrderStatusDTO.Ordered;
                orderDto.TotalAmount = _total;
                orderDto.Items = _orderItems;
                orderDto.Provider = ParcelProviderDTO.None;

                int orderId = orderService.CreateOrder(orderDto);

                if (orderId > 0)
                {
                    // Print receipts (future: integrate with PrintServiceWrapper)
                    // For now, just show success
                    
                    double change = cashReceived - _total;
                    MessageBox.Show(
                        $"Order #{orderId} Created Successfully!\n\n" +
                        $"Total: ${_total:F2}\n" +
                        $"Cash: ${cashReceived:F2}\n" +
                        $"Change: ${change:F2}", 
                        "Payment Complete", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Information);
                    
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
                MessageBox.Show($"Error processing payment: {ex.Message}", "Error", 
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
