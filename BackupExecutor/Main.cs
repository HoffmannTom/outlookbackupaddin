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
        private static String REG_PATH_EXECUTOR_SETTINGS = @"Software\CodePlex\BackupAddIn\ExecutorSettings";
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
                txtLog.AppendText(s2 + Environment.NewLine);
                txtLog.Refresh();
                //txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
            }, s);
        }

        /// <summary>
        ///  Load configuration and execute backup if necessary
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Show();

            applySettingsFromRegistry();

            startAsyncWork();
        }

        private bool applySettingsFromRegistry()
        {
            try
            {
                RegistryKey appKey = Registry.CurrentUser.OpenSubKey(REG_PATH_EXECUTOR_SETTINGS, false);
                if (appKey != null)
                {
                    int iHeight = (int)appKey.GetValue("WindowHeight", 0);
                    int iWidth = (int)appKey.GetValue("WindowWidth", 0);

                    //do some plausbility checks before applying
                    if (iHeight < this.MinimumSize.Height)
                        iHeight = this.MinimumSize.Height;
                    if (iHeight > SystemInformation.VirtualScreen.Height)
                        iHeight = SystemInformation.VirtualScreen.Height;

                    if (iWidth < this.MinimumSize.Width)
                        iWidth = this.MinimumSize.Width;
                    if (iWidth > SystemInformation.VirtualScreen.Width)
                        iWidth = SystemInformation.VirtualScreen.Width;

                    this.Height = iHeight;
                    this.Width = iWidth;

                    appKey.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }

        private bool saveSettingsToRegistry()
        {
            try
            {
                RegistryKey appKey = Registry.CurrentUser.CreateSubKey(REG_PATH_EXECUTOR_SETTINGS);

                appKey.SetValue("WindowHeight", this.Height);
                appKey.SetValue("WindowWidth", this.Width);

                appKey.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
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

            //if closing allowed, safe settings
            if (!e.Cancel)
                saveSettingsToRegistry();
        }

        private void startCountdown(BackupSettings config, BackupTool.Logger Log)
        {
            BackupTool.CanExit = true;
            for (int i = config.CountdownSeconds; i > 0; i--)
            {
                if (i > 1)
                     Log("Starting backup in " + i + " seconds");
                else Log("Starting backup in " + i + " second");

                Thread.Sleep(1000);
            }

            //no manual exit possible any more
            BackupTool.CanExit = false;
        }


    }
}
