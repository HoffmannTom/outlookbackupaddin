using Microsoft.Win32;
using BackupAddInCommon;
using System;
using System.Windows.Forms;


namespace BackupExecutor
{
    /// <summary>
    ///  Main window of the backup program
    /// </summary>
    public partial class frmMain : Form
    {
        /// <summary>
        ///  Default constructor
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
        }

        private void LogToScreen(String s)
        {
            txtLog.Text += s + Environment.NewLine;
            txtLog.Refresh();
        }

        /// <summary>
        ///  Load configuration and execute backup if necessary
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Show();

            int iError = 0;
            BackupSettings config = BackupTool.getSettings();

            if (config != null && config.LastRun == null)
            {
                iError = BackupTool.tryBackup(config, LogToScreen);
            }
            else if (config != null && config.LastRun.AddDays(config.Interval) <= DateTime.Now)
            {
                iError = BackupTool.tryBackup(config, LogToScreen);
            }

            if (iError == 0)
                Application.Exit();
        }

    }
}
