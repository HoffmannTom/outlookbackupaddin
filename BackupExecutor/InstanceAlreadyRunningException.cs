using System;

namespace BackupExecutor
{
    [Serializable()]
    class InstanceAlreadyRunningException : SystemException
    {
        public InstanceAlreadyRunningException(string message) : base(message)
        {
        }
    }
}
