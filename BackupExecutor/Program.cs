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
    }

    static class Program
    {

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

            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
            else if (args[0] == "/register")
            {
                RetCode = registerPlugin() ? 0 : 1;
            }
            else if (args[0] == "/unregister")
            {
                RetCode = unregisterPlugin() ? 0 : 1;
            }
            else if (args[0] == "/backupnow")
            {
                SafeNativeMethods.AttachConsole(SafeNativeMethods.ATTACH_PARENT_PROCESS);

                BackupSettings config = BackupTool.getSettings();
                int iError = BackupTool.tryBackup(config, LogToConsole);

                SafeNativeMethods.FreeConsole();
                RetCode = iError;
            }
            else showHelp(args);

            return RetCode;
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
            String outlookPath;
            uint binaryType;

            bool bRet = false; 
            // Default value - assume 32-bit unless proven otherwise.
            // RegQueryStringValue second param is '' to get the (default) value for the key
            // with no sub-key name, as described at
            // http://stackoverflow.com/questions/913938/

            //better: http://support.microsoft.com/kb/240794
            String clsid = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Classes\Outlook.Application\CLSID", "", null);

            if (clsid == null)
                throw new Exception("CLSID of outlook.application not found in registry!");

            //outlookPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE", "", null);
            //--> Sometimes the app path is only stored on wow6432node 

            outlookPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Classes\CLSID\" + clsid + @"\LocalServer32", "", null);

            if (outlookPath == null)
                throw new Exception("No installed outlook found!");

            if (Environment.Is64BitOperatingSystem)
            {
                try
                {
                    if (SafeNativeMethods.GetBinaryType(outlookPath, out binaryType))
                        bRet = (binaryType == SafeNativeMethods.SCS_64BIT_BINARY);
                }
                catch (Exception e)
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

        /* //depends on office version!
        private static bool isOffice64()
        {
            try
            {
                RegistryKey tmpKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", false);
                tmpKey = tmpKey.OpenSubKey("Microsoft");
                tmpKey = tmpKey.OpenSubKey("Office");
                if (tmpKey.OpenSubKey("15.0") != null)
                    tmpKey = tmpKey.OpenSubKey("15.0");
                else if (tmpKey.OpenSubKey("14.0") != null)
                    tmpKey = tmpKey.OpenSubKey("14.0");
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error when checking office version:" + e.Message);
                return false;
            }

            return false;

        }
         */

        /// <summary>
        ///  Create registry settings for outlook plugin and copy files to installation folder
        ///  depending on bit-ness
        /// </summary>
        private static bool registerPlugin()
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

                if (Is64BitOutlookFromRegisteredExe())
                    copySubfolder(sDir, "64");
                else copySubfolder(sDir, "32");
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
                SendKeys.SendWait("{ENTER}");
                SafeNativeMethods.FreeConsole();
            }
            return true;
        }

        /// <summary>
        ///  copy files from subfolder to main folder
        /// </summary>
        private static void copySubfolder(string sBase, string sSub)
        {
            Directory.GetFiles(sBase + sSub).ToList().ForEach(
                f => File.Copy(f, sBase + Path.GetFileName(f), true)
                );
        }

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
