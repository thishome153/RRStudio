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


//Отсюда - для Dxf, также добавить в ссылки проекта netDxf
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
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
//
using Traverser;

namespace Traverser
{
    public partial class Form1 : Form
    {
        TNumTxtFile TxtFile = new TNumTxtFile();            
        public Form1()
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

        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                
                TxtFile.ImportFile(openFileDialog1.FileName);
                listView1.Items.Clear();
                for (int i = 0; i <= TxtFile.Points.Count - 1; i++)
                {
                    ListViewItem Sub;
                  Sub=  listView1.Items.Add("Key", TxtFile.Points[i].Name, 0);
                   Sub.SubItems.Add(Convert.ToString(TxtFile.Points[i].x));
                   Sub.SubItems.Add(Convert.ToString(TxtFile.Points[i].y));
                   Sub.SubItems.Add(Convert.ToString(TxtFile.Points[i].z));
                  //Sub.SubItems.Add("");
                   Sub.SubItems.Add(TxtFile.Points[i].Descr);
                }



            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
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

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void xmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Serialize my Types
            saveFileDialog1.FilterIndex = 2; //xml
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TNumTxtFile));
                TextWriter writer = new StreamWriter(saveFileDialog1.FileName);
                serializer.Serialize(writer, TxtFile);
            }
        }

    }
}
