using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using POS.Bridge;
using POS.Bridge.DataTransferObjects;

namespace POS.UI.Controls
{
    public partial class NewOrderControl : UserControl
    {
        private MenuServiceWrapper _menuService;
        private List<OrderItemDTO> _cartItems;

        public NewOrderControl()
        {
            InitializeComponent();
            _menuService = new MenuServiceWrapper();
            _cartItems = new List<OrderItemDTO>();
        }

        private void NewOrderControl_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            LoadMenu();
        }

        private void ApplyTheme()
        {
            var theme = ThemeManager.Instance;
            this.BackColor = theme.BackgroundColor;
            
            // Top Panel
            topPanel.BackColor = theme.BackgroundColor;
            rbDineIn.ForeColor = theme.TextColor;
            rbParcel.ForeColor = theme.TextColor;
            lblTableParams.ForeColor = theme.TextColor;
            txtTableNumber.BackColor = theme.ButtonHoverColor; // Slightly lighter
            txtTableNumber.ForeColor = theme.TextColor;
            txtTableNumber.BorderStyle = BorderStyle.FixedSingle;

            // Cart Layout
            // cartPanel is set in Designer
            lvCart.BackColor = Color.FromArgb(50, 50, 50);
            lvCart.ForeColor = theme.TextColor;
        }

        private void LoadMenu()
        {
            try
            {
                flowMenu.Controls.Clear();
                var menuItems = _menuService.GetAllMenuItems();

                if (menuItems == null || menuItems.Count == 0)
                {
                    var lblEmpty = new Label();
                    lblEmpty.Text = "No menu items found.\nPlease check database connection.";
                    lblEmpty.ForeColor = Color.Yellow;
                    lblEmpty.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                    lblEmpty.AutoSize = true;
                    lblEmpty.Location = new Point(20, 20);
                    flowMenu.Controls.Add(lblEmpty);
                    return;
                }

                foreach (var item in menuItems)
                {
                    if (!item.IsActive) continue;

                    var btn = new Button();
                    btn.Text = $"{item.Name}\n${item.Price:F2}";
                    btn.Size = new Size(120, 100);
                    btn.BackColor = ThemeManager.Instance.PrimaryColor;
                    btn.ForeColor = ThemeManager.Instance.TextColor;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    btn.Cursor = Cursors.Hand;
                    
                    // Store item in Tag properly
                    btn.Tag = item; 
                    btn.Click += MediaType_Click;
                    
                    flowMenu.Controls.Add(btn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading menu: {ex.Message}\n\nPlease check:\n1. MySQL is running\n2. Database 'pos_db' exists\n3. .env file is configured correctly", 
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                var lblError = new Label();
                lblError.Text = "Database Error - See message box";
                lblError.ForeColor = Color.Red;
                lblError.AutoSize = true;
                lblError.Location = new Point(20, 20);
                flowMenu.Controls.Add(lblError);
            }
        }

        private void MediaType_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is MenuItemDTO item)
            {
                AddToCart(item);
            }
        }

        private void AddToCart(MenuItemDTO item)
        {
            var existing = _cartItems.Find(x => x.MenuItemId == item.Id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                var orderItem = new OrderItemDTO();
                orderItem.MenuItemId = item.Id;
                orderItem.MenuName = item.Name;
                orderItem.Price = item.Price;
                orderItem.Quantity = 1;
                _cartItems.Add(orderItem);
            }
            UpdateCartDisplay();
        }

        private void UpdateCartDisplay()
        {
            lvCart.Items.Clear();
            double total = 0;

            foreach (var item in _cartItems)
            {
                var lvi = new ListViewItem(item.MenuName);
                lvi.SubItems.Add(item.Quantity.ToString());
                lvi.SubItems.Add($"${item.Subtotal:F2}");
                lvCart.Items.Add(lvi);
                
                total += item.Subtotal;
            }

            lblTotal.Text = $"Total: ${total:F2}";
        }

        private void OrderType_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDineIn.Checked)
            {
                lblTableParams.Visible = true;
                txtTableNumber.Visible = true;
            }
            else
            {
                lblTableParams.Visible = false;
                txtTableNumber.Visible = false;
            }
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            if (_cartItems.Count == 0)
            {
                MessageBox.Show("Cart is empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int tableNo = 0;
            if (rbDineIn.Checked)
            {
                if (!int.TryParse(txtTableNumber.Text, out tableNo))
                {
                    MessageBox.Show("Please enter a valid table number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Open Checkout Dialog
            var orderType = rbDineIn.Checked ? OrderTypeDTO.DineIn : OrderTypeDTO.Parcel;
            var checkoutDialog = new Forms.CheckoutDialog(_cartItems, orderType, tableNo);
            
            if (checkoutDialog.ShowDialog() == DialogResult.OK)
            {
                // Order was successfully created, clear the cart
                _cartItems.Clear();
                UpdateCartDisplay();
                txtTableNumber.Clear();
            }
        }

        private void ProcessOrder(int tableNumber)
        {
            try
            {
                var orderService = new OrderServiceWrapper();
                var orderType = rbDineIn.Checked ? OrderTypeDTO.DineIn : OrderTypeDTO.Parcel;
                
                // Convert list to managed list compatible with Bridge
                // Bridge expects List<OrderItemDTO^>^
                // Our _cartItems is List<OrderItemDTO> (if OrderItemDTO is ref class, it holds references automatically)
                
                // Using helper if available or direct cast?
                // Bridge signature: CreateOrder(OrderType, tableNo, List<OrderItemDTO>^ items)
                
                // Correct C# Syntax for List of DTOs (which are ref classes from CLI)
                var bridgeList = new List<OrderItemDTO>();
                foreach(var item in _cartItems) bridgeList.Add(item);


                // Wait, the Wrapper signature in header is `int CreateOrder(OrderDTO^ orderDto)`
                // But my previous call was `CreateOrder(orderType, tableNumber, bridgeList)`
                // I need to adapt the C# call to match the Wrapper signature, OR update the Wrapper.
                // The Wrapper in step 86 shows: int CreateOrder(OrderDTO^ orderDto)
                // So I must construct an OrderDTO first.

                var orderDto = new OrderDTO();
                orderDto.Type = orderType;
                orderDto.TableNumber = tableNumber;
                orderDto.Status = OrderStatusDTO.Ordered;
                orderDto.TotalAmount = 0; // calculated by backend
                orderDto.Items = bridgeList;
                orderDto.Provider = ParcelProviderDTO.None; // Default

                int createdId = orderService.CreateOrder(orderDto);

                if (createdId > 0)
                {
                    MessageBox.Show($"Order #{createdId} Created Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _cartItems.Clear();
                    UpdateCartDisplay();
                    txtTableNumber.Clear();
                }
                else
                {
                    MessageBox.Show("Failed to create order.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing order: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
