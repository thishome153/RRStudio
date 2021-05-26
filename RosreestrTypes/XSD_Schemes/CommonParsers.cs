using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using netFteo.Spatial;
using netFteo.Cadaster;
using netFteo.Rosreestr;
using System.Collections.Generic;
using RRTypes.CommonCast;

namespace RRTypes.CommonCast
{
    /// <summary>
    /// Общий класс утилит для кастинга разных версий схем ОКС
    /// </summary>
    public static class CasterOKS
    {
        /// <summary>
        /// ES manipulation routines
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>


        public static TEntitySpatial ES_OKS2(string Definition, STD_TPV02.Entity_Spatial ES)
        {
            TEntitySpatial res = new TEntitySpatial();
            if (ES == null) return res;

            for (int i = 0; i <= ES.Spatial_Element.Count - 1; i++)
            {
                if (ES.Spatial_Element[i].Spelement_Unit[0].Ordinate[0].X == ES.Spatial_Element[i].Spelement_Unit[ES.Spatial_Element[i].Spelement_Unit.Count() - 1].Ordinate[0].X)
                {
                    TPolygon Polygon = new TPolygon();
                    Polygon.Definition = ES.Spatial_Element[i].Number;
                    //OUT ring
                    for (int ip = 0; ip <= ES.Spatial_Element[i].Spelement_Unit.Count - 1; ip++)
                    {
                        TPoint P = new TPoint();
                        P.x = Convert.ToDouble(ES.Spatial_Element[i].Spelement_Unit[ip].Ordinate[0].X);
                        P.y = Convert.ToDouble(ES.Spatial_Element[i].Spelement_Unit[ip].Ordinate[0].Y);
                        P.NumGeopointA = ES.Spatial_Element[i].Spelement_Unit[ip].Ordinate[0].Num_Geopoint;
                        P.Mt = Convert.ToDouble(ES.Spatial_Element[i].Spelement_Unit[ip].Ordinate[0].Delta_Geopoint);
                        Polygon.AddPoint(P);
                    }


                    //childs inner ring
                    for (int ii = 1; ii <= ES.Spatial_Element.Count - 1; ii++)
                    {
                        TRing ESch = Polygon.AddChild();

                        for (int ip = 0; ip <= ES.Spatial_Element[i].Spelement_Unit.Count - 1; ip++)
                        {
                            netFteo.Spatial.TPoint P = new netFteo.Spatial.TPoint();
                            P.x = Convert.ToDouble(ES.Spatial_Element[i].Spelement_Unit[ip].Ordinate[0].X);
                            P.y = Convert.ToDouble(ES.Spatial_Element[i].Spelement_Unit[ip].Ordinate[0].Y);
                            P.NumGeopointA = ES.Spatial_Element[i].Spelement_Unit[ip].Ordinate[0].Num_Geopoint;
                            P.Mt = Convert.ToDouble(ES.Spatial_Element[i].Spelement_Unit[ip].Ordinate[0].Delta_Geopoint);
                            ESch.AddPoint(P);
                        }
                    }
                    res.Add(Polygon);
                }
            }
            return res;
        }

        public static TEntitySpatial ES_OKS2(string Definition, V03_TP.tEntitySpatialOKSInp ES)
        {
            TEntitySpatial res = new TEntitySpatial();
            if (ES == null) return res;

            for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
            {

                if (ES.SpatialElement[i].SpelementUnit[0].TypeUnit == V03_TP.tSpelementUnitOKSInpTypeUnit.Окружность)
                {
                    TCircle fES = new TCircle(ES.SpatialElement[i].SpelementUnit[0].Ordinate.X,
                                              ES.SpatialElement[i].SpelementUnit[0].Ordinate.Y,
                                              ES.SpatialElement[i].SpelementUnit[0].R);
                    fES.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[0].Ordinate.DeltaGeopoint);
                    fES.NumGeopointA = ES.SpatialElement[i].Number;//.SpelementUnit[0].Ordinate.NumGeopoint;
                    fES.Definition = ES.SpatialElement[i].Number;
                    res.Add(fES);
                }

                if (ES.SpatialElement[i].SpelementUnit[0].TypeUnit == V03_TP.tSpelementUnitOKSInpTypeUnit.Точка)
                {
                    if (ES.SpatialElement[i].SpelementUnit[0].Ordinate.X == ES.SpatialElement[i].SpelementUnit[ES.SpatialElement[i].SpelementUnit.Count() - 1].Ordinate.X)
                    {
                        //may be already included as some child ??? used prdeicate (lambda ops):
                        if (!res.Exists(predicate_lambda_stuff => predicate_lambda_stuff.Definition == ES.SpatialElement[i].Number))
                        {
                            TPolygon Polygon = new TPolygon();
                            Polygon.Definition = ES.SpatialElement[i].Number;
                            //OUT ring
                            for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                            {
                                TPoint P = new TPoint();
                                P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                                P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                                P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                                P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                                Polygon.AddPoint(P);
                            }

                            // find other elements with same Number - childs
                            //childs inner ring
                            //TODO: howto detect childs rings ???

                            for (int ii = 0; ii <= ES.SpatialElement.Count - 1; ii++)
                            {
                                if ((ES.SpatialElement[ii].Number == Polygon.Definition) &&  //find childs
                                    (i != ii))
                                {
                                    TRing ESch = Polygon.AddChild();
                                    for (int ip = 0; ip <= ES.SpatialElement[ii].SpelementUnit.Count - 1; ip++)
                                    {
                                        TPoint P = new TPoint();
                                        P.x = Convert.ToDouble(ES.SpatialElement[ii].SpelementUnit[ip].Ordinate.X);
                                        P.y = Convert.ToDouble(ES.SpatialElement[ii].SpelementUnit[ip].Ordinate.Y);
                                        P.NumGeopointA = ES.SpatialElement[ii].SpelementUnit[ip].Ordinate.NumGeopoint;
                                        P.Mt = Convert.ToDouble(ES.SpatialElement[ii].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                                        ESch.AddPoint(P);
                                    }
                                }
                            }
                            res.Add(Polygon);
                        }
                    }
                    else
                    {   //unclosed line - polyline
                        TPolyLine line = new TPolyLine();
                        line.Definition = ES.SpatialElement[i].Number;
                        for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                        {
                            TPoint P = line.AddPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                       Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                            P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                            P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        }
                        res.Add(line);
                    }
                }
            }
            return res;
        }

        public static TEntitySpatial ES_OKS2(string Definition, kpt09.tEntitySpatialOKSOut ES)
        {
            TEntitySpatial res = new TEntitySpatial();
            if (ES == null) return res;

            for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
            {

                //if (ES.SpatialElement[i].SpelementUnit[0].TypeUnit == kpt09.tSpelementUnitZUOutTypeUnit.Окружность)
                if ((ES.SpatialElement[i].SpelementUnit.Count == 1) &&
                    (ES.SpatialElement[i].SpelementUnit[0].RSpecified))
                {
                    TCircle fES = new TCircle(ES.SpatialElement[i].SpelementUnit[0].Ordinate.X,
                                              ES.SpatialElement[i].SpelementUnit[0].Ordinate.Y,
                                              ES.SpatialElement[i].SpelementUnit[0].R);
                    fES.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[0].Ordinate.DeltaGeopoint);
                    fES.NumGeopointA = ES.SpatialElement[i].Number;//.SpelementUnit[0].Ordinate.NumGeopoint;
                    fES.Definition = Definition;
                    res.Add(fES);
                    goto NextElement; // skip to next, as here circle element
                }

                if (ES.SpatialElement[i].SpelementUnit[0].TypeUnit == kpt09.tSpelementUnitZUOutTypeUnit.Точка)
                {
                    if (ES.SpatialElement[i].SpelementUnit[0].Ordinate.X == ES.SpatialElement[i].SpelementUnit[ES.SpatialElement[i].SpelementUnit.Count() - 1].Ordinate.X)
                    {
                        //may be already included as some child ??? used prdeicate (lambda ops):
                        //if (!res.Exists(predicate_lambda_stuff => predicate_lambda_stuff.Definition == ES.SpatialElement[i].Number))
                        {
                            TPolygon Polygon = new TPolygon();
                            Polygon.Definition = Definition;
                            //OUT ring
                            for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                            {
                                TPoint P = new TPoint();
                                P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                                P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                                P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                                P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                                Polygon.AddPoint(P);
                            }

                            // find other elements with same Number - childs !!! not specified NAME!!!
                            //childs inner ring
                            //TODO: howto detect childs rings ???
                            /*
							for (int ii = 0; ii <= ES.SpatialElement.Count - 1; ii++)
							{
								if ((ES.SpatialElement[ii].Number == Polygon.Definition) &&  //find childs
									(i != ii))
								{
									TMyOutLayer ESch = Polygon.AddChild();
									for (int ip = 0; ip <= ES.SpatialElement[ii].SpelementUnit.Count - 1; ip++)
									{
										Point P = new Point();
										P.x = Convert.ToDouble(ES.SpatialElement[ii].SpelementUnit[ip].Ordinate.X);
										P.y = Convert.ToDouble(ES.SpatialElement[ii].SpelementUnit[ip].Ordinate.Y);
										P.NumGeopointA = ES.SpatialElement[ii].SpelementUnit[ip].Ordinate.NumGeopoint;
										P.Mt = Convert.ToDouble(ES.SpatialElement[ii].SpelementUnit[ip].Ordinate.DeltaGeopoint);
										ESch.AddPoint(P);
									}
								}
							} */
                            res.Add(Polygon);
                        }
                    }
                    else
                    {   //unclosed line - polyline
                        TPolyLine line = new TPolyLine();
                        line.Definition = Definition;
                        line.id = Gen_id.newId;
                        for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                        {
                            TPoint P = line.AddPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                       Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                            P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                            P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        }
                        res.Add(line);
                    }
                }
            NextElement:;
            }
            return res;
        }

        public static TEntitySpatial ES_OKS2(string Definition, kpt10_un.tEntitySpatialOKSOut ES)
        {
            TEntitySpatial res = new TEntitySpatial();
            if (ES == null) return res;
            for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
            {

                if (ES.SpatialElement[i].SpelementUnit[0].TypeUnit == kpt10_un.tSpelementUnitZUOutTypeUnit.Окружность)
                {
                    TCircle fES = new TCircle(ES.SpatialElement[i].SpelementUnit[0].Ordinate.X,
                                              ES.SpatialElement[i].SpelementUnit[0].Ordinate.Y,
                                              ES.SpatialElement[i].SpelementUnit[0].R);
                    fES.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[0].Ordinate.DeltaGeopoint);
                    fES.NumGeopointA = Definition + "." + ES.SpatialElement[i].Number;//.SpelementUnit[0].Ordinate.NumGeopoint;
                    fES.Definition = Definition;
                    res.Add(fES);
                }

                if (ES.SpatialElement[i].SpelementUnit[0].TypeUnit == kpt10_un.tSpelementUnitZUOutTypeUnit.Точка)
                {
                    if (ES.SpatialElement[i].SpelementUnit[0].Ordinate.X == ES.SpatialElement[i].SpelementUnit[ES.SpatialElement[i].SpelementUnit.Count() - 1].Ordinate.X)
                    {
                        //may be already included as some child ??? used prdeicate (lambda ops):
                        if (!res.Exists(predicate_lambda_stuff => predicate_lambda_stuff.Definition == ES.SpatialElement[i].Number))
                        {
                            TPolygon Polygon = new TPolygon();
                            Polygon.Definition = Definition + "." + ES.SpatialElement[i].Number;
                            Polygon.LayerHandle = res.Layers[0].LayerHandle; //  "FFFF"; //default
                                                                             //OUT ring
                            for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                            {
                                TPoint P = new TPoint();
                                P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                                P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                                P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                                P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                                Polygon.AddPoint(P);
                            }


                            //childs inner ring
                            for (int ii = 1; ii <= ES.SpatialElement.Count - 1; ii++)
                            {
                                if ((ES.SpatialElement[ii].Number == Polygon.Definition) &&  //find childs
                                    (i != ii))
                                {
                                    TRing ESch = Polygon.AddChild();

                                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                                    {
                                        netFteo.Spatial.TPoint P = new netFteo.Spatial.TPoint();
                                        P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                                        P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                                        ESch.AddPoint(P);
                                    }
                                }
                            }
                            res.Add(Polygon);
                        }
                    }
                    else
                    {   //unclosed line - polyline
                        TPolyLine line = new TPolyLine();
                        line.Definition = Definition + "." + ES.SpatialElement[i].Number;
                        line.id = Gen_id.newId;
                        line.LayerHandle = res.Layers[0].LayerHandle; //  "FFFF"; //default
                        for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                        {
                            TPoint P = line.AddPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                       Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                            P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                            P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        }
                        res.Add(line);
                    }
                }
            }
            return res;
        }

        public static TEntitySpatial ES_OKS2(string Definition, kpoks_v03.tEntitySpatialOKSOut ES)
        {
            TEntitySpatial res = new TEntitySpatial();
            if (ES == null) return res;
            for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
            {

                if (ES.SpatialElement[i].SpelementUnit[0].TypeUnit == kpoks_v03.tSpelementUnitOKSOutTypeUnit.Окружность)
                {
                    TCircle fES = new TCircle(ES.SpatialElement[i].SpelementUnit[0].Ordinate.X,
                                              ES.SpatialElement[i].SpelementUnit[0].Ordinate.Y,
                                              ES.SpatialElement[i].SpelementUnit[0].R);
                    fES.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[0].Ordinate.DeltaGeopoint);
                    fES.NumGeopointA = ES.SpatialElement[i].Number;//.SpelementUnit[0].Ordinate.NumGeopoint;
                    fES.Definition = Definition;
                    res.Add(fES);
                }

                if (ES.SpatialElement[i].SpelementUnit[0].TypeUnit == kpoks_v03.tSpelementUnitOKSOutTypeUnit.Точка)
                {
                    if (ES.SpatialElement[i].SpelementUnit[0].Ordinate.X == ES.SpatialElement[i].SpelementUnit[ES.SpatialElement[i].SpelementUnit.Count() - 1].Ordinate.X)
                    {
                        TPolygon Polygon = new TPolygon();
                        Polygon.Definition = Definition;
                        //OUT ring
                        for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                        {
                            TPoint P = new TPoint();
                            P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                            P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                            P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                            P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                            Polygon.AddPoint(P);
                        }


                        //childs inner ring
                        for (int ii = 1; ii <= ES.SpatialElement.Count - 1; ii++)
                        {
                            TRing ESch = Polygon.AddChild();

                            for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                            {
                                netFteo.Spatial.TPoint P = new netFteo.Spatial.TPoint();
                                P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                                P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                                P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                                P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                                ESch.AddPoint(P);
                            }
                        }
                        res.Add(Polygon);
                    }
                    else
                    {   //unclosed line - polyline
                        TPolyLine line = new TPolyLine();
                        line.Definition = Definition;
                        line.id = Gen_id.newId;
                        for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                        {
                            TPoint P = line.AddPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                       Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                            P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                            P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        }
                        res.Add(line);
                    }
                }
            }
            return res;
        }

        public static TEntitySpatial ES_OKS2(string Definition, kpoks_v04.tEntitySpatialOKSOut ES)
        {
            TEntitySpatial res = new TEntitySpatial();
            if (ES == null) return res;
            for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
            {

                if (ES.SpatialElement[i].SpelementUnit[0].TypeUnit == kpoks_v04.tSpelementUnitZUOutTypeUnit.Окружность)
                {
                    TCircle fES = new TCircle(ES.SpatialElement[i].SpelementUnit[0].Ordinate.X,
                                              ES.SpatialElement[i].SpelementUnit[0].Ordinate.Y,
                                              ES.SpatialElement[i].SpelementUnit[0].R);
                    fES.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[0].Ordinate.DeltaGeopoint);
                    fES.NumGeopointA = ES.SpatialElement[i].Number;//.SpelementUnit[0].Ordinate.NumGeopoint;
                    fES.Definition = Definition;
                    res.Add(fES);
                }

                if (ES.SpatialElement[i].SpelementUnit[0].TypeUnit == kpoks_v04.tSpelementUnitZUOutTypeUnit.Точка)
                {
                    if (ES.SpatialElement[i].SpelementUnit[0].Ordinate.X == ES.SpatialElement[i].SpelementUnit[ES.SpatialElement[i].SpelementUnit.Count() - 1].Ordinate.X)
                    {
                        TPolygon Polygon = new TPolygon();
                        Polygon.Definition = Definition;
                        //OUT ring
                        for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                        {
                            TPoint P = new TPoint();
                            P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                            P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                            P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                            P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                            Polygon.AddPoint(P);
                        }


                        //childs inner ring
                        for (int ii = 1; ii <= ES.SpatialElement.Count - 1; ii++)
                        {
                            TRing ESch = Polygon.AddChild();

                            for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                            {
                                netFteo.Spatial.TPoint P = new netFteo.Spatial.TPoint();
                                P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                                P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                                P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                                P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                                ESch.AddPoint(P);
                            }
                        }
                        res.Add(Polygon);
                    }
                    else
                    {   //unclosed line - polyline
                        TPolyLine line = new TPolyLine();
                        line.Definition = Definition;
                        line.id = Gen_id.newId;
                        for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                        {
                            TPoint P = line.AddPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                       Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                            P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                            P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        }
                        res.Add(line);
                    }
                }
            }
            return res;
        }

        public static TEntitySpatial ES_OKS2(string Definition, kvoks_v02.tEntitySpatialOKSOut ES)
        {
            TEntitySpatial res = new TEntitySpatial();
            if (ES == null) return res;
            for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
            {

                if (ES.SpatialElement[i].SpelementUnit[0].TypeUnit == kvoks_v02.tSpelementUnitOKSOutTypeUnit.Окружность)
                {
                    TCircle fES = new TCircle(ES.SpatialElement[i].SpelementUnit[0].Ordinate.X,
                                              ES.SpatialElement[i].SpelementUnit[0].Ordinate.Y,
                                              ES.SpatialElement[i].SpelementUnit[0].R);
                    fES.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[0].Ordinate.DeltaGeopoint);
                    fES.NumGeopointA = Definition;//.SpelementUnit[0].Ordinate.NumGeopoint;
                    fES.Definition = Definition;
                    res.Add(fES);
                }

                if (ES.SpatialElement[i].SpelementUnit[0].TypeUnit == kvoks_v02.tSpelementUnitOKSOutTypeUnit.Точка)
                {
                    if (ES.SpatialElement[i].SpelementUnit[0].Ordinate.X == ES.SpatialElement[i].SpelementUnit[ES.SpatialElement[i].SpelementUnit.Count() - 1].Ordinate.X)
                    {
                        TPolygon Polygon = new TPolygon();
                        Polygon.Definition = Definition;
                        //OUT ring
                        for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                        {
                            TPoint P = new TPoint();
                            P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                            P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                            P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                            P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                            Polygon.AddPoint(P);
                        }


                        //childs inner ring
                        for (int ii = 1; ii <= ES.SpatialElement.Count - 1; ii++)
                        {
                            TRing ESch = Polygon.AddChild();

                            for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                            {
                                netFteo.Spatial.TPoint P = new TPoint();
                                P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                                P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                                P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                                P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                                ESch.AddPoint(P);
                            }
                        }
                        res.Add(Polygon);
                    }
                    else
                    {   //unclosed line - polyline
                        TPolyLine line = new TPolyLine();
                        line.Definition = Definition;
                        line.id = Gen_id.newId;
                        for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                        {
                            TPoint P = line.AddPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                       Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                            P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                            P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        }
                        res.Add(line);
                    }
                }
            }
            return res;
        }

        public static TEntitySpatial ES_OKS2(string Definition, kvoks_v07.tEntitySpatialOKSOut ES)
        {
            TEntitySpatial res = new TEntitySpatial();
            if (ES == null) return res;
            for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
            {

                if (ES.SpatialElement[i].SpelementUnit[0].TypeUnit == kvoks_v07.tSpelementUnitZUOutTypeUnit.Окружность)
                {
                    TCircle fES = new TCircle(ES.SpatialElement[i].SpelementUnit[0].Ordinate.X,
                                              ES.SpatialElement[i].SpelementUnit[0].Ordinate.Y,
                                              ES.SpatialElement[i].SpelementUnit[0].R);
                    fES.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[0].Ordinate.DeltaGeopoint);
                    fES.NumGeopointA = ES.SpatialElement[i].Number;//.SpelementUnit[0].Ordinate.NumGeopoint;
                    fES.Definition = Definition;
                    res.Add(fES);
                }

                if (ES.SpatialElement[i].SpelementUnit[0].TypeUnit == kvoks_v07.tSpelementUnitZUOutTypeUnit.Точка)
                {
                    if (ES.SpatialElement[i].SpelementUnit[0].Ordinate.X == ES.SpatialElement[i].SpelementUnit[ES.SpatialElement[i].SpelementUnit.Count() - 1].Ordinate.X)
                    {
                        TPolygon Polygon = new TPolygon();
                        Polygon.Definition = Definition;

                        //OUT ring
                        for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                        {
                            TPoint P = new TPoint();
                            P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                            P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                            P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                            P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                            Polygon.AddPoint(P);
                        }


                        //childs (inner) ring //Full SHIT 
                        //TODO:  howto detect childs ???
                        // if next ring incoming in current ???
                        for (int ii = 0; ii <= ES.SpatialElement.Count - 1; ii++)
                        {
                            if (ii != i) // not self
                            {
                                TRing TestRing = new TRing();
                                for (int ip = 0; ip <= ES.SpatialElement[ii].SpelementUnit.Count - 1; ip++)
                                {
                                    netFteo.Spatial.TPoint P = new netFteo.Spatial.TPoint();
                                    P.x = Convert.ToDouble(ES.SpatialElement[ii].SpelementUnit[ip].Ordinate.X);
                                    P.y = Convert.ToDouble(ES.SpatialElement[ii].SpelementUnit[ip].Ordinate.Y);
                                    P.NumGeopointA = ES.SpatialElement[ii].SpelementUnit[ip].Ordinate.NumGeopoint;
                                    P.Mt = Convert.ToDouble(ES.SpatialElement[ii].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                                    TestRing.AddPoint(P);
                                }

                                Polygon.AddChild(TestRing);
                            }
                        }
                        res.Add(Polygon);
                    }
                    else
                    {   //unclosed line - polyline
                        TPolyLine line = new TPolyLine();
                        line.Definition = Definition;
                        line.id = Gen_id.newId;
                        for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                        {
                            TPoint P = line.AddPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                       Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                            P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                            P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        }
                        res.Add(line);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Adress manipulation routines
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public static TAddress CastAddress(kvoks_v07.tAddressOut Address)
        {
            if (Address == null) return null;
            TAddress Adr = new TAddress();
            Adr.KLADR = Address.KLADR;
            Adr.Note = Address.Note;
            // dRegionsRF:  
            // Namespace = "urn://x-artefacts-rosreestr-ru/commons/directories/regions/1.0.1"
            // Here we will use netFteo.XML.XSDEnumFile
            // 1. Need placement of xsd file
            // 2. Get Namespace
            //netFteo.XML.XSDEnumFile xsdenum = new netFteo.XML.XSDEnumFile(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "\\Schema\\SchemaCommon\\dRegionsRF_v01.xsd");
            //List<string> enumAnnot = xsdenum.Item2Annotation(Address.Region, xsdenum.SimpleTypeNames.First());
            //Adr.Region = Address.Region.GetType().ToString();

            if (Address.District != null)
                Adr.District = Address.District.Type + " " + Address.District.Name;
            if (Address.Locality != null)
                Adr.Locality = Address.Locality.Type + " " + Address.Locality.Name;
            if (Address.Street != null)
                Adr.Street = Address.Street.Type + " " + Address.Street.Name;
            if (Address.Level1 != null)
                Adr.Level1 = Address.Level1.Type + " " + Address.Level1.Value;
            if (Address.Apartment != null)
                Adr.Apartment = Address.Apartment.Type + " " + Address.Apartment.Value;

            Adr.Region = Address.Region.ToString().Substring(4);

            return Adr;
        }
        public static TAddress CastAddress(kpoks_v03.tAddressOut Address)
        {
            if (Address == null) return null;
            TAddress Adr = new TAddress();
            Adr.KLADR = Address.KLADR;
            Adr.Note = Address.Note;
            if (Address.District != null)
                Adr.District = Address.District.Type + " " + Address.District.Name;
            if (Address.Locality != null)
                Adr.Locality = Address.Locality.Type + " " + Address.Locality.Name;
            if (Address.Street != null)
                Adr.Street = Address.Street.Type + " " + Address.Street.Name;
            if (Address.Level1 != null)
                Adr.Level1 = Address.Level1.Type + " " + Address.Level1.Value;
            Adr.Region = Address.Region.ToString();

            return Adr;
        }
        public static TAddress CastAddress(kpoks_v04.tAddressOut Address)
        {
            if (Address == null) return null;
            TAddress Adr = new TAddress();
            Adr.KLADR = Address.KLADR;
            Adr.Note = Address.Note;
            Adr.Other = Address.Other;
            if (Address.District != null)
                Adr.District = Address.District.Type + " " + Address.District.Name;
            if (Address.Locality != null)
                Adr.Locality = Address.Locality.Type + " " + Address.Locality.Name;
            if (Address.City != null)
                Adr.City = Address.City.Type + " " + Address.City.Name;
            if (Address.Street != null)
                Adr.Street = Address.Street.Type + " " + Address.Street.Name;
            if (Address.Level1 != null)
                Adr.Level1 = Address.Level1.Type + " " + Address.Level1.Value;
            if (Address.Apartment != null)
                Adr.Apartment = Address.Apartment.Type + " " + Address.Apartment.Value;

            Adr.Region = Address.Region.ToString().Substring(4);

            return Adr;
        }

        public static TAddress CastAddress(V03_TP.tAddressInpFull Address)
        {
            if (Address == null) return null;
            TAddress Adr = new TAddress();
            Adr.KLADR = Address.KLADR;
            if (Address.Other != null) Adr.Other = Address.Other;
            if (Address.Note != null) Adr.Note = Address.Note;
            if (Address.City != null)
                Adr.City = Address.City.Type + " " + Address.City.Name;
            if (Address.District != null)
                Adr.District = Address.District.Type + " " + Address.District.Name;
            if (Address.Locality != null)
                Adr.Locality = Address.Locality.Type + " " + Address.Locality.Name;
            if (Address.Street != null)
                Adr.Street = Address.Street.Type + " " + Address.Street.Name;
            if (Address.Level1 != null)
                Adr.Level1 = Address.Level1.Type + " " + Address.Level1.Value;
            if (Address.Level2 != null)
                Adr.Level2 = Address.Level2.Type + " " + Address.Level2.Value;

            if (Address.Level3 != null)
                Adr.Level2 = Address.Level3.Type + " " + Address.Level3.Value;

            if (Address.Apartment != null)
                Adr.Apartment = Address.Apartment.Type + " " + Address.Apartment.Value;

            Adr.Region = Address.Region.ToString().Substring(4);

            return Adr;
        }

        public static string ObjectTypeToStr(string s)
        {
            //002001002000
            if (!s.Contains("Item")) s = "Item" + s;
            if (s == "Item002001002000") return "Здание";
            if (s == "Item002001003000") return "Помещение";
            if (s == "Item002001004000") return "Сооружение";
            if (s == "Item002001005000") return "Объект незавершённого строительства";
            return null;
        }
        public static string ObjectTypeToStr(kpt09.tBuildingObjectType OT)
        {
            return ObjectTypeToStr(OT.ToString());
        }
        public static string ObjectTypeToStr(kpoks_v03.tBuildingObjectType OT)
        {
            return ObjectTypeToStr(OT.ToString());
        }
        public static string ObjectTypeToStr(kpoks_v04.tBuildingObjectType OT)
        {
            return ObjectTypeToStr(OT.ToString());
        }
        public static string ObjectTypeToStr(kvoks_v07.tBuildingObjectType OT)
        {
            return ObjectTypeToStr(OT.ToString());
        }
        public static string ObjectTypeToStr(kpt10_un.tBuildingObjectType OT)
        {
            return ObjectTypeToStr(OT.ToString());
        }

    }

    /// <summary>
    /// Общий класс утилит для кастинга ЕГРП (/ReestrExtract) - также и "прикрученных к KVOKS/KVZU.
    /// Читаем xml без XSD, через  XPath.
    /// </summary>
    public static class CasterEGRP
    {

        public static netFteo.Rosreestr.TMyRights ParseKPSOKSRights(System.Xml.XmlDocument xmldoc)
        {
            // Add the namespace.
            System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
            nsmgr.AddNamespace("kpoks", xmldoc.DocumentElement.NamespaceURI);

            System.Xml.XmlNodeList lst = xmldoc.DocumentElement.SelectNodes("/kpoks:KPOKS/kpoks:Realty/kpoks:Flat/kpoks:Rights/kpoks:Right", nsmgr);

            netFteo.Rosreestr.TMyRights rs = new netFteo.Rosreestr.TMyRights();

            foreach (System.Xml.XmlNode cd in lst)
            {

                netFteo.Rosreestr.TRight rt = new netFteo.Rosreestr.TRight();
                rt.Name = cd.SelectSingleNode("kpoks:Name", nsmgr).FirstChild.Value;


                System.Xml.XmlNodeList ownlst = cd.SelectNodes("kpoks:Owners/kpoks:Owner", nsmgr);

                foreach (System.Xml.XmlNode ownitem in ownlst)
                {
                    netFteo.Rosreestr.TMyOwner own = new netFteo.Rosreestr.TMyOwner("");
                    if (ownitem.SelectSingleNode("kpoks:Person", nsmgr) != null)
                    { ///KPOKS/Realty/Flat/Rights/Right/Owners/Owner/Person/FamilyName
						own.OwnerName += ownitem.SelectSingleNode("kpoks:Person/kpoks:FamilyName", nsmgr).FirstChild.Value + " ";
                        own.OwnerName += ownitem.SelectSingleNode("kpoks:Person/kpoks:FirstName", nsmgr).FirstChild.Value + " ";
                        if (ownitem.SelectSingleNode("kpoks:Person/kpoks:Patronymic", nsmgr) != null)
                            own.OwnerName += ownitem.SelectSingleNode("kpoks:Person/kpoks:Patronymic", nsmgr).FirstChild.Value;
                    }

                    if (ownitem.SelectSingleNode("kpoks:Organization", nsmgr) != null)
                    {
                        System.Xml.XmlNode ownrOrg = ownitem.SelectSingleNode("kpoks:Organization/kpoks:Name", nsmgr);
                        own.OwnerName = ownrOrg.LastChild.Value;
                    }

                    if (ownitem.SelectSingleNode("kpoks:Governance", nsmgr) != null)
                    {
                        System.Xml.XmlNode ownrOrg = ownitem.SelectSingleNode("kpoks:Governance/kpoks:Name", nsmgr);
                        own.OwnerName = ownrOrg.LastChild.Value;
                    }
                    rt.Owners.Add(own);
                }

                // /KPOKS/Realty/Flat/Rights/Right/Registration
                rt.RegNumber = cd.SelectSingleNode("kpoks:Registration/kpoks:RegNumber", nsmgr).FirstChild.Value;
                //rt.Name = cd.SelectSingleNode("kpoks:Registration/kpoks:Name", nsmgr).FirstChild.Value;
                rt.RegDate = cd.SelectSingleNode("kpoks:Registration/kpoks:RegDate", nsmgr).FirstChild.Value;
                //rt.Type = cd.SelectSingleNode("kpoks:Registration/kpoks:Type", nsmgr).FirstChild.Value;

                if (cd.SelectSingleNode("kpoks:Registration/kpoks:ShareText", nsmgr) != null)
                    rt.Desc = cd.SelectSingleNode("kpoks:Registration/kpoks:ShareText", nsmgr).FirstChild.Value;

                rs.Add(rt);
            }
            return rs;
        }


        public static netFteo.Rosreestr.TMyRights ParseEGRNRights(System.Xml.XmlDocument xmldoc)
        {
            // Add the namespace.
            System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
            nsmgr.AddNamespace("docns", xmldoc.DocumentElement.NamespaceURI);
            string RootName = xmldoc.DocumentElement.Name; //Корни документа могут быть разными - KPZU, KVZU etc. Получим по факту из документа

            System.Xml.XmlNodeList Rights = xmldoc.DocumentElement.SelectNodes("/docns:" + RootName + "/docns:ReestrExtract/docns:ExtractObjectRight/docns:ExtractObject/docns:ObjectRight/docns:Right", nsmgr);


            netFteo.Rosreestr.TMyRights rs = new netFteo.Rosreestr.TMyRights();
            foreach (System.Xml.XmlNode cd in Rights)
            {
                netFteo.Rosreestr.TRight rt = new netFteo.Rosreestr.TRight();

                foreach (System.Xml.XmlNode child in cd.ChildNodes)
                {
                    if (child.Name == "Owner")
                    {
                        netFteo.Rosreestr.TMyOwner own = new netFteo.Rosreestr.TMyOwner("");
                        if (child.SelectSingleNode("docns:Person", nsmgr) != null)
                        {
                            own.OwnerName += child.SelectSingleNode("docns:Person/docns:FIO/docns:Surname", nsmgr).FirstChild.Value + " ";
                            own.OwnerName += child.SelectSingleNode("docns:Person/docns:FIO/docns:First", nsmgr).FirstChild.Value + " ";
                            if (child.SelectSingleNode("docns:Person/docns:FIO/docns:Patronymic", nsmgr) != null)
                                own.OwnerName += child.SelectSingleNode("docns:Person/docns:FIO/docns:Patronymic", nsmgr).FirstChild.Value + " ";
                        }

                        if (child.SelectSingleNode("docns:Organization", nsmgr) != null)
                        {
                            System.Xml.XmlNode ownrOrg = child.SelectSingleNode("docns:Organization/docns:Name", nsmgr);
                            own.OwnerName = ownrOrg.LastChild.Value;
                        }

                        if (child.SelectSingleNode("docns:Governance", nsmgr) != null)
                        {
                            System.Xml.XmlNode ownrOrg = child.SelectSingleNode("docns:Governance/docns:Name", nsmgr);
                            own.OwnerName = ownrOrg.LastChild.Value;
                        }

                        if (child.SelectSingleNode("docns:ContactOwner", nsmgr) != null)
                            own.ContactOwner = child.SelectSingleNode("docns:ContactOwner/docns:Address", nsmgr).FirstChild.Value;


                        rt.Owners.Add(own);
                    }

                    if (child.Name == "NoOwner")
                    {
                        rt.Name = child.FirstChild.Value;
                    }

                    if (child.Name == "Registration")
                    {
                        if (child.SelectSingleNode("docns:RegNumber", nsmgr) != null)
                        {
                            rt.RegNumber = child.SelectSingleNode("docns:RegNumber", nsmgr).FirstChild.Value;
                            rt.Name = child.SelectSingleNode("docns:Name", nsmgr).FirstChild.Value;
                            if (child.SelectSingleNode("docns:Desc", nsmgr) != null)
                                rt.Name = child.SelectSingleNode("docns:Desc", nsmgr).FirstChild.Value;
                            rt.RegDate = child.SelectSingleNode("docns:RegDate", nsmgr).FirstChild.Value;
                            rt.Type = child.SelectSingleNode("docns:Type", nsmgr).FirstChild.Value;
                        }

                        if (child.SelectSingleNode("docns:ShareText", nsmgr) != null)
                            rt.ShareText = child.SelectSingleNode("docns:ShareText", nsmgr).FirstChild.Value;

                        if (child.SelectSingleNode("docns:Share", nsmgr) != null) //Здесь дробь в атрибутах:
                            rt.ShareText = child.SelectSingleNode("docns:Share", nsmgr).Attributes.GetNamedItem("Numerator").Value +
                                "//" + child.SelectSingleNode("docns:Share", nsmgr).Attributes.GetNamedItem("Denominator").Value;



                    }
                }
                //old case: TODO reee
                if (cd.SelectSingleNode("docns:NoOwner", nsmgr) == null)
                {
                    System.Xml.XmlNodeList enclst = cd.SelectNodes("docns:Encumbrance", nsmgr);
                    foreach (System.Xml.XmlNode enc_node in enclst)
                    {
                        //-  /KVZU/ReestrExtract/ExtractObjectRight/ExtractObject/ObjectRight/Right[413]/Encumbrance[1]/Name
                        if (enc_node.SelectSingleNode("docns:Name", nsmgr) != null)
                        {
                            netFteo.Rosreestr.TMyEncumbrance MyEnc = new netFteo.Rosreestr.TMyEncumbrance();
                            MyEnc.Name = enc_node.SelectSingleNode("docns:Name", nsmgr).FirstChild.Value;
                            if (enc_node.SelectSingleNode("docns:ShareText", nsmgr) != null)
                                MyEnc.Desc = enc_node.SelectSingleNode("docns:ShareText", nsmgr).FirstChild.Value;

                            if (enc_node.SelectSingleNode("docns:Owner/docns:Organization/docns:Name", nsmgr) != null)

                            // /...root.../ReestrExtract/ExtractObjectRight/ExtractObject/ObjectRight/Right[413]/Encumbrance[1]/Owner/Organization/Name
                            {
                                netFteo.Rosreestr.TMyOwner ow = new netFteo.Rosreestr.TMyOwner(enc_node.SelectSingleNode("docns:Owner/docns:Organization/docns:Name", nsmgr).FirstChild.Value);
                                MyEnc.Owners.Add(ow);
                            }
                            rt.Encumbrances.Add(MyEnc);
                        }


                    }

                    /*

			
						*/

                }
                /*
                else
                    rt.Name = cd.SelectSingleNode("docns:NoOwner", nsmgr).FirstChild.Value;
                */

                rs.Add(rt);
            }
            return rs;
        }


        /// <summary>
        /// Разбор node - значение первого дочернего элемента
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <returns></returns>
        /*
        public static string Parse_Recipient(System.Xml.XmlDocument xmldoc)
        {
           return netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "/ReestrExtract/DeclarAttribute/ReceivName");
        }
  */

        /// <summary>
        /// Parse  /KPZU/eDocument/Sender
        /// </summary>
        /// <param name="xmldoc">Типа XMlDocument</param>
        /// <returns></returns>
        /*
        public static string Parse_SenderAppointment(System.Xml.XmlDocument xmldoc)
        {
           return RRTypes.CommonCast.CasterEGRP.Parse_Attribute(xmldoc, "Appointment", "/eDocument/Sender");
        }

  */




        /// <summary>
        /// Разбор ветви ReestrExtract в документах Росреестра - дописочки от
        /// регистраторов, содержащей сведения о документе
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <param name="res"></param>
        public static void Parse_DocumentProperties(System.Xml.XmlDocument xmldoc, netFteo.IO.FileInfo res)
        {

            if (netFteo.XML.XMLWrapper.NodeExist(xmldoc, "ReestrExtract"))
            //            if (xmldoc.DocumentElement.SelectSingleNode("ReestrExtract") != null)
            {
                res.DocType = netFteo.XML.XMLWrapper.Parse_Attribute(xmldoc, "ExtractTypeText", "ReestrExtract/DeclarAttribute");
                res.Version = netFteo.XML.XMLWrapper.Parse_Attribute(xmldoc, "Version", "eDocument");
                res.Number = netFteo.XML.XMLWrapper.Parse_Attribute(xmldoc, "ExtractNumber", "ReestrExtract/DeclarAttribute");
                res.Date = netFteo.XML.XMLWrapper.Parse_Attribute(xmldoc, "ExtractDate", "ReestrExtract/DeclarAttribute");
                res.RequeryNumber = netFteo.XML.XMLWrapper.Parse_Attribute(xmldoc, "RequeryNumber", "ReestrExtract/DeclarAttribute");
                res.Cert_Doc_Organization = netFteo.XML.XMLWrapper.Parse_Attribute(xmldoc, "Name", "eDocument/Sender");
                res.Appointment = netFteo.XML.XMLWrapper.Parse_Attribute(xmldoc, "Appointment", "eDocument/Sender");
                res.AppointmentFIO = netFteo.XML.XMLWrapper.Parse_Attribute(xmldoc, "Registrator", "ReestrExtract/DeclarAttribute");
                res.ReceivName = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "ReestrExtract/DeclarAttribute/ReceivName");
                res.ReceivAdress = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "ReestrExtract/DeclarAttribute/ReceivAdress");
            }
            else
            {
                // if ReestrExtract no present, try to read '/CertificationDoc'
                //if present node with namespace
                // TODO - malfunction of reading :
                /*in case with namespaces save for further:
                System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
                nsmgr.AddNamespace("parseNS", xmldoc.DocumentElement.NamespaceURI);
                */
                //case v 6.09
                System.Xml.XmlNode CertificationDoc = netFteo.XML.XMLWrapper.Parse_Node(xmldoc, "CertificationDoc");
                if (CertificationDoc != null)
                {
                    res.Number = netFteo.XML.XMLWrapper.SelectNodeChildValue(CertificationDoc, "Number");
                    res.Date = netFteo.XML.XMLWrapper.SelectNodeChildValue(CertificationDoc, "Date");
                    res.Cert_Doc_Organization = netFteo.XML.XMLWrapper.SelectNodeChildValue(CertificationDoc, "Organization");
                    res.Appointment = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "CertificationDoc/Official/Appointment");
                    res.AppointmentFIO = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "CertificationDoc/Official/FamilyName");
                }


                // vidimus 04/05 not use Namespaces:
                System.Xml.XmlNode CertificationDoc4 = netFteo.XML.XMLWrapper.Parse_Node(xmldoc, "Package/Certification_Doc");// xmldoc.SelectSingleNode(netFteo.XML.XMLWrapper.NS_Xpath(xmldoc, "/Package/Certification_Doc"));
                if (CertificationDoc4 != null)
                {
                    res.DocType = "Кадастровая выписка о земельном участке";
                    res.Number = netFteo.XML.XMLWrapper.SelectNodeChildValue(CertificationDoc4, "Number");
                    res.Appointment = netFteo.XML.XMLWrapper.SelectNodeChildValue(CertificationDoc4, "Appointment");
                    res.AppointmentFIO = netFteo.XML.XMLWrapper.SelectNodeChildValue(CertificationDoc4, "FIO");
                    res.Date = netFteo.XML.XMLWrapper.SelectNodeChildValue(CertificationDoc4, "Date");
                    res.Cert_Doc_Organization = netFteo.XML.XMLWrapper.SelectNodeChildValue(CertificationDoc4, "Organization");
                }

                if ((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "eDocument") != null) &&
                    (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "eDocument").Attributes.GetNamedItem("Version") != null))
                    res.Version = xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "eDocument").Attributes.GetNamedItem("Version").Value;
            }
        }
    }

    #region Cast KPT v11 , KVZU 8
    public static class CasterKPT11
    {
        public static void Parse_KTP11Info(System.Xml.XmlDocument xmldoc, netFteo.IO.FileInfo res)
        {
            res.Version = "11";
            res.Date = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "details_statement/group_top_requisites/date_formation");
            res.Number = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "details_statement/group_top_requisites/registration_number");
            res.Cert_Doc_Organization = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "details_statement/group_top_requisites/organ_registr_rights");
            //recipient_statement
            res.ReceivName =  netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "recipient_statement");
        }
        //TODO types 1, 2, 0

        public static string BoundToName(string Boundtype)
        {
            switch (Boundtype)
            {
                case "3": return "Граница муниципального образования";
                case "4": return "Граница нп";
                case "5": return "Береговая линия (граница водного объекта)";
            }
            // if (GKNBound.SubjectsBoundary != null) return "Граница между субъектами Российской Федерации";
            // if (GKNBound.InhabitedLocalityBoundary != null) return "Граница населенного пункта";
            return null;
        }


        /// <summary>
        /// KPT 11 Location parser
        /// </summary>
        /// <param name="xmllocation"></param>
        /// <returns></returns>
        public static TLocation Parse_Location(System.Xml.XmlNode xmllocation)
        {
            if (xmllocation == null) return null;
            TLocation loc = new TLocation();
            XmlNode Address = netFteo.XML.XMLWrapper.SelectNodeChild(xmllocation, "address");
            if (Address != null)
            {
                TAddress Adr = new TAddress();
                Adr.Note = netFteo.XML.XMLWrapper.SelectNodeChildValue(Address, "readable_address");

                if (netFteo.XML.XMLWrapper.SelectNodeChild(Address, "address_fias/level_settlement/city") != null)
                    Adr.City = netFteo.XML.XMLWrapper.SelectNodeChildValue(Address, "address_fias/level_settlement/city/type_city") + " " +
                        netFteo.XML.XMLWrapper.SelectNodeChildValue(Address, "address_fias/level_settlement/city/name_city");

                if (netFteo.XML.XMLWrapper.SelectNodeChild(Address, "address_fias/detailed_level/street/type_street") != null)
                {
                    Adr.Street = netFteo.XML.XMLWrapper.SelectNodeChildValue(Address, "address_fias/detailed_level/street/type_street") + " " +
                        netFteo.XML.XMLWrapper.SelectNodeChildValue(Address, "address_fias/detailed_level/street/name_street");
                }


                //if (netFteo.XML.XMLWrapper.SelectNodeChild(Address, "address_fias/detailed_level/level1") != null)
                {

                    Adr.Level1 = netFteo.XML.XMLWrapper.SelectNodeChildValue(Address, "address_fias/detailed_level/level1/type_level1") + " " +
                                 netFteo.XML.XMLWrapper.SelectNodeChildValue(Address, "address_fias/detailed_level/level1/name_level1");
                }

                Adr.Region = netFteo.XML.XMLWrapper.SelectNodeChildValue(Address, "address_fias/level_settlement/region/code");
                loc.Address = Adr;
            }
            return loc;
        }

        public static TParcel Parse_Parcel(XmlNode parcel)
        {
         
            TParcel MainObj = new TParcel(parcel.SelectSingleNode("object/common_data/cad_number").FirstChild.Value,
              netFteo.XML.XMLWrapper.SelectNodeChildValue(parcel, "object/common_data/type/code"));
            //subtype:
            string SubTypeCode = netFteo.XML.XMLWrapper.SelectNodeChildValue(parcel, "object/subtype/code");
            //case 02 - EZP
            if (SubTypeCode == "02")
            {
                MainObj.Name = netFteo.Rosreestr.dParcelsv01.ItemToName("Item02");
                MainObj.CompozitionEZ = new TCompozitionEZ();
            }
            //case 05 mk
            if (SubTypeCode == "05")
            {
                MainObj.Name = netFteo.Rosreestr.dParcelsv01.ItemToName("Item05");
            }

            MainObj.AreaGKN = parcel.SelectSingleNode("params/area/value").FirstChild.Value;
            if (parcel.SelectSingleNode("params/category") != null)
            MainObj.Category = parcel.SelectSingleNode("params/category/type/code").FirstChild.Value;
            if (parcel.SelectSingleNode("params/permitted_use") != null)
                MainObj.Utilization.UtilbyDoc = parcel.SelectSingleNode("params/permitted_use/permitted_use_established/by_document").FirstChild.Value;
            MainObj.Utilization.Untilization = netFteo.XML.XMLWrapper.SelectNodeChildValue(parcel, "params/permitted_use/permitted_use_established/land_use/code");
            MainObj.Location = Parse_Location(parcel.SelectSingleNode("address_location"));
            if (parcel.SelectSingleNode("cost/value") != null)
                MainObj.CadastralCost = Convert.ToDecimal(parcel.SelectSingleNode("cost/value").FirstChild.Value);
            //in case of 'KPT' document date not specified. So check, if exist
            if (parcel.SelectSingleNode("record_info/registration_date") != null)
                MainObj.DateCreated = parcel.SelectSingleNode("record_info/registration_date").FirstChild.Value;// .ToString("dd.MM.yyyy") 
            if (parcel.SelectSingleNode("special_notes") != null)
                MainObj.SpecialNote = parcel.SelectSingleNode("special_notes").FirstChild.Value;

            int TestSpatCount = parcel.SelectNodes("contours_location/contours/contour").Count;
            //SignleSpatial
            /*
            if (parcel.SelectNodes("contours_location/contours/contour").Count == 1)
            if (parcel.SelectSingleNode("contours_location/contours/contour/entity_spatial") != null)
            {
                TPolygon ents = CasterKPT11.LandEntSpatToFteo(MainObj.CN,
                                                      parcel.SelectSingleNode("contours_location/contours/contour/entity_spatial"));

                ents.AreaValue = (decimal)Convert.ToDouble(MainObj.AreaGKN);
                ents.Parent_Id = MainObj.id;
                ents.Definition = MainObj.CN;
                MainObj.EntSpat.Add(ents);
            }
            */
            //TODO:
            //Многоконтурный TODO: Got examples!!!
            // contours_location
            XmlNode contours = parcel.SelectSingleNode("contours_location/contours");

            if (contours != null)
            {
                for (int ic = 0; ic <= contours.ChildNodes.Count - 1; ic++)
                {
                    if (contours.ChildNodes.Count > 1)
                    {
                        //multiContours
                        TPolygon ents = CasterKPT11.LandEntSpatToFteo(contours.ChildNodes[ic].SelectSingleNode("number_pp").Value,
                                                                             contours.ChildNodes[ic].SelectSingleNode("entity_spatial"));
                        ents.Definition = ":" + MainObj.CN.Split(':')[3]+"(" +  contours.ChildNodes[ic].SelectSingleNode("number_pp").FirstChild.Value + ")";
                        MainObj.EntSpat.Add(ents);
                    }
                    else
                    { //single:
                        TPolygon ents = CasterKPT11.LandEntSpatToFteo(MainObj.CN,
                                                                          parcel.SelectSingleNode("contours_location/contours/contour/entity_spatial"));

                        ents.AreaValue = (decimal)Convert.ToDouble(MainObj.AreaGKN);
                        ents.Parent_Id = MainObj.id;
                        ents.Definition = ":"+ MainObj.CN.Split(':')[3];
                        MainObj.EntSpat.Add(ents);
                    }
                }
            }
            // TODO: Parsing CompozitionEZ for case subtype =02
            // KPT11, like kpt10, does not contain spatial data for EZP.....

            return MainObj;
        }
        public static TParcel Parse_OKS(XmlNode parcel)
        {

            TParcel MainObj = new TParcel(parcel.SelectSingleNode("object/common_data/cad_number").FirstChild.Value,
              netFteo.XML.XMLWrapper.SelectNodeChildValue(parcel, "object/common_data/type/code"));
            //subtype:
            string SubTypeCode = netFteo.XML.XMLWrapper.SelectNodeChildValue(parcel, "object/subtype/code");
            //case 02 - EZP
            if (SubTypeCode == "02")
            {
                MainObj.Name = netFteo.Rosreestr.dParcelsv01.ItemToName("Item02");
                MainObj.CompozitionEZ = new TCompozitionEZ();
            }
            //case 05 mk
            if (SubTypeCode == "05")
            {
                MainObj.Name = netFteo.Rosreestr.dParcelsv01.ItemToName("Item05");
            }

           /* MainObj.AreaGKN = parcel.SelectSingleNode("params/area/value").FirstChild.Value;
            if (parcel.SelectSingleNode("params/category") != null)
                MainObj.Category = parcel.SelectSingleNode("params/category/type/code").FirstChild.Value;
            if (parcel.SelectSingleNode("params/permitted_use") != null)
                MainObj.Utilization.UtilbyDoc = parcel.SelectSingleNode("params/permitted_use/permitted_use_established/by_document").FirstChild.Value;
            MainObj.Utilization.Untilization = netFteo.XML.XMLWrapper.SelectNodeChildValue(parcel, "params/permitted_use/permitted_use_established/land_use/code");
            MainObj.Location = Parse_Location(parcel.SelectSingleNode("address_location"));
            if (parcel.SelectSingleNode("cost/value") != null)
                MainObj.CadastralCost = Convert.ToDecimal(parcel.SelectSingleNode("cost/value").FirstChild.Value);
            //in case of 'KPT' document date not specified. So check, if exist
            if (parcel.SelectSingleNode("record_info/registration_date") != null)
                MainObj.DateCreated = parcel.SelectSingleNode("record_info/registration_date").FirstChild.Value;// .ToString("dd.MM.yyyy") 
            if (parcel.SelectSingleNode("special_notes") != null)
                MainObj.SpecialNote = parcel.SelectSingleNode("special_notes").FirstChild.Value;
           */
            int TestSpatCount = parcel.SelectNodes("contours/contour").Count;
 
            // contours_location
            XmlNode contours = parcel.SelectSingleNode("contours");

            if (contours != null)
            {
                for (int ic = 0; ic <= contours.ChildNodes.Count - 1; ic++)
                {
                    if (contours.ChildNodes.Count > 1)
                    {
                        //multiContours
                        TPolygon ents = CasterKPT11.LandEntSpatToFteo(contours.ChildNodes[ic].SelectSingleNode("number_pp").Value,
                                                                             contours.ChildNodes[ic].SelectSingleNode("entity_spatial"));
                        ents.Definition = ":" + MainObj.CN.Split(':')[3] + "(" + contours.ChildNodes[ic].SelectSingleNode("number_pp").FirstChild.Value + ")";
                        MainObj.EntSpat.Add(ents);
                    }
                    else
                    { //single:
                        TPolygon ents = CasterKPT11.LandEntSpatToFteo(MainObj.CN,
                                                                          parcel.SelectSingleNode("contours/contour/entity_spatial"));

                        ents.AreaValue = (decimal)Convert.ToDouble(MainObj.AreaGKN);
                        ents.Parent_Id = MainObj.id;
                        ents.Definition = ":" + MainObj.CN.Split(':')[3];
                        MainObj.EntSpat.Add(ents);
                    }
                }
            }
            // TODO: Parsing CompozitionEZ for case subtype =02
            // KPT11, like kpt10, does not contain spatial data for EZP.....

            return MainObj;
        }
        //Разбор ordinate KPT11
        private static TPoint ParseOrdinate(System.Xml.XmlNode Spelement_Unit)
        {
            TPoint Point = new TPoint();
            Point.x = Convert.ToDouble(Spelement_Unit.SelectSingleNode("x").FirstChild.Value);
            Point.y = Convert.ToDouble(Spelement_Unit.SelectSingleNode("y").FirstChild.Value);
            Point.oldX = Point.x;
            Point.oldY = Point.y;
            Point.Definition = netFteo.XML.XMLWrapper.SelectNodeChildValue(Spelement_Unit, "num_geopoint");
            if (netFteo.XML.XMLWrapper.SelectNodeChild(Spelement_Unit, "delta_geopoint") != null)
                Point.Mt = Convert.ToDouble(netFteo.XML.XMLWrapper.SelectNodeChildValue(Spelement_Unit, "delta_geopoint"));
            return Point;
        }

        /// <summary>
        /// Разбор Entity_Spatial KPT_V11
        /// </summary>
        /// <param name="Definition"></param>
        /// <param name="ES"></param>
        /// <returns></returns>
        public static TPolygon LandEntSpatToFteo(string Definition, System.Xml.XmlNode ES)
        {
            {
                if (ES == null) return null;
                TPolygon EntSpat = new TPolygon();
                EntSpat.Definition = Definition;

                System.Xml.XmlNodeList ESEntSpat = ES.SelectNodes("spatials_elements/spatial_element");

                //Первый Spatial_Element - внешний контур ( 0 )
                System.Xml.XmlNode OuterRing = ESEntSpat[0].SelectSingleNode("ordinates");


                for (int iSpelement = 0; iSpelement <= OuterRing.ChildNodes.Count - 1; iSpelement++)
                {
                    EntSpat.AddPoint(ParseOrdinate(OuterRing.ChildNodes[iSpelement]));
                }

                //Второй и след. Spatial_Element - Внутренние контура
                if (ESEntSpat.Count > 1)
                {
                    for (int ring = 1; ring <= ESEntSpat.Count - 1; ring++)
                    {
                        System.Xml.XmlNode childRing = ESEntSpat[ring].SelectSingleNode("ordinates");

                        TRing InLayer = EntSpat.AddChild();

                        for (int iSpelement = 1; iSpelement <= childRing.ChildNodes.Count - 1; iSpelement++)
                        {
                            InLayer.AddPoint(ParseOrdinate(childRing.ChildNodes[iSpelement]));
                        }
                    }
                }
                return EntSpat;
            }
        }


        /// <summary>
        /// Parse only into raw spatial (State == 14). Todo, spatial need be parsed
        /// </summary>
        /// <param name="Definition"></param>
        /// <param name="ES"></param>
        /// <returns></returns>
        public static TEntitySpatial LandEntSpatToES2(string Definition, System.Xml.XmlNode ES)
        {
            if (ES == null) return null;
            System.Xml.XmlNodeList ESEntSpat = ES.SelectNodes("spatials_elements/spatial_element");
            List<TRing> AllRings = new List<TRing>();
            //Read all present rings:
            for (int ring = 0; ring <= ESEntSpat.Count - 1; ring++)
            {
                System.Xml.XmlNode childRing = ESEntSpat[ring].SelectSingleNode("ordinates");

                TRing InLayer = new TRing();

                for (int iSpelement = 0; iSpelement <= childRing.ChildNodes.Count - 1; iSpelement++)
                {
                    InLayer.AddPoint(ParseOrdinate(childRing.ChildNodes[iSpelement]));
                }
                AllRings.Add(InLayer);
            }
            TEntitySpatial ESParsed = new TEntitySpatial(Definition);
            ESParsed.State = 14; //raw spatial
            ESParsed.AddRange(AllRings); //returned raw spatial, without parsing childs and parents
            return ESParsed;
        }



    }

    #endregion

    /// <summary>
    /// Общий класс утилит для кастинга разных версий схем ZU
    /// </summary>
    public static class CasterZU
    {
        public static TAddress CastAddress(kpzu.tAddressOut Address)
        {
            if (Address == null) return null;
            TAddress Adr = new TAddress();
            Adr.KLADR = Address.KLADR;
            Adr.Note = Address.Note;
            if (Address.City != null)
                Adr.City = Address.City.Type + " " + Address.City.Name;
            if (Address.District != null)
                Adr.District = Address.District.Type + " " + Address.District.Name;
            if (Address.Locality != null)
                Adr.Locality = Address.Locality.Type + " " + Address.Locality.Name;
            if (Address.Street != null)
                Adr.Street = Address.Street.Type + " " + Address.Street.Name;
            if (Address.Level1 != null)
                Adr.Level1 = Address.Level1.Type + " " + Address.Level1.Value;
            if (Address.Level2 != null)
                Adr.Level2 = Address.Level2.Type + " " + Address.Level2.Value;

            if (Address.Apartment != null)
                Adr.Apartment = Address.Apartment.Type + " " + Address.Apartment.Value;

            Adr.Region = Address.Region.ToString();

            return Adr;
        }
        public static TAddress CastAddress(kpzu06.tAddressOut Address)
        {
            if (Address == null) return null;
            TAddress Adr = new TAddress();
            Adr.KLADR = Address.KLADR;
            Adr.Note = Address.Note;
            if (Address.City != null)
                Adr.City = Address.City.Type + " " + Address.City.Name;
            if (Address.District != null)
                Adr.District = Address.District.Type + " " + Address.District.Name;
            if (Address.Locality != null)
                Adr.Locality = Address.Locality.Type + " " + Address.Locality.Name;
            if (Address.Street != null)
                Adr.Street = Address.Street.Type + " " + Address.Street.Name;
            if (Address.Level1 != null)
                Adr.Level1 = Address.Level1.Type + " " + Address.Level1.Value;
            if (Address.Level2 != null)
                Adr.Level2 = Address.Level2.Type + " " + Address.Level2.Value;

            if (Address.Apartment != null)
                Adr.Apartment = Address.Apartment.Type + " " + Address.Apartment.Value;

            Adr.Region = Address.Region.ToString();
            return Adr;
        }

        public static TAddress CastAddress(kvzu.tAddressOut Address)
        {
            if (Address == null) return null;
            TAddress Adr = new TAddress();
            Adr.KLADR = Address.KLADR;
            Adr.Note = Address.Note;
            if (Address.City != null)
                Adr.City = Address.City.Type + " " + Address.City.Name;
            if (Address.District != null)
                Adr.District = Address.District.Type + " " + Address.District.Name;
            if (Address.Locality != null)
                Adr.Locality = Address.Locality.Type + " " + Address.Locality.Name;
            if (Address.Street != null)
                Adr.Street = Address.Street.Type + " " + Address.Street.Name;
            if (Address.Level1 != null)
                Adr.Level1 = Address.Level1.Type + " " + Address.Level1.Value;
            if (Address.Level2 != null)
                Adr.Level2 = Address.Level2.Type + " " + Address.Level2.Value;

            if (Address.Apartment != null)
                Adr.Apartment = Address.Apartment.Type + " " + Address.Apartment.Value;

            Adr.Region = Address.Region.ToString();

            return Adr;
        }
        public static TAddress CastAddress(kvzu07.tAddressOut Address)
        {
            if (Address == null) return null;
            /*здесь большая фича по кастингу произошла, VS2015 сама создала (предложила)
             * такую весчь в классе kpzu.tAddress:
                public static explicit operator tAddressOut(kvzu07.tAddressOut v)
                {
                    throw new NotImplementedException();
                }
            */
            /*
          TAddress Adr = CastAddress((kpzu.tAddressOut)Address);

          return Adr; */
            if (Address == null) return null;
            TAddress Adr = new TAddress();
            Adr.KLADR = Address.KLADR;
            Adr.Note = Address.Note;
            if (Address.City != null)
                Adr.City = Address.City.Type + " " + Address.City.Name;
            if (Address.District != null)
                Adr.District = Address.District.Type + " " + Address.District.Name;
            if (Address.Locality != null)
                Adr.Locality = Address.Locality.Type + " " + Address.Locality.Name;
            if (Address.Street != null)
                Adr.Street = Address.Street.Type + " " + Address.Street.Name;
            if (Address.Level1 != null)
                Adr.Level1 = Address.Level1.Type + " " + Address.Level1.Value;
            if (Address.Level2 != null)
                Adr.Level2 = Address.Level2.Type + " " + Address.Level2.Value;

            if (Address.Apartment != null)
                Adr.Apartment = Address.Apartment.Type + " " + Address.Apartment.Value;

            Adr.Region = Address.Region.ToString().Substring(4);

            return Adr;
        }
        public static TAddress CastAddress(kpt09.tAddressOut Address)
        {
            if (Address == null) return null;
            TAddress Adr = new TAddress();
            Adr.KLADR = Address.KLADR;
            Adr.Note = Address.Note;
            if (Address.City != null)
                Adr.City = Address.City.Type + " " + Address.City.Name;
            if (Address.District != null)
                Adr.District = Address.District.Type + " " + Address.District.Name;
            if (Address.Locality != null)
                Adr.Locality = Address.Locality.Type + " " + Address.Locality.Name;
            if (Address.Street != null)
                Adr.Street = Address.Street.Type + " " + Address.Street.Name;
            if (Address.Level1 != null)
                Adr.Level1 = Address.Level1.Type + " " + Address.Level1.Value;
            if (Address.Level2 != null)
                Adr.Level2 = Address.Level2.Type + " " + Address.Level2.Value;

            if (Address.Apartment != null)
                Adr.Apartment = Address.Apartment.Type + " " + Address.Apartment.Value;

            Adr.Region = Address.Region.ToString().Substring(4);

            return Adr;
        }
        public static TAddress CastAddress(kpt10_un.tAddressOut Address)
        {
            if (Address == null) return null;
            TAddress Adr = new TAddress();
            Adr.KLADR = Address.KLADR;
            Adr.Note = Address.Note;
            if (Address.City != null)
                Adr.City = Address.City.Type + " " + Address.City.Name;
            if (Address.District != null)
                Adr.District = Address.District.Type + " " + Address.District.Name;
            if (Address.Locality != null)
                Adr.Locality = Address.Locality.Type + " " + Address.Locality.Name;
            if (Address.Street != null)
                Adr.Street = Address.Street.Type + " " + Address.Street.Name;
            if (Address.Level1 != null)
                Adr.Level1 = Address.Level1.Type + " " + Address.Level1.Value;
            if (Address.Level2 != null)
                Adr.Level2 = Address.Level2.Type + " " + Address.Level2.Value;

            if (Address.Apartment != null)
                Adr.Apartment = Address.Apartment.Type + " " + Address.Apartment.Value;

            Adr.Region = Address.Region.ToString().Substring(4);

            return Adr;
        }
        public static TAddress CastAddress(MP_V06.tNewParcelAddress Address)
        {
            if (Address == null) return null;
            TAddress Adr = new TAddress();
            Adr.KLADR = Address.KLADR;
            Adr.Note = Address.Other;
            if (Address.City != null)
                Adr.City = Address.City.Type + " " + Address.City.Name;
            if (Address.District != null)
                Adr.District = Address.District.Type + " " + Address.District.Name;
            if (Address.Locality != null)
                Adr.Locality = Address.Locality.Type + " " + Address.Locality.Name;
            if (Address.Street != null)
                Adr.Street = Address.Street.Type + " " + Address.Street.Name;
            if (Address.Level1 != null)
                Adr.Level1 = Address.Level1.Type + " " + Address.Level1.Value;
            if (Address.Level2 != null)
                Adr.Level2 = Address.Level2.Type + " " + Address.Level2.Value;

            if (Address.Apartment != null)
                Adr.Apartment = Address.Apartment.Type + " " + Address.Apartment.Value;

            Adr.Region = Address.Region.ToString().Substring(4); // supress 'Item'

            return Adr;
        }

        public static TLocation CastLocation(kvzu07.tLocation location)
        {

            if (location == null) return null;
            TLocation loc = new TLocation();
            loc.Address = CastAddress(location.Address);
            if (location.Elaboration != null)
            {
                if (location.Elaboration.Direction != null)
                    loc.Elaboration.Direction = location.Elaboration.Direction;

                if (location.Elaboration.Distance != null)
                    loc.Elaboration.Distance = location.Elaboration.Distance;

                if (location.Elaboration.ReferenceMark != null)
                    loc.Elaboration.ReferenceMark = location.Elaboration.ReferenceMark;
            }


            loc.Inbounds = location.inBounds.ToString();

            return loc;
        }
        public static TLocation CastLocation(kvzu.tLocation location)
        {

            if (location == null) return null;
            TLocation loc = new TLocation();
            loc.Address = CastAddress(location.Address);
            if (location.Elaboration != null)
            {
                if (location.Elaboration.Direction != null)
                    loc.Elaboration.Direction = location.Elaboration.Direction;

                if (location.Elaboration.Distance != null)
                    loc.Elaboration.Distance = location.Elaboration.Distance;

                if (location.Elaboration.ReferenceMark != null)
                    loc.Elaboration.ReferenceMark = location.Elaboration.ReferenceMark;
            }

            loc.Inbounds = location.inBounds.ToString();

            return loc;
        }
        public static TLocation CastLocation(kpzu.tLocation location)
        {
            if (location == null) return null;
            TLocation loc = new TLocation();
            loc.Address = CastAddress(location.Address);
            if (location.Elaboration != null)
            {
                if (location.Elaboration.Direction != null)
                    loc.Elaboration.Direction = location.Elaboration.Direction;

                if (location.Elaboration.Distance != null)
                    loc.Elaboration.Distance = location.Elaboration.Distance;

                if (location.Elaboration.ReferenceMark != null)
                    loc.Elaboration.ReferenceMark = location.Elaboration.ReferenceMark;
            }
            loc.Inbounds = location.inBounds.ToString();

            return loc;
        }
        public static TLocation CastLocation(kpzu06.tLocation location)
        {

            if (location == null) return null;
            TLocation loc = new TLocation();
            loc.Address = CastAddress(location.Address);
            if (location.Elaboration != null)
            {
                if (location.Elaboration.Direction != null)
                    loc.Elaboration.Direction = location.Elaboration.Direction;

                if (location.Elaboration.Distance != null)
                    loc.Elaboration.Distance = location.Elaboration.Distance;

                if (location.Elaboration.ReferenceMark != null)
                    loc.Elaboration.ReferenceMark = location.Elaboration.ReferenceMark;
            }
            loc.Inbounds = location.inBounds.ToString();

            return loc;
        }

        public static TLocation CastLocation(kpt09.tLocation location)
        {
            if (location == null) return null;
            TLocation loc = new TLocation();
            loc.Address = CastAddress(location.Address);
            if (location.Elaboration != null)
            {
                if (location.Elaboration.Direction != null)
                    loc.Elaboration.Direction = location.Elaboration.Direction;

                if (location.Elaboration.Distance != null)
                    loc.Elaboration.Distance = location.Elaboration.Distance;

                if (location.Elaboration.ReferenceMark != null)
                    loc.Elaboration.ReferenceMark = location.Elaboration.ReferenceMark;
            }

            loc.Inbounds = location.inBounds.ToString();

            return loc;
        }
        public static TLocation CastLocation(kpt10_un.tLocation location)
        {
            if (location == null) return null;
            TLocation loc = new TLocation();
            loc.Address = CastAddress(location.Address);
            if (location.Elaboration != null)
            {
                if (location.Elaboration.Direction != null)
                    loc.Elaboration.Direction = location.Elaboration.Direction;

                if (location.Elaboration.Distance != null)
                    loc.Elaboration.Distance = location.Elaboration.Distance;

                if (location.Elaboration.ReferenceMark != null)
                    loc.Elaboration.ReferenceMark = location.Elaboration.ReferenceMark;
            }
            else
                loc.Inbounds = location.inBounds.ToString();
            return loc;
        }


   

    }

    public static class CasterSpatial
    {
        #region Cast MP V05
        /// <summary>
        /// ES manipulation routines
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        /// 

        public static netFteo.Spatial.TPolygon AddEntSpatMP5(string Definition, MP_V05.tEntitySpatialBordersZUInp ES)
        {
            netFteo.Spatial.TPolygon EntSpat = new netFteo.Spatial.TPolygon();
            EntSpat.Definition = Definition;



            //Первый (внешний) контур
            for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
            {

                netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                Point.Description = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.GeopointZacrep;
                Point.Pref = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.PointPref;
                Point.Definition = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.NumGeopoint;
                EntSpat.AddPoint(Point);
            }
            //Внутренние контура
            for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
            {
                netFteo.Spatial.TRing InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {

                    netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                    Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                    Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                    Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                    Point.Definition = ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.NumGeopoint;
                    InLayer.AddPoint(Point);
                }
            }

            return EntSpat;
        }
        public static netFteo.Spatial.TPolygon AddEntSpatMP5(string Definition, MP_V05.tEntitySpatialZUInp ES)
        {
            netFteo.Spatial.TPolygon EntSpat = new netFteo.Spatial.TPolygon();
            EntSpat.Definition = Definition;



            //Первый (внешний) контур
            for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
            {

                netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                Point.Description = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.GeopointZacrep;
                Point.Pref = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.PointPref;
                Point.Definition = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.NumGeopoint;
                EntSpat.AddPoint(Point);
            }
            //Внутренние контура
            for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
            {
                netFteo.Spatial.TRing InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {

                    netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                    Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                    Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                    Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                    Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.NumGeopoint;
                    InLayer.AddPoint(Point);
                }
            }

            return EntSpat;
        }
        public static netFteo.Spatial.TPolygon AddEntSpatMP5(string Definition, MP_V05.tEntitySpatialOldNew ES)
        {
            netFteo.Spatial.TPolygon EntSpat = new netFteo.Spatial.TPolygon();
            EntSpat.Definition = Definition;



            //Первый (внешний) контур
            for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
            {

                netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].NewOrdinate.X);
                Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].NewOrdinate.Y);
                Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].NewOrdinate.DeltaGeopoint);
                Point.Description = ES.SpatialElement[0].SpelementUnit[iord].NewOrdinate.GeopointZacrep;
                Point.Pref = ES.SpatialElement[0].SpelementUnit[iord].NewOrdinate.PointPref;
                Point.NumGeopointA = ES.SpatialElement[0].SpelementUnit[iord].NewOrdinate.NumGeopoint;
                EntSpat.AddPoint(Point);
            }
            //Внутренние контура
            for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
            {
                netFteo.Spatial.TRing InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {

                    netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                    Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].NewOrdinate.X);
                    Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].NewOrdinate.Y);
                    Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].NewOrdinate.DeltaGeopoint);
                    Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].NewOrdinate.NumGeopoint;
                    InLayer.AddPoint(Point);
                }
            }

            return EntSpat;
        }
        public static netFteo.Spatial.TPolygon AddEntSpatMP5(string Definition, MP_V05.tContoursSubParcelContour ES)
        {

            netFteo.Spatial.TPolygon EntSpat = new netFteo.Spatial.TPolygon();
            EntSpat.Definition = Definition;



            //Первый (внешний) контур
            for (int iord = 0; iord <= ES.EntitySpatial.SpatialElement[0].SpelementUnit.Count - 1; iord++)
            {

                netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                Point.x = Convert.ToDouble(ES.EntitySpatial.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                Point.y = Convert.ToDouble(ES.EntitySpatial.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                Point.Mt = Convert.ToDouble(ES.EntitySpatial.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                Point.Description = ES.EntitySpatial.SpatialElement[0].SpelementUnit[iord].Ordinate.GeopointZacrep;
                Point.Pref = ES.EntitySpatial.SpatialElement[0].SpelementUnit[iord].Ordinate.PointPref;
                Point.NumGeopointA = ES.EntitySpatial.SpatialElement[0].SpelementUnit[iord].Ordinate.NumGeopoint;
                EntSpat.AddPoint(Point);
            }
            //Внутренние контура
            for (int iES = 1; iES <= ES.EntitySpatial.SpatialElement.Count - 1; iES++)
            {
                netFteo.Spatial.TRing InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.EntitySpatial.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {

                    netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                    Point.x = Convert.ToDouble(ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                    Point.y = Convert.ToDouble(ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                    Point.Mt = Convert.ToDouble(ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                    Point.NumGeopointA = ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.NumGeopoint;
                    InLayer.AddPoint(Point);
                }
            }

            return EntSpat;
        }
        public static TEntitySpatial AddContoursMP5(string Definition, MP_V05.tContoursSubParcelContourCollection cs)
        {
            TEntitySpatial res = new TEntitySpatial();
            foreach (MP_V05.tContoursSubParcelContour item in cs)
                res.Add(AddEntSpatMP5(item.Number, item));
            return res;
        }
        #endregion

        #region Cast MP V06
        public static TPolygon ES_ZU(string Definition, MP_V06.tEntitySpatialBordersZUOut ES)
        {
            /*
            netFteo.Spatial.TPolygon EntSpat = new netFteo.Spatial.TPolygon();
            EntSpat.Definition = Definition;
            //Первый (внешний) контур
            for (int iord = 0; iord <= ES.Spatial_Element[0].Spelement_Unit.Count - 1; iord++)
            {

                netFteo.Spatial.Point Point = new netFteo.Spatial.Point();

                Point.x = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].X);
                Point.y = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].Y);
                Point.Mt = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].Delta_Geopoint);
                //Point.Description = ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].Geopoint_Zacrep;
                Point.Pref = ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].Point_Pref;
                Point.NumGeopointA = ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].Num_Geopoint;
                EntSpat.Points.AddPoint(Point);
            }
            //Внутренние контура
            for (int iES = 1; iES <= ES.Spatial_Element.Count - 1; iES++)
            {
                netFteo.Spatial.TRing InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.Spatial_Element[iES].Spelement_Unit.Count - 1; iord++)
                {

                    netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
                    Point.x = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].X);
                    Point.y = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].Y);
                    Point.Mt = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].Delta_Geopoint);
                    Point.NumGeopointA = ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].Num_Geopoint;
                    InLayer.Points.AddPoint(Point);
                }
            }
            return EntSpat;
            */
            return null;
        }
        /// <summary>
        /// Кастинг ОИПД для МП6. Подсхема _Spatial_v03
        /// </summary>
        /// <param name="Definition"></param>
        /// <param name="ES"></param>
        /// <returns></returns>
        public static TPolygon ES_ZU(string Definition, MP_V06.tEntitySpatialZUInp ES)
        {

            netFteo.Spatial.TPolygon EntSpat = new netFteo.Spatial.TPolygon();
            EntSpat.Definition = Definition;
            //Первый (внешний) контур
            for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
            {

                netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();

                Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                //Point.Description = ES.SpatialElement[0].Spelement_Unit[iord].Ordinate[0].Geopoint_Zacrep;
                Point.Pref = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.PointPref;
                Point.Definition = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.NumGeopoint;
                EntSpat.AddPoint(Point);
            }
            //Внутренние контура
            for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
            {
                netFteo.Spatial.TRing InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {

                    netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                    Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                    Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                    Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                    Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.NumGeopoint;
                    InLayer.AddPoint(Point);
                }
            }
            return EntSpat;
        }/// 

        public static TPolygon ES_ZU(string Definition, MP_V06.tEntitySpatialBordersZUInp ES)
        {

            netFteo.Spatial.TPolygon EntSpat = new netFteo.Spatial.TPolygon();
            EntSpat.Definition = Definition;
            //Первый (внешний) контур
            for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
            {

                netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                Point.Status = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.PointPref == "н" ? 0 : 4;
                Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                //Point.Description = ES.SpatialElement[0].Spelement_Unit[iord].Ordinate[0].Geopoint_Zacrep;
                Point.Pref = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.PointPref;
                Point.NumGeopointA = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.NumGeopoint;
                EntSpat.AddPoint(Point);
            }
            //Внутренние контура
            for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
            {
                netFteo.Spatial.TRing InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {

                    netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                    Point.Status = ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.PointPref == "н" ? 0 : 4; // Ordinate.PointPref.Equals("н") fail if PointPref null
                    Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                    Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                    Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                    Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.NumGeopoint;
                    InLayer.AddPoint(Point);
                }
            }
            return EntSpat;
        }

        //TODO Check for MP6 adding of contours
        public static System.Collections.Generic.List<IGeometry> ES_ZU(MP_V06.tNewContourCollection ESs)
        {
            System.Collections.Generic.List<IGeometry> res = new System.Collections.Generic.List<IGeometry>();
            foreach (MP_V06.tNewContour item in ESs)
            {
                TPolygon collItem = ES_ZU(item.Definition, item.EntitySpatial);
                //collItem.ResetOrdinates(); // because is only new tNewContour
                collItem.State = 0; // tNewContour
                collItem.AreaValue = item.Area.Area;
                if (item.Area.InaccuracySpecified)
                    collItem.AreaInaccuracy = item.Area.Inaccuracy.ToString();
                res.Add(collItem);
            }
            return res;
        }




        /// <summary>
        /// Разбор юнита (например  - Точки) 
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        private static TPoint GetUnit(MP_V06.tSpelementUnitOldNew unit)
        {

            netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();

            if (unit.NewOrdinate != null)
            {
                //  " н "
                //Если указаны только новая - точка создается
                if (unit.OldOrdinate == null)
                {
                    Point.Status = 0;
                }
                // уточ / сущ.
                //Если указаны обе - точка существует либо уточняется
                if (unit.OldOrdinate != null)
                {
                    Point.oldX = Convert.ToDouble(unit.OldOrdinate.X);
                    Point.oldY = Convert.ToDouble(unit.OldOrdinate.Y);
                    Point.Status = 4;
                }

                if (unit.NewOrdinate != null)
                {
                    Point.x = Convert.ToDouble(unit.NewOrdinate.X);
                    Point.y = Convert.ToDouble(unit.NewOrdinate.Y);
                }

                Point.Mt = Convert.ToDouble(unit.NewOrdinate.DeltaGeopoint);
                //Point.Description = ES.SpatialElement[0].Spelement_Unit[iord].Ordinate[0].Geopoint_Zacrep;
                Point.Pref = unit.NewOrdinate.PointPref;
                Point.NumGeopointA = unit.NewOrdinate.NumGeopoint;
            }

            // " л "     TODO:
            //Если указаны только старая - точка ликвидируется. И это грабли всего дерева классов
            // Точка имеет два набора координат - фактически две границы - существующую в ЕГРН и новую
            if ((unit.OldOrdinate != null) && (unit.NewOrdinate == null))
            {
                Point.oldX = Convert.ToDouble(unit.OldOrdinate.X);
                Point.oldY = Convert.ToDouble(unit.OldOrdinate.Y);
                bool empt = Point.Empty;
                Point.NumGeopointA = "л " + unit.OldOrdinate.NumGeopoint;
                Point.Status = 6;
            }
            return Point;
        }

        /// <summary>
        /// Разбор Пространственных данных МП V 06
        /// </summary>
        /// <param name="Definition"></param>
        /// <param name="ES"></param>
        /// <returns></returns>
        public static TPolygon ES_ZU(string Definition, MP_V06.tEntitySpatialOldNew ES)
        {
            if (ES.SpatialElement.Count == 0) return null;

            netFteo.Spatial.TPolygon EntSpat = new netFteo.Spatial.TPolygon();
            EntSpat.Definition = Definition;

            //Первый (внешний) контур
            for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
            {  //TODO 
               // if ((ES.SpatialElement[0].SpelementUnit[iord]).NewOrdinate != null) 
               // только точки с новыми/уточняемымыи/сущесствующими коорд
                EntSpat.AddPoint(GetUnit(ES.SpatialElement[0].SpelementUnit[iord]));
            }

            //Внутренние контура
            for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
            {
                netFteo.Spatial.TRing InLayer = EntSpat.AddChild();

                for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {
                    //TODO
                    // только точки с новыми/уточняемымыи/сущесствующими коорд
                    //if ((ES.SpatialElement[iES].SpelementUnit[iord]).NewOrdinate != null) 
                    InLayer.AddPoint(GetUnit(ES.SpatialElement[iES].SpelementUnit[iord]));
                }
            }

            if (EntSpat.HasChanges == "*") EntSpat.State = 0; else EntSpat.State = 4;
            return EntSpat;
        }

        public static TPolygon ES_ZU(string Definition, MP_V06.tContoursSubParcelContour ES)
        {

            netFteo.Spatial.TPolygon EntSpat = new netFteo.Spatial.TPolygon();
            EntSpat.Definition = Definition;
            //Первый (внешний) контур
            for (int iord = 0; iord <= ES.EntitySpatial.SpatialElement[0].SpelementUnit.Count - 1; iord++)
            {

                netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();

                Point.x = Convert.ToDouble(ES.EntitySpatial.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                Point.y = Convert.ToDouble(ES.EntitySpatial.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                Point.Mt = Convert.ToDouble(ES.EntitySpatial.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                //Point.Description = ES.SpatialElement[0].Spelement_Unit[iord].Ordinate[0].Geopoint_Zacrep;
                Point.Pref = ES.EntitySpatial.SpatialElement[0].SpelementUnit[iord].Ordinate.PointPref;
                Point.NumGeopointA = ES.EntitySpatial.SpatialElement[0].SpelementUnit[iord].Ordinate.NumGeopoint;
                EntSpat.AddPoint(Point);
            }
            //Внутренние контура
            for (int iES = 1; iES <= ES.EntitySpatial.SpatialElement.Count - 1; iES++)
            {
                netFteo.Spatial.TRing InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.EntitySpatial.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {

                    netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                    Point.x = Convert.ToDouble(ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                    Point.y = Convert.ToDouble(ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                    Point.Mt = Convert.ToDouble(ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                    Point.NumGeopointA = ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.NumGeopoint;
                    InLayer.AddPoint(Point);
                }
            }
            return EntSpat;
        }

        public static TEntitySpatial ES_SubParcels(MP_V06.tContoursSubParcelContourCollection cs)
        {
            TEntitySpatial res = new TEntitySpatial();
            foreach (MP_V06.tContoursSubParcelContour item in cs)
                res.Add(ES_ZU(item.Number, item));
            return res;
        }
        #endregion

        #region SpelementUnitZUOut parsers
        private static TPoint GetUnit(kpzu06.tSpelementUnitZUOut unit)
        {

            netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();

            Point.x = Convert.ToDouble(unit.Ordinate.X);
            Point.y = Convert.ToDouble(unit.Ordinate.Y);
            // Заполним также и старые ординаты, чтобы не вызывать появление флага "*"
            Point.oldX = Point.x;
            Point.oldY = Point.y;
            Point.Status = 4;
            if (unit.Ordinate.DeltaGeopointSpecified)
                Point.Mt = Convert.ToDouble(unit.Ordinate.DeltaGeopoint);
            Point.NumGeopointA = unit.SuNmb;

            return Point;
        }

        private static TPoint GetUnit(kvzu07.tSpelementUnitZUOut unit)
        {

            netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();

            Point.x = Convert.ToDouble(unit.Ordinate.X);
            Point.y = Convert.ToDouble(unit.Ordinate.Y);
            // Заполним также и старые ординаты, чтобы не вызывать появление флага "*"
            Point.oldX = Point.x;
            Point.oldY = Point.y;
            Point.Status = 4;
            if (unit.Ordinate.DeltaGeopointSpecified)
                Point.Mt = Convert.ToDouble(unit.Ordinate.DeltaGeopoint);
            Point.Definition = unit.SuNmb;
            return Point;
        }

        public static TPoint GetUnit(kpt09.tSpelementUnitZUOut unit)
        {

            netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();

            Point.x = Convert.ToDouble(unit.Ordinate.X);
            Point.y = Convert.ToDouble(unit.Ordinate.Y);
            // Заполним также и старые ординаты, чтобы не вызывать появление флага "*"
            Point.oldX = Point.x;
            Point.oldY = Point.y;
            Point.Status = 4;
            if (unit.Ordinate.DeltaGeopointSpecified)
                Point.Mt = Convert.ToDouble(unit.Ordinate.DeltaGeopoint);
            Point.NumGeopointA = unit.SuNmb;

            return Point;
        }

        public static TPoint GetUnit(kpt10_un.tSpelementUnitZUOut unit)
        {
            TPoint Point = new TPoint();
            Point.x = Convert.ToDouble(unit.Ordinate.X);
            Point.y = Convert.ToDouble(unit.Ordinate.Y);
            // Заполним также и старые ординаты, чтобы не вызывать появление флага "*"
            Point.oldX = Point.x;
            Point.oldY = Point.y;
            Point.Status = 4;
            if (unit.Ordinate.DeltaGeopointSpecified)
                Point.Mt = Convert.ToDouble(unit.Ordinate.DeltaGeopoint);
            Point.Definition = unit.SuNmb;
            return Point;
        }

        public static TPoint GetUnit(kpt10_un.tSpelementUnitLandOut unit)
        {
            TPoint Point = new TPoint();
            Point.x = Convert.ToDouble(unit.Ordinate.X);
            Point.y = Convert.ToDouble(unit.Ordinate.Y);
            // Заполним также и старые ординаты, чтобы не вызывать появление флага "*"
            Point.oldX = Point.x;
            Point.oldY = Point.y;
            Point.Status = 4;
            if (unit.Ordinate.DeltaGeopointSpecified)
                Point.Mt = Convert.ToDouble(unit.Ordinate.DeltaGeopoint);
            Point.Definition = unit.SuNmb;
            return Point;
        }


        #endregion




        #region-----------------Конвертация из ОИПД КПЗУ в ОИПД Fteo.Spatial
        public static netFteo.Spatial.TPolygon AddEntSpatKPZU05(string Definition, RRTypes.kpzu.tEntitySpatialZUOut ES)
        {
            netFteo.Spatial.TPolygon EntSpat = new netFteo.Spatial.TPolygon();
            EntSpat.Definition = Definition;
            if (ES == null) { return EntSpat; }


            //Первый (внешний) контур
            for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
            {

                netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                Point.NumGeopointA = ES.SpatialElement[0].SpelementUnit[iord].SuNmb;
                EntSpat.AddPoint(Point);
            }
            //Внутренние контура
            for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
            {
                netFteo.Spatial.TRing InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {

                    netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                    Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                    Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                    Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                    Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].SuNmb;
                    InLayer.AddPoint(Point);
                }
            }
            return EntSpat;
        }


        #endregion


        #region Cast Spatial data

        //TODO - перенос в CommonUtils
        /// <summary>
        /// Разбор ПД KPZU V06
        /// </summary>
        /// <param name="Definition"></param>
        /// <param name="ES"></param>
        /// <returns></returns>
        public static TPolygon AddEntSpatKPZU06(string Definition, RRTypes.kpzu06.tEntitySpatialZUOut ES)
        {
            TPolygon EntSpat = new TPolygon();
            EntSpat.Definition = Definition;
            if (ES == null) { return EntSpat; }


            //Первый (внешний) контур
            for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
            {
                EntSpat.AddPoint(GetUnit(ES.SpatialElement[0].SpelementUnit[iord]));
            }
            //Внутренние контура
            for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
            {
                netFteo.Spatial.TRing InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {
                    InLayer.AddPoint(GetUnit(ES.SpatialElement[iES].SpelementUnit[iord]));
                }
            }
            return EntSpat;
        }

        public static TEntitySpatial ParseES_KPZU06(string Definition, kpzu06.tEntitySpatialZUOut ES)
        {
            if (ES == null) { return null; }

            List<TRing> AllRings = new List<TRing>();
            //Parse all present rings:
            for (int iES = 0; iES <= ES.SpatialElement.Count - 1; iES++)
            {
                TRing InLayer = new TRing();

                for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {
                    InLayer.AddPoint(GetUnit(ES.SpatialElement[iES].SpelementUnit[iord]));
                }
                AllRings.Add(InLayer);
            }

            TEntitySpatial ParsingES = new TEntitySpatial(Definition);
            ParsingES.AddRange(AllRings);
            ParsingES.ParseSpatial();
            return ParsingES;
        }

        /// <summary>
        /// Разбор ПД KVZU V07
        /// </summary>
        /// <param name="Definition"></param>
        /// <param name="ES"></param>
        /// <returns></returns>
        public static TPolygon AddEntSpatKVZU07(string Definition, RRTypes.kvzu07.tEntitySpatialBordersZUOut ES)
        {

            TPolygon EntSpat = new TPolygon();
            EntSpat.Definition = Definition;
            if (ES == null) { return EntSpat; }

            //First ring (possible outer)
            for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
            {
                EntSpat.AddPoint(GetUnit(ES.SpatialElement[0].SpelementUnit[iord]));
            }

            //Внутренние контура
            for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
            {
                TRing InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {
                    InLayer.AddPoint(GetUnit(ES.SpatialElement[iES].SpelementUnit[iord]));
                }
            }
            return EntSpat;
        }

        /// <summary>
        /// Parse spatial as possible multiborders (MC Slot, Parcel)
        /// </summary>
        /// <param name="Definition"></param>
        /// <param name="ES"></param>
        /// <returns></returns>
        public static TEntitySpatial ParseES_KVZU07(string Definition, kvzu07.tEntitySpatialBordersZUOut ES)
        {
            if (ES == null) { return null; }
            List<TRing> AllRings = new List<TRing>();
            //Parse all present rings:
            for (int iES = 0; iES <= ES.SpatialElement.Count - 1; iES++)
            {
                TRing InLayer = new TRing();

                for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {
                    InLayer.AddPoint(GetUnit(ES.SpatialElement[iES].SpelementUnit[iord]));
                }
                AllRings.Add(InLayer);
            }

            TEntitySpatial ParsingES = new TEntitySpatial(Definition);
            ParsingES.AddRange(AllRings);
            ParsingES.ParseSpatial();
            return ParsingES;
        }


        /// <summary>
        /// KPT_09: parse some case for Zones with multiborders (like MC Zone)
        /// </summary>
        /// <param name="Definition"></param>
        /// <param name="ES"></param>
        /// <returns></returns>
        public static TEntitySpatial ParseES_KPT09LandES(string Definition, kpt09.tEntitySpatialLandOut ES)
        {
            if (ES == null) { return null; }

            List<TRing> AllRings = new List<TRing>();
            //Parse all present rings:
            for (int iES = 0; iES <= ES.SpatialElement.Count - 1; iES++)
            {
                TRing InLayer = new TRing();

                for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {
                    InLayer.AddPoint(GetUnit(ES.SpatialElement[iES].SpelementUnit[iord]));
                }
                AllRings.Add(InLayer);
            }

            TEntitySpatial ParsingES = new TEntitySpatial(Definition);
            ParsingES.AddRange(AllRings);
            ParsingES.ParseSpatial();
            return ParsingES;
        }

        /// <summary>
        /// parse some case for Zones with multiborders (like MC Zone)
        /// scanning and tracing contours occupied too long time...
        /// so these routine in progress of develop
        /// </summary>
        /// <param name="Definition"></param>
        /// <param name="ES"></param>
        /// <returns>Returned raw spatial, without parsing childs and parents</returns>
        public static TEntitySpatial ParseES_KPT10LandES(string Definition, kpt10_un.tEntitySpatialLandOut ES)
        {
            if (ES == null) { return null; }

            List<TRing> AllRings = new List<TRing>();
            //Parse all present rings:
            for (int iES = 0; iES <= ES.SpatialElement.Count - 1; iES++)
            {
                TRing InLayer = new TRing();

                for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                {
                    InLayer.AddPoint(GetUnit(ES.SpatialElement[iES].SpelementUnit[iord]));
                }
                AllRings.Add(InLayer);
            }

            TEntitySpatial ESParsed = new TEntitySpatial(Definition);
            ESParsed.State = 14; //raw spatial
            ESParsed.AddRange(AllRings); //returned raw spatial, without parsing childs and parents
            return ESParsed;
        }


        /*
        //Разбор ordinate KPT11
        private static TPoint KPT11_ES_ParseOrdinate(System.Xml.XmlNode Spelement_Unit)
        {
            TPoint Point = new TPoint();
            Point.x = Convert.ToDouble(Spelement_Unit.SelectSingleNode("x").FirstChild.Value);
            Point.y = Convert.ToDouble(Spelement_Unit.SelectSingleNode("y").FirstChild.Value);
            Point.oldX = Point.x;
            Point.oldY = Point.y;
            Point.Definition = netFteo.XML.XMLWrapper.SelectNodeChildValue(Spelement_Unit, "num_geopoint");
            if (netFteo.XML.XMLWrapper.SelectNodeChild(Spelement_Unit, "delta_geopoint") != null)
                Point.Mt = Convert.ToDouble(netFteo.XML.XMLWrapper.SelectNodeChildValue(Spelement_Unit, "delta_geopoint"));
            return Point;
        }
        */

     

        #endregion

    }


}


namespace RRTypes.CommonParsers
{
    public delegate void XMLParsingHandler(object sender, ESCheckingEventArgs e);


    /// <summary>
    /// Base class for parsers
    /// </summary>
    public class Parser
    {

        /// <summary>
        /// An event handler invoked during parsing  of entries in the xml file.
        /// </summary>
        /// <remarks>
        /// Able to check total progress of file parsing
        /// </remarks>
        public event XMLParsingHandler OnParsing;
        public event XMLParsingHandler OnStartParsing;
        protected int FileParsePosition; // Текущая позиция parser`a
        public int TotalItems2Process;
        protected void XMLParsingProc(string sender, int process, byte[] Data)
        {
            if (OnParsing == null) return;
            ESCheckingEventArgs e = new ESCheckingEventArgs();
            e.Definition = sender;
            e.Data = Data;
            e.Process = process;
            OnParsing(this, e);
        }

        protected void XMLParsingStartProc(string sender, int process, byte[] Data)
        {
            if (OnParsing == null) return;
            ESCheckingEventArgs e = new ESCheckingEventArgs();
            e.Definition = sender;
            e.Data = Data;
            e.Process = process;
            OnStartParsing(this, e);
        }

        /// <summary>
        /// Десериализатор для любых типов. 
        /// </summary>
        /// <typeparam name="T">Целевой тип "<T>"</typeparam>
        /// <param name="xmldoc">Исходный xml документ</param>
        /// <returns></returns>
        protected object Desearialize<T>(XmlDocument xmldoc)
        {
            Stream stream = new MemoryStream();
            xmldoc.Save(stream); // Dramatic MEMORY consumption!
            xmldoc = null;       // due missing IDisposable, set null
            stream.Seek(0, 0);
            XmlSerializer serializerKPT = new XmlSerializer(typeof(T));
            object TResult = (T)serializerKPT.Deserialize(stream);
            stream.Dispose();
            GC.Collect();
            return TResult;
        }


        protected object Desearialize<T>(Stream stream)
        {
            //   System.IO.Stream stream = new System.IO.MemoryStream();
            // xmldoc.Save(stream); //Dramatic MEMORY consumption!
            stream.Seek(0, 0);
            XmlSerializer serializerKPT = new XmlSerializer(typeof(T));
            object TResult = (T)serializerKPT.Deserialize(stream);
            return TResult;
        }

        protected netFteo.IO.FileInfo InitFileInfo(netFteo.IO.FileInfo fi, System.Xml.XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = new netFteo.IO.FileInfo();
            res.FileName = fi.FileName;
            res.FilePath = fi.FilePath;
            res.Namespace = fi.Namespace;
            res.DocRootName = fi.DocRootName;
            res.Version = fi.Version;
            //if present xmldoc, overwrite fields:
            if (xmldoc != null)
            {
                res.DocRootName = xmldoc.DocumentElement.Name;
                res.Namespace = xmldoc.DocumentElement.NamespaceURI;
                if (xmldoc.DocumentElement.Attributes.GetNamedItem("Version") != null) // Для MP версия в корне
                    res.Version = xmldoc.DocumentElement.Attributes.GetNamedItem("Version").Value;
            }
            return res;
        }

    }

    public class Doc2Type : Parser
    {
        public netFteo.XML.XSDFile dutilizations_v01;
        public netFteo.XML.XSDFile dAllowedUse_v02;

        public Doc2Type()
        {
        }

        public Doc2Type(netFteo.XML.XSDFile dUtil, netFteo.XML.XSDFile dAllowedUse)
        {
            this.dAllowedUse_v02 = dAllowedUse;
            this.dutilizations_v01 = dUtil;
        }

        /// <summary>
        /// Parsing node Reestr_Contractors for xml Version=04
        /// </summary>
        /// <param name="xmldoc">xml Version=04</param>
        /// <param name="res"></param>
        private static void Parse_ContractorsV04(System.Xml.XmlDocument xmldoc, netFteo.IO.FileInfo res)
        {
            System.Xml.XmlNode Contractors = netFteo.XML.XMLWrapper.Parse_Node(xmldoc, "Region_Cadastr_Vidimus_KV/Reestr_Contractors");
            if (Contractors != null)
                foreach (System.Xml.XmlNode contr in Contractors.ChildNodes) // in "Contractors/Contractor[]"
                {
                    TEngineerOut eng = new TEngineerOut();
                    eng.FamilyName = netFteo.XML.XMLWrapper.SelectNodeChildValue(contr, "Cadastral_Engineer/FIO/Surname");
                    eng.FirstName = netFteo.XML.XMLWrapper.SelectNodeChildValue(contr, "Cadastral_Engineer/FIO/First");
                    eng.Patronymic = netFteo.XML.XMLWrapper.SelectNodeChildValue(contr, "Cadastral_Engineer/FIO/Patronymic");
                    /*
                    eng.NCertificate = netFteo.XML.XMLWrapper.SelectNodeChildValue(contr, "Cadastral_Engineer/GUID_FL");

                    System.Xml.XmlNode NameNode = netFteo.XML.XMLWrapper.SelectNodeChild(netFteo.XML.XMLWrapper.SelectNodeChild(contr, "Organization"), "Name");
                    if (NameNode != null)
                        eng.Organization_Name = netFteo.XML.XMLWrapper.SelectNodeChildValue(NameNode, "Name"); //NameNode.FirstChild.Value;
                    */
                    res.Contractors.Add(eng);

                }
            {
            }
        }

        private static void Parse_Contractors(System.Xml.XmlDocument xmldoc, netFteo.IO.FileInfo res)
        {
            System.Xml.XmlNode Contractors = netFteo.XML.XMLWrapper.Parse_Node(xmldoc, "Contractors");

            if (Contractors != null)
                foreach (System.Xml.XmlNode contr in Contractors.ChildNodes) // in "Contractors/Contractor[]"
                {
                    TEngineerOut eng = new TEngineerOut();
                    eng.FamilyName = netFteo.XML.XMLWrapper.SelectNodeChildValue(contr, "FamilyName");
                    eng.FirstName = netFteo.XML.XMLWrapper.SelectNodeChildValue(contr, "FirstName");
                    eng.Patronymic = netFteo.XML.XMLWrapper.SelectNodeChildValue(contr, "Patronymic");
                    eng.NCertificate = netFteo.XML.XMLWrapper.SelectNodeChildValue(contr, "NCertificate");
                    eng.Date = (contr.Attributes.GetNamedItem("Date") != null) ? contr.Attributes.GetNamedItem("Date").Value : eng.Date = "";
                    System.Xml.XmlNode NameNode = netFteo.XML.XMLWrapper.SelectNodeChild(netFteo.XML.XMLWrapper.SelectNodeChild(contr, "Organization"), "Name");
                    if (NameNode != null)
                        eng.Organization_Name = NameNode.FirstChild.Value;//netFteo.XML.XMLWrapper.SelectNodeChildValue(NameNode, "Name"); //
                    res.Contractors.Add(eng);
                }
            {
            }
        }

        #region  Разбор MP 04

        //**************************************************************** 
        // Разбор Межевого Плана V04/V03
        //private void ParseSTDMPV04(RRTypes.STD_MPV04.STD_MP MP)
        public netFteo.IO.FileInfo ParseMPV04(netFteo.IO.FileInfo fi, System.Xml.XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            try
            {
                res.DocType = "Межевой план";
                res.DocTypeNick = "STD_MP";
                res.Version = xmldoc.DocumentElement.SelectSingleNode("eDocument/@Version").Value; // /STD_MP/eDocument/@Version
                res.Number = xmldoc.DocumentElement.SelectSingleNode("eDocument/@GUID").Value;


                TCadastralBlock Bl = new TCadastralBlock();

                if (res.Version == "03")
                {
                    res.Version = "03 in dev.";
                    res.Date = xmldoc.DocumentElement.SelectSingleNode("eDocument/Sender/@Date_Upload").Value;
                    res.Cert_Doc_Organization = xmldoc.DocumentElement.SelectSingleNode("eDocument/Sender/@Name").Value; ;

                    if (xmldoc.DocumentElement.SelectSingleNode("eDocument/@CodeType").Value == "014")
                    {
                        System.Xml.XmlNodeList FormParcels = xmldoc.DocumentElement.SelectNodes("/" + xmldoc.DocumentElement.Name + "/Package/FormParcels/NewParcel");
                        for (int iP = 0; iP <= FormParcels.Count - 1; iP++)
                        {
                            System.Xml.XmlNode parcel = FormParcels[iP];
                            Bl.CN = parcel.SelectSingleNode("CadastralBlock").FirstChild.Value;
                            TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(parcel.Attributes.GetNamedItem("Definition").Value, parcel.Attributes.GetNamedItem("Name").Value));
                            if (parcel.SelectSingleNode("Entity_Spatial") != null)
                            {
                                TPolygon ents = KPT08LandEntSpatToFteo(parcel.Attributes.GetNamedItem("Definition").Value,
                                                                          parcel.SelectSingleNode("Entity_Spatial"));
                                ents.AreaValue = (decimal)Convert.ToDouble(parcel.SelectSingleNode("Area/Area").FirstChild.Value);
                                ents.Parent_Id = MainObj.id;
                                ents.Definition = parcel.Attributes.GetNamedItem("Definition").Value;
                                MainObj.EntSpat.Add(ents);
                            }
                        }
                    }

                    if (xmldoc.DocumentElement.SelectSingleNode("eDocument/@CodeType").Value == "015")
                    {
                        System.Xml.XmlNode ExistParcel = xmldoc.DocumentElement.SelectSingleNode("/" + xmldoc.DocumentElement.Name + "/Package/SpecifyParcels/ExistParcel");
                        if (ExistParcel != null)
                        {
                            Bl.CN = ExistParcel.SelectSingleNode("CadastralBlock").FirstChild.Value;
                            /// /STD_MP/Contractor/Cadastral_Engineer/FIO/Surname
                            /// /STD_MP/Contractor/Cadastral_Engineer/FIO/Surname/#text




                            if (ExistParcel.SelectSingleNode("Entity_Spatial") != null)
                            {
                                TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(ExistParcel.SelectSingleNode("@CadastralNumber").Value, "Item01"));
                                TPolygon ents = KPT08LandEntSpatToFteo(ExistParcel.Attributes.GetNamedItem("CadastralNumber").Value,
                                                                          ExistParcel.SelectSingleNode("Entity_Spatial"));
                                ents.AreaValue = (decimal)Convert.ToDouble(ExistParcel.SelectSingleNode("Area/Area").FirstChild.Value);
                                ents.Parent_Id = MainObj.id;
                                ents.Definition = ExistParcel.Attributes.GetNamedItem("CadastralNumber").Value;
                                MainObj.EntSpat.Add(ents);
                            }

                            //Многоконтурный
                            if (ExistParcel.SelectSingleNode("Contours") != null)
                            {
                                TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(ExistParcel.SelectSingleNode("@CadastralNumber").Value, "Item05"));
                                System.Xml.XmlNode contours = ExistParcel.SelectSingleNode("Contours");
                                string cn = ExistParcel.Attributes.GetNamedItem("CadastralNumber").Value;
                                for (int ic = 0; ic <= ExistParcel.SelectSingleNode("Contours").ChildNodes.Count - 1; ic++)
                                {
                                    /*  /Entity_Spatial/Spatial_Element/Spelement_Unit[1]/Ordinate/@X */
                                    // / Contours / Contour[1] / @Number_PP
                                    TPolygon NewCont = KPT08LandEntSpatToFteo(MainObj.CN + "(" +
                                                                          ExistParcel.SelectSingleNode("Contours").ChildNodes[ic].Attributes.GetNamedItem("Number_PP").Value + ")",
                                                                          contours.ChildNodes[ic].SelectSingleNode("Entity_Spatial"));
                                    MainObj.EntSpat.Add(NewCont);
                                }
                            }


                        }
                    }
                }

                if (res.Version == "04")
                {
                    STD_MPV04.STD_MP MP = (STD_MPV04.STD_MP)Desearialize<STD_MPV04.STD_MP>(xmldoc);
                    res.District.CSs.Add(new TCoordSystem(MP.Coord_Systems.Coord_System.Name, MP.Coord_Systems.Coord_System.Cs_Id));
                    if (MP.eDocument.CodeType ==STD_MPV04.STD_MPEDocumentCodeType.Item014)
                    {
                        //  richTextBox1.AppendText("\n014 - Пакет информации c заявлением о постановке на учет: \n");
                        if (MP.Package.FormParcels != null)
                            for (int i = 0; i <= MP.Package.FormParcels.NewParcel.Count - 1; i++)
                            {
                                string ParcelName;
                                if (MP.Package.FormParcels.NewParcel[i].Contours != null & MP.Package.FormParcels.NewParcel[i].Contours.Count > 0)
                                    ParcelName = "Item05";
                                else
                                    ParcelName = "Item01";
                                TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(MP.Package.FormParcels.NewParcel[i].Definition, ParcelName));
                                MainObj.AreaGKN = MP.Package.FormParcels.NewParcel[i].Area.Area;//Вычисленную!!
                                MainObj.CadastralBlock = MP.Package.FormParcels.NewParcel[i].CadastralBlock;
                                MainObj.Location.Address.Note = MP.Package.FormParcels.NewParcel[i].Location.District.Type + " " +
                                                       MP.Package.FormParcels.NewParcel[i].Location.District.Name + ", " +
                                                       MP.Package.FormParcels.NewParcel[i].Location.Locality.Type + " " +
                                                       MP.Package.FormParcels.NewParcel[i].Location.Locality.Name;//Что здесь?
                                MainObj.Location.Address.Region = MP.Package.FormParcels.NewParcel[i].Location.Region.ToString();
                                MainObj.Utilization.UtilbyDoc = MP.Package.FormParcels.NewParcel[i].Utilization.ByDoc;

                                if (MP.Package.FormParcels.NewParcel[i].Entity_Spatial != null)
                                    if (MP.Package.FormParcels.NewParcel[i].Entity_Spatial.Spatial_Element.Count > 0)
                                        MainObj.EntSpat.Add(STD_MP_Utils.AddEntSpatSTDMP4("", MP.Package.FormParcels.NewParcel[i].Entity_Spatial));
                                if (MP.Package.FormParcels.NewParcel[i].Contours != null)
                                    for (int ic = 0; ic <= MP.Package.FormParcels.NewParcel[i].Contours.Count - 1; ic++)
                                    {
                                        TPolygon NewCont = STD_MP_Utils.AddEntSpatSTDMP4(MP.Package.FormParcels.NewParcel[i].Contours[ic].Definition,
                                                                                        MP.Package.FormParcels.NewParcel[i].Contours[ic].Entity_Spatial);
                                        MainObj.EntSpat.Add(NewCont);
                                        /*  RRTypes.RetResult Checkresut = RRTypes.STD_MP_Utils.CheckESMP4(MP.Package.FormParcels.NewParcel[i].Contours[ic].Entity_Spatial);
										  if (Checkresut.HasError)
										  {
											  MessageBox.Show("Незамкнутый контур", "Проверка ОИПД",
																  MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
										  }
										  */

                                    }

                                //Части в образуемом участке:
                                if (MP.Package.FormParcels.NewParcel[i].SubParcels != null)
                                    if (MP.Package.FormParcels.NewParcel[i].SubParcels.Count > 0)
                                        for (int ii = 0; ii <= MP.Package.FormParcels.NewParcel[i].SubParcels.Count - 1; ii++)
                                        {
                                            TSlot Sl = new TSlot();
                                            Sl.NumberRecord = MP.Package.FormParcels.NewParcel[i].SubParcels[ii].Definition;
                                            Sl.Encumbrances.Add(new netFteo.Rosreestr.TMyEncumbrance() { Name = MP.Package.FormParcels.NewParcel[i].SubParcels[ii].Encumbrance.Name });
                                            MainObj.SubParcels.Add(Sl);
                                        }
                            }

                    }

                    if (MP.eDocument.CodeType == RRTypes.STD_MPV04.STD_MPEDocumentCodeType.Item015)
                    //Учет изменений
                    {
                        // richTextBox1.AppendText("\n015 - пакет информации с заявлением о внесении изменений: \n");

                        if (MP.Package.SpecifyParcel != null)
                            if (MP.Package.SpecifyParcel.ExistParcel != null)
                            {

                                if (MP.Package.SpecifyParcel.ExistParcel.Entity_Spatial != null)
                                {
                                    TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(MP.Package.SpecifyParcel.ExistParcel.CadastralNumber, "Item01"));
                                    MainObj.EntSpat.AddPolygon(STD_MP_Utils.AddEntSpatSTDMP4(MP.Package.SpecifyParcel.ExistParcel.CadastralNumber,
                                                                              MP.Package.SpecifyParcel.ExistParcel.Entity_Spatial));

                                }

                                if (MP.Package.SpecifyParcel.ExistParcel.Contours != null)
                                {
                                    TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(MP.Package.SpecifyParcel.ExistParcel.CadastralNumber, "Item05"));
                                    MainObj.AreaGKN = MP.Package.SpecifyParcel.ExistParcel.Area.Area;//Вычисленную!!
                                    MainObj.Location.Address.Note = MP.Package.SpecifyParcel.ExistParcel.Note;//Что здесь?
                                                                                                              //MainObj.SpecialNote  = ;//Что здесь?
                                    for (int ic = 0; ic <= MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour.Count - 1; ic++)
                                    {
                                        TPolygon NewCont = STD_MP_Utils.AddEntSpatSTDMP4(MP.Package.SpecifyParcel.ExistParcel.CadastralNumber + "(" +
                                                                              MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour[ic].Definition + ")",
                                                                              MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour[ic].Entity_Spatial);
                                        MainObj.EntSpat.Add(NewCont);
                                    }

                                }

                            }
                        //Если только образование частей:
                        if (MP.Package.NewSubParcel != null)
                            if (MP.Package.NewSubParcel.Count > 0)
                            {

                                TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(MP.Package.NewSubParcel[0].CadastralNumber_Parcel, "Item01"));
                                if (MP.Package.NewSubParcel.Count > 0)
                                    for (int ii = 0; ii <= MP.Package.NewSubParcel.Count - 1; ii++)
                                    {
                                        TSlot Sl = new TSlot();
                                        Sl.NumberRecord = MP.Package.NewSubParcel[ii].Definition;
                                        Sl.Encumbrances.Add(new netFteo.Rosreestr.TMyEncumbrance() { Name = MP.Package.NewSubParcel[ii].Encumbrance.Name });
                                        Sl.AreaGKN = MP.Package.NewSubParcel[ii].Area.Area;
                                        if (MP.Package.NewSubParcel[ii].Entity_Spatial != null) //Если одноконтурная чзу
                                            Sl.SpatialElement = RRTypes.STD_MP_Utils.AddSubParcelESTDMP4(MP.Package.NewSubParcel[ii].Definition, MP.Package.NewSubParcel[ii].Entity_Spatial);
                                        MainObj.SubParcels.Add(Sl);
                                    }

                            }

                    }

                    if (MP.Conclusion != null)
                    {
                        res.Comments += ("<br>");
                        res.Comments += ("\n______________________________________ЗАКЛЮЧЕНИЕ_____________________________________");
                        res.Comments += ("\n");
                        res.Comments += (MP.Conclusion);

                        res.CommentsType = "Заключение КИ";
                    }

                    if (MP.Title != null)
                    {
                        res.Comments += (MP.Title.Reason);
                        res.Date = MP.Title.Contractor.Date.ToString();
                        if (MP.Title.Contractor.Organization != null)
                        {
                            res.Cert_Doc_Organization = MP.Title.Contractor.Organization +
                                              "  " + MP.Title.Contractor.Address;
                        }

                        res.Appointment = MP.Title.Contractor.N_Certificate + " " +
                                          MP.Title.Contractor.Telephone;

                        //   / STD_MP / Contractor / Cadastral_Engineer / FIO / Surname

                        res.AppointmentFIO = MP.Title.Contractor.FIO.Surname + " " +
                                         MP.Title.Contractor.FIO.First + " " +
                                         MP.Title.Contractor.FIO.Patronymic + "\r" +
                                         MP.Title.Contractor.E_mail;


                        res.Contractors.Add(
                                   new TEngineerOut()
                                   {
                                       Date = MP.Title.Contractor.Date.ToString().Replace("0:00:00", ""),
                                       FamilyName = MP.Title.Contractor.FIO.Surname,
                                       FirstName = MP.Title.Contractor.FIO.First,
                                       Patronymic = MP.Title.Contractor.FIO.Patronymic,
                                       NCertificate = MP.Title.Contractor.N_Certificate,
                                       Email = MP.Title.Contractor.E_mail,
                                       Organization_Name = MP.Title.Contractor.Organization != null ? MP.Title.Contractor.Organization : "",
                                       AddressOrganization = MP.Title.Contractor.Address != null ? MP.Title.Contractor.Address : ""

                                   });
                    }

                }

                res.District.Blocks.Add(Bl);
                res.Comments += ("\n");
                res.Comments += ("<br>_______________________________________ТИТУЛЬНЫЙ ЛИСТ ___________________________________");
                res.Comments += ("<br> Межевой план подготовлен в результате выполнения кадастровых работ в связи с:");
                res.Comments += ("\n");



                CommonCast.CasterEGRP.Parse_DocumentProperties(xmldoc, res);
            }
            catch (Exception ex)
            {
                res.CommentsType = "Exception";
                res.Comments = ex.Message;
                return res;
            }
            return res;
        }

        //**************************************************************** 
        #endregion

        #region  Разбор MP 05
        public netFteo.IO.FileInfo ParseMPV05(netFteo.IO.FileInfo fi, System.Xml.XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            try
            {
                res.DocType = "Межевой план";
                res.DocTypeNick = "MP";
                MP_V05.MP MP = (MP_V05.MP)Desearialize<MP_V05.MP>(xmldoc);
                res.Number = MP.GUID;

                /*

				label_DocType.Text = "Межевой план";
				richTextBox1.AppendText("\n");
				richTextBox1.AppendText("\n____________________________________ТИТУЛЬНЫЙ ЛИСТ___________________________________");
				richTextBox1.AppendText("\nМежевой план подготовлен в результате выполнения кадастровых работ в связи с:");
				richTextBox1.AppendText("\n");
				richTextBox1.AppendText(MP.GeneralCadastralWorks.Reason);
				*/
                /*
				if (MP.Conclusion != null)
				{
					richTextBox1.AppendText("\n");
					richTextBox1.AppendText("\n______________________________________ЗАКЛЮЧЕНИЕ_____________________________________");
					richTextBox1.AppendText("\n");
					richTextBox1.AppendText(MP.Conclusion);
					richTextBox1.AppendText("\n");
				}
				*/
                /*
				ListViewItem LVi = new ListViewItem();
				ListViewItem LViadd = new ListViewItem();
				ListViewItem LViorg = new ListViewItem();
				ListViewItem LViTel = new ListViewItem();
				*/

                /*
				LVi.Text = MP.GeneralCadastralWorks.DateCadastral.ToString();
				LVi.SubItems.Add(MP.GeneralCadastralWorks.Contractor.FamilyName + " " + MP.GeneralCadastralWorks.Contractor.FirstName +
									 " " + MP.GeneralCadastralWorks.Contractor.Patronymic);
				LVi.SubItems.Add(MP.GeneralCadastralWorks.Contractor.NCertificate);

				if (MP.GeneralCadastralWorks.Contractor.Organization != null)
				{
					LVi.SubItems.Add(MP.GeneralCadastralWorks.Contractor.Organization.Name);
				}


				LViadd.SubItems.Add(MP.GeneralCadastralWorks.Contractor.Address);
				LViadd.SubItems.Add("-");
				if (MP.GeneralCadastralWorks.Contractor.Organization != null)
				{
					LViadd.SubItems.Add(MP.GeneralCadastralWorks.Contractor.Organization.AddressOrganization);
				}

				LViorg.SubItems.Add(MP.GeneralCadastralWorks.Contractor.Email);
				LViTel.SubItems.Add(MP.GeneralCadastralWorks.Contractor.Telephone);
				listView_Contractors.Items.Add(LVi);
				listView_Contractors.Items.Add(LViadd);
				listView_Contractors.Items.Add(LViorg);
				listView_Contractors.Items.Add(LViTel);
				textBox_DocNum.Text = MP.GUID;
				textBox_DocDate.Text = MP.GeneralCadastralWorks.DateCadastral.ToString("dd/MM/yyyy");
				*/

                TCadastralBlock Bl = new TCadastralBlock();
                string ParcelName;
                if (MP.Package.FormParcels != null)
                {
                    for (int i = 0; i <= MP.Package.FormParcels.NewParcel.Count - 1; i++)
                    {
                        if (MP.Package.FormParcels.NewParcel[i].Contours != null & MP.Package.FormParcels.NewParcel[i].Contours.Count > 0)
                            ParcelName = "Item05";
                        else
                            ParcelName = "Item01";
                        TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(MP.Package.FormParcels.NewParcel[i].Definition, ParcelName));
                        MainObj.AreaGKN = MP.Package.FormParcels.NewParcel[i].Area.Area;//Вычисленную??
                        MainObj.Category = MP.Package.FormParcels.NewParcel[i].Category.Category.ToString();// netFteo.Rosreestr.dCategoriesv01.ItemToName(MP.Package.FormParcels.NewParcel[i].Category.Category.ToString());
                        MainObj.Utilization.UtilbyDoc = MP.Package.FormParcels.NewParcel[i].Utilization.ByDoc;
                        MainObj.Utilization.Untilization = MP.Package.FormParcels.NewParcel[i].Utilization.Utilization.ToString();
                        if (MP.Package.FormParcels.NewParcel[i].EntitySpatial != null)
                        {
                            MainObj.EntSpat.Add(CasterSpatial.AddEntSpatMP5("", MP.Package.FormParcels.NewParcel[i].EntitySpatial));
                        }

                        if (MP.Package.FormParcels.NewParcel[i].Contours != null)
                            for (int ic = 0; ic <= MP.Package.FormParcels.NewParcel[i].Contours.Count - 1; ic++)
                            {
                                MainObj.EntSpat.Add(CasterSpatial.AddEntSpatMP5(MP.Package.FormParcels.NewParcel[i].Contours[ic].Definition,
                                                                                MP.Package.FormParcels.NewParcel[i].Contours[ic].EntitySpatial));
                            }
                    }

                }


                //Если Мп по уточнению:
                if (MP.Package.SpecifyParcel != null)
                {
                    //уточнение ЗУ, МКЗУ 
                    if (MP.Package.SpecifyParcel.ExistParcel != null)
                    {
                        ParcelName = "Item01"; // default
                        if (MP.Package.SpecifyParcel.ExistParcel.Contours != null)
                            if (MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour.Count > 0 || MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour.Count > 0)

                                ParcelName = "Item05";
                            else
                                ParcelName = "Item01";


                        TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(MP.Package.SpecifyParcel.ExistParcel.CadastralNumber, ParcelName));
                        MainObj.AreaGKN = MP.Package.SpecifyParcel.ExistParcel.Area.Area;//Вычисленную??
                        if (MP.Package.SpecifyParcel.ExistParcel.Contours != null)
                        {
                            for (int ic = 0; ic <= MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour.Count - 1; ic++)
                            {
                                MainObj.EntSpat.Add(CasterSpatial.AddEntSpatMP5(MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour[ic].Definition,
                                                                                MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour[ic].EntitySpatial));

                            }
                            for (int ic = 0; ic <= MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour.Count - 1; ic++)
                            {
                                MainObj.EntSpat.Add(CasterSpatial.AddEntSpatMP5(MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour[ic].NumberRecord,
                                                                                MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour[ic].EntitySpatial));
                            }
                        }

                        if (MP.Package.SpecifyParcel.ExistParcel.EntitySpatial != null)
                            MainObj.EntSpat.Add(CasterSpatial.AddEntSpatMP5("", MP.Package.SpecifyParcel.ExistParcel.EntitySpatial));

                    }
                    // Уточнение ЕЗП
                    if (MP.Package.SpecifyParcel.ExistEZ != null)
                    {
                        ParcelName = "Item02"; // 02 = ЕЗП RRCommon.cs,  там есть public static class dParcelsv01
                        TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.CadastralNumber, ParcelName));
                    }
                }
                //Только образование частей 
                if (MP.Package.SubParcels != null)
                {
                    ParcelName = "Item06";  // Значение отсутствует
                    TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(MP.Package.SubParcels.CadastralNumberParcel, ParcelName));

                    if (MP.Package.SubParcels.NewSubParcel.Count > 0)
                        for (int ii = 0; ii <= MP.Package.SubParcels.NewSubParcel.Count - 1; ii++)
                        {
                            TSlot Sl = new TSlot();
                            Sl.NumberRecord = MP.Package.SubParcels.NewSubParcel[ii].Definition;
                            Sl.Encumbrances.Add(new netFteo.Rosreestr.TMyEncumbrance() { Name = MP.Package.SubParcels.NewSubParcel[ii].Encumbrance.Name });
                            Sl.AreaGKN = MP.Package.SubParcels.NewSubParcel[ii].Area.Area;
                            if (MP.Package.SubParcels.NewSubParcel[ii].EntitySpatial != null) //Если одноконтурная чзу
                                Sl.SpatialElement.ImportPolygon(CasterSpatial.AddEntSpatMP5(MP.Package.SubParcels.NewSubParcel[ii].Definition, MP.Package.SubParcels.NewSubParcel[ii].EntitySpatial));
                            if (MP.Package.SubParcels.NewSubParcel[ii].Contours != null)
                                Sl.ES = CasterSpatial.AddContoursMP5(MP.Package.SubParcels.NewSubParcel[ii].Definition, MP.Package.SubParcels.NewSubParcel[ii].Contours);


                            MainObj.SubParcels.Add(Sl);
                        }



                }

                res.Comments += ("\n");
                res.Comments += ("<br>_______________________________________ТИТУЛЬНЫЙ ЛИСТ ___________________________________");
                res.Comments += ("<br> Межевой план подготовлен в результате выполнения кадастровых работ в связи с:");
                res.Comments += ("\n");
                res.Comments += (MP.GeneralCadastralWorks.Reason);


                if (MP.Conclusion != null)
                {
                    res.Comments += ("<br>");
                    res.Comments += ("\n______________________________________ЗАКЛЮЧЕНИЕ_____________________________________");
                    res.Comments += ("\n");
                    res.Comments += (MP.Conclusion);
                    /*
                    res.Comments += "<br>" +
                                           MP.GeneralCadastralWorks.DateCadastral.ToString("dd.MM.yyyy") + "<br>" +
                                           MP.GeneralCadastralWorks.Contractor.FamilyName + " " +
                                           MP.GeneralCadastralWorks.Contractor.FirstName + " " +
                                           MP.GeneralCadastralWorks.Contractor.Patronymic + "<br>";
                    */
                }
                res.CommentsType = "Заключение КИ";
                res.Date = MP.GeneralCadastralWorks.DateCadastral.ToString("dd.MM.yyyy").Replace("0:00:00", "date");
                if (MP.GeneralCadastralWorks.Contractor.Organization != null)
                {
                    res.Cert_Doc_Organization = MP.GeneralCadastralWorks.Contractor.Organization.Name +
                                      "  " + MP.GeneralCadastralWorks.Contractor.Organization.AddressOrganization;
                }

                res.Appointment = MP.GeneralCadastralWorks.Contractor.NCertificate + " " +
                                  MP.GeneralCadastralWorks.Contractor.Telephone;
                res.AppointmentFIO = MP.GeneralCadastralWorks.Contractor.FamilyName + " " +
                                 MP.GeneralCadastralWorks.Contractor.FirstName + " " +
                                 MP.GeneralCadastralWorks.Contractor.Patronymic + "\r" +
                                 MP.GeneralCadastralWorks.Contractor.Email;


                res.Contractors.Add(
                           new TEngineerOut()
                           {
                               Date = MP.GeneralCadastralWorks.DateCadastral.ToString().Replace("0:00:00", ""),
                               FamilyName = MP.GeneralCadastralWorks.Contractor.FamilyName,
                               FirstName = MP.GeneralCadastralWorks.Contractor.FirstName,
                               Patronymic = MP.GeneralCadastralWorks.Contractor.Patronymic,
                               NCertificate = MP.GeneralCadastralWorks.Contractor.NCertificate,
                               Email = MP.GeneralCadastralWorks.Contractor.Email,
                               Organization_Name = MP.GeneralCadastralWorks.Contractor.Organization != null ? MP.GeneralCadastralWorks.Contractor.Organization.Name : "",
                               AddressOrganization = MP.GeneralCadastralWorks.Contractor.Organization != null ? MP.GeneralCadastralWorks.Contractor.Organization.AddressOrganization : ""

                           });
                foreach (MP_V05.tClientIdentify client in MP.GeneralCadastralWorks.Clients)
                {
                    if (client.Organization != null)
                    {
                        res.ReceivName = client.Organization.Name;
                        res.RequeryNumber = client.Organization.INN + ", " + client.Organization.OGRN;
                    }
                    if (client.Person != null)
                    {
                        res.ReceivName = client.Person.FamilyName + " " + client.Person.FirstName + " " + client.Person.Patronymic;
                        //res.RequeryNumber = client.Person..SNILS + ",  " + client.Person.Address;
                    }

                }
                if (MP.InputData.GeodesicBases  != null)
                    foreach (MP_V05.tSetOfPoint oms in MP.InputData.GeodesicBases)
                    {
                        GeodethicBase pt = new GeodethicBase(); 
                        pt.x = Convert.ToDouble(oms.OrdX);
                        pt.y = Convert.ToDouble(oms.OrdY);
                        pt.PName = oms.PName;
                        pt.PKind = oms.PKind; //Type
                        pt.PKlass = oms.PKlass; // NetWork class
                        Bl.AddOmsPoint(pt);
                    }
                res.District.Blocks.Add(Bl);
            }
            catch (Exception ex)
            {
                res.CommentsType = "Exception";
                res.Comments = ex.Message;
                return res;
            }
            return res;
        }
        #endregion

        #region  Разбор MP 06
        public netFteo.IO.FileInfo ParseMPV06(netFteo.IO.FileInfo fi, System.Xml.XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            try
            {
                res.DocType = "Межевой план";
                res.DocTypeNick = "MP";
                RRTypes.MP_V06.MP MP = (RRTypes.MP_V06.MP)Desearialize<RRTypes.MP_V06.MP>(xmldoc);
                res.Number = MP.GUID;
                for (int i = 0; i <= MP.CoordSystems.Count - 1; i++)
                {
                    res.District.CSs.Add(new TCoordSystem(MP.CoordSystems[i].Name, MP.CoordSystems[i].CsId));

                }
                TCadastralBlock Bl = new TCadastralBlock();
                string ParcelName;
                //МП по образованию
                if (MP.Package.FormParcels != null)
                {

                    for (int i = 0; i <= MP.Package.FormParcels.NewParcel.Count - 1; i++)
                    {
                        TParcel MainObj = Bl.Parcels.AddParcel(new TParcel());
                        MainObj.CadastralBlock = MP.Package.FormParcels.NewParcel[i].CadastralBlock;
                        Bl.CN = MP.Package.FormParcels.NewParcel[i].CadastralBlock;
                        MainObj.Definition = MP.Package.FormParcels.NewParcel[i].Definition;
                        if (MP.Package.FormParcels.NewParcel[i].PrevCadastralNumbers != null)
                            MainObj.PrevCadastralNumbers.AddRange(MP.Package.FormParcels.NewParcel[i].PrevCadastralNumbers);
                        MainObj.AreaGKN = MP.Package.FormParcels.NewParcel[i].Area.Area;//Вычисленную??
                        MainObj.Category = MP.Package.FormParcels.NewParcel[i].Category.Category.ToString();// netFteo.Rosreestr.dCategoriesv01.ItemToName(MP.Package.FormParcels.NewParcel[i].Category.Category.ToString());
                        MainObj.Location.Address = RRTypes.CommonCast.CasterZU.CastAddress(MP.Package.FormParcels.NewParcel[i].Address);
                        if (MP.Package.FormParcels.NewParcel[i].Utilization != null)
                            MainObj.Utilization.UtilbyDoc = MP.Package.FormParcels.NewParcel[i].Utilization.ByDoc;
                        if (MP.Package.FormParcels.NewParcel[i].LandUse != null)
                            MainObj.Landuse.Land_Use = MP.Package.FormParcels.NewParcel[i].LandUse.LandUse.ToString();
                        if (MP.Package.FormParcels.NewParcel[i].Contours != null & MP.Package.FormParcels.NewParcel[i].Contours.Count > 0)
                            MainObj.EntSpat.AddRange(CasterSpatial.ES_ZU(MP.Package.FormParcels.NewParcel[i].Contours));
                        if (MP.Package.FormParcels.NewParcel[i].EntitySpatial != null)
                        {
                            MainObj.EntSpat.Add(CasterSpatial.ES_ZU(MainObj.CN, MP.Package.FormParcels.NewParcel[i].EntitySpatial));
                        }

                        if (MP.Package.FormParcels.NewParcel[i].ObjectRealty != null)

                            MainObj.InnerCadastralNumbers.AddRange(MP.Package.FormParcels.NewParcel[i].ObjectRealty.InnerCadastralNumbers);

                    }
                    //измененный зу
                    if (MP.Package.FormParcels.ChangeParcel != null)
                    {
                        foreach (RRTypes.MP_V06.tChangeParcel chzSrc in MP.Package.FormParcels.ChangeParcel)
                        {
                            TParcel chzObj = Bl.Parcels.AddParcel(new TParcel(chzSrc.CadastralNumber));
                            chzObj.CadastralBlock = chzSrc.CadastralBlock;
                            chzObj.CompozitionEZ = new TCompozitionEZ();
                            if (chzSrc.DeleteEntryParcels != null)
                            {
                                foreach (RRTypes.MP_V06.tCadastralNumberInp cn in chzSrc.DeleteEntryParcels)
                                    chzObj.CompozitionEZ.DeleteEntryParcels.Add(cn.CadastralNumber);
                            }

                            if (chzSrc.TransformationEntryParcels != null)
                            {
                                foreach (RRTypes.MP_V06.tCadastralNumberInp cn2 in chzSrc.TransformationEntryParcels)
                                    chzObj.CompozitionEZ.TransformationEntryParcel.Add(cn2.CadastralNumber);
                            }
                        }
                    }
                }


                //Если Мп по уточнению:
                if (MP.Package.SpecifyParcel != null)
                {
                    //уточнение ЗУ, МКЗУ 
                    if (MP.Package.SpecifyParcel.ExistParcel != null)
                    {
                        ParcelName = "Item01"; // default
                        if (MP.Package.SpecifyParcel.ExistParcel.Contours != null)
                            if (MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour.Count > 0 || MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour.Count > 0)

                                ParcelName = "Item05";
                            else
                                ParcelName = "Item01";


                        TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(MP.Package.SpecifyParcel.ExistParcel.CadastralNumber, ParcelName));
                        Bl.CN = MP.Package.SpecifyParcel.ExistParcel.CadastralBlock;
                        MainObj.AreaGKN = MP.Package.SpecifyParcel.ExistParcel.AreaInGKN;
                        MainObj.AreaValue = MP.Package.SpecifyParcel.ExistParcel.Area.Area; //Указанная площадь??

                        if (MP.Package.SpecifyParcel.ExistParcel.ObjectRealty != null)
                            MainObj.InnerCadastralNumbers.AddRange(MP.Package.SpecifyParcel.ExistParcel.ObjectRealty.InnerCadastralNumbers);

                        if (MP.Package.SpecifyParcel.ExistParcel.Contours != null)
                        {
                            for (int ic = 0; ic <= MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour.Count - 1; ic++)
                            {
                                TPolygon NewCont = CasterSpatial.ES_ZU(MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour[ic].Definition,
                                                                                MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour[ic].EntitySpatial);
                                NewCont.AreaValue = MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour[ic].Area.Area;
                                MainObj.EntSpat.Add(NewCont);
                            }

                            for (int ic = 0; ic <= MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour.Count - 1; ic++)
                            {
                                TPolygon ExistCont = CasterSpatial.ES_ZU(MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour[ic].NumberRecord,
                                                                                MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour[ic].EntitySpatial);
                                ExistCont.AreaValue = MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour[ic].Area.Area;
                                MainObj.EntSpat.Add(ExistCont);
                            }
                        }

                        if (MP.Package.SpecifyParcel.ExistParcel.EntitySpatial != null)
                            MainObj.EntSpat.Add(CasterSpatial.ES_ZU("", MP.Package.SpecifyParcel.ExistParcel.EntitySpatial));

                    }

                    // Уточнение ЕЗП
                    if (MP.Package.SpecifyParcel.ExistEZ != null)
                    {
                        res.DocTypeNick = "Уточнение ЗУ";
                        ParcelName = "Item02"; // 02 = ЕЗП RRCommon.cs,  там есть public static class dParcelsv01
                        TParcel MainObj = new TParcel(MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.CadastralNumber, ParcelName);
                        Bl.Parcels.AddParcel(MainObj);
                        MainObj.AreaGKN = MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.AreaInGKN;
                        MainObj.AreaValue = MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.Area.Area;
                        if (MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.ObjectRealty != null)
                        {
                            MainObj.InnerCadastralNumbers.AddRange(MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.ObjectRealty.InnerCadastralNumbers);
                        }

                        if (MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.ObjectRealty != null)
                            MainObj.InnerCadastralNumbers.AddRange(MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.ObjectRealty.InnerCadastralNumbers);

                        Bl.CN = MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.CadastralBlock;

                        if (MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.CompositionEZ != null)
                        {
                            if (MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.CompositionEZ.DeleteEntryParcels != null)
                            {
                                foreach (RRTypes.MP_V06.tCadastralNumberInp cn in MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.CompositionEZ.DeleteEntryParcels)
                                    MainObj.CompozitionEZ.DeleteEntryParcels.Add(cn.CadastralNumber);
                            }
                        }

                        foreach (RRTypes.MP_V06.tExistEZEntryParcel entry in MP.Package.SpecifyParcel.ExistEZ.ExistEZEntryParcels)
                        {
                            MainObj.AddEntry(entry.CadastralNumber,
                                                           entry.Area.Area,
                                                           entry.Area.Inaccuracy,
                                                           6, // для межевого плана входящие только учтеные

                                                          CasterSpatial.ES_ZU(entry.CadastralNumber,
                                                          entry.EntitySpatial));
                        }
                    }
                }

                //Только образование частей 
                if (MP.Package.SubParcels != null)
                {
                    ParcelName = "Item06";  // Значение отсутствует
                    TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(MP.Package.SubParcels.CadastralNumberParcel, ParcelName));
                    if (MP.Package.SubParcels.NewSubParcel.Count > 0)
                        for (int ii = 0; ii <= MP.Package.SubParcels.NewSubParcel.Count - 1; ii++)
                        {
                            TSlot Sl = new TSlot();
                            Sl.NumberRecord = MP.Package.SubParcels.NewSubParcel[ii].Definition;

                            Sl.Encumbrances.Add(new TMyEncumbrance()
                            {
                                Name = MP.Package.SubParcels.NewSubParcel[ii].Encumbrance.Name,
                            });

                            Sl.AreaGKN = MP.Package.SubParcels.NewSubParcel[ii].Area.Area;
                            if (MP.Package.SubParcels.NewSubParcel[ii].EntitySpatial != null) //Если одноконтурная чзу
                                Sl.SpatialElement.ImportPolygon(CasterSpatial.ES_ZU(MP.Package.SubParcels.NewSubParcel[ii].Definition, MP.Package.SubParcels.NewSubParcel[ii].EntitySpatial));
                            if (MP.Package.SubParcels.NewSubParcel[ii].Contours != null)
                                Sl.ES = CasterSpatial.ES_SubParcels(MP.Package.SubParcels.NewSubParcel[ii].Contours);
                            MainObj.SubParcels.Add(Sl);
                        }
                }
                res.District.Blocks.Add(Bl);
                /*
                if (MP.GeneralCadastralWorks.Clients.Count == 1)
                {
                    if (MP.GeneralCadastralWorks.Clients[0].Person != null)
                        linkLabel_Recipient.Text = MP.GeneralCadastralWorks.Clients[0].Person.FamilyName + " " + MP.GeneralCadastralWorks.Clients[0].Person.FirstName +
                                      " " + MP.GeneralCadastralWorks.Clients[0].Person.Patronymic;
                    if (MP.GeneralCadastralWorks.Clients[0].Organization != null)
                        linkLabel_Recipient.Text = MP.GeneralCadastralWorks.Clients[0].Organization.Name;
                    if (MP.GeneralCadastralWorks.Clients[0].Governance != null)
                        linkLabel_Recipient.Text = MP.GeneralCadastralWorks.Clients[0].Governance.Name;
                }
                */
                res.Comments += ("\n");
                res.Comments += ("<br>_______________________________________ТИТУЛЬНЫЙ ЛИСТ ___________________________________");
                res.Comments += ("<br> Межевой план подготовлен в результате выполнения кадастровых работ в связи с:");
                res.Comments += ("\n");
                res.Comments += (MP.GeneralCadastralWorks.Reason);


                if (MP.Conclusion != null)
                {
                    res.Comments += ("<br>");
                    res.Comments += ("\n______________________________________ЗАКЛЮЧЕНИЕ_____________________________________");
                    res.Comments += ("\n");
                    res.Comments += (MP.Conclusion);
                    /*
                    res.Comments += "<br>" +
                                           MP.GeneralCadastralWorks.DateCadastral.ToString("dd.MM.yyyy") + "<br>" +
                                           MP.GeneralCadastralWorks.Contractor.FamilyName + " " +
                                           MP.GeneralCadastralWorks.Contractor.FirstName + " " +
                                           MP.GeneralCadastralWorks.Contractor.Patronymic + "<br>";
                    */
                }
                res.CommentsType = "Заключение КИ";
                res.Date = MP.GeneralCadastralWorks.DateCadastral.ToString("dd.MM.yyyy").Replace("0:00:00", "date");
                if (MP.GeneralCadastralWorks.Contractor.Organization != null)
                {
                    res.Cert_Doc_Organization = MP.GeneralCadastralWorks.Contractor.Organization.Name +
                                      "  " + MP.GeneralCadastralWorks.Contractor.Organization.AddressOrganization;
                }

                res.Appointment = MP.GeneralCadastralWorks.Contractor.NCertificate + " " +
                                  MP.GeneralCadastralWorks.Contractor.Telephone;
                res.AppointmentFIO = MP.GeneralCadastralWorks.Contractor.FamilyName + " " +
                                 MP.GeneralCadastralWorks.Contractor.FirstName + " " +
                                 MP.GeneralCadastralWorks.Contractor.Patronymic + "\r" +
                                 MP.GeneralCadastralWorks.Contractor.Email;


                res.Contractors.Add(
                           new TEngineerOut()
                           {
                               Date = MP.GeneralCadastralWorks.DateCadastral.ToString().Replace("0:00:00", ""),
                               FamilyName = MP.GeneralCadastralWorks.Contractor.FamilyName,
                               FirstName = MP.GeneralCadastralWorks.Contractor.FirstName,
                               Patronymic = MP.GeneralCadastralWorks.Contractor.Patronymic,
                               NCertificate = MP.GeneralCadastralWorks.Contractor.NCertificate,
                               Email = MP.GeneralCadastralWorks.Contractor.Email,
                               Organization_Name = MP.GeneralCadastralWorks.Contractor.Organization != null ? MP.GeneralCadastralWorks.Contractor.Organization.Name : "",
                               AddressOrganization = MP.GeneralCadastralWorks.Contractor.Organization != null ? MP.GeneralCadastralWorks.Contractor.Organization.AddressOrganization : ""

                           });





                foreach (MP_V06.tClientIdentify client in MP.GeneralCadastralWorks.Clients)
                {
                    if (client.Organization != null)
                    {
                        res.ReceivName = client.Organization.Name;
                        res.RequeryNumber = client.Organization.INN + ", " + client.Organization.OGRN;
                    }
                    if (client.Person != null)
                    {
                        res.ReceivName = client.Person.FamilyName + " " + client.Person.FirstName + " " + client.Person.Patronymic;
                        res.RequeryNumber = client.Person.SNILS + ",  " + client.Person.Address;
                    }

                }
                if (MP.InputData.GeodesicBases != null)
                    foreach (MP_V06.tSetOfPoint oms in MP.InputData.GeodesicBases)
                    {
                        GeodethicBase pt = new GeodethicBase((double)oms.OrdX, (double)oms.OrdY);
                        pt.PName = oms.PName;
                        pt.PKind = oms.PKind; //Type
                        pt.PKlass = oms.PKlass; // NetWork class
                        Bl.AddOmsPoint(pt);
                    }

            }
            catch (Exception ex)
            {
                res.CommentsType = "Exception";
                res.Comments = ex.Message;
                return res;
            }
            return res;
        }
        #endregion


        private void ParseGeneralCadastralWorksTP06(netFteo.IO.FileInfo fi, XmlNode cadworks, string Conclusion)
        {
            // TP / Building / GeneralCadastralWorks / @DateCadastral
            fi.Date = cadworks.Attributes.GetNamedItem("DateCadastral").Value;

            fi.AppointmentFIO = cadworks.SelectSingleNode("Contractor/FamilyName").FirstChild.Value + " " +
                cadworks.SelectSingleNode("Contractor/FirstName").FirstChild.Value + "  " +
                cadworks.SelectSingleNode("Contractor/Patronymic").FirstChild.Value + "  " +
                cadworks.SelectSingleNode("Contractor/CadastralEngineerRegistryNumber").FirstChild.Value;
            fi.Appointment = cadworks.SelectSingleNode("Contractor/Email").FirstChild.Value + " " +
                    cadworks.SelectSingleNode("Contractor/Telephone").FirstChild.Value;
            fi.Appointment += "\n " + cadworks.SelectSingleNode("Contractor/Address").FirstChild.Value;

            fi.Cert_Doc_Organization = "СНИЛС " + cadworks.SelectSingleNode("Contractor/SNILS").FirstChild.Value +
                                        " Номер в реестре " + cadworks.SelectSingleNode("Contractor/CadastralEngineerRegistryNumber").FirstChild.Value + "\n" +
                                        "СРО:" + cadworks.SelectSingleNode("Contractor/SelfRegulatoryOrganization").FirstChild.Value;
            fi.Comments += ("\n");
            fi.Comments += ("<br>_______________________________________ТИТУЛЬНЫЙ ЛИСТ ___________________________________");
            fi.Comments += ("<br> Технический план подготовлен в результате выполнения кадастровых работ в связи с:");
            fi.Comments += ("\n");
            fi.Comments += cadworks.SelectSingleNode("Reason").FirstChild.Value;

            if (Conclusion != null)
            {
                fi.Comments += ("<br>");
                fi.Comments += ("\n______________________________________ЗАКЛЮЧЕНИЕ_____________________________________");
                fi.Comments += ("\n");
                fi.Comments += (Conclusion);
            }

            /*
            / TP / Building / GeneralCadastralWorks / Clients / Client / Person / FamilyName
            / TP / Building / GeneralCadastralWorks / Clients / Client / Person / FirstName
            / TP / Building / GeneralCadastralWorks / Clients / Client / Person / Patronymic
            / TP / Building / GeneralCadastralWorks / Clients / Client / Person / SNILS
            */
            if (cadworks.SelectSingleNode("Clients").ChildNodes.Count == 1)
            {
                XmlNode Client = cadworks.SelectSingleNode("Clients/Client");
                if (Client.SelectSingleNode("Person") != null)
                {
                    fi.ReceivName = Client.SelectSingleNode("Person/FamilyName").FirstChild.Value + " " +
                        Client.SelectSingleNode("Person/FirstName").FirstChild.Value + " " +
                        Client.SelectSingleNode("Person/Patronymic").FirstChild.Value;
                    fi.RequeryNumber = Client.SelectSingleNode("Person/SNILS").FirstChild.Value;
                }

                /*
             if (client.Organization != null)
                    {
                        res.ReceivName = client.Organization.Name;
                        res.RequeryNumber = client.Organization.INN + ", " + client.Organization.OGRN;
                    }
                 */
            }
        }

        #region  Разбор TP 02/03
        private void ParseGeneralCadastralWorks(netFteo.IO.FileInfo fi, RRTypes.V03_TP.tGeneralCadastralWorks GW, string Conclusion)
        {
            fi.Date = GW.DateCadastral.ToString("dd.MM.yyyy").Replace("0:00:00", "");

            fi.AppointmentFIO = GW.Contractor.FamilyName +
                                 " " + GW.Contractor.FirstName +
                                 " " + GW.Contractor.Patronymic +
                                 "\n" + GW.Contractor.NCertificate;
            fi.Appointment = GW.Contractor.Email + " " +
                           GW.Contractor.NCertificate + " " + GW.Contractor.Telephone;
            fi.Appointment += "\n " + GW.Contractor.Address;


            fi.Contractors.Add(
                new TEngineerOut()
                {
                    FamilyName = GW.Contractor.FamilyName,
                    FirstName = GW.Contractor.FirstName,
                    Patronymic = GW.Contractor.Patronymic,
                    NCertificate = GW.Contractor.NCertificate,
                    Email = GW.Contractor.Email,
                    Date = GW.DateCadastral.ToString().Replace("0:00:00", ""),
                    Organization_Name = GW.Contractor.Organization != null ? GW.Contractor.Organization.Name : "",
                    AddressOrganization = GW.Contractor.Organization != null ? GW.Contractor.Organization.AddressOrganization : ""

                });


            if (GW.Contractor.Organization != null)
            {
                fi.Cert_Doc_Organization = GW.Contractor.Organization.Name + " \n" +
                                        GW.Contractor.Organization.AddressOrganization;
            }


            if (GW.Clients.Count == 1)
            {
                if (GW.Clients[0].Organization != null)
                {
                    // linkLabel_Recipient.Text = GW.Clients[0].Organization.Name;
                    // linkLabel_Request.Text = GW.Clients[0].Organization.INN;
                }

                if (GW.Clients[0].Person != null)
                {
                    /*
                    linkLabel_Recipient.Text = GW.Clients[0].Person.FamilyName + " "
                        + GW.Clients[0].Person.FirstName + " " + GW.Clients[0].Person.Patronymic;
                    if (GW.Clients[0].Person.SNILS != null)
                        linkLabel_Request.Text = "СНИЛС " + GW.Clients[0].Person.SNILS;
                    else linkLabel_Request.Text = "";
                    */
                }
            }


            fi.Comments += ("\n");
            fi.Comments += ("<br>_______________________________________ТИТУЛЬНЫЙ ЛИСТ ___________________________________");
            fi.Comments += ("<br> Технический план подготовлен в результате выполнения кадастровых работ в связи с:");
            fi.Comments += ("\n");
            fi.Comments += (GW.Reason);

            if (Conclusion != null)
            {
                fi.Comments += ("<br>");
                fi.Comments += ("\n______________________________________ЗАКЛЮЧЕНИЕ_____________________________________");
                fi.Comments += ("\n");
                fi.Comments += (Conclusion);
                /*
                fi.Comments += "<br>" +
                                       GW.DateCadastral.ToString("dd.MM.yyyy") + "<br>" +
                                       GW.Contractor.FamilyName + " " +
                                       GW.Contractor.FirstName + " " +
                                       GW.Contractor.Patronymic + "<br>";
                                       */
            }

        }



        private List<GeodethicBase> ParseInputDataTP06(XmlNode InputDataNode)
        {
            List<GeodethicBase> res = new List<GeodethicBase>();
            /*
             // TP/Building/InputData/GeodethicBases/GeodethicBase[1]/PName/#text

            if (inpData.GeodethicBases != null)
                foreach (V03_TP.tSetOfPoint oms in inpData.GeodethicBases)
                {
            // TP/Building/InputData/GeodethicBases/GeodethicBase[1]/OrdX/#text
            / /TP/Building/InputData/GeodethicBases/GeodethicBase[1]/OrdY
                    TPoint pt = new TPoint((double)oms.OrdX, (double)oms.OrdY);
            // /TP/Building/InputData/GeodethicBases/GeodethicBase[1]/PName
                    pt.Definition = oms.PName;
            // /TP/Building/InputData/GeodethicBases/GeodethicBase[1]/PKind/#text ...1Б
            / /TP/Building/InputData/GeodethicBases/GeodethicBase[1]/PKlass/#text
                    pt.Description = oms.PKlass;
                    pt.Code = oms.PName;
                    res.AddPoint(pt);
                }
            */
            return res;
        }

        private List<GeodethicBase> ParseInputData(V03_TP.tInputData inpData)
        {
            List<GeodethicBase> res = new List<GeodethicBase>();
            if (inpData.GeodesicBases != null)
                foreach (V03_TP.tSetOfPoint oms in inpData.GeodesicBases)
                {
                    GeodethicBase pt = new GeodethicBase((double)oms.OrdX, (double)oms.OrdY);
                    pt.PName = oms.PName;
                    pt.PKlass = oms.PKlass;
                    pt.PName = oms.PName;
                    res.Add(pt);
                }
            return res;
        }


        public netFteo.IO.FileInfo ParseSTDTPV02(netFteo.IO.FileInfo fi, System.Xml.XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            STD_TPV02.STD_TP TP = (STD_TPV02.STD_TP)Desearialize<STD_TPV02.STD_TP>(xmldoc);
            res.DocType = "Технический план v2";
            res.DocTypeNick = "ТП";
            res.Version = "v02";
            res.Number = TP.GUID;
            res.CommentsType = "Заключение КИ";


            if (TP.Construction != null)
            {
                res.Date = TP.Construction.Contractor.Date.ToString();

                /*
                richTextBox1.AppendText(TP.Construction.Conclusion);
                this.DocInfo.MyBlocks.CSs.Add(new TCoordSystem(TP.Construction.Coord_Systems[0].Name, TP.Construction.Coord_Systems[0].Cs_Id));
                ListViewItem LVi = new ListViewItem();
                LVi.Text = TP.Construction.Contractor.Date.ToString();
                LVi.SubItems.Add(TP.Construction.Contractor.Cadastral_Engineer.FIO.Surname + " " + TP.Construction.Contractor.Cadastral_Engineer.FIO.First +
                                     " " + TP.Construction.Contractor.Cadastral_Engineer.FIO.Patronymic);
                LVi.SubItems.Add(TP.Construction.Contractor.Cadastral_Engineer.N_Certificate);
                listView_Contractors.Items.Add(LVi);
                textBox_Appointment.Text = TP.Construction.Contractor.Cadastral_Engineer.FIO.Surname + " " + TP.Construction.Contractor.Cadastral_Engineer.FIO.First +
                                 " " + TP.Construction.Contractor.Cadastral_Engineer.FIO.Patronymic + ";  " + TP.Construction.Contractor.Cadastral_Engineer.E_mail;
                if (TP.Construction.Contractor.Cadastral_Organization != null)
                    textBox_OrgName.Text = TP.Construction.Contractor.Cadastral_Organization.Name;
                textBox_Appointment.Text = TP.Construction.Contractor.Cadastral_Engineer.N_Certificate;
                */
                if (TP.Construction.Package.New_Construction.Count > 0)
                {
                    TCadastralBlock Bl = new TCadastralBlock();
                    TRealEstate Constructions = new TRealEstate(TP.Construction.Package.New_Construction[0].Name, netFteo.Rosreestr.dRealty_v03.Сооружение);
                    Constructions.Construction.AssignationName = TP.Construction.Package.New_Construction[0].Assignation_Name;
                    Constructions.Location.Address.Note = TP.Construction.Package.New_Construction[0].Location.Note;
                    Constructions.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(TP.Construction.Package.New_Construction[0].Assignation_Name,
                                                                                     TP.Construction.Package.New_Construction[0].Entity_Spatial);
                    Bl.AddOKS(Constructions);
                    res.District.SpatialData.AddRange(Constructions.EntSpat);
                    res.District.Blocks.Add(Bl);
                }
            }
            return res;
        }

        public netFteo.IO.FileInfo ParseTP_V06(netFteo.IO.FileInfo fi, System.Xml.XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            res.DocType = "Технический план";
            res.DocTypeNick = "ТП";
            res.Version = fi.Version;
            res.Namespace = NameSpaces.TP06;
            if (xmldoc.DocumentElement.Attributes.GetNamedItem("GUID") != null) 
                res.Number = xmldoc.DocumentElement.Attributes.GetNamedItem("GUID").Value;
            res.CommentsType = "Заключение КИ";
            //TODO : 
            /*
			 * waiting for example of xml instance 
			 * 
			 */
            if (xmldoc.DocumentElement.SelectSingleNode("Building") != null)
            {
                XmlNode BuildNode = xmldoc.DocumentElement.SelectSingleNode("Building");
                if (BuildNode != null)
                {
                    ParseGeneralCadastralWorksTP06(res, BuildNode.SelectSingleNode("GeneralCadastralWorks"), BuildNode.SelectSingleNode("Conclusion").FirstChild.Value);
                    TCadastralBlock Bl = new TCadastralBlock();
                    Bl.CN = "ТП v6";
                    TRealEstate OKS = null;
                    // /TP/Building/InputData
                    Bl.OMSPoints = ParseInputDataTP06(BuildNode.SelectSingleNode("InputData"));

                    // TP / Building / CoordSystems / CoordSystem / @Name
                    // TP / Building / Package / ExistBuilding / EntitySpatial / @Name
                    //XmlNodeList CSNode = BuildNode.SelectNodes("CoordSystems");
                    /*
                    res.MyBlocks.CSs.Add(new TCoordSystem(BuildNode.SelectSingleNode("CoordSystems/CoordSystem").Attributes.GetNamedItem("Name").Value,
                        BuildNode.SelectSingleNode("CoordSystems/CoordSystem").Attributes.GetNamedItem("CsId").Value));
                    */
                }
            }

            if (xmldoc.DocumentElement.SelectSingleNode("Construction") != null)
            {
                XmlNode BuildNode = xmldoc.DocumentElement.SelectSingleNode("Construction");
                ParseGeneralCadastralWorksTP06(res, BuildNode.SelectSingleNode("GeneralCadastralWorks"), BuildNode.SelectSingleNode("Conclusion").FirstChild.Value);
            }

            if (xmldoc.DocumentElement.SelectSingleNode("Uncompleted") != null)
            {
                XmlNode BuildNode = xmldoc.DocumentElement.SelectSingleNode("Uncompleted");
                ParseGeneralCadastralWorksTP06(res, BuildNode.SelectSingleNode("GeneralCadastralWorks"), BuildNode.SelectSingleNode("Conclusion").FirstChild.Value);
            }

            if (xmldoc.DocumentElement.SelectSingleNode("Flat") != null)
            {
                XmlNode BuildNode = xmldoc.DocumentElement.SelectSingleNode("Flat");
                ParseGeneralCadastralWorksTP06(res, BuildNode.SelectSingleNode("GeneralCadastralWorks"), BuildNode.SelectSingleNode("Conclusion").FirstChild.Value);
            }

            if (xmldoc.DocumentElement.SelectSingleNode("CarParkingSpace") != null)
            {
                XmlNode BuildNode = xmldoc.DocumentElement.SelectSingleNode("CarParkingSpace");
                ParseGeneralCadastralWorksTP06(res, BuildNode.SelectSingleNode("GeneralCadastralWorks"), BuildNode.SelectSingleNode("Conclusion").FirstChild.Value);
            }


            XmlNodeList Blocksnodes = xmldoc.DocumentElement.SelectNodes("/" + xmldoc.DocumentElement.Name + "/cadastral_blocks/cadastral_block");
            /* 
            * */
            return res;
        }
            
        public netFteo.IO.FileInfo ParseTP_V03(netFteo.IO.FileInfo fi, System.Xml.XmlDocument xmldoc)//RRTypes.V03_TP.TP TP)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            RRTypes.V03_TP.TP TP = (RRTypes.V03_TP.TP)Desearialize<RRTypes.V03_TP.TP>(xmldoc);
            res.DocType = "Технический план v3";
            res.DocTypeNick = "ТП";
            res.Version = "v03";
            res.Number = TP.GUID;
            res.CommentsType = "Заключение КИ";

            if (TP.Building != null)
            {
                TCadastralBlock Bl = new TCadastralBlock();
                Bl.CN = "ТП v3";
                TRealEstate OKS = null;
                Bl.OMSPoints = ParseInputData(TP.Building.InputData);
                ParseGeneralCadastralWorks(res, TP.Building.GeneralCadastralWorks, TP.Building.Conclusion);
                res.District.CSs.Add(new TCoordSystem(TP.Building.CoordSystems[0].Name, TP.Building.CoordSystems[0].CsId));

                //Здание, постановка на ГКУ
                if (TP.Building.Package.NewBuildings != null)
                {
                    //if (TP.Building.Package.NewBuildings.Count == 1) <--- POZOR, fucking sin
                    foreach (V03_TP.tNewBuilding bld in TP.Building.Package.NewBuildings)
                    {
                        OKS = new TRealEstate("Здание", netFteo.Rosreestr.dRealty_v03.Здание);
                        OKS.Name = bld.Name;
                        OKS.Floors = bld.Floors.Floors;
                        OKS.UndergroundFloors = bld.Floors.UndergroundFloors;
                        OKS.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(bld.Address);
                        OKS.CadastralBlock = bld.CadastralBlocks[0];
                        OKS.ParentCadastralNumbers.AddRange(bld.ParentCadastralNumbers);
                        if (bld.ElementsConstruct != null)
                        OKS.ElementsConstruct.Add(new TElementConstruct(bld.ElementsConstruct[0].Wall.ToString()));
                        OKS.Building.Area = bld.Area;
                        OKS.Building.AssignationBuilding = bld.AssignationBuilding.ToString(); ;
                        OKS.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2("", bld.EntitySpatial);// RRTypes.CommonCast.CasterOKS.ES_OKS("", bld.EntitySpatial);
                    }
                }

                //Многоэтажный жилой дом
                if (TP.Building.Package.NewApartHouse != null)
                {
                    OKS = new TRealEstate(TP.Building.Package.NewApartHouse.NewBuilding.Name, netFteo.Rosreestr.dRealty_v03.Здание);
                    OKS.Name = TP.Building.Package.NewApartHouse.NewBuilding.Name;
                    OKS.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(TP.Building.Package.NewApartHouse.NewBuilding.Address);
                    OKS.CadastralBlock = TP.Building.Package.NewApartHouse.NewBuilding.CadastralBlocks[0];
                    OKS.ParentCadastralNumbers.AddRange(TP.Building.Package.NewApartHouse.NewBuilding.ParentCadastralNumbers);
                    if (TP.Building.Package.NewApartHouse.NewBuilding.ElementsConstruct != null)
                        OKS.ElementsConstruct.Add(new TElementConstruct(TP.Building.Package.NewApartHouse.NewBuilding.ElementsConstruct[0].Wall.ToString()));
                    OKS.Building.Area = TP.Building.Package.NewApartHouse.NewBuilding.Area;
                    OKS.Building.AssignationBuilding = TP.Building.Package.NewApartHouse.NewBuilding.AssignationBuilding.ToString();
                    OKS.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2("", TP.Building.Package.NewApartHouse.NewBuilding.EntitySpatial);
                    if (TP.Building.Package.NewApartHouse.Flats.Count > 0)
                    {
                        for (int i = 0; i <= TP.Building.Package.NewApartHouse.Flats.Count - 1; i++)
                        {
                            TFlat Flat = new TFlat((i + 1).ToString());
                            foreach (RRTypes.V03_TP.tLevelsLevel level in TP.Building.Package.NewApartHouse.Flats[i].PositionInObject.Levels)
                            {
                                TLevel lvl = new TLevel(level.Type.ToString(),
                                    level.Number,
                                    level.Position.NumberOnPlan);

                                foreach (RRTypes.V03_TP.tPlanJPG jpegname in level.Position.Plans)// TP.Building.Package.NewApartHouse.Flats[i].PositionInObject.Levels[0].Position.Plans)
                                    lvl.AddPlan(jpegname.Name);
                                Flat.PositionInObject.Levels.Add(lvl);
                            }
                            Flat.AssignationCode = TP.Building.Package.NewApartHouse.Flats[i].Assignation.AssignationCode.ToString();
                            if (TP.Building.Package.NewApartHouse.Flats[i].Assignation.AssignationTypeSpecified)
                                Flat.AssignationType = TP.Building.Package.NewApartHouse.Flats[i].Assignation.AssignationType.ToString();
                            Flat.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(TP.Building.Package.NewApartHouse.Flats[i].Address);
                            Flat.Area = TP.Building.Package.NewApartHouse.Flats[i].Area;
                            OKS.Building.Flats.Add(Flat);
                        }
                    }
                }

                //Здание, изменение ОКС
                if (TP.Building.Package.ExistBuilding != null)
                {
                    OKS = new TRealEstate("Здание", netFteo.Rosreestr.dRealty_v03.Здание);
                    OKS.CN = TP.Building.Package.ExistBuilding.CadastralNumber;
                    OKS.Name = TP.Building.Package.ExistBuilding.Name;
                    OKS.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(TP.Building.Package.ExistBuilding.Address);
                    OKS.CadastralBlock = TP.Building.Package.ExistBuilding.CadastralBlocks[0];
                    OKS.ParentCadastralNumbers.AddRange(TP.Building.Package.ExistBuilding.ParentCadastralNumbers);
                    if (TP.Building.Package.ExistBuilding.ElementsConstruct!= null)
                        OKS.ElementsConstruct.Add(new TElementConstruct(TP.Building.Package.ExistBuilding.ElementsConstruct[0].Wall.ToString()));
                    OKS.Floors = TP.Building.Package.ExistBuilding.Floors.Floors;
                    OKS.Building.Area = TP.Building.Package.ExistBuilding.Area;
                    if (TP.Building.Package.ExistBuilding.AssignationBuildingSpecified)
                        OKS.Building.AssignationBuilding = TP.Building.Package.ExistBuilding.AssignationBuilding.ToString();
                    OKS.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2("", TP.Building.Package.ExistBuilding.EntitySpatial);
                }
                Bl.CN = OKS.CadastralBlock;
                Bl.AddOKS(OKS);
                res.District.Blocks.Add(Bl);
            }

            //construction:
            if (TP.Construction != null)
            {
                TCadastralBlock Bl = new TCadastralBlock();
                Bl.CN = "ТП v3";
                TRealEstate OKS = null;
                ParseGeneralCadastralWorks(res, TP.Construction.GeneralCadastralWorks, TP.Construction.Conclusion);
                Bl.OMSPoints = ParseInputData(TP.Construction.InputData);
                res.District.CSs.Add(new TCoordSystem(TP.Construction.CoordSystems[0].Name, TP.Construction.CoordSystems[0].CsId));
                //Сооружение, постановка на ГКУ
                if (TP.Construction.Package.NewConstructions != null)
                {
                    foreach (V03_TP.tNewConstruction constr in TP.Construction.Package.NewConstructions)
                    {
                        OKS = new TRealEstate("Сооружение", netFteo.Rosreestr.dRealty_v03.Сооружение);
                        OKS.Name = constr.Name;
                        OKS.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(constr.Address);
                        OKS.CadastralBlock = constr.CadastralBlocks[0];
                        if (constr.ParentCadastralNumbers != null)
                            OKS.ParentCadastralNumbers.AddRange(constr.ParentCadastralNumbers);
                        foreach (RRTypes.V03_TP.tKeyParameter param in constr.KeyParameters)
                            OKS.Construction.KeyParameters.AddParameter(param.Type.ToString(), param.Value.ToString());
                        OKS.Construction.AssignationName = constr.AssignationName; ;
                        OKS.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2("", constr.EntitySpatial);
                    }
                }

                //изменение
                if (TP.Construction.Package.ExistConstruction != null)
                {
                    OKS = new TRealEstate("Сооружение", dRealty_v03.Сооружение);
                    OKS.CN = TP.Construction.Package.ExistConstruction.CadastralNumber;
                    OKS.Name = TP.Construction.Package.ExistConstruction.Name;
                    OKS.Location.Address = CommonCast.CasterOKS.CastAddress(TP.Construction.Package.ExistConstruction.Address);
                    OKS.CadastralBlock = TP.Construction.Package.ExistConstruction.CadastralBlocks[0];
                    OKS.ParentCadastralNumbers.AddRange(TP.Construction.Package.ExistConstruction.ParentCadastralNumbers);
                    foreach (V03_TP.tKeyParameter param in TP.Construction.Package.ExistConstruction.KeyParameters)
                        OKS.Construction.KeyParameters.AddParameter(param.Type.ToString(), param.Value.ToString());
                    if (TP.Construction.Package.ExistConstruction.AssignationName != null)
                        OKS.Construction.AssignationName = TP.Construction.Package.ExistConstruction.AssignationName;
                    OKS.EntSpat = CommonCast.CasterOKS.ES_OKS2("", TP.Construction.Package.ExistConstruction.EntitySpatial);
                }

                Bl.CN = OKS.CadastralBlock;
                Bl.AddOKS(OKS);
                res.District.Blocks.Add(Bl);
            }
            // end construction

            if (TP.Flat != null)
            {
                TCadastralBlock Bl = new TCadastralBlock();
                Bl.CN = "ТП v3";
                TRealEstate OKS = null;
                ParseGeneralCadastralWorks(res, TP.Flat.GeneralCadastralWorks, TP.Flat.Conclusion);
                //Помещение, учет изменений ГКУ
                if (TP.Flat.Package.ExistFlat != null)
                {
                    OKS = new TRealEstate(TP.Flat.Package.ExistFlat.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Помещение);
                    //OKS.Name = TP.Flat.Package.ExistFlat......;
                    OKS.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(TP.Flat.Package.ExistFlat.Address);
                    OKS.CadastralBlock = TP.Flat.Package.ExistFlat.CadastralBlock;
                    //OKS.Building.ParentCadastralNumbers.AddRange(TP.Flat.Package.ExistFlat..ParentCadastralNumbers);
                    OKS.Flat.Area = TP.Flat.Package.ExistFlat.Area;
                    OKS.Flat.AssignationCode = TP.Flat.Package.ExistFlat.Assignation.AssignationCode.ToString();
                    TLevel lvl = new TLevel(
                                     TP.Flat.Package.ExistFlat.PositionInObject.Levels[0].Type.ToString(),
                                     TP.Flat.Package.ExistFlat.PositionInObject.Levels[0].Number,
                                     TP.Flat.Package.ExistFlat.PositionInObject.Levels[0].Position.NumberOnPlan);
                    foreach (RRTypes.V03_TP.tPlanJPG jpegname in TP.Flat.Package.ExistFlat.PositionInObject.Levels[0].Position.Plans)
                        lvl.AddPlan(jpegname.Name);
                    OKS.Flat.PositionInObject.Levels.Add(lvl);
                    Bl.CN = OKS.CadastralBlock;
                    Bl.AddOKS(OKS);
                    res.District.Blocks.Add(Bl);
                }



                if (TP.Flat.Package.NewFlats != null)
                {
                    foreach (RRTypes.V03_TP.tNewFlat fl in TP.Flat.Package.NewFlats)
                    {
                        OKS = new TRealEstate("Здание", netFteo.Rosreestr.dRealty_v03.Здание);
                        OKS.CN = fl.ParentCadastralNumber.Item.ToString();
                        TFlat Flat = new TFlat(fl.PositionInObject.Levels[0].Position.NumberOnPlan);
                        Bl.CN = fl.CadastralBlock;
                        TLevel lvl = new TLevel(
                            fl.PositionInObject.Levels[0].Type.ToString(),
                            fl.PositionInObject.Levels[0].Number,
                            fl.PositionInObject.Levels[0].Position.NumberOnPlan);

                        foreach (RRTypes.V03_TP.tPlanJPG jpegname in fl.PositionInObject.Levels[0].Position.Plans)
                            lvl.AddPlan(jpegname.Name);
                        Flat.PositionInObject.Levels.Add(lvl);
                        Flat.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(fl.Address);
                        Flat.Area = fl.Area;
                        OKS.Building.Flats.Add(Flat);
                        Bl.AddOKS(OKS);
                    }

                    res.District.Blocks.Add(Bl);
                }
            }

            if (TP.Uncompleted != null)
            {
                TCadastralBlock Bl = new TCadastralBlock();
                Bl.CN = "ТП v3";
                TRealEstate OKS = null;
                Bl.OMSPoints = ParseInputData(TP.Uncompleted.InputData);
                ParseGeneralCadastralWorks(res, TP.Uncompleted.GeneralCadastralWorks, TP.Uncompleted.Conclusion);
                res.District.CSs.Add(new TCoordSystem(TP.Uncompleted.CoordSystems[0].Name, TP.Uncompleted.CoordSystems[0].CsId));
                //Здание, постановка на ГКУ
                if (TP.Uncompleted != null)
                {
                    OKS = new TRealEstate("ОНС", netFteo.Rosreestr.dRealty_v03.Объект_незавершённого_строительства);
                    if (TP.Uncompleted.Package.NewUncompleteds.Count == 1)

                        foreach (V03_TP.tNewUncompleted un in TP.Uncompleted.Package.NewUncompleteds)
                        {
                            OKS.Uncompleted.AssignationName = un.AssignationName;
                            OKS.Location.Address = CommonCast.CasterOKS.CastAddress(un.Address);
                            OKS.CadastralBlock = un.CadastralBlocks[0];
                            OKS.EntSpat = CommonCast.CasterOKS.ES_OKS2("", un.EntitySpatial);

                            OKS.ParentCadastralNumbers.AddRange(un.ParentCadastralNumbers);
                            OKS.Uncompleted.DegreeReadiness = un.DegreeReadiness.ToString();
                            foreach (V03_TP.tKeyParameter param in un.KeyParameters)
                                OKS.KeyParameters.AddParameter(param.Type.ToString(), param.Value.ToString());
                        }
                }

                //Здание, изменение ОКС
                if (TP.Uncompleted.Package.ExistUncompleted != null)
                {
                    OKS.Uncompleted.AssignationName = TP.Uncompleted.Package.ExistUncompleted.AssignationName;
                    OKS.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(TP.Uncompleted.Package.ExistUncompleted.Address);
                    OKS.CadastralBlock = TP.Uncompleted.Package.ExistUncompleted.CadastralBlocks[0];
                    OKS.ParentCadastralNumbers.AddRange(TP.Uncompleted.Package.ExistUncompleted.ParentCadastralNumbers);
                    foreach (RRTypes.V03_TP.tKeyParameter param in TP.Uncompleted.Package.ExistUncompleted.KeyParameters)
                        OKS.KeyParameters.AddParameter(param.Type.ToString(), param.Value.ToString());
                    OKS.Uncompleted.DegreeReadiness = TP.Uncompleted.Package.ExistUncompleted.DegreeReadiness.ToString();
                    OKS.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2("", TP.Uncompleted.Package.ExistUncompleted.EntitySpatial);
                }
                Bl.CN = OKS.CadastralBlock;
                Bl.AddOKS(OKS);
                res.District.Blocks.Add(Bl);
            }
            return res;
        }
        #endregion

        #region  Разбор КПТ 08

        public netFteo.IO.FileInfo ParseKPT05(netFteo.IO.FileInfo fi, Stream ms)
        {
            ms.Seek(0, 0);
            System.Xml.XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(ms);
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";
            res.Version = "05";
            res.DocType = "Кадастровый план территории";
            res.DocTypeNick = "КПТ";
            res.Namespace = NameSpaces.KPT05; // "urn://fake/kpt/5.0.0";
                                              //TODO - need deserialization

            System.Xml.XmlNodeList Blocksnodes = xmldoc.DocumentElement.SelectNodes("/" + xmldoc.DocumentElement.Name + "/Package/Federal/Cadastral_Regions/Cadastral_Region/Cadastral_Districts/Cadastral_District/Cadastral_Blocks/Cadastral_Block");
            if (Blocksnodes != null)

                for (int i = 0; i <= Blocksnodes.Count - 1; i++)
                {
                    //TMyCadastralBlock Bl = new TMyCadastralBlock(Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value);

                    var parcels = Blocksnodes[i].SelectSingleNode("Parcels");
                    for (int iP = 0; iP <= parcels.ChildNodes.Count - 1; iP++)
                    {
                        this.TotalItems2Process++;
                    }
                }

            XMLParsingStartProc("start xml", TotalItems2Process, null);

            for (int i = 0; i <= Blocksnodes.Count - 1; i++)
            {
                TCadastralBlock Bl = new TCadastralBlock(Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value);

                var parcels = Blocksnodes[i].SelectSingleNode("Parcels");
                for (int iP = 0; iP <= parcels.ChildNodes.Count - 1; iP++)
                {
                    System.Xml.XmlNode parcel = parcels.ChildNodes[iP];
                    TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(parcel.Attributes.GetNamedItem("CadastralNumber").Value, parcel.Attributes.GetNamedItem("Name").Value));

                    MainObj.AreaGKN = parcel.SelectSingleNode("Areas/Area/Area").FirstChild.Value; // идентично : .SelectSingleNode("Area").SelectSingleNode("Area")
                    MainObj.State = parcel.Attributes.GetNamedItem("State").Value;
                    MainObj.DateCreated = parcel.Attributes.GetNamedItem("DateCreated").Value;//.ToString("dd.MM.yyyy");
                    if (parcel.SelectSingleNode("Utilization").Attributes.GetNamedItem("ByDoc") != null)
                        MainObj.Utilization.UtilbyDoc = parcel.SelectSingleNode("Utilization").Attributes.GetNamedItem("ByDoc").Value;
                    MainObj.Category = parcel.SelectSingleNode("Category").Attributes.GetNamedItem("Category").Value;//netFteo.Rosreestr.dCategoriesv01.ItemToName(parcel.SelectSingleNode("Category").Attributes.GetNamedItem("Category").Value);
                    MainObj.Location = this.Parse_Location(parcel.SelectSingleNode("Location"));
                    if (parcel.SelectSingleNode("Unified_Land_Unit/Preceding_Land_Unit") != null)
                        MainObj.ParentCN = parcel.SelectSingleNode("Unified_Land_Unit/Preceding_Land_Unit").FirstChild.Value;
                    if (parcel.SelectSingleNode("Ground_Payments/Ground_Payment/@Value") != null)
                        MainObj.CadastralCost = Convert.ToDecimal(parcel.SelectSingleNode("Ground_Payments/Ground_Payment/@Value").Value);


                    //  /Rights/Right/Name/#text

                    if (parcel.SelectNodes("Rights/Right") != null)
                    {
                        XmlNodeList rights = parcel.SelectNodes("Rights/Right");
                        TMyRights Rights = new TMyRights();
                        foreach (XmlNode right in rights)
                        {
                            Rights.Add(new TRight(right.SelectSingleNode("Name").FirstChild.Value));
                        }
                        MainObj.Rights = Rights;
                    }
                    //Землепользование
                    if (parcel.SelectSingleNode("Entity_Spatial") != null)
                    {
                        TPolygon ents = KPT08LandEntSpatToFteo(parcel.Attributes.GetNamedItem("CadastralNumber").Value,
                                                              parcel.SelectSingleNode("Entity_Spatial"));
                        ents.AreaValue = (decimal)Convert.ToDouble(parcel.SelectSingleNode("Areas").SelectSingleNode("Area").FirstChild.Value);
                        ents.Parent_Id = MainObj.id;
                        ents.Definition = parcel.Attributes.GetNamedItem("CadastralNumber").Value;
                        MainObj.EntSpat.Add(ents);
                    }
                    //Многоконтурный
                    if (parcel.SelectSingleNode("Contours") != null)
                    {
                        //26:04:090203:258
                        System.Xml.XmlNode contours = parcel.SelectSingleNode("Contours");
                        string cn = parcel.Attributes.GetNamedItem("CadastralNumber").Value;
                        for (int ic = 0; ic <= parcel.SelectSingleNode("Contours").ChildNodes.Count - 1; ic++)
                        {
                            TPolygon NewCont = KPT08LandEntSpatToFteo(parcel.Attributes.GetNamedItem("CadastralNumber").Value + "(" +
                                                                  parcel.SelectSingleNode("Contours").ChildNodes[ic].Attributes.GetNamedItem("Number_Record").Value + ")",
                                                                  contours.ChildNodes[ic].SelectSingleNode("Entity_Spatial"));
                            MainObj.EntSpat.Add(NewCont);
                        }
                    }
                    XMLParsingProc("xml", ++FileParsePosition, null);
                }


                //Пункты в Квартале
                var OMSs = Blocksnodes[i].SelectSingleNode("OMSPoints");
                if (OMSs != null)
                {
                    for (int iP = 0; iP <= OMSs.ChildNodes.Count - 1; iP++)
                    {
                        GeodethicBase OMS = new GeodethicBase();
                        System.Xml.XmlNode xmloms = OMSs.ChildNodes[iP];
                        OMS.PNmb = xmloms.SelectSingleNode("PNmb").FirstChild.Value;
                        OMS.PKlass = xmloms.SelectSingleNode("PKlass").FirstChild.Value;
                        OMS.PName = xmloms.SelectSingleNode("PName").FirstChild.Value;
                        OMS.x = Convert.ToDouble(xmloms.SelectSingleNode("OrdX").FirstChild.Value);
                        OMS.y = Convert.ToDouble(xmloms.SelectSingleNode("OrdY").FirstChild.Value);
                        Bl.AddOmsPoint(OMS);
                    }
                }


                //Zones:
                var zones = Blocksnodes[i].SelectSingleNode("Zones");
                if (zones != null)
                {
                    for (int iP = 0; iP <= zones.ChildNodes.Count - 1; iP++)
                    {
                        TZone ZoneItem;
                        System.Xml.XmlNode zone = zones.ChildNodes[iP];
                        ZoneItem = new TZone(zone.SelectSingleNode("AccountNumber").FirstChild.Value);
                        //ZoneItem.Description = KPT10.CadastralBlocks[i].Zones[iP].Description;
                        ZoneItem.SpatialElement = KPT08LandEntSpatToFteo(zone.SelectSingleNode("AccountNumber").FirstChild.Value,
                            zone.SelectSingleNode("Entity_Spatial"));
                        /*
						res.MifPolygons.Add(KPT08LandEntSpatToFteo(zone.SelectSingleNode("AccountNumber").FirstChild.Value,
																   zone.SelectSingleNode("Entity_Spatial"))); */
                        res.District.SpatialData.AddRange(KPT08LandEntSpatToFteo(zone.SelectSingleNode("AccountNumber").FirstChild.Value,
                                                                   zone.SelectSingleNode("Entity_Spatial")));
                        if (zone.SelectSingleNode("Documents") != null)
                            foreach (System.Xml.XmlNode doc in zone.SelectSingleNode("Documents"))
                            {


                                ZoneItem.AddDocument(doc.SelectSingleNode("Number").FirstChild.Value,
                                    doc.SelectSingleNode("Name").FirstChild.Value,
                                    doc.SelectSingleNode("Code_Document").FirstChild.Value,
                                    ((doc.SelectSingleNode("IssueOrgan") != null) ? doc.SelectSingleNode("IssueOrgan").FirstChild.Value : "-"),
                                    ((doc.SelectSingleNode("Series") != null) ? doc.SelectSingleNode("Series").FirstChild.Value : "-"),
                                    (doc.SelectSingleNode("Date") != null) ? doc.SelectSingleNode("Date").FirstChild.Value : "");

                            }
                        if (zone.SelectSingleNode("SpecialZone") != null)
                        {
                            ZoneItem.AddContentRestrictions(zone.SelectSingleNode("SpecialZone").SelectSingleNode("ContentRestrictions").FirstChild.Value);
                        }

                        Bl.AddZone(ZoneItem);
                    }
                }

                //ОИПД Квартала:
                //Виртуальный OIPD типа "Квартал":

                if (Blocksnodes[i].SelectSingleNode("SpatialData") != null)

                {
                    Bl.Entity_Spatial.ImportPolygon(KPT08LandEntSpatToFteo(Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value,
                                                                           Blocksnodes[i].SelectSingleNode("SpatialData").SelectSingleNode("Entity_Spatial")));
                    Bl.Entity_Spatial.Definition = "гр" + Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value;
                }

                if (Blocksnodes[i].SelectSingleNode("Coord_System") != null)
                {
                    res.District.CSs.Add(new TCoordSystem(Blocksnodes[i].SelectSingleNode("Coord_System").Attributes.GetNamedItem("Name").Value,
                        Blocksnodes[i].SelectSingleNode("Coord_System").Attributes.GetNamedItem("Cs_Id").Value));
                }

                res.RequeryNumber = (Blocksnodes[i].SelectSingleNode("Note") != null) ?
                    Blocksnodes[i].SelectSingleNode("Note").FirstChild.Value : "";
                res.District.Blocks.Add(Bl);
            }

            // end TODO

            Parse_KTP05Info(xmldoc, res);
            return res;
        }

        public netFteo.IO.FileInfo ParseKPT06(netFteo.IO.FileInfo fi, Stream ms)
        {
            ms.Seek(0, 0);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(ms);
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";
            res.Version = "06";
            res.DocType = "Кадастровый план территории";
            res.DocTypeNick = "КПТ";
            res.Namespace = NameSpaces.KPT06; // "urn://fake/kpt/6.0.0";
                                              //TODO - need deserialization

            System.Xml.XmlNodeList Blocksnodes = xmldoc.DocumentElement.SelectNodes("/" + xmldoc.DocumentElement.Name + "/Package/Federal/Cadastral_Regions/Cadastral_Region/Cadastral_Districts/Cadastral_District/Cadastral_Blocks/Cadastral_Block");
            if (Blocksnodes != null)

                for (int i = 0; i <= Blocksnodes.Count - 1; i++)
                {
                    //TMyCadastralBlock Bl = new TMyCadastralBlock(Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value);

                    var parcels = Blocksnodes[i].SelectSingleNode("Parcels");
                    for (int iP = 0; iP <= parcels.ChildNodes.Count - 1; iP++)
                    {
                        this.TotalItems2Process++;
                    }
                }

            XMLParsingStartProc("start xml", TotalItems2Process, null);

            for (int i = 0; i <= Blocksnodes.Count - 1; i++)
            {
                TCadastralBlock Bl = new TCadastralBlock(Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value);

                var parcels = Blocksnodes[i].SelectSingleNode("Parcels");
                for (int iP = 0; iP <= parcels.ChildNodes.Count - 1; iP++)
                {
                    System.Xml.XmlNode parcel = parcels.ChildNodes[iP];
                    TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(parcel.Attributes.GetNamedItem("CadastralNumber").Value, parcel.Attributes.GetNamedItem("Name").Value));

                    MainObj.AreaGKN = parcel.SelectSingleNode("Areas/Area/Area").FirstChild.Value; // идентично : .SelectSingleNode("Area").SelectSingleNode("Area")
                    MainObj.State = parcel.Attributes.GetNamedItem("State").Value;
                    MainObj.DateCreated = parcel.Attributes.GetNamedItem("DateCreated").Value;//.ToString("dd.MM.yyyy");
                    if (parcel.SelectSingleNode("Utilization").Attributes.GetNamedItem("ByDoc") != null)
                        MainObj.Utilization.UtilbyDoc = parcel.SelectSingleNode("Utilization").Attributes.GetNamedItem("ByDoc").Value;
                    MainObj.Category = parcel.SelectSingleNode("Category").Attributes.GetNamedItem("Category").Value;//netFteo.Rosreestr.dCategoriesv01.ItemToName(parcel.SelectSingleNode("Category").Attributes.GetNamedItem("Category").Value);
                    MainObj.Location = this.Parse_Location(parcel.SelectSingleNode("Location"));
                    if (parcel.SelectSingleNode("Unified_Land_Unit/Preceding_Land_Unit") != null)
                        MainObj.ParentCN = parcel.SelectSingleNode("Unified_Land_Unit/Preceding_Land_Unit").FirstChild.Value;
                    if (parcel.SelectSingleNode("Ground_Payments/Ground_Payment/@Value") != null)
                        MainObj.CadastralCost = Convert.ToDecimal(parcel.SelectSingleNode("Ground_Payments/Ground_Payment/@Value").Value);


                    //  /Rights/Right/Name/#text

                    if (parcel.SelectNodes("Rights/Right") != null)
                    {
                        XmlNodeList rights = parcel.SelectNodes("Rights/Right");
                        TMyRights Rights = new TMyRights();
                        foreach (XmlNode right in rights)
                        {
                            Rights.Add(new TRight(right.SelectSingleNode("Name").FirstChild.Value));
                        }
                        MainObj.Rights = Rights;
                    }
                    //Землепользование
                    if (parcel.SelectSingleNode("Entity_Spatial") != null)
                    {
                        TPolygon ents = KPT08LandEntSpatToFteo(parcel.Attributes.GetNamedItem("CadastralNumber").Value,
                                                              parcel.SelectSingleNode("Entity_Spatial"));
                        ents.AreaValue = (decimal)Convert.ToDouble(parcel.SelectSingleNode("Areas").SelectSingleNode("Area").FirstChild.Value);
                        ents.Parent_Id = MainObj.id;
                        ents.Definition = parcel.Attributes.GetNamedItem("CadastralNumber").Value;
                        MainObj.EntSpat.Add(ents);
                    }
                    //Многоконтурный
                    if (parcel.SelectSingleNode("Contours") != null)
                    {
                        //26:04:090203:258
                        System.Xml.XmlNode contours = parcel.SelectSingleNode("Contours");
                        string cn = parcel.Attributes.GetNamedItem("CadastralNumber").Value;
                        for (int ic = 0; ic <= parcel.SelectSingleNode("Contours").ChildNodes.Count - 1; ic++)
                        {
                            TPolygon NewCont = KPT08LandEntSpatToFteo(parcel.Attributes.GetNamedItem("CadastralNumber").Value + "(" +
                                                                  parcel.SelectSingleNode("Contours").ChildNodes[ic].Attributes.GetNamedItem("Number_Record").Value + ")",
                                                                  contours.ChildNodes[ic].SelectSingleNode("Entity_Spatial"));
                            MainObj.EntSpat.Add(NewCont);
                        }
                    }
                    XMLParsingProc("xml", ++FileParsePosition, null);
                }


                //Пункты в Квартале
                var OMSs = Blocksnodes[i].SelectSingleNode("OMSPoints");
                if (OMSs != null)
                {
                    for (int iP = 0; iP <= OMSs.ChildNodes.Count - 1; iP++)
                    {
                        GeodethicBase OMS = new GeodethicBase();
                        System.Xml.XmlNode xmloms = OMSs.ChildNodes[iP];
                        OMS.PNmb = xmloms.SelectSingleNode("PNmb").FirstChild.Value;
                        OMS.PKlass = xmloms.SelectSingleNode("PKlass").FirstChild.Value;
                        OMS.PName = xmloms.SelectSingleNode("PName").FirstChild.Value;
                        OMS.x = Convert.ToDouble(xmloms.SelectSingleNode("OrdX").FirstChild.Value);
                        OMS.y = Convert.ToDouble(xmloms.SelectSingleNode("OrdY").FirstChild.Value);
                        Bl.AddOmsPoint(OMS);
                    }
                }


                //Zones:
                var zones = Blocksnodes[i].SelectSingleNode("Zones");
                if (zones != null)
                {
                    for (int iP = 0; iP <= zones.ChildNodes.Count - 1; iP++)
                    {
                        TZone ZoneItem;
                        System.Xml.XmlNode zone = zones.ChildNodes[iP];
                        ZoneItem = new TZone(zone.SelectSingleNode("AccountNumber").FirstChild.Value);
                        //ZoneItem.Description = KPT10.CadastralBlocks[i].Zones[iP].Description;
                        ZoneItem.SpatialElement = KPT08LandEntSpatToFteo(zone.SelectSingleNode("AccountNumber").FirstChild.Value,
                            zone.SelectSingleNode("Entity_Spatial"));
                        /*
						res.MifPolygons.Add(KPT08LandEntSpatToFteo(zone.SelectSingleNode("AccountNumber").FirstChild.Value,
																   zone.SelectSingleNode("Entity_Spatial"))); */
                        res.District.SpatialData.AddRange(KPT08LandEntSpatToFteo(zone.SelectSingleNode("AccountNumber").FirstChild.Value,
                                                                   zone.SelectSingleNode("Entity_Spatial")));
                        if (zone.SelectSingleNode("Documents") != null)
                            foreach (System.Xml.XmlNode doc in zone.SelectSingleNode("Documents"))
                            {


                                ZoneItem.AddDocument(doc.SelectSingleNode("Number").FirstChild.Value,
                                    doc.SelectSingleNode("Name").FirstChild.Value,
                                    doc.SelectSingleNode("Code_Document").FirstChild.Value,
                                    ((doc.SelectSingleNode("IssueOrgan") != null) ? doc.SelectSingleNode("IssueOrgan").FirstChild.Value : "-"),
                                    ((doc.SelectSingleNode("Series") != null) ? doc.SelectSingleNode("Series").FirstChild.Value : "-"),
                                    (doc.SelectSingleNode("Date") != null) ? doc.SelectSingleNode("Date").FirstChild.Value : "");

                            }
                        if (zone.SelectSingleNode("SpecialZone") != null)
                        {
                            ZoneItem.AddContentRestrictions(zone.SelectSingleNode("SpecialZone").SelectSingleNode("ContentRestrictions").FirstChild.Value);
                        }

                        Bl.AddZone(ZoneItem);
                    }
                }

                //ОИПД Квартала:
                //Виртуальный OIPD типа "Квартал":

                if (Blocksnodes[i].SelectSingleNode("SpatialData") != null)

                {
                    Bl.Entity_Spatial.ImportPolygon(KPT08LandEntSpatToFteo(Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value,
                                                                           Blocksnodes[i].SelectSingleNode("SpatialData").SelectSingleNode("Entity_Spatial")));
                    Bl.Entity_Spatial.Definition = "гр" + Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value;
                }

                if (Blocksnodes[i].SelectSingleNode("Coord_System") != null)
                {
                    res.District.CSs.Add(new TCoordSystem(Blocksnodes[i].SelectSingleNode("Coord_System").Attributes.GetNamedItem("Name").Value,
                        Blocksnodes[i].SelectSingleNode("Coord_System").Attributes.GetNamedItem("Cs_Id").Value));
                }
                if (Blocksnodes[i].SelectSingleNode("Note") != null)
                    res.RequeryNumber = (Blocksnodes[i].SelectSingleNode("Note").FirstChild != null) ?
                        Blocksnodes[i].SelectSingleNode("Note").FirstChild.Value : "";
                res.District.Blocks.Add(Bl);
            }

            // end TODO

            Parse_KTP05Info(xmldoc, res);
            return res;
        }

        public netFteo.IO.FileInfo ParseKPT07(netFteo.IO.FileInfo fi, Stream ms )
        {
            ms.Seek(0, 0);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(ms);
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";
            res.Version = "07";
            res.DocType = "Кадастровый план территории";
            res.DocTypeNick = "КПТ";
            res.Namespace = NameSpaces.KPT07; //"urn://fake/kpt/7.0.0";
                                              //TODO - need deserialization

            System.Xml.XmlNodeList Blocksnodes = xmldoc.DocumentElement.SelectNodes("/" + xmldoc.DocumentElement.Name + "/Package/Federal/Cadastral_Regions/Cadastral_Region/Cadastral_Districts/Cadastral_District/Cadastral_Blocks/Cadastral_Block");
            if (Blocksnodes != null)

                for (int i = 0; i <= Blocksnodes.Count - 1; i++)
                {
                    var parcels = Blocksnodes[i].SelectSingleNode("Parcels");
                    if (parcels != null) //some cadastral blocks may be without parcels 
                        for (int iP = 0; iP <= parcels.ChildNodes.Count - 1; iP++)
                        {
                            this.TotalItems2Process++;
                        }
                }

            XMLParsingStartProc("start xml", TotalItems2Process, null);

            for (int i = 0; i <= Blocksnodes.Count - 1; i++)
            {
                TCadastralBlock Bl = new TCadastralBlock(Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value);

                var parcels = Blocksnodes[i].SelectSingleNode("Parcels");
                if (parcels != null) //some cadastral blocks may be without parcels 
                    for (int iP = 0; iP <= parcels.ChildNodes.Count - 1; iP++)
                    {
                        System.Xml.XmlNode parcel = parcels.ChildNodes[iP];
                        TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(parcel.Attributes.GetNamedItem("CadastralNumber").Value, parcel.Attributes.GetNamedItem("Name").Value));
                        MainObj.AreaGKN = parcel.SelectSingleNode("Areas/Area/Area").FirstChild.Value; // идентично : .SelectSingleNode("Area").SelectSingleNode("Area")
                        MainObj.State = parcel.Attributes.GetNamedItem("State").Value;
                        MainObj.DateCreated = parcel.Attributes.GetNamedItem("DateCreated").Value;//.ToString("dd.MM.yyyy");
                        if (parcel.SelectSingleNode("Ground_Payments/Ground_Payment/@Value") != null)
                            MainObj.CadastralCost = Convert.ToDecimal(parcel.SelectSingleNode("Ground_Payments/Ground_Payment/@Value").Value);
                        if (parcel.SelectSingleNode("Utilization").Attributes.GetNamedItem("ByDoc") != null)
                            MainObj.Utilization.UtilbyDoc = parcel.SelectSingleNode("Utilization").Attributes.GetNamedItem("ByDoc").Value;
                        MainObj.Category = parcel.SelectSingleNode("Category").Attributes.GetNamedItem("Category").Value;//netFteo.Rosreestr.dCategoriesv01.ItemToName(parcel.SelectSingleNode("Category").Attributes.GetNamedItem("Category").Value);
                        MainObj.Location = this.Parse_Location(parcel.SelectSingleNode("Location"));
                        if (parcel.SelectSingleNode("Unified_Land_Unit/Preceding_Land_Unit") != null)
                            MainObj.ParentCN = parcel.SelectSingleNode("Unified_Land_Unit/Preceding_Land_Unit").FirstChild.Value;
                        //  /Rights/Right/Name/#text

                        if (parcel.SelectNodes("Rights/Right") != null)
                        {
                            XmlNodeList rights = parcel.SelectNodes("Rights/Right");
                            TMyRights Rights = new TMyRights();
                            foreach (XmlNode right in rights)
                            {
                                Rights.Add(new TRight(right.SelectSingleNode("Name").FirstChild.Value));
                            }
                            MainObj.Rights = Rights;
                        }
                        //Землепользование
                        if (parcel.SelectSingleNode("Entity_Spatial") != null)
                        {
                            TPolygon ents = KPT08LandEntSpatToFteo(parcel.Attributes.GetNamedItem("CadastralNumber").Value,
                                                                  parcel.SelectSingleNode("Entity_Spatial"));
                            ents.AreaValue = (decimal)Convert.ToDouble(parcel.SelectSingleNode("Areas/Area/Area").FirstChild.Value);
                            ents.Parent_Id = MainObj.id;
                            ents.Definition = parcel.Attributes.GetNamedItem("CadastralNumber").Value;
                            MainObj.EntSpat.Add(ents);
                        }
                        //Многоконтурный
                        if (parcel.SelectSingleNode("Contours") != null)
                        {
                            System.Xml.XmlNode contours = parcel.SelectSingleNode("Contours");
                            string cn = parcel.Attributes.GetNamedItem("CadastralNumber").Value;
                            for (int ic = 0; ic <= parcel.SelectSingleNode("Contours").ChildNodes.Count - 1; ic++)
                            {
                                /*  /Entity_Spatial/Spatial_Element/Spelement_Unit[1]/Ordinate/@X */
                                // / Contours / Contour[1] / @Number_PP
                                TPolygon NewCont = KPT08LandEntSpatToFteo(MainObj.CN + "(" +
                                                                      parcel.SelectSingleNode("Contours").ChildNodes[ic].Attributes.GetNamedItem("Number_PP").Value + ")",
                                                                      contours.ChildNodes[ic].SelectSingleNode("Entity_Spatial"));
                                MainObj.EntSpat.Add(NewCont);
                            }
                        }
                        XMLParsingProc("xml", ++FileParsePosition, null);
                    }


                //Пункты в Квартале
                var OMSs = Blocksnodes[i].SelectSingleNode("OMSPoints");
                if (OMSs != null)
                {
                    for (int iP = 0; iP <= OMSs.ChildNodes.Count - 1; iP++)
                    {
                        GeodethicBase OMS = new GeodethicBase();
                        System.Xml.XmlNode xmloms = OMSs.ChildNodes[iP];
                        OMS.PNmb = xmloms.SelectSingleNode("PNmb").FirstChild.Value;
                        OMS.PKlass = xmloms.SelectSingleNode("PKlass").FirstChild.Value;
                        OMS.PName = xmloms.SelectSingleNode("PName").FirstChild.Value;
                        OMS.x = Convert.ToDouble(xmloms.SelectSingleNode("OrdX").FirstChild.Value);
                        OMS.y = Convert.ToDouble(xmloms.SelectSingleNode("OrdY").FirstChild.Value);
                        Bl.AddOmsPoint(OMS);
                    }
                }


                //Zones:
                var zones = Blocksnodes[i].SelectSingleNode("Zones");
                if (zones != null)
                {
                    for (int iP = 0; iP <= zones.ChildNodes.Count - 1; iP++)
                    {
                        TZone ZoneItem;
                        System.Xml.XmlNode zone = zones.ChildNodes[iP];
                        ZoneItem = new TZone(zone.SelectSingleNode("AccountNumber").FirstChild.Value);
                        //ZoneItem.Description = KPT10.CadastralBlocks[i].Zones[iP].Description;
                        ZoneItem.SpatialElement = KPT08LandEntSpatToFteo(zone.SelectSingleNode("AccountNumber").FirstChild.Value,
                            zone.SelectSingleNode("Entity_Spatial"));
                        /*
						res.MifPolygons.Add(KPT08LandEntSpatToFteo(zone.SelectSingleNode("AccountNumber").FirstChild.Value,
																   zone.SelectSingleNode("Entity_Spatial"))); */
                        res.District.SpatialData.AddRange(KPT08LandEntSpatToFteo(zone.SelectSingleNode("AccountNumber").FirstChild.Value,
                                                                   zone.SelectSingleNode("Entity_Spatial")));
                        if (zone.SelectSingleNode("Documents") != null)
                            foreach (System.Xml.XmlNode doc in zone.SelectSingleNode("Documents"))
                            {


                                ZoneItem.AddDocument(doc.SelectSingleNode("Number").FirstChild.Value,
                                    doc.SelectSingleNode("Name").FirstChild.Value,
                                    doc.SelectSingleNode("Code_Document").FirstChild.Value,
                                    ((doc.SelectSingleNode("IssueOrgan") != null) ? doc.SelectSingleNode("IssueOrgan").FirstChild.Value : "-"),
                                    ((doc.SelectSingleNode("Series") != null) ? doc.SelectSingleNode("Series").FirstChild.Value : "-"),
                                    (doc.SelectSingleNode("Date") != null) ? doc.SelectSingleNode("Date").FirstChild.Value : "");

                            }
                        if (zone.SelectSingleNode("SpecialZone") != null)
                        {
                            ZoneItem.AddContentRestrictions(zone.SelectSingleNode("SpecialZone").SelectSingleNode("ContentRestrictions").FirstChild.Value);
                        }

                        Bl.AddZone(ZoneItem);
                    }
                }

                //ОИПД Квартала:
                //Виртуальный OIPD типа "Квартал":

                if (Blocksnodes[i].SelectSingleNode("SpatialData") != null)

                {
                    Bl.Entity_Spatial.ImportPolygon(KPT08LandEntSpatToFteo(Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value,
                                                                           Blocksnodes[i].SelectSingleNode("SpatialData").SelectSingleNode("Entity_Spatial")));
                    Bl.Entity_Spatial.Definition = "гр" + Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value;
                }

                if (Blocksnodes[i].SelectSingleNode("Coord_System") != null)
                {
                    res.District.CSs.Add(new TCoordSystem(Blocksnodes[i].SelectSingleNode("Coord_System").Attributes.GetNamedItem("Name").Value,
                        Blocksnodes[i].SelectSingleNode("Coord_System").Attributes.GetNamedItem("Cs_Id").Value));
                }

                res.RequeryNumber = (Blocksnodes[i].SelectSingleNode("Note") != null) ?
                    Blocksnodes[i].SelectSingleNode("Note").FirstChild.Value : "";
                res.District.Blocks.Add(Bl);
            }

            // end TODO

            Parse_KTP05Info(xmldoc, res);
            return res;
        }

        /// <summary>
        /// Uses only XMLDocument, none desirialization required
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="xmldoc"></param>
        /// <returns></returns>
        public netFteo.IO.FileInfo ParseKPT08(netFteo.IO.FileInfo fi, Stream ms)
        {
            ms.Seek(0, 0);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(ms);
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";
            res.Version = "08";
            res.DocType = "Кадастровый план территории";
            res.DocTypeNick = "КПТ";
            res.Namespace = NameSpaces.KPT08;// "urn://fake/kpt/8.0.0";
             //TODO - need deserialization....
            XmlNodeList Blocksnodes = xmldoc.DocumentElement.SelectNodes("/" + xmldoc.DocumentElement.Name + "/Package/Cadastral_Blocks/Cadastral_Block");
            if (Blocksnodes != null)

                for (int i = 0; i <= Blocksnodes.Count - 1; i++)
                {
                    //TMyCadastralBlock Bl = new TMyCadastralBlock(Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value);
                    var parcels = Blocksnodes[i].SelectSingleNode("Parcels");
                    for (int iP = 0; iP <= parcels.ChildNodes.Count - 1; iP++)
                    {
                        this.TotalItems2Process++;
                    }
                }

            XMLParsingStartProc("start xml", TotalItems2Process, null);

            for (int i = 0; i <= Blocksnodes.Count - 1; i++)
            {
                TCadastralBlock Bl = new TCadastralBlock(Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value);

                var parcels = Blocksnodes[i].SelectSingleNode("Parcels");
                for (int iP = 0; iP <= parcels.ChildNodes.Count - 1; iP++)
                {
                    System.Xml.XmlNode parcel = parcels.ChildNodes[iP];
                    TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(parcel.Attributes.GetNamedItem("CadastralNumber").Value, parcel.Attributes.GetNamedItem("Name").Value));

                    MainObj.AreaGKN = parcel.SelectSingleNode("Area/Area").FirstChild.Value; // идентично : .SelectSingleNode("Area").SelectSingleNode("Area")
                    MainObj.State = parcel.Attributes.GetNamedItem("State").Value;
                    MainObj.DateCreated = parcel.Attributes.GetNamedItem("DateCreated").Value;//.ToString("dd.MM.yyyy");
                                                                                              // CadastralCost/@Value
                    if (parcel.SelectSingleNode("CadastralCost/@Value") != null)
                        MainObj.CadastralCost = Convert.ToDecimal(parcel.SelectSingleNode("CadastralCost/@Value").Value);
                    if ((parcel.SelectSingleNode("Utilization") != null) &&
                        (parcel.SelectSingleNode("Utilization").Attributes.GetNamedItem("ByDoc") != null)
                        )
                        MainObj.Utilization.UtilbyDoc = parcel.SelectSingleNode("Utilization").Attributes.GetNamedItem("ByDoc").Value;
                    MainObj.Category = parcel.SelectSingleNode("Category").Attributes.GetNamedItem("Category").Value;//netFteo.Rosreestr.dCategoriesv01.ItemToName(parcel.SelectSingleNode("Category").Attributes.GetNamedItem("Category").Value);
                    MainObj.Location = this.Parse_Location(parcel.SelectSingleNode("Location"));
                    /*
                    //
                    MainObj.Utilization.Untilization = KPT10.CadastralBlocks[i].Parcels[iP].Utilization.Utilization.ToString();
                    */

                    //Землепользование

                    if (parcel.SelectSingleNode("Entity_Spatial") != null)
                    {
                        TPolygon ents = KPT08LandEntSpatToFteo(parcel.Attributes.GetNamedItem("CadastralNumber").Value,
                                                              parcel.SelectSingleNode("Entity_Spatial"));
                        ents.AreaValue = (decimal)Convert.ToDouble(parcel.SelectSingleNode("Area").SelectSingleNode("Area").FirstChild.Value);
                        ents.Parent_Id = MainObj.id;
                        ents.Definition = parcel.Attributes.GetNamedItem("CadastralNumber").Value;
                        MainObj.EntSpat.Add(ents);
                    }
                    //Многоконтурный
                    if (parcel.SelectSingleNode("Contours") != null)
                    {
                        //26:04:090203:258
                        System.Xml.XmlNode contours = parcel.SelectSingleNode("Contours");
                        string cn = parcel.Attributes.GetNamedItem("CadastralNumber").Value;
                        for (int ic = 0; ic <= parcel.SelectSingleNode("Contours").ChildNodes.Count - 1; ic++)
                        {
                            TPolygon NewCont = KPT08LandEntSpatToFteo(parcel.Attributes.GetNamedItem("CadastralNumber").Value + "(" +
                                                                  parcel.SelectSingleNode("Contours").ChildNodes[ic].Attributes.GetNamedItem("Number_Record").Value + ")",
                                                                  contours.ChildNodes[ic].SelectSingleNode("Entity_Spatial"));
                            MainObj.EntSpat.Add(NewCont);
                        }
                    }
                    XMLParsingProc("xml", ++FileParsePosition, null);
                }


                //Пункты в Квартале
                var OMSs = Blocksnodes[i].SelectSingleNode("OMSPoints");
                if (OMSs != null)
                {
                    for (int iP = 0; iP <= OMSs.ChildNodes.Count - 1; iP++)
                    {
                        GeodethicBase OMS = new GeodethicBase();
                        System.Xml.XmlNode xmloms = OMSs.ChildNodes[iP];
                        OMS.PNmb = xmloms.SelectSingleNode("PNmb").FirstChild.Value;
                        OMS.PKlass = xmloms.SelectSingleNode("PKlass").FirstChild.Value;
                        OMS.PName = xmloms.SelectSingleNode("PName").FirstChild.Value;
                        OMS.x = Convert.ToDouble(xmloms.SelectSingleNode("OrdX").FirstChild.Value);
                        OMS.y = Convert.ToDouble(xmloms.SelectSingleNode("OrdY").FirstChild.Value);
                        Bl.AddOmsPoint(OMS);
                    }
                }


                //Zones:
                var zones = Blocksnodes[i].SelectSingleNode("Zones");
                if (zones != null)
                {
                    for (int iP = 0; iP <= zones.ChildNodes.Count - 1; iP++)
                    {
                        TZone ZoneItem;
                        System.Xml.XmlNode zone = zones.ChildNodes[iP];
                        ZoneItem = new TZone(zone.SelectSingleNode("AccountNumber").FirstChild.Value);
                        //ZoneItem.Description = KPT10.CadastralBlocks[i].Zones[iP].Description;
                        ZoneItem.SpatialElement = KPT08LandEntSpatToFteo(zone.SelectSingleNode("AccountNumber").FirstChild.Value,
                            zone.SelectSingleNode("Entity_Spatial"));
                        /*
						res.MifPolygons.Add(KPT08LandEntSpatToFteo(zone.SelectSingleNode("AccountNumber").FirstChild.Value,
																   zone.SelectSingleNode("Entity_Spatial"))); */
                        res.District.SpatialData.AddRange(KPT08LandEntSpatToFteo(zone.SelectSingleNode("AccountNumber").FirstChild.Value,
                                                                   zone.SelectSingleNode("Entity_Spatial")));
                        if (zone.SelectSingleNode("Documents") != null)
                            foreach (System.Xml.XmlNode doc in zone.SelectSingleNode("Documents"))
                            {


                                ZoneItem.AddDocument(doc.SelectSingleNode("Number").FirstChild.Value,
                                    doc.SelectSingleNode("Name").FirstChild.Value,
                                    doc.SelectSingleNode("Code_Document").FirstChild.Value,
                                    ((doc.SelectSingleNode("IssueOrgan") != null) ? doc.SelectSingleNode("IssueOrgan").FirstChild.Value : "-"),
                                    ((doc.SelectSingleNode("Series") != null) ? doc.SelectSingleNode("Series").FirstChild.Value : "-"),
                                    (doc.SelectSingleNode("Date") != null) ? doc.SelectSingleNode("Date").FirstChild.Value : "");

                            }
                        if (zone.SelectSingleNode("SpecialZone") != null)
                        {
                            ZoneItem.AddContentRestrictions(zone.SelectSingleNode("SpecialZone").SelectSingleNode("ContentRestrictions").FirstChild.Value);
                        }

                        Bl.AddZone(ZoneItem);
                    }
                }

                //ОИПД Квартала:
                //Виртуальный OIPD типа "Квартал":

                if (Blocksnodes[i].SelectSingleNode("SpatialData") != null)

                {
                    Bl.Entity_Spatial.ImportPolygon(KPT08LandEntSpatToFteo(Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value,
                                                                           Blocksnodes[i].SelectSingleNode("SpatialData").SelectSingleNode("Entity_Spatial")));
                    Bl.Entity_Spatial.Definition = "гр" + Blocksnodes[i].Attributes.GetNamedItem("CadastralNumber").Value;
                }

                if (Blocksnodes[i].SelectSingleNode("Coord_System") != null)
                {
                    res.District.CSs.Add(new TCoordSystem(Blocksnodes[i].SelectSingleNode("Coord_System").Attributes.GetNamedItem("Name").Value,
                        Blocksnodes[i].SelectSingleNode("Coord_System").Attributes.GetNamedItem("Cs_Id").Value));
                }

                res.RequeryNumber = (Blocksnodes[i].SelectSingleNode("Note") != null) ?
                    Blocksnodes[i].SelectSingleNode("Note").FirstChild.Value : "";
                res.District.Blocks.Add(Bl);
            }

            // end TODO

            Parse_KTP08Info(xmldoc, res);
            xmldoc = null;
            return res;
        }

        //Разбор Spelement_Unit
        private static TPoint KPT08_ES_ParseSpelement_Unit(XmlNode Spelement_Unit)
        {
            TPoint Point = new TPoint();
            Point.x = Convert.ToDouble(Spelement_Unit.SelectSingleNode("Ordinate").Attributes.GetNamedItem("X").Value);
            Point.y = Convert.ToDouble(Spelement_Unit.SelectSingleNode("Ordinate").Attributes.GetNamedItem("Y").Value);
            Point.oldX = Point.x;
            Point.oldY = Point.y;
            Point.Definition = Spelement_Unit.Attributes.GetNamedItem("Su_Nmb").Value;
            return Point;
        }

        private static TPoint SchemaParcels_ES_ParseSpelement_Unit(System.Xml.XmlNode Spelement_Unit)
        {
            TPoint Point = new TPoint();
            Point.x = Convert.ToDouble(Spelement_Unit.SelectSingleNode("NewOrdinate").Attributes.GetNamedItem("X").Value);
            Point.y = Convert.ToDouble(Spelement_Unit.SelectSingleNode("NewOrdinate").Attributes.GetNamedItem("Y").Value);
            //Point.oldX = Point.x;
            //Point.oldY = Point.y;
            Point.Definition = Spelement_Unit.SelectSingleNode("NewOrdinate").Attributes.GetNamedItem("Num_Geopoint").Value;
            return Point;
        }





        /// <summary>
        /// Разбор Entity_Spatial KPT_V08
        /// </summary>
        /// <param name="Definition"></param>
        /// <param name="ES"></param>
        /// <returns></returns>
        private static TPolygon KPT08LandEntSpatToFteo(string Definition, System.Xml.XmlNode ES)
        {
            {
                if (ES == null) return null;
                TPolygon EntSpat = new TPolygon();
                EntSpat.Definition = Definition;
                //Первый Spatial_Element - внешний контур ( 0 )
                System.Xml.XmlNodeList OuterRing = ES.ChildNodes[0].ChildNodes;

                for (int iSpelement = 0; iSpelement <= OuterRing.Count - 1; iSpelement++)
                {
                    EntSpat.AddPoint(KPT08_ES_ParseSpelement_Unit(OuterRing[iSpelement]));
                }

                //Второй и след. Spatial_Element - Внутренние контура
                if (ES.ChildNodes.Count > 1)

                {
                    for (int ring = 1; ring <= ES.ChildNodes.Count - 1; ring++)
                    {
                        System.Xml.XmlNodeList childRing = ES.ChildNodes[ring].ChildNodes;
                        if (ES.ChildNodes[ring].Name == "Spatial_Element") // prevent reading same like "Borders" 
                        {
                            TRing InLayer = EntSpat.AddChild();

                            for (int iSpelement = 1; iSpelement <= childRing.Count - 1; iSpelement++)
                            {
                                InLayer.AddPoint(KPT08_ES_ParseSpelement_Unit(childRing[iSpelement]));
                            }
                        }
                    }
                }
                return EntSpat;
            }
        }


        private static TPolygon SchemaParcelsEStoFteo(System.Xml.XmlNode ES)
        {
            {
                if (ES == null) return null;
                TPolygon EntSpat = new TPolygon();
                EntSpat.Definition = "[==]";
                //Первый Spatial_Element - внешний контур ( 0 )
                System.Xml.XmlNodeList OuterRing = ES.ChildNodes[0].ChildNodes;

                for (int iSpelement = 0; iSpelement <= OuterRing.Count - 1; iSpelement++)
                {
                    EntSpat.AddPoint(SchemaParcels_ES_ParseSpelement_Unit(OuterRing[iSpelement]));
                }

                //Второй и след. Spatial_Element - Внутренние контура
                if (ES.ChildNodes.Count > 1)

                {
                    for (int ring = 1; ring <= ES.ChildNodes.Count - 1; ring++)
                    {
                        System.Xml.XmlNodeList childRing = ES.ChildNodes[ring].ChildNodes;
                        if (ES.ChildNodes[ring].Name == "Spatial_Element") // prevent reading same like "Borders" 
                        {
                            TRing InLayer = EntSpat.AddChild();

                            for (int iSpelement = 1; iSpelement <= childRing.Count - 1; iSpelement++)
                            {
                                InLayer.AddPoint(SchemaParcels_ES_ParseSpelement_Unit(childRing[iSpelement]));
                            }
                        }
                    }
                }
                return EntSpat;
            }
        }
 
  
        

        private TLocation Parse_Location(System.Xml.XmlNode xmllocation)
        {
            if (xmllocation == null) return null;
            TLocation loc = new TLocation();
            XmlNode Address = netFteo.XML.XMLWrapper.SelectNodeChild(xmllocation, "Address");

            if (Address != null)
            {
                TAddress Adr = new TAddress();

                if (netFteo.XML.XMLWrapper.SelectNodeChild(Address, "District") != null)
                    Adr.District = netFteo.XML.XMLWrapper.SelectNodeChild(Address, "District").Attributes.GetNamedItem("Type").Value + " " +
                        netFteo.XML.XMLWrapper.SelectNodeChild(Address, "District").Attributes.GetNamedItem("Name").Value;

                if (netFteo.XML.XMLWrapper.SelectNodeChild(Address, "City") != null)
                    Adr.City = netFteo.XML.XMLWrapper.SelectNodeChild(Address, "City").Attributes.GetNamedItem("Type").Value + " " +
                        netFteo.XML.XMLWrapper.SelectNodeChild(Address, "City").Attributes.GetNamedItem("Name").Value;

                if (netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Locality") != null)
                    Adr.Locality = netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Locality").Attributes.GetNamedItem("Type").Value + " " +
                        netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Locality").Attributes.GetNamedItem("Name").Value;

                if (netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Street") != null)
                    Adr.Street = netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Street").Attributes.GetNamedItem("Type").Value + " " +
                        netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Street").Attributes.GetNamedItem("Name").Value;

                if ((netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Level1") != null) &&
                        (netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Level1").Attributes.GetNamedItem("Name") != null))
                    Adr.Level1 = netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Level1").Attributes.GetNamedItem("Type").Value + " " +
                        netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Level1").Attributes.GetNamedItem("Name").Value;

                if ((netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Level2") != null) &&
                        (netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Level2").Attributes.GetNamedItem("Name") != null))
                    Adr.Level2 = netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Level2").Attributes.GetNamedItem("Type").Value + " " +
                        netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Level2").Attributes.GetNamedItem("Name").Value;

                if ((netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Level3") != null) &&
                        (netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Level3").Attributes.GetNamedItem("Name") != null))
                    Adr.Level3 = netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Level3").Attributes.GetNamedItem("Type").Value + " " +
                        netFteo.XML.XMLWrapper.SelectNodeChild(Address, "Level3").Attributes.GetNamedItem("Name").Value;

                Adr.Region = netFteo.XML.XMLWrapper.SelectNodeChildValue(Address, "Region");
                Adr.Note = netFteo.XML.XMLWrapper.SelectNodeChildValue(Address, "Note");

                loc.Address = Adr;
            }



            if (netFteo.XML.XMLWrapper.SelectNodeChild(xmllocation, "Elaboration") != null)
            {
                XmlNode elaboration = netFteo.XML.XMLWrapper.SelectNodeChild(xmllocation, "Elaboration");
                if (netFteo.XML.XMLWrapper.SelectNodeChild(elaboration, "ReferenceMark") != null)
                    loc.Elaboration.ReferenceMark = netFteo.XML.XMLWrapper.SelectNodeChildValue(elaboration, "ReferenceMark");

                if (netFteo.XML.XMLWrapper.SelectNodeChild(elaboration, "Direction") != null)
                    loc.Elaboration.Direction = netFteo.XML.XMLWrapper.SelectNodeChildValue(elaboration, "Direction");

                if (netFteo.XML.XMLWrapper.SelectNodeChild(elaboration, "Distance") != null)
                    loc.Elaboration.Distance = netFteo.XML.XMLWrapper.SelectNodeChildValue(elaboration, "Distance");
            }
            else
            if (netFteo.XML.XMLWrapper.SelectNodeChild(xmllocation, "inBounds") != null)
                loc.Inbounds = netFteo.XML.XMLWrapper.SelectNodeChildValue(xmllocation, "inBounds");

            return loc;
        }

        //   /Package/Cadastral_Blocks/Cadastral_Block
        private TCadastralBlock Parse_KTP08Block(System.Xml.XmlNode xmlBlock)
        {
            return null;
        }

        //   /Package/Cadastral_Blocks/Cadastral_Block/Parcels/Parcel
        private static void Parse_KTP08Parcel(System.Xml.XmlNode xmlBlock)
        {

        }

        private static void Parse_KTP05Info(System.Xml.XmlDocument xmldoc, netFteo.IO.FileInfo res)
        {
            res.Version = netFteo.XML.XMLWrapper.Parse_Attribute(xmldoc, "Version", "/eDocument");
            res.Date = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "/Package/Certification_Doc/Date");
            res.Number = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "/Package/Certification_Doc/Number");
            res.Appointment = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "/Package/Certification_Doc/Appointment");
            res.AppointmentFIO = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "Package/Certification_Doc/FIO");
            res.Cert_Doc_Organization = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "/eDocument/Sender/@Name");
        }

        private static void Parse_KTP08Info(System.Xml.XmlDocument xmldoc, netFteo.IO.FileInfo res)
        {
            res.Version = netFteo.XML.XMLWrapper.Parse_Attribute(xmldoc, "Version", "");
            res.Date = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "Package/Certification_Doc/Date");
            res.Number = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "Package/Certification_Doc/Number");
            res.Appointment = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "Package/Certification_Doc/Appointment");
            res.AppointmentFIO = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "Package/Certification_Doc/FIO");
            res.Cert_Doc_Organization = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "Package/Certification_Doc/Organization");
        }

 



        #endregion

        #region  Разбор КПТ 09

        public netFteo.IO.FileInfo ParseKPT09(netFteo.IO.FileInfo fi, Stream xmldocStream)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, null);
            res.CommentsType = "-";
            res.Version = "09";
            res.DocType = "Кадастровый план территории";
            res.DocTypeNick = "КПТ";

            RRTypes.kpt09.KPT KPT09 = (RRTypes.kpt09.KPT)Desearialize<kpt09.KPT>(xmldocStream);

            for (int i = 0; i <= KPT09.CadastralBlocks.Count - 1; i++)
            {
                TCadastralBlock Bl = new TCadastralBlock(KPT09.CadastralBlocks[i].CadastralNumber);
                for (int iP = 0; iP <= KPT09.CadastralBlocks[i].Parcels.Count - 1; iP++)
                {
                    TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(KPT09.CadastralBlocks[i].Parcels[iP].CadastralNumber, KPT09.CadastralBlocks[i].Parcels[iP].Name.ToString()));
                    MainObj.Location = RRTypes.CommonCast.CasterZU.CastLocation(KPT09.CadastralBlocks[i].Parcels[iP].Location);
                    MainObj.Utilization.UtilbyDoc = KPT09.CadastralBlocks[i].Parcels[iP].Utilization.ByDoc;
                    if (KPT09.CadastralBlocks[i].Parcels[iP].Utilization.UtilizationSpecified)
                        MainObj.Utilization.Untilization = KPT09.CadastralBlocks[i].Parcels[iP].Utilization.Utilization.ToString();
                    MainObj.AreaGKN = KPT09.CadastralBlocks[i].Parcels[iP].Area.Area;
                    MainObj.State = KPT09.CadastralBlocks[i].Parcels[iP].State.ToString();
                    MainObj.Category = KPT09.CadastralBlocks[i].Parcels[iP].Category.ToString(); //netFteo.Rosreestr.dCategoriesv01.ItemToName(KPT09.CadastralBlocks[i].Parcels[iP].Category.ToString());
                    MainObj.DateCreated = KPT09.CadastralBlocks[i].Parcels[iP].DateCreated.ToString("dd.MM.yyyy");
                    if (KPT09.CadastralBlocks[i].Parcels[iP].CadastralCost != null)
                        MainObj.CadastralCost = KPT09.CadastralBlocks[i].Parcels[iP].CadastralCost.Value;
                    //Землепользование
                    if (KPT09.CadastralBlocks[i].Parcels[iP].ParentCadastralNumbers != null)
                        MainObj.ParentCN = KPT09.CadastralBlocks[i].Parcels[iP].ParentCadastralNumbers.CadastralNumber;

                    if (KPT09.CadastralBlocks[i].Parcels[iP].EntitySpatial != null)
                        if (KPT09.CadastralBlocks[i].Parcels[iP].EntitySpatial.SpatialElement.Count > 0)
                        {
                            TPolygon ents = KPT_v09Utils.AddEntSpatKPT09(KPT09.CadastralBlocks[i].Parcels[iP].CadastralNumber,
                                                                  KPT09.CadastralBlocks[i].Parcels[iP].EntitySpatial);
                            ents.Parent_Id = MainObj.id;
                            ents.AreaValue = (decimal)Convert.ToDouble(KPT09.CadastralBlocks[i].Parcels[iP].Area.Area);
                            ents.Definition = KPT09.CadastralBlocks[i].Parcels[iP].CadastralNumber;
                            MainObj.EntSpat.Add(ents);
                        }
                    //Многоконтурный
                    if (KPT09.CadastralBlocks[i].Parcels[iP].Contours != null)
                    {
                        for (int ic = 0; ic <= KPT09.CadastralBlocks[i].Parcels[iP].Contours.Count - 1; ic++)
                        {
                            TPolygon NewCont = KPT_v09Utils.AddEntSpatKPT09(KPT09.CadastralBlocks[i].Parcels[iP].CadastralNumber + "(" +
                                                                  KPT09.CadastralBlocks[i].Parcels[iP].Contours[ic].NumberRecord + ")",
                                                                  KPT09.CadastralBlocks[i].Parcels[iP].Contours[ic].EntitySpatial);
                            //NewCont.GKNArea = KPT09.CadastralBlocks[i].Parcels[iP].Contours[ic].
                            MainObj.EntSpat.Add(NewCont);
                        }
                    }
                    //ЕЗП - В КПТ нет ОИПД!!

                }
                //ОИПД Квартала:
                //Виртуальный OIPD типа "Квартал":
                Bl.Entity_Spatial.ImportPolygon(KPT_v09Utils.KPT09LandEntSpatToFteo(KPT09.CadastralBlocks[i].CadastralNumber, KPT09.CadastralBlocks[i].SpatialData.EntitySpatial));
                Bl.Entity_Spatial.Definition = "гр" + KPT09.CadastralBlocks[i].CadastralNumber;
                //Пункты в Квартале
                if (KPT09.CadastralBlocks[i].OMSPoints.Count > 0)
                {
                    for (int iP = 0; iP <= KPT09.CadastralBlocks[i].OMSPoints.Count - 1; iP++)
                    {
                        GeodethicBase OMS = new GeodethicBase();
                        OMS.PNmb = KPT09.CadastralBlocks[i].OMSPoints[iP].PNmb;
                        OMS.PKlass = KPT09.CadastralBlocks[i].OMSPoints[iP].PKlass;
                        OMS.PName = KPT09.CadastralBlocks[i].OMSPoints[iP].PName;
                        OMS.x = (double)KPT09.CadastralBlocks[i].OMSPoints[iP].OrdX;
                        OMS.y = (double)KPT09.CadastralBlocks[i].OMSPoints[iP].OrdY;
                        Bl.AddOmsPoint(OMS);
                    }
                }
                for (int ics = 0; ics <= KPT09.CadastralBlocks[i].CoordSystems.Count - 1; ics++)
                {
                    res.District.CSs.Add(new TCoordSystem(KPT09.CadastralBlocks[i].CoordSystems[ics].Name, KPT09.CadastralBlocks[i].CoordSystems[ics].CsId));

                }
                //Zones:
                if (KPT09.CadastralBlocks[i].Zones.Count > 0)
                {
                    for (int iP = 0; iP <= KPT09.CadastralBlocks[i].Zones.Count - 1; iP++)
                    {
                        TZone ZoneItem;
                        ZoneItem = new TZone(KPT09.CadastralBlocks[i].Zones[iP].AccountNumber);
                        ZoneItem.Description = KPT09.CadastralBlocks[i].Zones[iP].Description;
                        ZoneItem.EntitySpatial.AddFeatures(CasterSpatial.ParseES_KPT09LandES(KPT09.CadastralBlocks[i].Zones[iP].AccountNumber, KPT09.CadastralBlocks[i].Zones[iP].EntitySpatial));
                        ZoneItem.EntitySpatial.State = 200; //complete, not need to  reparse
                        if (KPT09.CadastralBlocks[i].Zones[iP].Documents != null)
                            foreach (RRTypes.kpt09.tDocumentWithoutAppliedFile doc in KPT09.CadastralBlocks[i].Zones[iP].Documents)
                            {
                                ZoneItem.AddDocument(doc.Number, doc.Name, doc.CodeDocument.ToString(), doc.IssueOrgan, doc.Series, doc.Date.ToString());
                            }

                        if (KPT09.CadastralBlocks[i].Zones[iP].SpecialZone != null)
                        {
                            ZoneItem.AddContentRestrictions(KPT09.CadastralBlocks[i].Zones[iP].SpecialZone.ContentRestrictions);
                        }

                        if (KPT09.CadastralBlocks[i].Zones[iP].TerritorialZone != null)
                        {
                            ZoneItem.AddPermittedUses(KPT_v09Utils.PermittedUseCollectionToList(KPT09.CadastralBlocks[i].Zones[iP].TerritorialZone.PermittedUses));

                        }
                        Bl.AddZone(ZoneItem);
                    }
                }

                //Bounds 
                if (KPT09.CadastralBlocks[i].Bounds.Count > 0)
                    for (int ib = 0; ib <= KPT09.CadastralBlocks[i].Bounds.Count - 1; ib++)
                    {
                        TBound BoundItem = new TBound(KPT09.CadastralBlocks[i].Bounds[ib].Description, KPT_v09Utils.BoundToName(KPT09.CadastralBlocks[i].Bounds[ib]));
                        for (int ibb = 0; ibb <= KPT09.CadastralBlocks[i].Bounds[ib].Boundaries.Count - 1; ibb++)
                        {
                            BoundItem.SpatialElement = KPT_v09Utils.KPT09LandEntSpatToFteo(KPT09.CadastralBlocks[i].Bounds[ib].AccountNumber, KPT09.CadastralBlocks[i].Bounds[ib].Boundaries[ibb].EntitySpatial);
                            res.District.SpatialData.AddRange(KPT_v09Utils.KPT09LandEntSpatToFteo(KPT09.CadastralBlocks[i].Bounds[ib].AccountNumber, KPT09.CadastralBlocks[i].Bounds[ib].Boundaries[ibb].EntitySpatial));
                        }
                        Bl.AddBound(BoundItem);
                    }
                //ObjectRealty
                if (KPT09.CadastralBlocks[i].ObjectsRealty.Count > 0)
                {
                    for (int iP = 0; iP <= KPT09.CadastralBlocks[i].ObjectsRealty.Count - 1; iP++)
                    {
                        //Виртуальный Участок :

                        if (KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building != null)
                        {

                            //Также параллельное TmyOKS
                            TRealEstate Building = new TRealEstate(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                            Building.EntSpat = CommonCast.CasterOKS.ES_OKS2(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralNumber, KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.EntitySpatial);
                            Building.Building.AssignationBuilding = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.AssignationBuilding.ToString();
                            Building.Location.Address = KPT_v09Utils.AddrKPT09(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.Address);
                            Building.Area = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.Area;
                            if (KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralCost != null)
                                Building.CadastralCost = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralCost.Value;
                            Building.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.ObjectType);
                            Bl.AddOKS(Building);
                        }


                        if (KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction != null)
                        {
                            TRealEstate Constructions = new TRealEstate(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);
                            Constructions.Construction.AssignationName = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.AssignationName;
                            if (KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralCost != null)
                                Constructions.CadastralCost = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralCost.Value;
                            Constructions.Location.Address = KPT_v09Utils.AddrKPT09(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.Address);


                            if (KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.KeyParameters.Count > 0)
                            {
                                foreach (kpt09.tKeyParameter param in KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.KeyParameters)
                                {
                                    Constructions.Construction.KeyParameters.AddParameter(param.Type.ToString(),
                                                                                          param.Value.ToString());
                                }
                            }


                            if (KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.KeyParameters.Count == 1)
                            {

                                // Constructions.Construction.KeyName = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.KeyParameters[0].Type.ToString();
                                // Constructions.Construction.KeyValue = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.KeyParameters[0].Value.ToString();
                            }

                            Constructions.ObjectType = CommonCast.CasterOKS.ObjectTypeToStr(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.ObjectType);
                            Constructions.EntSpat = CommonCast.CasterOKS.ES_OKS2(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralNumber,
                                                                                             KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.EntitySpatial);
                            Bl.AddOKS(Constructions);
                            if (Constructions.EntSpat != null)
                                res.District.SpatialData.AddRange(Constructions.EntSpat);
                        }

                        if (KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted != null)
                        {
                            TRealEstate Uncompleted = new TRealEstate(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Объект_незавершённого_строительства);
                            Uncompleted.Uncompleted.AssignationName = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.AssignationName;
                            if (KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralCost != null)
                                Uncompleted.CadastralCost = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralCost.Value;
                            Uncompleted.Location.Address = KPT_v09Utils.AddrKPT09(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.Address);
                            Uncompleted.EntSpat = CommonCast.CasterOKS.ES_OKS2(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralNumber,
                                                                                             KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.EntitySpatial);
                            Uncompleted.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.ObjectType);
                            foreach (RRTypes.kpt09.tKeyParameter param in KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.KeyParameters)
                                Uncompleted.KeyParameters.AddParameter(netFteo.Rosreestr.dTypeParameter_v01.ItemToName(param.Type.ToString()), param.Value.ToString());
                            Bl.AddOKS(Uncompleted);
                        }
                    }
                }

                res.District.Blocks.Add(Bl);
            }

            res.Number = KPT09.CertificationDoc.Number;
            res.Date = KPT09.CertificationDoc.Date.ToString("dd.MM.yyyy");
            res.Cert_Doc_Organization = KPT09.CertificationDoc.Organization;

            if (KPT09.CertificationDoc.Official != null)
            {
                res.Appointment = KPT09.CertificationDoc.Official.Appointment;
                res.AppointmentFIO = KPT09.CertificationDoc.Official.FamilyName + " " +
                                  KPT09.CertificationDoc.Official.FirstName +
                                    " " +
                                  KPT09.CertificationDoc.Official.Patronymic;
            }
            return res;
        }
        #endregion

        #region  Разбор КПТ 10

        /// <summary>
        /// Kill big data (z.B. Zones) and save only OMS folder data
        /// </summary>
        /// <param name="fileType">KPT v10, v11 (untested)</param>
        /// <param name="res"></param>
        public void KPTSplitFolders(dFileTypes fileType, XmlDocument res)
        {

            if (fileType == dFileTypes.KPT10)
            {    //due KPT10 use namespace, we use ns: :
                XmlNamespaceManager nmgr = new XmlNamespaceManager(res.NameTable);
                nmgr.AddNamespace("ns", res.DocumentElement.NamespaceURI);
                nmgr.AddNamespace("ns6", "urn://x-artefacts-rosreestr-ru/commons/complex-types/certification-doc/1.0");

                XmlComment newComment = res.CreateComment("File has wiped "+ DateTime.Now.ToString() + " to reduce file size");
                XmlComment newComment1 = res.CreateComment("Cleared childs of: Zones,Parcels,ObjectsRealty,Bounds");
                XmlComment newComment2 = res.CreateComment("Child nodes has wiped: Zones,Parcels,ObjectsRealty,Bounds");

                XmlNode NumberNode = res.DocumentElement.SelectSingleNode("ns:CertificationDoc/ns6:Number", nmgr);
                NumberNode.InnerText += " (ОМС)";

                 XmlNodeList Blocksnodes = res.DocumentElement.SelectNodes("ns:CadastralBlocks/ns:CadastralBlock", nmgr);   //"/" + res.DocumentElement.Name + "/CadastralBlocks");
                                                                                                                                      //Create a comment.
                res.DocumentElement.AppendChild(newComment);
                XmlNode NumberNode3 = res.DocumentElement.SelectSingleNode("ns:CadastralBlocks", nmgr);
                NumberNode3.AppendChild(newComment2);
                //XmlNode CertNode = res.DocumentElement.SelectSingleNode("ns:CertificationDoc", nmgr);
                //CertNode.AppendChild(newComment3);

                if (Blocksnodes != null)
                    for (int i = 0; i <= Blocksnodes.Count - 1; i++)
                    {
                        Blocksnodes[i].AppendChild(newComment1);
                        if (Blocksnodes[i].SelectSingleNode("ns:Zones", nmgr) != null)
                            Blocksnodes[i].SelectSingleNode("ns:Zones", nmgr).RemoveAll();
                        if (Blocksnodes[i].SelectSingleNode("ns:Parcels", nmgr) != null)
                            Blocksnodes[i].SelectSingleNode("ns:Parcels", nmgr).RemoveAll();
                        if (Blocksnodes[i].SelectSingleNode("ns:ObjectsRealty", nmgr) != null)
                            Blocksnodes[i].SelectSingleNode("ns:ObjectsRealty", nmgr).RemoveAll();
                        if (Blocksnodes[i].SelectSingleNode("ns:Bounds", nmgr) != null)
                            Blocksnodes[i].SelectSingleNode("ns:Bounds", nmgr).RemoveAll();
                    }
            }

            if (fileType == dFileTypes.KPT11) //this case are not checked(debugged and bugFixed)
            {
                XmlNodeList Blocksnodes = res.DocumentElement.SelectNodes("/" + res.DocumentElement.Name + "/cadastral_blocks/cadastral_block");
                if (Blocksnodes != null)
                    for (int i = 0; i <= Blocksnodes.Count - 1; i++)
                    {
                        //TCadastralBlock Bl = new TCadastralBlock(Blocksnodes[i].SelectSingleNode("cadastral_number").FirstChild.Value);
                        if (Blocksnodes[i].SelectSingleNode("record_data/base_data/land_records") != null)
                            Blocksnodes[i].SelectSingleNode("record_data/base_data/land_records").RemoveAll();
                        if (Blocksnodes[i].SelectSingleNode("record_data/base_data/build_records") != null)
                            Blocksnodes[i].SelectSingleNode("record_data/base_data/build_records").RemoveAll();
                        if (Blocksnodes[i].SelectSingleNode("record_data/base_data/construction_records") != null)
                            Blocksnodes[i].SelectSingleNode("record_data/base_data/construction_records").RemoveAll();
                        if (Blocksnodes[i].SelectSingleNode("record_data/base_data/object_under_construction_records") != null)
                            Blocksnodes[i].SelectSingleNode("record_data/base_data/object_under_construction_records").RemoveAll();
                        if (Blocksnodes[i].SelectSingleNode("municipal_boundaries") != null)
                            Blocksnodes[i].SelectSingleNode("municipal_boundaries").RemoveAll();
                        if (Blocksnodes[i].SelectSingleNode("inhabited_locality_boundaries") != null)
                            Blocksnodes[i].SelectSingleNode("inhabited_locality_boundaries").RemoveAll();
                        if (Blocksnodes[i].SelectSingleNode("zones_and_territories_boundaries") != null)
                            Blocksnodes[i].SelectSingleNode("zones_and_territories_boundaries").RemoveAll();
                    }
            }


            //return res;
        }


            public netFteo.IO.FileInfo ParseKPT10(netFteo.IO.FileInfo fi, Stream xmldocStream) //RRTypes.kpt10_un.KPT KPT10)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, null);
            res.CommentsType = "-";
            res.DocType = "Кадастровый план территории";
            res.DocTypeNick = "КПТ";
            res.Version = "10";

            kpt10_un.KPT KPT10 = (kpt10_un.KPT)Desearialize<kpt10_un.KPT>(xmldocStream);

            for (int i = 0; i <= KPT10.CadastralBlocks.Count - 1; i++)
            {
                TCadastralBlock Bl = new TCadastralBlock(KPT10.CadastralBlocks[i].CadastralNumber);
                for (int iP = 0; iP <= KPT10.CadastralBlocks[i].Parcels.Count - 1; iP++)
                {
                    TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(KPT10.CadastralBlocks[i].Parcels[iP].CadastralNumber, KPT10.CadastralBlocks[i].Parcels[iP].Name.ToString()));
                    MainObj.Location = RRTypes.CommonCast.CasterZU.CastLocation(KPT10.CadastralBlocks[i].Parcels[iP].Location);
                    MainObj.Utilization.UtilbyDoc = KPT10.CadastralBlocks[i].Parcels[iP].Utilization.ByDoc;
                    MainObj.Utilization.Untilization = KPT10.CadastralBlocks[i].Parcels[iP].Utilization.Utilization.ToString();
                    MainObj.AreaGKN = KPT10.CadastralBlocks[i].Parcels[iP].Area.Area;
                    MainObj.State = KPT10.CadastralBlocks[i].Parcels[iP].State.ToString();
                    MainObj.Category = KPT10.CadastralBlocks[i].Parcels[iP].Category.ToString();//netFteo.Rosreestr.dCategoriesv01.ItemToName(KPT10.CadastralBlocks[i].Parcels[iP].Category.ToString());
                    MainObj.DateCreated = KPT10.CadastralBlocks[i].Parcels[iP].DateCreated.ToString("dd.MM.yyyy");
                    if (KPT10.CadastralBlocks[i].Parcels[iP].CadastralCost != null)
                        MainObj.CadastralCost = KPT10.CadastralBlocks[i].Parcels[iP].CadastralCost.Value;

                    //Землепользование
                    if (KPT10.CadastralBlocks[i].Parcels[iP].ParentCadastralNumbers != null)
                        MainObj.ParentCN = KPT10.CadastralBlocks[i].Parcels[iP].ParentCadastralNumbers.CadastralNumber;

                    if (KPT10.CadastralBlocks[i].Parcels[iP].EntitySpatial != null)
                        if (KPT10.CadastralBlocks[i].Parcels[iP].EntitySpatial.SpatialElement.Count > 0)
                        {
                            TPolygon ents = KPT_v10Utils.AddEntSpatKPT10(KPT10.CadastralBlocks[i].Parcels[iP].CadastralNumber,
                                                                  KPT10.CadastralBlocks[i].Parcels[iP].EntitySpatial);
                            ents.AreaValue = (decimal)Convert.ToDouble(KPT10.CadastralBlocks[i].Parcels[iP].Area.Area);
                            ents.Parent_Id = MainObj.id;
                            ents.Definition = KPT10.CadastralBlocks[i].Parcels[iP].CadastralNumber;
                            MainObj.EntSpat.Add(ents);
                        }
                    //mc
                    if (KPT10.CadastralBlocks[i].Parcels[iP].Contours != null)
                    {
                        for (int ic = 0; ic <= KPT10.CadastralBlocks[i].Parcels[iP].Contours.Count - 1; ic++)
                        {
                            TPolygon NewCont = KPT_v10Utils.AddEntSpatKPT10(KPT10.CadastralBlocks[i].Parcels[iP].CadastralNumber + "(" +
                                                                  KPT10.CadastralBlocks[i].Parcels[iP].Contours[ic].NumberRecord + ")",
                                                                  KPT10.CadastralBlocks[i].Parcels[iP].Contours[ic].EntitySpatial);
                            MainObj.EntSpat.Add(NewCont);
                        }
                    }
                    //ЕЗП - В КПТ нет ОИПД!!
                }

                //ОИПД Квартала:
                //Виртуальный OIPD типа "Квартал":
                //TPolygon BlockSpat = new TPolygon();
                //BlockSpat = KPT_v09Utils.KPT09LandEntSpatToFteo(KPT10.CadastralBlocks[i].CadastralNumber,KPT10.CadastralBlocks[i].SpatialData.EntitySpatial);
                //MifPolygons.Items.Add(BlockSpat);
                Bl.Entity_Spatial.ImportPolygon(KPT_v10Utils.KPT10LandEntSpatToFteo(KPT10.CadastralBlocks[i].CadastralNumber, KPT10.CadastralBlocks[i].SpatialData.EntitySpatial));
                Bl.Entity_Spatial.Definition = "гр" + KPT10.CadastralBlocks[i].CadastralNumber;
                //Пункты в Квартале
                if (KPT10.CadastralBlocks[i].OMSPoints.Count > 0)
                {
                    for (int iP = 0; iP <= KPT10.CadastralBlocks[i].OMSPoints.Count - 1; iP++)
                    {
                        GeodethicBase OMS = new GeodethicBase();
                        OMS.PNmb = KPT10.CadastralBlocks[i].OMSPoints[iP].PNmb;       //94873
                        OMS.PKlass = KPT10.CadastralBlocks[i].OMSPoints[iP].PKlass;    //Класс ОМС – "; Система координат - МСК-26 от СК-95, зона 1
                        OMS.PName = KPT10.CadastralBlocks[i].OMSPoints[iP].PName;            //OMZ-117; 1Б; Труновский р-н, с. Донское, на пересечении ул. Кооперативная и ул. Ленина.
                        OMS.x = (double)KPT10.CadastralBlocks[i].OMSPoints[iP].OrdX;
                        OMS.y = (double)KPT10.CadastralBlocks[i].OMSPoints[iP].OrdY;
                        Bl.AddOmsPoint(OMS);
                    }
                }

                for (int ics = 0; ics <= KPT10.CadastralBlocks[i].CoordSystems.Count - 1; ics++)
                {
                    res.District.CSs.Add(new TCoordSystem(KPT10.CadastralBlocks[i].CoordSystems[ics].Name, KPT10.CadastralBlocks[i].CoordSystems[ics].CsId));

                }
                //Zones:
                if (KPT10.CadastralBlocks[i].Zones.Count > 0)
                {
                    for (int iP = 0; iP <= KPT10.CadastralBlocks[i].Zones.Count - 1; iP++)
                    {
                        TZone ZoneItem;
                        ZoneItem = new TZone(KPT10.CadastralBlocks[i].Zones[iP].AccountNumber);
                        ZoneItem.Description = KPT10.CadastralBlocks[i].Zones[iP].Description;
                        ZoneItem.EntitySpatial.AddFeatures(CasterSpatial.ParseES_KPT10LandES(KPT10.CadastralBlocks[i].Zones[iP].AccountNumber, KPT10.CadastralBlocks[i].Zones[iP].EntitySpatial));
                        ZoneItem.EntitySpatial.State = 14; // raw spatial
                        //TODO open too long time occured, when scanning:  
                        // ZoneItem.EntitySpatial.ParseSpatial();
                        if (KPT10.CadastralBlocks[i].Zones[iP].Documents != null)
                            foreach (RRTypes.kpt10_un.tDocumentWithoutAppliedFile doc in KPT10.CadastralBlocks[i].Zones[iP].Documents)
                            {
                                ZoneItem.AddDocument(doc.Number, doc.Name, doc.CodeDocument.ToString(), doc.IssueOrgan, doc.Series, doc.Date.ToString());
                            }

                        if (KPT10.CadastralBlocks[i].Zones[iP].SpecialZone != null)
                        {
                            ZoneItem.TypeName = "SpecialZone";
                            ZoneItem.AddContentRestrictions(KPT10.CadastralBlocks[i].Zones[iP].SpecialZone.ContentRestrictions);
                        }

                        if (KPT10.CadastralBlocks[i].Zones[iP].TerritorialZone != null)
                        {
                            ZoneItem.TypeName = "Территориальная зона";
                            ZoneItem.AddPermittedUses(KPT_v10Utils.PermittedUseCollectionToList(this.dAllowedUse_v02, KPT10.CadastralBlocks[i].Zones[iP].TerritorialZone.PermittedUses));
                        }
                        Bl.AddZone(ZoneItem);
                    }
                }

                //Bounds 
                if (KPT10.CadastralBlocks[i].Bounds.Count > 0)
                    for (int ib = 0; ib <= KPT10.CadastralBlocks[i].Bounds.Count - 1; ib++)
                    {
                        TBound BoundItem = new TBound(KPT10.CadastralBlocks[i].Bounds[ib].Description, KPT_v10Utils.BoundToName(KPT10.CadastralBlocks[i].Bounds[ib]));
                        for (int ibb = 0; ibb <= KPT10.CadastralBlocks[i].Bounds[ib].Boundaries.Count - 1; ibb++)
                        {
                            BoundItem.SpatialElement = KPT_v10Utils.KPT10LandEntSpatToFteo(KPT10.CadastralBlocks[i].Bounds[ib].AccountNumber, KPT10.CadastralBlocks[i].Bounds[ib].Boundaries[ibb].EntitySpatial);
                            res.District.SpatialData.AddRange(KPT_v10Utils.KPT10LandEntSpatToFteo(KPT10.CadastralBlocks[i].Bounds[ib].AccountNumber, KPT10.CadastralBlocks[i].Bounds[ib].Boundaries[ibb].EntitySpatial));
                        }
                        Bl.AddBound(BoundItem);
                    }

                //ObjectRealty
                if (KPT10.CadastralBlocks[i].ObjectsRealty.Count > 0)
                {
                    for (int iP = 0; iP <= KPT10.CadastralBlocks[i].ObjectsRealty.Count - 1; iP++)
                    {
                        //Виртуальный Участок :

                        if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building != null)
                        {

                            //Также параллельное TmyOKS
                            TRealEstate Building = new TRealEstate(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                            Building.EntSpat = CommonCast.CasterOKS.ES_OKS2(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralNumber, KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.EntitySpatial);
                            Building.Building.AssignationBuilding = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.AssignationBuilding.ToString();
                            Building.Location = KPT_v10Utils.LocAddrKPT10(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.Address);
                            Building.Area = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.Area;
                            if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralCost != null)
                                Building.CadastralCost = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralCost.Value;
                            Building.ObjectType = CommonCast.CasterOKS.ObjectTypeToStr(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.ObjectType);
                            Bl.AddOKS(Building);
                        }


                        if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction != null)
                        {
                            TRealEstate Constructions = new TRealEstate(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);
                            Constructions.Construction.AssignationName = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.AssignationName;
                            Constructions.Location = KPT_v10Utils.LocAddrKPT10(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.Address);
                            if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralCost != null)
                                Constructions.CadastralCost = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralCost.Value;
                            Constructions.EntSpat = CommonCast.CasterOKS.ES_OKS2(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralNumber,
                                                                                             KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.EntitySpatial);
                            if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.KeyParameters.Count > 0)
                            {
                                foreach (kpt10_un.tKeyParameter param in KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.KeyParameters)
                                {
                                    Constructions.Construction.KeyParameters.AddParameter(param.Type.ToString(),
                                                                                          param.Value.ToString());
                                }
                            }
                            Constructions.ObjectType = CommonCast.CasterOKS.ObjectTypeToStr(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.ObjectType);
                            Bl.AddOKS(Constructions);
                        }

                        if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted != null)
                        {
                            TRealEstate Uncompleted = new TRealEstate(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralNumber, dRealty_v03.Объект_незавершённого_строительства);
                            Uncompleted.Uncompleted.AssignationName = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.AssignationName;
                            Uncompleted.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.ObjectType);
                            Uncompleted.Location = KPT_v10Utils.LocAddrKPT10(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.Address);
                            if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralCost != null)
                                Uncompleted.CadastralCost = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralCost.Value;
                            Uncompleted.EntSpat = CommonCast.CasterOKS.ES_OKS2(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralNumber,
                                                                                             KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.EntitySpatial);

                            Bl.AddOKS(Uncompleted);
                        }
                    }
                }

                res.District.Blocks.Add(Bl);
            }
            res.Number = KPT10.CertificationDoc.Number;
            res.Date = KPT10.CertificationDoc.Date.ToString("dd.MM.yyyy");
            res.Cert_Doc_Organization = KPT10.CertificationDoc.Organization;

            if (KPT10.CertificationDoc.Official != null)
            {
                res.Appointment = KPT10.CertificationDoc.Official.Appointment;
                res.AppointmentFIO = KPT10.CertificationDoc.Official.FamilyName + " " +
                                     KPT10.CertificationDoc.Official.FirstName + " " +
                                     KPT10.CertificationDoc.Official.Patronymic;
            }

            return res;
        }

        public netFteo.IO.FileInfo ParseKPT10WithReader(netFteo.IO.FileInfo fi, Stream xmlStream)
        {
            
            using (XmlReader reader = XmlReader.Create(xmlStream))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {

                    }
                }
            }
            return null;
        }

        /*
        public netFteo.IO.FileInfo ParseKPT10(netFteo.IO.FileInfo fi, XmlDocument xmldoc) //RRTypes.kpt10_un.KPT KPT10)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, null);
            res.CommentsType = "-";
            res.DocType = "Кадастровый план территории";
            res.DocTypeNick = "КПТ";
            res.Version = "10";

            kpt10_un.KPT KPT10 = (kpt10_un.KPT)Desearialize<kpt10_un.KPT>(xmldoc);

            for (int i = 0; i <= KPT10.CadastralBlocks.Count - 1; i++)
            {
                TCadastralBlock Bl = new TCadastralBlock(KPT10.CadastralBlocks[i].CadastralNumber);
                for (int iP = 0; iP <= KPT10.CadastralBlocks[i].Parcels.Count - 1; iP++)
                {
                    TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(KPT10.CadastralBlocks[i].Parcels[iP].CadastralNumber, KPT10.CadastralBlocks[i].Parcels[iP].Name.ToString()));
                    MainObj.Location = RRTypes.CommonCast.CasterZU.CastLocation(KPT10.CadastralBlocks[i].Parcels[iP].Location);
                    MainObj.Utilization.UtilbyDoc = KPT10.CadastralBlocks[i].Parcels[iP].Utilization.ByDoc;
                    MainObj.Utilization.Untilization = KPT10.CadastralBlocks[i].Parcels[iP].Utilization.Utilization.ToString();
                    MainObj.AreaGKN = KPT10.CadastralBlocks[i].Parcels[iP].Area.Area;
                    MainObj.State = KPT10.CadastralBlocks[i].Parcels[iP].State.ToString();
                    MainObj.Category = KPT10.CadastralBlocks[i].Parcels[iP].Category.ToString();//netFteo.Rosreestr.dCategoriesv01.ItemToName(KPT10.CadastralBlocks[i].Parcels[iP].Category.ToString());
                    MainObj.DateCreated = KPT10.CadastralBlocks[i].Parcels[iP].DateCreated.ToString("dd.MM.yyyy");
                    if (KPT10.CadastralBlocks[i].Parcels[iP].CadastralCost != null)
                        MainObj.CadastralCost = KPT10.CadastralBlocks[i].Parcels[iP].CadastralCost.Value;

                    //Землепользование
                    if (KPT10.CadastralBlocks[i].Parcels[iP].ParentCadastralNumbers != null)
                        MainObj.ParentCN = KPT10.CadastralBlocks[i].Parcels[iP].ParentCadastralNumbers.CadastralNumber;

                    if (KPT10.CadastralBlocks[i].Parcels[iP].EntitySpatial != null)
                        if (KPT10.CadastralBlocks[i].Parcels[iP].EntitySpatial.SpatialElement.Count > 0)
                        {
                            TPolygon ents = KPT_v10Utils.AddEntSpatKPT10(KPT10.CadastralBlocks[i].Parcels[iP].CadastralNumber,
                                                                  KPT10.CadastralBlocks[i].Parcels[iP].EntitySpatial);
                            ents.AreaValue = (decimal)Convert.ToDouble(KPT10.CadastralBlocks[i].Parcels[iP].Area.Area);
                            ents.Parent_Id = MainObj.id;
                            ents.Definition = KPT10.CadastralBlocks[i].Parcels[iP].CadastralNumber;
                            MainObj.EntSpat.Add(ents);
                        }
                    //Многоконтурный
                    if (KPT10.CadastralBlocks[i].Parcels[iP].Contours != null)
                    {
                        for (int ic = 0; ic <= KPT10.CadastralBlocks[i].Parcels[iP].Contours.Count - 1; ic++)
                        {
                            TPolygon NewCont = KPT_v10Utils.AddEntSpatKPT10(KPT10.CadastralBlocks[i].Parcels[iP].CadastralNumber + "(" +
                                                                  KPT10.CadastralBlocks[i].Parcels[iP].Contours[ic].NumberRecord + ")",
                                                                  KPT10.CadastralBlocks[i].Parcels[iP].Contours[ic].EntitySpatial);
                            MainObj.EntSpat.Add(NewCont);
                        }
                    }
                    //ЕЗП - В КПТ нет ОИПД!!

                }
                //ОИПД Квартала:
                //Виртуальный OIPD типа "Квартал":
                //TPolygon BlockSpat = new TPolygon();
                //BlockSpat = KPT_v09Utils.KPT09LandEntSpatToFteo(KPT10.CadastralBlocks[i].CadastralNumber,KPT10.CadastralBlocks[i].SpatialData.EntitySpatial);
                //MifPolygons.Items.Add(BlockSpat);
                Bl.Entity_Spatial.ImportPolygon(KPT_v10Utils.KPT10LandEntSpatToFteo(KPT10.CadastralBlocks[i].CadastralNumber, KPT10.CadastralBlocks[i].SpatialData.EntitySpatial));
                Bl.Entity_Spatial.Definition = "гр" + KPT10.CadastralBlocks[i].CadastralNumber;
                //Пункты в Квартале
                if (KPT10.CadastralBlocks[i].OMSPoints.Count > 0)
                {
                    for (int iP = 0; iP <= KPT10.CadastralBlocks[i].OMSPoints.Count - 1; iP++)
                    {
                        TPoint OMS = new TPoint();
                        OMS.Definition = KPT10.CadastralBlocks[i].OMSPoints[iP].PNmb;
                        OMS.Code = KPT10.CadastralBlocks[i].OMSPoints[iP].PKlass;
                        OMS.Description = KPT10.CadastralBlocks[i].OMSPoints[iP].PName;
                        OMS.x = (double)KPT10.CadastralBlocks[i].OMSPoints[iP].OrdX;
                        OMS.y = (double)KPT10.CadastralBlocks[i].OMSPoints[iP].OrdY;
                        Bl.AddOmsPoint(OMS);
                    }
                }

                for (int ics = 0; ics <= KPT10.CadastralBlocks[i].CoordSystems.Count - 1; ics++)
                {
                    res.MyBlocks.CSs.Add(new TCoordSystem(KPT10.CadastralBlocks[i].CoordSystems[ics].Name, KPT10.CadastralBlocks[i].CoordSystems[ics].CsId));

                }
                //Zones:
                if (KPT10.CadastralBlocks[i].Zones.Count > 0)
                {
                    for (int iP = 0; iP <= KPT10.CadastralBlocks[i].Zones.Count - 1; iP++)
                    {
                        TZone ZoneItem;
                        ZoneItem = new TZone(KPT10.CadastralBlocks[i].Zones[iP].AccountNumber);
                        ZoneItem.Description = KPT10.CadastralBlocks[i].Zones[iP].Description;
                        ZoneItem.EntitySpatial = KPT_v10Utils.KPT10LandEntSpatToFteo(KPT10.CadastralBlocks[i].Zones[iP].AccountNumber, KPT10.CadastralBlocks[i].Zones[iP].EntitySpatial);
                        res.MyBlocks.SpatialData.AddRange(KPT_v10Utils.KPT10LandEntSpatToFteo(KPT10.CadastralBlocks[i].Zones[iP].AccountNumber, KPT10.CadastralBlocks[i].Zones[iP].EntitySpatial));
                        if (KPT10.CadastralBlocks[i].Zones[iP].Documents != null)
                            foreach (RRTypes.kpt10_un.tDocumentWithoutAppliedFile doc in KPT10.CadastralBlocks[i].Zones[iP].Documents)
                            {
                                ZoneItem.AddDocument(doc.Number, doc.Name, doc.CodeDocument.ToString(), doc.IssueOrgan, doc.Series, doc.Date.ToString());
                            }

                        if (KPT10.CadastralBlocks[i].Zones[iP].SpecialZone != null)
                        {
                            ZoneItem.TypeName = "SpecialZone";
                            ZoneItem.AddContentRestrictions(KPT10.CadastralBlocks[i].Zones[iP].SpecialZone.ContentRestrictions);
                        }

                        if (KPT10.CadastralBlocks[i].Zones[iP].TerritorialZone != null)
                        {
                            ZoneItem.TypeName = "Территориальная зона";
                            ZoneItem.AddPermittedUses(KPT_v10Utils.PermittedUseCollectionToList(this.dAllowedUse_v02, KPT10.CadastralBlocks[i].Zones[iP].TerritorialZone.PermittedUses));
                        }
                        Bl.AddZone(ZoneItem);
                    }
                }




                //Bounds 
                if (KPT10.CadastralBlocks[i].Bounds.Count > 0)
                    for (int ib = 0; ib <= KPT10.CadastralBlocks[i].Bounds.Count - 1; ib++)
                    {
                        TBound BoundItem = new TBound(KPT10.CadastralBlocks[i].Bounds[ib].Description, KPT_v10Utils.BoundToName(KPT10.CadastralBlocks[i].Bounds[ib]));
                        for (int ibb = 0; ibb <= KPT10.CadastralBlocks[i].Bounds[ib].Boundaries.Count - 1; ibb++)
                        {
                            BoundItem.EntitySpatial = KPT_v10Utils.KPT10LandEntSpatToFteo(KPT10.CadastralBlocks[i].Bounds[ib].AccountNumber, KPT10.CadastralBlocks[i].Bounds[ib].Boundaries[ibb].EntitySpatial);
                            res.MyBlocks.SpatialData.AddRange(KPT_v10Utils.KPT10LandEntSpatToFteo(KPT10.CadastralBlocks[i].Bounds[ib].AccountNumber, KPT10.CadastralBlocks[i].Bounds[ib].Boundaries[ibb].EntitySpatial));
                        }
                        Bl.AddBound(BoundItem);
                    }

                //ObjectRealty
                if (KPT10.CadastralBlocks[i].ObjectsRealty.Count > 0)
                {
                    for (int iP = 0; iP <= KPT10.CadastralBlocks[i].ObjectsRealty.Count - 1; iP++)
                    {
                        //Виртуальный Участок :

                        if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building != null)
                        {

                            //Также параллельное TmyOKS
                            TRealEstate Building = new TRealEstate(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                            Building.EntSpat = CommonCast.CasterOKS.ES_OKS2(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralNumber, KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.EntitySpatial);
                            Building.Building.AssignationBuilding = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.AssignationBuilding.ToString();
                            Building.Location = KPT_v10Utils.LocAddrKPT10(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.Address);
                            Building.Area = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.Area;
                            if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralCost != null)
                                Building.CadastralCost = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralCost.Value;
                            Building.ObjectType = CommonCast.CasterOKS.ObjectTypeToStr(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.ObjectType);
                            Bl.AddOKS(Building);
                        }


                        if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction != null)
                        {
                            TRealEstate Constructions = new TRealEstate(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);
                            Constructions.Construction.AssignationName = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.AssignationName;
                            Constructions.Location = KPT_v10Utils.LocAddrKPT10(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.Address);
                            if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralCost != null)
                                Constructions.CadastralCost = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralCost.Value;
                            Constructions.EntSpat = CommonCast.CasterOKS.ES_OKS2(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralNumber,
                                                                                             KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.EntitySpatial);
                            if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.KeyParameters.Count > 0)
                            {
                                foreach (kpt10_un.tKeyParameter param in KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.KeyParameters)
                                {
                                    Constructions.Construction.KeyParameters.AddParameter(param.Type.ToString(),
                                                                                          param.Value.ToString());
                                }
                            }
                            Constructions.ObjectType = CommonCast.CasterOKS.ObjectTypeToStr(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.ObjectType);
                            Bl.AddOKS(Constructions);
                        }

                        if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted != null)
                        {
                            TRealEstate Uncompleted = new TRealEstate(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Объект_незавершённого_строительства);
                            Uncompleted.Uncompleted.AssignationName = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.AssignationName;
                            Uncompleted.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.ObjectType);
                            Uncompleted.Location = KPT_v10Utils.LocAddrKPT10(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.Address);
                            if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralCost != null)
                                Uncompleted.CadastralCost = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralCost.Value;
                            Uncompleted.EntSpat = CommonCast.CasterOKS.ES_OKS2(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralNumber,
                                                                                             KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.EntitySpatial);

                            Bl.AddOKS(Uncompleted);
                        }
                    }
                }

                res.MyBlocks.Blocks.Add(Bl);
            }
            res.Number = KPT10.CertificationDoc.Number;
            res.Date = KPT10.CertificationDoc.Date.ToString("dd.MM.yyyy");
            res.Cert_Doc_Organization = KPT10.CertificationDoc.Organization;

            if (KPT10.CertificationDoc.Official != null)
            {
                res.Appointment = KPT10.CertificationDoc.Official.Appointment;
                res.AppointmentFIO = KPT10.CertificationDoc.Official.FamilyName + " " +
                                     KPT10.CertificationDoc.Official.FirstName + " " +
                                     KPT10.CertificationDoc.Official.Patronymic;
            }

            return res;
        }
        */
        #endregion

        #region KVZU 8 FGIS EGRN
        /// <summary>
        /// After starting FGIS EGRN at oct.2020 we reciev new xml doc.
        /// Name this KV8
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="xmldoc"></param>
        /// <returns></returns>
        public netFteo.IO.FileInfo ParseKV8(netFteo.IO.FileInfo fi, XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";
            res.DocType = "Выписка ФГИС (ир) ЕГРН";
            res.DocTypeNick = "КВ";
            res.Namespace = NameSpaces.KVZU_08;
            CasterKPT11.Parse_KTP11Info(xmldoc, res);
            res.Version = "8";
            XmlNode parcel = xmldoc.DocumentElement.SelectSingleNode("/" + xmldoc.DocumentElement.Name 
                                        + "/land_record");
            //parse Parcel nodes:
            TCadastralBlock Bl = new TCadastralBlock(parcel.SelectSingleNode("object/common_data/quarter_cad_number").FirstChild.Value);
            Bl.Parcels.AddParcel(CasterKPT11.Parse_Parcel(parcel));
            XmlNode parcel_Rights = xmldoc.DocumentElement.SelectSingleNode("/" + xmldoc.DocumentElement.Name
                                       + "/right_records");
            XmlNode parcel_Restrict = xmldoc.DocumentElement.SelectSingleNode("/" + xmldoc.DocumentElement.Name
                                       + "/restrict_records");
            res.District.Blocks.Add(Bl);
            return res;
        }
        #endregion


        #region KVOKS 8 FGIS EGRN
        /// <summary>
        ///  FGIS EGRN 
        /// Name this KVOKS8
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="xmldoc"></param>
        /// <returns></returns>
        public netFteo.IO.FileInfo ParseKVOKS8(netFteo.IO.FileInfo fi, XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";
            res.DocType = "Выписка ФГИС (ир) ЕГРН";
            res.DocTypeNick = "КВ";
            res.Namespace = NameSpaces.KVOKS_081;
            CasterKPT11.Parse_KTP11Info(xmldoc, res);
            res.Version = "8";
            XmlNode parcel = xmldoc.DocumentElement.SelectSingleNode("/" + xmldoc.DocumentElement.Name
                                        + "/object_under_construction_record");
            //parse Parcel nodes:
            TCadastralBlock Bl = new TCadastralBlock(parcel.SelectSingleNode("object/common_data/quarter_cad_number").FirstChild.Value);
            Bl.Parcels.AddParcel(CasterKPT11.Parse_OKS(parcel));
            XmlNode parcel_Rights = xmldoc.DocumentElement.SelectSingleNode("/" + xmldoc.DocumentElement.Name
                                       + "/right_records");
            XmlNode parcel_Restrict = xmldoc.DocumentElement.SelectSingleNode("/" + xmldoc.DocumentElement.Name
                                       + "/restrict_records");
            res.District.Blocks.Add(Bl);
            return res;
        }
        #endregion

        #region  Разбор КПТ 11
        /// <summary>
        /// Parse XMLDocument. Types not used, also deserializations no needed
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="xmldoc"></param>
        /// <returns></returns>
        public netFteo.IO.FileInfo ParseKPT11(netFteo.IO.FileInfo fi, XmlDocument xmldoc) //RRTypes.kpt10_un.KPT KPT10)
        {
            // Simplifiyeng func: 
            //SplitKPT11(fi, xmldoc);
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";
            res.DocType = "Кадастровый план территории";
            res.DocTypeNick = "КПТ";
            res.Namespace = NameSpaces.KPT11;
            res.Version = "11";
            CasterKPT11.Parse_KTP11Info(xmldoc, res);

            XmlNodeList Blocksnodes = xmldoc.DocumentElement.SelectNodes("/" + xmldoc.DocumentElement.Name + "/cadastral_blocks/cadastral_block");
            if (Blocksnodes != null)
            {  //count items of every block:
                for (int i = 0; i <= Blocksnodes.Count - 1; i++)
                {
                    if (Blocksnodes[i].SelectSingleNode("record_data/base_data/land_records") != null)
                    {
                        this.TotalItems2Process += Blocksnodes[i].SelectSingleNode("record_data/base_data/land_records").ChildNodes.Count;
                    }
                    if (Blocksnodes[i].SelectSingleNode("record_data/base_data/build_records") != null)
                    {
                        TotalItems2Process += Blocksnodes[i].SelectSingleNode("record_data/base_data/build_records").ChildNodes.Count;
                    }
                    if (Blocksnodes[i].SelectSingleNode("record_data/base_data/construction_records") != null)
                    {
                        TotalItems2Process += Blocksnodes[i].SelectSingleNode("record_data/base_data/construction_records").ChildNodes.Count;
                    }
                    if (Blocksnodes[i].SelectSingleNode("record_data/base_data/object_under_construction_records") != null)
                    {
                        TotalItems2Process += Blocksnodes[i].SelectSingleNode("record_data/base_data/object_under_construction_records").ChildNodes.Count;
                    }
                }

                XMLParsingStartProc("start xml", TotalItems2Process, null);

                for (int i = 0; i <= Blocksnodes.Count - 1; i++)
                {
                    TCadastralBlock Bl = new TCadastralBlock(Blocksnodes[i].SelectSingleNode("cadastral_number").FirstChild.Value);

                    var parcels = Blocksnodes[i].SelectSingleNode("record_data/base_data/land_records");
                    var build_records = Blocksnodes[i].SelectSingleNode("record_data/base_data/build_records");
                    var construction_records = Blocksnodes[i].SelectSingleNode("record_data/base_data/construction_records");
                    var under_constr_records = Blocksnodes[i].SelectSingleNode("record_data/base_data/object_under_construction_records");
                    var boundary = Blocksnodes[i].SelectSingleNode("municipal_boundaries");
                    var inhabited_locality_boundaries = Blocksnodes[i].SelectSingleNode("inhabited_locality_boundaries");
                    //  zones_and_territories_boundaries/zones_and_territories_record
                    var zones = Blocksnodes[i].SelectSingleNode("zones_and_territories_boundaries");
                    var coastline_boundaries = Blocksnodes[i].SelectSingleNode("coastline_boundaries");
                    var surveying_project = Blocksnodes[i].SelectSingleNode("surveying_project");
                    if (parcels != null)
                    {
                        for (int iP = 0; iP <= parcels.ChildNodes.Count - 1; iP++)
                        {
                            XMLParsingProc("xml", ++FileParsePosition, null);
                            Bl.Parcels.AddParcel(CasterKPT11.Parse_Parcel(parcels.ChildNodes[iP]));
                        }
                    }

                    // Здания
                    if (build_records != null)
                    {
                        for (int iP = 0; iP <= build_records.ChildNodes.Count - 1; iP++)
                        {
                            System.Xml.XmlNode build = build_records.ChildNodes[iP];
                            //   object/common_data/cad_number

                            //Также параллельное TmyOKS
                            //TRealEstate Building = new TRealEstate(build.SelectSingleNode("object/common_data/cad_number").FirstChild.Value, CasterOKS.ObjectTypeToStr(netFteo.XML.XMLWrapper.SelectNodeChildValue(build, "object/common_data/type/code")));
                            TRealEstate Building = new TRealEstate(build.SelectSingleNode("object/common_data/cad_number").FirstChild.Value, netFteo.XML.XMLWrapper.SelectNodeChildValue(build, "object/common_data/type/code"));
                            Building.Building.AssignationBuilding = build.SelectSingleNode("params/purpose/value").FirstChild.Value;
                            Building.Location = CasterKPT11.Parse_Location(build.SelectSingleNode("address_location"));
                            if (build.SelectSingleNode("cost/value") != null)
                                Building.CadastralCost = Convert.ToDecimal(build.SelectSingleNode("cost/value").FirstChild.Value);
                            if (build.SelectSingleNode("contours/contour/entity_spatial") != null)
                            {
                                Building.EntSpat = CasterKPT11.LandEntSpatToES2(Building.CN, build.SelectSingleNode("contours/contour/entity_spatial"));
                                Building.EntSpat.ParseSpatial();
                            }

                            /*
                                Building.Building.AssignationBuilding = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.AssignationBuilding.ToString();
                                Building.Area = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.Area;
                    */
                            Bl.AddOKS(Building);
                            //res.MifOKSSpatialCollection.Add(Building.Building.ES);
                            XMLParsingProc("xml", ++FileParsePosition, null);
                        }
                    }
                    //Сооружения
                    if (construction_records != null)
                    {
                        for (int iP = 0; iP <= construction_records.ChildNodes.Count - 1; iP++)
                        {
                            System.Xml.XmlNode build = construction_records.ChildNodes[iP];
                            //   object/common_data/cad_number
                            //TRealEstate Construct = new TRealEstate(build.SelectSingleNode("object/common_data/cad_number").FirstChild.Value, RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(netFteo.XML.XMLWrapper.SelectNodeChildValue(build, "object/common_data/type/code")));
                            TRealEstate Construct = new TRealEstate(build.SelectSingleNode("object/common_data/cad_number").FirstChild.Value, netFteo.XML.XMLWrapper.SelectNodeChildValue(build, "object/common_data/type/code"));
                            if (build.SelectSingleNode("cost/value") != null)
                                Construct.CadastralCost = Convert.ToDecimal(build.SelectSingleNode("cost/value").FirstChild.Value);

                            if (build.SelectSingleNode("params/purpose") != null)
                                Construct.Construction.AssignationName = build.SelectSingleNode("params/purpose").FirstChild.Value;
                            Construct.Location = CasterKPT11.Parse_Location(build.SelectSingleNode("address_location"));
                            Bl.AddOKS(Construct);
                            XMLParsingProc("xml", ++FileParsePosition, null);
                        }
                    }
                    //ОНС
                    if (under_constr_records != null)
                    {
                        for (int iP = 0; iP <= under_constr_records.ChildNodes.Count - 1; iP++)
                        {
                            System.Xml.XmlNode under = under_constr_records.ChildNodes[iP];
                            //   object/common_data/cad_number
                            //TRealEstate UnderConstruct = new TRealEstate(under.SelectSingleNode("object/common_data/cad_number").FirstChild.Value, RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(netFteo.XML.XMLWrapper.SelectNodeChildValue(under, "object/common_data/type/code")));
                            TRealEstate UnderConstruct = new TRealEstate(under.SelectSingleNode("object/common_data/cad_number").FirstChild.Value, netFteo.XML.XMLWrapper.SelectNodeChildValue(under, "object/common_data/type/code"));
                            UnderConstruct.Location = CasterKPT11.Parse_Location(under.SelectSingleNode("address_location"));
                            if (under.SelectSingleNode("cost/value") != null)
                                UnderConstruct.CadastralCost = Convert.ToDecimal(under.SelectSingleNode("cost/value").FirstChild.Value);
                            if (under.SelectSingleNode("params/purpose") != null)
                                UnderConstruct.Uncompleted.AssignationName = under.SelectSingleNode("params/purpose").FirstChild.Value;
                            Bl.AddOKS(UnderConstruct);
                            XMLParsingProc("xml", ++FileParsePosition, null);
                        }
                    }
                    //Bounds 
                    if (boundary != null)
                    {
                        for (int iP = 0; iP <= boundary.ChildNodes.Count - 1; iP++)
                        {
                            System.Xml.XmlNode bound = boundary.ChildNodes[iP];
                            TBound BoundItem = new TBound(bound.SelectSingleNode("b_object_municipal_boundary/b_object/reg_numb_border").FirstChild.Value,
                                CasterKPT11.BoundToName(bound.SelectSingleNode("b_object_municipal_boundary /b_object/type_boundary/code").FirstChild.Value));
                            //TODO - spatial detecting:
                            BoundItem.SpatialElement = CasterKPT11.LandEntSpatToFteo(BoundItem.AccountNumber, bound.SelectSingleNode("b_contours_location/contours/contour/entity_spatial"));
                            res.District.SpatialData.AddRange(BoundItem.SpatialElement);
                            Bl.AddBound(BoundItem);
                        }
                    }

                    if (inhabited_locality_boundaries != null)
                    {
                        for (int iP = 0; iP <= inhabited_locality_boundaries.ChildNodes.Count - 1; iP++)
                        {
                            XmlNode bound = inhabited_locality_boundaries.ChildNodes[iP];
                            TBound BoundItem = new TBound(bound.SelectSingleNode("b_object_inhabited_locality_boundary/b_object/reg_numb_border").FirstChild.Value,
                                  CasterKPT11.BoundToName(bound.SelectSingleNode("b_object_inhabited_locality_boundary/b_object/type_boundary/code").FirstChild.Value));
                            BoundItem.EntitySpatial = CasterKPT11.LandEntSpatToES2(BoundItem.AccountNumber, bound.SelectSingleNode("b_contours_location/contours/contour/entity_spatial"));
                            res.District.SpatialData.AddRange(BoundItem.EntitySpatial);
                            Bl.AddBound(BoundItem);
                        }
                    }

                    if (coastline_boundaries != null)
                    {
                        for (int iP = 0; iP <= coastline_boundaries.ChildNodes.Count - 1; iP++)
                        {
                            XmlNode bound = coastline_boundaries.ChildNodes[iP];

                            TBound BoundItem = new TBound(bound.SelectSingleNode("b_object_zones_and_territories/b_object/reg_numb_border").FirstChild.Value,
                                  CasterKPT11.BoundToName(bound.SelectSingleNode("b_object_zones_and_territories/b_object/type_boundary/code").FirstChild.Value));
                            BoundItem.AccountNumber = bound.SelectSingleNode("b_object_zones_and_territories/b_object/reg_numb_border").FirstChild.Value;
                            BoundItem.Description = bound.SelectSingleNode("b_object_zones_and_territories /water/water_object_name").FirstChild.Value;
                            BoundItem.EntitySpatial = CasterKPT11.LandEntSpatToES2(BoundItem.AccountNumber, bound.SelectSingleNode("b_contours_location/contours/contour/entity_spatial"));
                            res.District.SpatialData.AddRange(BoundItem.EntitySpatial);
                            Bl.AddBound(BoundItem);
                        }
                    }

                    if (surveying_project != null)
                    {
                        for (int iP = 0; iP <= surveying_project.ChildNodes.Count - 1; iP++)
                        {
                            XmlNode bound = surveying_project.ChildNodes[iP];
                            /*
                            TBound BoundItem = new TBound(bound.SelectSingleNode("b_object_inhabited_locality_boundary/b_object/reg_numb_border").FirstChild.Value,
                                  CasterKPT11.BoundToName(bound.SelectSingleNode("b_object_inhabited_locality_boundary/b_object/type_boundary/code").FirstChild.Value));
                            BoundItem.EntitySpatial = CasterKPT11.LandEntSpatToES2(BoundItem.AccountNumber, bound.SelectSingleNode("b_contours_location/contours/contour/entity_spatial"));
                            res.District.SpatialData.AddRange(BoundItem.EntitySpatial);
                            Bl.AddBound(BoundItem);
                            */
                        }
                    }


                    //Zones:
                    if (zones != null)
                    {
                        for (int iP = 0; iP <= zones.ChildNodes.Count - 1; iP++)
                        {
                            System.Xml.XmlNode zone = zones.ChildNodes[iP];
                            TZone ZoneItem;

                            ZoneItem = new TZone(zone.SelectSingleNode("b_object_zones_and_territories/b_object/reg_numb_border").FirstChild.Value); //KPT10.CadastralBlocks[i].Zones[iP].AccountNumber);

                            ZoneItem.Description = netFteo.XML.XMLWrapper.SelectNodeChildValue(zone, "b_object_zones_and_territories/b_object/type_boundary/value") + " " +
                                                   netFteo.XML.XMLWrapper.SelectNodeChildValue(zone, "b_object_zones_and_territories/number");
                            ZoneItem.TypeName = netFteo.XML.XMLWrapper.SelectNodeChildValue(zone, "b_object_zones_and_territories/type_zone/value");
                            ZoneItem.EntitySpatial = CasterKPT11.LandEntSpatToES2(ZoneItem.AccountNumber, zone.SelectSingleNode("b_contours_location/contours/contour/entity_spatial"));
                            Bl.AddZone(ZoneItem);
                        }
                    }

                    //ОИПД Квартала:
                    //Виртуальный OIPD типа "Квартал":
                    if (Blocksnodes[i].SelectSingleNode("spatial_data") != null)
                    {
                        Bl.Entity_Spatial.ImportPolygon(CasterKPT11.LandEntSpatToFteo(Bl.CN,
                                                                               Blocksnodes[i].SelectSingleNode("spatial_data/entity_spatial")));
                        Bl.Entity_Spatial.Definition = "гр" + Bl.CN;
                    }

                    if (Blocksnodes[i].SelectSingleNode("spatial_data/entity_spatial/sk_id") != null)
                    {
                        res.District.CSs.Add(new TCoordSystem("СК",
                            Blocksnodes[i].SelectSingleNode("spatial_data/entity_spatial/sk_id").FirstChild.Value));
                    }

                    var oms_points = Blocksnodes[i].SelectSingleNode("oms_points");
                    if ( oms_points!= null)
                    {
                        for (int iP = 0; iP <= oms_points.ChildNodes.Count - 1; iP++)
                        {
                            XmlNode oms_point = oms_points.ChildNodes[iP];
                            GeodethicBase OMS = new GeodethicBase();
                            OMS.Name = iP.ToString();
                            OMS.PName = oms_point.SelectSingleNode("p_name").FirstChild.Value;
                            OMS.PNmb = oms_point.SelectSingleNode("p_nmb").FirstChild.Value;
                            OMS.PKlass = oms_point.SelectSingleNode("p_klass").FirstChild.Value;
                            OMS.x = Convert.ToDouble(oms_point.SelectSingleNode("ord_x").FirstChild.Value);
                            OMS.y = Convert.ToDouble(oms_point.SelectSingleNode("ord_y").FirstChild.Value);
                            Bl.AddOmsPoint(null);
                        }

                        /*
                          //Пункты в Квартале
                if (KPT10.CadastralBlocks[i].OMSPoints.Count > 0)
                {
                    for (int iP = 0; iP <= KPT10.CadastralBlocks[i].OMSPoints.Count - 1; iP++)
                    {
                        GeodethicBase OMS = new GeodethicBase();
                        OMS.PNmb = KPT10.CadastralBlocks[i].OMSPoints[iP].PNmb;       //94873
                        OMS.PKlass = KPT10.CadastralBlocks[i].OMSPoints[iP].PKlass;   //Класс ОМС – "; Система координат - МСК-26 от СК-95, зона 1
                        OMS.PName = KPT10.CadastralBlocks[i].OMSPoints[iP].PName;     //OMZ-117; 1Б; Труновский р-н, с. Донское, на пересечении ул. Кооперативная и ул. Ленина.
                        OMS.x = (double)KPT10.CadastralBlocks[i].OMSPoints[iP].OrdX;
                        OMS.y = (double)KPT10.CadastralBlocks[i].OMSPoints[iP].OrdY;
                        Bl.AddOmsPoint(OMS);
                    }
                }
                         */

                    }

                    res.District.Blocks.Add(Bl);
                }
            }
            return res;
        }

        #endregion

                #region  SchemaParcels V2.0
                public netFteo.IO.FileInfo ParseSchemaParcels(netFteo.IO.FileInfo fi, System.Xml.XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";
            res.DocType = "Схема на КПТ";
            res.DocTypeNick = "СКПТ";
            res.Version = xmldoc.DocumentElement.SelectSingleNode("/" + xmldoc.DocumentElement.Name + "/eDocument/@Version").FirstChild.Value;
            res.Number = xmldoc.DocumentElement.SelectSingleNode("/" + xmldoc.DocumentElement.Name + "/eDocument/@GUID").FirstChild.Value;

            // /SchemaParcels/Document/Name

            // /SchemaParcels/Document/Date

            // /SchemaParcels/Document/Number

            /// SchemaParcels / Document / IssueOrgans / IssueOrgan
            res.AppointmentFIO = xmldoc.DocumentElement.SelectSingleNode("/" + xmldoc.DocumentElement.Name + "/Document/IssueOrgans/IssueOrgan").FirstChild.Value;
            res.Cert_Doc_Organization = xmldoc.DocumentElement.SelectSingleNode("/" + xmldoc.DocumentElement.Name + "/Document/Name").FirstChild.Value +
                 "\n\r" +
                xmldoc.DocumentElement.SelectSingleNode("/" + xmldoc.DocumentElement.Name + "/Document/Number").FirstChild.Value +
                "  " +
               xmldoc.DocumentElement.SelectSingleNode("/" + xmldoc.DocumentElement.Name + "/Document/Date").FirstChild.Value;
            //TODO : 
            //  SchemaParcels / NewParcels
            System.Xml.XmlNodeList NewParcels = xmldoc.DocumentElement.SelectNodes("/" + xmldoc.DocumentElement.Name + "/NewParcels/NewParcel");
            string ParcelDefinition = "ЗУ";
            if (NewParcels != null)
            {  //count items of every parcel:
                for (int i = 0; i <= NewParcels.Count - 1; i++)
                {
                    // / SchemaParcels / NewParcels / NewParcel / CadastralBlock
                    if (NewParcels[i].SelectSingleNode("CadastralBlock") != null)
                    {
                        TCadastralBlock Bl = new TCadastralBlock(NewParcels[i].SelectSingleNode("CadastralBlock").FirstChild.Value);
                        TParcel MainObj = Bl.Parcels.AddParcel(new TParcel());
                        MainObj.Definition = ParcelDefinition + (i + 1).ToString();
                        if (NewParcels[i].SelectSingleNode("Note") != null)
                        {
                            MainObj.Location.Address.Note = NewParcels[i].SelectSingleNode("Note").FirstChild.Value;
                        }

                        // / SchemaParcels / NewParcels / NewParcel / Utilization / @ByDoc
                        if (NewParcels[i].SelectSingleNode("Utilization") != null)
                        {
                            MainObj.Utilization.UtilbyDoc = NewParcels[i].SelectSingleNode("Utilization/@ByDoc").FirstChild.Value;
                        }
                        // / SchemaParcels / NewParcels / NewParcel / Category / @Category
                        if (NewParcels[i].SelectSingleNode("Category") != null)
                        {
                            MainObj.Category = NewParcels[i].SelectSingleNode("Category/@Category").FirstChild.Value;
                        }
                        // / SchemaParcels / NewParcels / NewParcel / Area / Area /#text
                        if (NewParcels[i].SelectSingleNode("Area/Area") != null)
                        {
                            MainObj.AreaValue = NewParcels[i].SelectSingleNode("Area/Area").FirstChild.Value;
                        }

                        // / SchemaParcels / NewParcels / NewParcel / Note
                        if (NewParcels[i].SelectSingleNode("Note") != null)
                        {
                            MainObj.SpecialNote = NewParcels[i].SelectSingleNode("Note").FirstChild.Value;
                            MainObj.Location.Address.Note = NewParcels[i].SelectSingleNode("Note").FirstChild.Value;
                            MainObj.Location.Address.Region = "99"; // fake region
                        }
                        // / SchemaParcels / NewParcels / NewParcel / Entity_Spatial / @Ent_Sys
                        // Single ES
                        // / SchemaParcels / NewParcels / NewParcel / Entity_Spatial / Spatial_Element / Spelement_Unit[1] / @Type_Unit
                        // / SchemaParcels / NewParcels / NewParcel / Entity_Spatial / Spatial_Element / Spelement_Unit[1] / NewOrdinate / @Num_Geopoint
                        // / SchemaParcels / NewParcels / NewParcel / Entity_Spatial / Spatial_Element / Spelement_Unit[1] / NewOrdinate / @X

                        if (NewParcels[i].SelectSingleNode("Entity_Spatial") != null)
                            MainObj.EntSpat.AddPolygon(SchemaParcelsEStoFteo(NewParcels[i].SelectSingleNode("Entity_Spatial")));
                        // otherweis : multicontours:
                        // /SchemaParcels/NewParcels/NewParcel/Contours
                        // /SchemaParcels/NewParcels/NewParcel/Contours/Contour[1]/@Definition
                        // /SchemaParcels/NewParcels/NewParcel/Contours/Contour[1]/Entity_Spatial/Spatial_Element/Spelement_Unit[1]/NewOrdinate/@Num_Geopoint

                        if (NewParcels[i].SelectSingleNode("Contours") != null)
                        {
                            System.Xml.XmlNode contours = NewParcels[i].SelectSingleNode("Contours");
                            for (int ic = 0; ic <= NewParcels[i].SelectSingleNode("Contours").ChildNodes.Count - 1; ic++)
                            {
                                TPolygon NewCont = SchemaParcelsEStoFteo(contours.ChildNodes[ic].SelectSingleNode("Entity_Spatial"));
                                NewCont.Definition = /* MainObj.Definition + */  NewParcels[i].SelectSingleNode("Contours").ChildNodes[ic].Attributes.GetNamedItem("Definition").Value;
                                MainObj.EntSpat.Add(NewCont);
                            }
                        }
                        res.District.Blocks.Add(Bl);
                    }
                }
            }
            return res;
        }
        #endregion

        #region  Разбор KPZU 5.0.8
        public netFteo.IO.FileInfo ParseKPZU508(netFteo.IO.FileInfo fi, Stream ms) //RRTypes.kpzu06.KPZU kp, XmlDocument xmldoc)
        {
            ms.Seek(0, 0);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(ms);
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            RRTypes.kpzu.KPZU kp = (kpzu.KPZU)Desearialize<kpzu.KPZU>(xmldoc);
            TCadastralBlock Bl = new TCadastralBlock();
            //----------
            for (int i = 0; i <= kp.CoordSystems.Count - 1; i++)
            {

                res.District.CSs.Add(new TCoordSystem(kp.CoordSystems[i].Name, kp.CoordSystems[i].CsId));

            }

            TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(kp.Parcel.CadastralNumber, kp.Parcel.Name.ToString()));
            MainObj.CadastralBlock = kp.Parcel.CadastralBlock;
            Bl.CN = kp.Parcel.CadastralBlock;
            MainObj.SpecialNote = kp.Parcel.SpecialNote;
            res.CommentsType = "Особые отметки";
            //res.Comments    = kp.Parcel.SpecialNote;
            MainObj.AreaGKN = kp.Parcel.Area.Area;
            MainObj.Utilization.UtilbyDoc = kp.Parcel.Utilization.ByDoc;
            if (kp.Parcel.Utilization.UtilizationSpecified)
                MainObj.Utilization.Untilization = kp.Parcel.Utilization.Utilization.ToString();
            MainObj.DateCreated = kp.Parcel.DateCreated.ToString("dd.MM.yyyy");
            MainObj.CadastralCost = kp.Parcel.CadastralCost.Value;
            MainObj.Category = kp.Parcel.Category.ToString();// netFteo.Rosreestr.dCategoriesv01.ItemToName(kp.Parcel.Category.ToString());
            MainObj.Location = RRTypes.CommonCast.CasterZU.CastLocation(kp.Parcel.Location);
            MainObj.Rights = EGRP_v60Utils.ParseEGRRights(xmldoc);
            MainObj.Encumbrances = KPZU_v05Utils.KPZUEncumstoFteoEncums(kp.Parcel.Encumbrances);
            if (kp.Parcel.PrevCadastralNumbers != null)
                MainObj.PrevCadastralNumbers.AddRange(kp.Parcel.PrevCadastralNumbers);
            if (kp.Parcel.AllOffspringParcel != null) //Кадастровые номера всех земельных участков, образованных из данного земельного участка (строка 17.4)
                MainObj.AllOffspringParcel.AddRange(kp.Parcel.AllOffspringParcel);
            if (kp.Parcel.InnerCadastralNumbers != null)
                foreach (string s in kp.Parcel.InnerCadastralNumbers)
                    MainObj.InnerCadastralNumbers.Add(s);

            //OIPD:
            //Землепользование

            if (kp.Parcel.EntitySpatial != null)
                if (kp.Parcel.EntitySpatial.SpatialElement.Count > 0)
                {
                    MainObj.EntSpat.Add(CasterSpatial.AddEntSpatKPZU05(kp.Parcel.CadastralNumber, kp.Parcel.EntitySpatial));
                }

            //Многоконтурный
            if (kp.Parcel.Contours != null)
            {

                for (int ic = 0; ic <= kp.Parcel.Contours.Count - 1; ic++)
                {
                    /*
					res.MyBlocks.SpatialData.Add(CommonCast.CasterZU.AddEntSpatKPZU05(kp.Parcel.CadastralNumber + "(" +
														 kp.Parcel.Contours[ic].NumberRecord + ")",
														 kp.Parcel.Contours[ic].EntitySpatial));
					*/
                    TPolygon NewCont = CasterSpatial.AddEntSpatKPZU05(kp.Parcel.CadastralNumber + "(" +
                                                          kp.Parcel.Contours[ic].NumberRecord + ")",
                                                          kp.Parcel.Contours[ic].EntitySpatial);
                    NewCont.AreaValue = kp.Parcel.Contours[ic].Area.Area;
                    MainObj.EntSpat.Add(NewCont);
                }
            }
            //ЕЗП:
            if (kp.Parcel.CompositionEZ.Count > 0)
            {
                for (int i = 0; i <= kp.Parcel.CompositionEZ.Count - 1; i++)
                {
                    MainObj.AddEntry(kp.Parcel.CompositionEZ[i].CadastralNumber,
                        kp.Parcel.CompositionEZ[i].Area.Area,
                        0,
                        6, //для сведений ЕГРН это всегда "учтеный"
                        CasterSpatial.AddEntSpatKPZU05(kp.Parcel.CompositionEZ[i].CadastralNumber,
                                                                     kp.Parcel.CompositionEZ[i].EntitySpatial));

                    /*
					res.MyBlocks.SpatialData.Add(CommonCast.CasterZU.AddEntSpatKPZU05(kp.Parcel.CompositionEZ[i].CadastralNumber,
														  kp.Parcel.CompositionEZ[i].EntitySpatial));
					*/
                }
            }
            //Части 
            if (kp.Parcel.SubParcels.Count > 0)
            {
                for (int i = 0; i <= kp.Parcel.SubParcels.Count - 1; i++)
                {
                    TSlot Sl = MainObj.AddSubParcel(kp.Parcel.SubParcels[i].NumberRecord);
                    Sl.AreaGKN = kp.Parcel.SubParcels[i].Area.Area.ToString();

                    if (kp.Parcel.SubParcels[i].Encumbrance != null)
                        Sl.Encumbrances.Add(KPZU_v05Utils.KVZUEncumtoFteoEncum(kp.Parcel.SubParcels[i].Encumbrance));
                    if (kp.Parcel.SubParcels[i].EntitySpatial != null)
                    {
                        TPolygon SlEs = CasterSpatial.AddEntSpatKPZU05(kp.Parcel.SubParcels[i].NumberRecord,
                                                                               kp.Parcel.SubParcels[i].EntitySpatial);
                        Sl.SpatialElement.ImportPolygon(SlEs);
                    }

                }
            }

            //Прикрутим сюды парсинг через XPATH ЕГРН
            MainObj.EGRN = RRTypes.CommonCast.CasterEGRP.ParseEGRNRights(xmldoc); // мдаааа!!!
            res.District.Blocks.Add(Bl);
            res.DocTypeNick = "КПЗУ";
            res.DocType = "Кадастровый паспорт земельного участка";
            res.Version = "5.0.8";
            res.Namespace = netFteo.Rosreestr.NameSpaces.KPZU_05;
            CommonCast.CasterEGRP.Parse_DocumentProperties(xmldoc, res);
            Parse_Contractors(xmldoc, res);
            return res;
        }
        #endregion

        #region  Разбор KPZU 6.0.1 (как бы ЕГРН)
        /// <summary>
        /// Разбор выписка ЕГРН
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="xmldoc">файл по схеме urn://x-artefacts-rosreestr-ru/outgoing/kpzu/6.0.1</param>
        /// <returns></returns>
        public netFteo.IO.FileInfo ParseKPZU(netFteo.IO.FileInfo fi, System.Xml.XmlDocument xmldoc) //RRTypes.kpzu06.KPZU kp, XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            RRTypes.kpzu06.KPZU kp = (RRTypes.kpzu06.KPZU)Desearialize<RRTypes.kpzu06.KPZU>(xmldoc);
            TCadastralBlock Bl = new TCadastralBlock();

            for (int i = 0; i <= kp.CoordSystems.Count - 1; i++)
            {

                res.District.CSs.Add(new TCoordSystem(kp.CoordSystems[i].Name, kp.CoordSystems[i].CsId));

            }

            TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(kp.Parcel.CadastralNumber, kp.Parcel.Name.ToString()));
            MainObj.CadastralBlock = kp.Parcel.CadastralBlock;
            Bl.CN = kp.Parcel.CadastralBlock;
            MainObj.SpecialNote = kp.Parcel.SpecialNote;
            res.CommentsType = "Особые отметки";
            //res.Comments    = kp.Parcel.SpecialNote;
            MainObj.AreaGKN = kp.Parcel.Area.Area;
            MainObj.Utilization.UtilbyDoc = kp.Parcel.Utilization.ByDoc;
            if (kp.Parcel.Utilization.UtilizationSpecified)
                MainObj.Utilization.Untilization = kp.Parcel.Utilization.Utilization.ToString();
            MainObj.DateCreated = kp.Parcel.DateCreated.ToString("dd.MM.yyyy");
            MainObj.Category = kp.Parcel.Category.ToString();// netFteo.Rosreestr.dCategoriesv01.ItemToName(kp.Parcel.Category.ToString());
            MainObj.Location = RRTypes.CommonCast.CasterZU.CastLocation(kp.Parcel.Location);
            MainObj.Rights = KVZU_v06Utils.KVZURightstoFteorights(kp.Parcel.Rights);
            MainObj.Encumbrances = KPZU_v05Utils.KPZUEncumstoFteoEncums(kp.Parcel.Encumbrances);
            if (kp.Parcel.PrevCadastralNumbers != null)
                MainObj.PrevCadastralNumbers.AddRange(kp.Parcel.PrevCadastralNumbers);
            if (kp.Parcel.AllOffspringParcel != null) //Кадастровые номера всех земельных участков, образованных из данного земельного участка (строка 17.4)
                MainObj.AllOffspringParcel.AddRange(kp.Parcel.AllOffspringParcel);
            if (kp.Parcel.InnerCadastralNumbers != null)
                foreach (string s in kp.Parcel.InnerCadastralNumbers)
                    MainObj.InnerCadastralNumbers.Add(s);

            //OIPD:
            //Землепользование

            if (kp.Parcel.EntitySpatial != null)
                if (kp.Parcel.EntitySpatial.SpatialElement.Count > 0)
                {
                    MainObj.EntSpat.Add(CasterSpatial.AddEntSpatKPZU06(kp.Parcel.CadastralNumber, kp.Parcel.EntitySpatial));
                }
            //Многоконтурный
            if (kp.Parcel.Contours != null)
            {

                for (int ic = 0; ic <= kp.Parcel.Contours.Count - 1; ic++)
                {
                    res.District.SpatialData.Add(CasterSpatial.AddEntSpatKPZU06(kp.Parcel.CadastralNumber + "(" +
                                                         kp.Parcel.Contours[ic].NumberRecord + ")",
                                                         kp.Parcel.Contours[ic].EntitySpatial));
                    TPolygon NewCont = CasterSpatial.AddEntSpatKPZU06(kp.Parcel.CadastralNumber + "(" +
                                                          kp.Parcel.Contours[ic].NumberRecord + ")",
                                                          kp.Parcel.Contours[ic].EntitySpatial);
                    NewCont.AreaValue = kp.Parcel.Contours[ic].Area.Area;
                    MainObj.EntSpat.Add(NewCont);
                }
            }
            //ЕЗП:
            if (kp.Parcel.CompositionEZ.Count > 0)
            {
                for (int i = 0; i <= kp.Parcel.CompositionEZ.Count - 1; i++)
                {
                    MainObj.AddEntry(kp.Parcel.CompositionEZ[i].CadastralNumber,
                        kp.Parcel.CompositionEZ[i].Area.Area,
                        0,
                        6, //для сведений ЕГРН это всегда "учтеный"
                        CasterSpatial.AddEntSpatKPZU06(kp.Parcel.CompositionEZ[i].CadastralNumber,
                                                                     kp.Parcel.CompositionEZ[i].EntitySpatial));

                    /*
					res.MyBlocks.SpatialData.Add(CommonCast.CasterZU.AddEntSpatKPZU06(kp.Parcel.CompositionEZ[i].CadastralNumber,
														  kp.Parcel.CompositionEZ[i].EntitySpatial));
*/
                }
            }
            //Части 
            if (kp.Parcel.SubParcels.Count > 0)
            {
                for (int i = 0; i <= kp.Parcel.SubParcels.Count - 1; i++)
                {
                    TSlot Sl = MainObj.AddSubParcel(kp.Parcel.SubParcels[i].NumberRecord);
                    Sl.AreaGKN = kp.Parcel.SubParcels[i].Area.Area.ToString();

                    if (kp.Parcel.SubParcels[i].Encumbrance != null)
                        Sl.Encumbrances.Add(KPZU_v05Utils.KVZUEncumtoFteoEncum(kp.Parcel.SubParcels[i].Encumbrance));

                    if (kp.Parcel.SubParcels[i].EntitySpatial != null)
                    {
                        Sl.ES.AddFeatures(CasterSpatial.ParseES_KPZU06(kp.Parcel.SubParcels[i].NumberRecord,
                                                                               kp.Parcel.SubParcels[i].EntitySpatial));
                    }
                    /*
                    {
                        TPolygon SlEs = CasterZU.AddEntSpatKPZU06(kp.Parcel.SubParcels[i].NumberRecord,
                                                                               kp.Parcel.SubParcels[i].EntitySpatial);
                        Sl.SpatialElement.ImportPolygon(SlEs);
                        res.MyBlocks.SpatialData.Add(SlEs);
                    }
                    */

                }
            }

            //Прикрутим сюды парсинг через XPATH ЕГРН
            MainObj.EGRN = RRTypes.CommonCast.CasterEGRP.ParseEGRNRights(xmldoc); // мдаааа!!!
            res.District.Blocks.Add(Bl);
            res.DocType = "Кадастровый паспорт земельного участка";
            res.DocTypeNick = "ЕГРН";
            res.Version = "6.0.1";
            CommonCast.CasterEGRP.Parse_DocumentProperties(xmldoc, res);
            Parse_Contractors(xmldoc, res);
            return res;
        }

        #endregion

        #region разбор КВЗУ04 - Region_Cadastr_Vidimus_KV V04
        public netFteo.IO.FileInfo ParseKVZU04(netFteo.IO.FileInfo fi, Stream xmldocStream)
        {
            xmldocStream.Seek(0,0);
            System.Xml.XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(xmldocStream);
            netFteo.IO.FileInfo res = InitFileInfo(fi, null);
            STD_KV04.Region_Cadastr_Vidimus_KV kv = (STD_KV04.Region_Cadastr_Vidimus_KV)Desearialize<STD_KV04.Region_Cadastr_Vidimus_KV>(xmldocStream);


            for (int i = 0; i <= kv.Coord_Systems.Count - 1; i++)
            {
                res.District.CSs.Add(new TCoordSystem(kv.Coord_Systems[i].Name, kv.Coord_Systems[i].Cs_Id));
            }

            Parse_Contractors(xmldoc, res);
            foreach (STD_KV04.tCadastral_Region region in kv.Package.Federal.Cadastral_Regions)
                foreach (STD_KV04.tCadastr_District district in region.Cadastral_Districts)
                    foreach (STD_KV04.tCadastral_Block block in district.Cadastral_Blocks)
                    {
                        TCadastralBlock Bl = new TCadastralBlock(block.CadastralNumber);
                        foreach (STD_KV04.ParcelsParcel parcel in block.Parcels.Parcel)
                        {
                            TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(parcel.CadastralNumber, parcel.Name.ToString()));
                            MainObj.CadastralBlock = block.CadastralNumber;
                            //MainObj.SpecialNote = parcel.SpecialNote;
                            MainObj.Utilization.UtilbyDoc = parcel.Utilization.ByDoc;
                            MainObj.Category = parcel.Category.Category.ToString();// netFteo.Rosreestr.dCategoriesv01.ItemToName(parcel.Category.ToString());
                                                                                   //MainObj.Location = RRTypes.CommonCast.CasterZU.CastLocation(parcel.Location);
                            MainObj.Rights = KV04_Utils.KVZURightstoFteorights(parcel.Rights);
                            //MainObj.Encumbrances = KV04_Utils.KVZUEncumstoFteoEncums(parcel. ???.)


                            MainObj.AreaGKN = parcel.Areas[0].Area.ToString();
                            MainObj.State = parcel.State.ToString();
                            MainObj.DateCreated = parcel.DateCreated.ToString("dd.MM.yyyy");
                            //MainObj.CadastralCost = parcel. kv.Parcels.Parcel.CadastralCost.Value;
                            //Землепользование
                            if (parcel.Entity_Spatial != null)
                                if (parcel.Entity_Spatial.Spatial_Element.Count > 0)
                                {
                                    TPolygon ents = KV04_Utils.AddEntSpatKVZU04(parcel.CadastralNumber,
                                                                           parcel.Entity_Spatial);
                                    ents.Parent_Id = MainObj.id;
                                    MainObj.EntSpat.Add(ents);
                                }

                            //Многоконтурный
                            if (parcel.Contours != null)
                            {
                                //   if (MainObj.Contours == null) MainObj.Contours = new TPolygonCollection(MainObj.id);
                                for (int ic = 0; ic <= parcel.Contours.Count - 1; ic++)
                                {
                                    res.District.SpatialData.Add(KV04_Utils.AddEntSpatKVZU04(parcel.Contours[ic].Number_PP,
                                                                                                 parcel.Contours[ic].Entity_Spatial));
                                    TPolygon NewCont = KV04_Utils.AddEntSpatKVZU04(parcel.Contours[ic].Number_PP,
                                                                                                                            parcel.Contours[ic].Entity_Spatial);
                                    NewCont.AreaValue = parcel.Contours[ic].Areas[0].Area;
                                    MainObj.EntSpat.Add(NewCont);
                                }
                            }
                            //ЕЗП: ??? Части - они же entrys:
                            if (parcel.SubParcels.Count > 0)
                            {
                                for (int i = 0; i <= parcel.SubParcels.Count - 1; i++)
                                {
                                    if (parcel.SubParcels[i].Entity_Spatial != null)
                                    {
                                        TPolygon SlEs = KV04_Utils.AddEntSpatKVZU04(parcel.SubParcels[i].Number_PP,
                                            parcel.SubParcels[i].Entity_Spatial);

                                        if (parcel.SubParcels[i].Object_Entry != null)
                                        {
                                            MainObj.AddEntry(parcel.SubParcels[i].Object_Entry.CadastralNumber, -1, -1, 6, SlEs);
                                            res.District.SpatialData.Add(SlEs);
                                        }
                                    }
                                }

                            }



                            //Части
                            if (parcel.SubParcels.Count > 0)
                            {
                                for (int i = 0; i <= parcel.SubParcels.Count - 1; i++)
                                {
                                    if (parcel.SubParcels[i].Object_Entry == null)
                                    {
                                        TSlot Sl = MainObj.AddSubParcel(parcel.SubParcels[i].Number_PP);

                                        Sl.AreaGKN = parcel.SubParcels[i].Areas[0].Area.ToString();

                                        if ((parcel.SubParcels[i].Encumbrances != null) &&
                                            (parcel.SubParcels[i].Encumbrances.Count == 1))
                                            Sl.Encumbrances.Add(KV04_Utils.KVZUEncumtoFteoEncum(parcel.SubParcels[i].Encumbrances[0]));

                                        if (parcel.SubParcels[i].Entity_Spatial != null)
                                        {
                                            TPolygon SlEs = KV04_Utils.AddEntSpatKVZU04(parcel.SubParcels[i].Number_PP,
                                                parcel.SubParcels[i].Entity_Spatial);
                                            Sl.SpatialElement.ImportPolygon(SlEs);
                                            res.District.SpatialData.Add(SlEs);
                                        }
                                    }

                                }
                            }

                            MainObj.EGRN = RRTypes.CommonCast.CasterEGRP.ParseEGRNRights(xmldoc); // мдаааа!!!
                        }
                        res.District.Blocks.Add(Bl);
                    }
            //Прикрутим сюды парсинг Прав ветки ЕГРП через XPATH ЕГРН

            res.DocTypeNick = "КВЗУ";
            res.Version = "04";
            res.Namespace = netFteo.Rosreestr.NameSpaces.KVZU_04;
            CommonCast.CasterEGRP.Parse_DocumentProperties(xmldoc, res);
            Parse_ContractorsV04(xmldoc, res);

            return res;
        }

        #endregion

        #region разбор КВ KVZU_05

        /// <summary>
        /// ParseKVZU05 without type deserialization
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="xmldoc"> XmlDocument (monster Hi)</param>
        /// <returns></returns>
        public netFteo.IO.FileInfo ParseKVZU05(netFteo.IO.FileInfo fi,XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            // Region_Cadastr_Vidimus_KV / @Version
            if ((xmldoc.DocumentElement.Name == "Region_Cadastr_Vidimus_KV") &&
                (xmldoc.DocumentElement.Attributes.GetNamedItem("Version") != null) &&
                (xmldoc.DocumentElement.Attributes.GetNamedItem("Version").Value == "05"))
            {
                res.DocTypeNick = "КВЗУ";
                res.Version = "05";
                res.Namespace = netFteo.Rosreestr.NameSpaces.KVZU_05;
            }

            //TODO
            // / Region_Cadastr_Vidimus_KV / Package / Parcels / Parcel / CadastralBlock /#text
            XmlNodeList Parcelnodes = xmldoc.DocumentElement.SelectNodes("/" + xmldoc.DocumentElement.Name + "/Package /Parcels/Parcel");
            if (Parcelnodes != null)
            {
                this.TotalItems2Process = Parcelnodes.Count;
                XMLParsingStartProc("start xml", TotalItems2Process, null);

                foreach (XmlNode Parcel in Parcelnodes)
                {
                    TCadastralBlock Bl = new TCadastralBlock(Parcel.SelectSingleNode("CadastralBlock").FirstChild.Value);
                    TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(Parcel.Attributes.GetNamedItem("CadastralNumber").Value, Parcel.Attributes.GetNamedItem("Name").Value));

                    // /Region_Cadastr_Vidimus_KV/Package/Parcels/Parcel/Area/Area/#text
                    MainObj.AreaGKN = Parcel.SelectSingleNode("Area/Area").FirstChild.Value; // идентично : .SelectSingleNode("Area").SelectSingleNode("Area")
                    MainObj.State = Parcel.Attributes.GetNamedItem("State").Value;
                    MainObj.DateCreated = Parcel.Attributes.GetNamedItem("DateCreated").Value;//.ToString("dd.MM.yyyy");
                                                                                              //  /Region_Cadastr_Vidimus_KV/Package/Parcels/Parcel/CadastralCost/@Value
                    if (Parcel.SelectSingleNode("CadastralCost/@Value") != null)
                        MainObj.CadastralCost = Convert.ToDecimal(Parcel.SelectSingleNode("CadastralCost/@Value").Value);
                    // Region_Cadastr_Vidimus_KV / Package / Parcels / Parcel / Utilization / @ByDoc
                    if (Parcel.SelectSingleNode("Utilization").Attributes.GetNamedItem("ByDoc") != null)
                        MainObj.Utilization.UtilbyDoc = Parcel.SelectSingleNode("Utilization").Attributes.GetNamedItem("ByDoc").Value;
                    // Region_Cadastr_Vidimus_KV / Package / Parcels / Parcel / Category / @Category
                    MainObj.Category = Parcel.SelectSingleNode("Category").Attributes.GetNamedItem("Category").Value;//netFteo.Rosreestr.dCategoriesv01.ItemToName(parcel.SelectSingleNode("Category").Attributes.GetNamedItem("Category").Value);
                                                                                                                     // Region_Cadastr_Vidimus_KV / Package / Parcels / Parcel / Location / Address / Note
                    MainObj.Location = this.Parse_Location(Parcel.SelectSingleNode("Location"));
                    if (Parcel.SelectSingleNode("Unified_Land_Unit/Preceding_Land_Unit") != null)
                        MainObj.ParentCN = Parcel.SelectSingleNode("Unified_Land_Unit/Preceding_Land_Unit").FirstChild.Value;

                    if (Parcel.SelectSingleNode("Entity_Spatial") != null)
                    {
                        TPolygon ents = KPT08LandEntSpatToFteo(MainObj.CN,
                                                              Parcel.SelectSingleNode("Entity_Spatial"));
                        //ents.AreaValue = (decimal)Convert.ToDouble(Parcel.SelectSingleNode("Areas/Area/Area").FirstChild.Value);
                        ents.Parent_Id = MainObj.id;
                        ents.Definition = MainObj.CN;
                        MainObj.EntSpat.Add(ents);
                    }

                    XmlNodeList Contours = Parcel.SelectNodes("Contours/Contour");
                    if (Contours != null)
                    {
                        foreach (XmlNode Contour in Contours)
                        {
                            MainObj.EntSpat.Add(KPT08LandEntSpatToFteo(Contour.SelectSingleNode("@Number_Record").Value, //Attributes.GetNamedItem("Number_Record").Value,
                                                                      Contour.SelectSingleNode("Entity_Spatial")));
                        }
                    }

                    // Region_Cadastr_Vidimus_KV / Package / Parcels / Parcel / SubParcels / SubParcel[1] / @Number_Record
                    // Region_Cadastr_Vidimus_KV / Package / Parcels / Parcel / SubParcels / SubParcel[1] / Object_Entry / @CadastralNumber
                    // Region_Cadastr_Vidimus_KV / Package / Parcels / Parcel / SubParcels / SubParcel[1] / Entity_Spatial
                    XmlNodeList EZPEntrys = Parcel.SelectNodes("SubParcels/SubParcel");
                    if (EZPEntrys != null)
                    {
                        foreach (XmlNode Entry in EZPEntrys)
                        {
                            if (Entry.SelectSingleNode("Object_Entry") != null)
                            {

                                string CN = Entry.SelectSingleNode("Object_Entry").Attributes.GetNamedItem("CadastralNumber").Value;
                                // SubParcel[1] / Area / Area /#text
                                // SubParcel[1] / Entity_Spatial

                                MainObj.AddEntry(CN, -1, -1,
                                    -1, KPT08LandEntSpatToFteo(CN, Entry.SelectSingleNode("Entity_Spatial")));
                            }

                        }
                    }



                    res.District.Blocks.Add(Bl);
                }
            }

            CommonCast.CasterEGRP.Parse_DocumentProperties(xmldoc, res);
            Parse_ContractorsV04(xmldoc, res);
            return res;
        }
        #endregion

        #region разбор КВ KVZU_06
        /*-----------------------------------------------------------------------------------------------------------*/
        public netFteo.IO.FileInfo ParseKVZU06(netFteo.IO.FileInfo fi, System.Xml.XmlDocument xmldoc)
        {


            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);

            RRTypes.kvzu.KVZU kv = (RRTypes.kvzu.KVZU)Desearialize<RRTypes.kvzu.KVZU>(xmldoc);

            TCadastralBlock Bl = new TCadastralBlock();


            for (int i = 0; i <= kv.CoordSystems.Count - 1; i++)
            {
                res.District.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));

            }
            TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(kv.Parcels.Parcel.CadastralNumber, kv.Parcels.Parcel.Name.ToString()));
            MainObj.CadastralBlock = kv.Parcels.Parcel.CadastralBlock;
            //MainObj.SpecialNote = kv.Parcels.Parcel.SpecialNote;
            MainObj.Utilization.UtilbyDoc = kv.Parcels.Parcel.Utilization.ByDoc;
            MainObj.Category = kv.Parcels.Parcel.Category.ToString();// netFteo.Rosreestr.dCategoriesv01.ItemToName(kv.Parcels.Parcel.Category.ToString());
            MainObj.Location = RRTypes.CommonCast.CasterZU.CastLocation(kv.Parcels.Parcel.Location);
            MainObj.Rights = KVZU_v06Utils.KVZURightstoFteorights(kv.Parcels.Parcel.Rights);
            MainObj.Encumbrances = KVZU_v06Utils.KVZUEncumstoFteoEncums(kv.Parcels.Parcel.Encumbrances);
            MainObj.AreaGKN = kv.Parcels.Parcel.Area.Area;
            MainObj.State = kv.Parcels.Parcel.State.ToString();
            if (kv.Parcels.Parcel.CadastralCost != null)
                MainObj.CadastralCost = kv.Parcels.Parcel.CadastralCost.Value;
            MainObj.DateCreated = kv.Parcels.Parcel.DateCreated.ToString("dd.MM.yyyy");

            Bl.CN = kv.Parcels.Parcel.CadastralBlock;

            //Землепользование
            if (kv.Parcels.Parcel.EntitySpatial != null)
                if (kv.Parcels.Parcel.EntitySpatial.SpatialElement.Count > 0)
                {
                    TPolygon ents = KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.Parcel.CadastralNumber,
                                                           kv.Parcels.Parcel.EntitySpatial);
                    ents.Parent_Id = MainObj.id;
                    MainObj.EntSpat.Add(ents);
                }
            //Многоконтурный
            if (kv.Parcels.Parcel.Contours != null)
            {
                //   if (MainObj.Contours == null) MainObj.Contours = new TPolygonCollection(MainObj.id);
                for (int ic = 0; ic <= kv.Parcels.Parcel.Contours.Count - 1; ic++)
                {
                    res.District.SpatialData.Add(RRTypes.KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.Parcel.Contours[ic].NumberRecord,
                                                                                 kv.Parcels.Parcel.Contours[ic].EntitySpatial));
                    TPolygon NewCont = KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.Parcel.Contours[ic].NumberRecord,
                                                                                                            kv.Parcels.Parcel.Contours[ic].EntitySpatial);
                    NewCont.AreaValue = kv.Parcels.Parcel.Contours[ic].Area.Area;
                    MainObj.EntSpat.Add(NewCont);
                }
            }
            //ЕЗП:
            if (kv.Parcels.Parcel.CompositionEZ.Count > 0)
            {
                for (int i = 0; i <= kv.Parcels.Parcel.CompositionEZ.Count - 1; i++)
                // if ( kv.Parcels.Parcel.CompositionEZ[i].EntitySpatial != null)
                {
                    MainObj.AddEntry(kv.Parcels.Parcel.CompositionEZ[i].CadastralNumber, kv.Parcels.Parcel.CompositionEZ[i].Area.Area, -1,
                                                    KVZU_v06Utils.KVZUState(kv.Parcels.Parcel.CompositionEZ[i].State),

                    KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.Parcel.CompositionEZ[i].CadastralNumber,
                                                                                     kv.Parcels.Parcel.CompositionEZ[i].EntitySpatial));
                    /*
					res.MyBlocks.SpatialData.Add(RRTypes.KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.Parcel.CompositionEZ[i].CadastralNumber,
														   kv.Parcels.Parcel.CompositionEZ[i].EntitySpatial));
														   */
                }
            }
            //Части 
            if (kv.Parcels.Parcel.SubParcels.Count > 0)
            {
                for (int i = 0; i <= kv.Parcels.Parcel.SubParcels.Count - 1; i++)
                {
                    TSlot Sl = MainObj.AddSubParcel(kv.Parcels.Parcel.SubParcels[i].NumberRecord);
                    Sl.AreaGKN = kv.Parcels.Parcel.SubParcels[i].Area.Area.ToString();
                    if (kv.Parcels.Parcel.SubParcels[i].Encumbrance != null)
                        Sl.Encumbrances.Add(KVZU_v06Utils.KVZUEncumtoFteoEncum(kv.Parcels.Parcel.SubParcels[i].Encumbrance));
                    if (kv.Parcels.Parcel.SubParcels[i].EntitySpatial != null)
                    {
                        TPolygon SlEs = RRTypes.KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.Parcel.SubParcels[i].NumberRecord, kv.Parcels.Parcel.SubParcels[i].EntitySpatial);
                        Sl.SpatialElement.ImportPolygon(SlEs);
                        res.District.SpatialData.Add(SlEs);
                    }

                }
            }
            // Кадастровые номера всех земельных участков, образованных из данного земельного участка
            if (kv.Parcels.Parcel.AllOffspringParcel != null)
                foreach (string s in kv.Parcels.Parcel.AllOffspringParcel)
                    MainObj.AllOffspringParcel.Add(s);
            if (kv.Parcels.Parcel.InnerCadastralNumbers != null)
                foreach (string s in kv.Parcels.Parcel.InnerCadastralNumbers)
                    MainObj.InnerCadastralNumbers.Add(s);
            if (kv.Parcels.Parcel.PrevCadastralNumbers != null)
                foreach (string s in kv.Parcels.Parcel.PrevCadastralNumbers)
                    MainObj.PrevCadastralNumbers.Add(s);

            // Сведения об образованных из данного земельного участка
            if (kv.Parcels.OffspringParcel != null)
                for (int i = 0; i <= kv.Parcels.OffspringParcel.Count() - 1; i++)
                {
                    TParcel OffObj = Bl.Parcels.AddParcel(new TParcel(kv.Parcels.OffspringParcel[i].CadastralNumber, i + 1));
                    OffObj.State = "Item05";
                    OffObj.EntSpat.Add(KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.OffspringParcel[i].CadastralNumber,
                                                                                  kv.Parcels.OffspringParcel[i].EntitySpatial));
                }



            //Прикрутим сюды парсинг Прав ветки ЕГРП через XPATH ЕГРН
            MainObj.EGRN = RRTypes.CommonCast.CasterEGRP.ParseEGRNRights(xmldoc); // мдаааа!!!
            res.District.Blocks.Add(Bl);
            res.DocTypeNick = "КВЗУ";
            res.DocType = "Кадастровая выписка о земельном участке";
            res.Version = "06";
            CommonCast.CasterEGRP.Parse_DocumentProperties(xmldoc, res);
            Parse_Contractors(xmldoc, res);


            /*
   label_DocType.Text = "Кадастровая выписка";
   tabPage1.Text = "Земельные участки";
   textBox_DocNum.Text = kv.CertificationDoc.Number;
   textBox_DocDate.Text = kv.CertificationDoc.Date.ToString("dd/MM/yyyy");
   if (kv.CertificationDoc.Official != null)
   {
       textBox_Appointment.Text = kv.CertificationDoc.Official.Appointment;
       textBox_Appointment.Text = kv.CertificationDoc.Official.FamilyName + " " + kv.CertificationDoc.Official.FirstName + " " + kv.CertificationDoc.Official.Patronymic;
   }

   textBox_OrgName.Text = kv.CertificationDoc.Organization;
   */



            return res;
        }

        #endregion

        #region  Разбор KVZU 7.
        /// <summary>
        /// Разбор KVZU 7.
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="xmldoc">файл по схеме urn://x-artefacts-rosreestr-ru/outgoing/kvzu/7.0.1</param>
        /// <returns></returns>
        public netFteo.IO.FileInfo ParseKVZU07(netFteo.IO.FileInfo fi, Stream ms )//  RRTypes.kvzu07.KVZU kv, XmlDocument xmldoc)
        {
            ms.Seek(0, 0);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(ms);
            netFteo.IO.FileInfo res = InitFileInfo(fi, null);

            kvzu07.KVZU kv = (kvzu07.KVZU)Desearialize<kvzu07.KVZU>(ms);

            TCadastralBlock Bl = new TCadastralBlock();


            for (int i = 0; i <= kv.CoordSystems.Count - 1; i++)
            {
                res.District.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));

            }

            TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(kv.Parcels.Parcel.CadastralNumber, kv.Parcels.Parcel.Name.ToString()));

            MainObj.CadastralBlock = kv.Parcels.Parcel.CadastralBlock;
            Bl.CN = kv.Parcels.Parcel.CadastralBlock; // !!! Иначе будет пустой 

            MainObj.SpecialNote = kv.Parcels.Parcel.SpecialNote;
            res.CommentsType = "Особые отметки";
            //res.Comments = kv.Parcels.Parcel.SpecialNote;

            MainObj.Utilization.UtilbyDoc = kv.Parcels.Parcel.Utilization.ByDoc;
            if (kv.Parcels.Parcel.Utilization.UtilizationSpecified)
                MainObj.Utilization.Untilization = kv.Parcels.Parcel.Utilization.Utilization.ToString();
            MainObj.Category = kv.Parcels.Parcel.Category.ToString();// netFteo.Rosreestr.dCategoriesv01.ItemToName(kv.Parcels.Parcel.Category.ToString());

            MainObj.Location = CommonCast.CasterZU.CastLocation(kv.Parcels.Parcel.Location);
            if (kv.Parcels.Parcel.CadastralCost != null)
                MainObj.CadastralCost = kv.Parcels.Parcel.CadastralCost.Value;
            MainObj.Rights = KVZU_v06Utils.KVZURightstoFteorights(kv.Parcels.Parcel.Rights);
            MainObj.Encumbrances = KVZU_v06Utils.KVZUEncumstoFteoEncums(kv.Parcels.Parcel.Encumbrances);
            MainObj.AreaGKN = kv.Parcels.Parcel.Area.Area;
            MainObj.State = kv.Parcels.Parcel.State.ToString();
            MainObj.DateCreated = kv.Parcels.Parcel.DateCreated.ToString("dd.MM.yyyy");
            if (kv.Parcels.Parcel.CadastralCost != null)
                MainObj.CadastralCost = kv.Parcels.Parcel.CadastralCost.Value;
            //Single spatial
            if (kv.Parcels.Parcel.EntitySpatial != null)
                if (kv.Parcels.Parcel.EntitySpatial.SpatialElement.Count > 0)
                {
                    /*
                    TPolygon ents = CasterSpatial.AddEntSpatKVZU07(kv.Parcels.Parcel.CadastralNumber,
                                                           kv.Parcels.Parcel.EntitySpatial);
                    ents.Parent_Id = MainObj.id;
                    MainObj.EntSpat.Add(ents);
                    */

                    //Due happend case, in wich "kv.Parcels.Parcel.EntitySpatial" contains contours
                    //So, parse it as possible multicountoured:
                    MainObj.EntSpat.AddFeatures(CasterSpatial.ParseES_KVZU07(kv.Parcels.Parcel.CadastralNumber,
                                                           kv.Parcels.Parcel.EntitySpatial));
                }

            //MultiContour spatial:
            if (kv.Parcels.Parcel.Contours != null)
            {
                for (int ic = 0; ic <= kv.Parcels.Parcel.Contours.Count - 1; ic++)
                {
                    /* Does not affect, due .SpatialData is readonly property:
                    res.MyBlocks.SpatialData.Add(CommonCast.CasterZU.AddEntSpatKVZU07(kv.Parcels.Parcel.Contours[ic].NumberRecord,
                                                                                 kv.Parcels.Parcel.Contours[ic].EntitySpatial));
                    */
                    TPolygon NewCont = CasterSpatial.AddEntSpatKVZU07(kv.Parcels.Parcel.Contours[ic].NumberRecord,
                                                                                                            kv.Parcels.Parcel.Contours[ic].EntitySpatial);
                    NewCont.AreaValue = kv.Parcels.Parcel.Contours[ic].Area.Area;
                    MainObj.EntSpat.Add(NewCont);
                }
            }
            //ЕЗП:
            if (kv.Parcels.Parcel.CompositionEZ.Count > 0)
            {
                for (int i = 0; i <= kv.Parcels.Parcel.CompositionEZ.Count - 1; i++)
                // if ( kv.Parcels.Parcel.CompositionEZ[i].EntitySpatial != null)
                {
                    MainObj.AddEntry(kv.Parcels.Parcel.CompositionEZ[i].CadastralNumber,
                                                   kv.Parcels.Parcel.CompositionEZ[i].Area.Area, -1,
                                                  KVZU_v06Utils.KVZUState(kv.Parcels.Parcel.CompositionEZ[i].State),
                    CasterSpatial.AddEntSpatKVZU07(kv.Parcels.Parcel.CompositionEZ[i].CadastralNumber,
                                                                                         kv.Parcels.Parcel.CompositionEZ[i].EntitySpatial));
                    /*
					res.MyBlocks.SpatialData.Add(CommonCast.CasterZU.AddEntSpatKVZU07(kv.Parcels.Parcel.CompositionEZ[i].CadastralNumber,
														   kv.Parcels.Parcel.CompositionEZ[i].EntitySpatial));
					*/
                    //MainObj.CompozitionEZ[MainObj.CompozitionEZ.Count - 1]..AreaValue = kv.Parcels.Parcel.CompositionEZ[i].Area.Area;

                }
            }
            //Части 
            if (kv.Parcels.Parcel.SubParcels.Count > 0)
            {
                for (int i = 0; i <= kv.Parcels.Parcel.SubParcels.Count - 1; i++)
                {
                    TSlot Sl = MainObj.AddSubParcel(kv.Parcels.Parcel.SubParcels[i].NumberRecord);
                    Sl.AreaGKN = kv.Parcels.Parcel.SubParcels[i].Area.Area.ToString();
                    if (kv.Parcels.Parcel.SubParcels[i].Encumbrance != null)
                        Sl.Encumbrances.Add(KVZU_v06Utils.KVZUEncumtoFteoEncum(kv.Parcels.Parcel.SubParcels[i].Encumbrance));

                    if (kv.Parcels.Parcel.SubParcels[i].EntitySpatial != null)
                    {
                        TEntitySpatial SlEs = CasterSpatial.ParseES_KVZU07(kv.Parcels.Parcel.SubParcels[i].NumberRecord, kv.Parcels.Parcel.SubParcels[i].EntitySpatial);
                        Sl.ES.AddFeatures(SlEs);

/*
                        foreach (kvzu07.tEntitySpatialZUOutSpatialElement spatEl in kv.Parcels.Parcel.SubParcels[i].EntitySpatial.SpatialElement)
                        {
                            TPolygon SlEs = CommonCast.CasterZU.ParseES_KVZU07(kv.Parcels.Parcel.SubParcels[i].NumberRecord, spatEl);// kv.Parcels.Parcel.SubParcels[i].EntitySpatial);
                            Sl.EntSpat.ImportPolygon(SlEs);
                            res.MyBlocks.SpatialData.Add(SlEs);
                        }
*/
                    }

                }
            }
            // Кадастровые номера всех земельных участков, образованных из данного земельного участка
            if (kv.Parcels.Parcel.AllOffspringParcel != null)
                foreach (string s in kv.Parcels.Parcel.AllOffspringParcel)
                    MainObj.AllOffspringParcel.Add(s);
            //Кадастровые номера зданий, сооружений, объектов незавершенного строительства, расположенных на земельном участке
            if (kv.Parcels.Parcel.InnerCadastralNumbers != null)
                foreach (string s in kv.Parcels.Parcel.InnerCadastralNumbers)
                    MainObj.InnerCadastralNumbers.Add(s);
            //Кадастровые номера земельных участков, из которых образован данный участок
            if (kv.Parcels.Parcel.PrevCadastralNumbers != null)
                foreach (string s in kv.Parcels.Parcel.PrevCadastralNumbers)
                    MainObj.PrevCadastralNumbers.Add(s);


            // Сведения об образованных из данного земельного участка
            if (kv.Parcels.OffspringParcel != null)
                for (int i = 0; i <= kv.Parcels.OffspringParcel.Count() - 1; i++)
                {
                    TParcel OffObj = Bl.Parcels.AddParcel(new TParcel(kv.Parcels.OffspringParcel[i].CadastralNumber, i + 1));
                    OffObj.EntSpat.Add(CasterSpatial.AddEntSpatKVZU07(kv.Parcels.OffspringParcel[i].CadastralNumber,
                                                                                  kv.Parcels.OffspringParcel[i].EntitySpatial));
                    OffObj.State = "Item05";
                }

            //Прикрутим сюды парсинг Прав ветки ЕГРП через XPATH ЕГРН
            MainObj.EGRN = CommonCast.CasterEGRP.ParseEGRNRights(xmldoc); // мдаааа!!!
            res.District.Blocks.Add(Bl);
            res.DocTypeNick = "КВЗУ";
            res.DocType = "Кадастровая выписка о земельном участке";
            res.Version = "07";
            CommonCast.CasterEGRP.Parse_DocumentProperties(xmldoc, res);
            Parse_Contractors(xmldoc, res);
            xmldoc = null;
            return res;
        }
        #endregion

        #region  Разбор KPOKS 03/04.

        public netFteo.IO.FileInfo ParseKPOKS03(netFteo.IO.FileInfo fi, System.Xml.XmlDocument xmldoc)
        {

            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";
            res.DocType = "Кадастровый паспорт";
            res.DocTypeNick = "КПОКС";
            res.Version = "03";
            res.Namespace = NameSpaces.KPOKS_03;

            kpoks_v03.KPOKS kv = (kpoks_v03.KPOKS)Desearialize<kpoks_v03.KPOKS>(xmldoc);
            for (int i = 0; i <= kv.CoordSystems.Count - 1; i++)
            {
                res.District.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));
            }

            /*
            label_DocType.Text = "Кадастровый паспорт";
            tabPage1.Text = "ОКС";
            textBox_DocNum.Text = kv.CertificationDoc.Number;
            textBox_DocDate.Text = kv.CertificationDoc.Date.ToString("dd/MM/yyyy");
            if (kv.CertificationDoc.Official != null)
            {
                textBox_Appointment.Text = kv.CertificationDoc.Official.Appointment;
                textBox_Appointment.Text = kv.CertificationDoc.Official.FamilyName + " " + kv.CertificationDoc.Official.FirstName + " " + kv.CertificationDoc.Official.Patronymic;
            }

            textBox_OrgName.Text = kv.CertificationDoc.Organization;


            for (int i = 0; i <= kv.CoordSystems.Count - 1; i++)
            {
                this.DocInfo.MyBlocks.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));

            }

            for (int i = 0; i <= kv.Contractors.Count - 1; i++)
            {
                ListViewItem LVi = new ListViewItem();
                LVi.Text = kv.Contractors[i].Date.ToString("dd/MM/yyyy");
                LVi.SubItems.Add(kv.Contractors[i].FamilyName + " " + kv.Contractors[i].FirstName + " " + kv.Contractors[i].Patronymic);
                LVi.SubItems.Add(kv.Contractors[i].NCertificate);

                if (kv.Contractors[i].Organization != null)
                    LVi.SubItems.Add(kv.Contractors[i].Organization.Name);
                else LVi.SubItems.Add("-");


                listView_Contractors.Items.Add(LVi);

            }
            */
            if (kv.Realty.Building != null)
            {
                TCadastralBlock Bl = new TCadastralBlock(kv.Realty.Building.CadastralBlocks[0].ToString());
                TRealEstate Bld = new TRealEstate(kv.Realty.Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                Bld.Building.AssignationBuilding = netFteo.Rosreestr.dAssBuildingv01.ItemToName(kv.Realty.Building.AssignationBuilding.ToString());
                Bld.Name = kv.Realty.Building.Name;
                Bld.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(kv.Realty.Building.Address);
                Bld.Area = kv.Realty.Building.Area;
                Bld.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(kv.Realty.Building.CadastralNumber, kv.Realty.Building.EntitySpatial);
                Bld.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Building.ObjectType);
                if (kv.Realty.Building.CadastralNumbersFlats != null)
                    if (kv.Realty.Building.CadastralNumbersFlats.Count() > 0)
                    {
                        foreach (string s in kv.Realty.Building.CadastralNumbersFlats)
                        {
                            TFlat flat = new TFlat(s);
                            Bld.Building.Flats.Add(flat);
                        }
                    }

                Bl.AddOKS(Bld);
                //MifOKSPolygons.AddPolygon((TPolygon) Constructions.ES);
                res.District.Blocks.Add(Bl);
            }



            if (kv.Realty.Construction != null)
            {
                TCadastralBlock Bl = new TCadastralBlock(kv.Realty.Construction.CadastralBlocks[0].ToString());
                TRealEstate Constructions = new TRealEstate(kv.Realty.Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);
                Constructions.Construction.AssignationName = kv.Realty.Construction.AssignationName;
                //Constructions.Address = KPT_v09Utils.AddrKPT09(kv.Realty.Construction.Address);
                Constructions.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(kv.Realty.Construction.CadastralNumber, kv.Realty.Construction.EntitySpatial);
                Constructions.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Construction.ObjectType);
                Bl.AddOKS(Constructions);
                //MifOKSPolygons.AddPolygon((TPolygon) Constructions.ES);
                res.District.Blocks.Add(Bl);
            }

            return res;
        }


        /// <summary>
        /// Разбор KPOKS 04.
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="xmldoc">файл по схеме urn://x-artefacts-rosreestr-ru/outgoing/kpoks/4.0.1</param>
        /// <returns></returns>
        public netFteo.IO.FileInfo ParseKPOKS04(netFteo.IO.FileInfo fi, System.Xml.XmlDocument xmldoc)//RRTypes.kpoks_v04.KPOKS kv, XmlDocument xmldoc)
        {
            if (xmldoc.DocumentElement.NamespaceURI != "urn://x-artefacts-rosreestr-ru/outgoing/kpoks/4.0.1") return null;
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";
            res.DocType = "Кадастровый паспорт";
            res.DocTypeNick = "КПОКС";
            res.Version = "04";


            RRTypes.kpoks_v04.KPOKS kv = (RRTypes.kpoks_v04.KPOKS)Desearialize<RRTypes.kpoks_v04.KPOKS>(xmldoc);
            System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
            nsmgr.AddNamespace("kpoks401", xmldoc.DocumentElement.NamespaceURI);

            System.Xml.XmlNode recnode = xmldoc.DocumentElement.SelectSingleNode("/kpoks401:KPOKS/kpoks401:ReestrExtract/kpoks401:DeclarAttribute/kpoks401:ReceivName", nsmgr);
            /*
            if (recnode != null)
            {
                linkLabel_Recipient.Text = recnode.FirstChild.Value;
            }

            recnode = xmldoc.DocumentElement.SelectSingleNode("/kpoks401:KPOKS/kpoks401:ReestrExtract/kpoks401:DeclarAttribute", nsmgr);
            if (recnode != null)
            {
                linkLabel_Request.Text = recnode.Attributes.GetNamedItem("RequeryNumber").Value;
            }
            */

            for (int i = 0; i <= kv.CoordSystems.Count - 1; i++)
            {
                res.District.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));
            }

            if (kv.Realty.Building != null)
            {
                TCadastralBlock Bl = new TCadastralBlock(kv.Realty.Building.CadastralBlocks[0].ToString());
                TRealEstate Bld = new TRealEstate(kv.Realty.Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                Bld.DateCreated = kv.Realty.Building.DateCreated.ToString("dd.MM.yyyy");
                Bld.Building.AssignationBuilding = netFteo.Rosreestr.dAssBuildingv01.ItemToName(kv.Realty.Building.AssignationBuilding.ToString());
                Bld.Name = kv.Realty.Building.Name;
                Bld.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(kv.Realty.Building.Address);
                Bld.Area = kv.Realty.Building.Area;
                Bld.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(kv.Realty.Building.CadastralNumber, kv.Realty.Building.EntitySpatial);
                Bld.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Building.ObjectType);
                Bld.KeyParameters.AddParameter("Этажность", kv.Realty.Building.Floors.Floors);
                Bld.KeyParameters.AddParameter("Подземная этажность", kv.Realty.Building.Floors.UndergroundFloors);
                if (kv.Realty.Building.ExploitationChar != null)
                {
                    Bld.KeyParameters.AddParameter("Год завершения строительства", kv.Realty.Building.ExploitationChar.YearBuilt.ToString());
                    Bld.KeyParameters.AddParameter("Год ввода в эксплуатацию", kv.Realty.Building.ExploitationChar.YearUsed.ToString());
                }
                if (kv.Realty.Building.ParentCadastralNumbers != null)
                    Bld.ParentCadastralNumbers.AddRange(kv.Realty.Building.ParentCadastralNumbers);

                if (kv.Realty.Building.Notes != null)
                {
                    Bld.Notes = kv.Realty.Building.Notes;
                    res.CommentsType = "Особые отметки";
                    //res.Comments=Bld.Notes;
                }
                if (kv.Realty.Building.CadastralNumbersFlats != null)
                    if (kv.Realty.Building.CadastralNumbersFlats.Count() > 0)
                    {
                        foreach (string s in kv.Realty.Building.CadastralNumbersFlats)
                        {
                            TFlat flat = new TFlat(s);
                            //                             flat.PositionInObject.Levels.Add(new TLevel("","",flat.PositionInObject.Levels[0].Position.NumberOnPlan));
                            Bld.Building.Flats.Add(flat);
                        }
                    }

                Bl.AddOKS(Bld);
                res.District.Blocks.Add(Bl);

            }

            if (kv.Realty.Flat != null)
            {
                TCadastralBlock Bl = new TCadastralBlock(kv.Realty.Flat.CadastralBlock);
                TRealEstate flatObject = new TRealEstate(kv.Realty.Flat.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Помещение);
                if (kv.Realty.Flat.ParentCadastralNumbers.CadastralNumberOKS != null)
                    flatObject.ParentCadastralNumbers.Add(kv.Realty.Flat.ParentCadastralNumbers.CadastralNumberOKS);
                if (kv.Realty.Flat.ParentCadastralNumbers.CadastralNumberFlat != null)
                    flatObject.ParentCadastralNumbers.Add(kv.Realty.Flat.ParentCadastralNumbers.CadastralNumberFlat);
                flatObject.DateCreated = kv.Realty.Flat.DateCreated.ToString("dd.MM.yyyy");
                flatObject.Flat.AssignationCode = netFteo.Rosreestr.dAssFlatv01.ItemToName(kv.Realty.Flat.Assignation.AssignationCode.ToString());
                flatObject.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(kv.Realty.Flat.Address);
                flatObject.Flat.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(kv.Realty.Flat.Address);
                flatObject.Area = kv.Realty.Flat.Area;
                flatObject.Flat.Area = kv.Realty.Flat.Area;

                // ES для Flat нету совсеем в КПОКС 4.0.1 !!!!
                //Bld.ES = RRTypes.kvoks_v02.Utils.AddEntSpatKPOKS04(kv.Realty.Flat.CadastralNumber, kv.Realty.Flat.EntitySpatial);

                if (kv.Realty.Flat.PositionInObject.Position != null)
                    flatObject.Flat.PositionInObject.Levels.Add(new TLevel("", "", kv.Realty.Flat.PositionInObject.Position.NumberOnPlan));

                if (kv.Realty.Flat.PositionInObject.Levels != null)
                    foreach (RRTypes.kpoks_v04.tLevelsOutLevel level in kv.Realty.Flat.PositionInObject.Levels)
                    {
                        flatObject.Flat.PositionInObject.Levels.Add(new TLevel(level.Type.ToString(),
                            level.Number,
                            level.Position.NumberOnPlan));

                    }

                flatObject.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Flat.ObjectType);
                flatObject.Rights = RRTypes.CommonCast.CasterEGRP.ParseKPSOKSRights(xmldoc);
                flatObject.EGRN = RRTypes.CommonCast.CasterEGRP.ParseEGRNRights(xmldoc);
                flatObject.Notes = kv.Realty.Flat.Notes;
                Bl.AddOKS(flatObject);

                res.CommentsType = "Особые отметки";
                //res.Comments = flat.Notes;
                res.District.Blocks.Add(Bl);
            }

            if (kv.Realty.Construction != null)
            {
                TCadastralBlock Bl = new TCadastralBlock(kv.Realty.Construction.CadastralBlocks[0].ToString());
                TRealEstate Constructions = new TRealEstate(kv.Realty.Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);
                Constructions.DateCreated = kv.Realty.Construction.DateCreated.ToString("dd.MM.yyyy");
                Constructions.Construction.AssignationName = kv.Realty.Construction.AssignationName;
                Constructions.Name = kv.Realty.Construction.Name;
                Constructions.ObjectType = CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Construction.ObjectType);
                Constructions.Location.Address = CommonCast.CasterOKS.CastAddress(kv.Realty.Construction.Address);
                Constructions.EntSpat = CommonCast.CasterOKS.ES_OKS2(kv.Realty.Construction.CadastralNumber, kv.Realty.Construction.EntitySpatial);
                res.District.SpatialData.AddRange(Constructions.EntSpat);
                res.CommentsType = "Особые отметки";
                res.Comments = Constructions.Notes;
                Bl.AddOKS(Constructions);
                res.District.Blocks.Add(Bl);
            }
            /*
                        res.Number = kv.CertificationDoc.Number;
                        res.Date = kv.CertificationDoc.Date.ToString("dd.MM.yyyy");
                        res.Cert_Doc_Organization = kv.CertificationDoc.Organization;

                        if (kv.CertificationDoc.Official != null)
                        {
                            res.Appointment = RRTypes.CommonCast.CasterEGRP.Parse_SenderAppointment(xmldoc); // мдаааа!!! XPATH !
                            res.AppointmentFIO = kv.CertificationDoc.Official.FamilyName + " " + kv.CertificationDoc.Official.FirstName + " " + kv.CertificationDoc.Official.Patronymic;
                        }
            */


            CommonCast.CasterEGRP.Parse_DocumentProperties(xmldoc, res);
            Parse_Contractors(xmldoc, res);
            return res;
        }
        #endregion

        #region Parse KVOKS V02
        public netFteo.IO.FileInfo ParseKVOKS02(netFteo.IO.FileInfo fi, XmlDocument xmldoc)//RRTypes.kvoks_v02.KVOKS kv)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);

            kvoks_v02.KVOKS kv = (kvoks_v02.KVOKS)Desearialize<kvoks_v02.KVOKS>(xmldoc);

            for (int i = 0; i <= kv.CoordSystems.Count - 1; i++)
            {
                res.District.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));

            }
            //TMyCadastralBlock Bl = new TMyCadastralBlock();

            /* 
            //TODO: need review before delete:
            label_DocType.Text = "Кадастровая выписка";
            tabPage1.Text = "ОКС";

            textBox_DocNum.Text = kv.CertificationDoc.Number;
            textBox_DocDate.Text = kv.CertificationDoc.Date.ToString("dd/MM/yyyy");
            if (kv.CertificationDoc.Official != null)
            {
                textBox_Appointment.Text = kv.CertificationDoc.Official.Appointment;
                textBox_Appointment.Text = kv.CertificationDoc.Official.FamilyName + " " + kv.CertificationDoc.Official.FirstName + " " + kv.CertificationDoc.Official.Patronymic;
            }

            textBox_OrgName.Text = kv.CertificationDoc.Organization;


            for (int i = 0; i <= kv.CoordSystems.Count - 1; i++)
            {
                this.DocInfo.MyBlocks.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));

            }

            for (int i = 0; i <= kv.Contractors.Count - 1; i++)
            {
                ListViewItem LVi = new ListViewItem();
                LVi.Text = kv.Contractors[i].Date.ToString("dd/MM/yyyy");
                LVi.SubItems.Add(kv.Contractors[i].FamilyName + " " + kv.Contractors[i].FirstName + " " + kv.Contractors[i].Patronymic);
                LVi.SubItems.Add(kv.Contractors[i].NCertificate);

                if (kv.Contractors[i].Organization != null)
                    LVi.SubItems.Add(kv.Contractors[i].Organization.Name);
                else LVi.SubItems.Add("-");


                listView_Contractors.Items.Add(LVi);

            }
            */
            if (kv.Realty.Building != null)
            {
                TCadastralBlock Bl = new TCadastralBlock(kv.Realty.Building.CadastralBlocks[0].ToString());
                TRealEstate Bld = new TRealEstate(kv.Realty.Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                Bld.Building.AssignationBuilding = kv.Realty.Building.AssignationBuilding.ToString();
                Bld.Name = kv.Realty.Building.Name;
                //Constructions.Address = KPT_v09Utils.AddrKPT09(kv.Realty.Construction.Address);
                Bld.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(kv.Realty.Building.CadastralNumber, kv.Realty.Building.EntitySpatial);
                Bl.AddOKS(Bld);
                //MifOKSPolygons.AddPolygon((TPolygon) Constructions.ES);
                res.District.Blocks.Add(Bl);
            }



            if (kv.Realty.Construction != null)
            {
                TCadastralBlock Bl = new TCadastralBlock(kv.Realty.Construction.CadastralBlocks[0].ToString());
                TRealEstate Constructions = new TRealEstate(kv.Realty.Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);

                Constructions.Construction.AssignationName = kv.Realty.Construction.AssignationName;
                Constructions.Name = kv.Realty.Construction.Name;
                Constructions.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(kv.Realty.Construction.CadastralNumber, kv.Realty.Construction.EntitySpatial);
                foreach (kvoks_v02.tOldNumber n in kv.Realty.Construction.OldNumbers)
                    Constructions.Construction.OldNumbers.Add(new TKeyParameter() { Type = n.Type.ToString(), Value = n.Number });
                Bl.AddOKS(Constructions);
                res.District.Blocks.Add(Bl);
            }

            res.DocTypeNick = "КВОКС";
            CommonCast.CasterEGRP.Parse_DocumentProperties(xmldoc, res);
            Parse_Contractors(xmldoc, res);
            return res;
        }
        #endregion

        #region  Разбор KVOKS 3.0.1. (как бы ЕГРН V07)
        // С Наступающим 2018! Где взять KVOKS 3.0.1  - ОКС v07 (KVOKS 3.0.1)????
        /// <summary>
        /// Разбор KVOKS 3.0.1. (как бы ЕГРН V07)
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="xmldoc">файл по схеме urn://x-artefacts-rosreestr-ru/outgoing/kvoks/3.0.1</param>
        /// <returns></returns>
        public netFteo.IO.FileInfo ParseKVOKS07(netFteo.IO.FileInfo fi, System.Xml.XmlDocument xmldoc)// RRTypes.kvoks_v07.KVOKS kv, XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";

            RRTypes.kvoks_v07.KVOKS kv = (RRTypes.kvoks_v07.KVOKS)Desearialize<RRTypes.kvoks_v07.KVOKS>(xmldoc);

            for (int i = 0; i <= kv.CoordSystems.Count - 1; i++)
            {
                res.District.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));

            }

            if (kv.Realty.Building != null)
            {
                TCadastralBlock Bl = new TCadastralBlock(kv.Realty.Building.CadastralBlocks[0].ToString());
                TRealEstate Bld = new TRealEstate(kv.Realty.Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                Bld.DateCreated = (kv.Realty.Building.DateCreatedSpecified) ? kv.Realty.Building.DateCreated.ToString().Replace("0:00:00", "") : "";
                if (kv.Realty.Building.ParentCadastralNumbers != null)
                    Bld.ParentCadastralNumbers.AddRange(kv.Realty.Building.ParentCadastralNumbers);
                Bld.Building.AssignationBuilding = kv.Realty.Building.AssignationBuilding.ToString();
                Bld.Name = kv.Realty.Building.Name;
                if (kv.Realty.Building.ElementsConstruct != null)
                    Bld.ElementsConstruct.Add(new TElementConstruct(kv.Realty.Building.ElementsConstruct[0].Wall.ToString()));
                Bld.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(kv.Realty.Building.Address);
                Bld.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(kv.Realty.Building.CadastralNumber, kv.Realty.Building.EntitySpatial);
                Bld.Building.Area = kv.Realty.Building.Area;
                Bld.Rights = RRTypes.CommonCast.CasterEGRP.ParseEGRNRights(xmldoc);
                if (kv.Realty.Building.Floors != null)
                {
                    Bld.Floors = kv.Realty.Building.Floors.Floors;
                    Bld.UndergroundFloors = kv.Realty.Building.Floors.UndergroundFloors;
                }
                Bld.Notes = kv.Realty.Building.Notes;

                foreach (RRTypes.kvoks_v07.tOldNumber n in kv.Realty.Building.OldNumbers)
                    Bld.Building.OldNumbers.Add(new TKeyParameter() { Type = netFteo.Rosreestr.dOldNumber_v01.ItemToName(n.Type.ToString()), Value = n.Number });

                res.CommentsType = "Особые отметки";
                res.Comments = Bld.Notes;
                if (kv.Realty.Building.Flats.Count > 0)
                {
                    for (int i = 0; i <= kv.Realty.Building.Flats.Count - 1; i++)
                    {
                        TFlat Flat = new TFlat(kv.Realty.Building.Flats[i].CadastralNumber);
                        if (kv.Realty.Building.Flats[i].PositionInObject.Levels.Count > 1)
                        {
                            int levs = kv.Realty.Building.Flats[i].PositionInObject.Levels.Count;
                        }
                        foreach (RRTypes.kvoks_v07.tLevelsOutLevel level in kv.Realty.Building.Flats[i].PositionInObject.Levels)
                        {
                            TLevel lvl = new TLevel(
                                level.Type.ToString(),
                                level.Number,
                                level.Position.NumberOnPlan);

                            foreach (RRTypes.kvoks_v07.tPlan jpegname in level.Position.Plans)
                                lvl.AddPlan(jpegname.Name);
                            Flat.PositionInObject.Levels.Add(lvl);
                        }
                        Flat.Area = kv.Realty.Building.Flats[i].Area;
                        Bld.Building.Flats.Add(Flat);
                    }
                }
                Bl.AddOKS(Bld);
                res.District.Blocks.Add(Bl);
            }

            //Uncompleted
            if (kv.Realty.Uncompleted != null)
            {
                TCadastralBlock Bl = new TCadastralBlock(kv.Realty.Uncompleted.CadastralBlocks[0].ToString());
                TRealEstate Unc = new TRealEstate(kv.Realty.Uncompleted.CadastralNumber, dRealty_v03.Объект_незавершённого_строительства);
                Unc.DateCreated = (kv.Realty.Uncompleted.DateCreatedSpecified) ? kv.Realty.Uncompleted.DateCreated.ToString().Replace("0:00:00", "") : "";
                if (kv.Realty.Uncompleted.ParentCadastralNumbers != null)
                    Unc.ParentCadastralNumbers.AddRange(kv.Realty.Uncompleted.ParentCadastralNumbers);
                Unc.Uncompleted.AssignationName = kv.Realty.Uncompleted.AssignationName;
                Unc.Location.Address = CommonCast.CasterOKS.CastAddress(kv.Realty.Uncompleted.Address);
                Unc.EntSpat = CommonCast.CasterOKS.ES_OKS2(kv.Realty.Uncompleted.CadastralNumber, kv.Realty.Uncompleted.EntitySpatial);
                Unc.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Uncompleted.ObjectType);
                Unc.Uncompleted.DegreeReadiness = kv.Realty.Uncompleted.DegreeReadiness.ToString();
                foreach (kvoks_v07.tKeyParameter param in kv.Realty.Uncompleted.KeyParameters)
                    Unc.KeyParameters.AddParameter(param.Type.ToString(), param.Value.ToString());

                Unc.Rights = CommonCast.CasterEGRP.ParseEGRNRights(xmldoc);
                Unc.Notes = kv.Realty.Uncompleted.Notes;

                foreach (kvoks_v07.tOldNumber n in kv.Realty.Uncompleted.OldNumbers)
                    Unc.Uncompleted.OldNumbers.Add(new TKeyParameter() { Type = dOldNumber_v01.ItemToName(n.Type.ToString()), Value = n.Number });

                res.CommentsType = " Особые отметки";
                res.Comments = Unc.Notes;
                Bl.AddOKS(Unc);
                res.District.Blocks.Add(Bl);
            }

            if (kv.Realty.Construction != null)
            {
                TCadastralBlock Bl = new TCadastralBlock(kv.Realty.Construction.CadastralBlocks[0].ToString());
                TRealEstate Constructions = new TRealEstate(kv.Realty.Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);
                Constructions.DateCreated = (kv.Realty.Construction.DateCreatedSpecified) ? kv.Realty.Construction.DateCreated.ToString("dd.MM.yyyy").Replace("0:00:00", "") : "";
                Constructions.Construction.AssignationName = kv.Realty.Construction.AssignationName;
                Constructions.Name = kv.Realty.Construction.Name;
                Constructions.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Construction.ObjectType);
                Constructions.ParentCadastralNumbers.AddRange(kv.Realty.Construction.ParentCadastralNumbers);
                Constructions.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(kv.Realty.Construction.Address);
                Constructions.EntSpat = CommonCast.CasterOKS.ES_OKS2(kv.Realty.Construction.CadastralNumber, kv.Realty.Construction.EntitySpatial);
                if (kv.Realty.Construction.Floors != null)
                    if (kv.Realty.Construction.Floors != null)
                    {
                        Constructions.Floors = kv.Realty.Construction.Floors.Floors;
                        Constructions.UndergroundFloors = kv.Realty.Construction.Floors.UndergroundFloors;
                    }
                foreach (kvoks_v07.tKeyParameter param in kv.Realty.Construction.KeyParameters)
                    Constructions.KeyParameters.AddParameter(param.Type.ToString(), param.Value.ToString());
                foreach (kvoks_v07.tOldNumber n in kv.Realty.Construction.OldNumbers)
                    Constructions.Construction.OldNumbers.Add(new TKeyParameter() { Type = netFteo.Rosreestr.dOldNumber_v01.ItemToName(n.Type.ToString()), Value = n.Number });
                Bl.AddOKS(Constructions);
                Constructions.Notes = kv.Realty.Construction.Notes;
                res.CommentsType = "Особые отметки";
                res.Comments = Constructions.Notes;
                Constructions.Rights = RRTypes.CommonCast.CasterEGRP.ParseEGRNRights(xmldoc);

                res.District.Blocks.Add(Bl);
            }



            res.DocTypeNick = "КВОКС";
            CommonCast.CasterEGRP.Parse_DocumentProperties(xmldoc, res);
            Parse_Contractors(xmldoc, res);
            return res;
        }
        #endregion

        #region  Разбор ЕГРП 06 через Xpath
        /// <summary>
        /// Разбор ЕГРП 06 через Xpath
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="xmldoc">файл по схеме V04_EXTRACT_FULL </param>
        /// <returns></returns>
        public netFteo.IO.FileInfo ParseEGRP(netFteo.IO.FileInfo fi, System.Xml.XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";
            res.DocType = "ЕГРП";
            res.DocTypeNick = "ЕГРП";
            // /Extract/eDocument/@Version
            res.Version = netFteo.XML.XMLWrapper.Parse_Attribute(xmldoc, "Version", "eDocument");
            if (res.Version.Equals("06"))
                res.Namespace = NameSpaces.EGRP_06;
            if (res.Version.Equals("04"))
                res.Namespace = NameSpaces.EGRP_04;


            TCadastralBlock Bl = new TCadastralBlock();
            System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
            nsmgr.AddNamespace("egrp", xmldoc.DocumentElement.NamespaceURI);
            res.DocType = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:ReestrExtract/egrp:DeclarAttribute/@egrp:ExtractTypeText", nsmgr).Value.ToString();
            res.Number = netFteo.XML.XMLWrapper.Parse_Attribute(xmldoc, "ExtractNumber", "ReestrExtract/DeclarAttribute"); //xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:ReestrExtract/egrp:DeclarAttribute/@egrp:ExtractNumber", nsmgr).Value.ToString();
            res.Date = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:ReestrExtract/egrp:DeclarAttribute/@egrp:ExtractDate", nsmgr).Value.ToString();
            res.RequeryNumber = netFteo.XML.XMLWrapper.Parse_Attribute(xmldoc, "RequeryNumber", "ReestrExtract/DeclarAttribute");
            res.ReceivName = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "ReestrExtract/DeclarAttribute/ReceivName");
            res.ReceivAdress = netFteo.XML.XMLWrapper.Parse_NodeValue(xmldoc, "ReestrExtract/DeclarAttribute/ReceivAdress");



            if (xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:eDocument/egrp:Sender/@egrp:Name", nsmgr) != null)
            {
                res.Cert_Doc_Organization = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:eDocument/egrp:Sender/@egrp:Name", nsmgr).Value.ToString();
                res.Appointment = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:eDocument/egrp:Sender/@egrp:Appointment", nsmgr).Value.ToString();

                res.AppointmentFIO = netFteo.XML.XMLWrapper.Parse_Attribute(xmldoc, "FIO", "eDocument/Sender");// xmldoc.DocumentElement.SelectSingleNode(" / egrp:Extract/egrp:eDocument/egrp:Sender/@egrp:FIO", nsmgr).Value.ToString();
            }



            System.Xml.XmlNode ParcelNode = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:ReestrExtract/egrp:ExtractObjectRight/egrp:ExtractObject", nsmgr);///
			if (ParcelNode != null)
            {
                if (ParcelNode.SelectSingleNode("egrp:ObjectRight/egrp:ObjectDesc/egrp:ObjectTypeText", nsmgr).FirstChild.Value.ToString() == "Земельный участок")
                {
                    TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(ParcelNode.SelectSingleNode("egrp:ObjectRight/egrp:ObjectDesc/egrp:CadastralNumber", nsmgr).FirstChild.Value.ToString(),
                                                                           "Item01"));

                    Bl.CN = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:eDocument/@egrp:Scope", nsmgr).Value.ToString();
                    MainObj.Location.Address = new netFteo.Rosreestr.TAddress();
                    MainObj.Location.Address.Note = ParcelNode.SelectSingleNode("egrp:ObjectRight/egrp:ObjectDesc/egrp:Address/egrp:Content", nsmgr).FirstChild.Value.ToString();
                    if (ParcelNode.SelectSingleNode("egrp:ObjectRight/egrp:ObjectDesc/egrp:GroundCategoryText", nsmgr) != null)
                        MainObj.Utilization.UtilbyDoc = ParcelNode.SelectSingleNode("egrp:ObjectRight/egrp:ObjectDesc/egrp:GroundCategoryText", nsmgr).FirstChild.Value.ToString();
                    MainObj.AreaGKN = ParcelNode.SelectSingleNode("egrp:ObjectRight/egrp:ObjectDesc/egrp:Area/egrp:AreaText", nsmgr).FirstChild.Value.ToString();
                    //Прикрутим сюды парсинг через XPATH ЕГРН
                    MainObj.EGRN = EGRP_v60Utils.ParseEGRRights(xmldoc); // мдаааа!!!
                }
            }
            res.District.Blocks.Add(Bl);
            return res;
        }
        #endregion

        #region  Разбор DXF
        /// <summary>
        /// Разбор dxf
        /// </summary>
        /// 
        /// <returns></returns>
        public netFteo.IO.FileInfo ParseDXF(netFteo.IO.FileInfo fi, netFteo.IO.DXFReader dxfreader) //RRTypes.kpzu06.KPZU kp, XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, null);
            TEntitySpatial DXfEntitys = dxfreader.ParseDXF();


            if (DXfEntitys != null)
            {
                // TODO where place for results ??? : 
                res.District.ParsedSpatial = DXfEntitys;
            }

            res.DocTypeNick = "dxf";
            res.CommentsType = "DXF";
            res.Comments = "AttributeDefinition:\r";
            foreach (netDxf.Entities.AttributeDefinition attr in dxfreader.dxfFile.AttributeDefinition)
                res.Comments += attr.Tag + " = " + attr.Value + " (Text=" + attr.Text + ")\r";
            res.Comments += "\r LoadExceptions: \r";
            foreach (string s in dxfreader.dxfFile.LoadExceptions)
                res.Comments += s + "\r";
            res.Comments += "\r" + dxfreader.Body;
            res.Encoding = dxfreader.BodyEncoding.ToString();
            res.Number = "Encoding  " + dxfreader.BodyEncoding;
            res.DocType = "dxf";
            res.Version = dxfreader.Version;
            return res;
        }

        #endregion

        #region  Разбор MIF
        /// <summary>
        /// Разбор mif
        /// </summary>
        /// 
        /// <returns></returns>
        public netFteo.IO.FileInfo ParseMIF(netFteo.IO.FileInfo fi, netFteo.IO.MIFReader mifreader) //RRTypes.kpzu06.KPZU kp, XmlDocument xmldoc)
        {
            netFteo.IO.FileInfo res = InitFileInfo(fi, null);
            TEntitySpatial Entitys = mifreader.ParseMIF();


            if (Entitys != null)
            {
                // TODO where place for results ??? : 
                res.District.ParsedSpatial = Entitys; // not Add, need assume to update Layers
            }

            res.DocTypeNick = "Mapinfo mif";
            res.CommentsType = "MIF";
            res.Comments = mifreader.Body;//.GetType().ToString() + " file info \r Blocked LWPOLYLINE.Count = " + mifreader.PolygonsCount().ToString() + " \rFileBody:\r" + mifreader.Body;
            res.Encoding = mifreader.BodyEncoding.ToString();
            res.Number = "Encoding  " + mifreader.BodyEncoding;
            res.DocType = "Mapinfo mif";
            //res.Version = mifreader.Version;
            return res;

        }
        #endregion

        #region Common Parsing func
        //HEre ....

        #endregion

    }

    public static class ParserCommon
    {
        /// <summary>
        /// XSD enumerations. Must be set before parsing
        /// </summary>
        public static netFteo.XML.XSDFile dutilizations_v01;
        /// <summary>
        /// XSD enumerations. Must be set before parsing
        /// </summary>
        public static netFteo.XML.XSDFile dAllowedUse_v02;

        /*      TODO: body for rewriting: ParseKVOKS 
                 private void ParseKVOKS(RRTypes.kvoks_v02.KVOKS kv)
                {

                    label_DocType.Text = "Кадастровая выписка";
                    tabPage1.Text = "ОКС";
                    textBox_DocNum.Text = kv.CertificationDoc.Number;
                    textBox_DocDate.Text = kv.CertificationDoc.Date.ToString("dd/MM/yyyy");
                    if (kv.CertificationDoc.Official != null)
                    {
                        textBox_Appointment.Text = kv.CertificationDoc.Official.Appointment;
                        textBox_Appointment.Text = kv.CertificationDoc.Official.FamilyName + " " + kv.CertificationDoc.Official.FirstName + " " + kv.CertificationDoc.Official.Patronymic;
                    }

                    textBox_OrgName.Text = kv.CertificationDoc.Organization;


                    for (int i = 0; i <= kv.CoordSystems.Count - 1; i++)
                    {
                        this.DocInfo.MyBlocks.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));

                    }

                    for (int i = 0; i <= kv.Contractors.Count - 1; i++)
                    {
                        ListViewItem LVi = new ListViewItem();
                        LVi.Text = kv.Contractors[i].Date.ToString("dd/MM/yyyy");
                        LVi.SubItems.Add(kv.Contractors[i].FamilyName + " " + kv.Contractors[i].FirstName + " " + kv.Contractors[i].Patronymic);
                        LVi.SubItems.Add(kv.Contractors[i].NCertificate);

                        if (kv.Contractors[i].Organization != null)
                            LVi.SubItems.Add(kv.Contractors[i].Organization.Name);
                        else LVi.SubItems.Add("-");


                        listView_Contractors.Items.Add(LVi);

                    }

                    if (kv.Realty.Building != null)
                    {
                        TMyCadastralBlock Bl = new TMyCadastralBlock(kv.Realty.Building.CadastralBlocks[0].ToString());
                        TMyRealty Bld = new TMyRealty(kv.Realty.Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                        Bld.Building.AssignationBuilding = kv.Realty.Building.AssignationBuilding.ToString();
                        Bld.Name = kv.Realty.Building.Name;
                        //Constructions.Address = KPT_v09Utils.AddrKPT09(kv.Realty.Construction.Address);
                        Bld.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(kv.Realty.Building.CadastralNumber, kv.Realty.Building.EntitySpatial);
                        Bl.AddOKS(Bld);
                        //MifOKSPolygons.AddPolygon((TPolygon) Constructions.ES);
                        this.DocInfo.MyBlocks.Blocks.Add(Bl);
                    }



                    if (kv.Realty.Construction != null)
                    {
                        TMyCadastralBlock Bl = new TMyCadastralBlock(kv.Realty.Construction.CadastralBlocks[0].ToString());
                        TMyRealty Constructions = new TMyRealty(kv.Realty.Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);

                        Constructions.Construction.AssignationName = kv.Realty.Construction.AssignationName;
                        Constructions.Name = kv.Realty.Construction.Name;
                        Constructions.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(kv.Realty.Construction.CadastralNumber, kv.Realty.Construction.EntitySpatial);
                        foreach (RRTypes.kvoks_v02.tOldNumber n in kv.Realty.Construction.OldNumbers)
                            Constructions.Construction.OldNumbers.Add(new TKeyParameter() { Type = n.Type.ToString(), Value = n.Number });
                        Bl.AddOKS(Constructions);
                        this.DocInfo.MyBlocks.Blocks.Add(Bl);
                    }

                    ListMyCoolections(this.DocInfo.MyBlocks);
                }

        */

        /*    TODO: body for rewriting: ParseKPOKS
   private void ParseKPOKS(RRTypes.kpoks_v03.KPOKS kv)
    {

        label_DocType.Text = "Кадастровый паспорт";
        tabPage1.Text = "ОКС";
        textBox_DocNum.Text = kv.CertificationDoc.Number;
        textBox_DocDate.Text = kv.CertificationDoc.Date.ToString("dd/MM/yyyy");
        if (kv.CertificationDoc.Official != null)
        {
            textBox_Appointment.Text = kv.CertificationDoc.Official.Appointment;
            textBox_Appointment.Text = kv.CertificationDoc.Official.FamilyName + " " + kv.CertificationDoc.Official.FirstName + " " + kv.CertificationDoc.Official.Patronymic;
        }

        textBox_OrgName.Text = kv.CertificationDoc.Organization;


        for (int i = 0; i <= kv.CoordSystems.Count - 1; i++)
        {
            this.DocInfo.MyBlocks.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));

        }

        for (int i = 0; i <= kv.Contractors.Count - 1; i++)
        {
            ListViewItem LVi = new ListViewItem();
            LVi.Text = kv.Contractors[i].Date.ToString("dd/MM/yyyy");
            LVi.SubItems.Add(kv.Contractors[i].FamilyName + " " + kv.Contractors[i].FirstName + " " + kv.Contractors[i].Patronymic);
            LVi.SubItems.Add(kv.Contractors[i].NCertificate);

            if (kv.Contractors[i].Organization != null)
                LVi.SubItems.Add(kv.Contractors[i].Organization.Name);
            else LVi.SubItems.Add("-");


            listView_Contractors.Items.Add(LVi);

        }

        if (kv.Realty.Building != null)
        {
            TMyCadastralBlock Bl = new TMyCadastralBlock(kv.Realty.Building.CadastralBlocks[0].ToString());
            TMyRealty Bld = new TMyRealty(kv.Realty.Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
            Bld.Building.AssignationBuilding = netFteo.Rosreestr.dAssBuildingv01.ItemToName(kv.Realty.Building.AssignationBuilding.ToString());
            Bld.Name = kv.Realty.Building.Name;
            Bld.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(kv.Realty.Building.Address);
            Bld.Area = kv.Realty.Building.Area;
            Bld.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(kv.Realty.Building.CadastralNumber, kv.Realty.Building.EntitySpatial);
            Bld.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Building.ObjectType);
            if (kv.Realty.Building.CadastralNumbersFlats != null)
                if (kv.Realty.Building.CadastralNumbersFlats.Count() > 0)
                {
                    foreach (string s in kv.Realty.Building.CadastralNumbersFlats)
                    {
                        TFlat flat = new TFlat(s);
                        Bld.Building.Flats.Add(flat);
                    }
                }

            Bl.AddOKS(Bld);
            //MifOKSPolygons.AddPolygon((TPolygon) Constructions.ES);
            this.DocInfo.MyBlocks.Blocks.Add(Bl);
        }



        if (kv.Realty.Construction != null)
        {
            TMyCadastralBlock Bl = new TMyCadastralBlock(kv.Realty.Construction.CadastralBlocks[0].ToString());
            TMyRealty Constructions = new TMyRealty(kv.Realty.Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);
            Constructions.Construction.AssignationName = kv.Realty.Construction.AssignationName;
            //Constructions.Address = KPT_v09Utils.AddrKPT09(kv.Realty.Construction.Address);
            Constructions.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(kv.Realty.Construction.CadastralNumber, kv.Realty.Construction.EntitySpatial);
            Constructions.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Construction.ObjectType);
            Bl.AddOKS(Constructions);
            //MifOKSPolygons.AddPolygon((TPolygon) Constructions.ES);
            this.DocInfo.MyBlocks.Blocks.Add(Bl);
        }

        ListMyCoolections(this.DocInfo.MyBlocks);
    }

         */

        /* TODO: body for rewriting: ParseSTDTPV02
   private void ParseSTDTPV02(RRTypes.STD_TPV02.STD_TP TP)
    {
        label_DocType.Text = "Технический план";
        tabPage1.Text = "ОКС";
        if (TP.Construction != null)
        {

            richTextBox1.AppendText(TP.Construction.Conclusion);
            this.DocInfo.MyBlocks.CSs.Add(new TCoordSystem(TP.Construction.Coord_Systems[0].Name, TP.Construction.Coord_Systems[0].Cs_Id));
            ListViewItem LVi = new ListViewItem();
            LVi.Text = TP.Construction.Contractor.Date.ToString();
            LVi.SubItems.Add(TP.Construction.Contractor.Cadastral_Engineer.FIO.Surname + " " + TP.Construction.Contractor.Cadastral_Engineer.FIO.First +
                                 " " + TP.Construction.Contractor.Cadastral_Engineer.FIO.Patronymic);
            LVi.SubItems.Add(TP.Construction.Contractor.Cadastral_Engineer.N_Certificate);
            listView_Contractors.Items.Add(LVi);
            textBox_Appointment.Text = TP.Construction.Contractor.Cadastral_Engineer.FIO.Surname + " " + TP.Construction.Contractor.Cadastral_Engineer.FIO.First +
                             " " + TP.Construction.Contractor.Cadastral_Engineer.FIO.Patronymic + ";  " + TP.Construction.Contractor.Cadastral_Engineer.E_mail;
            textBox_DocDate.Text = TP.Construction.Contractor.Date.ToString();
            if (TP.Construction.Contractor.Cadastral_Organization != null)
                textBox_OrgName.Text = TP.Construction.Contractor.Cadastral_Organization.Name;
            textBox_Appointment.Text = TP.Construction.Contractor.Cadastral_Engineer.N_Certificate;
            textBox_DocNum.Text = TP.GUID;

            if (TP.Construction.Package.New_Construction.Count > 0)
            {
                TMyCadastralBlock Bl = new TMyCadastralBlock();
                TMyRealty Constructions = new TMyRealty(TP.Construction.Package.New_Construction[0].Name, netFteo.Rosreestr.dRealty_v03.Сооружение);
                Constructions.Construction.AssignationName = TP.Construction.Package.New_Construction[0].Assignation_Name;
                Constructions.Location.Address.Note = TP.Construction.Package.New_Construction[0].Location.Note;
                Constructions.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(TP.Construction.Package.New_Construction[0].Assignation_Name,
                                                                                 TP.Construction.Package.New_Construction[0].Entity_Spatial);
                Bl.AddOKS(Constructions);
                this.DocInfo.MyBlocks.SpatialData.AddRange(Constructions.EntSpat);
                this.DocInfo.MyBlocks.Blocks.Add(Bl);
            }

            ListMyCoolections(this.DocInfo.MyBlocks);

        }
    }
         */

        /// <summary>
        /// Parse XMl Document several types
        /// </summary>
        /// <param name="xmldoc">XmlDocument</param>
        /// <returns></returns>
        public static netFteo.IO.FileInfo ParseXMLDocument(Stream xmlStream)
        {
            if (xmlStream == null)
            {
                return null;
            }

            netFteo.IO.FileInfo DocInfo = new netFteo.IO.FileInfo();
            DocInfo.DocRootName = netFteo.XML.XMLWrapper.XMLReader_GetRoot(xmlStream);
            DocInfo.Namespace = netFteo.XML.XMLWrapper.XMLReader_GetNameSpace(xmlStream);
            DocInfo.Version = netFteo.XML.XMLWrapper.XMLReader_GetRootAtrr(xmlStream, "Version"); //Для MP версия в корне
            Doc2Type parser = new Doc2Type(dutilizations_v01, dAllowedUse_v02);

            //DocInfo.DocRootName = xmldoc.DocumentElement.Name;
            //DocInfo.Namespace = xmldoc.DocumentElement.NamespaceURI;  // "urn://x-artefacts-rosreestr-ru/outgoing/kpt/10.0.1"

            //if (xmldoc.DocumentElement.Attributes.GetNamedItem("Version") != null) // Для MP версия в корне
            //  DocInfo.Version = xmldoc.DocumentElement.Attributes.GetNamedItem("Version").Value;



            if (DocInfo.DocRootName == "Region_Cadastr")
            {
                DocInfo.DocTypeNick = "КПТ";
                DocInfo.DocType = "Кадастровый план территории";
                /*
                if ((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument") != null) &&
                          (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version") != null) &&
                            (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version").Value.Equals("05")))
*/
                if (netFteo.XML.XMLWrapper.XMLReader_GetNodeAttr(xmlStream, "eDocument", "Version") == "05")
                {
                    DocInfo = parser.ParseKPT05(DocInfo, xmlStream);
                }

                /*
                if (((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument") != null) &&
                                  (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version") != null) &&
                                    (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version").Value.Equals("06"))))
                    */
                if (netFteo.XML.XMLWrapper.XMLReader_GetNodeAttr(xmlStream, "eDocument", "Version") == "06")
                {
                    DocInfo = parser.ParseKPT06(DocInfo, xmlStream);
                }

                //Не КПТ v07 ли это?   
                /*
                if (((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument") != null) &&
                  (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version") != null) &&
                    (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version").Value.Equals("07"))))
                    */
                if (netFteo.XML.XMLWrapper.XMLReader_GetNodeAttr(xmlStream, "eDocument", "Version") == "07")
                {
                    DocInfo = parser.ParseKPT07(DocInfo, xmlStream);
                }

                //Не КПТ v08 ли это?            
                /*
                if ((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name).Attributes.GetNamedItem("Version") != null) &&
                (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name).Attributes.GetNamedItem("Version").Value.Equals("08")))
                */
                if (netFteo.XML.XMLWrapper.XMLReader_GetRootAtrr(xmlStream, "Version") == "08")
                {
                    //toolStripProgressBar1.Minimum = 0;
                    // toolStripProgressBar1.Value = 0;
                    //parser.OnParsing += XMLStateUpdater; // handlers most linked outside class
                    //parser.OnStartParsing += XMLStartUpdater; //
                    DocInfo = parser.ParseKPT08(DocInfo, xmlStream);
                }
            }


            //Не КПТ v09 ли это?            
            if ((DocInfo.DocRootName == "KPT") && (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kpt/9.0.3"))
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                DocInfo = parser.ParseKPT09(DocInfo, xmlStream);
                return DocInfo;
            }

            //Не КПТ v10 ли это?
            if ((DocInfo.DocRootName == "KPT") && (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kpt/10.0.1"))
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                DocInfo = parser.ParseKPT10(DocInfo, xmlStream);
                return DocInfo; // Need return, due below estimated XMLDocument creation
            }
            
  
            //next deprecated metods for XMLDoc:
            xmlStream.Seek(0, 0);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(xmlStream);

            //Не КПТ v11 ли это?
            if (DocInfo.DocRootName == "extract_cadastral_plan_territory")
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                DocInfo = parser.ParseKPT11(DocInfo, xmldoc);
            }

            
            if (DocInfo.DocRootName == "extract_about_property_land")
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                DocInfo = parser.ParseKV8(DocInfo, xmldoc);
            }


            //extract_base_params_land
            if (DocInfo.DocRootName == "extract_base_params_land")
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                DocInfo = parser.ParseKV8(DocInfo, xmldoc);
            }

            //Если это КВЗУ V04/V05
            if (DocInfo.DocRootName == "Region_Cadastr_Vidimus_KV")
            {
                // toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                /*
                string ver_Vidimus = "";
                if ((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument") != null) &&
                        (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version") != null))
                    ver_Vidimus = xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version").Value;
                */

                if (netFteo.XML.XMLWrapper.XMLReader_GetNodeAttr(xmlStream, "eDocument", "Version") == "04")
                {
                    DocInfo = parser.ParseKVZU04(DocInfo, xmlStream);
                }

                // Проверим
                /*
                if ((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name).Attributes.GetNamedItem("Version") != null) &&
                    (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name).Attributes.GetNamedItem("Version").Value.Equals("05")))
                    */
                    if (netFteo.XML.XMLWrapper.XMLReader_GetRootAtrr(xmlStream,  "Version") == "05")
                    {
                    DocInfo = parser.ParseKVZU05(DocInfo, xmldoc);
                }
            }

            if ((DocInfo.DocRootName == "KVZU") & (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kvzu/6.0.9"))
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                ///tabPage1.Text = "Кадастровая выписка 6";
                DocInfo = parser.ParseKVZU06(DocInfo, xmldoc);
            }


            if ((DocInfo.DocRootName == "KVZU") & (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kvzu/7.0.1"))
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                DocInfo = parser.ParseKVZU07(DocInfo, xmlStream);
            }

            if (DocInfo.DocRootName == "KPZU")
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                if (DocInfo.Namespace != "urn://x-artefacts-rosreestr-ru/outgoing/kpzu/6.0.1")
                {
                    DocInfo = parser.ParseKPZU508(DocInfo, xmlStream);
                }

                // KPZU_V6 01  - ЕГРН
                if (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kpzu/6.0.1")
                {
                    DocInfo = parser.ParseKPZU(DocInfo, xmldoc);
                }
            }

            //Выписка ЕГРП, блядь есть и такое
            if (xmldoc.DocumentElement.Name == "Extract")
            {
                DocInfo = parser.ParseEGRP(DocInfo, xmldoc);

            }


            if (xmldoc.DocumentElement.Name == "Users")
            {
                XmlElement xRoot = xmldoc.DocumentElement;

                // выбор всех дочерних узлов
                //XmlNodeList childnodes = xRoot.SelectNodes("*");
                //Выберем все узлы <user>:
                XmlNodeList childnodes = xRoot.SelectNodes("//user/company");
            }





            if (DocInfo.DocRootName == "tExistEZEntryParcelCollection")
            {
                // toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                //   tabPage1.Text = "tExistEZEntryParcelCollection";
                {
                    //toolStripStatusLabel3.Text = "tExistEZEntryParcelCollection";
                    XmlSerializer serializer = new XmlSerializer(typeof(MP_V06.tExistEZEntryParcelCollection));
                    // MP_V06.tExistEZEntryParcelCollection xmlPolygons = (MP_V06.tExistEZEntryParcelCollection)serializer.Deserialize(xmlStream);
                }
            }

            if (DocInfo.DocRootName == "KVOKS")
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                if (DocInfo.Namespace != "urn://x-artefacts-rosreestr-ru/outgoing/kvoks/3.0.1")
                {
                    //XmlSerializer serializer = new XmlSerializer(typeof(RRTypes.kvoks_v02.KVOKS));
                    //kvoks_v02.KVOKS KVoks02 = (kvoks_v02.KVOKS)serializer.Deserialize(xmlStream);
                    DocInfo = parser.ParseKVOKS02(DocInfo, xmldoc); //NOTE : need test
                }

                //Под этим urn urn://x-artefacts-rosreestr-ru/outgoing/kvoks/3.0.1 как бы выписка версии KVOKS_V07
                if (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kvoks/3.0.1")
                {
                    DocInfo = parser.ParseKVOKS07(DocInfo, xmldoc);
                }
            }


            //extract_base_params_under_construction
            if (DocInfo.DocRootName == "extract_base_params_under_construction")
            {
                DocInfo = parser.ParseKVOKS8(DocInfo, xmldoc);
            }


            if (DocInfo.DocRootName == "KPOKS")
            {
                // KPOKS_V4
                if (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kpoks/4.0.1")
                {
                    DocInfo = parser.ParseKPOKS04(DocInfo, xmldoc);
                }
                else
                    DocInfo = parser.ParseKPOKS03(DocInfo, xmldoc);
            }

            if (DocInfo.DocRootName == "SchemaParcels")
            {
                /* as file SchemaKPTMainForm was corrupted with wrong unicode bytes
                 * we hide them
                 * TODO: check form
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.misc28;
                SchemaKPTMainForm frm = new SchemaKPTMainForm();
                frm.Top = this.Top; frm.Left = this.Left;
                frm.StartPosition = FormStartPosition.Manual;
                frm.OpenXML(xmldoc);
                this.Visible = false;
                frm.ShowDialog();
                this.Close();
                */
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.page_white_csharp;
                DocInfo = parser.ParseSchemaParcels(DocInfo, xmldoc);
            }


            if (DocInfo.DocRootName == "STD_MP")
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                DocInfo = parser.ParseMPV04(DocInfo, xmldoc);
            }


            if ((DocInfo.DocRootName == "MP") && (DocInfo.Version == "05"))
            {

                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                DocInfo = parser.ParseMPV05(DocInfo, xmldoc);
            }

            // Типы MP Версия 06 - без XSD to clasess. напрямую XSD.exe
            if ((DocInfo.DocRootName == "MP") && (DocInfo.Version == "06"))
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.cross;
                DocInfo = parser.ParseMPV06(DocInfo, xmldoc);
            }


            if (DocInfo.DocRootName == "STD_TP")
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                // XmlSerializer serializerTP = new XmlSerializer(typeof(RRTypes.STD_TPV02.STD_TP));
                //STD_TPV02.STD_TP TP = (STD_TPV02.STD_TP)serializerTP.Deserialize(xmlStream);
                //TODO : rewrite ParseSTDTPV02(TP);
                DocInfo = parser.ParseSTDTPV02(DocInfo, xmldoc);
            }

            //TP
            if ((DocInfo.DocRootName == "TP") && (DocInfo.Version == "03"))
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                DocInfo = parser.ParseTP_V03(DocInfo, xmldoc);
            }

            if ((DocInfo.DocRootName == "TP") && (DocInfo.Version == "06"))
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                DocInfo = parser.ParseTP_V06(DocInfo, xmldoc);
            }

        FinalPoint: xmlStream.Dispose();
            GC.Collect(); //start mem garbage 
            return DocInfo;
        }

        /// <summary>
        /// Alternate func below
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <returns></returns>
        public static netFteo.IO.FileInfo ParseXMLDocument(XmlDocument xmldoc)
        {
            if (xmldoc == null)
            {
                return null;
            }
            netFteo.IO.FileInfo DocInfo = new netFteo.IO.FileInfo();
            DocInfo.DocRootName = xmldoc.DocumentElement.Name;
            DocInfo.Namespace = xmldoc.DocumentElement.NamespaceURI;  // "urn://x-artefacts-rosreestr-ru/outgoing/kpt/10.0.1"
            if (xmldoc.DocumentElement.Attributes.GetNamedItem("Version") != null) // Для MP версия в корне
                DocInfo.Version = xmldoc.DocumentElement.Attributes.GetNamedItem("Version").Value;


            Doc2Type parser = new Doc2Type(dutilizations_v01, dAllowedUse_v02);// (dutilizations_v01, dAllowedUse_v02);

            if (DocInfo.DocRootName == "Region_Cadastr")
            {
                DocInfo.DocTypeNick = "КПТ";
                DocInfo.DocType = "Кадастровый план территории";
                if (((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument") != null) &&
                          (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version") != null) &&
                            (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version").Value.Equals("05"))))
                {
                    //DocInfo = parser.ParseKPT05(DocInfo, xmldoc);
                }

                if (((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument") != null) &&
                                  (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version") != null) &&
                                    (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version").Value.Equals("06"))))
                {
                    //DocInfo = parser.ParseKPT06(DocInfo, xmldoc);
                }

                //Не КПТ v07 ли это?   
                if (((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument") != null) &&
                  (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version") != null) &&
                    (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version").Value.Equals("07"))))
                {
                    //DocInfo = parser.ParseKPT07(DocInfo, xmlstream);
                }
                
                
                
                //Не КПТ v08 ли это?            
                if ((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name).Attributes.GetNamedItem("Version") != null) &&
                (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name).Attributes.GetNamedItem("Version").Value.Equals("08")))
                {
                    //toolStripProgressBar1.Minimum = 0;
                    // toolStripProgressBar1.Value = 0;
                    //parser.OnParsing += XMLStateUpdater; // handlers most linked outside class
                    //parser.OnStartParsing += XMLStartUpdater; //
                    DocInfo.Version = "08";
                   // DocInfo = parser.ParseKPT08(DocInfo, xmldoc);
                }
            }

            //Не КПТ v09 ли это?            
            if ((DocInfo.DocRootName == "KPT") && (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kpt/9.0.3"))
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                //DocInfo = parser.ParseKPT09(DocInfo, xmldoc);
            }

            //Не КПТ v10 ли это?
            if ((DocInfo.DocRootName == "KPT") && (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kpt/10.0.1"))
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                //DocInfo = parser.ParseKPT10(DocInfo, xmldoc);
            }


            //Не КПТ v11 ли это?
            if (DocInfo.DocRootName == "extract_cadastral_plan_territory")
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                DocInfo = parser.ParseKPT11(DocInfo, xmldoc);
            }




            //Если это КВЗУ V04/V05
            if (DocInfo.DocRootName == "Region_Cadastr_Vidimus_KV")
            {
                // toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                string ver_Vidimus = "";
                if ((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument") != null) &&
                        (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version") != null))
                    ver_Vidimus = xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version").Value;

                if (ver_Vidimus.Equals("04")) // Проверим
                {
                    //DocInfo = parser.ParseKVZU04(DocInfo, xmldoc);
                }
                // Проверим
                if ((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name).Attributes.GetNamedItem("Version") != null) &&
                    (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name).Attributes.GetNamedItem("Version").Value.Equals("05")))
                {
                    DocInfo = parser.ParseKVZU05(DocInfo, xmldoc);
                }
            }

            if ((DocInfo.DocRootName == "KVZU") & (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kvzu/6.0.9"))
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                ///tabPage1.Text = "Кадастровая выписка 6";
                DocInfo = parser.ParseKVZU06(DocInfo, xmldoc);
            }


            if ((DocInfo.DocRootName == "KVZU") & (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kvzu/7.0.1"))
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                //DocInfo = parser.ParseKVZU07(DocInfo, xmldoc);
            }

            if (DocInfo.DocRootName == "KPZU")
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                if (DocInfo.Namespace != "urn://x-artefacts-rosreestr-ru/outgoing/kpzu/6.0.1")
                {
                  //  DocInfo = parser.ParseKPZU508(DocInfo, xmldoc);
                }

                // KPZU_V6 01  - ЕГРН
                if (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kpzu/6.0.1")
                {
                    DocInfo = parser.ParseKPZU(DocInfo, xmldoc);
                }
            }

            //Выписка ЕГРП, блядь есть и такое
            if (xmldoc.DocumentElement.Name == "Extract")
            {
                DocInfo = parser.ParseEGRP(DocInfo, xmldoc);

            }


            if (xmldoc.DocumentElement.Name == "Users")
            {
                XmlElement xRoot = xmldoc.DocumentElement;

                // выбор всех дочерних узлов
                //XmlNodeList childnodes = xRoot.SelectNodes("*");
                //Выберем все узлы <user>:
                XmlNodeList childnodes = xRoot.SelectNodes("//user/company");
            }

            if (DocInfo.DocRootName == "tExistEZEntryParcelCollection")
            {
                // toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                //   tabPage1.Text = "tExistEZEntryParcelCollection";
                {
                    //toolStripStatusLabel3.Text = "tExistEZEntryParcelCollection";
                    XmlSerializer serializer = new XmlSerializer(typeof(MP_V06.tExistEZEntryParcelCollection));
                    // MP_V06.tExistEZEntryParcelCollection xmlPolygons = (MP_V06.tExistEZEntryParcelCollection)serializer.Deserialize(xmlStream);
                }
            }

            if (DocInfo.DocRootName == "KVOKS")
            {
                //Под этим urn urn://x-artefacts-rosreestr-ru/outgoing/kvoks/3.0.1 как бы выписка версии KVOKS_V07
                if (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kvoks/3.0.1")
                {
                    DocInfo = parser.ParseKVOKS07(DocInfo, xmldoc);
                }
                else
                    DocInfo = parser.ParseKVOKS02(DocInfo, xmldoc); //NOTE : need test
            }

            if (DocInfo.DocRootName == "KPOKS")
            {
                // KPOKS_V4
                if (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kpoks/4.0.1")
                {
                    DocInfo = parser.ParseKPOKS04(DocInfo, xmldoc);
                }
                else
                    DocInfo = parser.ParseKPOKS03(DocInfo, xmldoc);
            }

            if (DocInfo.DocRootName == "SchemaParcels")
            {
                /* as file SchemaKPTMainForm was corrupted with wrong unicode bytes
                 * we hide them
                 * TODO: check form
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.misc28;
                SchemaKPTMainForm frm = new SchemaKPTMainForm();
                frm.Top = this.Top; frm.Left = this.Left;
                frm.StartPosition = FormStartPosition.Manual;
                frm.OpenXML(xmldoc);
                this.Visible = false;
                frm.ShowDialog();
                this.Close();
                */
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.page_white_csharp;
                DocInfo = parser.ParseSchemaParcels(DocInfo, xmldoc);
            }


            if (DocInfo.DocRootName == "STD_MP")
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                DocInfo = parser.ParseMPV04(DocInfo, xmldoc);
            }


            if ((DocInfo.DocRootName == "MP") && (DocInfo.Version == "05"))
            {

                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                DocInfo = parser.ParseMPV05(DocInfo, xmldoc);
            }

            // Типы MP Версия 06 - без XSD to clasess. напрямую XSD.exe
            if ((DocInfo.DocRootName == "MP") && (DocInfo.Version == "06"))
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.cross;
                DocInfo = parser.ParseMPV06(DocInfo, xmldoc);
            }


            if (DocInfo.DocRootName == "STD_TP")
            {
                DocInfo = parser.ParseSTDTPV02(DocInfo, xmldoc);
            }

            //TP
            if (DocInfo.DocRootName == "TP")
            {
                //toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                DocInfo = parser.ParseTP_V03(DocInfo, xmldoc);
            }
            //xmlStream.Dispose();
            xmldoc = null;
            GC.Collect(); //start mem garbage 
            return DocInfo;
        }
    }

}




