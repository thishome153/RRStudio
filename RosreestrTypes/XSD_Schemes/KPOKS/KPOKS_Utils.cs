using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RRTypes.kpoks_v03;
using netFteo.BaseClasess;

namespace RRTypes.OKSCast
{
    /*
    public static class KPOKS_Utils
    {

    }


    public static class Utils
    {
        //Если здесь везде потребоавть заполнение parent_id ??
        public static Object AddEntSpatKVOKS02(string Definition, RRTypes.kvoks_v02.tEntitySpatialOKSOut ES)
        {
            if (ES == null) return null;
            if (ES.SpatialElement[0].SpelementUnit[0].Ordinate.X == ES.SpatialElement[0].SpelementUnit[ES.SpatialElement[0].SpelementUnit.Count() - 1].Ordinate.X)
            {
                netFteo.BaseClasess.TMyPolygon fES = new netFteo.BaseClasess.TMyPolygon();

                //OUT
                for (int ip = 0; ip <= ES.SpatialElement[0].SpelementUnit.Count - 1; ip++)
                {
                    netFteo.BaseClasess.TmyPointO P = new netFteo.BaseClasess.TmyPointO();
                    P.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.X);
                    P.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.Y);
                    P.NumGeopointA = ES.SpatialElement[0].SpelementUnit[ip].Ordinate.NumGeopoint;
                    P.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                    fES.AppendPoint(P);
                }


                //childs
                for (int i = 1; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.BaseClasess.TMyOutLayer ESch = fES.AddChild();

                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.BaseClasess.TmyPointO P = new netFteo.BaseClasess.TmyPointO();
                        P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                        P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        ESch.AppendPoint(P);
                    }

                }



                return fES;
            }
            else
            {
                netFteo.BaseClasess.TPolyLines PolyCollection = new netFteo.BaseClasess.TPolyLines(netFteo.BaseClasess.Gen_id.newId);
                for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.BaseClasess.TPolyLine line = new netFteo.BaseClasess.TPolyLine();
                    line.Layer_id = netFteo.BaseClasess.Gen_id.newId;
                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.BaseClasess.TmyPointO P = line.AppendPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                   Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);

                    }
                    PolyCollection.Add(line);
                }
                return PolyCollection;
            }





        }
        public static Object AddEntSpatKPOKS03(string Definition, RRTypes.kpoks_v03.tEntitySpatialOKSOut ES)
        {
            if (ES == null) return null;
            if (ES.SpatialElement[0].SpelementUnit[0].Ordinate.X == ES.SpatialElement[0].SpelementUnit[ES.SpatialElement[0].SpelementUnit.Count() - 1].Ordinate.X)
            {
                netFteo.BaseClasess.TMyPolygon fES = new netFteo.BaseClasess.TMyPolygon();

                //OUT
                for (int ip = 0; ip <= ES.SpatialElement[0].SpelementUnit.Count - 1; ip++)
                {
                    netFteo.BaseClasess.TmyPointO P = new netFteo.BaseClasess.TmyPointO();
                    P.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.X);
                    P.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.Y);
                    P.NumGeopointA = ES.SpatialElement[0].SpelementUnit[ip].Ordinate.NumGeopoint;
                    P.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                    fES.AppendPoint(P);
                }


                //childs
                for (int i = 1; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.BaseClasess.TMyOutLayer ESch = fES.AddChild();

                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.BaseClasess.TmyPointO P = new netFteo.BaseClasess.TmyPointO();
                        P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                        P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        ESch.AppendPoint(P);
                    }

                }



                return fES;
            }
            else
            {
                netFteo.BaseClasess.TPolyLines PolyCollection = new netFteo.BaseClasess.TPolyLines(netFteo.BaseClasess.Gen_id.newId);
                for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.BaseClasess.TPolyLine line = new netFteo.BaseClasess.TPolyLine();
                    line.Layer_id = netFteo.BaseClasess.Gen_id.newId;
                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.BaseClasess.TmyPointO P = line.AppendPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                   Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);

                    }
                    PolyCollection.Add(line);
                }
                return PolyCollection;
            }





        }
        public static Object AddEntSpatKPOKS04(string Definition, RRTypes.kpoks_v04.tEntitySpatialOKSOut ES)
        {
            if (ES == null) return null;
            if (ES.SpatialElement[0].SpelementUnit[0].Ordinate.X == ES.SpatialElement[0].SpelementUnit[ES.SpatialElement[0].SpelementUnit.Count() - 1].Ordinate.X)
            {
                netFteo.BaseClasess.TMyPolygon fES = new netFteo.BaseClasess.TMyPolygon();

                //OUT
                for (int ip = 0; ip <= ES.SpatialElement[0].SpelementUnit.Count - 1; ip++)
                {
                    netFteo.BaseClasess.TmyPointO P = new netFteo.BaseClasess.TmyPointO();
                    P.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.X);
                    P.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.Y);
                    P.NumGeopointA = ES.SpatialElement[0].SpelementUnit[ip].Ordinate.NumGeopoint;
                    P.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                    fES.AppendPoint(P);
                }


                //childs
                for (int i = 1; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.BaseClasess.TMyOutLayer ESch = fES.AddChild();

                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.BaseClasess.TmyPointO P = new netFteo.BaseClasess.TmyPointO();
                        P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                        P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        ESch.AppendPoint(P);
                    }

                }



                return fES;
            }
            else
            {
                netFteo.BaseClasess.TPolyLines PolyCollection = new netFteo.BaseClasess.TPolyLines(netFteo.BaseClasess.Gen_id.newId);
                for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.BaseClasess.TPolyLine line = new netFteo.BaseClasess.TPolyLine();
                    line.Layer_id = netFteo.BaseClasess.Gen_id.newId;
                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.BaseClasess.TmyPointO P = line.AppendPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                   Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);

                    }
                    PolyCollection.Add(line);
                }
                return PolyCollection;
            }





        }
        public static Object AddEntSpatKVOKS07(string Definition, RRTypes.kvoks_v07.tEntitySpatialOKSOut ES)
        {
            if (ES == null) return null;
            if (ES.SpatialElement[0].SpelementUnit[0].Ordinate.X == ES.SpatialElement[0].SpelementUnit[ES.SpatialElement[0].SpelementUnit.Count() - 1].Ordinate.X)
            {
                netFteo.BaseClasess.TMyPolygon fES = new netFteo.BaseClasess.TMyPolygon();

                //OUT
                for (int ip = 0; ip <= ES.SpatialElement[0].SpelementUnit.Count - 1; ip++)
                {
                    netFteo.BaseClasess.TmyPointO P = new netFteo.BaseClasess.TmyPointO();
                    P.x = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.X);
                    P.y = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.Y);
                    P.NumGeopointA = ES.SpatialElement[0].SpelementUnit[ip].Ordinate.NumGeopoint;
                    P.Mt = Convert.ToDouble(ES.SpatialElement[0].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                    fES.AppendPoint(P);
                }


                //childs
                for (int i = 1; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.BaseClasess.TMyOutLayer ESch = fES.AddChild();

                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.BaseClasess.TmyPointO P = new netFteo.BaseClasess.TmyPointO();
                        P.x = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X);
                        P.y = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y);
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);
                        ESch.AppendPoint(P);
                    }

                }



                return fES;
            }
            else
            {
                netFteo.BaseClasess.TPolyLines PolyCollection = new netFteo.BaseClasess.TPolyLines(netFteo.BaseClasess.Gen_id.newId);
                for (int i = 0; i <= ES.SpatialElement.Count - 1; i++)
                {
                    netFteo.BaseClasess.TPolyLine line = new netFteo.BaseClasess.TPolyLine();
                    line.Layer_id = netFteo.BaseClasess.Gen_id.newId;
                    for (int ip = 0; ip <= ES.SpatialElement[i].SpelementUnit.Count - 1; ip++)
                    {
                        netFteo.BaseClasess.TmyPointO P = line.AppendPoint((i + 1).ToString(), Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.X),
                                                                   Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.Y), "");
                        P.NumGeopointA = ES.SpatialElement[i].SpelementUnit[ip].Ordinate.NumGeopoint;
                        P.Mt = Convert.ToDouble(ES.SpatialElement[i].SpelementUnit[ip].Ordinate.DeltaGeopoint);

                    }
                    PolyCollection.Add(line);
                }
                return PolyCollection;
            }





        }
        public static TAddress AddrKP(RRTypes.kvoks_v07.tAddressOut Address)
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
            if (Address.Apartment != null)
                Adr.Apartment = Address.Apartment.Type + " " + Address.Apartment.Value;

            Adr.Region = Address.Region.ToString();

            return Adr;
        }
        public static TAddress AddrKP(RRTypes.kpoks_v03.tAddressOut Address)
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
        public static TAddress AddrKP(RRTypes.kpoks_v04.tAddressOut Address)
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
            if (Address.Apartment != null)
                Adr.Apartment = Address.Apartment.Type + " " + Address.Apartment.Value;

            Adr.Region = Address.Region.ToString();

            return Adr;
        }
        public static TAddress AddrKP(RRTypes.V03_TP.tAddressInpFull Address)
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
            if (Address.Apartment != null)
                Adr.Apartment = Address.Apartment.Type + " " + Address.Apartment.Value;

            Adr.Region = Address.Region.ToString();

            return Adr;
        }
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

    }
    */
  
}
