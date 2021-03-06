﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RRTypes.kpt10_un;
using netFteo.Spatial;

namespace RRTypes
{
    /// <summary>
    /// Утилиты преобразований из классов KPT_10 в NetFteo.Baseclasses
    /// </summary>
  public static class KPT_v10Utils
    {

      public static TPolygon KPT10OKSEntSpatToFteo(string Definition, tEntitySpatialOKSOut ES)
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
                    Point.Definition = ES.SpatialElement[0].SpelementUnit[iord].SuNmb;
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
                        Point.Definition = ES.SpatialElement[iES].SpelementUnit[iord].SuNmb;
                        InLayer.AddPoint (Point);
                    }
                }
                return EntSpat;

            }
        }

        /// <summary>
        /// TODO : develop here like 
        /// </summary>
        /// <param name="Definition"></param>
        /// <param name="ES"></param>
        /// <returns></returns>
		public static TPolygon KPT10LandEntSpatToFteo(string Definition, tEntitySpatialLandOut ES)
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
                        Point.Definition = ES.SpatialElement[iES].SpelementUnit[iord].SuNmb;
                        InLayer.AddPoint(Point);
                    }
                }
                return EntSpat;
            }
        }
             /*
        public static TEntitySpatial KPT10LandES(string Definition, tEntitySpatialLandOut ES)
        {

            TEntitySpatial ResES = new TEntitySpatial();
       
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
      
            return ResES;
        }
    */
        public static TPolygon AddEntSpatKPT10(string Definition, tEntitySpatialZUOut ES)
        {
            {
                TPolygon EntSpat = new TPolygon();
                EntSpat.Definition = Definition;
               // Random gen = new Random();
              //  EntSpat.Layer_id = gen.Next();
                //Первый (внешний) контур
                for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
                {
                   
                    
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
                       
                        InLayer.AddPoint(CommonCast.CasterSpatial.GetUnit(ES.SpatialElement[iES].SpelementUnit[iord]));
                    }
                }
                return EntSpat;

            }
        }

		public static netFteo.Rosreestr.TLocation LocAddrKPT10(tAddressOut Address)
		{
			if (Address == null) return null;
			netFteo.Rosreestr.TLocation Loc = new netFteo.Rosreestr.TLocation();
			Loc.Address.KLADR = Address.KLADR;
			Loc.Address.Note = Address.Note;
			Loc.Address.OKATO = Address.OKATO;
			Loc.Address.OKTMO = Address.Note;
			Loc.Address.Region = Address.Region.ToString();
			if (Address.District != null)
				Loc.Address.District = Address.District.Name;
			if (Address.Locality != null)
				Loc.Address.Locality = Address.Locality.Name;
			if (Address.City != null)
				Loc.Address.City = Address.City.Type + " " + Address.City.Name;
			if (Address.Street != null)
				Loc.Address.Street = Address.Street.Name;
			if (Address.Level1 != null)
				Loc.Address.Level1 = Address.Level1.Type.ToString() + " " + Address.Level1.Value;

			// dRegionsRF.Item99.
			return Loc;
		}

		public static string BoundToName(tCadastralBlockBound GKNBound)
		{


			if (GKNBound.SubjectsBoundary != null) return "Граница между субъектами Российской Федерации";
			if (GKNBound.MunicipalBoundary != null) return "Граница муниципального образования";
			if (GKNBound.InhabitedLocalityBoundary != null) return "Граница населенного пункта";
			//return GKNBound.Description;
			return null;
		}

		public static string ZoneToName(tCadastralBlockZone GKNZone)
		{
			if (GKNZone.SpecialZone != null) return "Зона с особыми условиями использования территорий";
			if (GKNZone.TerritorialZone != null) return "Территориальная зона";

			//return GKNZone.Description;
			return null;
		}

		public static List<string> PermittedUseCollectionToList(netFteo.XML.XSDFile dAllowedUse_v02,  tPermittedUseAllCollection pu)
		{
			List<string> res = new List<string>();
			foreach (tPermittedUseAll use in pu)
			{
				if (use.LandUseSpecified)
				{
                    if (dAllowedUse_v02 != null)
					res.Add(dAllowedUse_v02.Item2Annotation(use.LandUse.ToString()));
				}
			}
			return res;
		}


	}
}
