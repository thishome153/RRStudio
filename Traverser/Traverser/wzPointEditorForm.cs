using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using netFteo.BaseClasess;

namespace Traverser
{
    public partial class wzPointEditorForm : Form
    {
        public wzPointEditorForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public TmyPointO EditingPoint;

        private void wzPointEditorForm_Shown(object sender, EventArgs e)
        {

            textBox1.Text = this.EditingPoint.NumGeopointA;
            ordx_textBox.Text = Convert.ToString(this.EditingPoint.x);

        }
    }
}
