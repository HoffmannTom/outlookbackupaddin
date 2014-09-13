using Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;

// TODO: Führen Sie diese Schritte aus, um das Element auf dem Menüband (XML) zu aktivieren:

// 1: Kopieren Sie folgenden Codeblock in die ThisAddin-, ThisWorkbook- oder ThisDocument-Klasse.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon();
//  }

// 2. Erstellen Sie Rückrufmethoden im Abschnitt "Menübandrückrufe" dieser Klasse, um Benutzeraktionen
//    zu behandeln, z.B. das Klicken auf eine Schaltfläche. Hinweis: Wenn Sie dieses Menüband aus dem Menüband-Designer exportiert haben,
//    verschieben Sie den Code aus den Ereignishandlern in die Rückrufmethoden, und ändern Sie den Code für die Verwendung mit dem
//    Programmmodell für die Menübanderweiterung (RibbonX).

// 3. Weisen Sie den Steuerelementtags in der Menüband-XML-Datei Attribute zu, um die entsprechenden Rückrufmethoden im Code anzugeben.  

// Weitere Informationen erhalten Sie in der Menüband-XML-Dokumentation in der Hilfe zu Visual Studio-Tools für Office.


namespace BackupAddIn
{
    /// <summary>
    /// Ribbon extension for backup configuration dialog
    /// </summary>
    [ComVisible(true)]
    public class Ribbon : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;
        //private Microsoft.Office.Interop.Outlook.Application app;
        ThisAddIn addin;
        ResourceManager rm;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Ribbon(ThisAddIn a)
        {
            rm = new ResourceManager("BackupAddIn.lang.langres", typeof(Ribbon).Assembly);
            addin = a;
        }
        
        #region IRibbonExtensibility-Member

        /// <summary>
        /// Returns the xml for ribbon extension
        /// </summary>
        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("BackupAddIn.Ribbon.xml");
        }

        #endregion

        #region Menübandrückrufe
        //Erstellen Sie hier Rückrufmethoden. Weitere Informationen zum Hinzufügen von Rückrufmethoden finden Sie unter "http://go.microsoft.com/fwlink/?LinkID=271226".

        /// <summary>
        /// Extends ribbon of outlook for backup configuration
        /// </summary>
        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }

        /// <summary>
        /// Open configuration dialog
        /// </summary>
        public void OpenBackupSettings(Office.IRibbonControl control)
        {
            //Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
            //NameSpace outlookNs = app.GetNamespace("MAPI");

            FBackupSettings frm = new FBackupSettings();
            //frm.setStores(outlookNs.Stores);

            frm.setStores(addin.getApplication().Session.Stores);
            frm.ShowDialog();
        }

        #endregion

        public string GetLabel(Office.IRibbonControl Control)
        {
            string result = null;
            switch (Control.Id)
            {
                case "btnBackupSettings":
                    result = rm.GetString("RibbonSettings");
                    break;
                case "ribTabBackup":
                    result = rm.GetString("RibbonTabBackup");
                    break;
                case "ribBtnBackup":
                    result = rm.GetString("RibbonButtonBackup");
                    break;
            }
            return result;
        }

        #region Hilfsprogramme

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
