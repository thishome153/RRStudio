using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace XMLReaderCS
{
    public partial class frmValidator : Form
    {
        public string xmlToValide;
		public string xsdToValide;
		public frmValidator()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Schemas|*.xsd";
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                richTextBox1.Text = " Open schema " + System.IO.Path.GetFileName(fd.FileName);
				xsdToValide = fd.FileName;
                ValideXML(xsdToValide);
				button2.Enabled = true;
            }
        }


        public void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
			var ee = e;
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    richTextBox1.Text +="Error: "+  e.Message + "\r\n";
                    break;
                case XmlSeverityType.Warning:
                    richTextBox1.Text += "Warning: " + e.Message + "\r\n";
                    break;
            }

        }


		public void ValideXML(string xsdfilename)
		{
			try
			{
				richTextBox1.Text = "Validating against schema " + xsdfilename + "\r\n";
				button2.Enabled = false;
				ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);
				if (radioButton_XMLValReader.Checked)
				{
					richTextBox1.Text += "using obsolete XmlValidationReader\r\n";
					XmlTextReader tr = new XmlTextReader(xmlToValide);
					XmlValidatingReader vr = new XmlValidatingReader(tr);
					vr.ValidationType = ValidationType.Schema;
					vr.Schemas.Add(null, xsdfilename);
					vr.ValidationEventHandler += eventHandler;
					while (vr.Read()) ;
					vr.Close();
				}
				else

				{
					richTextBox1.Text += "using Xmlreader /Document \r\n";
					XmlReaderSettings settings = new XmlReaderSettings();
					// settings.Schemas.Add("http://www.w3.org/2001/XMLSchema", xsdfilename);
					settings.Schemas.Add(null, xsdfilename);
					settings.ValidationType = ValidationType.Schema;
					settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessIdentityConstraints
						| XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation;
					XmlReader reader = XmlReader.Create(xmlToValide, settings);
					XmlDocument document = new XmlDocument();
					document.Load(reader);
					// the following call to Validate succeeds.
					document.Validate(eventHandler);
				}
			}
			catch (Exception ex)
			{
				richTextBox1.Text += ex.Message;
				button2.Enabled = false;
			}
			button2.Enabled = true;
		}

        private void button2_Click(object sender, EventArgs e)
        {

        }

		private void button2_Click_1(object sender, EventArgs e)
		{
			ValideXML(xsdToValide);
		}
	}
}
