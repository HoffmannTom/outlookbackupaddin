using Microsoft.Win32;
using BackupAddInCommon;
using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;


namespace BackupExecutor
{
    /// <summary>
    ///  Main window of the backup program
    /// </summary>
    public partial class frmMain : Form
    {
        private SynchronizationContext m_SynchronizationContext;
        /// <summary>
        ///  Default constructor
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
            m_SynchronizationContext = SynchronizationContext.Current;
        }

        private void LogToScreen(String s)
        {
            m_SynchronizationContext.Post((@object) =>
            {
                String s2 = (String)@object;
                txtLog.Text += s2 + Environment.NewLine;
                txtLog.Refresh();
            }, s);
        }

        /// <summary>
        ///  Load configuration and execute backup if necessary
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Show();

            startAsyncWork();
        }

        private /*async*/ void startAsyncWork()
        {
            Console.WriteLine("DoWork Starting");
            //await Task.Run(() =>
            Task t = Task.Factory.StartNew(() =>
            {
                int iError = 0;
                BackupSettings config = BackupSettingsDao.loadSettings();

                BackupTool.setFileLabel(this.lblFilename);
                BackupTool.setProgressBar(this.pbCopyProgress);
                BackupTool.setTotalProgressBar(this.pbTotalProgress);
                BackupTool.setMegaByesPerSecondLabel(this.lblMegaBytesPerSecond);

                if (config != null && config.LastRun == null)
                {
                    startCountdown(config, LogToScreen);
                    iError = BackupTool.tryBackup(config, LogToScreen);
                }
                else if (config != null && config.LastRun.AddDays(config.Interval) <= DateTime.Now)
                {
                    startCountdown(config, LogToScreen);
                    iError = BackupTool.tryBackup(config, LogToScreen);
                }

                if (iError == 0)
                    Application.Exit();
            });
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = !BackupTool.CanExit;
        }

        private void startCountdown(BackupSettings config, BackupTool.Logger LogToScreen)
        {
            BackupTool.CanExit = true;
            for (int i = config.CountdownSeconds; i > 0; i--)
            {
                if (i > 1)
                     LogToScreen("Starting backup in " + i + " seconds");
                else LogToScreen("Starting backup in " + i + " second");

                Thread.Sleep(1000);
            }

            //no manual exit possible any more
            BackupTool.CanExit = false;
        }


    }
}
