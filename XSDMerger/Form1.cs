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
using System.IO;

namespace XSDMerger
{
   
    public partial class XSDMergerForm : Form
    {
        public XSDMergerForm()
        {
            InitializeComponent();
        }
        XSDItem MainXsd = new XSDItem();
        XSDCollection XSDList = new XSDCollection(); 
        
        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.MainXsd.XSDFileName = openFileDialog1.FileName;
                MainXsd = new XSDItem(openFileDialog1.FileName);
                textBox1.Text = this.MainXsd.XSDFileName;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.listBox1.Items.Add(XSDList.AddXsd(openFileDialog1.FileName));
            }
        }


        private void Merge()
        {
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);
            schemaSet.Add(MainXsd.XSDNS, this.MainXsd.XSDFileName);
            for (int i = 0; i <= XSDList.Count - 1; i++)
            {
                schemaSet.Add(XSDList.Items[i].XSDNS, XSDList.Items[i].XSDFileName);

            }
                
                schemaSet.Compile();
                
            XmlSchema MainSch = null;

            foreach (XmlSchema schema in schemaSet.Schemas())
            {
                if (schema.TargetNamespace == MainXsd.XSDNS)
                    MainSch = schema;
            }


            for (int i = 0; i <= MainSch.Includes.Count - 1; i++)
            {
                ConsoleText.AppendText(MainSch.Includes[i].Namespaces.ToString()+"\n");
            }
            //Импортируемые схемы:
            for (int i = 0; i <= XSDList.Count - 1; i++)
            {
                XmlSchemaImport import = new XmlSchemaImport();
                XmlSchema ImportSch = null;
                import.Namespace = XSDList.Items[i].XSDNS;
                foreach (XmlSchema schema in schemaSet.Schemas())
                {
                    if (schema.TargetNamespace == XSDList.Items[i].XSDNS)
                        ImportSch = schema;
                }
                import.Schema = ImportSch;
                MainSch.Includes.Add(import);
            }
            schemaSet.Reprocess(MainSch);
            schemaSet.Compile();

            StreamWriter writer = File.CreateText("MAINXSD.xsd"); //.c XmlWriter.Create("MAINXSD.xsd", settings);
            MainSch.Write(writer);
            
            RecurseExternals(MainSch, writer);  
            
            writer.Close();
        }


        private void RecurseExternals(XmlSchema schema, StreamWriter wr)
        {
            foreach (XmlSchemaExternal external in schema.Includes)
            {
                if (external.SchemaLocation != null)
                {
                    ConsoleText.AppendText("External SchemaLocation: {0}"+external.SchemaLocation);
                }

                if (external is XmlSchemaImport)
                {
                    XmlSchemaImport import = external as XmlSchemaImport;
                    ConsoleText.AppendText("Imported namespace: {0}"+ import.Namespace);
                }

                if (external.Schema != null)
                {
                    external.Schema.Write(wr);
                    RecurseExternals(external.Schema, wr);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Merge();
        }

        private void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
               ConsoleText.AppendText("WARNING: ");
            else if (args.Severity == XmlSeverityType.Error)
                ConsoleText.AppendText("ERROR: ");

            ConsoleText.AppendText(args.Message);
        }
        public class XSDCollection
        {
            public int Count
            {
                get { return this.Items.Count; }
            }
            public XSDCollection()
            {
                this.Items = new List<XSDItem>();
            }
            public List<XSDItem> Items;

            public string AddXsd(string FileName)
            {
                this.Items.Add(new XSDItem(FileName));
                return this.Items[this.Items.Count - 1].XSDFileName;
            }
        }

        public class XSDItem
        {
            public string XSDFileName;
            public string XSDNS;
            public XSDItem()
            {
                this.XSDNS = "";
                this.XSDFileName = "";
            }
            public XSDItem(string FileName)
            {
                TextReader reader = new StreamReader(FileName);
                XmlDocument XMLDoc;
                XMLDoc = new XmlDocument();
                XMLDoc.Load(reader);
                XmlAttributeCollection attr = XMLDoc.DocumentElement.Attributes;
                this.XSDNS = attr.GetNamedItem("targetNamespace").Value;
                this.XSDFileName = FileName;
                reader.Close();
            }
        }


    }
}
