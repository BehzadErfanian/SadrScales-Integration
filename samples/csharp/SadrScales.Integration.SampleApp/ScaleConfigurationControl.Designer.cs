#nullable enable

namespace SadrScales.Integration.SampleApp
{
    partial class ScaleConfigurationControl
    {
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.CheckBox chkEnableConfigurationWrites;
        private System.Windows.Forms.Label lblWriteWarning;
        private System.Windows.Forms.TabControl tabConfiguration;
        private System.Windows.Forms.TabPage tabAssignments;
        private System.Windows.Forms.TabPage tabMappings;
        private System.Windows.Forms.TabPage tabHotKeys;

        private System.Windows.Forms.Button btnLoadAssignments;
        private System.Windows.Forms.Label lblAssignmentScale;
        private System.Windows.Forms.NumericUpDown nudAssignmentScale;
        private System.Windows.Forms.Label lblAssignmentGroups;
        private System.Windows.Forms.TextBox txtAssignmentGroups;
        private System.Windows.Forms.Button btnReplaceAssignments;
        private System.Windows.Forms.Label lblAssignmentStatus;
        private System.Windows.Forms.DataGridView dgvAssignments;

        private System.Windows.Forms.Button btnLoadMappings;
        private System.Windows.Forms.Label lblMappingScale;
        private System.Windows.Forms.NumericUpDown nudMappingScale;
        private System.Windows.Forms.Label lblMappingDestination;
        private System.Windows.Forms.NumericUpDown nudMappingDestination;
        private System.Windows.Forms.Button btnAddMapping;
        private System.Windows.Forms.Button btnRemoveMapping;
        private System.Windows.Forms.Button btnReplaceMappings;
        private System.Windows.Forms.Button btnCopyMappings;
        private System.Windows.Forms.Label lblMappingStatus;
        private System.Windows.Forms.DataGridView dgvMappings;

        private System.Windows.Forms.Button btnLoadHotKeys;
        private System.Windows.Forms.Label lblHotKeyGroup;
        private System.Windows.Forms.TextBox txtHotKeyGroup;
        private System.Windows.Forms.Button btnAddHotKey;
        private System.Windows.Forms.Button btnRemoveHotKey;
        private System.Windows.Forms.Button btnReplaceHotKeys;
        private System.Windows.Forms.Label lblHotKeyStatus;
        private System.Windows.Forms.DataGridView dgvHotKeys;

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
            this.chkEnableConfigurationWrites = new System.Windows.Forms.CheckBox();
            this.lblWriteWarning = new System.Windows.Forms.Label();
            this.tabConfiguration = new System.Windows.Forms.TabControl();
            this.tabAssignments = new System.Windows.Forms.TabPage();
            this.dgvAssignments = new System.Windows.Forms.DataGridView();
            this.lblAssignmentStatus = new System.Windows.Forms.Label();
            this.btnReplaceAssignments = new System.Windows.Forms.Button();
            this.txtAssignmentGroups = new System.Windows.Forms.TextBox();
            this.lblAssignmentGroups = new System.Windows.Forms.Label();
            this.nudAssignmentScale = new System.Windows.Forms.NumericUpDown();
            this.lblAssignmentScale = new System.Windows.Forms.Label();
            this.btnLoadAssignments = new System.Windows.Forms.Button();
            this.tabMappings = new System.Windows.Forms.TabPage();
            this.dgvMappings = new System.Windows.Forms.DataGridView();
            this.lblMappingStatus = new System.Windows.Forms.Label();
            this.btnCopyMappings = new System.Windows.Forms.Button();
            this.btnReplaceMappings = new System.Windows.Forms.Button();
            this.btnRemoveMapping = new System.Windows.Forms.Button();
            this.btnAddMapping = new System.Windows.Forms.Button();
            this.nudMappingDestination = new System.Windows.Forms.NumericUpDown();
            this.lblMappingDestination = new System.Windows.Forms.Label();
            this.nudMappingScale = new System.Windows.Forms.NumericUpDown();
            this.lblMappingScale = new System.Windows.Forms.Label();
            this.btnLoadMappings = new System.Windows.Forms.Button();
            this.tabHotKeys = new System.Windows.Forms.TabPage();
            this.dgvHotKeys = new System.Windows.Forms.DataGridView();
            this.lblHotKeyStatus = new System.Windows.Forms.Label();
            this.btnReplaceHotKeys = new System.Windows.Forms.Button();
            this.btnRemoveHotKey = new System.Windows.Forms.Button();
            this.btnAddHotKey = new System.Windows.Forms.Button();
            this.txtHotKeyGroup = new System.Windows.Forms.TextBox();
            this.lblHotKeyGroup = new System.Windows.Forms.Label();
            this.btnLoadHotKeys = new System.Windows.Forms.Button();
            this.tabConfiguration.SuspendLayout();
            this.tabAssignments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAssignments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAssignmentScale)).BeginInit();
            this.tabMappings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMappings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMappingDestination)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMappingScale)).BeginInit();
            this.tabHotKeys.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHotKeys)).BeginInit();
            this.SuspendLayout();
            //
            // chkEnableConfigurationWrites
            //
            this.chkEnableConfigurationWrites.AutoSize = true;
            this.chkEnableConfigurationWrites.Location = new System.Drawing.Point(4, 7);
            this.chkEnableConfigurationWrites.Name = "chkEnableConfigurationWrites";
            this.chkEnableConfigurationWrites.Size = new System.Drawing.Size(159, 17);
            this.chkEnableConfigurationWrites.TabIndex = 0;
            this.chkEnableConfigurationWrites.Text = "Enable configuration writes";
            this.chkEnableConfigurationWrites.UseVisualStyleBackColor = true;
            this.chkEnableConfigurationWrites.CheckedChanged += new System.EventHandler(this.chkEnableConfigurationWrites_CheckedChanged);
            //
            // lblWriteWarning
            //
            this.lblWriteWarning.AutoSize = true;
            this.lblWriteWarning.Location = new System.Drawing.Point(175, 8);
            this.lblWriteWarning.Name = "lblWriteWarning";
            this.lblWriteWarning.Size = new System.Drawing.Size(641, 13);
            this.lblWriteWarning.TabIndex = 1;
            this.lblWriteWarning.Text = "Replace operations are atomic and guarded. Replaced means SQL configuration changed; device transfer happens later through Sadr Scales AutoSend.";
            //
            // tabConfiguration
            //
            this.tabConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tabConfiguration.Controls.Add(this.tabAssignments);
            this.tabConfiguration.Controls.Add(this.tabMappings);
            this.tabConfiguration.Controls.Add(this.tabHotKeys);
            this.tabConfiguration.Location = new System.Drawing.Point(0, 31);
            this.tabConfiguration.Name = "tabConfiguration";
            this.tabConfiguration.SelectedIndex = 0;
            this.tabConfiguration.Size = new System.Drawing.Size(1120, 530);
            this.tabConfiguration.TabIndex = 2;
            //
            // tabAssignments
            //
            this.tabAssignments.Controls.Add(this.dgvAssignments);
            this.tabAssignments.Controls.Add(this.lblAssignmentStatus);
            this.tabAssignments.Controls.Add(this.btnReplaceAssignments);
            this.tabAssignments.Controls.Add(this.txtAssignmentGroups);
            this.tabAssignments.Controls.Add(this.lblAssignmentGroups);
            this.tabAssignments.Controls.Add(this.nudAssignmentScale);
            this.tabAssignments.Controls.Add(this.lblAssignmentScale);
            this.tabAssignments.Controls.Add(this.btnLoadAssignments);
            this.tabAssignments.Location = new System.Drawing.Point(4, 22);
            this.tabAssignments.Name = "tabAssignments";
            this.tabAssignments.Padding = new System.Windows.Forms.Padding(8);
            this.tabAssignments.Size = new System.Drawing.Size(1112, 504);
            this.tabAssignments.TabIndex = 0;
            this.tabAssignments.Text = "Scale Assignments";
            this.tabAssignments.UseVisualStyleBackColor = true;
            //
            // btnLoadAssignments
            //
            this.btnLoadAssignments.Location = new System.Drawing.Point(10, 10);
            this.btnLoadAssignments.Name = "btnLoadAssignments";
            this.btnLoadAssignments.Size = new System.Drawing.Size(105, 25);
            this.btnLoadAssignments.TabIndex = 0;
            this.btnLoadAssignments.Text = "Load groups";
            this.btnLoadAssignments.UseVisualStyleBackColor = true;
            this.btnLoadAssignments.Click += new System.EventHandler(this.btnLoadAssignments_Click);
            //
            // lblAssignmentScale
            //
            this.lblAssignmentScale.AutoSize = true;
            this.lblAssignmentScale.Location = new System.Drawing.Point(128, 16);
            this.lblAssignmentScale.Name = "lblAssignmentScale";
            this.lblAssignmentScale.Size = new System.Drawing.Size(49, 13);
            this.lblAssignmentScale.TabIndex = 1;
            this.lblAssignmentScale.Text = "Scale ID:";
            //
            // nudAssignmentScale
            //
            this.nudAssignmentScale.Location = new System.Drawing.Point(183, 13);
            this.nudAssignmentScale.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            this.nudAssignmentScale.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudAssignmentScale.Name = "nudAssignmentScale";
            this.nudAssignmentScale.Size = new System.Drawing.Size(55, 20);
            this.nudAssignmentScale.TabIndex = 2;
            this.nudAssignmentScale.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // lblAssignmentGroups
            //
            this.lblAssignmentGroups.AutoSize = true;
            this.lblAssignmentGroups.Location = new System.Drawing.Point(252, 16);
            this.lblAssignmentGroups.Name = "lblAssignmentGroups";
            this.lblAssignmentGroups.Size = new System.Drawing.Size(91, 13);
            this.lblAssignmentGroups.TabIndex = 3;
            this.lblAssignmentGroups.Text = "Groups (comma):";
            //
            // txtAssignmentGroups
            //
            this.txtAssignmentGroups.Location = new System.Drawing.Point(349, 13);
            this.txtAssignmentGroups.Name = "txtAssignmentGroups";
            this.txtAssignmentGroups.Size = new System.Drawing.Size(330, 20);
            this.txtAssignmentGroups.TabIndex = 4;
            //
            // btnReplaceAssignments
            //
            this.btnReplaceAssignments.Enabled = false;
            this.btnReplaceAssignments.Location = new System.Drawing.Point(690, 10);
            this.btnReplaceAssignments.Name = "btnReplaceAssignments";
            this.btnReplaceAssignments.Size = new System.Drawing.Size(125, 25);
            this.btnReplaceAssignments.TabIndex = 5;
            this.btnReplaceAssignments.Text = "Replace groups";
            this.btnReplaceAssignments.UseVisualStyleBackColor = true;
            this.btnReplaceAssignments.Click += new System.EventHandler(this.btnReplaceAssignments_Click);
            //
            // lblAssignmentStatus
            //
            this.lblAssignmentStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAssignmentStatus.Location = new System.Drawing.Point(830, 16);
            this.lblAssignmentStatus.Name = "lblAssignmentStatus";
            this.lblAssignmentStatus.Size = new System.Drawing.Size(270, 30);
            this.lblAssignmentStatus.TabIndex = 6;
            this.lblAssignmentStatus.Text = "Ready";
            //
            // dgvAssignments
            //
            ConfigureReadOnlyGrid(this.dgvAssignments, new System.Drawing.Point(10, 49), new System.Drawing.Size(1090, 440));
            this.dgvAssignments.Name = "dgvAssignments";
            this.dgvAssignments.TabIndex = 7;
            //
            // tabMappings
            //
            this.tabMappings.Controls.Add(this.dgvMappings);
            this.tabMappings.Controls.Add(this.lblMappingStatus);
            this.tabMappings.Controls.Add(this.btnCopyMappings);
            this.tabMappings.Controls.Add(this.btnReplaceMappings);
            this.tabMappings.Controls.Add(this.btnRemoveMapping);
            this.tabMappings.Controls.Add(this.btnAddMapping);
            this.tabMappings.Controls.Add(this.nudMappingDestination);
            this.tabMappings.Controls.Add(this.lblMappingDestination);
            this.tabMappings.Controls.Add(this.nudMappingScale);
            this.tabMappings.Controls.Add(this.lblMappingScale);
            this.tabMappings.Controls.Add(this.btnLoadMappings);
            this.tabMappings.Location = new System.Drawing.Point(4, 22);
            this.tabMappings.Name = "tabMappings";
            this.tabMappings.Padding = new System.Windows.Forms.Padding(8);
            this.tabMappings.Size = new System.Drawing.Size(1112, 504);
            this.tabMappings.TabIndex = 1;
            this.tabMappings.Text = "Scale Mapping";
            this.tabMappings.UseVisualStyleBackColor = true;
            //
            // btnLoadMappings
            //
            this.btnLoadMappings.Location = new System.Drawing.Point(10, 10);
            this.btnLoadMappings.Name = "btnLoadMappings";
            this.btnLoadMappings.Size = new System.Drawing.Size(105, 25);
            this.btnLoadMappings.TabIndex = 0;
            this.btnLoadMappings.Text = "Load mapping";
            this.btnLoadMappings.UseVisualStyleBackColor = true;
            this.btnLoadMappings.Click += new System.EventHandler(this.btnLoadMappings_Click);
            //
            // lblMappingScale
            //
            this.lblMappingScale.AutoSize = true;
            this.lblMappingScale.Location = new System.Drawing.Point(128, 16);
            this.lblMappingScale.Text = "Scale ID:";
            //
            // nudMappingScale
            //
            this.nudMappingScale.Location = new System.Drawing.Point(183, 13);
            this.nudMappingScale.Minimum = 1;
            this.nudMappingScale.Maximum = 99;
            this.nudMappingScale.Value = 1;
            this.nudMappingScale.Size = new System.Drawing.Size(55, 20);
            //
            // lblMappingDestination
            //
            this.lblMappingDestination.AutoSize = true;
            this.lblMappingDestination.Location = new System.Drawing.Point(250, 16);
            this.lblMappingDestination.Text = "Copy to:";
            //
            // nudMappingDestination
            //
            this.nudMappingDestination.Location = new System.Drawing.Point(300, 13);
            this.nudMappingDestination.Minimum = 1;
            this.nudMappingDestination.Maximum = 99;
            this.nudMappingDestination.Value = 2;
            this.nudMappingDestination.Size = new System.Drawing.Size(55, 20);
            //
            // btnAddMapping
            //
            this.btnAddMapping.Location = new System.Drawing.Point(370, 10);
            this.btnAddMapping.Size = new System.Drawing.Size(75, 25);
            this.btnAddMapping.Text = "Add row";
            this.btnAddMapping.Click += new System.EventHandler(this.btnAddMapping_Click);
            //
            // btnRemoveMapping
            //
            this.btnRemoveMapping.Location = new System.Drawing.Point(450, 10);
            this.btnRemoveMapping.Size = new System.Drawing.Size(85, 25);
            this.btnRemoveMapping.Text = "Remove row";
            this.btnRemoveMapping.Click += new System.EventHandler(this.btnRemoveMapping_Click);
            //
            // btnReplaceMappings
            //
            this.btnReplaceMappings.Enabled = false;
            this.btnReplaceMappings.Location = new System.Drawing.Point(545, 10);
            this.btnReplaceMappings.Size = new System.Drawing.Size(110, 25);
            this.btnReplaceMappings.Text = "Replace mapping";
            this.btnReplaceMappings.Click += new System.EventHandler(this.btnReplaceMappings_Click);
            //
            // btnCopyMappings
            //
            this.btnCopyMappings.Enabled = false;
            this.btnCopyMappings.Location = new System.Drawing.Point(665, 10);
            this.btnCopyMappings.Size = new System.Drawing.Size(95, 25);
            this.btnCopyMappings.Text = "Copy mapping";
            this.btnCopyMappings.Click += new System.EventHandler(this.btnCopyMappings_Click);
            //
            // lblMappingStatus
            //
            this.lblMappingStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMappingStatus.Location = new System.Drawing.Point(775, 16);
            this.lblMappingStatus.Size = new System.Drawing.Size(325, 30);
            this.lblMappingStatus.Text = "Ready";
            //
            // dgvMappings
            //
            ConfigureEditableGrid(this.dgvMappings, new System.Drawing.Point(10, 49), new System.Drawing.Size(1090, 440));
            this.dgvMappings.Name = "dgvMappings";
            //
            // tabHotKeys
            //
            this.tabHotKeys.Controls.Add(this.dgvHotKeys);
            this.tabHotKeys.Controls.Add(this.lblHotKeyStatus);
            this.tabHotKeys.Controls.Add(this.btnReplaceHotKeys);
            this.tabHotKeys.Controls.Add(this.btnRemoveHotKey);
            this.tabHotKeys.Controls.Add(this.btnAddHotKey);
            this.tabHotKeys.Controls.Add(this.txtHotKeyGroup);
            this.tabHotKeys.Controls.Add(this.lblHotKeyGroup);
            this.tabHotKeys.Controls.Add(this.btnLoadHotKeys);
            this.tabHotKeys.Location = new System.Drawing.Point(4, 22);
            this.tabHotKeys.Name = "tabHotKeys";
            this.tabHotKeys.Padding = new System.Windows.Forms.Padding(8);
            this.tabHotKeys.Size = new System.Drawing.Size(1112, 504);
            this.tabHotKeys.TabIndex = 2;
            this.tabHotKeys.Text = "Group HotKeys";
            this.tabHotKeys.UseVisualStyleBackColor = true;
            //
            // btnLoadHotKeys
            //
            this.btnLoadHotKeys.Location = new System.Drawing.Point(10, 10);
            this.btnLoadHotKeys.Size = new System.Drawing.Size(105, 25);
            this.btnLoadHotKeys.Text = "Load HotKeys";
            this.btnLoadHotKeys.Click += new System.EventHandler(this.btnLoadHotKeys_Click);
            //
            // lblHotKeyGroup
            //
            this.lblHotKeyGroup.AutoSize = true;
            this.lblHotKeyGroup.Location = new System.Drawing.Point(128, 16);
            this.lblHotKeyGroup.Text = "Group code:";
            //
            // txtHotKeyGroup
            //
            this.txtHotKeyGroup.Location = new System.Drawing.Point(195, 13);
            this.txtHotKeyGroup.MaxLength = 50;
            this.txtHotKeyGroup.Text = "0";
            this.txtHotKeyGroup.Size = new System.Drawing.Size(110, 20);
            //
            // btnAddHotKey
            //
            this.btnAddHotKey.Location = new System.Drawing.Point(320, 10);
            this.btnAddHotKey.Size = new System.Drawing.Size(75, 25);
            this.btnAddHotKey.Text = "Add row";
            this.btnAddHotKey.Click += new System.EventHandler(this.btnAddHotKey_Click);
            //
            // btnRemoveHotKey
            //
            this.btnRemoveHotKey.Location = new System.Drawing.Point(400, 10);
            this.btnRemoveHotKey.Size = new System.Drawing.Size(85, 25);
            this.btnRemoveHotKey.Text = "Remove row";
            this.btnRemoveHotKey.Click += new System.EventHandler(this.btnRemoveHotKey_Click);
            //
            // btnReplaceHotKeys
            //
            this.btnReplaceHotKeys.Enabled = false;
            this.btnReplaceHotKeys.Location = new System.Drawing.Point(495, 10);
            this.btnReplaceHotKeys.Size = new System.Drawing.Size(120, 25);
            this.btnReplaceHotKeys.Text = "Replace HotKeys";
            this.btnReplaceHotKeys.Click += new System.EventHandler(this.btnReplaceHotKeys_Click);
            //
            // lblHotKeyStatus
            //
            this.lblHotKeyStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHotKeyStatus.Location = new System.Drawing.Point(630, 16);
            this.lblHotKeyStatus.Size = new System.Drawing.Size(470, 30);
            this.lblHotKeyStatus.Text = "Only positive-PLU user keys are shown. Internal/system rows are preserved.";
            //
            // dgvHotKeys
            //
            ConfigureEditableGrid(this.dgvHotKeys, new System.Drawing.Point(10, 49), new System.Drawing.Size(1090, 440));
            this.dgvHotKeys.Name = "dgvHotKeys";
            //
            // ScaleConfigurationControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabConfiguration);
            this.Controls.Add(this.lblWriteWarning);
            this.Controls.Add(this.chkEnableConfigurationWrites);
            this.Name = "ScaleConfigurationControl";
            this.Size = new System.Drawing.Size(1120, 561);
            this.tabConfiguration.ResumeLayout(false);
            this.tabAssignments.ResumeLayout(false);
            this.tabAssignments.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAssignments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAssignmentScale)).EndInit();
            this.tabMappings.ResumeLayout(false);
            this.tabMappings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMappings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMappingDestination)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMappingScale)).EndInit();
            this.tabHotKeys.ResumeLayout(false);
            this.tabHotKeys.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHotKeys)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private static void ConfigureReadOnlyGrid(
            System.Windows.Forms.DataGridView grid,
            System.Drawing.Point location,
            System.Drawing.Size size)
        {
            ConfigureEditableGrid(grid, location, size);
            grid.ReadOnly = true;
        }

        private static void ConfigureEditableGrid(
            System.Windows.Forms.DataGridView grid,
            System.Drawing.Point location,
            System.Drawing.Size size)
        {
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid.Location = location;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            grid.Size = size;
        }

        #endregion
    }
}
