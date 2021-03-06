﻿using System;
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
using netFteo.Cadaster;

namespace GKNData

{
    public partial class MainGKNForm : Form
    {

        private DataTable data;
        private MySqlDataAdapter da;
        //       private MySqlCommandBuilder cb;
        private int TIMEOUT_DONE;
        private Timer TimeOutTimer; // iddle timer
        ConnectorForm CF = new ConnectorForm();
        public TCadastralDistrict CadBloksList; // Global collection
        ZipFile zip;

        /// <summary>
        /// Common application unzipping work folder
        /// </summary>
        // string Folder_Unzip;

        /// <summary>
        /// Current archive folder
        /// </summary>
        string Archive_Folder;
        //bool UnzipinPorgressFlag;
        AutoCompleteStringCollection SearchAutoComplete = new AutoCompleteStringCollection();

        /// <summary>
        /// List of files, dragged on to form
        /// </summary>
        string[] DraggedFiles;

        string CurrentDraggedFile;

        Font Font_Arial10, Font_Arial12;

        public MainGKNForm()
        {
            InitializeComponent();
            treeView1.BeforeExpand += OnItemexpanding; //Подключаем обработчик раскрытия
            Application.Idle += On_Iddle;
            treeView1.Nodes.Clear();
            Font_Arial10 = new Font("Arial", 10);
            Font_Arial12 = new Font("Arial", 12);
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

            StatusLabel_AllMessages.Text = "wait... " + (ms / 1000) + " last " + (last);
            if (last < 1000) toolStripProgressBar1.Value = toolStripProgressBar1.Maximum; // reset bar

            if (ms < 0)
            {
                //this.Close();
                StatusLabel_AllMessages.Text = "will close.." + ms.ToString() ;
            }

            else if (ms < 5000)
            {
                //GoDisconnect();
                StatusLabel_AllMessages.Text = "disconnect by timeout.." + ms.ToString();
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
        private void ConnectOps(long District_id)
        {
            CadBloksList = LoadBlockList(CF.conn, CF.conn2, District_id);
            Application.DoEvents();
            CF.Cfg.BlockCount = CadBloksList.Blocks.Count();
            ListBlockListTreeView(CadBloksList, treeView1);
            CF.Cfg.ViewLevel = ViewLevel.vlBlocks;
            DBWrapper.Config = CF.Cfg;
            string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            DBWrapper.DB_AppendHistory(ItemTypes.it_Connect, -1, DBLogRecordStatus.it_Connect, "Connect App V" + ver, CF.conn);
            toolStripButton_Connect.Enabled = false;
            MenuItem_Connect.Enabled = false;
            button4.Enabled = true;
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
            this.treeView1.BackColor = SystemColors.Control;
            this.treeView1.Nodes.Clear();
            loadingCircleToolStripMenuItem1.LoadingCircleControl.Color = SystemColors.Highlight;
            loadingCircleToolStripMenuItem1.Visible = true;
            loadingCircleToolStripMenuItem1.LoadingCircleControl.Active = true;
            TIMEOUT_DONE = Convert.ToInt16(CF.Cfg.IddleTimeOut);
            {
                if (CF.conn != null)
                    CF.conn.Close();
                //SSL Mode=Required
                string connStr = String.Format("server={0};user id={1}; password={2}; database={3}; CharSet={4}; pooling=false;SSL Mode=None",
                    CF.Cfg.ServerName, CF.Cfg.UserName, CF.Cfg.UserPwrd, CF.Cfg.DatabaseName, CF.Cfg.CharSet);

                try
                {
                    CF.conn = new MySqlConnection(connStr);
                    CF.conn.Open();
                    CF.conn2 = new MySqlConnection(connStr);
                    CF.conn2.Open();

                    if (CF.conn.State == ConnectionState.Open)
                    {
                        // MySqlCommand SetNamesCMD = new MySqlCommand("SET NAMES 'cp1251'", CF.conn2);
                        // SetNamesCMD.ExecuteNonQuery();
                        // SetNamesCMD.CommandText = "SET CHARACTER SET 'cp1251'";
                        // SetNamesCMD.ExecuteNonQuery();
                        ConnectOps(CF.Cfg.District_id);
                        LoadSubRF(CF.conn, Explorer_listView);
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

                    treeView1.Nodes.Add("Connect Error").Nodes.Add(ex.Message);
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
                CadBloksList.Blocks.Clear();
                CadBloksList.ParsedSpatial.Clear();
                CadBloksList.SpatialData.Clear();
                treeView1.Nodes.Clear();
                Explorer_listView.Items.Clear();
                treeView1.EndUpdate();
                this.treeView1.BackColor = Color.DarkGray;
                loadingCircleToolStripMenuItem1.LoadingCircleControl.Active = false;
                loadingCircleToolStripMenuItem1.LoadingCircleControl.Visible = false;
                toolStripButton_Connect.Enabled = true;
                button4.Enabled = false;
                MenuItem_Connect.Enabled = true;
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
        /// Change Object context - when scrolling, typing, clicking nodes/items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeObj(object sender, EventArgs e)
        {
            TreeNode ENode = null;
            if (sender.GetType().ToString() == "System.Windows.Forms.TreeView")
            {
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
                    if (ENode.Tag.GetType().ToString() == "netFteo.TreeNodeTag")
                    {
                        CF.Cfg.CurrentItem.Item_id = ((netFteo.TreeNodeTag)ENode.Tag).Item_id;
                        CF.Cfg.CurrentItem.Item_TypeName = ((netFteo.TreeNodeTag)ENode.Tag).Type;
                        CF.Cfg.CurrentItem.Item_NameExt = ((netFteo.TreeNodeTag)ENode.Tag).NameExt;
                    }
                }
                CF.Cfg.CurrentItem.SelectedNode = ENode;
            }

            if (sender.GetType().ToString() == "System.Windows.Forms.ListView")
            {
                ListViewItem EItem = null;
                if (e.GetType().ToString() == "System.Windows.Forms.KeyEventArgs")
                {
                    EItem = ((ListView)sender).SelectedItems[0];
                }
                string CheckType1 = sender.GetType().ToString();
                string CheckType2 = e.GetType().ToString();
                
                if (e.GetType().ToString() == "System.EventArgs")
                {
                    EItem = ((ListView)sender).SelectedItems[0];
                }
                

                if ((EItem != null) && (EItem.Tag != null))
                {

                    if (EItem.Tag.GetType().ToString() == "netFteo.TreeNodeTag")
                    {
                        CF.Cfg.CurrentItem.Item_id = ((netFteo.TreeNodeTag)EItem.Tag).Item_id;
                        CF.Cfg.CurrentItem.Item_TypeName = ((netFteo.TreeNodeTag)EItem.Tag).Type;
                        CF.Cfg.CurrentItem.Item_NameExt = ((netFteo.TreeNodeTag)EItem.Tag).NameExt;
                    }
                    CF.Cfg.CurrentItem.SelectedItem = EItem;
                }
            }
            StatusLabel_CurrentItem.Text = "id " + CF.Cfg.CurrentItem.Item_id.ToString();
            Text = "TypeName " + CF.Cfg.CurrentItem.Item_TypeName + ".  " + CF.Cfg.CurrentItem.Item_NameExt + ". Name " + CF.Cfg.CurrentItem.Name;
        }

  
        //теперь Проверка количества зу в квартале:
        private bool CheckParcels(MySqlConnection conn, long block_id)
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


        private bool AddItem(TCurrentItem Item, ViewLevel Level)
        {
            switch (Level)
            {
                case ViewLevel.vlExploreSubRF:
                    {
                        string SubRFCN = "00:00";
                        if (InputBox.doInputBox("Добавление Кадастрового округа", "Введите номер", ref SubRFCN) == DialogResult.OK)
                        {
                            TCadasterItem SubRF = new TCadasterItem(SubRFCN);
                            if (AddSubRF(SubRF))
                            {
                                return true;
                            }
                            
                        }
                        return false;
                    }

                case ViewLevel.vlExploreDistricts:
                    {
                        if (Item.Item_TypeName == "subrf")
                        {
                            string DistrictCN = "00:00";
                            if (InputBox.doInputBox("Добавление Кадастрового района", "Введите номер", ref DistrictCN) == DialogResult.OK)
                            {
                                TCadastralDistrict District = new TCadastralDistrict();
                                District.CN = DistrictCN;
                                District.Name = "Район";
                                District.SubRF_id = Convert.ToByte(Item.Item_id);
                                /*
                                District.Parent.id = CF.Cfg.Subrf_id;
                                District.Parent.CN = CF.Cfg.SubRF_KN;
                                District.Parent.Definition = CF.Cfg.SubRF_Name;
                                */
                                if (AddDistrict(District))
                                {
                                    return true;
                                }
                            }
                        }
                        return false;
                    }

                case ViewLevel.vlBlocks:
                    {

                        if (Item.Item_TypeName == "district")
                        {
                            //add block
                            TCadastralBlock Block = new TCadastralBlock();
                            if (InputBox.doInputBox("Добавление кадастрового квартала", "Введите номер", ref Block.CN) == DialogResult.OK)
                            {
                                if (!CadBloksList.BlockExist(Block.CN))
                                {
                                    Block.Parent_id = CF.Cfg.District_id;
                                    {
                                        if (DBWrapper.DB_AppendBlock(Block, CF.conn) > 0)
                                        {
                                            CadBloksList.AddBlock(Block);
                                            //wzlBlockEd blEd = new wzlBlockEd();
                                            insertItem(Block, treeView1);
                                            StatusLabel_AllMessages.Text =Block.CN +  " добавлен ";
                                        }
                                        else
                                            MessageBox.Show(DBWrapper.LastErrorMsg, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Question);
                                    }
                                }
                            }
                        }

                            if (Item.isBlock)

                            {
                                //add Parcel
                                TCadastralBlock bl = CadBloksList.GetBlock(Item.Item_id);
                                TParcel parcel = new TParcel();
                                parcel.CadastralBlock = bl.CN;
                                parcel.CadastralBlock_id = bl.id;
                                parcel.CN = bl.CN + ":1";

                                if (bl.Parcels.Count > 0)
                                {
                                    parcel.CN = bl.Parcels.Last().CN;
                                }

                                if (InputBox.doInputBox("Добавление объекта недвижимости", "Введите номер", ref parcel.CN) == DialogResult.OK)
                                {
                                    if (!bl.ParcelExist(parcel.CN))
                                    {
                                        if (AddParcel(parcel,bl, Item.SelectedNode))
                                        {
                                        //    bl.Parcels.AddParcel(parcel);
                                        //populate new node by new item
                                        //insertItem(parcel, Item.SelectedNode);
                                        }
                                    }
                                }
                            }

                            if (Item.Item_TypeName == "netFteo.Spatial.TMyParcel")
                            {
                                //add document
                            }
                            return false;
                        }
                    }
            return false;
        }


        private bool AddSubRF(TCadasterItem item)
        {
            item.id = DBWrapper.DB_AppendSubRF(item, CF.conn);
            if (item.id > 0)
                return true;
            else return false;
        }

        private bool AddDistrict(TCadastralDistrict item)
        {
            item.id = DBWrapper.DB_AppendDistrict(item, CF.conn);
            if (item.id > 0)
            {
                StatusLabel_AllMessages.Text = item.CN + " добавлен ";
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Add block record into database,
        /// next into blocks collection of district
        /// and at end onto treeview
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool AddBlock(TCadastralBlock Block, TCadastralDistrict District, TreeView tv)
        {
            // ??? Block.Parent_id ????
            Block.id = DBWrapper.DB_AppendBlock(Block, CF.conn);
            if (Block.id> 0)
            {
                District.AddBlock(Block);
                //wzlBlockEd blEd = new wzlBlockEd();
                TreeNode NewBlockNode = insertItem(Block, tv);
                StatusLabel_AllMessages.Text = NewBlockNode.Text + " добавлен ";
                return true;
            }
            else return false;
        }

            /// <summary>
            /// Add parcel record into database,
            /// next into Block parcels collection
            /// and at end onto treeview
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            private bool AddParcel(TParcel item, TCadastralBlock Block, TreeNode itemViewNode)
        {
            item.CadastralBlock_id = Block.id;
            item.id = DBWrapper.DB_AppendParcel(item, CF.conn);
            if (item.id > 0)
            {
                Block.Parcels.AddParcel(item);
                //populate new node by new item, TODO find item:
                insertItem(item, itemViewNode);
                StatusLabel_AllMessages.Text = item.CN + " добавлен ";
                return true;
            }
            else return false;
        }
        //-------------------------------- Edititng -------------------------------
        private bool Edit(TCurrentItem Item)
        {
            if (this.CF.conn.State == ConnectionState.Closed) return false;

            if (Item.isDistrict)
            {
                //TOdo: where stores collections
                //District context required:
                TCurrentItem DistrictFake = LoadDistrict(Item.Item_id, CF.conn);
                if (DistrictFake != null)
                {
                    wzDistrict wzDistrictFrm = new wzDistrict(DistrictFake);
                    //wzDistrictFrm.Item = DistrictFake;
                    if (wzDistrictFrm.ShowDialog(this) == DialogResult.OK)
                    {
                        StatusLabel_AllMessages.Text = "Update district.... ";
                        if (DBWrapper.DB_UpdateCadastralDistrict(DistrictFake, CF.conn))
                        {
                            treeView1.SelectedNode.Text = DistrictFake.Item_NameExt + " " + DistrictFake.Item_TypeName;
                        }
                    }
                }
            }

            if (Item.isBlock)
            {
                TCadastralBlock block = CadBloksList.GetBlock(Item.Item_id);
                if (Edit(block))
                {
                    // Update node after changes
                    treeView1.SelectedNode.ToolTipText = block.Comments;
                    treeView1.SelectedNode.Text = block.CN + " " + block.Name;// ((TMyCadastralBlock)treeView1.SelectedNode.Tag).CN;
                }
                //anyway save to history
                History_ListBox.Items.Add(block.CN);
                SearchAutoComplete.Add(block.CN);
            }

            if (Item.isParcel)
            {
                if (Edit(CadBloksList.GetParcel(Item.Item_id)))
                {
                    // Update node while editing
                    treeView1.SelectedNode.Text = CadBloksList.GetParcel(Item.Item_id).CN;     // ((TMyParcel)treeView1.SelectedNode.Tag).CN;
                }
            }
            return false;
        }

        /*
        private bool Edit(TCurrentItem district)
        {
            if (district == null) return false;
            wzDistrict wzDistrictFrm = new wzDistrict();
            wzDistrictFrm.Item = district;
            if (wzDistrictFrm.ShowDialog(this) == DialogResult.OK)
            {
                StatusLabel_AllMessages.Text = "Update district.... ";
                return DBWrapper.DB_UpdateCadastralDistrict(district, CF.conn);
            }
            else return false;
        }
        */
        private bool Edit(TCadastralBlock block)
        {
            //if (block.KPTXmlBodyList.Count == 0)
            //anyway load files:
            if (block == null) return false;
            block.KPTXmlBodyList.Clear();
            block.KPTXmlBodyList = DBWrapper.LoadBlockFiles(CF.conn, block.id);
            wzlBlockEd frmBlockEditor = new wzlBlockEd();
            frmBlockEditor.CF.conn = CF.conn;
            frmBlockEditor.Left = this.Left + 20; frmBlockEditor.Top = this.Top + 25;
            frmBlockEditor.ITEM = block;

            if (frmBlockEditor.ShowDialog() == DialogResult.OK)
            {
                StatusLabel_AllMessages.Text = "Update block.... ";
                return DBWrapper.DB_UpdateCadastralBlock(block, 0, 0, CF.conn);
            }
            else return false;
        }

        private bool Edit(TParcel item)
        {
            if (item == null) return false;
            //anyway load files:
            item.XmlBodyList.Clear();
            item.XmlBodyList = DBWrapper.LoadParcelFiles(CF.conn, item.id);
            wzParcelfrm frmParcelEditor = new wzParcelfrm();
            frmParcelEditor.CF.conn = CF.conn;
            frmParcelEditor.Left = this.Left + 20; frmParcelEditor.Top = this.Top + 25;
            frmParcelEditor.ITEM = item;

            if (frmParcelEditor.ShowDialog() == DialogResult.OK)
            {
                return DBWrapper.DB_UpdateParcel(item, CF.conn);
            }
            else return false;
        }

        private bool Erase(TCurrentItem Item)
        {
            if (this.CF.conn.State == ConnectionState.Closed) return false;
            if (Item.isBlock)
            {
                TCadastralBlock block = CadBloksList.GetBlock(Item.Item_id);
                if (Erase(block, CF.conn))
                {
                    // Update node after changes
                    treeView1.Nodes.Remove(treeView1.SelectedNode);
                }
            }

            if (Item.isParcel)
            {
                if (Erase(CadBloksList.GetParcel(Item.Item_id), CF.conn))
                {
                    // Update node while editing
                    treeView1.Nodes.Remove(treeView1.SelectedNode);
                }
            }
            return false;
        }

        private bool Erase(TCadastralBlock block, MySqlConnection conn)
        {
            string message = "Удалить " + block.CN;
            if (MessageBox.Show(message, "Подтвердите",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //kill docs
                foreach (TFile file in block.KPTXmlBodyList)
                {
                    if (DBWrapper.EraseKPT(file.id, conn))
                    {
                        // kill file body
                        file.File_BLOB = null;
                    }
                }

                foreach (TParcel Parcel in block.Parcels)
                {
                    Erase(Parcel, conn);
                }

                if (DBWrapper.EraseBlock(block.id, conn))
                {
                    CadBloksList.Blocks.Remove(block);
                    return true;
                }
                else
                    MessageBox.Show(DBWrapper.LastErrorMsg, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Question);

            }
            return false;
        }

        private bool Erase(TParcel parcel, MySqlConnection conn)
        {
            string message = "Удалить " + parcel.CN;
            if (MessageBox.Show(message, "Подтвердите",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //kill docs
                foreach (TFile file in parcel.XmlBodyList)
                {
                    if (DBWrapper.EraseVidimus(file.id, conn))
                    {
                        // kill file body
                        file.File_BLOB = null;
                    }
                }
                if (DBWrapper.EraseParcel(parcel.id, conn))
                    return true;
                else
                    MessageBox.Show(DBWrapper.LastErrorMsg, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Question);

            }
            return false;
        }

        private TParcels LoadParcelsList(MySqlConnection conn, long block_id)
        {
            if (conn == null) return null; if (conn.State != ConnectionState.Open) return null;
            StatusLabel_SubRf_CN.Text = "Загрузка участков.... ";
            StatusLabel_AllMessages.Text = "Квартал  " + block_id.ToString();
            this.Update();
            TParcels ParcelsList = new TParcels();
            data = new DataTable();
            da = new MySqlDataAdapter("SELECT * FROM lottable where lottable.block_id = " + block_id.ToString() +
                                      " order by lottable.lot_kn asc", conn);
            da.Fill(data);
            foreach (DataRow row in data.Rows)
            {
                TParcel parcel = new TParcel(row[1].ToString(), Convert.ToInt32(row[0])); // CN , id
                                                                                              //parcel.Name = row[3].ToString(); //Name
                parcel.CadastralBlock_id = Convert.ToInt32(row[4]); // block_id
                parcel.SpecialNote = row[5].ToString(); // lot_comment
                parcel.Name = row[3].ToString(); // lot_name
                parcel.AreaGKN = row[6].ToString();
                ParcelsList.Add(parcel);
            }
            return ParcelsList;
        }




        /// <summary>
        /// Load BlockList from database
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="conn2"></param>
        /// <param name="distr_id"></param>
        /// <returns></returns>
        private TCadastralDistrict LoadBlockList(MySqlConnection conn, MySqlConnection conn2, long distr_id)
        {
            if (conn == null) return null; if (conn.State != ConnectionState.Open) return null;
            StatusLabel_AllMessages.Text = "Загрузка кварталов.... ";
            toolStripProgressBar1.Maximum = 0;
            toolStripProgressBar1.Value = 0;

            TCadastralDistrict CadBloksList = new TCadastralDistrict();
            CadBloksList.Name = distr_id.ToString();
            CadBloksList.id = distr_id;

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
                TCadastralBlock Block = new TCadastralBlock(dataReader["block_kn"].ToString()); // CN
                Block.id = int.Parse(dataReader["block_id"].ToString());
                Block.Name = dataReader["block_name"].ToString();
                Block.Comments = dataReader["block_comment"].ToString();
                //Загрузка участков - только при expande ??? Но тогда в начае неизвестно
                Block.HasParcels = CheckParcels(conn2, Block.id);// Set only parcel present flag
                CadBloksList.AddBlock(Block);

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


 


        /// <summary>
        /// Change blocks for appropiate district
        /// </summary>
        /// <param name="District_id">Parent district id</param>
        /// <param name="Cfg">Configuration</param>
        /// <param name="District_Name"></param>
        private void LoadBlocks(long District_id, TAppCfgRecord Cfg, TreeView tv, string District_Name = "***")
        {
            //change id
            Cfg.District_id = District_id; //((netFteo.TreeNodeTag)lvItem.Tag).Item_id;
            Cfg.District_Name = District_Name; //((netFteo.TreeNodeTag)lvItem.Tag).Name;
            CF.Cfg.ViewLevel = ViewLevel.vlBlocks;
            Cfg.CfgWrite(); //save them
            CadBloksList = LoadBlockList(CF.conn, CF.conn2, District_id);
            ListBlockListTreeView(CadBloksList, tv);
            Button_Import.Enabled = true;
        }


        private void LoadSubRF(MySqlConnection conn, ListView lv)
        {
            treeView1.Visible = false;
            treeView1.Dock = DockStyle.None;
            lv.Visible = true;
            lv.Dock = DockStyle.Fill;
            lv.View = View.LargeIcon;
            lv.Items.Clear();
            Hide_SearchTextBox(SearchTextBox);
            CF.Cfg.ViewLevel = ViewLevel.vlExploreSubRF;

            if (conn != null)
                if (conn.State == ConnectionState.Open)
                {
                    MySqlDataReader reader;
                    MySqlCommand command = new MySqlCommand();
                    string commandString = "select * from subrf;";
                    command.CommandText = commandString;
                    command.Connection = conn;
                    Byte cnt=0;

                    try
                    {
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            netFteo.TreeNodeTag tag = new netFteo.TreeNodeTag(Convert.ToInt32(reader["subrf_id"]), "subrf");
                            tag.Name = reader["subrf_Name"].ToString();
                            tag.NameExt = reader["subrf_kn"].ToString();
                            ListViewItem subRFItem = new ListViewItem(tag.Name);

                            subRFItem.Tag = tag;
                            subRFItem.ToolTipText = tag.NameExt;
                            subRFItem.ImageIndex = 5;
                            lv.Items.Add(subRFItem);
                            cnt++;
                        }
                        reader.Close();
                        StatusLabel_AllMessages.Text = cnt.ToString();
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Error: \r\n{0}", ex.ToString());
                    }
                    finally
                    {
                        //command.Connection.Close();
                    }

                }
        }

        private void LoadDistricts(short subrf_id, MySqlConnection conn, ListView lv)
        {
            treeView1.Visible = false;
            Button_Import.Enabled = false;
            treeView1.Dock = DockStyle.None;
            lv.Visible = true;
            lv.Dock = DockStyle.Fill;
            lv.View = View.LargeIcon;
            lv.Items.Clear();
            Hide_SearchTextBox(SearchTextBox);
            CF.Cfg.ViewLevel = ViewLevel.vlExploreDistricts;
            CF.Cfg.Subrf_id = subrf_id;
            if (conn != null)
                if (conn.State == ConnectionState.Open)
                {
                    MySqlDataReader reader;
                    MySqlCommand command = new MySqlCommand();
                    string commandString = "SELECT * FROM districts where 	districts.subrf_id = " + subrf_id;
                    command.CommandText = commandString;
                    command.Connection = conn;
                    Byte cnt = 0;
                    try
                    {
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            netFteo.TreeNodeTag tag = new netFteo.TreeNodeTag(Convert.ToInt32(reader["district_id"]),"netFteo.Cadaster.TCadastralDistrict");
                            tag.Name = reader["district_Name"].ToString();
                            tag.NameExt = reader["district_kn"].ToString();

                            ListViewItem subRFItem = new ListViewItem(tag.Name);
                            subRFItem.Tag = tag;//reader["district_id"].ToString();
                            subRFItem.ToolTipText = tag.NameExt;
                            subRFItem.ImageIndex = 1;
                            lv.Items.Add(subRFItem);
                            cnt++;
                        }
                        reader.Close();
                        StatusLabel_AllMessages.Text = cnt.ToString();
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Error: \r\n{0}", ex.ToString());
                    }
                    finally
                    {
                        //command.Connection.Close();
                    }

                }
        }

        /// <summary>
        /// Load fake district object - just CN, Name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        private TCurrentItem LoadDistrict(long id, MySqlConnection conn)
        {
            if ((conn != null) &&
                 (conn.State == ConnectionState.Open))
                {
                    MySqlDataReader reader;
                    MySqlCommand command = new MySqlCommand();
                    string commandString = "SELECT * FROM districts where 	districts.district_id = " + id;
                    command.CommandText = commandString;
                    command.Connection = conn;
                    TCurrentItem res = new TCurrentItem();
                    try
                    {
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            res.Item_id = Convert.ToInt32(reader["district_id"]);
                            res.Item_NameExt = reader["district_kn"].ToString();
                            res.Item_TypeName = reader["district_Name"].ToString();
                        }
                        reader.Close();
                        return res;
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Error: \r\n{0}", ex.ToString());
                        return null;
                    }
                    finally
                    {
                        //command.Connection.Close();
                    }

                }
                else return null;
        }

        // TreeView  TCadastralBlockList:
        private void ListBlockListTreeView(TCadastralDistrict List, TreeView WhatTree)
        {
            if (List == null) return;
            WhatTree.BeginUpdate();
            WhatTree.Nodes.Clear();

            foreach (TCadastralBlock block in List.Blocks)
            {
                insertItem(block, WhatTree);
            }
            WhatTree.EndUpdate();
        }

        /// <summary>
        /// Insert node for Cadatastral Block
        /// </summary>
        /// <param name="item">CadastralBlock</param>
        /// <param name="hParent">Target Treeview</param>
        private TreeNode insertItem(TCadastralBlock item, TreeView hParent)
        {
            TreeNode nn = hParent.Nodes.Add(item.CN);//nodeName);
            if (item.Name != null)
                nn.Text = item.CN + " " + item.Name;
            //nn.Tag = item; 
            //instead of putting full object insert just smaller:
            nn.Tag = new netFteo.TreeNodeTag(item.id, item.GetType().ToString());
            if (item.Name != null) nn.ToolTipText = item.Comments;
            if (item.HasParcels) // ДОбавим ноду-пустышку, по этому признаку будут подгружаться зу при expande ноды
            {
                TreeNode hChildItem = nn.Nodes.Add("");
                hChildItem.Tag = null;
            }
            nn.NodeFont = Font_Arial12;// new Font("Arial", 12);
            nn.ImageIndex = 0; nn.SelectedImageIndex = 0;
            return nn;
        }

        //Добавление участка(объекта недвижимости)
        private TreeNode insertItem(TParcel item, TreeNode hParentBlock)
        {
            if (hParentBlock == null) return null;
            TreeNode nn = hParentBlock.Nodes.Add(item.CN);
            //nn.Tag = node;
            //instead of putting full object insert just smaller:
            nn.Tag = new netFteo.TreeNodeTag(item.id, item.GetType().ToString());
            if (item.SpecialNote != null)
                nn.ToolTipText = item.SpecialNote;
            nn.NodeFont = Font_Arial10;//new Font("Arial", 10);
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
            if (nodeType == NetFteoTypes.Block)
            {
                // Еще не загружены ли ЗУ
                if ((((TCadastralBlock)inNode).HasParcels) && (((TCadastralBlock)inNode).Parcels.Count == 0))
                {
                    TParcels reloadP = LoadParcelsList(CF.conn, ((TCadastralBlock)inNode).id);
                    ((TCadastralBlock)inNode).Parcels.AddParcels(reloadP);
                }
                {
                    foreach (TParcel parcel in ((TCadastralBlock)inNode).Parcels)
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
            if ((BlockInTree != null) && (BlockInTree.GetType().ToString() == NetFteoTypes.Block))
            {
                TreeNode hChildItem = hItem.FirstNode;
                if ((hChildItem) != null)
                {
                    if (hItem.FirstNode.Tag == null) // Если загружена "пустышка"
                    {
                        hItem.Nodes.Remove(hChildItem); // удаляем пустышку
                        if (BlockInTree != null)        //
                            if (((TCadastralBlock)BlockInTree).HasParcels) // и заполняем участками, если они есть
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
            TCadastralBlock block = CadBloksList.GetBlock(((netFteo.TreeNodeTag)e.Node.Tag).Item_id);
            if (block != null)
                PrepareNode(e.Node, block);
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

        /// <summary>
        /// Open file and if it of parsed types, insert it into DB
        /// </summary>
        /// <param name="FileName"></param>
        private void OpenFile(string FileName)
        {
            if (!File.Exists(FileName)) return;
            string EXTention = Path.GetExtension(FileName).ToUpper();

            // got spatial file of several kind:
            if (EXTention.Equals(".MIF") ||
                EXTention.Equals(".DXF") ||
                EXTention.Equals(".TXT")
                )
            {
                XMLReaderCS.KVZU_Form frmReader = new XMLReaderCS.KVZU_Form();
                frmReader.DocInfo.FileName = FileName;
                frmReader.StartPosition = FormStartPosition.Manual;
                frmReader.Tag = 3; // XMl Reader в составе приложения
                frmReader.Read(FileName, false);
                frmReader.Left = this.Left + 25; frmReader.Top = this.Top + 25;
                frmReader.ShowDialog(this);
            }

            // got xml file of several kind:
            if (EXTention.Equals(".XML"))
            {
                //parse XMlDocument as stream:
                netFteo.IO.FileInfo ParsedDoc = RRTypes.CommonParsers.ParserCommon.ParseXMLDocument(new MemoryStream(File.ReadAllBytes(FileName)));
                //so go on:
                //case 1  - got KPT kind:
                if ((netFteo.Rosreestr.NameSpaces.NStoFileType(ParsedDoc.Namespace) == netFteo.Rosreestr.dFileTypes.KPT10) ||
                    (netFteo.Rosreestr.NameSpaces.NStoFileType(ParsedDoc.Namespace) == netFteo.Rosreestr.dFileTypes.KPT11))
                {
                    if (CadBloksList.BlockExist(ParsedDoc.District.SingleCN))
                    {
                        SearchInTreeNodes(ParsedDoc.District.SingleCN, treeView1);
                        TCadastralBlock block = CadBloksList.GetBlock(ParsedDoc.District.SingleCN);
                        wzlBlockEd blEd = new wzlBlockEd();
                        blEd.ImportXMLKPT(FileName, block, CF.conn);
                    }
                    else
                    {
                        // Need new Block
                        TCadastralBlock Block = new TCadastralBlock(ParsedDoc.District.SingleCN);
                        Block.Parent_id = CF.Cfg.District_id;
                        //wzParcelfrm ParcelEd = new wzParcelfrm();
                        //ParcelEd.CF.conn = CF.conn;
                        if (AddBlock(Block, CadBloksList, treeView1))
                        {
                            TreeNode NewBlockNode = SearchInTreeNodes(Block.CN, treeView1);
                            wzlBlockEd blEd = new wzlBlockEd();
                            blEd.ImportXMLKPT(FileName, Block, CF.conn);
                            //insertItem(Block, treeView1);
                        }
                        else
                            MessageBox.Show(DBWrapper.LastErrorMsg, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                }

                //case 2 - KVZU_08 (FGIS ir EGRN, motherfuka etc)
                if (netFteo.Rosreestr.NameSpaces.NStoFileType(ParsedDoc.Namespace) == netFteo.Rosreestr.dFileTypes.KVZU_08)
                {
                    if (CadBloksList.BlockExist(ParsedDoc.District.SingleCN))
                    {
                        SearchInTreeNodes(ParsedDoc.District.SingleCN, treeView1);
                        TCadastralBlock block = CadBloksList.GetBlock(ParsedDoc.District.SingleCN);
                        if (block.Parcels.Count() == 0) //may be not loaded, but present in DB
                        {
                            TParcels reloadP = LoadParcelsList(CF.conn, block.id);
                            block.Parcels.AddParcels(reloadP);
                        }
                        wzParcelfrm ParcelEd = new wzParcelfrm();
                        ParcelEd.CF.conn = CF.conn;
                        if (ParsedDoc.District.Blocks[0].Parcels.Count() == 1)
                        {
                            string ParcelCN = ParsedDoc.District.Blocks[0].Parcels[0].CN;
                            if (block.ParcelExist(ParcelCN))
                            {
                                TParcel TargetParcel = block.GetParcel(ParcelCN);
                                if (ParcelEd.ImportXMLVidimus(FileName, TargetParcel))
                                {
                                    StatusLabel_DBName.Text = ParcelCN;
                                    StatusLabel_AllMessages.Text = "Файл добавлен ";
                                }
                            }
                            else
                            {
                                //no parcels exist in the block, create new:
                                TreeNode ParentNode = SearchInTreeNodes(ParsedDoc.District.SingleCN, treeView1);
                                if (AddParcel(ParsedDoc.District.Blocks[0].Parcels[0], block, ParentNode))
                                {
                                    if (
                                    ParcelEd.ImportXMLVidimus(FileName, ParsedDoc.District.Blocks[0].Parcels[0]))
                                    {
                                        StatusLabel_DBName.Text = ParcelCN;
                                        StatusLabel_AllMessages.Text = "Файл добавлен ";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Need new Block
                        TCadastralBlock Block = new TCadastralBlock(ParsedDoc.District.SingleCN);
                        Block.Parent_id = CF.Cfg.District_id;
                        wzParcelfrm ParcelEd = new wzParcelfrm();
                        ParcelEd.CF.conn = CF.conn;
                        if (AddBlock(Block, CadBloksList, treeView1))
                        {
                            TreeNode NewBlockNode = SearchInTreeNodes(Block.CN, treeView1);
                            foreach (TParcel item in ParsedDoc.District.Blocks[0].Parcels)
                            {
                                if (AddParcel(item, Block, NewBlockNode))
                                {
                                    ParcelEd.ImportXMLVidimus(FileName, item);
                                }
                            }
                        }
                        else
                            MessageBox.Show(DBWrapper.LastErrorMsg, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                }
            }

            //may be here packet with RR response: zip archive, containing xml with detached signature
            if (EXTention.Equals(".ZIP"))
            {
                //1: unzip files to tmp
                {
                    CurrentDraggedFile = FileName; // store to global var
                    /*
                    BackgroundWorker w1 = new BackgroundWorker();
                    w1.WorkerSupportsCancellation = false;
                    w1.WorkerReportsProgress = true;
                    w1.DoWork += this.UnZipit;
                    w1.RunWorkerCompleted += this.UnZipComplete;
                    w1.RunWorkerAsync(CurrentDraggedFile);
                    */
                    RRTypes.CommonParsers.Doc2Type parser = new RRTypes.CommonParsers.Doc2Type();
                    parser.UnZipToCNFolders(FileName, UnZipitNoBackgrounds(FileName));
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

                    Archive_Folder = CF.Cfg.Folder_Unzip + "\\" + Path.GetFileNameWithoutExtension((string)e.Argument);
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

        private string UnZipitNoBackgrounds(string Argument)
        {
            try
            {
                ReadOptions ro = new ReadOptions() { Encoding = Encoding.ASCII };
                zip = ZipFile.Read(Argument, ro);
                Archive_Folder = CF.Cfg.Folder_Unzip + "\\" + Path.GetFileNameWithoutExtension(Argument);
                {
                    zip.ExtractAll(Archive_Folder);
                }
                return Archive_Folder;
            }
            catch (Exception ex1)
            {
                string CatchText = ex1.ToString();
                return "fuck up";
            }
        }

        //Распаковка окончена, читаем результаты:
        private void UnZipComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            
            Console.WriteLine("Parse archieve file "+ Path.GetFileName( CurrentDraggedFile));
            if (Directory.Exists(Archive_Folder))
            {
                DirectoryInfo di = new DirectoryInfo(Archive_Folder);
                string firstFileName = di.GetFiles().Select(fi => fi.Name).FirstOrDefault(name => name != "*.xml");

                if ((File.Exists(Archive_Folder + "\\" + firstFileName))
                    &&
                     (Path.GetExtension(firstFileName.ToUpper()).Equals(".XML")))
                {

                    Stream fs = new MemoryStream(File.ReadAllBytes(Archive_Folder + "\\" + firstFileName));
                    string TargetDirectoryName = ParseResponse(fs);
                    if (TargetDirectoryName.Contains("FILE TYPE WRONG"))
                    Console.WriteLine(firstFileName + " " + TargetDirectoryName);

                    string[] CNs = TargetDirectoryName.Split(':');

                    string DraggedFromPath = Path.GetDirectoryName(CurrentDraggedFile);


                    if ((CNs != null) && (CNs.Count() == 3)) // check CN corrects
                    {
                        string Target_Folder_Path = DraggedFromPath + "\\" + CNs[0] + "\\" + CNs[1] + "\\" + CNs[2];
                        if (Directory.Exists(Target_Folder_Path))
                        {
                            Console.WriteLine(CNs[0] + "\\" + CNs[1] + "\\" + CNs[2] + " - That path exists already.");
                            // return;
                        }

                        // Try to create the directory.
                        DirectoryInfo Target_Folder_Info = Directory.CreateDirectory(Target_Folder_Path);

                        //Copy files from disarchivedPlace to TargetDir
                        //TODO:
                        string[] filePaths = Directory.GetFiles(Archive_Folder);
                        foreach (string filename in filePaths)
                        {
                            //Do job for "filename"  
                            string str = Target_Folder_Path + "\\" + Path.GetFileName(filename);
                            if (!File.Exists(str))
                            {
                                File.Copy(filename, str);
                                Console.WriteLine("write to folder " + Target_Folder_Path);
                                Console.WriteLine("write file "+ Path.GetFileName(filename));
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Moved to public class Doc2Type
        /// </summary>
        /// <param name="CurrentDraggedFile"></param>
        private void UnZipToCNFolders(string CurrentDraggedFile)
        {
            Console.WriteLine("Parse archieve file " + Path.GetFileName(CurrentDraggedFile));
            if (Directory.Exists(Archive_Folder))
            {
                DirectoryInfo di = new DirectoryInfo(Archive_Folder);
                string firstFileName = di.GetFiles().Select(fi => fi.Name).FirstOrDefault(name => name != "*.xml");

                if ((File.Exists(Archive_Folder + "\\" + firstFileName))
                    &&
                     (Path.GetExtension(firstFileName.ToUpper()).Equals(".XML")))
                {

                    Stream fs = new MemoryStream(File.ReadAllBytes(Archive_Folder + "\\" + firstFileName));
                    string TargetDirectoryName = ParseResponse(fs);
                    if (TargetDirectoryName.Contains("FILE TYPE WRONG"))
                        Console.WriteLine(firstFileName + " " + TargetDirectoryName);

                    string[] CNs = TargetDirectoryName.Split(':');

                    string DraggedFromPath = Path.GetDirectoryName(CurrentDraggedFile);


                    if ((CNs != null) && (CNs.Count() == 3)) // check CN corrects
                    {
                        string Target_Folder_Path = DraggedFromPath + "\\" + CNs[0] + "\\" + CNs[1] + "\\" + CNs[2];
                        if (Directory.Exists(Target_Folder_Path))
                        {
                            Console.WriteLine(CNs[0] + "\\" + CNs[1] + "\\" + CNs[2] + " - That path exists already.");
                            // return;
                        }

                        // Try to create the directory.
                        DirectoryInfo Target_Folder_Info = Directory.CreateDirectory(Target_Folder_Path);

                        //Copy files from disarchivedPlace to TargetDir
                        //TODO:
                        string[] filePaths = Directory.GetFiles(Archive_Folder);
                        foreach (string filename in filePaths)
                        {
                            //Do job for "filename"  
                            string str = Target_Folder_Path + "\\" + Path.GetFileName(filename);
                            if (!File.Exists(str))
                            {
                                File.Copy(filename, str);
                                Console.WriteLine("write to folder " + Target_Folder_Path);
                                Console.WriteLine("write file " + Path.GetFileName(filename));
                            }
                        }
                    }

                }
            }
        }


        /// <summary>
        /// Parse xml document to detect CN of block
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <returns></retu
        private string ParseResponse(Stream fs)
        {

           // RRTypes.CommonParsers.Doc2Type parser = new RRTypes.CommonParsers.Doc2Type(); // construct instance without  classificators {dutilizations_v01, dAllowedUse_v02}
            netFteo.IO.FileInfo DocInfo = RRTypes.CommonParsers.ParserCommon.ParseXMLDocument(fs);

            // КПТ v10 is here?
            if ((DocInfo.DocRootName == "KPT") && (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kpt/10.0.1"))
            {
                if (DocInfo.District.Blocks.Count() == 1)
                    return DocInfo.District.SingleCN;
            }

            //kpt11
            if ((DocInfo.DocRootName == "extract_cadastral_plan_territory") && ( DocInfo.Namespace == "urn://fake/kpt/11.0.0"))
            {
                if (DocInfo.District.Blocks.Count() == 1)
                    return DocInfo.District.SingleCN;
            }
            
            return "FILE TYPE WRONG: " + DocInfo.DocType;
        }

        private void ClearFiles()
        {
            if (Directory.Exists(CF.Cfg.Folder_Unzip))
            {
                Directory.Delete(CF.Cfg.Folder_Unzip, true);
            }
        }

        /*
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
           
                    ConnectOps(CF.Cfg.District_id);
                    return true;
                }
            }
            return false;
        }
*/
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

        private void Hide_SearchTextBox(TextBox sender)
        {
            if (sender.Visible)
            {
                panel1.Visible = false;
                sender.Visible = false;
            }
        }

        // Also working code, writed before version below
        // Used MySqlDataReader
        private void AppendHistory1(MySqlConnection conn, TAppCfgRecord Config)
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
            //SelectDistrict(CF.Cfg);
        }

        private void Button_Import_Click(object sender, EventArgs e)
        {
            if (CF.Cfg.ViewLevel == ViewLevel.vlBlocks)
            {
                OpenFileDialog od = new OpenFileDialog();
                od.Filter = "Документы xml|*.xml";
                od.FileName = "";
                if (od.ShowDialog() == DialogResult.OK)
                {
                    OpenFile(od.FileName);
                }
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
            //SelectDistrict(CF.Cfg);

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



        private void SearchWithTree(object sender, EventArgs e)
        {
            TextBox SearchTbox = (TextBox)sender;
            if (SearchTbox.Visible)
            {   // начинаем с высшей ноды:
                /*
                if (searchtbox.Text == "")
                {
                    treeView1.SelectedNode = treeView1.Nodes[0]; // hi root node, seek to begin
                    treeView1.CollapseAll();
                    treeView1.Update();
                }
                TreeNode res = TreeViewFinder.SearchNodes(treeView1.Nodes[0], searchtbox.Text.ToUpper());

                if (res != null)
                {
                    treeView1.BeginUpdate();
                    treeView1.SelectedNode = res;
                    //TODO:  
                    //В случае поиска до раскрытия нод, для которых еще недогружены дочерние
                    TCadastralBlock block = CadBloksList.GetBlock(((netFteo.TreeNodeTag)res.Tag).Item_id);
                    if (block != null)
                        PrepareNode(treeView1.SelectedNode, res.Tag);
                    treeView1.SelectedNode.EnsureVisible();
                    treeView1.Update();
                    treeView1.EndUpdate();
                }
                */
                if (SearchInTreeNodes(SearchTbox.Text, treeView1) != null)
                {
                    //SearchAutoComplete.Add(SearchTextBox.Text);
                    /*
                    foreach(string item in SearchAutoComplete)
                    listBox1.Items.Add(item);*/
                }
               // SearchTbox.Focus();
            }
        }

        private TreeNode SearchInTreeNodes(string Text, TreeView TV)
        {
            if (Text == "")
            {
                TV.SelectedNode = TV.Nodes[0]; // hi root node, seek to begin
                TV.CollapseAll();
                TV.Update();
            }
            TreeNode res = TreeViewFinder.SearchNodes(TV.Nodes[0], Text.ToUpper());

            if (res != null)
            {
                TV.BeginUpdate();
                TV.SelectedNode = res;
                //TODO:  
                //В случае поиска до раскрытия нод, для которых еще недогружены дочерние
                TCadastralBlock block = CadBloksList.GetBlock(((netFteo.TreeNodeTag)res.Tag).Item_id);
                if (block != null)
                    PrepareNode(TV.SelectedNode, res.Tag);
                TV.SelectedNode.EnsureVisible();
                TV.Update();
                TV.EndUpdate();
                return res;
            }
            return null;
        }
    
        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            SearchWithTree(sender, e);
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
            if (CF.Cfg.CurrentItem.isParcel)
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
                    if (CF.Cfg.ViewLevel == ViewLevel.vlBlocks)
                    {
                        OpenFile(filename);
                    }
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
            treeView1.Visible = false;
            treeView1.Dock = DockStyle.None;
            Explorer_listView.Visible = true;
            Explorer_listView.Dock = DockStyle.Fill;
            Explorer_listView.View = View.LargeIcon;
            Explorer_listView.Items.Clear();
            History_ListBox.Visible = false;
            History_ListBox.Dock = DockStyle.None;
            History_ListBox.Items.Clear();
            Button_Import.Enabled = false;
            SearchTextBox.AutoCompleteCustomSource = SearchAutoComplete;
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
            //SelectDistrict(CF.Cfg);
        }

        private void Button_Exit_Click(object sender, EventArgs e)
        {
            GoDisconnect();
            this.Close();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Explorer_listView.Visible = false;
            Explorer_listView.Dock = DockStyle.None;
            History_ListBox.Visible = false;
            History_ListBox.Dock = DockStyle.None;
            treeView1.Visible = true;
            treeView1.Dock = DockStyle.Fill;
            Hide_SearchTextBox(SearchTextBox);
            ListBlockListTreeView(CadBloksList, treeView1);
            CF.Cfg.ViewLevel = ViewLevel.vlBlocks;
        }

        private void button_History_Click(object sender, EventArgs e)
        {
            Hide_SearchTextBox(SearchTextBox);
            treeView1.Visible = false;
            treeView1.Dock = DockStyle.None;
            CF.Cfg.ViewLevel = ViewLevel.vlHistory;
            History_ListBox.Visible = true;
            History_ListBox.Dock = DockStyle.Fill;
            //CF.Cfg.ViewMode = ViewMode.vmHistory;
        }

        private void button_Favorites_Click(object sender, EventArgs e)
        {
            Hide_SearchTextBox(SearchTextBox);
            treeView1.Visible = false;
            treeView1.Dock = DockStyle.None;
            CF.Cfg.ViewLevel = ViewLevel.vlFavorites;
           // CF.Cfg.ViewMode = ViewMode.vmFavorites;
        }

        private void ToolStripButton1_Click_1(object sender, EventArgs e)
        {
            ConnectGo();
        }

        private void КопироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                Clipboard.SetText(treeView1.SelectedNode.Text);
            }
        }

        private void УдалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Erase(CF.Cfg.CurrentItem);
        }

        private void TreeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            /*
            if (treeView1.SelectedNode != null)
                treeView1.SelectedNode.ForeColor = Color.Black;
            e.Node.ForeColor = Color.Blue;
            */
        }

        // When DrawMode= OwnerDrawText , need draw text self:
        private void TreeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node == null) return;

            // if treeview's HideSelection property is "True", 
            // this will always returns "False" on unfocused treeview
            var selected = (e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected;
            var unfocused = !e.Node.TreeView.Focused;

            // we need to do owner drawing only on a selected node
            // and when the treeview is unfocused, else let the OS do it for us
            if (selected && unfocused)
            {
                var font = e.Node.NodeFont ?? e.Node.TreeView.Font;
                e.Graphics.FillRectangle(SystemBrushes.MenuHighlight, e.Bounds);
                TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, SystemColors.HighlightText, TextFormatFlags.GlyphOverhangPadding);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void TreeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            TreeView_DrawNode(sender, e);
        }

        private void TreeView1_Enter(object sender, EventArgs e)
        {
            Button_Property.Enabled = true;
        }

        private void TreeView1_Leave(object sender, EventArgs e)
        {
            Button_Property.Enabled = false;
        }

       

        private void Button4_Click(object sender, EventArgs e)
        {
            ChangeViewLevel(CF.Cfg.ViewLevel);
            //LoadSubRF(CF.conn, Explorer_listView);
        }


        private void Explorer_listView_DoubleClick(object sender, EventArgs e)
        {
            var senderList = (ListView)sender;
            if (senderList.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = senderList.SelectedItems;
                ListViewItem lvItem = items[0];
                if (((netFteo.TreeNodeTag)lvItem.Tag).Type == "subrf")
                {
                    CF.Cfg.SubRF_Name = ((netFteo.TreeNodeTag)lvItem.Tag).Name;
                    CF.Cfg.SubRF_KN = ((netFteo.TreeNodeTag)lvItem.Tag).NameExt;
                    CF.Cfg.District_KN = "*";
                    CF.Cfg.District_Name = "*";
                    LoadDistricts(Convert.ToByte(((netFteo.TreeNodeTag)lvItem.Tag).Item_id), CF.conn, Explorer_listView);
                }
                if (((netFteo.TreeNodeTag)lvItem.Tag).Type == "netFteo.Cadaster.TCadastralDistrict")
                {
                    CF.Cfg.District_Name = ((netFteo.TreeNodeTag)lvItem.Tag).Name;
                    CF.Cfg.District_KN = ((netFteo.TreeNodeTag)lvItem.Tag).NameExt;
                    LoadBlocks(((netFteo.TreeNodeTag)lvItem.Tag).Item_id, CF.Cfg, treeView1,CF.Cfg.District_Name );
                    StatusLabel_SubRf_CN.Text = CF.Cfg.SubRF_Name + " " + CF.Cfg.District_Name;
                    Explorer_listView.Visible = false;
                    Explorer_listView.Dock = DockStyle.None;
                    treeView1.Visible = true;
                    treeView1.Dock = DockStyle.Fill;
                }
            }
        }

        private void ChangeViewLevel(ViewLevel vl)
        {
            switch (vl)
            {
                case ViewLevel.vlExploreSubRF: { return; } // nothing  - Top level
                case ViewLevel.vlExploreDistricts: { LoadSubRF(CF.conn, Explorer_listView); return; }
                case ViewLevel.vlBlocks: { LoadDistricts(CF.Cfg.Subrf_id, CF.conn, Explorer_listView); return; }
                case ViewLevel.vlFavorites: { return; }
                case ViewLevel.vlHistory: { return; }
                default: { return; }
            }
        }

        private void ToolStripButton1_Click_2(object sender, EventArgs e)
        {
            //back button:
            ChangeViewLevel(CF.Cfg.ViewLevel);
            /*
            switch (CF.Cfg.ViewLevel)
            {
                case ViewLevel.vlExploreSubRF: { return; } // nothing  - Top level
                case ViewLevel.vlExploreDistricts: { LoadSubRF(CF.conn, Explorer_listView); return; }
                case ViewLevel.vlBlocks: { LoadDistricts(CF.Cfg.Subrf_id, CF.conn, Explorer_listView); return; }
                case ViewLevel.vlFavorites: { return; }
                case ViewLevel.vlHistory: { return; }
                default: { return; }
            }
            */
        }

        private void ToolStripButton_Forward_Click(object sender, EventArgs e)
        {
            //forward button:
            /*
            switch (CF.Cfg.ViewLevel)
            {
                case ViewLevel.vlExploreSubRF: { return; } // nothing  - Top level
                case ViewLevel.vlExploreDistricts: { LoadSubRF(CF.conn, Explorer_listView); return; }
                case ViewLevel.vlBlocks: { LoadDistricts(CF.Cfg.Subrf_id, CF.conn, Explorer_listView); return; }
                case ViewLevel.vlFavorites: { return; }
                case ViewLevel.vlHistory: { return; }
                default: { return; }
            }
             */
        }

        private void Explorer_listView_KeyUp(object sender, KeyEventArgs e)
        {
            ChangeObj(sender, e);
        }

        private void Explorer_listView_ItemActivate(object sender, EventArgs e)
        {
            ChangeObj(sender, e);
        }

        private void ToolStripButton2_Click_1(object sender, EventArgs e)
        {
            AddItem(CF.Cfg.CurrentItem, CF.Cfg.ViewLevel);
        }

        private void ДобавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddItem(CF.Cfg.CurrentItem, CF.Cfg.ViewLevel);
        }
    }

    /*
     * 
     */

}
