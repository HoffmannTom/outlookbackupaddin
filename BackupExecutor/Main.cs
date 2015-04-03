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

        private async void startAsyncWork()
        {
            Console.WriteLine("DoWork Starting");
            await Task.Run(() =>
            {
                int iError = 0;
                BackupSettings config = BackupTool.getSettings();

                BackupTool.setFileLabel(this.lblFilename);
                BackupTool.setProgressBar(this.pbCopyProgress);

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
            });
        }


    }
}
