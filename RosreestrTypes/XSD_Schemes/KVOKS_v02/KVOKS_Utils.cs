using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RRTypes.kvoks_v02
{
    /*
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
    }
    */
}


    