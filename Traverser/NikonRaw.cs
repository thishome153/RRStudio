using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Traverser
{
      // Веcь Raw, Разобранный из файла NikonRaw
 
    class TNikonRaw
    {
   
        //public List<TRawObserwation> SS;
        public List<TStation> ST;
        public string Filename, CO_Instrument, CO_S_N;
        const int Zero_VA_Zenith = 1;
        const int Zero_VA_Unknown = -1;
        const int HA_Raw_Data_Azimuth =1;
        const int HA_Quick_Station = 2;
        const int HA_Raw_Data_Uncknown =-1;
        public  int Zero_VA; // Место нуля вертикального угла Зенит ?
        public  int HA_Raw_Data; // Углы горизонтальные откуда считаются ?
 
        public TNikonRaw()// Конструктор
        {
            this.ST = new List<TStation>();
            this.Filename = "NikonRaw1";
            this.HA_Raw_Data = HA_Raw_Data_Uncknown;
            this.Zero_VA = Zero_VA_Unknown;
        }

        TStation AddStation(string STName, string BackName)
        {
            TStation NewStation = new TStation();
            NewStation.StationName = STName;
            NewStation.BackStation = BackName;
            ST.Add(NewStation);
            return NewStation;
        }

         public void ImportTxtRawFile(string Fname)
         {
         try
            {
                string line = null;
                this.Filename  = Fname;
                string RawDelimiter = ",";  // разделители в NikonTaw - запятые
                System.IO.TextReader readFile = new StreamReader(Fname);
          
                while (readFile.Peek() != -1)
                {
                    line = readFile.ReadLine();

                    if (line != null) //Читаем строку
                    {      //по строке
                        if (line.Contains("CO,Instrument:"))
                            this.CO_Instrument = line;
                        if (line.Contains("CO,S/N"))
                            this.CO_S_N = line;

                       if (line.Contains("CO,Zero VA: Zenith")) //Комментарий в файлах, пропустим его
                        this.Zero_VA = Zero_VA_Zenith;
                       if (line.Contains("CO,HA Raw data: Azimuth")) // Горизонтальные углы с азимутом (могут быть и с нулем назаднюю точку)
                        this.HA_Raw_Data = HA_Raw_Data_Azimuth ;

                       if (line.Contains("CO,HA set in Quick Station")) // Пошло объявление станции
                       {
                           string StationString = readFile.ReadLine();
                           string[] SplittedStr = StationString.Split(RawDelimiter.ToCharArray()); //Сплиттер по , (\t)
                           TStation NewStation = this.AddStation(SplittedStr[1].ToString(), SplittedStr[3].ToString());
                           NewStation.HaSetinQuickStation = true;
                           //дергаем все измерения при этой станции:

                       }
               } 

                }  // покa не EOF

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
       public float HorizontalDistantion //Горизонтальное проложение
       {
           get
           {
            return this.SlopeDistantion;
           }
       }

    }
    // Станции ST
    class TStation
    {
        public string StationName, BackStation;
        public List<TRawObserwation> SS;
        public bool HaSetinQuickStation;
        
            public TStation()//(string StName, string BackName)
        {
            this.SS = new List<TRawObserwation>();
            this.StationName = "ST1";
            this.BackStation = "Unknown";
            this.HaSetinQuickStation = false;
        }
        void AddObserv(TRawObserwation Observ)
         {
             this.SS.Add(Observ);
         }
    }

 }




