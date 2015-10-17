using BackupAddInCommon;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackupExecutor
{
    internal static class SafeNativeMethods
    {
        [DllImport("kernel32.dll")]
        internal static extern bool AttachConsole(int dwProcessId);
        internal static int ATTACH_PARENT_PROCESS = -1;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int FreeConsole();

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool GetBinaryType(string lpApplicationName, out uint lpBinaryType);
        internal static int SCS_64BIT_BINARY = 6;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName,
           CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref bool pbCancel,
           CopyFileFlags dwCopyFlags);

        public delegate CopyProgressResult CopyProgressRoutine(
            long TotalFileSize,
            long TotalBytesTransferred,
            long StreamSize,
            long StreamBytesTransferred,
            uint dwStreamNumber,
            CopyProgressCallbackReason dwCallbackReason,
            IntPtr hSourceFile,
            IntPtr hDestinationFile,
            IntPtr lpData);

        public enum CopyProgressCallbackReason : uint
        {
            CALLBACK_CHUNK_FINISHED = 0x00000000,
            CALLBACK_STREAM_SWITCH = 0x00000001
        }

        public enum CopyProgressResult : uint
        {
            PROGRESS_CONTINUE = 0,
            PROGRESS_CANCEL = 1,
            PROGRESS_STOP = 2,
            PROGRESS_QUIET = 3
        }

        [Flags]
        public enum CopyFileFlags : uint
        {
            COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
            COPY_FILE_RESTARTABLE = 0x00000002,
            COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
            COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008,
            COPY_FILE_COPY_SYMLINK = 0x00000800 //NT 6.0+
        }
    }

    static class Program
    {
        static String outlookPath;

        public static bool IsElevated
        {
            get
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            int RetCode = 0;

            Dictionary<String, String> argsDict = new Dictionary<String, String>();
            if (!parseArgs(args, argsDict))
            {
                showHelp(args);
                return 0;
            }

            if (argsDict.ContainsKey("/register"))
            {
                RetCode = registerPlugin(true) ? 0 : 1;
            }
            else if (argsDict.ContainsKey("/registersetup"))
            {
                RetCode = registerPlugin(false) ? 0 : 1;
            }
            else if (argsDict.ContainsKey("/unregister"))
            {
                RetCode = unregisterPlugin() ? 0 : 1;
            }
            else if (argsDict.ContainsKey("/backupnow"))
            {
                SafeNativeMethods.AttachConsole(SafeNativeMethods.ATTACH_PARENT_PROCESS);

                BackupSettings config = BackupSettingsDao.loadSettings();
                int iError = BackupTool.tryBackup(config, LogToConsole);

                SendKeys.SendWait("{ENTER}");
                SafeNativeMethods.FreeConsole();
                RetCode = iError;
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }

            return RetCode;
        }

        private static bool parseArgs(string[] args, Dictionary<string, string> argsDict)
        {
            bool bOK = true;
            int iMainArgs = 0;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "/register" || args[i] == "/registersetup" ||
                    args[i] == "/unregister" || args[i] == "/backupnow")
                {
                    argsDict.Add(args[i], "");
                    iMainArgs++;
                }
                /*else if (args[i].StartsWith("/profile="))
                {
                    String val = args[i].Substring(9);
                    argsDict.Add("/profile", val);
                }*/
                else bOK = false;
            }

            return bOK && (iMainArgs <= 1);
        }

        /// <summary>
        ///  print errors to console
        /// </summary>
        private static void LogToConsole(string s)
        {
            Console.WriteLine(s + Environment.NewLine);
        }

        /// <summary>
        ///  print valid parameters and help information to console
        /// </summary>
        private static void showHelp(string[] args)
        {
            SafeNativeMethods.AttachConsole(SafeNativeMethods.ATTACH_PARENT_PROCESS);

            Console.WriteLine("");
            Console.WriteLine("Unknown option " + args[0]);
            Console.WriteLine("The following parameters are available:");
            Console.WriteLine("/register    will register the plugin and create the necessary registry settings");
            Console.WriteLine("/unregister  will deactivate the plugin and delete the registry settings");
            Console.WriteLine("/backupnow   starts the backup without taking the elapsed time since last backup into account");
            SendKeys.SendWait("{ENTER}");

            SafeNativeMethods.FreeConsole();
        }


        /// <summary>
        ///  Check whether outlook-executable is 64 bit
        /// </summary>
        private static bool Is64BitOutlookFromRegisteredExe()
        {
            uint binaryType;
            bool bRet = false;

            if (String.IsNullOrEmpty(outlookPath))
                outlookPath = getOutlookPath();

            if (String.IsNullOrEmpty(outlookPath))
                throw new Exception("Outlook not found!");

            if (Environment.Is64BitOperatingSystem)
            {
                try
                {
                    if (SafeNativeMethods.GetBinaryType(outlookPath, out binaryType))
                        bRet = (binaryType == SafeNativeMethods.SCS_64BIT_BINARY);
                }
                catch (Exception /*e*/)
                {
                    //MessageBox.Show(e.Message);
                  // Ignore - better just to assume it's 32-bit than to let the installation
                  // fail.  This could fail because the GetBinaryType function is not
                  // available.  I understand it's only available in Windows 2000
                  // Professional onwards.
                }
            }

            return bRet;
        }

        /// <summary>
        ///  Try to evaluate the outlook path. If it fails, choose manually
        /// </summary>
        private static string getOutlookPath()
        {
            String sPath = "";

            // Default value - assume 32-bit unless proven otherwise.
            // RegQueryStringValue second param is '' to get the (default) value for the key
            // with no sub-key name, as described at
            // http://stackoverflow.com/questions/913938/

            //better: http://support.microsoft.com/kb/240794
            String clsid = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Classes\Outlook.Application\CLSID", "", null);

            if (clsid != null)
            {
                //outlookPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE", "", null);
                //--> Sometimes the app path is only stored on wow6432node 

                sPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Classes\CLSID\" + clsid + @"\LocalServer32", "", null);
                if (String.IsNullOrEmpty(sPath) && Environment.Is64BitOperatingSystem)
                {
                    //if it is Office64 on Win64, try 64-Bit Key (current programm running with 32-Bit):
                    RegistryKey  tmpKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
                    tmpKey = tmpKey.OpenSubKey(@"Software\Classes\CLSID\" + clsid + @"\LocalServer32");
                    if (tmpKey != null)
                        sPath = (string)tmpKey.GetValue("");
                }
            }

            if (String.IsNullOrEmpty(sPath))
            {
                OpenFileDialog odlg = new OpenFileDialog();
                odlg.DefaultExt = ".exe";
                odlg.CheckFileExists = true;
                odlg.Filter = "Outlook.exe|outlook.exe";
                odlg.Title = "Please select outlook.exe from the installation folder";
                if (odlg.ShowDialog() == DialogResult.OK)
                {
                    sPath = odlg.FileName;
                }
            }

            //                throw new Exception("CLSID of outlook.application not found in registry!");
            return sPath;
        }


        /// <summary>
        ///  Create registry settings for outlook plugin and copy files to installation folder
        ///  depending on bit-ness
        /// </summary>
        private static bool registerPlugin(bool sendRet)
        {
            SafeNativeMethods.AttachConsole(SafeNativeMethods.ATTACH_PARENT_PROCESS);
            try
            {
                RegistryKey tmpKey = getOutlookRootKey();

                Console.WriteLine("Creating subkey Addins");
                tmpKey = tmpKey.CreateSubKey("Addins");

                Console.WriteLine("Creating subkey Codeplex.BackupAddIn");
                tmpKey = tmpKey.CreateSubKey("Codeplex.BackupAddIn");

                String sDir = AppDomain.CurrentDomain.BaseDirectory; //Directory.GetCurrentDirectory();
                tmpKey.SetValue("Description", "Outlook-Backup-Addin", RegistryValueKind.String);
                tmpKey.SetValue("FriendlyName", "Outlook Backup Addin", RegistryValueKind.String);
                tmpKey.SetValue("Manifest", System.IO.Path.Combine(sDir, "BackupAddIn.vsto") + "|vstolocal", RegistryValueKind.String);
                tmpKey.SetValue("LoadBehavior", 3, RegistryValueKind.DWord);

                //if (Is64BitOutlookFromRegisteredExe())
                //     DirectoryCopy(Path.Combine(sDir, "64"), sDir, true);
                //else DirectoryCopy(Path.Combine(sDir, "32"), sDir, true);
            }
            catch (Exception e)
            {
                String sMsg = "";
                if (!IsElevated)
                    sMsg = Environment.NewLine + "Please run as administrator!";
                MessageBox.Show("Error registering: " + e.Message + sMsg);
                return false;
            }
            finally
            {
                //within DOS mode it needs a return to get back the prompt
                //when sent in setup-mode, install shield interrupts installation
                if (sendRet)
                    SendKeys.SendWait("{ENTER}");
                SafeNativeMethods.FreeConsole();
            }
            return true;
        }

        // <summary>
        //  copy files from subfolder to main folder
        // </summary>
        /*
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                try
                {
                    file.CopyTo(temppath, true);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error copying (" + e.Message + "): " + file.Name + " to " + temppath);
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }

        }
        */
        /// <summary>
        ///  delete the registry settings and disables outlook plugin
        /// </summary>
        private static bool unregisterPlugin()
        {
            try
            {
                RegistryKey tmpKey = getOutlookRootKey();
                tmpKey = tmpKey.OpenSubKey("Addins", true);

                if (tmpKey != null)
                    tmpKey.DeleteSubKey("Codeplex.BackupAddIn");
            }
            catch (Exception e)
            {
                String sMsg = "";
                if (!IsElevated)
                    sMsg = Environment.NewLine + "Please run as administrator!";

                MessageBox.Show("Error:" + e.Message + sMsg);
                return false;
            }
            return true;
        }


        /// <summary>
        ///  returnes the registry key for outlook depending on bit-ness
        /// </summary>
        private static RegistryKey getOutlookRootKey()
        {
            RegistryKey tmpKey;
            if (Environment.Is64BitOperatingSystem == false || Is64BitOutlookFromRegisteredExe() == false)
            {
                //Office32 on Win64 or Office32 on Win32
                Console.WriteLine(@"Detected office 32 Bit");
                tmpKey = Registry.LocalMachine;
            }
            else  //Office64 on Win64
            {
                //Program is run in 32 Bit-mode on Win64 --> access Registry via Win32-Hive
                Console.WriteLine(@"Detected office 64 Bit");
                tmpKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
            }

            Console.WriteLine(@"Fetching Software\Microsoft\Office\Outlook...");
            return tmpKey.OpenSubKey(@"Software\Microsoft\Office\Outlook", true);
        }
    }
}
