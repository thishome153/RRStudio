using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using InspectorAct;


namespace InspectorAct
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
                
        }
        InspectionAct Act1 = new InspectionAct();
        

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Act1.Conclusion = richTextBox1.Text;
            Act1.GUID = Guid.NewGuid().ToString();
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(InspectionAct));
                TextWriter writer = new StreamWriter(saveFileDialog1.FileName);
                serializer.Serialize(writer, Act1);
                writer.Close();
            }
        }
    }
}
