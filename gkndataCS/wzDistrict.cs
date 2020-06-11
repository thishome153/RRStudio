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
        //public TCurrentItem Item;
        
        public wzDistrict(TCurrentItem Item)
        {
            InitializeComponent();
            bindingSource1.Add(Item);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            bindingSource1.EndEdit();
            this.Close();
        }

        private void WzDistrict_Shown(object sender, EventArgs e)
        {
        //    textBox_FileName.Text = Item.Item_TypeName;
        //    textBox_CN.Text = Item.Item_NameExt;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            bindingSource1.CancelEdit();
            this.Close();
        }
    }
}
