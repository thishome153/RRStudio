using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using netFteo;


namespace netFteo.Spatial

{

    #region Общий генератор типа PrimaryKey
    /// <summary>
    /// Общий генератор типа PrimaryKey
    /// </summary>
    public static class Gen_id
    {
        private static int fid;
        public static int newId
        {
            get
            {
                fid++;
                return fid;
            }
        }
        public static void Reset()
        {
            fid = 0;
        }
    }
    #endregion

    #region Base classes of all base classes 

    /// <summary>
    /// Мать всех матерей
    /// </summary>
    public interface IGeometry//, ICloneable, IComparable, IComparable<IGeometry>, IPuntal
    {
        int id { get; set; }
    }

    /// <summary>
    /// А также его имплементация
    /// </summary>
    public class Geometry : IGeometry
    {
        private int fid;
        public int id
        {
            get { return this.fid; }
            set { this.fid = value; }
        }
        /// <summary>
        /// Construct base Geometry object
        /// </summary>
        public Geometry()
        {
            this.id = Gen_id.newId;
        }
    }



    public interface ICoordinate
    {
        //
        // Summary:
        //     The x-ordinate value
        double X { get; set; }
        //
        // Summary:
        //     The y-ordinate value
        double Y { get; set; }
        //
        // Summary:
        //     The z-ordinate value
        double Z { get; set; }
        //
        // Summary:
        //     The measure value
        // double M { get; set; }
        double Mt { get; set; }

    }

    public class Coordinate : ICoordinate
    {
        public const double NullOrdinate = double.NaN;

        /// <summary>
        /// Defualt on create: NAN, not zero !!
        /// </summary>
        public Coordinate()
        {
            this.X = NullOrdinate;
            this.Y = NullOrdinate;
            this.Z = NullOrdinate;
            this.Mt = NullOrdinate;
        }
        //
        // Summary:
        //     X coordinate.
        public double X { get; set; }
        //
        // Summary:
        //     Y coordinate.
        public double Y { get; set; }
        //
        // Summary:
        //     Z coordinate.
        public double Z { get; set; }
        public double Mt { get; set; }
    }

    public interface IPoint : IGeometry, ICloneable //IGeometry, ICloneable, IComparable, IComparable<IGeometry>, IPuntal
    {
        double x { get; set; }
        double y { get; set; }
        double z { get; set; }
        //double M { get; set; }
        //ICoordinateSequence CoordinateSequence { get; }
    }

    public class Point : Geometry, IPoint //port из  FteoClasses.pas
    {
        //implementation of interface method Clone():
        public object Clone()
        {
            return new Point(this.x, this.y, this.NumGeopointA);
        }

        public int Borderid, fStatus, NumGeopoint;//, Order;
        string fNumGeopointA,
                       fPref, fCode, fPlace,
                       fDescription;
        //fFormula,
        //fBorderDef; // ссылка на AREA.OBJDescr;
        //double fx, fy, fz, fMt, foldX, foldY;
        private Coordinate newOrd;
        private Coordinate oldOrd;


        /// <summary>
        /// Default constructor. Inherited from parent, of coarse
        /// Неявно вызывает родительский конструктор
        /// </summary>
        public Point()
        {
            this.newOrd = new Coordinate();
            this.oldOrd = new Coordinate();
        }

        public Point(double initx, double inity) : this()
        {
            this.x = initx;
            this.y = inity;
        }


        public Point(double initx, double inity, double initz)
            : this(initx, inity)
        {
            this.z = initz;
        }
        public Point(double initx, double inity, string name) : this(initx, inity)
        {
            this.NumGeopointA = name;
        }

        public string NumGeopointA
        {
            get { return this.fNumGeopointA; }
            set { this.fNumGeopointA = value; }
        }
        public string Pref
        {
            get { return this.fPref; }
            set { this.fPref = value; }
        }
        public string Code
        {
            get { return this.fCode; }
            set { this.fCode = value; }
        }
        public string Place
        {
            get { return this.fPlace; }
            set { this.fPlace = value; }
        }
        public string Description
        {
            get { return this.fDescription; }
            set { this.fDescription = value; }
        }

        /*
        public double x_any
        {
            get
            {
                if (!Double.IsNaN(this.oldOrd.X)) return this.newOrd.X;
                else return this.oldOrd.X;
            }

        }

        public double y_any
        {
            get
            {
                if (!Double.IsNaN(this.newOrd.Y)) return this.newOrd.Y;
                else return this.oldOrd.Y;
            }

        }
        */
        public double x
        {
            /*
            get { return this.fx; }
            set { this.fx = value; }
            */
            get { return this.newOrd.X; }
            set { this.newOrd.X = value; }
        }

        public double y
        {
            /*
            get { return this.fy; }
            set { this.fy = value; }
            */
            get { return this.newOrd.Y; }
            set { this.newOrd.Y = value; }
        }

        public double z
        {
            /*
            get { return this.fz; }
            set { this.fz = value; }
            */
            get { return this.newOrd.Z; }
            set { this.newOrd.Z = value; }
        }

        public double oldX
        {
            get { return this.oldOrd.X; }
            set
            {
                this.oldOrd.X = value;
            }
        }

        public double oldY
        {
            /*
            get { return this.foldY; }
            set { this.foldY = value; }
            */
            get { return this.oldOrd.Y; }
            set { this.oldOrd.Y = value; }
        }

        public double Mt
        {
            get { return this.newOrd.Mt; }
            set { this.newOrd.Mt = value; }
        }

        /// <summary>
        /// Признак изменений в точке. Если * - есть изменения
        /// </summary>
        public string OrdIdent
        {
            get
            {
                if ((Double.IsNaN(this.oldX)) ||
                    (Double.IsNaN(this.oldY))
                    ) return ""; 

                if ((this.x == this.oldX) && (this.y == this.oldY))
                    return "";
                else return "*"; //признак изменений в точек
            }
        }

        /// <summary>
        /// Ордината Х как строка
        /// </summary>
        public string x_s
        {
            get
            {
                if (!Double.IsNaN(this.x)) return this.x.ToString();
                else
                    return "-";
            }
        }

        /// <summary>
        /// Ордината Y как строка
        /// </summary>
        public string y_s
        {
            get
            {
                if (!Double.IsNaN(this.y)) return this.y.ToString();
                else
                    return "-";
            }
        }



        public string z_s
        {
            get
            {
                if (!Double.IsNaN(this.z)) return Convert.ToString(this.z);
                else return "0.00"; // ??
            }

        }


        public string Mt_s
        {
            get
            {
                if (!Double.IsNaN(this.Mt)) return this.Mt.ToString();
                else
                    return "-";
            }
        }

        public string oldX_s
        {
            get
            {
                if (!Double.IsNaN(this.oldX)) return this.oldX.ToString();
                else
                    return "-";
            }
        }

        public string oldY_s
        {
            get
            {
                if (!Double.IsNaN(this.oldY)) return this.oldY.ToString();
                else
                    return "-";
            }
        }

        public int Status
        {
            get { return this.fStatus; }
            set { this.fStatus = value; }
        }


        public bool Zero
        {
            get
            {
                if ((this.x == 0) && (this.y == 0)) return true;
                else return false;
            }
        }

        /// <summary>
        /// Проверка точки на установленость значений
        /// </summary>
        public bool Empty
        {
            get
            {
                if ((Double.IsNaN(this.newOrd.X))
                //The IEEE 754 floating point standard states that comparing NaN with NaN will 
                // always return false.If you must do this, use Double.isNaN().
                && (Double.IsNaN(this.newOrd.Y)))
                    return true;
                else
                    return false;
            }
        }

        public void RenamePoint()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Сложение точек....
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Point operator +(Point A, Point B)
        {
            Point res = new Point();
            res.id = Gen_id.newId;
            res.NumGeopointA = A.NumGeopointA + B.NumGeopointA;
            res.x = A.x + B.x;
            res.y = A.y + B.y;
            res.z = A.z + B.z;
            return res;
        }
        //Что же будет равенством точки:
        /*
        static public bool operator ==(Point A, Point B)
        {

        }
        
        static public bool operator !=(Point A, Point B)
        {
            return !(A == B);
        }
        */
        public bool Equals(Point B)//Точки одинаковые ?
        {
            //return (this == TestPoint); //применим оператор
            if ((this.id == B.id) &&                                 // один перв. ключ  
                (this.x == B.x) && (this.y == B.y) && (this.z == B.z) // идентичные координаты
                )
                return true;
            else
                return false;
        }

        public void Draw()//System.Windows.Controls.Canvas canvas, Member cursor, System.Windows.Media.SolidColorBrush color, SolidColorBrush bckclr)
        {
            /*
            System.Windows.Shapes.Rectangle rct = new Rectangle();
            rct.Stroke = Brushes.LightSlateGray; rct.Width = 25; rct.Height = 25;
            rct.Fill = bckclr;
            rct.StrokeThickness = 1;
            Canvas.SetLeft(rct, cursor.j * 28+5); Canvas.SetTop(rct, cursor.i * 28+5);
            canvas.Children.Add(rct);

            TextBlock CursorText = new TextBlock();
            CursorText.Text = cursor.Value.ToString();
            CursorText.Foreground = color;
            Canvas.SetLeft(CursorText, cursor.j * 28+9); Canvas.SetTop(CursorText, cursor.i * 28+9);
            canvas.Children.Add(CursorText);
        */
        }

    };
   

    #endregion
    
    /// <summary>
    /// Circle. Just circle
    /// </summary>
    public class TCircle : Point
    {
        /// <summary>
        /// Radius
        /// </summary>
        public double R;
        //public Point Center;
        public TCircle(decimal x, decimal y, decimal radius)
        {
			this.x = Convert.ToDouble(x);
            this.y = Convert.ToDouble(y);
            this.R = Convert.ToDouble(radius);
        }
    }

    #region  Список точек. Его будем сериализовать в XML для обменов

    /// <summary>
    /// Список точек на базе Dictionary
    /// </summary>
    //public class PointList : Dictionary<int, Point>  // есть мнение, что словарь работает быстрее.....
    //  {
    //     public void AddPoint(Point point)
    //   {
    //       if (point.id < 1) point.id = Gen_id.newId;
    //       this.Add(point.id, point);
    // }
    //} 

    /// <summary>
    /// Список точек на базе BindingList
    /// </summary>
    public class PointList : BindingList<Point>, IGeometry
    {
        public const string TabDelimiter = "\t";  // tab
        //public PointList Points;
        public int Parent_Id; // ид Участка или чего тоо ттам
        private int fid;
        public int id
        {
            get { return this.fid; }
            set { this.fid = value; }
        }

        /// <summary>
        /// Конструктор base Geometry object,
        /// Для сериализаций в Xml конструктор должен быть без параметров
        /// </summary>
        public PointList()
        {
            this.id = Gen_id.newId;
        }


        public bool HasChangesBool
        {
            get
            {
                foreach (Point pt in this)
                {
                    if (pt.OrdIdent == "*")
                        return true;
                }
                return false;
            }
        }

        public int Index (int item_id)
        {
            int res=-1; 
            foreach(Point pt in this)
            {
                res++;
                if (pt.id == item_id)
                {
                    return res;
                }
            }
            return -1; //point to nothing
        }

        /// <summary>
        /// Move point in point list up or down
        /// </summary>
        /// <param name="newPosition"></param>
        public void Move (int index, int newPosition)

        {
            Point Swap = this[newPosition]; // save object
            this.RemoveAt(newPosition);
            this.Insert(newPosition, this[index]);
            this.RemoveAt(index);
            this.Insert(index, Swap);
            
            //  this.[0].
        }

        public string HasChanges
        {
            get
            {
                if (this.HasChangesBool)
                    return "*";
                else
                    return "";
            }
        }



        /// <summary>
        /// Предельные размеры
        /// </summary>
        public TMyRect Bounds
        {
            get
            {
                TMyRect bounds = new TMyRect();
                // проверим данные
                if (this.PointCount == 0) { return null; }
                for (int MapIter = 0; MapIter <= this.Count - 1; MapIter++)

                    if (this[MapIter].Zero)
                    {
                        // нет координат
                        return null;
                    }


                //Поиск начальной точки
                foreach (Point pt in this)
                {
                    if (!Double.IsNaN(pt.x))
                    {

                        bounds.MinX = pt.x;
                        bounds.MaxX = pt.x;
                        bounds.MaxY = pt.y;
                        bounds.MinY = pt.y;
                        goto SCAN; //выходим, найдена первая точка с координатами
                    }
                }

SCAN:
                // найдем макс координаты
                for (int MapIter = 0; MapIter <= this.Count - 1; MapIter++)
                {
                    if (!Double.IsNaN(this[MapIter].x)) // Если точка существующая/измененная. Для ликвидируемых (newOrd.x = NaN) пропускаем поиск
                    {
                        if (this[MapIter].x < bounds.MinX)
                        { bounds.MinX = this[MapIter].x; }

                        if (this[MapIter].x > bounds.MaxX)
                            bounds.MaxX = this[MapIter].x;
                        if (this[MapIter].y > bounds.MaxY)
                            bounds.MaxY = this[MapIter].y;
                        if (this[MapIter].y < bounds.MinY)
                            bounds.MinY = this[MapIter].y;
                    }
                }
                // чистые предельные координаты
                //   bounds.MaX= bounds.MaxX;
                //  bounds.Mx:= bounds.MinX; // чистые предельные координаты
                //  bounds.My:= bounds.MinY; // чистые предельные координаты
                //  bounds.May:= bounds.MaxY; // чистые предельные координаты
                //  bounds.DeltaX:= bounds.MaxX-bounds.MinX;
                //  bounds.DeltaY:= bounds.Maxy-bounds.Miny;
                return bounds;
            }
        }

        /// <summary>
        /// Это размеры для карт M@pInfo, они чуть больше чем рассчитанные min max
        /// </summary>
        public TMyRect BoundsEnlarged
        {
            get
            {
                TMyRect bounds = new TMyRect();
                bounds = Bounds;

                if (bounds.MinX < 0) { bounds.MinX = Bounds.MinX * 1.1; }
                else bounds.MinX = Bounds.MinX * 0.9;
                if (bounds.MinY < 0) bounds.MinY = Bounds.MinY * 1.1;
                else bounds.MinY = Bounds.MinY * 0.9;
                if (bounds.MaxX < 0) bounds.MaxX = Bounds.MaxX * 0.9;
                else bounds.MaxX = Bounds.MaxX * 1.1;
                if (bounds.MaxX < 0) bounds.MaxY = Bounds.MaxY * 0.9;
                else bounds.MaxY = Bounds.MaxY * 1.1;
                return bounds;
            }
        }
        /*
       public void ImportTxtFile(string Fname)
       {
           try
           {
               string line = null;
               int StrCounter = 0;
               System.IO.TextReader readFile = new StreamReader(Fname);

               while (readFile.Peek() != -1)
               {
                   line = readFile.ReadLine();

                   if (line != null) //Читаем строку
                   {      //по строке

                       while (line.Contains ("CO,")) //Комментарий в файлах, пропустим его
                       {
                           goto next;
                       };

                       if (line.Contains("#")) //Комментарий в файлах, пропустим его
                       {  goto next;
                       };

                       StrCounter++;
                       string[] SplittedStr = line.Split(TabDelimiter.ToCharArray()); //Сплпиттер по tab (\t)
                       Point FilePoint = new Point();
                       FilePoint.id = StrCounter;
                       FilePoint.NumGeopointA = SplittedStr[0].ToString();
                       FilePoint.x = Convert.ToDouble(SplittedStr[1].ToString());
                       FilePoint.y = Convert.ToDouble(SplittedStr[2].ToString());
                       FilePoint.z = Convert.ToDouble(SplittedStr[3].ToString());
                       FilePoint.Description = SplittedStr[4].ToString();
                       this.AddPoint(FilePoint);
                   }
               next:;
               }
               readFile.Close();
               readFile = null;
           }
           catch (IOException ex)
           {
               //  MessageBox.Show(ex.ToString());
           }

       } //***Читаем файл формата Num xyz Mt Descr
       public void WriteTxtFile(string FileName)
       { 
         StreamWriter TxtFile = new StreamWriter(FileName);

           TxtFile.WriteLine("#Fixosoft NumXYZD data format V2014");
           TxtFile.WriteLine("#Producer: netFteoBaseClasses application");
           TxtFile.WriteLine("# Файл формата FTEO - Разделители полей tab");
           TxtFile.WriteLine("# Поля файла:");
           TxtFile.WriteLine("# ИмяТочки,X,Y,Z,Описание-Код.");

         for (int i = 0; i <= this.PointCount - 1; i++)
             TxtFile.WriteLine(this[i].NumGeopointA+TabDelimiter+
                               this[i].x_s+TabDelimiter+
                               this[i].y_s+TabDelimiter+
                               this[i].z_s+TabDelimiter+
                               this[i].Mt_s);
         TxtFile.Close();

       }

       public void ClearItems()
       {

           while (this.Count != 0)
           {
               this.Remove(this[0]);
           }
           Point newP = new Point();
           newP.NumGeopointA = "#++";
           this.Add(newP);
       }
        */
        public void ImportObjects(BindingList<Point> Points)
        {
            for (int i = 0; i <= Points.Count - 1; i++)
                this.AddPoint(Points[i]);
        }

        public void AppendPoints(PointList src)
        {
            if (src != null)
                for (int i = 0; i <= src.Count - 1; i++)
                    this.AddPoint(src[i]);
        }
        /*
        public void AddPoint(Point point)
        {
            if (point.id < 1) point.id = Gen_id.newId;
            this.Points.Add(point);
        }
        */
        public void AddPoint(Point point)
        {
            if (point == null) return;
            if (point.id < 1) point.id = Gen_id.newId;
            this.Add(point);

        }
        public Point AddPoint(string Name, double x_, double y_, string Descr)
        {
            Point Point = new Point();
            Point.x = x_;
            Point.y = y_;
            Point.Description = Descr;
            Point.NumGeopointA = Name;
            this.AddPoint(Point);
            return this[this.Count - 1];
        }

        public Point GetPointbyName(string ptName)
        {
            for (int i = 0; i <= this.Count - 1; i++)
            {
                if (this[i].NumGeopointA == ptName)
                {
                    return this[i];
                }
            }
            return null;

        }


        public int PointCount
        {
            get { return this.Count; }
        }

        /// <summary>
        /// Проверка пересечения полилинии с прямой b1b2
        /// </summary>
        /// <param name="b1">Точка 1</param>
        /// <param name="b2">Точка 2</param>
        /// <returns></returns>
        public PointList FindSects(Point b1, Point b2)
        {
            PointList ResLayer = new PointList(); Point ResPoint;
            int PointCounter = 0; int NextPointIndex = 0;
            for (int i = 0; i <= this.PointCount - 1; i++)
            {
                //ResPoint  =nil;
                if (PointCounter == this.PointCount - 1)
                    NextPointIndex = 0;
                else NextPointIndex = PointCounter + 1; // чтобы не вылетать на последней границе
                ResPoint = Geodethic.FindIntersect(this[PointCounter++],
                                         this[NextPointIndex],
                                         b1, b2);

                if ((ResPoint != null) && (!ResPoint.Zero))
                {
                    ResLayer.AddPoint(ResPoint);
                    //ResPoint.Free;
                }
            }
            return ResLayer;
        }



        /// <summary>
        ///  For only single ring (aka Anus lat.):
        /// </summary>
        /// <param name="ES">Entity Spatial - polygon, polyline, just points list</param>
        /// <returns></returns>
        public PointList FindSects(PointList ES)
        {
            PointList res = new PointList();
            PointList PlREs;

            for (int ic = 0; ic <= ES.Count - 2; ic++)
            {
                PlREs = FindSects(ES[ic], ES[ic + 1]);

                if ((PlREs != null) && (PlREs.PointCount > 0))
                { res.AppendPoints(PlREs); }
            }

            // last point to first point
            PlREs = this.FindSects(ES[ES.Count - 1], ES[0]);
            if ((PlREs != null) && (PlREs.PointCount > 0))
            { res.AppendPoints(PlREs); }
            return res;
        }


        /// <summary>
        /// Проверка принадлежности точки фигуре. Трассировка лучом
        /// </summary>
        /// <param name="b1">Точка для проверки</param>
        /// <returns></returns>
        public bool Pointin(Point b1)
        {
            int CountP = 0; //счетчик событий, он же и результат
            bool fDown = false;
            int j = 0;
            int n = PointCount;

            double yi = 0; double xj = 0;
            double yj = 0; double xi = 0;
            double ifx = b1.x;
            double ify = b1.y;


            for (int i = 0; i <= this.PointCount - 1; i++)
            {
                //j =(i+1)  mod (n);     // Вычисление "соседней" вершины
                j = MathExt.ModInt(n, i + 1);
                yi = this[i].y; yj = this[j].y;
                xi = this[i].x; xj = this[j].x;

                if (yi == yj) continue;  // вертикальный отрезок - пропускаем
                                         // над ребром или под ребром
                if (((yi > ify) && (yj > ify)) || ((yi < ify) && (yj < ify))) continue;
                if ((yj == ify) && (xj < ifx))
                {
                    fDown = yi < ify;
                    continue;
                }

                if ((yi == ify) && (xi < ifx))
                {
                    if (((yj > ify) && fDown) || ((yj < ify) && (!fDown)))
                        CountP++;
                }
                else
      if (((xj - xi) * ((ify - yi) / (yj - yi)) + xi) < ifx) CountP++;
            }
            return (CountP & 1) == 1;    // & - BITWISE operation ??!!
        }

        private void PoininTest()
        {
            this.AddPoint(new Point(0, 0, "11"));
            this.AddPoint(new Point(1000, 0, "12"));
            this.AddPoint(new Point(1000, 1000, "13"));
            this.AddPoint(new Point(0, 1000, "14"));
            bool flag = Pointin(new Point(1001, 1001)); // out
            flag = Pointin(new Point(999.99, 999.99)); // in
            flag = Pointin(new Point(1000.001, 1000.0001)); // on border (on ring, on anus...)
        }

        /// <summary>
        /// Проверка принадлежности набора (списка etc) точек фигуре
        /// </summary>
        /// <param name="points"> ring of points</param>
        /// <returns></returns>
        public PointList Pointin(PointList points)
        {
            PointList res = new PointList();
            foreach (Point pt in points)
                if (this.Pointin(pt))
                    res.AddPoint(pt);
            if (res.PointCount > 0)
                return res;
            else return null; //зануляем свободные концы"-: Денисюк И..
        }


        /// <summary>
        /// Поиск общих точек
        /// </summary>
        /// <param name="ES"></param>
        /// <returns></returns>
        public bool HasCommonPoints(PointList points)
        {
            if (this.CommonPoints(points).PointCount > 0)
                return true;
            else
                return false;
        }


        public PointList CommonPoints(PointList points)
        {
            PointList ResLayer = new PointList();

            for (int i = 0; i <= this.PointCount - 1; i++)
            {
                foreach (Point pt in points)

                    if ((this[i].x == pt.x)
                        &&
                        (this[i].y == pt.y))
                    {
                        ResLayer.AddPoint(pt);
                    }
            }
            return ResLayer;
        }

        public PointList CommonPoints(TMyPolygon poly)
        {
            PointList ResLayer = new PointList();
            //main ring:
            ResLayer.AppendPoints(this.CommonPoints((PointList)poly));
            //child rings:
            foreach (TMyOutLayer child in poly.Childs)
                ResLayer.AppendPoints(this.CommonPoints((PointList)child));
            return ResLayer;
        }

    }
    #endregion

    #region   Границы
    public class TMyRect
    {
        public double MinX;
        public double MinY;
        public double MaxX;
        public double MaxY;
        public TMyRect()
        {
            this.MinX = 0;
            this.MinY = 0;
            this.MaxX = 0;
            this.MaxY = 0;
        }
        public TMyRect(double minx, double miny, double maxx, double maxy)
        {

            this.MinX = minx;

            this.MinY = miny;

            this.MaxX = maxx;

            this.MaxY = maxy;

        }
    }
    #endregion

    #region TMyOutLayer Полигон
    /// <summary>
    /// TODO must stay into RING - YES of coarse,  unsere dear Friend!!!
    /// </summary>

    //public class Ring: PointList
    public class TMyOutLayer : PointList
    {
        // public BindingList<Point> Points;
        private int FLayer_id;
        private string fDefinition;
        public string Definition
        {
            get { return this.fDefinition; }
            set { this.fDefinition = value; }
        }

        public int Layer_id
        {
            get { return this.FLayer_id; }
            set { this.FLayer_id = value; }
        }

        public string PerymethrFmt(string Format)
        {

            return this.Perymethr().ToString(Format);
        }

        public double Perymethr()
        {
            if (this.PointCount == 0) return -1;
            double Peryd = 0;
            double Test = 0;
            for (int i = 0; i <= this.Count - 2; i++)
            {
                Test = Geodethic.lent(this[i].x, this[i].y, this[i + 1].x, this[i + 1].y);
                if (!Double.IsNaN(Test))
                    Peryd += Test;
            }

            Test = Geodethic.lent(this[this.Count - 1].x, this[this.Count - 1].y, this[0].x, this[0].y);
            if (!Double.IsNaN(Test))
                Peryd += Test;
            return Peryd;
        }


        /// <summary>
        ///  Проверка вершин, 
        /// </summary>
        /// <returns></returns>
        public bool Valide
        {
            get
            {
                foreach (Point pt in this)
                {
                    if (pt.Empty) return false; // return on first empty vertex
                }

                return true; // all vertex valid (have coords)
            }

        }

        /// <summary>
        /// Расчет площади по формуле трапеции
        /// с учетом ликвидированных точек
        /// </summary>
        public double Area
        {
            get
            {
                PointList ring = new PointList();
  
                foreach (Point pt in this)

                {
                    //выбираем только точки с существующими ординатами .newOrd :
                    if (! Double.IsNaN(pt.x))
                    {
                        ring.AddPoint(pt);
                    }
                }

                return AreaofRing(ring);
            }
        }

        /// <summary>
        /// Расчет площади кольца (ring) по формуле трапеции
        /// Значение -1 - ошибка расчета
        /// </summary>
        private double AreaofRing(PointList ring)
        {

            //     if (this.Valide)
            {
                double rr = 0;
                double x1_, y3_, y1_, y2_;
                int PtsCnt = ring.Count;
                int CurPts = 0;
                if (PtsCnt > 2)
                {
                    PtsCnt--; // декремент сразу чтобы не возиться в массиве с выражениями типа array_[PtsCnt-1].x;
                    y3_ = ring[CurPts].y; // Вначало !!!
                    CurPts++; // Вперёд !!!
                    y2_ = ring[CurPts].y;
                    x1_ = ring[PtsCnt].x; // В конец
                    y1_ = ring[PtsCnt].y; // В конец

                    rr = x1_ * (y2_ - y1_);
                    y2_ = ring[PtsCnt - 1].y; // Назад !!!
                    rr = rr + x1_ * (y3_ - y2_);
                    CurPts = 0;

                    for (int i = 2; i <= PtsCnt; i++)
                    {
                        y1_ = ring[CurPts].y;
                        CurPts++; // Вперёд !!!
                        x1_ = ring[CurPts].x;
                        CurPts++; // Вперёд !!!
                        y2_ = ring[CurPts].y;
                        rr = rr + x1_ * (y2_ - y1_);
                        CurPts--;
                    }
                    rr = Math.Abs(rr / 2);
                    // if  not inmeters then rr:=rr/10000;
                    // SendControlMessage(FHdnle,3,'Рассчитана площадь '+FormatFloat(PrecFrmt,rr),0,0,0);
                    return rr;
                    //break;
                }
                /* else
                  begin
                   ShowMessage('Число точек полигона меньше 3. '+IntToStr(id));
                 result :=-1;
                  end;*/
                return rr;

                //
                //}
                //else return -1;
                //
            }
        }
    


    /*
      private double Area(PointList ring)
        {
            
                if (this.Valide)
                {
                    double rr = 0;
                    double x1_, y3_, y1_, y2_;
                    int PtsCnt = this.Count;
                    int CurPts = 0;
                    if (PtsCnt > 2)
                    {
                        PtsCnt--; // декремент сразу чтобы не возиться в массиве с выражениями типа array_[PtsCnt-1].x;
                        y3_ = this[CurPts].y; // Вначало !!!
                        CurPts++; // Вперёд !!!
                        y2_ = this[CurPts].y;
                        x1_ = this[PtsCnt].x; // В конец
                        y1_ = this[PtsCnt].y; // В конец

                        rr = x1_ * (y2_ - y1_);
                        y2_ = this[PtsCnt - 1].y; // Назад !!!
                        rr = rr + x1_ * (y3_ - y2_);
                        CurPts = 0;

                        for (int i = 2; i <= PtsCnt; i++)
                        {
                            y1_ = this[CurPts].y;
                            CurPts++; // Вперёд !!!
                            x1_ = this[CurPts].x;
                            CurPts++; // Вперёд !!!
                            y2_ = this[CurPts].y;
                            rr = rr + x1_ * (y2_ - y1_);
                            CurPts--;
                        }
                        rr = Math.Abs(rr / 2);
                        // if  not inmeters then rr:=rr/10000;
                        // SendControlMessage(FHdnle,3,'Рассчитана площадь '+FormatFloat(PrecFrmt,rr),0,0,0);
                        return rr;
                        //break;
                    }
                    
                    return rr;
                }
                else return -1;
            }
    */
        /// <summary>
        ///  Центр масс полигона: 
        ///  возвращает точку центра масс
        /// </summary>
        public Point CentroidMassive
        {
            get
            {
                double Xsumm = 0;
                double Ysumm = 0;
                for (int i = 0; i <= this.PointCount - 2; i++)
                {
                    Xsumm = Xsumm + (this[i].x + this[i + 1].x) * (this[i].x * this[i + 1].y - this[i + 1].x * this[i].y);
                    Ysumm = Ysumm + (this[i].y + this[i + 1].y) * (this[i].x * this[i + 1].y - this[i + 1].x * this[i].y);
                }
                Point Respoint = new Point();
                Respoint.NumGeopointA = "Centroid";
                Respoint.x = Xsumm * (1 / (6 * Area));
                Respoint.y = Ysumm * (1 / (6 * Area));
                return Respoint;
            }

        }

        /// <summary>
        /// Расчет центроида на поверхность (canvas) 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Point Centroid(double canvas_width, double canvas_height, double scale)
        {
            Point pt = new Point();
            double canvasX;
            double canvasY;

            canvasX = canvas_width / 2 - (this.AverageCenter.y - this.CentroidMassive.y) / scale;
            canvasY = canvas_height / 2 - (this.AverageCenter.x - this.CentroidMassive.x) / scale;
            pt.NumGeopointA = "CanvasCentroid";
            pt.x = canvasX;
            pt.y = canvasY;
            return pt;
        }


        /// <summary>
        /// Расчет масштаба на поверхность (z.B: WPF canvas) 
        /// </summary>
        /// <param name="canvas_width">Ширина полотна</param>
        /// <param name="canvas_height">Высота полотна</param>
        /// <param name="ViewKoefficient">Масштаб (1 = 100 %)</param>
        /// <returns></returns>
        public double ScaleEntity(double canvas_width, double canvas_height)//, double ViewKoefficient)
        {
            double scale = 0;
            double dx = (this.Bounds.MaxX - this.Bounds.MinX); // размах по вертикали
            double dy = (this.Bounds.MaxY - this.Bounds.MinY); // размах по горизонтали 
                                                               //label1.Content = "dx " + dx.ToString("0.0") + " dy " + dy.ToString("0.0");
                                                               //Определим, в какой оси больше размах, в ней и вычислим масштаб:

            if (dx > dy)
            {
                scale = (dx / canvas_height); // / ViewKoefficient;

            }
            else
            {
                scale = (dy / canvas_width); // / ViewKoefficient;
            }
            return scale;
        }

        /// <summary>
        /// Средние значения координат полигона
        /// </summary>
        public Point AverageCenter
        {
            get
            {
                double Xsumm = 0;
                double Ysumm = 0; int pointcnt = 0;
                for (int i = 0; i <= this.PointCount - 1; i++)
                {
                    if (! Double.IsNaN(this[i].x))
                    {
                        Xsumm = Xsumm + (this[i].x);
                        Ysumm = Ysumm + (this[i].y);
                        pointcnt++;
                    }
                }
                Point Respoint = new Point();
                Respoint.NumGeopointA = "Center";
                Respoint.x = Xsumm / pointcnt;
                Respoint.y = Ysumm / pointcnt;
                return Respoint;
            }
        }

        public void Exchange_XY()
        {
            double dx;
            for (int i = 0; i <= this.Count - 1; i++)
            {
                dx = this[i].x;
                this[i].x = this[i].y;
                this[i].y = dx;
            }
        }

        public void Reset_Ordinates()
        {
            foreach (Point pt in this)
            {
                pt.oldX = Coordinate.NullOrdinate;
                pt.oldY = Coordinate.NullOrdinate;
            }
        }


        public void Set_Mt(double mt)
        {
            foreach (Point pt in this)
                pt.Mt = mt;
        }

        public void Set_Fraq(string format)
        {
            foreach (Point pt in this)
            {
                if (Double.TryParse(pt.x.ToString(format), out double fraqtedX))
                {
                    pt.x = fraqtedX;
                    pt.oldX = fraqtedX;
                }

                if (Double.TryParse(pt.y.ToString(format), out double fraqtedY))
                {
                    pt.y = fraqtedY;
                    pt.oldY = fraqtedY;
                }
            }
        }

   

        /// <summary>
        /// Closed figure - point First and Last are ident (in ordinates, not by name/Number)
        /// </summary>
        public bool Closed
        {
            get
            {
                return ((this.First().x == this.Last().x) &&
                 (this.First().y == this.Last().y));
            }
        }

        /// <summary>
        /// Close figure - append last point, if not present
        /// </summary>
        public void Close()
        {
            if (! Closed)
            {
                this.Add(this.First());
            }
        }

		public void Reverse_Points()
		{
			TMyOutLayer tmpList = new TMyOutLayer();
			//for (int i = this.Count-1 ; i <= this.Count - 1; i++)
			int count = this.Count;
			foreach(Point pt in this)
			{
				tmpList.AddPoint(this[(count--) - 1]);
			}
			this.Clear();
			this.ImportObjects(tmpList);
		}

        public int Reorder_Points(int StartIndex)
        {
            foreach (Point pt in this)
            {
                pt.NumGeopointA = StartIndex++.ToString();
            }

            if (this.Closed)
                this.Last().NumGeopointA = this.First().NumGeopointA;// closing point are ident
            return StartIndex;
        }



    }
    #endregion

    #region Отрезки границ
    public class TBorder
    {
        private int fid;
        double fLength;
        public string Definition;
        public string PointNames;
        public double Length
        {
            get { return this.fLength; }
        }
        public double id
        {
            get { return this.fid; }
        }
        public TBorder(string definition, double length)
        {
            this.fLength = length;
            this.Definition = definition;

            this.fid = Gen_id.newId;
        }
    }

    public class BrdList : BindingList<TBorder>
    {
        public void AddItem(string definition, Point A, Point B)
        {
            TBorder NewBrd = new TBorder(definition, Geodethic.lent(A.x, A.y, B.x, B.y));
            NewBrd.PointNames = A.NumGeopointA + " - " + B.NumGeopointA;
            this.Add(NewBrd);
        }

        public void AddItems(string Pref, PointList ES)
        {
            for (int i = 0; i <= ES.Count - 2; i++)
            {
                this.AddItem("-", ES[i], ES[i + 1]);
            }

        }
    }

    #endregion

    #region TMyPolygon Полигон c внутренними границами
    /// <summary>
    /// Класс Полигон
    /// </summary>
    public class TMyPolygon : TMyOutLayer
    {

        public List<TMyOutLayer> Childs;

        public TMyPolygon()
        {
            this.Childs = new List<TMyOutLayer>();
            this.Layer_id = Gen_id.newId; //RND.Next(1, 10000);
            this.AreaValue = -1; // default, 'not specified'
        }

        public TMyPolygon(int id):this()
        {
          //  this.Childs = new List<TMyOutLayer>();
            this.Layer_id = id;
        }

        public TMyPolygon(string Def):this()
        {
            //this.Childs = new List<TMyOutLayer>();
            //this.Layer_id = Gen_id.newId;
            this.Definition = Def;
        }

        public TMyPolygon(int id, string Def):this(id)
        {
          //  this.Childs = new List<TMyOutLayer>();
          //  this.Layer_id = id;
            this.Definition = Def;
        }


        public void ExchangeXY()
        {
            this.Exchange_XY();
            for (int i = 0; i <= this.Childs.Count - 1; i++)
                this.Childs[i].Exchange_XY();

        }

        /// <summary>
        /// Reset old values of ordinates - like tnewContour
        /// </summary>
        public void ResetOrdinates()
        {
            this.Reset_Ordinates();
            foreach (TMyOutLayer child in this.Childs)
                child.Reset_Ordinates();

        }

        public void SetMT(double mt)
        {
            this.Set_Mt(mt);
            foreach (TMyOutLayer child in this.Childs)
                child.Set_Mt(mt);
        }


        public void Fraq(string format)
        {
            this.Set_Fraq(format);
            foreach (TMyOutLayer child in this.Childs)
                child.Set_Fraq(format);
        }


        public void ReorderPoints(int StartIndex)
        {
            StartIndex += this.Reorder_Points(StartIndex);
            foreach (TMyOutLayer child in this.Childs)
                StartIndex += child.Reorder_Points(StartIndex);
        }


        public int State;
        /// <summary>
        /// Площадь - значение (указанное). Типа Семантическая в ЕГРН
        /// </summary>
        public decimal AreaValue;
        public string AreaInaccuracy; // Погрешность определения
        /// <summary>
        /// Площадь полигона геометрическая (пространственная)
        /// </summary>
        public double AreaSpatial
        {
            get
            {
                double AreaFull = -1; // ненормальный результат
                AreaFull = this.Area;
                for (int i = 0; i <= this.Childs.Count - 1; i++)
                    AreaFull = AreaFull - this.Childs[i].Area;
                return AreaFull;
            }
        }
        public string AreaSpatialFmt(string Format)
        {
            return this.AreaSpatial.ToString(Format);
        }

        public int PolygonPointCount
        {
            get
            {
                int res = this.Count;
                foreach (PointList child in this.Childs)
                {
                    res += child.Count;
                }

                return res;
            }
        }

        public PointList AsPointList()
        {
            PointList Res = new PointList();

            for (int ip = 0; ip <= this.Count - 2; ip++) // без замыкающей
                Res.Add(this[ip]);

            for (int i = 0; i <= this.Childs.Count - 1; i++)
            {
                for (int ip = 0; ip <= this.Childs[i].Count - 2; ip++)// без замыкающей
                    Res.Add(this.Childs[i][ip]);
            }
            return Res;

        }

        public void ImportPolygon(TMyPolygon Poly)
        {
            if (Poly == null) return;
            for (int i = 0; i <= Poly.PointCount - 1; i++)
                this.AddPoint(Poly[i]);

            for (int i = 0; i <= Poly.Childs.Count - 1; i++)
                this.Childs.Add(Poly.Childs[i]);
        }

        /*
        public PointList FindSectVector(Point b1, Point b2)
        {
            PointList res = new PointList();
            PointList PlREs = this.FindSects(b1, b1);
           if (PlREs != null) { res.AppendPoints(PlREs); }
           for (int ic = 0; ic <= this.Childs.Count - 1; ic++)
           {
               PointList PlREsc = this.Childs[ic].FindSects(b1, b1);
               if (PlREs != null) { res.AppendPoints(PlREs); }
           }

            return res;
        }
        */



        public PointList FindCommonPoints(TMyPolygon ES)
        {
            PointList ResLayer = new PointList();
            // 1.
            ResLayer.AppendPoints(this.CommonPoints(ES));
            // 2.
            //for this childs
            foreach (PointList child in this.Childs)
                ResLayer.AppendPoints(child.CommonPoints(ES));
            return ResLayer;
        }

        /// <summary>
        /// Проверка принадлежности точек полигону
        /// с проверкой для внутренних границ ("дырок")
        /// </summary>
        /// <param name="ES"> ring of points</param>
        /// <returns></returns>
        public PointList PointsIn(PointList ES)
        {
            PointList res = new PointList();
            foreach (Point pt in ES)
            {
                if (this.Pointin(pt))
                {
                    bool inchildFlag = false;
                    //childs .... 
                    //для внутренних границ точка как раз не должна быть внутри
                    //если она принадлежит "дырявому" полигону, то фактом её принадлежности будет 
                    // принадлежность внешнему и отсутствие принадлежности всем детям
                    foreach (TMyOutLayer child in this.Childs)
                    {
                        inchildFlag = child.Pointin(pt);
                    }
                    if (!inchildFlag) // если не попало ни в одну дырку
                        res.AddPoint(pt);
                }

            }

            if (res.PointCount > 0) return res;
            else
                return null;
        }

        public PointList PointsIn(TMyPolygon ES)
        {
            PointList res = new PointList();
            res.AppendPoints(this.PointsIn(ES));
            //for each child in checked ES
            foreach (TMyOutLayer child in ES.Childs)
                res.AppendPoints(this.PointsIn(child));
            if (res.PointCount > 0) return res;
            else
                return null;
        }

        /// <summary>
        /// Проверка пересечений полигона с полилинией
        /// </summary>
        /// <param name="ES">Полилиния, без внутр. границ</param>
        /// <returns></returns>
        /// 
        public PointList FindSect(PointList ES)
        {
            PointList res = new PointList();
            PointList PlREs;

            // I
            PlREs = this.FindSects(ES);
            if (PlREs != null) { res.AppendPoints(PlREs); }

            foreach (TMyOutLayer child in this.Childs)
            {
                res.AppendPoints(child.FindSects(ES));
            }

            return res;
        }

        /// <summary>
        /// Проверка пересечений полигона с полигоном
        /// </summary>
        /// <param name="ES">Полигон (возможны внутр. границы)</param>
        /// <returns></returns>
        public PointList FindSect(TMyPolygon ES)
        {
            PointList res = new PointList();
            PointList PlREs;
            string ESDefinition = this.Definition + " x " + ES.Definition;
            // I - main ring
            PlREs = this.FindSect((PointList)ES);
            if (PlREs != null)
            {
                ESDefinition.Contains("stop me");
                res.AppendPoints(PlREs);
            }

            // II - for checking polygon childs
            for (int ic = 0; ic <= ES.Childs.Count - 1; ic++)
            {
                PlREs = this.FindSect(ES.Childs[ic]);
                if (PlREs != null) { res.AppendPoints(PlREs); }
            }
            return res;
        }

        /// <summary>
        /// Программа полной проверки пересечений (clipping`a )с полигоном
        /// </summary>
        /// <param name="ES">Полигон типа TMyPolygon</param>
        /// <returns></returns>
        public PointList FindClip(TMyPolygon ES)
        {
            PointList ResultClip = new PointList();
            //  I. Self check - selfCrossing and Overlapping 
            /*
              ------ TODO !
            */
            //  II.  Пересечения типа линия - линия
            ResultClip.AppendPoints(FindSect(ES));
            //  III. FindPointin
            //  ResultClip.AppendPoints(this.PointsIn(ES)); //TODO - вылет для mod ()
            //  IV.  Common points
            ResultClip.AppendPoints(FindCommonPoints(ES));
            //   V. Build complete clipping regions
            /*
             ----- TODO !!! 
             */
            return ResultClip;
        }

        /// <summary>
        /// Проверка пересечений полигона с коллекцией полигонов
        /// </summary>
        /// <param name="ES">Коллекция полигонов</param>
        /// <returns></returns>
        public PointList FindClip(TPolygonCollection ES)
        {
            PointList res = new PointList();
            PointList PlREs;
            for (int ic = 0; ic <= ES.Count - 1; ic++)
            {
                PlREs = this.FindClip(ES[ic]);
                if (PlREs != null) { res.AppendPoints(PlREs); }
            }
            return res;
        }

        public TMyOutLayer AddChild()
        {
            this.Childs.Add(new TMyOutLayer());
            this.Childs[this.Childs.Count - 1].Layer_id = Gen_id.newId;
            return this.Childs[this.Childs.Count - 1];
        }

        public TMyOutLayer AddChild(TMyOutLayer child)
        {
            this.Childs.Add(child);
            this.Childs[this.Childs.Count - 1].Layer_id = Gen_id.newId;
            return this.Childs[this.Childs.Count - 1];
        }

        /// <summary>
        /// Return '*' as mark of changed item
        /// </summary>
        public string Has_Changes
        {
            get
            {
                if (this.HasChanges == "*") return "*";

                for (int i = 0; i <= this.Childs.Count - 1; i++)
                {
                    if (this.Childs[i].HasChanges == "*") return "*";
                }
                return "";
            }
        }

        public TMyOutLayer GetEs(int Layer_id)
        {
            if (this.Layer_id == Layer_id) return this;

            for (int i = 0; i <= this.Childs.Count - 1; i++)
            {
                if (this.Childs[i].Layer_id == Layer_id) return this.Childs[i];
            }
            return null;
        }
    }
    #endregion

    #region TPolygonCollection Коллекция Полигонов

    public class MIFColumn
    {
        public string Name;
        public string Declaration;
        public MIFColumn(string initName, string initdecl)
        {
            this.Name = initName;
            this.Declaration = initdecl;
        }
    }
    public class MifOptions
    {
        public string Delimiter;
        public string DefaultProjection = "NonEarth Units \"m\"";
        public string DelimiterDefault = "$";
        public string CoordSys; //z.b.: CoordSys NonEarth Units "m" Bounds (1301144.92628516,549175.488422918)  (1319077.30433243,568919.661887878)
        public List<MIFColumn> Columns;
        public MifOptions()
        {
            this.Columns = new List<MIFColumn>();
            CoordSys = DefaultProjection;
            Delimiter = DelimiterDefault;
        }

        public void AddColumn(string name, string declaration)
        {
            this.Columns.Add(new MIFColumn(name, declaration));
        }
    }

    public class ESCheckingEventArgs : EventArgs
    {
        public string Definition;
        public int Process;
        public byte[] Data;
    }

    public delegate void ESCheckingHandler(object sender, ESCheckingEventArgs e);

    public class TPolygonCollection : List<TMyPolygon>
    {
        public event ESCheckingHandler OnChecking;
        public MifOptions MIF_Options; // настройки для полигонов MIF
        private int totalItems;
        private const string TabDelimiter = "\t";  // tab
        private int fParent_id;
        public int id;
        //public List<TMyPolygon> Items;

        public TPolygonCollection()  /// Конструктор
        {
            //this.Items = new List<TMyPolygon>();
            this.id = Gen_id.newId;
            this.MIF_Options = new MifOptions();
        }
        public TPolygonCollection(int parent_id)  /// Конструктор
                                               : this()
        {
            this.fParent_id = parent_id;
        }

        public TPolygonCollection(TPolygonCollection polys_)  /// Конструктор
           : this()
        {
            this.AddPolygons(polys_);
        }


        public int Parent_id
        { get { return this.fParent_id; } }
        public int TotalPointCount
        {
            get
            {
                int res = 0;
                foreach (TMyPolygon item in this)
                {
                    res += item.PolygonPointCount;
                }
                return res;
            }
        }

        public TMyRect Get_Bounds
        {
            get
            {
                TMyRect Result = new TMyRect();

                Result.MinX = this[0].Bounds.MinX;
                Result.MaxX = this[0].Bounds.MaxX;
                Result.MaxY = this[0].Bounds.MaxY;
                Result.MinY = this[0].Bounds.MinY;

                for (int i = 1; i <= this.Count; i++)
                {
                    //рассчитаем диапазон карты
                    if (this[i - 1].Bounds != null)
                    {
                        if (this[i - 1].Bounds.MinX < Result.MinX) Result.MinX = this[i - 1].Bounds.MinX;
                        if (this[i - 1].Bounds.MinY < Result.MinY) Result.MinY = this[i - 1].Bounds.MinY;
                        if (this[i - 1].Bounds.MaxX > Result.MaxX) Result.MaxX = this[i - 1].Bounds.MaxX;
                        if (this[i - 1].Bounds.MaxY > Result.MaxY) Result.MaxY = this[i - 1].Bounds.MaxY;
                    }
                }

                return Result;
            }
        }
        public string Defintion;

        public TMyPolygon AddPolygon(object poly_)
        {
            if (poly_ == null) return null;
            if ((poly_.GetType().ToString().Equals("netFteo.Spatial.TMyPolygon")) &&
				(((TMyPolygon)poly_).PointCount >0))
            {
                this.Add((TMyPolygon)poly_);
                return (TMyPolygon)poly_;
            }
			
			if ((poly_.GetType().ToString().Equals("netFteo.Spatial.TMyOutLayer")) &&
		 (((TMyOutLayer)poly_).PointCount > 0))
			{
				TMyPolygon Vpoly = new TMyPolygon();
				Vpoly.AppendPoints((TMyOutLayer)poly_);
				Vpoly.Definition = ((TMyOutLayer)poly_).Definition;
				 this.AddPolygon(Vpoly);
				return Vpoly;
			}

			//"netFteo.Spatial.TPolyLine" ????
			return null;
        }

        public TPolygonCollection AddPolygons(TPolygonCollection polys_)
        {
            TPolygonCollection res = new TPolygonCollection();
            for (int i = 0; i <= polys_.Count - 1; i++)
            {
                this.AddPolygon(polys_[i]);
            }
            return res;
        }

        public double AreaSpatial
        {
            get
            {
                double AreaC = 0;
                for (int i = 0; i <= this.Count - 1; i++)
                {

                    AreaC += this[i].AreaSpatial;
                }
                return AreaC;
            }
        }
        /// <summary>
        /// Общая сумма геометрических площадей
        /// </summary>
        /// <param name="format"></param>
        /// <param name="ReturnCount"></param>
        /// <returns></returns>
        public string AreaSpatialFmt(string format, bool ReturnCount)
        {
            if (ReturnCount) return AreaSpatial.ToString(format) + "  (1.." + this.Count.ToString() + ") ";
            else return AreaSpatial.ToString(format);
        }

        public decimal AreaSpecified
        {
            get
            {
                decimal AreaC = 0;
                for (int i = 0; i <= this.Count - 1; i++)
                {

                    AreaC += this[i].AreaValue;
                }
                return AreaC;
            }
        }
        /// <summary>
        /// Общая сумма указанных площадей
        /// </summary>
        /// <param name="format"></param>
        /// <param name="ReturnCount"></param>
        /// <returns></returns>
        public string AreaSpecifiedFmt(string format, bool ReturnCount)
        {
            if (ReturnCount) return AreaSpecified.ToString(format) + "  (1.." + this.Count.ToString() + ") ";
            else return AreaSpecified.ToString(format);
        }
        /// <summary>
        /// Расхождение в сумме площадей семантически (указанных) и пространственных
        /// </summary>
        public decimal AreaVariance
        {
            get
            {
                return AreaSpecified - (decimal)AreaSpatial;
            }
        }

        public TMyPolygon GetEs(int Layer_id)
        {
            for (int i = 0; i <= this.Count - 1; i++)
            {
                if (this[i].Layer_id == Layer_id)
                {
                    return this[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Проверка на пересечения с полигоном ES
        /// </summary>
        /// <param name="ESs">Полигон для проверки</param>
        /// <returns></returns>
        public PointList CheckES(TMyPolygon ES)
        {
            PointList res = new PointList();
            PointList PlREs;
            totalItems++;// += ES.PolygonPointCount;

            for (int i = 0; i <= this.Count - 1; i++)
            {
                //totalItems += this[i].PolygonPointCount;
                totalItems++;
                PlREs = this[i].FindClip(ES);
                EsChekerProc(this[i].Definition, totalItems, null);
                if (PlREs != null)
                {
                    res.AppendPoints(PlREs);
                }
            }
            return res;
        }


        /// <summary>
        /// Проверка на пересечения с коллекцией полигонов ESs
        /// </summary>
        /// <param name="ESs">Коллекция полигонов</param>
        /// <returns></returns>
        public PointList CheckESs(TPolygonCollection ESs)
        {
            PointList res = new PointList();
            PointList PlREs;
            totalItems = 0;// this.Count + ESs.Count;
            for (int i = 0; i <= ESs.Count - 1; i++)
            {
                PlREs = this.CheckES(ESs[i]);

                if (PlREs != null)
                { res.AppendPoints(PlREs); }
            }
            return res;
        }


        /// <summary>
        /// Поиск общих точек  полигона
        /// </summary>
        /// <param name="ES">Полигон</param>
        /// <returns></returns>
        public PointList CheckCommon(TMyPolygon ES)
        {
            PointList res = new PointList();
            PointList PlREs;
            totalItems += ES.PolygonPointCount;
            for (int i = 0; i <= this.Count - 1; i++)
            {
                {
                    PlREs = this[i].FindCommonPoints(ES);
                    if (PlREs != null)
                    { res.AppendPoints(PlREs); }
                }
            }
            return res;
        }

        /// <summary>
        /// Поиск общих точек в коллекции полигонов
        /// </summary>
        /// <param name="ESs"></param>
        /// <returns></returns>
        public PointList CheckCommon(TPolygonCollection ESs)
        {
            PointList res = new PointList();
            PointList PlREs;
            totalItems = 0;// this.Count + ESs.Count;
            for (int i = 0; i <= ESs.Count - 1; i++)
            {
                PlREs = this.CheckCommon(ESs[i]);
                if (PlREs != null)
                { res.AppendPoints(PlREs); }
            }
            return res;
        }
        /// <summary>
        /// Поиск накрытий
        /// </summary>
        public PointList CheckOverLapping(TPolygonCollection ESs)
        {
            PointList res = new PointList();
            PointList PlREs = null;
            totalItems = 0;// this.Count + ESs.Count;
                           //Есть общие точки?:
            if (CheckCommon(ESs).PointCount > 0)
            { //возожны накрытия через узлы

            }
            for (int i = 0; i <= ESs.Count - 1; i++)
            {
                //PlREs = this.CheckCommon(ESs[i]);
                if (PlREs != null)
                { res.AppendPoints(PlREs); }
            }
            return res;
        }
        private void EsChekerProc(string sender, int process, byte[] Data)
        {
            if (OnChecking == null) return;
            ESCheckingEventArgs e = new ESCheckingEventArgs();
            e.Definition = sender;
            e.Data = Data;
            e.Process = totalItems;
            OnChecking(this, e);
        }

        /// <summary>
        /// Запись в файл коллекции полигонов
        /// </summary>
        /// <param name="FileName">Имя файле, есте....сссно</param>

        public PointList AsPointList()
        {
            PointList Res = new PointList();
            for (int i = 0; i <= this.Count - 1; i++)

            {
                for (int ip = 0; ip <= this[i].Count - 1; ip++)

                    Res.AddPoint(this[i][ip]);
            }
            return Res;

        }

        public List<string> AsList()
        {
            List<string> aslist = new List<string>();
            for (int i = 0; i <= this.Count - 1; i++)
                aslist.Add(this[i].Definition +    "\t" + 
                    this[i].AreaSpatialFmt("0.00") + "\t" +
                                                       (this[i].AreaValue > 0 ? this[i].AreaValue.ToString():"-")  + "\t" +
                                                       ((decimal)this[i].AreaSpatial - this[i].AreaValue).ToString("0.00") +"\t"+
                                                       (this[i].AreaInaccuracy != "" ? this[i].AreaInaccuracy : "-") + "\t" +
                                                       this[i].Has_Changes 
                    );
            
            return aslist;

        }

    }
    #endregion

    public class TCadastralNumbers : List<string>
    {
    }

    public class TCompozitionEZ : TPolygonCollection
    {
        public TCadastralNumbers DeleteEntryParcels;
        public TCadastralNumbers TransformationEntryParcel;
        public TCompozitionEZ()
        {
            this.DeleteEntryParcels = new TCadastralNumbers();
            this.TransformationEntryParcel = new TCadastralNumbers();
        }

        public void AddEntry(string entrynumber, decimal areaEntry, decimal Inaccuracy, int state, TMyPolygon ES)
        {
            if (ES == null) return;
            ES.AreaValue = areaEntry;
            ES.AreaInaccuracy = Inaccuracy != 0 ?  Inaccuracy.ToString(): "";
            ES.Definition = entrynumber;
            ES.State = state;
            this.Add(ES);
        }

        /// <summary>
        /// Периметр всех входящих в ЕЗП. 
        /// Замозговано 11-04-18:
        /// ДА уж.... такая нужная хуяйня. Росреестр без нее никак 
        /// </summary>
        public double TotalPerimeter
        {
            get
            {
                double pery = 0;
                foreach (TMyPolygon poly in this)
                    pery += poly.Perymethr();
                return pery;
            }
        }


    }




    public class OMSPoints : PointList
    {

    }


    #region  Части - ЧЗУ
    public class TmySlot
    {
        private int Fid;
        public string NumberRecord;

        public string AreaGKN;
        public netFteo.Rosreestr.TMyEncumbrances Encumbrances;
        public TMyPolygon EntSpat;
        public TPolygonCollection Contours;
        public TmySlot()
        {
            this.EntSpat = new TMyPolygon();
            this.Contours = new TPolygonCollection();
            this.Encumbrances = new Rosreestr.TMyEncumbrances();
            this.Fid = Gen_id.newId;
        }
        public int id { get { return this.Fid; } }
        // public string EncumbranceType { get { if (this.Encumbrance != null) return this.Encumbrance.Type; return ""; } }


    }

    public class TMySlots : BindingList<TmySlot>
    {
        public TMyPolygon GetEs(int Layer_id)
        {
            for (int i = 0; i <= this.Items.Count - 1; i++)
            {
                if (this.Items[i].EntSpat.Layer_id == Layer_id)
                    return this.Items[i].EntSpat;

                for (int ic = 0; ic <= this.Items[i].Contours.Count - 1; ic++)
                    return (TMyPolygon)this.Items[i].Contours[ic].GetEs(Layer_id);
            }



            return null;
        }
    }
    #endregion

    public static class TMyState
    {
        public static string StateToString(int stateAsInteger)
        {
            switch (stateAsInteger)
            {
                case 0: { return "Новый"; }
                case 1: { return "Ранее учтенный"; }
                case 5: { return "Временный"; }
                case 6: { return "Учтенный"; }
                case 7: { return "Снят с учета"; }
                case 8: { return "Аннулированный"; }
                default: { return ""; }
            }
        }
    }

    public static class TMyColors
    {
        public static System.Drawing.Color StatusToColor(int Status)
        {
            switch (Status)
            {
                case 0: { return System.Drawing.Color.Red; }
                case 1: { return System.Drawing.Color.Green; }
                case 5: { return System.Drawing.Color.Gold; }
                case 6: { return System.Drawing.Color.Black; }
                case 7: { return System.Drawing.Color.Blue; }
                case 8: { return System.Drawing.Color.Blue; }
                default: { return System.Drawing.Color.Black; }
            }
        }
    };






    public class TCadasterItem : Geometry
    {

        public string CN;
        public string DateCreated;
        public TCadasterItem()
        {
            this.CN = "";
            this.id = Gen_id.newId;
        }
        public TCadasterItem(string cn)
        {
            this.CN = cn;
            this.id = Gen_id.newId;
        }

        public TCadasterItem(string cn, int item_id)
        {
            this.CN = cn;
            this.id = item_id;
        }

    }

    #region Земельный участок

    /// <summary>
    /// Справочник dUtilizations_v01.xsd
    /// </summary>
    public class Utilization
    {
        string fUtil; // Значение по классификатору dUtilizations_v01.xsd
        public string UtilbyDoc;
        public bool UtilizationSpecified;
        public Utilization() { UtilizationSpecified = false; }
        public string Untilization
        {
            get
            { return this.fUtil; }
            set
            {
                //this.fUtil = netFteo.Rosreestr.dUtlizationsv01.ItemToName(value);
                this.fUtil = value;
                UtilizationSpecified = true;
            }
        }
    }
    /// <summary>
    /// dAllowedUse_v01
    /// Вид разрешенного использования земельного участка в соответствии с классификатором, 
    /// утвержденным приказом Минэкономразвития России от 01.09.2014 № 540.
    /// </summary>
    public class LandUse
    {
        public string Land_Use;    //Вид разрешенного использования участка по классификатору видов разрешенного использования земельных участков dAllowedUse
        public string DocLandUse; //Реквизиты документа, устанавливающего вид разрешенного использования земельного участка
    }

    public delegate Object ESDelegate();

    public class TMyParcel
    {
        private string FParentCN;

        private int Fid;
        private TPolygonCollection fContours;
        private TCompozitionEZ fCompozitionEZ;
        private TMyPolygon fEntitySpatial;
        public string State;
        public string DateCreated;
        public string CN;
        public string Definition;// Обозначение
        public string CadastralBlock;
        public int CadastralBlock_id;
        public string Name;

        public string AreaGKN;
        /// <summary>
        /// Значение, указаное
        /// </summary>
        public string AreaValue;
        public string Purpose;
        public Utilization Utilization;
        public LandUse Landuse;
        public string Category;
        public string SpecialNote;


        public Rosreestr.TLocation Location;
        public Rosreestr.TMyRights Rights;
        public Rosreestr.TMyRights EGRN;
        public Rosreestr.TMyEncumbrances Encumbrances;
        public Object EntitySpatial_noType;
        // Взаимоисключающее свойство по отношению к .Contours
        public TMyPolygon EntitySpatial
        {
            set
            {
                if (value != null)          // проверим на "эффект волны"
                    this.fContours = null;   // "обнулим" признаки МК
                this.fEntitySpatial = new TMyPolygon();
                this.fEntitySpatial.ImportPolygon(value);

            }
            get
            {
                return this.fEntitySpatial;
            }
        }

        // Взаимоисключающее свойство по отношению к .EntitySpatial
        public TPolygonCollection Contours
        {
            set
            {
                if (value != null)               // проверим на "эффект волны"
                    this.fEntitySpatial = null; ; // "обнулим" признаки 
                this.fContours = new TPolygonCollection(value);
            }
            get
            {
                return this.fContours;
            }
        }
        public TFiles XmlBodyList;
        public TCompozitionEZ CompozitionEZ
        {
            set
            {
                if (value != null)
                    this.fEntitySpatial = null;
                this.fContours = new TPolygonCollection(value); // на свякий случай, как в Осетии: ЕЗП с контурами 
                this.fCompozitionEZ = new TCompozitionEZ();
            }
            get
            {
                return this.fCompozitionEZ;
            }
        }


        public TMySlots SubParcels;
        public List<String> AllOffspringParcel;// Кадастровые номера всех земельных участков, образованных из данного земельного участка
        public List<String> PrevCadastralNumbers; //Кадастровые номера земельных участков, из которых образован
        public List<String> InnerCadastralNumbers;// Кадастровые номера зданий, сооружений, объектов незавершенного строительства, расположенных на земельном участке
        public TMyParcel()
        {
            Utilization = new Utilization();
            Landuse = new LandUse();
            Encumbrances = new Rosreestr.TMyEncumbrances();
            this.XmlBodyList = new TFiles();
            this.SubParcels = new TMySlots();
            this.AllOffspringParcel = new List<string>();
            this.InnerCadastralNumbers = new List<string>();
            this.PrevCadastralNumbers = new List<string>();
            this.Location = new netFteo.Rosreestr.TLocation();
            this.Rights = new Rosreestr.TMyRights();
            //this.SpecialNote = "";
            this.Fid = Gen_id.newId;
            this.AreaGKN = "-1";
        }
        public TMyParcel(string cn) : this() // Вызов Конструктора по умолчанию
        {
            this.CN = cn;
        }
        public TMyParcel(string cn, int parcel_id) : this() // Вызов Конструктора по умолчанию
        {
            this.Fid = parcel_id;
            this.CN = cn;
        }

        public TMyParcel(string CN, string name) : this(CN, Gen_id.newId) //Вызов конструктора переопределенного
        {
            switch (netFteo.Rosreestr.dParcelsv01.ItemToName(name))
            {
                case "Землепользование": { this.EntitySpatial = new TMyPolygon(); this.Name = netFteo.Rosreestr.dParcelsv01.ItemToName(name); return; }
                case "Обособленный участок": { this.EntitySpatial = new TMyPolygon(); this.Name = netFteo.Rosreestr.dParcelsv01.ItemToName(name); return; }
                case "Условный участок": { this.EntitySpatial = new TMyPolygon(); this.Name = netFteo.Rosreestr.dParcelsv01.ItemToName(name); return; }
                case "Многоконтурный участок": { this.Contours = new TPolygonCollection(); this.Name = netFteo.Rosreestr.dParcelsv01.ItemToName(name); return; }
                case "Полигоны dxf":  { this.Contours = new TPolygonCollection(); this.Name = netFteo.Rosreestr.dParcelsv01.ItemToName(name); return; }
                case "Полигоны mif": { this.Contours = new TPolygonCollection(); this.Name = netFteo.Rosreestr.dParcelsv01.ItemToName(name); return; }
                case "Единое землепользование":
                    {
                        //this.Contours = new TPolygonCollection(); // на свякий случай, как в Осетии: ЕЗП с контурами 
                        this.CompozitionEZ = new TCompozitionEZ();
                        this.Name = netFteo.Rosreestr.dParcelsv01.ItemToName(name);
                        return;
                    }
                case "Значение отсутствует": return;
                default: { this.EntitySpatial = new TMyPolygon(); this.Name = netFteo.Rosreestr.dParcelsv01.ItemToName(name); return; }
            }
        }

        public TMyParcel(Rosreestr.dParcelsv01_enum name, string cadastralblock, string definition)
              : this() //Вызов конструктора переопределенного
        {
            this.CadastralBlock = cadastralblock;
            this.Definition = definition;
            this.Name = name.ToString();
        }

        public int id
        {
            get { return this.Fid; }
            set { this.Fid = value; }
        }
        public TmySlot AddSubParcel(string SlotNumber)
        {
            TmySlot Sl = new TmySlot();
            Sl.NumberRecord = SlotNumber;
            this.SubParcels.Add(Sl);
            return this.SubParcels[this.SubParcels.Count - 1];
        }
        public string ParentCN
        {
            get
            {
                if (this.Name == "Обособленный участок") return this.FParentCN;
                if (this.Name == "Условный участок") return this.FParentCN;
                return null;
            }

            set
            {
                /*
                if (this.Name == "Обособленный участок") this.FParentCN = value;

                if (this.Name == "Условный участок") this.FParentCN = value; 
                   else this.FParentCN = null;
                 * */
                this.FParentCN = value;
            }
        }

        public object GetEs(int Layer_id)
        {
            if (this.EntitySpatial != null) if (this.EntitySpatial.Layer_id == Layer_id) return this.EntitySpatial;
            if (this.Contours != null) if (this.Contours.GetEs(Layer_id) != null) return this.Contours.GetEs(Layer_id);
            if (this.Contours != null) if (this.Contours.id == Layer_id) return this.Contours;
            if (this.CompozitionEZ != null) if (this.CompozitionEZ.GetEs(Layer_id) != null) return this.CompozitionEZ.GetEs(Layer_id);
            if (this.SubParcels != null) if (this.SubParcels.GetEs(Layer_id) != null) return this.SubParcels.GetEs(Layer_id);
            return null;
        }

        /// <summary>
        /// Проверка пересечений 
        /// </summary>
        /// <param name="Layer_id"></param>
        /// <returns></returns>
        public PointList CheckEs(TMyPolygon ES)
        {
            switch (this.Name)
            {
                case "Землепользование": { return this.EntitySpatial.FindClip(ES); }
                case "Обособленный участок": { return this.EntitySpatial.FindClip(ES); }
                case "Условный участок": { return this.EntitySpatial.FindClip(ES); }
                case "Многоконтурный участок": { return this.Contours.CheckES(ES); }
                // case "Единое землепользование": { return this.CompozitionEZ.CheckES(ES); }
                case "Значение отсутствует": { return null; }
                default: return null;
            }
        }

        public PointList CheckEs(TPolygonCollection Contours)
        {
            switch (this.Name)
            {
                case "Землепользование": { return this.EntitySpatial.FindClip(Contours); }
                case "Обособленный участок": { return this.EntitySpatial.FindClip(Contours); }
                case "Условный участок": { return this.EntitySpatial.FindClip(Contours); }
                case "Многоконтурный участок": { return this.Contours.CheckESs(Contours); }
                //  case "Единое землепользование": { return this.CompozitionEZ.CheckES(Contours); }
                case "Значение отсутствует": { return null; }
                default: return null;
            }
        }
        public string Area(string Format)
        {
            switch (this.Name)
            {
                case "Землепользование": { return this.EntitySpatial.AreaSpatialFmt(Format); }
                case "Обособленный участок": { return this.EntitySpatial.AreaSpatialFmt(Format); }
                case "Условный участок": { return this.EntitySpatial.AreaSpatialFmt(Format); }
                case "Многоконтурный участок": { return this.Contours.AreaSpatialFmt(Format, true); }
                case "Единое землепользование": { return this.CompozitionEZ.AreaSpatialFmt(Format, true); }
                case "Значение отсутствует": { return ""; }
                default: return "";
            }
        }

        public double Area_float
        {
            get
            {
                switch (this.Name)
                {
                    case "Землепользование": { return this.EntitySpatial.AreaSpatial; }
                    case "Обособленный участок": { return this.EntitySpatial.AreaSpatial; }
                    case "Условный участок": { return this.EntitySpatial.AreaSpatial; }
                    case "Многоконтурный участок": { return this.Contours.AreaSpatial; }
                    case "Единое землепользование": { return this.CompozitionEZ.AreaSpatial; }
                    case "Значение отсутствует": { return -1; }
                    default: return -1;
                }
            }
        }
    }
    #endregion
    /// <summary>
    /// Расположение на плане
    /// </summary>
    public class Position
    {
        /// <summary>
        /// Номер на плане
        /// </summary>
        public string NumberOnPlan;
        /// <summary>
        /// Планы
        /// </summary>
        public List<string> Plans_Plan_JPEG;

        public Position()
        {
            this.Plans_Plan_JPEG = new List<string>();
        }
        /// <summary>
        /// Единственный план
        /// </summary>
        public string Plan00_JPEG
        {
            get
            {
                if (this.Plans_Plan_JPEG.Count == 1)
                    return this.Plans_Plan_JPEG[0];
                else return "";
            }
        }

    }

    public class TLevel
    {
        public string Type;
        public string Number;
        /// <summary>
        /// Расположение в пределах объекта недвижимости
        /// </summary>
        public Position Position;
        public TLevel(string type, string number, string numberonplan)
        {
            this.Number = number;
            this.Type = Rosreestr.dTypeStorey_v01.ItemToName(type);
            this.Position = new Position();
            this.Position.NumberOnPlan = numberonplan;
        }
        public void AddPlan(string jpegname)
        {
            this.Position.Plans_Plan_JPEG.Add(jpegname);
        }
    }

    public class TPositionInObject
    {
        public List<TLevel> Levels;
        public TPositionInObject()
        {
            this.Levels = new List<TLevel>();
        }

    }
    /// <summary>
    /// Основная характеристика
    /// </summary>
    public class TKeyParameter
    {
        public string Type; //Тип характеристики  -dTypeParameter_v01.xsd
        public string Value; //Значение (величина в метрах (кв. метрах для площади, куб. метрах для объема))
    }

    /// <summary>
    /// ХАРАКТЕРИСТИКИ ОБЪЕКТОВ КАПИТАЛЬНОГО СТРОИТЕЛЬСТВА
    /// </summary>
    public class TKeyParameters : List<TKeyParameter>
    {
        public TKeyParameters()
        {
            //this.KeyParameters = new TKeyParameters();
        }
        public void AddParameter(string type, string value)
        {
            TKeyParameter param = new TKeyParameter();
            param.Type = Rosreestr.dTypeParameter_v01.ItemToName(type);
            param.Value = value;
            this.Add(param);
        }
    }



    //Для оксов отдельный кадастровый юнит, №2
    public class TCadasterItem2 : TCadasterItem
    {
        public TKeyParameters OldNumbers; //Кадастровые номера земельных участков, из которых образован
                                        //    public TKeyParameters KeyParameters; // для всех оКСОв, и для зданий  -в них этажность будем вносить И разную хуйню
        public decimal Area;
        public string Notes; // Особые отметки
  //      public Rosreestr.TAddress Address;
		public Rosreestr.TLocation Location;
		public TCadasterItem2()
        {
            this.OldNumbers = new TKeyParameters();
        }
    }


    public class TFlat : TCadasterItem2
    {
        private string fAssignationCode;
        private string fAssignationType;
        public TKeyParameters KeyParameters; // 
        public TPositionInObject PositionInObject;
        /// <summary>
        /// Назначение помещения
        /// </summary>
        public string AssignationCode
        {
            get
            {
                switch (fAssignationCode)
                {
                    case "Item206001000000": { return "Нежилое помещение"; }
                    case "Item206002000000": { return "Жилое помещение"; }
                    default: { return ""; }
                }
            }
            set
            {
                this.fAssignationCode = value;
            }
        }

        /// <summary>
        ///  Вид помещения
        /// </summary>
        public string AssignationType
        {
            get
            {
                switch (fAssignationType)
                {
                    case "Item205001000000": { return "Квартира"; }
                    case "Item205002000000": { return "Комната"; }
                    default: { return ""; }
                }
            }

            set
            {
                this.fAssignationType = value;
            }
        }

        public TFlat(string cn)
        {
            this.PositionInObject = new TPositionInObject();
            this.CN = cn;
			this.Location = new Rosreestr.TLocation();
            this.KeyParameters = new TKeyParameters();
        }
    }

    public class TFlats : List<TFlat>
    {

        public decimal TotalArea
        {
            get
            {
                decimal Sum = 0;
                foreach (TFlat flat in this)
                    Sum += flat.Area;
                return Sum;
            }
        }

        public int CountbyType(string type)
        {
            int res = 0;
            foreach (TFlat fl in this)
            {
                if (fl.AssignationType == type)
                    res++;
            }
            return res;
        }

        public int CountbyCode(string code)
        {
            int res = 0;
            foreach (TFlat fl in this)
            {
                if (fl.AssignationCode == code)
                    res++;
            }
            return res;
        }

        public decimal AreabyCode(string code)
        {
            decimal res = 0;
            foreach (TFlat fl in this)
            {
                if (fl.AssignationCode == code)
                    res += fl.Area;
            }
            return res;
        }

        public decimal Area
        {
            get
            {
                decimal Sum = 0;
                foreach (TFlat flat in this)
                    Sum += flat.Area;
                return Sum;
            }
        }


        public void AddFlat(TFlat flat)
        {
            this.Add(flat);
        }
    }

    public class TConstruction : TCadasterItem2
    {
        private Object fEntitySpatial; //Может быть многоконтурным???
        public string AssignationName;  // Назначение сооружения; 
        //public TMyPolygon EntitySpatial; //Может быть многоконтурным???
        public TKeyParameters KeyParameters; // 
		/*
		public Object ES
        {
            get { return this.fEntitySpatial; }
            set
            {
                if (value == null) return;
                string test = value.GetType().Name;

                if (value.GetType().Name == "TMyPolygon")
                    this.fEntitySpatial = (TMyPolygon)value;

                if (value.GetType().Name == "TPolyLines")
                    this.fEntitySpatial = (TPolyLines)value;
				
				if (value.GetType().Name == "TCircle")
					this.fEntitySpatial = (TCircle)value;
			}
        }
		*/
		public TConstruction()
        {
            this.KeyParameters = new TKeyParameters();
        }
    }

    public class TUncompleted : TCadasterItem2
    {
        private Object fEntitySpatial; //Может быть многоконтурным???
        public string AssignationName;  // Проектируемое назначение
        public TKeyParameters KeyParameters; // 
        public string DegreeReadiness; //Степень готовности в процентах
        public Object ES
        {
            get { return this.fEntitySpatial; }
            set
            {
                if (value == null) return;
                string test = value.GetType().Name;

                if (value.GetType().Name == "TMyPolygon")
                    this.fEntitySpatial = (TMyPolygon)value;

                if (value.GetType().Name == "TPolyLines")
                    this.fEntitySpatial = (TPolyLines)value;

            }
        }

	

        public TUncompleted()
        {
            this.KeyParameters = new TKeyParameters();
        }
    }

    public class TBuilding : TCadasterItem2
    {
        private string fAssBuilding;
        private Object fEntitySpatial; //Может быть многоконтурным???
        //public List<TFlat> Flats;//Кадастровые номера помещений, расположенных в объекте недвижимости
        public TKeyParameters KeyParameters; // 
		public TFlats Flats; // Помещения, расположенных в объекте недвижимости
		/// <summary>
		/// Кадастровый номер земельного участка (земельных участков), в пределах которого (которых) расположен данный объект недвижимости (сведения ГКН)
		/// </summary>
		//public TMyPolygon EntitySpatial; //Может быть многоконтурным???
		public Object ES
        {
            get { return this.fEntitySpatial; }
            set
            {
                if (value == null) return;
                string test = value.GetType().Name;

                if (value.GetType().Name == "TMyPolygon")
                    this.fEntitySpatial = (TMyPolygon)value;

                if (value.GetType().Name == "TPolyLines")
                    this.fEntitySpatial = (TPolyLines)value;

            }
        }

        public TBuilding()
        {
            this.fEntitySpatial = new TMyPolygon();
            this.KeyParameters = new TKeyParameters();
            this.Flats = new TFlats();//new List<TFlat>();
        }

        public string AssignationBuilding
        {
            get { return netFteo.Rosreestr.dAssBuildingv01.ItemToName(fAssBuilding); }
            set { this.fAssBuilding = value; }
        }
    }



    public class TMyRealty : TCadasterItem2
    {
        private string fObjectType;
        public string CadastralBlock;
        public List<string> ParentCadastralNumbers; //
        public string Name;//Наименование
        //public decimal Area;
        public string Type;
		public string Floors;
		public string UndergroundFloors;
		public Rosreestr.TMyRights Rights; // как бы ГКН-Права
        public Rosreestr.TMyRights EGRN;
		public TEntitySpatial ES2;
		//SubTypes:
		public TBuilding Building;
		public TFlat Flat;
		public TConstruction Construction;
		public TUncompleted Uncompleted;

		public string ObjectType
        {
            set { this.fObjectType = value; }
            get { return this.fObjectType; }
        }

       // public Rosreestr.TAddress Address;
        public TMyRealty()
        {
            this.id = Gen_id.newId;
            this.ParentCadastralNumbers = new List<string>();
			this.Location = new Rosreestr.TLocation();
        }

        public TMyRealty(string cn) : this()
        {
            this.CN = cn;
        }

        public TMyRealty(string cn, Rosreestr.dRealty_v03 rlt_type) : this(cn)
        {
            switch (rlt_type)
            {
                case Rosreestr.dRealty_v03.Здание: { this.Building = new TBuilding(); break; }
                case Rosreestr.dRealty_v03.Помещение: { this.Flat = new TFlat(cn); break; }
                case Rosreestr.dRealty_v03.Сооружение: { this.Construction = new TConstruction(); break; }
                case Rosreestr.dRealty_v03.Объект_незавершённого_строительства: { this.Uncompleted = new TUncompleted(); break; }

                default: this.Building = new TBuilding(); break;
            }
        }

		/// <summary>
		/// Contructor for KPT11 realty`s
		/// </summary>
		/// <param name="cn"></param>
		/// <param name="type_code"></param>
		public TMyRealty(string cn, string type_code) : this(cn)
		{
			this.ObjectType = type_code;
			switch (type_code)
			{
				case "002001002000":
					{ this.Building = new TBuilding();
						break; }
				case "002001004000" :
				 { this.Construction = new TConstruction(); break; }
				case "002001005000":
				{ this.Uncompleted = new TUncompleted(); break; }

				default: this.Building = new TBuilding(); break;
			}
		}

		public TKeyParameters KeyParameters
        {
            get
            {
                if (this.Building != null) return Building.KeyParameters;
                if (this.Construction != null) return Construction.KeyParameters;
                if (this.Uncompleted != null) return Uncompleted.KeyParameters;
                if (this.Flat != null) return Flat.KeyParameters;
                return null;
            }
        }
 
    }


    public class TRealtys : List<TMyRealty>
    {
        public TMyRealty GetItem(int item_id)
        {
            foreach (TMyRealty item in this)
            {
                if (item.id == item_id)
                    return item;
            }
            return null;
        }

        public Object GetEs(int Layer_id)
        {
            for (int i = 0; i <= this.Count - 1; i++)
            {

                if (this[i].Building != null)

                    if ((this[i]).Building.ES != null)
                    {

                        if ((this[i]).Building.ES.GetType().Name == "TPolyLines")
                        {
                            if (((TPolyLines)(this[i]).Building.ES).ParentID == Layer_id)
                                return (TPolyLines)(this[i]).Building.ES;
                        }

                        if ((this[i]).Building.ES.GetType().Name == "TPolyLines")
                        {
                            TPolyLines ES = (TPolyLines)this[i].Building.ES;

                            for (int il = 0; il <= ES.Count - 1; il++)
                                if (ES[il].Layer_id == Layer_id)
                                    return ES[il];
                        }

                        if (this[i].Building.ES.GetType().Name == "TMyPolygon")
                            if (((TMyPolygon)this[i].Building.ES).Layer_id == Layer_id)
                                return (TMyPolygon)this[i].Building.ES;
                    }

				/* TODO kill, goes to ES2
                if (this[i].Construction != null)

                    if ((this[i]).Construction.ES != null)
                    {

                        if ((this[i]).Construction.ES.GetType().Name == "TPolyLines")
                        {
                            if (((TPolyLines)(this[i]).Construction.ES).ParentID == Layer_id)
                                return (TPolyLines)(this[i]).Construction.ES;
                        }

                        if ((this[i]).Construction.ES.GetType().Name == "TPolyLines")
                        {
                            TPolyLines ES = (TPolyLines)this[i].Construction.ES;

                            for (int il = 0; il <= ES.Count - 1; il++)
                                if (ES[il].Layer_id == Layer_id)
                                    return ES[il];
                        }

                        if (this[i].Construction.ES.GetType().Name == "TMyPolygon")
                            if (((TMyPolygon)this[i].Construction.ES).Layer_id == Layer_id)
                                return (TMyPolygon)this[i].Construction.ES;
                    }

				*/
				if (this[i].Uncompleted != null)
					if ((this[i]).Uncompleted.ES != null)
					{

						if ((this[i]).Uncompleted.ES.GetType().Name == "TPolyLines")
						{
							if (((TPolyLines)(this[i]).Uncompleted.ES).ParentID == Layer_id)
								return (TPolyLines)(this[i]).Uncompleted.ES;
						}

						if ((this[i]).Uncompleted.ES.GetType().Name == "TPolyLines")
						{
							TPolyLines ES = (TPolyLines)this[i].Uncompleted.ES;

							for (int il = 0; il <= ES.Count - 1; il++)
								if (ES[il].Layer_id == Layer_id)
									return ES[il];
						}

						if (this[i].Uncompleted.ES.GetType().Name == "TMyPolygon")
							if (((TMyPolygon)this[i].Uncompleted.ES).Layer_id == Layer_id)
								return (TMyPolygon)this[i].Uncompleted.ES;
					}

				//again for ES2 (common spatial data collection)
				if ((this[i]).ES2 != null)
				{
					foreach (IGeometry feature in (this[i]).ES2)
					{
						//string test = feature.GetType().Name;
						if (feature.GetType().Name == "TCircle")
						{
							if (((TCircle)feature).id == Layer_id)
								return (TCircle)feature;
						}

						if (feature.GetType().Name == "TPolyLine")
						{
							if (((TPolyLine)feature).Layer_id == Layer_id)
								return (TPolyLine)feature;
						}

						if (feature.GetType().Name == "TMyPolygon")
							if (((TMyPolygon)feature).Layer_id == Layer_id)
								return (TMyPolygon)feature;
					}
				}
			}
			return null;
        }
    }
    public class TMyParcelCollection
    {
        public List<TMyParcel> Parcels;

        public TMyParcelCollection()
        {
            this.Parcels = new List<TMyParcel>();

        }
        public TMyParcel AddParcel(TMyParcel Parcel)
        {
            this.Parcels.Add(Parcel);
            return this.Parcels[this.Parcels.Count - 1];
        }

        public int AddParcels(TMyParcelCollection parcels)
        {
            foreach (TMyParcel inParcel in parcels.Parcels)
            {
                this.Parcels.Add(inParcel);
            }

            return parcels.Parcels.Count;
        }


    }

    //Oбъекты землеустройства
    public class TCoordSystem
    {
        public string Name;
        public string CSid;
        public TCoordSystem(string name, string id)
        {
            this.Name = name;
            this.CSid = id;
        }
    }

    public class TCoordSystems : List<TCoordSystem>
    {
    }

    public class TDocument
    {
        public string Number;
        public string Name;
        public string CodeDocument;
        public string IssueOrgan;
        public string Serial;
        public string Doc_Date;
    }


	public enum dFileTypes
	{
		KPT11 = 111,
		KPT10 = 110,
		KPT09 = 109,
		KPT08 = 108,
		KPT07 = 107,
		Undefined = -1
	}

	/// <summary>
	/// класс Файл. Представляет xml-файл всех видов
	/// </summary>
	public class TFile : TDocument
    {
        public int id;
		public dFileTypes Type;
        public string AccessCode;
        public string FileName;
        public string RequestNum;
        public string RootName;
        public string xmlns;
        public double xmlSize_SQL; // размер вычисленный сервером
                                   // public  MemoryStream FileBody ;
        private System.Xml.XmlDocument fFileBody;
        public System.Xml.XmlDocument xml_file_body
        {
            get
            {
                /* System.Xml.XmlDocument resDoc = new System.Xml.XmlDocument();
                 resDoc.Load(FileBody);
                 return resDoc;
                 */
                return this.fFileBody;
            }
        }
        public void DownLoadFileBody(MemoryStream filebody)
        {
            filebody.Seek(0, 0);
            if (this.fFileBody == null)
                this.fFileBody = new System.Xml.XmlDocument();
            this.fFileBody.Load(filebody);
        }

        public TFile()
        {
            // this.fFileBody = new System.Xml.XmlDocument();
        }

    }
    /// <summary>
    /// Коллекция файлов типа Файл
    /// </summary>
    public class TFiles : List<TFile>
    {
        public TFile DownLoadFileBody(int file_id, MemoryStream filebody)
        {
            foreach (TFile file in this)
            {
                if (file.id == file_id)
                {
                    file.DownLoadFileBody(filebody);
                    //FileBody = filebody;
                    return file;
                }
            }
            return null;
        }

        public bool BodyEmpty(int file_id)
        {
            foreach (TFile file in this)
            {
                if (file.id == file_id)
                {
                    if (file.xml_file_body == null)
                        return true;
                }
            }
            return false;
        }

		public dFileTypes GetFileType(int file_id)
		{
			foreach (TFile file in this)
			{
				if (file.id == file_id)
				{
					return file.Type;
				}
			}
			return dFileTypes.Undefined;
		}

		public System.Xml.XmlDocument GetFileBody(int file_id)
        {
            foreach (TFile file in this)
            {
                if (file.id == file_id)
                {
                    return file.xml_file_body;
                }
            }
            return null;
        }

		public TFile GetFile(int file_id)
        {
            foreach (TFile file in this)
            {
                if (file.id == file_id)
                {
                    return file;
                }
            }
            return null;
        }

		public string GetFileName(int file_id)
		{
			foreach (TFile file in this)
			{
				if (file.id == file_id)
				{
					return file.FileName;
				}
			}
			return null;
		}
	}

    public class TFileHistoryItem
    {
        public int id;
        public string hi_data;
        public string hi_comment;
        public string hi_host;
        public string hi_ip;
        public string hi_item_id; // id объекта, вызвавшего событие
        public string hi_systemusername;
        public string hi_dbusername;
        public TFileHistoryItem(int item_id)
        {
            this.id = item_id;
        }
    }
    public class TFileHistory : List<TFileHistoryItem>
    {
        int Block_id;
        public TFileHistory(int block_id)
        {
            this.Block_id = block_id;
        }
    }

    //Границы между субъектами РФ, границы населенных пунктов, муниципальных образований, расположенных в кадастровом квартале
    public class TBound
    {
        public string Description;
        public string TypeName;
        public string AccountNumber;
        public int id;
        public TBound(string Descr, string typename)
        {
            this.Description = Descr;
            this.TypeName = typename;
            this.id = Gen_id.newId;
        }
        public TMyPolygon EntitySpatial;
    }

    //Oбъекты землеустройства - зоны и границы
    public class TZone
    {
        const string TerritorialZone = "TerritorialZone";
        public int id;
        public string Description;
        public string AccountNumber;
        public string TypeName;
        public string ContentRestrictions; // SpecialZone
        public List<string> PermittedUses;
        public List<TDocument> Documents;
        public TMyPolygon EntitySpatial;
        public TZone(string accountnumber)
        {
            this.AccountNumber = accountnumber;
            this.Documents = new List<TDocument>();
            this.id = Gen_id.newId;
        }

        public void AddContentRestrictions(string contentrestrictions)
        {
            this.ContentRestrictions = contentrestrictions; ;
            this.TypeName = "Зона с особыми условиями использования территорий";
            this.PermittedUses = null;
        }

        public void AddPermittedUses(List<string> permitteduses)
        {
            if (this.PermittedUses == null) this.PermittedUses = new List<string>();
            this.PermittedUses.AddRange(permitteduses);
            this.TypeName = "Территориальная зона";
        }
        public void AddDocument(string number, string name,
                                string codedocument, string issueorgan,
                                string serial, string doc_date)
        {
            TDocument doc = new TDocument();
            doc.CodeDocument = codedocument;
            doc.Doc_Date = doc_date;
            doc.IssueOrgan = issueorgan;
            doc.Name = name;
            doc.Number = number;
            doc.Serial = serial;
            this.Documents.Add(doc);
        }

    }



    public class TZonesList : List<TZone>
    {
        public TPolygonCollection GetEs()
        {
            TPolygonCollection Res = new TPolygonCollection();
            for (int i = 0; i <= this.Count - 1; i++)
            {
                if (this[i].EntitySpatial != null)
                    Res.AddPolygon(this[i].EntitySpatial);
            }
            return Res;
        }
        public TMyPolygon GetEsId(int Layer_id)
        {
            for (int i = 0; i <= this.Count - 1; i++)
            {
                if (this[i].EntitySpatial.Layer_id == Layer_id)
                    return this[i].EntitySpatial;
            }

            return null;
        }
        public void AddZone(TZone zone, string contentrestrictions)
        {
            this.Add(zone);
            zone.ContentRestrictions = contentrestrictions;
        }
        public void AddZone(TZone zone, List<string> PermittedUses)
        {
            this.Add(zone);
            zone.PermittedUses.AddRange(PermittedUses);
        }
    }

    public class TBoundsList : List<TBound>
    {
        public TMyPolygon GetEs(int Layer_id)
        {
            for (int i = 0; i <= this.Count - 1; i++)
            {
                if (this[i].EntitySpatial.Layer_id == Layer_id)
                    return this[i].EntitySpatial;
            }

            return null;
        }
    }


    public class TCadastralDistrict : TCadasterItem
    {
        public string Name;
    }

    #region Кадастровый квартал

    public class TMyCadastralBlock
    {
        public int id;
        public bool HasParcels;
        public TMyPolygon Entity_Spatial;
        public TMyParcelCollection Parcels;
        public TRealtys ObjectRealtys;
        public PointList OMSPoints;
        public TBoundsList GKNBounds;
        public TZonesList GKNZones;
        public TFiles KPTXmlBodyList;
        public string CN;
        public string Name;
        public string Comments;
        public TMyCadastralBlock()
        {
            OMSPoints = new OMSPoints();
            Parcels = new TMyParcelCollection();
            ObjectRealtys = new TRealtys();
            GKNBounds = new TBoundsList();
            GKNZones = new TZonesList();
            Entity_Spatial = new TMyPolygon();
            KPTXmlBodyList = new TFiles();
        }
        public TMyCadastralBlock(string cn) : this() // call constructor with ()
        {
            this.CN = cn;
        }

        public OMSPoints AddOmsPoint(PointList oms)
        {
            OMSPoints res = new OMSPoints();
            foreach (Point pt in oms)
               res.AddPoint( AddOmsPoint(pt));
            return res;
        }

        public Point AddOmsPoint(Point OMS)
        {
            this.OMSPoints.AddPoint(OMS);
            return this.OMSPoints[this.OMSPoints.PointCount - 1];
        }

        public TMyRealty AddOKS(TMyRealty newOKS)
        {
            this.ObjectRealtys.Add(newOKS);
            return (TMyRealty)this.ObjectRealtys[this.ObjectRealtys.Count - 1];

        }
        public TBound AddBound(TBound newBound)
        {
            this.GKNBounds.Add(newBound);
            return this.GKNBounds[this.GKNBounds.Count - 1];

        }
        public TZone AddZone(TZone zone)
        {
            this.GKNZones.Add(zone);
            return this.GKNZones[this.GKNZones.Count - 1];

        }

        //ИПД в квартале
        public Object GetEs(int Layer_id)
        {
            for (int i = 0; i <= this.Parcels.Parcels.Count - 1; i++)
            {
                if (this.Parcels.Parcels[i].GetEs(Layer_id) != null)
                    return this.Parcels.Parcels[i].GetEs(Layer_id);
            }
            if (this.GKNBounds.GetEs(Layer_id) != null)
                return this.GKNBounds.GetEs(Layer_id);
            if (this.GKNZones.GetEsId(Layer_id) != null)
                return this.GKNZones.GetEsId(Layer_id);

            if (this.ObjectRealtys.GetEs(Layer_id) != null)
                return this.ObjectRealtys.GetEs(Layer_id);

            if (this.Entity_Spatial.PointCount > 0)
                return this.Entity_Spatial;
            return null;
        }

        public object GetObject(int id)
        {
            for (int i = 0; i <= this.Parcels.Parcels.Count - 1; i++)
            {
                if (this.Parcels.Parcels[i].id == id)
                    return this.Parcels.Parcels[i];
            }

            for (int i = 0; i <= this.ObjectRealtys.Count - 1; i++)
            {
                if (this.ObjectRealtys[i].Building != null)
                {
                    foreach (TFlat flat in this.ObjectRealtys[i].Building.Flats)
                        if (flat.id == id)
                            return flat;
                }

                if (this.ObjectRealtys[i].Flat != null)
                {
                    if (this.ObjectRealtys[i].Flat.id == id)
                        return this.ObjectRealtys[i].Flat;
                }

            }

            for (int i = 0; i <= this.Parcels.Parcels.Count - 1; i++)
            {
                if (this.Parcels.Parcels[i].CompozitionEZ != null)
                    for (int ij = 0; ij <= this.Parcels.Parcels[i].CompozitionEZ.Count - 1; ij++)
                        if (this.Parcels.Parcels[i].CompozitionEZ[ij].Layer_id == id)
                            return this.Parcels.Parcels[i].CompozitionEZ[ij];
            }

            for (int i = 0; i <= this.Parcels.Parcels.Count - 1; i++)
            {
                if (this.Parcels.Parcels[i].Contours != null)
                    if (this.Parcels.Parcels[i].Contours.id == id)
                        return this.Parcels.Parcels[i].Contours;
            }

            //если ищем чзу:
            for (int i = 0; i <= this.Parcels.Parcels.Count - 1; i++)

                for (int sli = 0; sli <= this.Parcels.Parcels[i].SubParcels.Count - 1; sli++)
                {
                    if (this.Parcels.Parcels[i].SubParcels[sli].id == id)
                        return this.Parcels.Parcels[i].SubParcels[sli];
                }

            for (int i = 0; i <= this.ObjectRealtys.Count - 1; i++)
            {
                if (((TMyRealty)this.ObjectRealtys[i]).id == id)
                    return this.ObjectRealtys[i];
            }

            for (int i = 0; i <= this.GKNZones.Count - 1; i++)
            {
                if (this.GKNZones[i].id == id)
                    return this.GKNZones[i];
            }

            for (int i = 0; i <= this.GKNBounds.Count - 1; i++)
            {
                if (this.GKNBounds[i].id == id)
                    return this.GKNBounds[i];
            }

            return null;
        }
    }
    #endregion

    #region Кадастровый район (коллекция кадастровых кварталов)
    /// <summary>
    /// Кадастровый район (коллекция кадастровых кварталов)
    /// </summary>
    public class TMyBlockCollection
    {
        public string DistrictCN;   // Кадастровый номер района
        public string DistrictName; // Название района
        public List<TMyCadastralBlock> Blocks;
        public OMSPoints OMSPoints
        {
            get
            {
                OMSPoints res = new OMSPoints();
                foreach (TMyCadastralBlock bl in Blocks)
                    res.AppendPoints(bl.OMSPoints);
                return res;
            }
        }

        public Rosreestr.TMyRights EGRN; // Временно  прикручиваем сюды ???
        public TMyBlockCollection()
        {
            this.Blocks = new List<TMyCadastralBlock>();
            this.CSs = new TCoordSystems();
        }
        public string SingleCN() // Если квартал один, вернуть его CN
        {
            if (this.Blocks.Count == 1)
                return this.Blocks[0].CN;
            else
                return null;
        }

        public Object GetEs(int Layer_id)
        {
            for (int i = 0; i <= this.Blocks.Count - 1; i++)
            {
                Object Entity = this.Blocks[i].GetEs(Layer_id);
                if (Entity != null)
                    return Entity;
                {

                }
            }
            return null;
        }

        /// <summary>
        /// Выборка ОИПД из коллекции зон
        /// </summary>
        /// <param name="ZoneType">"1 - тер. зоны"</param>
        /// <returns></returns>
        public TPolygonCollection GetZonesEs(int ZoneType)
        {
            TPolygonCollection Res = new TPolygonCollection();
            for (int i = 0; i <= this.Blocks.Count - 1; i++)
                for (int iz = 0; iz <= this.Blocks[i].GKNZones.Count - 1; iz++)
                {
                    if (((this.Blocks[i].GKNZones[iz].PermittedUses != null) &&
                        (ZoneType == 1)) ||
                        ((this.Blocks[i].GKNZones[iz].PermittedUses == null) &&
                        (ZoneType != 1))
                        )
                    {
                        if (this.Blocks[i].GKNZones[iz].EntitySpatial != null)

                            Res.AddPolygon(this.Blocks[i].GKNZones[iz].EntitySpatial);
                    }

                }
            return Res;
        }

        public TPolygonCollection GetRealtyEs()
        {
            TPolygonCollection Res = new TPolygonCollection();
            for (int i = 0; i <= this.Blocks.Count - 1; i++)
                for (int iz = 0; iz <= this.Blocks[i].ObjectRealtys.Count - 1; iz++)
                {

					/*
                  if ((this.Blocks[i].ObjectRealtys[iz].Construction != null) &&
                        (this.Blocks[i].ObjectRealtys[iz].Construction.ES != null))
                            Res.AddPolygon(this.Blocks[i].ObjectRealtys[iz].Construction.ES);
				  */
                    if (
                        (this.Blocks[i].ObjectRealtys[iz].Building != null) &&
                        (this.Blocks[i].ObjectRealtys[iz].Building.ES != null)
                        )
                        Res.AddPolygon(this.Blocks[i].ObjectRealtys[iz].Building.ES);

                    if (
                        (this.Blocks[i].ObjectRealtys[iz].Uncompleted    != null) &&
                        (this.Blocks[i].ObjectRealtys[iz].Uncompleted.ES != null)
                        )
                        Res.AddPolygon(this.Blocks[i].ObjectRealtys[iz].Uncompleted.ES);

                }
            return Res;
        }



        public TMyCadastralBlock GetBlock(int id)
        {
            for (int i = 0; i <= this.Blocks.Count - 1; i++)
            {
                if (this.Blocks[i].id == id)
                    return this.Blocks[i];
            }
            return null;
        }
        public TMyParcel GetParcel(int id)
        {
            return (TMyParcel)this.GetObject(id);
        }
        public object GetObject(int id)
        {
            for (int i = 0; i <= this.Blocks.Count - 1; i++)
            {
                if (this.Blocks[i].GetObject(id) != null)
                    return this.Blocks[i].GetObject(id);
            }
            return null;
        }
        public TCoordSystems CSs;
    }
    #endregion

    #region Полилиния (знает площадь)
    public class TPolyLine : TMyOutLayer, IGeometry
    {
        public double Length()
        {
            return this.Perymethr();
        }

     
        public TPolyLine()
        {
            // this.MainPoint = new netFteoPoints();
        }

    }


	public class TPolyLines : BindingList<TPolyLine>
	{
		public int ParentID;
		public TPolyLines(int parentid)
		{
			this.ParentID = parentid;
		}
	}

	/// <summary>
	/// Getero spatial data collection -lines, polygons, points, circles 
	/// </summary>
	public class TEntitySpatial : List<IGeometry>, IGeometry
	{
		private int fid;
		public int id
		{
			get { return this.fid; }
			set { this.fid = value; }
		}
		public TEntitySpatial()
		{
			this.id = Gen_id.newId;
		}
	}

    #endregion
}


