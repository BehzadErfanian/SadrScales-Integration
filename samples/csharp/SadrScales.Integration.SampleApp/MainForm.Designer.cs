namespace SadrScales.Integration.SampleApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.Label lblConnectionString;
        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.Label lblBarcode;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Button btnLookup;
        private System.Windows.Forms.CheckBox chkEnableWrites;
        private System.Windows.Forms.Button btnAck;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.SplitContainer splitInvoice;
        private System.Windows.Forms.DataGridView dgvInvoice;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.Label lblInvoiceHeader;
        private System.Windows.Forms.Label lblInvoiceItems;

        #region Windows Form Designer generated code

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblConnectionString = new System.Windows.Forms.Label();
            this.txtConnectionString = new System.Windows.Forms.TextBox();
            this.lblBarcode = new System.Windows.Forms.Label();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.btnLookup = new System.Windows.Forms.Button();
            this.chkEnableWrites = new System.Windows.Forms.CheckBox();
            this.btnAck = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.splitInvoice = new System.Windows.Forms.SplitContainer();
            this.dgvInvoice = new System.Windows.Forms.DataGridView();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.lblInvoiceHeader = new System.Windows.Forms.Label();
            this.lblInvoiceItems = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitInvoice)).BeginInit();
            this.splitInvoice.Panel1.SuspendLayout();
            this.splitInvoice.Panel2.SuspendLayout();
            this.splitInvoice.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            //
            // lblConnectionString
            //
            this.lblConnectionString.AutoSize = true;
            this.lblConnectionString.Location = new System.Drawing.Point(12, 15);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(96, 13);
            this.lblConnectionString.TabIndex = 0;
            this.lblConnectionString.Text = "SQL connection:";
            //
            // txtConnectionString
            //
            this.txtConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConnectionString.Location = new System.Drawing.Point(114, 12);
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.Size = new System.Drawing.Size(1054, 20);
            this.txtConnectionString.TabIndex = 1;
            //
            // lblBarcode
            //
            this.lblBarcode.AutoSize = true;
            this.lblBarcode.Location = new System.Drawing.Point(12, 48);
            this.lblBarcode.Name = "lblBarcode";
            this.lblBarcode.Size = new System.Drawing.Size(74, 13);
            this.lblBarcode.TabIndex = 2;
            this.lblBarcode.Text = "TotalBarcode:";
            //
            // txtBarcode
            //
            this.txtBarcode.Location = new System.Drawing.Point(114, 45);
            this.txtBarcode.MaxLength = 14;
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(190, 20);
            this.txtBarcode.TabIndex = 3;
            //
            // btnLookup
            //
            this.btnLookup.Location = new System.Drawing.Point(315, 43);
            this.btnLookup.Name = "btnLookup";
            this.btnLookup.Size = new System.Drawing.Size(105, 25);
            this.btnLookup.TabIndex = 4;
            this.btnLookup.Text = "Lookup invoice";
            this.btnLookup.UseVisualStyleBackColor = true;
            this.btnLookup.Click += new System.EventHandler(this.btnLookup_Click);
            //
            // chkEnableWrites
            //
            this.chkEnableWrites.AutoSize = true;
            this.chkEnableWrites.Location = new System.Drawing.Point(440, 48);
            this.chkEnableWrites.Name = "chkEnableWrites";
            this.chkEnableWrites.Size = new System.Drawing.Size(119, 17);
            this.chkEnableWrites.TabIndex = 5;
            this.chkEnableWrites.Text = "Enable ACK write";
            this.chkEnableWrites.UseVisualStyleBackColor = true;
            this.chkEnableWrites.CheckedChanged += new System.EventHandler(this.chkEnableWrites_CheckedChanged);
            //
            // btnAck
            //
            this.btnAck.Enabled = false;
            this.btnAck.Location = new System.Drawing.Point(570, 43);
            this.btnAck.Name = "btnAck";
            this.btnAck.Size = new System.Drawing.Size(105, 25);
            this.btnAck.TabIndex = 6;
            this.btnAck.Text = "ACK invoice";
            this.btnAck.UseVisualStyleBackColor = true;
            this.btnAck.Click += new System.EventHandler(this.btnAck_Click);
            //
            // lblStatus
            //
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.Location = new System.Drawing.Point(694, 48);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(474, 17);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "Ready";
            //
            // splitInvoice
            //
            this.splitInvoice.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.splitInvoice.Location = new System.Drawing.Point(12, 83);
            this.splitInvoice.Name = "splitInvoice";
            this.splitInvoice.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //
            // splitInvoice.Panel1
            //
            this.splitInvoice.Panel1.Controls.Add(this.dgvInvoice);
            this.splitInvoice.Panel1.Controls.Add(this.lblInvoiceHeader);
            //
            // splitInvoice.Panel2
            //
            this.splitInvoice.Panel2.Controls.Add(this.dgvItems);
            this.splitInvoice.Panel2.Controls.Add(this.lblInvoiceItems);
            this.splitInvoice.Size = new System.Drawing.Size(1156, 605);
            this.splitInvoice.SplitterDistance = 190;
            this.splitInvoice.TabIndex = 8;
            //
            // dgvInvoice
            //
            this.dgvInvoice.AllowUserToAddRows = false;
            this.dgvInvoice.AllowUserToDeleteRows = false;
            this.dgvInvoice.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvInvoice.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvInvoice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInvoice.Location = new System.Drawing.Point(0, 24);
            this.dgvInvoice.MultiSelect = false;
            this.dgvInvoice.Name = "dgvInvoice";
            this.dgvInvoice.ReadOnly = true;
            this.dgvInvoice.RowHeadersVisible = false;
            this.dgvInvoice.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInvoice.Size = new System.Drawing.Size(1156, 166);
            this.dgvInvoice.TabIndex = 1;
            //
            // dgvItems
            //
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Location = new System.Drawing.Point(0, 24);
            this.dgvItems.MultiSelect = false;
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.ReadOnly = true;
            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItems.Size = new System.Drawing.Size(1156, 387);
            this.dgvItems.TabIndex = 1;
            //
            // lblInvoiceHeader
            //
            this.lblInvoiceHeader.AutoSize = true;
            this.lblInvoiceHeader.Location = new System.Drawing.Point(0, 5);
            this.lblInvoiceHeader.Name = "lblInvoiceHeader";
            this.lblInvoiceHeader.Size = new System.Drawing.Size(78, 13);
            this.lblInvoiceHeader.TabIndex = 0;
            this.lblInvoiceHeader.Text = "Invoice header";
            //
            // lblInvoiceItems
            //
            this.lblInvoiceItems.AutoSize = true;
            this.lblInvoiceItems.Location = new System.Drawing.Point(0, 5);
            this.lblInvoiceItems.Name = "lblInvoiceItems";
            this.lblInvoiceItems.Size = new System.Drawing.Size(69, 13);
            this.lblInvoiceItems.TabIndex = 0;
            this.lblInvoiceItems.Text = "Invoice items";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 700);
            this.Controls.Add(this.splitInvoice);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnAck);
            this.Controls.Add(this.chkEnableWrites);
            this.Controls.Add(this.btnLookup);
            this.Controls.Add(this.txtBarcode);
            this.Controls.Add(this.lblBarcode);
            this.Controls.Add(this.txtConnectionString);
            this.Controls.Add(this.lblConnectionString);
            this.MinimumSize = new System.Drawing.Size(900, 550);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sadr Scales Integration Sample — Structured Invoices";
            this.splitInvoice.Panel1.ResumeLayout(false);
            this.splitInvoice.Panel1.PerformLayout();
            this.splitInvoice.Panel2.ResumeLayout(false);
            this.splitInvoice.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitInvoice)).EndInit();
            this.splitInvoice.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
