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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
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
            resources.ApplyResources(this.txtLog, "txtLog");
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            // 
            // pbCopyProgress
            // 
            resources.ApplyResources(this.pbCopyProgress, "pbCopyProgress");
            this.pbCopyProgress.Name = "pbCopyProgress";
            // 
            // lblFilename
            // 
            resources.ApplyResources(this.lblFilename, "lblFilename");
            this.lblFilename.Name = "lblFilename";
            // 
            // pbTotalProgress
            // 
            resources.ApplyResources(this.pbTotalProgress, "pbTotalProgress");
            this.pbTotalProgress.Name = "pbTotalProgress";
            // 
            // lblMegaBytesPerSecond
            // 
            resources.ApplyResources(this.lblMegaBytesPerSecond, "lblMegaBytesPerSecond");
            this.lblMegaBytesPerSecond.Name = "lblMegaBytesPerSecond";
            // 
            // cbxShutdownWhenFinished
            // 
            resources.ApplyResources(this.cbxShutdownWhenFinished, "cbxShutdownWhenFinished");
            this.cbxShutdownWhenFinished.Name = "cbxShutdownWhenFinished";
            this.cbxShutdownWhenFinished.UseVisualStyleBackColor = true;
            // 
            // FrmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbxShutdownWhenFinished);
            this.Controls.Add(this.lblMegaBytesPerSecond);
            this.Controls.Add(this.pbTotalProgress);
            this.Controls.Add(this.lblFilename);
            this.Controls.Add(this.pbCopyProgress);
            this.Controls.Add(this.txtLog);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMain";
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

