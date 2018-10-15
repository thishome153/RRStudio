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

namespace XMLReaderCS
{
    public partial class SchemaKPTMainForm : Form
    {
        public SchemaKPTMainForm()
        {
            InitializeComponent();
        }

        public RRTypes.SKPT01.SchemaParcels Schema1;
        netFteo.XML.FileInfo DocInfo = new netFteo.XML.FileInfo();
        XmlDocument XMLDoc;
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private RRTypes.SKPT01.SchemaParcels NewSchema()
        {
            this.Schema1 = new RRTypes.SKPT01.SchemaParcels();
            Schema1.eDocument = new RRTypes.SKPT01.SchemaParcelsEDocument();
            Schema1.eDocument.GUID = Guid.NewGuid().ToString();
            Schema1.ParcelSchema_In_Block = new RRTypes.SKPT01.SchemaParcelsAppliedFileCollection();
            Schema1.Document = new RRTypes.SKPT01.SchemaParcelsDocument();
            Schema1.NewParcels = new RRTypes.SKPT01.tNewParcelCollection();
            Schema1.Coord_Systems = new RRTypes.SKPT01.Coord_Systems();
            return this.Schema1;
        }

        private void ListSchema(RRTypes.SKPT01.SchemaParcels sc)
        {
            this.label3_GUID.Text = sc.eDocument.GUID;
            TV_Parcels.Nodes.Clear();
            for (int i = 0; i <= sc.NewParcels.Count - 1; i++)
            {
                TreeNode ParcelNode = TV_Parcels.Nodes.Add(sc.NewParcels[i].Definition);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            NewSchema();
            ListSchema(this.Schema1);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            NewSchema();
            ListSchema(this.Schema1);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SaveDoc(this.Schema1);
        }

        private void SaveDoc(RRTypes.SKPT01.SchemaParcels sc)
        {
            if (sc == null) return;
            PopulateSC(sc);
            saveFileDialog1.FileName = "SchemaParcels_" + sc.eDocument.GUID;
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(RRTypes.SKPT01.SchemaParcels));
                TextWriter writer = new StreamWriter(saveFileDialog1.FileName);
                serializer.Serialize(writer, sc);
                writer.Close();
            }

        }

// Заполнение информацией:
        private void PopulateSC(RRTypes.SKPT01.SchemaParcels sc)
        {
            RRTypes.SKPT01.SchemaParcelsAppliedFile fl = new RRTypes.SKPT01.SchemaParcelsAppliedFile();
            fl.Name = ParcelSchema_In_Block_textBox.Text;
            sc.ParcelSchema_In_Block.Add(fl);
            sc.Document.Number = textBox_NumberReG.Text;
            sc.Document.IssueOrgans = new string[] {textBox_NameIssueOrgan.Text};
            sc.Document.Name = DocName_comboBox.Text;
           //sc.NewParcels
            RRTypes.SKPT01.Coord_System csc = new RRTypes.SKPT01.Coord_System();
                    csc.Name = comboBox1_CSName.Text;
                    csc.Cs_Id = "ID0";
             sc.Coord_Systems.Coord_System = csc;
        }

        // Разбор информацией:
        private void ParseSC(RRTypes.SKPT01.SchemaParcels sc)
        {
            label3_GUID.Text = sc.eDocument.GUID;
            textBox_NumberReG.Text = sc.Document.Number;
            DocName_comboBox.Text = sc.Document.Name;
            textBox_DateReg.Text = sc.Document.Date.ToString();
            if (sc.Document.IssueOrgans.Count() > 0)
                textBox_NameIssueOrgan.Text = sc.Document.IssueOrgans[0];
            if (sc.ParcelSchema_In_Block.Count > 0)
                ParcelSchema_In_Block_textBox.Text = sc.ParcelSchema_In_Block[0].Name;
            comboBox1_CSName.Text = sc.Coord_Systems.Coord_System.Name;

            if (sc.NewParcels.Count == 1)
            {
                tabPage3.Text = "pkk5.........";
                pkk5Viewer1.Start(sc.NewParcels[0].CadastralBlock, RRTypes.pkk5.pkk5_Types.Block);
            }
        }
        


        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                ParcelSchema_In_Block_textBox.Text = Path.GetFileName(openFileDialog1.FileName);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox_DateReg.Text = DateTime.Now.ToString();
        }


        private void OpenFile(string FileName)
        {
            //RRTypes.SKPT01.SchemaParcels SKPT
            OpenXML(FileName);
        }

        public void OpenXML(string FileName)
        {
            DocInfo.FileName = Path.GetFileName(FileName);
            //ClearControls();
            if (Path.GetExtension(FileName).Equals(".xml"))
            {
                TextReader reader = new StreamReader(FileName);
                XMLDoc = new XmlDocument();
                XMLDoc.Load(reader);
                 reader.Dispose();
                OpenXML(XMLDoc);
            }
        }

        public void OpenXML(XmlDocument XMLDoc)
        {
            DocInfo.DocRootName = XMLDoc.DocumentElement.Name;
            DocInfo.Namespace = XMLDoc.DocumentElement.NamespaceURI;  // "urn://x-artefacts-rosreestr-ru/outgoing/kpt/10.0.1"
            Stream stream = new MemoryStream();
            XMLDoc.Save(stream);
            stream.Seek(0, 0);

            if (XMLDoc.DocumentElement.Name == "SchemaParcels")
            {
                XmlSerializer serializerTP = new XmlSerializer(typeof(RRTypes.SKPT01.SchemaParcels));
                RRTypes.SKPT01.SchemaParcels SKPT = (RRTypes.SKPT01.SchemaParcels)serializerTP.Deserialize(stream);
                ParseSC(SKPT);
            }
            cXmlTreeView1.LoadXML(XMLDoc); 
            //На пся крев просидел два дня....  SaveOpenedFileInfo(DocInfo, FileName);
        }
        

        private RRTypes.SKPT01.tNewParcel Addparcel(RRTypes.SKPT01.SchemaParcels sc)
        { /*
            RRTypes.SKPT01.tNewParcel NewParcel = new RRTypes.SKPT01.tNewParcel();
            wzParcelForm wzP = new wzParcelForm();
            wzP.Parcel = NewParcel;
            if (wzP.ShowDialog(this) == DialogResult.OK)
            {
                sc.NewParcels.Add(wzP.Parcel);
                ListSchema(sc);
                return wzP.Parcel;
            }
            else return null;
            */
            return null;
        }
        private void добавитьЗУToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Addparcel(this.Schema1);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                OpenFile(openFileDialog1.FileName);
            }
        }

        

        private void pkk5Viewer1_QuerySuccefull(object sender, EventArgs e)
        {
            tabPage3.Text = "pkk5  ok";
        }

        private void pkk5Viewer1_QueryStart(object sender, EventArgs e)
        {
            //tabPage3.Text = "pkk5.........";
        }

    }
}
