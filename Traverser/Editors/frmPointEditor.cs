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
        /*
        /// <summary>
        /// Editing item
        /// </summary>
        public netFteo.Spatial.TPoint Point;
*/
        public frmPointEditor(netFteo.Spatial.IGeometry feature)
        {
            InitializeComponent();
            if (feature.TypeName == NetFteoTypes.Point)
            {
                tPointBindingSource.Add(feature);
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

        private void Button3_Click(object sender, EventArgs e)
        {
            //Point = (netFteo.Spatial.TPoint)tPointBindingSource.List[0];
            //Point.oldX = netFteo.Spatial.Coordinate.NullOrdinate;
            ((netFteo.Spatial.TPoint)tPointBindingSource.List[0]).oldX = netFteo.Spatial.Coordinate.NullOrdinate;
            tPointBindingSource.ResetBindings(true);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            //Point = (netFteo.Spatial.TPoint)tPointBindingSource.List[0];
            //Point.oldY = netFteo.Spatial.Coordinate.NullOrdinate;
            ((netFteo.Spatial.TPoint)tPointBindingSource.List[0]).oldY = netFteo.Spatial.Coordinate.NullOrdinate;
            tPointBindingSource.ResetBindings(true);
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if (((netFteo.Spatial.TPoint)tPointBindingSource.List[0]).Pref == "")
                ((netFteo.Spatial.TPoint)tPointBindingSource.List[0]).Pref = "н";
            else
                ((netFteo.Spatial.TPoint)tPointBindingSource.List[0]).Pref = "";
            tPointBindingSource.ResetBindings(true);
        }
    }
}
