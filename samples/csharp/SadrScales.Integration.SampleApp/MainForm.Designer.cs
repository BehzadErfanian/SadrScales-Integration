#nullable enable

namespace SadrScales.Integration.SampleApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.Label lblConnectionString;
        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabInvoices;
        private System.Windows.Forms.TabPage tabScales;
        private System.Windows.Forms.Label lblBarcode;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Button btnLookup;
        private System.Windows.Forms.CheckBox chkEnableWrites;
        private System.Windows.Forms.Button btnAck;
        private System.Windows.Forms.Label lblInvoiceStatus;
        private System.Windows.Forms.SplitContainer splitInvoice;
        private System.Windows.Forms.DataGridView dgvInvoice;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.Label lblInvoiceHeader;
        private System.Windows.Forms.Label lblInvoiceItems;
        private System.Windows.Forms.Button btnRefreshScales;
        private System.Windows.Forms.Label lblScaleId;
        private System.Windows.Forms.NumericUpDown nudScaleId;
        private System.Windows.Forms.CheckBox chkEnableScaleWrites;
        private System.Windows.Forms.Button btnRequestItemResend;
        private System.Windows.Forms.Button btnRequestHotKeyResend;
        private System.Windows.Forms.Label lblScaleStatus;
        private System.Windows.Forms.DataGridView dgvScales;

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
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabInvoices = new System.Windows.Forms.TabPage();
            this.splitInvoice = new System.Windows.Forms.SplitContainer();
            this.dgvInvoice = new System.Windows.Forms.DataGridView();
            this.lblInvoiceHeader = new System.Windows.Forms.Label();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.lblInvoiceItems = new System.Windows.Forms.Label();
            this.lblInvoiceStatus = new System.Windows.Forms.Label();
            this.btnAck = new System.Windows.Forms.Button();
            this.chkEnableWrites = new System.Windows.Forms.CheckBox();
            this.btnLookup = new System.Windows.Forms.Button();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.lblBarcode = new System.Windows.Forms.Label();
            this.tabScales = new System.Windows.Forms.TabPage();
            this.dgvScales = new System.Windows.Forms.DataGridView();
            this.lblScaleStatus = new System.Windows.Forms.Label();
            this.btnRequestHotKeyResend = new System.Windows.Forms.Button();
            this.btnRequestItemResend = new System.Windows.Forms.Button();
            this.chkEnableScaleWrites = new System.Windows.Forms.CheckBox();
            this.nudScaleId = new System.Windows.Forms.NumericUpDown();
            this.lblScaleId = new System.Windows.Forms.Label();
            this.btnRefreshScales = new System.Windows.Forms.Button();
            this.tabMain.SuspendLayout();
            this.tabInvoices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitInvoice)).BeginInit();
            this.splitInvoice.Panel1.SuspendLayout();
            this.splitInvoice.Panel2.SuspendLayout();
            this.splitInvoice.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.tabScales.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScaleId)).BeginInit();
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
            // tabMain
            // 
            this.tabMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tabMain.Controls.Add(this.tabInvoices);
            this.tabMain.Controls.Add(this.tabScales);
            this.tabMain.Location = new System.Drawing.Point(12, 43);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1156, 645);
            this.tabMain.TabIndex = 2;
            // 
            // tabInvoices
            // 
            this.tabInvoices.Controls.Add(this.splitInvoice);
            this.tabInvoices.Controls.Add(this.lblInvoiceStatus);
            this.tabInvoices.Controls.Add(this.btnAck);
            this.tabInvoices.Controls.Add(this.chkEnableWrites);
            this.tabInvoices.Controls.Add(this.btnLookup);
            this.tabInvoices.Controls.Add(this.txtBarcode);
            this.tabInvoices.Controls.Add(this.lblBarcode);
            this.tabInvoices.Location = new System.Drawing.Point(4, 22);
            this.tabInvoices.Name = "tabInvoices";
            this.tabInvoices.Padding = new System.Windows.Forms.Padding(8);
            this.tabInvoices.Size = new System.Drawing.Size(1148, 619);
            this.tabInvoices.TabIndex = 0;
            this.tabInvoices.Text = "Invoices";
            this.tabInvoices.UseVisualStyleBackColor = true;
            // 
            // lblBarcode
            // 
            this.lblBarcode.AutoSize = true;
            this.lblBarcode.Location = new System.Drawing.Point(11, 17);
            this.lblBarcode.Name = "lblBarcode";
            this.lblBarcode.Size = new System.Drawing.Size(74, 13);
            this.lblBarcode.TabIndex = 0;
            this.lblBarcode.Text = "TotalBarcode:";
            // 
            // txtBarcode
            // 
            this.txtBarcode.Location = new System.Drawing.Point(99, 14);
            this.txtBarcode.MaxLength = 14;
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(190, 20);
            this.txtBarcode.TabIndex = 1;
            // 
            // btnLookup
            // 
            this.btnLookup.Location = new System.Drawing.Point(300, 12);
            this.btnLookup.Name = "btnLookup";
            this.btnLookup.Size = new System.Drawing.Size(105, 25);
            this.btnLookup.TabIndex = 2;
            this.btnLookup.Text = "Lookup invoice";
            this.btnLookup.UseVisualStyleBackColor = true;
            this.btnLookup.Click += new System.EventHandler(this.btnLookup_Click);
            // 
            // chkEnableWrites
            // 
            this.chkEnableWrites.AutoSize = true;
            this.chkEnableWrites.Location = new System.Drawing.Point(425, 17);
            this.chkEnableWrites.Name = "chkEnableWrites";
            this.chkEnableWrites.Size = new System.Drawing.Size(119, 17);
            this.chkEnableWrites.TabIndex = 3;
            this.chkEnableWrites.Text = "Enable ACK write";
            this.chkEnableWrites.UseVisualStyleBackColor = true;
            this.chkEnableWrites.CheckedChanged += new System.EventHandler(this.chkEnableWrites_CheckedChanged);
            // 
            // btnAck
            // 
            this.btnAck.Enabled = false;
            this.btnAck.Location = new System.Drawing.Point(555, 12);
            this.btnAck.Name = "btnAck";
            this.btnAck.Size = new System.Drawing.Size(105, 25);
            this.btnAck.TabIndex = 4;
            this.btnAck.Text = "ACK invoice";
            this.btnAck.UseVisualStyleBackColor = true;
            this.btnAck.Click += new System.EventHandler(this.btnAck_Click);
            // 
            // lblInvoiceStatus
            // 
            this.lblInvoiceStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInvoiceStatus.AutoEllipsis = true;
            this.lblInvoiceStatus.Location = new System.Drawing.Point(676, 17);
            this.lblInvoiceStatus.Name = "lblInvoiceStatus";
            this.lblInvoiceStatus.Size = new System.Drawing.Size(457, 17);
            this.lblInvoiceStatus.TabIndex = 5;
            this.lblInvoiceStatus.Text = "Ready";
            // 
            // splitInvoice
            // 
            this.splitInvoice.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.splitInvoice.Location = new System.Drawing.Point(11, 48);
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
            this.splitInvoice.Size = new System.Drawing.Size(1122, 558);
            this.splitInvoice.SplitterDistance = 180;
            this.splitInvoice.TabIndex = 6;
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
            this.dgvInvoice.Size = new System.Drawing.Size(1122, 156);
            this.dgvInvoice.TabIndex = 1;
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
            this.dgvItems.Size = new System.Drawing.Size(1122, 350);
            this.dgvItems.TabIndex = 1;
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
            // tabScales
            // 
            this.tabScales.Controls.Add(this.dgvScales);
            this.tabScales.Controls.Add(this.lblScaleStatus);
            this.tabScales.Controls.Add(this.btnRequestHotKeyResend);
            this.tabScales.Controls.Add(this.btnRequestItemResend);
            this.tabScales.Controls.Add(this.chkEnableScaleWrites);
            this.tabScales.Controls.Add(this.nudScaleId);
            this.tabScales.Controls.Add(this.lblScaleId);
            this.tabScales.Controls.Add(this.btnRefreshScales);
            this.tabScales.Location = new System.Drawing.Point(4, 22);
            this.tabScales.Name = "tabScales";
            this.tabScales.Padding = new System.Windows.Forms.Padding(8);
            this.tabScales.Size = new System.Drawing.Size(1148, 619);
            this.tabScales.TabIndex = 1;
            this.tabScales.Text = "Scales";
            this.tabScales.UseVisualStyleBackColor = true;
            // 
            // btnRefreshScales
            // 
            this.btnRefreshScales.Location = new System.Drawing.Point(11, 12);
            this.btnRefreshScales.Name = "btnRefreshScales";
            this.btnRefreshScales.Size = new System.Drawing.Size(110, 25);
            this.btnRefreshScales.TabIndex = 0;
            this.btnRefreshScales.Text = "Refresh scales";
            this.btnRefreshScales.UseVisualStyleBackColor = true;
            this.btnRefreshScales.Click += new System.EventHandler(this.btnRefreshScales_Click);
            // 
            // lblScaleId
            // 
            this.lblScaleId.AutoSize = true;
            this.lblScaleId.Location = new System.Drawing.Point(140, 18);
            this.lblScaleId.Name = "lblScaleId";
            this.lblScaleId.Size = new System.Drawing.Size(49, 13);
            this.lblScaleId.TabIndex = 1;
            this.lblScaleId.Text = "Scale ID:";
            // 
            // nudScaleId
            // 
            this.nudScaleId.Location = new System.Drawing.Point(195, 15);
            this.nudScaleId.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            this.nudScaleId.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudScaleId.Name = "nudScaleId";
            this.nudScaleId.Size = new System.Drawing.Size(55, 20);
            this.nudScaleId.TabIndex = 2;
            this.nudScaleId.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // chkEnableScaleWrites
            // 
            this.chkEnableScaleWrites.AutoSize = true;
            this.chkEnableScaleWrites.Location = new System.Drawing.Point(270, 17);
            this.chkEnableScaleWrites.Name = "chkEnableScaleWrites";
            this.chkEnableScaleWrites.Size = new System.Drawing.Size(127, 17);
            this.chkEnableScaleWrites.TabIndex = 3;
            this.chkEnableScaleWrites.Text = "Enable resend writes";
            this.chkEnableScaleWrites.UseVisualStyleBackColor = true;
            this.chkEnableScaleWrites.CheckedChanged += new System.EventHandler(this.chkEnableScaleWrites_CheckedChanged);
            // 
            // btnRequestItemResend
            // 
            this.btnRequestItemResend.Enabled = false;
            this.btnRequestItemResend.Location = new System.Drawing.Point(410, 12);
            this.btnRequestItemResend.Name = "btnRequestItemResend";
            this.btnRequestItemResend.Size = new System.Drawing.Size(135, 25);
            this.btnRequestItemResend.TabIndex = 4;
            this.btnRequestItemResend.Text = "Request item resend";
            this.btnRequestItemResend.UseVisualStyleBackColor = true;
            this.btnRequestItemResend.Click += new System.EventHandler(this.btnRequestItemResend_Click);
            // 
            // btnRequestHotKeyResend
            // 
            this.btnRequestHotKeyResend.Enabled = false;
            this.btnRequestHotKeyResend.Location = new System.Drawing.Point(552, 12);
            this.btnRequestHotKeyResend.Name = "btnRequestHotKeyResend";
            this.btnRequestHotKeyResend.Size = new System.Drawing.Size(145, 25);
            this.btnRequestHotKeyResend.TabIndex = 5;
            this.btnRequestHotKeyResend.Text = "Request HotKey resend";
            this.btnRequestHotKeyResend.UseVisualStyleBackColor = true;
            this.btnRequestHotKeyResend.Click += new System.EventHandler(this.btnRequestHotKeyResend_Click);
            // 
            // lblScaleStatus
            // 
            this.lblScaleStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblScaleStatus.AutoEllipsis = true;
            this.lblScaleStatus.Location = new System.Drawing.Point(715, 17);
            this.lblScaleStatus.Name = "lblScaleStatus";
            this.lblScaleStatus.Size = new System.Drawing.Size(418, 30);
            this.lblScaleStatus.TabIndex = 6;
            this.lblScaleStatus.Text = "Ready. Resend writes only request a later eligible AutoSend cycle.";
            // 
            // dgvScales
            // 
            this.dgvScales.AllowUserToAddRows = false;
            this.dgvScales.AllowUserToDeleteRows = false;
            this.dgvScales.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvScales.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvScales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScales.Location = new System.Drawing.Point(11, 55);
            this.dgvScales.MultiSelect = false;
            this.dgvScales.Name = "dgvScales";
            this.dgvScales.ReadOnly = true;
            this.dgvScales.RowHeadersVisible = false;
            this.dgvScales.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvScales.Size = new System.Drawing.Size(1122, 551);
            this.dgvScales.TabIndex = 7;
            this.dgvScales.SelectionChanged += new System.EventHandler(this.dgvScales_SelectionChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 700);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.txtConnectionString);
            this.Controls.Add(this.lblConnectionString);
            this.MinimumSize = new System.Drawing.Size(900, 550);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sadr Scales Integration — Developer Sample";
            this.tabMain.ResumeLayout(false);
            this.tabInvoices.ResumeLayout(false);
            this.tabInvoices.PerformLayout();
            this.splitInvoice.Panel1.ResumeLayout(false);
            this.splitInvoice.Panel1.PerformLayout();
            this.splitInvoice.Panel2.ResumeLayout(false);
            this.splitInvoice.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitInvoice)).EndInit();
            this.splitInvoice.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.tabScales.ResumeLayout(false);
            this.tabScales.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScaleId)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
