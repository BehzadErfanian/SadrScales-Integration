#nullable enable

namespace SadrScales.Integration.SampleApp
{
    partial class SalesReportsControl
    {
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.TextBox txtIdentify;
        private System.Windows.Forms.TextBox txtScaleId;
        private System.Windows.Forms.TextBox txtPlu;
        private System.Windows.Forms.TextBox txtFid;
        private System.Windows.Forms.Label lblIdentify;
        private System.Windows.Forms.Label lblScaleId;
        private System.Windows.Forms.Label lblPlu;
        private System.Windows.Forms.Label lblFid;
        private System.Windows.Forms.NumericUpDown nudPage;
        private System.Windows.Forms.NumericUpDown nudPageSize;
        private System.Windows.Forms.Label lblPage;
        private System.Windows.Forms.Label lblPageSize;
        private System.Windows.Forms.Button btnToday;
        private System.Windows.Forms.Button btnWeek;
        private System.Windows.Forms.Button btnMonth;
        private System.Windows.Forms.Button btnClearDates;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnDaily;
        private System.Windows.Forms.Button btnByScale;
        private System.Windows.Forms.Button btnByItem;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.DataGridView dgvResults;

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
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.lblStart = new System.Windows.Forms.Label();
            this.lblEnd = new System.Windows.Forms.Label();
            this.txtIdentify = new System.Windows.Forms.TextBox();
            this.txtScaleId = new System.Windows.Forms.TextBox();
            this.txtPlu = new System.Windows.Forms.TextBox();
            this.txtFid = new System.Windows.Forms.TextBox();
            this.lblIdentify = new System.Windows.Forms.Label();
            this.lblScaleId = new System.Windows.Forms.Label();
            this.lblPlu = new System.Windows.Forms.Label();
            this.lblFid = new System.Windows.Forms.Label();
            this.nudPage = new System.Windows.Forms.NumericUpDown();
            this.nudPageSize = new System.Windows.Forms.NumericUpDown();
            this.lblPage = new System.Windows.Forms.Label();
            this.lblPageSize = new System.Windows.Forms.Label();
            this.btnToday = new System.Windows.Forms.Button();
            this.btnWeek = new System.Windows.Forms.Button();
            this.btnMonth = new System.Windows.Forms.Button();
            this.btnClearDates = new System.Windows.Forms.Button();
            this.btnQuery = new System.Windows.Forms.Button();
            this.btnDaily = new System.Windows.Forms.Button();
            this.btnByScale = new System.Windows.Forms.Button();
            this.btnByItem = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblSummary = new System.Windows.Forms.Label();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.nudPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPageSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.SuspendLayout();
            //
            // date filters
            //
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(4, 9);
            this.lblStart.Text = "Start:";
            this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStart.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtpStart.Location = new System.Drawing.Point(45, 5);
            this.dtpStart.ShowCheckBox = true;
            this.dtpStart.Checked = false;
            this.dtpStart.Size = new System.Drawing.Size(150, 20);

            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(205, 9);
            this.lblEnd.Text = "End exclusive:";
            this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEnd.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtpEnd.Location = new System.Drawing.Point(286, 5);
            this.dtpEnd.ShowCheckBox = true;
            this.dtpEnd.Checked = false;
            this.dtpEnd.Size = new System.Drawing.Size(150, 20);

            this.btnToday.Location = new System.Drawing.Point(447, 3);
            this.btnToday.Size = new System.Drawing.Size(65, 24);
            this.btnToday.Text = "Today";
            this.btnToday.Click += new System.EventHandler(this.btnToday_Click);
            this.btnWeek.Location = new System.Drawing.Point(518, 3);
            this.btnWeek.Size = new System.Drawing.Size(75, 24);
            this.btnWeek.Text = "This week";
            this.btnWeek.Click += new System.EventHandler(this.btnWeek_Click);
            this.btnMonth.Location = new System.Drawing.Point(599, 3);
            this.btnMonth.Size = new System.Drawing.Size(100, 24);
            this.btnMonth.Text = "Persian month";
            this.btnMonth.Click += new System.EventHandler(this.btnMonth_Click);
            this.btnClearDates.Location = new System.Drawing.Point(705, 3);
            this.btnClearDates.Size = new System.Drawing.Size(75, 24);
            this.btnClearDates.Text = "All dates";
            this.btnClearDates.Click += new System.EventHandler(this.btnClearDates_Click);

            //
            // second row filters
            //
            this.lblIdentify.AutoSize = true;
            this.lblIdentify.Location = new System.Drawing.Point(4, 39);
            this.lblIdentify.Text = "Identify:";
            this.txtIdentify.Location = new System.Drawing.Point(52, 36);
            this.txtIdentify.MaxLength = 50;
            this.txtIdentify.Size = new System.Drawing.Size(120, 20);

            this.lblScaleId.AutoSize = true;
            this.lblScaleId.Location = new System.Drawing.Point(184, 39);
            this.lblScaleId.Text = "Scale ID:";
            this.txtScaleId.Location = new System.Drawing.Point(235, 36);
            this.txtScaleId.Size = new System.Drawing.Size(50, 20);

            this.lblPlu.AutoSize = true;
            this.lblPlu.Location = new System.Drawing.Point(297, 39);
            this.lblPlu.Text = "PLU:";
            this.txtPlu.Location = new System.Drawing.Point(328, 36);
            this.txtPlu.Size = new System.Drawing.Size(65, 20);

            this.lblFid.AutoSize = true;
            this.lblFid.Location = new System.Drawing.Point(405, 39);
            this.lblFid.Text = "FID:";
            this.txtFid.Location = new System.Drawing.Point(432, 36);
            this.txtFid.Size = new System.Drawing.Size(65, 20);

            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(510, 39);
            this.lblPage.Text = "Page:";
            this.nudPage.Location = new System.Drawing.Point(546, 36);
            this.nudPage.Minimum = 1;
            this.nudPage.Maximum = 100000;
            this.nudPage.Value = 1;
            this.nudPage.Size = new System.Drawing.Size(60, 20);

            this.lblPageSize.AutoSize = true;
            this.lblPageSize.Location = new System.Drawing.Point(618, 39);
            this.lblPageSize.Text = "Size:";
            this.nudPageSize.Location = new System.Drawing.Point(651, 36);
            this.nudPageSize.Minimum = 1;
            this.nudPageSize.Maximum = 5000;
            this.nudPageSize.Value = 200;
            this.nudPageSize.Size = new System.Drawing.Size(65, 20);

            //
            // actions
            //
            this.btnQuery.Location = new System.Drawing.Point(7, 68);
            this.btnQuery.Size = new System.Drawing.Size(100, 27);
            this.btnQuery.Text = "Query page";
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            this.btnDaily.Location = new System.Drawing.Point(113, 68);
            this.btnDaily.Size = new System.Drawing.Size(100, 27);
            this.btnDaily.Text = "Daily report";
            this.btnDaily.Click += new System.EventHandler(this.btnDaily_Click);
            this.btnByScale.Location = new System.Drawing.Point(219, 68);
            this.btnByScale.Size = new System.Drawing.Size(100, 27);
            this.btnByScale.Text = "By scale";
            this.btnByScale.Click += new System.EventHandler(this.btnByScale_Click);
            this.btnByItem.Location = new System.Drawing.Point(325, 68);
            this.btnByItem.Size = new System.Drawing.Size(100, 27);
            this.btnByItem.Text = "By item";
            this.btnByItem.Click += new System.EventHandler(this.btnByItem_Click);

            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Location = new System.Drawing.Point(442, 72);
            this.lblStatus.Size = new System.Drawing.Size(665, 20);
            this.lblStatus.Text = "Read-only sales query/report sample. Feed cursor is not changed.";

            this.lblSummary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSummary.Location = new System.Drawing.Point(7, 103);
            this.lblSummary.Size = new System.Drawing.Size(1100, 22);
            this.lblSummary.Text = "Summary: —";

            this.dgvResults.AllowUserToAddRows = false;
            this.dgvResults.AllowUserToDeleteRows = false;
            this.dgvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Location = new System.Drawing.Point(7, 128);
            this.dgvResults.MultiSelect = false;
            this.dgvResults.ReadOnly = true;
            this.dgvResults.RowHeadersVisible = false;
            this.dgvResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResults.Size = new System.Drawing.Size(1100, 425);

            //
            // SalesReportsControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvResults);
            this.Controls.Add(this.lblSummary);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnByItem);
            this.Controls.Add(this.btnByScale);
            this.Controls.Add(this.btnDaily);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.nudPageSize);
            this.Controls.Add(this.lblPageSize);
            this.Controls.Add(this.nudPage);
            this.Controls.Add(this.lblPage);
            this.Controls.Add(this.txtFid);
            this.Controls.Add(this.lblFid);
            this.Controls.Add(this.txtPlu);
            this.Controls.Add(this.lblPlu);
            this.Controls.Add(this.txtScaleId);
            this.Controls.Add(this.lblScaleId);
            this.Controls.Add(this.txtIdentify);
            this.Controls.Add(this.lblIdentify);
            this.Controls.Add(this.btnClearDates);
            this.Controls.Add(this.btnMonth);
            this.Controls.Add(this.btnWeek);
            this.Controls.Add(this.btnToday);
            this.Controls.Add(this.dtpEnd);
            this.Controls.Add(this.lblEnd);
            this.Controls.Add(this.dtpStart);
            this.Controls.Add(this.lblStart);
            this.Name = "SalesReportsControl";
            this.Size = new System.Drawing.Size(1120, 561);
            ((System.ComponentModel.ISupportInitialize)(this.nudPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPageSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
