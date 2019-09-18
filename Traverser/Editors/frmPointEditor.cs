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
        public netFteo.Spatial.TPoint Point;
        public frmPointEditor(netFteo.Spatial.IGeometry point)
        {
            InitializeComponent();
            if (point.TypeName == "netFteo.Spatial.TPoint")
            {
                tPointBindingSource.Add(point);
            }
        }


        private void Button2_Click(object sender, EventArgs e)
        {
            tPointBindingSource.CancelEdit();
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            tPointBindingSource.EndEdit();
            this.Close();
        }
    }
}
