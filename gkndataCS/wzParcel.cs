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
using netFteo.Spatial;
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
            textBox_Block_Comment.Text = ITEM.SpecialNote;
            ListFiles();
            pkk5Viewer1.Start(ITEM.CN, pkk5_Types.Parcel);
            this.BlockHistory = new TFileHistory(ITEM.id);
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
            foreach (TFile file in ITEM.XmlBodyList)
            {
                ListViewItem LV = new ListViewItem(file.Doc_Date);
                LV.Tag = file.id;
                LV.SubItems.Add(file.Number);
                LV.SubItems.Add("(" + file.Type.ToString() + ") " + file.FileName);
                //LV.SubItems.Add(file.FileName);
                LV.SubItems.Add(file.xmlSize_SQL.ToString("0"));
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
           this.ITEM.SpecialNote = ((TextBox)sender).Text;
        }

        private void toolButton_SaveXML_Click(object sender, EventArgs e)
        {
            SaveXMLfromSelectedNode();
        }

        // **** 
      
        /* TODO KIll
        private TFileHistory LoadParcelHistory(MySqlConnection conn, long item_id)
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
        */
        private void SaveXMLfromSelectedNode()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                netFteo.Spatial.TFile xmlFile = ITEM.XmlBodyList.GetFile((long)listView1.SelectedItems[0].Tag);
                saveFileDialog1.FileName = xmlFile.FileName;
                saveFileDialog1.FilterIndex = 1;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (ITEM.XmlBodyList.BodyEmpty((int)listView1.SelectedItems[0].Tag))
                        ITEM.XmlBodyList.ReadFileBody((int)listView1.SelectedItems[0].Tag, DBWrapper.FetchVidimusBody(CF.conn, (long)listView1.SelectedItems[0].Tag));
                    //xmlFile.File_Stream ===>   .Save(saveFileDialog1.FileName);
                }
            }
        }

        private void ReadXMLfromSelectedNode(long item_id)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                if (ITEM.XmlBodyList.BodyEmpty(item_id))
                    ITEM.XmlBodyList.ReadFileBody(item_id, DBWrapper.FetchVidimusBody(CF.conn, item_id));
               // System.Xml.XmlDocument body = ITEM.XmlBodyList.XML_file_body(item_id);
                if (!ITEM.XmlBodyList.BodyEmpty(item_id))
                {
                    XMLReaderCS.KVZU_Form frmReader = new XMLReaderCS.KVZU_Form();
                    frmReader.StartPosition = FormStartPosition.Manual;
                    frmReader.Tag = 3; // XMl Reader as application part
                    frmReader.DocInfo.FileName = ITEM.XmlBodyList.GetFileName(item_id);
                    frmReader.Read(null, ITEM.XmlBodyList.File_stream (item_id));
                    frmReader.Left = this.Left + 25; frmReader.Top = this.Top + 25;
                    frmReader.ShowDialog();
                }
            }
        }

        private void toolButton_ReadXML_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ReadXMLfromSelectedNode((long)listView1.SelectedItems[0].Tag);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ReadXMLfromSelectedNode((long)listView1.SelectedItems[0].Tag);
            }
        }

        private void toolButton_PropertyXML_Click(object sender, EventArgs e)
        {
             
            if (listView1.SelectedItems.Count == 1)
            {
                
            wzKPTProperty wzkptproperty = new wzKPTProperty(ITEM.XmlBodyList.GetFile((long)listView1.SelectedItems[0].Tag));
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
                BlockHistory = DBWrapper.LoadBlockHistory(CF.conn, ITEM.id);
        }

        private void backgroundWorker_History_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ListHistory();
        }

        private void wzParcelfrm_Shown(object sender, EventArgs e)
        {
            SetupControls();
        }

        private void Button2_Click(object sender, EventArgs e)
        {

        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Документы xml|*.xml";
            od.FileName = "";
            if (od.ShowDialog() == DialogResult.OK)
            {
                ImportXMLVidimus(od.FileName);
            }
        }


        public void ImportXMLVidimus(string FileName)
        {

            FileInfo fi = new FileInfo(FileName);
            TFile xmlUploaded = new TFile();
            xmlUploaded.FileName = fi.Name;
            xmlUploaded.xmlSize_SQL = fi.Length / 1024;
            xmlUploaded.File_BLOB = File.ReadAllBytes(FileName);

            //parse XMlDocument:
            netFteo.IO.FileInfo ParsedDoc = RRTypes.CommonParsers.ParserCommon.ParseXMLDocument(null, xmlUploaded.File_Stream);

            xmlUploaded.xmlns = ParsedDoc.Namespace;
            xmlUploaded.Number = ParsedDoc.Number;
            xmlUploaded.RootName = ParsedDoc.DocRootName;
            xmlUploaded.Doc_Date = ParsedDoc.DateMySQL;// dateValue.ToString("yyyy-MM-dd");//DateTime.Now.ToString("yyyy-MM-dd");


            //wich type of XML accquried:? 
            if ((xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KVZU_07) ||
                (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KVZU_06) ||
                (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KVZU_05) ||
                (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KVZU_04) ||
                (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KPZU_05)  ||
                (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KPZU_06) ||
                (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KVOKS_07) ||
                (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KPOKS_04) ||
                (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.EGRP_04) ||
                (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.EGRP_06) 
                    )

            {
                if (DBWrapper.DB_AddParcel_Vidimus(ITEM.id, xmlUploaded, CF.conn) > 0)             //KPT11
                {
                    ITEM.XmlBodyList.Add(xmlUploaded);
                    ListFiles();
                }
            }

        
        }

        private void ToolStripButton2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                string message = "Удалить запись " + listView1.SelectedItems[0].SubItems[1].Text;
                const string caption = "Подтвердите";
               if ( MessageBox.Show(message, caption,
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question) == DialogResult.Yes)

                if (listView1.SelectedItems.Count == 1)
                {
                    if (Delete_Entry((long)listView1.SelectedItems[0].Tag))// ReadXMLfromSelectedNode((int)listView1.SelectedItems[0].Tag);
                    listView1.Items.Remove(listView1.SelectedItems[0]);
                }
            }
        }

        private bool Delete_Entry(long item_id)
        {
            //if (ITEM.KPTXmlBodyList.Exists(ITEM.KPTXmlBodyList.GetFile(item_id)))
            {
                if (DBWrapper.EraseVidimus(item_id, CF.conn))
                {
                  return  ITEM.XmlBodyList.Remove(ITEM.XmlBodyList.GetFile(item_id));
                }
            }
            return false;
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
