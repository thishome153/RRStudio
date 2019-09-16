using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;





namespace Traverser
{
    #region Точка
    public class TmyPointO //Копия из  FteoClasses.pas
    {
        public int id, Borderid, Status, NumGeopoint, Order;
        public string NumGeopointA,
                       Pref, Code, Place,
                       Description,
                       Formula,
                       BorderDef; // ссылка на AREA.OBJDescr;
        public float x, y, z, Mt, oldX, oldY;
        public void RenamePoint()
        {
            throw new System.NotImplementedException();
        }
    };
    #endregion


    #region  Список точек Его будем сериализовать в XML для обменов
    public class TMyPoints
    {
        public List<TmyPointO> PointList;
        public int Parent_Id; // ид Участка или чего тоо ттам
        //Contructor:
        public TMyPoints() //Для сериализаций в Xml конструктор должен быть без параметров
        {
            PointList = new List<TmyPointO>();
        }

        //***Читаем файл формата Num xyz Mt Descr**********************************************************
        public void ImportTxtFile(string Fname)
        {
            try
            {
                string line = null;
                int StrCounter = 0;
                //this.FileName = Fname;
                string TabDelimiter = "\t";  // tab
                System.IO.TextReader readFile = new StreamReader(Fname);

                while (readFile.Peek() != -1)
                {
                    line = readFile.ReadLine();

                    if (line != null) //Читаем строку
                    {      //по строке

                        while (line.Substring(0, 1).Equals("#")) //Комментарий в файлах, пропустим его
                        {
                            if (readFile.Peek() != -1)
                                line = readFile.ReadLine();
                        };
                        StrCounter++;
                        //this.FileStrings.Add(line);
                        string[] SplittedStr = line.Split(TabDelimiter.ToCharArray()); //Сплпиттер по tab (\t)
                        TmyPointO FilePoint = new TmyPointO();
                        FilePoint.id = StrCounter;
                        FilePoint.NumGeopointA = SplittedStr[0].ToString();
                        FilePoint.x = Convert.ToSingle(SplittedStr[1].ToString());
                        FilePoint.y = Convert.ToSingle(SplittedStr[2].ToString());
                        FilePoint.z = Convert.ToSingle(SplittedStr[3].ToString());
                        FilePoint.Description = SplittedStr[4].ToString();
                        this.PointList.Add(FilePoint);
                    }

                }
                readFile.Close();
                readFile = null;
            }
            catch (IOException ex)
            {
                //  MessageBox.Show(ex.ToString());
            }

        }
        public void ClearItems()
        {

            while (this.PointList.Count != 0)
            {
                this.PointList.Remove(this.PointList[0]);
            }


        }


    }
    #endregion


    #region Файла формата Num x y z Descr.txt
    public class TNumTxtFile : TMyPoints
    {
        public List<string> FileStrings;
        /// <remarks>Имя файла источника</remarks>
        public string CoordinateSystem, FileName, DateImport;
        public void ImportObjects(List<TmyPointO> Points)
        {
            for (int i = 0; i <= Points.Count - 1; i++)
                this.PointList.Add(Points[i]);
        }


    }
    #endregion


    #region Полигон TmyOutLayer
    public class TmyOutLayer : TMyPoints
    {
        private int FLayer_id;
        private float FArea;
        public int Layer_id
        {
            get { return this.FLayer_id; }
            set { this.FLayer_id = value; }
        }
        public float Area
        {
            get { return 0; }
        }

    }
    #endregion


    #region Полилиния
    public class TmyPolyline : TMyPoints
    {
        public bool Closed
        {
            get { return true; } // написать процедуру проверки иденичности точки по-координатам
        }
    }
    #endregion

}



