#nullable enable

namespace SadrScales.Integration.SampleApp
{
    partial class DemoDataControl
    {
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.Button btnInspect;
        private System.Windows.Forms.Label lblSafety;
        private System.Windows.Forms.Label lblConfirmDatabase;
        private System.Windows.Forms.TextBox txtConfirmDatabase;
        private System.Windows.Forms.Button btnInitializeMarker;
        private System.Windows.Forms.Label lblSeed;
        private System.Windows.Forms.NumericUpDown nudSeed;
        private System.Windows.Forms.Button btnRandomSeed;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.DataGridView dgvPreview;

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
            this.btnInspect = new System.Windows.Forms.Button();
            this.lblSafety = new System.Windows.Forms.Label();
            this.lblConfirmDatabase = new System.Windows.Forms.Label();
            this.txtConfirmDatabase = new System.Windows.Forms.TextBox();
            this.btnInitializeMarker = new System.Windows.Forms.Button();
            this.lblSeed = new System.Windows.Forms.Label();
            this.nudSeed = new System.Windows.Forms.NumericUpDown();
            this.btnRandomSeed = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.dgvPreview = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.nudSeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).BeginInit();
            this.SuspendLayout();
            //
            // btnInspect
            //
            this.btnInspect.Location = new System.Drawing.Point(6, 6);
            this.btnInspect.Name = "btnInspect";
            this.btnInspect.Size = new System.Drawing.Size(105, 26);
            this.btnInspect.TabIndex = 0;
            this.btnInspect.Text = "Inspect database";
            this.btnInspect.UseVisualStyleBackColor = true;
            this.btnInspect.Click += new System.EventHandler(this.btnInspect_Click);
            //
            // lblSafety
            //
            this.lblSafety.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSafety.Location = new System.Drawing.Point(123, 7);
            this.lblSafety.Name = "lblSafety";
            this.lblSafety.Size = new System.Drawing.Size(985, 42);
            this.lblSafety.TabIndex = 1;
            this.lblSafety.Text = "Inspect the target database before any Demo write. Demo generation is disabled by default.";
            //
            // lblConfirmDatabase
            //
            this.lblConfirmDatabase.AutoSize = true;
            this.lblConfirmDatabase.Location = new System.Drawing.Point(6, 48);
            this.lblConfirmDatabase.Name = "lblConfirmDatabase";
            this.lblConfirmDatabase.Size = new System.Drawing.Size(137, 13);
            this.lblConfirmDatabase.TabIndex = 2;
            this.lblConfirmDatabase.Text = "Type exact database name:";
            //
            // txtConfirmDatabase
            //
            this.txtConfirmDatabase.Location = new System.Drawing.Point(149, 45);
            this.txtConfirmDatabase.Name = "txtConfirmDatabase";
            this.txtConfirmDatabase.Size = new System.Drawing.Size(210, 20);
            this.txtConfirmDatabase.TabIndex = 3;
            //
            // btnInitializeMarker
            //
            this.btnInitializeMarker.Enabled = false;
            this.btnInitializeMarker.Location = new System.Drawing.Point(369, 42);
            this.btnInitializeMarker.Name = "btnInitializeMarker";
            this.btnInitializeMarker.Size = new System.Drawing.Size(145, 26);
            this.btnInitializeMarker.TabIndex = 4;
            this.btnInitializeMarker.Text = "Initialize Demo marker";
            this.btnInitializeMarker.UseVisualStyleBackColor = true;
            this.btnInitializeMarker.Click += new System.EventHandler(this.btnInitializeMarker_Click);
            //
            // lblSeed
            //
            this.lblSeed.AutoSize = true;
            this.lblSeed.Location = new System.Drawing.Point(6, 83);
            this.lblSeed.Name = "lblSeed";
            this.lblSeed.Size = new System.Drawing.Size(35, 13);
            this.lblSeed.TabIndex = 5;
            this.lblSeed.Text = "Seed:";
            //
            // nudSeed
            //
            this.nudSeed.Location = new System.Drawing.Point(47, 80);
            this.nudSeed.Maximum = new decimal(new int[] { 2147483647, 0, 0, 0 });
            this.nudSeed.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudSeed.Name = "nudSeed";
            this.nudSeed.Size = new System.Drawing.Size(105, 20);
            this.nudSeed.TabIndex = 6;
            this.nudSeed.Value = new decimal(new int[] { 12345, 0, 0, 0 });
            //
            // btnRandomSeed
            //
            this.btnRandomSeed.Location = new System.Drawing.Point(162, 77);
            this.btnRandomSeed.Name = "btnRandomSeed";
            this.btnRandomSeed.Size = new System.Drawing.Size(95, 26);
            this.btnRandomSeed.TabIndex = 7;
            this.btnRandomSeed.Text = "Random seed";
            this.btnRandomSeed.UseVisualStyleBackColor = true;
            this.btnRandomSeed.Click += new System.EventHandler(this.btnRandomSeed_Click);
            //
            // btnPreview
            //
            this.btnPreview.Location = new System.Drawing.Point(267, 77);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(95, 26);
            this.btnPreview.TabIndex = 8;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            //
            // btnGenerate
            //
            this.btnGenerate.Enabled = false;
            this.btnGenerate.Location = new System.Drawing.Point(372, 77);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(120, 26);
            this.btnGenerate.TabIndex = 9;
            this.btnGenerate.Text = "Generate Demo Data";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            //
            // btnReset
            //
            this.btnReset.Enabled = false;
            this.btnReset.Location = new System.Drawing.Point(502, 77);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(110, 26);
            this.btnReset.TabIndex = 10;
            this.btnReset.Text = "Reset Demo Data";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            //
            // lblResult
            //
            this.lblResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResult.Location = new System.Drawing.Point(626, 82);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(482, 35);
            this.lblResult.TabIndex = 11;
            this.lblResult.Text = "Seed 12345 is deterministic. Preview is always safe/read-only.";
            //
            // dgvPreview
            //
            this.dgvPreview.AllowUserToAddRows = false;
            this.dgvPreview.AllowUserToDeleteRows = false;
            this.dgvPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPreview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPreview.Location = new System.Drawing.Point(6, 122);
            this.dgvPreview.MultiSelect = false;
            this.dgvPreview.Name = "dgvPreview";
            this.dgvPreview.ReadOnly = true;
            this.dgvPreview.RowHeadersVisible = false;
            this.dgvPreview.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPreview.Size = new System.Drawing.Size(1102, 431);
            this.dgvPreview.TabIndex = 12;
            //
            // DemoDataControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvPreview);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.btnRandomSeed);
            this.Controls.Add(this.nudSeed);
            this.Controls.Add(this.lblSeed);
            this.Controls.Add(this.btnInitializeMarker);
            this.Controls.Add(this.txtConfirmDatabase);
            this.Controls.Add(this.lblConfirmDatabase);
            this.Controls.Add(this.lblSafety);
            this.Controls.Add(this.btnInspect);
            this.Name = "DemoDataControl";
            this.Size = new System.Drawing.Size(1120, 561);
            ((System.ComponentModel.ISupportInitialize)(this.nudSeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
