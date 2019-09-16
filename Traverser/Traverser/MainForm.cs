using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Net; // IPhost
using System.Net.Sockets; // AdressFamily

//Отсюда - для Dxf, также добавить в ссылки проекта netDxf
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using netFteo.NikonRaw;
using netFteo.BaseClasess;

using netDxf;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Header;
using netDxf.Objects;
using netDxf.Tables;
using Group = netDxf.Objects.Group;
using Point = netDxf.Entities.Point;
using Attribute = netDxf.Entities.Attribute;
using Image = netDxf.Entities.Image;


namespace Traverser
{
    public partial class MainForm : Form
    {        
        TraverserProject Project = new TraverserProject();
       
        public MainForm()
        {
            InitializeComponent();
          
       }


        private void опрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm AbForm = new AboutForm();
            AbForm.ShowDialog(this);
        }

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            AboutForm AbForm = new AboutForm();
            AbForm.ShowDialog(this);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.Project != null)
            {
                this.Project = new TraverserProject();
            }

            this.Project.ProjectName = "НовыйПроект";
            this.Project.Points.ClearItems();
            this.RawtreeView.Nodes.Clear();
            this.dataGridView1.DataSource = null;
            this.ListNumTxtObjects(this.Project.Points.Points);
            this.Text = "Fixosoft Nikon Traverser " + this.Project.ProjectName;
        }

        #region//--------Открытть файл с Проектом
        //----------------------------------------------------------------------------------------
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.FilterIndex = 2;
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                // xml desrialize:
                if (Path.GetExtension(openFileDialog1.FileName).Equals(".xml"))

                  {
                    XmlSerializer serializer = new XmlSerializer(typeof(TraverserProject));
                    TextReader reader = new StreamReader(openFileDialog1.FileName);
                    RAWBodyrichTextBox.Clear();
                    RAWBodyrichTextBox.Text = reader.ReadToEnd();
                    reader.Dispose();
                    reader = new StreamReader(openFileDialog1.FileName); //еще разок зачитаем для Десериализатора
                    try
                    { Project = (TraverserProject)serializer.Deserialize(reader); 
                      reader.Close();
                      }
                    catch (InvalidOperationException ex)
                    {
                        //  MessageBox.Show(ex.ToString());
                        toolStripStatusLabel1.Text = Path.GetFileName(openFileDialog1.FileName) + " ощибка формата";
                        RawtreeView.Nodes.Clear();
                        PointsdataGridView.DataSource = null;// listView1.Items.Clear();
                        this.Text = "Fixosoft Nikon Traverser ";
                        goto GoodBy;
                    }
                  }

            
             
                TMyOutLayer Layer = new TMyOutLayer();
                Layer.ImportObjects(Project.Points.Points);
                double TestArea = Layer.Area;

                this.ListNumTxtObjects(Project.Points.Points);
                FillRawtreeSTOnly (Project.Raw);
                ListVertex();
                ListTraverseMapping();
                this.Text = "Fixosoft Nikon Traverser " + this.Project.ProjectName;
                toolStripStatusLabel1.Text = Path.GetFileName(openFileDialog1.FileName) + " cохранен " + Project.SavingDateTime;
                toolStripStatusLabel2.Text = "Area [" + Convert.ToString(Layer.Points.Count) + "] " + Convert.ToString(Layer.Area);
            }
        GoodBy: ;
      }
        //----------------------------------------------------------------------------------------
#endregion

     
        #region Список точек в gridView 

        private void ListNumTxtObjects(BindingList<TmyPointO> Points)
        {
            PointsdataGridView.AutoGenerateColumns = false;
            PointsdataGridView.Columns.Clear();

            DataGridViewTextBoxColumn NameCL = new DataGridViewTextBoxColumn();
            NameCL.DataPropertyName = "NumGeopointA";
            NameCL.HeaderText = "Имя";
            PointsdataGridView.Columns.Add(NameCL);

            DataGridViewTextBoxColumn xCL = new DataGridViewTextBoxColumn();
            xCL.DataPropertyName = "x";
            xCL.HeaderText = "x";
            PointsdataGridView.Columns.Add(xCL);

            DataGridViewTextBoxColumn yCL = new DataGridViewTextBoxColumn();
            yCL.DataPropertyName = "y";
            yCL.HeaderText = "y";
            PointsdataGridView.Columns.Add(yCL);

            DataGridViewTextBoxColumn zCL = new DataGridViewTextBoxColumn();
            zCL.DataPropertyName = "z";
            zCL.HeaderText = "z";
            PointsdataGridView.Columns.Add(zCL);

            DataGridViewTextBoxColumn MtCL = new DataGridViewTextBoxColumn();
            MtCL.DataPropertyName = "Mt";
            MtCL.HeaderText = "Mt";
            PointsdataGridView.Columns.Add(MtCL);

            DataGridViewTextBoxColumn CodeCL = new DataGridViewTextBoxColumn();
            CodeCL.DataPropertyName = "Code";
            CodeCL.HeaderText = "Код";
            PointsdataGridView.Columns.Add(CodeCL);

            DataGridViewTextBoxColumn DesCL = new DataGridViewTextBoxColumn();
            DesCL.DataPropertyName = "Description";
            DesCL.HeaderText = "Описание";
            PointsdataGridView.Columns.Add(DesCL);
            PointsdataGridView.DataSource = Points;
        }
        private void ListNumTxtPolygons(TPolygonCollection PC)
        {
            TMyPoints FullList = new TMyPoints();
            
            for (int ic = 0; ic <= PC.Items.Count - 1; ic++)
            {
                //ListNumTxtObjects(PC.Items[ic].Points);
                FullList.AppendPoints(PC.Items[ic]);
                 for (int icc = 0; icc <= PC.Items[ic].Childs.Count - 1; icc++)
                 FullList.AppendPoints(PC.Items[ic].Childs[icc]);
            }
            ListNumTxtObjects(FullList.Points);

        }
        #endregion



               
        #region Запись в DXF
        //------------------------------------------------------------------------------------------
        private  void WriteDxf(string Filename)
        {

        /*    // sample.dxf contains all supported entities by netDxf
            string file = "Source_binary.dxf";
            bool isBinary;
            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file, out isBinary);
            if (dxfVersion < DxfVersion.AutoCad2000)
            {
                Console.WriteLine("THE FILE {0} IS NOT A VALID DXF", file);
                Console.WriteLine();

                Console.WriteLine("FILE VERSION: {0}", dxfVersion);
                Console.WriteLine();

                Console.WriteLine("Press a key to continue...");
                Console.ReadLine();
                
                return;
            }
            //Существующий
            //DxfDocument dxf =  DxfDocument.Load(file);
         */
            //новый dxf
            DxfDocument dxf = new DxfDocument();
            //DxfVersion dxfVersion = new DxfVersion();

            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            
           //Список Vertexов (вершин) полилинии:
            List<PolylineVertex> PlVertexList =new  List<PolylineVertex>();
            PolylineVertex Vertex = new PolylineVertex(0,0,0);
            PlVertexList.Add(Vertex);
            PlVertexList.Add(new PolylineVertex(0, 117, 0));
            PlVertexList.Add(new PolylineVertex(117, 117, 0));
            PlVertexList.Add(new PolylineVertex(130, 0, 0));
            
            //Сама полилиния, замкнутая true:
            Polyline Pline = new Polyline(PlVertexList, true);
            //Вгоняем в dxf:
            dxf.AddEntity(Pline);
            dxf.Save(Filename);// "sample 2004.dxf"); 
        }
        

        private void dxfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                WriteDxf(saveFileDialog1.FileName);
        }
        #endregion
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        //Serialize my Types
        private void xmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            saveFileDialog1.FilterIndex = 2; //xml
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TMyPoints));
                TextWriter writer = new StreamWriter(saveFileDialog1.FileName);
                serializer.Serialize(writer, Project.Points);
                writer.Close();
            }
        }

      

        #region  // пробуем грузить Nikon RAW data format V2.00
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenRawFile(); 
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            OpenRawFile();
        }
        private void OpenRawFile()
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (Path.GetExtension(openFileDialog1.FileName).Equals(".txt"))
                {
                    System.IO.TextReader readFile = new StreamReader(openFileDialog1.FileName);
                    string line = null;

                    if (readFile.Peek() != -1)
                        line = readFile.ReadLine();
                    RAWBodyrichTextBox.Clear();
                    RAWBodyrichTextBox.Text = readFile.ReadToEnd();
                    if (line != null) //Читаем строку, Проверим форомат файла: 
                        if (line.Contains("CO,Nikon RAW data format V2.00"))
                        {
                            Project.Raw.ImportTxtRawFile(openFileDialog1.FileName);

                            toolStripStatusLabel1.Text = Path.GetFileName(openFileDialog1.FileName);
                            toolStripStatusLabel2.Text = "ST = " + Convert.ToString(Project.Raw.ST.Count);
                            FillRawtreeSTOnly (Project.Raw);
                         }

                }
            }
        }
        private void FillRawtree(TNikonRaw raw)
        {
            RawtreeView.Nodes.Clear();
            RawtreeView.Nodes.Add(raw.CO_Instrument + " " + raw.CO_S_N);
            for (int i = 0; i <= raw.ST.Count-1; i++)
            {
               TreeNode StNode = RawtreeView.Nodes.Add(raw.ST[i].StationName);
               StNode.Tag = raw.ST[i].id;
               for (int ii = 0; ii <= raw.ST[i].SS.Count-1; ii++)
                {
                   TreeNode SSNode = StNode.Nodes.Add(raw.ST[i].SS[ii].TargetName);
                   SSNode.Nodes.Add(Convert.ToString(raw.ST[i].SS[ii].TargetHeight));

                   TreeNode HANode = SSNode.Nodes.Add(Convert.ToString(raw.ST[i].SS[ii].HA));
                   HANode.ToolTipText ="HA";
                   TreeNode VANode = SSNode.Nodes.Add(Convert.ToString(raw.ST[i].SS[ii].VA_degree ));

                   VANode.ToolTipText = "M0 " + raw.ST[i].Properties.Zero_VA_To_String();
                   SSNode.Nodes.Add(Convert.ToString(raw.ST[i].SS[ii].HorizontalDistantion));
                   SSNode.Nodes.Add(Convert.ToString(raw.ST[i].SS[ii].SlopeDistantion));
                   SSNode.Nodes.Add(raw.ST[i].SS[ii].TimeLabel);
                    TreeNode CodeNode= SSNode.Nodes.Add(raw.ST[i].SS[ii].Code);
                      CodeNode.ToolTipText = "Код";
                   SSNode.Tag = raw.ST[i].SS[ii].id;
                }
            
            }
        }
        private void FillRawtreeSTOnly(TNikonRaw raw)
        {
            RawtreeView.Nodes.Clear();
            dataGridView1.DataSource = null;
           // RawtreeView.Nodes.Add(raw.CO_Instrument + " " + raw.CO_S_N);
            for (int i = 0; i <= raw.ST.Count - 1; i++)
            {
                TreeNode StNode = RawtreeView.Nodes.Add(raw.ST[i].StationName);
                StNode.Tag = raw.ST[i].id;
            }
        }
        private List<TRawObservation> ListObservations(int StationID)
        {

            if (this.Project.Raw.GetStation(StationID) != null)
            {
                TStation Station = this.Project.Raw.GetStation(StationID);
                List<TRawObservation> SS = Station.GetObservations();
                //List<TRawObservation> SS = List<TRawObservation>(Station.GetObservations());


                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.Columns.Clear();

                DataGridViewTextBoxColumn TargetNameCL = new DataGridViewTextBoxColumn();
                TargetNameCL.DataPropertyName = "TargetName";
                TargetNameCL.HeaderText = "Цель";
                dataGridView1.Columns.Add(TargetNameCL);

                DataGridViewCheckBoxColumn TraverseVertexCL = new DataGridViewCheckBoxColumn();
                TraverseVertexCL.DataPropertyName = "isTravVertex";
                TraverseVertexCL.ThreeState = false;
                TraverseVertexCL.HeaderText = "Вершина";
                dataGridView1.Columns.Add(TraverseVertexCL);

                DataGridViewTextBoxColumn TargetHACL = new DataGridViewTextBoxColumn();
                TargetHACL.DataPropertyName = "HA";
                TargetHACL.HeaderText = "HA";
                dataGridView1.Columns.Add(TargetHACL);

                DataGridViewTextBoxColumn TargetVACL = new DataGridViewTextBoxColumn();
                TargetVACL.DataPropertyName = "VA_degree";
                TargetVACL.HeaderText = "VA";
                dataGridView1.Columns.Add(TargetVACL);

                DataGridViewTextBoxColumn sdCL = new DataGridViewTextBoxColumn();
                sdCL.DataPropertyName = "SlopeDistantion";
                sdCL.HeaderText = "Накл. расст.";
                dataGridView1.Columns.Add(sdCL);

                DataGridViewTextBoxColumn TargetCodeCL = new DataGridViewTextBoxColumn();
                TargetCodeCL.DataPropertyName = "Code";
                TargetCodeCL.HeaderText = "Code";
                dataGridView1.Columns.Add(TargetCodeCL);

               



                dataGridView1.DataSource = SS;
                return SS;
            }
            else return null;
        }


        #endregion

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            OpenXYfile();
        }


      


        // Сохранить проект:
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FilterIndex = 2; //xml
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TraverserProject));
                Project.ProjectName = Path.GetFileNameWithoutExtension(saveFileDialog1.FileName);
                Project.SavingDateTime = DateTime.Now.ToString();
                Project.HostName = System.Environment.MachineName;
                Project.UserDomainName = System.Environment.UserDomainName + "/" + System.Environment.UserName;
                Project.HostIP = LocalIPAddress();
                TextWriter writer = new StreamWriter(saveFileDialog1.FileName);
                serializer.Serialize(writer, Project);
                writer.Close();
            }
        }

        public string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        #region//--------Открытть файл с с иходными данными ---------
        private void OpenXYfile()
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {

                if (Path.GetExtension(openFileDialog1.FileName).Equals(".txt"))
                {

                    System.IO.TextReader readFile = new StreamReader(openFileDialog1.FileName);
                    string line = null;

                    if (readFile.Peek() != -1)
                        line = readFile.ReadLine();
                    RAWBodyrichTextBox.Clear();
                    RAWBodyrichTextBox.Text = readFile.ReadToEnd();
                    if (line != null) //Читаем строку, Проверим формат файла: 
                        if (line.Contains("#Fixosoft NumXYZD data format V2015"))
                        { this.Project.Polygons.ImportNXYZDFile(openFileDialog1.FileName);
                        this.Project.Points.AppendPoints(this.Project.Polygons.AsPointList());  
                          this.ListNumTxtPolygons(this.Project.Polygons);
                          toolStripStatusLabel1.Text = Path.GetFileName(openFileDialog1.FileName);
                        }
                        else
                        {
                            toolStripStatusLabel1.Text = Path.GetFileName(openFileDialog1.FileName) + " Ошибка формата файла";
                            toolStripStatusLabel2.Text = "";
                            goto Errorlab;
                        }
                }

                // xml desrialize:
                if (Path.GetExtension(openFileDialog1.FileName).Equals(".xml"))
                {

                    XmlSerializer serializer = new XmlSerializer(typeof(TMyPoints));
                    TextReader reader = new StreamReader(openFileDialog1.FileName);
                    try
                    {
                        Project.Points = (TMyPoints)serializer.Deserialize(reader);
                        reader.Close();
                    }
                    catch (InvalidOperationException ex)
                    {
                        toolStripStatusLabel1.Text = Path.GetFileName(openFileDialog1.FileName) + " Ошибка формата файла";
                        toolStripStatusLabel2.Text = "";
                        goto Errorlab;
                    }
                }

                TMyOutLayer Layer = new TMyOutLayer();
                Layer.ImportObjects(Project.Points.Points);
                if (Layer.Points.Count > 1)
                {
                    double TestArea = Layer.Area;
                    this.ListNumTxtObjects(this.Project.Points.Points);
                    toolStripStatusLabel1.Text = Path.GetFileName(openFileDialog1.FileName);
                    toolStripStatusLabel2.Text = "Area [" + Convert.ToString(Layer.Points.Count) + "] " + Convert.ToString(Layer.Area);
                }
            }

        Errorlab: ;
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            OpenXYfile();
        }
        #endregion



        






        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {  CheckBox cb = sender as CheckBox;
           if (cb.Checked)
           {
            this.textBoxAA.Text = "Установлена";
            }
               else this.textBoxAA.Clear();
        }

        private void вставкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetObservation();
            GetMappings();

            ListVertex();
            ListTraverseMapping();
        }
        
        
        
        #region Получить наблюдения хода
        /// <summary>
      /// Получить наблюдения хода
      /// </summary>
        private void GetObservation()
        { this.Project.Travers.ClearVertexs();
        this.Project.Travers.ImportSourcePoints(this.Project.Points);
          for (int i =0; i<= this.Project.Raw.ST.Count-1; i++)
          {
           for (int ii = 0; ii <= this.Project.Raw.ST[i].SS.Count-1; ii++)
           {
              //TmyPointO this.Project.Points.GetPointbyName(this.Project.Raw.ST[i].BackStation);
               if (this.Project.Raw.ST[i].SS[ii].isTravVertex) // если это вершина хода, то вставляем:
                   Project.Travers.AddVertex(this.Project.Raw.ST[i].SS[ii], this.Project.Raw.ST[i]);
            }
          }
        }
        #endregion

        #region Получить mapping хода
        /// <summary>
        /// Получить исходные данные хода
        /// </summary>
        private void GetMappings()
        {
            // если вершины еще не  внесены? то будет ошибочка
            if (this.Project.Travers.VertexList.Count == 0) return;
            TmyPointO bp = this.Project.Points.GetPointbyName(this.Project.Travers.VertexList[0].Station.StationName) ;
            TmyPointO bop = this.Project.Points.GetPointbyName(this.Project.Travers.VertexList[0].Station.BackStation) ;
            TmyPointO ep = this.Project.Points.GetPointbyName(this.Project.Travers.VertexList[this.Project.Travers.VertexList.Count-1].Station.StationName);    
            TmyPointO eop = this.Project.Points.GetPointbyName(this.Project.Travers.VertexList[this.Project.Travers.VertexList.Count-1].NextVertexName);


            this.Project.Travers.SetTraverseBegin(bp, bop);
            this.Project.Travers.SetTraverseEnd(ep, eop);            
          
       }
        #endregion


        #region Листинг точек теодолитного хода
        /// <summary>
        /// Листинг точек теодолитного хода
        /// </summary>
        private void ListVertex()
        {
            dataGridTraverseView.AutoGenerateColumns = false;
            dataGridTraverseView.Columns.Clear();

            DataGridViewTextBoxColumn STCL = new DataGridViewTextBoxColumn();
            STCL.DataPropertyName = "VertexName";
            STCL.HeaderText = "Станция";
            dataGridTraverseView.Columns.Add(STCL);

            DataGridViewTextBoxColumn NameCL = new DataGridViewTextBoxColumn();
            NameCL.DataPropertyName = "NextVertexName";
            NameCL.HeaderText = "Цель";
            dataGridTraverseView.Columns.Add(NameCL);

            /*
            DataGridViewTextBoxColumn LeftAngleCL = new DataGridViewTextBoxColumn();
            LeftAngleCL.DataPropertyName = "LeftAngle_r";
            LeftAngleCL.HeaderText = "Левый угол, рад.";
            dataGridTraverseView.Columns.Add(LeftAngleCL);
            */

            DataGridViewTextBoxColumn HA = new DataGridViewTextBoxColumn();
            HA.DataPropertyName = "HA_";
            HA.HeaderText = "Измер. угол, гр.";
            dataGridTraverseView.Columns.Add(HA);

            DataGridViewTextBoxColumn LeftAngleSCL = new DataGridViewTextBoxColumn();
            LeftAngleSCL.DataPropertyName = "LeftAngle_s";
            LeftAngleSCL.HeaderText = "Левый угол, гр.";
            dataGridTraverseView.Columns.Add(LeftAngleSCL);

            DataGridViewTextBoxColumn HDCL = new DataGridViewTextBoxColumn();
            HDCL.DataPropertyName = "HorizontalDistantion";
            HDCL.HeaderText = "Г. проложение";
            dataGridTraverseView.Columns.Add(HDCL);

            dataGridTraverseView.DataSource = this.Project.Travers.VertexList ;
        }
        /// <summary>
        /// Отображение точек исходных данных хода - 
        /// точек привязки в Начале и конце хода
        /// </summary>
        private void ListTraverseMapping()
        {
              if (this.Project.Travers.BeginPoint != null)
                {
                 textBoxAA.Text = this.Project.Travers.BeginPoint.NumGeopointA;
                 if (this.Project.Points.GetPointbyName(this.Project.Travers.BeginPoint.NumGeopointA) != null)
                     checkBox1.Checked = true;
                 else checkBox1.Checked = false;
                 }
                    else {textBoxAA.Clear();}
           
            // Намекнем? какой должен быть ориентир
              textBoxAO.Text = this.Project.Travers.VertexList[0].Station.BackStation;
            if (this.Project.Travers.BeginOrientir != null)
            {
                if (this.Project.Points.GetPointbyName(this.Project.Travers.BeginOrientir.NumGeopointA) != null)
                {
                    checkBox2.Checked = true;
                    textBoxAO.ForeColor = Color.Black;
                }
                else
                {
                    checkBox2.Checked = false;
                    textBoxAO.ForeColor = Color.Red;
                }
            }
            else
            {
                checkBox2.Checked = false;
                textBoxAO.ForeColor = Color.Red;
            }


            if (this.Project.Travers.EndPoint != null)
            {
             if (this.Project.Points.GetPointbyName(this.Project.Travers.EndPoint.NumGeopointA) != null)
                  checkBoxBB.Checked = true;
                    else checkBoxBB.Checked = false;
                textBoxBB.Text = this.Project.Travers.EndPoint.NumGeopointA;
            }
            else  {textBoxBB.Clear();}

            if (this.Project.Travers.EndOrientir != null)
            {
                if (this.Project.Points.GetPointbyName(this.Project.Travers.EndOrientir.NumGeopointA) != null)
                    checkBoxBO.Checked = true;
                else checkBoxBO.Checked = false;
                textBoxBO.Text = this.Project.Travers.EndOrientir.NumGeopointA;
            }
            else
            {     textBoxBO.Clear();}

            Project.Travers.Process(); // Расчет!

            PointsdataGridView.DataSource = null; //выключим грид с точками
            listBox1.Items.Clear();
            listBox1.Items.Add("Сумма углов " + Geodethic.RadiantoStr(this.Project.Travers.AngleSumm()));
            listBox1.Items.Add("Угловая невязка: " +Geodethic.RadiantoStr(this.Project.Travers.AngleError()));

            for (int i = 0; i <= this.Project.Travers.VertexList.Count - 1; i++)
            {
                if (this.Project.Travers.VertexList[i].Station.Status != 4)
                {
                    listBox1.Items.Add(this.Project.Travers.VertexList[i].VertexName +
                                     " " + this.Project.Travers.VertexList[i].Station.x_s +
                                     " " + this.Project.Travers.VertexList[i].Station.y_s);

                    this.Project.Points.Points.Add(this.Project.Travers.VertexList[i].Station);
                }
            }

            PointsdataGridView.DataSource = this.Project.Points.Points;//Включим грид с точками

        }
        #endregion


        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int ind = dataGridTraverseView.SelectedCells[0].RowIndex;
            dataGridTraverseView.Rows.RemoveAt(ind);
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            int ind = dataGridTraverseView.SelectedCells[0].RowIndex;
            dataGridTraverseView.Rows.RemoveAt(ind);
        }

    

      

        //получить  информацию о выделенном элементе в TreeVie
        private void RawtreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode trNode = e.Node;

            List<TRawObservation> SS = ListObservations(Convert.ToInt32(trNode.Tag));

            toolStripStatusLabel2.Text = Convert.ToString(trNode.Tag);

        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            double test = Geodethic.RawAngleToRadians(180.0335);
            toolStripStatusLabel2.Text = "RawAngleToRadians(180.0335) -> " + Convert.ToString(test);
        }

        private void RawtreeView_KeyUp(object sender, KeyEventArgs e)
        {

            TreeNode trNode = RawtreeView.SelectedNode;
            List<TRawObservation> SS = ListObservations(Convert.ToInt32(trNode.Tag));
            toolStripStatusLabel2.Text = Convert.ToString(trNode.Tag);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            GetObservation();
            GetMappings(); //Получить исходные данные хода
            ListVertex();
            ListTraverseMapping();
        }

        private void отметитьВсеВершиныToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void TraverseCheckAllVertex()
        {
            this.Project.Raw.SetAllVertex(true);
        }

        private void txtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsFixosoftTXT();
         }

        private void SaveAsFixosoftTXT()
        {
            saveFileDialog1.FilterIndex =3;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                this.Project.Points.WriteTxtFile(saveFileDialog1.FileName);
          
          
        }


        private void SaveAsmif()
        {
            saveFileDialog1.FilterIndex = 4;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)    ;


        }

        private void mifToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsmif();
        }

        private void dataGridTraverseView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {

           TMyPoints  res = PointsOnLine(this.Project.Points);
            if (res != null)
                this.Project.Points.AppendPoints(res);
        }

        private TMyPoints PointsOnLine(TMyPoints SourcePoints)
        {
            Quests.frmQuests_PointOnLine QLForm = new Quests.frmQuests_PointOnLine();
            QLForm.ShowDialog(this);

            return null;
        }

        private void текстовыйФайлNXYZD2015ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FilterIndex = 3;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                this.Project.Polygon.SaveAsFixosoftTXT2015(saveFileDialog1.FileName);
        }
     

      
    }
}
