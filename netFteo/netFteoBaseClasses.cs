using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Windows.Forms;


/// <summary>
/// Class for checking type names
/// </summary>
public static class NetFteoTypes
{
    public static string LibraryName =   String.Format(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString());
    public static string LibraryVersion = String.Format(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
    public static string Parcel = new netFteo.Cadaster.TParcel().GetType().ToString();
    public static string RealEstate = new netFteo.Cadaster.TRealEstate().GetType().ToString();
    public static string Block = new netFteo.Cadaster.TCadastralBlock().GetType().ToString();
    public static string Point = new netFteo.Spatial.TPoint().GetType().ToString();
    public static string PointList = new netFteo.Spatial.PointList().GetType().ToString();
    public static string Border = new netFteo.Spatial.TBorder("fake border", 3.14).GetType().ToString();//  "netFteo.Spatial.TBorder";
    public static string BorderLength = "Length";
    //public static string Border = new netFteo.Spatial.TBorder.GetType().ToString();
    public static string Polygon = new netFteo.Spatial.TPolygon().GetType().ToString();
    public static string District = new netFteo.Cadaster.TCadastralDistrict().GetType().ToString();
    public static string SubRf = new netFteo.Cadaster.TCadastralSubject().GetType().ToString();
}


#region Common primary key generator

/// <summary>
/// Common primary key generator
/// </summary>
public static class Gen_id
{
    private static int _id;
    public static int newId
    {
        get
        {
            _id++;
            return _id;
        }
    }
    public static void Reset()
    {
        _id = 0;
    }
}
#endregion

namespace netFteo.Spatial

{

  
    #region Base classes of all base classes 

    /// <summary>
    /// Мать всех матерей
    /// </summary>
    public interface IGeometry//, ICloneable, IComparable, IComparable<IGeometry>, IPuntal
    {
        long id { get; set; }
        int State { get; set; }
        string Definition { get; set; }
        string Name { get; set; }
        string TypeName { get; }
        string LayerHandle { get; set; } // handle of the layer (dxf ecosystem)

        /// <summary>
        /// Fraq ordinates values to format
        /// </summary>
        /// <param name="Format"></param>
        void Fraq(string Format);

        /// <summary>
        /// Listing geometry as ListView items (columns, rows)
        /// </summary>
        /// <param name="LV"></param>
        /// <param name="SetTag"></param>
        void ShowasListItems(ListView LV, bool SetTag);

        /// <summary>
        /// Get center (medium) ordinates of geometry
        /// </summary>
        TPoint AverageCenter { get; }

        /// <summary>
        /// Расчет масштаба на поверхность (z.B: WPF canvas) 
        /// </summary>
        /// <param name="canvas_width">Ширина полотна</param>
        /// <param name="canvas_height">Высота полотна</param>
        /// <param name="ViewKoefficient">Масштаб (1 = 100 %)</param>
        /// <returns></returns>
        double ScaleEntity(double canvas_width, double canvas_height);//, double ViewKoefficient)
        bool EmptySpatial { get; }
    }

    /// <summary>
    /// А также его имплементация
    /// </summary>
    public class Geometry : IGeometry
    {
        private long  fid;
        public long id
        {
            get { return this.fid; }
            set { this.fid = value; }
        }
        private int fState;
        public int State
        {
            get { return this.fState; }
            set { this.fState = value; }
        }
        private string fDefinition;

        public string Definition
        {
            get { return this.fDefinition; }
            set { this.fDefinition = value; }
        }

        private string fName;
        public string Name
        {
            get { return this.fName; }
            set { this.fName = value; }
        }

        private string fLayerHandle;
        public string LayerHandle
        {
            get { return this.fLayerHandle; }
            set { this.fLayerHandle = value; }
        }

        public string TypeName
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        public bool EmptySpatial
        {
            get
            {
                return true;
            }
        }

        public void Fraq(string Format)
        {
            // nothing to fraq :)
        }

        public void ShowasListItems(ListView LV, bool SetTag)
        {

        }

        public TPoint AverageCenter
        {
            get
            {
                return new TPoint(0, 0);
            }
        }

        public double ScaleEntity(double canvas_width, double canvas_height)
        {
            return 1;
        }

        /// <summary>
        /// Construct base Geometry object
        /// </summary>
        public Geometry()
        {
            this.id = Gen_id.newId;
            this.fLayerHandle = "F"; //Default. Autocad layer "0" has handle "F" by default
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

    public class TPoint : Geometry, IPoint, IGeometry, IEditableObject  //port из  FteoClasses.pas
    {
        //implementation of interface method Clone():
        public object Clone()
        {
            return new TPoint(this.x, this.y, this.Definition);
        }

        public int Borderid, fStatus;

        public int NumGeopoint;//, Order;
        string //fNumGeopointA,
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
        public TPoint()
        {
            this.newOrd = new Coordinate();
            this.oldOrd = new Coordinate();
        }

        public TPoint(double initx, double inity) : this()
        {
            this.x = initx;
            this.y = inity;
        }


        public TPoint(double initx, double inity, double initz)
            : this(initx, inity)
        {
            this.z = initz;
        }
        public TPoint(double initx, double inity, string definition) : this(initx, inity)
        {
            this.Definition = definition;
        }

        /// <summary>
        /// NumGeopointA - obsolete field. Instead use Definition
        /// </summary>
        [Obsolete]
        public string NumGeopointA
        {
            get { return this.Definition; }
            set { this.Definition = value; }
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
        public new TPoint AverageCenter
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Расчет масштаба Tpoint на поверхность (z.B: WPF canvas) 
        /// </summary>
        /// <param name="canvas_width">Ширина полотна</param>
        /// <param name="canvas_height">Высота полотна</param>
        /// <param name="ViewKoefficient">Масштаб (1 = 100 %)</param>
        /// <returns></returns>
        public double ScaleEntity(double canvas_width, double canvas_height)//, double ViewKoefficient)
        {
            //TODO need code. like Ring
            if (this.Empty) return -1;
            double scale = 0;
            double dx = 4; // размах по вертикали

            double dy = 4; // размах по горизонтали 
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

        public void ShowasListItems(ListView LV, bool SetTag)
        {
            if (Empty) return;
            string BName;
            LV.BeginUpdate();
            LV.Items.Clear();
            LV.Controls.Clear();
            LV.Columns[0].Text = "Имя";
            LV.Columns[1].Text = "x, м.";
            LV.Columns[2].Text = "y, м.";
            LV.Columns[3].Text = "z, м.";
            LV.Columns[4].Text = "Mt, м.";
            LV.Columns[5].Text = "Описание";
            LV.Columns[6].Text = "-";
            LV.View = View.Details;

            //LV.Tag = PList.Parent_Id;
            if (SetTag) LV.Tag = id;
            //ListViewItem res = null; ;
            BName = this.Pref + this.Definition + this.OrdIdent;
            ListViewItem LVi = new ListViewItem();
            LVi.Text = BName;
            //LVi.Tag = id;
            LVi.Tag = "TPoint." + id;
            LVi.SubItems.Add(x_s);
            LVi.SubItems.Add(y_s);
            LVi.SubItems.Add(z_s);
            LVi.SubItems.Add(Mt_s);
            LVi.SubItems.Add(Description);
            LVi.SubItems.Add(Code);
            if (Pref == "н")
                LVi.ForeColor = System.Drawing.Color.Red;
            else LVi.ForeColor = System.Drawing.Color.Black;
            if (Status == 6)
                LVi.ForeColor = System.Drawing.Color.Blue;
            LV.Items.Add(LVi);
            LV.EndUpdate();
            return;
        }

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
                else return "_._"; // ??
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

        public bool EmptySpatial
        {
            get
            {
                return Empty;
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
        public static TPoint operator +(TPoint A, TPoint B)
        {
            TPoint res = new TPoint();
            res.id = Gen_id.newId;
            res.Definition = A.Definition + B.Definition;
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
        public bool Equals(TPoint B)//Точки одинаковые ?
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

        TPoint UndoPoint;
        //     Начинает процедуру изменения объекта.
        public void BeginEdit()
        {
            UndoPoint = new TPoint(this.x, this.y);
            UndoPoint.Definition = this.Definition;
            UndoPoint.Mt = this.Mt;
        }

        //     Отменяет изменения с момента последнего System.ComponentModel.IEditableObject.BeginEdit
        //     вызова.
        public void CancelEdit()
        {
            this.x = UndoPoint.x;
            this.y = UndoPoint.y;
            this.Mt = UndoPoint.Mt;
            this.Definition = UndoPoint.Definition;
        }
        //
        // Сводка:
        //     Передает изменения с момента последнего System.ComponentModel.IEditableObject.BeginEdit
        //     или System.ComponentModel.IBindingList.AddNew вызов базового объекта.
        public void EndEdit()
        {

        }


    }


    #endregion

    /// <summary>
    /// Circle. Just circle
    /// </summary>
    public class TCircle : TPoint, IGeometry
    {
        /// <summary>
        /// Radius
        /// </summary>
        public double R;
        //public Point Center;

        public TCircle()
        {
            Name = "Окружность";
        }
        public TCircle(double x, double y, double radius) : this()
        {
            this.x = x;
            this.y = y;
            this.R = radius;
        }

        public TCircle(decimal x, decimal y, decimal radius) : this()
        {
            this.x = Convert.ToDouble(x);
            this.y = Convert.ToDouble(y);
            this.R = Convert.ToDouble(radius);
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
                if (!this.Empty)
                {
                    bounds.MaxX = this.x + this.R;
                    bounds.MinX = this.x - this.R;
                    bounds.MaxY = this.y + this.R;
                    bounds.MinY = this.y - this.R;
                }
                return bounds;
            }
        }

        public new void ShowasListItems(ListView LV, bool SetTag)
        {
            if (Empty) return;
            string BName;
            LV.BeginUpdate();
            LV.Items.Clear();
            LV.Controls.Clear();
            LV.Columns[0].Text = "Имя";
            LV.Columns[1].Text = "x, м.";
            LV.Columns[2].Text = "y, м.";
            LV.Columns[3].Text = "z, м.";
            LV.Columns[4].Text = "Mt, м.";
            LV.Columns[5].Text = "радиус";
            LV.Columns[6].Text = "-";
            LV.View = View.Details;


            //LV.Tag = PList.Parent_Id;
            if (SetTag) LV.Tag = id;
            BName = this.Pref + this.Definition + this.OrdIdent;
            ListViewItem LVi = new ListViewItem();
            LVi.Text = BName;
            LVi.Tag = id;
            LVi.SubItems.Add(x_s);
            LVi.SubItems.Add(y_s);
            LVi.SubItems.Add(z_s);
            LVi.SubItems.Add(Mt_s);
            LVi.SubItems.Add(R.ToString());
            if (Pref == "н")
                LVi.ForeColor = System.Drawing.Color.Red;
            else LVi.ForeColor = System.Drawing.Color.Black;
            if (Status == 6)
                LVi.ForeColor = System.Drawing.Color.Blue;
            LV.Items.Add(LVi);
            LV.EndUpdate();
            return;
        }

        /// <summary>
        /// Расчет масштаба Circle на поверхность (z.B: WPF canvas) 
        /// </summary>
        /// <param name="canvas_width">Ширина полотна</param>
        /// <param name="canvas_height">Высота полотна</param>
        /// <param name="ViewKoefficient">Масштаб (1 = 100 %)</param>
        /// <returns></returns>
        public new double ScaleEntity(double canvas_width, double canvas_height)//, double ViewKoefficient)
        {
            //TODO need code. like Ring
            if (this.Empty) return -1;
            double scale = 0;
            double dx = (this.Bounds.MaxX - this.Bounds.MinX); // размах по вертикали

            double dy = (this.Bounds.MaxY - this.Bounds.MinY); // размах по горизонтали 
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


    public interface IPointList : IGeometry
    {
        void ReversePoints();
        /// <summary>
        /// Reset old values of ordinates - like tnewContour
        /// </summary>
        void ResetOrdinates();
        void ResetStatus(string Prefix);
        void ExchangeOrdinates();

        
        /// <summary>
        /// Установка "ГКН" точек
        /// </summary>
        void DetectSpins(IPointList src);
        int ReorderPoints(int StartNumber);

        /// <summary>
        /// Close figure - append last point, if not present
        /// </summary>
        void Close();

        /// <summary>
        /// Set precission (delta) for ordinates
        /// </summary>
        /// <param name="mt"></param>
        void SetMt(double mt);
        bool RemovePoint(long PointID);
        TPoint GetPoint(long PointID);
    }

    /// <summary>
    /// Список точек на базе BindingList
    /// </summary>
    public class PointList : BindingList<TPoint>, IPointList
    {
        public const string TabDelimiter = "\t";  // tab
                                                  //public PointList Points;
        public long Parent_Id; // ид Участка или чего тоо ттам
        private long fid;
        private int fState;
        private string fDefinition;
        public long id
        {
            get { return this.fid; }
            set { this.fid = value; }
        }

        public int State
        {
            get { return this.fState; }
            set { this.fState = value; }
        }


        public string Definition
        {
            get { return this.fDefinition; }
            set { this.fDefinition = value; }
        }
 
        public void DetectSpins(IPointList src)
        {
            PointList Detectors = (PointList)src;

            foreach (TPoint Point in this)
                foreach (TPoint srcPoint in Detectors)
                {
                    if ((Point.x == srcPoint.x) &&
                        (Point.y == srcPoint.y)
                            )
                    {
                        Point.State = 6;//Rosreestr.dStatesv01_enum.Учтенный;
                        Point.Pref = "";
                    }
                }
        }

        public void ExchangeOrdinates()
        {
            double dx;
            for (int i = 0; i <= this.Count - 1; i++)
            {
                dx = this[i].x;
                this[i].x = this[i].y;
                this[i].y = dx;
            }
        }
    


        private string fLayerHandle;
        public string LayerHandle
        {
            get { return this.fLayerHandle; }
            set { this.fLayerHandle = value; }
        }
        private string fName;
        public string Name
        {
            get { return this.fName; }
            set { this.fName = value; }
        }
        public string TypeName
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        public void Close()
        {
            // nothing to
        }

        public void Fraq(string Format)
        {
            foreach (TPoint pt in this)
            {
                if (Double.TryParse(pt.x.ToString(Format), out double fraqtedX))
                {

                    pt.x = fraqtedX;
                    pt.oldX = fraqtedX;
                }

                if (Double.TryParse(pt.y.ToString(Format), out double fraqtedY))
                {
                    pt.y = fraqtedY;
                    pt.oldY = fraqtedY;
                }
            }
        }

        public bool RemovePoint(long PointID)
        {
            //linq:
            // case 1
            try
            {
                return this.Remove(this.Single<TPoint>(point => point.id == PointID));
            }
            catch (InvalidOperationException ex)
            {

                return false;
            }
        }

        public bool RemovePoint(TPoint pt)
        {
            return this.Remove(pt);
        }

        public TPoint GetPoint(long PointID)
        {
            foreach (TPoint pt in this)
            {
                if (pt.id == PointID)
                {
                    return pt;

                }
            }
            return null;
        }

        public int ReorderPoints(int StartIndex = 1)
        {
            foreach (TPoint pt in this)
            {
                pt.Definition = StartIndex++.ToString();
            }
            return StartIndex;
        }

        public void ResetOrdinates()
        {
            foreach (TPoint pt in this)
            {
                pt.oldX = Coordinate.NullOrdinate;
                pt.oldY = Coordinate.NullOrdinate;
            }
        }
        public void ResetStatus(string Prefix)
        {
            foreach (TPoint pt in this)
            {
                pt.Status = 0;
                pt.Pref = Prefix;
            }

        }

        public void SetMt(double mt)
        {
            foreach (TPoint pt in this)
                pt.Mt = mt;
        }

        public void ShowasListItems(ListView LV, bool SetTag)
        {
            if (Count == 0) return;
            string BName;
            LV.BeginUpdate();
            LV.Items.Clear();
            LV.Controls.Clear();
            LV.Columns[0].Text = "Имя";
            LV.Columns[1].Text = "x, м.";
            LV.Columns[2].Text = "y, м.";
            LV.Columns[3].Text = "z, м.";
            LV.Columns[4].Text = "Mt, м.";
            LV.Columns[5].Text = "Описание";
            LV.Columns[6].Text = "-";
            LV.View = View.Details;

            //LV.Tag = PList.Parent_Id;
            if (SetTag) LV.Tag = id;
            ListViewItem res = null; ;
            for (int i = 0; i <= Count - 1; i++)
            {
                BName = this[i].Pref + this[i].Definition + this[i].OrdIdent;
                ListViewItem LVi = new ListViewItem();
                LVi.Text = BName;
                //LVi.Tag = this[i].id;
                LVi.Tag = "TPoint." + this[i].id;
                LVi.SubItems.Add(this[i].x_s);
                LVi.SubItems.Add(this[i].y_s);
                LVi.SubItems.Add(this[i].z_s);
                LVi.SubItems.Add(this[i].Mt_s);
                LVi.SubItems.Add(this[i].Description);
                if (this[i].Pref == "н")
                    LVi.ForeColor = System.Drawing.Color.Red;
                else LVi.ForeColor = System.Drawing.Color.Black;
                if (this[i].Status == 6)
                    LVi.ForeColor = System.Drawing.Color.Blue;
                if (i == 0) res = LV.Items.Add(LVi);
                else
                    LV.Items.Add(LVi);
            }
            LV.EndUpdate();
            return;
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
            if (this.Bounds == null) return -1;
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

        public void ReversePoints()
        {
            PointList tmpList = new PointList();

            int count = this.Count;
            foreach (TPoint pt in this)
            {
                tmpList.AddPoint(this[count-- - 1]); //countdown  index
            }
            this.Clear();
            this.ImportObjects(tmpList);
        }

        /// <summary>
        /// Конструктор base Geometry object,
        /// Для сериализаций в Xml конструктор должен быть без параметров
        /// </summary>
        public PointList()
        {
            this.id = Gen_id.newId;
            this.fLayerHandle = "F"; //Default
        }


        public bool HasChangesBool
        {
            get
            {
                foreach (TPoint pt in this)
                {
                    if (pt.OrdIdent == "*")
                        return true;
                }
                return false;
            }
        }

        public int Index(int item_id)
        {
            int res = -1;
            foreach (TPoint pt in this)
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
        public void Move(int index, int newPosition)

        {
            TPoint Swap = this[newPosition]; // save object
            this.RemoveAt(newPosition);
            this.Insert(newPosition, this[index]);
            this.RemoveAt(index);
            this.Insert(index, Swap);

            //  this.[0].
        }



        public bool EmptySpatial
        {
            get
            {
                return (this.PointCount < 1);
            }
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
        /// Средние значения координат полигона
        /// </summary>
        public TPoint AverageCenter
        {
            get
            {
                double Xsumm = 0;
                double Ysumm = 0; int pointcnt = 0;
                for (int i = 0; i <= this.PointCount - 1; i++)
                {
                    if (!Double.IsNaN(this[i].x))
                    {
                        Xsumm += (this[i].x);
                        Ysumm += (this[i].y);
                        pointcnt++;
                    }
                }
                TPoint Respoint = new TPoint();
                Respoint.Definition = "Center";
                Respoint.x = Xsumm / pointcnt;
                Respoint.y = Ysumm / pointcnt;
                return Respoint;
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
                foreach (TPoint pt in this)
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

        public void ImportObjects(BindingList<TPoint> Points)
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

        public void AddPoint(TPoint point)
        {
            if (point == null) return;
            if (point.id < 1) point.id = Gen_id.newId;
            this.Add(point);

        }
        public TPoint AddPoint(string Name, double x_, double y_, string Descr)
        {
            TPoint Point = new TPoint();
            Point.x = x_;
            Point.y = y_;
            Point.Description = Descr;
            Point.Definition = Name;
            this.AddPoint(Point);
            return this[this.Count - 1];
        }

        public TPoint GetPointbyName(string ptName)
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
        public PointList FindSects(TPoint b1, TPoint b2)
        {
            PointList ResLayer = new PointList(); TPoint ResPoint;
            int PointCounter = 0; int NextPointIndex = 0;
            for (int i = 0; i <= this.PointCount - 1; i++)
            {
                //ResPoint  =nil;
                if (PointCounter == this.PointCount - 1)
                    NextPointIndex = 0;
                else NextPointIndex = PointCounter + 1; // чтобы не вылетать на последней границе
                ResPoint = Geodethics.Geodethic.FindIntersect(this[PointCounter++],
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

/*
        /// <summary>
        /// Проверка принадлежности точки фигуре. Трассировка лучом
        /// </summary>
        /// <param name="b1">Точка для проверки</param>
        /// <returns></returns>
        public bool Pointin(TPoint b1)
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
                yi = this[i].y;
                yj = this[j].y;
                xi = this[i].x;
                xj = this[j].x;

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
*/
/*
        private void PoininTest()
        {
            this.AddPoint(new TPoint(0, 0, "11"));
            this.AddPoint(new TPoint(1000, 0, "12"));
            this.AddPoint(new TPoint(1000, 1000, "13"));
            this.AddPoint(new TPoint(0, 1000, "14"));
            bool flag = Pointin(new TPoint(1001, 1001)); // out
            flag = Pointin(new TPoint(999.99, 999.99)); // in
            flag = Pointin(new TPoint(1000.001, 1000.0001)); // on border (on ring, on anus...)
        }
        */
/*
        /// <summary>
        /// Проверка принадлежности набора (списка etc) точек фигуре
        /// </summary>
        /// <param name="points"> ring of points</param>
        /// <returns></returns>
        public PointList Pointin(PointList points)
        {
            PointList res = new PointList();
            foreach (TPoint pt in points)
                if (this.Pointin(pt))
                    res.AddPoint(pt);
            if (res.PointCount > 0)
                return res;
            else return null; //зануляем свободные концы"-: Денисюк И..
        }

        */

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
                foreach (TPoint pt in points)

                    if ((this[i].x == pt.x)
                        &&
                        (this[i].y == pt.y))
                    {
                        ResLayer.AddPoint(pt);
                    }
            }
            return ResLayer;
        }

        public PointList CommonPoints(TPolygon poly)
        {
            PointList ResLayer = new PointList();
            //main ring:
            ResLayer.AppendPoints(this.CommonPoints((PointList)poly));
            //child rings:
            foreach (TRing child in poly.Childs)
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

    public class TPolyLine : PointList, IGeometry
    {
        public TPolyLine()
        {
            long check = this.id;
            Name = "Ломаная";
        }

        /// <summary>
        /// Length of polyline. Just summ of fragments
        /// </summary>
        public double Length
        {
            get
            {
                if (this.PointCount == 0) return -1;
                double Peryd = 0;
                double Test = 0;
                for (int i = 0; i <= this.Count - 2; i++)
                {
                    Test = Geodethics.Geodethic.lent(this[i].x, this[i].y, this[i + 1].x, this[i + 1].y);
                    if (!Double.IsNaN(Test))
                        Peryd += Test;
                }

                /* Closing figure:
				Test = Geodethic.lent(this[this.Count - 1].x, this[this.Count - 1].y, this[0].x, this[0].y);
				if (!Double.IsNaN(Test))
					Peryd += Test;
				*/
                return Peryd;
            }
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


    #region TRing  - closed polyline
    /// <summary>
    /// Ring - imperative closed polyline, other words - simple polygon.
    /// It have area, perymethr - total length of all border fragments
    /// </summary>
    public class TRing : TPolyLine
    {

        public string PerymethrFmt(string Format)
        {

            return this.Perymethr().ToString(Format);
        }

        public double Perymethr()
        {
            if (this.PointCount == 0) return -1;
            double Peryd = 0;
            double Test = 0;
            /*
            for (int i = 0; i <= this.Count - 2; i++)
            {
                Test = Geodethic.lent(this[i].x, this[i].y, this[i + 1].x, this[i + 1].y);
                if (!Double.IsNaN(Test))
                    Peryd += Test;
            }
			*/
            Peryd = Length;
            //Add last (closing) fragment.
            Test = Geodethics.Geodethic.lent(this[this.Count - 1].x, this[this.Count - 1].y, this[0].x, this[0].y);
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
                foreach (TPoint pt in this)
                {
                    if (pt.Empty) return false; // return on first empty vertex
                }

                return true; // all vertex valid (have coords)
            }

        }

        /// <summary>
        /// Расчет площади по формуле трапеции
        /// с учетом (c исключением) ликвидированных точек
        /// </summary>
        public double Area
        {
            get
            {
                PointList ring = new PointList();

                foreach (TPoint pt in this)

                {
                    //выбираем только точки с существующими ординатами .newOrd :
                    if (!Double.IsNaN(pt.x))
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
        public TPoint CentroidMassive
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
                TPoint Respoint = new TPoint();
                Respoint.Definition = "Centroid";
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
        public TPoint Centroid(double canvas_width, double canvas_height, double scale)
        {
            TPoint pt = new TPoint();
            double canvasX;
            double canvasY;

            canvasX = canvas_width / 2 - (this.AverageCenter.y - this.CentroidMassive.y) / scale;
            canvasY = canvas_height / 2 - (this.AverageCenter.x - this.CentroidMassive.x) / scale;
            pt.Definition = "CanvasCentroid";
            pt.x = canvasX;
            pt.y = canvasY;
            return pt;
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
        public new void Close()
        {
            if (!Closed)
            {
                this.Add(this.First());
            }
        }


        /// <summary>
        /// Rebuild point list to begin from specified point
        /// </summary>
        /// <param name="Point_id">First point id</param>
        public void MakeFirstPoint(long Point_id)
        {
            //get point index in this
            int StartPointIndex = this.IndexOf(GetPoint(Point_id));
            if (StartPointIndex == 0) return;// it already begin point
            PointList Reciever = new PointList();
            //TODO:
            //first part - from index to prev-last (Count-2) item
            //....
            for (int i = StartPointIndex; i <= this.Count - 2; i++)
            {
                if (!Reciever.Contains(this[i])) //prevent doublicates
                    Reciever.AddPoint(this[i]);
            }

            //second part - from first item to index, before them
            for (int i = 0; i <= StartPointIndex - 1; i++)
            {
                if (!Reciever.Contains(this[i])) //prevent doublicates
                    Reciever.AddPoint(this[i]);
            }

            //auto closing list
            Reciever.Add(GetPoint(Point_id)); // last point, closing etc

            this.Clear();
            foreach (TPoint pt in Reciever)
                this.Add(pt);
            Reciever.Clear();
        }



        /// <summary>
        /// Check if the point is within the polyline
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool InsideRing(TPoint pt)
        {
            int n = this.PointCount;// polygon.NumberOfVertices;
            double angle = 0;
            TPoint pt1 = new TPoint();
            TPoint pt2 = new TPoint();

            for (int i = 0; i < n; i++)
            {
                pt1.x = this[i].x - pt.x;
                pt1.y = this[i].y - pt.y;
                pt2.x = this[(i + 1) % n].x - pt.x;
                pt2.y = this[(i + 1) % n].y - pt.y;
                angle += Geodethics.Geodethic.Angle2D(pt1.x, pt1.y, pt2.x, pt2.y);
            }

            if (Math.Abs(angle) < Math.PI)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Check if the point is within the polyline.
        /// </summary>
        /// <param name="ring"></param>
        /// <returns>if value equal vertex count of test ring - full overlapping (incoming)</returns>
        public int InsideRing(TRing ring)
        {
            int retCount = 0;

            foreach(TPoint pt in ring)
            {
                if (this.InsideRing(pt))
                    ++retCount;
            }
            return retCount;
        }

   

  

    }
    #endregion

    #region Отрезки границ
    public class TBorder : Geometry
    {
        //private int fid;
        double fLength;
        //public string Definition;
        
        /// <summary>
        /// Contains names of point "from a" "to b"
        /// </summary>
        public string PointNames;
        public double Length
        {
            get { return this.fLength; }
        }

        /*
        public double id
        {
            get { return this.fid; }
        }

        */
        public TBorder(string definition, double length)
        {
            this.fLength = length;
            this.Definition = definition;
            //this.id = Gen_id.newId;
        }
    }

    public class BrdList : BindingList<TBorder>
    {

        public void AddItem(string definition, TPoint A, TPoint B)
        {
            TBorder NewBrd;
            if ((!Double.IsNaN(A.x)) && (!Double.IsNaN(A.y)) && (!Double.IsNaN(B.x)) && (!Double.IsNaN(B.y)))
            {
                NewBrd = new TBorder(definition, Geodethics.Geodethic.lent(A.x, A.y, B.x, B.y));
                NewBrd.PointNames = A.Definition + " - " + B.Definition;
                this.Add(NewBrd);

            }
        }

        public void AddItems(string Pref, PointList Points)
        {
            PointList NewPoints = new PointList();
            foreach (TPoint point in Points)
            {
                if ((!Double.IsNaN(point.x)) && (!Double.IsNaN(point.y)))
                {
                    NewPoints.AddPoint(point);
                }
            }

            for (int i = 0; i <= NewPoints.Count - 2; i++)
            {
                this.AddItem("-", NewPoints[i], NewPoints[i + 1]);
            }
            // this.AddItem("-", Points[Points.Count-1], Points.First());  //Closing border ??
        }

        public double Length
        {
            get
            {
                double res = 0;
                foreach (TBorder border in this)
                {
                    res += border.Length;
                }
                return res;
            }
        }
    }

    #endregion

    #region TPolygon Полигон c внутренними границами
    /// <summary>
    /// Polygon - closed ring with inner rings (childs)
    /// </summary>
    public class TPolygon : TRing, IPointList, IGeometry
    {

        public List<TRing> Childs;

        public TPolygon()
        {
            this.Childs = new List<TRing>();
            this.id = Gen_id.newId; //RND.Next(1, 10000);
            this.AreaValue = -1; // default, 'not specified'
            Name = "Полигон";
            //			this.LayerHandle = "FFFF"; // default
        }

        public TPolygon(int id) : this()
        {
            //  this.Childs = new List<TMyOutLayer>();
            this.id = id;
        }

        public TPolygon(string Def) : this()
        {
            //this.Childs = new List<TMyOutLayer>();
            //this.Layer_id = Gen_id.newId;
            this.Definition = Def;
        }

        public TPolygon(TPolyLine Ring) : this()
        {
            this.ImportRing(Ring);
            //	this.id = Ring.id;
        }

        public TPolygon(PointList SrcPoints): this()
        {
            foreach(TPoint pt in SrcPoints)
            {
                this.Add(pt);
            }
            this.Close();
        }

        public TPolygon(int id, string Def) : this(id)
        {
            //  this.Childs = new List<TMyOutLayer>();
            //  this.Layer_id = id;
            this.Definition = Def;
        }


        public new void ExchangeOrdinates()
        {
            base.ExchangeOrdinates();
            for (int i = 0; i <= this.Childs.Count - 1; i++)
                this.Childs[i].ExchangeOrdinates();
        }

        public new void ReversePoints()
        {
            base.ReversePoints();
            foreach (TRing child in this.Childs)
                child.ReversePoints();
        }

        public new int ReorderPoints(int StartIndex = 1)
        {
            base.ReorderPoints();
            foreach (TRing child in this.Childs)
                StartIndex += child.ReorderPoints(StartIndex);
            return StartIndex;
        }

        /// <summary>
        /// Reset old values of ordinates - like tnewContour
        /// </summary>
        public new void ResetOrdinates()
        {
            base.ResetOrdinates();
            foreach (TRing child in this.Childs)
                child.ResetOrdinates();
        }

        public new void SetMt(double mt)
        {
            base.SetMt(mt);
            foreach (TRing child in this.Childs)
                child.SetMt(mt);
        }

        void ResetStatus(string Prefix)
        {

        }

        public new bool RemovePoint(int PointID)
        {
            bool Flag = base.RemovePoint(PointID);

            foreach (TRing child in this.Childs)
                Flag = child.RemovePoint(PointID);
            return Flag;
        }

        public new void Fraq(string Format)
        {
            base.Fraq(Format);
            foreach (TRing child in this.Childs)
                child.Fraq(Format);
        }

        private void ShowListPoints(ListView LV, PointList points)
        {
            string BName;
            for (int i = 0; i <= points.Count - 1; i++)
            {
                BName = points[i].Pref + points[i].Definition + points[i].OrdIdent;
                ListViewItem LVi = new ListViewItem();
                LVi.Text = BName;
                LVi.Tag = "TPoint." + points[i].id;
                LVi.SubItems.Add(points[i].x_s);
                LVi.SubItems.Add(points[i].y_s);
                LVi.SubItems.Add(points[i].z_s);
                LVi.SubItems.Add(points[i].Mt_s);
                LVi.SubItems.Add(points[i].Description);

                LVi.ForeColor = TMyColors.StatusToColor(points[i].State);
                
				if (points[i].Pref == "н")
					LVi.ForeColor = System.Drawing.Color.Red;
				else LVi.ForeColor = System.Drawing.Color.Black;
				if (points[i].Status == 6)
					LVi.ForeColor = System.Drawing.Color.Blue;
				
                //if (i == 0) res = LV.Items.Add(LVi);
                //else
                LV.Items.Add(LVi);
            }
        }

        public new void ShowasListItems(ListView LV, bool SetTag)
        {
            if (PointCount == 0) return;
            LV.BeginUpdate();
            LV.Items.Clear();
            LV.Controls.Clear();
            LV.View = View.Details;
            LV.Columns[0].Text = "Имя";
            LV.Columns[1].Text = "x, м.";
            LV.Columns[2].Text = "y, м.";
            LV.Columns[3].Text = "z, м.";
            LV.Columns[4].Text = "Mt, м.";
            LV.Columns[5].Text = "Описание";
            LV.Columns[6].Text = "-";
            if (SetTag) LV.Tag = id;

            ShowListPoints(LV, (PointList)this);

            for (int ic = 0; ic <= this.Childs.Count - 1; ic++)
            {  //Пустая строчка - разделитель
                ListViewItem LViEmpty_ch = new ListViewItem();
                LViEmpty_ch.Text = "";
                LV.Items.Add(LViEmpty_ch);
                ShowListPoints(LV, this.Childs[ic]);
            }

            ListViewItem LViEmpty = new ListViewItem();
            LViEmpty.Text = "";
            LV.Items.Add(LViEmpty);
            LV.EndUpdate();
        }


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

        /// <summary>
        /// Set points of ring - main border by importing 
        /// </summary>
        /// <param name="Ring"></param>
        public void ImportRing(TPolyLine Ring)
        {
            if (Ring == null) return;
            this.Clear();
            this.Childs.Clear();
            //			this.Definition = Ring.Definition;
            for (int i = 0; i <= Ring.PointCount - 1; i++)
                this.AddPoint(Ring[i]);
        }


        public void ImportPolygon(TPolygon Poly)
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

        public PointList FindCommonPoints(TPolygon ES)
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
            foreach (TPoint pt in ES)
            {
                if (this.InsideRing(pt))
                {
                    bool inchildFlag = false;
                    //childs .... 
                    //для внутренних границ точка как раз не должна быть внутри
                    //если она принадлежит "дырявому" полигону, то фактом её принадлежности будет 
                    // принадлежность внешнему и отсутствие принадлежности всем детям
                    foreach (TRing child in this.Childs)
                    {
                        inchildFlag = child.InsideRing(pt);
                    }
                    if (!inchildFlag) // если не попало ни в одну дырку
                        res.AddPoint(pt);
                }

            }

            if (res.PointCount > 0) return res;
            else
                return null;
        }

        public PointList PointsIn(TPolygon ES)
        {
            PointList res = new PointList();
            res.AppendPoints(this.PointsIn(ES));
            //for each child in checked ES
            foreach (TRing child in ES.Childs)
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

            foreach (TRing child in this.Childs)
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
        public PointList FindSect(TPolygon ES)
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
        /// <param name="ES">Полигон типа TPolygon</param>
        /// <returns></returns>
        public PointList FindClip(TPolygon ES)
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
        /// Установка "ГКН" точек полигона
        /// </summary>
        public new void DetectSpins(IPointList src)
        {
            base.DetectSpins(src);
            foreach (PointList child in this.Childs)
            {
                child.DetectSpins(src);
            }
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

        /// <summary>
        /// Add an empty child
        /// </summary>
        /// <returns></returns>
        public TRing AddChild()
        {
            this.Childs.Add(new TRing());
            this.Childs[this.Childs.Count - 1].id = Gen_id.newId;
            return this.Childs[this.Childs.Count - 1];
        }

        /// <summary>
        /// Add a child ring with validation of geometry
        /// </summary>
        /// <param name="child">Ring of points (closed polyline)</param>
        public void AddChild(TRing child)
        {
            if (child == null) return;

            var test = this.HasCommonPoints(child);
            var inn = this.PointsIn(child);

            if ((child.Count > 1) &&              //
                (child.Closed) &&              //if child is true ring
                (!this.HasCommonPoints(child)) &&
               (this.PointsIn(child) != null)) //if child in parent figure TODO !!! Pointsin got fake results
            {
                this.Childs.Add(child);
                this.Childs[this.Childs.Count - 1].id = Gen_id.newId;
                //return this.Childs[this.Childs.Count - 1];
            }
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

        public TRing GetEs(int Layer_id)
        {
            if (this.id == Layer_id) return this;

            for (int i = 0; i <= this.Childs.Count - 1; i++)
            {
                if (this.Childs[i].id == Layer_id) return this.Childs[i];
            }
            return null;
        }
    }
    #endregion


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
        public int Max;
        public byte[] Data;
    }

    #region TPolygonCollection Коллекция Полигонов
    public delegate void ESCheckingHandler(object sender, ESCheckingEventArgs e);

    public class TPolygonCollection : List<TPolygon>
    {
        public event ESCheckingHandler OnChecking;
        //public MifOptions MIF_Options; // настройки для полигонов MIF
        private int totalItems;
        private int fParent_id;
        public int id;
        //public List<TPolygon> Items;

        public TPolygonCollection()  /// Конструктор
        {
            //this.Items = new List<TPolygon>();
            this.id = Gen_id.newId;
            //this.MIF_Options = new MifOptions();
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
        /*
        public int TotalPointCount
        {
            get
            {
                int res = 0;
                foreach (TPolygon item in this)
                {
                    res += item.PolygonPointCount;
                }
                return res;
            }
        }
		*/
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

        public TPolygon AddPolygon(object poly_)
        {
            if (poly_ == null) return null;
            if ((poly_.GetType().ToString().Equals("netFteo.Spatial.TPolygon")) &&
                (((TPolygon)poly_).PointCount > 0))
            {
                this.Add((TPolygon)poly_);
                return (TPolygon)poly_;
            }

            if ((poly_.GetType().ToString().Equals("netFteo.Spatial.TRing")) &&
         (((TRing)poly_).PointCount > 0))
            {
                TPolygon Vpoly = new TPolygon();
                Vpoly.AppendPoints((TRing)poly_);
                Vpoly.Definition = ((TRing)poly_).Definition;
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

        public TPolygon GetEs(int Layer_id)
        {
            for (int i = 0; i <= this.Count - 1; i++)
            {
                if (this[i].id == Layer_id)
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
        public PointList CheckES(TPolygon ES)
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
        public PointList CheckCommon(TPolygon ES)
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
                aslist.Add(this[i].Definition + "\t" +
                    this[i].AreaSpatialFmt("0.00") + "\t" +
                                                       (this[i].AreaValue > 0 ? this[i].AreaValue.ToString() : "-") + "\t" +
                                                       ((decimal)this[i].AreaSpatial - this[i].AreaValue).ToString("0.00") + "\t" +
                                                       (this[i].AreaInaccuracy != "" ? this[i].AreaInaccuracy : "-") + "\t" +
                                                       this[i].Has_Changes
                    );

            return aslist;

        }

    }
    #endregion


    /*
    public class OMSPoints : PointList
    {

    }

    */

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




    public delegate Object ESDelegate();

    
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
        /// <summary>
        /// MySQL retrieves and displays DATE values in 'YYYY-MM-DD' format
        /// </summary>
        public string Doc_Date;
    }


    /// <summary>
    /// класс Файл. Представляет xml-файл всех видов
    /// </summary>
    public class TFile : TDocument
    {

        public long id;
        public Rosreestr.dFileTypes Type
        {
            get
            {
                return Rosreestr.NameSpaces.NStoFileType(xmlns);
            }
        }
        public string AccessCode;
        public string FileName;
        public string RequestNum;
        public string GUID;
        public string RootName;
        public string xmlns;
        public long xmlSize_SQL; // size of body ( prepared by server)

        //private System.Xml.XmlDocument fXML_file_body;
        private byte[] fFile_BLOB;

        /// <summary>
        /// BLOB file body as binary array - byte[]
        /// </summary>
        public byte[] File_BLOB
        {
            set
            {
                this.fFile_BLOB = value;
            }

            get
            {
                return this.fFile_BLOB;
            }
        }

        /// <summary>
        /// Create and return File_Body as memory stream 
        /// </summary>
        public MemoryStream File_BLOB_Stream
        {
            get
            {
                MemoryStream ms = new MemoryStream(this.File_BLOB);
                return ms;
            }
        }

        /*
        /// <summary>
        /// File body as XmlDocument. Loaded when File_Blob setuped
        /// </summary>
        System.Xml.XmlDocument XML_file_body
        {
            get
            {
                /*
                if (File_BLOB != null)
                {
                    System.Xml.XmlDocument resDoc = new System.Xml.XmlDocument();
                    using (MemoryStream ms = new MemoryStream(File_BLOB))
                    {
                        resDoc.Load(ms);
                        ms.Close();
                    }
                    return resDoc;
                }
                else return null;
                */
        /*
        return this.fXML_file_body;
    }
}

*/

        /*
        /// <summary>
    /// Read file body as binary array - BLOB from stream
    /// </summary>
    /// <param name="filestreambody"></param>
    public bool ReadFileBodyStream(MemoryStream filestreambody)
    {
        if (filestreambody != null)
        {
            filestreambody.Seek(0, 0);
            byte[] tmp = filestreambody.ToArray();
            if (FileBody_Stream != null) FileBody_Stream.Dispose();
                FileBody_Stream = new MemoryStream(tmp);
              filestreambody.Dispose();
            return true;
        }
        return false;
    }

*/
        public bool ReadFileBody(byte[] buffer)
        {
            if (buffer.Length > 0)
            {
                this.File_BLOB = buffer;
                return true;
            }
            return false;
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

        /// <summary>
        /// Read file body as binary array - BLOB from stream
        /// </summary>
        /// <param name="file_id"></param>
        /// <param name="filebody"></param>
        /// <returns></returns>
        public TFile ReadFileBody(long file_id, byte[] filebody)
        {
            foreach (TFile file in this)
            {
                if (file.id == file_id)
                {
                    file.ReadFileBody(filebody);
                    //filebody.Close();
                    return file;
                }
            }
            return null;
        }

        public bool BodyEmpty(long file_id)
        {
            foreach (TFile file in this)
            {
                if (file.id == file_id)
                {

                   // if ((file.FileBody_Stream == null) || (file.FileBody_Stream.Length == 0))
                   if ((file.File_BLOB == null) || (file.File_BLOB.Length == 0))
                        return true;
                }
            }
            return false;
        }

        public Rosreestr.dFileTypes GetFileType(long file_id)
        {
            foreach (TFile file in this)
            {
                if (file.id == file_id)
                {
                    return file.Type;
                }
            }
            return Rosreestr.dFileTypes.Undefined;
        }


        /// <summary>
        /// Select file body from list as XmlDocument by id
        /// </summary>
        /// <param name="id">File id</param>
        /// <returns>XmlDocument</returns>
        /*
        System.Xml.XmlDocument XML_file_body(long id)
        {
            foreach (TFile file in this)
            {
                if (file.id == id)
                {
                    return file.XML_file_body;
                }
            }
            return null;
        }

        */

        public Stream File_stream(long id)
        {
            foreach (TFile file in this)
            {
                if (file.id == id)
                {
                    MemoryStream ms = new MemoryStream(file.File_BLOB);
                    return ms;
                }
            }
            return null;
        }

        public String GetFileName(long file_id)
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

        public long GetFileSize(long file_id)
        {
            foreach (TFile file in this)
            {
                if (file.id == file_id)
                {
                    return file.xmlSize_SQL;
                }
            }
            return -1; ;
        }


        public bool FileBodyExist(int file_id)
        {
            foreach (TFile file in this)
            {
                if (file.id == file_id)
                {
                    return true;
                }
            }
            return false;
        }

        public TFile GetFile(long file_id)
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
        long Block_id;
        public TFileHistory(long block_id)
        {
            this.Block_id = block_id;
        }
    }




    #region Spatial classes

    public class TLayer : Geometry
    {
        public long Parent_id; //? what you think
        public TLayer()
        {
            this.Name = "0";
        }

        public TLayer(long Parent_id) : this()
        {
            this.Parent_id = Parent_id;
        }
    }

    /// <summary>
    /// Getero spatial data collection -lines, polygons, points, circles 
    /// </summary>
    public class TEntitySpatial : List<IGeometry>, IGeometry, IEnumerable
    {
        private long fid;
        private string fDefinition;
        private int totalItems;
        public event ESCheckingHandler OnChecking;
        public long id
        {
            get { return this.fid; }
            set { this.fid = value; }
        }

        private int fState;
        public int State
        {
            get { return this.fState; }
            set { this.fState = value; }
        }

        private string fLayerHandle;
        public string LayerHandle
        {
            get { return this.fLayerHandle; }
            set { this.fLayerHandle = value; }
        }
        private string fName;
        public string Name
        {
            get { return this.fName; }
            set { this.fName = value; }
        }
        public string Definition
        {
            get { return this.fDefinition; }
            set { this.fDefinition = value; }
        }

        public string TypeName
        {
            get
            {
                return this.GetType().ToString();
            }
        }
        public List<string> LoadExceptions; // Log of load
        public List<TLayer> Layers;

        public TEntitySpatial()
        {
            this.id = Gen_id.newId;
            this.Definition = "Границы";
            Name = "Границы";
            this.Layers = new List<TLayer>();
            this.Layers.Add(new TLayer(this.id)); // default layer 0, handle = FFFF
            this.LoadExceptions = new List<string>();
        }

        public TEntitySpatial(string Definition): this()
        {
            this.Definition = Definition;
        }

        public TPolygon AddPolygon(object poly_)
        {

            if (poly_ == null) return null;
            if (poly_.GetType().ToString().Equals("netFteo.Spatial.TPolygon") &&
                (((TPolygon)poly_).PointCount > 0))
            {

                this.Add((TPolygon)poly_);
                return (TPolygon)poly_;
            }

            if ((poly_.GetType().ToString().Equals("netFteo.Spatial.TRing")) &&
         (((TRing)poly_).PointCount > 0))
            {
                TPolygon Vpoly = new TPolygon();
                Vpoly.AppendPoints((TRing)poly_);
                Vpoly.Definition = ((TRing)poly_).Definition;
                this.AddPolygon(Vpoly);
                return Vpoly;
            }

            //"netFteo.Spatial.TPolyLine" ????
            return null;
        }

        /// <summary>
        /// Most used instead Add, due check fake(empty) Features collection
        /// </summary>
        /// <param name="ES">Source  spatial collection</param>
        /// <returns></returns>
        public bool AddES(TEntitySpatial ES)
        {
            if (ES.Count > 0)
            {
                this.Add(ES);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Insert each feature as item
        /// </summary>
        /// <param name="Features">Source  spatial collection</param>
        /// <returns></returns>
        public bool AddFeatures(TEntitySpatial Features)
        {
            if (Features == null) return false;
            if (Features.Count > 0)
            {
                foreach (IGeometry feature in Features)
                {
                    this.Add(feature);
                }
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Summ of polygons areas
        /// </summary>
        public double AreaSpatial
        {
            get
            {
                double AreaC = 0;
                foreach (IGeometry feature in this)
                {
                    if (feature.TypeName == "netFteo.Spatial.TPolygon")

                        AreaC += ((TPolygon)feature).AreaSpatial;
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
            if (AreaSpatial > 0)
            {
                if (ReturnCount) return AreaSpatial.ToString(format) + "  (1.." + this.Count.ToString() + ") ";
                else return AreaSpatial.ToString(format);
            }
            else return "";
        }

        public bool EmptySpatial
        {
            get
            {
                
                foreach(IGeometry feature in this)
                {
                    if (feature.EmptySpatial)
                        return true;
                }
                return false;
            }
        }
        
        public bool FeatureExists(int Item_id)
        {
            foreach (IGeometry feature in this)
            {
                if (feature.id == Item_id)
                    return true;

            }
            return false;
        }

        /// <summary>
        /// Select feature by id
        /// </summary>
        /// <param name="id">id for desired feature</param>
        /// <returns></returns>
        public Object GetFeature(int Item_id)
        {

            //From OKS
            foreach (IGeometry feature in this)
            {
                if (feature.id == Item_id)
                    return feature;

            }
            return null;
        }

        /// <summary>
        /// Select features by Layer
        /// </summary>
        /// <param name="LayerHandle">Handle of desired layer</param>
        /// <returns></returns>
        public TEntitySpatial GetFeatures(string LayerHandle)
        {
            TEntitySpatial res = new TEntitySpatial();
            foreach (IGeometry feature in this)
            {
                if (feature.LayerHandle == LayerHandle)
                {
                    res.Add(feature);
                }
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
        /// Проверка на пересечения с полигоном ES
        /// </summary>
        /// <param name="ESs">Полигон для проверки</param>
        /// <returns></returns>
        public PointList CheckES(TPolygon ES)
        {
            PointList res = new PointList();
            PointList PlREs;
            totalItems++;// += ES.PolygonPointCount;

            foreach (IGeometry feature in this)
            {
                if (feature.TypeName == "netFteo.Spatial.TPolygon")
                {
                    totalItems++;
                    PlREs = ((TPolygon)feature).FindClip(ES);
                    EsChekerProc(((TPolygon)feature).Definition, totalItems, null);
                    if (PlREs != null)
                    {
                        res.AppendPoints(PlREs);
                    }
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
        /// Установка "ГКН" точек
        /// </summary>
        public void DetectSpins(TEntitySpatial src)
        {
            foreach (IGeometry feature in this)
            {
                if (feature.TypeName == "netFteo.Spatial.TPolygon")
                {
                    TPolygon Poly = (TPolygon)feature;
                    foreach (IGeometry srcFeature in src)
                    {
                        if (srcFeature.TypeName == "netFteo.Spatial.TPolygon")
                        {
                            Poly.DetectSpins((PointList)srcFeature);
                        }
                    }

                }
            }
        }


        /// <summary>
        /// Remove parent number part in features numbers
        /// </summary>
        /// <param name="ParentCN"></param>
        public void RemoveParentCN(string ParentCN)
        {
            if (ParentCN == null) return;
            foreach (IGeometry poly in this)
            {
                if (poly.Definition != null &&
                    poly.Definition.Contains(ParentCN))
                    if (poly.Definition.Substring(0, ParentCN.Length) == ParentCN)
                        poly.Definition = poly.Definition.Substring(ParentCN.Length);
            }
        }

        /// <summary>
        /// Remove Elevation, Codes, descriptions for polygons
        /// </summary>
        public void RemovePointDescriptions()
        {
            foreach (IGeometry feature in this)
            {
                
                if (feature.TypeName == "netFteo.Spatial.TPolygon")
                {
                     TPolygon poly = (TPolygon)feature;
                    foreach (TPoint pt in poly)
                    {
                        pt.Description = "";
                        pt.Code = "";
                        pt.z = Coordinate.NullOrdinate;
                    }
                }
            }
        }

        public void Fraq(string Format)
        {
            foreach (IGeometry feature in this)
            {
                feature.Fraq(Format);
            }

        }

        public int FeaturesCount(string TypeName)
        {
            int res = 0;
            if (TypeName == "*")
            {
                return this.Count();
            }

            foreach (IGeometry feature in this)
            {
                if (feature.TypeName == TypeName)
                {
                    res++;
                }
            }
            return res;
        }

        /// <summary>
        /// Summ of all polylines length
        /// </summary>
        /// <param name="TypeName"></param>
        /// <returns></returns>
        public double PolylinesLength
        {
            get
            {
                double res = 0;
                foreach (IGeometry feature in this)
                {
                    if (feature.TypeName == "netFteo.Spatial.TPolyLine")
                    {
                        res += ((TPolyLine)feature).Length;
                    }
                }
                return res;
            }
        }

        /// <summary>
        /// Summ of all poly areas
        /// </summary>
        /// <param name="TypeName"></param>
        /// <returns></returns>
        public double PolyArea 
        {
            get
            {
                double res = 0; // default value
                foreach (IGeometry feature in this)
                {
                    if (feature.TypeName == "netFteo.Spatial.TPolygon")
                    {
                        res += ((TPolygon)feature).AreaSpatial;

                    }
                }
                return res;
            }
        }

        /// <summary>
        ///Count of all polygons
        /// </summary>
        /// <param name="TypeName"></param>
        /// <returns></returns>
        public int PolyCount
        {
            get
            {
                int res = 0; // default empty value
                foreach (IGeometry feature in this)
                {
                    if (feature.TypeName == "netFteo.Spatial.TPolygon")
                    {
                        res++;
                    }
                }
                return res;
            }
        }

        public string PolyAreaString
        {
            get
            {
                double res = -1; // default empty value
                int resCount = 0;
                foreach (IGeometry feature in this)
                {
                    if (feature.TypeName == "netFteo.Spatial.TPolygon")
                    {
                        res += ((TPolygon)feature).AreaSpatial;
                        resCount++;
                    }
                }
                return res + " [" + resCount.ToString() + "]";
            }
        }

        public void ShowasListItems(ListView LV, bool SetTag)
        {
            if (Count == 0) return;
            LV.BeginUpdate();
            LV.Items.Clear();

            //LV.Tag = PList.Parent_Id;
            if (SetTag) LV.Tag = id;

#if (DEBUG)
            LV.Columns[0].Text = "Name";
            LV.Columns[1].Text = "Type";
            LV.Columns[2].Text = "id";
            LV.Columns[3].Text = "layer handle";
            LV.Columns[4].Text = "-";

            foreach (TLayer Layer in this.Layers)
            {

                ListViewItem LVi = new ListViewItem();
                LVi.Text = Layer.Name;
                LVi.Tag = "Layer." + Layer.id;
                LVi.SubItems.Add(Layer.TypeName);
                LVi.SubItems.Add(Layer.id.ToString());
                LVi.SubItems.Add(Layer.LayerHandle);
                LVi.SubItems.Add("-");
                LVi.SubItems.Add("-");
                LV.Items.Add(LVi);
            }
            /*
			foreach (IGeometry feature in this)
			{
				ListViewItem LVi = new ListViewItem();
				LVi.Text = feature.Definition;
				LVi.Tag = feature.id;
				LVi.SubItems.Add(feature.Name);
				LVi.SubItems.Add(feature.id.ToString());
				LVi.SubItems.Add(feature.LayerHandle);
				LV.Items.Add(LVi);
			}
			*/
#endif
#if (!DEBUG)
			LV.Columns[0].Text = "Обозн.";
			LV.Columns[1].Text = "Имя";
			LV.Columns[2].Text = "-";
			LV.Columns[3].Text = "-";
			LV.Columns[4].Text = "-";
#endif

            foreach (IGeometry feature in this)
            {
                ListViewItem LVi = new ListViewItem();
                LVi.Text = feature.Definition;
                LVi.Tag = feature.id;
                if (feature is IPoint)
                {
                    TPoint pt = (TPoint)feature;
                    LVi.Tag = "TPoint." + pt.id;
                    LVi.SubItems.Add(pt.x_s);
                    LVi.SubItems.Add(pt.y_s);
                    LVi.SubItems.Add(pt.z_s);
                    LVi.SubItems.Add(pt.Mt_s);
                    LVi.SubItems.Add(pt.Code);
                }
                else
                LVi.SubItems.Add(feature.Name);

                if (feature.TypeName == "netFteo.Spatial.TPolygon")
                {
                    LVi.Tag = "Polygon." + feature.id;
                    LV.Columns[2].Text = "Площадь";
                    LV.Columns[3].Text = "Площ. гр.";
                    LV.Columns[4].Text = "Δ";
                    if (((TPolygon)feature).AreaValue != -1)
                        LVi.SubItems.Add(((TPolygon)feature).AreaValue.ToString());
                    else LVi.SubItems.Add("-");
                    LVi.SubItems.Add(((TPolygon)feature).AreaSpatialFmt("0.00"));
                    LVi.SubItems.Add(((TPolygon)feature).AreaInaccuracy);
                }

                if (feature.TypeName == "netFteo.Spatial.TPolyLine")
                {
                    //TODO:
                }
                LV.Items.Add(LVi);
            }

            if (FeaturesCount("netFteo.Spatial.TPolyLine") > 0)
            {
                ListViewItem LVTotal = new ListViewItem();
                LVTotal.Text = "Полилиний";
                //LVTotal.Tag = feature.id;
                LVTotal.SubItems.Add(FeaturesCount("netFteo.Spatial.TPolyLine").ToString());
                LVTotal.SubItems.Add("Общая длина");
                LVTotal.SubItems.Add(PolylinesLength.ToString("0.00"));
                LV.Items.Add(LVTotal);
            }
            LV.EndUpdate();
        }

        public double ScaleEntity(double canvas_width, double canvas_height)
        {
            if (this.Bounds == null) return -1;
            double scale = 0;
            double dx = (this.Bounds.MaxX - this.Bounds.MinX); // размах по вертикали
            double dy = (this.Bounds.MaxY - this.Bounds.MinY); // размах по горизонтали 


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

        public int ReorderPoints(int Startindex = 1)
        {
            foreach (IPointList feature in this)
            {
                Startindex += feature.ReorderPoints(Startindex);
            }
            return Startindex;
        }

        public void SetMt(double mt)
        {
            foreach (IPointList feature in this)
            {
                feature.SetMt(mt);
            }
        }

        public PointList AsPointList
        {
            get
            {
                PointList res = new PointList();

                foreach (IGeometry feature in this)
                {
                    if (feature.TypeName == "netFteo.Spatial.TPolygon")
                        res.AppendPoints(((TPolygon)feature).AsPointList());

                    if (feature.TypeName == "netFteo.Spatial.PointList")
                        res.AppendPoints(((PointList)feature));

                    if (feature.TypeName == "netFteo.Spatial.OMSPoints")
                        res.AppendPoints(((PointList)feature));
                }


                return res;
            }
        }

        /// <summary>
        /// TODO: wrong util code
        /// </summary>
        /// 
        public TMyRect Bounds
        {
            get
            {
                TMyRect Result = new TMyRect();

                // init bounds by first polygon/polyline
                foreach (IGeometry feature in this)
                {
                    if (!feature.EmptySpatial)
                    {
                        if (feature.TypeName == "netFteo.Spatial.TPolygon")
                        {
                            TPolygon poly = (TPolygon)feature;
                            Result.MinX = poly.Bounds.MinX;
                            Result.MinY = poly.Bounds.MinY;
                            Result.MaxX = poly.Bounds.MaxX;
                            Result.MaxY = poly.Bounds.MaxY;
                            goto SCAN;
                        }

                        if (feature.TypeName == "netFteo.Spatial.TPolyLine")
                        {
                            TPolyLine poly = (TPolyLine)feature;
                            Result.MinX = poly.Bounds.MinX;
                            Result.MinY = poly.Bounds.MinY;
                            Result.MaxX = poly.Bounds.MaxX;
                            Result.MaxY = poly.Bounds.MaxY;
                            goto SCAN;
                        }

                        if (feature.TypeName == "netFteo.Spatial.TCircle")
                        {
                            TCircle poly = (TCircle)feature;
                            Result.MinX = poly.Bounds.MinX;
                            Result.MinY = poly.Bounds.MinY;
                            Result.MaxX = poly.Bounds.MaxX;
                            Result.MaxY = poly.Bounds.MaxY;
                            goto SCAN;
                        }
                    }
                }

            SCAN: foreach (IGeometry feature in this)
                {
                    if (!feature.EmptySpatial)
                    {
                        if (feature.TypeName == "netFteo.Spatial.TPolygon")
                        {
                            TPolygon poly = (TPolygon)feature;
                            if (poly.Bounds != null)
                            {
                                if (poly.Bounds.MinX < Result.MinX) Result.MinX = poly.Bounds.MinX;
                                if (poly.Bounds.MinY < Result.MinY) Result.MinY = poly.Bounds.MinY;
                                if (poly.Bounds.MaxX > Result.MaxX) Result.MaxX = poly.Bounds.MaxX;
                                if (poly.Bounds.MaxY > Result.MaxY) Result.MaxY = poly.Bounds.MaxY;
                            }
                        }

                        if (feature.TypeName == "netFteo.Spatial.TPolyLine")
                        {
                            TPolyLine poly = (TPolyLine)feature;
                            {
                                if (poly.Bounds != null)
                                {
                                    if (poly.Bounds.MinX < Result.MinX) Result.MinX = poly.Bounds.MinX;
                                    if (poly.Bounds.MinY < Result.MinY) Result.MinY = poly.Bounds.MinY;
                                    if (poly.Bounds.MaxX > Result.MaxX) Result.MaxX = poly.Bounds.MaxX;
                                    if (poly.Bounds.MaxY > Result.MaxY) Result.MaxY = poly.Bounds.MaxY;
                                }
                            }
                        }

                        if (feature.TypeName == "netFteo.Spatial.TCircle")
                        {
                            TCircle Circle = (TCircle)feature;
                            {
                                if (Circle.Bounds != null)
                                {
                                    if (Circle.Bounds.MinX < Result.MinX) Result.MinX = Circle.Bounds.MinX;
                                    if (Circle.Bounds.MinY < Result.MinY) Result.MinY = Circle.Bounds.MinY;
                                    if (Circle.Bounds.MaxX > Result.MaxX) Result.MaxX = Circle.Bounds.MaxX;
                                    if (Circle.Bounds.MaxY > Result.MaxY) Result.MaxY = Circle.Bounds.MaxY;
                                }
                            }
                        }
                    }
                }
                return Result;
            }
        }

        /// <summary>
        /// Medium value of ordinates all features in this
        /// </summary>
        /// 
        public TPoint AverageCenter
        {
            get
            {
                double Ord_X_Summ = 0;
                double Ord_Y_Summ = 0;
                int CenterCount = 0;
                foreach (IGeometry feature in this)
                {
                    Ord_X_Summ += feature.AverageCenter.x;
                    Ord_Y_Summ += feature.AverageCenter.y;
                    CenterCount++;
                }

                return new TPoint(Ord_X_Summ / CenterCount, Ord_Y_Summ / CenterCount);
            }
        }

        public void Close()
        {
            // nothing to do ?
        }


        /// <summary>
        /// Parse spatial, detect parent and childs
        /// and create complete polygons with child rings
        /// </summary>
        public bool ParseSpatial()
        {
            if (State == 200) return false;// already parsed
            List<TRing> AllRings = new List<TRing>();
            int OuterRingCount = 0;
            //Collect all present rings:
            foreach (IGeometry item in this)
            {

                AllRings.Add((TRing)item);
            }

            Clear(); //reset collection
            this.State = 0;//clear
            //now scan collection:
            foreach (TRing OuterCandidate in AllRings)
            {
                if (OuterCandidate.State == 0)
                {
                    TPolygon ParentPolygon = new TPolygon();
                    ParentPolygon.Definition = OuterCandidate.Definition;
                    ParentPolygon.ImportRing(OuterCandidate); //First ring (possible outer)

                    // Scan source list for incoming rings (that covered by first/current)
                    foreach (TRing ring in AllRings)
                    {
                        if (ParentPolygon.InsideRing(ring) == ring.Count)

                        {
                            TRing InLayer = ParentPolygon.AddChild();
                            {
                                InLayer.AppendPoints(ring);
                                ring.State = 04;// set "already linked" flag
                            }
                        }
                    }
                    //insert only not child inserted instead parent:
                    if (ParentPolygon.State == 0)
                    {
                        ++OuterRingCount;
                        if (OuterRingCount > 0)  //add () marker for multicontours
                            ParentPolygon.Definition = "(" + (OuterRingCount).ToString() + ")" + " [1.." + ParentPolygon.PolygonPointCount.ToString()+"]";
                        ParentPolygon.State = 200;//complete
                        this.AddPolygon(ParentPolygon);
                    }
                }
            }
            if (OuterRingCount > 0)
            {
                State = 200;// ok, parsed
                return true;
            }
            else return false;//nothing happend
        }
    }

    #endregion




}


