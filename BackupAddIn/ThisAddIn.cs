using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using Speed4Trade;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace BackupAddIn
{
    public partial class ThisAddIn
    {
        
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            ((Outlook.ApplicationEvents_11_Event)Application).Quit 
            += new Outlook.ApplicationEvents_11_QuitEventHandler(ThisAddIn_Quit);
        }

        /// <summary>
        /// Run backup program if neccessary
        /// </summary>
        void ThisAddIn_Quit()
        {
            BackupSettings config = FBackupSettings.loadSettings();  
            
            //if configuration was done and backup program was configured
            if (config != null && !String.IsNullOrEmpty(config.BackupProgram))
            {
                //and if not yet backuped or if backup too old, then run program
                if (config.LastRun == null || config.LastRun.AddDays(config.Interval) <= DateTime.Now)
                {
                    try
                    {
                        Process.Start(config.BackupProgram);
                    }
                    catch (Exception e)
                    {
                        System.Windows.Forms.MessageBox.Show(config.BackupProgram + " not found! " + e.Message);
                    }
                }
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
            return new Ribbon();
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
        
        #endregion
    }
}
