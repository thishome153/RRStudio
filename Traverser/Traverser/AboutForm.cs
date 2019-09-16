using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection; // Assembly&
using System.Diagnostics;// File version
using System.Windows.Forms;

namespace Traverser
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void richTextBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AboutForm_Shown(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            this.Text = "Traverser V."+ version;
        }
    }
}
