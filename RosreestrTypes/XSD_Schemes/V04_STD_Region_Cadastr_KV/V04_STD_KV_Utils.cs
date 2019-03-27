using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RRTypes
{
    public static class KV04_Utils
    {

        #region-----------------Конвертация из ОИПД КВЗУ в ОИПД Fteo.Spatial
        public static netFteo.Spatial.TMyPolygon AddEntSpatKVZU04(string Definition, RRTypes.STD_KV04.Entity_Spatial ES)
        {
            netFteo.Spatial.TMyPolygon EntSpat = new netFteo.Spatial.TMyPolygon();
            EntSpat.Definition = Definition;
            if (ES == null) { return EntSpat; }

           
            //Первый (внешний) контур
            
            for (int iord = 0; iord <= ES.Spatial_Element[0].Spelement_Unit.Count - 1; iord++)
            {

                netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                Point.x = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].X);
                Point.y = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].Y);
                Point.oldX = Point.x;
                Point.oldY = Point.y;
                Point.Mt = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].Delta_Geopoint);
                Point.NumGeopointA = ES.Spatial_Element[0].Spelement_Unit[iord].Ordinate[0].Num_Geopoint;
                EntSpat.AddPoint(Point);
            }
            //Есть замыкающие точки в KVZU 04?  Кажись нема!
            //Добавим 
            netFteo.Spatial.TPoint Point_ = new netFteo.Spatial.TPoint();
            Point_.x = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[0].Ordinate[0].X);
            Point_.y = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[0].Ordinate[0].Y);
            Point_.oldX = Point_.x;
            Point_.oldY = Point_.y;
            Point_.Mt = Convert.ToDouble(ES.Spatial_Element[0].Spelement_Unit[0].Ordinate[0].Delta_Geopoint);
            Point_.NumGeopointA = ES.Spatial_Element[0].Spelement_Unit[0].Ordinate[0].Num_Geopoint;
            EntSpat.AddPoint(Point_);

            //Внутренние контура
            for (int iES = 1; iES <= ES.Spatial_Element.Count - 1; iES++)
            {
                netFteo.Spatial.TRing InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.Spatial_Element[iES].Spelement_Unit.Count - 1; iord++)
                {

                    netFteo.Spatial.TPoint Point = new netFteo.Spatial.TPoint();
                    Point.x = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].X);
                    Point.y = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].Y);
                    Point.oldX = Point.x; Point.oldY = Point.y;
                    Point.Mt = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].Delta_Geopoint);
                    Point.NumGeopointA = ES.Spatial_Element[iES].Spelement_Unit[iord].Su_Nmb;
                    InLayer.AddPoint (Point);
                }
                //Добавим замыкающие точки
                netFteo.Spatial.TPoint LastPoint = new netFteo.Spatial.TPoint();
                LastPoint.x = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[0].Ordinate[0].X);
                LastPoint.y = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[0].Ordinate[0].Y);
                LastPoint.oldX = LastPoint.x; LastPoint.oldY = LastPoint.y;
                LastPoint.Mt = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[0].Ordinate[0].Delta_Geopoint);
                LastPoint.NumGeopointA = ES.Spatial_Element[iES].Spelement_Unit[0].Su_Nmb;
                InLayer.AddPoint (LastPoint);
            }
           
            return EntSpat;
        }
        #endregion

        public static netFteo.Rosreestr.TMyEncumbrance KVZUEncumtoFteoEncum(RRTypes.STD_KV04.tEncumbrance Encumfrom)
        {
            netFteo.Rosreestr.TMyEncumbrance MyEnc = new netFteo.Rosreestr.TMyEncumbrance();
            MyEnc.Name = Encumfrom.Name;
            /* if (Encumfrom.Registration != null)
             {
                 MyEnc.RegDate = Encumfrom.Registration.RegDate.ToString();
                 MyEnc.RegNumber = Encumfrom.Registration.RegNumber.ToString();
             }
             * 
             MyEnc.Type = Encumfrom.Type.ToString();
             for (int io = 0; io <= Encumfrom.Owner_Restriction_InFavorem.Count - 1; io++)
             {
                 if (Encumfrom.Owner_Restriction_InFavorem[io].Person != null)
                     MyEnc.Owners.Add(new netFteo.Rosreestr.TMyOwner(Encumfrom.OwnersRestrictionInFavorem[io].Person.FirstName + " " +
                        Encumfrom.Owner_Restriction_InFavorem[io].Person.Patronymic + " " + Encumfrom.OwnersRestrictionInFavorem[io].Person.FamilyName));
                 if (Encumfrom.Owner_Restriction_InFavorem[io].Organization != null)
                     MyEnc.Owners.Add(new netFteo.Rosreestr.TMyOwner(Encumfrom.OwnersRestrictionInFavorem[io].Organization.Name));
                 if (Encumfrom.OwnersRestrictionInFavorem[io].Governance != null)
                     MyEnc.Owners.Add(new netFteo.Rosreestr.TMyOwner(Encumfrom.OwnersRestrictionInFavorem[io].Governance.Name));
             }*/
            return MyEnc;
        }

        public static netFteo.Rosreestr.TMyEncumbrances KVZUEncumstoFteoEncums(RRTypes.STD_KV04.tSubParcelEncumbranceCollection Encumsfrom)
        {
            netFteo.Rosreestr.TMyEncumbrances MyEncs = new netFteo.Rosreestr.TMyEncumbrances();
            for (int i = 0; i <= Encumsfrom.Count - 1; i++)
            {
                MyEncs.Add(KVZUEncumtoFteoEncum(Encumsfrom[i]));
            }

            return MyEncs;

        }
        public static netFteo.Rosreestr.TMyRights KVZURightstoFteorights(RRTypes.STD_KV04.tRightCollection Rightsfrom)
        {
            netFteo.Rosreestr.TMyRights MyRights = new netFteo.Rosreestr.TMyRights();

            for (int i = 0; i <= Rightsfrom.Count - 1; i++)
            {
                netFteo.Rosreestr.TRight Right = new netFteo.Rosreestr.TRight();
                Right.Type = Rightsfrom[i].Type.ToString();
                Right.Name = Rightsfrom[i].Name;
                if (Rightsfrom[i].ShareText != null)
                    Right.Desc = Rightsfrom[i].ShareText;
               
                for (int io = 0; io <= Rightsfrom[i].Owner.Count - 1; io++)
                {
                    if (Rightsfrom[i].Owner[0].Person != null)
                        Right.Owners.Add(new netFteo.Rosreestr.TMyOwner(Rightsfrom[i].Owner[0].Person.FIO.First + " " +
                           Rightsfrom[i].Owner[0].Person.FIO.Patronymic + " " + Rightsfrom[i].Owner[0].Person.FIO.Surname));
                    if (Rightsfrom[i].Owner[0].Organization != null)
                        Right.Owners.Add(new netFteo.Rosreestr.TMyOwner(Rightsfrom[i].Owner[0].Organization.Name));
                    if (Rightsfrom[i].Owner[0].Governance != null)
                        Right.Owners.Add(new netFteo.Rosreestr.TMyOwner(Rightsfrom[i].Owner[0].Governance.Name));

                }
     
                MyRights.Add(Right);

            }

            return MyRights;

        }
    }
}
