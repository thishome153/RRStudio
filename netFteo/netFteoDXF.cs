using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using netFteo.Spatial;

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



namespace netFteo.IO
{


    public delegate void DxfParsingHandler(object sender, ESCheckingEventArgs e);

    /// <summary>
    /// DXF reader for dxf reading.
    /// Fixosoft wrapper for netdxf library code
    /// </summary>
    /// <remarks>
    /// Targetting NET Framework 4.0 
    /// </remarks> 
    public class  DXFReader : TextReader
    {

        private netDxf.DxfDocument dxfFile;
        public string FileName
        {
            get
            {
                return dxfFile.Name;
            }
        }
        public string Version
        {
            get
            {
               return dxfFile.DrawingVariables.AcadVer.ToString();
            }
        }

        public  DXFReader(string FileName)
        {
            dxfFile = netDxf.DxfDocument.Load(FileName);

            this.BlocksCount = dxfFile.Blocks.Count;

            this.AddedObjects = dxfFile.AddedObjects.Count;
            BodyLoad(FileName);
        }

        /// <summary>
        /// Количество геометрий 
        /// </summary>
        ///<param name="codename"> Тип геометрии. По умолчанию "LWPOLYLINE"</param>

        public int PolygonsCount(string codename = "LWPOLYLINE")
        {
                int res = 0;
                foreach (netDxf.Blocks.Block block in dxfFile.Blocks)
                {
                    foreach (netDxf.Entities.EntityObject entity in block.Entities) // in block may be placed inner borders (second, third ring)
                    {
                        if (entity.CodeName.Equals(codename))
                            res++;
                    }
                }
                return res;
        }

		/// <summary>
		/// Импорт dxf-файлов
		/// </summary>
		/// <returns></returns>
		public TPolygonCollection ParseDXF()
		{
			TPolygonCollection res = new TPolygonCollection();
			if (dxfFile == null) return null;
			// Direct objects (not blocked):
			// Polylines (every - closed & open)
			foreach (LwPolyline poly in dxfFile.LwPolylines)
			{
				DxfParsingProc("dxf", ++FileParsePosition, null);
				TMyOutLayer Polyline = DXF_ParseRing(poly);
				if (Polyline != null)
					try
					{
						Polyline.Definition = poly.CodeName + "." + poly.Handle; ;
						res.AddPolygon(Polyline);
					}

					catch (ArgumentException error)
					{

						//return null; // wrong block entities
					}
			}


			//Blocked Objects
			if (dxfFile.Blocks.Count > 0)
			{
				foreach (Block block in dxfFile.Blocks)
				{
					foreach (EntityObject entity in block.Entities) // in block may be placed inner borders (second, third ring)
					{
						if (entity.CodeName.Equals("LWPOLYLINE"))
						{
							DxfParsingProc("dxf", ++FileParsePosition, null);
							TMyPolygon Polygon = DXF_ParseRegion(block.Entities);
							if (Polygon != null)
								try
								{
									if ((block.AttributeDefinitions.Count > 0) && (block.AttributeDefinitions["Кад_номер"].Value != null))
										Polygon.Definition = (string)block.AttributeDefinitions["Кад_номер"].Value;
									else
										Polygon.Definition = block.CodeName + "." + block.Handle;
									if (Polygon.Definition == "")
										Polygon.Definition = block.CodeName + "." + block.Handle;
									res.AddPolygon(Polygon);
									goto NEXTBlock; // all entites here is ring + inner rings
								}
								catch (ArgumentException error)
								{
									goto NEXTBlock;
									//return null; // wrong block entities
								}


						}
					}
					NEXTBlock: int fakeVariable = 0;
				}


	
			}
			return res;
		}


        public int  AddedObjects;
        public int  BlocksCount;
        private int FileParsePosition; // Текущая позиция parser`a

        /// <summary>
        /// An event handler invoked during parsing  of entries in the dxf file.
        /// </summary>
        /// <remarks>
        /// Able to check total progress of file parsing
        /// </remarks>
        public event DxfParsingHandler OnParsing;
        private void DxfParsingProc(string sender, int process, byte[] Data)
        {
            if (OnParsing == null) return;
            ESCheckingEventArgs e = new ESCheckingEventArgs();
            e.Definition = sender;
            e.Data = Data;
            e.Process = process;
            OnParsing(this, e);
        }

		/// <summary>
		/// Parse dxf LwPolyLine to point list
		/// </summary>
		/// <param name="poly"></param>
		/// <returns></returns>
        private TMyOutLayer DXF_ParseRing(LwPolyline poly)
        {
          //  if (!poly.IsClosed) return null;
            if (poly.Vertexes.Count < 2) return null;
            int ptNum = 0;
            TMyOutLayer res = new TMyOutLayer();
            try
            {
                foreach (netDxf.Entities.LwPolylineVertex vertex in poly.Vertexes)
                {
                    netFteo.Spatial.Point point = new  netFteo.Spatial.Point(vertex.Location.Y, vertex.Location.X);
                    point.oldX = vertex.Location.Y;
                    point.oldY = vertex.Location.X;
                    point.NumGeopointA = "dxf" + (++ptNum).ToString();
                    res.AddPoint(point);
                }
            }

            catch (IOException ex)
            {
                return null;
                //  MessageBox.Show(ex.ToString());
            }
            return res;
        }

        private TMyPolygon DXF_ParseRegion(EntityCollection polys)
        {
            TMyPolygon res = new TMyPolygon("dxfblock");
            try
            {
                if ((polys.Count > 0) && (polys[0].CodeName.Equals("LWPOLYLINE")))
                {
                    res.AppendPoints(DXF_ParseRing((LwPolyline)polys[0]));
                }
                // childs:

                for (int i = 1; i <= polys.Count - 1; i++)
                {
                    if ((polys.Count > 0) && (polys[i].CodeName.Equals("LWPOLYLINE")))
                    {
                        res.AddChild(DXF_ParseRing((LwPolyline)polys[i]));
                    }
                }
            }

            catch (IOException ex)
            {
                return null;
                //  MessageBox.Show(ex.ToString());
            }
            return res;
        }


    }
    
    /// <summary>
    /// DXF writer for netfteo application ecosphere.
    /// Fixosoft wrapper for netdxf library code
    /// </summary>
    /// <remarks>
    /// Targetting NET Framework 4.0 
    /// </remarks> 
    public class DXFWriter
    {
        // Полигон, точнее замкнутуая полилиния, понимаемый и редактируемый NanoCad
        private EntityObject CreateDxfPolygon(DxfDocument dxfDoc, netDxf.Tables.Layer LayerPoints, netDxf.Tables.Layer LayerText, netDxf.Tables.Layer LayerPoly, netFteo.Spatial.PointList Points)
        {

            List<LwPolylineVertex> PlVertexLst = new List<LwPolylineVertex>();  //Список Vertexов (вершин) полилинии:
            double xx;
            double yy;

            for (int i = 0; i <= Points.Count - 1; i++)
            {
                if (!Double.IsNaN(Points[i].x))
                {
                    yy = Points[i].y;
                    xx = Points[i].x;
                }
                else  // accept old ord
                {
                    yy = Points[i].oldY;
                    xx = Points[i].oldX;
                }

                PlVertexLst.Add(new LwPolylineVertex(yy, xx));

                CreatePoint(dxfDoc, LayerPoints, LayerText, Points[i]);
            }

            /*/       //3Д Полилиния
                   Polyline PLine = new Polyline(PlVertexLst, true); //Сама полилиния, замкнутая true:
                   PLine.Layer = LayerPoly;
                   dxfDoc.AddEntity(PLine);        //Вгоняем в dxf:
             * */
            // 2d Полилиния
            LwPolyline lwpolyline = new LwPolyline(PlVertexLst, true); //Сама полилиния, замкнутая true:
            lwpolyline.Layer = LayerPoly;
            //dxfDoc.AddEntity(lwpolyline);        //Вгоняем в dxf: вгоним в  блок
            return lwpolyline;
        }

        //--------Штриховки окружностей - эмуляция точек. 0.75 для 1:500  45 для 1:33333
        private void CreatePolygonHatches(DxfDocument dxfDoc, netDxf.Tables.Layer LayerHatches, netFteo.Spatial.PointList Points, double Radius)
        {
            for (int i = 0; i <= Points.Count - 1; i++)
            {
                //Point3d center = new Point3d(Layer[i].y, Layer[i].x, Layer[i].z);
                Vector2 center = new Vector2(Points[i].y, Points[i].x);
                Circle circle = new Circle(center, Radius);
                circle.Layer = LayerHatches;
                dxfDoc.AddEntity(circle);


                netDxf.Entities.HatchPattern hp = new HatchPattern("SOLID");
                hp.Scale = 2;
                hp.Type = HatchType.Predefined;
                List<HatchBoundaryPath> hbPathList = new List<HatchBoundaryPath>(1);
                HatchBoundaryPath hbPath = new HatchBoundaryPath(new List<EntityObject> { circle });
                hbPathList.Add(hbPath);
                Hatch PointHatch = new Hatch(hp, hbPathList);
                PointHatch.Layer = LayerHatches;
                dxfDoc.AddEntity(PointHatch);
            }
        }
        private EntityObject CreatePoint(DxfDocument dxfDoc, netDxf.Tables.Layer LayerPoints, netDxf.Tables.Layer LayerText, netFteo.Spatial.Point point)
        {
            netDxf.Entities.Point Pt = new Point(point.y, point.x, point.z);
            Pt.Layer = LayerPoints;
            dxfDoc.AddEntity(Pt);
            netDxf.Entities.Text PointName = new Text();
            if (point.NumGeopointA == null)
                PointName.Value = "-";
            else PointName.Value = point.NumGeopointA;
            PointName.Height = 5;
            PointName.Position = new Vector3(point.y, point.x, point.z);
            PointName.Layer = LayerText;
            dxfDoc.AddEntity(PointName);
            return Pt;
        }

        /// <summary>
        /// Запись пространственных данных в файл dxf
        /// </summary>
        /// <param name="Filename">File name, of coarse</param>
        /// <param name="ES">Spatial data</param>
        /// <param name="HatchRadius"> Штриховки окружностей - эмуляция точек. 0.75 для 1:500, 45 - для 1:33333</param>
        public void SaveAsDxfScale(string Filename, Object ES, double HatchRadius)
        {
            DxfDocument dxfDoc = new DxfDocument();
            dxfDoc.Comments.Clear();
            dxfDoc.Comments.Add("Producer: netfteo v"+
            String.Format(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));  //"Версия {0}", 
            dxfDoc.Comments.Add("DxfVersion.AutoCad2004");
            //DxfVersion dxfVersion = new DxfVersion();
            dxfDoc.DrawingVariables.AcadVer =  netDxf.Header.DxfVersion.AutoCad2004;
            //netDxf.Matrix3 ucsMatrix = new Matrix3();
            netDxf.Tables.Layer LayerHatches = new netDxf.Tables.Layer(Path.GetFileNameWithoutExtension(Filename) + " Штриховки");
            netDxf.Tables.Layer LayerText = new netDxf.Tables.Layer(Path.GetFileNameWithoutExtension(Filename) + " ТочкиНомер");
            netDxf.Tables.Layer LayerPoints = new netDxf.Tables.Layer(Path.GetFileNameWithoutExtension(Filename) + " Точки");
            netDxf.Tables.Layer LayerCN = new netDxf.Tables.Layer(Path.GetFileNameWithoutExtension(Filename) + " КН");
            netDxf.Tables.Layer LayerPoly = new netDxf.Tables.Layer(Path.GetFileNameWithoutExtension(Filename) + " Полигоны");
            LayerPoly.Color = AciColor.Magenta;
            LayerCN.Color = AciColor.Blue;
            LayerCN.IsVisible = false;
            LayerPoints.IsVisible = false;
            LayerText.IsVisible = false; LayerHatches.IsVisible = false;
            dxfDoc.Layers.Add(LayerHatches);
            dxfDoc.Layers.Add(LayerText);
            dxfDoc.Layers.Add(LayerPoints);
            dxfDoc.Layers.Add(LayerCN);
            dxfDoc.Layers.Add(LayerPoly);
            dxfDoc.Layers.Add(LayerCN);


            string test = ES.GetType().Name;
            if ((ES.GetType().Name == "TPolygonCollection")
                || (ES.GetType().Name == "TCompozitionEZ"))
            {
                TPolygonCollection Contours = (TPolygonCollection)ES;
                for (int ic = 0; ic <= Contours.Count - 1; ic++)
                {
                    netDxf.Entities.Text ContourDef = new netDxf.Entities.Text();
                    ContourDef.Value = Contours[ic].Definition;
                    ContourDef.Height = HatchRadius;
                    ContourDef.Position = new Vector3(Math.Abs(Contours[ic].CentroidMassive.y), Math.Abs(Contours[ic].CentroidMassive.x), Contours[ic].CentroidMassive.z);
                    ContourDef.Layer = LayerCN;
                    dxfDoc.AddEntity(ContourDef); // если в блок то не надо - дважды Entity не вставтляется


                    netDxf.Blocks.Block block = new netDxf.Blocks.Block("Polygon" + ic.ToString());
                    //block.Layer = LayerPoly;// new netDxf.Tables.Layer("BlockSample");
                    //block.Position = new Vector3(Math.Abs(Contours.Items[ic].Centroid.y), Math.Abs(Contours.Items[ic].Centroid.x), Contours.Items[ic].Centroid.z);
                    AttributeDefinition attdef = new AttributeDefinition("Кад_номер"); // без пробелов!!!
                    attdef.Text = "atribute def .Text";///Contours.Items[ic].Definition;
                    attdef.Value = Contours[ic].Definition;
                    attdef.Flags = AttributeFlags.Hidden;
                    /* //Можно отображать  атрибуты:
                    attdef.Position = new Vector3(Math.Abs(Contours.Items[ic].Centroid.y), Math.Abs(Contours.Items[ic].Centroid.x+1), Contours.Items[ic].Centroid.z);
                    TextStyle txt = new TextStyle("MyStyle", "Arial.ttf");
                    txt.IsVertical = true;
                    attdef.Style = txt;
                    attdef.WidthFactor = 1;
                    attdef.Height = HatchRadius;
                    attdef.Alignment = TextAlignment.MiddleCenter;
                    attdef.Rotation = 45;*/
                    AttributeDefinition attdefArea = new AttributeDefinition("Площадь"); // без пробелов!!!
                    attdefArea.Flags = AttributeFlags.Hidden;
                    attdefArea.Value = Contours[ic].AreaSpatialFmt("0.00");
                    block.AttributeDefinitions.Add(attdef);
                    block.AttributeDefinitions.Add(attdefArea);

                    block.Entities.Add((LwPolyline)CreateDxfPolygon(dxfDoc, LayerPoints, LayerText, LayerPoly, Contours[ic]));

                    CreatePolygonHatches(dxfDoc, LayerHatches, Contours[ic], HatchRadius);
                    //внутренние границы   
                    for (int i = 0; i <= Contours[ic].Childs.Count - 1; i++)
                    {
                        block.Entities.Add(CreateDxfPolygon(dxfDoc, LayerPoints, LayerText, LayerPoly, Contours[ic].Childs[i]));
                        CreatePolygonHatches(dxfDoc, LayerHatches, Contours[ic].Childs[i], HatchRadius);
                    }
                    Insert insDm = new Insert(block);
                    insDm.Layer = LayerPoly;
                    dxfDoc.AddEntity(insDm);
                }
            }

            if (ES.GetType().Name == "TPolyLines")
            {
                TPolyLines Plines = (TPolyLines)ES;
                for (int ic = 0; ic <= Plines.Count - 1; ic++)
                {
                    netDxf.Entities.Text ContourDef = new Text();
                    ContourDef.Value = Plines[ic].Definition;
                    ContourDef.Height = 30;
                    ContourDef.Position = new Vector3(Math.Abs(Plines[ic].CentroidMassive.y), Math.Abs(Plines[ic].CentroidMassive.x), Plines[ic].CentroidMassive.z);
                    ContourDef.Layer = LayerCN;
                    dxfDoc.AddEntity(ContourDef);

                    dxfDoc.AddEntity(CreateDxfPolygon(dxfDoc, LayerPoints, LayerText, LayerPoly, Plines[ic]));
                    CreatePolygonHatches(dxfDoc, LayerHatches, Plines[ic], HatchRadius);

                }
            }

            if (ES.GetType().Name == "TMyPoints")
            {
                PointList Points = (PointList)ES;
                for (int ic = 0; ic <= Points.PointCount - 1; ic++)
                {
                    CreatePoint(dxfDoc, LayerPoints, LayerText, Points[ic]);
                }
            }



            dxfDoc.Save(Filename);// "sample 2004.dxf"); 
        }

        #region Пример использования dxf.Block
        private static void BlockWithAttributes(DxfDocument dxf)
        {

            //DxfDocument dxf = new DxfDocument();
            Block block = new Block("BlockWithAttributes");
            block.Layer = new netDxf.Tables.Layer("BlockSample");
            // It is possible to change the block position, even though it is recommended to keep it at Vector3.Zero,
            // since the block geometry is expressed in local coordinates of the block.
            // The block position defines the base point when inserting an Insert entity.
            block.Position = new Vector3(10, 5, 0);

            // create an attribute definition, the attdef tag must be unique as it is the way to identify the attribute.
            // even thought AutoCad allows multiple attribute definition in block definitions, it is not recommended
            AttributeDefinition attdef = new AttributeDefinition("NewAttribute");
            // this is the text prompt shown to introduce the attribute value when a new Insert entity is inserted into the drawing
            attdef.Text = "InfoText";
            // optionally we can set a default value for new Insert entities
            attdef.Value = 0;
            // the attribute definition position is in local coordinates to the Insert entity to which it belongs
            attdef.Position = new Vector3(1, 1, 0);

            // modifying directly the text style might not get the desired results. Create one or get one from the text style table, modify it and assign it to the attribute text style.
            // one thing to note, if there is already a text style with the assigned name, the existing one in the text style table will override the new one.
            //attdef.Style.IsVertical = true;

            TextStyle txt = new TextStyle("MyStyle", "Arial.ttf");
            txt.IsVertical = true;
            attdef.Style = txt;
            attdef.WidthFactor = 2;
            // not all alignment options are avaliable for ttf fonts 
            attdef.Alignment = TextAlignment.MiddleCenter;
            attdef.Rotation = 90;

            // remember, netDxf does not allow adding attribute definitions with the same tag, even thought AutoCad allows this behaviour, it is not recommended in anyway.
            // internally attributes and their associated attribute definitions are handled through dictionaries,
            // and the tags work as ids to easily identify the information stored in the attributte value.
            // When reading a file the attributes or attribute definitions with duplicate tags will be automatically removed.
            // This is subject to change on public demand, it is possible to reimplement this behaviour with simple collections to allow for duplicate tags.
            block.AttributeDefinitions.Add(attdef);

            // The entities list defines the actual geometry of the block, they are expressed in th block local coordinates
            Line line1 = new Line(new Vector3(-5, -5, 0), new Vector3(5, 5, 0));
            Line line2 = new Line(new Vector3(5, -5, 0), new Vector3(-5, 5, 0));
            block.Entities.Add(line1);
            block.Entities.Add(line2);

            // You can check the entity ownership with:
            Block line1Owner = line1.Owner;
            Block line2Owner = line2.Owner;
            // in this example line1Oner = line2Owner = block
            // As explained in the PaperSpace() sample, the layout associated with a common block will always be null
            Layout associatedLayout = line1.Owner.Record.Layout;
            // associatedLayout = null

            // create an Insert entity with the block definition, during the initialization the Insert attributes list will be created with the default attdef properties
            Insert insert1 = new Insert(block)
            {
                Position = new Vector3(5, 5, 5),
                Normal = new Vector3(1, 1, 1),
                Rotation = 45
            };

            // When the insert position, rotation, normal and/or scale are modified we need to transform the attributes.
            // It is not recommended to manually change the attribute position and orientation and let the Insert entity handle the transformations to mantain them in the same local position.
            // The attribute position and orientation are stored in WCS (world coordinate system) even if the documentation says they are in OCS (object coordinate system). The documentation is WRONG!.
            // In this particular case we have changed the position, normal and rotation.
            insert1.TransformAttributes();

            // Once the insert has been created we can modify the attributes properties, the list cannot be modified only the items stored in it
            insert1.Attributes[attdef.Tag].Value = 24;

            // Modifying directly the layer might not get the desired results. Create one or get one from the layers table, modify it and assign it to the insert
            // One thing to note, if there is already a layer with the same name, the existing one in the layers table will override the new one, when the entity is added to the document.
            netDxf.Tables.Layer layer = new netDxf.Tables.Layer("MyInsertLayer");
            layer.Color.Index = 4;

            // optionally we can add the new layer to the document, if not the new layer will be added to the Layers collection when the insert entity is added to the document
            // in case a new layer is found in the list the add method will return the layer already stored in the list
            // this behaviour is similar for all TableObject elements, all table object names must be unique (case insensitive)
            layer = dxf.Layers.Add(layer);

            // assign the new layer to the insert
            insert1.Layer = layer;

            // add the entity to the document
            dxf.AddEntity(insert1);

            // create a second insert entity
            // the constructor will automatically reposition the insert2 attributes to the insert local position
            Insert insert2 = new Insert(block, new Vector3(10, 5, 0));

            // as before now we can change the insert2 attribute value
            insert2.Attributes[attdef.Tag].Value = 34;

            // additionally we can insert extended data information
            XData xdata1 = new XData(new ApplicationRegistry("netDxf"));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata1.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionX, 0));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionY, 0));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionZ, 0));
            xdata1.XDataRecord.Add(XDataRecord.CloseControlString);

            insert2.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata1.ApplicationRegistry.Name, xdata1},
                             };
            dxf.AddEntity(insert2);

            // all entities support this feature
            XData xdata2 = new XData(new ApplicationRegistry("MyApplication1"));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata2.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "string record"));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Int32, 350));
            xdata2.XDataRecord.Add(XDataRecord.CloseControlString);

            // multiple extended data entries might be added
            XData xdata3 = new XData(new ApplicationRegistry("MyApplication2"));
            xdata3.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata3.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata3.XDataRecord.Add(new XDataRecord(XDataCode.String, "string record"));
            xdata3.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata3.XDataRecord.Add(new XDataRecord(XDataCode.Int32, 350));
            xdata3.XDataRecord.Add(XDataRecord.CloseControlString);

            Circle circle = new Circle(Vector3.Zero, 5);
            circle.Layer = new netDxf.Tables.Layer("MyCircleLayer");
            // AutoCad 2000 does not support true colors, in that case an approximated color index will be used instead
            circle.Layer.Color = new AciColor(System.Drawing.Color.MediumSlateBlue);
            circle.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata2.ApplicationRegistry.Name, xdata2},
                                 {xdata3.ApplicationRegistry.Name, xdata3},
                             };
            dxf.AddEntity(circle);

            // dxf.Save("BlockWithAttributes.dxf");
            // DxfDocument dxfLoad = DxfDocument.Load("BlockWithAttributes.dxf");
        }
        #endregion




    }
}
