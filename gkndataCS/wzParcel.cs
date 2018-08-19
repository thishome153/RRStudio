using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;

using MySql.Data.MySqlClient;

using System.Threading;
using System.Net; // http???
using System.IO;
using System.Web;

using RRTypes.pkk5;
using netFteo.BaseClasess;
namespace GKNData
{
    public partial class wzParcelfrm : Form
    {
        public TMyParcel ITEM;
        private DataTable data;
        private MySqlDataAdapter da;
        private MySqlCommandBuilder cb;
        public ConnectorForm CF = new ConnectorForm();
        public MySqlConnection conn;
        public TFileHistory BlockHistory;
       // public pkk5_Rosreestr_ru pkk5Server;
       //public Thread BackGroundProcess;// = new Thread(BackGroundWorker.DoWork);
       //Thread demoThread;
        public wzParcelfrm()
        {
            InitializeComponent();
           
        }

        private void SetupControls()
        {
            textBox_CN.Text = ITEM.CN;
            textBox_BlockName.Text = ITEM.Name;
            ListFiles();
            pkk5Viewer1.Start(ITEM.CN, pkk5_Types.Parcel);
            this.BlockHistory = new netFteo.BaseClasess.TFileHistory(ITEM.id);
            if (!backgroundWorker_History.IsBusy)
                backgroundWorker_History.RunWorkerAsync();
        }

        private void ListHistory()
        {
            listView_History.Items.Clear();
           
                
            foreach (TFileHistoryItem file in BlockHistory)
            {
                ListViewItem LV = new ListViewItem(file.hi_data);
                LV.Tag = file.id;
                LV.SubItems.Add(file.hi_comment);
                LV.SubItems.Add(file.hi_item_id); LV.SubItems.Add(file.hi_ip);
                LV.SubItems.Add(file.hi_host);
                LV.SubItems.Add(file.hi_systemusername); LV.SubItems.Add(file.hi_dbusername);
                listView_History.Items.Add(LV);
            }
        }
        // Отображение файлов в listview- КПТ
        private void ListFiles()
        {
            listView1.Items.Clear();
            foreach (netFteo.BaseClasess.TFile file in ITEM.XmlBodyList)
            {
                ListViewItem LV = new ListViewItem(file.Doc_Date);
                LV.Tag = file.id;
                LV.SubItems.Add(file.Number);
                LV.SubItems.Add(file.FileName);
                LV.SubItems.Add(file.xmlSize_SQL.ToString());
                LV.SubItems.Add(file.RequestNum); //Комментарий/ Номер запроса
                listView1.Items.Add(LV);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
          
        }
        //Вход на заколадку On-Line: threading parallesl??
        private void tabPage3_Enter(object sender, EventArgs e)
        {
    
        }

        

        private void wzlBlock_Shown(object sender, EventArgs e)
        {
            SetupControls();
        }

      

        //Просмотр истории:
        private void tabControl1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox_CN_TextChanged(object sender, EventArgs e)
        {
            this.ITEM.CN = ((TextBox)sender).Text;
        }
        private void textBox_BlockName_TextChanged(object sender, EventArgs e)
        {
            this.ITEM.Name = ((TextBox)sender).Text;
        }
        private void textBox_Block_Komment_TextChanged(object sender, EventArgs e)
        {
           // this.ITEM.Comments = ((TextBox)sender).Text;
        }

        private void toolButton_SaveXML_Click(object sender, EventArgs e)
        {
            SaveXMLfromSelectedNode();
        }

        // **** Read BLOB from the Database and save it on the Filesystem
        public MemoryStream GetVidimusBody(MySqlConnection conn, int vidimus_id)
        {
            if (conn == null) return null; if (conn.State != ConnectionState.Open) return null;

            data = new DataTable();
            da = new MySqlDataAdapter("SELECT vidimus_id, xml_file_body," +
                                            " OCTET_LENGTH(xml_file_body)/1024 as xml_size_kb from vidimus "  +
                                            " where vidimus_id  = " + vidimus_id.ToString(), conn);

            da.Fill(data);

            if (data.Rows.Count == 1)
            {
                DataRow row = data.Rows[0];
                byte[] outbyte = (byte[])row[1];
                MemoryStream res = new MemoryStream(outbyte);
                return res;
            }
            else return null;
        }
        private TFileHistory LoadParcelHistory(MySqlConnection conn, int item_id)
        {
            TFileHistory files = new TFileHistory(item_id);
            if (conn == null) return null; if (conn.State != ConnectionState.Open) return null;
            data = new DataTable();
            da = new MySqlDataAdapter("SELECT *" +
                                      " from history where hi_item_id = " + item_id.ToString() +
                                      " order by history_id asc", conn);

            da.Fill(data);
            foreach (DataRow row in data.Rows)
            {
                TFileHistoryItem file = new TFileHistoryItem(Convert.ToInt32(row[0])); //id
                /*
                 1`hi_disrtict_id`,
                 2`hi_item_type`, 
                 3`hi_item_id`, 
                 4`hi_data`, 
                 5`hi_status_id`, 
                 6`hi_rid_id`, 
                 7 `hi_comment`, `hi_host`, `hi_ip`, 
                 10 `hi_systemusername`, `hi_dbusername
                 * */
                file.hi_item_id = "Type("+row[2].ToString()+").id "+row[3].ToString();
                file.hi_data = Convert.ToString(row[4]).Substring(0, Convert.ToString(row[4]).Length - 7);
                // срезать семь нулей времени MySQL "05.04.2016 0:00:00"
                file.hi_comment = row[7].ToString();
                file.hi_host = row[8].ToString();
                file.hi_ip = row[9].ToString();
                file.hi_systemusername = row[10].ToString();
                file.hi_dbusername = row[11].ToString();
                files.Add(file);
            }
            return files;
        }
        private void SaveXMLfromSelectedNode()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                netFteo.BaseClasess.TFile xmlFile = ITEM.XmlBodyList.GetFile((int)listView1.SelectedItems[0].Tag);
                saveFileDialog1.FileName = xmlFile.FileName;
                saveFileDialog1.FilterIndex = 1;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (ITEM.XmlBodyList.BodyEmpty((int)listView1.SelectedItems[0].Tag))
                        ITEM.XmlBodyList.UploadFileBody((int)listView1.SelectedItems[0].Tag, GetVidimusBody(CF.conn, (int)listView1.SelectedItems[0].Tag));
                    xmlFile.xml_file_body.Save(saveFileDialog1.FileName);
                }
            }
        }

        private void ReadXMLfromSelectedNode()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                if (ITEM.XmlBodyList.BodyEmpty((int)listView1.SelectedItems[0].Tag))
                    ITEM.XmlBodyList.UploadFileBody((int)listView1.SelectedItems[0].Tag, GetVidimusBody(CF.conn, (int)listView1.SelectedItems[0].Tag));
                XMLReaderCS.KVZU_Form frmReader = new XMLReaderCS.KVZU_Form();
                frmReader.StartPosition = FormStartPosition.Manual;
                frmReader.Tag = 3; // XMl Reader в составе приложения
                frmReader.Read(ITEM.XmlBodyList.GetBody((int)listView1.SelectedItems[0].Tag));
                frmReader.Left = this.Left + 25; frmReader.Top = this.Top + 25;
                frmReader.ShowDialog();
            }
        }
        private void toolButton_ReadXML_Click(object sender, EventArgs e)
        {
            ReadXMLfromSelectedNode(); 
        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ReadXMLfromSelectedNode(); 
        }

        private void toolButton_PropertyXML_Click(object sender, EventArgs e)
        {
             
            if (listView1.SelectedItems.Count == 1)
            {
                
            wzKPTProperty wzkptproperty = new wzKPTProperty(ITEM.XmlBodyList.GetFile((int)listView1.SelectedItems[0].Tag));
            wzkptproperty.Top = this.Top + listView1.SelectedItems[0].Index*20 + 180; wzkptproperty.Left = this.Left + +395 + 60;
            wzkptproperty.ShowDialog();
             }
        }

        private void снимокКартыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pkk5Viewer1.Image != null)
            {
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.FileName = "pkk5-shoot-" + netFteo.StringUtils.ReplaceSlash(textBox_CN.Text);
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    pkk5Viewer1.Image.Save(saveFileDialog1.FileName);
                }
            }

        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            //ListHistory();
        }

        private void tabPage3_MouseMove(object sender, MouseEventArgs e)
        {
          
        }

        


      


        private void wzlBlock_FormClosing(object sender, FormClosingEventArgs e)
        {
            //backgroundWorker1.CancelAsync();
        }

        private void backgroundWorker_History_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.BlockHistory.Count == 0)
                BlockHistory = LoadParcelHistory(CF.conn, ITEM.id);
        }

        private void backgroundWorker_History_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ListHistory();
        }

        private void wzParcelfrm_Shown(object sender, EventArgs e)
        {
            SetupControls();
        }

       

       

      
     /* 
      
        private void label_MapScale_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                label_MapScale.Text = "    M 1:" + pkk5Viewer1.Server.mapScale.ToString() +"      .... ";
                tabPage3.Text = "Он-лайн.........";
                backgroundWorker1.RunWorkerAsync();
            }
        }


        */


        


     
       
   
        
       
        
    }
    // Threading issues:
    /*
    public class Worker
    {
        private volatile bool _sholdStop;
        public string WorkString;
        public bool Tick;
        public void DoWork()
        {

                    WebRequest wrGETURL;
                    //Запрос кварталов по кадастровому номеру, возвращает массив (сокращенные атрибуты):
                    wrGETURL = WebRequest.Create("http://pkk5.rosreeestr.ru/api/features/1");
                    wrGETURL.Proxy = WebProxy.GetDefaultProxy();
                    wrGETURL.Timeout = 500;
                    Stream objStream;
                    WebResponse wr = wrGETURL.GetResponse();
                    objStream = wr.GetResponseStream();
                    if (objStream != null)
                        this.WorkString = objStream.ToString();
                    else this.WorkString = "server fail ";

        }
        public void DoWork1()
        {
            if (this.Tick)
            {
                this.WorkString = "tick";
                this.Tick = false;
            }
            else
            {
                this.WorkString = "-";
                this.Tick = true;
            };

        }
        public void RequestStop()
        {
            this._sholdStop = true;
        }
    }
    */
}
