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
//using namespace System.Xml;
using MySql.Data.MySqlClient;
using Ionic.Zip;
using netFteo.Windows;
using netFteo.Spatial;
namespace GKNData

{
    public partial class MainGKNForm : Form
    {

        private DataTable data;
        private MySqlDataAdapter da;
        private MySqlCommandBuilder cb;
        private int TIMEOUT_DONE;
        private Timer TimeOutTimer; // iddle timer
        ConnectorForm CF = new ConnectorForm();
        public TMyBlockCollection CadBloksList; // Глобальный перечень кварталов
        ZipFile zip;

        /// <summary>
        /// Common application unzipping work folder
        /// </summary>
       // string Folder_Unzip;
        
        /// <summary>
        /// Current archive folder
        /// </summary>
        string Archive_Folder; 



        /// <summary>
        /// List of files, dragged on to form
        /// </summary>
        string[] DraggedFiles;


        public MainGKNForm()
        {
            InitializeComponent();
            treeView1.BeforeExpand += OnItemexpanding; //Подключаем обработчик раскрытия
            Application.Idle += On_Iddle;
            treeView1.Nodes.Clear();
            /*
            MRG.Controls.UI.LoadingCircleToolStripMenuItem trobbler = new MRG.Controls.UI.LoadingCircleToolStripMenuItem();
            trobbler.Visible = true;
            toolStrip1.Items.Add(trobbler);
            */
#if DEBUG
            this.Text = "ГКН Дата. debug version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Button_Import.Enabled = true;
            Button_Exit.Visible = true;
#else
            this.Text = "ГКН Дата " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Button_Import.Enabled = false;
            //toolStripButton_Exit.Visible = false;
#endif
        }

        #region Функции

        private void On_Iddle(Object sender, EventArgs e)
        {

            StatusLabel_DBName.Image = null;
            StatusLabel_DBName.ToolTipText = "-";
            StatusLabel_SubRf_CN.Text = "-";
            StatusLabel_SubRf_CN.ToolTipText = "-";
            //		StatusLabel_AllMessages.Text = "-";


            if (CF.conn != null)
            {
                StatusLabel_DBName.Text = CF.conn.DataSource + ":/" + CF.conn.Database;
                if (CF.conn.State == ConnectionState.Open)
                {
                    StatusLabel_DBName.ToolTipText = "Соединено";//CF.conn.State.ToString();
                    StatusLabel_DBName.Image = GKNData.Properties.Resources.accept;
                    StatusLabel_SubRf_CN.Text = CF.Cfg.SubRF_KN + ":" + CF.Cfg.District_KN;
                    StatusLabel_SubRf_CN.ToolTipText = CF.Cfg.SubRF_Name + ":" + CF.Cfg.District_Name;
                    toolStripProgressBar1.ToolTipText = "Всего " + CF.Cfg.BlockCount.ToString() + " записей";
                    StatusLabel_CurrentItem.ToolTipText = "Всего " + CF.Cfg.BlockCount.ToString() + " записей";
                }
                else
                {
                    StatusLabel_DBName.Image = GKNData.Properties.Resources.cross;
                    StatusLabel_DBName.ToolTipText = "Отключено";
                    StatusLabel_AllMessages.Text = "Отключено";
                    StatusLabel_CurrentItem.Text = "-";
                }
            }
        }


        private void TimeOut_TimerTick(object sender, EventArgs e)
        {
            //	throw new NotImplementedException();
            var last = netFteo.Runtime.UserInput.GetLastInputTime();
            var ms = TIMEOUT_DONE - last;

            //StatusLabel_AllMessages.Text = "wait... " + (ms / 1000) + " last " + (last);
            if (last < 1000) toolStripProgressBar1.Value = toolStripProgressBar1.Maximum; // reset bar

            if (ms < 0)
            {
                this.Close();
            }

            else if (ms < 5000)
            {
                GoDisconnect();
            }
            else
            {
                if (toolStripProgressBar1.Value > 0)
                    toolStripProgressBar1.Value--;
            }
        }


        /// <summary>
        /// Операции при загрузке/перезагрузке районов/кварталов
        /// </summary>
        /// <param name="tv">Target TreeView for works</param>
        private void ConnectOps(TreeView tv)
        {
            tv.BackColor = SystemColors.Control;
            loadingCircleToolStripMenuItem1.LoadingCircleControl.Color = SystemColors.Highlight;
            loadingCircleToolStripMenuItem1.Visible = true;
            loadingCircleToolStripMenuItem1.LoadingCircleControl.Active = true;
            tv.Nodes.Clear();
            CadBloksList = LoadBlockList(CF.conn, CF.conn2, CF.Cfg.District_id);
            Application.DoEvents();
            CF.Cfg.BlockCount = CadBloksList.Blocks.Count();
            ListBlockListTreeView(CadBloksList, tv);
            AppendHistory(CF.conn, CF.Cfg);
            loadingCircleToolStripMenuItem1.LoadingCircleControl.Active = false;
            loadingCircleToolStripMenuItem1.LoadingCircleControl.Visible = false;
#if !DEBUG
			TimeOutTimer.Start(); // start timeout timer for distrubuted release
#endif
            netFteo.Runtime.ScreenWriter screen = new netFteo.Runtime.ScreenWriter();
            screen.CaptureScreenToFile("Screenshot.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private bool ConnectGo()
        {
            CF.Cfg.CfgRead(); // Read reg

            TIMEOUT_DONE = Convert.ToInt16(CF.Cfg.IddleTimeOut);
            {
                if (CF.conn != null)
                    CF.conn.Close();
                //SSL Mode=Required
                string connStr = String.Format("server={0};user id={1}; password={2}; database={3}; pooling=false;SSL Mode=None",
                    CF.Cfg.ServerName, CF.Cfg.UserName, CF.Cfg.UserPwrd, CF.Cfg.DatabaseName);

                try
                {

                    CF.conn = new MySqlConnection(connStr);
                    CF.conn.Open();
                    CF.conn2 = new MySqlConnection(connStr);
                    CF.conn2.Open();

                    if (CF.conn.State == ConnectionState.Open)
                    {
                        ConnectOps(treeView1);
                        return true;
                    }
                    else
                    {
                        loadingCircleToolStripMenuItem1.LoadingCircleControl.Active = false;
                        loadingCircleToolStripMenuItem1.LoadingCircleControl.Color = Color.Red;
                        this.treeView1.BackColor = Color.Red;
                    }
                }
                catch (MySqlException ex)
                {

                    //MessageBox.Show("Error connecting to the server: " + ex.Message);
                    /*
					{ "You have an error in your SQL syntax; check the manual that corresponds to your MySQL server version for the right syntax to " +
							"use near '', '-1','C# GKNDATA connect',\t 'developer_machine', '10.66.77.400', 'this', 'u1'' at line 1"}
					*/
                    StatusLabel_DBName.ToolTipText = "Connect error " + CF.conn.Database;
                    StatusLabel_DBName.Image = GKNData.Properties.Resources.cross;
                    StatusLabel_AllMessages.Text = ex.Message;
                    this.treeView1.BackColor = Color.DarkRed;
                    loadingCircleToolStripMenuItem1.LoadingCircleControl.Active = false;
                    loadingCircleToolStripMenuItem1.LoadingCircleControl.Visible = true;
                    loadingCircleToolStripMenuItem1.LoadingCircleControl.Color = Color.Red;
                    toolStripProgressBar1.Value = 0;
                    return false;
                }
            }
            return false;
        }

        private void GoDisconnect()
        {
            if (CF.conn != null)
            {
                CF.conn.Close();
                if (CF.conn.State == ConnectionState.Closed) ;
                StatusLabel_SubRf_CN.Text = "Disconnected";
                treeView1.BeginUpdate();
                treeView1.Nodes.Clear();
                treeView1.EndUpdate();
                this.treeView1.BackColor = Color.DarkGray;
                loadingCircleToolStripMenuItem1.LoadingCircleControl.Active = false;
                loadingCircleToolStripMenuItem1.LoadingCircleControl.Visible = false;
                toolStripProgressBar1.Value = 0;
            }
            StatusLabel_AllMessages.Text = "";

        }

        private void CloseAllForm()
        {
            //Cf
            //Edit Block
            //Edit Lot
            //XMLReader

        }

        /// <summary>
        /// Опредление текущей ноды - что содержит
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeObj(object sender, EventArgs e)
        {
            TreeNode ENode = null;
            if (e.GetType().ToString() == "System.Windows.Forms.KeyEventArgs")
            {
                ENode = ((TreeView)sender).SelectedNode;
            }

            if (e.GetType().ToString() == "System.Windows.Forms.TreeNodeMouseClickEventArgs")
            {
                ENode = ((TreeNodeMouseClickEventArgs)e).Node;
            }

            if ((ENode != null) && (ENode.Tag != null))
            {
                if ((ENode.Tag.GetType().ToString() == "netFteo.Spatial.TMyCadastralBlock"))
                {
                    CF.Cfg.CurrentItem.Item_id = ((TMyCadastralBlock)ENode.Tag).id;
                    CF.Cfg.CurrentItem.Item_TypeName = ENode.Tag.GetType().ToString();
                }

                if ((ENode.Tag.GetType().ToString() == "netFteo.Spatial.TMyParcel"))
                {
                    CF.Cfg.CurrentItem.Item_id = ((TMyParcel)ENode.Tag).id;
                    CF.Cfg.CurrentItem.Item_TypeName = ENode.Tag.GetType().ToString();
                }
                StatusLabel_CurrentItem.Text = "id " + CF.Cfg.CurrentItem.Item_id.ToString();
            }
        }

        //теперь Проверка количества зу в квартале:
        private bool CheckParcels(MySqlConnection conn, int block_id)
        {
            if (conn == null) return false;
            /*
            TMyParcelCollection ParcelsList = new TMyParcelCollection();
            data = new DataTable();
            da = new MySqlDataAdapter("SELECT COUNT(*) FROM lottable where lottable.block_id = " + block_id.ToString(),
                                      conn);
            da.Fill(data);
            DataRow res = data.Rows[0];
            return (Convert.ToInt32(res[0]) > 0);
            */
            MySqlCommand count = new MySqlCommand("SELECT COUNT(*) FROM lottable where lottable.block_id = " + block_id.ToString(), conn);
            var cntO = count.ExecuteScalar();
            int cnt = int.Parse(cntO.ToString());
            return (cnt > 0);
        }

        //-------------------------------- Edititng -------------------------------
        private bool Edit(TCurrentItem Item)
        {
            if (this.CF.conn.State == ConnectionState.Closed) return false;
            if (Item.Item_TypeName == "netFteo.Spatial.TMyCadastralBlock")
            {
                if (Edit(CadBloksList.GetBlock(Item.Item_id)))
                {
                    // Update node while editing
                    treeView1.SelectedNode.ToolTipText = Item.Item_id.ToString() + "*";
                    treeView1.SelectedNode.Text = ((TMyCadastralBlock)treeView1.SelectedNode.Tag).CN;
                }
            }

            if (Item.Item_TypeName == "netFteo.Spatial.TMyParcel")
            {
                if (Edit(CadBloksList.GetParcel(Item.Item_id)))
                {
                    // Update node while editing
                    treeView1.SelectedNode.Text = ((TMyParcel)treeView1.SelectedNode.Tag).CN;
                }
            }
            return false;
        }

        private bool Edit(TMyCadastralBlock block)
        {
            if (block.KPTXmlBodyList.Count == 0)
                block.KPTXmlBodyList = LoadBlockFiles(CF.conn, block.id);
            wzlBlockEd frmBlockEditor = new wzlBlockEd();
            frmBlockEditor.CF.conn = CF.conn;
            frmBlockEditor.Left = this.Left + 20; frmBlockEditor.Top = this.Top + 25;
            frmBlockEditor.ITEM = block;

            if (frmBlockEditor.ShowDialog() == DialogResult.OK)
                return true;
            else return false;
        }

        private bool Edit(TMyParcel item)
        {
            if (item.XmlBodyList.Count == 0)
                item.XmlBodyList = LoadParcelFiles(CF.conn, item.id);
            wzParcelfrm frmParcelEditor = new wzParcelfrm();
            frmParcelEditor.CF.conn = CF.conn;
            frmParcelEditor.Left = this.Left + 20; frmParcelEditor.Top = this.Top + 25;
            frmParcelEditor.ITEM = item;

            if (frmParcelEditor.ShowDialog() == DialogResult.OK)
                return true;
            else return false;
        }


        private TMyParcelCollection LoadParcelsList(MySqlConnection conn, int block_id)
        {
            if (conn == null) return null; if (conn.State != ConnectionState.Open) return null;
            StatusLabel_SubRf_CN.Text = "Загрузка участков.... ";
            StatusLabel_AllMessages.Text = "Квартал  " + block_id.ToString();
            this.Update();
            TMyParcelCollection ParcelsList = new TMyParcelCollection();
            data = new DataTable();
            da = new MySqlDataAdapter("SELECT * FROM lottable where lottable.block_id = " + block_id.ToString() +
                                      " order by lottable.lot_kn asc", conn);
            da.Fill(data);
            foreach (DataRow row in data.Rows)
            {
                TMyParcel parcel = new TMyParcel(row[1].ToString(), Convert.ToInt32(row[0])); // CN , id
                                                                                              //parcel.Name = row[3].ToString(); //Name
                parcel.CadastralBlock_id = Convert.ToInt32(row[4]); // block_id
                parcel.SpecialNote = row[5].ToString(); // lot_comment
                parcel.AreaGKN = row[6].ToString();
                ParcelsList.Add(parcel);
            }
            return ParcelsList;
        }

        /// <summary>
        /// Выборка записей из kpt, без blob поля xml_file_body, только сведения о его размере 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="block_id">id квартала</param>
        /// <returns></returns>
        private TFiles LoadBlockFiles(MySqlConnection conn, int block_id)
        {

            TFiles files = new TFiles();
            if (conn == null) return null; if (conn.State != ConnectionState.Open) return null;

            data = new DataTable();

            da = new MySqlDataAdapter("SELECT kpt_id,block_id,xml_file_name,kpt_num,kpt_date,kpt_serial,xml_ns,requestnum,acesscode," +
                   " OCTET_LENGTH(xml_file_body)/1024 as xml_size_kb from kpt where block_id = " + block_id.ToString() +
                                      " order by kpt_id asc", conn);

            da.Fill(data);
            foreach (DataRow row in data.Rows)
            {
                TFile file = new TFile(); // CN
                file.id = Convert.ToInt32(row[0]);           // id
                file.FileName = row[2].ToString();                       // block_name
                file.Number = row[3].ToString();
                file.Doc_Date = Convert.ToString(row[4]).Substring(0, Convert.ToString(row[4]).Length - 7);
                // срезать семь нулей времени MySQL "05.04.2016 0:00:00"
                file.Serial = row[5].ToString();
                file.xmlns = row[6].ToString();
                file.RequestNum = row[7].ToString();
                file.AccessCode = row[8].ToString();
                if (row[9] != DBNull.Value)
                    file.xmlSize_SQL = Math.Round(Convert.ToDouble(row[9]));
                file.Type = dFileTypes.KPT10; //KPT old than V11
                if (file.xmlns.Equals("urn://fake/kpt/5.0.0")) file.Type = dFileTypes.KPT05;
                if (file.xmlns.Equals("urn://fake/kpt/6.0.0")) file.Type = dFileTypes.KPT06;
                if (file.xmlns.Equals("urn://fake/kpt/7.0.0")) file.Type = dFileTypes.KPT07;
                if (file.xmlns.Equals("urn://fake/kpt/8.0.0")) file.Type = dFileTypes.KPT08;
                files.Add(file);
            }
            data.Reset();

            //KPT11 load:
            da = new MySqlDataAdapter("SELECT kpt_id, kpt_type,block_id,GUID,kpt_num,kpt_serial,kpt_date,requestnum,acesscode,	xml_file_name," +
                   " OCTET_LENGTH(xml_file_body)/1024 as xml_size_kb from kpt11 where block_id = " + block_id.ToString() +
                                      " order by kpt_id asc", conn);

            da.Fill(data);
            foreach (DataRow row in data.Rows)
            {
                TFile file = new TFile(); // CN
                file.id = Convert.ToInt32(row[0]);           // id
                file.Type = dFileTypes.KPT11; //Convert.ToByte(row[1]);           // kpt type
                file.FileName = row[9].ToString();           // block_id
                file.Number = row[4].ToString();
                file.Doc_Date = Convert.ToString(row[6]).Substring(0, Convert.ToString(row[6]).Length - 7);
                // срезать семь нулей времени MySQL "05.04.2016 0:00:00"
                file.Serial = row[5].ToString();
                file.RequestNum = row[7].ToString();
                file.AccessCode = row[8].ToString();
                if (row[10] != DBNull.Value)
                    file.xmlSize_SQL = Math.Round(Convert.ToDouble(row[10]));
                files.Add(file);
            }
            return files;
        }

        private TFiles LoadParcelFiles(MySqlConnection conn, int parcel_id)
        {

            TFiles files = new TFiles();
            if (conn == null) return null; if (conn.State != ConnectionState.Open) return null;
            data = new DataTable();
            da = new MySqlDataAdapter("SELECT vidimus_id,parcel_id, " +
                                      "xml_file_name,v_num,v_date,v_serial,xml_file_tns,requestnum,acesscode," +
                   " OCTET_LENGTH(xml_file_body)/1024 as xml_size_kb, vidimus_type from vidimus where parcel_id = " + parcel_id.ToString() +
                                      " order by vidimus_id asc", conn);

            da.Fill(data);
            foreach (DataRow row in data.Rows)
            {
                TFile file = new TFile(); // CN
                file.id = Convert.ToInt32(row[0]);           // id
                file.FileName = row[2].ToString();                       // block_name
                file.Number = row[3].ToString();
                file.Doc_Date = row[4].ToString();
                //file.Doc_Date = Convert.ToString(row[4]).Substring(0, Convert.ToString(row[4]).Length - 7);
                // срезать семь нулей времени MySQL "05.04.2016 0:00:00"
                file.Serial = row[5].ToString();
                file.xmlns = row[6].ToString();
                file.RequestNum = row[7].ToString();
                file.AccessCode = row[8].ToString();
                if (row[9].ToString().Length > 0)
                    file.xmlSize_SQL = Convert.ToDouble(row[9]);
                //file.id = Convert.ToInt32(row[10]); // vidimus_type, int
                files.Add(file);
            }
            return files;
        }


        //Одуренная поцедура Заполенния Полями Таблиц
        private TMyBlockCollection LoadBlockList(MySqlConnection conn, MySqlConnection conn2, int distr_id)
        {
            if (conn == null) return null; if (conn.State != ConnectionState.Open) return null;
            StatusLabel_AllMessages.Text = "Загрузка кварталов.... ";
            toolStripProgressBar1.Maximum = 0;
            toolStripProgressBar1.Value = 0;

            TMyBlockCollection CadBloksList = new TMyBlockCollection();
            CadBloksList.DistrictName = distr_id.ToString();

            data = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM blocks where blocks.district_id =" + distr_id.ToString() +
                                      " order by blocks.block_kn asc", conn);
            MySqlCommand count = new MySqlCommand("SELECT COUNT(*) FROM blocks where blocks.district_id =" + distr_id.ToString(), conn);
            int cnt = int.Parse(count.ExecuteScalar().ToString());
            toolStripProgressBar1.Maximum = cnt;
            StatusLabel_AllMessages.Text = "Кварталов: " + cnt.ToString();


            MySqlDataReader dataReader = adapter.SelectCommand.ExecuteReader();

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                data.Columns.Add(new DataColumn(dataReader.GetName(i), dataReader.GetFieldType(i)));
            }



            while (dataReader.Read())
            {
                //toolStripProgressBar1.Value = cnt++;
                toolStripProgressBar1.PerformStep();
                StatusLabel_AllMessages.Text = toolStripProgressBar1.Value.ToString() + "/" + cnt.ToString();
                Application.DoEvents();
                TMyCadastralBlock Block = new TMyCadastralBlock(dataReader["block_kn"].ToString()); // CN
                Block.id = int.Parse(dataReader["block_id"].ToString());
                Block.Name = dataReader["block_name"].ToString();
                //Загрузка участков - только при expande ??? Но тогда в начае неизвестно
                Block.HasParcels = CheckParcels(conn2, Block.id);// Запишем только признак наличия ЗУ
                CadBloksList.Blocks.Add(Block);

                // data.Rows.Add(dataReader);
                // Update progress view..
                //cnt++;
            }




            // z.b.: Доступ к полям:
            //foreach (DataColumn column in data.Columns) //values of each column
            //DataColumn Field_CN = data.Columns[1]; //values of column 1
            //DataColumn Field_id = data.Columns[0]; //values of column 1

            //load datatable at once "fill":
            /* data.Fill(data);
               StatusLabel_AllMessages.Text = "Кварталов: "+ data.Rows.Count;
            foreach (DataRow row in data.Rows)
            {
                TMyCadastralBlock Block   = new TMyCadastralBlock(row[1].ToString()); // CN
                               Block.id   = Convert.ToInt32      (row[0]) ;           // id
                               Block.Name = row[2].ToString();                       // block_name
               //Загрузка участков - только при expande ??? Но тогда в начае неизвестно
                //Block.Parcels.AddParcels(LoadParcelsList(conn, Block.id));
                Block.HasParcels = CheckParcels(conn, Block.id);// Запишем только признак наличия ЗУ
                CadBloksList.Blocks.Add(Block);
            }
            */
            dataReader.Close();
            return CadBloksList;
        }



        //Одуренная поцедура Заполенния TreeView Полями Класса TCadastralBlockList:
        private void ListBlockListTreeView(netFteo.Spatial.TMyBlockCollection List, TreeView WhatTree)
        {
            if (List == null) return;
            WhatTree.BeginUpdate();
            //WhatTree.Nodes.Add(List.DistrictName);
            //insertItem(, WhatTree)

            foreach (TMyCadastralBlock block in List.Blocks)
            {
                TreeNode node_ = insertItem(block, WhatTree);
                //  node_.Expand();

            }
            WhatTree.EndUpdate();
        }


        //Добавление района
        private TreeNode insertItem(TCadastralDistrict District, TreeView hParent)
        {
            TreeNode nn = hParent.Nodes.Add(District.CN);//nodeName);
            nn.Tag = District;
            if (District.Name != null) nn.ToolTipText = District.Name;

            nn.NodeFont = new Font("Arial", 12);//, FontStyle.Bold);
            nn.ImageIndex = 0; nn.SelectedImageIndex = 0;
            return nn;
        }

        //Добавление квартала
        private TreeNode insertItem(TMyCadastralBlock item, TreeView hParent)
        {
            TreeNode nn = hParent.Nodes.Add(item.CN);//nodeName);
            if (item.Name != null)
                nn.Text = item.CN + " " + item.Name;

            nn.Tag = item;
            if (item.Name != null) nn.ToolTipText = item.Comments;
            if (item.HasParcels) // ДОбавим ноду-пустышку, по этому признаку будут подгружаться зу при expande ноды
            {
                TreeNode hChildItem = nn.Nodes.Add("");
                hChildItem.Tag = null;
            }
            nn.NodeFont = new Font("Arial", 12);//, FontStyle.Bold);
            nn.ImageIndex = 0; nn.SelectedImageIndex = 0;
            return nn;
        }

        //Добавление участка(объекта недвижимости)
        private TreeNode insertItem(TMyParcel node, TreeNode hParent)
        {
            TreeNode nn = hParent.Nodes.Add(node.CN);
            nn.Tag = node;
            if (node.SpecialNote != null)
                nn.ToolTipText = node.SpecialNote;
            // а что для зу нужно в childnodes запихивать вообще?
            /*
            if (node.Parcels.Parcels.Count > 0)
            {
                TreeNode hChildItem = nn.Nodes.Add("");
                hChildItem.Tag = null;
            }*/
            nn.NodeFont = new Font("Arial", 10);
            nn.ImageIndex = 1;
            nn.SelectedImageIndex = 2;
            return nn;
        }

        // This function populates all siblings of the [in] node.
        private bool populateNode(object inNode, TreeNode inTreeNode)
        {
            string nodeType;
            //TreeNode popuToNode;
            // что в ноде:
            nodeType = inNode.ToString();
            if (nodeType == "netFteo.Spatial.TMyCadastralBlock")
            {
                // Еще не загружены ли ЗУ
                if ((((TMyCadastralBlock)inNode).HasParcels) && (((TMyCadastralBlock)inNode).Parcels.Count == 0))
                {
                    TMyParcelCollection reloadP = LoadParcelsList(CF.conn, ((TMyCadastralBlock)inNode).id);
                    ((TMyCadastralBlock)inNode).Parcels.AddParcels(reloadP);
                }
                {
                    foreach (TMyParcel parcel in ((TMyCadastralBlock)inNode).Parcels)
                    {
                        insertItem(parcel, inTreeNode);
                    }
                }
            }

            return true;
        }

        //Проверка загрузки участков и замена "пустышки" при необходимости
        private void PrepareNode(TreeNode hItem, object BlockInTree)
        {
            if ((BlockInTree != null) && (BlockInTree.GetType().ToString() == "netFteo.Spatial.TMyCadastralBlock"))
            {
                TreeNode hChildItem = hItem.FirstNode;
                if ((hChildItem) != null)
                {
                    if (hItem.FirstNode.Tag == null) // Если загружена "пустышка"
                    {
                        hItem.Nodes.Remove(hChildItem); // удаляем пустышку
                        if (BlockInTree != null)        //
                            if (((TMyCadastralBlock)BlockInTree).HasParcels) // и заполняем участками, если они есть
                            {
                                populateNode(BlockInTree, hItem);
                            }
                    }
                }
                /*
                else
                {
                    hItem.Nodes.Remove(hChildItem);
                    if (BlockInTree.HasParcels)
                    {
                        populateNode(BlockInTree, hItem);
                    }

                }
                */
            }
        }

        //Обработчик события по раскрытию ноды - проверка загрузки участков 
        // и замена "пустышки" при необходимости
        private void OnItemexpanding(object sender, TreeViewCancelEventArgs e)
        {
            PrepareNode(e.Node, (TMyCadastralBlock)e.Node.Tag);
        }


        private void OpenFile(string FileName)
        {
            if (!File.Exists(FileName)) return;
            string EXTention = Path.GetExtension(FileName).ToUpper();

            // got spatial file of several kind:
            if ((EXTention.Equals(".MIF")) ||
                (EXTention.Equals(".DXF")) ||
                (EXTention.Equals(".TXT")) ||
                (EXTention.Equals(".XML")))
            {
                XMLReaderCS.KVZU_Form frmReader = new XMLReaderCS.KVZU_Form();
                frmReader.DocInfo.FileName = FileName;
                frmReader.StartPosition = FormStartPosition.Manual;
                frmReader.Tag = 3; // XMl Reader в составе приложения
                frmReader.Read(FileName, false);
                frmReader.Left = this.Left + 25; frmReader.Top = this.Top + 25;
                frmReader.ShowDialog(this);
            }
            //may be here packet with RR response: zip archive, containing xml with detached signature
            if (EXTention.Equals(".ZIP"))
            {
                //1: unzip files to tmp
                {
                    //   ClearFiles();
                    BackgroundWorker w1 = new BackgroundWorker();
                    w1.WorkerSupportsCancellation = false;
                    w1.WorkerReportsProgress = true;
                    w1.DoWork += this.UnZipit;
                    w1.RunWorkerCompleted += this.UnZipComplete;
                    w1.RunWorkerAsync(FileName);
                }
                //2: parse xml, get CN of Block
                //3: make dir, copy archive files onto
                //4: dispose and kill tmp
            }

            /*
            if 

            {
                System.IO.TextReader reader = new System.IO.StreamReader(FileName);
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                xml.Load(reader);
                StatusLabel_AllMessages.Text = xml.DocumentElement.Name.ToString();
                frmReader.Read(xml);
                frmReader.ShowDialog();
            }
            */

        }

        private void UnZipit(object sender, DoWorkEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Object, DoWorkEventArgs>(UnZipit), new Object[] { sender, e });
            }
            else
            {
                try
                {
                    ReadOptions ro = new ReadOptions() { Encoding = Encoding.ASCII };
                    //ro.ReadProgress += ZipReadProgress;

                    zip = ZipFile.Read((string)e.Argument, ro);
                    this.toolStripProgressBar1.Maximum = zip.Count();
                    this.toolStripProgressBar1.Value = 0;
                    this.toolStripProgressBar1.Minimum = 0;
                    this.toolStripProgressBar1.Step = 1;

                    //zip.ExtractProgress += this.ZipExtractProgress;

                    //this.Folder_Unzip = Path.GetTempPath() + Application.ProductName + "-tmp-zip";

                    Archive_Folder = CF.Cfg.Folder_Unzip +   "\\" + Path.GetFileNameWithoutExtension((string)e.Argument);
                    /*
                     */
                    //if (zip.EntryFileNames.Contains(".xml")) // wrong ???? not working .Contains !!!
                    {
                        zip.ExtractAll(Archive_Folder);
                    }

                }
                catch (Exception ex1)
                {
                    //this.richTextBox1.Text = "Zip Exception: " + ex1.ToString();
                    string CatchText = ex1.ToString();
                }
            }
        }

        //Распаковка окончена, читаем результаты:
        private void UnZipComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            // string ArchiveFolder = this.Folder_Unzip +
            //                 "\\" + Path.GetFileNameWithoutExtension((string)e.Argument);
            //string ArchiveFolder = (string)e.Result;
            if (Directory.Exists(Archive_Folder))
            {
                DirectoryInfo di = new DirectoryInfo(Archive_Folder);
                string firstFileName = di.GetFiles().Select(fi => fi.Name).FirstOrDefault(name => name != "*.xml");


                if (File.Exists(Archive_Folder + "\\" + firstFileName))
                       if (Path.GetExtension(firstFileName.ToUpper()).Equals(".XML"))
                {
                        TextReader reader = new StreamReader(Archive_Folder + "\\" + firstFileName);
                        XmlDocument XMLDocFromFile = new XmlDocument();
                        XMLDocFromFile.Load(reader);
                        reader.Close();


                        //DocInfo.FileName = firstFileName;
                        //Read(ArchiveFolder + "\\" + firstFileName, true); // теперь загружаем xml
                        /*
                         *      if (Path.GetExtension(FileName).Equals(".xml"))
                {
                    this.DocInfo.FileName = Path.GetFileName(FileName);
                    this.DocInfo.FilePath = Path.GetFullPath(FileName);
                    TextReader reader = new StreamReader(FileName);
                    XmlDocument XMLDocFromFile = new XmlDocument();
                    XMLDocFromFile.Load(reader);
                    reader.Close();
                         */
                    }

            }



        }

        private void ClearFiles()
        {
            if (Directory.Exists(CF.Cfg.Folder_Unzip))
            {
                Directory.Delete(CF.Cfg.Folder_Unzip, true);
            }
        }

        bool SelectDistrict(TAppCfgRecord CfgRec)
        {
            if (this.CF.conn.State == ConnectionState.Closed) return false;
            SubRFForm SubSelectfrm = new SubRFForm(CF.conn2);
            //SubSelectfrm.StartPosition = FormStartPosition.Manual;
            //SubSelectfrm.Location = new Point(this.Top+10, this.Left+10);

            if (SubSelectfrm.ShowDialog() == DialogResult.Yes)
            {
                CfgRec.Subrf_id = SubSelectfrm.subrf_id;
                CfgRec.SubRF_KN = SubSelectfrm.subrf_kn;
                CfgRec.SubRF_Name = SubSelectfrm.subrf_Name;

                DistrictForm DistrSelectfrm = new DistrictForm(CF.conn2, CfgRec.Subrf_id);
                if (DistrSelectfrm.ShowDialog() == DialogResult.Yes)
                {

                    CfgRec.District_id = DistrSelectfrm.district_id;
                    CfgRec.District_KN = DistrSelectfrm.district_kn;
                    CfgRec.District_Name = DistrSelectfrm.district_Name;
                    StatusLabel_SubRf_CN.Text = CfgRec.SubRF_Name + " " + CfgRec.District_Name;
                    CfgRec.CfgWrite();
                    /*
                    CadBloksList = LoadBlockList(CF.conn, CF.conn2, CF.Cfg.District_id);
                    Application.DoEvents();
                    CfgRec.BlockCount = CadBloksList.Blocks.Count();
                    ListBlockListTreeView(CadBloksList, treeView1);
                    */
                    ConnectOps(treeView1);
                    return true;
                }
            }
            return false;
        }

        private bool Toggle_SearchTextBox(TextBox sender)
        {
            if (!sender.Visible)
            {
                panel1.Visible = true;
                sender.Visible = true;
                sender.Clear();
                sender.Focus();
                return true;
            }
            else

            {
                panel1.Visible = false;
                sender.Visible = false;
                return false;
            }
        }

        private void AppendHistory(MySqlConnection conn, TAppCfgRecord Config)
        {
            TFiles files = new TFiles();
            if (conn == null) return;
            if (conn.State != ConnectionState.Open) return;

            string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:s");


            string Query = "INSERT INTO `history` (`history_id`,`hi_disrtict_id`,`hi_item_type`,`hi_item_id`," +
                                     "`hi_data`, `hi_status_id`,  `hi_rid_id`,`hi_comment`,  `hi_host`,  `hi_ip`, " +
                                     "`hi_systemusername`,`hi_dbusername`) " +
                                     "VALUES(NULL," + Config.District_id.ToString() + ",'200'," + Config.CurrentItem.Item_id.ToString() +
                                     ",'" + dt + "','200','-1','.NET App v" + ver + " connect','" + netFteo.NetWork.NetWrapper.Host + "','" + netFteo.NetWork.NetWrapper.HostIP + "'," +
                                     "'" + netFteo.NetWork.NetWrapper.UserName + "','" + Config.UserName + "');";


            //INSERT INTO `history` (`history_id`, `hi_disrtict_id`, `hi_item_type`, `hi_item_id`, `hi_data`, `hi_status_id`, `hi_rid_id`, `hi_comment`, `hi_host`, `hi_ip`, `hi_systemusername`, `hi_dbusername`) VALUES(NULL, NULL, '200', '1', 
            //'2019-02-18 02:09:10', '200', '1', '11', '1', '1', '1', '1');
            //"18.02.2019 11:58:02"


            MySqlCommand cmd = new MySqlCommand(Query, conn);
            MySqlDataReader re = cmd.ExecuteReader();
            re.Close();

        }


        #endregion

        #region Назначенные Обработчики событий
        private void MenuItem_Connect_Click(object sender, EventArgs e)
        {
            ConnectGo();
        }
        private void Button_Connect_Click(object sender, EventArgs e)
        {
            ConnectGo();
        }
        private void Button_ChangeSub_Click(object sender, EventArgs e)
        {
            SelectDistrict(CF.Cfg);
        }

        private void Button_Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Документы xml|*.xml";
            od.FileName = "";
            if (od.ShowDialog() == DialogResult.OK)
            {
                OpenFile(od.FileName);
            }
        }
        private void импортToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoDisconnect();
            this.Close();
        }
        private void сбросПодключенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoDisconnect();
        }



        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            GoDisconnect();
            this.Close();
        }

        private void treeView1_KeyUp(object sender, KeyEventArgs e)
        {
            ChangeObj(sender, e);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ChangeObj(sender, e);
        }


        private void сменитьСубъектToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectDistrict(CF.Cfg);

        }

        private void Button_Property_Click(object sender, EventArgs e)
        {
            Edit(CF.Cfg.CurrentItem);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            ConnectGo();
        }
        #endregion

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 Aboutgkndatafrm = new AboutBox1();
            Aboutgkndatafrm.ShowDialog();
        }

        private void онлайнToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Edit(CF.Cfg.CurrentItem);
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            ChangeObj(sender, e);
        }

        //
        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CF.Cfg.CfgRead();
            CF.ShowDialog();
        }

        private void поискToolStripMenuItem_Click(object sender, EventArgs e)
        {
            поискToolStripMenuItem.Checked = Toggle_SearchTextBox(SearchTextBox);
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Control) && (e.KeyCode == Keys.D))
                Toggle_SearchTextBox((TextBox)sender);

        }





        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox searchtbox = (TextBox)sender;
            if (searchtbox.Visible)
            {   // начинаем с высшей ноды:
                treeView1.BeginUpdate();

                if (searchtbox.Text == "")
                {
                    treeView1.SelectedNode = treeView1.Nodes[0]; // hi root node, seek to begin
                    treeView1.CollapseAll();
                }
                //FindNode не ходит далее одного root элемента:
                //FindNode(treeView1.Nodes[0], searchtbox.Text.ToUpper(), false);

                TreeNode res = TreeViewFinder.SearchNodes(treeView1.Nodes[0], searchtbox.Text.ToUpper());

                if (res != null)
                {
                    treeView1.SelectedNode = res;
                    //TODO:  
                    //В случае поиска до раскрытия нод, для которых еще недогружены дочерние
                    PrepareNode(treeView1.SelectedNode, res.Tag);
                    treeView1.SelectedNode.EnsureVisible();
                }
                SearchTextBox.Focus();
                treeView1.EndUpdate();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if ((treeView1.SelectedNode != null) && (treeView1.SelectedNode.NextNode != null))
            {
                TreeNode res = TreeViewFinder.SeekNode(TreeViewFinder.SearchNextNode(treeView1.SelectedNode), SearchTextBox.Text.ToUpper());
                if (res != null)
                {
                    treeView1.SelectedNode = res;
                    treeView1.SelectedNode.EnsureVisible();
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (CF.Cfg.CurrentItem.Item_TypeName == CF.Cfg.CurrentItem.TypeName_Parcel)
                Edit(CF.Cfg.CurrentItem);
        }

        private void Button_Import_CheckStateChanged(object sender, EventArgs e)
        {

        }

        private void MainGKNForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void MainGKNForm_DragDrop(object sender, DragEventArgs e)
        {
            ClearFiles();
            this.DraggedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string filename in DraggedFiles)
            {
                try
                {
                    OpenFile(filename);
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    //ClearControls();
                    //TreeNode errNodePatr = TV_Parcels.Nodes.Add("Error in " + Path.GetFileName(filename));
                    //errNodePatr.Nodes.Add(ex.Message);

                    //if (TV_Parcels.TopNode != null) TV_Parcels.TopNode.Expand();
                    //else
                    //  errNodePatr.ExpandAll();
                    //return;
                }
            }

        }
    

		private void MainGKNForm_Load(object sender, EventArgs e)
		{
			loadingCircleToolStripMenuItem1.BackColor = Color.Transparent;
			this.TimeOutTimer = new Timer();
			TimeOutTimer.Interval = 1000;
			TimeOutTimer.Tick += TimeOut_TimerTick;
            CF.Cfg.Folder_Unzip = Path.GetTempPath() + Application.ProductName + "-tmp-zip";
            ClearFiles();
        }




		private void свойстваToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Edit(CF.Cfg.CurrentItem);
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{

		}

		private void button3_Click(object sender, EventArgs e)
		{
			SelectDistrict(CF.Cfg);
		}

		private void Button_Exit_Click(object sender, EventArgs e)
		{
			GoDisconnect();
			this.Close();

		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			treeView1.Visible = true;
			treeView1.Dock = DockStyle.Fill;
		}

		private void button_History_Click(object sender, EventArgs e)
		{
			treeView1.Visible = false;
			treeView1.Dock = DockStyle.None;
		}

		private void button_Favorites_Click(object sender, EventArgs e)
		{
			treeView1.Visible = false;
			treeView1.Dock = DockStyle.None;
		}

		/*
		 * 
		 */
	}
}
