using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RRTypes.kpt09;
using netFteo.Spatial;

namespace RRTypes
{
    /// <summary>
    /// Утилиты преобразований из классов KPT V 09 в NetFteo.Baseclasses
    /// </summary>
    public static class KPT_v09Utils
    {

        public static TPolygon     KPT09OKSEntSpatToFteo(string Definition, tEntitySpatialOKSOut ES)
        {
            {
                if (ES == null) return null;
                TPolygon EntSpat = new TPolygon();
                EntSpat.Definition = Definition;


                //Первый (внешний) контур
                for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
                {

                    TPoint Point = new TPoint();
                    Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                    Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                    Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                    Point.NumGeopointA = ES.SpatialElement[0].SpelementUnit[iord].SuNmb;
                    EntSpat.AddPoint(Point);
                }
                //Внутренние контура
                for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
                {
                    TRing InLayer = EntSpat.AddChild();
                    for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                    {

                        TPoint Point = new TPoint();
                        Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                        Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                        Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                        Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].SuNmb;
                        InLayer.AddPoint(Point);
                    }
                }
                return EntSpat;

            }
        }
		public static TPolygon KPT09LandEntSpatToFteo(string Definition, tEntitySpatialLandOut ES)
        {
            {
                if (ES == null) return null;
                TPolygon EntSpat = new TPolygon();
                EntSpat.Definition = Definition;


                //Первый (внешний) контур
                for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
                {

                    TPoint Point = new TPoint();
                    Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                    Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                    Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                    Point.NumGeopointA = ES.SpatialElement[0].SpelementUnit[iord].SuNmb;
                    EntSpat.AddPoint (Point);
                }
                //Внутренние контура
                for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
                {
                    TRing InLayer = EntSpat.AddChild();
                    for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                    {

                        TPoint Point = new TPoint();
                        Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                        Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                        Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                        Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].SuNmb;
                        InLayer.AddPoint(Point);
                    }
                }
                return EntSpat;

            }
        }
        public static TPolygon AddEntSpatKPT09(string Definition, tEntitySpatialZUOut ES)
        {
            {
                TPolygon EntSpat = new TPolygon();
                EntSpat.Definition = Definition;
                // Random gen = new Random();
                //  EntSpat.Layer_id = gen.Next();

                //Первый (внешний) контур
                for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
                {
                    /*
                    Point Point = new Point();
                    Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                    Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                    Point.oldX = Point.x; Point.oldY = Point.y;
                    if (ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopointSpecified)
                        Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                    Point.NumGeopointA = ES.SpatialElement[0].SpelementUnit[iord].SuNmb;
                    */
                    EntSpat.AddPoint(CommonCast.CasterSpatial.GetUnit(ES.SpatialElement[0].SpelementUnit[iord]));
                }
                //Внутренние контура
                for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
                {
                    TRing InLayer = EntSpat.AddChild();
                    // Random Ingen = new Random();
                    // InLayer.Layer_id = Ingen.Next();
                    for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                    {
                        /*
                        Point Point = new Point();
                        Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                        Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                        Point.oldX = Point.x; Point.oldY = Point.y;
                        if (ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopointSpecified)
                            Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                        Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].SuNmb;

                        */
                        InLayer.AddPoint(CommonCast.CasterSpatial.GetUnit(ES.SpatialElement[iES].SpelementUnit[iord]));
                    }
                }
                return EntSpat;

            }
        }


        public static netFteo.Rosreestr.TAddress AddrKPT09(tAddressOut Address)
        {
            if (Address == null) return null;
            netFteo.Rosreestr.TAddress Adr = new netFteo.Rosreestr.TAddress();
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

        public static string BoundToName(tCadastralBlockBound GKNBound)
        {
            if (GKNBound.SubjectsBoundary != null) return "Граница между субъектами Российской Федерации";
            if (GKNBound.MunicipalBoundary != null) return "Граница муниципального образования";
            if (GKNBound.InhabitedLocalityBoundary != null) return "Граница населенного пункта";
            return null;
        }

 

        public static string ZoneToName(tCadastralBlockZone GKNZone)
        {
            if (GKNZone.SpecialZone != null) return "Зона с особыми условиями использования территорий";
            if (GKNZone.TerritorialZone != null) return "Территориальная зона";
            return null;
        }

        public static List<string> PermittedUseCollectionToList(tTerritorialZonePermittedUseCollection pu)
        {
            List<string> res = new List<string>();
            foreach (tTerritorialZonePermittedUse use in pu)
            {
                res.Add(use.PermittedUse);
            }
            return res;
        }

        /*
        public static List<string> PermittedUseCollectionToList(kpt10_un.tPermittedUseCollection  pu)

        {
            
            List<string> res = new List<string>();
            foreach (kpt10_un.tPermittedUses  use in pu)
            {
                res.Add(use.PermittedUse);
            }
            return res;
        }
         * */
    }
}
