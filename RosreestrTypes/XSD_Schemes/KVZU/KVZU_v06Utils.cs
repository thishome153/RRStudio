using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using RRTypes.kvzu;

namespace RRTypes
{
   public static class KVZU_v06Utils
    {
       public static netFteo.Rosreestr.TMyRights KVZURightstoFteorights(tRightCollection Rightsfrom)
       {
           netFteo.Rosreestr.TMyRights MyRights = new netFteo.Rosreestr.TMyRights();
           
           for (int i = 0; i <= Rightsfrom.Count - 1; i++)
           {
               netFteo.Rosreestr.TRight Right = new netFteo.Rosreestr.TRight();
               Right.Type = Rightsfrom[i].Type.ToString();
               Right.Name = Rightsfrom[i].Name;
               Right.RegDate = Rightsfrom[i].Registration.RegDate.ToString();
               if (Rightsfrom[i].Registration.RegNumber != null)
                Right.RegNumber = Rightsfrom[i].Registration.RegNumber.ToString();
  
               for (int io = 0; io <= Rightsfrom[i].Owners.Count - 1; io++)
               {
                   netFteo.Rosreestr.TMyOwner own = new netFteo.Rosreestr.TMyOwner();

                   if (Rightsfrom[i].Owners[io].Person != null)
                       own.OwnerName = Rightsfrom[i].Owners[io].Person.FamilyName + " " +
                          Rightsfrom[i].Owners[io].Person.FirstName + " " + Rightsfrom[i].Owners[io].Person.Patronymic;
                   if (Rightsfrom[i].Owners[io].Organization != null)
                       own.OwnerName = Rightsfrom[i].Owners[io].Organization.Name;
                   if (Rightsfrom[i].Owners[io].Governance != null)
                       own.OwnerName = Rightsfrom[i].Owners[io].Governance.Name;
                    //KPZU не содержит контактных адресов:
                   if (Rightsfrom[i].Owners[io].ContactOwner != null)
                       own.ContactOwner = Rightsfrom[i].Owners[io].ContactOwner.Address + " " + Rightsfrom[i].Owners[io].ContactOwner.Email;
                   
                   Right.Owners.Add(own);
               }


               MyRights.Add(Right);

           }

           return MyRights;

           }
       public static netFteo.Rosreestr.TMyRights KVZURightstoFteorights(kpzu06.tRightCollection Rightsfrom)
       {
           netFteo.Rosreestr.TMyRights MyRights = new netFteo.Rosreestr.TMyRights();

           for (int i = 0; i <= Rightsfrom.Count - 1; i++)
           {
               netFteo.Rosreestr.TRight Right = new netFteo.Rosreestr.TRight();
               Right.Type = Rightsfrom[i].Type.ToString();
               Right.Name = Rightsfrom[i].Name;
               Right.RegDate = Rightsfrom[i].Registration.RegDate.ToString();
               if (Rightsfrom[i].Registration.RegNumber != null)
                   Right.RegNumber = Rightsfrom[i].Registration.RegNumber.ToString();
               if (Rightsfrom[i].ShareText != null)
                   Right.ShareText = Rightsfrom[i].ShareText;
               if (Rightsfrom[i].Share != null)
                   Right.ShareText = Rightsfrom[i].Share.Numerator + "//" + Rightsfrom[i].Share.Denominator;
/*
               for (int io = 0; io <= Rightsfrom[i].Owners.Count - 1; io++)
               {
                   if (Rightsfrom[i].Owners[0].Person != null)
                       Right.Owners.Add(new netFteo.Rosreestr.TMyOwner(Rightsfrom[i].Owners[0].Person.FirstName + " " +
                          Rightsfrom[i].Owners[0].Person.Patronymic + " " + Rightsfrom[i].Owners[0].Person.FamilyName));
                   if (Rightsfrom[i].Owners[0].Organization != null)
                       Right.Owners.Add(new netFteo.Rosreestr.TMyOwner(Rightsfrom[i].Owners[0].Organization.Name));
                   if (Rightsfrom[i].Owners[0].Governance != null)
                       Right.Owners.Add(new netFteo.Rosreestr.TMyOwner(Rightsfrom[i].Owners[0].Governance.Name));

               }
               */
               for (int io = 0; io <= Rightsfrom[i].Owners.Count - 1; io++)
               {
                   netFteo.Rosreestr.TMyOwner own = new netFteo.Rosreestr.TMyOwner();

                   if (Rightsfrom[i].Owners[io].Person != null)
                       own.OwnerName = Rightsfrom[i].Owners[io].Person.FamilyName + " " +
                          Rightsfrom[i].Owners[io].Person.FirstName + " " + Rightsfrom[i].Owners[io].Person.Patronymic;
                   if (Rightsfrom[i].Owners[io].Organization != null)
                       own.OwnerName = Rightsfrom[i].Owners[io].Organization.Name;
                   if (Rightsfrom[i].Owners[io].Governance != null)
                       own.OwnerName = Rightsfrom[i].Owners[io].Governance.Name;
                   /* //KPZU не содержит контактных адресов:
                   if (Rightsfrom[i].Owners[io].GovernanceContactOwner != null)
                       own.ContactOwner = Rightsfrom[i].Owners[io].ContactOwner.Address + " " + Rightsfrom[i].Owners[io].ContactOwner.Email;
                   */
                   Right.Owners.Add(own);
               }


               MyRights.Add(Right);

           }

           return MyRights;

       }
       public static netFteo.Rosreestr.TMyRights KVZURightstoFteorights(kvzu07.tRightCollection Rightsfrom)
       {
           netFteo.Rosreestr.TMyRights MyRights = new netFteo.Rosreestr.TMyRights();

           for (int i = 0; i <= Rightsfrom.Count - 1; i++)
           {
               netFteo.Rosreestr.TRight Right = new netFteo.Rosreestr.TRight();
               Right.Type = Rightsfrom[i].Type.ToString();
               Right.Name = Rightsfrom[i].Name;
               if (Rightsfrom[i].ShareText != null)
               Right.ShareText = Rightsfrom[i].ShareText;

               if (Rightsfrom[i].Share != null)
                   Right.ShareText = Rightsfrom[i].Share.Numerator + "//" + Rightsfrom[i].Share.Denominator;

               Right.RegDate = Rightsfrom[i].Registration.RegDate.ToString();
               if (Rightsfrom[i].Registration.RegNumber != null)
                   Right.RegNumber = Rightsfrom[i].Registration.RegNumber.ToString();

               for (int io = 0; io <= Rightsfrom[i].Owners.Count - 1; io++)
               {
                   netFteo.Rosreestr.TMyOwner own = new netFteo.Rosreestr.TMyOwner();

                   if (Rightsfrom[i].Owners[io].Person != null)
                       own.OwnerName = Rightsfrom[i].Owners[io].Person.FamilyName + " " +
                          Rightsfrom[i].Owners[io].Person.FirstName + " " + Rightsfrom[i].Owners[io].Person.Patronymic;
                   if (Rightsfrom[i].Owners[io].Organization != null)
                       own.OwnerName = Rightsfrom[i].Owners[io].Organization.Name;
                   if (Rightsfrom[i].Owners[io].Governance != null)
                       own.OwnerName = Rightsfrom[i].Owners[io].Governance.Name;

                   if (Rightsfrom[i].Owners[io].ContactOwner != null)
                       own.ContactOwner = Rightsfrom[i].Owners[io].ContactOwner.Address + " " + Rightsfrom[i].Owners[io].ContactOwner.Email;

                   Right.Owners.Add(own);
               }

               MyRights.Add(Right);

           }

           return MyRights;

       }

       public static netFteo.Rosreestr.TMyEncumbrance KVZUEncumtoFteoEncum(tEncumbranceZU Encumfrom)
       {
           netFteo.Rosreestr.TMyEncumbrance MyEnc = new netFteo.Rosreestr.TMyEncumbrance();
           MyEnc.Name = Encumfrom.Name;

             if (Encumfrom.AccountNumber != null)
            MyEnc.AccountNumber = Encumfrom.AccountNumber;

             if (Encumfrom.CadastralNumberRestriction != null)
                 MyEnc.AccountNumber = Encumfrom.CadastralNumberRestriction;

           if (Encumfrom.Registration != null)
           {
               MyEnc.RegDate = Encumfrom.Registration.RegDate.ToString();
               if (Encumfrom.Registration.RegNumber != null)
               MyEnc.RegNumber = Encumfrom.Registration.RegNumber.ToString();
           }
           MyEnc.Type = Encumfrom.Type.ToString();
           for (int io = 0; io <= Encumfrom.OwnersRestrictionInFavorem.Count - 1; io++)
           {
               if (Encumfrom.OwnersRestrictionInFavorem[io].Person != null)
                   MyEnc.Owners.Add(new netFteo.Rosreestr.TMyOwner(Encumfrom.OwnersRestrictionInFavorem[io].Person.FirstName + " " +
                      Encumfrom.OwnersRestrictionInFavorem[io].Person.Patronymic + " " + Encumfrom.OwnersRestrictionInFavorem[io].Person.FamilyName));
               if (Encumfrom.OwnersRestrictionInFavorem[io].Organization != null)
                   MyEnc.Owners.Add(new netFteo.Rosreestr.TMyOwner(Encumfrom.OwnersRestrictionInFavorem[io].Organization.Name));
               if (Encumfrom.OwnersRestrictionInFavorem[io].Governance != null)
                    MyEnc.Owners.Add(new netFteo.Rosreestr.TMyOwner(Encumfrom.OwnersRestrictionInFavorem[io].Governance.Name));
           }
           if (Encumfrom.Duration != null)
           {
               if (Encumfrom.Duration.Started != null)
                   MyEnc.DurationStarted = Encumfrom.Duration.Started.ToString();
               if (Encumfrom.Duration.Stopped != null)
                   MyEnc.DurationStopped = Encumfrom.Duration.Stopped.ToString();
               if (Encumfrom.Duration.Term != null)
                   MyEnc.DurationStopped = Encumfrom.Duration.Term;
           }

           if (Encumfrom.Document != null)
           {
               MyEnc.Document.DocName = Encumfrom.Document.Name;
               if (Encumfrom.Document.Number != null)
               MyEnc.Document.Number = Encumfrom.Document.Number;
               if (Encumfrom.Document.Date != null)
                   MyEnc.Document.Date = Encumfrom.Document.Date.ToString("dd/MM/yyyy");
               
           }
           return MyEnc;
       }
       public static netFteo.Rosreestr.TMyEncumbrance KVZUEncumtoFteoEncum(kvzu07.tEncumbranceZU Encumfrom)
       {
           netFteo.Rosreestr.TMyEncumbrance MyEnc = new netFteo.Rosreestr.TMyEncumbrance();
           MyEnc.Name = Encumfrom.Name;

           if (Encumfrom.AccountNumber != null)
               MyEnc.AccountNumber = Encumfrom.AccountNumber;

           if (Encumfrom.CadastralNumberRestriction != null)
               MyEnc.AccountNumber = Encumfrom.CadastralNumberRestriction;

           if (Encumfrom.Registration != null)
           {
               MyEnc.RegDate = Encumfrom.Registration.RegDate.ToString();
               if (Encumfrom.Registration.RegNumber != null)
                   MyEnc.RegNumber = Encumfrom.Registration.RegNumber.ToString();
           }
           MyEnc.Type = Encumfrom.Type.ToString();
           for (int io = 0; io <= Encumfrom.OwnersRestrictionInFavorem.Count - 1; io++)
           {
               if (Encumfrom.OwnersRestrictionInFavorem[io].Person != null)
                   MyEnc.Owners.Add(new netFteo.Rosreestr.TMyOwner(Encumfrom.OwnersRestrictionInFavorem[io].Person.FirstName + " " +
                      Encumfrom.OwnersRestrictionInFavorem[io].Person.Patronymic + " " + Encumfrom.OwnersRestrictionInFavorem[io].Person.FamilyName));
               if (Encumfrom.OwnersRestrictionInFavorem[io].Organization != null)
                   MyEnc.Owners.Add(new netFteo.Rosreestr.TMyOwner(Encumfrom.OwnersRestrictionInFavorem[io].Organization.Name));
               if (Encumfrom.OwnersRestrictionInFavorem[io].Governance != null)
                   MyEnc.Owners.Add(new netFteo.Rosreestr.TMyOwner(Encumfrom.OwnersRestrictionInFavorem[io].Governance.Name));
           }
           if (Encumfrom.Duration != null)
           {
               if (Encumfrom.Duration.Started != null)
                   MyEnc.DurationStarted = Encumfrom.Duration.Started.ToString();
               if (Encumfrom.Duration.Stopped != null)
                   MyEnc.DurationStopped = Encumfrom.Duration.Stopped.ToString();
               if (Encumfrom.Duration.Term != null)
                   MyEnc.DurationStopped = Encumfrom.Duration.Term;
           }

           if (Encumfrom.Document != null)
           {
               MyEnc.Document.DocName = Encumfrom.Document.Name;
               if (Encumfrom.Document.Number != null)
                   MyEnc.Document.Number = Encumfrom.Document.Number;
               if (Encumfrom.Document.Date != null)
                   MyEnc.Document.Date = Encumfrom.Document.Date.ToString("dd/MM/yyyy");

           }
           return MyEnc;
       }

       public static netFteo.Rosreestr.TMyEncumbrances KVZUEncumstoFteoEncums(tEncumbranceZUCollection Encumsfrom)
       {
           netFteo.Rosreestr.TMyEncumbrances MyEncs = new netFteo.Rosreestr.TMyEncumbrances();
           for (int i = 0; i <= Encumsfrom.Count - 1; i++)
           {
              MyEncs.Add(KVZUEncumtoFteoEncum(Encumsfrom[i]));
           }

           return MyEncs;

       }
       public static netFteo.Rosreestr.TMyEncumbrances KVZUEncumstoFteoEncums(kvzu07.tEncumbranceZUCollection Encumsfrom)
       {
           netFteo.Rosreestr.TMyEncumbrances MyEncs = new netFteo.Rosreestr.TMyEncumbrances();
           for (int i = 0; i <= Encumsfrom.Count - 1; i++)
           {
               MyEncs.Add(KVZUEncumtoFteoEncum(Encumsfrom[i]));
           }

           return MyEncs;

       }
       public static int KVZUState(dStates st)
       {
           switch (st.ToString())
           {
               case "Item0": { return 0; }
               case "0": { return 0; }
               case "Item01": { return 1; }
               case "Item05": { return 5; }
               case "Item06": { return 6; }
               case "Item07": { return 7; }
               case "Item08": { return 8; }
               default: { return -1; }
           }
       }
       public static int KVZUState(kvzu07.dStates st)
       {
           switch (st.ToString())
           {
               case "Item0": { return 0; }
               case "0": { return 0; }
               case "Item01": { return 1; }
               case "Item05": { return 5; }
               case "Item06": { return 6; }
               case "Item07": { return 7; }
               case "Item08": { return 8; }
               default: { return -1; }
           }
       }
       /*
        /// <summary>
        /// Код статуса в текст
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
       public static string State2Str(string value)
       {
           if (value.Contains("Item")) { value = value.Substring(4); }

        if (value == "01")
        {
               return "Ранее учтенный";
        }

        if (value == "03") // из http://maps.rosreestr.ru/arcgis/rest/services/Cadastre/CadastreSelected/MapServer/1?f=pjson
        {
               return "Условный";
        }
           
           if (value == "05")
           {
               return "Временный";
           }
              if (value == "06")
              {
               return "Учтенный";
              }
           if (value == "07")
              {
               return "Снят с учета";
              }
           if (value == "08")
              {
               return "Аннулированный";
              }
           if (value == "00")
           {
               return "00";
           }
           return "-";
       }
       */
       public static Color State2Color(string value)
       
       { 
           if (value.Contains("Item")) { value = value.Substring(4); }

           
           if (value == "01")
           {
               return Color.Green; ;
           }
           if (value == "05")
           {
               return Color.Red;
           }
           if (value == "06")
           {
               return Color.Black;
           }
           if (value == "07")
           {
               return Color.Gray;
           }
           if (value == "08")
           {
               return Color.LightGray;
           }
           return  Color.Black;
       }
     
       #region-----------------Конвертация из ОИПД КВЗУ в ОИПД Fteo.Spatial
       public static netFteo.Spatial.TMyPolygon AddEntSpatKVZU06(string Definition, RRTypes.kvzu.tEntitySpatialBordersZUOut ES)
       {
           netFteo.Spatial.TMyPolygon EntSpat = new netFteo.Spatial.TMyPolygon();
           EntSpat.Definition = Definition;
           if (ES == null) { return EntSpat; }


           //Первый (внешний) контур
           for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
           {

               netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
               Point.Status = 1;
               Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
               Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
               Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
               Point.NumGeopointA = ES.SpatialElement[0].SpelementUnit[iord].SuNmb;
               EntSpat.AddPoint(Point);
           }
           //Внутренние контура
           for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
           {
               netFteo.Spatial.TMyOutLayer InLayer = EntSpat.AddChild();
               for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
               {

                   netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
                   Point.Status = 1;
                   Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                   Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                   Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                   Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].SuNmb;
                   InLayer.AddPoint(Point);
               }
           }
           return EntSpat;
       }
       public static netFteo.Spatial.TMyPolygon AddEntSpatKVZU06(string Definition, RRTypes.kvzu07.tEntitySpatialBordersZUOut ES)
       {

           netFteo.Spatial.TMyPolygon EntSpat = new netFteo.Spatial.TMyPolygon();
           EntSpat.Definition = Definition;
           if (ES == null) { return EntSpat; }


           //Первый (внешний) контур
           for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
           {

               netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
               Point.Status = 1;
               Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
               Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
               Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
               Point.NumGeopointA = ES.SpatialElement[0].SpelementUnit[iord].SuNmb;
               EntSpat.AddPoint(Point);
           }
           //Внутренние контура
           for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
           {
               netFteo.Spatial.TMyOutLayer InLayer = EntSpat.AddChild();
               for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
               {

                   netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
                   Point.Status = 1;
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
        
    }
}
