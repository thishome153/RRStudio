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
        public netFteo.Spatial.TFile ITEM;
        public wzKPTProperty(netFteo.Spatial.TFile item)
        {
            InitializeComponent();
            this.ITEM = item;
        }

        private void SetupControls()
        {
           textBox_FileName.Text = ITEM.FileName;
            textBox_AccessCode.Text = ITEM.AccessCode;
            textBox_Date.Text = ITEM.Doc_Date;
            textBox_Number.Text = ITEM.Number;
            Text = "Свойства файла КПТ." + ITEM.id.ToString(); //textBox_id.Text = ITEM.id.ToString();
            textBox_RequestNumber.Text = ITEM.RequestNum;
            textBox_xmlns.Text = ITEM.xmlns;
            label_sizeXML.Text = ITEM.xmlSize_SQL.ToString("0.00");
            label_DocType.Text = ITEM.Type.ToString();
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

		private void textBox_xmlns_TextChanged(object sender, EventArgs e)
		{
			ITEM.xmlns = ((TextBox)sender).Text;
		}
	}
}
