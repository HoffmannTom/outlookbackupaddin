using BackupAddInCommon;
using BackupExecutor.Models;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BackupExecutor.BackupTool;

namespace BackupExecutor
{
    /// <summary>
    ///  Main window of the backup program
    /// </summary>
    public partial class FrmMain : Form
    {
        private static readonly String REG_PATH_EXECUTOR_SETTINGS = @"Software\ITEC\BackupAddIn\ExecutorSettings";
        private readonly SynchronizationContext m_SynchronizationContext;
        //private static StringBuilder sbLogs = new StringBuilder();
        //private static String EVENT_SRC = "Application Error";
        /// <summary>
        ///  Default constructor
        /// </summary>
        public FrmMain()
        {
            BackupSettings config = BackupSettingsDao.LoadSettings();
            foreach (string item in config.Items)
            {

                int result = GetFileSizeover15(item);

                if (result == 0)
                {
                    //CanExit = true;
                    //\n(adicionar o website aqui para adicionar os tutorias de como fazer o um arquivo e resize do ficheiro) 
                    MessageBox.Show($"O ficheiro tem mais que 15 GB é recomendado fazer um arquivo de ano a ano,e depois dar resize do ficheiro (Peça ajuda ao GG)", "TAMANHO DO FICHEIRO", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Thread.Sleep(1000);
                    //Environment.Exit(0);
                }

            }
            if (DialogResult.No == MessageBox.Show("Pretende fazer o backup?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                CanExit = true;
                Thread.Sleep(1000);
                Environment.Exit(0);

            }
            InitializeComponent();
            m_SynchronizationContext = SynchronizationContext.Current;


        }

        private void LogToScreen(String s)
        {
            m_SynchronizationContext.Post((@object) =>
            {
                String s2 = (String)@object;
                s2 = DateTime.Now.TimeOfDay.ToString("hh\\:mm\\:ss") + " " + s2 + Environment.NewLine;
                CreateLog.CriarLog(s2);
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

        private int GetFileSizeover15(string item)
        {

            //Long path might occur
            //https://docs.microsoft.com/en-us/windows/win32/fileio/naming-a-file?redirectedfrom=MSDN#maxpath
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };

            long len;

            //verificar este if
            SafeNativeMethods.WIN32_FILE_ATTRIBUTE_DATA fileData;
            if (!SafeNativeMethods.GetFileAttributesEx(item, SafeNativeMethods.GET_FILEEX_INFO_LEVELS.GetFileExInfoStandard, out fileData))
            {
                return 0;
            }
            len = (long)(((ulong)fileData.nFileSizeHigh << 32) + (ulong)fileData.nFileSizeLow);

            //algoritmo para converter o tamanho do ficheiro
            //double len = new FileInfo(filename).Length;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            if (len >= 15 && order == 3)
            {
                return 0;
            }

            return 1;
        }



        private async void StartAsyncWork()
        {
            LogToScreen("DoWork Starting VERSION->1.1.0.5");

            BackupSettings config = BackupSettingsDao.LoadSettings();
            DateTime inputDate = DateTime.Now;
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            var weekNum = currentCulture.Calendar.GetWeekOfYear(
                  inputDate,
                  CalendarWeekRule.FirstDay,
                  DayOfWeek.Monday);
            config.BackupPrefix = $"{DateTime.Now.Year.ToString()}_CW_{weekNum}";
            int iError = 1;

            BackupTool.SetFileLabel(this.lblFilename);
            BackupTool.SetProgressBar(this.pbCopyProgress);
            BackupTool.SetTotalProgressBar(this.pbTotalProgress);
            BackupTool.SetMegaByesPerSecondLabel(this.lblMegaBytesPerSecond);



            if (config != null && config.LastRun == null)
            {
                await StartCountdownAsync(config, LogToScreen); // Await the countdown
                iError = await Task.Run(() => BackupTool.TryBackup(config, LogToScreen));
            }
            else if (config != null && config.LastRun.AddDays(config.Interval).AddHours(config.IntervalHours) <= DateTime.Now)
            {
                await StartCountdownAsync(config, LogToScreen); // Await the countdown
                iError = await Task.Run(() => BackupTool.TryBackup(config, LogToScreen));
            }

            if (iError == 0)
            {
                bool resultemail = await Task.Run(() => Utils.SendSMTPEmail(LogToScreen, config));
                if (!resultemail)
                {
                    LogToScreen("No internet connection. Email has been queued.");
                }
                else
                {
                    LogToScreen("Email sent successfully.");
                }

                string url = "https://api.itecapi.duckdns.org/add";
                await Utils.SendPostRequestAsync(url, config, LogToScreen);

                if (cbxShutdownWhenFinished.Checked)
                {
                    LogToScreen("Shutting down ...");
                    await Task.Delay(1000);
                    BackupTool.ShutdownComputer();
                }

                await Task.Delay(1000);
                BackupTool.CanExit = true;
                // Application.Exit();
            }
            else
            {
                BackupTool.CanExit = true; //allow manual closing
                LogToScreen("One or more errors occurred. Please check the messages above and close this window manually.");
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = !BackupTool.CanExit;

            if (!e.Cancel)
                SaveSettingsToRegistry();
        }

        private async Task StartCountdownAsync(BackupSettings config, BackupTool.Logger Log)
        {
            BackupTool.CanExit = true;
            for (int i = config.CountdownSeconds; i > 0; i--)
            {
                if (i > 1)
                    Log("Starting backup in " + i + " seconds");
                else
                    Log("Starting backup in " + i + " second");

                await Task.Delay(1000); // Use await Task.Delay instead of Thread.Sleep for async
            }
            BackupTool.CanExit = false;
        }
    }
}
