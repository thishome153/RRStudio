using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;





namespace Traverser
{
    
   public  class TmyPoint
    {
      public  int id, Order;
      public  string Name,  Descr;
      public float  x, y, z, Mt;
    };

    // Точки исходные, результирующие
    public class TNumTxtFile
    {
       public List<TmyPoint> Points;
       public List<string> FileStrings;
       public string FileName, DateImport;
        public TNumTxtFile() //Для сериализаций в Xml конструктор должен быть без параметров
        {
          Points = new List<TmyPoint>();
          FileStrings = new List<string>();
        }
        //***Читаем файл формата Num xyz Mt Descr**********************************************************
        public void ImportFile(string Fname)
        {
            try
            {
                string line = null;
                int StrCounter = 0;
                this.FileName = Fname;
                string TabDelimiter = "\t";  // tab
                System.IO.TextReader readFile = new StreamReader(Fname);
          
                while (readFile.Peek() != -1)
                {
                    line = readFile.ReadLine();

                    if (line != null) //Читаем строку
                    {      //по строке

                        while (line.Substring(0,1).Equals("#")) //Комментарий в файлах
                        { if (readFile.Peek() != -1)
                          line = readFile.ReadLine();
                        };
                        StrCounter++;
                        this.FileStrings.Add(line);
                         string[] SplittedStr = line.Split(TabDelimiter.ToCharArray()); //Сплпиттер по tab (\t)
                         TmyPoint FilePoint = new TmyPoint();
                         FilePoint.id = StrCounter;
                         FilePoint.Name = SplittedStr[0].ToString();
                         FilePoint.x = Convert.ToSingle(SplittedStr[1].ToString());
                         FilePoint.y = Convert.ToSingle(SplittedStr[2].ToString());
                         FilePoint.z = Convert.ToSingle(SplittedStr[3].ToString());
                         FilePoint.Descr = SplittedStr[4].ToString();
                          this.Points.Add(FilePoint);
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
    }
    
    // Наблюдения SS
    class TRawObserwation
    {
       public string PointName;
       public float SlopeDistantion, HA, VA;
       public float HorizontalDistantion //Горизонтиальное проложение
       {
           get
           {
            return this.SlopeDistantion;
           }
       }

    }
    // Станции ST
    class TRawStation
    {
        public string StationName, BackStation;
        public List<TRawObserwation> SS;
        public TRawStation(string StName, string BackName)
        {
            this.SS = new List<TRawObserwation>();
            this.StationName = StName;
            this.BackStation = BackName;
        }
        void AddObserv(TRawObserwation Observ)
         {
             this.SS.Add(Observ);
         }
    }

    // Веcь Raw, Разобранный из NumTxtFile
    class TRaw
    {
       //public List<TRawObserwation> SS;
       public List<TRawStation> ST;
       public string Filename;
        public TRaw(string FName)// Конструктор
        {
           this.ST = new List<TRawStation>();
           this.Filename = FName;
        }

       TRawStation AddStation(string STName, string BackName)
       { 
           TRawStation NewStation = new TRawStation(STName,BackName);
           ST.Add(NewStation);
           return NewStation;
       }
    }

}

