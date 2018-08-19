using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XMLReaderCS
{
    public partial class GUIDfrm : Form
    {
        public GUIDfrm()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
        }

        private void GUIDfrm_Load(object sender, EventArgs e)
        {
          GUID_gen();
        }

        private void GUID_gen()
        {
            Guid g;
            // Create and display the value of two GUIDs.
            g = Guid.NewGuid();
            linkLabel_GUID.Text = g.ToString(); //"GKUZU_" + 
            linkLabel_GKUOKS.Text = "GKUOKS_" + linkLabel_GUID.Text;
            linkLabel_GKUZU.Text = "GKUZU_" + linkLabel_GUID.Text;
        }        

        private void linkLabel_GKUOKS_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(linkLabel_GKUOKS.Text);
        }

        private void linkLabel_GUID_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(linkLabel_GUID.Text);
        }

        private void linkLabel_GKUZU_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(linkLabel_GKUZU.Text);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            GUID_gen();
        }

        private void GUIDfrm_Shown(object sender, EventArgs e)
        {
            GUID_gen();
        }
    }
}
