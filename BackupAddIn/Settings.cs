using Speed4Trade;
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
    public partial class FBackupSettings : Form
    {
        private Microsoft.Office.Interop.Outlook.Stores stores;
        private const String CONFIG_FILE_NAME = "OutlookBackup.config";

        public FBackupSettings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Populate form and display saved settings (if available)
        /// </summary>
        /// <param name="e">OnLoad-Event-Args</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //cleanup
            txtDestination.Text = "";
            lvStores.Items.Clear();

            //Add psd-files to list
            for (int i = 1; i <= stores.Count; i++)
            {
                lvStores.Items.Add(stores[i].FilePath);
            }

            applySettings();
        }

        /// <summary>
        /// Determine config-file location
        /// </summary>
        /// <returns>Location of the config file</returns>
        public static String getConfigFilePath()
        {
            String sPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            object[] attributes = Assembly.GetAssembly(typeof(FBackupSettings)).GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

            if (attributes.Length != 0)
                sPath += Path.DirectorySeparatorChar + ((AssemblyCompanyAttribute)attributes[0]).Company;

            attributes = Assembly.GetAssembly(typeof(FBackupSettings)).GetCustomAttributes(typeof(AssemblyProductAttribute), false);
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
                catch (IOException)
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
                /*
                System.Configuration.ExeConfigurationFileMap con = new System.Configuration.ExeConfigurationFileMap();
                con.ExeConfigFilename = sFile;
                con.RoamingUserConfigFilename = sFile;
                con.LocalUserConfigFilename = sFile;

                System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(con, ConfigurationUserLevel.PerUserRoamingAndLocal);

                BackupSettingsSection sec = (BackupSettingsSection)config.GetSection("BackupSettings");
                if (sec != null)
                {
                    txtDestination.Text = sec.DestinationPath;

                    numInterval.Value = sec.Interval;

                    foreach (Item item in sec.Items)
                    {
                        foreach (ListViewItem lvItem in lvStores.Items)
                            if (lvItem.Text.Equals(item.Value))
                                lvItem.Checked = true;
                    }

                }
                */

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

            }
        }

        /// <summary>
        /// Saves the current settings from the form to disk
        /// </summary>
        /// <returns>true, if save action was successful</returns>
        private bool saveSettings()
        {
           String sFile = getConfigFilePath();
            /*
           System.Configuration.ExeConfigurationFileMap con = new System.Configuration.ExeConfigurationFileMap();
           con.ExeConfigFilename = sFile;
           con.RoamingUserConfigFilename = sFile;
           con.LocalUserConfigFilename = sFile;
           System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(con, ConfigurationUserLevel.PerUserRoamingAndLocal);

           BackupSettingsSection sec = (BackupSettingsSection)config.GetSection("BackupSettings");
           if (sec == null)
           {
               sec = new BackupSettingsSection();
               sec.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;

               //sec.LockItem = false;
               config.Sections.Add("BackupSettings", sec);
           }

           sec.DestinationPath = txtDestination.Text;

           sec.Interval = (int)numInterval.Value;

           sec.Items.Clear();
           for (int i = 0; i < lvStores.Items.Count; i ++)
           {
               if (lvStores.Items[i].Checked)
                   sec.Items.Add(new Item(lvStores.Items[i].Text));
           }

           config.Save();
           */

            BackupSettings config = new BackupSettings();
            config.DestinationPath = txtDestination.Text;
            config.Interval = (int)numInterval.Value;
            config.BackupProgram = txtBackupExe.Text;

            for (int i = 0; i < lvStores.Items.Count; i++)
            {
                if (lvStores.Items[i].Checked)
                    config.Items.Add(lvStores.Items[i].Text);
            }
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
            catch (IOException)
            {
                MessageBox.Show("Error during saving settings to file " + sFile,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            DialogResult res = fileOpenDialog.ShowDialog();
            if (res.Equals(DialogResult.OK))
                txtBackupExe.Text = fileOpenDialog.FileName;
        }
    }
}
