﻿using BackupAddInCommon;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace BackupExecutor
{
    public enum BinaryType : uint
    {
        SCS_32BIT_BINARY = 0,
        SCS_64BIT_BINARY = 6
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
            if (!ParseArgs(args, argsDict))
            {
                ShowHelp(args);
                return 0;
            }

            if (argsDict.ContainsKey("/register"))
            {
                RetCode = RegisterPlugin(true) ? 0 : 1;
            }
            else if (argsDict.ContainsKey("/registersetup"))
            {
                RetCode = RegisterPlugin(false) ? 0 : 1;
            }
            else if (argsDict.ContainsKey("/unregister"))
            {
                RetCode = UnregisterPlugin() ? 0 : 1;
            }
            else if (argsDict.ContainsKey("/backupnow"))
            {
                int iError = 0;
                SafeNativeMethods.AttachConsole(SafeNativeMethods.ATTACH_PARENT_PROCESS);
                try
                {
                    LogToConsole("Reading settings ...");
                    BackupSettings config = BackupSettingsDao.LoadSettings();
                    LogToConsole("Reading settings finished");
                    iError = BackupTool.TryBackup(config, LogToConsole);
                }
                catch (Exception e)
                {
                    LogToConsole(e.Message);
                    LogToConsole(e.StackTrace);
                }

                SendKeys.SendWait("{ENTER}");
                SafeNativeMethods.FreeConsole();
                RetCode = iError;
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmMain());
            }

            return RetCode;
        }

        private static bool ParseArgs(string[] args, Dictionary<string, string> argsDict)
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
            s = DateTime.Now.TimeOfDay.ToString("hh\\:mm\\:ss") + " " + s;
            Console.WriteLine(s + Environment.NewLine);
        }

        /// <summary>
        ///  print valid parameters and help information to console
        /// </summary>
        private static void ShowHelp(string[] args)
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
                outlookPath = GetOutlookPath();

            if (String.IsNullOrEmpty(outlookPath))
                throw new Exception("Outlook not found!");

            if (Environment.Is64BitOperatingSystem)
            {
                try
                {
                    //if (SafeNativeMethods.GetBinaryType(outlookPath, out binaryType))
                    //    bRet = (binaryType == SafeNativeMethods.SCS_64BIT_BINARY);
                    //else MessageBox.Show("Error: " + Marshal.GetLastWin32Error());
                    if (GetBinaryType(outlookPath) == BinaryType.SCS_64BIT_BINARY)
                        bRet = true;

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
        ///  Get binary type of a file by reading PE Header
        /// </summary>
        public static BinaryType? GetBinaryType(string path)
        {
            // thanks to
            // https://stackoverflow.com/questions/44337501/get-type-of-binary-on-filesystem-via-c-sharp-running-in-64-bit
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                stream.Seek(0x3C, SeekOrigin.Begin);
                using (var reader = new BinaryReader(stream))
                {
                    if (stream.Position + sizeof(int) > stream.Length)
                        return null;
                    var peOffset = reader.ReadInt32();
                    stream.Seek(peOffset, SeekOrigin.Begin);
                    if (stream.Position + sizeof(uint) > stream.Length)
                        return null;
                    var peHead = reader.ReadUInt32();
                    if (peHead != 0x00004550) // "PE\0\0"
                        return null;
                    if (stream.Position + sizeof(ushort) > stream.Length)
                        return null;
                    switch (reader.ReadUInt16())
                    {
                        case 0x14c:
                            return BinaryType.SCS_32BIT_BINARY;
                        case 0x8664:
                            return BinaryType.SCS_64BIT_BINARY;
                        default:
                            return null;
                    }
                }
            }
        }

        /// <summary>
        ///  Try to evaluate the outlook path. If it fails, choose manually
        /// </summary>
        private static string GetOutlookPath()
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
                    RegistryKey tmpKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
                    tmpKey = tmpKey.OpenSubKey(@"Software\Classes\CLSID\" + clsid + @"\LocalServer32");
                    if (tmpKey != null)
                        sPath = (string)tmpKey.GetValue("");
                }
            }

            if (String.IsNullOrEmpty(sPath))
            {
                System.Windows.Forms.OpenFileDialog odlg = new System.Windows.Forms.OpenFileDialog
                {
                    DefaultExt = ".exe",
                    CheckFileExists = true,
                    Filter = "Outlook.exe|outlook.exe",
                    Title = "Please select outlook.exe from the installation folder"
                };
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
        private static bool RegisterPlugin(bool sendRet)
        {
            SafeNativeMethods.AttachConsole(SafeNativeMethods.ATTACH_PARENT_PROCESS);
            try
            {
                RegistryKey tmpKey = GetOutlookRootKey();

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

        /// <summary>
        ///  delete the registry settings and disables outlook plugin
        /// </summary>
        private static bool UnregisterPlugin()
        {
            try
            {
                RegistryKey tmpKey = GetOutlookRootKey();
                tmpKey = tmpKey.OpenSubKey("Addins", true);

                tmpKey?.DeleteSubKey("Codeplex.BackupAddIn");
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
        private static RegistryKey GetOutlookRootKey()
        {
            RegistryKey tmpKey;
            if (!Environment.Is64BitOperatingSystem || !Is64BitOutlookFromRegisteredExe())
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
