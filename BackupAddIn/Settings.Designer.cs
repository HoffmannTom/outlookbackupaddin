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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBackupSelect = new System.Windows.Forms.Button();
            this.txtBackupExe = new System.Windows.Forms.TextBox();
            this.lblBackup = new System.Windows.Forms.Label();
            this.numInterval = new System.Windows.Forms.NumericUpDown();
            this.lblInterval = new System.Windows.Forms.Label();
            this.btnDirSelect = new System.Windows.Forms.Button();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.lblDestination = new System.Windows.Forms.Label();
            this.lvStores = new System.Windows.Forms.ListView();
            this.folderBrowserdlg = new System.Windows.Forms.FolderBrowserDialog();
            this.fileOpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSpeichern
            // 
            this.btnSpeichern.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSpeichern.Location = new System.Drawing.Point(363, 275);
            this.btnSpeichern.Name = "btnSpeichern";
            this.btnSpeichern.Size = new System.Drawing.Size(75, 23);
            this.btnSpeichern.TabIndex = 7;
            this.btnSpeichern.Text = "&Save";
            this.btnSpeichern.UseVisualStyleBackColor = true;
            this.btnSpeichern.Click += new System.EventHandler(this.btnSpeichern_click);
            // 
            // btnAbbrechen
            // 
            this.btnAbbrechen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbbrechen.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAbbrechen.Location = new System.Drawing.Point(282, 275);
            this.btnAbbrechen.Name = "btnAbbrechen";
            this.btnAbbrechen.Size = new System.Drawing.Size(75, 23);
            this.btnAbbrechen.TabIndex = 6;
            this.btnAbbrechen.Text = "&Cancel";
            this.btnAbbrechen.UseVisualStyleBackColor = true;
            this.btnAbbrechen.Click += new System.EventHandler(this.btnAbbrechen_click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnBackupSelect);
            this.groupBox1.Controls.Add(this.txtBackupExe);
            this.groupBox1.Controls.Add(this.lblBackup);
            this.groupBox1.Controls.Add(this.numInterval);
            this.groupBox1.Controls.Add(this.lblInterval);
            this.groupBox1.Controls.Add(this.btnDirSelect);
            this.groupBox1.Controls.Add(this.txtDestination);
            this.groupBox1.Controls.Add(this.lblDestination);
            this.groupBox1.Controls.Add(this.lvStores);
            this.groupBox1.Location = new System.Drawing.Point(12, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(426, 247);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Outlook-Files";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(177, 170);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "days";
            // 
            // btnBackupSelect
            // 
            this.btnBackupSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBackupSelect.Location = new System.Drawing.Point(387, 217);
            this.btnBackupSelect.Name = "btnBackupSelect";
            this.btnBackupSelect.Size = new System.Drawing.Size(24, 23);
            this.btnBackupSelect.TabIndex = 5;
            this.btnBackupSelect.Text = "...";
            this.btnBackupSelect.UseVisualStyleBackColor = true;
            this.btnBackupSelect.Click += new System.EventHandler(this.btnBackupSelect_Click);
            // 
            // txtBackupExe
            // 
            this.txtBackupExe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBackupExe.Location = new System.Drawing.Point(122, 219);
            this.txtBackupExe.Name = "txtBackupExe";
            this.txtBackupExe.Size = new System.Drawing.Size(253, 20);
            this.txtBackupExe.TabIndex = 4;
            // 
            // lblBackup
            // 
            this.lblBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBackup.AutoSize = true;
            this.lblBackup.Location = new System.Drawing.Point(14, 221);
            this.lblBackup.Name = "lblBackup";
            this.lblBackup.Size = new System.Drawing.Size(109, 13);
            this.lblBackup.TabIndex = 6;
            this.lblBackup.Text = "BackupExecutor.exe:";
            // 
            // numInterval
            // 
            this.numInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numInterval.Location = new System.Drawing.Point(122, 168);
            this.numInterval.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numInterval.Name = "numInterval";
            this.numInterval.Size = new System.Drawing.Size(49, 20);
            this.numInterval.TabIndex = 1;
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
            this.lblInterval.Location = new System.Drawing.Point(14, 170);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(45, 13);
            this.lblInterval.TabIndex = 4;
            this.lblInterval.Text = "Interval:";
            // 
            // btnDirSelect
            // 
            this.btnDirSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDirSelect.Location = new System.Drawing.Point(387, 191);
            this.btnDirSelect.Name = "btnDirSelect";
            this.btnDirSelect.Size = new System.Drawing.Size(24, 23);
            this.btnDirSelect.TabIndex = 3;
            this.btnDirSelect.Text = "...";
            this.btnDirSelect.UseVisualStyleBackColor = true;
            this.btnDirSelect.Click += new System.EventHandler(this.btnDirSelect_Click);
            // 
            // txtDestination
            // 
            this.txtDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestination.Location = new System.Drawing.Point(122, 193);
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(253, 20);
            this.txtDestination.TabIndex = 2;
            // 
            // lblDestination
            // 
            this.lblDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDestination.AutoSize = true;
            this.lblDestination.Location = new System.Drawing.Point(14, 195);
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
            this.lvStores.Size = new System.Drawing.Size(397, 142);
            this.lvStores.TabIndex = 0;
            this.lvStores.UseCompatibleStateImageBehavior = false;
            this.lvStores.View = System.Windows.Forms.View.Details;
            // 
            // fileOpenDialog
            // 
            this.fileOpenDialog.DefaultExt = "*.exe";
            this.fileOpenDialog.FileName = "BackupExecutor";
            this.fileOpenDialog.Filter = "Executables|*.exe";
            this.fileOpenDialog.Title = "Select BackupExecutor.exe";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 369;
            // 
            // FBackupSettings
            // 
            this.AcceptButton = this.btnSpeichern;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAbbrechen;
            this.ClientSize = new System.Drawing.Size(450, 309);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnAbbrechen);
            this.Controls.Add(this.btnSpeichern);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "FBackupSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Backup Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSpeichern;
        private System.Windows.Forms.Button btnAbbrechen;
        private System.Windows.Forms.GroupBox groupBox1;
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}