using BackupAddInCommon;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BackupExecutor
{
    /// <summary>
    ///  Utility class for backing up outlook files
    /// </summary>
    public class BackupTool
    {
        private static readonly ResourceManager rm;
        static BackupTool()
        {
            rm = new ResourceManager("BackupExecutor.lang.langres", typeof(BackupTool).Assembly);
        }
        private static System.Windows.Forms.ProgressBar pbCopyProgress;
        private static System.Windows.Forms.ProgressBar pbTotalCopyProgress;
        private static System.Windows.Forms.Label lblFilename;
        private static System.Windows.Forms.Label lblMegaBytesPerSecond;

        /// <summary>
        /// Flag, whether user can close main window
        /// </summary>
        public static bool CanExit = false;

        /// <summary>
        /// Log-delegate for sending error information
        /// </summary>
        public delegate void Logger(string message);

        //private static Logger logger;

        private const String OUTLOOK_PROC = "OUTLOOK";

        private static long TotalBytesToCopy;
        private static long TotalBytesCopied;
        private static long StartTimeCopy;
        private static readonly string LONG_PATH_INDICATOR = "\\\\?\\";

        /// <summary>
        /// Set a label to report currently copied file
        /// </summary>
        public static void SetFileLabel(System.Windows.Forms.Label lbl)
        {
            lblFilename = lbl;
        }

        /// <summary>
        /// Set a label to report transfer speed
        /// </summary>
        public static void SetMegaByesPerSecondLabel(System.Windows.Forms.Label lbl)
        {
            lblMegaBytesPerSecond = lbl;
        }

        /// <summary>
        /// Set a progress bar to report copy progress
        /// </summary>
        public static void SetProgressBar(System.Windows.Forms.ProgressBar pb)
        {
            pbCopyProgress = pb;
        }

        /// <summary>
        /// Set a progress bar to report status of whole copy progress
        /// </summary>
        public static void SetTotalProgressBar(System.Windows.Forms.ProgressBar pb)
        {
            pbTotalCopyProgress = pb;
        }



        /// <summary>
        /// Waits, till outlook ends and then starts the backup process
        /// </summary>
        /// <param name="config">Stored configuration from outlook plugin</param>
        /// <param name="log">logging delegate to send error information</param>
        /// <returns>number of occurred errors</returns>
        public static int TryBackup(BackupSettings config, Logger log)
        {
            int iError = 0;
            if (config == null)
            {
                log("backup not configured");
                return 0;
            }

            log("Starting backup...please wait...");

            try
            {
                if (config.Items.Count > 0)
                {
                    log("Check whether outlooks is still running ...");
                    if (WaitForProcessEnd(OUTLOOK_PROC, log))
                    {
                        log("No outlook process found");
                        iError += DoBackup(config, log);
                        if (!String.IsNullOrEmpty(config.PostBackupCmd))
                        {
                            int iRes = RunPostCmd(config.PostBackupCmd, config.ProfileName, log);
                            iError += iRes;
                        }
                    }
                    else
                    {
                        iError++;
                        log("Error waiting for " + OUTLOOK_PROC);
                    }

                }
                else
                {
                    iError++;
                    log("Error: no files for backup selected");
                }

                //if no errors occurred, save current timestamp
                if (iError == 0)
                {
                    config.LastRun = DateTime.Now;
                    BackupSettingsDao.SaveSettings(config);
                }
            }
            catch (InstanceAlreadyRunningException)
            {
                log("Backup already running...");
                iError++;
            }

            return iError;
        }

        /// <summary>
        /// Runs a program and waits for finish (after backup)
        /// </summary>
        /// <param name="cmd">Command to execute</param>
        /// <param name="param">command line parameters</param>
        /// <param name="log">logging delegate to send error information</param>
        /// <returns>number of occurred errors</returns>
        private static int RunPostCmd(string cmd, String param, Logger log)
        {
            log("Starting post-backup cmd: " + cmd);

            int iError = 0;
            Process p = new Process();
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8; //Encoding.GetEncoding(437)
            p.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            p.StartInfo.WorkingDirectory = "";
            p.StartInfo.FileName = cmd;
            p.StartInfo.Arguments = param;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;

            p.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
                {
                    log(e.Data);
                }
            );

            p.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                log(e.Data);
            }
            );

            try
            {
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                p.WaitForExit();
                int iExit = p.ExitCode;
                if (iExit > 0)
                {
                    /*
                    String sOut = p.StandardOutput.ReadToEnd();
                    String sErr = p.StandardError.ReadToEnd();
                    if (!String.IsNullOrEmpty(sOut))
                        log("Script output: " + sOut);
                    if (!String.IsNullOrEmpty(sErr))
                        log("Script error: " + sOut);
                    */
                    log("Process exited with code " + iExit);
                }
                return iExit;
            }
            catch (Exception e)
            {
                iError++;
                log("Error executing " + cmd + ": " + e.Message);
            }

            return iError;
        }

        /// <summary>
        /// Tries to backup the configures files from outlook
        /// </summary>
        /// <param name="config">Stored configuration from outlook plugin</param>
        /// <param name="log">logging delegate to send error information</param>
        /// <returns>number of occurred errors</returns>
        private static int DoBackup(BackupSettings config, Logger log)
        {
            int iError = 0;

            log("Ensure only one instance running ...");
            using (new SingleInstance(500))
            {
                //logger = log;
                String sPath = config.DestinationPath;

                //Expand environment variables
                sPath = Environment.ExpandEnvironmentVariables(sPath);

                if (!sPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    sPath += Path.DirectorySeparatorChar;

                //Create target directory if not exists
                log("Check whether target directory exists...");
                if (!Directory.Exists(sPath))
                {
                    try
                    {
                        Directory.CreateDirectory(sPath);
                    }
                    catch (Exception e)
                    {
                        log("Can't create directory " + sPath + ": " + e.Message);
                        return 1;
                    }
                }

                String sDst;
                int iCounter = 0;

                //Gather all file sizes
                log("Summing up file sizes ...");
                TotalBytesToCopy = 0;
                long[] FileSizes = new long[config.Items.Count];
                foreach (String item in config.Items)
                {
                    log("Adding size of " + item);
                    //FileSizes[iCounter] = (new System.IO.FileInfo(item)).Length;
                    FileSizes[iCounter] = GetFileLength(item, log);
                    TotalBytesToCopy += FileSizes[iCounter];
                    iCounter++;
                }
                log("Total bytes calculated...");

                //Copy files
                TotalBytesCopied = 0;
                iCounter = 0;
                foreach (String item in config.Items)
                {
                    iCounter++;
                    try
                    {
                        lblFilename?.Invoke(new Action(() => lblFilename.Text = "File " + iCounter + "/" + config.Items.Count + ": " + item));

                        log("Evaluate destination path...");
                        sDst = sPath + Environment.ExpandEnvironmentVariables(config.BackupPrefix) + Path.GetFileName(item);
                        if (config.UseCompression)
                            sDst += ".gz";
                        sDst += Environment.ExpandEnvironmentVariables(config.BackupSuffix);

                        if (item.Equals(sDst) || item.Equals(LONG_PATH_INDICATOR + sDst))
                        {
                            log("Can't copy file on it's own, skipping: " + item);
                            iError++;
                        }
                        else
                        {
                            //src and dest are different, lets backup
                            log("copy " + item + " to " + sDst);
                            log("Getting file lock...");
                            if (WaitForFile(item, log, config.WaitTimeFileLock))
                            {
                                //log("file lock was successful");
                                bool bOK = true;
                                if (config.UseCompression)
                                    bOK = CopyAndCompressFileForBackup(sDst, item, log);
                                else
                                    bOK = CopyFileForBackup(config, sDst, item, log);

                                if (!bOK)
                                    iError++;
                            }
                            else
                            {
                                log("Skipping file " + item + " because it is locked");
                                iError++;
                            }
                        }

                        TotalBytesCopied += FileSizes[iCounter - 1];
                    }
                    catch (Exception e)
                    {
                        iError++;
                        log(e.Message);
                    }
                } //for each
            } //using

            return iError;
        }

        private static bool CopyAndCompressFileForBackup(String sDst, String item, Logger log)
        {
            try
            {
                FileInfo fi = new FileInfo(item);

                // Get the stream of the source file.
                using (FileStream inFile = fi.OpenRead())
                using (FileStream outFile = File.Create(sDst))
                {
                    // Create the compressed file.
                    GZipStream Compress = new GZipStream(outFile, CompressionMode.Compress);
                    byte[] buffer = new byte[32 * 1024];
                    int read;
                    int readTotal = 0;
                    while ((read = inFile.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        Compress.Write(buffer, 0, read);
                        readTotal += read;
                        UpdateProgressIndicators(readTotal, fi.Length);
                    };

                    Compress.Close();
                }
            }
            catch (System.Exception e)
            {
                log("Failed to copy file: " + e.Message);
                return false;
            }
            return true;
        }

        private static bool CopyFileForBackup(BackupSettings config, String sDst, String item, Logger log)
        {
            SafeNativeMethods.CopyFileFlags dwCopyFlags = 0;
            if (config.IgnoreEncryption)
                dwCopyFlags = SafeNativeMethods.CopyFileFlags.COPY_FILE_ALLOW_DECRYPTED_DESTINATION;

            bool pbCancel = false;
            SafeNativeMethods.CopyProgressRoutine cb = new SafeNativeMethods.CopyProgressRoutine(Callback);

            StartTimeCopy = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            //CopyFileEx
            //https://msdn.microsoft.com/en-us/library/windows/desktop/aa363852%28v=vs.85%29.aspx
            log("Starting copying...");
            bool bOK = SafeNativeMethods.CopyFileEx(item, sDst, cb, IntPtr.Zero, ref pbCancel, dwCopyFlags);
            if (!bOK)
                log("Failed to copy file: " + new Win32Exception(Marshal.GetLastWin32Error()).Message);
            return bOK;
        }

        private static SafeNativeMethods.CopyProgressResult Callback(long TotalFileSize,
            long TotalFileBytesTransferred, long StreamSize,
            long StreamBytesTransferred, uint dwStreamNumber,
            SafeNativeMethods.CopyProgressCallbackReason dwCallbackReason,
            IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData)
        {
            if (dwCallbackReason == SafeNativeMethods.CopyProgressCallbackReason.CALLBACK_CHUNK_FINISHED)
                UpdateProgressIndicators(TotalFileBytesTransferred, TotalFileSize);

            return SafeNativeMethods.CopyProgressResult.PROGRESS_CONTINUE;
        }

        private static void UpdateProgressIndicators(long TotalFileBytesTransferred, long FileSize)
        {
            long i = (TotalFileBytesTransferred * 100L) / FileSize;
            if (pbCopyProgress != null)
            {
                if (pbCopyProgress.Minimum <= i && i <= pbCopyProgress.Maximum)
                    pbCopyProgress.Invoke(new Action(() => pbCopyProgress.Value = (int)i));
                else
                {
                    String s = "Error in reporting progress" + System.Environment.NewLine;
                    s += "TotalFileBytesTransferred:" + TotalFileBytesTransferred + System.Environment.NewLine;
                    s += "FileSize:" + FileSize;
                    throw new System.Exception(s);
                }
            }

            int iTotal = (int)((TotalBytesCopied + TotalFileBytesTransferred) * 100 / TotalBytesToCopy);
            if (pbTotalCopyProgress != null)
            {
                if (pbTotalCopyProgress.Minimum <= iTotal && iTotal <= pbTotalCopyProgress.Maximum)
                    pbTotalCopyProgress.Invoke(new Action(() => pbTotalCopyProgress.Value = iTotal));
                else
                {
                    String s = "Error in reporting total progress" + System.Environment.NewLine;
                    s += "TotalBytesCopied:" + TotalBytesCopied + System.Environment.NewLine;
                    s += "TotalFileBytesTransferred:" + TotalFileBytesTransferred + System.Environment.NewLine;
                    s += "TotalBytesToCopy:" + TotalBytesToCopy;
                    throw new System.Exception(s);
                }
            }

            double TimeElapsedSec = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - StartTimeCopy) / 1000.0;
            if (TimeElapsedSec > 0.1 && lblMegaBytesPerSecond != null && TotalFileBytesTransferred > 0)
            {
                int iMbPerSec = (int)(TotalFileBytesTransferred / 1024 / 1024 / TimeElapsedSec);
                lblMegaBytesPerSecond.Invoke(new Action(() => lblMegaBytesPerSecond.Text = iMbPerSec + " MiB/s"));
            }
        }

        /// <summary>
        /// Wait max. 5 seconds for releasing locks on the file
        /// </summary>
        /// <param name="item">filename including path</param>
        /// <param name="log">logging delegate to send error information</param>
        /// <param name="waittime">wait time if file is locked</param>
        /// <returns></returns>
        private static bool WaitForFile(string item, Logger log, int waittime = 500)
        {
            int i = 0;
            while (i < 10 && IsFileLocked(item, log))
            {
                //log("waiting before starting new try");
                Thread.Sleep(waittime);
                i++;
            }

            return (i < 10);
        }


        /// <summary>
        /// Waits, till the process is not running any more
        /// </summary>
        /// <param name="name">process name (!) within task manager</param>
        /// <param name="log">logging delegate to send error information</param>
        /// <returns>returns true if the process is not running any more</returns>
        public static bool WaitForProcessEnd(string name, Logger log)
        {
            int i = 0;
            while (i < 10 && IsProcessOpen(name, log))
            {
                log("Waiting for " + name + "...");
                Application.DoEvents();
                System.Threading.Thread.Sleep(1000);
                i++;
            }

            return !IsProcessOpen(name, log);
        }

        /// <summary>
        /// Checks whether a certain process is running
        /// </summary>
        /// <param name="name">process name as seen witin the task  manager</param>
        /// <param name="log">logging delegate to send error information</param>
        /// <returns>true, if process is still running</returns>
        public static bool IsProcessOpen(string name, Logger log)
        {
            String LoggedOnUser = Environment.UserName;
            foreach (Process clsProcess in Process.GetProcessesByName(name))
            {
                try
                {
                    String ProcUser = GetProcessUser(clsProcess);
                    //txtLog.Text += clsProcess.ProcessName + Environment.NewLine;// MainModule.ModuleName ;

                    //for multiple users logged on, just check the own processes
                    if (clsProcess.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase)
                        && LoggedOnUser.Equals(ProcUser, StringComparison.OrdinalIgnoreCase))
                    {
                        log("Found outlook with PID " + clsProcess.Id);
                        return true;
                    }
                }
                catch (Exception)
                {
                    //some processes are dont like to be inspected... just go on...
                }
            }
            return false;
        }

        /// <summary>
        /// Get the username of the process
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private static string GetProcessUser(Process process)
        {
            IntPtr processHandle = IntPtr.Zero;
            try
            {
                SafeNativeMethods.OpenProcessToken(process.Handle, 8, out processHandle);
                WindowsIdentity wi = new WindowsIdentity(processHandle);
                string user = wi.Name;
                return user.Contains(@"\") ? user.Substring(user.IndexOf(@"\") + 1) : user;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (processHandle != IntPtr.Zero)
                {
                    SafeNativeMethods.CloseHandle(processHandle);
                }
            }
        }

        /// <summary>
        /// Check whether a file is locked 
        /// </summary>
        /// <param name="file">file to be checked</param>
        /// <param name="log">logging delegate to send error information</param>
        /// <returns></returns>
        protected static bool IsFileLocked(String file, Logger log)
        {
            if (!FileExists(file))
                return false;

            SafeFileHandle fileHandle = null;

            try
            {
                //log("Trying to acquire file lock via CreateFile API");
                fileHandle = SafeNativeMethods.CreateFile(file,
                                        SafeNativeMethods.EFileAccess.GenericRead, //GenericAll, 
                                        SafeNativeMethods.EFileShare.None, IntPtr.Zero,
                                        SafeNativeMethods.ECreationDisposition.OpenExisting, 0, IntPtr.Zero);
                //log("Finished trying to acquire file lock via CreateFile API");
                int lastWin32Error = Marshal.GetLastWin32Error();
                if (fileHandle.IsInvalid)
                {
                    //log("Acquire file lock via CreateFile API failed");
                    throw new IOException(lastWin32Error.ToString());
                }
            }
            catch (IOException e)
            {

                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist 
                log("File is locked: " + file + " Err-No: " + e.Message);

                int errCode;
                if (Int32.TryParse(e.Message, out errCode))
                {
                    string errorMessage = new Win32Exception(errCode).Message;
                    log(errorMessage);
                }
                return true;
            }
            finally
            {
                if (fileHandle != null)
                {
                    //log("Closing file handle");
                    //get sure to close all handles (also own one)
                    if (!fileHandle.IsInvalid)
                        fileHandle.Close();
                    //log("Running GC");
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    //log("Finished GC");
                }
            }

            //file is not locked
            return false;
        }

        /// <summary>
        /// Get lenght of file in bytes
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static long GetFileLength(string path, Logger log)
        {
            //in order to support long path syntax, native methods are used
            try
            {
                SafeNativeMethods.WIN32_FILE_ATTRIBUTE_DATA fileData;
                if (!SafeNativeMethods.GetFileAttributesEx(path,
                        SafeNativeMethods.GET_FILEEX_INFO_LEVELS.GetFileExInfoStandard, out fileData))
                {
                    log("Error retrieving file size from: -" + path + "-");
                    return -1;
                }
                return (long)(((ulong)fileData.nFileSizeHigh << 32) + (ulong)fileData.nFileSizeLow);
            }
            catch (Exception e)
            {
                log("Error occured while retrieving file size from: -" + path + "-" + e.Message);
                return -1;
            }
        }

        /// <summary>
        /// Check whether file exists
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool FileExists(string file)
        {
            FileAttributes fa = SafeNativeMethods.GetFileAttributes(file);
            if (fa > 0)
            {
                return !(fa.HasFlag(FileAttributes.Directory));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// shutdown computer
        /// </summary>
        /// <returns></returns>
        public static void ShutdownComputer()
        {
            ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
            mcWin32.Get();
            // You can't shutdown without security privileges 
            mcWin32.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject mboShutdownParams = mcWin32.GetMethodParameters("Win32Shutdown");
            // Flag 5 means force
            mboShutdownParams["Flags"] = "5";
            mboShutdownParams["Reserved"] = "0";
            foreach (ManagementObject manObj in mcWin32.GetInstances().Cast<ManagementObject>())
            {
                _ = manObj.InvokeMethod("Win32Shutdown", mboShutdownParams, null);
            }
        }
    }
}
