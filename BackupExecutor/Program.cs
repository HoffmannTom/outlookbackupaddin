using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackupExecutor
{
    static class Program
    {
        static int SCS_64BIT_BINARY = 6;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
            else if (args[0] == "/register")
            {
                registerPlugin();
            }
            else if (args[0] == "/unregister")
            {
                unregisterPlugin();
            }
        }

        [DllImport("kernel32.dll")]
        static extern bool GetBinaryType(string lpApplicationName, out uint lpBinaryType);

        private static bool Is64BitOutlookFromRegisteredExe()
        {
            String outlookPath;
            uint binaryType;

            bool bRet = false; // Default value - assume 32-bit unless proven otherwise.
            // RegQueryStringValue second param is '' to get the (default) value for the key
            // with no sub-key name, as described at
            // http://stackoverflow.com/questions/913938/
            outlookPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE", "", null);

            if (outlookPath == null)
                throw new Exception("No installed outlook found!");

            if (Environment.Is64BitOperatingSystem)
            {
                try
                {
                  if (GetBinaryType(outlookPath, out binaryType))
                     bRet = (binaryType == SCS_64BIT_BINARY);
                }
                catch (Exception)
                {
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

        private static bool registerPlugin()
        {
            try
            {
                RegistryKey tmpKey = getOutlookRootKey(); 

                tmpKey = tmpKey.CreateSubKey("Addins");
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
                MessageBox.Show("Error registering: " + e.Message);
                return false;
            }
            return true;
        }

        private static void copySubfolder(string sBase, string sSub)
        {
            Directory.GetFiles(sBase + sSub).ToList().ForEach(
                f => File.Copy(f, sBase + Path.GetFileName(f), true)
                );
        }

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
                MessageBox.Show("Error:" + e.Message);
                return false;
            }
            return true;
        }

        private static RegistryKey getOutlookRootKey()
        {
            RegistryKey tmpKey;
            if (Environment.Is64BitOperatingSystem == false || Is64BitOutlookFromRegisteredExe() == false)
            {
                //Office32 on Win64 or Office32 on Win32
                tmpKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", false);
            }
            else  //Office64 on Win64
            {
                tmpKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
            }

            return tmpKey.OpenSubKey(@"Software\Microsoft\Office\Outlook", true);
        }
    }
}
