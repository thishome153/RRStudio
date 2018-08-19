using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RRTypes
{
   public static class STD_TP02_Utils
    {
       public static netFteo.BaseClasess.TMyPolygon AddEntSpatTP02(string Definition, RRTypes.STD_TPV02.Entity_Spatial ES)
       {
           netFteo.BaseClasess.TMyPolygon EntSpat = new netFteo.BaseClasess.TMyPolygon();
           EntSpat.Definition = Definition;


           //Первый (внешний) контур
           for (int iord = 0; iord <= ES.Spatial_Element[0].Spelement_Unit.Count - 1; iord++)
           {

               netFteo.BaseClasess.TmyPointO Point = new netFteo.BaseClasess.TmyPointO();

               Point.x = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].X);
               Point.y = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].Y);
               Point.Mt = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].Delta_Geopoint);
               //Point.Description = ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].Geopoint_Zacrep;
               Point.Pref = ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].Point_Pref;
               Point.NumGeopointA = ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].Num_Geopoint;
               EntSpat.Points.AddPoint (Point);
           }
           //Внутренние контура
           for (int iES = 1; iES <= ES.Spatial_Element.Count - 1; iES++)
           {
               netFteo.BaseClasess.TMyOutLayer InLayer = EntSpat.AddChild();
               for (int iord = 0; iord <= ES.Spatial_Element[iES].Spelement_Unit.Count - 1; iord++)
               {

                   netFteo.BaseClasess.TmyPointO Point = new netFteo.BaseClasess.TmyPointO();
                   Point.x = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].X);
                   Point.y = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].Y);
                   Point.Mt = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].Delta_Geopoint);
                   Point.NumGeopointA = ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].Num_Geopoint;
                   InLayer.Points.AddPoint(Point);
               }
           }
           return EntSpat;
       }
    }
}
