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
    public partial class wzlBlockEd : Form
    {
        public TMyCadastralBlock ITEM;
        private DataTable data;
        private MySqlDataAdapter da;
        private MySqlCommandBuilder cb;
        public ConnectorForm CF = new ConnectorForm();
        public MySqlConnection conn;
        public TFileHistory BlockHistory;

        public wzlBlockEd()
        {
            InitializeComponent();

        }

        private void SetupControls()
        {
            textBox_CN.Text = ITEM.CN;
            textBox_BlockName.Text = ITEM.Name;
            textBox_Block_Komment.Text = ITEM.Comments;
            pkk5Viewer1.Server.mapScale = 5000; // Для квартала старт с М5000
            pkk5Viewer1.Start(ITEM.CN, pkk5_Types.Block);
            ListFiles();
            this.BlockHistory = new netFteo.Spatial.TFileHistory(ITEM.id);
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
            foreach (TFile file in ITEM.KPTXmlBodyList)
            {
                ListViewItem LV = new ListViewItem(file.Doc_Date);
                LV.Tag = file.id;
                LV.SubItems.Add(file.Number);
                LV.SubItems.Add("(" + file.Type.ToString() + ") " + file.FileName);
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

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

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
            this.ITEM.Comments = ((TextBox)sender).Text;
        }

        private void toolButton_SaveXML_Click(object sender, EventArgs e)
        {
            SaveXMLfromSelectedNode();
        }


        /// <summary>
        /// Read (SELECT) BLOB from the Database and save it in the stream (RAM)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="kpt_id"></param>
        /// <returns>Stream object</returns>
        public MemoryStream FetchKPTBody(MySqlConnection conn, long kpt_id)
        {
            if (conn == null) return null;
            if (conn.State != ConnectionState.Open) return null;

            data = new DataTable();
            da = new MySqlDataAdapter("SELECT kpt_id, xml_file_body," +
                                            " OCTET_LENGTH(xml_file_body)/1024 as xml_size_kb from kpt" +
                                            " where kpt_id = " + kpt_id.ToString(), conn);
            da.Fill(data);
            if (data.Rows.Count == 1)
            {
                DataRow row = data.Rows[0];
                if (row[1].ToString().Length > 0)
                {
                    byte[] outbyte = (byte[])row[1];
                    MemoryStream res = new MemoryStream(outbyte);
                    return res;
                }
            }
            return null;
        }

        /// <summary>
        /// Read (SELECT) BLOB from the Database and save it in the stream (RAM). KPT11 only
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="kpt_id"></param>
        /// <returns></returns>
        public MemoryStream FetchKPT11Body(MySqlConnection conn, long kpt_id)
        {
            if (conn == null) return null;
            if (conn.State != ConnectionState.Open) return null;

            data = new DataTable();
            da = new MySqlDataAdapter("SELECT kpt_id, xml_file_body," +
                                            " OCTET_LENGTH(xml_file_body)/1024 as xml_size_kb from kpt11" +
                                            " where kpt_id = " + kpt_id.ToString(), conn);
            try
            {
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
            catch (Exception ex)
            {
                object exept = ex;
                return null;
            }
        }

        private TFileHistory DB_LoadBlockHistory(MySqlConnection conn, int block_id)
        {
            TFileHistory files = new TFileHistory(block_id);
            if (conn == null) return null; if (conn.State != ConnectionState.Open) return null;
            data = new DataTable();
            da = new MySqlDataAdapter("SELECT *" +
                                      " from history where hi_item_id = " + block_id.ToString() +
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
                file.hi_item_id = "Type(" + row[2].ToString() + ").id " + row[3].ToString();
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
                netFteo.Spatial.TFile xmlFile = ITEM.KPTXmlBodyList.GetFile((long)listView1.SelectedItems[0].Tag);
                saveFileDialog1.FileName = xmlFile.FileName;
                saveFileDialog1.FilterIndex = 1;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (ITEM.KPTXmlBodyList.BodyEmpty((long)listView1.SelectedItems[0].Tag))
                        ITEM.KPTXmlBodyList.ReadFileBody((long)listView1.SelectedItems[0].Tag, FetchKPTBody(CF.conn, (long)listView1.SelectedItems[0].Tag));
                    xmlFile.XML_file_body.Save(saveFileDialog1.FileName);
                }
            }
        }


        /// <summary>
        /// Загрузка тела КПТ из BLOB поля таблицы
        /// </summary>
        /// <param name="item_id"></param>
        private void ReadXMLfromSelectedNode(long item_id)
        {
          netFteo.Rosreestr.dFileTypes item_type = ITEM.KPTXmlBodyList.GetFileType(item_id);

            if (ITEM.KPTXmlBodyList.BodyEmpty(item_id))
            {
                switch (item_type)
                {
                    case netFteo.Rosreestr.dFileTypes.KPT10:
                        {
                            ITEM.KPTXmlBodyList.ReadFileBody(item_id, FetchKPTBody(CF.conn, item_id)); break;
                        }

                    case netFteo.Rosreestr.dFileTypes.KPT11:
                        {
                            ITEM.KPTXmlBodyList.ReadFileBody(item_id, FetchKPT11Body(CF.conn, item_id)); break;
                        }

                    default:
                        {
                            ITEM.KPTXmlBodyList.ReadFileBody(item_id, FetchKPTBody(CF.conn, item_id)); break;
                        }
                }
            }
            // After all attemps to load not empty:
            if (!ITEM.KPTXmlBodyList.BodyEmpty(item_id))
            {
                XMLReaderCS.KVZU_Form frmReader = new XMLReaderCS.KVZU_Form();
                frmReader.StartPosition = FormStartPosition.Manual;
                frmReader.Tag = 3; // XMl Reader as Application part
                frmReader.DocInfo.FileName = ITEM.KPTXmlBodyList.GetFileName(item_id);
                frmReader.Read(ITEM.KPTXmlBodyList.XML_file_body(item_id));
                frmReader.Left = this.Left + 25; frmReader.Top = this.Top + 25;
                frmReader.ShowDialog(this);
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

                wzKPTProperty wzkptproperty = new wzKPTProperty(ITEM.KPTXmlBodyList.GetFile((long)listView1.SelectedItems[0].Tag));
                wzkptproperty.Top = this.Top + listView1.SelectedItems[0].Index * 20 + 180; wzkptproperty.Left = this.Left + +395 + 60;
                wzkptproperty.ShowDialog();
            }
        }

        private void снимокКартыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if (pkk5Viewer1.Image != null)
            {
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.FileName = "pkk5-shoot-" + netFteo.StringUtils.ReplaceSlash(textBox_CN.Text);
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    pkk5Viewer1.Image.Save(saveFileDialog1.FileName);
                }
            }
            */
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            //ListHistory();
        }

        private void tabPage3_MouseMove(object sender, MouseEventArgs e)
        {

        }
        /*
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            pkk5Server.GetCadastralBlockWebOnline_th(ITEM);
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            treeView_OnLine.Nodes.Clear();
            tabPage3.Text = "Он-лайн ок";
            foreach (TreeNode tn in this.pkk5Server.Nodes)
            {
                treeView_OnLine.Nodes.Add(tn);
            }
            if (pkk5Server.Image != null )
               this.pictureBox1.Image = pkk5Server.Image;
        }
        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            
        }
        private void wzlBlock_FormClosing(object sender, FormClosingEventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }
        */
        private void backgroundWorker_History_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.BlockHistory.Count == 0)
                BlockHistory = DB_LoadBlockHistory(CF.conn, ITEM.id);
        }

        private void backgroundWorker_History_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ListHistory();
        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Документы xml|*.xml";
            od.FileName = "";
            if (od.ShowDialog() == DialogResult.OK)
            {
                ImportXMLKPT(od.FileName);
            }
        }

        public void ImportXMLKPT(string FileName)
        {

            FileInfo fi = new FileInfo(FileName);
            TFile xmlUploaded = new TFile();
            xmlUploaded.FileName = fi.Name;
            xmlUploaded.xmlSize_SQL = fi.Length / 1024;
            xmlUploaded.File_BLOB = File.ReadAllBytes(FileName);
            //xmlUploaded.ReadFileBody(new MemoryStream(xmlUploaded.File_BLOB));

            //parse XMlDocument:
            netFteo.IO.FileInfo ParsedDoc = RRTypes.CommonParsers.ParserCommon.ReadXML(xmlUploaded.XML_file_body);
             
            xmlUploaded.xmlns = ParsedDoc.Namespace;
            xmlUploaded.Number = ParsedDoc.Number;
            xmlUploaded.Doc_Date = ParsedDoc.DateMySQL;// dateValue.ToString("yyyy-MM-dd");//DateTime.Now.ToString("yyyy-MM-dd");

            //wich type of KPT accquried:? 
            //KPT10
            if ((xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KPT09) || (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KPT10))
                if (DB_AddBlock_KPT(ITEM.id, xmlUploaded, CF.conn) > 0)
                {
                    ITEM.KPTXmlBodyList.Add(xmlUploaded);
                }

            //KPT11

            ListFiles();
        }

        private long DB_AddBlock_KPT(long block_id, TFile KPT, MySqlConnection conn)
        {

            if (conn == null) return -1; if (conn.State != ConnectionState.Open) return 1;
            // StatusLabel_AllMessages.Text = "Adding KPT file.... ";

            MySqlCommand cmd = new MySqlCommand(

            "INSERT INTO kpt (kpt_id, block_id, " +
                             " kpt_num,  " +
                             "kpt_date," +
                             "xml_file_name, xml_ns, xml_file_body) " +
            /*  xml_file_name,
                xml_file_body,
                pdf_file_name,
                pdf_file_body,
                zip_file_name,
                zip_file_body" + */
            "  VALUES(NULL, ?block_id, ?kpt_num, " +
            "?kpt_date," +
                           "?xml_file_name, ?xml_ns, ?xml_file_body)", conn);

            //cmd.Parameters.Add("?kpt_id", MySqlDbType.Int32).Value = item_type; - set to NULL due autoIncrement by MySQL server
            cmd.Parameters.Add("?block_id", MySqlDbType.Int32).Value = block_id;
            cmd.Parameters.Add("?kpt_num", MySqlDbType.VarChar).Value =  KPT.Number;
            cmd.Parameters.Add("?kpt_date", MySqlDbType.Date).Value = KPT.Doc_Date;
            cmd.Parameters.Add("?xml_file_name", MySqlDbType.VarChar).Value = Path.GetFileName(KPT.FileName);
            cmd.Parameters.Add("?xml_ns", MySqlDbType.VarChar).Value = KPT.xmlns;
            cmd.Parameters.Add("?xml_file_body", MySqlDbType.LongBlob).Value = KPT.File_BLOB;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string exMesssage = ex.Message;
                return -1;
            }

            long last_id = cmd.LastInsertedId;
            KPT.id = last_id;
            DBWrapper.DB_AppendHistory(ItemTypes.it_kpt, block_id, 111, "kpt++." + last_id.ToString(), conn);
            return last_id;
        }

        /// <summary>
        /// Remove kpt entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_toolStripButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                Delete_KPTEntry((long)listView1.SelectedItems[0].Tag);// ReadXMLfromSelectedNode((int)listView1.SelectedItems[0].Tag);
                listView1.Items.Remove(listView1.SelectedItems[0]);
            }
        }

        private void Delete_KPTEntry(long item_id)
        {
            //if (ITEM.KPTXmlBodyList.Exists(ITEM.KPTXmlBodyList.GetFile(item_id)))
            {
                DBWrapper.DB_EraseKPT(item_id, CF.conn);
                ITEM.KPTXmlBodyList.Remove(ITEM.KPTXmlBodyList.GetFile(item_id));
            }

        }

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
