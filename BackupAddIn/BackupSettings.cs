using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speed4Trade
{
    [Serializable()]
    public class BackupSettings
    {
        public BackupSettings()
        {
            Items = new StringCollection();
        }

        public string           DestinationPath { get; set; }
        public string           BackupProgram { get; set; }
        public int              Interval { get; set; }
        public DateTime         LastRun { get; set; }
        public StringCollection Items;

    }
}
