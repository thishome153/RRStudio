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

        public netFteo.XML.SchemaSet XMLSchemas;
        private string ValidateXMLMessage;
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
                //treeView1.ExpandAll();
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


        public void ValidateXMLEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    ValidateXMLMessage  += "Error: " + e.Message;
                    break;
                case XmlSeverityType.Warning:
                   ValidateXMLMessage += "Warning: " + e.Message;
                    break;
            }

        }

        /// <summary>
        /// Validate file against schema
        /// </summary>
        /// <param name="Schema"></param>
        /// <param name="XMLFile"></param>
        private void ValidateXML(XmlSchema Schema, XmlDocument XMLFile)
        {
            ValidationEventHandler eventHandler = new ValidationEventHandler(ValidateXMLEventHandler);
            if ((this.XMLSchemas != null) &&
                (XMLFile != null))
            {
                string testc = this.XMLSchemas.SchemaName;
                // the following call to Validate succeeds.
                XMLFile.Validate(eventHandler);
            }
            else
            {
                //throw event
                //ValidationEventArgs args = new EventArgs();
                //args.Message = "Schema missied";
                //EventArgs args = new EventArgs();
                //eventHandler(this, args);
                ValidateXMLMessage = "Schema not served";
            }
        }

        /// <summary>
        /// Populate trewview for xml nodes - links to files (pdf)
        /// </summary>
        /// <param name="workDir"></param>     
        private void PopulateChapterNodes(TreeNodeCollection nodes, XmlNode xmlBaseNode, string ChapterXpath, string ChapterName, string workDir)
        {
            if (xmlBaseNode.SelectSingleNode(ChapterXpath) != null)
            {
                TreeNode DiagramParcels = nodes.Add(ChapterName);
                TreeNode ItemNode = DiagramParcels.Nodes.Add(xmlBaseNode.SelectSingleNode(ChapterXpath+"/@Name").Value);
                if (xmlBaseNode.SelectSingleNode(ChapterXpath + "/@Name").Value.ToUpper().Contains(".PDF"))
                {
                    ItemNode.SelectedImageIndex = 13; ItemNode.ImageIndex = 13; // pdf image for node
                }

                ItemNode.Tag = "filelink-pdf";
                if (SignaturePresent(workDir + "\\" + xmlBaseNode.SelectSingleNode(ChapterXpath + "/@Name").Value))
                {
                    PopulateSignatureNodes(DiagramParcels, workDir + "\\" + xmlBaseNode.SelectSingleNode(ChapterXpath+"/@Name").Value + ".sig");
                }
            }
        }

        private void PopulateSignatureNodes(TreeNode node, string SignatureFileName)
        {
            node.ImageIndex = 10; node.SelectedImageIndex = 10;
            TreeNode sigNode = node.Nodes.Add("Подпись");
            sigNode.ImageIndex = 14;
            sigNode.SelectedImageIndex = 14;
            foreach (string subject in SignatureSubjects(SignatureFileName))
            {
                TreeNode subnode = sigNode.Nodes.Add(subject);
                subnode.ImageIndex = 9;
                subnode.SelectedImageIndex = 9;
            }
        }

        private List<string> SignatureSubjects(string SignatureFileName)
        {
            frmCertificates certfrm = new frmCertificates();
            return certfrm.ParseSignature(SignatureFileName);
        }

        /// <summary>
        /// Check packet files in previosly unpacked archive
        /// </summary>
        /// <param name="workDir"></param>
        private void CheckIt(string workDir)
        {


            if (Directory.Exists(workDir))
            {
                AddCheckPosition(listView1, "Состав пакетa", "Наличие лишних файлов", "....");
                DirectoryInfo di = new DirectoryInfo(workDir);
                string ze_local = di.GetFiles().Select(fi => fi.Name).FirstOrDefault(name => name != "*.xml");
                string ze = workDir + "\\" + ze_local;
                // теперь загружаем xml

                if (ze.Contains(".xml") && !ze.Contains(".sig"))
                {
                    this.fMP_v06_xml = new XmlDocument();

                    XmlReaderSettings settings = new XmlReaderSettings();
                    // settings.Schemas.Add("http://www.w3.org/2001/XMLSchema", xsdfilename);
                    //TODO: settings.Schemas.Add(null, xsdfilename);
                    settings.ValidationType = ValidationType.Schema;
                    settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessIdentityConstraints
                        | XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation;
                    XmlReader reader = XmlReader.Create(ze, settings);
                    this.fMP_v06_xml.Load(reader);

                    XmlNode MP_Root = fMP_v06_xml.DocumentElement;
                    // Вначале отобразим xml, вдруг далее парсеры слажают... :)
                    cXmlTreeView2.RootName = ze_local;
                    cXmlTreeView2.LoadXML(fMP_v06_xml); // Загрузим тело в дерево XMlTreeView - собственный клас/компонент, умеющий показывать XmlDocument
                    string version = "-";
                    if (MP_Root.Attributes.GetNamedItem("Version") != null) // Для MP версия в корне
                        version = MP_Root.Attributes.GetNamedItem("Version").Value;

                    // Типы MP Версия 06 - без XSD to clasess. 
                    if ((MP_Root.Name == "MP") && (version == "06"))
                    {
                        //StartValidate file against schema
                        // Got MP schema: frmValidator, frmOptions
                        ValidateXML(null, fMP_v06_xml);

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

                        label_doc_GUID.Text = ze_local;
                        TreeNode MPNode = treeView1.Nodes.Add(MP_Root.Name + "( "+ MP_Root.SelectSingleNode("GeneralCadastralWorks/@DateCadastral").Value+ ")");

                        if (SignaturePresent(ze))
                        {
                            Label_sig_Properties.Image = XMLReaderCS.Properties.Resources.sign;
                            Label_sig_Properties.Text = "";
                            foreach (string sub in SignatureSubjects(ze + ".sig"))
                            {
                                Label_sig_Properties.Text += sub;
                            }
                        }
                        else
                        {
                            Label_sig_Properties.Image = XMLReaderCS.Properties.Resources.cross;
                            Label_sig_Properties.Text = "Документ не подписан";
                        }

                        TreeNode TitleNodes = MPNode.Nodes.Add("Общие сведения");
                        // / MP / GeneralCadastralWorks / Reason
                        TitleNodes.Nodes.Add("Межевой план подготовлен в результате выполнения кадастровых работ в связи с:").Nodes.Add(MP_Root.SelectSingleNode("GeneralCadastralWorks/Reason").FirstChild.Value);

                        // /MP / InputData / Documents / Document[1] / Name
                        if (MP_Root.SelectSingleNode("InputData") != null)
                        {
                            TreeNode inpDataNodes =MPNode.Nodes.Add("Исходные данные ");
                            inpDataNodes.ImageIndex = 11;
                            inpDataNodes.SelectedImageIndex = 11;

                            if ((MP_Root.SelectSingleNode("InputData/Documents") != null) &&
                                  (MP_Root.SelectSingleNode("InputData/Documents").ChildNodes.Count > 0))
                            {
                                TreeNode docsNode = inpDataNodes.Nodes.Add("Документы");
                                foreach (XmlNode doc in MP_Root.SelectSingleNode("InputData/Documents").ChildNodes)
                                {
                                    TreeNode docNode = docsNode.Nodes.Add(doc.SelectSingleNode("Number").FirstChild.Value); //minoccurs= 1
                                    if (doc.SelectSingleNode("Name") != null)
                                    {
                                        docNode.Text = doc.SelectSingleNode("Name").FirstChild.Value;
                                        docNode.Nodes.Add(doc.SelectSingleNode("Number").FirstChild.Value);
                                    }
                                        if (doc.SelectSingleNode("Date") != null)
                                        docNode.Nodes.Add(doc.SelectSingleNode("Date").FirstChild.Value);  // /MP/InputData/Documents/Document[1]/Number
                                }
                            }

                            // / MP / InputData / GeodesicBases / GeodesicBase[1] / PName
                            if ((MP_Root.SelectSingleNode("InputData/GeodesicBases") != null) &&
                                 (MP_Root.SelectSingleNode("InputData/GeodesicBases").ChildNodes.Count > 0))
                            {
                                TreeNode docsNode = inpDataNodes.Nodes.Add("Сведения о геодезической основе");
                                docsNode.ImageIndex = 5;
                                docsNode.SelectedImageIndex = 5;

                                foreach (XmlNode doc in MP_Root.SelectSingleNode("InputData/GeodesicBases").ChildNodes)
                                {
                                    TreeNode docNode = docsNode.Nodes.Add(doc.SelectSingleNode("PName").FirstChild.Value);
                                    /// MP / InputData / GeodesicBases / GeodesicBase[1] / PKind
                                    /// 
                                    docNode.Nodes.Add(doc.SelectSingleNode("PKind").FirstChild.Value);
                                    // / MP / InputData / GeodesicBases / GeodesicBase[1] / PKlass
                                    docNode.Nodes.Add(doc.SelectSingleNode("PKlass").FirstChild.Value);
                                    docNode.Nodes.Add(doc.SelectSingleNode("OrdX").FirstChild.Value);
                                    docNode.Nodes.Add(doc.SelectSingleNode("OrdY").FirstChild.Value);
                                    docNode.ImageIndex = 5;
                                    docNode.SelectedImageIndex = 5;
                                }
                            }
                            inpDataNodes.Expand();
                        }
                        if (MP_Root.SelectSingleNode("Conclusion") != null)
                        {
                            var conclusion = MP_Root.SelectSingleNode("Conclusion").FirstChild.Value;
                        }
                        // AddCheckPosition(listView1, "Пространственные данные", "----------", "-");

                        PopulateChapterNodes(MPNode.Nodes, MP_Root, "SchemeGeodesicPlotting", "СГП", workDir);
                        PopulateChapterNodes(MPNode.Nodes, MP_Root, "SchemeDisposition", "СРП", workDir);
                        PopulateChapterNodes(MPNode.Nodes, MP_Root, "DiagramParcelsSubParcels", "Чертеж", workDir);
                        PopulateChapterNodes(MPNode.Nodes, MP_Root, "AgreementDocument", "Акт согласования", workDir);

                        if (MP_Root.SelectSingleNode("Appendix") != null)
                        {
                            TreeNode appndxNodes = MPNode.Nodes.Add("Приложения (AppliedFiles)");
                            {
                                foreach (XmlNode app in MP_Root.SelectSingleNode("Appendix").ChildNodes)
                                {
                                    // / MP/Appendix/AppliedFiles[1]/AppliedFile/@Name
                                    PopulateChapterNodes(appndxNodes.Nodes, app, "AppliedFile", app.SelectSingleNode("NameAppendix").FirstChild.Value, workDir);
                                }
                            }
                            appndxNodes.Expand();
                        }
                        MPNode.Expand();
                        AddCheckPosition(listView1, "Xml validation", "MP_v06", ValidateXMLMessage); //Show validation results
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
            //listView1.Height = this.Height - 244;
        }

        private void ESChecker_MP06Form_Load(object sender, EventArgs e)
        {

        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            TreeNode selnode = ((TreeView)sender).SelectedNode;

            if ((selnode != null) && (selnode.Tag != null)
                &&
                (selnode.Tag.ToString() == "filelink-pdf"))
            {
                string test = ((TreeView)sender).SelectedNode.Tag.ToString();
            }

        }
    }
}
