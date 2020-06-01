using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//namespace RosreestrTypes.XSD_Schemes.V04_STD_MP
namespace RRTypes
{
    public class RetResult
    {
       public bool HasError;
       public string Message;
       public RetResult()
       {
           this.HasError = false;
           this.Message = "Contructed";
       }

    }
    public static class  STD_MP_Utils
    {
        #region-----------------Конвертация из ОИПД Межевого плана в ОИПД Fteo.Spatial
        public static netFteo.Spatial.TPolygon AddEntSpatSTDMP4(string Definition, RRTypes.STD_MPV04.Entity_Spatial ES)
        {
            netFteo.Spatial.TPolygon EntSpat = new netFteo.Spatial.TPolygon();
            EntSpat.Definition = Definition;

            
            //Первый (внешний) контур
            for (int iord = 0; iord <= ES.Spatial_Element[0].Spelement_Unit.Count - 1; iord++)
            {

                netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                Point.x = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].NewOrdinate.X);
                Point.y = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].NewOrdinate.Y);
                Point.Mt = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].NewOrdinate.Delta_Geopoint);
                Point.Description = ES.Spatial_Element[0].Spelement_Unit[iord].NewOrdinate.Geopoint_Zacrep;
                Point.Pref = ES.Spatial_Element[0].Spelement_Unit[iord].NewOrdinate.Point_Pref;
                Point.NumGeopointA = ES.Spatial_Element[0].Spelement_Unit[iord].NewOrdinate.Num_Geopoint;
                EntSpat.AddPoint (Point);
            }
            //Внутренние контура
            for (int iES = 1; iES <= ES.Spatial_Element.Count - 1; iES++)
            {
                netFteo.Spatial.TRing InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.Spatial_Element[iES].Spelement_Unit.Count - 1; iord++)
                {

                    netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                    Point.x = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].NewOrdinate.X);
                    Point.y = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].NewOrdinate.Y);
                    Point.Mt= Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].NewOrdinate.Delta_Geopoint);
                    Point.NumGeopointA = ES.Spatial_Element[iES].Spelement_Unit[iord].NewOrdinate.Num_Geopoint;
                    InLayer.AddPoint(Point);
                }
            }
            return EntSpat;
        }
        public static RetResult CheckESMP4(RRTypes.STD_MPV04.Entity_Spatial ES)
        {
            //netFteo.Spatial.TPolygon EntSpat = new netFteo.Spatial.TPolygon();
            RetResult res = new RetResult();
            
            //Проверка замыкания во внешнем контуре в текстовом виде
            int lastId = ES.Spatial_Element[0].Spelement_Unit.Count - 1;
            if ((ES.Spatial_Element[0].Spelement_Unit[0].NewOrdinate.X == ES.Spatial_Element[0].Spelement_Unit[lastId].NewOrdinate.X) ||
                 (ES.Spatial_Element[0].Spelement_Unit[0].NewOrdinate.Y == ES.Spatial_Element[0].Spelement_Unit[lastId].NewOrdinate.Y)
              )
            {  
                res.HasError = false;
                res.Message = "Контур проверен";
                
            }

            //Первый (внешний) контур
         
            //Внутренние контура
            for (int iES = 1; iES <= ES.Spatial_Element.Count - 1; iES++)
            {
                if ((ES.Spatial_Element[iES].Spelement_Unit[0].NewOrdinate.X == ES.Spatial_Element[iES].Spelement_Unit[ES.Spatial_Element[iES].Spelement_Unit.Count - 1].NewOrdinate.X) ||
                    (ES.Spatial_Element[iES].Spelement_Unit[0].NewOrdinate.Y == ES.Spatial_Element[iES].Spelement_Unit[ES.Spatial_Element[iES].Spelement_Unit.Count - 1].NewOrdinate.Y)
                    )
                {
                    {
                        res.HasError = false;
                        res.Message = "Контур проверен";

                    }
                }

            }

            return res;

        }

        public static netFteo.Spatial.TPolygon AddSubParcelESTDMP4(string Definition, RRTypes.STD_MPV04.tNewSubParcelEntity_Spatial ES)
        {
            netFteo.Spatial.TPolygon EntSpat = new netFteo.Spatial.TPolygon();
            EntSpat.Definition = Definition;


            //Первый (внешний) контур
            for (int iord = 0; iord <= ES.Spatial_Element[0].Spelement_Unit.Count - 1; iord++)
            {

                netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                if (ES.Spatial_Element[0].Spelement_Unit[iord].NewOrdinate != null)
                {
                    Point.x = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].NewOrdinate.X);
                    Point.y = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].NewOrdinate.Y);
                    Point.Mt = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].NewOrdinate.Delta_Geopoint);
                    Point.Description = ES.Spatial_Element[0].Spelement_Unit[iord].NewOrdinate.Geopoint_Zacrep;
                    Point.Pref = ES.Spatial_Element[0].Spelement_Unit[iord].NewOrdinate.Point_Pref;
                    Point.NumGeopointA = ES.Spatial_Element[0].Spelement_Unit[iord].NewOrdinate.Num_Geopoint;
                }
                EntSpat.AddPoint(Point);
            }
            //Внутренние контура
            for (int iES = 1; iES <= ES.Spatial_Element.Count - 1; iES++)
            {
                netFteo.Spatial.TRing InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.Spatial_Element[iES].Spelement_Unit.Count - 1; iord++)
                {

                    netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                    Point.x = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].NewOrdinate.X);
                    Point.y = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].NewOrdinate.Y);
                    Point.Mt = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].NewOrdinate.Delta_Geopoint);
                    Point.NumGeopointA = ES.Spatial_Element[iES].Spelement_Unit[iord].NewOrdinate.Num_Geopoint;
                    InLayer.AddPoint(Point);
                }
            }
            return EntSpat;
        }

        #endregion
    }
}
