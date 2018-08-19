using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GKNData
{
    public partial class wzKPTProperty : Form
    {
        public netFteo.BaseClasess.TFile ITEM;
        public wzKPTProperty(netFteo.BaseClasess.TFile item)
        {
            InitializeComponent();
            this.ITEM = item;
        }

        private void SetupControls()
        {
           textBox_FileName.Text = ITEM.FileName;
            textBox_Code.Text = ITEM.AccessCode;
            textBox_Date.Text = ITEM.Doc_Date;
            textBox_id.Text = ITEM.id.ToString();
            textBox_RequestNumber.Text = ITEM.RequestNum;
            textBox_xmlns.Text = ITEM.xmlns;
            label_sizeXML.Text = ITEM.xmlSize_SQL.ToString("0.00");
        }

        private void wzKPTProperty_Shown(object sender, EventArgs e)
        {
            SetupControls();
        }

        private void textBox_FileName_TextChanged(object sender, EventArgs e)
        {
            this.ITEM.FileName = ((TextBox)sender).Text;
        }

        private void textBox_id_TextChanged(object sender, EventArgs e)
        {
           // this.ITEM.id = ((TextBox)sender).Text;
        }
    }
}
