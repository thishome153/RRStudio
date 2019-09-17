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
    public partial class frmPointEditor : Form
    {
        /// <summary>
        /// Editing item
        /// </summary>
        netFteo.Spatial.TPoint Point;
        public frmPointEditor(netFteo.Spatial.IGeometry point)
        {
            InitializeComponent();
            if (point.TypeName == "netFteo.Spatial.TPoint")
            {
                Point = (netFteo.Spatial.TPoint) point;
              //  textBox_Name.Text = Point.NumGeopointA;
                textBox_x.Text = Point.x_s;
                //textBox_y.Text = Point.y_s;
                textBox_z.Text = Point.z_s;
                textBox_Mt.Text = Point.Mt_s;
                string test1 = tPointBindingSource1.ToString();
                 test1 = tPointBindingSource.ToString();
            }
        }


        private void Button2_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
