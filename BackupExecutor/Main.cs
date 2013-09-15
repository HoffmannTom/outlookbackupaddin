using Microsoft.Win32;
using BackupAddInCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BackupExecutor
{
    /// <summary>
    ///  Main window of the backup program
    /// </summary>
    public partial class frmMain : Form
    {
        private const String CONFIG_FILE_NAME = "OutlookBackup.config";
        private const String OUTLOOK_PROC = "OUTLOOK";

        /// <summary>
        ///  Default constructor
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        ///  Load configuration and execute backup if necessary
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Show();

            int iError = 0;
            BackupSettings config = getSettings();

            if (config != null && config.LastRun == null)
            {
                iError = tryBackup(config);
            }
            else if (config != null && config.LastRun.AddDays(config.Interval) <= DateTime.Now)
            {
                iError = tryBackup(config);
            }

            if (iError == 0)
                Application.Exit();
        }


        /// <summary>
        /// Evaluates the location of the config file
        /// </summary>
        /// <returns>filname including path to the config file</returns>
        public static String getConfigFilePath()
        {
            String sPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            object[] attributes = Assembly.GetAssembly(typeof(frmMain)).GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

            if (attributes.Length != 0)
                sPath += Path.DirectorySeparatorChar + ((AssemblyCompanyAttribute)attributes[0]).Company;

            sPath += Path.DirectorySeparatorChar + "BackupAddIn";

            return sPath + Path.DirectorySeparatorChar + CONFIG_FILE_NAME;
        }

        /// <summary>
        /// Read and deserialize config from file
        /// </summary>
        /// <returns>saved settings from outlook plugin</returns>
        private BackupSettings getSettings()
        {
            BackupSettings config = null;
            String sFile = getConfigFilePath();
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
        /// Waits, till outlook ends and then starts the backup process
        /// </summary>
        /// <param name="config">Stored configuration from outlook plugin</param>
        /// <returns>number of occured errors</returns>
        private int tryBackup(BackupSettings config)
        {
            int iError = 0;

            if (config.Items.Count > 0)
            {
                if (WaitForProcessEnd(OUTLOOK_PROC))
                    iError += doBackup(config);
                else
                {
                    iError++;
                    txtLog.Text += "Error waiting for " + OUTLOOK_PROC + Environment.NewLine;
                }
            }

            return iError;
        }

        /// <summary>
        /// Tries to backup the configures files from outlook
        /// </summary>
        /// <param name="config">Stored configuration from outlook plugin</param>
        /// <returns>number of occured errors</returns>
        private int doBackup(BackupSettings config)
        {
            int iError = 0;
            String sPath = config.DestinationPath;

            if (!sPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                sPath += Path.DirectorySeparatorChar;

            foreach (String item in config.Items)
            {
                try
                {
                    txtLog.Text += "copy " + item + " to " + config.DestinationPath + Environment.NewLine;
                    WaitForFile(item);
                    File.Copy(item, sPath + Path.GetFileName(item), true);
                }
                catch (Exception e)
                {
                    iError++;
                    txtLog.Text += e.Message + Environment.NewLine;
                }
            }

            config.LastRun = DateTime.Now;
            saveConfig(config);

            return iError;
        }

        /// <summary>
        /// Wait max. 5 seconds for releasing locks on the file
        /// </summary>
        /// <param name="item">filename including path</param>
        /// <returns></returns>
        private bool WaitForFile(string item)
        {
            int i = 0;
            FileInfo fi = new FileInfo(item);
            while (IsFileLocked(fi) && i < 10)
            {
                Thread.Sleep(500);
                i++;
            }

            return (i < 10);
        }

        /// <summary>
        /// Saves the configuration to disk / updates last-run
        /// </summary>
        /// <param name="config">config which should be saved</param>
        private void saveConfig(BackupSettings config)
        {
            String sFile = getConfigFilePath();
            try
            {
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
        }

        /// <summary>
        /// Waits, till the process is not running any more
        /// </summary>
        /// <param name="name">process name (!) within task manager</param>
        /// <returns>returns true if the process is not running any more</returns>
        public bool WaitForProcessEnd(string name)
        {
            int i = 0;
            while (i < 10 && IsProcessOpen(name))
            {
                txtLog.Text += "Waiting for " + name + "..." + Environment.NewLine;
                this.Refresh();
                Application.DoEvents();
                System.Threading.Thread.Sleep(1000);
                i++;
            }

            return !IsProcessOpen(name);
        }

        /// <summary>
        /// Checks whether a certain process is running
        /// </summary>
        /// <param name="name">process name as seen witin the task  manager</param>
        /// <returns>true, if process is still running</returns>
        public bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                try
                {
                    //txtLog.Text += clsProcess.ProcessName + Environment.NewLine;// MainModule.ModuleName ;
                    if (clsProcess.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                catch (Exception )
                {
                    //some processes are dont like to be inspected... just go on...
                }
            }
            return false;
        }

        /// <summary>
        /// Check whether a file is locked 
        /// </summary>
        /// <param name="file">file to be checked</param>
        /// <returns></returns>
        protected bool IsFileLocked(FileInfo file)
        {
            if (!file.Exists)
                return false;

            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {

                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist 
                txtLog.Text += "File is locked: " + file + Environment.NewLine;
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

    }
}
