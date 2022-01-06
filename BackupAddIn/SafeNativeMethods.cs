using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BackupAddIn
{
    static class SafeNativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct WIN32_FILE_ATTRIBUTE_DATA
        {
            public FileAttributes dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
        }

        public enum GET_FILEEX_INFO_LEVELS
        {
            GetFileExInfoStandard,
            GetFileExMaxInfoLevel
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool GetFileAttributesEx(string lpFileName, GET_FILEEX_INFO_LEVELS fInfoLevelId, out WIN32_FILE_ATTRIBUTE_DATA fileData);
    }
}
