using BackupAddInCommon;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BackupAddIn
{
    class BackupUtils
    {
        private const String CONFIG_FILE_NAME = "OutlookBackup.config";

        /// <summary>
        /// Determine config-file location
        /// </summary>
        /// <returns>Location of the config file</returns>
        public static String getConfigFilePath()
        {
            String sPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            object[] attributes = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

            if (attributes.Length != 0)
                sPath += Path.DirectorySeparatorChar + ((AssemblyCompanyAttribute)attributes[0]).Company;

            attributes = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (attributes.Length != 0)
                sPath += Path.DirectorySeparatorChar + ((AssemblyProductAttribute)attributes[0]).Product;

            return sPath + Path.DirectorySeparatorChar + CONFIG_FILE_NAME;
        }

        /// <summary>
        /// Saves the current settings to disk
        /// </summary>
        /// /// <param name="config">Configration to save</param>
        /// <returns>true, if save action was successful</returns>
        public static bool saveSettingsToFile(BackupSettings config)
        {
            String sFile = getConfigFilePath();
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(sFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(sFile));

                using (Stream stream = File.Open(sFile, FileMode.Create))
                {
                    //BinaryFormatter bin = new BinaryFormatter();
                    XmlSerializer bin = new XmlSerializer(typeof(BackupSettings));
                    bin.Serialize(stream, config);
                }
            }
            catch (System.Exception)
            {
                MessageBox.Show("Error during saving settings to file " + sFile,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }


        public static List<String> GetStoreLocations(BackupSettings config, Stores st)
        {
            var list = new List<string>();

            for (int i = 1; i <= st.Count; i++)
            {
                try
                {
                    //Ignore http- and imap-stores
                    if (st[i].FilePath != null)
                        list.Add(st[i].FilePath);
                    else if (st[i].ExchangeStoreType == OlExchangeStoreType.olNotExchange)
                    {
                        //Ugly solution...not supported
                        if (config != null && config.ShowOSTFiles)
                        {
                            String sPath = ParsePathFromStoreID(st[i]);
                            if (!String.IsNullOrEmpty(sPath))
                                list.Add(sPath);
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    //FilePath might be corrupt, check accounts -> files
                    MessageBox.Show("Error when iterating stores(" + i + "): " + ex.Message);
                }
            }

            return list;
        }

        /// <summary>
        /// Tries to extract the filepath from a store-id
        /// </summary>
        /// <param name="store">outlook file store</param>
        public static string ParsePathFromStoreID(Store store)
        {
            //hidden mapi properties
            //http://www.slipstick.com/developer/read-mapi-properties-exposed-outlooks-object-model/
            //http://stackoverflow.com/questions/24552747/outlook-profile-pst-and-ost-file-location-using-mapi-in-delphi
            //MSDN: http://msdn.microsoft.com/en-us/library/gg158290(v=winembedded.70).aspx
            //http://msdn.microsoft.com/en-us/library/ee203516%28v=exchg.80%29.aspx
            //VBA-Decode: http://www.pcreview.co.uk/forums/get-pst-file-path-string-t2965078.html
            //http://msdn.microsoft.com/en-us/library/office/cc765630(v=office.15).aspx

            //decode PR_STORE_ENTRYID
            int SkipBytes = 58;
            int TerminatingBytes = 2;
            byte[] b = store.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x0FFB0102");

            string s = "";
            if (b.Length > SkipBytes + TerminatingBytes)
            {
                //Last Bytes are for null terminating
                //byte[] b2 = new ArraySegment<byte>(b, SkipBytes, b.Length - SkipBytes - TerminatingBytes).ToArray<byte>();
                byte[] b2 = new byte[b.Length - SkipBytes - TerminatingBytes];
                Array.Copy(b, SkipBytes, b2, 0, b2.Length);
                s = System.Text.UnicodeEncoding.Unicode.GetString(b2);
            }

            return s;
        }
    }
}
