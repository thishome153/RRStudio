using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using netFteo.BaseClasess;

namespace Test_NetFteo_APP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //double test = Geodethic.RawAngleToRadians(180.0000);
            this.textBox2.Text  = Convert.ToString(Geodethic.RawAngleToRadians(Convert.ToDouble(textBox1.Text)));
            
        }
    }
}
