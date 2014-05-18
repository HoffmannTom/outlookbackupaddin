namespace BackupAddIn
{
    partial class FBackupSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSpeichern = new System.Windows.Forms.Button();
            this.btnAbbrechen = new System.Windows.Forms.Button();
            this.gbSettings = new System.Windows.Forms.GroupBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.txtLastBackup = new System.Windows.Forms.TextBox();
            this.lblLastBackup = new System.Windows.Forms.Label();
            this.lblBackupAll = new System.Windows.Forms.Label();
            this.cbxBackupAll = new System.Windows.Forms.CheckBox();
            this.lblDays = new System.Windows.Forms.Label();
            this.btnBackupSelect = new System.Windows.Forms.Button();
            this.txtBackupExe = new System.Windows.Forms.TextBox();
            this.lblBackup = new System.Windows.Forms.Label();
            this.numInterval = new System.Windows.Forms.NumericUpDown();
            this.lblInterval = new System.Windows.Forms.Label();
            this.btnDirSelect = new System.Windows.Forms.Button();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.lblDestination = new System.Windows.Forms.Label();
            this.lvStores = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.folderBrowserdlg = new System.Windows.Forms.FolderBrowserDialog();
            this.fileOpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.lblBPrefix = new System.Windows.Forms.Label();
            this.lblSuffix = new System.Windows.Forms.Label();
            this.txtSuffix = new System.Windows.Forms.TextBox();
            this.gbSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSpeichern
            // 
            this.btnSpeichern.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSpeichern.Location = new System.Drawing.Point(385, 351);
            this.btnSpeichern.Name = "btnSpeichern";
            this.btnSpeichern.Size = new System.Drawing.Size(75, 23);
            this.btnSpeichern.TabIndex = 12;
            this.btnSpeichern.Text = "&Save";
            this.btnSpeichern.UseVisualStyleBackColor = true;
            this.btnSpeichern.Click += new System.EventHandler(this.btnSpeichern_click);
            // 
            // btnAbbrechen
            // 
            this.btnAbbrechen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbbrechen.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAbbrechen.Location = new System.Drawing.Point(304, 351);
            this.btnAbbrechen.Name = "btnAbbrechen";
            this.btnAbbrechen.Size = new System.Drawing.Size(75, 23);
            this.btnAbbrechen.TabIndex = 11;
            this.btnAbbrechen.Text = "&Cancel";
            this.btnAbbrechen.UseVisualStyleBackColor = true;
            this.btnAbbrechen.Click += new System.EventHandler(this.btnAbbrechen_click);
            // 
            // gbSettings
            // 
            this.gbSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSettings.Controls.Add(this.txtSuffix);
            this.gbSettings.Controls.Add(this.lblSuffix);
            this.gbSettings.Controls.Add(this.txtPrefix);
            this.gbSettings.Controls.Add(this.lblBPrefix);
            this.gbSettings.Controls.Add(this.btnReset);
            this.gbSettings.Controls.Add(this.txtLastBackup);
            this.gbSettings.Controls.Add(this.lblLastBackup);
            this.gbSettings.Controls.Add(this.lblBackupAll);
            this.gbSettings.Controls.Add(this.cbxBackupAll);
            this.gbSettings.Controls.Add(this.lblDays);
            this.gbSettings.Controls.Add(this.btnBackupSelect);
            this.gbSettings.Controls.Add(this.txtBackupExe);
            this.gbSettings.Controls.Add(this.lblBackup);
            this.gbSettings.Controls.Add(this.numInterval);
            this.gbSettings.Controls.Add(this.lblInterval);
            this.gbSettings.Controls.Add(this.btnDirSelect);
            this.gbSettings.Controls.Add(this.txtDestination);
            this.gbSettings.Controls.Add(this.lblDestination);
            this.gbSettings.Controls.Add(this.lvStores);
            this.gbSettings.Location = new System.Drawing.Point(12, 10);
            this.gbSettings.Name = "gbSettings";
            this.gbSettings.Size = new System.Drawing.Size(448, 335);
            this.gbSettings.TabIndex = 2;
            this.gbSettings.TabStop = false;
            this.gbSettings.Text = "Outlook-Files";
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(269, 229);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(53, 20);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // txtLastBackup
            // 
            this.txtLastBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLastBackup.Enabled = false;
            this.txtLastBackup.Location = new System.Drawing.Point(131, 229);
            this.txtLastBackup.Name = "txtLastBackup";
            this.txtLastBackup.Size = new System.Drawing.Size(132, 20);
            this.txtLastBackup.TabIndex = 3;
            // 
            // lblLastBackup
            // 
            this.lblLastBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLastBackup.AutoSize = true;
            this.lblLastBackup.Location = new System.Drawing.Point(11, 233);
            this.lblLastBackup.Name = "lblLastBackup";
            this.lblLastBackup.Size = new System.Drawing.Size(70, 13);
            this.lblLastBackup.TabIndex = 10;
            this.lblLastBackup.Text = "Last Backup:";
            // 
            // lblBackupAll
            // 
            this.lblBackupAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBackupAll.AutoSize = true;
            this.lblBackupAll.Location = new System.Drawing.Point(11, 183);
            this.lblBackupAll.Name = "lblBackupAll";
            this.lblBackupAll.Size = new System.Drawing.Size(104, 13);
            this.lblBackupAll.TabIndex = 9;
            this.lblBackupAll.Text = "Backup all psd-Files:";
            // 
            // cbxBackupAll
            // 
            this.cbxBackupAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbxBackupAll.AutoSize = true;
            this.cbxBackupAll.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbxBackupAll.Location = new System.Drawing.Point(131, 182);
            this.cbxBackupAll.Name = "cbxBackupAll";
            this.cbxBackupAll.Size = new System.Drawing.Size(15, 14);
            this.cbxBackupAll.TabIndex = 1;
            this.cbxBackupAll.UseVisualStyleBackColor = true;
            this.cbxBackupAll.CheckedChanged += new System.EventHandler(this.cbxBackupAll_CheckedChanged);
            // 
            // lblDays
            // 
            this.lblDays.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDays.AutoSize = true;
            this.lblDays.Location = new System.Drawing.Point(187, 208);
            this.lblDays.Name = "lblDays";
            this.lblDays.Size = new System.Drawing.Size(29, 13);
            this.lblDays.TabIndex = 7;
            this.lblDays.Text = "days";
            // 
            // btnBackupSelect
            // 
            this.btnBackupSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBackupSelect.Location = new System.Drawing.Point(415, 278);
            this.btnBackupSelect.Name = "btnBackupSelect";
            this.btnBackupSelect.Size = new System.Drawing.Size(24, 23);
            this.btnBackupSelect.TabIndex = 8;
            this.btnBackupSelect.Text = "...";
            this.btnBackupSelect.UseVisualStyleBackColor = true;
            this.btnBackupSelect.Click += new System.EventHandler(this.btnBackupSelect_Click);
            // 
            // txtBackupExe
            // 
            this.txtBackupExe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBackupExe.Location = new System.Drawing.Point(131, 279);
            this.txtBackupExe.Name = "txtBackupExe";
            this.txtBackupExe.Size = new System.Drawing.Size(278, 20);
            this.txtBackupExe.TabIndex = 7;
            // 
            // lblBackup
            // 
            this.lblBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBackup.AutoSize = true;
            this.lblBackup.Location = new System.Drawing.Point(11, 283);
            this.lblBackup.Name = "lblBackup";
            this.lblBackup.Size = new System.Drawing.Size(109, 13);
            this.lblBackup.TabIndex = 6;
            this.lblBackup.Text = "BackupExecutor.exe:";
            // 
            // numInterval
            // 
            this.numInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numInterval.Location = new System.Drawing.Point(131, 204);
            this.numInterval.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numInterval.Name = "numInterval";
            this.numInterval.Size = new System.Drawing.Size(50, 20);
            this.numInterval.TabIndex = 2;
            this.numInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblInterval
            // 
            this.lblInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblInterval.AutoSize = true;
            this.lblInterval.Location = new System.Drawing.Point(11, 208);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(45, 13);
            this.lblInterval.TabIndex = 4;
            this.lblInterval.Text = "Interval:";
            // 
            // btnDirSelect
            // 
            this.btnDirSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDirSelect.Location = new System.Drawing.Point(415, 253);
            this.btnDirSelect.Name = "btnDirSelect";
            this.btnDirSelect.Size = new System.Drawing.Size(24, 23);
            this.btnDirSelect.TabIndex = 6;
            this.btnDirSelect.Text = "...";
            this.btnDirSelect.UseVisualStyleBackColor = true;
            this.btnDirSelect.Click += new System.EventHandler(this.btnDirSelect_Click);
            // 
            // txtDestination
            // 
            this.txtDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestination.Location = new System.Drawing.Point(131, 254);
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(278, 20);
            this.txtDestination.TabIndex = 5;
            // 
            // lblDestination
            // 
            this.lblDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDestination.AutoSize = true;
            this.lblDestination.Location = new System.Drawing.Point(11, 258);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(63, 13);
            this.lblDestination.TabIndex = 1;
            this.lblDestination.Text = "Destination:";
            // 
            // lvStores
            // 
            this.lvStores.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.lvStores.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvStores.AutoArrange = false;
            this.lvStores.CausesValidation = false;
            this.lvStores.CheckBoxes = true;
            this.lvStores.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvStores.FullRowSelect = true;
            this.lvStores.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvStores.LabelWrap = false;
            this.lvStores.Location = new System.Drawing.Point(14, 19);
            this.lvStores.Name = "lvStores";
            this.lvStores.ShowGroups = false;
            this.lvStores.Size = new System.Drawing.Size(419, 151);
            this.lvStores.TabIndex = 0;
            this.lvStores.UseCompatibleStateImageBehavior = false;
            this.lvStores.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 369;
            // 
            // fileOpenDialog
            // 
            this.fileOpenDialog.DefaultExt = "*.exe";
            this.fileOpenDialog.FileName = "BackupExecutor";
            this.fileOpenDialog.Filter = "Executables|*.exe";
            this.fileOpenDialog.Title = "Select BackupExecutor.exe";
            // 
            // txtPrefix
            // 
            this.txtPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtPrefix.Location = new System.Drawing.Point(131, 305);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(101, 20);
            this.txtPrefix.TabIndex = 9;
            // 
            // lblBPrefix
            // 
            this.lblBPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBPrefix.AutoSize = true;
            this.lblBPrefix.Location = new System.Drawing.Point(11, 309);
            this.lblBPrefix.Name = "lblBPrefix";
            this.lblBPrefix.Size = new System.Drawing.Size(76, 13);
            this.lblBPrefix.TabIndex = 11;
            this.lblBPrefix.Text = "Backup Prefix:";
            // 
            // lblSuffix
            // 
            this.lblSuffix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSuffix.AutoSize = true;
            this.lblSuffix.Location = new System.Drawing.Point(266, 308);
            this.lblSuffix.Name = "lblSuffix";
            this.lblSuffix.Size = new System.Drawing.Size(36, 13);
            this.lblSuffix.TabIndex = 13;
            this.lblSuffix.Text = "Suffix:";
            // 
            // txtSuffix
            // 
            this.txtSuffix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSuffix.Location = new System.Drawing.Point(308, 305);
            this.txtSuffix.Name = "txtSuffix";
            this.txtSuffix.Size = new System.Drawing.Size(101, 20);
            this.txtSuffix.TabIndex = 10;
            // 
            // FBackupSettings
            // 
            this.AcceptButton = this.btnSpeichern;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAbbrechen;
            this.ClientSize = new System.Drawing.Size(472, 385);
            this.ControlBox = false;
            this.Controls.Add(this.gbSettings);
            this.Controls.Add(this.btnAbbrechen);
            this.Controls.Add(this.btnSpeichern);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(460, 350);
            this.Name = "FBackupSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Backup Settings";
            this.Load += new System.EventHandler(this.FBackupSettings_Load);
            this.gbSettings.ResumeLayout(false);
            this.gbSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSpeichern;
        private System.Windows.Forms.Button btnAbbrechen;
        private System.Windows.Forms.GroupBox gbSettings;
        private System.Windows.Forms.ListView lvStores;
        private System.Windows.Forms.Button btnDirSelect;
        private System.Windows.Forms.TextBox txtDestination;
        private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserdlg;
        private System.Windows.Forms.NumericUpDown numInterval;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.Button btnBackupSelect;
        private System.Windows.Forms.TextBox txtBackupExe;
        private System.Windows.Forms.Label lblBackup;
        private System.Windows.Forms.OpenFileDialog fileOpenDialog;
        private System.Windows.Forms.Label lblDays;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.CheckBox cbxBackupAll;
        private System.Windows.Forms.Label lblBackupAll;
        private System.Windows.Forms.Label lblLastBackup;
        private System.Windows.Forms.TextBox txtLastBackup;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TextBox txtSuffix;
        private System.Windows.Forms.Label lblSuffix;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.Label lblBPrefix;
    }
}