using BackupAddInCommon;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace BackupAddIn
{
    public partial class ThisAddIn
    {
        private Ribbon Rib;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            ((Outlook.ApplicationEvents_11_Event)Application).Quit
            += new Outlook.ApplicationEvents_11_QuitEventHandler(ThisAddIn_Quit);

            //set language to office language
            //http://blog.sebastianbrand.com/2010/03/vsto-using-net-multi-language-support.html
            Thread.CurrentThread.CurrentUICulture =
            CultureInfo.GetCultureInfo(this.Application.LanguageSettings.get_LanguageID(Microsoft.Office.Core.MsoAppLanguageID.msoLanguageIDUI));

            BackupSettings config = BackupSettingsDao.LoadSettings();
            Rib.SetAccessSettingsAllowed(config.AllowSettingsAccess);
        }

        /// <summary>
        /// Accessor for ribbon gui element
        /// </summary>
        public Ribbon GetRibbon()
        {
            return Rib;
        }

        /// <summary>
        /// Run backup program if neccessary
        /// </summary>
        void ThisAddIn_Quit()
        {
            try
            {
                BackupSettings config = BackupSettingsDao.LoadSettings();

                //if configuration was done and backup program was configured
                if (config != null && !String.IsNullOrEmpty(config.BackupProgram))
                {
                    //and if not yet backuped or if backup too old, then run program
                    if (config.LastRun == null
                        || config.LastRun.AddDays(config.Interval).AddHours(config.IntervalHours) <= DateTime.Now)
                    {
                        bool configChanged = false;
                        if (config.BackupAll)
                        {
                            //If alles pdf-files should be saved, enumerate them and save to config-file
                            //Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
                            config.Items.Clear();

                            var list = BackupUtils.GetStoreLocations(config, Application.Session.Stores);
                            config.Items.AddRange(list.ToArray());

                            configChanged = true;
                        }


                        if (config.ProfileName != Application.Session.CurrentProfileName)
                        {
                            config.ProfileName = Application.Session.CurrentProfileName;
                            configChanged = true;
                        }

                        if (configChanged)
                            BackupSettingsDao.SaveSettings(config);

                        try
                        {
                            Process.Start(config.BackupProgram);
                        }
                        catch (Exception e)
                        {
                            System.Windows.Forms.MessageBox.Show(config.BackupProgram + " not found! " + e.Message);
                        }
                    }
                }//config loaded

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// Extends ribbon bar of outlook
        /// </summary>
        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            Rib = new Ribbon(this);
            return Rib;
        }

        #region Von VSTO generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        /// <summary>
        /// returns the instance of the outlook application.
        /// </summary>
        public Outlook.Application GetApplication()
        {
            return Application;
        }

        #endregion
    }
}
