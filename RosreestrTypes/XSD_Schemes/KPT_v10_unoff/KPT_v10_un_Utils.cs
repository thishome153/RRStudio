using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RRTypes.kpt10_un;
using netFteo.Spatial;

namespace RRTypes
{
    /// <summary>
    /// Утилиты преобразований из классов KPT V 09 в NetFteo.Baseclasses
    /// </summary>
  public static class KPT_v10Utils
    {

      public static TMyPolygon KPT10OKSEntSpatToFteo(string Definition, tEntitySpatialOKSOut ES)
        {
            {
                if (ES == null) return null;
                TMyPolygon EntSpat = new TMyPolygon();
                EntSpat.Definition = Definition;


                //Первый (внешний) контур
                for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
                {

                    Point Point = new Point();
                    Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                    Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                    Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                    Point.NumGeopointA = ES.SpatialElement[0].SpelementUnit[iord].SuNmb;
                    EntSpat.AddPoint (Point);
                }
                //Внутренние контура
                for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
                {
                    TMyOutLayer InLayer = EntSpat.AddChild();
                    for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                    {

                        Point Point = new Point();
                        Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                        Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                        Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                        Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].SuNmb;
                        InLayer.AddPoint (Point);
                    }
                }
                return EntSpat;

            }
        }
      
      public static TMyPolygon KPT10LandEntSpatToFteo(string Definition, tEntitySpatialLandOut ES)
        {
            {
                if (ES == null) return null;
                TMyPolygon EntSpat = new TMyPolygon();
                EntSpat.Definition = Definition;
                //Первый (внешний) контур
                for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
                {

                    Point Point = new Point();
                    Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                    Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                    Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                    Point.NumGeopointA = ES.SpatialElement[0].SpelementUnit[iord].SuNmb;
                    EntSpat.AddPoint(Point);
                }
                //Внутренние контура
                for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
                {
                    TMyOutLayer InLayer = EntSpat.AddChild();
                    for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                    {

                        Point Point = new Point();
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
       
        public static TMyPolygon AddEntSpatKPT10(string Definition, tEntitySpatialZUOut ES)
        {
            {
                TMyPolygon EntSpat = new TMyPolygon();
                EntSpat.Definition = Definition;
               // Random gen = new Random();
              //  EntSpat.Layer_id = gen.Next();
                //Первый (внешний) контур
                for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
                {

                    Point Point = new Point();
                    Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                    Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                    Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                    Point.NumGeopointA = ES.SpatialElement[0].SpelementUnit[iord].SuNmb;
                    EntSpat.AddPoint(Point);
                }
                //Внутренние контура
                for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
                {
                    TMyOutLayer InLayer = EntSpat.AddChild();
                   // Random Ingen = new Random();
                   // InLayer.Layer_id = Ingen.Next();
                    for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                    {

                        Point Point = new Point();
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
      public static netFteo.Rosreestr.TAddress AddrKPT10(tAddressOut Address)
      {
          if (Address == null) return null;
            netFteo.Rosreestr.TAddress Adr = new netFteo.Rosreestr.TAddress();
       Adr.KLADR = Address.KLADR;
       Adr.Note = Address.Note;
       Adr.OKATO = Address.OKATO;
       Adr.OKTMO = Address.Note;
       Adr.Region = Address.Region.ToString();
       if (Address.District != null)
           Adr.District = Address.District.Name;
       if (Address.Locality != null)
           Adr.Locality = Address.Locality.Name;
       if (Address.City != null)
           Adr.City = Address.City.Type + " "+ Address.City.Name;
       if (Address.Street != null)
           Adr.Street = Address.Street.Name;
       if (Address.Level1 != null)
           Adr.Level1 = Address.Level1.Type.ToString()+ " " + Address.Level1.Value;

         // dRegionsRF.Item99.
       return Adr;
      }
      
      public static string BoundToName(tCadastralBlockBound GKNBound)
      {


          if (GKNBound.SubjectsBoundary  != null) return "Граница между субъектами Российской Федерации";
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
   

       
    }
}
