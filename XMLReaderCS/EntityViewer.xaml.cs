using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using netFteo.Spatial;
using netFteo.Drawing;

namespace XMLReaderCS
{
    /// <summary>
    /// Логика взаимодействия для EntityViewer.xaml
    /// </summary>
    public partial class EntityViewer : UserControl
    {
        public int i = 0;
        public double pointSignRadius = 2.5;
        public string Definition;
        public double Scale;
        double fViewKoeffecient = 0.9;// polygon.AreaSpatial * 0.0003; //200; 
        private IGeometry fSpatial; //Пд для визуализации
        public EntityViewer()
        {
            InitializeComponent();
        }

        public void CreateView(object Entity)
        {
            this.Definition = "On create View";
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
          
            //button1.Content = "Click "+ i++.ToString();
            //this.Height++;
           
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

            InitCanvas(canvas1);
        }



        public double ViewKoeffecient
        {
            set
            {
                this.fViewKoeffecient = value;
                if (fSpatial != null)
                {
                    if (fSpatial.TypeName == "netFteo.Spatial.TMyPolygon")
                    {

                        Scale = ((TMyPolygon)Spatial).ScaleEntity(canvas1.Width, canvas1.Height) / value;
                    }
					
					if (fSpatial.TypeName == "netFteo.Spatial.TPolyLine")
					{
						Scale = ((TPolyLine)Spatial).ScaleEntity(canvas1.Width, canvas1.Height) / value;
					}

					DrawSpatial();
				}
                else
                    InitCanvas(canvas1);
            }

            get
            {
                return this.fViewKoeffecient;
            }
        }

        public IGeometry Spatial
        {
            set
            {
                if (value != null)
                {
                    this.fSpatial = value;
                    this.ViewKoeffecient = 0.7; //default 70%
                }
                else InitCanvas(canvas1);  // сотрем картинку (последнюю)
            }
            get
            {
                return this.fSpatial;
            }
        }

        public void InitCanvas(Canvas canvas)
        {
            canvas1.Children.Clear();
            label2.Content = "";
            Label_resScale.Content = "";
            label1_Canvas_Sizes.Content = "";
            i = 0;

            TextBlock textBlock = new TextBlock();
            Canvas.SetLeft(textBlock, canvas1.Width / 2);
            textBlock.Text = canvas1.Width.ToString();
            Canvas.SetTop(textBlock, canvas1.Height - 20);
            canvas.Children.Add(textBlock);

            TextBlock textBlock2 = new TextBlock();
            Canvas.SetLeft(textBlock2, 5);
            textBlock2.Text = canvas1.Height.ToString();
            Canvas.SetTop(textBlock2, canvas1.Height / 2);
            canvas.Children.Add(textBlock2);
            CheckWPFTier();
        }

        private void DrawSpatial()
        {
            if (this.fSpatial != null)
            {
                InitCanvas(canvas1);
                canvas1.Children.Clear();
                label_DirectXMode.Content = "";
                image1.Visibility = System.Windows.Visibility.Hidden; image2.Visibility = System.Windows.Visibility.Hidden;
                if (fSpatial.GetType().ToString() == "netFteo.Spatial.TMyPolygon")
                {
                    TMyPolygon polygon = (TMyPolygon)Spatial;
                    label2.Content = polygon.Definition;
                    label1_Canvas_Sizes.Content = "Площадь " + polygon.AreaSpatialFmt("0.00");
                    Label_resScale.Content = "M 1: " + (Scale).ToString("0.00") + "    Vk = " + ViewKoeffecient.ToString("0.0");
                    //Полигон
                    foreach(UIElement el in CreateCanvasPolygons(polygon))
                    canvas1.Children.Add(el);
                    //Точки
                    List<UIElement> cnspts = CreateCanvasPoints(polygon);
                    foreach (UIElement pt in cnspts)
                        canvas1.Children.Add(pt);

                    //Метки
                    List<TextBlock> labels = CreateCanvasPointLabels((netFteo.Spatial.TMyOutLayer) polygon);
                    foreach (TextBlock label in labels)
                        canvas1.Children.Add(label);

                }
				
				if (fSpatial.GetType().ToString() == "netFteo.Spatial.TPolyLine")
				{
					TPolyLine polygon = (TPolyLine)Spatial;
					label2.Content = polygon.Definition;
					label1_Canvas_Sizes.Content = "Длина " + polygon.Length.ToString("0.00");
					Label_resScale.Content = "M 1: " + (Scale).ToString("0.00") + "    Vk = " + ViewKoeffecient.ToString("0.0");
					//Полигон
					foreach (UIElement el in CreateCanvasPolygons(polygon))
						canvas1.Children.Add(el);
					//Точки
					List<UIElement> cnspts = CreateCanvasPoints(polygon);
					foreach (UIElement pt in cnspts)
						canvas1.Children.Add(pt);

					//Метки
					List<TextBlock> labels = CreateCanvasPointLabels((TMyOutLayer)polygon);
					foreach (TextBlock label in labels)
						canvas1.Children.Add(label);
				}
			}
        }

        //Измененение цветов меток точек
        //Для изучения доступа к элементам WPF Canvas`a после его и их создания
        private void FlashLabels(byte clRed )
        {
            byte red = clRed;
            foreach (UIElement el in canvas1.Children)
            {
                if (el.GetType().ToString() == "System.Windows.Controls.TextBlock")
                {
                    System.Windows.Media.Color cl = new Color();
                    cl = Color.FromRgb(red++, 125, 30);
                    ((TextBlock)el).Foreground = new SolidColorBrush(cl);
                }
            }
        }


 

        /// <summary>
        /// Создание "точки" в форме эллипса
        /// </summary>
        /// <param name="x">the positive X-axis points to the right</param>
        /// <param name="y">the positive Y-axis points to downward</param>
        /// <returns></returns>
        private UIElement CreateCanvasPoint(double x, double y)
        {
            System.Windows.Shapes.Ellipse el = new Ellipse();
            el.Stroke = System.Windows.Media.Brushes.Red;
            el.Fill = System.Windows.Media.Brushes.LightGreen;
            Canvas.SetLeft(el, x); //polygon.Centroid(canvas1.Width, canvas1.Height, Scale).x);
            Canvas.SetTop(el, y); //polygon.Centroid(canvas1.Width, canvas1.Height, Scale).y);
            el.Height = pointSignRadius*2; el.Width = pointSignRadius*2;
            return el;
        }

        private List<UIElement> CreateCanvasPoints(netFteo.Spatial.TMyOutLayer polygon)
        {
            List<UIElement> res = new List<UIElement>();
            PointCollection pts =   PointsToWindowsPoints(polygon.AverageCenter.x, polygon.AverageCenter.y, polygon, true);
            int sourcePointsIndex = 0;
            foreach (Point pt in pts)
            {
                res.Add(CreateCanvasPoint(pt.X-pointSignRadius, pt.Y- pointSignRadius));
                sourcePointsIndex++;
            }
            return res;
        }

        private TextBlock CreateCanvasTextBlock(double x, double y, string label, double fontsize)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = label;
            textBlock.FontSize = fontsize;
            System.Windows.Media.Color cl = new Color();
            cl = Color.FromRgb(100, 25, 30);
            textBlock.Foreground = new SolidColorBrush(cl);
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            return textBlock;
        }
        private List<TextBlock> CreateCanvasPointLabels(netFteo.Spatial.TMyOutLayer polygon)
        {
            List<TextBlock> res = new List<TextBlock>();
            PointCollection pts = PointsToWindowsPoints(polygon.AverageCenter.x, polygon.AverageCenter.y, polygon, true);
            int sourcePointsIndex = 0;
            foreach (Point pt in pts)
            {
                res.Add(CreateCanvasTextBlock(pt.X + 3, pt.Y - 9,
                                              polygon[sourcePointsIndex].NumGeopointA, 10));
                sourcePointsIndex++;
            }
            return res;
        }
        /// <summary>
        /// WPF Polygon
        /// Add the Polygon Element from System.Windows.Shapes.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private UIElement CreateCanvasPolygon(PointCollection polygon, bool Closed)

		{
			if (Closed)
			{
				var myPolyElement = new Polygon();
				myPolyElement.Stroke = System.Windows.Media.Brushes.Black;
				myPolyElement.Fill = System.Windows.Media.Brushes.LightGray;
				myPolyElement.StrokeThickness = 0.5;
				myPolyElement.HorizontalAlignment = HorizontalAlignment.Left;
				myPolyElement.VerticalAlignment = VerticalAlignment.Center;
				myPolyElement.Points = polygon;
				return myPolyElement;
			}

			Polyline WPFPolyLine = new Polyline();
			WPFPolyLine.Stroke = System.Windows.Media.Brushes.Black;
			//WPFPolyLine.Fill = System.Windows.Media.Brushes.LightGray;
			WPFPolyLine.StrokeThickness = 0.5;
			WPFPolyLine.HorizontalAlignment = HorizontalAlignment.Left;
			WPFPolyLine.VerticalAlignment = VerticalAlignment.Center;
			WPFPolyLine.Points = polygon;
			return WPFPolyLine;
		}

        private List<UIElement> CreateCanvasPolygons(TMyPolygon polygon)
        {
            List<UIElement> res = new List<UIElement>();
            List<PointCollection> polys =   PolygonToWindowsShape(polygon, true);
            List<PointCollection> polysOld = PolygonToWindowsShape(polygon, false);

            foreach (PointCollection poly in polys)
                res.Add(CreateCanvasPolygon(poly,true));

            foreach (PointCollection poly in polysOld) // старые границы
                res.Add(CreateCanvasPolygon(poly, true));
            return res;
        }


		private List<UIElement> CreateCanvasPolygons(TPolyLine polygon)
		{
			List<UIElement> res = new List<UIElement>();
			List<PointCollection> polys = PolygonToWindowsShape(polygon, true);
			List<PointCollection> polysOld = PolygonToWindowsShape(polygon, false);

			foreach (PointCollection poly in polys)
				res.Add(CreateCanvasPolygon(poly,false));

			foreach (PointCollection poly in polysOld) // старые границы
				res.Add(CreateCanvasPolygon(poly,false));
			return res;
		}

		/* WPF Coordinate System:
         * For 2D graphics, the WPF coordinate system locates the origin in the upper-left corner of the rendering area. 
         * In the 2D space, the positive X-axis points to the right, and the positive Y-axis points to downward
         */

		/// <summary>
		/// Конвертация и масштабирование точек netfteo в точки windows PointCollection
		/// </summary>
		/// 
		private PointCollection PointsToWindowsPoints(double originX, double originY, PointList layer, bool newOnly)
        {
            PointCollection myPointCollection = new PointCollection();
            double canvasX;
            double canvasY;

            foreach (TPoint point in layer)
            {
                if (newOnly)
                {
                    if (!Double.IsNaN(point.x))
                    {
                        canvasX = Math.Round(canvas1.Width / 2 - (originY - point.y) / Scale);
                        canvasY = Math.Round(canvas1.Height / 2 + (originX - point.x) / Scale);
                        myPointCollection.Add(new Point(canvasX, canvasY));
                    }
                }
                else
                {
                    if (!Double.IsNaN(point.oldX))
                    {
                        canvasX = Math.Round(canvas1.Width / 2 - (originY - point.oldX) / Scale);
                        canvasY = Math.Round(canvas1.Height / 2 + (originX - point.oldY) / Scale);
                        myPointCollection.Add(new Point(canvasX, canvasY));
                    }
                }
            }
            return myPointCollection;
        }


        //Обратное преобразование с координат canvas в координаты МСК
        private netFteo.Spatial.TPoint WindowsPointsToPoints(double x, double y, TMyOutLayer sourcelayer)
        {
            netFteo.Spatial.TPoint res = new netFteo.Spatial.TPoint();

            res.y = sourcelayer.AverageCenter.y - (canvas1.Width/2 - x) * Scale;
            res.x = sourcelayer.AverageCenter.x - (y - canvas1.Height/2)* Scale;
            return res;
        }

        /// <summary>
        /// Конвертация полигона netfteo в список точек PointCollection
        /// TODO - отображение двойного полигона - для old newOrd?
        /// </summary>
        private List<PointCollection> PolygonToWindowsShape(TMyPolygon polygon, bool newOnly)
        {
            List<PointCollection> res = new List<PointCollection>();
            PointCollection winPoints = new PointCollection();

            winPoints = PointsToWindowsPoints(polygon.AverageCenter.x, polygon.AverageCenter.y, polygon, newOnly);
            res.Add(winPoints);
            foreach (netFteo.Spatial.TMyOutLayer chld in polygon.Childs)
            {
                PointCollection childwinPoints = new PointCollection();
                childwinPoints = PointsToWindowsPoints(polygon.AverageCenter.x, polygon.AverageCenter.y, chld,newOnly);
                res.Add(childwinPoints);
            }
            return res;
        }

		private List<PointCollection> PolygonToWindowsShape(TPolyLine polygon, bool newOnly)
		{
			List<PointCollection> res = new List<PointCollection>();
			PointCollection winPoints = new PointCollection();

			winPoints = PointsToWindowsPoints(polygon.AverageCenter.x, polygon.AverageCenter.y, polygon, newOnly);
			res.Add(winPoints);
			/*
			foreach (netFteo.Spatial.TMyOutLayer chld in polygon.Childs)
			{
				PointCollection childwinPoints = new PointCollection();
				childwinPoints = PointsToWindowsPoints(polygon.AverageCenter.x, polygon.AverageCenter.y, chld, newOnly);
				res.Add(childwinPoints);
			}
			*/
			return res;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitCanvas(canvas1);
            DrawSpatial();
        }

        /// <summary>
        /// Check WPF Tier - Hardware GPU capabiltes in context of DirectX Tehnology
        /// </summary>
        private void CheckWPFTier() 
        { //Shift the value 16 bits to retrieve the rendering tier. 
            int currentRenderingTier = (RenderCapability.Tier >> 16);
            switch (currentRenderingTier)
            { //DirectX version level less than 7.0. 
                case 0: label_DirectXMode.Content = string.Format("DirectX [{0}] - No hardware acceleration.", currentRenderingTier.ToString());
                    break;
                //DirectX version level greater than 7.0 but less than 9.0. 
                case 1: label_DirectXMode.Content = string.Format("DirectX [{0}] - Partial acceleration.", currentRenderingTier.ToString());
                    break;
                //DirectX version level greater than or equal to 9.0. 
                case 2: label_DirectXMode.Content = string.Format("DirectX [{0}] - Total acceleration.", currentRenderingTier.ToString()); break;
            }
            image1.Visibility = System.Windows.Visibility.Visible; image2.Visibility = System.Windows.Visibility.Visible;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void label_res_Scale_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void button_ScaleUp_Click(object sender, RoutedEventArgs e)
        {
            ViewKoeffecient += 0.1;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            ViewKoeffecient -= 0.1;
        }


        private bool SnapPosition(Canvas cnv, double x, double y)
        {
            foreach (UIElement child in cnv.Children)
            {
                if (child.GetType().ToString() == "System.Windows.Shapes.Polygon")
                {
                    System.Windows.Shapes.Polygon poly = (System.Windows.Shapes.Polygon) child;
                    foreach (Point pt in poly.Points)
                    {
                        if ((pt.X == Math.Round(x)) && (pt.Y == Math.Round(y)))
                            return true;
                    }
                }
            }
            return false;
        }


        private void canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            /*            
           label_DirectXMode.Content = "x " + e.GetPosition(canvas1).X.ToString("0.000") +
                                       " y " + e.GetPosition(canvas1).Y.ToString("0.000");

           label_DirectXMode.Content = String.Format("{0} {1}",
                                                           e.GetPosition(canvas1).X, e.GetPosition(canvas1).Y);
                                       */
            label_DirectXMode.Content = "";
			if (Spatial == null) return;
			if (Spatial.TypeName == "netFteo.Spatial.TMyPolygon")
			{
				netFteo.Spatial.TMyPolygon polygon = (netFteo.Spatial.TMyPolygon)Spatial;
				if (polygon != null)
					label_DirectXMode.Content = String.Format(" {0:F3}, {1:F3} ",
																WindowsPointsToPoints(e.GetPosition(canvas1).X,
																					  e.GetPosition(canvas1).Y, polygon).x,
																WindowsPointsToPoints(e.GetPosition(canvas1).X,
																					  e.GetPosition(canvas1).Y, polygon).y);
				if (SnapPosition(canvas1, e.GetPosition(canvas1).X, e.GetPosition(canvas1).Y))
				{
					label_DirectXMode.Content = "s! " + label_DirectXMode.Content; // Mouse  got SNAP !!
				}
			}
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //FlashLabels(Convert.ToByte(textBox1.Text));
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Label_resScale.Content = "User Control size changed ";
        }
    }
}
