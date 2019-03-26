using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using netFteo.Spatial;

namespace netFteo.NikonRaw
{
#region Веcь Raw, Разобранный из файла NikonRaw

  public class RawList : List<TNikonRaw> // dataSource
    {
    }
    
    /// <summary>
    /// Свойства RAW файла Nikon
    /// </summary>
  public class TNikonRawProperties
    {
        public const int Zero_VA_Zenith = 90;
        public const int Zero_VA_Horizontal = 0;
        public const int Zero_VA_Unknown = -1;
        public const int HA_Raw_Data_Azimuth = 180;
        public const int HA_Raw_Data_Zero_to_BS = 10;
        public const int HA_Quick_Station = 1000;
        public const int HA_Raw_Data_Uncknown = -1;
        public int Zero_VA; // Место нуля вертикального угла M0
        public string Zero_VA_To_String()
        {
            //return "--";
            if (this.Zero_VA == Zero_VA_Horizontal)
                return "Horizontal";
            if (this.Zero_VA == Zero_VA_Zenith)
                return "Zenith";
            if (this.Zero_VA == Zero_VA_Unknown)
                return "Не определен";
            else return "-";
        }
        public int HA_Raw_Data; // Углы горизонтальные откуда считаются?
        public string HA_Raw_Data_To_String()
        {
            //return "--";
            if (this.HA_Raw_Data == HA_Raw_Data_Azimuth)
                return "Azimuth";
            if (this.HA_Raw_Data == HA_Raw_Data_Zero_to_BS)
                return "Zero_to_BS";
            if (this.HA_Raw_Data == HA_Raw_Data_Uncknown)
                return "Uncknown";
            else return "-";
        }
        public TNikonRawProperties()
        {
            this.HA_Raw_Data = HA_Raw_Data_Uncknown;
            this.Zero_VA = Zero_VA_Unknown;
        }
    }

  public class TNikonRaw
    {
        public List<TStation> ST; //Список станций
        public string Filename, CO_Instrument, CO_S_N;
        public TNikonRawProperties Properties;
        public TNikonRaw()// Конструктор
        {
            this.ST = new List<TStation>();
            this.Properties = new TNikonRawProperties();
            this.Filename = "NikonRaw1";
        }

        //Добавить станцию
        TStation AddStation(string STName, string BackName)
        {
            TStation NewStation = new TStation();
            NewStation.StationName = STName;
            NewStation.BackStation = BackName;
            NewStation.Properties = this.Properties;
            ST.Add(NewStation);
            NewStation.id = this.ST.Count;
            return NewStation;
        } 
      
        public void ImportRawFile(string Fname) //Parcing файла Nikon
        {
            try
            {   this.ClearRAW();
                string line = null;
                bool HASetQuickStation = false;
                this.Filename = Fname;
                string RawDelimiter = ",";  // разделители в NikonTaw - запятые
                System.IO.TextReader readFile = new StreamReader(Fname);

                while (readFile.Peek() != -1) //пока предстоит что-то считать
                {
                    line = readFile.ReadLine();

                    if (line != null) //Читаем строку
                    {      //по строке
                        if (line.Contains("CO,Instrument:"))
                            this.CO_Instrument = line;
                        if (line.Contains("CO,S/N"))
                            this.CO_S_N = line;

                        if (line.Contains("CO,Zero VA: Zenith")) //Место нуля 90
                            this.Properties.Zero_VA = TNikonRawProperties.Zero_VA_Zenith;
                        if (line.Contains("CO,Zero VA: Horizontal")) // Место нуля 0
                            this.Properties.Zero_VA = TNikonRawProperties.Zero_VA_Horizontal;
                       
                        if (line.Contains("CO,HA Raw data: Azimuth")) // Горизонтальные углы с азимутом
                            this.Properties.HA_Raw_Data = TNikonRawProperties.HA_Raw_Data_Azimuth;
                        if (line.Contains("CO,HA Raw data: HA zero to BS")) // Горизонтальные углы с нулем назаднюю точку)
                            this.Properties.HA_Raw_Data = TNikonRawProperties.HA_Raw_Data_Zero_to_BS;

                        if (line.Contains("CO,HA set in Quick Station") || line.Contains("ST")) // Пошло объявление Quick-станции
                        {
                        QuickStation:
                            if (line.Contains("CO,HA set in Quick Station"))
                            {
                                line = readFile.ReadLine();
                                HASetQuickStation = true;
                            }
                            else HASetQuickStation = false;

                            string[] SplittedStr = line.Split(RawDelimiter.ToCharArray()); //Сплиттер по , (\t)
                            TStation NewStation = this.AddStation(SplittedStr[1].ToString(), SplittedStr[3].ToString());
                            //NewStation.Properties.HA_Raw_Data = this.Properties.HA_Raw_Data;
                            //NewStation.Properties.Zero_VA = this.Properties.Zero_VA;
                            NewStation.StationHeight = Convert.ToDouble(SplittedStr[5].ToString());
                            
                            NewStation.HaSetinQuickStation = HASetQuickStation;
                            if (HASetQuickStation)
                            {
                                NewStation.BSAzim = 0;
                                NewStation.BSHA = 0;
                            }
                            {
                                NewStation.BSAzim = Convert.ToDouble(SplittedStr[6].ToString());
                                if (this.Properties.HA_Raw_Data == TNikonRawProperties.HA_Raw_Data_Zero_to_BS)
                                    NewStation.BSHA = 0; else
                                     NewStation.BSHA = Convert.ToDouble(SplittedStr[7].ToString());
                            }
                            //дергаем все измерения при этой станции:

                            while (readFile.Peek() != -1)
                            {FindObservable:
                                line = readFile.ReadLine();
                                if (line != null) //Читаем строку
                                    if (line.Contains("SS,")) // Измерение при текущей станции 
                                    {
                                        SplittedStr = line.Split(RawDelimiter.ToCharArray());
                                        TRawObservation NewOBserv = NewStation.AddObserv(SplittedStr[1].ToString());
                                        NewOBserv.TargetHeight = Convert.ToDouble(SplittedStr[2].ToString());
                                        if (SplittedStr[3] != "")
                                        NewOBserv.SlopeDistantion = Convert.ToDouble(SplittedStr[3].ToString());
                                        NewOBserv.HA = Convert.ToDouble(SplittedStr[4].ToString());
                                        NewOBserv.VA_degree = Convert.ToDouble(SplittedStr[5].ToString());
                                        NewOBserv.TimeLabel = SplittedStr[6].ToString();
                                        NewOBserv.Code = SplittedStr[7].ToString();
                                        NewOBserv.BackPoint = NewStation.BackStation;
                                        NewOBserv.BSHA = NewStation.BSHA;
                                        NewOBserv.STName = NewStation.StationName;
                                    }
                                if (line.Contains("CO,")) //Пропускаем
                                    goto FindObservable;// line = readFile.ReadLine();

                                if (line.Contains("CO,HA set in Quick Station"))// Это след/ станция? переходим
                                    goto QuickStation;
                                if (line.Contains("ST,"))// Это след/ станция? переходим
                                    goto QuickStation;
                            }
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
        public void ClearRAW() //Очистить Станции
        {
            this.Properties.HA_Raw_Data = TNikonRawProperties.HA_Raw_Data_Uncknown;
            this.Properties.Zero_VA = TNikonRawProperties.Zero_VA_Unknown;
            while (this.ST.Count != 0)
            {
                this.ST.Remove(this.ST[0]);
            }
        }


        /// <summary>
        /// Установить/убрать все вершины как вершины хода
        /// </summary>
        /// <param name="Checked"></param>
        public void SetAllVertex(bool Checked)
        {
            for (int i = 0; i <= this.ST.Count - 1; i++)
                for (int j = 0; j <= this.ST[i].SS.Count - 1; j++)
                    this.ST[i].SS[j].isTravVertex = Checked;
        }

        public TStation GetStation(int StTag)
        {
            for (int i = 0; i <= this.ST.Count - 1; i++)
                if (this.ST[i].id == StTag)
                {
                    return this.ST[i];
                    //break;
                }

                return null;
        }
    }
    /// <summary>
    /// Класс Станция Nikon
    /// </summary>
  public class TStation : TPoint
    {
        public List<TRawObservation> SS; //Список наблюдений со станции
        public TNikonRawProperties Properties;
        //public int id;
        public string  BackStation;
        public double BSAzim, StationHeight;
        public double BSHA; // Отсчет по гор. кругу при наведении на ЗТ
        public double BSHA_rad // Отсчет по гор. кругу при наведении на ЗТ
        { get { return Geodethic.RawAngleToRadians(this.BSHA); } }
        public double BSAzim_rad
        { get { return Geodethic.RawAngleToRadians(this.BSAzim); } }
        public bool HaSetinQuickStation; // проверить по инструкции. если в QuickStation азимут назад всегда ноль ??
        public string StationName
        {
            get { return this.NumGeopointA; }
            set { this.NumGeopointA = value; }
        }
           public TStation()//(string StName, string BackName)
        {
            this.SS = new List<TRawObservation>();
            this.Properties = new TNikonRawProperties();
            this.StationName = "ST1";
            this.BackStation = "Unknown";
            this.BSAzim = 0.00;
            this.StationHeight = 0;
            this.HaSetinQuickStation = false;
        }

       public TRawObservation AddObserv(string SSName)
         {
             TRawObservation Observ = new TRawObservation();
             Observ.TargetName  = SSName;
             Observ.Properties = this.Properties;
             this.SS.Add(Observ);
             Observ.id = this.SS.Count;
             return Observ;
         }
       public List<TRawObservation> GetObservations()
       {
           if (this.SS.Count >= 0)
               return this.SS;
           else return null;
    
       }

       public void SetasVertex(int Observindex) // отметить как следющую станцию, наблюдение на вершину ходa. Как будет при ветвлении хода ??
       { 
       }
    } // Станция ST

  public class TRawObservation
  {
      //Geodethic geo = new Geodethic();
      public int id;
      public TNikonRawProperties Properties;
      string fTargetName, fCode, fTimeLabel, fSTName, fBackName;
      bool fTraverseVertex; // признак наблюдения на след/ станцию - вершину хода

      double fSlopeDistantion, fHA, fVA, fBSHA; //Если SD = 0 - это только угловые измерения (примычные например)
      public double TargetHeight; 
      public string TargetName
      {
          get { return this.fTargetName;}
          set { this.fTargetName = value; }
      }
      public string TimeLabel       
      {
          get { return this.fTimeLabel; }
          set { this.fTimeLabel = value; }
      }
      public string Code
      {
          get { return this.fCode; }
          set { this.fCode = value; }
      }
      public double  HA
      {
          get { return this.fHA; }
          set { this.fHA = value; }
      }

      public double HA_rad
      {
          get { return Geodethic.RawAngleToRadians(this.fHA); }
          
      }
      /// <summary>
      /// // Вертикальный угол в градусах, формат Nikon. Человекочитаемый вид? для редактирования
      /// </summary>
      public double VA_degree 
      {
          get { return this.fVA; }
          set { this.fVA = value; }
      }
      /// <summary>
      /// // Вертикальный угол в радианах, формат Nikon/ Для расчетов
      /// </summary>
      public double VA_r
      {
          get { return Geodethic.RawAngleToRadians ( this.fVA); }
          
      }
      public bool isTravVertex
      {
          get { return this.fTraverseVertex ; }
          set { this.fTraverseVertex = value; }
      }

      public string STName
      {
          get { return this.fSTName; }
          set { this.fSTName = value; }
      }

      public string BackPoint
      {
          get { return this.fBackName; }
          set { this.fBackName = value; }
      }

      public double BSHA
      {
          get { return this.fBSHA; }
          set { this.fBSHA = value; }
      }
      public double SlopeDistantion
      {
          get { return this.fSlopeDistantion; }
          set { this.fSlopeDistantion = value; }
      }
      public double HorizontalDistantion //Горизонтальное проложение
      {
          get
          {
              if (Properties == null) return 0;
              if (Properties.Zero_VA == TNikonRawProperties.Zero_VA_Zenith)
                  return Math.Round(Math.Cos(Math.PI / 2 - Geodethic.RawAngleToRadians(this.VA_degree)) * SlopeDistantion,3); // проверить место нуля?
              //ZERO_VA  = VAHorizontal
              else
                  return Math.Round(Math.Cos(Geodethic.RawAngleToRadians(this.VA_degree)) * SlopeDistantion,3); // 

          }
      }
      // Циклическая Ссылка.. однако public TNikonRaw ParentClass; // Ссылка на родительский, как заполнить..??
  } // Наблюдение SS

#endregion
       
 }




