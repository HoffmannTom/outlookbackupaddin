using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupAddIn.Models
{
    public class LoadXML
    {
        /// <summary>
        /// config do scanner
        /// </summary>
        internal string block { get; private set; }
        public string ipapi { get; private set; }
        /// <summary>
        /// ip da impressora da zebra
        /// </summary>
        //public string ipZebraPrinter { get; private set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public int portZebraprinter { get; private set; }


        public bool LoadingXMLFILE()
        {

            string sSettingsZebraPrinterPath = AppDomain.CurrentDomain.BaseDirectory + @"\FILES\Settings.xml";

            // Dataset table
            string tbParameters = "Parameters";
            //string cIP = "IP";
            //string cPort = "Port";
            string cPortCom = "block";
            string ipapivarible = "ipapi";

            DataSet dsSettingsZebra = new DataSet();

            // Path to your XML configuration file
            if (File.Exists(sSettingsZebraPrinterPath))
            {
                try
                {
                    dsSettingsZebra.Clear();
                    dsSettingsZebra.ReadXml(sSettingsZebraPrinterPath, XmlReadMode.ReadSchema);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                // write program sequence in configuration file
                dsSettingsZebra.WriteXml(sSettingsZebraPrinterPath, XmlWriteMode.WriteSchema);
                return false;
            }

            if (dsSettingsZebra.Tables[tbParameters].Rows.Count == 0)
            {
                dsSettingsZebra.Tables[tbParameters].Rows.Add(dsSettingsZebra.Tables[tbParameters].NewRow());
                dsSettingsZebra.WriteXml(sSettingsZebraPrinterPath, XmlWriteMode.WriteSchema);
            }

            try
            {
                //ipZebraPrinter = dsSettingsZebra.Tables[tbParameters].Rows[0][cIP].ToString();
                //portZebraprinter = Convert.ToInt32(dsSettingsZebra.Tables[tbParameters].Rows[0][cPort].ToString());
                block = dsSettingsZebra.Tables[tbParameters].Rows[0][cPortCom].ToString();
                ipapi = dsSettingsZebra.Tables[tbParameters].Rows[0][ipapivarible].ToString();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}

