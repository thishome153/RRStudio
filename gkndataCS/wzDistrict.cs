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
    public partial class wzDistrict : Form
    {
        public netFteo.Cadaster.TCadastralDistrict Item;
        
        public wzDistrict()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void WzDistrict_Shown(object sender, EventArgs e)
        {
            textBox_FileName.Text = Item.Name;
            textBox_Number.Text = Item.CN;
        }
    }
}
