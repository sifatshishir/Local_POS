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
        private List<MenuItemDTO> _allMenuItems; // Store full menu for filtering
        private Panel pnlCartActions;
        private FlowLayoutPanel flowCategories;
        private Button btnRemove;
        private Button btnPlus;
        private Button btnMinus;

        public NewOrderControl()
        {
            InitializeComponent();
            _menuService = new MenuServiceWrapper();
            _cartItems = new List<OrderItemDTO>();
            InitializeCartActions();
            InitializeCategoryPanel();
            ThemeManager.Instance.ThemeChanged += Instance_ThemeChanged;
        }

        private void InitializeCategoryPanel()
        {
            flowCategories = new FlowLayoutPanel();
            flowCategories.Dock = DockStyle.Top;
            flowCategories.Height = 60; 
            flowCategories.Padding = new Padding(5, 5, 5, 15);
            flowCategories.AutoScroll = true;
            flowCategories.WrapContents = false; 
            
            if (splitContent.Panel1.Controls.Contains(flowMenu))
            {
               splitContent.Panel1.Controls.Add(flowCategories);
               
               // Spacer
               Panel spacer = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = Color.Transparent };
               splitContent.Panel1.Controls.Add(spacer);

               // CRITICAL: Docking order is reverse of Z-order (Index).
               // We want Categories (Top) -> Spacer (Top) -> Menu (Fill).
               // So Categories must be docked FIRST (Highest Index/Back).
               // Spacer docked SECOND.
               // Menu docked LAST (Lowest Index/Front).
               
               spacer.SendToBack();         // Spacer becomes Last
               flowCategories.SendToBack(); // Categories becomes Last (Spacer becomes 2nd Last)
            }
        }

        private void Instance_ThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
            
            // Re-render categories to update their colors
            RenderCategories();
            
            // Update existing menu buttons without reloading from DB
            var theme = ThemeManager.Instance;
            foreach (Control ctrl in flowMenu.Controls)
            {
                if (ctrl is ModernMenuButton btn)
                {
                    btn.BaseColor = theme.PrimaryColor;
                    btn.ForeColor = theme.TextColor;
                    btn.Invalidate(); // Force repaint
                }
                else if (ctrl is Label lbl)
                {
                    lbl.ForeColor = theme.TextColor; // Error labels
                }
            }
        }

        private void InitializeCartActions()
        {
            pnlCartActions = new Panel();
            pnlCartActions.Height = 40;
            pnlCartActions.Dock = DockStyle.Top;
            pnlCartActions.Padding = new Padding(5);

            btnRemove = CreateActionButton("Remove", Color.IndianRed);
            btnRemove.Click += BtnRemove_Click;
            
            btnMinus = CreateActionButton("-", ThemeManager.Instance.ButtonHoverColor);
            btnMinus.Width = 40;
            btnMinus.Click += BtnMinus_Click;

            btnPlus = CreateActionButton("+", ThemeManager.Instance.ButtonHoverColor);
            btnPlus.Width = 40;
            btnPlus.Click += BtnPlus_Click;
            
            // Add buttons to panel (Right aligned for actions, Left for Remove)
            btnRemove.Dock = DockStyle.Left;
            btnPlus.Dock = DockStyle.Right;
            
            // Spacer for visually separating + and -
            Panel spacer = new Panel { Width = 5, Dock = DockStyle.Right };
            
            btnMinus.Dock = DockStyle.Right;

            pnlCartActions.Controls.Add(btnRemove);
            pnlCartActions.Controls.Add(btnMinus);
            pnlCartActions.Controls.Add(spacer);
            pnlCartActions.Controls.Add(btnPlus);

            // Add to Cart Panel logic
            cartPanel.Controls.Add(pnlCartActions);
            
            // Fix Cart Panel Z-Order for Docking
            // We want: Title (Top) -> Actions (Top) -> List (Fill).
            // Title must be docked FIRST -> Lowest Z-order? NO, Highest Index (Back).
            
            // 1. Send Actions to Back (Last)
            pnlCartActions.SendToBack(); 
            // 2. Send Title to Back (Title becomes Last, Actions becomes 2nd Last)
            lblCartTitle.SendToBack();
            
            // Result: Title Docks Top (Y=0). Actions Docks Top (Y=Title). List Docks Fill (Y=Title+Actions).
             
            pnlCartActions.BackColor = ThemeManager.Instance.BackgroundColor;
        }

        private Button CreateActionButton(string text, Color bg)
        {
            var btn = new Button();
            btn.Text = text;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = bg;
            btn.ForeColor = Color.White;
            btn.Cursor = Cursors.Hand;
            btn.Width = 80;
            return btn;
        }

        private void BtnRemove_Click(object? sender, EventArgs e)
        {
            if (lvCart.SelectedIndices.Count == 0) return;
            int index = lvCart.SelectedIndices[0];
            
            if (index >= 0 && index < _cartItems.Count)
            {
                _cartItems.RemoveAt(index);
                UpdateCartDisplay();
            }
        }

        private void BtnPlus_Click(object? sender, EventArgs e)
        {
            if (lvCart.SelectedIndices.Count == 0) return;
            int index = lvCart.SelectedIndices[0];
            
            if (index >= 0 && index < _cartItems.Count)
            {
                _cartItems[index].Quantity++;
                UpdateCartDisplay(index);
            }
        }

        private void BtnMinus_Click(object? sender, EventArgs e)
        {
            if (lvCart.SelectedIndices.Count == 0) return;
            int index = lvCart.SelectedIndices[0];
            
            if (index >= 0 && index < _cartItems.Count)
            {
                if (_cartItems[index].Quantity > 1)
                {
                    _cartItems[index].Quantity--;
                    UpdateCartDisplay(index);
                }
            }
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
            txtTableNumber.BackColor = theme.ButtonHoverColor; 
            txtTableNumber.ForeColor = theme.TextColor;
            txtTableNumber.BorderStyle = BorderStyle.FixedSingle;

            // Cart Layout
            cartPanel.BackColor = theme.BackgroundColor; 
            lvCart.BackColor = theme.IsDarkMode ? Color.FromArgb(50, 50, 50) : Color.WhiteSmoke;
            lvCart.ForeColor = theme.TextColor;
            
            lblCartTitle.ForeColor = theme.TextColor;
            lblTotal.ForeColor = theme.TextColor; 
            
            if (pnlCartActions != null)
            {
                pnlCartActions.BackColor = theme.BackgroundColor;
                btnPlus.BackColor = theme.ButtonHoverColor;
                btnPlus.ForeColor = theme.TextColor;
                btnMinus.BackColor = theme.ButtonHoverColor;
                btnMinus.ForeColor = theme.TextColor;
            }
        }

        private void LoadMenu()
        {
            try
            {
                // Fetch once
                _allMenuItems = _menuService.GetAllMenuItems();

                if (_allMenuItems == null || _allMenuItems.Count == 0)
                {
                    ShowError("No menu items found.");
                    return;
                }
                
                // Initial Render
                RenderCategories();
                FilterMenu("All");
            }
            catch (Exception ex)
            {
                ShowError($"Error loading menu: {ex.Message}");
            }
        }

        private void RenderCategories()
        {
            if (flowCategories == null) InitializeCategoryPanel();

            flowCategories.Controls.Clear();
            if (_allMenuItems == null) return;

            // Extract unique categories
            var categories = new HashSet<string>();
            foreach(var item in _allMenuItems) categories.Add(item.Category);
            
            // "All" Button
            AddCategoryButton("All");

            foreach(var cat in categories)
            {
                AddCategoryButton(cat);
            }
        }

        private void AddCategoryButton(string text)
        {
            var btn = new Button();
            btn.Text = text;
            btn.AutoSize = true;
            btn.Padding = new Padding(10, 5, 10, 5);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            
            var theme = ThemeManager.Instance;
            btn.BackColor = theme.ButtonHoverColor;
            btn.ForeColor = theme.TextColor;
            
            btn.Click += (s, e) => FilterMenu(text);
            
            flowCategories.Controls.Add(btn);
        }

        private void FilterMenu(string category)
        {
            flowMenu.Controls.Clear();
            flowMenu.SuspendLayout(); // Performance

            var itemsToShow = (category == "All") 
                ? _allMenuItems 
                : _allMenuItems.FindAll(i => i.Category == category);

            foreach (var item in itemsToShow)
            {
                if (!item.IsActive) continue;

                var btn = new ModernMenuButton();
                btn.Text = item.Name;
                btn.PriceText = $"${item.Price:F2}";
                btn.Size = new Size(140, 110);
                btn.BaseColor = ThemeManager.Instance.PrimaryColor; 
                btn.ForeColor = ThemeManager.Instance.TextColor;
                
                btn.Cursor = Cursors.Hand;
                btn.Tag = item; 
                btn.Click += MediaType_Click; 
                
                flowMenu.Controls.Add(btn);
            }
            flowMenu.ResumeLayout();
        }

        private void ShowError(string msg)
        {
            flowMenu.Controls.Clear();
            var lbl = new Label { 
                Text = msg, 
                ForeColor = Color.Red, 
                AutoSize = true, 
                Padding = new Padding(20),
                Font = new Font("Segoe UI", 12)
            };
            flowMenu.Controls.Add(lbl);
        }

        // Custom Button Class for "Lucrative" Design
        private class ModernMenuButton : Control
        {
            public string PriceText { get; set; } = "$0.00";
            public Color BaseColor { get; set; } = Color.Gray;

            public ModernMenuButton()
            {
                this.DoubleBuffered = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                var rect = this.ClientRectangle;
                rect.Width -= 1; rect.Height -= 1; // Adjust for border

                // Background Gradient
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    this.ClientRectangle, 
                    ControlPaint.Light(BaseColor, 0.1f), 
                    BaseColor, 
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, rect);
                }

                // Border
                using (var pen = new Pen(ControlPaint.Dark(BaseColor, 0.2f), 1))
                {
                    g.DrawRectangle(pen, rect);
                }

                // Text Formatting
                var titleFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                var priceFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far };

                var titleRect = new Rectangle(0, 0, Width, Height - 30);
                var priceRect = new Rectangle(0, Height - 30, Width, 25);

                using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
                using (var brush = new SolidBrush(this.ForeColor))
                {
                    g.DrawString(this.Text, font, brush, titleRect, titleFormat);
                }

                using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
                using (var brush = new SolidBrush(Color.DarkOrange)) // Fix: DarkOrange for visibility
                {
                    g.DrawString(PriceText, font, brush, priceRect, priceFormat);
                }
            }

            // Expose Click event behavior manually since we inherit Control
            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
            }
            
            // Interaction effects
            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                BaseColor = ControlPaint.Light(BaseColor, 0.1f);
                Invalidate();
            }
            
            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                BaseColor = ThemeManager.Instance.PrimaryColor; // Reset
                Invalidate();
            }
        }

        private void MediaType_Click(object sender, EventArgs e)
        {
            if (sender is Control btn && btn.Tag is MenuItemDTO item)
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

        private void UpdateCartDisplay(int selectedIndex = -1)
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
            
            if (selectedIndex >= 0 && selectedIndex < lvCart.Items.Count)
            {
                lvCart.Items[selectedIndex].Selected = true;
                lvCart.Select(); // Focus list to show selection
            }
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
    }
}
