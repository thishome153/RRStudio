using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XMLReaderCS
{
    public partial class ESChecker_MP06Form : Form
    {
        public ESChecker_MP06Form()
        {
            InitializeComponent();
            treeView1.Nodes.Clear();
            listView1.Items.Clear();

        }

        public netFteo.XML.SchemaSet schemas;
        private Ionic.Zip.ZipFile fMP06ZiptoCkeck;
        private string fMP06UnzippedFolder;

        public Ionic.Zip.ZipFile MP06ZiptoCheck
        {
            get

            {
                return this.fMP06ZiptoCkeck;
            }

            set
            {
                this.fMP06ZiptoCkeck = value;
                //CheckArchieve();
            }
        }

        private XmlDocument fMP_v06_xml;
        public XmlDocument MP_v06
        {
            get
            {
                return this.fMP_v06_xml;
            }
            set
            {
                this.fMP_v06_xml = value;
            }
        }

        public string MP06UnZiptoCheck
        {
            set
            {
                this.fMP06UnzippedFolder = value;
                CheckIt(value);

            }
        }
        /*
        private RRTypes.MP_V06.MP fMP_v06;
        public RRTypes.MP_V06.MP MP_v06
        {
            get
            {
                return this.fMP_v06;
            }
            set
            {
                this.fMP_v06 = value;
            }
        }
        */

        private ListViewItem AddCheckPosition(ListView lv, string ParamName, string Value, string resValue)
        {
            ListViewItem res = lv.Items.Add(ParamName);
            if (Value != null)
                res.SubItems.Add(Value);
            if (resValue != null)
                res.SubItems.Add(resValue);
            return res;
        }

        private bool SignaturePresent(string filename)
        {
            return File.Exists(filename + ".sig");
        }


        public void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                   // richTextBox1.Text += "Error: " + e.Message;
                    break;
                case XmlSeverityType.Warning:
                   // richTextBox1.Text += "Warning: " + e.Message;
                    break;
            }

        }


        /// <summary>
        /// Chtcking unflatted archieve files for packet MP V06
        /// </summary>
        /*
        private void CheckArchieve()
        {
            frmCertificates certfrm = new frmCertificates();
            ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);

            TreeNode zipNode = this.treeView1.Nodes.Add("Пакет").Nodes.Add(MP06ZiptoCheck.Name);
            foreach (Ionic.Zip.ZipEntry ze in MP06ZiptoCheck)
            {
                ze.Extract(Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                if (ze.Attributes != FileAttributes.Directory)
                {
                    zipNode.Nodes.Add(ze.FileName);//.Nodes.Add(ze.Info);
                    if (ze.FileName.Contains(".sig"))
                    {

                        StreamReader sigreader = File.OpenText(ze.FileName);
                        List<string> sig = certfrm.ParseSignature(ze.FileName);
                        foreach (string subject in sig)
                            AddCheckPosition(listView1, "Signature", ze.FileName, subject);
                    }

                    if (ze.FileName.Contains(".xml") && !ze.FileName.Contains(".sig"))
                    {
                        // Stream zipEntryStream = new MemoryStream();
                        // zipEntryStream = ze.InputStream;
                        TextReader reader = new StreamReader(ze.FileName);
                        XmlDocument XMLDocFromFile = new XmlDocument();
                        XMLDocFromFile.Load(reader);
                        reader.Close();
                        //Read(XMLDocFromFile);
                        string rootname = XMLDocFromFile.DocumentElement.Name;
                        string version = "-";
                        if (XMLDocFromFile.DocumentElement.Attributes.GetNamedItem("Version") != null) // Для MP версия в корне
                            version = XMLDocFromFile.DocumentElement.Attributes.GetNamedItem("Version").Value;

                        // Типы MP Версия 06 - без XSD to clasess. напрямую XSD.exe
                        if ((rootname == "MP") && (version == "06"))
                        {
                            Stream stream = new MemoryStream();
                            XMLDocFromFile.Save(stream);
                            stream.Seek(0, 0);


                            XmlSerializer serializerMP = new XmlSerializer(typeof(RRTypes.MP_V06.MP));
                            this.fMP_v06 = (RRTypes.MP_V06.MP)serializerMP.Deserialize(stream);

                            Guid testGUID;
                            bool validGuid = Guid.TryParse(fMP_v06.GUID, out testGUID);
                            if (validGuid)
                                AddCheckPosition(listView1, "MP_v06 guid ", "GUID", "OK");
                            else
                                AddCheckPosition(listView1, "MP_v06 ", "GUID", "invalid");
                            label_doc_GUID.Text = fMP_v06.GUID + "  " + fMP_v06.GeneralCadastralWorks.DateCadastral.ToString();

                        }

                    }
                }
            }// foreach entry
            treeView1.ExpandAll();
            AddCheckPosition(listView1, "Исходные данные", "..", "-");
            AddCheckPosition(listView1, "Пространственные данные", "..", "-");
            if (this.fMP_v06.Appendix != null)
                AddCheckPosition(listView1, "Приложения", this.fMP_v06.Appendix.Count().ToString(), "..");
            {
                foreach (RRTypes.MP_V06.tAppendixAppliedFiles appxs in this.fMP_v06.Appendix)
                    AddCheckPosition(listView1, appxs.NameAppendix, appxs.AppliedFile.Name, "-");
            }


        }
        */
        /// <summary>
        /// Check packet files in previosly unpacked archive
        /// </summary>
        /// <param name="workDir"></param>
        private void CheckIt(string workDir)
        {
            frmCertificates certfrm = new frmCertificates();

            if (Directory.Exists(workDir))
            {
                DirectoryInfo di = new DirectoryInfo(workDir);
                string ze = workDir + "\\" + di.GetFiles().Select(fi => fi.Name).FirstOrDefault(name => name != "*.xml");
                // теперь загружаем xml

                if (ze.Contains(".xml") && !ze.Contains(".sig"))
                {
                    this.fMP_v06_xml = new XmlDocument();
                    /*
                    TextReader reader = new StreamReader(ze);
                    fMP_v06_xml.Load(reader);
                    reader.Close();
                    */

                    XmlReaderSettings settings = new XmlReaderSettings();
                    // settings.Schemas.Add("http://www.w3.org/2001/XMLSchema", xsdfilename);
                    //TODO: settings.Schemas.Add(null, xsdfilename);
                    settings.ValidationType = ValidationType.Schema;
                    settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessIdentityConstraints
                        | XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation;
                    XmlReader reader = XmlReader.Create(ze, settings);
                    this.fMP_v06_xml.Load(reader);
                    XmlNode MP_Root = fMP_v06_xml.DocumentElement;
                    string version = "-";
                    if (MP_Root.Attributes.GetNamedItem("Version") != null) // Для MP версия в корне
                        version = MP_Root.Attributes.GetNamedItem("Version").Value;

                    // Типы MP Версия 06 - без XSD to clasess. 
                    if ((MP_Root.Name == "MP") && (version == "06"))
                    {
                        //Validate file against schema
                        // Got MP schema: frmValidator, frmOptions
                        AddCheckPosition(listView1, "Xml validation", "MP_v06", "Valide");

                        // / MP / @GUID
                        string guid = MP_Root.Attributes.GetNamedItem("GUID").Value;

                        // / MP / GeneralCadastralWorks / @DateCadastral

                        Guid testGUID;
                        //bool validGuid = Guid.TryParse(fMP_v06.GUID, out testGUID);
                        bool validGuid = Guid.TryParse(guid, out testGUID); ;

                        if (validGuid)
                            AddCheckPosition(listView1, "GUID", guid, "OK");
                        else
                            AddCheckPosition(listView1, "GUID", guid, "invalid");
                        label_doc_GUID.Text = guid;
                    }


                    if (SignaturePresent(ze))
                    {

                        StreamReader sigreader = File.OpenText(ze + ".sig");
                        List<string> sig = certfrm.ParseSignature(ze + ".sig");
                        string sigs = "";
                        foreach (string subject in sig)
                            sigs += subject + ", ";
                        AddCheckPosition(listView1, "Signature", Path.GetFileName(ze), sigs);
                    }




                    treeView1.ExpandAll();

                    AddCheckPosition(listView1, "Состав пакетa", "Наличие лишних файлов", "....");
                    AddCheckPosition(listView1, "Исходные данные", "----------", "-");
                    AddCheckPosition(listView1, "Пространственные данные", "----------", "-");


                    //xpath: /MP/SchemeGeodesicPlotting/@Name
                    AddCheckPosition(listView1, "СГП", MP_Root.SelectSingleNode("SchemeGeodesicPlotting") != null ? Path.GetFileName(MP_Root.SelectSingleNode("SchemeGeodesicPlotting/@Name").Value) : "-", "?");
                    AddCheckPosition(listView1, "СРП", MP_Root.SelectSingleNode("SchemeDisposition") != null ? Path.GetFileName(MP_Root.SelectSingleNode("SchemeDisposition/@Name").Value) : " - ", "?");
                    AddCheckPosition(listView1, "Чертеж", MP_Root.SelectSingleNode("DiagramParcelsSubParcels") != null ?  Path.GetFileName(MP_Root.SelectSingleNode("DiagramParcelsSubParcels/@Name").Value):"-", " ? ");
                    if (SignaturePresent(workDir + "\\" + MP_Root.SelectSingleNode("DiagramParcelsSubParcels/@Name").Value))
                        AddCheckPosition(listView1, "Чертеж", "ЭЦП", "OK");
                    AddCheckPosition(listView1, "Акт согласования", MP_Root.SelectSingleNode("AgreementDocument") != null ? Path.GetFileName(MP_Root.SelectSingleNode("AgreementDocument/@Name").Value) : " - ", "?");

                    if (MP_Root.SelectSingleNode("Appendix") != null)
                        AddCheckPosition(listView1, "Приложения", "----------", "..");
                    { 
                        foreach(XmlNode app in MP_Root.SelectSingleNode("Appendix").ChildNodes)
                        {
                            // / MP/Appendix/AppliedFiles[1]/AppliedFile/@Name
                            AddCheckPosition(listView1, app.SelectSingleNode("NameAppendix").FirstChild.Value, Path.GetFileName(app.SelectSingleNode("AppliedFile/@Name").Value), "-");

                        }
                    }
                    
                }
            }
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ESChecker_MP06Form_SizeChanged(object sender, EventArgs e)
        {
            treeView1.Width = this.Width - 14;
            listView1.Width = this.Width - 14;
            listView1.Height = this.Height - 354;
        }

    }
}
