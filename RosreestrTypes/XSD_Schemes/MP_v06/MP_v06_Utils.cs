using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RRTypes.MP_V06
{
    public static class CasterEZPEntrys
    {

        public static string EntSysDefault = "Id35722ef2-a7d7-4159-af6a-ed98cb6236e3";
        public static string GeopointZacrepDefault = "Закрепление отсутствует";
        /*
        private static tSpelementUnitOldNewCollection CastElementUnits(netFteo.Spatial.TRing layer)
        {
            tSpelementUnitOldNewCollection res = new tSpelementUnitOldNewCollection();
               foreach(netFteo.Spatial.Point point in layer.Points)
            {
                tSpelementUnitOldNew pt = new tSpelementUnitOldNew();
                pt.NewOrdinate = new tSpelementUnitOldNewNewOrdinate();
                pt.OldOrdinate = new tOrdinateXY();
                pt.OldOrdinate.NumGeopoint = point.NumGeopointA;
                pt.OldOrdinate.X = (decimal) point.x;
                pt.OldOrdinate.Y = (decimal)point.y;

                pt.NewOrdinate.GeopointZacrep = GeopointZacrepDefault;
                pt.NewOrdinate.NumGeopoint = point.NumGeopointA;
                pt.NewOrdinate.X = (decimal) point.x;
                pt.NewOrdinate.Y = (decimal) point.y;
                pt.NewOrdinate.DeltaGeopoint = Convert.ToDecimal(point.Mt.ToString("0.00"));
                res.Add(pt);
            }
            return res;
        }

        private static tEntitySpatialOldNewSpatialElement CastElement(tSpelementUnitOldNewCollection units)
        {
            tEntitySpatialOldNewSpatialElement res = new tEntitySpatialOldNewSpatialElement();
            res.SpelementUnit = new tSpelementUnitOldNewCollection();
            res.SpelementUnit.AddRange(units);
            return res;
        }
        */
        private static tEntitySpatialOldNewSpatialElement CastElement(netFteo.Spatial.TRing layer)
        {
            bool with_OldOrdinate = false; //случай с :511
            tEntitySpatialOldNewSpatialElement res = new tEntitySpatialOldNewSpatialElement();

            tSpelementUnitOldNewCollection resP = new tSpelementUnitOldNewCollection();
            foreach (netFteo.Spatial.TPoint point in layer)
            {
                tSpelementUnitOldNew pt = new tSpelementUnitOldNew();
                pt.NewOrdinate = new tSpelementUnitOldNewNewOrdinate();
                if (with_OldOrdinate)
                {
                    pt.OldOrdinate = new tOrdinateXY();
                    pt.OldOrdinate.NumGeopoint = point.NumGeopointA;
                    pt.OldOrdinate.X = (decimal)point.x;
                    pt.OldOrdinate.Y = (decimal)point.y;
                }

                pt.NewOrdinate.GeopointZacrep = GeopointZacrepDefault;
                pt.NewOrdinate.NumGeopoint = point.NumGeopointA;
                pt.NewOrdinate.X = (decimal)point.x;
                pt.NewOrdinate.Y = (decimal)point.y;
                pt.NewOrdinate.DeltaGeopoint = Convert.ToDecimal(point.Mt.ToString("0.00"));
                resP.Add(pt);
            }

            res.SpelementUnit = new tSpelementUnitOldNewCollection();
            res.SpelementUnit.AddRange(resP);

            return res;
        }

        //Построение отрезков границ
        private static List<tBordersInpBorder> BuildBorders(netFteo.Spatial.TRing layer, string spatial)
        {
           List< tBordersInpBorder> res = new List<tBordersInpBorder>();

            for (int i = 0; i <= layer.Count - 2; i++)
            {
                tBordersInpBorder tst = new tBordersInpBorder();
                tst.Point1 = layer[i].NumGeopointA;
                tst.Point2 = layer[i+1].NumGeopointA;
                tst.Spatial = spatial;
                tst.Edge = new tLength();
                tst.Edge.LengthSpecified = true;
                tst.Edge.Length =Convert.ToDecimal( netFteo.Spatial.Geodethic.lent(layer[i].x, layer[i].y, layer[i + 1].x, layer[i + 1].y).ToString("0.00"));
                res.Add(tst);
            }
            /* // Последнее неверно - в списке точек обязательно замыкающая (начальная)
             * 
            tBordersInpBorder tstLast = new tBordersInpBorder();
            tstLast.Point1 = layer.Points[layer.Points.Count-1].NumGeopointA;
            tstLast.Point2 = layer.Points[0].NumGeopointA;
            tstLast.Spatial = spatial;
            tstLast.Edge = new tLength();
            tstLast.Edge.LengthSpecified = true;
            tstLast.Edge.Length = Convert.ToDecimal(netFteo.Spatial.Geodethic.lent(layer.Points[layer.Points.Count - 1].x, 
                                                                                   layer.Points[layer.Points.Count - 1].y, 
                                                                                   layer.Points[0].x, 
                                                                                   layer.Points[0].y)
                                                                                   .ToString("0.00"));
            res.Add(tstLast);
            */


            return res;
        }

        private static tEntitySpatialOldNew CastES(netFteo.Spatial.TMyPolygon entryES)
        {
            tEntitySpatialOldNew res = new tEntitySpatialOldNew();
            res.EntSys = EntSysDefault;
            res.SpatialElement = new tEntitySpatialOldNewSpatialElementCollection();


            res.SpatialElement.Add(CastElement(entryES)); // внешняя граница
            //Остальные внутренние
            foreach (netFteo.Spatial.TRing child in entryES.Childs)
            {
                res.SpatialElement.Add(CastElement(child));
            }

            //Отрезочки границ:
            int spatial_Num = 1;
            res.Borders = new tBordersInpBorderCollection();
            res.Borders.AddRange(BuildBorders(entryES, (spatial_Num++).ToString("0")));

            foreach (netFteo.Spatial.TRing child in entryES.Childs)
            {
                res.Borders.AddRange(BuildBorders(child, (spatial_Num++).ToString("0")));
            }



            return res;
        }

        private static tExistEZEntryParcel CastEntry (netFteo.Spatial.TMyPolygon entry)
        {
            tExistEZEntryParcel res = new tExistEZEntryParcel();
            res.CadastralNumber = entry.Definition;
            res.Area = new tAreaContour();
            res.Area.Area = Convert.ToDecimal(entry.AreaSpatialFmt("0.00")); // фактическую площадь указываем
            res.Area.InaccuracySpecified = true;
            res.Area.Inaccuracy = (decimal) Math.Round (3.5*2.5*Math.Sqrt(entry.AreaSpatial));
            res.EntitySpatial = new tEntitySpatialOldNew();
            res.EntitySpatial = CastES(entry);
            return res;
        }

        public static tExistEZEntryParcelCollection CastEZP (netFteo.Spatial.TCompozitionEZ EZP)
        {
            tExistEZEntryParcelCollection res = new tExistEZEntryParcelCollection();
			/* TODO
            foreach (netFteo.Spatial.EZPEntry entry in EZP)
                res.Add(CastEntry(entry));
			*/
            return res;
        }

    }
}
