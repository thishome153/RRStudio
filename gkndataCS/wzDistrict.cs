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
        public TCurrentItem Item;
        
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
            textBox_FileName.Text = Item.Item_TypeName;
            textBox_Number.Text = Item.Item_NameExt;
        }
    }
}
