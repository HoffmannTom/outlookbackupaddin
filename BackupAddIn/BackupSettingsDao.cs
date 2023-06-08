﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BackupAddInCommon
{
    class BackupSettingsDao
    {
        private const String CONFIG_FILE_NAME = "OutlookBackup.config";
        private const String REG_PATH_SETTINGS = @"Software\CodePlex\BackupAddIn\Settings";

        /// <summary>
        /// Determine config-file location
        /// </summary>
        /// <returns>Location of the config file</returns>
        public static String GetConfigFilePath()
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
        /// Saves the current settings to registry or file
        /// </summary>
        /// /// <param name="config">Configration to save</param>
        /// <returns>true, if save action was successful</returns>
        public static bool SaveSettings(BackupSettings config)
        {
            return SaveSettingsToRegistry(config);
            //return saveSettingsToFile(config);
        }

        /// <summary>
        /// Saves the current settings to registry
        /// </summary>
        /// /// <param name="config">Configration to save</param>
        /// <returns>true, if save action was successful</returns>
        private static bool SaveSettingsToRegistry(BackupSettings config)
        {
            bool bRet = true;
            try
            {
                Type type = config.GetType();
                PropertyInfo[] properties = type.GetProperties();
                RegistryKey appKey = Registry.CurrentUser.CreateSubKey(REG_PATH_SETTINGS);

                //iterate all properties of config object
                foreach (PropertyInfo property in properties)
                {
                    bRet &= SavePropertyToRegistry(config, appKey, property);
                }

                appKey.Close();
            }
            catch (System.Exception e)
            {
                MessageBox.Show("Error during saving settings to registry " + e.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return bRet;
        }

        private static bool SavePropertyToRegistry(BackupSettings config, RegistryKey appKey, PropertyInfo property)
        {
            try
            {
                Object val = "";
                RegistryValueKind t = RegistryValueKind.Unknown;

                if (typeof(String).IsAssignableFrom(property.PropertyType))
                {
                    t = RegistryValueKind.String;
                    val = property.GetValue(config, null) as String;
                }
                else if (typeof(DateTime).IsAssignableFrom(property.PropertyType))
                {
                    //office culture may not be system culture
                    var ci = CultureInfo.InstalledUICulture;
                    t = RegistryValueKind.String;
                    DateTime? dt = property.GetValue(config, null) as DateTime?;
                    if (dt.HasValue)
                        val = dt.Value.ToString(ci);
                    else val = null;
                }
                else if (typeof(int).IsAssignableFrom(property.PropertyType))
                {
                    t = RegistryValueKind.DWord;
                    val = (property.GetValue(config, null) as int?).ToString();
                }
                else if (typeof(bool).IsAssignableFrom(property.PropertyType))
                {
                    t = RegistryValueKind.String;
                    val = (property.GetValue(config, null) as bool?).ToString();
                }
                else if (typeof(StringCollection).IsAssignableFrom(property.PropertyType))
                {
                    t = RegistryValueKind.MultiString;
                    StringCollection col = property.GetValue(config, null) as StringCollection;
                    val = col.Cast<String>().ToArray<String>();
                }
                else if (typeof(List<int>).IsAssignableFrom(property.PropertyType))
                {
                    t = RegistryValueKind.String;
                    List<int> l = (property.GetValue(config, null) as List<int>);
                    if (l != null)
                        val = String.Join(",", l.Select(p => p.ToString()).ToArray());
                    else val = "";
                }

                if (t != RegistryValueKind.Unknown)
                    if (val != null)
                         appKey.SetValue(property.Name, val, t);
                    else appKey.DeleteValue(property.Name, false);
            }
            catch (System.Exception e)
            {
                MessageBox.Show("Error during saving property " + property.Name + " to registry " + e.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Saves the current settings to disk
        /// </summary>
        /// /// <param name="config">Configration to save</param>
        /// <returns>true, if save action was successful</returns>
        /*
        private static bool SaveSettingsToFile(BackupSettings config)
        {
            String sFile = GetConfigFilePath();
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
            catch (System.Exception e)
            {
                MessageBox.Show("Error during saving settings to file " + sFile + ": " + e.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        */

        /// <summary>
        /// Returns the saved settings or null if not present
        /// </summary>
        /// <returns>Returns the saved settings from disk</returns>
        public static BackupSettings LoadSettings()
        {
            return LoadSettingsFromRegistry();
            //return loadSettingsFromFile();
        }

        /// <summary>
        /// Returns the saved settings from registry or null if not present
        /// </summary>
        /// <returns>Returns the saved settings from disk</returns>
        private static BackupSettings LoadSettingsFromRegistry()
        {
            BackupSettings config = new BackupSettings();
            RegistryKey appKey = Registry.CurrentUser.OpenSubKey(REG_PATH_SETTINGS, false);
            if (appKey != null)
            {
                //config = new BackupSettings();
                String[] names = appKey.GetValueNames();

                //iterate registry entries
                foreach (String name in names)
                {
                    TransferRegistryEntryToConfig(config, appKey, name);
                }
                appKey.Close();
            }

            //for (int i = 0; i < config.Items.Count; i++)
            //   config.Items[i] = Path.(config.Items[i]);

            return config;
        }

        private static void TransferRegistryEntryToConfig(BackupSettings config, RegistryKey appKey, String name)
        {
            RegistryValueKind typ;
            PropertyInfo pi;

            try
            {
                //checked whether property exists
                pi = config.GetType().GetProperty(name);
                if (pi != null)
                {
                    typ = appKey.GetValueKind(name);
                    if (typeof(String).IsAssignableFrom(pi.PropertyType))
                        pi.SetValue(config, appKey.GetValue(name) as String, null);
                    else if (typeof(int).IsAssignableFrom(pi.PropertyType))
                        pi.SetValue(config, appKey.GetValue(name) as int?, null);
                    else if (typeof(bool).IsAssignableFrom(pi.PropertyType))
                        pi.SetValue(config, (appKey.GetValue(name) as String).Equals(bool.TrueString), null);
                    else if (typeof(DateTime).IsAssignableFrom(pi.PropertyType))
                    {
                        //office culture may not be system culture
                        var ci = CultureInfo.InstalledUICulture;
                        pi.SetValue(config, DateTime.Parse(appKey.GetValue(name) as String, ci), null);
                    }
                    else if (typeof(StringCollection).IsAssignableFrom(pi.PropertyType))
                    {
                        String[] sArr = appKey.GetValue(name) as String[];
                        StringCollection sc = new StringCollection();
                        sc.AddRange(sArr);
                        pi.SetValue(config, sc, null);
                    }
                    else if (typeof(List<int>).IsAssignableFrom(pi.PropertyType))
                    {
                        String s = appKey.GetValue(name) as String;
                        List<int> l = new List<int>();
                        char[] sep = new char[] { ',' };
                        s.Split(sep, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => l.Add(Convert.ToInt32(x)));
                        pi.SetValue(config, l, null);
                    }
                }
            }
            catch (System.Exception e)
            {
                MessageBox.Show("Error during fetching settings " + name + ": " + e.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Returns the saved settings from xml file or null if not present
        /// </summary>
        /// <returns>Returns the saved settings from disk</returns>
        /// 
        /*
        private static BackupSettings LoadSettingsFromFile()
        {
            String sFile = GetConfigFilePath();
            BackupSettings config = null;

            if (File.Exists(sFile))
            {
                try
                {
                    using (Stream stream = File.Open(sFile, FileMode.Open))
                    {
                        XmlSerializer bin = new XmlSerializer(typeof(BackupSettings));
                        config = (BackupSettings)bin.Deserialize(stream);
                    }
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Error during reading settings from file " + sFile,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return config;
        }
        */
    }
}
