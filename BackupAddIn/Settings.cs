using BackupAddInCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BackupAddIn
{
    /// <summary>
    /// Settings dialog for backup configuration
    /// </summary>
    public partial class FBackupSettings : Form
    {
        private Microsoft.Office.Interop.Outlook.Stores stores;
        private const String CONFIG_FILE_NAME = "OutlookBackup.config";

        /// <summary>
        /// Default constructor
        /// </summary>
        public FBackupSettings()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Determine config-file location
        /// </summary>
        /// <returns>Location of the config file</returns>
        public static String getConfigFilePath()
        {
            String sPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            object[] attributes = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

            if (attributes.Length != 0)
                sPath += Path.DirectorySeparatorChar + ((AssemblyCompanyAttribute)attributes[0]).Company;

            attributes = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (attributes.Length != 0)
                sPath += Path.DirectorySeparatorChar + ((AssemblyProductAttribute)attributes[0]).Product;

            return sPath + Path.DirectorySeparatorChar + CONFIG_FILE_NAME;
        }

        /// <summary>
        /// Returns the saved settings or null if not present
        /// </summary>
        /// <returns>Returns the saved settings from disk</returns>
        public static BackupSettings loadSettings()
        {
            String sFile = getConfigFilePath();
            BackupSettings config = null;

            if (File.Exists(sFile))
            {
                try
                {
                    using (Stream stream = File.Open(sFile, FileMode.Open))
                    {
                        XmlSerializer bin = new XmlSerializer(typeof(BackupSettings));
                        config = (BackupSettings)bin.Deserialize(stream);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Error during reading settings from file " + sFile,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return config;
        }

        /// <summary>
        /// Gets the configuration from disk and populates the form accordingly
        /// </summary>
        private void applySettings()
        {
            BackupSettings config = loadSettings();

            if (config != null)
            {

                txtDestination.Text = config.DestinationPath;

                txtBackupExe.Text = config.BackupProgram;

                numInterval.Value = config.Interval;

                foreach (String item in config.Items)
                {
                    foreach (ListViewItem lvItem in lvStores.Items)
                        if (lvItem.Text.Equals(item))
                            lvItem.Checked = true;
                }

                cbxBackupAll.Checked = config.BackupAll;

                if (config.LastRun > DateTime.MinValue)
                    txtLastBackup.Text = config.LastRun.ToString("dd.MM.yyyy HH:mm:ss");
            }
            else
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
        private bool saveSettings()
        {
            //preserve hidden flags
            BackupSettings config = loadSettings();
            if (config == null)
                config = new BackupSettings();
            config.DestinationPath = txtDestination.Text;
            config.Interval = (int)numInterval.Value;
            config.BackupProgram = txtBackupExe.Text;
            config.BackupAll = cbxBackupAll.Checked;
            if (String.IsNullOrEmpty(txtLastBackup.Text))
                config.LastRun = DateTime.MinValue;

            config.Items.Clear();
            for (int i = 0; i < lvStores.Items.Count; i++)
            {
                if (lvStores.Items[i].Checked)
                    config.Items.Add(lvStores.Items[i].Text);
            }
            return saveSettingsToFile(config);
        }

        /// <summary>
        /// Saves the current settings to disk
        /// </summary>
        /// /// <param name="config">Configration to save</param>
        /// <returns>true, if save action was successful</returns>
        public static bool saveSettingsToFile(BackupSettings config)
        {
            String sFile = getConfigFilePath();
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(sFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(sFile));

                using (Stream stream = File.Open(sFile, FileMode.Create))
                {
                    //BinaryFormatter bin = new BinaryFormatter();
                    XmlSerializer bin = new XmlSerializer(typeof(BackupSettings));
                    bin.Serialize(stream, config);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error during saving settings to file " + sFile,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void btnAbbrechen_click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSpeichern_click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txtDestination.Text))
            {
                System.Windows.Forms.MessageBox.Show("Destination-folder doesn't exists!");
            }
            else
            {
                if (saveSettings())
                    this.Close();
            }
        }

        /// <summary>
        /// Gets the list of outlook stores for further display
        /// </summary>
        /// <param name="stores"></param>
        internal void setStores(Microsoft.Office.Interop.Outlook.Stores stores)
        {
            this.stores = stores;
        }

        private void btnDirSelect_Click(object sender, EventArgs e)
        {
            DialogResult res = folderBrowserdlg.ShowDialog();
            if (res.Equals(DialogResult.OK))
                txtDestination.Text = folderBrowserdlg.SelectedPath;
        }

        private void btnBackupSelect_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtBackupExe.Text))
                fileOpenDialog.InitialDirectory = Path.GetDirectoryName(txtBackupExe.Text);

            DialogResult res = fileOpenDialog.ShowDialog();
            if (res.Equals(DialogResult.OK))
                txtBackupExe.Text = fileOpenDialog.FileName;
        }

        /// <summary>
        /// Enables or disables the list of psd files
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

        private void cbxBackupAll_CheckedChanged(object sender, EventArgs e)
        {
            SetBackupAll();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtLastBackup.Text = "";
        }


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

            //Add psd-files to list
            for (int i = 1; i <= stores.Count; i++)
            {
                try
                {
                    //MessageBox.Show("store " + i + ":" + stores[i].FilePath + Environment.NewLine
                    //    + stores[i].DisplayName);
                    //Ignore http- and imap-stores
                    //if (stores[i].IsDataFileStore)
                    if (stores[i].FilePath != null)
                        lvStores.Items.Add(stores[i].FilePath);

                }
                catch (Exception ex)
                {
                    //FilePath might be corrupt, check accounts -> files
                    MessageBox.Show("Error when iterating stores(" + i + "): " + ex.Message);
                }
            }
            /*
            for (int i = 1; i <= 20; i++)
                lvStores.Items.Add("test test test test test testes testes testest " + i);
            */

            lvStores.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            applySettings();

        }
    }
}
