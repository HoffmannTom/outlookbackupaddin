using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        ///  Date of the last run of backup
        /// </summary>
        public DateTime LastRun { get; set; }

        /// <summary>
        ///  List of files to backup
        /// </summary>
        public StringCollection Items;

        /// <summary>
        ///  Flag whether to backup all psd-files
        /// </summary>
        public bool BackupAll { get; set; }

        /// <summary>
        ///  Optional File-Extension
        /// </summary>
        public String BackupExtension { get; set; }
    }
}
