namespace BackupExecutor
{
    partial class FrmMain
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
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.pbCopyProgress = new System.Windows.Forms.ProgressBar();
            this.lblFilename = new System.Windows.Forms.Label();
            this.pbTotalProgress = new System.Windows.Forms.ProgressBar();
            this.lblMegaBytesPerSecond = new System.Windows.Forms.Label();
            this.cbxShutdownWhenFinished = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(12, 12);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(420, 196);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // pbCopyProgress
            // 
            this.pbCopyProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbCopyProgress.Location = new System.Drawing.Point(12, 234);
            this.pbCopyProgress.Name = "pbCopyProgress";
            this.pbCopyProgress.Size = new System.Drawing.Size(420, 23);
            this.pbCopyProgress.TabIndex = 1;
            // 
            // lblFilename
            // 
            this.lblFilename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFilename.AutoSize = true;
            this.lblFilename.Location = new System.Drawing.Point(12, 218);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(0, 13);
            this.lblFilename.TabIndex = 2;
            // 
            // pbTotalProgress
            // 
            this.pbTotalProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbTotalProgress.Location = new System.Drawing.Point(12, 263);
            this.pbTotalProgress.Name = "pbTotalProgress";
            this.pbTotalProgress.Size = new System.Drawing.Size(420, 23);
            this.pbTotalProgress.TabIndex = 3;
            // 
            // lblMegaBytesPerSecond
            // 
            this.lblMegaBytesPerSecond.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMegaBytesPerSecond.Location = new System.Drawing.Point(368, 294);
            this.lblMegaBytesPerSecond.Name = "lblMegaBytesPerSecond";
            this.lblMegaBytesPerSecond.Size = new System.Drawing.Size(64, 13);
            this.lblMegaBytesPerSecond.TabIndex = 4;
            this.lblMegaBytesPerSecond.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbxShutdownWhenFinished
            // 
            this.cbxShutdownWhenFinished.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbxShutdownWhenFinished.AutoSize = true;
            this.cbxShutdownWhenFinished.Location = new System.Drawing.Point(12, 293);
            this.cbxShutdownWhenFinished.Name = "cbxShutdownWhenFinished";
            this.cbxShutdownWhenFinished.Size = new System.Drawing.Size(142, 17);
            this.cbxShutdownWhenFinished.TabIndex = 5;
            this.cbxShutdownWhenFinished.Text = "Shutdown when finished";
            this.cbxShutdownWhenFinished.UseVisualStyleBackColor = true;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 316);
            this.Controls.Add(this.cbxShutdownWhenFinished);
            this.Controls.Add(this.lblMegaBytesPerSecond);
            this.Controls.Add(this.pbTotalProgress);
            this.Controls.Add(this.lblFilename);
            this.Controls.Add(this.pbCopyProgress);
            this.Controls.Add(this.txtLog);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Backup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.ProgressBar pbCopyProgress;
        private System.Windows.Forms.Label lblFilename;
        private System.Windows.Forms.ProgressBar pbTotalProgress;
        private System.Windows.Forms.Label lblMegaBytesPerSecond;
        private System.Windows.Forms.CheckBox cbxShutdownWhenFinished;
    }
}

