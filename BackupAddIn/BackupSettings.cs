using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace BackupAddInCommon
{
    /// <summary>
    ///  Class holding the configuration options
    /// </summary>
    [Serializable()]
    public class BackupSettings
    {
        /// <summary>
        ///  Default constructor
        /// </summary>
        public BackupSettings()
        {
            Items = new StringCollection();
            IgnoreEncryption = false;
            CountdownSeconds = 0;
            Interval = 1;
            IntervalHours = 0;
            WaitTimeFileLock = 500;
            ShowOSTFiles = false;
            UseCompression = false;
            AllowSettingsAccess = true;
            BackupPrefix = "";
            BackupSuffix = "";
            PostBackupCmd = "";
            ShutdownWhenFinished = false;
        }

        /// <summary>
        ///  Destination path for the backup files
        /// </summary>
        public string DestinationPath { get; set; }

        /// <summary>
        ///  Program to run after exiting outlook
        /// </summary>
        public string BackupProgram { get; set; }

        /// <summary>
        ///  Interval in days when to run the backup program again
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        ///  Additional interval in hours which is added to the days
        /// </summary>
        public int IntervalHours { get; set; }

        /// <summary>
        ///  Date of the last run of backup
        /// </summary>
        public DateTime LastRun { get; set; }

        /// <summary>
        ///  List of files to backup
        /// </summary>
        public StringCollection Items { get; set; }

        /// <summary>
        ///  Wait time when file is locked
        /// </summary>
        public int WaitTimeFileLock { get; set; }

        /// <summary>
        ///  Prefix for filename of backup
        /// </summary>
        public string BackupPrefix { get; set; }

        /// <summary>
        ///  Suffix for filename of backup
        /// </summary>
        public string BackupSuffix { get; set; }

        /// <summary>
        ///  Flag whether to backup all pst-files
        /// </summary>
        public bool BackupAll { get; set; }

        /// <summary>
        ///  Command to run after backup finished
        /// </summary>
        public string PostBackupCmd { get; set; }

        /// <summary>
        ///  Try to decode path to OST-files
        /// </summary>
        public bool ShowOSTFiles { get; set; }

        /// <summary>
        ///  Flag whether encrypted files might get decrypted
        /// </summary>
        public bool IgnoreEncryption { get; set; }

        /// <summary>
        ///  Flag whether files should be compressed
        /// </summary>
        public bool UseCompression { get; set; }

        /// <summary>
        ///  Flag whether user can open settings dialog
        /// </summary>

        /* Nicht gemergte Änderung aus Projekt "BackupAddIn"
        Vor:
                public bool AllowSettingsAccess { get; set; }

                /// <summary>
        Nach:
                public bool AllowSettingsAccess { get; set; }

                /// <summary>
        */
        public bool AllowSettingsAccess { get; set; }

        /// <summary>
        /// flag for the default settings to shutdown when finished
        /// </summary>
        public bool ShutdownWhenFinished { get; set; }

        /// <summary>
        ///  Counter to give user opportunity to close window before backup starts
        /// </summary>
        private int countdownSeconds;

        /// <summary>
        ///  Counter to give user opportunity to close window before backup starts
        /// </summary>
        public int CountdownSeconds
        {
            get
            {
                return countdownSeconds;
            }
            set
            {
                if (value >= 0 && value <= 100)
                    countdownSeconds = value;
                else countdownSeconds = 0;
            }
        }

        /// <summary>
        ///  List of store types to hide
        /// </summary>
        public List<int> StoreTypeBlacklist { get; set; }

        /// <summary>
        ///  used profile name
        /// </summary>
        public string ProfileName { get; set; }
    }
}
