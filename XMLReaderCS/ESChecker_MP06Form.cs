using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
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


        private Ionic.Zip.ZipFile fMP06ZiptoCkeck;
        private string fMP06UnzippedFolder;
        public  Ionic.Zip.ZipFile MP06ZiptoCheck
        {
            get

            {
                return this.fMP06ZiptoCkeck;
            }

            set 
            {
                this.fMP06ZiptoCkeck = value;
                CheckArchieve();
                //this.fMP06ZiptoCkeck.
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

        private RRTypes.MP_V06.MP fMP_v06;
        /*
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
            return false;
        }

        private void CheckArchieve()
        {
            frmCertificates certfrm = new frmCertificates();

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
                            AddCheckPosition(listView1, "MP_v06 xml", fMP_v06.GUID, "OK");
                            label_doc_GUID.Text = fMP_v06.GUID + "  " + fMP_v06.GeneralCadastralWorks.DateCadastral.ToString();

                        }

                    }
                }
            }// foreach entry
                treeView1.ExpandAll();
                AddCheckPosition(listView1, "Исходные данные", "..",  "-");
                AddCheckPosition(listView1, "Пространственные данные", "..", "-");
            if (this.fMP_v06.Appendix != null)
                AddCheckPosition(listView1, "Приложения", this.fMP_v06.Appendix.Count().ToString(),"..");
            {
                foreach (RRTypes.MP_V06.tAppendixAppliedFiles appxs in this.fMP_v06.Appendix)
                    AddCheckPosition(listView1, appxs.NameAppendix , appxs.AppliedFile.Name, "-");
            }

            
        }

        /// <summary>
        /// Check previosly unpacked archive
        /// </summary>
        /// <param name="workDir"></param>
        private void CheckIt(string workDir)
        {
            frmCertificates certfrm = new frmCertificates();

            if (Directory.Exists(workDir))
            {
                DirectoryInfo di = new DirectoryInfo(workDir);
                string ze = di.GetFiles().Select(fi => fi.Name).FirstOrDefault(name => name != "*.xml");
                // теперь загружаем xml

                if (ze.Contains(".xml") && !ze.Contains(".sig"))
                {
                    // Stream zipEntryStream = new MemoryStream();
                    // zipEntryStream = ze.InputStream;
                    TextReader reader = new StreamReader(ze);
                    this.fMP_v06_xml = new XmlDocument();
                   fMP_v06_xml.Load(reader);
                    reader.Close();
                    //Read(XMLDocFromFile);
                    string rootname = fMP_v06_xml.DocumentElement.Name;
                    string version = "-";
                    if (fMP_v06_xml.DocumentElement.Attributes.GetNamedItem("Version") != null) // Для MP версия в корне
                        version = fMP_v06_xml.DocumentElement.Attributes.GetNamedItem("Version").Value;

                    // Типы MP Версия 06 - без XSD to clasess. напрямую XSD.exe
                    if ((rootname == "MP") && (version == "06"))
                    {
                        Stream stream = new MemoryStream();
                        fMP_v06_xml.Save(stream);
                        stream.Seek(0, 0);


                        XmlSerializer serializerMP = new XmlSerializer(typeof(RRTypes.MP_V06.MP));
                        this.fMP_v06 = (RRTypes.MP_V06.MP)serializerMP.Deserialize(stream);
                        AddCheckPosition(listView1, "MP_v06 xml", fMP_v06.GUID, "OK");
                        label_doc_GUID.Text = fMP_v06.GUID + "  " + fMP_v06.GeneralCadastralWorks.DateCadastral.ToString();

                    }

                }

                if (ze.Contains(".sig"))
                    {

                        StreamReader sigreader = File.OpenText(ze);
                        List<string> sig = certfrm.ParseSignature(ze);
                        foreach (string subject in sig)
                            AddCheckPosition(listView1, "Signature", ze, subject);
                    }

                    
                }

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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ESChecker_MP06Form_SizeChanged(object sender, EventArgs e)
        {
            treeView1.Width = this.Width - 14;
            listView1.Width = this.Width - 14;
            listView1.Height = this.Height -354;
        }

    }
}
