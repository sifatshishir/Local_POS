namespace POS.UI.Forms
{
    partial class CheckoutDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlMain = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lvOrderSummary = new System.Windows.Forms.ListView();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.lblTax = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.pnlPayment = new System.Windows.Forms.Panel();
            this.lblCashReceived = new System.Windows.Forms.Label();
            this.txtCashReceived = new System.Windows.Forms.TextBox();
            this.lblChange = new System.Windows.Forms.Label();
            this.lblChangeAmount = new System.Windows.Forms.Label();
            this.btnConfirm = new POS.UI.Controls.FlatButton();
            this.btnCancel = new POS.UI.Controls.FlatButton();
            this.pnlMain.SuspendLayout();
            this.pnlPayment.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.btnCancel);
            this.pnlMain.Controls.Add(this.btnConfirm);
            this.pnlMain.Controls.Add(this.pnlPayment);
            this.pnlMain.Controls.Add(this.lblTotal);
            this.pnlMain.Controls.Add(this.lblTax);
            this.pnlMain.Controls.Add(this.lblSubtotal);
            this.pnlMain.Controls.Add(this.lvOrderSummary);
            this.pnlMain.Controls.Add(this.lblTitle);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(20);
            this.pnlMain.Size = new System.Drawing.Size(500, 600);
            this.pnlMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(460, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Checkout";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lvOrderSummary
            // 
            this.lvOrderSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvOrderSummary.Dock = System.Windows.Forms.DockStyle.Top;
            this.lvOrderSummary.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lvOrderSummary.FullRowSelect = true;
            this.lvOrderSummary.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvOrderSummary.Location = new System.Drawing.Point(20, 60);
            this.lvOrderSummary.Name = "lvOrderSummary";
            this.lvOrderSummary.Size = new System.Drawing.Size(460, 250);
            this.lvOrderSummary.TabIndex = 1;
            this.lvOrderSummary.UseCompatibleStateImageBehavior = false;
            this.lvOrderSummary.View = System.Windows.Forms.View.Details;
            this.lvOrderSummary.Columns.Add("Item", 250);
            this.lvOrderSummary.Columns.Add("Qty", 60);
            this.lvOrderSummary.Columns.Add("Price", 100);
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSubtotal.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblSubtotal.Location = new System.Drawing.Point(20, 310);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Padding = new System.Windows.Forms.Padding(0, 10, 20, 0);
            this.lblSubtotal.Size = new System.Drawing.Size(460, 35);
            this.lblSubtotal.TabIndex = 2;
            this.lblSubtotal.Text = "Subtotal: $0.00";
            this.lblSubtotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTax
            // 
            this.lblTax.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTax.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblTax.Location = new System.Drawing.Point(20, 345);
            this.lblTax.Name = "lblTax";
            this.lblTax.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.lblTax.Size = new System.Drawing.Size(460, 30);
            this.lblTax.TabIndex = 3;
            this.lblTax.Text = "Tax (0%): $0.00";
            this.lblTax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTotal
            // 
            this.lblTotal.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.Gold;
            this.lblTotal.Location = new System.Drawing.Point(20, 375);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Padding = new System.Windows.Forms.Padding(0, 5, 20, 5);
            this.lblTotal.Size = new System.Drawing.Size(460, 45);
            this.lblTotal.TabIndex = 4;
            this.lblTotal.Text = "Total: $0.00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlPayment
            // 
            this.pnlPayment.Controls.Add(this.lblChangeAmount);
            this.pnlPayment.Controls.Add(this.lblChange);
            this.pnlPayment.Controls.Add(this.txtCashReceived);
            this.pnlPayment.Controls.Add(this.lblCashReceived);
            this.pnlPayment.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPayment.Location = new System.Drawing.Point(20, 420);
            this.pnlPayment.Name = "pnlPayment";
            this.pnlPayment.Size = new System.Drawing.Size(460, 100);
            this.pnlPayment.TabIndex = 5;
            // 
            // lblCashReceived
            // 
            this.lblCashReceived.AutoSize = true;
            this.lblCashReceived.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblCashReceived.Location = new System.Drawing.Point(0, 10);
            this.lblCashReceived.Name = "lblCashReceived";
            this.lblCashReceived.Size = new System.Drawing.Size(120, 25);
            this.lblCashReceived.TabIndex = 0;
            this.lblCashReceived.Text = "Cash Received:";
            // 
            // txtCashReceived
            // 
            this.txtCashReceived.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtCashReceived.Location = new System.Drawing.Point(150, 8);
            this.txtCashReceived.Name = "txtCashReceived";
            this.txtCashReceived.Size = new System.Drawing.Size(150, 34);
            this.txtCashReceived.TabIndex = 1;
            this.txtCashReceived.TextChanged += new System.EventHandler(this.txtCashReceived_TextChanged);
            // 
            // lblChange
            // 
            this.lblChange.AutoSize = true;
            this.lblChange.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblChange.Location = new System.Drawing.Point(0, 60);
            this.lblChange.Name = "lblChange";
            this.lblChange.Size = new System.Drawing.Size(75, 25);
            this.lblChange.TabIndex = 2;
            this.lblChange.Text = "Change:";
            // 
            // lblChangeAmount
            // 
            this.lblChangeAmount.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblChangeAmount.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblChangeAmount.Location = new System.Drawing.Point(150, 55);
            this.lblChangeAmount.Name = "lblChangeAmount";
            this.lblChangeAmount.Size = new System.Drawing.Size(150, 35);
            this.lblChangeAmount.TabIndex = 3;
            this.lblChangeAmount.Text = "$0.00";
            this.lblChangeAmount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnConfirm
            // 
            this.btnConfirm.BackColor = System.Drawing.Color.SeaGreen;
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.FlatAppearance.BorderSize = 0;
            this.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirm.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Location = new System.Drawing.Point(20, 530);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(220, 50);
            this.btnConfirm.TabIndex = 6;
            this.btnConfirm.Text = "Confirm Payment";
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(260, 530);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(220, 50);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CheckoutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 600);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckoutDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Checkout";
            this.pnlMain.ResumeLayout(false);
            this.pnlPayment.ResumeLayout(false);
            this.pnlPayment.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ListView lvOrderSummary;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.Label lblTax;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Panel pnlPayment;
        private System.Windows.Forms.Label lblCashReceived;
        private System.Windows.Forms.TextBox txtCashReceived;
        private System.Windows.Forms.Label lblChange;
        private System.Windows.Forms.Label lblChangeAmount;
        private POS.UI.Controls.FlatButton btnConfirm;
        private POS.UI.Controls.FlatButton btnCancel;
    }
}
