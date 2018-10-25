using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RRTypes.kpzu;
using RRTypes.kpzu06;

namespace RRTypes
{
    public static class KPZU_v05Utils
    { /*
        public static netFteo.Rosreestr.TMyRights KVZURightstoFteorights(tRightCollection Rightsfrom)
        {
            netFteo.Rosreestr.TMyRights MyRights = new netFteo.Rosreestr.TMyRights();

            for (int i = 0; i <= Rightsfrom.Count - 1; i++)
            {
                netFteo.Rosreestr.TRight Right = new netFteo.Rosreestr.TRight();
                Right.Type = Rightsfrom[i].Type.ToString();
                Right.Name = Rightsfrom[i].Name;
                Right.RegDate = Rightsfrom[i].Registration.RegDate.ToString();
                Right.RegNumber = Rightsfrom[i].Registration.RegNumber.ToString();

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

                MyRights.Add(Right);

            }

            return MyRights;

        }
*/
        public static netFteo.Rosreestr.TMyEncumbrance KVZUEncumtoFteoEncum(kpzu.tEncumbranceZU Encumfrom)
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
            return MyEnc;
        }
        public static netFteo.Rosreestr.TMyEncumbrance KVZUEncumtoFteoEncum(kpzu06.tEncumbranceZU Encumfrom)
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
            return MyEnc;
        }
        public static netFteo.Rosreestr.TMyEncumbrances KPZUEncumstoFteoEncums(kpzu.tEncumbranceZUCollection Encumsfrom)
        {
            netFteo.Rosreestr.TMyEncumbrances MyEncs = new netFteo.Rosreestr.TMyEncumbrances();
            for (int i = 0; i <= Encumsfrom.Count - 1; i++)
            {
                MyEncs.Add(KVZUEncumtoFteoEncum(Encumsfrom[i]));
            }

            return MyEncs;

        }
        public static netFteo.Rosreestr.TMyEncumbrances KPZUEncumstoFteoEncums(kpzu06.tEncumbranceZUCollection Encumsfrom)
        {
            netFteo.Rosreestr.TMyEncumbrances MyEncs = new netFteo.Rosreestr.TMyEncumbrances();
            for (int i = 0; i <= Encumsfrom.Count - 1; i++)
            {
                MyEncs.Add(KVZUEncumtoFteoEncum(Encumsfrom[i]));
            }

            return MyEncs;

        }



    }


    /// <summary>
    /// Парсинг прав из выписки ЕГРП. Читаем xml без XSD, через  XPath
    /// </summary>
    public static class EGRP_v60Utils
    {

        public static netFteo.Rosreestr.TMyRights ParseEGRRights(System.Xml.XmlDocument xmldoc)
        {
            // Add the namespace.
            System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
            nsmgr.AddNamespace("egrp", xmldoc.DocumentElement.NamespaceURI);

            System.Xml.XmlNodeList lst = xmldoc.DocumentElement.SelectNodes("/egrp:Extract/egrp:ReestrExtract/egrp:ExtractObjectRight/egrp:ExtractObject/egrp:ObjectRight/egrp:Right", nsmgr);

            netFteo.Rosreestr.TMyRights rs = new netFteo.Rosreestr.TMyRights();

            foreach (System.Xml.XmlNode cd in lst)
            {

                netFteo.Rosreestr.TRight rt = new netFteo.Rosreestr.TRight();
                netFteo.Rosreestr.TMyOwner own = new netFteo.Rosreestr.TMyOwner("");
                if (cd.SelectSingleNode("egrp:Owner/egrp:Person", nsmgr) != null)
                {
                    own.OwnerName += cd.SelectSingleNode("egrp:Owner/egrp:Person/egrp:FIO/egrp:Surname", nsmgr).FirstChild.Value + " ";
                    own.OwnerName += cd.SelectSingleNode("egrp:Owner/egrp:Person/egrp:FIO/egrp:First", nsmgr).FirstChild.Value + " ";
                    
                    System.Xml.XmlNode ptr = cd.SelectSingleNode("egrp:Owner/egrp:Person/egrp:FIO/egrp:Patronymic", nsmgr);

                    if ((ptr != null) && (ptr.FirstChild != null) && (ptr.FirstChild.Value != null))

                      own.OwnerName += ptr.FirstChild.Value;
                }

                if (cd.SelectSingleNode("egrp:Owner/egrp:Organization", nsmgr) != null)
                {
                    System.Xml.XmlNode ownrOrg = cd.SelectSingleNode("egrp:Owner/egrp:Organization/egrp:Name", nsmgr);
                    own.OwnerName = ownrOrg.LastChild.Value;
                }

                if (cd.SelectSingleNode("egrp:Owner/egrp:Governance", nsmgr) != null)
                {
                    System.Xml.XmlNode ownrOrg = cd.SelectSingleNode("egrp:Owner/egrp:Governance/egrp:Name", nsmgr);
                    own.OwnerName = ownrOrg.LastChild.Value;
                }

                if (cd.SelectSingleNode("egrp:NoOwner", nsmgr) != null)
                {
                    own.OwnerName = cd.SelectSingleNode("egrp:NoOwner", nsmgr).LastChild.Value;
                }

                
                if (cd.SelectSingleNode("egrp:Registration", nsmgr) != null)
                {
                    rt.RegNumber = cd.SelectSingleNode("egrp:Registration/egrp:RegNumber", nsmgr).FirstChild.Value;
                    rt.Name = cd.SelectSingleNode("egrp:Registration/egrp:Name", nsmgr).FirstChild.Value;
                    rt.RegDate = cd.SelectSingleNode("egrp:Registration/egrp:RegDate", nsmgr).FirstChild.Value;
                    rt.Type = cd.SelectSingleNode("egrp:Registration/egrp:Type", nsmgr).FirstChild.Value;

                    if (cd.SelectSingleNode("egrp:Registration/egrp:ShareText", nsmgr) != null)
                        rt.Desc = cd.SelectSingleNode("egrp:Registration/egrp:ShareText", nsmgr).FirstChild.Value;
                }

                //Encumbrance
                if (cd.SelectSingleNode("egrp:Encumbrance", nsmgr) != null)
                {
                    netFteo.Rosreestr.TMyEncumbrance enc = new netFteo.Rosreestr.TMyEncumbrance();
                    enc.RegNumber = cd.SelectSingleNode("egrp:Encumbrance/egrp:RegNumber", nsmgr).FirstChild.Value;
                    enc.Name = cd.SelectSingleNode("egrp:Encumbrance/egrp:Name", nsmgr).FirstChild.Value;
                    enc.RegDate = cd.SelectSingleNode("egrp:Encumbrance/egrp:RegDate", nsmgr).FirstChild.Value;
                    enc.Type = cd.SelectSingleNode("egrp:Encumbrance/egrp:Type", nsmgr).FirstChild.Value;
                    if (cd.SelectSingleNode("egrp:Encumbrance/egrp:Duration/egrp:Started", nsmgr) != null)
                    enc.DurationStarted = cd.SelectSingleNode("egrp:Encumbrance/egrp:Duration/egrp:Started", nsmgr).FirstChild.Value;
                    if (cd.SelectSingleNode("egrp:Encumbrance/egrp:Duration/egrp:Stopped", nsmgr) != null)
                    enc.DurationStopped = cd.SelectSingleNode("egrp:Encumbrance/egrp:Duration/egrp:Stopped", nsmgr).FirstChild.Value;

                    if (cd.SelectSingleNode("egrp:Encumbrance/egrp:Owner", nsmgr) != null)
                    {
                        netFteo.Rosreestr.TMyOwner ownEnc = new netFteo.Rosreestr.TMyOwner("");
                        if (cd.SelectSingleNode("egrp:Encumbrance/egrp:Owner/egrp:Person", nsmgr) != null)
                        {
                           ownEnc.OwnerName = cd.SelectSingleNode("egrp:Encumbrance/egrp:Owner/egrp:Person/egrp:Content", nsmgr).FirstChild.Value;
                        }
                        enc.Owners.Add(ownEnc);
                    }

                           
                    rt.Encumbrances.Add(enc);
                }
                rt.Owners.Add(own);
                rs.Add(rt);
            }
            return rs;
        }
    }

    
}
