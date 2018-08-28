using System;
using System.Linq;
using netFteo.Spatial;
using netFteo.Rosreestr;

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
        public static Object ES_OKS(string Definition, kvoks_v02.tEntitySpatialOKSOut ES)
        {
            if (ES == null) return null;
            if (ES.SpatialElement[0].SpelementUnit[0].Ordinate.X == ES.SpatialElement[0].SpelementUnit[ES.SpatialElement[0].SpelementUnit.Count() - 1].Ordinate.X)
            {
                netFteo.Spatial.TMyPolygon fES = new netFteo.Spatial.TMyPolygon();

                //OUT
                for (int ip = 0; ip <= ES.SpatialElement[0].SpelementUnit.Count - 1; ip++)
                {
                    netFteo.Spatial.Point P = new netFteo.Spatial.Point();
                    P.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.X);
                    P.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.Y);
                    P.NumGeopointA = ES.SpatialElement[0].SpelementUnit[ip].Ordinate.NumGeopoint;
                    P.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                    fES.AddPoint(P);
                }


                //childs
                for (int i = 1; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.Spatial.TMyOutLayer ESch = fES.AddChild();

                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.Spatial.Point P = new netFteo.Spatial.Point();
                        P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                        P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        ESch.AddPoint(P);
                    }

                }



                return fES;
            }
            else
            {
                netFteo.Spatial.TPolyLines PolyCollection = new netFteo.Spatial.TPolyLines(netFteo.Spatial.Gen_id.newId);
                for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.Spatial.TPolyLine line = new netFteo.Spatial.TPolyLine();
                    line.Layer_id = netFteo.Spatial.Gen_id.newId;
                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.Spatial.Point P = line.AddPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                   Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);

                    }
                    PolyCollection.Add(line);
                }
                return PolyCollection;
            }





        }
        public static Object ES_OKS(string Definition, kpoks_v03.tEntitySpatialOKSOut ES)
        {
            if (ES == null) return null;
            if (ES.SpatialElement[0].SpelementUnit[0].Ordinate.X == ES.SpatialElement[0].SpelementUnit[ES.SpatialElement[0].SpelementUnit.Count() - 1].Ordinate.X)
            {
                netFteo.Spatial.TMyPolygon fES = new netFteo.Spatial.TMyPolygon();

                //OUT
                for (int ip = 0; ip <= ES.SpatialElement[0].SpelementUnit.Count - 1; ip++)
                {
                    netFteo.Spatial.Point P = new netFteo.Spatial.Point();
                    P.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.X);
                    P.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.Y);
                    P.NumGeopointA = ES.SpatialElement[0].SpelementUnit[ip].Ordinate.NumGeopoint;
                    P.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                    fES.AddPoint(P);
                }


                //childs
                for (int i = 1; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.Spatial.TMyOutLayer ESch = fES.AddChild();

                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.Spatial.Point P = new netFteo.Spatial.Point();
                        P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                        P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        ESch.AddPoint(P);
                    }

                }



                return fES;
            }
            else
            {
                netFteo.Spatial.TPolyLines PolyCollection = new netFteo.Spatial.TPolyLines(netFteo.Spatial.Gen_id.newId);
                for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.Spatial.TPolyLine line = new netFteo.Spatial.TPolyLine();
                    line.Layer_id = netFteo.Spatial.Gen_id.newId;
                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.Spatial.Point P = line.AddPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                   Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);

                    }
                    PolyCollection.Add(line);
                }
                return PolyCollection;
            }





        }
        public static Object ES_OKS(string Definition, kpoks_v04.tEntitySpatialOKSOut ES)
        {
            if (ES == null) return null;
            if (ES.SpatialElement[0].SpelementUnit[0].Ordinate.X == ES.SpatialElement[0].SpelementUnit[ES.SpatialElement[0].SpelementUnit.Count() - 1].Ordinate.X)
            {
                netFteo.Spatial.TMyPolygon fES = new netFteo.Spatial.TMyPolygon();

                //OUT
                for (int ip = 0; ip <= ES.SpatialElement[0].SpelementUnit.Count - 1; ip++)
                {
                    netFteo.Spatial.Point P = new netFteo.Spatial.Point();
                    P.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.X);
                    P.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.Y);
                    P.NumGeopointA = ES.SpatialElement[0].SpelementUnit[ip].Ordinate.NumGeopoint;
                    P.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                    fES.AddPoint(P);
                }


                //childs
                for (int i = 1; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.Spatial.TMyOutLayer ESch = fES.AddChild();

                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.Spatial.Point P = new netFteo.Spatial.Point();
                        P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                        P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        ESch.AddPoint(P);
                    }

                }



                return fES;
            }
            else
            {
                netFteo.Spatial.TPolyLines PolyCollection = new netFteo.Spatial.TPolyLines(netFteo.Spatial.Gen_id.newId);
                for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.Spatial.TPolyLine line = new netFteo.Spatial.TPolyLine();
                    line.Layer_id = netFteo.Spatial.Gen_id.newId;
                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.Spatial.Point P = line.AddPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                   Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);

                    }
                    PolyCollection.Add(line);
                }
                return PolyCollection;
            }





        }
        public static Object ES_OKS(string Definition, kvoks_v07.tEntitySpatialOKSOut ES)
        {
            if (ES == null) return null;
            if (ES.SpatialElement[0].SpelementUnit[0].Ordinate.X == ES.SpatialElement[0].SpelementUnit[ES.SpatialElement[0].SpelementUnit.Count() - 1].Ordinate.X)
            {
                netFteo.Spatial.TMyPolygon fES = new netFteo.Spatial.TMyPolygon();

                //OUT
                for (int ip = 0; ip <= ES.SpatialElement[0].SpelementUnit.Count - 1; ip++)
                {
                    netFteo.Spatial.Point P = new netFteo.Spatial.Point();
                    P.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.X);
                    P.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.Y);
                    P.NumGeopointA = ES.SpatialElement[0].SpelementUnit[ip].Ordinate.NumGeopoint;
                    P.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                    fES.AddPoint(P);
                }


                //childs
                for (int i = 1; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.Spatial.TMyOutLayer ESch = fES.AddChild();

                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.Spatial.Point P = new netFteo.Spatial.Point();
                        P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                        P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        ESch.AddPoint(P);
                    }

                }



                return fES;
            }
            else
            {
                netFteo.Spatial.TPolyLines PolyCollection = new netFteo.Spatial.TPolyLines(netFteo.Spatial.Gen_id.newId);
                for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.Spatial.TPolyLine line = new netFteo.Spatial.TPolyLine();
                    line.Layer_id = netFteo.Spatial.Gen_id.newId;
                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.Spatial.Point P = line.AddPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                   Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);

                    }
                    PolyCollection.Add(line);
                }
                return PolyCollection;
            }





        }
        public static TMyPolygon ES_OKS(string Definition, STD_TPV02.Entity_Spatial ES)
        {
            netFteo.Spatial.TMyPolygon EntSpat = new netFteo.Spatial.TMyPolygon();
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
                EntSpat.AddPoint(Point);
            }
            //Внутренние контура
            for (int iES = 1; iES <= ES.Spatial_Element.Count - 1; iES++)
            {
                netFteo.Spatial.TMyOutLayer InLayer = EntSpat.AddChild();
                for (int iord = 0; iord <= ES.Spatial_Element[iES].Spelement_Unit.Count - 1; iord++)
                {

                    netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
                    Point.x = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].X);
                    Point.y = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].Y);
                    Point.Mt = Convert.ToDouble(ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].Delta_Geopoint);
                    Point.NumGeopointA = ES.Spatial_Element[iES].Spelement_Unit[iord].Ordinate[0].Num_Geopoint;
                    InLayer.AddPoint(Point);
                }
            }
            return EntSpat;
        }
        public static Object ES_OKS(string Definition, V03_TP.tEntitySpatialOKSInp ES)
        {
            if (ES == null) return null;
            if (ES.SpatialElement[0].SpelementUnit[0].Ordinate.X == ES.SpatialElement[0].SpelementUnit[ES.SpatialElement[0].SpelementUnit.Count() - 1].Ordinate.X)
            {
                netFteo.Spatial.TMyPolygon fES = new netFteo.Spatial.TMyPolygon();

                //OUT
                for (int ip = 0; ip <= ES.SpatialElement[0].SpelementUnit.Count - 1; ip++)
                {
                    netFteo.Spatial.Point P = new netFteo.Spatial.Point();
                    P.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.X);
                    P.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.Y);
                    P.NumGeopointA = ES.SpatialElement[0].SpelementUnit[ip].Ordinate.NumGeopoint;
                    P.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                    fES.AddPoint(P);
                }


                //childs
                for (int i = 1; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.Spatial.TMyOutLayer ESch = fES.AddChild();

                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.Spatial.Point P = new netFteo.Spatial.Point();
                        P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                        P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        ESch.AddPoint(P);
                    }
                }
                return fES;
            }
            else
            {
                netFteo.Spatial.TPolyLines PolyCollection = new netFteo.Spatial.TPolyLines(netFteo.Spatial.Gen_id.newId);
                for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.Spatial.TPolyLine line = new netFteo.Spatial.TPolyLine();
                    line.Layer_id = netFteo.Spatial.Gen_id.newId;
                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.Spatial.Point P = line.AddPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                   Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);

                    }
                    PolyCollection.Add(line);
                }
                return PolyCollection;
            }





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

            if (Address.Level3 != null)
                Adr.Level2 = Address.Level3.Type + " " + Address.Level3.Value;

            if (Address.Apartment != null)
                Adr.Apartment = Address.Apartment.Type + " " + Address.Apartment.Value;

            Adr.Region = Address.Region.ToString().Substring(4);

            return Adr;
        }

        private static string ObjectTypeToStr(string s)
        {

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

              System.Xml.XmlNodeList lst = xmldoc.DocumentElement.SelectNodes("/docns:" + RootName + "/docns:ReestrExtract/docns:ExtractObjectRight/docns:ExtractObject/docns:ObjectRight/docns:Right", nsmgr);


              netFteo.Rosreestr.TMyRights rs = new netFteo.Rosreestr.TMyRights();

              foreach (System.Xml.XmlNode cd in lst)
              {

                  netFteo.Rosreestr.TRight rt = new netFteo.Rosreestr.TRight();
                  if (cd.SelectSingleNode("docns:NoOwner", nsmgr) == null)
                  {
                      netFteo.Rosreestr.TMyOwner own = new netFteo.Rosreestr.TMyOwner("");


                      if (cd.SelectSingleNode("docns:Owner/docns:Person", nsmgr) != null)
                      {
                          own.OwnerName += cd.SelectSingleNode("docns:Owner/docns:Person/docns:FIO/docns:Surname", nsmgr).FirstChild.Value + " ";
                          own.OwnerName += cd.SelectSingleNode("docns:Owner/docns:Person/docns:FIO/docns:First", nsmgr).FirstChild.Value + " ";
                          if (cd.SelectSingleNode("docns:Owner/docns:Person/docns:FIO/docns:Patronymic", nsmgr) != null)
                              own.OwnerName += cd.SelectSingleNode("docns:Owner/docns:Person/docns:FIO/docns:Patronymic", nsmgr).FirstChild.Value + " ";
                      }

                      if (cd.SelectSingleNode("docns:Owner/docns:Organization", nsmgr) != null)
                      {
                          System.Xml.XmlNode ownrOrg = cd.SelectSingleNode("docns:Owner/docns:Organization/docns:Name", nsmgr);
                          own.OwnerName = ownrOrg.LastChild.Value;
                      }


                      if (cd.SelectSingleNode("docns:Owner/docns:Governance", nsmgr) != null)
                      {
                          System.Xml.XmlNode ownrOrg = cd.SelectSingleNode("docns:Owner/docns:Governance/docns:Name", nsmgr);
                          own.OwnerName = ownrOrg.LastChild.Value;
                      }

                      if (cd.SelectSingleNode("docns:Registration/docns:RegNumber", nsmgr) != null)
                      {
                          rt.RegNumber = cd.SelectSingleNode("docns:Registration/docns:RegNumber", nsmgr).FirstChild.Value;
                          rt.Name = cd.SelectSingleNode("docns:Registration/docns:Name", nsmgr).FirstChild.Value;
                          if (cd.SelectSingleNode("docns:Registration/docns:Desc", nsmgr) != null)
                              rt.Name = cd.SelectSingleNode("docns:Registration/docns:Desc", nsmgr).FirstChild.Value;
                          rt.RegDate = cd.SelectSingleNode("docns:Registration/docns:RegDate", nsmgr).FirstChild.Value;
                          rt.Type = cd.SelectSingleNode("docns:Registration/docns:Type", nsmgr).FirstChild.Value;
                      }
                      if (cd.SelectSingleNode("docns:Registration/docns:ShareText", nsmgr) != null)
                          rt.ShareText = cd.SelectSingleNode("docns:Registration/docns:ShareText", nsmgr).FirstChild.Value;

                      if (cd.SelectSingleNode("docns:Registration/docns:Share", nsmgr) != null) //Здесь дробь в атрибутах:
                          rt.ShareText = cd.SelectSingleNode("docns:Registration/docns:Share", nsmgr).Attributes.GetNamedItem("Numerator").Value +
                              "//" + cd.SelectSingleNode("docns:Registration/docns:Share", nsmgr).Attributes.GetNamedItem("Denominator").Value;

                      ///KVZU/ReestrExtract/ExtractObjectRight/ExtractObject/ObjectRight/Right[1461]/Registration/Share
                      rt.Owners.Add(own);
                      ///KVZU/ReestrExtract/ExtractObjectRight/ExtractObject/ObjectRight/Right[413]/Encumbrance[1]
                      /* netFteo.Rosreestr.TMyEncumbrances MyEncs = new netFteo.Rosreestr.TMyEncumbrances();           
                       * */

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

                      
                  }
                  else
                      rt.Name = cd.SelectSingleNode("docns:NoOwner", nsmgr).FirstChild.Value;
                  rs.Add(rt);
              }
              return rs;
          }

          /// <summary>
          /// Разбор node - значение первого дочернего элемента
          /// </summary>
          /// <param name="xmldoc"></param>
          /// <returns></returns>
          public static string Parse_Recipient(System.Xml.XmlDocument xmldoc)
          {
              /*
              System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
              nsmgr.AddNamespace("parseNS", xmldoc.DocumentElement.NamespaceURI);
              string docRootName = xmldoc.DocumentElement.Name;
              System.Xml.XmlNode recnode = xmldoc.DocumentElement.SelectSingleNode("/parseNS:" +
                                              docRootName +
                                              "/parseNS:ReestrExtract/parseNS:DeclarAttribute/parseNS:ReceivName", nsmgr);
              if (recnode != null)
              {
                  return recnode.FirstChild.Value;
              }
              else
                  return null;
              */
              return Parse_Node(xmldoc, "/ReestrExtract/DeclarAttribute/ReceivName");
          }
    
          
          /// <summary>
          /// Parse  /KPZU/eDocument/Sender
          /// </summary>
          /// <param name="xmldoc">Типа XMlDocument</param>
          /// <returns></returns>
          public static string Parse_SenderAppointment(System.Xml.XmlDocument xmldoc)
          {
             return RRTypes.CommonCast.CasterEGRP.Parse_Attribute(xmldoc, "Appointment", "/eDocument/Sender");
          }
   
          /// <summary>
          /// Parse any Attibute in document
          /// </summary>
          /// <param name="xmldoc">Документ</param>
          /// <param name="AttributeName">Имя атрибута</param>
          /// <param name="Xpath">xpath without root element (detect here)</param>
          /// <returns></returns>
          public static string Parse_Attribute(System.Xml.XmlDocument xmldoc, string AttributeName, string Xpath)
          {
              System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
              nsmgr.AddNamespace("parseNS", xmldoc.DocumentElement.NamespaceURI);
              string docRootName = xmldoc.DocumentElement.Name;
              string testXpath = "/parseNS:" + docRootName + Xpath.Replace("/", "/parseNS:"); //replace to '/parseNS:ReestrExtract/parseNS:DeclarAttribute'
              System.Xml.XmlNode recnode = xmldoc.DocumentElement.SelectSingleNode(testXpath, nsmgr);
              if (recnode != null)
              {
                  return recnode.Attributes.GetNamedItem(AttributeName).Value;
              }
              else
                  return null;
          }

          public static string Parse_Node(System.Xml.XmlDocument xmldoc, string Xpath)
          {
              System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
              nsmgr.AddNamespace("parseNS", xmldoc.DocumentElement.NamespaceURI);
              string docRootName = xmldoc.DocumentElement.Name;
              string testXpath = "/parseNS:" + docRootName + Xpath.Replace("/", "/parseNS:"); //replace to '/parseNS:ReestrExtract/parseNS:DeclarAttribute'
              System.Xml.XmlNode recnode = xmldoc.DocumentElement.SelectSingleNode(testXpath, nsmgr);
              if (recnode != null)
              {
                  return recnode.FirstChild.Value;
              }
              else
                  return null;
          }
          
          /// <summary>
          /// Разбор ветви ReestrExtract в документах Росреестра - дописочки от
          /// регистраторов, содержащей сведения о документе
          /// </summary>
          /// <param name="xmldoc"></param>
          /// <param name="res"></param>
          public static void Parse_ReestrExtract(System.Xml.XmlDocument xmldoc,  netFteo.XML.FileInfo res)
          {
              //tabPage1.Text = "Земельные участки";
              res.DocType = Parse_Attribute(xmldoc, "ExtractTypeText", "/ReestrExtract/DeclarAttribute");
              res.Version = Parse_Attribute(xmldoc, "Version", "/eDocument");
              res.Number  = Parse_Attribute(xmldoc, "ExtractNumber", "/ReestrExtract/DeclarAttribute");
              res.Date    = Parse_Attribute(xmldoc, "ExtractDate", "/ReestrExtract/DeclarAttribute");
              res.RequeryNumber = Parse_Attribute(xmldoc, "RequeryNumber", "/ReestrExtract/DeclarAttribute");
              res.Cert_Doc_Organization = Parse_Attribute(xmldoc, "Name", "/eDocument/Sender");
              res.Appointment = Parse_Attribute(xmldoc, "Appointment", "/eDocument/Sender");
              res.AppointmentFIO = Parse_Attribute(xmldoc, "Registrator", "/ReestrExtract/DeclarAttribute");
              res.ReceivName =     Parse_Node(xmldoc, "/ReestrExtract/DeclarAttribute/ReceivName");
              res.ReceivAdress =   Parse_Node(xmldoc, "/ReestrExtract/DeclarAttribute/ReceivAdress");
          }

      }
    
    
    /*
    public class TAddress2
    {
        public static explicit operator TAddress2(kvzu07.tAddressOut v)
        {
            TAddress adr = new TAddress();
            adr.Apartment = v.Apartment.Value;
            adr.Note = v.Note;
            return adr;
            //throw new NotImplementedException();
        }
    }
    */
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
          public static TAddress CastAdresss(System.Xml.XmlNodeList Adress)
        {
            ///KVZU/Parcels/Parcel/Location/Address
            return null;
        }

          public static TLocation CastLocation(kvzu07.tLocation location)
          {

              if (location  == null) return null;
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
              if (location.inBounds != null)
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
              if (location.inBounds != null)
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
              if (location.inBounds != null)
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
              if (location.inBounds != null)
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
              if (location.inBounds != null)
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
              { int test = 0; }
              if (location.inBounds != null)
                  loc.Inbounds = location.inBounds.ToString();

              return loc;
          }
          /// <summary>
          /// ES manipulation routines
          /// </summary>
          /// <param name="Address"></param>
          /// <returns></returns>
          /// 

          public static netFteo.Spatial.TMyPolygon AddEntSpatMP5(string Definition, MP_V05.tEntitySpatialBordersZUInp ES)
          {
              netFteo.Spatial.TMyPolygon EntSpat = new netFteo.Spatial.TMyPolygon();
              EntSpat.Definition = Definition;



              //Первый (внешний) контур
              for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
              {

                  netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
                  Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                  Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                  Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                  Point.Description = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.GeopointZacrep;
                  Point.Pref = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.PointPref;
                  Point.NumGeopointA = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.NumGeopoint;
                  EntSpat.AddPoint(Point);
              }
              //Внутренние контура
              for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
              {
                  netFteo.Spatial.TMyOutLayer InLayer = EntSpat.AddChild();
                  for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                  {

                      netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
                      Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                      Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                      Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                      Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.NumGeopoint;
                      InLayer.AddPoint(Point);
                  }
              }

              return EntSpat;
          }
          public static netFteo.Spatial.TMyPolygon AddEntSpatMP5(string Definition, MP_V05.tEntitySpatialZUInp ES)
          {
              netFteo.Spatial.TMyPolygon EntSpat = new netFteo.Spatial.TMyPolygon();
              EntSpat.Definition = Definition;



              //Первый (внешний) контур
              for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
              {

                  netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
                  Point.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.X);
                  Point.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.Y);
                  Point.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                  Point.Description = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.GeopointZacrep;
                  Point.Pref = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.PointPref;
                  Point.NumGeopointA = ES.SpatialElement[0].SpelementUnit[iord].Ordinate.NumGeopoint;
                  EntSpat.AddPoint(Point);
              }
              //Внутренние контура
              for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
              {
                  netFteo.Spatial.TMyOutLayer InLayer = EntSpat.AddChild();
                  for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                  {

                      netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
                      Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                      Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                      Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                      Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.NumGeopoint;
                      InLayer.AddPoint(Point);
                  }
              }

              return EntSpat;
          }
          public static netFteo.Spatial.TMyPolygon AddEntSpatMP5(string Definition, MP_V05.tEntitySpatialOldNew ES)
          {
              netFteo.Spatial.TMyPolygon EntSpat = new netFteo.Spatial.TMyPolygon();
              EntSpat.Definition = Definition;



              //Первый (внешний) контур
              for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
              {

                  netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
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
                  netFteo.Spatial.TMyOutLayer InLayer = EntSpat.AddChild();
                  for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                  {

                      netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
                      Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].NewOrdinate.X);
                      Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].NewOrdinate.Y);
                      Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].NewOrdinate.DeltaGeopoint);
                      Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].NewOrdinate.NumGeopoint;
                      InLayer.AddPoint(Point);
                  }
              }

              return EntSpat;
          }
          public static netFteo.Spatial.TMyPolygon AddEntSpatMP5(string Definition, MP_V05.tContoursSubParcelContour ES)
          {

              netFteo.Spatial.TMyPolygon EntSpat = new netFteo.Spatial.TMyPolygon();
              EntSpat.Definition = Definition;



              //Первый (внешний) контур
              for (int iord = 0; iord <= ES.EntitySpatial.SpatialElement[0].SpelementUnit.Count - 1; iord++)
              {

                  netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
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
                  netFteo.Spatial.TMyOutLayer InLayer = EntSpat.AddChild();
                  for (int iord = 0; iord <= ES.EntitySpatial.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                  {

                      netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
                      Point.x = Convert.ToDouble(ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                      Point.y = Convert.ToDouble(ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                      Point.Mt = Convert.ToDouble(ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                      Point.NumGeopointA = ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.NumGeopoint;
                      InLayer.AddPoint(Point);
                  }
              }

              return EntSpat;
          }
          public static netFteo.Spatial.TPolygonCollection AddContoursMP5(string Definition, MP_V05.tContoursSubParcelContourCollection cs)
          {
              netFteo.Spatial.TPolygonCollection res = new netFteo.Spatial.TPolygonCollection();
              foreach (MP_V05.tContoursSubParcelContour item in cs)
                  res.AddPolygon(AddEntSpatMP5(item.Number, item));
              return res;
          }
          public static TMyPolygon ES_ZU(string Definition, MP_V06.tEntitySpatialBordersZUOut ES)
          {
              /*
              netFteo.Spatial.TMyPolygon EntSpat = new netFteo.Spatial.TMyPolygon();
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
                  netFteo.Spatial.TMyOutLayer InLayer = EntSpat.AddChild();
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
          public static TMyPolygon ES_ZU(string Definition, MP_V06.tEntitySpatialZUInp ES)
          {

              netFteo.Spatial.TMyPolygon EntSpat = new netFteo.Spatial.TMyPolygon();
              EntSpat.Definition = Definition;
              //Первый (внешний) контур
              for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
              {

                  netFteo.Spatial.Point Point = new netFteo.Spatial.Point();

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
                  netFteo.Spatial.TMyOutLayer InLayer = EntSpat.AddChild();
                  for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                  {

                      netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
                      Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                      Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                      Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                      Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.NumGeopoint;
                      InLayer.AddPoint(Point);
                  }
              }
              return EntSpat;

              return null;
          }/// 

          public static TMyPolygon ES_ZU(string Definition, MP_V06.tEntitySpatialBordersZUInp ES)
          {

              netFteo.Spatial.TMyPolygon EntSpat = new netFteo.Spatial.TMyPolygon();
              EntSpat.Definition = Definition;
              //Первый (внешний) контур
              for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
              {

                  netFteo.Spatial.Point Point = new netFteo.Spatial.Point();

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
                  netFteo.Spatial.TMyOutLayer InLayer = EntSpat.AddChild();
                  for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                  {

                      netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
                      Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                      Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                      Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                      Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].Ordinate.NumGeopoint;
                      InLayer.AddPoint(Point);
                  }
              }
              return EntSpat;

              return null;
          }

          public static TPolygonCollection ES_ZU(MP_V06.tNewContourCollection ESs)
          {
              TPolygonCollection res = new netFteo.Spatial.TPolygonCollection();
              foreach (MP_V06.tNewContour item in ESs)
                  res.AddPolygon(ES_ZU(item.Definition, item.EntitySpatial));
              return res;
          }

     /// <summary>
     /// Разбор юнита (например  - Точки) 
     /// </summary>
     /// <param name="unit"></param>
     /// <returns></returns>
        private static Point GetUnit(MP_V06.tSpelementUnitOldNew unit)
        {

            netFteo.Spatial.Point Point = new netFteo.Spatial.Point();

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

                Point.x = Convert.ToDouble(unit.NewOrdinate.X);
                Point.y = Convert.ToDouble(unit.NewOrdinate.Y);
                Point.Mt = Convert.ToDouble(unit.NewOrdinate.DeltaGeopoint);
                //Point.Description = ES.SpatialElement[0].Spelement_Unit[iord].Ordinate[0].Geopoint_Zacrep;
                Point.Pref = unit.NewOrdinate.PointPref;
                Point.NumGeopointA = unit.NewOrdinate.NumGeopoint;
            }

            // " л "     TODO:
            //Если указаны только старая - точка ликвидируется. И это грабли всего дерева классов
            // Точка имеет два набора координат - фактически две границы - существующую в ЕГРН и новую
            if ((unit.OldOrdinate != null) &&    (unit.NewOrdinate == null))
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
        public static TMyPolygon ES_ZU(string Definition, MP_V06.tEntitySpatialOldNew ES)
          {
              netFteo.Spatial.TMyPolygon EntSpat = new netFteo.Spatial.TMyPolygon();
              EntSpat.Definition = Definition;

              //Первый (внешний) контур
              for (int iord = 0; iord <= ES.SpatialElement[0].SpelementUnit.Count - 1; iord++)
              {  if ((ES.SpatialElement[0].SpelementUnit[iord]).NewOrdinate != null) // только точки с новыми/уточняемымыи/сущесствующими коорд
                  EntSpat.AddPoint(GetUnit(ES.SpatialElement[0].SpelementUnit[iord]));
              }

              //Внутренние контура
              for (int iES = 1; iES <= ES.SpatialElement.Count - 1; iES++)
              {
                  netFteo.Spatial.TMyOutLayer InLayer = EntSpat.AddChild();

                  for (int iord = 0; iord <= ES.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                  {
                    /*

                      netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
                      if (ES.SpatialElement[iES].SpelementUnit[iord].NewOrdinate != null)
                      {
                          Point.x = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].NewOrdinate.X);
                          Point.y = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].NewOrdinate.Y);
                          Point.Mt = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].NewOrdinate.DeltaGeopoint);
                          Point.NumGeopointA = ES.SpatialElement[iES].SpelementUnit[iord].NewOrdinate.NumGeopoint;
                      }
                      else // указана только OldOrdinate - точка ликвидируется
                           //это "л", имя берем из старой ординаты
                      {
                          Point.NumGeopointA = "л " + ES.SpatialElement[iES].SpelementUnit[iord].OldOrdinate.NumGeopoint;
                          Point.Status = 6;
                      }

                      if (ES.SpatialElement[iES].SpelementUnit[iord].OldOrdinate != null)
                      {
                          Point.oldX = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].OldOrdinate.X);
                          Point.oldY = Convert.ToDouble(ES.SpatialElement[iES].SpelementUnit[iord].OldOrdinate.Y);
                      }
                      */
                      if ((ES.SpatialElement[iES].SpelementUnit[iord]).NewOrdinate != null) // только точки с новыми/уточняемымыи/сущесствующими коорд
                        InLayer.AddPoint(GetUnit(ES.SpatialElement[iES].SpelementUnit[iord]));
                  }
              }

              if (EntSpat.HasChanges == "*") EntSpat.State = 0; else EntSpat.State = 4;
              return EntSpat;
          }

          public static TMyPolygon ES_ZU(string Definition, MP_V06.tContoursSubParcelContour ES)
          {

              netFteo.Spatial.TMyPolygon EntSpat = new netFteo.Spatial.TMyPolygon();
              EntSpat.Definition = Definition;
              //Первый (внешний) контур
              for (int iord = 0; iord <= ES.EntitySpatial.SpatialElement[0].SpelementUnit.Count - 1; iord++)
              {

                  netFteo.Spatial.Point Point = new netFteo.Spatial.Point();

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
                  netFteo.Spatial.TMyOutLayer InLayer = EntSpat.AddChild();
                  for (int iord = 0; iord <= ES.EntitySpatial.SpatialElement[iES].SpelementUnit.Count - 1; iord++)
                  {

                      netFteo.Spatial.Point Point = new netFteo.Spatial.Point();
                      Point.x = Convert.ToDouble(ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.X);
                      Point.y = Convert.ToDouble(ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.Y);
                      Point.Mt = Convert.ToDouble(ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.DeltaGeopoint);
                      Point.NumGeopointA = ES.EntitySpatial.SpatialElement[iES].SpelementUnit[iord].Ordinate.NumGeopoint;
                      InLayer.AddPoint(Point);
                  }
              }
              return EntSpat;

              return null;
          }
          public static TPolygonCollection ES_SubParcels(MP_V06.tContoursSubParcelContourCollection cs)
          {
              TPolygonCollection res = new netFteo.Spatial.TPolygonCollection();
              foreach (MP_V06.tContoursSubParcelContour item in cs)
                  res.AddPolygon(ES_ZU(item.Number, item));
              return res;
          }


          
      }
    
    
}
namespace RRTypes.CommonParsers
{
    public class Doc2Type
    {
        /// <summary>
        /// Десериализатор для любых типов
        /// </summary>
        /// <typeparam name="T">Целевой тип "<T>"</typeparam>
        /// <param name="xmldoc">Исходный xml документ</param>
        /// <returns></returns>
        private object Desearialize<T>(System.Xml.XmlDocument xmldoc)
        {
            System.IO.Stream stream = new System.IO.MemoryStream();
            xmldoc.Save(stream);
            stream.Seek(0, 0);
            System.Xml.Serialization.XmlSerializer serializerKPT = new System.Xml.Serialization.XmlSerializer(typeof(T));
            return (T)serializerKPT.Deserialize(stream);
        }

        private netFteo.XML.FileInfo InitFileInfo(netFteo.XML.FileInfo fi, System.Xml.XmlDocument xmldoc)
        {
            netFteo.XML.FileInfo res = new netFteo.XML.FileInfo();
            res.FileName = fi.FileName;
            res.FilePath = fi.FilePath;
            res.DocRootName = xmldoc.DocumentElement.Name;
            res.Namespace = xmldoc.DocumentElement.NamespaceURI;
            if (xmldoc.DocumentElement.Attributes.GetNamedItem("Version") != null) // Для MP версия в корне
                res.Version = xmldoc.DocumentElement.Attributes.GetNamedItem("Version").Value;
            return res;
        }

        #region  Разбор MP 06
        public netFteo.XML.FileInfo ParseMPV06(netFteo.XML.FileInfo fi,System.Xml.XmlDocument xmldoc)
        {
            netFteo.XML.FileInfo res = InitFileInfo(fi, xmldoc);

            RRTypes.MP_V06.MP MP = (RRTypes.MP_V06.MP)Desearialize<RRTypes.MP_V06.MP>(xmldoc);

            for (int i = 0; i <= MP.CoordSystems.Count - 1; i++)
            {
                res.MyBlocks.CSs.Add(new TCoordSystem(MP.CoordSystems[i].Name, MP.CoordSystems[i].CsId));

            }
            TMyCadastralBlock Bl = new TMyCadastralBlock();
            string ParcelName;
            //МП по образованию
            if (MP.Package.FormParcels != null)
            {

                for (int i = 0; i <= MP.Package.FormParcels.NewParcel.Count - 1; i++)
                {
                    TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel());
                    MainObj.CadastralBlock = MP.Package.FormParcels.NewParcel[i].CadastralBlock;
                    Bl.CN = MP.Package.FormParcels.NewParcel[i].CadastralBlock;
                    MainObj.Definition = MP.Package.FormParcels.NewParcel[i].Definition;
                    MainObj.PrevCadastralNumbers.AddRange(MP.Package.FormParcels.NewParcel[i].PrevCadastralNumbers);
                    MainObj.AreaGKN = MP.Package.FormParcels.NewParcel[i].Area.Area;//Вычисленную??
                    MainObj.Category = netFteo.Rosreestr.dCategoriesv01.ItemToName(MP.Package.FormParcels.NewParcel[i].Category.Category.ToString());
                    MainObj.Location.Address = RRTypes.CommonCast.CasterZU.CastAddress(MP.Package.FormParcels.NewParcel[i].Address);
                    if (MP.Package.FormParcels.NewParcel[i].Utilization != null)
                        MainObj.Utilization.UtilbyDoc = MP.Package.FormParcels.NewParcel[i].Utilization.ByDoc;
                    if (MP.Package.FormParcels.NewParcel[i].LandUse != null)
                        MainObj.Landuse.Land_Use = MP.Package.FormParcels.NewParcel[i].LandUse.LandUse.ToString();
                    if (MP.Package.FormParcels.NewParcel[i].Contours != null & MP.Package.FormParcels.NewParcel[i].Contours.Count > 0)
                        MainObj.Contours = RRTypes.CommonCast.CasterZU.ES_ZU(MP.Package.FormParcels.NewParcel[i].Contours);
                    if (MP.Package.FormParcels.NewParcel[i].EntitySpatial != null)
                        MainObj.EntitySpatial = RRTypes.CommonCast.CasterZU.ES_ZU("", MP.Package.FormParcels.NewParcel[i].EntitySpatial);
                }
                //измененный зу
                if (MP.Package.FormParcels.ChangeParcel != null)
                {
                    foreach (RRTypes.MP_V06.tChangeParcel chzSrc in MP.Package.FormParcels.ChangeParcel)
                    {
                        TMyParcel chzObj = Bl.Parcels.AddParcel(new TMyParcel(chzSrc.CadastralNumber));
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


                    TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(MP.Package.SpecifyParcel.ExistParcel.CadastralNumber, ParcelName));
                    Bl.CN = MP.Package.SpecifyParcel.ExistParcel.CadastralBlock;
                    MainObj.AreaGKN = MP.Package.SpecifyParcel.ExistParcel.AreaInGKN;
                    MainObj.AreaValue = MP.Package.SpecifyParcel.ExistParcel.Area.Area; //Указанная площадь??
                    if (MP.Package.SpecifyParcel.ExistParcel.ObjectRealty != null)
                        MainObj.InnerCadastralNumbers.AddRange(MP.Package.SpecifyParcel.ExistParcel.ObjectRealty.InnerCadastralNumbers);
                    if (MP.Package.SpecifyParcel.ExistParcel.Contours != null)
                    {
                        for (int ic = 0; ic <= MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour.Count - 1; ic++)
                        {
                            TMyPolygon NewCont = MainObj.Contours.AddPolygon(RRTypes.CommonCast.CasterZU.ES_ZU(MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour[ic].Definition,
                                                                            MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour[ic].EntitySpatial));
                        }
                        for (int ic = 0; ic <= MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour.Count - 1; ic++)
                        {
                            TMyPolygon NewCont = MainObj.Contours.AddPolygon(RRTypes.CommonCast.CasterZU.ES_ZU(MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour[ic].NumberRecord,
                                                                            MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour[ic].EntitySpatial));
                        }
                    }

                    if (MP.Package.SpecifyParcel.ExistParcel.EntitySpatial != null)
                        MainObj.EntitySpatial.ImportPolygon(RRTypes.CommonCast.CasterZU.ES_ZU("", MP.Package.SpecifyParcel.ExistParcel.EntitySpatial));

                }

                // Уточнение ЕЗП
                if (MP.Package.SpecifyParcel.ExistEZ != null)
                {
                    res.DocTypeNick = "Уточнение ЗУ";
                    ParcelName = "Item02"; // 02 = ЕЗП RRCommon.cs,  там есть public static class dParcelsv01
                    TMyParcel MainObj = new TMyParcel(MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.CadastralNumber, ParcelName);
                    Bl.Parcels.AddParcel(MainObj);
                    MainObj.AreaGKN = MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.AreaInGKN;
                    MainObj.AreaValue = MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.Area.Area;
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

                        MainObj.CompozitionEZ.AddEntry(entry.CadastralNumber,
                                                       entry.Area.Area,
                                                      (RRTypes.CommonCast.CasterZU.ES_ZU(entry.CadastralNumber, entry.EntitySpatial)));
                    }
                }
            }
            //Только образование частей 
            if (MP.Package.SubParcels != null)
            {
                ParcelName = "Item06";  // Значение отсутствует
                TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(MP.Package.SubParcels.CadastralNumberParcel, ParcelName));
                if (MP.Package.SubParcels.NewSubParcel.Count > 0)
                    for (int ii = 0; ii <= MP.Package.SubParcels.NewSubParcel.Count - 1; ii++)
                    {
                        TmySlot Sl = new TmySlot();
                        Sl.NumberRecord = MP.Package.SubParcels.NewSubParcel[ii].Definition;
                        Sl.Encumbrance.Name = MP.Package.SubParcels.NewSubParcel[ii].Encumbrance.Name;
                        Sl.AreaGKN = MP.Package.SubParcels.NewSubParcel[ii].Area.Area;
                        if (MP.Package.SubParcels.NewSubParcel[ii].EntitySpatial != null) //Если одноконтурная чзу
                            Sl.EntSpat.ImportPolygon(RRTypes.CommonCast.CasterZU.ES_ZU(MP.Package.SubParcels.NewSubParcel[ii].Definition, MP.Package.SubParcels.NewSubParcel[ii].EntitySpatial));
                        if (MP.Package.SubParcels.NewSubParcel[ii].Contours != null)
                            Sl.Contours = RRTypes.CommonCast.CasterZU.ES_SubParcels(MP.Package.SubParcels.NewSubParcel[ii].Contours);
                        MainObj.SubParcels.Add(Sl);
                    }
            }
            res.MyBlocks.Blocks.Add(Bl);
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
                res.Comments += "<br>" +
                                       MP.GeneralCadastralWorks.DateCadastral.ToString("dd.MM.yyyy") + "<br>" +
                                       MP.GeneralCadastralWorks.Contractor.FamilyName + " " +
                                       MP.GeneralCadastralWorks.Contractor.FirstName + " " +
                                       MP.GeneralCadastralWorks.Contractor.Patronymic+ "<br>";
            }
            res.CommentsType = "Заключение КИ";
            res.DocType = "Межевой план";
            res.DocTypeNick = "MP";
            res.Number = MP.GUID;
            res.Date = MP.GeneralCadastralWorks.DateCadastral.ToString("dd.MM.yyyy");
            if (MP.GeneralCadastralWorks.Contractor.Organization != null)
            {
                res.Cert_Doc_Organization = MP.GeneralCadastralWorks.Contractor.Organization.Name +
                                  "  " + MP.GeneralCadastralWorks.Contractor.Organization.AddressOrganization;
            }

                res.Appointment = MP.GeneralCadastralWorks.Contractor.Email + " " +
                           MP.GeneralCadastralWorks.Contractor.NCertificate + " " +
                           MP.GeneralCadastralWorks.Contractor.Telephone;
              res.AppointmentFIO = MP.GeneralCadastralWorks.Contractor.FamilyName + " " +
                               MP.GeneralCadastralWorks.Contractor.FirstName + " " +
                               MP.GeneralCadastralWorks.Contractor.Patronymic;
            return res;
        }
        #endregion
      


        #region  Разбор TP 03

        private void ParseGeneralCadastralWorks(netFteo.XML.FileInfo fi, RRTypes.V03_TP.tGeneralCadastralWorks GW, string Conclusion )
        {
            fi.Date = GW.DateCadastral.ToString("dd.MM.yyyy").Replace("0:00:00", "");
            fi.AppointmentFIO = GW.Contractor.FamilyName +
                                 " " + GW.Contractor.FirstName +
                                 " " + GW.Contractor.Patronymic +
                                 "\n" + GW.Contractor.NCertificate;
            fi.Appointment = GW.Contractor.Email + " " +
                           GW.Contractor.NCertificate + " " +     GW.Contractor.Telephone;
            fi.Appointment += "\n "+GW.Contractor.Address;

            if (GW.Contractor.Organization != null)
                fi.Cert_Doc_Organization = GW.Contractor.Organization.Name + " \n" +
                                        GW.Contractor.Organization.AddressOrganization;


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
                fi.Comments += "<br>" +
                                       GW.DateCadastral.ToString("dd.MM.yyyy") + "<br>" +
                                       GW.Contractor.FamilyName + " " +
                                       GW.Contractor.FirstName + " " +
                                       GW.Contractor.Patronymic+ "<br>";
                
            }

        }


        public netFteo.XML.FileInfo ParseTP_V03(netFteo.XML.FileInfo fi, System.Xml.XmlDocument xmldoc)//RRTypes.V03_TP.TP TP)
        {
            netFteo.XML.FileInfo res = InitFileInfo(fi, xmldoc);
            RRTypes.V03_TP.TP TP = (RRTypes.V03_TP.TP)Desearialize<RRTypes.V03_TP.TP>(xmldoc);
            res.DocType = "Технический план v3";
            res.DocTypeNick = "ТП";
            res.Version = "v03";
            res.Number = TP.GUID;
            res.CommentsType = "Заключение КИ";

            if (TP.Building != null)
            {
                ParseGeneralCadastralWorks(res, TP.Building.GeneralCadastralWorks, TP.Building.Conclusion);
                res.MyBlocks.CSs.Add(new TCoordSystem(TP.Building.CoordSystems[0].Name, TP.Building.CoordSystems[0].CsId));
                TMyCadastralBlock Bl = new TMyCadastralBlock();
                Bl.CN = "ТП v3";
                TMyRealty OKS = null;
                //Здание, постановка на ГКУ
                if (TP.Building.Package.NewBuildings != null)
                {
                    if (TP.Building.Package.NewBuildings.Count == 1)
                    {
                        OKS = new TMyRealty("Здание", netFteo.Rosreestr.dRealty_v03.Здание);
                        OKS.Name = TP.Building.Package.NewBuildings[0].Name;
                        OKS.Address = RRTypes.CommonCast.CasterOKS.CastAddress(TP.Building.Package.NewBuildings[0].Address);
                        OKS.CadastralBlock = TP.Building.Package.NewBuildings[0].CadastralBlocks[0];
                        OKS.ParentCadastralNumbers.AddRange(TP.Building.Package.NewBuildings[0].ParentCadastralNumbers);
                        OKS.Building.Area = TP.Building.Package.NewBuildings[0].Area;
                        OKS.Building.AssignationBuilding = TP.Building.Package.NewBuildings[0].AssignationBuilding.ToString(); ;
                        OKS.Building.ES = RRTypes.CommonCast.CasterOKS.ES_OKS("", TP.Building.Package.NewBuildings[0].EntitySpatial);
                    }
                }
                //Многоэтажный жилой дом
                if (TP.Building.Package.NewApartHouse != null)
                {
                    OKS = new TMyRealty(TP.Building.Package.NewApartHouse.NewBuilding.Name, netFteo.Rosreestr.dRealty_v03.Здание);
                    OKS.Name = TP.Building.Package.NewApartHouse.NewBuilding.Name;
                    OKS.Address = RRTypes.CommonCast.CasterOKS.CastAddress(TP.Building.Package.NewApartHouse.NewBuilding.Address);
                    OKS.CadastralBlock = TP.Building.Package.NewApartHouse.NewBuilding.CadastralBlocks[0];
                    OKS.ParentCadastralNumbers.AddRange(TP.Building.Package.NewApartHouse.NewBuilding.ParentCadastralNumbers);
                    OKS.Building.Area = TP.Building.Package.NewApartHouse.NewBuilding.Area;
                    OKS.Building.AssignationBuilding = TP.Building.Package.NewApartHouse.NewBuilding.AssignationBuilding.ToString();
                    OKS.Building.ES = RRTypes.CommonCast.CasterOKS.ES_OKS("", TP.Building.Package.NewApartHouse.NewBuilding.EntitySpatial);
                    if (TP.Building.Package.NewApartHouse.Flats.Count > 0)
                    {
                        for (int i = 0; i <= TP.Building.Package.NewApartHouse.Flats.Count - 1; i++)
                        {
                            TFlat Flat = new TFlat(TP.Building.Package.NewApartHouse.Flats[i].PositionInObject.Levels[0].Position.NumberOnPlan);
                            TLevel lvl = new TLevel(
                                TP.Building.Package.NewApartHouse.Flats[i].PositionInObject.Levels[0].Type.ToString(),
                                TP.Building.Package.NewApartHouse.Flats[i].PositionInObject.Levels[0].Number,
                                TP.Building.Package.NewApartHouse.Flats[i].PositionInObject.Levels[0].Position.NumberOnPlan);
                            Flat.AssignationCode = TP.Building.Package.NewApartHouse.Flats[i].Assignation.AssignationCode.ToString();
                            if (TP.Building.Package.NewApartHouse.Flats[i].Assignation.AssignationTypeSpecified)
                                Flat.AssignationType = TP.Building.Package.NewApartHouse.Flats[i].Assignation.AssignationType.ToString();
                            foreach (RRTypes.V03_TP.tPlanJPG jpegname in TP.Building.Package.NewApartHouse.Flats[i].PositionInObject.Levels[0].Position.Plans)
                                lvl.AddPlan(jpegname.Name);
                            Flat.PositionInObject.Levels.Add(lvl);
                            Flat.Address = RRTypes.CommonCast.CasterOKS.CastAddress(TP.Building.Package.NewApartHouse.Flats[i].Address);
                            Flat.Area = TP.Building.Package.NewApartHouse.Flats[i].Area;
                            OKS.Building.Flats.Add(Flat);
                        }
                    }
                }



                //Здание, изменение ОКС
                if (TP.Building.Package.ExistBuilding != null)
                {
                    OKS = new TMyRealty("Здание", netFteo.Rosreestr.dRealty_v03.Здание);
                    OKS.Name = TP.Building.Package.ExistBuilding.Name;
                    OKS.Address = RRTypes.CommonCast.CasterOKS.CastAddress(TP.Building.Package.ExistBuilding.Address);
                    OKS.CadastralBlock = TP.Building.Package.ExistBuilding.CadastralBlocks[0];
                    OKS.ParentCadastralNumbers.AddRange(TP.Building.Package.ExistBuilding.ParentCadastralNumbers);
                    OKS.Building.Area = TP.Building.Package.ExistBuilding.Area;
                    if (TP.Building.Package.ExistBuilding.AssignationBuildingSpecified)
                        OKS.Building.AssignationBuilding = TP.Building.Package.ExistBuilding.AssignationBuilding.ToString();
                    OKS.Building.ES = RRTypes.CommonCast.CasterOKS.ES_OKS("", TP.Building.Package.ExistBuilding.EntitySpatial);
                }

                Bl.CN = OKS.CadastralBlock;
                Bl.AddOKS(OKS);
                res.MyBlocks.Blocks.Add(Bl);
            }
            //construction:
            if (TP.Construction != null)
            {
                ParseGeneralCadastralWorks(res, TP.Construction.GeneralCadastralWorks, TP.Construction.Conclusion);
                res.MyBlocks.CSs.Add(new TCoordSystem(TP.Construction.CoordSystems[0].Name, TP.Construction.CoordSystems[0].CsId));
                TMyCadastralBlock Bl = new TMyCadastralBlock();
                Bl.CN = "ТП v3";
                TMyRealty OKS = null;
                //Здание, постановка на ГКУ
                if (TP.Construction.Package.NewConstructions != null)
                {
                    if (TP.Construction.Package.NewConstructions.Count == 1)
                    {
                        OKS = new TMyRealty("Сооружение", netFteo.Rosreestr.dRealty_v03.Сооружение);
                        OKS.Name = TP.Construction.Package.NewConstructions[0].Name;
                        OKS.Address = RRTypes.CommonCast.CasterOKS.CastAddress(TP.Construction.Package.NewConstructions[0].Address);
                        OKS.CadastralBlock = TP.Construction.Package.NewConstructions[0].CadastralBlocks[0];
                        OKS.ParentCadastralNumbers.AddRange(TP.Construction.Package.NewConstructions[0].ParentCadastralNumbers);
                        foreach (RRTypes.V03_TP.tKeyParameter param in TP.Construction.Package.NewConstructions[0].KeyParameters)
                            OKS.Construction.KeyParameters.AddParameter(param.Type.ToString(), param.Value.ToString());
                        OKS.Construction.AssignationName = TP.Construction.Package.NewConstructions[0].AssignationName; ;
                        OKS.Construction.ES = RRTypes.CommonCast.CasterOKS.ES_OKS("", TP.Construction.Package.NewConstructions[0].EntitySpatial);
                    }
                }




                //изменение
                if (TP.Construction.Package.ExistConstruction != null)
                {
                    OKS = new TMyRealty("Сооружение", netFteo.Rosreestr.dRealty_v03.Сооружение);
                    OKS.Name = TP.Construction.Package.ExistConstruction.Name;
                    OKS.Address = RRTypes.CommonCast.CasterOKS.CastAddress(TP.Construction.Package.ExistConstruction.Address);
                    OKS.CadastralBlock = TP.Construction.Package.ExistConstruction.CadastralBlocks[0];
                    OKS.ParentCadastralNumbers.AddRange(TP.Construction.Package.ExistConstruction.ParentCadastralNumbers);
                    foreach (RRTypes.V03_TP.tKeyParameter param in TP.Construction.Package.ExistConstruction.KeyParameters)
                        OKS.Construction.KeyParameters.AddParameter(param.Type.ToString(), param.Value.ToString());
                    if (TP.Construction.Package.ExistConstruction.AssignationName != null)
                        OKS.Construction.AssignationName = TP.Construction.Package.ExistConstruction.AssignationName;
                    OKS.Construction.ES = RRTypes.CommonCast.CasterOKS.ES_OKS("", TP.Construction.Package.ExistConstruction.EntitySpatial);
                }

                Bl.CN = OKS.CadastralBlock;
                Bl.AddOKS(OKS);
                res.MyBlocks.Blocks.Add(Bl);
            }
            // end construction



            if (TP.Flat != null)
            {
                ParseGeneralCadastralWorks(res, TP.Flat.GeneralCadastralWorks, TP.Flat.Conclusion);
                if (
                TP.Flat.Package.ExistFlat != null)
                {
                    TMyCadastralBlock Bl = new TMyCadastralBlock();
                    Bl.CN = "ТП v3";
                    TMyRealty OKS = null;
                    //Помещение, учет изменений ГКУ
                    OKS = new TMyRealty(TP.Flat.Package.ExistFlat.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Помещение);
                    //OKS.Name = TP.Flat.Package.ExistFlat......;
                    OKS.Address = RRTypes.CommonCast.CasterOKS.CastAddress(TP.Flat.Package.ExistFlat.Address);
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
                    res.MyBlocks.Blocks.Add(Bl);
                }



                if (TP.Flat.Package.NewFlats != null)
                {
                    TMyCadastralBlock Bl = new TMyCadastralBlock();
                    Bl.CN = "ТП v3";
                    TMyRealty OKS = null;
                    foreach (RRTypes.V03_TP.tNewFlat fl in TP.Flat.Package.NewFlats)
                    {
                        OKS = new TMyRealty("Здание", netFteo.Rosreestr.dRealty_v03.Здание);
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
                        Flat.Address = RRTypes.CommonCast.CasterOKS.CastAddress(fl.Address);
                        Flat.Area = fl.Area;
                        OKS.Building.Flats.Add(Flat);
                        Bl.AddOKS(OKS);
                    }

                    res.MyBlocks.Blocks.Add(Bl);
                }
            }

            if (TP.Uncompleted != null)
            {
                ParseGeneralCadastralWorks(res, TP.Uncompleted.GeneralCadastralWorks, TP.Uncompleted.Conclusion);
                res.MyBlocks.CSs.Add(new TCoordSystem(TP.Uncompleted.CoordSystems[0].Name, TP.Uncompleted.CoordSystems[0].CsId));
                TMyCadastralBlock Bl = new TMyCadastralBlock();
                Bl.CN = "ТП v3";
                TMyRealty OKS = null;
                //Здание, постановка на ГКУ
                if (TP.Uncompleted != null)
                {
                    OKS = new TMyRealty("ОНС", netFteo.Rosreestr.dRealty_v03.Объект_незавершённого_строительства);
                    if (TP.Uncompleted.Package.NewUncompleteds.Count == 1)

                        foreach (RRTypes.V03_TP.tNewUncompleted un in TP.Uncompleted.Package.NewUncompleteds)
                        {
                            OKS.Uncompleted.AssignationName = un.AssignationName;
                            OKS.Address = RRTypes.CommonCast.CasterOKS.CastAddress(un.Address);
                            OKS.CadastralBlock = un.CadastralBlocks[0];
                            OKS.Uncompleted.ES = RRTypes.CommonCast.CasterOKS.ES_OKS("", un.EntitySpatial);
                            OKS.ParentCadastralNumbers.AddRange(un.ParentCadastralNumbers);
                            OKS.Uncompleted.DegreeReadiness = un.DegreeReadiness.ToString();
                            foreach (RRTypes.V03_TP.tKeyParameter param in un.KeyParameters)
                                OKS.KeyParameters.AddParameter(param.Type.ToString(), param.Value.ToString());
                        }
                }

                //Здание, изменение ОКС
                if (TP.Uncompleted.Package.ExistUncompleted != null)
                {
                    OKS.Uncompleted.AssignationName = TP.Uncompleted.Package.ExistUncompleted.AssignationName;
                    OKS.Address = RRTypes.CommonCast.CasterOKS.CastAddress(TP.Uncompleted.Package.ExistUncompleted.Address);
                    OKS.CadastralBlock = TP.Uncompleted.Package.ExistUncompleted.CadastralBlocks[0];
                    OKS.ParentCadastralNumbers.AddRange(TP.Uncompleted.Package.ExistUncompleted.ParentCadastralNumbers);
                    foreach (RRTypes.V03_TP.tKeyParameter param in TP.Uncompleted.Package.ExistUncompleted.KeyParameters)
                        OKS.KeyParameters.AddParameter(param.Type.ToString(), param.Value.ToString());
                    OKS.Uncompleted.DegreeReadiness = TP.Uncompleted.Package.ExistUncompleted.DegreeReadiness.ToString();
                    OKS.Uncompleted.ES = RRTypes.CommonCast.CasterOKS.ES_OKS("", TP.Uncompleted.Package.ExistUncompleted.EntitySpatial);
                }

                Bl.CN = OKS.CadastralBlock;
                Bl.AddOKS(OKS);
                res.MyBlocks.Blocks.Add(Bl);
            }
            return res;
        }
        #endregion
        
        #region  Разбор КПТ 09

        public netFteo.XML.FileInfo ParseKPT09(netFteo.XML.FileInfo fi,System.Xml.XmlDocument xmldoc)
        {
            netFteo.XML.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";
            res.Version = "09";
            res.DocType = "Кадастровый план территории";
            res.DocTypeNick = "КПТ";

            RRTypes.kpt09.KPT KPT09 = (RRTypes.kpt09.KPT)Desearialize<RRTypes.kpt09.KPT>(xmldoc);

            for (int i = 0; i <= KPT09.CadastralBlocks.Count - 1; i++)
            {
                TMyCadastralBlock Bl = new TMyCadastralBlock(KPT09.CadastralBlocks[i].CadastralNumber);
                for (int iP = 0; iP <= KPT09.CadastralBlocks[i].Parcels.Count - 1; iP++)
                {
                    TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(KPT09.CadastralBlocks[i].Parcels[iP].CadastralNumber, KPT09.CadastralBlocks[i].Parcels[iP].Name.ToString()));
                    MainObj.Location = RRTypes.CommonCast.CasterZU.CastLocation(KPT09.CadastralBlocks[i].Parcels[iP].Location);
                    MainObj.Utilization.UtilbyDoc = KPT09.CadastralBlocks[i].Parcels[iP].Utilization.ByDoc;
                    if (KPT09.CadastralBlocks[i].Parcels[iP].Utilization.UtilizationSpecified)
                        MainObj.Utilization.Untilization = KPT09.CadastralBlocks[i].Parcels[iP].Utilization.Utilization.ToString();
                    MainObj.AreaGKN = KPT09.CadastralBlocks[i].Parcels[iP].Area.Area;
                    MainObj.State = KPT09.CadastralBlocks[i].Parcels[iP].State.ToString();
                    MainObj.Category = netFteo.Rosreestr.dCategoriesv01.ItemToName(KPT09.CadastralBlocks[i].Parcels[iP].Category.ToString());
                    MainObj.DateCreated = KPT09.CadastralBlocks[i].Parcels[iP].DateCreated.ToString("dd.MM.yyyy");
                    //Землепользование
                    if (KPT09.CadastralBlocks[i].Parcels[iP].ParentCadastralNumbers != null)
                        MainObj.ParentCN = KPT09.CadastralBlocks[i].Parcels[iP].ParentCadastralNumbers.CadastralNumber;

                    if (KPT09.CadastralBlocks[i].Parcels[iP].EntitySpatial != null)
                        if (KPT09.CadastralBlocks[i].Parcels[iP].EntitySpatial.SpatialElement.Count > 0)
                        {
                            MainObj.EntitySpatial.ImportPolygon(KPT_v09Utils.AddEntSpatKPT09(KPT09.CadastralBlocks[i].Parcels[iP].CadastralNumber,
                                                                  KPT09.CadastralBlocks[i].Parcels[iP].EntitySpatial));
                            MainObj.EntitySpatial.Parent_Id = MainObj.id;
                            MainObj.EntitySpatial.AreaValue = (decimal)Convert.ToDouble(KPT09.CadastralBlocks[i].Parcels[iP].Area.Area);
                            MainObj.EntitySpatial.Definition = KPT09.CadastralBlocks[i].Parcels[iP].CadastralNumber;
                            res.MifPolygons.Add(KPT_v09Utils.AddEntSpatKPT09(KPT09.CadastralBlocks[i].Parcels[iP].CadastralNumber,
                                                                  KPT09.CadastralBlocks[i].Parcels[iP].EntitySpatial));
                        }
                    //Многоконтурный
                    if (KPT09.CadastralBlocks[i].Parcels[iP].Contours != null)
                    {
                        for (int ic = 0; ic <= KPT09.CadastralBlocks[i].Parcels[iP].Contours.Count - 1; ic++)
                        {
                            TMyPolygon NewCont = MainObj.Contours.AddPolygon(KPT_v09Utils.AddEntSpatKPT09(KPT09.CadastralBlocks[i].Parcels[iP].CadastralNumber + "(" +
                                                                  KPT09.CadastralBlocks[i].Parcels[iP].Contours[ic].NumberRecord + ")",
                                                                  KPT09.CadastralBlocks[i].Parcels[iP].Contours[ic].EntitySpatial));
                            //NewCont.GKNArea = KPT09.CadastralBlocks[i].Parcels[iP].Contours[ic].
                            res.MifPolygons.Add(NewCont);
                        }
                    }
                    //ЕЗП - В КПТ нет ОИПД!!

                }
                //ОИПД Квартала:
                //Виртуальный OIPD типа "Квартал":
                //TMyPolygon BlockSpat = new TMyPolygon();
                //BlockSpat = KPT_v09Utils.KPT09LandEntSpatToFteo(KPT09.CadastralBlocks[i].CadastralNumber,KPT09.CadastralBlocks[i].SpatialData.EntitySpatial);
                //this.DocInfo.MifPolygons.Items.Add(BlockSpat);
                Bl.Entity_Spatial.ImportPolygon(KPT_v09Utils.KPT09LandEntSpatToFteo(KPT09.CadastralBlocks[i].CadastralNumber, KPT09.CadastralBlocks[i].SpatialData.EntitySpatial));
                Bl.Entity_Spatial.Definition = "гр" + KPT09.CadastralBlocks[i].CadastralNumber;
                //Пункты в Квартале
                if (KPT09.CadastralBlocks[i].OMSPoints.Count > 0)
                {
                    for (int iP = 0; iP <= KPT09.CadastralBlocks[i].OMSPoints.Count - 1; iP++)
                    {
                        Point OMS = new Point();
                        OMS.NumGeopointA = KPT09.CadastralBlocks[i].OMSPoints[iP].PNmb;
                        OMS.Description = KPT09.CadastralBlocks[i].OMSPoints[iP].PKlass;
                        OMS.Code = KPT09.CadastralBlocks[i].OMSPoints[iP].PName;
                        OMS.x = (double)KPT09.CadastralBlocks[i].OMSPoints[iP].OrdX;
                        OMS.y = (double)KPT09.CadastralBlocks[i].OMSPoints[iP].OrdY;
                        Bl.AddOMSPoint(OMS);
                    }
                }
                for (int ics = 0; ics <= KPT09.CadastralBlocks[i].CoordSystems.Count - 1; ics++)
                {
                    res.MyBlocks.CSs.Add(new TCoordSystem(KPT09.CadastralBlocks[i].CoordSystems[ics].Name, KPT09.CadastralBlocks[i].CoordSystems[ics].CsId));

                }
                //Zones:
                if (KPT09.CadastralBlocks[i].Zones.Count > 0)
                {
                    for (int iP = 0; iP <= KPT09.CadastralBlocks[i].Zones.Count - 1; iP++)
                    {
                        TZone ZoneItem;
                        ZoneItem = new TZone(KPT09.CadastralBlocks[i].Zones[iP].AccountNumber);
                        ZoneItem.Description = KPT09.CadastralBlocks[i].Zones[iP].Description;
                        ZoneItem.EntitySpatial = KPT_v09Utils.KPT09LandEntSpatToFteo(KPT09.CadastralBlocks[i].Zones[iP].AccountNumber, KPT09.CadastralBlocks[i].Zones[iP].EntitySpatial);
                        res.MifPolygons.Add(KPT_v09Utils.KPT09LandEntSpatToFteo(KPT09.CadastralBlocks[i].Zones[iP].AccountNumber, KPT09.CadastralBlocks[i].Zones[iP].EntitySpatial));
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
                            BoundItem.EntitySpatial = KPT_v09Utils.KPT09LandEntSpatToFteo(KPT09.CadastralBlocks[i].Bounds[ib].AccountNumber, KPT09.CadastralBlocks[i].Bounds[ib].Boundaries[ibb].EntitySpatial);
                            res.MifPolygons.Add(KPT_v09Utils.KPT09LandEntSpatToFteo(KPT09.CadastralBlocks[i].Bounds[ib].AccountNumber, KPT09.CadastralBlocks[i].Bounds[ib].Boundaries[ibb].EntitySpatial));
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
                            TMyRealty Building = new TMyRealty(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                            Building.Building.ES = KPT_v09Utils.KPT09OKSEntSpatToFteo(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralNumber, KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.EntitySpatial);
                            Building.Building.AssignationBuilding = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.AssignationBuilding.ToString();
                            Building.Address = KPT_v09Utils.AddrKPT09(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.Address);
                            Building.Area = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.Area;
                            Building.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Building.ObjectType);
                            Bl.AddOKS(Building);
                            res.MifOKSPolygons.AddPolygon((TMyPolygon)Building.Building.ES);
                        }


                        if (KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction != null)
                        {
                            TMyRealty Constructions = new TMyRealty(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);
                            Constructions.Construction.AssignationName = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.AssignationName;
                            Constructions.Address = KPT_v09Utils.AddrKPT09(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.Address);
                            if (KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.KeyParameters.Count == 1)
                            {
                                // Constructions.Construction.KeyName = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.KeyParameters[0].Type.ToString();
                                // Constructions.Construction.KeyValue = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.KeyParameters[0].Value.ToString();
                            }
                            Constructions.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.ObjectType);
                            Constructions.Construction.ES = KPT_v09Utils.KPT09OKSEntSpatToFteo(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralNumber,
                                                                                             KPT09.CadastralBlocks[i].ObjectsRealty[iP].Construction.EntitySpatial);
                            Bl.AddOKS(Constructions);
                            res.MifOKSPolygons.AddPolygon((TMyPolygon)Constructions.Construction.ES);
                        }

                        if (KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted != null)
                        {
                            TMyRealty Uncompleted = new TMyRealty(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Объект_незавершённого_строительства);
                            Uncompleted.Uncompleted.AssignationName = KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.AssignationName;
                            Uncompleted.Address = KPT_v09Utils.AddrKPT09(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.Address);
                            Uncompleted.Uncompleted.ES = KPT_v09Utils.KPT09OKSEntSpatToFteo(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralNumber,
                                                                                             KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.EntitySpatial);
                            Uncompleted.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.ObjectType);
                            foreach (RRTypes.kpt09.tKeyParameter param in KPT09.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.KeyParameters)
                                Uncompleted.KeyParameters.AddParameter(netFteo.Rosreestr.dTypeParameter_v01.ItemToName(param.Type.ToString()), param.Value.ToString());
                            Bl.AddOKS(Uncompleted);
                            res.MifOKSPolygons.AddPolygon((TMyPolygon)Uncompleted.Uncompleted.ES);
                        }
                    }
                }

                res.MyBlocks.Blocks.Add(Bl);
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
        public netFteo.XML.FileInfo ParseKPT10(netFteo.XML.FileInfo fi,System.Xml.XmlDocument xmldoc) //RRTypes.kpt10_un.KPT KPT10)
        {
            netFteo.XML.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";
            res.DocType = "Кадастровый план территории";
            res.DocTypeNick = "КПТ";
            res.Version = "10";

            RRTypes.kpt10_un.KPT KPT10 = (RRTypes.kpt10_un.KPT)Desearialize<RRTypes.kpt10_un.KPT>(xmldoc);

            for (int i = 0; i <= KPT10.CadastralBlocks.Count - 1; i++)
            {
                TMyCadastralBlock Bl = new TMyCadastralBlock(KPT10.CadastralBlocks[i].CadastralNumber);
                for (int iP = 0; iP <= KPT10.CadastralBlocks[i].Parcels.Count - 1; iP++)
                {
                    TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(KPT10.CadastralBlocks[i].Parcels[iP].CadastralNumber, KPT10.CadastralBlocks[i].Parcels[iP].Name.ToString()));
                    MainObj.Location = RRTypes.CommonCast.CasterZU.CastLocation(KPT10.CadastralBlocks[i].Parcels[iP].Location);
                    MainObj.Utilization.UtilbyDoc = KPT10.CadastralBlocks[i].Parcels[iP].Utilization.ByDoc;
                    MainObj.Utilization.Untilization = KPT10.CadastralBlocks[i].Parcels[iP].Utilization.Utilization.ToString();
                    MainObj.AreaGKN = KPT10.CadastralBlocks[i].Parcels[iP].Area.Area;
                    MainObj.State = KPT10.CadastralBlocks[i].Parcels[iP].State.ToString();
                    MainObj.Category = netFteo.Rosreestr.dCategoriesv01.ItemToName(KPT10.CadastralBlocks[i].Parcels[iP].Category.ToString());
                    MainObj.DateCreated = KPT10.CadastralBlocks[i].Parcels[iP].DateCreated.ToString("dd.MM.yyyy");

                    //Землепользование
                    if (KPT10.CadastralBlocks[i].Parcels[iP].ParentCadastralNumbers != null)
                        MainObj.ParentCN = KPT10.CadastralBlocks[i].Parcels[iP].ParentCadastralNumbers.CadastralNumber;

                    if (KPT10.CadastralBlocks[i].Parcels[iP].EntitySpatial != null)
                        if (KPT10.CadastralBlocks[i].Parcels[iP].EntitySpatial.SpatialElement.Count > 0)
                        {
                            MainObj.EntitySpatial.ImportPolygon(KPT_v10Utils.AddEntSpatKPT10(KPT10.CadastralBlocks[i].Parcels[iP].CadastralNumber,
                                                                  KPT10.CadastralBlocks[i].Parcels[iP].EntitySpatial));
                            MainObj.EntitySpatial.AreaValue = (decimal)Convert.ToDouble(KPT10.CadastralBlocks[i].Parcels[iP].Area.Area);
                            MainObj.EntitySpatial.Parent_Id = MainObj.id;
                            MainObj.EntitySpatial.Definition = KPT10.CadastralBlocks[i].Parcels[iP].CadastralNumber;
                          res.MifPolygons.Add(KPT_v10Utils.AddEntSpatKPT10(KPT10.CadastralBlocks[i].Parcels[iP].CadastralNumber,
                                                                  KPT10.CadastralBlocks[i].Parcels[iP].EntitySpatial));
                        }
                    //Многоконтурный
                    if (KPT10.CadastralBlocks[i].Parcels[iP].Contours != null)
                    {
                        for (int ic = 0; ic <= KPT10.CadastralBlocks[i].Parcels[iP].Contours.Count - 1; ic++)
                        {
                            TMyPolygon NewCont = MainObj.Contours.AddPolygon(KPT_v10Utils.AddEntSpatKPT10(KPT10.CadastralBlocks[i].Parcels[iP].CadastralNumber + "(" +
                                                                  KPT10.CadastralBlocks[i].Parcels[iP].Contours[ic].NumberRecord + ")",
                                                                  KPT10.CadastralBlocks[i].Parcels[iP].Contours[ic].EntitySpatial));
                            //NewCont.GKNArea = KPT10.CadastralBlocks[i].Parcels[iP].Contours[ic].
                           res.MifPolygons.Add(KPT_v10Utils.AddEntSpatKPT10(KPT10.CadastralBlocks[i].Parcels[iP].CadastralNumber + "(" +
                                                                  KPT10.CadastralBlocks[i].Parcels[iP].Contours[ic].NumberRecord + ")",
                                                                  KPT10.CadastralBlocks[i].Parcels[iP].Contours[ic].EntitySpatial));
                        }
                    }
                    //ЕЗП - В КПТ нет ОИПД!!

                }
                //ОИПД Квартала:
                //Виртуальный OIPD типа "Квартал":
                //TMyPolygon BlockSpat = new TMyPolygon();
                //BlockSpat = KPT_v09Utils.KPT09LandEntSpatToFteo(KPT10.CadastralBlocks[i].CadastralNumber,KPT10.CadastralBlocks[i].SpatialData.EntitySpatial);
                //MifPolygons.Items.Add(BlockSpat);
                Bl.Entity_Spatial.ImportPolygon(KPT_v10Utils.KPT10LandEntSpatToFteo(KPT10.CadastralBlocks[i].CadastralNumber, KPT10.CadastralBlocks[i].SpatialData.EntitySpatial));
                Bl.Entity_Spatial.Definition = "гр" + KPT10.CadastralBlocks[i].CadastralNumber;
                //Пункты в Квартале
                if (KPT10.CadastralBlocks[i].OMSPoints.Count > 0)
                {
                    for (int iP = 0; iP <= KPT10.CadastralBlocks[i].OMSPoints.Count - 1; iP++)
                    {
                        Point OMS = new Point();
                        OMS.NumGeopointA = KPT10.CadastralBlocks[i].OMSPoints[iP].PNmb;
                        OMS.Description = KPT10.CadastralBlocks[i].OMSPoints[iP].PKlass;
                        OMS.Code = KPT10.CadastralBlocks[i].OMSPoints[iP].PName;
                        OMS.x = (double)KPT10.CadastralBlocks[i].OMSPoints[iP].OrdX;
                        OMS.y = (double)KPT10.CadastralBlocks[i].OMSPoints[iP].OrdY;
                        Bl.AddOMSPoint(OMS);
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
                        res.MifPolygons.Add(KPT_v10Utils.KPT10LandEntSpatToFteo(KPT10.CadastralBlocks[i].Zones[iP].AccountNumber, KPT10.CadastralBlocks[i].Zones[iP].EntitySpatial));
                        if (KPT10.CadastralBlocks[i].Zones[iP].Documents != null)
                            foreach (RRTypes.kpt10_un.tDocumentWithoutAppliedFile doc in KPT10.CadastralBlocks[i].Zones[iP].Documents)
                            {
                                ZoneItem.AddDocument(doc.Number, doc.Name, doc.CodeDocument.ToString(), doc.IssueOrgan, doc.Series, doc.Date.ToString());
                            }

                        if (KPT10.CadastralBlocks[i].Zones[iP].SpecialZone != null)
                        {
                            ZoneItem.AddContentRestrictions(KPT10.CadastralBlocks[i].Zones[iP].SpecialZone.ContentRestrictions);
                        }

                        if (KPT10.CadastralBlocks[i].Zones[iP].TerritorialZone != null)
                        {
                            //  ZoneItem.AddPermittedUses(KPT_v09Utils.PermittedUseCollectionToList(KPT10.CadastralBlocks[i].Zones[iP].TerritorialZone.PermittedUses));
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
                            res.MifPolygons.Add(KPT_v10Utils.KPT10LandEntSpatToFteo(KPT10.CadastralBlocks[i].Bounds[ib].AccountNumber, KPT10.CadastralBlocks[i].Bounds[ib].Boundaries[ibb].EntitySpatial));
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
                            TMyRealty Building = new TMyRealty(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                            Building.Building.ES = KPT_v10Utils.KPT10OKSEntSpatToFteo(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.CadastralNumber, KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.EntitySpatial);
                            Building.Building.AssignationBuilding = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.AssignationBuilding.ToString();
                            Building.Address = KPT_v10Utils.AddrKPT10(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.Address);
                            Building.Area = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.Area;
                            Building.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Building.ObjectType);
                            Bl.AddOKS(Building);
                            res.MifOKSPolygons.AddPolygon((TMyPolygon)Building.Building.ES);
                        }


                        if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction != null)
                        {
                            TMyRealty Constructions = new TMyRealty(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);
                            Constructions.Construction.AssignationName = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.AssignationName;
                            Constructions.Address = KPT_v10Utils.AddrKPT10(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.Address);
                            Constructions.Construction.ES = KPT_v10Utils.KPT10OKSEntSpatToFteo(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.CadastralNumber,
                                                                                             KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.EntitySpatial);
                            if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.KeyParameters.Count > 0)
                            {
                                foreach (RRTypes.kpt10_un.tKeyParameter param in KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.KeyParameters)
                                {
                                    Constructions.Construction.KeyParameters.AddParameter(param.Type.ToString(),
                                                                                          param.Value.ToString());
                                }
                            }
                            Constructions.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Construction.ObjectType);
                            Bl.AddOKS(Constructions);
                            res.MifOKSPolygons.AddPolygon((TMyPolygon)Constructions.Construction.ES);
                        }

                        if (KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted != null)
                        {
                            TMyRealty Uncompleted = new TMyRealty(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Объект_незавершённого_строительства);
                            Uncompleted.Uncompleted.AssignationName = KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.AssignationName;
                            Uncompleted.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.ObjectType);
                            Uncompleted.Address = KPT_v10Utils.AddrKPT10(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.Address);
                            Uncompleted.Uncompleted.ES = KPT_v10Utils.KPT10OKSEntSpatToFteo(KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.CadastralNumber,
                                                                                             KPT10.CadastralBlocks[i].ObjectsRealty[iP].Uncompleted.EntitySpatial);

                            Bl.AddOKS(Uncompleted);
                            res.MifOKSPolygons.AddPolygon((TMyPolygon)Uncompleted.Uncompleted.ES);
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
                res.Appointment  = KPT10.CertificationDoc.Official.Appointment;
                res.AppointmentFIO = KPT10.CertificationDoc.Official.FamilyName + " " + KPT10.CertificationDoc.Official.FirstName + " " + KPT10.CertificationDoc.Official.Patronymic;
            }
            return res;
            //ListMyCoolections(this.MyBlocks, this.MifPolygons);
        }
        #endregion

        #region  Разбор KPZU 6.0.1 (как бы ЕГРН)
        /// <summary>
        /// Разбор выписка ЕГРН
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="xmldoc">файл по схеме urn://x-artefacts-rosreestr-ru/outgoing/kpzu/6.0.1</param>
        /// <returns></returns>
        public netFteo.XML.FileInfo ParseKPZU(netFteo.XML.FileInfo fi,System.Xml.XmlDocument xmldoc) //RRTypes.kpzu06.KPZU kp, XmlDocument xmldoc)
        {
            netFteo.XML.FileInfo res = InitFileInfo(fi, xmldoc);
            RRTypes.kpzu06.KPZU kp = (RRTypes.kpzu06.KPZU) Desearialize <RRTypes.kpzu06.KPZU> (xmldoc);
            TMyCadastralBlock Bl = new TMyCadastralBlock();

            for (int i = 0; i <= kp.CoordSystems.Count - 1; i++)
            {

                res.MyBlocks.CSs.Add(new TCoordSystem(kp.CoordSystems[i].Name, kp.CoordSystems[i].CsId));

            }

            TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(kp.Parcel.CadastralNumber, kp.Parcel.Name.ToString()));
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
            MainObj.Category = netFteo.Rosreestr.dCategoriesv01.ItemToName(kp.Parcel.Category.ToString());
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
                    MainObj.EntitySpatial = RRTypes.KPZU_v05Utils.AddEntSpatKPZU05(kp.Parcel.CadastralNumber, kp.Parcel.EntitySpatial);
                     res.MifPolygons.Add(RRTypes.KPZU_v05Utils.AddEntSpatKPZU05(kp.Parcel.CadastralNumber, kp.Parcel.EntitySpatial));
                }
            //Многоконтурный
            if (kp.Parcel.Contours != null)
            {

                for (int ic = 0; ic <= kp.Parcel.Contours.Count - 1; ic++)
                {
                     res.MifPolygons.Add(RRTypes.KPZU_v05Utils.AddEntSpatKPZU05(kp.Parcel.CadastralNumber + "(" +
                                                          kp.Parcel.Contours[ic].NumberRecord + ")",
                                                          kp.Parcel.Contours[ic].EntitySpatial));
                    TMyPolygon NewCont = MainObj.Contours.AddPolygon(RRTypes.KPZU_v05Utils.AddEntSpatKPZU05(kp.Parcel.CadastralNumber + "(" +
                                                          kp.Parcel.Contours[ic].NumberRecord + ")",
                                                          kp.Parcel.Contours[ic].EntitySpatial));
                    NewCont.AreaValue = kp.Parcel.Contours[ic].Area.Area;
                }
            }
            //ЕЗП:
            if (kp.Parcel.CompositionEZ.Count > 0)
            {
                for (int i = 0; i <= kp.Parcel.CompositionEZ.Count - 1; i++)
                // if ( kp.Parcel.CompositionEZ[i].EntitySpatial != null)
                {
                    MainObj.CompozitionEZ.AddEntry(kp.Parcel.CompositionEZ[i].CadastralNumber,
                        kp.Parcel.CompositionEZ[i].Area.Area,
                        RRTypes.KPZU_v05Utils.AddEntSpatKPZU05(kp.Parcel.CompositionEZ[i].CadastralNumber,
                                                                     kp.Parcel.CompositionEZ[i].EntitySpatial));


                     res.MifPolygons.Add(RRTypes.KPZU_v05Utils.AddEntSpatKPZU05(kp.Parcel.CompositionEZ[i].CadastralNumber,
                                                           kp.Parcel.CompositionEZ[i].EntitySpatial));
                    //MainObj.CompozitionEZ[MainObj.CompozitionEZ.Count - 1].AreaGKN = kp.Parcel.CompositionEZ[i].Area.Area.ToString();

                }
            }
            //Части 
            if (kp.Parcel.SubParcels.Count > 0)
            {
                for (int i = 0; i <= kp.Parcel.SubParcels.Count - 1; i++)
                {
                    TmySlot Sl = MainObj.AddSubParcel(kp.Parcel.SubParcels[i].NumberRecord);
                    Sl.AreaGKN = kp.Parcel.SubParcels[i].Area.Area.ToString();

                    if (kp.Parcel.SubParcels[i].Encumbrance != null)
                        Sl.Encumbrance = RRTypes.KPZU_v05Utils.KVZUEncumtoFteoEncum(kp.Parcel.SubParcels[i].Encumbrance);

                    if (kp.Parcel.SubParcels[i].EntitySpatial != null)
                    {
                        TMyPolygon SlEs = RRTypes.KPZU_v05Utils.AddEntSpatKPZU05(kp.Parcel.SubParcels[i].NumberRecord, kp.Parcel.SubParcels[i].EntitySpatial);
                        Sl.EntSpat.ImportPolygon(SlEs);
                        res.MifPolygons.Add(SlEs);
                    }

                }
            }

            //Прикрутим сюды парсинг через XPATH ЕГРН
            MainObj.EGRN = RRTypes.CommonCast.CasterEGRP.ParseEGRNRights(xmldoc); // мдаааа!!!
             res.MyBlocks.Blocks.Add(Bl);
            res.DocTypeNick = "ЕГРН";
            CommonCast.CasterEGRP.Parse_ReestrExtract(xmldoc, res);
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
        public netFteo.XML.FileInfo ParseKVZU07(netFteo.XML.FileInfo fi, System.Xml.XmlDocument xmldoc)//  RRTypes.kvzu07.KVZU kv, XmlDocument xmldoc)
        {

            netFteo.XML.FileInfo res = InitFileInfo(fi, xmldoc);

            RRTypes.kvzu07.KVZU kv = (RRTypes.kvzu07.KVZU)Desearialize<RRTypes.kvzu07.KVZU>(xmldoc);

            TMyCadastralBlock Bl = new TMyCadastralBlock();


            for (int i = 0; i <= kv.CoordSystems.Count - 1; i++)
            {
                 res.MyBlocks.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));

            }
            /*
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
            TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(kv.Parcels.Parcel.CadastralNumber, kv.Parcels.Parcel.Name.ToString()));

            MainObj.CadastralBlock = kv.Parcels.Parcel.CadastralBlock;
            Bl.CN = kv.Parcels.Parcel.CadastralBlock; // !!! Иначе будет пустой 
            
            MainObj.SpecialNote = kv.Parcels.Parcel.SpecialNote;
            res.CommentsType = "Особые отметки";
            //res.Comments = kv.Parcels.Parcel.SpecialNote;

            MainObj.Utilization.UtilbyDoc = kv.Parcels.Parcel.Utilization.ByDoc;
            if (kv.Parcels.Parcel.Utilization.UtilizationSpecified)
                MainObj.Utilization.Untilization = kv.Parcels.Parcel.Utilization.Utilization.ToString();
            MainObj.Category = netFteo.Rosreestr.dCategoriesv01.ItemToName(kv.Parcels.Parcel.Category.ToString());

            MainObj.Location = RRTypes.CommonCast.CasterZU.CastLocation(kv.Parcels.Parcel.Location);

            MainObj.Rights = KVZU_v06Utils.KVZURightstoFteorights(kv.Parcels.Parcel.Rights);
            MainObj.Encumbrances = KVZU_v06Utils.KVZUEncumstoFteoEncums(kv.Parcels.Parcel.Encumbrances);
            MainObj.AreaGKN = kv.Parcels.Parcel.Area.Area;
            MainObj.State = kv.Parcels.Parcel.State.ToString();
            MainObj.DateCreated = kv.Parcels.Parcel.DateCreated.ToString("dd.MM.yyyy");
            //Землепользование
            if (kv.Parcels.Parcel.EntitySpatial != null)
                if (kv.Parcels.Parcel.EntitySpatial.SpatialElement.Count > 0)
                {
                    MainObj.EntitySpatial = RRTypes.KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.Parcel.CadastralNumber,
                                                           kv.Parcels.Parcel.EntitySpatial);
                    MainObj.EntitySpatial.Parent_Id = MainObj.id;
                    res.MifPolygons.Add(RRTypes.KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.Parcel.CadastralNumber,
                                                           kv.Parcels.Parcel.EntitySpatial));
                }
            //Многоконтурный
            if (kv.Parcels.Parcel.Contours != null)
            {
                // ??? MainObj.Contours.Parent_id = MainObj.id;
                for (int ic = 0; ic <= kv.Parcels.Parcel.Contours.Count - 1; ic++)
                {
                    res.MifPolygons.Add(RRTypes.KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.Parcel.Contours[ic].NumberRecord,
                                                                                 kv.Parcels.Parcel.Contours[ic].EntitySpatial));
                    TMyPolygon NewCont = MainObj.Contours.AddPolygon(RRTypes.KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.Parcel.Contours[ic].NumberRecord,
                                                                                                            kv.Parcels.Parcel.Contours[ic].EntitySpatial));
                    NewCont.AreaValue = kv.Parcels.Parcel.Contours[ic].Area.Area;
                }
            }
            //ЕЗП:
            if (kv.Parcels.Parcel.CompositionEZ.Count > 0)
            {
                for (int i = 0; i <= kv.Parcels.Parcel.CompositionEZ.Count - 1; i++)
                // if ( kv.Parcels.Parcel.CompositionEZ[i].EntitySpatial != null)
                {
                    MainObj.CompozitionEZ.AddPolygon(
                                                  RRTypes.KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.Parcel.CompositionEZ[i].CadastralNumber,
                                                                                         kv.Parcels.Parcel.CompositionEZ[i].EntitySpatial));
                    MainObj.CompozitionEZ[MainObj.CompozitionEZ.Count - 1].AreaValue = kv.Parcels.Parcel.CompositionEZ[i].Area.Area;
                    MainObj.CompozitionEZ[MainObj.CompozitionEZ.Count - 1].State = RRTypes.KVZU_v06Utils.KVZUState(kv.Parcels.Parcel.CompositionEZ[i].State);

                    res.MifPolygons.Add(RRTypes.KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.Parcel.CompositionEZ[i].CadastralNumber,
                                                           kv.Parcels.Parcel.CompositionEZ[i].EntitySpatial));
                    MainObj.CompozitionEZ[MainObj.CompozitionEZ.Count - 1].AreaValue = kv.Parcels.Parcel.CompositionEZ[i].Area.Area;
                }
            }
            //Части 
            if (kv.Parcels.Parcel.SubParcels.Count > 0)
            {
                for (int i = 0; i <= kv.Parcels.Parcel.SubParcels.Count - 1; i++)
                {
                    TmySlot Sl = MainObj.AddSubParcel(kv.Parcels.Parcel.SubParcels[i].NumberRecord);
                    Sl.AreaGKN = kv.Parcels.Parcel.SubParcels[i].Area.Area.ToString();
                    if (kv.Parcels.Parcel.SubParcels[i].Encumbrance != null)
                        Sl.Encumbrance = RRTypes.KVZU_v06Utils.KVZUEncumtoFteoEncum(kv.Parcels.Parcel.SubParcels[i].Encumbrance);
                    if (kv.Parcels.Parcel.SubParcels[i].EntitySpatial != null)
                    {
                        TMyPolygon SlEs = RRTypes.KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.Parcel.SubParcels[i].NumberRecord, kv.Parcels.Parcel.SubParcels[i].EntitySpatial);
                        Sl.EntSpat.ImportPolygon(SlEs);
                        res.MifPolygons.Add(SlEs);
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
                    TMyParcel OffObj = Bl.Parcels.AddParcel(new TMyParcel(kv.Parcels.OffspringParcel[i].CadastralNumber, i + 1));
                    OffObj.EntitySpatial = RRTypes.KVZU_v06Utils.AddEntSpatKVZU06(kv.Parcels.OffspringParcel[i].CadastralNumber,
                                                                                  kv.Parcels.OffspringParcel[i].EntitySpatial);
                    OffObj.State = "Item05";
                }

            //Прикрутим сюды парсинг Прав ветки ЕГРП через XPATH ЕГРН
            MainObj.EGRN = RRTypes.CommonCast.CasterEGRP.ParseEGRNRights(xmldoc); // мдаааа!!!
            res.MyBlocks.Blocks.Add(Bl);
            res.DocTypeNick = "КВЗУ";
            CommonCast.CasterEGRP.Parse_ReestrExtract(xmldoc, res);
            return res;
        }
        #endregion

        #region  Разбор KPOKS 04.
        /// <summary>
        /// Разбор KPOKS 04.
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="xmldoc">файл по схеме urn://x-artefacts-rosreestr-ru/outgoing/kpoks/4.0.1</param>
        /// <returns></returns>
        public netFteo.XML.FileInfo ParseKPOKS(netFteo.XML.FileInfo fi, System.Xml.XmlDocument xmldoc)//RRTypes.kpoks_v04.KPOKS kv, XmlDocument xmldoc)
        {
            if (xmldoc.DocumentElement.NamespaceURI != "urn://x-artefacts-rosreestr-ru/outgoing/kpoks/4.0.1") return null;
            netFteo.XML.FileInfo res = InitFileInfo(fi, xmldoc);
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
                res.MyBlocks.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));
            }
    
            if (kv.Realty.Building != null)
            {
                TMyCadastralBlock Bl = new TMyCadastralBlock(kv.Realty.Building.CadastralBlocks[0].ToString());
                TMyRealty Bld = new TMyRealty(kv.Realty.Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                Bld.DateCreated = kv.Realty.Building.DateCreated.ToString("dd.MM.yyyy");
                Bld.Building.AssignationBuilding = netFteo.Rosreestr.dAssBuildingv01.ItemToName(kv.Realty.Building.AssignationBuilding.ToString());
                Bld.Name = kv.Realty.Building.Name;
                Bld.Address = RRTypes.CommonCast.CasterOKS.CastAddress(kv.Realty.Building.Address);
                Bld.Area = kv.Realty.Building.Area;
                Bld.Building.ES = RRTypes.CommonCast.CasterOKS.ES_OKS(kv.Realty.Building.CadastralNumber, kv.Realty.Building.EntitySpatial);
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
                res.MyBlocks.Blocks.Add(Bl);

            }

            if (kv.Realty.Flat != null)
            {
                TMyCadastralBlock Bl = new TMyCadastralBlock(kv.Realty.Flat.CadastralBlock);
                TMyRealty flat = new TMyRealty(kv.Realty.Flat.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Помещение);
                flat.DateCreated = kv.Realty.Flat.DateCreated.ToString("dd.MM.yyyy");
                flat.Flat.AssignationCode = netFteo.Rosreestr.dAssFlatv01.ItemToName(kv.Realty.Flat.Assignation.AssignationCode.ToString());
                //Bld.Name = kv.Realty.Flat.Name;
                flat.Address = RRTypes.CommonCast.CasterOKS.CastAddress(kv.Realty.Flat.Address);
                flat.Area = kv.Realty.Flat.Area;
                // ES для Flat нету совсеем в КПОКС 4.0.1 !!!!
                //Bld.ES = RRTypes.kvoks_v02.Utils.AddEntSpatKPOKS04(kv.Realty.Flat.CadastralNumber, kv.Realty.Flat.EntitySpatial);
                if (kv.Realty.Flat.PositionInObject.Position != null)
                    flat.Flat.PositionInObject.Levels.Add(new TLevel("", "", kv.Realty.Flat.PositionInObject.Position.NumberOnPlan));
                if (kv.Realty.Flat.PositionInObject.Levels != null)
                    flat.Flat.PositionInObject.Levels.Add(new TLevel(kv.Realty.Flat.PositionInObject.Levels[0].Type.ToString(), kv.Realty.Flat.PositionInObject.Levels[0].Number, kv.Realty.Flat.PositionInObject.Levels[0].Position.NumberOnPlan));
                flat.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Flat.ObjectType);
                flat.Rights = RRTypes.CommonCast.CasterEGRP.ParseKPSOKSRights(xmldoc);
                flat.EGRN = RRTypes.CommonCast.CasterEGRP.ParseEGRNRights(xmldoc);
                Bl.AddOKS(flat);
                flat.Notes = kv.Realty.Flat.Notes;
                res.CommentsType = "Особые отметки";
                //res.Comments = flat.Notes;
                res.MyBlocks.Blocks.Add(Bl);
            }

            if (kv.Realty.Construction != null)
            {
                TMyCadastralBlock Bl = new TMyCadastralBlock(kv.Realty.Construction.CadastralBlocks[0].ToString());
                TMyRealty Constructions = new TMyRealty(kv.Realty.Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);
                Constructions.DateCreated = kv.Realty.Construction.DateCreated.ToString("dd.MM.yyyy");
                Constructions.Construction.AssignationName = kv.Realty.Construction.AssignationName;
                Constructions.Name = kv.Realty.Construction.Name;
                Constructions.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Construction.ObjectType);
                Constructions.Address = RRTypes.CommonCast.CasterOKS.CastAddress(kv.Realty.Construction.Address);
                Constructions.Construction.ES = RRTypes.CommonCast.CasterOKS.ES_OKS(kv.Realty.Construction.CadastralNumber, kv.Realty.Construction.EntitySpatial);
                if (Constructions.Construction.ES.GetType().Name == "TMyPolygon")
                    res.MifOKSPolygons.AddPolygon((TMyPolygon)Constructions.Construction.ES);
                res.CommentsType = "Особые отметки";
                res.Comments = Constructions.Notes;

                Bl.AddOKS(Constructions);


                res.MyBlocks.Blocks.Add(Bl);
            }
            res.Number = kv.CertificationDoc.Number;
            res.Date = kv.CertificationDoc.Date.ToString("dd.MM.yyyy");
            res.Cert_Doc_Organization = kv.CertificationDoc.Organization;

            if (kv.CertificationDoc.Official != null)
            {
                res.Appointment = RRTypes.CommonCast.CasterEGRP.Parse_SenderAppointment(xmldoc); // мдаааа!!! XPATH !
                res.AppointmentFIO = kv.CertificationDoc.Official.FamilyName + " " + kv.CertificationDoc.Official.FirstName + " " + kv.CertificationDoc.Official.Patronymic;
            }
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
        public netFteo.XML.FileInfo ParseKVOKS07(netFteo.XML.FileInfo fi, System.Xml.XmlDocument xmldoc)// RRTypes.kvoks_v07.KVOKS kv, XmlDocument xmldoc)
        {
            netFteo.XML.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";

            RRTypes.kvoks_v07.KVOKS kv = (RRTypes.kvoks_v07.KVOKS)Desearialize<RRTypes.kvoks_v07.KVOKS>(xmldoc);

            for (int i = 0; i <= kv.CoordSystems.Count - 1; i++)
            {
                res.MyBlocks.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));

            }

            if (kv.Realty.Building != null)
            {
                TMyCadastralBlock Bl = new TMyCadastralBlock(kv.Realty.Building.CadastralBlocks[0].ToString());
                TMyRealty Bld = new TMyRealty(kv.Realty.Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                Bld.ParentCadastralNumbers.AddRange(kv.Realty.Building.ParentCadastralNumbers);
                Bld.Building.AssignationBuilding = kv.Realty.Building.AssignationBuilding.ToString();
                Bld.Name = kv.Realty.Building.Name;
                Bld.Address = RRTypes.CommonCast.CasterOKS.CastAddress(kv.Realty.Building.Address);
                Bld.Building.ES = RRTypes.CommonCast.CasterOKS.ES_OKS(kv.Realty.Building.CadastralNumber, kv.Realty.Building.EntitySpatial);
                Bld.Building.Area = kv.Realty.Building.Area;
                Bld.Rights = RRTypes.CommonCast.CasterEGRP.ParseEGRNRights(xmldoc);
                res.CommentsType = "Особые отметки";
                res.Comments = Bld.Notes;
                if (kv.Realty.Building.Flats.Count > 0)
                {
                    for (int i = 0; i <= kv.Realty.Building.Flats.Count - 1; i++)
                    {
                        TFlat Flat = new TFlat(kv.Realty.Building.Flats[i].CadastralNumber);
                        TLevel lvl = new TLevel(
                            kv.Realty.Building.Flats[i].PositionInObject.Levels[0].Type.ToString(),
                            kv.Realty.Building.Flats[i].PositionInObject.Levels[0].Number,
                            kv.Realty.Building.Flats[i].PositionInObject.Levels[0].Position.NumberOnPlan);
                        foreach (RRTypes.kvoks_v07.tPlan jpegname in kv.Realty.Building.Flats[i].PositionInObject.Levels[0].Position.Plans)
                            lvl.AddPlan(jpegname.Name);
                        Flat.PositionInObject.Levels.Add(lvl);
                        Flat.Area = kv.Realty.Building.Flats[i].Area;
                        Bld.Building.Flats.Add(Flat);
                    }
                }
                Bl.AddOKS(Bld);
                res.MyBlocks.Blocks.Add(Bl);
            }



            if (kv.Realty.Construction != null)
            {
                TMyCadastralBlock Bl = new TMyCadastralBlock(kv.Realty.Construction.CadastralBlocks[0].ToString());
                TMyRealty Constructions = new TMyRealty(kv.Realty.Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);
                Constructions.DateCreated = kv.Realty.Construction.DateCreated.ToString("dd.MM.yyyy");
                Constructions.Construction.AssignationName = kv.Realty.Construction.AssignationName;
                Constructions.Name = kv.Realty.Construction.Name;
                Constructions.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Construction.ObjectType);
                Constructions.ParentCadastralNumbers.AddRange(kv.Realty.Construction.ParentCadastralNumbers);
                Constructions.Address = RRTypes.CommonCast.CasterOKS.CastAddress(kv.Realty.Construction.Address);
                Constructions.Construction.ES = RRTypes.CommonCast.CasterOKS.ES_OKS(kv.Realty.Construction.CadastralNumber, kv.Realty.Construction.EntitySpatial);
                foreach (RRTypes.kvoks_v07.tKeyParameter param in kv.Realty.Construction.KeyParameters)
                    Constructions.KeyParameters.AddParameter(param.Type.ToString(), param.Value.ToString());
                foreach (RRTypes.kvoks_v07.tOldNumber n in kv.Realty.Construction.OldNumbers)
                    Constructions.Construction.OldNumbers.Add(n.Number);
                Bl.AddOKS(Constructions);
                Constructions.Notes = kv.Realty.Construction.Notes;
                res.CommentsType = "Особые отметки";
                res.Comments= Constructions.Notes;
                Constructions.Rights = RRTypes.CommonCast.CasterEGRP.ParseEGRNRights(xmldoc);

                res.MyBlocks.Blocks.Add(Bl);
            }



            res.DocTypeNick = "КВОКС";
            CommonCast.CasterEGRP.Parse_ReestrExtract(xmldoc, res);
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
        public netFteo.XML.FileInfo ParseEGRP(netFteo.XML.FileInfo fi, System.Xml.XmlDocument xmldoc)
        {
            netFteo.XML.FileInfo res = InitFileInfo(fi, xmldoc);
            res.CommentsType = "-";
            res.DocType = "ЕГРП";
            res.DocTypeNick = "ЕГРП";
            res.Version = "v04";


            TMyCadastralBlock Bl = new TMyCadastralBlock();
            System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
            nsmgr.AddNamespace("egrp", xmldoc.DocumentElement.NamespaceURI);

             res.DocType = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:ReestrExtract/egrp:DeclarAttribute/@egrp:ExtractTypeText", nsmgr).Value.ToString();
             res.Number = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:ReestrExtract/egrp:DeclarAttribute/@egrp:ExtractNumber", nsmgr).Value.ToString();
             res.Date = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:ReestrExtract/egrp:DeclarAttribute/@egrp:ExtractDate", nsmgr).Value.ToString();


            //Extract     /ReestrExtract     /DeclarAttribute     /ReceivName
            if ((xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:ReestrExtract/egrp:DeclarAttribute/egrp:ReceivName", nsmgr) != null) &&
                (xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:ReestrExtract/egrp:DeclarAttribute/egrp:ReceivAdress", nsmgr) != null))
            //    linkLabel_Recipient.Text = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:ReestrExtract/egrp:DeclarAttribute/egrp:ReceivName", nsmgr).FirstChild.Value.ToString() +
            //                        "  " + xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:ReestrExtract/egrp:DeclarAttribute/egrp:ReceivAdress", nsmgr).FirstChild.Value.ToString();
            //linkLabel_Request.Text = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:ReestrExtract/egrp:DeclarAttribute/@egrp:RequeryNumber", nsmgr).Value.ToString();


            if (xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:eDocument/egrp:Sender/@egrp:Name", nsmgr) != null)
            {
               res.Cert_Doc_Organization = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:eDocument/egrp:Sender/@egrp:Name", nsmgr).Value.ToString();
               res.Appointment = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:eDocument/egrp:Sender/@egrp:Appointment", nsmgr).Value.ToString();
               res.AppointmentFIO = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:eDocument/egrp:Sender/@egrp:FIO", nsmgr).Value.ToString();
            }



            System.Xml.XmlNode ParcelNode = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:ReestrExtract/egrp:ExtractObjectRight/egrp:ExtractObject", nsmgr);///
            if (ParcelNode != null)
            {
                if (ParcelNode.SelectSingleNode("egrp:ObjectRight/egrp:ObjectDesc/egrp:ObjectTypeText", nsmgr).FirstChild.Value.ToString() == "Земельный участок")
                {
                    TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(ParcelNode.SelectSingleNode("egrp:ObjectRight/egrp:ObjectDesc/egrp:CadastralNumber", nsmgr).FirstChild.Value.ToString(),
                                                                           "Item01"));

                    Bl.CN = xmldoc.DocumentElement.SelectSingleNode("/egrp:Extract/egrp:eDocument/@egrp:Scope", nsmgr).Value.ToString();
                    MainObj.Location.Address = new netFteo.Rosreestr.TAddress();
                    MainObj.Location.Address.Note = ParcelNode.SelectSingleNode("egrp:ObjectRight/egrp:ObjectDesc/egrp:Address/egrp:Content", nsmgr).FirstChild.Value.ToString();
                    MainObj.Utilization.UtilbyDoc = ParcelNode.SelectSingleNode("egrp:ObjectRight/egrp:ObjectDesc/egrp:GroundCategoryText", nsmgr).FirstChild.Value.ToString();
                    MainObj.AreaGKN = ParcelNode.SelectSingleNode("egrp:ObjectRight/egrp:ObjectDesc/egrp:Area/egrp:AreaText", nsmgr).FirstChild.Value.ToString();
                    //Прикрутим сюды парсинг через XPATH ЕГРН
                    MainObj.EGRN = EGRP_v60Utils.ParseEGRRights(xmldoc); // мдаааа!!!
                }
            }
            res.MyBlocks.Blocks.Add(Bl);
           return res;
        }
        #endregion
    }

}


