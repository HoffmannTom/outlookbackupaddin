using BackupAddInCommon;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace BackupAddIn
{
    /// <summary>
    /// Settings dialog for backup configuration
    /// </summary>
    public partial class FBackupSettings : Form
    {
        private Microsoft.Office.Interop.Outlook.Stores stores;
        private BackupSettings config;

        /// <summary>
        /// Default constructor
        /// </summary>
        public FBackupSettings()
        {
            //entra aqui 
            InitializeComponent();
            GetSoftwareVersion();
        }


        private void GetSoftwareVersion()
        {
            // Get the currently executing assembly
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Get the version attribute from the assembly
            Version version = assembly.GetName().Version;

            // Alternatively, to get the AssemblyFileVersion
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            string fileVersion = "Not Available";
            if (attributes.Length > 0)
            {
                AssemblyFileVersionAttribute fileVersionAttribute = (AssemblyFileVersionAttribute)attributes[0];
                fileVersion = fileVersionAttribute.Version;
            }

            // Set the label text with the assembly version information
            label_version.Text = $"Assembly Version: {version}\nAssembly File Version: {fileVersion}";
        }

        /// <summary>
        /// Gets the configuration from disk and populates the form accordingly
        /// </summary>
        private void ApplySettings()
        {
            //BackupSettings config = BackupSettingsDao.loadSettings();

            if (config != null)
            {
                txtDestination.Text = config.DestinationPath;
                txtBackupExe.Text = config.BackupProgram;
                numInterval.Value = config.Interval;
                //(29-05-2024)mudança feita para aparecer sempre o mes a frente do backup
                DateTime inputDate = DateTime.Now;
                CultureInfo currentCulture = CultureInfo.CurrentCulture;
                var weekNum = currentCulture.Calendar.GetWeekOfYear(
                  inputDate,
                  CalendarWeekRule.FirstDay,
                  DayOfWeek.Monday);
                txtPrefix.Text = $"{DateTime.Now.Year.ToString()}_CW_{weekNum}";
                txtSuffix.Text = config.BackupSuffix;
                txtPostBackupCmd.Text = config.PostBackupCmd;
                numCountdown.Value = config.CountdownSeconds;

                foreach (String item in config.Items)
                {
                    foreach (ListViewItem lvItem in lvStores.Items)
                        if (lvItem.ImageKey.Equals(item))
                            lvItem.Checked = true;
                }

                cbxBackupAll.Checked = config.BackupAll;

                if (config.LastRun > DateTime.MinValue)
                    txtLastBackup.Text = config.LastRun.ToString("dd.MM.yyyy HH:mm:ss");
            }

            if (String.IsNullOrEmpty(txtBackupExe.Text))
            {
                //Check dll folder whether exe file exists
                String sFile = AppDomain.CurrentDomain.BaseDirectory;
                sFile = Path.Combine(sFile, "BackupExecutor.exe");
                if (File.Exists(sFile))
                {
                    txtBackupExe.Text = sFile;
                }

            }


        }

        /// <summary>
        /// Saves the current settings from the form to disk
        /// </summary>
        /// <returns>true, if save action was successful</returns>
        private bool SaveSettings()
        {
            //preserve hidden flags
            //BackupSettings config = BackupSettingsDao.loadSettings();
            if (config == null)
                config = new BackupSettings();
            config.DestinationPath = txtDestination.Text;
            config.Interval = (int)numInterval.Value;
            config.BackupProgram = txtBackupExe.Text;
            config.BackupPrefix = txtPrefix.Text;
            config.BackupSuffix = txtSuffix.Text;
            config.PostBackupCmd = txtPostBackupCmd.Text;
            config.BackupAll = cbxBackupAll.Checked;
            config.CountdownSeconds = (int)numCountdown.Value;
            if (String.IsNullOrEmpty(txtLastBackup.Text))
                config.LastRun = DateTime.MinValue;

            config.Items.Clear();
            for (int i = 0; i < lvStores.Items.Count; i++)
            {
                if (lvStores.Items[i].Checked)
                    config.Items.Add(lvStores.Items[i].ImageKey);
            }
            return BackupSettingsDao.SaveSettings(config);
        }


        private void BtnAbbrechen_click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnSpeichern_click(object sender, EventArgs e)
        {
            String sDest = Environment.ExpandEnvironmentVariables(txtDestination.Text);
            if (!Directory.Exists(sDest))
            {
                System.Windows.Forms.MessageBox.Show("Destination-folder doesn't exists!");
            }
            else
            {
                if (SaveSettings())
                    this.Close();
            }
        }

        /// <summary>
        /// Gets the list of outlook stores for further display
        /// </summary>
        /// <param name="stores"></param>
        internal void SetStores(Microsoft.Office.Interop.Outlook.Stores stores)
        {
            this.stores = stores;
        }

        private void BtnDirSelect_Click(object sender, EventArgs e)
        {
            DialogResult res = folderBrowserdlg.ShowDialog();
            if (res.Equals(DialogResult.OK))
                txtDestination.Text = folderBrowserdlg.SelectedPath;
        }

        private void BtnBackupSelect_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtBackupExe.Text))
                fileOpenDialog.InitialDirectory = Path.GetDirectoryName(txtBackupExe.Text);

            DialogResult res = fileOpenDialog.ShowDialog();
            if (res.Equals(DialogResult.OK))
                txtBackupExe.Text = fileOpenDialog.FileName;
        }

        /// <summary>
        /// Enables or disables the list of pst files
        /// </summary>
        private void SetBackupAll()
        {
            bool bBA = false;
            if (cbxBackupAll.Checked)
                bBA = true;

            lvStores.Enabled = !bBA;

            if (bBA)
                foreach (ListViewItem lvItem in lvStores.Items)
                    lvItem.Checked = bBA;
        }

        private void CbxBackupAll_CheckedChanged(object sender, EventArgs e)
        {
            SetBackupAll();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            txtLastBackup.Text = "";
        }

        //Primeira funcao quando abre o programa
        /// <summary>
        /// Populate form and display saved settings (if available)
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">OnLoad-Event-Args</param>
        private void FBackupSettings_Load(object sender, EventArgs e)
        {
            //cleanup
            txtDestination.Text = "";
            lvStores.Items.Clear();
            config = BackupSettingsDao.LoadSettings();

            //Add pst-files to list
            var list = BackupUtils.GetStoreLocations(config, stores);

            ListViewItem[] lItem = list.Select(f => new ListViewItem(f + " (" + GetHumanReadableFileSize(f) + ")", f))
                                      .ToArray();


            lvStores.Items.AddRange(lItem);

            lvStores.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            ApplySettings();
        }

        /// <summary>
        /// Return human readable file size of the given file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private String GetHumanReadableFileSize(String filename)
        {
            //Long path might occur
            //https://docs.microsoft.com/en-us/windows/win32/fileio/naming-a-file?redirectedfrom=MSDN#maxpath
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };

            long len;
            SafeNativeMethods.WIN32_FILE_ATTRIBUTE_DATA fileData;
            if (!SafeNativeMethods.GetFileAttributesEx(filename, SafeNativeMethods.GET_FILEEX_INFO_LEVELS.GetFileExInfoStandard, out fileData))
            {
                return "? KB";
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


            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }

    }
}
