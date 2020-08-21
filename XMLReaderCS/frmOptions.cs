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

namespace XMLReaderCS
{
    public partial class frmOptions : Form
    {
        public netFteo.XML.XSDFile dutilizations_v01;
        public netFteo.XML.XSDFile dAllowedUse_v02;
        public netFteo.XML.XSDFile dRegionsRF_v01;
        public netFteo.XML.XSDFile dStates_v01;
        public netFteo.XML.XSDFile dWall_v01;
        public netFteo.XML.XSDFile dCategories_v01;
        public netFteo.XML.XSDFile dLocationLevel1_v01;
        public string dLocationLevel2_v01;
        public string MP_06_schema;
        public netFteo.XML.SchemaSet schemas;

        public frmOptions()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {
            // XSDSchemasList(dutilizations_v01, true);
            // XSDSchemasList(dRegionsRF_v01,false);
            // XSDSchemasList(dLocationLevel1_v01, false);
            LV_SchemaDisAssembly.Items.Clear();
            LV_SchemaDisAssembly.Columns[0].Text = "Значение";
            LV_SchemaDisAssembly.Columns[1].Text = "Документация/Аннотация";
            listView_Schemas.Items.Clear();

            schemas = new netFteo.XML.SchemaSet();

            schemas.AddSchema(dutilizations_v01.XSDFileName);
            schemas.AddSchema(dAllowedUse_v02.XSDFileName);
            schemas.AddSchema(dRegionsRF_v01.XSDFileName);
            schemas.AddSchema(dStates_v01.XSDFileName);
            schemas.AddSchema(dWall_v01.XSDFileName);
            schemas.AddSchema(dLocationLevel1_v01.XSDFileName); // already declared ???  !!!
            schemas.AddSchema(dCategories_v01.XSDFileName); // already declared ???  !!!
            schemas.AddSchema(dLocationLevel2_v01);
            schemas.AddSchema(MP_06_schema);
            schemas.CompileSet();
            foreach (System.Xml.Schema.XmlSchema sc in schemas.schemaSet.Schemas())
            {
                string rootName = "";
                // TODO Source URI to Path
                //Uri uri = new Uri(sc.SourceUri);
                //netFteo.XML.XSDFile xsd = new netFteo.XML.XSDFile(uri.LocalPath);
                netFteo.XML.XSDFile xsd = new netFteo.XML.XSDFile(sc);
                if (xsd.EnumerationPresent)
                {
                    XSDSchemasList(xsd, false);
                    rootName = xsd.SimpleTypeNamesAnoSafeFirst;
                }

                //TODO упаковать в netfteo.XML поиск root`a:
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                System.IO.MemoryStream str = new System.IO.MemoryStream();
                sc.Write(str);
                str.Seek(0, 0);
                doc.Load(str);
                System.Xml.XmlElement rootEl = doc.DocumentElement;

                foreach (System.Xml.XmlNode child in rootEl.ChildNodes)
                {
                    if (child.Name == "xs:element")
                        rootName = child.Attributes.GetNamedItem("name").Value;
                }
                ///end TODO

                ListViewItem sc_item = listView_Schemas.Items.Add(rootName);
                sc_item.SubItems.Add(sc.TargetNamespace);
                sc_item.SubItems.Add(System.IO.Path.GetFileName(new Uri(sc.SourceUri).LocalPath));
            }
        }

        /// <summary>
        /// Листинг xsd-перечислений
        /// </summary>
        /// <param name="xsdenum"></param>
        /// <param name="clear"></param>
        private void XSDSchemasList(netFteo.XML.XSDFile xsdenum, bool clear)
        {
            if (clear)
            {
                LV_SchemaDisAssembly.Items.Clear();
                LV_SchemaDisAssembly.Columns[0].Text = "Значение";
                LV_SchemaDisAssembly.Columns[1].Text = "Документация/Аннотация";
            }

            //Если это справочник XSD (enumeration):
            if (xsdenum.EnumerationPresent)
            {
                List<string> enumValues = xsdenum.Item2Item("", xsdenum.SimpleTypeNamesSafeFirst);
                List<string> enumAnno = xsdenum.Item2Annotation("", xsdenum.SimpleTypeNamesSafeFirst);
                LV_SchemaDisAssembly.Items.Add(xsdenum.SimpleTypeNamesAnoSafeFirst).SubItems.Add(xsdenum.SimpleTypeNamesSafeFirst);
                LV_SchemaDisAssembly.Items.Add("targetNamespace").SubItems.Add(xsdenum.targetNamespace);
                int i = 0;
                foreach (string s in enumValues)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = s;
                    lvi.SubItems.Add(enumAnno.ElementAt(i++)); //
                    LV_SchemaDisAssembly.Items.Add(lvi);
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {


        }


        private void listBox1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void копироватьToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            List<string> items = new List<string>();
            Control parent = ((ContextMenuStrip)(((ToolStripMenuItem)sender).Owner)).SourceControl;

            foreach (ListViewItem lvit in ((ListView)parent).Items)
            {
                string sub = "";
                if (lvit.Text != "")
                {
                    foreach (ListViewItem.ListViewSubItem lv in lvit.SubItems)
                    {
                        if (lv.Text != lvit.Text) // бывает что суб повторяет саму ноду
                            sub += "\t" + lv.Text;
                    }


                    items.Add(lvit.Text + sub);
                }
            }

            if (items.Count > 0)
            {
                //LINQ:
                Clipboard.SetText(string.Join(Environment.NewLine, items.Cast<string>().Select(o => o.ToString()).ToArray()));
            }
        }

        private void listView_Schemas_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void listView_Schemas_change(object sender)
        {
            if (listView_Schemas.SelectedItems.Count == 1)
            {
                LV_SchemaDisAssembly.Items.Clear();
                string targetnamespace = listView_Schemas.SelectedItems[0].SubItems[1].Text;
                System.Xml.Schema.XmlSchema sch = schemas.GetSchema(targetnamespace);

                LV_SchemaDisAssembly.Items.Add("Elements of schema");
                foreach (XmlSchemaElement elt in sch.Elements.Values)
                {
                    ListViewItem el_item = LV_SchemaDisAssembly.Items.Add(elt.Name);
                    if (elt.Annotation != null)
                        foreach (XmlSchemaDocumentation s in elt.Annotation.Items)
                        {
                            el_item.SubItems.Add(s.Markup[0].Value);
                        }

                }

                LV_SchemaDisAssembly.Items.Add("Types of schema");
                foreach (XmlSchemaType sct in sch.SchemaTypes.Values)
                {
                    ListViewItem el_item = LV_SchemaDisAssembly.Items.Add(sct.Name);
                    if (sct.Annotation != null)
                    {
                        foreach (XmlSchemaDocumentation s in sct.Annotation.Items)
                        {
                            el_item.SubItems.Add(s.Markup[0].Value);
                        }

                    }
                    else el_item.SubItems.Add("-");
                    el_item.SubItems.Add(sct.SourceUri);


                }


                LV_SchemaDisAssembly.Items.Add("includes of schema");
                foreach (XmlSchemaInclude incl in sch.Includes)
                {
                    ListViewItem el_item = LV_SchemaDisAssembly.Items.Add(incl.Schema.SourceUri);
                }



                netFteo.XML.XSDFile xsd = new netFteo.XML.XSDFile(sch);
                if (xsd.EnumerationPresent)
                {
                    XSDSchemasList(xsd, true);
                }

            }

        }

        private void listView_Schemas_KeyUp(object sender, KeyEventArgs e)
        {
            listView_Schemas_change(sender);
        }

        private void listView_Schemas_Click(object sender, EventArgs e)
        {
            listView_Schemas_change(sender);
        }
    }
}
