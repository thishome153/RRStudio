using System;
using System.ComponentModel; // Binding list prereq
using System.Collections.Generic;
using System.Linq;
using System.Text;
using netFteo.Spatial;

namespace netFteo
{
    /*
   class TCadwork
    {

       public Spatial.PointList Points;
       public Spatial.TMyParcelCollection Parcels;
       public NikonRaw.TNikonRaw RawData;
       public TCadwork()
       {
           this.Points = new Spatial.PointList();
           this.RawData = new NikonRaw.TNikonRaw();
           this.Parcels = new Spatial.TMyParcelCollection();
       }
    }
    */

/// <summary>
/// Cadaster namespace contains classes which present cadastral objects
/// </summary>
    namespace Cadaster
    {

        #region Cadaster hierarchy

        public class TCadastralSubject : TCadasterItem
        {
            public List<TCadastralDistrict> Districts;
            public TCadastralSubject()
            {
                Districts = new List<TCadastralDistrict>();
            }
        }

        /// <summary>
        /// Cadastral district - collection of cadastral blocks
        /// </summary>
        public class TCadastralDistrict : TCadasterItem
        {

            public string SubRF_Name;
            public string SubRF_CN;
            public Byte SubRF_id;

            public void SelTest()
            {

            }

            public List<TCadastralBlock> Blocks;
            
            /*
            public TEntitySpatial OMSPoints
            {
               
                get
                {
                    TEntitySpatial res = new TEntitySpatial();
                    foreach (TCadastralBlock bl in Blocks)
                        foreach(GeodesicBase basePoint in bl.OMSPoints)
                        res.Add(basePoint);
                    return res;
                }
                */

            public GeodethicBases OMSPoints
            {
                get
                {
                    GeodethicBases res = new GeodethicBases();
                    foreach (TCadastralBlock bl in Blocks)
                        foreach (GeodesicBase basePoint in bl.OMSPoints)
                            res.Add(basePoint);
                    return res;
                }
            }




            /// <summary>
            /// Common spatial data collection -results of parsing xml 
            /// Store result of xml only ( with Cadastral Blocks)
            /// </summary>
            public TEntitySpatial SpatialData
            {
                get
                {
                    TEntitySpatial res = new TEntitySpatial();
                    //Realtys
                    res.AddES(this.GetRealtyEs());

                    //Parcels
                    res.AddES(this.GetParcelsEs());
                    //Zones
                    res.AddES(this.GetZonesEs());
                    //Bound
                    return res;
                }
            }

            /// <summary>
            /// DXF, MIF spatial data collection - -results of parsing spatial files 
            /// Store result of mif, dxf, csv, txt
            /// </summary>
            //public List<TEntitySpatial> ParsedSpatial;
            public TEntitySpatial ParsedSpatial;


            public Rosreestr.TMyRights EGRN; // Временно  прикручиваем сюды ???

            public TCadastralDistrict()
            {
                this.Blocks = new List<TCadastralBlock>();
                //this.ParsedSpatial = new List<TEntitySpatial>();
                this.ParsedSpatial = new TEntitySpatial();
                TEntitySpatial checkFake = this.SpatialData;
                this.CSs = new TCoordSystems();
            }

            public string SingleCN // Если квартал один, вернуть его CN
            {
                get
                {
                    if (this.Blocks.Count == 1)
                        return this.Blocks[0].CN;
                    else
                        return null;
                }
            }

            public void AddBlock(TCadastralBlock block)
            {
                block.Parent_id = this.id;
                this.Blocks.Add(block);
            }

            public Object GetEs(int Item_id)
            {
                //From Parcels
                for (int i = 0; i <= this.Blocks.Count - 1; i++)
                {
                    Object Entity = this.Blocks[i].GetEs(Item_id);
                    if (Entity != null)
                        return Entity;
                }

                //From OKS
                /*
                foreach (IGeometry feature in this.SpatialData)
                {
                    if (feature.id == Item_id)
                        return feature;

                }
                */
                if (this.SpatialData.FeatureExists(Item_id))
                    return this.SpatialData.GetFeature(Item_id);

                //From dxf, mif
                // Single feature
                /*
                foreach (IGeometry feature in this.ParsedSpatial)
                {
                    if (feature.id == Item_id)
                        return feature;
                }
                */
                if (this.ParsedSpatial.FeatureExists(Item_id))
                    return this.ParsedSpatial.GetFeature(Item_id);

                //next, point in features:
                foreach (IGeometry item in this.ParsedSpatial)
                {

                    if (item is TPolygon)
                    {
                        TPoint pt = ((TPolygon)item).GetPoint(Item_id);
                        if (pt != null) return pt;

                        foreach (TRing child in ((TPolygon)item).Childs)
                        {
                            TPoint pt1 = child.GetPoint(Item_id);
                            if (pt1 != null) return pt1;
                        }
                    }

                    if (item is IPointList)
                    {
                        TPoint pt = ((IPointList)item).GetPoint(Item_id);
                        if (pt != null) return pt;
                    }
                }

                //Full ES
                if (this.ParsedSpatial.id == Item_id)
                    return this.ParsedSpatial;

                //Anyway - nothing found. Return null
                return null;
            }

            public bool RemoveGeometry(long id)
            {
                //From dxf, mif
                // Single feature
                foreach (IGeometry feature in this.ParsedSpatial)
                {
                    if (feature.id == id)
                    {
                        if (this.ParsedSpatial.Remove(feature))
                            return true;
                    }

                    if (feature is IPointList)
                    {
                        if (((IPointList)feature).RemovePoint(id))

                            return true;

                    }
                }


                //Full ES
                if (this.ParsedSpatial.id == id)
                {
                    this.ParsedSpatial.Clear();
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Select features from ES by handling layer
            /// </summary>
            /// <param name="Layer_Handle">Owner layer</param>
            /// <returns>ES</returns>
            public IGeometry GetEs(string Layer_Handle)
            {
                TEntitySpatial res = new TEntitySpatial();
                res.Definition = this.SpatialData.Definition;
                foreach (IGeometry feature in this.SpatialData)
                {
                    if (feature.LayerHandle == Layer_Handle)
                        res.Add(feature);
                }

                foreach (IGeometry feature in this.ParsedSpatial)
                {
                    if (feature.LayerHandle == Layer_Handle)
                        res.Add(feature);
                }

                if (res.Count == 0) return null;
                else
                    return res;
            }

            /// <summary>
            /// Выборка ОИПД из коллекции зон
            /// </summary>
            /// <param name="ZoneType">"1 - тер. зоны"</param>
            /// <returns></returns>
            public TEntitySpatial GetZonesEs(int ZoneType =1)
            {
                TEntitySpatial Res = new TEntitySpatial();
                for (int i = 0; i <= this.Blocks.Count - 1; i++)
                    for (int iz = 0; iz <= this.Blocks[i].Zones.Count - 1; iz++)
                    {
                        if (((this.Blocks[i].Zones[iz].PermittedUses != null) &&
                            (ZoneType == 1)) ||
                            ((this.Blocks[i].Zones[iz].PermittedUses == null) &&
                            (ZoneType != 1))
                            )
                        {
                            if (this.Blocks[i].Zones[iz].EntitySpatial != null)

                                Res.AddFeatures(this.Blocks[i].Zones[iz].EntitySpatial);
                        }

                    }
                return Res;
            }

            /// <summary>
            /// Show parcels collection as list (like KPT table)
            /// </summary>
            /// <param name="LV"></param>
            /// <param name="SetTag"></param>
            public void ParcelsToList(System.Windows.Forms.ListView LV, bool SetTag)
            {
                foreach (TCadastralBlock block in this.Blocks)
                {
                    block.Parcels.ShowasListItems(LV, SetTag);
                }
            }

            public void ObjectRealtysToList(System.Windows.Forms.ListView LV, bool SetTag, bool ClearItems = false)
            {
                foreach (TCadastralBlock block in this.Blocks)
                {
                    block.ObjectRealtys.ShowasListItems(LV, SetTag,ClearItems);
                }
            }

            public TEntitySpatial GetParcelsEs()
            {
                TEntitySpatial Res = new TEntitySpatial();
                for (int i = 0; i <= this.Blocks.Count - 1; i++)
                    foreach (TParcel parcel in this.Blocks[i].Parcels)
                    {
                        foreach (IGeometry feature in parcel.EntSpat)
                            Res.Add(feature);
                        /*
                        if (parcel.CompozitionEZ != null)
                            foreach (IGeometry feature in parcel.CompozitionEZ)
                                Res.Add(feature);
                        */
                    }
                return Res;
            }

            public TEntitySpatial GetRealtyEs()
            {
                TEntitySpatial Res = new TEntitySpatial();
                for (int i = 0; i <= this.Blocks.Count - 1; i++)
                    for (int iz = 0; iz <= this.Blocks[i].ObjectRealtys.Count - 1; iz++)
                    {
                        if (
                       (this.Blocks[i].ObjectRealtys[iz].EntSpat != null))
                            foreach (IGeometry feature in this.Blocks[i].ObjectRealtys[iz].EntSpat)
                                Res.Add(feature);
                        //TODO ES to ES2 migrate 
                        /*


                        if (
                            (this.Blocks[i].ObjectRealtys[iz].Uncompleted != null) &&
                            (this.Blocks[i].ObjectRealtys[iz].Uncompleted.ES != null)
                            )
                            Res.Add(this.Blocks[i].ObjectRealtys[iz].Uncompleted.ES);
                        */
                    }
                return Res;
            }

            public TCadastralBlock GetBlock(long id)
            {
                if (this.Blocks.Exists(x => x.id == id))
                {
                    return Blocks.Find(x => x.id == id);
                }
                else
                    /* TODO :  kill
                    for (int i = 0; i <= this.Blocks.Count - 1; i++)
                    {
                        if (this.Blocks[i].id == id)
                            return this.Blocks[i];
                    }
                    */
                    return null;
            }

            public bool BlockExist(int id)
            {
                return Blocks.Exists(PredicateAkaBlock => PredicateAkaBlock.id == id);
            }

            public bool BlockExist(string cn)
            {
                return Blocks.Exists(PredicateAkaBlock => PredicateAkaBlock.CN == cn);
            }

            public TCadastralBlock GetBlock(string CN)
            {
                if (this.Blocks.Exists(x => x.CN == CN))
                {
                    return Blocks.Find(x => x.CN == CN);
                }
                else
                    return null;
            }


            public bool ParcelExist(string CN)
            {
                //return (TMyParcel)this.GetObject(id);

                for (int i = 0; i <= this.Blocks.Count - 1; i++)
                {
                    if (this.Blocks[i].ParcelExist(CN))
                        return true;
                }
                return false;
            }

            public TParcel GetParcel(long id)
            {
                return (TParcel)this.GetObject(id);
            }


            public object GetObject(long id)
            {
                for (int i = 0; i <= this.Blocks.Count - 1; i++)
                {
                    if (this.Blocks[i].GetObject(id) != null)
                        return this.Blocks[i].GetObject(id);
                }
                return null;
            }

            public TCoordSystems CSs;
        }

        public class TCadastralBlock
        {
            public long id;
            /// <summary>
            /// District id
            /// </summary>
            public long Parent_id;
            public bool HasParcels;
            public TPolygon Entity_Spatial;
            public TParcels Parcels;

            /// <summary>
            /// ОКСЫ
            /// </summary>
            public TRealtys ObjectRealtys;
            //public PointList OMSPoints;
            public List<GeodesicBase> OMSPoints;
            public TBoundsList GKNBounds;
            public TZonesList Zones;
            public TFiles KPTXmlBodyList;
            public string CN;
            public string Name;
            public string Comments;
            public TCadastralBlock()
            {
                //OMSPoints = new PointList();
                OMSPoints = new List<GeodesicBase>();
                Parcels = new TParcels();
                ObjectRealtys = new TRealtys();
                GKNBounds = new TBoundsList();
                Zones = new TZonesList();
                Entity_Spatial = new TPolygon();
                KPTXmlBodyList = new TFiles();
            }
            public TCadastralBlock(string cn) : this() // call constructor with ()
            {
                this.CN = cn;
            }

            /*
            public PointList AddOmsPoint(List<GeodesicBase> oms)
            {
                PointList res = new PointList();
                foreach (GeodesicBase pt in oms)
                    res.AddPoint(AddOmsPoint(pt));
                return res;
            }
            */

            public GeodesicBase AddOmsPoint(GeodesicBase OMS)
            {
                this.OMSPoints.Add(OMS);
                return this.OMSPoints[this.OMSPoints.Count() - 1];
            }

            public TRealEstate AddOKS(TRealEstate newOKS)
            {
                this.ObjectRealtys.Add(newOKS);
                return (TRealEstate)this.ObjectRealtys[this.ObjectRealtys.Count - 1];
            }

            public TBound AddBound(TBound newBound)
            {
                this.GKNBounds.Add(newBound);
                return this.GKNBounds[this.GKNBounds.Count - 1];

            }
            public TZone AddZone(TZone zone)
            {
                this.Zones.Add(zone);
                return this.Zones[this.Zones.Count - 1];

            }

            //ИПД в квартале
            public Object GetEs(int Layer_id)
            {
                if (this.Parcels.GetEs(Layer_id) != null)
                    return this.Parcels.GetEs(Layer_id);

                if (this.GKNBounds.GetEs(Layer_id) != null)
                    return this.GKNBounds.GetEs(Layer_id);

                if (this.Zones.GetEsId(Layer_id) != null)
                    return this.Zones.GetEsId(Layer_id);

                if (this.ObjectRealtys.GetEs(Layer_id) != null)
                    return this.ObjectRealtys.GetEs(Layer_id);

                if ((this.Entity_Spatial.PointCount > 0) &&
                    (this.Entity_Spatial.id == Layer_id)
                    )
                    return this.Entity_Spatial;
                return null;
            }

            public bool ParcelExist(string CN)
            {
                for (int i = 0; i <= this.Parcels.Count - 1; i++)
                {
                    if (this.Parcels[i].CN == CN)
                        return true;
                }
                return false;
            }

            public TParcel GetParcel(string CN)
            {
                for (int i = 0; i <= this.Parcels.Count - 1; i++)
                {
                    if (this.Parcels[i].CN == CN)
                        return this.Parcels[i];
                }
                return null;
            }

            public object GetObject(long id)
            {
                for (int i = 0; i <= this.Parcels.Count - 1; i++)
                {
                    if (this.Parcels[i].id == id)
                        return this.Parcels[i];
                }

                for (int i = 0; i <= this.ObjectRealtys.Count - 1; i++)
                {
                    if (this.ObjectRealtys[i].Building != null)
                    {
                        foreach (TFlat flat in this.ObjectRealtys[i].Building.Flats)
                            if (flat.id == id)
                                return flat;
                    }

                    if (this.ObjectRealtys[i].Flat != null)
                    {
                        if (this.ObjectRealtys[i].Flat.id == id)
                            return this.ObjectRealtys[i].Flat;
                    }

                }

                /* TODO KILL
                for (int i = 0; i <= this.Parcels.Count - 1; i++)
                {
                    if (this.Parcels[i].CompozitionEZ != null)
                        for (int ij = 0; ij <= this.Parcels[i].CompozitionEZ.Count - 1; ij++)
                            if (this.Parcels[i].CompozitionEZ[ij].id == id)
                                return this.Parcels[i].CompozitionEZ[ij];
                }

                for (int i = 0; i <= this.Parcels.Count - 1; i++)
                {
                    if (this.Parcels[i].Contours != null)
                        if (this.Parcels[i].Contours.id == id)
                            return this.Parcels[i].Contours;
                }
                */
                //если ищем чзу:
                for (int i = 0; i <= this.Parcels.Count - 1; i++)

                    for (int sli = 0; sli <= this.Parcels[i].SubParcels.Count - 1; sli++)
                    {
                        if (this.Parcels[i].SubParcels[sli].id == id)
                            return this.Parcels[i].SubParcels[sli];
                    }

                for (int i = 0; i <= this.ObjectRealtys.Count - 1; i++)
                {
                    if (((TRealEstate)this.ObjectRealtys[i]).id == id)
                        return this.ObjectRealtys[i];
                }

                for (int i = 0; i <= this.Zones.Count - 1; i++)
                {
                    if (this.Zones[i].id == id)
                        return this.Zones[i];
                }

                for (int i = 0; i <= this.GKNBounds.Count - 1; i++)
                {
                    if (this.GKNBounds[i].id == id)
                        return this.GKNBounds[i];
                }

                return null;
            }
        }

        public class TParcels : List<TParcel>
        {
            //   public List<TMyParcel> Parcels;

            public TParcels()
            {
                //this.Parcels = new List<TMyParcel>();

            }

            /// <summary>
            /// Add item to collection
            /// </summary>
            /// <param name="Parcel"></param>
            /// <returns></returns>
            public TParcel AddParcel(TParcel Parcel)
            {
                this.Add(Parcel);
                return this[this.Count - 1];
            }

            public int AddParcels(TParcels parcels)
            {
                foreach (TParcel inParcel in parcels)
                {
                    this.Add(inParcel);
                }

                return parcels.Count;
            }

            public IGeometry GetEs(int Layer_id)
            {
                for (int i = 0; i <= this.Count() - 1; i++)
                {
                    if (this[i].GetEs(Layer_id) != null)
                        return this[i].GetEs(Layer_id);
                }
                return null;
            }

            public void ShowasListItems(System.Windows.Forms.ListView LV, bool SetTag)
            {
                //if (PointCount == 0) return;
                LV.BeginUpdate();
                LV.Items.Clear();
                LV.Controls.Clear();
                LV.View = System.Windows.Forms.View.Details;
                LV.Columns[0].Text = "КН";
                LV.Columns[1].Text = "Address";
                LV.Columns[2].Text = "Площадь";
                LV.Columns[3].Text = "Площадь граф.";
                LV.Columns[4].Text = "-";
                LV.Columns[5].Text = "-";
                LV.Columns[6].Text = "-";
                // if (SetTag) LV.Tag = id;

                foreach (TParcel Parcel in this)
                {
                    System.Windows.Forms.ListViewItem LViItem = new System.Windows.Forms.ListViewItem();
                    LViItem.Text = Parcel.CN;
                    LV.Items.Add(LViItem);
                    LViItem.SubItems.Add(Parcel.Location.Address.AsString(true));
                    LViItem.SubItems.Add(Parcel.AreaGKN);
                    LViItem.SubItems.Add(Parcel.EntSpat.AreaSpatialFmt("0", false));
                    LViItem.SubItems.Add(Parcel.Utilization.UtilbyDoc);
                   //need xsd enums: LViItem.SubItems.Add(Parcel.Utilization.Untilization);
                }
                LV.EndUpdate();
            }

        }
    

        /// <summary>
        /// Земельный участок
        /// </summary>
        public class TParcel : TCadasterItem
        {
            private string FParentCN;
            private string FParcelName;
            private TCompozitionEZ fCompozitionEZ;

            public new string State
            {
                get
                {
                    return base.State.ToString();
                }
                set
                {
                    int test = -1;
                    if (Int32.TryParse(value, out test))
                        base.State = test;
                }
            }
            public string CadastralBlock;
            public long CadastralBlock_id;
            public string AreaGKN;
            /// <summary>
            /// Значение, указаное
            /// </summary>
            public string AreaValue;
            public string Purpose;
            public Utilization Utilization;
            public LandUse Landuse;
            public string Category;
            public string SpecialNote;
            public Rosreestr.TLocation Location;
            public Rosreestr.TMyRights Rights;
            public Rosreestr.TMyRights EGRN;
            public Rosreestr.TMyEncumbrances Encumbrances;

            /// <summary>
            ///  Spatial data. Universal, both for single ES and multi (contours/EZP).
            ///  will replace polygon/contours
            /// </summary>
            public TEntitySpatial EntSpat;

            public TCompozitionEZ CompozitionEZ
            {
                set
                {

                    this.fCompozitionEZ = new TCompozitionEZ();
                    this.fCompozitionEZ = value;


                }
                get
                {
                    return this.fCompozitionEZ;
                }
            }

            public TFiles XmlBodyList;
            public TSlots SubParcels;
            public List<String> AllOffspringParcel;// Кадастровые номера всех земельных участков, образованных из данного земельного участка
            public List<String> PrevCadastralNumbers; //Кадастровые номера земельных участков, из которых образован
            public List<String> InnerCadastralNumbers;// Кадастровые номера зданий, сооружений, объектов незавершенного строительства, расположенных на земельном участке

            public TParcel()
            {
                Utilization = new Utilization();
                Landuse = new LandUse();
                Encumbrances = new Rosreestr.TMyEncumbrances();
                this.XmlBodyList = new TFiles();
                this.SubParcels = new TSlots();
                this.AllOffspringParcel = new List<string>();
                this.InnerCadastralNumbers = new List<string>();
                this.PrevCadastralNumbers = new List<string>();
                this.Location = new netFteo.Rosreestr.TLocation();
                this.Rights = new Rosreestr.TMyRights();
                this.EntSpat = new TEntitySpatial();
                //this.SpecialNote = "";
                this.id = Gen_id.newId;
                this.AreaGKN = "-1";
            }

            /// <summary>
            /// Override inherited property for customizing behavior
            /// </summary>
            public string ParcelName
            {
                get
                {
                    if ((this.Name != null) &&

                        (this.Name != ""))
                        return this.Name;

                    if (this.CompozitionEZ != null)
                        return "Единое землепользование";

                    if (this.EntSpat.Count > 1)
                        return "Многоконтурный участок";
                    return "Землепользование"; //default
                }
                set
                {
                    this.FParcelName = value;
                }
            }

            public TParcel(string cn) : this() // Вызов Конструктора по умолчанию
            {
                this.CN = cn;
            }

            public TParcel(string cn, int parcel_id) : this() // Вызов Конструктора по умолчанию
            {
                this.id = parcel_id;
                this.CN = cn;
            }

            public TParcel(string CN, string name) : this(CN, Gen_id.newId) //Вызов конструктора переопределенного
            {
                this.Name = netFteo.Rosreestr.dParcelsv01.ItemToName(name);
                switch (Name)
                {
                    /*
                    case "Землепользование": { this.EntitySpatial = new TPolygon(); this.Name = netFteo.Rosreestr.dParcelsv01.ItemToName(name); return; }
                    case "Обособленный участок": { this.EntitySpatial = new TPolygon(); this.Name = netFteo.Rosreestr.dParcelsv01.ItemToName(name); return; }
                    case "Условный участок": { this.EntitySpatial = new TPolygon(); this.Name = netFteo.Rosreestr.dParcelsv01.ItemToName(name); return; }
                    case "Многоконтурный участок": { this.Contours = new TEntitySpatial(); this.Name = netFteo.Rosreestr.dParcelsv01.ItemToName(name); return; }
                        */
                    case "Единое землепользование": // "Item02": //
                        {
                            //this.Contours = new TPolygonCollection(); // на свякий случай, как в Осетии: ЕЗП с контурами 
                            this.CompozitionEZ = new TCompozitionEZ();
                            return;
                        }
                    case "Значение отсутствует": return;
                    default: return;
                        //	{ this.EntitySpatial = new TPolygon(); this.Name = netFteo.Rosreestr.dParcelsv01.ItemToName(name); return; }
                }
            }

            public TParcel(Rosreestr.dParcelsv01_enum name, string cadastralblock, string definition)
                  : this() //Вызов конструктора переопределенного
            {
                this.CadastralBlock = cadastralblock;
                this.Definition = definition;
                // this.Name = name.ToString();
            }
            /*
             * 
            private int Fid;
            public int id
            {
                get { return this.Fid; }
                set { this.Fid = value; }
            }
            */


            public void AddEntry(string entrynumber, decimal areaEntry, decimal Inaccuracy, int state, TPolygon ESs)
            {
                if (ESs == null) return;
                EZPEntry entry = new EZPEntry();
                entry.Spatial_ID = ESs.id; // links to spatial
                entry.CN = entrynumber;
                entry.Definition = entrynumber;
                //entry.ImportPolygon(ESs);
                entry.AreaEntry = areaEntry;
                ESs.AreaValue = areaEntry;
                ESs.AreaInaccuracy = Inaccuracy != 0 ? Inaccuracy.ToString() : "";
                entry.State = state;

                this.EntSpat.AddPolygon(ESs);
                this.CompozitionEZ.Add(entry);
            }


            public TSlot AddSubParcel(string SlotNumber)
            {
                TSlot Sl = new TSlot();
                Sl.NumberRecord = SlotNumber;
                this.SubParcels.Add(Sl);
                return this.SubParcels[this.SubParcels.Count - 1];
            }

            public string ParentCN
            {
                get
                {
                    if (this.Name == "Обособленный участок") return this.FParentCN;
                    if (this.Name == "Условный участок") return this.FParentCN;
                    return null;
                }

                set
                {
                    /*
                    if (this.Name == "Обособленный участок") this.FParentCN = value;

                    if (this.Name == "Условный участок") this.FParentCN = value; 
                       else this.FParentCN = null;
                     * */
                    this.FParentCN = value;
                }
            }

            public IGeometry GetEs(long Feature_id)
            {
                //same ES
                if (this.EntSpat.id == Feature_id)
                    return this.EntSpat;
                //same feature in ES
                foreach (IGeometry Feauture in this.EntSpat)
                {
                    if (Feauture.id == Feature_id)
                        return Feauture;
                }

                //Same point in ES:
                foreach (IGeometry item in this.EntSpat)
                {

                    if (item is TPolygon)
                    {
                        TPoint pt = ((TPolygon)item).GetPoint(Feature_id);
                        if (pt != null) return pt;

                        foreach (TRing child in ((TPolygon)item).Childs)
                        {
                            TPoint pt1 = child.GetPoint(Feature_id);
                            if (pt1 != null) return pt1;
                        }
                    }

                    if (item is IPointList)
                    {
                        TPoint pt = ((IPointList)item).GetPoint(Feature_id);
                        if (pt != null) return pt;
                    }
                }
                //From Slot(s)
                if (this.SubParcels != null) 
                    if (this.SubParcels.GetEs(Feature_id) != null) 
                        return this.SubParcels.GetEs(Feature_id);
                return null;
            }

            /// <summary>
            /// Проверка пересечений 
            /// </summary>
            /// <param name="Layer_id"></param>
            /// <returns></returns>
            public PointList CheckEs(TPolygon ES)
            {
                switch (this.Name)
                {
                    /* TODO : recode func:
                    case "Землепользование": { return this.EntitySpatial.FindClip(ES); }
                    case "Обособленный участок": { return this.EntitySpatial.FindClip(ES); }
                    case "Условный участок": { return this.EntitySpatial.FindClip(ES); }
                    case "Многоконтурный участок": { return this.Contours.CheckES(ES); }

                    // case "Единое землепользование": { return this.CompozitionEZ.CheckES(ES); }
                    */
                    case "Значение отсутствует": { return null; }
                    default: return null;
                }
            }

            public PointList CheckEs(TPolygonCollection Contours)
            {
                switch (this.Name)
                {
                    /*
                    case "Землепользование": { return this.EntitySpatial.FindClip(Contours); }
                    case "Обособленный участок": { return this.EntitySpatial.FindClip(Contours); }
                    case "Условный участок": { return this.EntitySpatial.FindClip(Contours); }
                    //case "Многоконтурный участок": { return this.Contours.CheckESs(Contours); }
                    //  case "Единое землепользование": { return this.CompozitionEZ.CheckES(Contours); }
                    */
                    case "Значение отсутствует": { return null; }
                    default: return null;
                }
            }

            public string Area(string Format)
            {
                switch (this.Name)
                {
                    /* TODO kill
                    case "Землепользование": { return this.EntitySpatial.AreaSpatialFmt(Format); }
                    case "Обособленный участок": { return this.EntitySpatial.AreaSpatialFmt(Format); }
                    case "Условный участок": { return this.EntitySpatial.AreaSpatialFmt(Format); }
                    case "Многоконтурный участок": { return this.Contours.AreaSpatialFmt(Format, true); }
                        */

                    //   case "Единое землепользование": { return this.CompozitionEZ.AreaSpatialFmt(Format, true); }
                    case "Значение отсутствует": { return ""; }
                    default: return this.EntSpat.AreaSpatialFmt(Format, true);
                }
            }

            public double Area_float
            {
                get
                {
                    switch (this.Name)
                    {
                        /*
                        case "Землепользование": { return this.EntitySpatial.AreaSpatial; }
                        case "Обособленный участок": { return this.EntitySpatial.AreaSpatial; }
                        case "Условный участок": { return this.EntitySpatial.AreaSpatial; }
                        case "Многоконтурный участок": { return this.Contours.AreaSpatial; }
                     //   case "Единое землепользование": { return this.CompozitionEZ.AreaSpatial; }
                     */
                        case "Значение отсутствует": { return -1; }
                        default: return this.EntSpat.AreaSpatial;
                    }
                }
            }
        }

        public class TSlot
        {
            private int Fid;
            public string NumberRecord;

            public string AreaGKN;
            public Rosreestr.TMyEncumbrances Encumbrances;
            /// <summary>
            /// Just Polygon
            /// </summary>
            public TPolygon SpatialElement;
            /// <summary>
            /// Anything spatial features
            /// </summary>
            public TEntitySpatial ES;
            public TSlot()
            {
                this.SpatialElement = new TPolygon();
                this.ES = new TEntitySpatial();
                this.Encumbrances = new Rosreestr.TMyEncumbrances();
                this.Fid = Gen_id.newId;
            }
            public int id { get { return this.Fid; } }
            // public string EncumbranceType { get { if (this.Encumbrance != null) return this.Encumbrance.Type; return ""; } }


        }

        public class TSlots : BindingList<TSlot>
        {
            public IGeometry GetEs(long Layer_id)
            {
                for (int i = 0; i <= this.Items.Count - 1; i++)
                {
                    if (this.Items[i].SpatialElement.id == Layer_id)
                        return this.Items[i].SpatialElement;
                    //otherwise, multicontour spatial slot:
                    if (this.Items[i].ES.id == Layer_id)
                        return this.Items[i].ES;

                    for (int ic = 0; ic <= this.Items[i].ES.Count - 1; ic++)
                        if ((this.Items[i].ES[ic].id == Layer_id) &&
                                (this.Items[i].ES[ic].TypeName == NetFteoTypes.Polygon))
                            return this.Items[i].ES[ic];
                }
                return null;
            }
        }

        public class TRealtys : List<TRealEstate>
        {
            public TRealEstate GetItem(int item_id)
            {
                foreach (TRealEstate item in this)
                {
                    if (item.id == item_id)
                        return item;
                }
                return null;
            }

            public Object GetEs(int Layer_id)
            {
                for (int i = 0; i <= this.Count - 1; i++)
                {
                    /* TODO kill, goes to ES2
                    if (this[i].Building != null)

                        if ((this[i]).Building.ES != null)
                        {

                            if ((this[i]).Building.ES.GetType().Name == "TPolyLines")
                            {
                                if (((TPolyLines)(this[i]).Building.ES).ParentID == Layer_id)
                                    return (TPolyLines)(this[i]).Building.ES;
                            }

                            if ((this[i]).Building.ES.GetType().Name == "TPolyLines")
                            {
                                TPolyLines ES = (TPolyLines)this[i].Building.ES;

                                for (int il = 0; il <= ES.Count - 1; il++)
                                    if (ES[il].id == Layer_id)
                                        return ES[il];
                            }

                            if (this[i].Building.ES.GetType().Name == "TPolygon")
                                if (((TPolygon)this[i].Building.ES).id == Layer_id)
                                    return (TPolygon)this[i].Building.ES;
                        }


                    if (this[i].Construction != null)

                        if ((this[i]).Construction.ES != null)
                        {

                            if ((this[i]).Construction.ES.GetType().Name == "TPolyLines")
                            {
                                if (((TPolyLines)(this[i]).Construction.ES).ParentID == Layer_id)
                                    return (TPolyLines)(this[i]).Construction.ES;
                            }

                            if ((this[i]).Construction.ES.GetType().Name == "TPolyLines")
                            {
                                TPolyLines ES = (TPolyLines)this[i].Construction.ES;

                                for (int il = 0; il <= ES.Count - 1; il++)
                                    if (ES[il].Layer_id == Layer_id)
                                        return ES[il];
                            }

                            if (this[i].Construction.ES.GetType().Name == "TPolygon")
                                if (((TPolygon)this[i].Construction.ES).Layer_id == Layer_id)
                                    return (TPolygon)this[i].Construction.ES;
                        }


                    if (this[i].Uncompleted != null)
                        if ((this[i]).Uncompleted.ES != null)
                        {

                            if ((this[i]).Uncompleted.ES.GetType().Name == "TPolyLines")
                            {
                                if (((TPolyLines)(this[i]).Uncompleted.ES).ParentID == Layer_id)
                                    return (TPolyLines)(this[i]).Uncompleted.ES;
                            }

                            if ((this[i]).Uncompleted.ES.GetType().Name == "TPolyLines")
                            {
                                TPolyLines ES = (TPolyLines)this[i].Uncompleted.ES;

                                for (int il = 0; il <= ES.Count - 1; il++)
                                    if (ES[il].id == Layer_id)
                                        return ES[il];
                            }

                            if (this[i].Uncompleted.ES.GetType().Name == "TPolygon")
                                if (((TPolygon)this[i].Uncompleted.ES).id == Layer_id)
                                    return (TPolygon)this[i].Uncompleted.ES;
                                        */

                    //again for ES2 (common spatial data collection)
                    if ((this[i].EntSpat != null) &&
                        (this[i].EntSpat.id == Layer_id))
                        return this[i].EntSpat;

                    if (this[i].EntSpat != null)
                    {
                        foreach (IGeometry feature in (this[i]).EntSpat)
                        {
                            //string test = feature.GetType().Name;
                            if (feature.GetType().Name == "TCircle")
                            {
                                if (((TCircle)feature).id == Layer_id)
                                    return (TCircle)feature;
                            }

                            if (feature.GetType().Name == "TPolyLine")
                            {
                                if (((TPolyLine)feature).id == Layer_id)
                                    return (TPolyLine)feature;
                            }

                            if (feature.GetType().Name == "TPolygon")
                                if (((TPolygon)feature).id == Layer_id)
                                    return (TPolygon)feature;
                        }

                    }

                }
                return null;
            }
            public void ShowasListItems(System.Windows.Forms.ListView LV, bool SetTag, bool ClearItems = false)
            {
                //if (PointCount == 0) return;
                LV.BeginUpdate();
                if (ClearItems)
                LV.Items.Clear();
                LV.Controls.Clear();
                LV.View = System.Windows.Forms.View.Details;
                LV.Columns[0].Text = "КН";
                LV.Columns[1].Text = "Address";
               // LV.Columns[2].Text = "-";
               // LV.Columns[3].Text = "-";
               // LV.Columns[4].Text = "-";
               // LV.Columns[5].Text = "-";
               // LV.Columns[6].Text = "-";
                // if (SetTag) LV.Tag = id;

                foreach (TRealEstate Parcel in this)
                {
                    System.Windows.Forms.ListViewItem LViItem = new System.Windows.Forms.ListViewItem();
                    LViItem.Text = Parcel.CN;
                    LV.Items.Add(LViItem);
                    LViItem.SubItems.Add(Parcel.Location.Address.AsString(true));
     
                    LViItem.SubItems.Add(Parcel.ObjectType);
                    if (Parcel.Building != null)
                        LViItem.SubItems.Add(Parcel.Building.AssignationBuilding);
                    if (Parcel.Construction != null)
                        LViItem.SubItems.Add(Parcel.Construction.AssignationName);
                    if (Parcel.Uncompleted != null)
                        LViItem.SubItems.Add(Parcel.Uncompleted.AssignationName);
                    if (Parcel.Name != "")
                        LViItem.SubItems.Add(Parcel.Name);
                    if (Parcel.KeyParameters.Count > 0)
                    {
                        LViItem.SubItems.Add(Parcel.KeyParameters[0].Type); //Keyparams ?
                        LViItem.SubItems.Add(Parcel.KeyParameters[0].Value); //Keyparams ?
                    }
                }
                LV.EndUpdate();
            }
        }

        /// <summary>
        /// Real estate - OKS, property etc
        /// </summary>
        public class TRealEstate : TCadasterItem2
        {
            private string fObjectType;
            public string CadastralBlock;
            public List<string> ParentCadastralNumbers; //
                                                        //public decimal Area;
            public string Type;
            public string Floors;
            public string UndergroundFloors;
            public List<TElementConstruct> ElementsConstruct;
            public Rosreestr.TMyRights Rights; // как бы ГКН-Права
            public Rosreestr.TMyRights EGRN;
            public TEntitySpatial EntSpat; //Spatial data
                                           //SubTypes:
            public TBuilding Building;
            public TFlat Flat;
            public TConstruction Construction;
            public TUncompleted Uncompleted;

            public string ObjectType
            {
                set { this.fObjectType = value; }
                get
                {
                    switch (this.fObjectType)
                    {
                        case "002001002000":
                            {
                                return "Здание";
                            }
                        case "002001004000":
                            return "Сооружение";
                        case "002001005000":
                            return "ОНС";
                        default: return this.fObjectType; 
                    }
                }
            }

            // public Rosreestr.TAddress Address;
            public TRealEstate()
            {
                this.id = Gen_id.newId;
                this.ParentCadastralNumbers = new List<string>();
                this.Location = new Rosreestr.TLocation();
                this.EntSpat = new TEntitySpatial();
                ElementsConstruct = new List<TElementConstruct>();
            }

            public TRealEstate(string cn) : this()
            {
                this.CN = cn;
            }

            public TRealEstate(string cn, Rosreestr.dRealty_v03 rlt_type) : this(cn)
            {
                switch (rlt_type)
                {
                    case Rosreestr.dRealty_v03.Здание: { this.Building = new TBuilding(); break; }
                    case Rosreestr.dRealty_v03.Помещение: { this.Flat = new TFlat(cn); break; }
                    case Rosreestr.dRealty_v03.Сооружение: { this.Construction = new TConstruction(); break; }
                    case Rosreestr.dRealty_v03.Объект_незавершённого_строительства: { this.Uncompleted = new TUncompleted(); break; }
                    default: this.Building = new TBuilding(); break;
                }
            }

            /// <summary>
            /// Contructor for KPT11 realty`s
            /// </summary>
            /// <param name="cn"></param>
            /// <param name="type_code"></param>
            public TRealEstate(string cn, string type_code) : this(cn)
            {
                this.ObjectType = type_code;
                switch (type_code)
                {
                    case "002001002000":
                        {
                            this.Building = new TBuilding();
                            break;
                        }
                    case "002001004000":
                        { this.Construction = new TConstruction(); break; }
                    case "002001005000":
                        { this.Uncompleted = new TUncompleted(); break; }

                    default: this.Building = new TBuilding(); break;
                }
            }

            public TKeyParameters KeyParameters
            {
                get
                {
                    if (this.Building != null) return Building.KeyParameters;
                    if (this.Construction != null) return Construction.KeyParameters;
                    if (this.Uncompleted != null) return Uncompleted.KeyParameters;
                    if (this.Flat != null) return Flat.KeyParameters;
                    return null;
                }
            }

        }

        public class TCadasterItem : Geometry
        {

            public string CN;
            public string DateCreated;
            public decimal CadastralCost;
            //     public TCadasterItem Parent;

            public TCadasterItem()
            {
                this.CN = "";
                this.id = Gen_id.newId;
                //this.Parent = new TCadasterItem(); //causes StackOverFlow
            }
            public TCadasterItem(string cn)
            {
                this.CN = cn;
                this.id = Gen_id.newId;
            }

            public TCadasterItem(string cn, int item_id)
            {
                this.CN = cn;
                this.id = item_id;
            }

        }

        //Для оксов отдельный кадастровый юнит, №2
        public class TCadasterItem2 : TCadasterItem
        {
            public TKeyParameters OldNumbers;
            // TP/Building/Package/NewBuildings/NewBuilding/ElementsConstruct/Material/@Wall
            public decimal Area;
            public string Notes; // Особые отметки
                                 //      public Rosreestr.TAddress Address;
            public Rosreestr.TLocation Location;
            public TCadasterItem2()
            {
                this.OldNumbers = new TKeyParameters();
            }
        }

        public class TFlat : TCadasterItem2
        {
            private string fAssignationCode;
            private string fAssignationType;
            public TKeyParameters KeyParameters; // 
            public TPositionInObject PositionInObject;
            /// <summary>
            /// Назначение помещения
            /// </summary>
            public string AssignationCode
            {
                get
                {
                    switch (fAssignationCode)
                    {
                        case "Item206001000000": { return "Нежилое помещение"; }
                        case "Item206002000000": { return "Жилое помещение"; }
                        default: { return ""; }
                    }
                }
                set
                {
                    this.fAssignationCode = value;
                }
            }

            /// <summary>
            ///  Вид помещения
            /// </summary>
            public string AssignationType
            {
                get
                {
                    switch (fAssignationType)
                    {
                        case "Item205001000000": { return "Квартира"; }
                        case "Item205002000000": { return "Комната"; }
                        default: { return ""; }
                    }
                }

                set
                {
                    this.fAssignationType = value;
                }
            }

            public TFlat(string cn)
            {
                this.PositionInObject = new TPositionInObject();
                this.CN = cn;
                this.Location = new Rosreestr.TLocation();
                this.KeyParameters = new TKeyParameters();
            }
        }

        public class TFlats : List<TFlat>
        {

            public decimal TotalArea
            {
                get
                {
                    decimal Sum = 0;
                    foreach (TFlat flat in this)
                        Sum += flat.Area;
                    return Sum;
                }
            }

            public int CountbyType(string type)
            {
                int res = 0;
                foreach (TFlat fl in this)
                {
                    if (fl.AssignationType == type)
                        res++;
                }
                return res;
            }

            public int CountbyCode(string code)
            {
                int res = 0;
                foreach (TFlat fl in this)
                {
                    if (fl.AssignationCode == code)
                        res++;
                }
                return res;
            }

            public decimal AreabyCode(string code)
            {
                decimal res = 0;
                foreach (TFlat fl in this)
                {
                    if (fl.AssignationCode == code)
                        res += fl.Area;
                }
                return res;
            }

            public decimal Area
            {
                get
                {
                    decimal Sum = 0;
                    foreach (TFlat flat in this)
                        Sum += flat.Area;
                    return Sum;
                }
            }


            public void AddFlat(TFlat flat)
            {
                this.Add(flat);
            }
        }

        public class TConstruction : TCadasterItem2
        {
            //private Object fEntitySpatial; 
            public string AssignationName;  // Назначение сооружения; 
                                            //public TPolygon EntitySpatial; //Может быть многоконтурным???
            public TKeyParameters KeyParameters; // 
                                                 /*
                                                 public Object ES
                                                 {
                                                     get { return this.fEntitySpatial; }
                                                     set
                                                     {
                                                         if (value == null) return;
                                                         string test = value.GetType().Name;

                                                         if (value.GetType().Name == "TPolygon")
                                                             this.fEntitySpatial = (TPolygon)value;

                                                         if (value.GetType().Name == "TPolyLines")
                                                             this.fEntitySpatial = (TPolyLines)value;

                                                         if (value.GetType().Name == "TCircle")
                                                             this.fEntitySpatial = (TCircle)value;
                                                     }
                                                 }
                                                 */
            public TConstruction()
            {
                this.KeyParameters = new TKeyParameters();
            }
        }

        public class TUncompleted : TCadasterItem2
        {


            public string AssignationName;  // Проектируемое назначение
            public TKeyParameters KeyParameters; // 
            public string DegreeReadiness; //Степень готовности в процентах

            /*
                    private Object fEntitySpatial; //Может быть многоконтурным???
                    public Object ES
                    {
                        get { return this.fEntitySpatial; }
                        set
                        {
                            if (value == null) return;
                            string test = value.GetType().Name;

                            if (value.GetType().Name == "TPolygon")
                                this.fEntitySpatial = (TPolygon)value;

                            if (value.GetType().Name == "TPolyLines")
                                this.fEntitySpatial = (TPolyLines)value;
                        }
                    }
            */
            public TUncompleted()
            {
                this.KeyParameters = new TKeyParameters();
            }
        }

        public class TBuilding : TCadasterItem2
        {
            private string fAssBuilding;

            //public List<TFlat> Flats;//Кадастровые номера помещений, расположенных в объекте недвижимости
            public TKeyParameters KeyParameters; // 
            public TFlats Flats; // Помещения, расположенных в объекте недвижимости
                                 /// <summary>
                                 /// Кадастровый номер земельного участка (земельных участков), в пределах которого (которых) расположен данный объект недвижимости (сведения ГКН)
                                 /// </summary>

            public TBuilding()
            {
                //this.fEntitySpatial = new TPolygon();
                this.KeyParameters = new TKeyParameters();
                this.Flats = new TFlats();//new List<TFlat>();
            }

            public string AssignationBuilding
            {
                get { return netFteo.Rosreestr.dAssBuildingv01.ItemToName(fAssBuilding); }
                set { this.fAssBuilding = value; }
            }
        }

        public class TElementConstruct
        {
            public string WallMaterial;
            public TElementConstruct(string pWallMaterial)
            {
                WallMaterial = pWallMaterial;
            }
        }
        /// <summary>
        /// Расположение на плане
        /// </summary>
        public class Position
        {
            /// <summary>
            /// Номер на плане
            /// </summary>
            public string NumberOnPlan;
            /// <summary>
            /// Планы
            /// </summary>
            public List<string> Plans_Plan_JPEG;

            public Position()
            {
                this.Plans_Plan_JPEG = new List<string>();
            }
            /// <summary>
            /// Единственный план
            /// </summary>
            public string Plan00_JPEG
            {
                get
                {
                    if (this.Plans_Plan_JPEG.Count == 1)
                        return this.Plans_Plan_JPEG[0];
                    else return "";
                }
            }

        }

        public class TLevel
        {
            public string Type;
            public string Number;
            /// <summary>
            /// Расположение в пределах объекта недвижимости
            /// </summary>
            public Position Position;
            public TLevel(string type, string number, string numberonplan)
            {
                this.Number = number;
                this.Type = Rosreestr.dTypeStorey_v01.ItemToName(type);
                this.Position = new Position();
                this.Position.NumberOnPlan = numberonplan;
            }
            public void AddPlan(string jpegname)
            {
                this.Position.Plans_Plan_JPEG.Add(jpegname);
            }
        }

        public class TPositionInObject
        {
            public List<TLevel> Levels;
            public TPositionInObject()
            {
                this.Levels = new List<TLevel>();
            }

        }
        /// <summary>
        /// Основная характеристика
        /// </summary>
        public class TKeyParameter
        {
            public string Type; //Тип характеристики  -dTypeParameter_v01.xsd
            public string Value; //Значение (величина в метрах (кв. метрах для площади, куб. метрах для объема))
        }

        /// <summary>
        /// ХАРАКТЕРИСТИКИ ОБЪЕКТОВ КАПИТАЛЬНОГО СТРОИТЕЛЬСТВА
        /// </summary>
        public class TKeyParameters : List<TKeyParameter>
        {
            public TKeyParameters()
            {
                //this.KeyParameters = new TKeyParameters();
            }
            public void AddParameter(string type, string value)
            {
                TKeyParameter param = new TKeyParameter();
                param.Type = Rosreestr.dTypeParameter_v01.ItemToName(type);
                param.Value = value;
                this.Add(param);
            }
        }

        #endregion


        #region  Oбъекты землеустройства - зоны и границы
        /// <summary>
        /// //Границы между субъектами РФ, границы населенных пунктов, муниципальных образований, расположенных в кадастровом квартале
        /// </summary>
        public class TBound
        {
            public string Description;
            public string TypeName;
            public string AccountNumber;
            public int id;
            public TBound(string Descr, string typename)
            {
                this.Description = Descr;
                this.TypeName = typename;
                this.id = Gen_id.newId;
            }
            public TPolygon EntitySpatial;
        }

        public class TZone
        {
            const string TerritorialZone = "TerritorialZone";
            public int id;
            public string Description;
            public string AccountNumber;
            public string TypeName;
            public string ContentRestrictions; // SpecialZone
            public List<string> PermittedUses;
            public List<TDocument> Documents;

            public TEntitySpatial EntitySpatial; //TODO: Spatials may be multi ?

            /// <summary>
            /// As single geometry - polygon, obsolete field. Instead use EntitySpatial
            /// </summary>
            [Obsolete]
            public TPolygon SpatialElement; //TODO: Spatials may be multi ?
            
            public TZone(string accountnumber)
            {
                this.AccountNumber = accountnumber;
                this.Documents = new List<TDocument>();
                EntitySpatial = new TEntitySpatial();
                this.id = Gen_id.newId;
            }

            public void AddContentRestrictions(string contentrestrictions)
            {
                this.ContentRestrictions = contentrestrictions; ;
                this.TypeName = "Зона с особыми условиями использования территорий";
                this.PermittedUses = null;
            }

            public void AddPermittedUses(List<string> permitteduses)
            {
                if (this.PermittedUses == null) this.PermittedUses = new List<string>();
                this.PermittedUses.AddRange(permitteduses);
                this.TypeName = "Территориальная зона";
            }

            public void AddDocument(string number, string name,
                                    string codedocument, string issueorgan,
                                    string serial, string doc_date)
            {
                TDocument doc = new TDocument();
                doc.CodeDocument = codedocument;
                doc.Doc_Date = doc_date;
                doc.IssueOrgan = issueorgan;
                doc.Name = name;
                doc.Number = number;
                doc.Serial = serial;
                this.Documents.Add(doc);
            }

        }

        public class TZonesList : List<TZone>
        {
            /*
            public TPolygonCollection GetEs()
            {
                TPolygonCollection Res = new TPolygonCollection();
                for (int i = 0; i <= this.Count - 1; i++)
                {
                    if (this[i].EntitySpatial != null)
                        Res.AddPolygon(this[i].EntitySpatial);
                }
                return Res;
            }
            */
            public Object GetEsId(int Layer_id)
            {

                for (int i = 0; i <= this.Count - 1; i++)
                {
                    if ((this[i].SpatialElement != null) && (this[i].SpatialElement.id == Layer_id))
                        return this[i].SpatialElement;
                    if ((this[i].EntitySpatial != null) && (this[i].EntitySpatial.FeatureExists(Layer_id)))
                        return this[i].EntitySpatial.GetFeature(Layer_id);
                    if ((this[i].EntitySpatial != null) && (this[i].EntitySpatial.id == Layer_id))
                        return this[i].EntitySpatial;
                }
                return null;
            }

            public void AddZone(TZone zone, string contentrestrictions)
            {
                this.Add(zone);
                zone.ContentRestrictions = contentrestrictions;
            }
            public void AddZone(TZone zone, List<string> PermittedUses)
            {
                this.Add(zone);
                zone.PermittedUses.AddRange(PermittedUses);
            }

            /// <summary>
            /// Parse spatial data for all items in collection
            /// </summary>
            public void ParseSpatial()
            {
                foreach(TZone item in this)
                {
                    //item.EntitySpatial.Definition = ;
                    item.EntitySpatial.ParseSpatial(item.AccountNumber);
                    
                }
            }
        }

        public class TBoundsList : List<TBound>
        {
            public TPolygon GetEs(int Layer_id)
            {
                for (int i = 0; i <= this.Count - 1; i++)
                {
                    if (this[i].EntitySpatial.id == Layer_id)
                        return this[i].EntitySpatial;
                }

                return null;
            }
        }
        #endregion


        #region Addititonal classes
        public class TCadastralNumbers : List<string>
        {
        }

        public class EZPEntry : TCadasterItem
        {
            public long Spatial_ID;
            public decimal AreaEntry;
        }

        public class TCompozitionEZ : List<EZPEntry>// : List<TPolygon>
        {
            public TCadastralNumbers DeleteEntryParcels;
            public TCadastralNumbers TransformationEntryParcel;
            public TCompozitionEZ()
            {
                this.DeleteEntryParcels = new TCadastralNumbers();
                this.TransformationEntryParcel = new TCadastralNumbers();
            }



            /// <summary>
            /// Периметр всех входящих в ЕЗП. 
            /// Замозговано 11-04-18:
            /// ДА уж.... такая нужная //уйня. Росреестр без нее никак 
            /// </summary>
            /*
            public double TotalPerimeter
            {
                get
                {
                    double pery = 0;
                    foreach (TPolygon poly in this)
                        pery += poly.Length;
                    return pery;
                }
            }
            */
        }

        /// <summary>
        /// Справочник dUtilizations_v01.xsd
        /// </summary>
        public class Utilization
        {
            string fUtil; // Значение по классификатору dUtilizations_v01.xsd
            public string UtilbyDoc;
            public bool UtilizationSpecified;
            public Utilization() { UtilizationSpecified = false; }
            public string Untilization
            {
                get
                { return this.fUtil; }
                set
                {
                    //this.fUtil = netFteo.Rosreestr.dUtlizationsv01.ItemToName(value);
                    this.fUtil = value;
                    UtilizationSpecified = true;
                }
            }
        }

        /// <summary>
        /// dAllowedUse_v01
        /// Вид разрешенного использования земельного участка в соответствии с классификатором, 
        /// утвержденным приказом Минэкономразвития России от 01.09.2014 № 540.
        /// </summary>
        public class LandUse
        {
            public string Land_Use;   //Вид разрешенного использования участка по классификатору видов разрешенного использования земельных участков dAllowedUse
            public string DocLandUse; //Реквизиты документа, устанавливающего вид разрешенного использования земельного участка
        }

        #endregion

    }
}
