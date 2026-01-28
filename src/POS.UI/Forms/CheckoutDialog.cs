using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using POS.Bridge;
using POS.Bridge.DataTransferObjects;
using POS.UI.Services;
using System.IO;
using POS.Client.Common.Helpers;

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

        private async void btnConfirm_ClickAsync(object sender, EventArgs e)
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
                // Prepare Protocol String
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("CMD:CREATE");
                sb.AppendLine($"TYPE:{(_orderType == OrderTypeDTO.Parcel ? "Parcel" : "DineIn")}");
                if (_orderType == OrderTypeDTO.DineIn) sb.AppendLine($"TABLE:{_tableNumber}");
                
                string providerStr = "None";
                if (_orderType == OrderTypeDTO.Parcel)
                {
                     if (cmbProvider != null && cmbProvider.SelectedItem != null)
                     {
                         string selected = cmbProvider.SelectedItem.ToString();
                         // Match C++ server checks
                         if (selected == "Self") providerStr = "Self";
                         else if (selected == "FoodPanda") providerStr = "FoodPanda";
                     }
                     else
                     {
                         // Default fallback
                          providerStr = "Self"; // Fallback to Self
                     }
                }
                sb.AppendLine($"PROVIDER:{providerStr}");

                foreach (var item in _orderItems)
                {
                    // Item Format: ID,NAME,PRICE,QTY
                    // Ensure Name has no commas? For MVP assuming it's safe or replacing comma
                    string safeName = item.MenuName.Replace(",", " ");
                    sb.AppendLine($"ITEM:{item.MenuItemId},{safeName},{item.Price},{item.Quantity}");
                }
                sb.AppendLine("END");

                string command = sb.ToString();

                // Send via WebSocket
                var tcs = new System.Threading.Tasks.TaskCompletionSource<int>();
                var ws = new WebSocketHelper();
                
                // Connect and Listen
                await ws.ConnectAsync();
                
                // Fire and forget listening, but we hook the callback
                _ = ws.StartListening((msg) => {
                    if (msg.StartsWith("ORDER_CREATED:"))
                    {
                        if (int.TryParse(msg.Substring("ORDER_CREATED:".Length), out int newId))
                        {
                            tcs.TrySetResult(newId);
                        }
                    }
                    else if (msg.StartsWith("ERROR:"))
                    {
                         tcs.TrySetException(new Exception(msg));
                    }
                });

                // Send
                await ws.SendMessageAsync(command);

                // Wait for response (timeout 5s)
                var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(5000));
                
                if (completedTask == tcs.Task)
                {
                    int orderId = await tcs.Task;
                    
                     // Success - Generate Receipt
                    double change = cashReceived - _total;

                    // Generate Receipt
                    try
                    {
                        var receiptService = new ReceiptService();
                         // Save to configured folder or Downloads
                        string receiptDir = EnvLoader.Get("RECEIPT_PATH");
                        if (string.IsNullOrEmpty(receiptDir) || !Directory.Exists(receiptDir))
                        {
                            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                            receiptDir = Path.Combine(downloadsPath, "POS_Receipts");
                        }
                        Directory.CreateDirectory(receiptDir);
                        
                        string fileName = $"Receipt_{orderId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                        string fullPath = Path.Combine(receiptDir, fileName);

                        // Update OrderDTO with ID and Date for the receipt
                        OrderDTO orderDto = new OrderDTO(); // We can't reuse the one we didn't populate fully?
                        // Actually we need to reconstruct orderDto for the receipt service or reuse components
                        // Reconstruct minimalist one for receipt:
                        orderDto.Id = orderId;
                        orderDto.Type = _orderType;
                        orderDto.TableNumber = _tableNumber;
                        orderDto.TotalAmount = _total; // Assuming calculated
                        orderDto.Provider = (ParcelProviderDTO)Enum.Parse(typeof(ParcelProviderDTO), providerStr); // Approximate
                        foreach(var i in _orderItems) orderDto.Items.Add(i);

                        receiptService.GenerateReceipt(orderDto, cashReceived, change, fullPath);

                        // Open the PDF automatically
                        try
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(fullPath) { UseShellExecute = true });
                        }
                        catch { /* Ignore */ }

                        MessageBox.Show(
                            $"Order #{orderId} Created Successfully!\n" +
                            $"Receipt saved and opened.", 
                            "Payment Complete", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Receipt Error: {ex.Message}");
                    }
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                     MessageBox.Show("Server timed out. Check network.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
