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
                valide(fd.FileName);

            }
        }


        public void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    richTextBox1.Text +="Error: "+  e.Message;
                    break;
                case XmlSeverityType.Warning:
                    richTextBox1.Text += "Warning: " + e.Message;
                    break;
            }

        }


        private void valide(string xsdfilename)
        {
        try
        {
            XmlReaderSettings settings = new XmlReaderSettings();
           // settings.Schemas.Add("http://www.w3.org/2001/XMLSchema", xsdfilename);
            settings.Schemas.Add(null, xsdfilename);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessIdentityConstraints
                | XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation;
            XmlReader reader = XmlReader.Create(xmlToValide, settings);
            XmlDocument document = new XmlDocument();
            document.Load(reader);

            ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);

            // the following call to Validate succeeds.
            document.Validate(eventHandler);
            /*
            // add a node so that the document is no longer valid
            XPathNavigator navigator = document.CreateNavigator();
            navigator.MoveToFollowing("price", "http://www.contoso.com/books");
            XmlWriter writer = navigator.InsertAfter();
            writer.WriteStartElement("anotherNode", "http://www.contoso.com/books");
            writer.WriteEndElement();
            writer.Close();
            // the document will now fail to successfully validate
            document.Validate(eventHandler);
             *             
             */
        }
        catch (Exception ex)
        {
           richTextBox1.Text =ex.Message;
        }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

    }
}
