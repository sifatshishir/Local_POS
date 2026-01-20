namespace POS.UI.Controls
{
    partial class NewOrderControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            // Dispose managed wrapper manually if needed or relies on finalizer
            if (disposing && _menuService != null)
            {
               // _menuService.Dispose(); // Assuming IDisposable is implemented via ref class destructor
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.topPanel = new System.Windows.Forms.Panel();
            this.lblTableParams = new System.Windows.Forms.Label();
            this.txtTableNumber = new System.Windows.Forms.TextBox();
            this.rbParcel = new System.Windows.Forms.RadioButton();
            this.rbDineIn = new System.Windows.Forms.RadioButton();
            this.splitContent = new System.Windows.Forms.SplitContainer();
            this.flowMenu = new System.Windows.Forms.FlowLayoutPanel();
            this.cartPanel = new System.Windows.Forms.Panel();
            this.btnCheckout = new POS.UI.Controls.FlatButton();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lvCart = new System.Windows.Forms.ListView();
            this.lblCartTitle = new System.Windows.Forms.Label();
            
            this.mainLayout.SuspendLayout();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContent)).BeginInit();
            this.splitContent.Panel1.SuspendLayout();
            this.splitContent.Panel2.SuspendLayout();
            this.splitContent.SuspendLayout();
            this.cartPanel.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.topPanel, 0, 0);
            this.mainLayout.Controls.Add(this.splitContent, 0, 1);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.RowCount = 2;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Size = new System.Drawing.Size(1000, 700);
            this.mainLayout.TabIndex = 0;

            // 
            // topPanel
            // 
            this.topPanel.Controls.Add(this.lblTableParams);
            this.topPanel.Controls.Add(this.txtTableNumber);
            this.topPanel.Controls.Add(this.rbParcel);
            this.topPanel.Controls.Add(this.rbDineIn);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topPanel.Location = new System.Drawing.Point(3, 3);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(994, 54);
            this.topPanel.TabIndex = 0;

            // 
            // rbDineIn
            // 
            this.rbDineIn.AutoSize = true;
            this.rbDineIn.Checked = true;
            this.rbDineIn.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.rbDineIn.Location = new System.Drawing.Point(20, 15);
            this.rbDineIn.Name = "rbDineIn";
            this.rbDineIn.Size = new System.Drawing.Size(84, 27);
            this.rbDineIn.TabIndex = 0;
            this.rbDineIn.TabStop = true;
            this.rbDineIn.Text = "Dine In";
            this.rbDineIn.UseVisualStyleBackColor = true;
            this.rbDineIn.CheckedChanged += new System.EventHandler(this.OrderType_CheckedChanged);

            // 
            // rbParcel
            // 
            this.rbParcel.AutoSize = true;
            this.rbParcel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.rbParcel.Location = new System.Drawing.Point(130, 15);
            this.rbParcel.Name = "rbParcel";
            this.rbParcel.Size = new System.Drawing.Size(76, 27);
            this.rbParcel.TabIndex = 1;
            this.rbParcel.Text = "Parcel";
            this.rbParcel.UseVisualStyleBackColor = true;
            this.rbParcel.CheckedChanged += new System.EventHandler(this.OrderType_CheckedChanged);

            // 
            // txtTableNumber
            // 
            this.txtTableNumber.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtTableNumber.Location = new System.Drawing.Point(400, 14);
            this.txtTableNumber.Name = "txtTableNumber";
            this.txtTableNumber.Size = new System.Drawing.Size(100, 30);
            this.txtTableNumber.TabIndex = 2;
            
            // 
            // lblTableParams
            // 
            this.lblTableParams.AutoSize = true;
            this.lblTableParams.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTableParams.Location = new System.Drawing.Point(280, 17);
            this.lblTableParams.Name = "lblTableParams";
            this.lblTableParams.Size = new System.Drawing.Size(118, 23);
            this.lblTableParams.TabIndex = 3;
            this.lblTableParams.Text = "Table Number:";

            // 
            // splitContent
            // 
            this.splitContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContent.Location = new System.Drawing.Point(3, 63);
            this.splitContent.Name = "splitContent";
            // 
            // splitContent.Panel1
            // 
            this.splitContent.Panel1.Controls.Add(this.flowMenu);
            // 
            // splitContent.Panel2
            // 
            this.splitContent.Panel2.Padding = new System.Windows.Forms.Padding(10);
            this.splitContent.Panel2.Controls.Add(this.cartPanel);
            this.splitContent.Size = new System.Drawing.Size(994, 634);
            this.splitContent.SplitterDistance = 600;
            this.splitContent.TabIndex = 1;

            // 
            // flowMenu
            // 
            this.flowMenu.AutoScroll = true;
            this.flowMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowMenu.Location = new System.Drawing.Point(0, 0);
            this.flowMenu.Name = "flowMenu";
            this.flowMenu.Padding = new System.Windows.Forms.Padding(10);
            this.flowMenu.Size = new System.Drawing.Size(600, 634);
            this.flowMenu.TabIndex = 0;

            // 
            // cartPanel
            // 
            this.cartPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40))))); // Slightly lighter
            this.cartPanel.Controls.Add(this.btnCheckout);
            this.cartPanel.Controls.Add(this.lblTotal);
            this.cartPanel.Controls.Add(this.lvCart);
            this.cartPanel.Controls.Add(this.lblCartTitle);
            this.cartPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cartPanel.Location = new System.Drawing.Point(10, 10);
            this.cartPanel.Name = "cartPanel";
            this.cartPanel.Size = new System.Drawing.Size(370, 614);
            this.cartPanel.TabIndex = 0;

            // 
            // lblCartTitle
            // 
            this.lblCartTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCartTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblCartTitle.ForeColor = System.Drawing.Color.White;
            this.lblCartTitle.Location = new System.Drawing.Point(0, 0);
            this.lblCartTitle.Name = "lblCartTitle";
            this.lblCartTitle.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.lblCartTitle.Size = new System.Drawing.Size(370, 50);
            this.lblCartTitle.TabIndex = 0;
            this.lblCartTitle.Text = "Current Order";
            this.lblCartTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // lvCart
            // 
            this.lvCart.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvCart.Dock = System.Windows.Forms.DockStyle.Top;
            this.lvCart.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lvCart.FullRowSelect = true;
            this.lvCart.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvCart.Location = new System.Drawing.Point(0, 50);
            this.lvCart.Name = "lvCart";
            this.lvCart.Size = new System.Drawing.Size(370, 400); // Fixed height for now
            this.lvCart.TabIndex = 1;
            this.lvCart.UseCompatibleStateImageBehavior = false;
            this.lvCart.View = System.Windows.Forms.View.Details;
            this.lvCart.Columns.Add("Item", 180);
            this.lvCart.Columns.Add("Qty", 50);
            this.lvCart.Columns.Add("Price", 80);

            // 
            // lblTotal
            // 
            this.lblTotal.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.Gold;
            this.lblTotal.Location = new System.Drawing.Point(0, 450);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(370, 60);
            this.lblTotal.TabIndex = 2;
            this.lblTotal.Text = "Total: $0.00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // btnCheckout
            // 
            this.btnCheckout.BackColor = System.Drawing.Color.SeaGreen;
            this.btnCheckout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnCheckout.FlatAppearance.BorderSize = 0;
            this.btnCheckout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckout.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCheckout.ForeColor = System.Drawing.Color.White;
            this.btnCheckout.Location = new System.Drawing.Point(0, 554);
            this.btnCheckout.Name = "btnCheckout";
            this.btnCheckout.Size = new System.Drawing.Size(370, 60);
            this.btnCheckout.TabIndex = 3;
            this.btnCheckout.Text = "Checkout";
            this.btnCheckout.UseVisualStyleBackColor = false;
            this.btnCheckout.Click += new System.EventHandler(this.btnCheckout_Click);

            this.mainLayout.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.splitContent.Panel1.ResumeLayout(false);
            this.splitContent.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContent)).EndInit();
            this.splitContent.ResumeLayout(false);
            this.cartPanel.ResumeLayout(false);
            this.ResumeLayout(false);

            this.Controls.Add(this.mainLayout);
            this.Name = "NewOrderControl";
            this.Size = new System.Drawing.Size(1000, 700);
            this.Load += new System.EventHandler(this.NewOrderControl_Load);
        }

        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.RadioButton rbDineIn;
        private System.Windows.Forms.RadioButton rbParcel;
        private System.Windows.Forms.TextBox txtTableNumber;
        private System.Windows.Forms.Label lblTableParams;
        private System.Windows.Forms.SplitContainer splitContent;
        private System.Windows.Forms.FlowLayoutPanel flowMenu;
        private System.Windows.Forms.Panel cartPanel;
        private System.Windows.Forms.Label lblCartTitle;
        private System.Windows.Forms.ListView lvCart;
        private System.Windows.Forms.Label lblTotal;
        private POS.UI.Controls.FlatButton btnCheckout;
    }
}
