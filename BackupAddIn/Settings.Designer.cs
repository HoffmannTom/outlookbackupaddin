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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FBackupSettings));
            this.btnSpeichern = new System.Windows.Forms.Button();
            this.btnAbbrechen = new System.Windows.Forms.Button();
            this.gbSettings = new System.Windows.Forms.GroupBox();
            this.cbxBackupAll = new System.Windows.Forms.CheckBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.txtLastBackup = new System.Windows.Forms.TextBox();
            this.lblLastBackup = new System.Windows.Forms.Label();
            this.lblBackupAll = new System.Windows.Forms.Label();
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
            this.gbOptionalSettings = new System.Windows.Forms.GroupBox();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.numCountdown = new System.Windows.Forms.NumericUpDown();
            this.lblCountdown = new System.Windows.Forms.Label();
            this.txtPostBackupCmd = new System.Windows.Forms.TextBox();
            this.lblPostBackupCmd = new System.Windows.Forms.Label();
            this.txtSuffix = new System.Windows.Forms.TextBox();
            this.lblSuffix = new System.Windows.Forms.Label();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.lblBPrefix = new System.Windows.Forms.Label();
            this.gbSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).BeginInit();
            this.gbOptionalSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCountdown)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSpeichern
            // 
            resources.ApplyResources(this.btnSpeichern, "btnSpeichern");
            this.btnSpeichern.Name = "btnSpeichern";
            this.btnSpeichern.UseVisualStyleBackColor = true;
            this.btnSpeichern.Click += new System.EventHandler(this.btnSpeichern_click);
            // 
            // btnAbbrechen
            // 
            resources.ApplyResources(this.btnAbbrechen, "btnAbbrechen");
            this.btnAbbrechen.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAbbrechen.Name = "btnAbbrechen";
            this.btnAbbrechen.UseVisualStyleBackColor = true;
            this.btnAbbrechen.Click += new System.EventHandler(this.btnAbbrechen_click);
            // 
            // gbSettings
            // 
            resources.ApplyResources(this.gbSettings, "gbSettings");
            this.gbSettings.Controls.Add(this.cbxBackupAll);
            this.gbSettings.Controls.Add(this.btnReset);
            this.gbSettings.Controls.Add(this.txtLastBackup);
            this.gbSettings.Controls.Add(this.lblLastBackup);
            this.gbSettings.Controls.Add(this.lblBackupAll);
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
            this.gbSettings.Name = "gbSettings";
            this.gbSettings.TabStop = false;
            // 
            // cbxBackupAll
            // 
            resources.ApplyResources(this.cbxBackupAll, "cbxBackupAll");
            this.cbxBackupAll.Name = "cbxBackupAll";
            this.cbxBackupAll.UseVisualStyleBackColor = true;
            this.cbxBackupAll.CheckedChanged += new System.EventHandler(this.cbxBackupAll_CheckedChanged);
            // 
            // btnReset
            // 
            resources.ApplyResources(this.btnReset, "btnReset");
            this.btnReset.Name = "btnReset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // txtLastBackup
            // 
            resources.ApplyResources(this.txtLastBackup, "txtLastBackup");
            this.txtLastBackup.Name = "txtLastBackup";
            // 
            // lblLastBackup
            // 
            resources.ApplyResources(this.lblLastBackup, "lblLastBackup");
            this.lblLastBackup.Name = "lblLastBackup";
            // 
            // lblBackupAll
            // 
            resources.ApplyResources(this.lblBackupAll, "lblBackupAll");
            this.lblBackupAll.Name = "lblBackupAll";
            // 
            // lblDays
            // 
            resources.ApplyResources(this.lblDays, "lblDays");
            this.lblDays.Name = "lblDays";
            // 
            // btnBackupSelect
            // 
            resources.ApplyResources(this.btnBackupSelect, "btnBackupSelect");
            this.btnBackupSelect.Name = "btnBackupSelect";
            this.btnBackupSelect.UseVisualStyleBackColor = true;
            this.btnBackupSelect.Click += new System.EventHandler(this.btnBackupSelect_Click);
            // 
            // txtBackupExe
            // 
            resources.ApplyResources(this.txtBackupExe, "txtBackupExe");
            this.txtBackupExe.Name = "txtBackupExe";
            // 
            // lblBackup
            // 
            resources.ApplyResources(this.lblBackup, "lblBackup");
            this.lblBackup.Name = "lblBackup";
            // 
            // numInterval
            // 
            resources.ApplyResources(this.numInterval, "numInterval");
            this.numInterval.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numInterval.Name = "numInterval";
            this.numInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblInterval
            // 
            resources.ApplyResources(this.lblInterval, "lblInterval");
            this.lblInterval.Name = "lblInterval";
            // 
            // btnDirSelect
            // 
            resources.ApplyResources(this.btnDirSelect, "btnDirSelect");
            this.btnDirSelect.Name = "btnDirSelect";
            this.btnDirSelect.UseVisualStyleBackColor = true;
            this.btnDirSelect.Click += new System.EventHandler(this.btnDirSelect_Click);
            // 
            // txtDestination
            // 
            resources.ApplyResources(this.txtDestination, "txtDestination");
            this.txtDestination.Name = "txtDestination";
            // 
            // lblDestination
            // 
            resources.ApplyResources(this.lblDestination, "lblDestination");
            this.lblDestination.Name = "lblDestination";
            // 
            // lvStores
            // 
            resources.ApplyResources(this.lvStores, "lvStores");
            this.lvStores.AutoArrange = false;
            this.lvStores.CausesValidation = false;
            this.lvStores.CheckBoxes = true;
            this.lvStores.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvStores.FullRowSelect = true;
            this.lvStores.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvStores.Name = "lvStores";
            this.lvStores.ShowGroups = false;
            this.lvStores.UseCompatibleStateImageBehavior = false;
            this.lvStores.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // fileOpenDialog
            // 
            this.fileOpenDialog.DefaultExt = "*.exe";
            this.fileOpenDialog.FileName = "BackupExecutor";
            resources.ApplyResources(this.fileOpenDialog, "fileOpenDialog");
            // 
            // gbOptionalSettings
            // 
            resources.ApplyResources(this.gbOptionalSettings, "gbOptionalSettings");
            this.gbOptionalSettings.Controls.Add(this.lblSeconds);
            this.gbOptionalSettings.Controls.Add(this.numCountdown);
            this.gbOptionalSettings.Controls.Add(this.lblCountdown);
            this.gbOptionalSettings.Controls.Add(this.txtPostBackupCmd);
            this.gbOptionalSettings.Controls.Add(this.lblPostBackupCmd);
            this.gbOptionalSettings.Controls.Add(this.txtSuffix);
            this.gbOptionalSettings.Controls.Add(this.lblSuffix);
            this.gbOptionalSettings.Controls.Add(this.txtPrefix);
            this.gbOptionalSettings.Controls.Add(this.lblBPrefix);
            this.gbOptionalSettings.Name = "gbOptionalSettings";
            this.gbOptionalSettings.TabStop = false;
            // 
            // lblSeconds
            // 
            resources.ApplyResources(this.lblSeconds, "lblSeconds");
            this.lblSeconds.Name = "lblSeconds";
            // 
            // numCountdown
            // 
            resources.ApplyResources(this.numCountdown, "numCountdown");
            this.numCountdown.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numCountdown.Name = "numCountdown";
            // 
            // lblCountdown
            // 
            resources.ApplyResources(this.lblCountdown, "lblCountdown");
            this.lblCountdown.Name = "lblCountdown";
            // 
            // txtPostBackupCmd
            // 
            resources.ApplyResources(this.txtPostBackupCmd, "txtPostBackupCmd");
            this.txtPostBackupCmd.Name = "txtPostBackupCmd";
            // 
            // lblPostBackupCmd
            // 
            resources.ApplyResources(this.lblPostBackupCmd, "lblPostBackupCmd");
            this.lblPostBackupCmd.Name = "lblPostBackupCmd";
            // 
            // txtSuffix
            // 
            resources.ApplyResources(this.txtSuffix, "txtSuffix");
            this.txtSuffix.Name = "txtSuffix";
            // 
            // lblSuffix
            // 
            resources.ApplyResources(this.lblSuffix, "lblSuffix");
            this.lblSuffix.Name = "lblSuffix";
            // 
            // txtPrefix
            // 
            resources.ApplyResources(this.txtPrefix, "txtPrefix");
            this.txtPrefix.Name = "txtPrefix";
            // 
            // lblBPrefix
            // 
            resources.ApplyResources(this.lblBPrefix, "lblBPrefix");
            this.lblBPrefix.Name = "lblBPrefix";
            // 
            // FBackupSettings
            // 
            this.AcceptButton = this.btnSpeichern;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAbbrechen;
            this.ControlBox = false;
            this.Controls.Add(this.gbOptionalSettings);
            this.Controls.Add(this.gbSettings);
            this.Controls.Add(this.btnAbbrechen);
            this.Controls.Add(this.btnSpeichern);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FBackupSettings";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FBackupSettings_Load);
            this.gbSettings.ResumeLayout(false);
            this.gbSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).EndInit();
            this.gbOptionalSettings.ResumeLayout(false);
            this.gbOptionalSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCountdown)).EndInit();
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
        private System.Windows.Forms.GroupBox gbOptionalSettings;
        private System.Windows.Forms.TextBox txtPostBackupCmd;
        private System.Windows.Forms.Label lblPostBackupCmd;
        private System.Windows.Forms.TextBox txtSuffix;
        private System.Windows.Forms.Label lblSuffix;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.Label lblBPrefix;
        private System.Windows.Forms.Label lblSeconds;
        private System.Windows.Forms.NumericUpDown numCountdown;
        private System.Windows.Forms.Label lblCountdown;
    }
}