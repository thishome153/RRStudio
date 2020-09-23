﻿//----- -------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by Я.
//
//     Утилиты к XSD схемам РосРеестра
// 
// </autogenerated>
using System.Collections.Generic;

namespace netFteo.Rosreestr
{
    public enum dFileTypes
    {
        KPT11 = 111,
        KPT10 = 110,
        KPT09 = 109,
        KPT08 = 108,
        KPT07 = 107,
        KPT06 = 106,
        KPT05 = 105,
        /// <summary>
        /// KPT family older KPT11
        /// </summary>
        /*
        KPT = KPT05 | KPT06| KPT07 |KPT08 | KPT09| KPT10,

 *         */
        KVZU_04 = 204,
        KVZU_05 = 205,
        KVZU_06 = 206,
        KVZU_07 = 207,

        EGRP_04 = 304,
        EGRP_06 = 306,

        KPOKS_03 = 403,
        KPOKS_04 = 404,
        KVOKS_07 = 507,
//        KVOKS_03 = 2031, // ???

        KPZU_05 = 605,
        KPZU_06 = 606,

        Undefined = -1
    }

    public static class NameSpaces
    {
        public static string KVZU_04 = "urn://fake/kvzu/4.0.0";
        public static string KVZU_05 = "urn://fake/kvzu/5.0.0";
        public static string KVZU_06 = "urn://x-artefacts-rosreestr-ru/outgoing/kvzu/6.0.9";
        public static string KVZU_07  = "urn://x-artefacts-rosreestr-ru/outgoing/kvzu/7.0.1";
        public static string KPZU_06  = "urn://x-artefacts-rosreestr-ru/outgoing/kpzu/6.0.1";
        public static string KPZU_05 = "urn://x-artefacts-rosreestr-ru/outgoing/kpzu/5.0.8";
        public static string KVOKS_07 = "urn://x-artefacts-rosreestr-ru/outgoing/kvoks/3.0.1";
        public static string KPOKS_03 = "urn://fake/kpoks/3.0.0";
        public static string KPOKS_04 = "urn://x-artefacts-rosreestr-ru/outgoing/kpoks/4.0.1";
        public static string KPT05    = "urn://fake/kpt/5.0.0";
        public static string KPT06    = "urn://fake/kpt/6.0.0";
        public static string KPT07    = "urn://fake/kpt/7.0.0";
        public static string TP06 = "urn://fake/TP/6.0.0";
        public static string KPT08    = "urn://fake/kpt/8.0.0";
        public static string KPT09    = "urn://x-artefacts-rosreestr-ru/outgoing/kpt/9.0.3";
        public static string KPT10    = "urn://x-artefacts-rosreestr-ru/outgoing/kpt/10.0.1";
        public static string KPT11    = "urn://fake/kpt/11.0.0";
        public static string EGRP_04 = "urn://fake/egrp/4.0.0";
        public static string EGRP_06 = "urn://fake/egrp/6.0.0";

        public static string FileTypeToNS(netFteo.Rosreestr.dFileTypes ftype)
        {

            Dictionary<dFileTypes, string> DocumentTypes = new Dictionary<dFileTypes, string>()
            {
              { dFileTypes.KVZU_06,KVZU_06},
              { dFileTypes.KVZU_07,KVZU_07},
              { dFileTypes.KPZU_06,KPZU_06},
              { dFileTypes.KVOKS_07,KVOKS_07},
              { dFileTypes.KPZU_06,KPZU_06},
              { dFileTypes.KPT05,KPT05},
              { dFileTypes.KPT06,KPT06},
              { dFileTypes.KPT07,KPT07},
              { dFileTypes.KPT08,KPT08},
              { dFileTypes.KPT09,KPT09},
              { dFileTypes.KPT10,KPT10},
              { dFileTypes.KPT11,KPT11}
                };
            string Test = DocumentTypes[ftype];
            return Test;
        }

        public static dFileTypes NStoFileType(string ns)
        {
            if (ns == null) return dFileTypes.Undefined;

            Dictionary<string, dFileTypes > DocumentTypes = new Dictionary< string, dFileTypes>()
            {
              { EGRP_04,dFileTypes.EGRP_04},
              { EGRP_06,dFileTypes.EGRP_06},
              { KVZU_04,dFileTypes.KVZU_04},
              { KVZU_05,dFileTypes.KVZU_05},
              { KVZU_06,dFileTypes.KVZU_06},
              { KVZU_07,dFileTypes.KVZU_07},
              { KPZU_05,dFileTypes.KPZU_05},
              { KPZU_06,dFileTypes.KPZU_06},
              { KVOKS_07,dFileTypes.KVOKS_07},
              { KPOKS_03,dFileTypes.KPOKS_03},
              { KPOKS_04,dFileTypes.KPOKS_04},
              { KPT05,dFileTypes.KPT05},
              { KPT06,dFileTypes.KPT06},
              { KPT07,dFileTypes.KPT07},
              { KPT08,dFileTypes.KPT08},
              { KPT09,dFileTypes.KPT09},
              { KPT10,dFileTypes.KPT10},
              { KPT11,dFileTypes.KPT11}};

            if (DocumentTypes.ContainsKey(ns))
            {
                dFileTypes Test = DocumentTypes[ns];
                return Test;
            }
            else return dFileTypes.Undefined;
        }
    }

    public class DocumentType
    {

    }
  
    #region Cправочники для преобразований из xsd.enum 
    /// <summary>
    /// Справочник типов обьектов недвижимости
    /// dParcelsV01 -Cправочник "Имя земельного участка"
    /// </summary>
    public static class dParcelsv01
    {
        public static string ItemToName(string Item)
        {
            if (!Item.Contains("Item")) Item = "Item" + Item; // допишем Item
            Dictionary<string, string> items = new Dictionary<string, string>()
            {
                { "Item01","Землепользование"},
                { "Item0101","Квартал"}, // Дополнительные
                { "Item0102","Территориальная Зона"}, // Дополнительные
                { "Item0103","Зона Охранная"},// Дополнительные
                { "Item0104","Здание"},// Дополнительные
                { "Item0105","Сооружение"},// Дополнительные
                { "Item0106","НезавершенныйОКС"},// Дополнительные
                { "Item02","Единое землепользование"},
                { "Item03","Обособленный участок"},
                { "Item04","Условный участок"},
                { "Item05","Многоконтурный участок"},
                { "ItemDXF","Полигоны dxf"},
                { "ItemMIF","Полигоны mif"},
                { "Itemnefteo::TMyPolygon","Полигон"},
                { "Item06","Значение отсутствует"},
				// KPT11 types:
				{ "Item002001001000","Землепользование"},
            };
            if (Item != null)
                return items[Item];
            else return null;
        }

    }




/// <summary>
/// Справочник типов обьектов недвижимости
/// dParcelsV01 -Перечисление "Имя земельного участка"
/// </summary>
public enum dParcelsv01_enum
    {
        Землепользование = 01,
        Квартал = 0101,
        Территориальная_Зона = 0102,
        Зона_Охранная = 0103,
        Здание = 0104,
        Сооружение = 0105,
        НезавершенныйОКС = 0106,
        Единое_землепользование = 02,
        Обособленный_участок = 03,
        Условный_участок = 04,
        Многоконтурный_участок = 05,
        Значение_отсутствует = 06
    }


	public enum dStatesv01_enum
	{
		Ранее_учтенный = 01,
		Временный = 05,
		Учтенный = 06,
		Снят_с_учета = 07,
		Аннулированный = 08,
	}



	/// <summary>
	/// Виды объектов государственного кадастра недвижимости (ГКН) и 
	/// Единого государственного реестра прав на недвижимое имущество и сделок с ним (ЕГРП)
	/// urn://x-artefacts-rosreestr-ru/commons/directories/realty/3.0.1
	/// </summary>
	public enum dRealty_v03
    {
        Земельный_участок = 002001001000,
        Здание     = 002001002000,
        Помещение  = 002001003000,
        Сооружение = 002001004000,
        Линейное_сооружение_расположенное_более_чем_в_одном_кадастровом_округе = 002001004001,
        Условная_часть_линейного_сооружения = 002001004002,
        Объект_незавершённого_строительства = 002001005000,
        Предприятие_как_имущественный_комплекс_ПИК = 002001006000
    }

    /// <summary>
    /// Тип основного параметра ОКС
    /// </summary>
    public static class dTypeParameter_v01
    {
        public static string ItemToName(string Item)
        {

            switch (Item)
            {
                case "Item01": return "Протяженность";
                case "Item02": return "Глубина";
                case "Item03": return "Жилое помещение";
                case "Item04": return "Высота";
                case "Item05": return "Площадь";
                case "Item06": return "Площадь застройки";
                case "Item07": return "Глубина залегания";
                default: return Item;
            }
        }
    }

    public static class dOldNumber_v01
    {
        public static string ItemToName(string Item)
        {

            switch (Item)
            {
                case "Item01": return "Инвентарный номер";
                case "Item02": return "Условный номер";
                case "Item03": return "Кадастровый номер";
                case "Item99": return "Иной номер";
                default: return Item;
            }
        }
    }

    public static class dTypeStorey_v01
    {
        public static string ItemToName(string Item)
        {

            switch (Item)
            {
                case "Item01": return "Этаж";
                case "Item02": return "Мансарда";
                case "Item03": return "Мезонин";
                case "Item04": return "Подвал";
                case "Item05": return "Техническое подполье";
                case "Item06": return "Цокольный этаж";
                case "Item07": return "Надстроенный этаж";
                case "Item08": return "Технический этаж";
                case "Item09": return "Чердак";
                case "Item10": return "Антресоль";
                case "Item11": return "Светелка";
                case "Item12": return "Полуподвал";
                case "Item13": return "Антресоль подвала";
                case "Item14": return "Антресоль цокольного этажа";
                case "Item15": return "Чердачная надстройка";
                case "Item16": return "Подземный этаж";
                case "Item17": return "Значение  отсутствует";
                default: return Item;
            }
        }
    }
    public static class dAssBuildingv01
    {
       public static string ItemToName(string Item)
        {

            switch (Item)
            {
                case "Item204001000000": return "Нежилое здание";
                case "Item204002000000": return "Жилой дом"; // Дополнительные
                case "Item204003000000": return "Многоквартирный дом"; // Дополнительные
              
                default: return Item;
            }
        }
    }
    public static class dAssFlatv01
    {
        public static string ItemToName(string Item)
        {

            switch (Item)
            {
                case "Item206001000000": return "Нежилое помещение";
                case "Item206002000000": return "Жилое помещение"; 
                default: return Item;
            }
        }
    }
    public static class dRightsv01
    {
       public static string ItemToName(string Item)
        {

            switch (Item)
            {
                case "Item001001000000": return "Собственность";
                case "Item001002000000": return "Долевая собственность"; // Дополнительные
                case "Item001003000000": return "Совместная собственность"; // Дополнительные
                case "Item001004000000": return "Хозяйственное ведение";
                case "Item001005000000": return "Оперативное управление"; // Дополнительные
                case "Item001006000000": return "Пожизненное наследуемое владение"; // Дополнительные
                case "Item001007000000": return "Постоянное (бессрочное) пользование";
                case "Item001008000000": return "Сервитут (право)"; // Дополнительные
                case "Item001009900000": return "Иные права"; // Дополнительны
                default: return Item;
            }
        }
    }

     public static class dEncumbrancesv201
     {
         public static string ItemToName(string Item)
         {
             switch (Item)
             {
                 case "Item022001000000": return "Сервитут";
                 case "Item022001001000": return "Публичный сервитут";
                 case "Item022001002000": return "Частный сервитут";
                 case "Item022002000000": return "Арест";
                 case "Item022003000000": return "Запрещение";
                 case "Item022004000000": return "Ограничения прав на земельный участок, предусмотренные статьями 56, 56.1 Земельного кодекса Российской Федерации";
                 case "Item022005000000": return "Решение об изъятии земельного участка, жилого помещения";
                 case "Item022006000000": return "Аренда (в том числе, субаренда)";
                 case "Item022007000000": return "Ипотека";
                 case "Item022008000000": return "Ипотека в силу закона";
                 case "Item022009000000": return "Безвозмездное (срочное) пользование земельным/лесным участком";
                 case "Item022010000000": return "Доверительное управление";
                 case "Item022011000000": return "Рента";
                 case "Item022012000000": return "Запрет на совершение действий в сфере ГКУ в отношении ОН";
                 case "Item022099000000": return "Иные ограничения (обременения) прав";
                 default: return Item;
             }
         }
     }
    //    dEncumbrances_v03.xsd
    //    dEncumbrances_v02.xsd
    #endregion

    #region Классы для преобразований в схемах 
    /// <summary>
    /// Адрес (описание местоположения)
    /// </summary>
    public class TAddress
    {
        public string Note;
        public string Other;
        public string OKATO;
        public string KLADR;
        public string OKTMO;
        public string Region;  //dRegionsRF 
        public string District;
        public string City;  // есть в _AddressOut_v04.xsd, однако был пропущен
        public string Locality;
        public string Street;
        public string Level1;    // Дом
        public string Level2;    // корпус
        public string Level3;    // Строение
        public string Apartment; // Квартира, помещение

        public string AsString()
        {
            return this.City + " " + this.District + " " + this.Locality +
                               " " + this.Street + " " + this.Level1 + " " + this.Level2 +
                               " " + this.Level3 +
                               " " + this.Apartment;
        }
        /*
         * //Встроенный явный оператор для кастинга:
        public static explicit operator TAddress( kvzu07.tAddressOut v)
        {
            tAddressOut adr = new tAddressOut();
            adr.Apartment.Value = v.Apartment.Value;
            adr.Note = v.Note;
            return adr;


            //throw new NotImplementedException();
        }
        */
		public bool Empty
		{
			get
			{
				return ((this.Region == null) && (this.City == null) && (this.District == null));
			}
				}
    }

    /// <summary>
    /// Уточнение местоположения
    /// </summary>
    public class TElaboration
    {
        public int InBounds;
        /// <summary>
        /// Наименование ориентира
        /// </summary>
        public string ReferenceMark; //
        /// <summary>
        /// Расстояние
        /// </summary>
        public string Distance; //
        /// <summary>
        /// Направление
        /// </summary>
        public string Direction; //
        /*
         * установлено относительно ориентира, расположенного за пределами участка. 
         * Ориентир здание администрации с. Красногвардейское. 
         * Участок находится примерно в 25 км от ориентира по направлению на восток. 
         * */

         public bool Empty
        {
            get
            {
                return ((this.Distance == null) &&
                (this.Direction == null) &&
                (this.ReferenceMark == null));
            }
        }

        public string AsString()
        {
            if (this.Empty)  return null;
            Dictionary<int, string> InBoundsDic; //"В границах"
            InBoundsDic = new Dictionary<int, string>();
            InBoundsDic.Add(0, "установлено относительно ориентира, расположенного за пределами участка");
            InBoundsDic.Add(1, "Расположение ориентира в границах участка");
            InBoundsDic.Add(2, "Неопределено");
            if (this.Distance != null)
            {
                return
                    InBoundsDic[this.InBounds] + "." +
                    "Ориентир " + this.ReferenceMark + "." +
                    " Участок находится примерно в " + this.Distance + " от ориентира по направлению на " + this.Direction + ".";
            }
            else
            {
                return  InBoundsDic[this.InBounds] + ". Ориентир" + this.ReferenceMark;
            }

        }
    }


    /// <summary>
    /// Уточнение местоположения и адрес (описание местоположения) земельного участка
    /// </summary>
    public class TLocation
    {
        public TAddress Address;
        public TElaboration Elaboration;
        public string Inbounds
        {
            set
            {
                 // because expected value may  be like "Item0"
                this.Elaboration.InBounds = System.Convert.ToInt32(value.Replace("Item", ""));
            } 
            get { return this.Elaboration.AsString(); }
        }

        public TLocation()
        {
            this.Elaboration = new TElaboration();
			this.Address = new TAddress();
        }

        public string AsString()
        {
            if (this.Elaboration.AsString() != null)
            return this.Elaboration.AsString() + 
                            (this.Address != null ? " Почтовый адрес ориентира " + this.Address.Note : "");
            return null;
        }
    }


    /// <summary>
    /// Человекосущность, 1 штука
    /// </summary>
    public class HumanItem
    {
        public string FamilyName;
        public string FirstName;
        public string Patronymic;
        public string SNILS;
        public string INN;
        public string BirthDate;
        public string Sex;

    }

    /// <summary>
    /// Кадастровый инженер
    /// </summary>
    public class TEngineerOut : HumanItem
    {
        public string Date;
        public string NCertificate;
        public string Email;
        public string Organization_Name;
        public string AddressOrganization;
    }



    public class TMyOwner
   {
       public string OwnerName;
       public string ContactOwner;
       public TMyOwner()
       {
       }
       public TMyOwner(string Name_)
       {
           this.OwnerName = Name_;
       }

       public TMyOwner(string name_, string contactowner)
       {
           this.OwnerName = name_;
           this.ContactOwner = contactowner;
       }

   }

   public class TMyDocument
   {
       public string DocName;
       public string Number;
       public string Date;
   }

   public class TMyEncumbrance
   {
       private string FType;
       public List<TMyOwner> Owners;
       public string Name;
       public string RegDate;
       public string DurationStarted;
       public string DurationStopped;
       public string DurationTerm;   //Продолжительность
       public string RegNumber;
       public string Desc;
       public string AccountNumber; //Учётный номер зоны ГКН, для чзу под охр.зонами например или Кадастровый номер ЗУ, в пользу которого установлен сервитут
       public TMyDocument Document; // окумента, на основании которого возникло ограничение
       public TMyEncumbrance()
       {
           this.Owners = new List<TMyOwner>();
           this.Document = new TMyDocument();
       }
       public string Type
       {
           get { return dEncumbrancesv201.ItemToName(this.FType); }
           set { this.FType = value; }
       }
   }

    public class Contractor
    {        /// <summary />
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        //string FamilyName;
        //   string Patronymic;
        /// string Telephone;
        //string Email;
        //  string SNILS;

        private string familyNameField;
        private string firstNameField;
        private string patronymicField;
        private string SNILSField;
        private string EmailField;
        private string TelephoneField;
        private string nCertificateField;
        private string addressField;

        public string FamilyName
        {
            get
            {
                return this.familyNameField;
            }
            set
            {
                this.familyNameField = value;
                this.RaisePropertyChanged("FamilyName");
            }
        }
        public string FirstName
        {
            get
            {
                return this.firstNameField;
            }
            set
            {
                this.firstNameField = value;
                this.RaisePropertyChanged("FirstName");
            }
        }
        public string Patronymic
        {
            get
            {
                return this.patronymicField;
            }
            set
            {
                this.patronymicField = value;
                this.RaisePropertyChanged("Patronymic");
            }
        }
        public string SNILS
        {
            get
            {
                return this.SNILSField;
            }
            set
            {
                this.SNILSField = value;
                this.RaisePropertyChanged("SNILS");
            }
        }
        public string Email
        {
            get
            {
                return this.EmailField;
            }
            set
            {
                this.EmailField = value;
                this.RaisePropertyChanged("Email");
            }
        }
        public string Telephone
        {
            get
            {
                return this.TelephoneField;
            }
            set
            {
                this.TelephoneField = value;
                this.RaisePropertyChanged("Telephone");
            }
        }
        public string NCertificate
        {
            get
            {
                return this.nCertificateField;
            }
            set
            {
                this.nCertificateField = value;
                this.RaisePropertyChanged("NCertificate");
            }
        }
        public string Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
                this.RaisePropertyChanged("Address");
            }
        }
    }



    public class GeneralCadWorks
    {
        public Contractor Contractor;
        public string DateCadastral;
        public GeneralCadWorks() => Contractor = new Contractor();
    }

   public class TRight
   {
       private string FType;
       public List<TMyOwner> Owners; 
       public string Name;
       public string RegDate;
       public string RegNumber;
       public string ShareText; //Значение доли текстом. Есть Выбор в Схеме ShareText либо Share
       public string Desc; //Особые отметки
       public List<TMyEncumbrance> Encumbrances;
       public TRight()
       {
           this.Owners = new List<TMyOwner>();
           this.Encumbrances = new List<TMyEncumbrance>();
       }

       public string Type
       {
           get { return dRightsv01.ItemToName(this.FType); }
           set { this.FType = value; }
       }

		public TRight(string Name) : this()
		{
			this.Name = Name;
		}

   }

   public class TMyRights : System.ComponentModel.BindingList<TRight> {
		public List<string> AsList()
		{
			List<string> aslist = new List<string>();
			string listedRight = "";
			for (int i = 0; i <= this.Count - 1; i++)
			{
				foreach (TMyOwner own in this[i].Owners)
				{
					listedRight = (own.OwnerName + "\t" + this[i].Name + "\t" + this[i].RegNumber +"\t" + this[i].RegDate+ "\t" + this[i].ShareText + "\t" + this[i].Desc);


					if (this[i].Encumbrances.Count > 0)
					{
					
						foreach (TMyEncumbrance enc in this[i].Encumbrances)
						{
							listedRight = listedRight + "\t" + enc.Name + ", " + enc.Desc;
							foreach (TMyOwner ow in enc.Owners)
								listedRight += " "+ ow.OwnerName + ",";
						}
					}
					aslist.Add(listedRight);
					listedRight = ""; // clear string
				}

			}
			return aslist;
		}
   
   
   }
   public class TMyEncumbrances : System.ComponentModel.BindingList<TMyEncumbrance> { }
}
#endregion

    

