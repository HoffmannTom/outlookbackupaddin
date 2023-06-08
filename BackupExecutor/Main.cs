using BackupAddInCommon;
using Microsoft.Win32;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackupExecutor
{
    /// <summary>
    ///  Main window of the backup program
    /// </summary>
    public partial class FrmMain : Form
    {
        private static readonly String REG_PATH_EXECUTOR_SETTINGS = @"Software\CodePlex\BackupAddIn\ExecutorSettings";
        private readonly SynchronizationContext m_SynchronizationContext;
        //private static StringBuilder sbLogs = new StringBuilder();
        //private static String EVENT_SRC = "Application Error";
        /// <summary>
        ///  Default constructor
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();
            m_SynchronizationContext = SynchronizationContext.Current;
        }

        private void LogToScreen(String s)
        {
            m_SynchronizationContext.Post((@object) =>
            {
                String s2 = (String)@object;
                s2 = DateTime.Now.TimeOfDay.ToString("hh\\:mm\\:ss") + " " + s2 + Environment.NewLine;
                txtLog.AppendText(s2);
                //sbLogs.Append(s2);
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

            ApplySettingsFromRegistry();

            StartAsyncWork();
        }

        private bool ApplySettingsFromRegistry()
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

        private bool SaveSettingsToRegistry()
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

        private /*async*/ void StartAsyncWork()
        {
            Console.WriteLine("DoWork Starting");
            //await Task.Run(() =>
            Task t = Task.Factory.StartNew(() =>
            {
                int iError = 0;
                BackupSettings config = BackupSettingsDao.LoadSettings();

                BackupTool.SetFileLabel(this.lblFilename);
                BackupTool.SetProgressBar(this.pbCopyProgress);
                BackupTool.SetTotalProgressBar(this.pbTotalProgress);
                BackupTool.SetMegaByesPerSecondLabel(this.lblMegaBytesPerSecond);

                if (config != null && config.LastRun == null)
                {
                    StartCountdown(config, LogToScreen);
                    iError = BackupTool.TryBackup(config, LogToScreen);
                }
                else if (config != null && config.LastRun.AddDays(config.Interval).AddHours(config.IntervalHours) <= DateTime.Now)
                {
                    StartCountdown(config, LogToScreen);
                    iError = BackupTool.TryBackup(config, LogToScreen);
                }

                //if (!EventLog.SourceExists(EVENT_SRC))
                //    EventLog.CreateEventSource(EVENT_SRC, "Application");

                if (iError == 0)
                {
                    //if (sbLogs.Length > 0)
                    //    EventLog.WriteEntry(EVENT_SRC, sbLogs.ToString(), EventLogEntryType.Information);

                    if (cbxShutdownWhenFinished.Checked)
                    {
                        BackupTool.ShutdownComputer();
                    }

                    Application.Exit();
                }
                else
                {
                    BackupTool.CanExit = true; //allow manual closing
                    LogToScreen("One or more errors occurred. Please check the messages above and close this window manually.");
                    //if (sbLogs.Length > 0)
                    //    EventLog.WriteEntry(EVENT_SRC, sbLogs.ToString(), EventLogEntryType.Warning);
                }
            });
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = !BackupTool.CanExit;

            //if closing allowed, safe settings
            if (!e.Cancel)
                SaveSettingsToRegistry();
        }

        private void StartCountdown(BackupSettings config, BackupTool.Logger Log)
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
