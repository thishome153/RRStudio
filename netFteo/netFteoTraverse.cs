using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using netFteo.NikonRaw;

namespace netFteo.Spatial
{

    #region ТеодолитныйХод    {theodolite traverse}
    /*    1) Engineering: survey traverse, theodolite traverse, traverse
          2) Construction: transit line
          3) Geodesy: progression
          4) Cartography: field traverse, polygonal course, transit traverse
     */
    public class TTraverse
    {
       // public TNikonRawProperties Properties;
        public string Name;
        public const int travType_Free = 1;
        public const int travType_Closed = 2;   //замкнутый, точки в начале и в конце одинаковые
        public const int travType_Unclosed = 3; //разомкнутый, точки в начале и в конце разные
        public double BegindirectAngle; // начальный дирекционный угол
        public TPoint BeginPoint,  EndPoint,
                         BeginOrientir, EndOrientir;
        public PointList SourcePoints;
        public TTraverse()
        {
            this.BeginPoint = null;
            this.EndPoint = null;
            this.BeginOrientir = null;
            this.EndOrientir = null;
            this.Name = "Traverse1";
            this.VertexList = new BindingList<TTraverseVertex>();
           // this.Properties = Prop;
         }
        public void ImportSourcePoints(PointList Src)
        {
            this.SourcePoints = new PointList();
            this.SourcePoints.AppendPoints(Src);
        }
        private void SetTraverseBegin(TPoint begPoint, TPoint begOrient) //Установить начало хода
        {
            if (begPoint != null)
            {
                this.BeginPoint = begPoint;
                this.BeginPoint.Status = 4;
            }
            

            if (begOrient != null)
            {   this.BeginOrientir = begOrient;
                this.BeginOrientir.Status = 4;
            }
            if (begOrient != null & BeginPoint != null)
            BegindirectAngle = Geodethics.Geodethic.Atan(begOrient.x, begOrient.y,BeginPoint.x, BeginPoint.y);
       }
        private void SetTraverseEnd(TPoint EPoint, TPoint EOrient) //Установить начало хода
        {
            if (EndPoint != null)
            {
                this.EndPoint = EPoint;
                this.EndPoint.Status = 4;
            }
            if (EOrient != null)
            {
                this.EndOrientir = EOrient;
                this.EndOrientir.Status = 4;
            }
        }
        /// <summary>
        /// Получить исходные пункты хода ()
        /// </summary>
        public void GetMappings()
        {
            // если вершины еще не  внесены? то будет ошибочка
            if (this.VertexList.Count == 0) return;
            TPoint bp = this.SourcePoints.GetPointbyName(this.VertexList[0].Station.StationName);
            TPoint bop = this.SourcePoints.GetPointbyName(this.VertexList[0].Station.BackStation);
            TPoint ep = this.SourcePoints.GetPointbyName(this.VertexList[this.VertexList.Count - 1].Station.StationName);
            TPoint eop = this.SourcePoints.GetPointbyName(this.VertexList[this.VertexList.Count - 1].NextVertexName);
            SetTraverseBegin(bp, bop);
            SetTraverseEnd(ep, eop);
        }
        
        /// <summary>
        /// // Сумма углов в радианах
        /// </summary>
        /// <returns></returns>
        public double AngleSumm() 
         {
             double Summ = 0;
             for (int i = 0; i <= this.VertexList.Count - 1; i++)
                 Summ += this.VertexList[i].TargetDirection;
          return Summ;
         }
        /// <summary>
        /// // Невязка углов в радианах
        /// </summary>
        /// <returns></returns>
        public double AngleError() 
        {
         return AngleSumm() / (180 * this.VertexList.Count);
        }

        public int TraverseType() //Тип хода
        {

            //if ((this.BeginPoint.Identical(this.EndPoint)) && (this.BeginOrientir.Identical(this.EndOrientir)))
            if ((this.BeginPoint == this.EndPoint) && (this.BeginOrientir == this.EndOrientir))
            { return travType_Closed; }
              else return -1;  // Не ясно какой

            //return travType_Free;
            //return travType_Unclosed;
        }
        public BindingList<TTraverseVertex> VertexList; //Список вершин
        public void AddVertex(TRawObservation Observ, TStation ST) // Добавить точку (вершину) хода
        {   
            TTraverseVertex V = new TTraverseVertex();
            V.NextVertex = Observ;
            V.Station = ST;
            TPoint backst = this.SourcePoints.GetPointbyName(ST.BackStation);
            TPoint thisst = this.SourcePoints.GetPointbyName(ST.StationName);
            if (backst != null & thisst != null)
             V.DirectionalAngle = Geodethics.Geodethic.Atan(backst.x, backst.y, thisst.x, thisst.y);    
            this.VertexList.Add(V);
      
        }
      

        /// <summary>
        /// Расчитать вершину
        /// </summary>
        /// <param name="vx_index"> Индекс вершины</param>
        public void ProcessVertex(int vx_index)
        {
         if (this.VertexList[vx_index].NextVertexName == null) return; //считать больше нечего
         
            if (vx_index == VertexList.Count - 1)
            // это последняя вершина
            {
                TTraverseVertex LastVertex = new TTraverseVertex();
                
                VertexList.Add(LastVertex);
            }
            if (this.VertexList[vx_index+1].Station.Status != 4 )
            {
            this.VertexList[vx_index+ 1].Station.NumGeopointA = this.VertexList[vx_index].NextVertexName;
            this.VertexList[vx_index+1].Station.Code = this.VertexList[vx_index].NextVertex.Code;
            this.VertexList[vx_index+ 1].Station.Status = 0;

            if (vx_index == 0) //Вначале хода дир. угол из опорных точек
                this.VertexList[vx_index].DirectionalAngle = BegindirectAngle;
              else
                this.VertexList[vx_index].DirectionalAngle = Geodethics.Geodethic.Atan(this.VertexList[vx_index - 1].Station.x, this.VertexList[vx_index - 1].Station.y, this.VertexList[vx_index].Station.x, this.VertexList[vx_index].Station.y);
            
                this.VertexList[vx_index + 1].Station.x = Math.Round(this.VertexList[vx_index].NextX(), 3);
                this.VertexList[vx_index + 1].Station.y = Math.Round(this.VertexList[vx_index].NextY(), 3);
            

          
            }
        }

        public void Process()
        {
            if (this.VertexList.Count == 0 | this.BeginPoint == null ) return;
            if (this.VertexList[0].Station.x == 0) this.VertexList[0].Station.x = this.BeginPoint.x;
            if (this.VertexList[0].Station.y == 0) this.VertexList[0].Station.y = this.BeginPoint.y;
                this.VertexList[0].Station.StationName = this.BeginPoint.NumGeopointA;
                this.VertexList[0].Station.Code = this.BeginPoint.Code;
                this.VertexList[0].Station.Status = 4;

            for (int i = 0; i <= this.VertexList.Count-1; i++)
            {
                this.ProcessVertex(i);

            }
        }
        /// <summary>
        /// Уравнять ход по МНК(LSE)
        ///  least square error
        /// </summary>
        public void Adjust()
        { }

        public void ClearVertexs()
        {   while (this.VertexList.Count != 0)
            {
                this.VertexList.Remove(this.VertexList[0]);
            }
    }
    }
    #endregion

    #region Вершина Теодолитного хода
    public class TTraverseVertex
    {   /// <summary>
        /// Конструктор
        /// </summary>
        public TTraverseVertex()
        {
            this.NextVertex = new TRawObservation();
            this.Station = new TStation();
        }

        public double DirectionalAngle;
        /// <summary>
        /// Дирекционный угол в формате Строка. Для ListView (как свойство класса).
        /// </summary>
        public string DirAngle_s
        {
            get
            {
                return Geodethics.Geodethic.RadiantoStr(this.DirectionalAngle);
            }
        }
        public TRawObservation NextVertex;
        public TStation Station;
       
        public string VertexName
        {
            get { return this.Station.StationName; }
        }
        public string NextVertexName
        {
            get { return  this.NextVertex.TargetName; }
            
        }
        /// <summary>
        /// Приращение ординаты X
        /// </summary>
        /// <returns></returns>
        public double NextdX()
        {
            //Дирекционный угол задней точки: проверить знак сложения
            return Math.Cos(this.TargetDirection) * HorizontalDistantion;
        }
        /// <summary>
        /// Приращение ординаты Y
        /// </summary>
        /// <returns></returns>
        public double NextdY()
        {
            //Дирекционный угол задней точки: проверить знак сложения
            //return Math.Sin(Geodethic.AngleTo360(this.DirectionalAngle + this.LeftAngle_r - Math.PI)) * HorizontalDistantion;
            return Math.Sin(this.TargetDirection) * HorizontalDistantion;
        }
        /// <summary>
        /// Расчетная ордината X на следующю точку
        /// </summary>
        public double NextX()
        {
            return this.Station.x + NextdX();
        }
        /// <summary>
        /// Расчетная ордината Y на следующю точку
        /// </summary>
        public double NextY()
        {
          return this.Station.y + NextdY();// Math.Sin(Geodethic.AngleTo360(this.DirectionalAngle + this.LeftAngle_r - Math.PI)) * HorizontalDistantion;
        }
        /// <summary>
        /// Горизонтальный угол в формате Nikon RAW (350.2245)
        /// </summary>
        public double HA       
        {
            get { return this.NextVertex.HA; }
        }
        /// <summary>
        /// Измеренный угол
        /// </summary>
        public string HA_
        {
            get
            {
                return this.HA.ToString();
            }
        }
       
        /// <summary>
        /// Левый угол в радианах
        /// </summary>
        public double TargetDirection
        {
            
            get
            {
                double d = 0;
                if (this.Station.HaSetinQuickStation)
                    return  this.DirectionalAngle + (this.NextVertex.HA_rad - 0) - Math.PI;// d:=d+(r2-0)-pi;
                //Azimuth to BS    
                if (this.Station.Properties.HA_Raw_Data == TNikonRawProperties.HA_Raw_Data_Azimuth)
                {
                   d = this.DirectionalAngle + (this.NextVertex.HA_rad - this.Station.BSHA_rad) - Math.PI;// d:=d+(r2-r1)-pi;   //  Дирекционный Translate directional angle to Point "Picket"
                }
                //Zero to BS ? 
                if (this.Station.Properties.HA_Raw_Data == TNikonRawProperties.HA_Raw_Data_Zero_to_BS)
                {
                     d = this.DirectionalAngle + (this.NextVertex.HA_rad - 0) - Math.PI;// d:=d+(r2-0)-pi;   //  Дирекционный Translate directional angle to Point "Picket"
                }
                return d;
            }
        }
     
        /// <summary>
        /// Левый угол в формате Строка, для Credo в том числе
        /// </summary>
        public string LeftAngle_s
        {
            get
            {
                return  Geodethics.Geodethic.RadiantoStr(Geodethics.Geodethic.AngleTo360(this.NextVertex.HA_rad - this.Station.BSHA_rad));
            }
        }
        /// <summary>
        /// Отсчет по горизонтальному кругу на заднюю точку.
        /// </summary>
        public string BSHA
        { get { return Geodethics.Geodethic.RadiantoStr(this.Station.BSHA_rad); } }

       


        public double HorizontalDistantion
        {
            get { return this.NextVertex.HorizontalDistantion; }
            //set { this.fHD = value;}
          }
     }
#endregion 

}
