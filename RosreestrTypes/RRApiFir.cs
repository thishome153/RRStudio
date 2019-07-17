using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net; // http
using System.IO;

namespace RRTypes
{
	namespace FIR
	{

		#region FIR service data

		/// <summary>
		/// Class represents data, provided by services RR. 
		/// rosreestr.ru/api/online/fir_object/  instead of pkk5.rosreestr.ru/api/features
		/// </summary>
		public class FIRJsonData
		{


			public string objectId { get; set; }            // ИД объекта
			public string firActualDate { get; set; }       // дата актуализации в ФГИС ЕГРН
															/// <summary>
															/// Kраткие данные объекта
															/// </summary>
			public ObjectData objectData { get; set; }
			public ParcelData parcelData { get; set; }      // подробные данные для parcel
			public RealtyData realtyData { get; set; }      // данные об объекте недвижимости, полученные из ЕГРП; очень редко встречал
			public PremisesData premisesData { get; set; }  // подробные данные для premises
			public RightEncumbranceObject[] rightEncumbranceObjects { get; set; } // сведения о правах
			public OldNumber[] oldNumbers { get; set; }     // предыдущие номера

			/*
				{"objectId":"26:5:43433:7",
				"type":"parcel",
				"regionKey":126,
				"source":1,
				"firActualDate":"2016-12-08",

				"objectData":
				   {"id":"26:5:43433:7",
				   "regionKey":126,"
				   srcObject":1,
				   "objectType":"002009000000",
				   "objectName":"01",
				   "removed":0,
				   "dateLoad":"2016-08-19",
				   "addressNote":"край Ставропольский, р-н Труновский, с. Донское,, ул. Московская, дом 134, квартира 2",
				   "objectCn":"26:05:043433:7",
				   "objectCon":null,
				   "objectInv":null,
				   "objectUn":"26:05:043433:7",
				   "rsCode":"071082600201",
				   "actualDate":"2016-08-19",
				   "brkStatus":0,
				   "brkDate":null,
				   "formRights":null,
					"objectAddress":
							{"id":"26:5:43433:7",
							"regionKey":126,
							"okato":"07254804001",
							"kladr":"26025000001004000",
							"region":"26",
							"district":"Труновский",
							"districtType":"р-н",
							"place":null,
							"placeType":"неопр",
							"locality":"Донское",
							"localityType":"с",
							"street":"Московская",
							"streetType":"ул",
							"house":"134",
							"building":null,
							"structure":null,
							"apartment":"квартира 2",
							"addressNotes":"край Ставропольский, р-н Труновский, с. Донское,, ул. Московская, дом 134, квартира 2",
							"mergedAddress":"Труновский р-н, с Донское, ул Московская, д. 134, квартира 2"
							}
					},


			"parcelData":
				{"id":"26:5:43433:7",
				"regionKey":126,
				"parcelCn":"26:05:043433:7",
				"parcelStatus":"01",
				"dateCreate":"2001-04-25",
				"dateRemove":null,
				"categoryType":"003002000000",
				"areaValue":1557.0,
				"areaType":"009",
				"areaUnit":"055",
				"areaTypeValue":null,
				"areaUnitValue":null,
				"categoryTypeValue":"Земли населенных пунктов",
				"rightsReg":false,
				"cadCost":210739.95,
				"cadUnit":"383",
				"dateCost":"2015-11-29",
				"oksFlag":0,
				"oksType":null,
				"oksFloors":null,
				"oksUFloors":null,
				"oksElementsConstruct":null,
				"oksYearUsed":null,
				"oksInventoryCost":0.0,
				"oksInn":null,
				"oksExecutor":null,
				"oksYearBuilt":null,
				"oksCostDate":null,
				"rcType":null,
				"rcDate":null,
				"guidUl":null,
				"guidFl":null,
				"ciSurname":null,
				"ciFirst":null,
				"ciPatronymic":null,
				"ciNCertificate":null,
				"ciPhone":null,
				"ciEmail":null,
				"ciAddress":null,
				"coName":null,
				"coInn":null,
				"utilCode":"141003000000",
				"utilByDoc":"Для ведения личного подсобного хозяйства",
				"cadastralBlockId":"26:5:43433",
				"parcelStatusStr":"Ранее учтенный",
				"oksElementsConstructStr":null,
				"utilCodeDesc":"Для ведения личного подсобного хозяйства"
				},

			"realtyData":null,
			"premisesData":null,
			"rightEncumbranceObjects":null,
			"oldNumbers":[]
			}
			 */
		}

		public class ObjectData
		{
			public string id { get; set; }
			public int regionKey { get; set; }
			public int srcObject { get; set; }
			public string objectType { get; set; }
			public string objectName { get; set; }          // название объекта, например: "Жилой дом"
			public int removed { get; set; }
			public string dateLoad { get; set; }
			public string addressNote { get; set; }         // полный неструктурированный адрес
			public string objectCn { get; set; }            // полный кад.номер
			public string objectCon { get; set; }
			public string objectInv { get; set; }
			public string objectUn { get; set; }
			public string rsCode { get; set; }
			public string actualDate { get; set; }          // дата актуализации в ЕГРН
			public int brkStatus { get; set; }
			public string brkDate { get; set; }
			public string formRights { get; set; }
			public ObjectAddress objectAddress { get; set; }
		}

		/// <summary>
		/// Описывает подробные адресные данные объекта
		/// </summary>
		public class ObjectAddress
		{
			public string id { get; set; }
			public int regionKey { get; set; }
			public string okato { get; set; }
			public string kladr { get; set; }
			public string region { get; set; }
			public string district { get; set; }
			public string districtType { get; set; }
			public string place { get; set; }
			public string placeType { get; set; }
			public string locality { get; set; }
			public string localityType { get; set; }
			public string street { get; set; }              // название геонима
			public string streetType { get; set; }          // сокр.название типа геонима
			public string house { get; set; }               // номер дома
			public string building { get; set; }            // номер корпуса
			public string structure { get; set; }           // литера? строение?
			public string apartment { get; set; }           // номер квартиры
			public string addressNotes { get; set; }        // полный неструктурированный адрес
			public string mergedAddress { get; set; }       // адрес внутри нас.пункта (геоним и дальше)
		}

		public class ParcelData
		{
			public string id { get; set; }
			public int regionKey { get; set; }
			public string parcelCn { get; set; }            // полный кад.номер объекта
			public string parcelStatus { get; set; }        // код статуса объекта: "01" = "Ранее учтенный"
			public string dateCreate { get; set; }          // дата постановки на учет
			public object dateRemove { get; set; }          // дата снятия с учета
			public object categoryType { get; set; }        // код категории земель для ЗУ
			public decimal areaValue { get; set; }          // площадь объекта
			public string areaType { get; set; }
			public string areaUnit { get; set; }
			public object areaTypeValue { get; set; }
			public object areaUnitValue { get; set; }
			public string categoryTypeValue { get; set; }   // название категории земель для ЗУ
			public bool rightsReg { get; set; }             // признак зарегистрированности прав
			public decimal cadCost { get; set; }            // кадастровая стоимость
			public string cadUnit { get; set; }             // код ЕИ кад.стоимости по ОКЕИ, 383 = рубль
			public string dateCost { get; set; }            // дата утверждения кад.стоимости
			public int oksFlag { get; set; }                // признак ОКСа: 0 - неОКС, 1 - ОКС
			public string oksType { get; set; }             // тип ОКС: "building" = здание
			public string oksFloors { get; set; }           // этажность ОКС
			public string oksUFloors { get; set; }          // название подземного этажа ОКС, например: "подвал цокольный"
			public string oksElementsConstruct { get; set; }// код материала конструктивных элементов, если несколько - через ", "
			public string oksYearUsed { get; set; }         // год ввода ОКС в эксплуатацию
			public decimal oksInventoryCost { get; set; }   // инвентарная стоимость
			public string oksInn { get; set; }
			public string oksExecutor { get; set; }
			public string oksYearBuilt { get; set; }
			public string oksCostDate { get; set; }
			public object rcType { get; set; }
			public string rcDate { get; set; }              // признак расшифрованности сведений о КИ. Если "ci" - см.нижеследующие  поля
			public string guidUl { get; set; }
			public string guidFl { get; set; }
			public string ciSurname { get; set; }           // фамилия КИ
			public string ciFirst { get; set; }             // имя КИ
			public string ciPatronymic { get; set; }        // отчество КИ
			public string ciNCertificate { get; set; }      // № сертификата КИ
			public string ciPhone { get; set; }             // телефон КИ
			public string ciEmail { get; set; }             // эл.почта КИ
			public string ciAddress { get; set; }           // адрес КИ
			public string coName { get; set; }              // название организации КИ
			public string coInn { get; set; }               // ИНН организации КИ
			public string utilCode { get; set; }            // код разрешенного использования по классификатору
			public string utilByDoc { get; set; }           // название разрешенного использования по документам
			public object cadastralBlockId { get; set; }    // ИД кадастрового квартала (часто пустой)
			public string parcelStatusStr { get; set; }     // название статуса объекта
			public string oksElementsConstructStr { get; set; }// название материала конструктивных элемементов, если несколько - через ", " и каждый с заглавной
			public object utilCodeDesc { get; set; }
		}

		public class RealtyData
		{
			public string id { get; set; }
			public int regionKey { get; set; }
			public string realtyCn { get; set; }
			public string realtyCon { get; set; }
			public string realtyInv { get; set; }
			public string realtyUn { get; set; }
			public string literBti { get; set; }
			public string realtyType { get; set; }
			public string assignType { get; set; }
			public string realtyName { get; set; }
			public decimal areaValue { get; set; }
			public string areaType { get; set; }
			public string areaUnit { get; set; }
			public string floorGround { get; set; }
			public string floorUnder { get; set; }
			public string floorGroundStr { get; set; }
			public string floorUnderStr { get; set; }
			public string realtyTypeValue { get; set; }
			public string areaUnitValue { get; set; }
			public bool rightsReg { get; set; }
			public bool multiFlat { get; set; }
			public bool incomplete { get; set; }
			public string realtyTypeStr { get; set; }
		}

		/// <summary>
		/// Описывает подробные данные "внутреннего" объекта (помещение)
		/// </summary>
		public class PremisesData
		{
			public string id { get; set; }                  // ИД объекта
			public int regionKey { get; set; }
			public string premisesCn { get; set; }          // полный кад.номер объекта
			public string premisesCon { get; set; }         // полный номер права на объект, например: "02-04-17/043/2005-434"
			public string premisesInv { get; set; }         // номер инв.дела ? например: "3893"
			public string premisesUn { get; set; }
			public string literBti { get; set; }            // литер БТИ
			public string premisesType { get; set; }        // код типа объекта, для premises = 002002002000
			public string assignType { get; set; }
			public string premisesName { get; set; }        // название объекта, например "Квартира"
			public decimal areaValue { get; set; }          // площадь объекта
			public string areaType { get; set; }            // код вида площади, 060001003000 = общая (приказ Роснедвижимости от 09.07.07 № П/0160)
			public string areaUnit { get; set; }            // код ЕИ площади по классификатору РР (приказ РР от 13.10.11 № П/389), 012002001000 = 055 (ОКЕИ) = кв.м
			public int premisesFloor { get; set; }          // номер этажа
			public string premisesFloorStr { get; set; }    // номер этажа строкой
			public string premisesNum { get; set; }         // номер квартиры
			public string premisesTypeValue { get; set; }
			public string areaUnitValue { get; set; }
			public bool rightsReg { get; set; }             // признак зарегистрированности прав
			public bool multiFlat { get; set; }             // признак многоквартирности (общага?)
			public string premisesTypeStr { get; set; }     // название типа объекта, например "Помещение"
		}

		/// <summary>
		/// Описывает данные о правах на объект
		/// </summary>
		public class RightEncumbranceObject
		{
			public RightData rightData { get; set; }        // сведения о праве
			public Encumbrance[] encumbrances { get; set; } // сведения об обременениях
		}

		/// <summary>
		/// Описывает данные о праве
		/// </summary>
		public class RightData
		{
			public int tempId { get; set; }
			public string id { get; set; }                  // ИД права
			public string objectId { get; set; }            // ИД объекта
			public int updatePackId { get; set; }
			public int regionKey { get; set; }
			public string code { get; set; }                // код вида права: "001001000000" = "Собственность", "022010000000" = "Доверительное управление"
			public string codeDesc { get; set; }            // название вида права, например "Собственность"
			public string partSize { get; set; }            // размер доли в праве
			public string type { get; set; }
			public string regNum { get; set; }              // регистрационный номер права
			public string regDate { get; set; }             // дата регистрации права
			public string rsCode { get; set; }
			public string packageId { get; set; }
			public string actualDate { get; set; }          // дата актуализации права
		}

		/// <summary>
		/// Описывает данные об обременении
		/// </summary>
		public class Encumbrance
		{
			public int tempId { get; set; }
			public string id { get; set; }                  // ИД обременения
			public string objectId { get; set; }            // ИД объекта
			public int updatePackId { get; set; }
			public int regionKey { get; set; }
			public string code { get; set; }                // код вида права: "001001000000" = "Собственность"
			public string codeDesc { get; set; }            // название вида права, например "Собственность"
			public string periodStart { get; set; }         // дата начала действия
			public string periodEnd { get; set; }           // дата окончания действия
			public string periodDuration { get; set; }      // период действия, например: "с 27.01.2012 по 16.10.2026"
			public object type { get; set; }
			public string regNum { get; set; }              // регистрационный номер обременения
			public string regDate { get; set; }             // дата регистрации
			public string rsCode { get; set; }
			public string packageId { get; set; }
			public string actualDate { get; set; }          // дата актуализации
		}

		/// <summary>
		/// Описывает данные о предыдущем номере объекта
		/// </summary>
		public class OldNumber
		{
			public int tempId { get; set; }
			public string objectId { get; set; }
			public int regionKey { get; set; }
			public string numberType { get; set; }      // код типа номера: "03" = "Кадастровый номер"
			public string numberValue { get; set; }     // номер
			public string normalizedNumberValue { get; set; }
			public string rsCode { get; set; }
			public string packageId { get; set; }
			public string actualDate { get; set; }
			public string numberTypeStr { get; set; }   // тип номера
		}

		#endregion

		#region FIR service classes

		public interface IRESTServer
		{
			string Url { get; set; }
		}

		/// <summary>
		/// 
		/// </summary>
		public class FIR_Server_ru :IRESTServer
		{
			public const string url_FIR = "http://rosreestr.ru/api/online/fir_object/";

			public System.Diagnostics.Stopwatch watch;
			public int Timeout;
			public FIR.FIRJsonData jsonResponse; //Ответ сервера, краткий

			public FIR_Server_ru()
			{
				this.Timeout = 8000;// default 8sec
				this.watch = new System.Diagnostics.Stopwatch();
				this.fUrl = url_FIR;
			}
			private string fUrl;
			public string Url
			{
				get { return this.fUrl; }
				set { this.fUrl = value; }
			}
			/// <summary>
			/// Запрос к /api/online/fir... серверу ФГИС ФИР ЕГРН
			/// </summary>
			/// <param name="CN">Кадастровый номер поиска</param>
			/// <returns></returns>
			public bool GET_WebOnline_th(string CN)
			{
				if (CN == null) return false;
				this.watch.Reset();
				this.watch.Start();
				//this.Nodes.Clear();
				//this.Image = null;

				WebRequest wrGETURL = null;
				wrGETURL = WebRequest.Create(url_FIR + CommonCast.CasterCN.CNToId(CN));
				//wrGETURL.Proxy = WebProxy.GetDefaultProxy();
				wrGETURL.Credentials = CredentialCache.DefaultCredentials;
				wrGETURL.Proxy.Credentials = CredentialCache.DefaultCredentials;
				wrGETURL.Timeout = this.Timeout;
				Stream objStream;
				WebResponse wr = null;

				try
				{
					wr = wrGETURL.GetResponse();
				}
				catch (IOException ex)
				{
					System.Windows.Forms.MessageBox.Show(ex.ToString());
					this.watch.Stop();
					return false;
				}

				objStream = wr.GetResponseStream();
				if (objStream != null)
				{
					StreamReader objReader = new StreamReader(objStream);
					string jsonResult = objReader.ReadToEnd();
					objReader.Close();
					//Понадобилась ссылка на System.Web.Extensions
					System.Web.Script.Serialization.JavaScriptSerializer sr = new System.Web.Script.Serialization.JavaScriptSerializer();
					jsonResponse = sr.Deserialize<FIR.FIRJsonData>(jsonResult);
					if (jsonResponse != null)
						if (jsonResponse.objectData != null)
						{
							string test = jsonResponse.objectData.id;
							/*
						
							 */
						}

					this.watch.Stop();
					return false;
				}
				this.watch.Stop();
				return false;


			}


		}
	}
	#endregion

}

