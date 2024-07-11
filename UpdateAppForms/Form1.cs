using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpdateAppForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void GetSoftwareVersion()
        {
            // Get the currently executing assembly
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Get the version attribute from the assembly
            Version version = assembly.GetName().Version;

            // Alternatively, to get the AssemblyFileVersion
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            string fileVersion = "Not Available";
            if (attributes.Length > 0)
            {
                AssemblyFileVersionAttribute fileVersionAttribute = (AssemblyFileVersionAttribute)attributes[0];
                fileVersion = fileVersionAttribute.Version;
            }

            // Set the label text with the assembly version information
            version_label.Text = $"Assembly Version: {version}";
        }


    }
}
