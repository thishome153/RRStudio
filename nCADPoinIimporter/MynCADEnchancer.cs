namespace netFteo.CAD
{
    using System;
    
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using HostMgd.ApplicationServices;
    using HostMgd.EditorInput;
    using Teigha.DatabaseServices;
    using Teigha.Geometry;
    using Teigha.Runtime;
    using Platform = HostMgd;
    using PlatformDb = Teigha;
    using netFteo;
    using netFteo.Spatial;




        public class MynCADEnchancer
    {
		public MynCADEnchancer()
		{
			LogStarttoWebServer("Nanocad .NET plugin");
			DocumentCollection dm = Platform.ApplicationServices.Application.DocumentManager;
			// Get the command line editor object
			Editor ed = dm.MdiActiveDocument.Editor;
			ed.WriteMessage("\nFixosoft Nanocad plugin @2015-19. v" + AssemblyVersion());
			ed.WriteMessage("\nInstance created. Log success");
		}
            private bool ParseData(DocumentCollection dm, Editor ed, PromptFileNameResult sourceFileName, TEntitySpatial FteoFile)
            {
                if (FteoFile == null) return false;
                ed.WriteMessage("\nFixosoft Nanocad plugin @2015-19. v" + AssemblyVersion());
                ed.WriteMessage("\nОбработка файла " + Path.GetFileNameWithoutExtension(sourceFileName.StringResult));
                ed.WriteMessage("\nРазбор файла  " + sourceFileName.StringResult);
                if (FteoFile.Count == 0) { ed.WriteMessage("\nОшибка файла - пустой файл. Проверьте формат и файл!"); return false; };
                /*   0 - Creates a point at 5 percent of the drawing area height.
                    >0 - Specifies an absolute size [The "Set Size in Absolute Units" is selected].
                    <0 - Specifies a percentage of the viewport size [The "Set Size Relative to Screen" is selected].
                 * */
                SetPoinStyles(35, -2);
                Matrix3d ucsMatrix = ed.CurrentUserCoordinateSystem;
                Database db = dm.MdiActiveDocument.Database;
                using (db)
                {
                    // Create a transaction
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        LayerTable LT = (LayerTable)db.TransactionManager.GetObject(db.LayerTableId, OpenMode.ForRead, false);
                        ed.WriteMessage("\nТаблица слоев : " + LT.ObjectId.ToString());

                        LayerTableRecord ltr = new LayerTableRecord();
                        ltr.Name = Path.GetFileNameWithoutExtension(sourceFileName.StringResult) + " Точки_Имя";
                        LT.UpgradeOpen();
                        ObjectId ltId = LT.Add(ltr);
                        tr.AddNewlyCreatedDBObject(ltr, true);
                        ed.WriteMessage(ltr.Name + " " + ltId.ObjectClass.AppName);

                        LayerTableRecord ltrP = new LayerTableRecord();
                        ltrP.Name = Path.GetFileNameWithoutExtension(sourceFileName.StringResult) + " Точки";
                        LT.UpgradeOpen();
                        ObjectId ltPId = LT.Add(ltrP);
                        tr.AddNewlyCreatedDBObject(ltrP, true);
                        ed.WriteMessage(ltrP.Name + " " + ltPId.ObjectClass.AppName);

                        LayerTableRecord ltrCir = new LayerTableRecord();
                        ltrCir.Name = Path.GetFileNameWithoutExtension(sourceFileName.StringResult) + " Штриховки";
                        LT.UpgradeOpen();
                        ObjectId ltCirId = LT.Add(ltrCir);
                        tr.AddNewlyCreatedDBObject(ltrCir, true);
                        ed.WriteMessage(ltrCir.Name + " " + ltCirId.ObjectClass.AppName);

                        LayerTableRecord ltrPoly = new LayerTableRecord();
                        ltrPoly.Name = Path.GetFileNameWithoutExtension(sourceFileName.StringResult) + " Полигоны";
                        LT.UpgradeOpen();
                        ObjectId ltPolyid = LT.Add(ltrPoly);
                        tr.AddNewlyCreatedDBObject(ltrPoly, true);
                        ed.WriteMessage(ltrPoly.Name + " " + ltPolyid.ObjectClass.AppName);

                        LayerTableRecord ltrColorator = new LayerTableRecord();
                        ltrColorator.Name = Path.GetFileNameWithoutExtension(sourceFileName.StringResult) + " Отмывка";
                        LT.UpgradeOpen();
                        ObjectId ltColorid = LT.Add(ltrColorator);
                        tr.AddNewlyCreatedDBObject(ltrColorator, true);
                        ed.WriteMessage(ltrColorator.Name + " " + ltColorid.ObjectClass.AppName);

                        // Get the table block record for current drawing space
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);


                        if (FteoFile != null)

                        foreach(IGeometry Feature in FteoFile)
                            {
							
							if (Feature.TypeName == "netFteo.Spatial.TMyPolygon")
                                CreatePolygonFull(ucsMatrix, btr, tr, ltPolyid, ltPId, ltId, ltCirId, ltColorid, Feature);
                            }

                        ed.WriteMessage("Добавлено " + FteoFile.Count.ToString() + " полигонов.");

                        btr.Dispose();
                        // Commit the transaction
                        tr.Commit();
                        //ed.WriteMessage("Добавлено " + Convert.ToString(PointCount) + " точек");
                        ed.WriteMessage("\n Ok");

                    }
                }
                return true;
            }
                  


        //[CommandMethod("IMPORT_PKZO","Формат x tab y ПКЗО", CommandFlags.Modal)]
        [CommandMethod("IMPORT_PKZO")]
            //  [CommandMethod("LabelAreaFields", "lap", CommandFlags.Modal | CommandFlags.Redraw)] 
        public void ImportCoordsPKZO()
        {
            // Get the manager object for all opened documents and then use it for getting the drawing database
            DocumentCollection dm = Platform.ApplicationServices.Application.DocumentManager;
            Database db = dm.MdiActiveDocument.Database;
           
            // Get the command line editor object
            Editor ed = dm.MdiActiveDocument.Editor;

            // An object that gets the result of user input
            PromptFileNameResult sourceFileName;

            Matrix3d ucsMatrix = ed.CurrentUserCoordinateSystem;

            // Print the prompt within the command line and get the result
            sourceFileName = ed.GetFileNameForOpen("\nEnter the name of the coordinates file to be imported:");
            if (sourceFileName.Status == PromptStatus.OK)
            {
                if (Path.GetExtension(sourceFileName.StringResult).Equals(".txt", StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        using (db)
                        {
                            // Create a transaction
                            using (Transaction tr = db.TransactionManager.StartTransaction())
                            {

                                ed.WriteMessage("\nLayerTable получена: ");
                                // Get the table block record for current drawing space
                                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                                // Read the file and get its contents as a string array
                                string[] lines = File.ReadAllLines(sourceFileName.StringResult);

                                // For each string create an array of substrings delimited by space symbol,
                                // then convert array elements to numeric values.
                                string[] coord;
                                int PointCount = 0;
                                foreach (string s in lines)
                                {
                                    
                                    //coord = s.Split(new char[] { ' ' });
                                    string TabDelimiter = "\t";  // tab
                                    coord = s.Split(TabDelimiter.ToCharArray());
                                    ed.WriteMessage("\nFile string: " +s);

                                    // Set a decimal point as the decimal delimiter
                                    NumberFormatInfo nfi = new NumberFormatInfo();
                                    nfi.NumberDecimalSeparator = ".";

                                    // Parse string values
                                    double coordX = double.Parse(coord[1], nfi);//меняем местами
                                    double coordY = double.Parse(coord[0], nfi);
                                    
                                  //  double coordZ = double.Parse(coord[2], nfi);

                                    // Create a point object
                                    DBPoint point = new DBPoint(new Point3d(coordX, coordY,0));
                                    point.TransformBy(ucsMatrix.Inverse());
                                    // Append the point to the database
                                    btr.AppendEntity(point);
                                    // Add the object to the transaction
                                    tr.AddNewlyCreatedDBObject(point, true);
                                    PointCount++;

                                }
                                
                                ed.WriteMessage("\n Добавлено точек: " + Convert.ToString(PointCount));
                                ed.WriteMessage("\n BlockTableRecord.Bounds: " + btr.Bounds.ToString());
                                btr.Dispose();

                                // Commit the transaction
                                tr.Commit();
                            }
                        }
                    }
                    catch (PlatformDb.Runtime.Exception ex)
                    {
                        ed.WriteMessage("\nError while importing coordinates: " + ex.Message);
                    }
                }
                else
                {
                    ed.WriteMessage("\nInput file must be in .TXT format.");
                }
            }
        }

        [CommandMethod("ShowMe")]
        public void ShowMe()
        {
            System.Windows.Forms.Form form = new AboutnCADLibraryForm();
            Platform.ApplicationServices.Application.ShowModalDialog(form);
        }


        #region //----------------------------------IMPORT FTEO NumXYZDescr 2014-2015-2016----------------------------//
        [CommandMethod("IMPORT_TextFiles")]  // NumXYZDescr multiple
        public void ImportCoordsFteo2()
        {

            // Get the manager object for all opened documents and then use it for getting the drawing database
            DocumentCollection dm = Platform.ApplicationServices.Application.DocumentManager;
            // Get the command line editor object
            Editor ed = dm.MdiActiveDocument.Editor;
            // An object that gets the result of user input
            PromptFileNameResult sourceFileName;

            // Print the prompt within the command line and get the result
            PromptOpenFileOptions PO = new PromptOpenFileOptions("Введите имя текстового файла :");
            PO.Filter = "Текстовые файлы|*.txt";
            sourceFileName = ed.GetFileNameForOpen(PO);
            if (sourceFileName.Status == PromptStatus.OK)
            {
                if (Path.GetExtension(sourceFileName.StringResult).Equals(".txt", StringComparison.CurrentCultureIgnoreCase))
                {
                    ed.WriteMessage("\nИмпорт текстового файла (Fixosoft2014, 2015,  2018)");
                    try
                    {
                        TEntitySpatial fteofile = new TEntitySpatial();
                        IO.TextReader TR = new IO.TextReader(sourceFileName.StringResult);
                        netFteo.IO.FileInfo fi = TR.ImportTxtFile(sourceFileName.StringResult);
                        ParseData(dm,ed,sourceFileName, fi.MyBlocks.ParsedSpatial);
                    }
                    catch (PlatformDb.Runtime.Exception ex)
                    {
                        ed.WriteMessage("\nError while importing coordinates: " + ex.Message);
                        //ed.WriteMessage("FileName" +sourceFileName.ToString());
                        ed.WriteMessage("FileName" + netFteo.StringUtils.ReplaceSlash(Path.GetFileNameWithoutExtension(sourceFileName.StringResult)));
                    }
                }
                else
                {
                    ed.WriteMessage("\nInput file must be in .#fteo format.");
                }
            }
        }
        #endregion

        #region //----------------------------------IMPORT CSV  ----------------------------//
        [CommandMethod("IMPORT_CSVFiles")]  // NumXYZDescr multiple
        public void ImportCSV()
        {
            // Get the manager object for all opened documents and then use it for getting the drawing database
            DocumentCollection dm = Platform.ApplicationServices.Application.DocumentManager;
            // Get the command line editor object
            Editor ed = dm.MdiActiveDocument.Editor;
            // An object that gets the result of user input
            PromptFileNameResult sourceFileName;

            // Print the prompt within the command line and get the result
            PromptOpenFileOptions PO = new PromptOpenFileOptions("Введите имя файла csv:");
            PO.Filter = "Текстовые файлы csv|*.csv";
            sourceFileName = ed.GetFileNameForOpen(PO);
            if (sourceFileName.Status == PromptStatus.OK)
            {
                if (Path.GetExtension(sourceFileName.StringResult).Equals(".csv", StringComparison.CurrentCultureIgnoreCase))
                {
                    ed.WriteMessage("\nИмпорт  CSV файла (Technocad)");
                    try
                    {
                        netFteo.Spatial.TEntitySpatial fteofile = new TEntitySpatial();
                        netFteo.IO.TextReader TR = new IO.TextReader(sourceFileName.StringResult);
                        fteofile = TR.ImportCSVFile();
                        ParseData(dm, ed, sourceFileName, fteofile);
                    }
                    catch (PlatformDb.Runtime.Exception ex)
                    {
                        ed.WriteMessage("\nError while importing coordinates: " + ex.Message);
                        //ed.WriteMessage("FileName" +sourceFileName.ToString());
                        ed.WriteMessage("FileName" + netFteo.StringUtils.ReplaceSlash(Path.GetFileNameWithoutExtension(sourceFileName.StringResult)));
                    }
                }
                else
                {
                    ed.WriteMessage("\nInput file must be in .csv (Technokad format.");
                }
            }
        }

        #endregion

        #region //----------------------------------IMPORT xml------------------------------
        [CommandMethod("IMPORT_xml")]
        public void ImportXML()
        {
            // Get the manager object for all opened documents and then use it for getting the drawing database
            DocumentCollection dm = Platform.ApplicationServices.Application.DocumentManager;
            // Get the command line editor object
            Editor ed = dm.MdiActiveDocument.Editor;
            // An object that gets the result of user input
            PromptFileNameResult sourceFileName;

            // Print the prompt within the command line and get the result
            PromptOpenFileOptions PO = new PromptOpenFileOptions("Введите имя текстового файла :");
            PO.Filter = "xml|*.xml";
            sourceFileName = ed.GetFileNameForOpen(PO);
            if (sourceFileName.Status == PromptStatus.OK)
            {
                if (Path.GetExtension(sourceFileName.StringResult).Equals(".xml", StringComparison.CurrentCultureIgnoreCase))
                {
                    ed.WriteMessage("\nИмпорт xml формата  (netfteo::BaseClasses)");
                    try
                    {
                        netFteo.Spatial.TEntitySpatial fteofile = new TEntitySpatial();
                        netFteo.IO.TextReader TR = new IO.TextReader(sourceFileName.StringResult);
                        fteofile = TR.ImportXML(sourceFileName.StringResult);
                        ParseData(dm, ed, sourceFileName, fteofile);
                    }
                    catch (PlatformDb.Runtime.Exception ex)
                    {
                        ed.WriteMessage("\nError while importing coordinates: " + ex.Message);
                        //ed.WriteMessage("FileName" +sourceFileName.ToString());
                        ed.WriteMessage("FileName" + netFteo.StringUtils.ReplaceSlash(Path.GetFileNameWithoutExtension(sourceFileName.StringResult)));
                    }
                }
                else
                {
                    ed.WriteMessage("\nInput file must be in .#xml format.");
                }
            }


        }


        #endregion

        #region //----------------------------------IMPORT mif------------------------------//Импорт mif
        [CommandMethod("IMPORT_mif")] 
        public void Import_mif()
        {
         // Get the manager object for all opened documents and then use it for getting the drawing database
            DocumentCollection dm = Platform.ApplicationServices.Application.DocumentManager;
            // Get the command line editor object
            Editor ed = dm.MdiActiveDocument.Editor;
            // An object that gets the result of user input
            PromptFileNameResult sourceFileName;

            // Print the prompt within the command line and get the result
            PromptOpenFileOptions PO = new PromptOpenFileOptions("Введите имя файла :");
            PO.Filter = "Файлы mif|*.mif";
            sourceFileName = ed.GetFileNameForOpen(PO);
            if (sourceFileName.Status == PromptStatus.OK)
            {
                if (Path.GetExtension(sourceFileName.StringResult).Equals(".mif", StringComparison.CurrentCultureIgnoreCase))
                {
                    ed.WriteMessage("\nИмпорт mif формата");
                    try
                    {
                        netFteo.Spatial.TEntitySpatial fteofile = new TEntitySpatial();
                        netFteo.IO.MIFReader TR = new IO.MIFReader(sourceFileName.StringResult);
                        fteofile = TR.ParseMIF();
                        ParseData(dm, ed, sourceFileName, fteofile);
                    }

                    catch (PlatformDb.Runtime.Exception ex)
                    {
                        ed.WriteMessage("\nError while importing coordinates: " + ex.Message);
                        //ed.WriteMessage("FileName" +sourceFileName.ToString());
                        ed.WriteMessage("FileName" + netFteo.StringUtils.ReplaceSlash(Path.GetFileNameWithoutExtension(sourceFileName.StringResult)));
                    }
                }
                else
                {
                    ed.WriteMessage("\nInput file must be in . format.");
                }
            }


        }

        #endregion

        #region  Teigha nCAD subroutines

        /// <summary>
/// Для внутреннего использования. Создает один полигон+ в слои записывает точки и имена
/// </summary>
/// <param name="ucsMatrix"></param>
/// <param name="btr"></param>
/// <param name="tr"></param>
/// <param name="polyline"></param>
/// <param name="ltPId">"id слоя с точками"</param>
/// <param name="ltId">id слоя с Текстами</param>
/// <param name="Layer">Сам слой netfteo</param>
/// <returns></returns>
        private ObjectId Create3dPolygon(Matrix3d ucsMatrix,BlockTableRecord btr, Transaction tr, ObjectId ltPolyid, ObjectId ltPId, ObjectId ltId, netFteo.Spatial.PointList Layer)
        {
            Polyline3d polyline = new Polyline3d();
            polyline.SetLayerId(ltPolyid, true);

            for (int i = 0; i <= Layer.Count - 1; i++)
            {
                DBPoint point = new DBPoint(new Point3d(Layer[i].y, Layer[i].x, Layer[i].z));
                point.SetLayerId(ltPId, true);

                DBText PointName = new DBText();
                PointName.TextString = Layer[i].Definition + "." + Layer[i].Description;
                PointName.Height = 2;
                PointName.SetLayerId(ltId, true);
                PointName.Position = new Point3d(Layer[i].y, Layer[i].x, Layer[i].z);

                PolylineVertex3d Vert = new PolylineVertex3d();
                Vert.Position = point.Position;
                polyline.AppendVertex(Vert);

                point.TransformBy(ucsMatrix.Inverse());
                PointName.TransformBy(ucsMatrix.Inverse());

                // Append the point to the database
                btr.AppendEntity(point);
                btr.AppendEntity(PointName);

                // Add the object to the transaction
                tr.AddNewlyCreatedDBObject(point, true);
                tr.AddNewlyCreatedDBObject(PointName, true);
                
            }
            polyline.TransformBy(ucsMatrix.Inverse());
          ObjectId Pline_id =  btr.AppendEntity(polyline);
            tr.AddNewlyCreatedDBObject(polyline, true);
            return Pline_id;
        }


        private ObjectId CreatePolygon(Matrix3d ucsMatrix, BlockTableRecord btr, Transaction tr, ObjectId ltPolyid, ObjectId ltPId, ObjectId ltId, netFteo.Spatial.PointList Layer)
        {
            Polyline polyline = new Polyline();
            Polyline polylineOld = new Polyline(); //  for old ordinates
            int oldlinePosition = 0;
            int newlinePosition = 0;
            polyline.SetLayerId(ltPolyid, true);
            polylineOld.SetLayerId(ltPolyid, true);
            double xx;
            double yy;

            for (int i = 0; i <= Layer.Count - 1; i++)
            {
                if (!Double.IsNaN(Layer[i].oldX))
                {
                    polylineOld.AddVertexAt(oldlinePosition++, new Point2d(Layer[i].oldY, Layer[i].oldX), 0, 0, 0);
                }


                if (!Double.IsNaN(Layer[i].y))
                {
                    yy = Layer[i].y;
                    xx = Layer[i].x;
                    polyline.AddVertexAt(newlinePosition++, new Point2d(yy, xx), 0, 0, 0);
                    DBPoint point = new DBPoint(new Point3d(yy, xx, 0));
                    point.SetLayerId(ltPId, true);
                    point.TransformBy(ucsMatrix.Inverse());
                    btr.AppendEntity(point);
                    tr.AddNewlyCreatedDBObject(point, true);
                }
                else  // accept old ord
                {
                    yy = Layer[i].oldY;
                    xx = Layer[i].oldX;
                }



                    DBText PointName = new DBText();
                    string Descriptor = Layer[i].Description;
                    if (Layer[i].Description == "-") Descriptor = "";
                    if (Layer[i].Description == "") Descriptor = "";
                    if (Layer[i].Description == null) Descriptor = "";
                    //if (Layer[i].NumGeopointA.Substring(0, 1) == ("н"))
                    // PointName.TextString = Layer[i].NumGeopointA + " " + Descriptor + "Новая!";                else 
                    PointName.TextString = Layer[i].Definition + " " + Descriptor;
                    PointName.Height = 2;
                    PointName.SetLayerId(ltId, true);
                    PointName.Position = new Point3d(yy, xx, 0);
                    PointName.TransformBy(ucsMatrix.Inverse());
                    // Append the point to the database

                    btr.AppendEntity(PointName);

                    // Add the object to the transaction
                    tr.AddNewlyCreatedDBObject(PointName, true);

            }

            polyline.TransformBy(ucsMatrix.Inverse());
            polylineOld.TransformBy(ucsMatrix.Inverse());
            ObjectId Pline_id = btr.AppendEntity(polyline);
                                btr.AppendEntity(polylineOld);
            tr.AddNewlyCreatedDBObject(polyline, true);
            tr.AddNewlyCreatedDBObject(polylineOld, true);
            return Pline_id;
        }
            //Создать отмывку границы
        private void MapsColor(Matrix3d ucsMatrix, BlockTableRecord btr, Transaction tr, ObjectId ltId, netFteo.Spatial.PointList Layer)
        {

            for (int i = 0; i <= Layer.Count - 2; i++)
            {
                // Полилиния будет  только по двум точкам
                Polyline polyline = new Polyline();
                polyline.SetLayerId(ltId, true);
                polyline.AddVertexAt(0, new Point2d(Layer[i].y, Layer[i].x), 0, 0, 0);
                polyline.AddVertexAt(1, new Point2d(Layer[i + 1].y, Layer[i + 1].x), 0, 0, 0);
                        polyline.Color = PlatformDb.Colors.Color.FromColor(System.Drawing.Color.Black); // default
                   if (Layer[i].Definition.Substring(0, 1) == ("н"))
                   {
                       polyline.Color = PlatformDb.Colors.Color.FromColor(System.Drawing.Color.Red);
                   }

                   if (Layer[i+1].Definition.Substring(0, 1) == ("н"))
                   {
                       polyline.Color = PlatformDb.Colors.Color.FromColor(System.Drawing.Color.Red);
                   }
                   
                polyline.TransformBy(ucsMatrix.Inverse());
                ObjectId Pline_id = btr.AppendEntity(polyline);
                tr.AddNewlyCreatedDBObject(polyline, true);

                //Из конца в начало :                
                /*
                Polyline polylineC = new Polyline();
                polylineC.SetLayerId(ltId, true);
                polylineC.AddVertexAt(i, new Point2d(Layer[Layer.Count-1].y, Layer[Layer.Count-1].x), 0, 0, 0);
                polylineC.AddVertexAt(i, new Point2d(Layer[0].y, Layer[0].x), 0, 0, 0);
                polylineC.Color = PlatformDb.Colors.Color.FromColor(System.Drawing.Color.Black); // default
                if (Layer[Layer.Count - 1].NumGeopointA.Substring(0, 1) == ("н"))
                {
                    polylineC.Color = PlatformDb.Colors.Color.FromColor(System.Drawing.Color.Red);
                }

                if (Layer[0].NumGeopointA.Substring(0, 1) == ("н"))
                {
                    polylineC.Color = PlatformDb.Colors.Color.FromColor(System.Drawing.Color.Red);
                }

                polylineC.TransformBy(ucsMatrix.Inverse());
                ObjectId PlineC_id = btr.AppendEntity(polylineC);
                tr.AddNewlyCreatedDBObject(polylineC, true);
                */
            }
        }

        #region Deprecated routines
        /*
                 private ObjectId Create2dPolygon(Matrix3d ucsMatrix, BlockTableRecord btr, Transaction tr, ObjectId ltPolyid, ObjectId ltPId, ObjectId ltId, netFteo.Spatial.PointList Layer)
        {
            Polyline2d polyline = new Polyline2d();
            
            polyline.SetLayerId(ltPolyid, true);

            for (int i = 0; i <= Layer.Count - 1; i++)
            {
                DBPoint point = new DBPoint(new Point3d(Layer[i].y, Layer[i].x, Layer[i].z));
                point.SetLayerId(ltPId, true);

                DBText PointName = new DBText();
                string Descriptor = Layer[i].Description;
                if (Layer[i].Description == "-") Descriptor = "";
                if (Layer[i].Description == "") Descriptor = "";
                if (Layer[i].Description == null) Descriptor = "";
                PointName.TextString = Layer[i].NumGeopointA;
                PointName.Height = 2;
                PointName.SetLayerId(ltId, true);
                PointName.Position = new Point3d(Layer[i].y, Layer[i].x, Layer[i].z);

                 Vertex2d Vert = new Vertex2d();
                 Vert.Position = point.Position;
                 polyline.AppendVertex(Vert);

                point.TransformBy(ucsMatrix.Inverse());
                PointName.TransformBy(ucsMatrix.Inverse());

                // Append the point to the database
                btr.AppendEntity(point);
                btr.AppendEntity(PointName);

                // Add the object to the transaction
                tr.AddNewlyCreatedDBObject(point, true);
                tr.AddNewlyCreatedDBObject(PointName, true);

            }
            polyline.TransformBy(ucsMatrix.Inverse());
            ObjectId Pline_id = btr.AppendEntity(polyline);
            tr.AddNewlyCreatedDBObject(polyline, true);
            return Pline_id;
        }
                private ObjectId Create3dPolygonFull(Matrix3d ucsMatrix, BlockTableRecord btr, Transaction tr, ObjectId ltPolyid, ObjectId ltPId, ObjectId ltId, netFteo.Spatial.TMyPolygon Layer)
                {

                    DBText PolygonName = new DBText();
                    PolygonName.TextString = Layer.Definition;
                    PolygonName.Height = 2;
                    PolygonName.SetLayerId(ltId, true);
                    PolygonName.Position = new Point3d(Layer.AverageCenter.y,Layer.AverageCenter.x,Layer.AverageCenter.z);
                    PolygonName.TransformBy(ucsMatrix.Inverse());
                    // Append the point to the database
                     btr.AppendEntity(PolygonName);
                    // Add the object to the transaction
                    tr.AddNewlyCreatedDBObject(PolygonName, true);

                   ObjectId res = Create3dPolygon(ucsMatrix, btr, tr, ltPolyid, ltPId, ltId, Layer);
                    for (int ic = 0; ic <= Layer.Childs.Count - 1; ic++)
                    {
                      Create3dPolygon(ucsMatrix, btr, tr, ltPolyid, ltPId, ltId, Layer.Childs[ic]);
                    }

                    return res;
                }
                private ObjectId Create2dPolygonFull(Matrix3d ucsMatrix, BlockTableRecord btr, Transaction tr, ObjectId ltPolyid, ObjectId ltPId, ObjectId ltId, netFteo.Spatial.TMyPolygon Layer)
                {

                    DBText PolygonName = new DBText();
                    PolygonName.TextString = Layer.Definition;
                    PolygonName.Height = 2;
                    PolygonName.SetLayerId(ltId, true);
                    PolygonName.Position = new Point3d(Math.Abs(Layer.AverageCenter.y), Math.Abs(Layer.AverageCenter.x), Layer.AverageCenter.z);
                    PolygonName.TransformBy(ucsMatrix.Inverse());
                    // Append the point to the database
                    btr.AppendEntity(PolygonName);
                    // Add the object to the transaction
                    tr.AddNewlyCreatedDBObject(PolygonName, true);

                    ObjectId res = Create2dPolygon(ucsMatrix, btr, tr, ltPolyid, ltPId, ltId, Layer);
                    for (int ic = 0; ic <= Layer.Childs.Count - 1; ic++)
                    {
                        Create2dPolygon(ucsMatrix, btr, tr, ltPolyid, ltPId, ltId, Layer.Childs[ic]);
                    }

                    return res;
                }
                 */
        #endregion


        //Главная процедура генерации
        private ObjectId CreatePolygonFull(Matrix3d ucsMatrix, BlockTableRecord btr, 
                                                                Transaction tr, 
                                                           ObjectId ltPolyid, // id слоя с полигоном
                                                           ObjectId ltPId, 
                                                           ObjectId ltId,
                                                           ObjectId ltCircleId, ObjectId ltColorsId, netFteo.Spatial.IGeometry Layerfeature)
        {
			TMyPolygon Layer = (TMyPolygon)Layerfeature;
            DBText PolygonName = new DBText();
            PolygonName.TextString = Layer.Definition;
            PolygonName.Height = 2;
            PolygonName.SetLayerId(ltPolyid, true); // Обозначение полигона в слой "Полигоны"
            PolygonName.Position = new Point3d(Math.Abs(Layer.AverageCenter.y), Math.Abs(Layer.AverageCenter.x), 0);
            PolygonName.TransformBy(ucsMatrix.Inverse());
            // Append the point to the database
            btr.AppendEntity(PolygonName);
            // Add the object to the transaction
            tr.AddNewlyCreatedDBObject(PolygonName, true);

            ObjectId res = CreatePolygon(ucsMatrix, btr, tr, ltPolyid, ltPId, ltId, Layer);
                           CreateCircle(ucsMatrix, btr, tr, ltCircleId, Layer);
                           MapsColor(ucsMatrix, btr, tr, ltColorsId, Layer);
            //childs
            for (int ic = 0; ic <= Layer.Childs.Count - 1; ic++)
            {
                CreatePolygon(ucsMatrix, btr, tr, ltPolyid, ltPId, ltId, Layer.Childs[ic]);
                 CreateCircle(ucsMatrix, btr, tr, ltCircleId, Layer.Childs[ic]);
                 MapsColor(ucsMatrix, btr, tr, ltColorsId, Layer.Childs[ic]);
            }

            return res;
        }


        private void CreateCircle(Matrix3d ucsMatrix, BlockTableRecord btr, Transaction tr, ObjectId ltCircId, netFteo.Spatial.PointList Layer)
        {
            for (int i = 0; i <= Layer.Count - 1; i++)
            {
               Point3d center = new Point3d(Layer[i].y, Layer[i].x, 0);
               Vector3d normal = new Vector3d(0.0, 0.0, 1.0);
                Circle circle = new Circle(center, normal, 0.75);
                circle.SetLayerId(ltCircId, true);
                circle.TransformBy(ucsMatrix.Inverse());
                // Append the point to the database
                ObjectId CircleID = new ObjectId();
                CircleID = btr.AppendEntity(circle);
                // Add the object to the transaction
                tr.AddNewlyCreatedDBObject(circle, true);
                ObjectIdCollection ObjIds = new ObjectIdCollection();
                ObjIds.Add(CircleID);
                Hatch oHatch = new Hatch();
                      oHatch.Normal = normal;
                      oHatch.Elevation = 0.0;
                      oHatch.PatternScale = 2.0;
                      oHatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID"); // also ZIGZAG
                      oHatch.ColorIndex = 0;
                      oHatch.SetLayerId(ltCircId, true);
                      oHatch.TransformBy(ucsMatrix.Inverse());
                       btr.AppendEntity(oHatch);
                       tr.AddNewlyCreatedDBObject(oHatch, true);
                       //this works ok  
                      oHatch.Associative = true;
                      oHatch.AppendLoop((int)HatchLoopTypes.Default, ObjIds);
                      oHatch.EvaluateHatch(true);
            }
        }
            /// <summary>
            /// Функция установки стилей отображения точек
            /// </summary>
            /// <param name="tr" remarks="Транзакция"></param>
        private void SetPoinStyles(int PointType, int PointSizing)
        {
            HostMgd.ApplicationServices.Application.SetSystemVariable("PDMODE", PointType); // тип точки 35 кружок с рожками
            HostMgd.ApplicationServices.Application.SetSystemVariable("PDSIZE", PointSizing); // Values for PDSIZE variable:
            
        }
        #endregion


        private string AssemblyVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

		private void LogStarttoWebServer(string AppTypeName)
		{
			string AppConfiguration;
#if (DEBUG)
			AppConfiguration = "DEBUG";
#else
			AppConfiguration = "";
#endif
			IO.LogServer srv = new IO.LogServer("82.119.136.82",
				new IO.LogServer_response()
				{
					ApplicationType = AppTypeName + " " + AppConfiguration,
					AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),

					Client = NetWork.NetWrapper.UserName
				});

			//	srv.Get_WebOnline_th("");
		}



	}
}
 

