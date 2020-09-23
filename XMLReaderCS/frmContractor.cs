using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace XMLReaderCS
{
    public partial class frmContractor : Form
    {
        XmlDocument xmldoc;
        XmlNode CadWorksNode;
        netFteo.Rosreestr.GeneralCadWorks cw;
        public frmContractor()
        {
            InitializeComponent();
        }

        private void Button_CloseReader_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolButton_Open_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "ТехПлан, Межевой план|*.xml";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                Open(openFileDialog1.FileName);
        }

        private void Open(string FileName)
        {
            if (Path.GetExtension(FileName).Equals(".xml"))
            {

                Stream xmlStream = new MemoryStream(File.ReadAllBytes(FileName));
                xmlStream.Seek(0, 0);
                XmlNode BuildNode = null;
                xmldoc = new XmlDocument();
                xmldoc.Load(xmlStream);

                if ((netFteo.XML.XMLWrapper.XMLReader_GetRoot(xmlStream) == "TP") &&
                    (netFteo.XML.XMLWrapper.XMLReader_GetRootAtrr(xmlStream, "Version") == "06"))
                {


                    BuildNode = xmldoc.DocumentElement.SelectSingleNode("Building");
                    if (BuildNode == null)
                        BuildNode = xmldoc.DocumentElement.SelectSingleNode("Construction");
                    if (BuildNode == null)
                        BuildNode = xmldoc.DocumentElement.SelectSingleNode("Uncompleted");
                    if (BuildNode == null)
                        BuildNode = xmldoc.DocumentElement.SelectSingleNode("Flat");
                    if (BuildNode == null)
                        BuildNode = xmldoc.DocumentElement.SelectSingleNode("CarParkingSpace");

                    if (BuildNode != null)
                    {
                        CadWorksNode = BuildNode.SelectSingleNode("GeneralCadastralWorks");
                        toolButton_Replace_Contractor.Enabled = true;
                        ParseGeneralCadastralWorksTP06(CadWorksNode);
                    }

                    /*
                    if (xmldoc.DocumentElement.SelectSingleNode("Construction") != null)
                    {
                        
                        ParseGeneralCadastralWorksTP06(BuildNode.SelectSingleNode("GeneralCadastralWorks"));
                    }

                    if (xmldoc.DocumentElement.SelectSingleNode("Uncompleted") != null)
                    {
                        
                        ParseGeneralCadastralWorksTP06(BuildNode.SelectSingleNode("GeneralCadastralWorks"));
                    }

                    if (xmldoc.DocumentElement.SelectSingleNode("Flat") != null)
                    {

                        ParseGeneralCadastralWorksTP06(BuildNode.SelectSingleNode("GeneralCadastralWorks"));
                    }

                    if (xmldoc.DocumentElement.SelectSingleNode("CarParkingSpace") != null)
                    {

                        ParseGeneralCadastralWorksTP06(BuildNode.SelectSingleNode("GeneralCadastralWorks"));
                    }
                    */
                }
                xmlStream.Dispose();
            }
        }

        private void ParseGeneralCadastralWorksTP06(XmlNode cadworks)
        {
            // TP / Building / GeneralCadastralWorks / @DateCadastral
            //fi.Date = cadworks.Attributes.GetNamedItem("DateCadastral").Value;

            
            //    cadworks.SelectSingleNode("Contractor/CadastralEngineerRegistryNumber").FirstChild.Value;
            textBox1.Text  = cadworks.SelectSingleNode("Contractor/FamilyName").FirstChild.Value;
            textBox4.Text  = cadworks.SelectSingleNode("Contractor/FirstName").FirstChild.Value;
            textBox6.Text  = cadworks.SelectSingleNode("Contractor/Patronymic").FirstChild.Value;
            textBox10.Text = cadworks.SelectSingleNode("Contractor/Telephone").FirstChild.Value;
            textBox14.Text = cadworks.SelectSingleNode("Contractor/Email").FirstChild.Value;
            textBox24.Text = cadworks.SelectSingleNode("Contractor/SNILS").FirstChild.Value;

            //fi.Appointment += "\n " + cadworks.SelectSingleNode("Contractor/Address").FirstChild.Value;

            //fi.Cert_Doc_Organization = "СНИЛС " + cadworks.SelectSingleNode("Contractor/SNILS").FirstChild.Value +
            //                            " Номер в реестре " + cadworks.SelectSingleNode("Contractor/CadastralEngineerRegistryNumber").FirstChild.Value + "\n" +
            //                            "СРО:" + cadworks.SelectSingleNode("Contractor/SelfRegulatoryOrganization").FirstChild.Value;

        }

        private void frmContractor_Load(object sender, EventArgs e)
        {
            textBox2.Text = XMLReaderCS.Properties.Settings.Default.FamilyName;
            textBox3.Text = XMLReaderCS.Properties.Settings.Default.FirstName;
            textBox5.Text = XMLReaderCS.Properties.Settings.Default.Patronymic;
            textBox7.Text = XMLReaderCS.Properties.Settings.Default.NCertificate;
            textBox9.Text = XMLReaderCS.Properties.Settings.Default.Telephone;
            textBox11.Text = XMLReaderCS.Properties.Settings.Default.Address;
            textBox13.Text = XMLReaderCS.Properties.Settings.Default.Email;
            textBox15.Text = XMLReaderCS.Properties.Settings.Default.Organization_Name;
            textBox17.Text = XMLReaderCS.Properties.Settings.Default.Organization_AddressOrganization;
            textBox19.Text = XMLReaderCS.Properties.Settings.Default.NameSoftware;
            textBox21.Text = XMLReaderCS.Properties.Settings.Default.VersionSoftware;
            textBox23.Text = XMLReaderCS.Properties.Settings.Default.SNILS;
            cw = new netFteo.Rosreestr.GeneralCadWorks();
            cw.Contractor.FamilyName = XMLReaderCS.Properties.Settings.Default.FamilyName; ;
            cw.Contractor.FirstName = XMLReaderCS.Properties.Settings.Default.FirstName;
            cw.Contractor.Patronymic= XMLReaderCS.Properties.Settings.Default.Patronymic;
            cw.Contractor.NCertificate = XMLReaderCS.Properties.Settings.Default.NCertificate;
            cw.Contractor.Telephone = XMLReaderCS.Properties.Settings.Default.Telephone;
            cw.Contractor.Email = XMLReaderCS.Properties.Settings.Default.Email;
            cw.Contractor.Address = XMLReaderCS.Properties.Settings.Default.Address;
            cw.Contractor.SNILS = XMLReaderCS.Properties.Settings.Default.SNILS;
        }

   
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            XMLReaderCS.Properties.Settings.Default.FamilyName = textBox2.Text;
            XMLReaderCS.Properties.Settings.Default.FirstName = textBox3.Text;
            XMLReaderCS.Properties.Settings.Default.Patronymic = textBox5.Text;
            XMLReaderCS.Properties.Settings.Default.NCertificate = textBox7.Text;
            XMLReaderCS.Properties.Settings.Default.Telephone = textBox9.Text;
            XMLReaderCS.Properties.Settings.Default.Address = textBox11.Text;
            XMLReaderCS.Properties.Settings.Default.Email = textBox13.Text;
            XMLReaderCS.Properties.Settings.Default.Organization_Name = textBox15.Text;
            XMLReaderCS.Properties.Settings.Default.Organization_AddressOrganization = textBox17.Text;
            XMLReaderCS.Properties.Settings.Default.NameSoftware = textBox19.Text;
            XMLReaderCS.Properties.Settings.Default.VersionSoftware = textBox21.Text;
            XMLReaderCS.Properties.Settings.Default.SNILS = textBox23.Text;
        }

        private void toolButton_Save_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                xmldoc.Save(saveFileDialog1.FileName);
            }
        }

        private void Replace(XmlNode cadworks)
        {
            textBox1.Text = textBox2.Text;
            cadworks.SelectSingleNode("Contractor/FamilyName").FirstChild.Value = textBox2.Text;
            textBox4.Text = textBox3.Text;
            cadworks.SelectSingleNode("Contractor/FirstName").FirstChild.Value = textBox4.Text ;
            textBox6.Text = textBox5.Text;
            cadworks.SelectSingleNode("Contractor/Patronymic").FirstChild.Value = textBox6.Text;
            textBox10.Text = textBox9.Text;
            cadworks.SelectSingleNode("Contractor/Telephone").FirstChild.Value = textBox10.Text;
            textBox14.Text = textBox13.Text;
            cadworks.SelectSingleNode("Contractor/Email").FirstChild.Value = textBox14.Text;
            textBox24.Text = textBox23.Text;
            cadworks.SelectSingleNode("Contractor/SNILS").FirstChild.Value = textBox24.Text;
        }
        
        private void toolButton_Replace_Contractor_Click(object sender, EventArgs e)
        {
            Replace(CadWorksNode);
        }
    }
}
