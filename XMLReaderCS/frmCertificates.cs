using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Security.Permissions;
using System.Security.Cryptography;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;


   



namespace XMLReaderCS
{
    public partial class frmCertificates : Form
    {
         DataTable datatable = new DataTable();
         

        public frmCertificates()
        {
              this.DoubleBuffered = true;
           // dataGridView1.DataSource = datatable;
           // SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();

        }

        private X509Certificate2 UpdateCertificateInfo(string serial)
        {
            listView_Details.Items.Clear();
            datatable.Rows.Clear();
            toolStripButton1.Enabled = false; toolStripButton3.Enabled = false;
            X509Certificate2 x509 = netFteo.Crypto.Wrapper.GetCertBySerial(serial);
            if (x509 != null)
            {
                /*
                datatable.Rows.Add(x509.GetNameInfo(X509NameType.SimpleName, false),
                                   x509.GetNameInfo(X509NameType.SimpleName, true), x509.GetExpirationDateString());
                */
                ListViewItem sub = new ListViewItem("Субьект");
                sub.SubItems.Add(x509.GetNameInfo(X509NameType.SimpleName, false));
                listView_Details.Items.Add(sub);
                ListViewItem subIS = new ListViewItem("Издатель");
                subIS.SubItems.Add(x509.GetNameInfo(X509NameType.SimpleName, true));
                listView_Details.Items.Add(subIS);
                ListViewItem subExp = new ListViewItem("Срок действия");
                subExp.SubItems.Add(x509.GetExpirationDateString());
                listView_Details.Items.Add(subExp);


                ListViewItem subExp2 = new ListViewItem("Алгоритм");
                subExp2.SubItems.Add(x509.SignatureAlgorithm.FriendlyName);
                listView_Details.Items.Add(subExp2);

                ListViewItem subExp22 = new ListViewItem("Сер. номер");
                subExp22.SubItems.Add(serial);
                listView_Details.Items.Add(subExp22);

                //szOID_RSA_MD5RSA   "1.2.840.113549.1.1.4"
                if ((x509.SignatureAlgorithm.Value == "1.2.840.113549.1.1.4")
                     || (x509.SignatureAlgorithm.Value == "1.2.840.113549.1.1.5"))
                {
                    RSACryptoServiceProvider prov = (RSACryptoServiceProvider)x509.PublicKey.Key;
                    CspKeyContainerInfo cinfo = (CspKeyContainerInfo) prov.CspKeyContainerInfo;

                    ListViewItem subExp21 = new ListViewItem("CSP");
                    subExp21.SubItems.Add(cinfo.ProviderName);
                    listView_Details.Items.Add(subExp21);

                }



                //Если это ГОСТ (Крипто про):
                /*
                if (x509.SignatureAlgorithm.Value == "1.2.643.2.2.3")
                    try
                    {
                        CAdESCOM.CPCertificate GOST_Cert = netFteo.IO.CadesWrapper.FindBySerial(serial);
                        if (GOST_Cert != null)
                            if (GOST_Cert.HasPrivateKey())
                            {
                                ListViewItem subExp21 = new ListViewItem("CSP");
                                subExp21.SubItems.Add(GOST_Cert.PrivateKey.ProviderName);
                                listView1.Items.Add(subExp21);

                                ListViewItem subExp23 = new ListViewItem("Контейнер");
                                subExp23.SubItems.Add(GOST_Cert.PrivateKey.ContainerName);
                                listView1.Items.Add(subExp23);

                            }
                    }
                    catch (System.Runtime.InteropServices.COMException  ex)
                    {
                        ListViewItem subExp23 = new ListViewItem("Ошибка CSP "+ ex.Source);
                        subExp23.SubItems.Add(ex.Message);
                        listView1.Items.Add(subExp23);
                    }
                */
                toolStripButton1.Enabled = true; toolStripButton3.Enabled = true;
                return x509;
            }
            else return null;   
        }



        private void SignFile(string SubjectCNName)
        {
            /*
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog(this) == DialogResult.OK)
                netFteo.IO.CadesWrapper.SignFile(fd.FileName, SubjectCNName);
            */
        }

        private void SignFile_cspUtils(string SubjectCNName)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog(this) == DialogResult.OK)
            { /*
                cspUtils.CadesWrapper cswr = new cspUtils.CadesWrapper();
                cswr.SignFile(fd.FileName, SubjectCNName);
                */
            }
        }



        private void frmCertificates_Load(object sender, EventArgs e)
        {
            //CADESCOM interop NET:  
            List<X509Certificate2> certs = netFteo.Crypto.Wrapper.DisplayCerts("my");

            //cpp api wrapper:
            //ucrtbased.dll missing
            //cspUtils.CadesWrapper cwr = new cspUtils.CadesWrapper();
            
            /*
            DataColumn dc1 = new DataColumn("SubjectCN", typeof(string));
            DataColumn dc2 = new DataColumn("Provider", typeof(string));
            DataColumn dc3 = new DataColumn("Expaired_Date", typeof(string));
            datatable.Columns.Add(dc1);
            datatable.Columns.Add(dc2);
            datatable.Columns.Add(dc3);
            datatable.Rows.Add("-", "-", "-");
            */
            listView_certs.Items.Clear();
            listView_Details.Items.Clear();
            foreach (X509Certificate2 cert in certs)
            {
                ListViewItem certItem = new ListViewItem(cert.GetNameInfo(X509NameType.SimpleName, false));
                certItem.Tag = cert.GetSerialNumberString();
                listView_certs.Items.Add(certItem);
            }
            if (listView_certs.Items.Count > 0)
            {
                listView_certs.Items[0].Focused = true;
                if (listView_certs.FocusedItem != null)
                    UpdateCertificateInfo(listView_certs.FocusedItem.Tag.ToString());
            }
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
        }


        private void listBox1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SignFile(listView_certs.SelectedItems[0].Text);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SignFile_cspUtils(listView_certs.SelectedItems[0].Text);
        }


        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog(this) == DialogResult.OK)
            {  /*
                cspUtils.CadesWrapper cwrp = new cspUtils.CadesWrapper();
                cwrp.DisplaySig(fd.FileName, this.Handle);
                */
            }
            
        }

        public List<string> ParseSignature(string filename)
        {
            List<string> res = new List<string>();
           // if (Path.GetExtension(filename).Equals(".xml"))
            {
                using (Stream datastream = File.OpenRead(filename))
                {
                    if (File.Exists(filename))// + ".sig"))
                        using (Stream sigdatastream = File.OpenRead(filename))// + ".sig"))
                        {
                            // verifySig:
                            var test = sigdatastream;
                            byte[] sigAsArray = new byte[sigdatastream.Length];
                            int sz = (int)sigdatastream.Length;
                            sigdatastream.Read(sigAsArray, 0, sz);
                            List<string> certs = netFteo.Crypto.Wrapper.DisplayCerts(sigAsArray);
                            if (certs != null)
                            {
                                foreach (string SubjectName in certs)
                                {
                                   // listView_Contractors.Items.Add("Подпись").SubItems.Add(SubjectName);

                                    res.Add(SubjectName);
                                }
                            }
                        }
                }
            }
            return res;
        }

        private void listView_certs_MouseClick(object sender, MouseEventArgs e)
        {
            if (listView_certs.FocusedItem != null)
            UpdateCertificateInfo(listView_certs.FocusedItem.Tag.ToString());
        }

        private void listView_certs_KeyUp(object sender, KeyEventArgs e)
        {
            if (listView_certs.FocusedItem != null)
                UpdateCertificateInfo(listView_certs.FocusedItem.Tag.ToString());
        }

        private void listView_certs_DoubleClick(object sender, EventArgs e)
        
        {
            if (listView_certs.FocusedItem != null)
            {
                X509Certificate2 x509 = netFteo.Crypto.Wrapper.GetCertBySerial(listView_certs.FocusedItem.Tag.ToString());
                if (x509 != null)
                {
                    X509Certificate2UI.DisplayCertificate(x509);

                }
            }
        }
    }
}
