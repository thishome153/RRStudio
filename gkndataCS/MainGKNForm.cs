using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
//using namespace System.Xml;
using MySql.Data.MySqlClient;
using netFteo;
using netFteo.Spatial;
namespace GKNData

{
    public partial class MainGKNForm : Form
    {

        private DataTable data;
        private MySqlDataAdapter da;
        private MySqlCommandBuilder cb;
        ConnectorForm CF = new ConnectorForm();
        public  TMyBlockCollection CadBloksList; // Глобальный перечень кварталов

        public MainGKNForm()
        {
            InitializeComponent();
            treeView1.BeforeExpand += OnItemexpanding; //Подключаем обработчик раскрытия
            Application.Idle += On_Iddle;
            treeView1.Nodes.Clear();
        #if DEBUG
            this.Text = "ГКН Дата. debug version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Button_Import.Enabled = true;
            toolStripButton_Exit.Visible = true;
        #else
            this.Text = "ГКН Дата " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Button_Import.Enabled = false;
            toolStripButton_Exit.Visible = false;
#endif
        }

        #region Функции

        private void On_Iddle(Object sender, EventArgs e)
        {

            StatusLabel_DBName.Image = null;
            StatusLabel_DBName.ToolTipText = "-";
            StatusLabel_SubRf_CN.Text = "-";
            StatusLabel_SubRf_CN.ToolTipText = "-";
            StatusLabel_AllMessages.Text = "-";
            

            if (CF.conn != null)
            {
                StatusLabel_DBName.Text = CF.conn.DataSource +":/"+ CF.conn.Database;
                if (CF.conn.State == ConnectionState.Open)
                {
                    StatusLabel_DBName.ToolTipText = "Соединено";//CF.conn.State.ToString();
                    StatusLabel_DBName.Image = GKNData.Properties.Resources.accept;
                    StatusLabel_SubRf_CN.Text = CF.Cfg.SubRF_KN + ":" + CF.Cfg.District_KN;
                    StatusLabel_SubRf_CN.ToolTipText = CF.Cfg.SubRF_Name + ":" + CF.Cfg.District_Name;
                }
                else
                {
                    StatusLabel_DBName.Image = GKNData.Properties.Resources.cross;
                    StatusLabel_DBName.ToolTipText = "Отключено";
                    StatusLabel_CurrentItem.Text = "-";
                }
            }
        }

        private bool GoConnect()
        {
            CF.Cfg.CfgRead(); // Загрyзимся из реестра
          //  if (CF.ShowDialog() == DialogResult.Yes)
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

                    if (CF.conn.State == ConnectionState.Open)
                    {
                        //CF.conn.ChangeDatabase(CF.Cfg.DatabaseName);
                       // // if (SelectDistrict(CF.Cfg)) //Загрузим прошлый регион/район
                        {
                            CadBloksList = LoadBlockList(CF.conn, CF.Cfg.District_id);
                            ListBlockListTreeView(CadBloksList, treeView1);
                            this.treeView1.BackColor = Color.White;
                        }
                        // else
                        // {
                        //     StatusLabel_DBName.Text = "";
                        // }
                        return true;
                    }
                    else
                        this.treeView1.BackColor = Color.DarkRed;
                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show("Error connecting to the server: " + ex.Message);
                    StatusLabel_DBName.ToolTipText = "Ошибка соединения " + CF.conn.Database;
                    StatusLabel_DBName.Image = GKNData.Properties.Resources.cross;
                    StatusLabel_AllMessages.Text = ex.Message;
                    this.treeView1.BackColor = Color.DarkRed;
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
                StatusLabel_SubRf_CN.Text = "Отключено";
                treeView1.BeginUpdate();
                treeView1.Nodes.Clear();
                treeView1.EndUpdate();
                this.treeView1.BackColor = Color.DarkGray;
            }
            StatusLabel_AllMessages.Text = "";

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

            if ((ENode != null)  && (ENode.Tag != null))
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
                StatusLabel_CurrentItem.Text ="id " + CF.Cfg.CurrentItem.Item_id.ToString();
            }
        }
    
        //теперь Проверка количества зу в квартале:
        private bool CheckParcels(MySqlConnection conn, int block_id)
        {
            if (conn == null) return false;
            TMyParcelCollection ParcelsList = new TMyParcelCollection();
            data = new DataTable();
            da = new MySqlDataAdapter("SELECT COUNT(*) FROM lottable where lottable.block_id = " + block_id.ToString(),
                                      conn);
            da.Fill(data);
            DataRow res = data.Rows[0];
            return (Convert.ToInt32(res[0]) > 0);
        }

//-------------------------------- Редактируем -------------------------------
        private bool Edit(TCurrentItem Item)
        {
            if (this.CF.conn.State == ConnectionState.Closed) return false;
            if (Item.Item_TypeName == "netFteo.Spatial.TMyCadastralBlock")
            {
                if (Edit(CadBloksList.GetBlock(Item.Item_id))) // реентерабельно вызываем
                {
                 // ОБновим ноду на всякой случай
                  treeView1.SelectedNode.ToolTipText = Item.Item_id.ToString()+ "*";
                  treeView1.SelectedNode.Text = ((TMyCadastralBlock)treeView1.SelectedNode.Tag).CN; 
                }
            }

            if (Item.Item_TypeName == "netFteo.Spatial.TMyParcel")
            {
                if (Edit(CadBloksList.GetParcel(Item.Item_id))) // реентерабельно вызываем
                {
                 // ОБновим ноду на всякой случай
                  treeView1.SelectedNode.Text = ((TMyParcel)treeView1.SelectedNode.Tag).CN;
                //treeView1.SelectedNode.ToolTipText = "item " + Item.Item_id.ToString()+ "отредактирован";
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
                TMyParcel parcel = new TMyParcel(row[1].ToString(),Convert.ToInt32(row[0])); // CN , id
                parcel.Name = row[3].ToString();
                parcel.CadastralBlock_id = Convert.ToInt32(row[4]); // block_id
                parcel.SpecialNote = row[5].ToString(); // lot_comment
                parcel.AreaGKN     = row[6].ToString();
                ParcelsList.Parcels.Add(parcel);
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
            da = new MySqlDataAdapter("SELECT kpt_id,block_id,xml_file_name,kpt_num,kpt_date,kpt_serial,xml_ns,requestnum,acesscode,"+
                   " OCTET_LENGTH(xml_file_body)/1024 as xml_size_kb from kpt where block_id = " + block_id.ToString() +
                                      " order by kpt_id asc", conn);

            da.Fill(data);
            foreach (DataRow row in data.Rows)
            {
                TFile file = new TFile(); // CN
                file.id = Convert.ToInt32(row[0]);           // id
                file.FileName= row[2].ToString();                       // block_name
                file.Number = row[3].ToString();
                file.Doc_Date=  Convert.ToString(row[4]).Substring(0,Convert.ToString(row[4]).Length-7);
                // срезать семь нулей времени MySQL "05.04.2016 0:00:00"
                file.Serial = row[5].ToString();
                file.xmlns = row[6].ToString();
                file.RequestNum = row[7].ToString();
                file.AccessCode = row[8].ToString();
                if (row[9] != DBNull.Value)
                file.xmlSize_SQL = Convert.ToDouble(row[9]);
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
                file.xmlSize_SQL = Convert.ToDouble(row[9]);
                //file.id = Convert.ToInt32(row[10]); // vidimus_type, int
                files.Add(file);
            }
            return files;
        }
       

        //Одуренная поцедура Заполенния Полями Таблиц
        private TMyBlockCollection LoadBlockList(MySqlConnection conn, int distr_id)
        {
            if (conn == null) return null; if (conn.State != ConnectionState.Open) return null;
            StatusLabel_AllMessages.Text = "Загрузка кварталов.... ";
            TMyBlockCollection CadBloksList = new TMyBlockCollection();
            CadBloksList.DistrictName = distr_id.ToString();

            data = new DataTable();
            da = new MySqlDataAdapter("SELECT * FROM blocks where blocks.district_id =" + distr_id.ToString() +
                                      " order by blocks.block_kn asc", conn);


            //da.Fill(data);

            var dataReader = da.SelectCommand.ExecuteReader();

            var columns = new List<string>();

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                DataColumn dc = new DataColumn(dataReader.GetName(i));
                dc.DataType = dataReader.GetFieldType(i);
                data.Columns.Add(dc);
            }

            foreach (DataColumn field in data.Columns)
            {
                string FName = field.ColumnName;
            }


            toolStripProgressBar1.Maximum = 250;// data.Rows.Count; // TODO ??? 
   
            while (dataReader.Read())
            {
                data.Rows.Add(dataReader);
                // Update progress view..
                toolStripProgressBar1.Value++;
            }

            

            StatusLabel_AllMessages.Text = "Кварталов: "+ data.Rows.Count;
            // z.b.: Доступ к полям:
            //foreach (DataColumn column in data.Columns) //values of each column
            //DataColumn Field_CN = data.Columns[1]; //values of column 1
            //DataColumn Field_id = data.Columns[0]; //values of column 1

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
            return CadBloksList;
        }

        //Одуренная поцедура Заполенния TreeView Полями Класса TCadastralBlockList:
        private void ListBlockListTreeView(netFteo.Spatial.TMyBlockCollection List, TreeView WhatTree)
        {
            if (List == null) return;
            WhatTree.Nodes.Clear();
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
        private TreeNode insertItem(TCadastralDistrict  District, TreeView hParent)
        {
            TreeNode nn = hParent.Nodes.Add(District.CN);//nodeName);
            nn.Tag = District;
            if (District.Name != null) nn.ToolTipText = District.Name;

            nn.NodeFont = new Font("Arial", 12);//, FontStyle.Bold);
            nn.ImageIndex = 0; nn.SelectedImageIndex = 0;
            return nn;
        }

        //Добавление квартала
        private TreeNode insertItem(TMyCadastralBlock item,  TreeView hParent)
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
        private TreeNode insertItem(TMyParcel node,TreeNode hParent)
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
        private bool populateNode(object  inNode, TreeNode inTreeNode)
        {
            string nodeType;
            //TreeNode popuToNode;
            // что в ноде:
             nodeType = inNode.ToString();
             if (nodeType == "netFteo.Spatial.TMyCadastralBlock")
             {
                 // Еще не загружены ли ЗУ
                 if ((((TMyCadastralBlock)inNode).HasParcels) && (((TMyCadastralBlock)inNode).Parcels.Parcels.Count == 0))
                 {
                     TMyParcelCollection reloadP = LoadParcelsList(CF.conn, ((TMyCadastralBlock)inNode).id);
                     ((TMyCadastralBlock)inNode).Parcels.AddParcels(reloadP);
                 }
                 {
                     foreach (TMyParcel parcel in ((TMyCadastralBlock)inNode).Parcels.Parcels)
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
            string extention = Path.GetExtension(FileName).ToUpper();

            // got spatial file of several kind:
            if ((extention.Equals(".MIF")) ||
                (extention.Equals(".DXF")) ||
                (extention.Equals(".TXT"))  )
            {
                XMLReaderCS.KVZU_Form frmReader = new XMLReaderCS.KVZU_Form();
                frmReader.StartPosition = FormStartPosition.Manual;
                frmReader.Tag = 3; // XMl Reader в составе приложения
                frmReader.Read(FileName, false);
                frmReader.Left = this.Left + 25; frmReader.Top = this.Top + 25;
                frmReader.ShowDialog(this);
            }

            if (extention.Equals(".XML"))
            {
                System.IO.TextReader reader = new System.IO.StreamReader(FileName);
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                xml.Load(reader);
                StatusLabel_AllMessages.Text = xml.DocumentElement.Name.ToString();
                XMLReaderCS.KVZU_Form MyReader = new XMLReaderCS.KVZU_Form();
                MyReader.Read(xml);
                MyReader.ShowDialog();
            }


        }




        bool SelectDistrict(TAppCfgRecord CfgRec)
        {
            if (this.CF.conn.State == ConnectionState.Closed) return false;
            SubRFForm SubSelectfrm = new SubRFForm(CF.conn);
            //SubSelectfrm.StartPosition = FormStartPosition.Manual;
            //SubSelectfrm.Location = new Point(this.Top+10, this.Left+10);

            if (SubSelectfrm.ShowDialog() == DialogResult.Yes)
            {
                CfgRec.Subrf_id = SubSelectfrm.subrf_id;
                CfgRec.SubRF_KN = SubSelectfrm.subrf_kn;
                CfgRec.SubRF_Name = SubSelectfrm.subrf_Name;

                DistrictForm DistrSelectfrm = new DistrictForm(CF.conn, CfgRec.Subrf_id);
                if (DistrSelectfrm.ShowDialog() == DialogResult.Yes)
                {
                    CfgRec.District_id = DistrSelectfrm.district_id;
                    CfgRec.District_KN = DistrSelectfrm.district_kn;
                    CfgRec.District_Name = DistrSelectfrm.district_Name;
                    StatusLabel_SubRf_CN.Text = CfgRec.SubRF_Name + " " + CfgRec.District_Name;
                    CfgRec.CfgWrite();
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
        #endregion

        #region Назначенные Обработчики событий
        private void MenuItem_Connect_Click(object sender, EventArgs e)
        {
            GoConnect();
        }
        private void Button_Connect_Click(object sender, EventArgs e)
        {
            GoConnect();
        }
        private void Button_ChangeSub_Click(object sender, EventArgs e)
        {
            if (SelectDistrict(CF.Cfg))
            {
                CadBloksList = LoadBlockList(CF.conn, CF.Cfg.District_id);
                ListBlockListTreeView(CadBloksList, treeView1);
            }
        }

        private void Button_Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Документы xml|*.xml";
            od.FileName = "";
            if (od.ShowDialog() == DialogResult.OK)
            {
                OpenFile (od.FileName);
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
            CadBloksList = LoadBlockList(CF.conn, CF.Cfg.District_id);
            ListBlockListTreeView(CadBloksList, treeView1);
        }

        private void Button_Property_Click(object sender, EventArgs e)
        {
            Edit(CF.Cfg.CurrentItem);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            GoConnect();
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

                TreeNode res = netFteo.TreeViewFinder.SearchNodes(treeView1.Nodes[0], searchtbox.Text.ToUpper());

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
                TreeNode res = netFteo.TreeViewFinder.SeekNode(netFteo.TreeViewFinder.SearchNextNode(treeView1.SelectedNode), SearchTextBox.Text.ToUpper());
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
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Count() == 1)
            {
              OpenFile(files[0]);
            }
        }
    }
}
