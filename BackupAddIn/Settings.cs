using BackupAddInCommon;
using Microsoft.Office.Interop.Outlook;
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
        private BackupSettings config;

        /// <summary>
        /// Default constructor
        /// </summary>
        public FBackupSettings()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Gets the configuration from disk and populates the form accordingly
        /// </summary>
        private void applySettings()
        {
            //BackupSettings config = BackupSettingsDao.loadSettings();

            if (config != null)
            {
                txtDestination.Text = config.DestinationPath;
                txtBackupExe.Text = config.BackupProgram;
                numInterval.Value = config.Interval;
                txtPrefix.Text = config.BackupPrefix;
                txtSuffix.Text = config.BackupSuffix;
                txtPostBackupCmd.Text = config.PostBackupCmd;

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
        private bool saveSettings()
        {
            //preserve hidden flags
            //BackupSettings config = BackupSettingsDao.loadSettings();
            if (config == null)
                config = new BackupSettings();
            config.DestinationPath = txtDestination.Text;
            config.Interval        = (int)numInterval.Value;
            config.BackupProgram   = txtBackupExe.Text;
            config.BackupPrefix    = txtPrefix.Text;
            config.BackupSuffix    = txtSuffix.Text;
            config.PostBackupCmd   = txtPostBackupCmd.Text;
            config.BackupAll       = cbxBackupAll.Checked;
            if (String.IsNullOrEmpty(txtLastBackup.Text))
                config.LastRun = DateTime.MinValue;

            config.Items.Clear();
            for (int i = 0; i < lvStores.Items.Count; i++)
            {
                if (lvStores.Items[i].Checked)
                    config.Items.Add(lvStores.Items[i].Text);
            }
            return BackupSettingsDao.saveSettings(config);
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
            config = BackupSettingsDao.loadSettings();

            //Add pst-files to list
            var list =  BackupUtils.GetStoreLocations(config, stores);
            ListViewItem[] lItem = list.Select(X => new ListViewItem(X)).ToArray();
            lvStores.Items.AddRange(lItem);

            /*
            for (int i = 1; i <= 20; i++)
                lvStores.Items.Add("test test test test test testes testes testest " + i);
            */

            lvStores.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            applySettings();
        }
    }
}
