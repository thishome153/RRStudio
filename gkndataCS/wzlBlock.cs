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
using netFteo.Cadaster;
namespace GKNData
{
    public partial class wzlBlockEd : Form
    {
        public TCadastralBlock ITEM;
        private DataTable data;
        private MySqlDataAdapter da;
       // private MySqlCommandBuilder cb;
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
            ListFiles(ITEM);
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
        private void ListFiles(TCadastralBlock Block)
        {
            listView1.Items.Clear();
            foreach (TFile file in Block.KPTXmlBodyList)
            {
                ListViewItem LV = new ListViewItem(file.Doc_Date);
                LV.Tag = file.id;
                LV.SubItems.Add(file.Number);
                LV.SubItems.Add("(" + file.Type.ToString() + ") " + file.FileName);
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
                   //byte[] outbyte = (byte[])row[1];
                    MemoryStream res = new MemoryStream((byte[])row[1]);
                    return res;
                }
            }
            return null;
        }


        private void SaveXMLfromSelectedNode()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                TFile xmlFile = ITEM.KPTXmlBodyList.GetFile((long)listView1.SelectedItems[0].Tag);
                saveFileDialog1.FileName = xmlFile.FileName;
                saveFileDialog1.FilterIndex = 1;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (ITEM.KPTXmlBodyList.BodyEmpty((long)listView1.SelectedItems[0].Tag)) //fetch from DB file body, when empty
                        ITEM.KPTXmlBodyList.ReadFileBody((long)listView1.SelectedItems[0].Tag, DBWrapper.FetchKPTBodyToArray(CF.conn, (long)listView1.SelectedItems[0].Tag));
                    File.WriteAllBytes(saveFileDialog1.FileName, xmlFile.File_BLOB);
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
                            ITEM.KPTXmlBodyList.ReadFileBody(item_id, DBWrapper.FetchKPTBodyToArray(CF.conn, item_id)); break;
                        }

                    case netFteo.Rosreestr.dFileTypes.KPT11:
                        {
                            ITEM.KPTXmlBodyList.ReadFileBody(item_id, DBWrapper.FetchKPT11Body(CF.conn, item_id)); 
                            break;
                        }

                    default:
                        {
                            ITEM.KPTXmlBodyList.ReadFileBody(item_id, DBWrapper.FetchKPTBodyToArray(CF.conn, item_id)); break;
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
                frmReader.DocInfo.FileSize = ITEM.KPTXmlBodyList.GetFileSize(item_id);
                frmReader.Read(ITEM.KPTXmlBodyList.File_stream(item_id));
                frmReader.Left = this.Left + 25; frmReader.Top = this.Top + 25;
                frmReader.ShowDialog(this);
            }
        }

        private void ShowProperties()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                wzKPTProperty wzkptproperty = new wzKPTProperty(ITEM.KPTXmlBodyList.GetFile((long)listView1.SelectedItems[0].Tag));
                wzkptproperty.Top = this.Top + listView1.SelectedItems[0].Index * 20 + 180; wzkptproperty.Left = this.Left + +395 + 60;
                wzkptproperty.ShowDialog();
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
            ShowProperties();
            /*
            if (listView1.SelectedItems.Count == 1)
            {

                wzKPTProperty wzkptproperty = new wzKPTProperty(ITEM.KPTXmlBodyList.GetFile((long)listView1.SelectedItems[0].Tag));
                wzkptproperty.Top = this.Top + listView1.SelectedItems[0].Index * 20 + 180; wzkptproperty.Left = this.Left + +395 + 60;
                wzkptproperty.ShowDialog();
            }
            */
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
                BlockHistory = DBWrapper.LoadBlockHistory(CF.conn, ITEM.id);
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
                ImportXMLKPT(od.FileName, ITEM, this.CF.conn);
            }
        }

        /// <summary>
        /// Add file record into database,
        /// next into file collection of block
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="TargetBlock"></param>
        /// <param name="conn"></param>
        public void ImportXMLKPT(string FileName, TCadastralBlock TargetBlock, MySqlConnection conn)
        {
            FileInfo fi = new FileInfo(FileName);
            TFile xmlUploaded = new TFile();
            xmlUploaded.FileName = fi.Name;
            xmlUploaded.xmlSize_SQL = fi.Length / 1024;
            xmlUploaded.File_BLOB = File.ReadAllBytes(FileName);
            //parse XMlDocument as stream:
            netFteo.IO.FileInfo ParsedDoc = RRTypes.CommonParsers.ParserCommon.ParseXMLDocument(new MemoryStream(xmlUploaded.File_BLOB));

            GC.Collect();

            xmlUploaded.xmlns = ParsedDoc.Namespace;
            xmlUploaded.Number = ParsedDoc.Number;
            xmlUploaded.Doc_Date = ParsedDoc.DateMySQL;


            //wich type of KPT accquried:? 
            if (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KPT11)
            {
                if (DBWrapper.DB_AddBlock_KPT11(TargetBlock.id, xmlUploaded, conn) > 0)             //KPT11
                {
                    TargetBlock.KPTXmlBodyList.Add(xmlUploaded);
                    ListFiles(TargetBlock);
                }
                else
                    MessageBox.Show(DBWrapper.LastErrorMsg, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }

            //All known types, except KPT11
            if ((xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KPT05) ||
                (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KPT06) ||
                (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KPT07) ||
                (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KPT08) ||
                (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KPT09) ||
                (xmlUploaded.Type == netFteo.Rosreestr.dFileTypes.KPT10))

                //TODO: check if file with same filename already in DB: ?
                // ??

                if (DBWrapper.DB_AddBlock_KPT(TargetBlock.id, xmlUploaded, conn) > 0)
                {
                    TargetBlock.KPTXmlBodyList.Add(xmlUploaded);
                    ListFiles(TargetBlock);
                }
                else
                    MessageBox.Show(DBWrapper.LastErrorMsg, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Question);

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
                string message = "Удалить запись " + listView1.SelectedItems[0].SubItems[1].Text;
                if (MessageBox.Show(message, "Подтвердите",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question) == DialogResult.Yes)

                {
                    if (DBWrapper.EraseKPT((long)listView1.SelectedItems[0].Tag, CF.conn))// ReadXMLfromSelectedNode((int)listView1.SelectedItems[0].Tag);
                    {
                        if (ITEM.KPTXmlBodyList.Remove(ITEM.KPTXmlBodyList.GetFile((long)listView1.SelectedItems[0].Tag)))
                            listView1.Items.Remove(listView1.SelectedItems[0]);
                    }
                    else
                        MessageBox.Show(DBWrapper.LastErrorMsg, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Question);

                }
            }
        }

        private void СохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveXMLfromSelectedNode();
        }

        private void СвойстваToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowProperties();
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
