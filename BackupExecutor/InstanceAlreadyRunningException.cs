using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BackupExecutor
{
    class InstanceAlreadyRunningException : SystemException
    {
        public InstanceAlreadyRunningException(string message) : base(message)
        {
        }
    }
}
