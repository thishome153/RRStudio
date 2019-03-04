using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Net; // http???
using System.IO;

namespace RRTypes
{
    namespace pkk5
    {

        /// <summary>
        /// Парсер JSON протокола сервера Росреестра. Сущность - ответ сервера
        /// </summary>
        public class JSON_Response
        {
            public List<JSON_feature> features { get; set; }
            public int featuresCount { get; set; }
            public JSON_debug debug { get; set; }
        }
        public class JSON_feature
        {
            public JSON_atributes attributes { get; set; }
        }
        public class JSON_atributes
        {
            public string OBJECTID { get; set; } //	Integer	Внутренний идентификатор
            public string REGION_KEY { get; set; } //	Text	Код региона
            public string PARENT_ID { get; set; } //	Text	Идентификатор кадастрового квартала*/
            public string CAD_NUM { get; set; }//	Text	Кадастровый номер земельного участка
            public string PARCEL_STATUS { get; set; }
            public string PKK_ID { get; set; } //Text	Идентификатор земельного участка
            public string UTIL_BY_DOC { get; set; }
            public string UTIL_CODE { get; set; } //	Text	Код вида разрешенного использования земель
            public string CATEGORY_CODE { get; set; }//	Text Код категории земель
            public string OBJECT_ADDRESS { get; set; }
            public string AREA_VALUE { get; set; }
            public string AREA_TYPE { get; set; }
            //public int ACTUAL_DATE { get; set; }  //Дата актуальности
            /*
    PARCEL_ID	Text	Строковый идентификатор земельного участка в ИПГУ
    TEMP_ID	Integer	Числовой идентификатор земельного участка в ИПГУ
    /*
      STATE_CODE	Text	Код статуса земельного участка
    ANNO_TEXT	Text	Текст подписи земельного участка на ПКК
    CP_VALUE	Double	Значение кадастровой стоимости
    ERROR_CODE	Integer	Код ошибки
    XC	Double	Координата X центра 
    YC	Double	Координата Y центра 
    XMIN	Double	Минимальная координата X 
    XMAX	Double	Максимальная координата X 
    YMIN	Double	Минимальная координата Y 
    YMAX	Double	Максимальная координата Y 
    DEL_FEATURE	Integer	Служебное поле обработки данных для ПКК.
    G_AREA	Double	Служебное поле отображения данных для ПКК.*/
            /*
             "{\"features\":[{\"attributes\":{\"CAD_NUM\":\"26:06:093501:14\",\"OBJECT_ID\":\"26:6:93501:14\",\"REGION_KEY\":126,\"PARCEL_CN\":\"26:06:093501:14\",\"PARCEL_STATUS\":\"01\",\"DATE_CREATE\":1159488000000,\"DATE_REMOVE\":null,\"CATEGORY_TYPE\":\"003002000000\",\"AREA_VALUE\":7543,\"AREA_TYPE\":\"008\",\"AREA_UNIT\":\"055\",\"RIGHT_REG\":0,\"CAD_COST\":4296341.94,\"CAD_UNIT\":\"383\",\"DATE_COST\":1448755200000,\"ONLINE_ACTUAL_DATE\":1461628800000,\"PARENT_ID\":\"26060093501\",\"OBJECT_ADDRESS\":\"Ставропольский край, р-н Изобильненский, п Солнечнодольск, ул Техническая,  14/3\",\"DATE_LOAD\":1461628800000,\"CI_SURNAME\":null,\"CI_FIRST\":null,\"CI_PATRONYMIC\":null,\"RC_DATE\":null,\"RC_TYPE\":null,\"CO_NAME\":null,\"CO_INN\":null,\"OBJECT_DISTRICT\":\"Изобильненский\",\"DISTRICT_TYPE\":\"р-н\",\"OBJECT_PLACE\":null,\"PLACE_TYPE\":\"неопр\",\"OBJECT_LOCALITY\":\"Солнечнодольск\",\"LOCALITY_TYPE\":\"п\",\"OBJECT_STREET\":\"Техническая\",\"STREET_TYPE\":\"ул\",\"OBJECT_HOUSE\":\"14/3\",\"OBJECT_BUILDING\":null,\"OBJECT_STRUCTURE\":null,\"OBJECT_APARTMENT\":null,\"UTIL_BY_DOC\":\"под производственную базу\",\"UTIL_CODE\":\"143001000000\",\"OKS_FLAG\":0,\"OKS_TYPE_ONLINE\":null,\"OKS_FLOORS\":null,\"OKS_U_FLOORS\":null,\"OKS_ELEMENTS_CONSTRUCT\":null,\"OKS_YEAR_USED\":null,\"OKS_INVENTORY_COST\":0,\"OKS_INN\":null,\"OKS_EXECUTOR\":null,\"YEAR_BUILT\":null,\"OKS_COST_DATE\":null,\"FORM_RIGHTS\":null,\"OBJECTID\":75065895,\"PARCEL_ID\":\"26:6:93501:14\",\"TEMP_ID\":0,\"PKK_ID\":\"2606009350100014\",\"STATE_CODE\":\"01\",\"ANNO_TEXT\":\"14\",\"CP_VALUE\":6015.31621,\"CATEGORY_CODE\":\"003002000000\",\"ACTUAL_DATE\":1438300800000,\"ERROR_CODE\":0,\"XC\":4620625.69527156,\"YC\":5669780.942772,\"XMIN\":4620541.2844,\"XMAX\":4620711.2168,\"YMIN\":5669732.7951,\"YMAX\":5669829.5086,\"DEL_FEATURE\":null,\"G_AREA\":15257.06550982,\"SHAPE_Length\":517.30093309324741,\"SHAPE_Area\":15257.06920196116,\"ERRORCODE\":0}}],\"featuresCount\":1,\"debug\":{\"parseParametersTime\":\"00:00:00\",\"queryCountExecutionTime\":\"00:00:00.0190011\",\"countTableIsNull\":false,\"queryExecutionTime\":\"00:00:00.1710098\",\"queryExecution\":{\"createConnection\":\"00:00:00\",\"fillTable\":\"00:00:00.1710098\"},\"dataTableIsNull\":false,\"queryToGDBExecutionTime\":\"00:00:00.0350020\",\"queryToGDBParcelCount\":\"1\",\"attachGdbDataToOracleDataExecutionTime\":\"00:00:00.0010000\",\"attachGdbDataToOracleDataDebugInfo\":{\"addFields\":\"00:00:00.0010000\",\"addRows\":\"00:00:00\",\"t1\":\"00:00:00\",\"t2\":\"00:00:00\"},\"server\":\"ARCGIS9\",\"totalExecuteTime\":\"00:00:00.2260129\"}}"
             * */
        }
        public class JSON_debug
        {
            public string server { get; set; }
            public string queryExecutionTime { get; set; }
        }


        //Класс, описывающий ответ сервера в виде массива объектов (при поиске типа /1?text=26...)
        public class pkk5_json_response
        {
            public List<pkk5_json_feature> features { get; set; }
        }

        public class pkk5_json_feature
        {
            public pkk5_json_attrs attrs { get; set; }
            public pkk5_json_extent extent { get; set; }
            public pkk5_json_center center { get; set; }
        }

        public class pkk5_json_attrs
        {
            public string address { get; set; }
            public string cn { get; set; } // "cn": "26:05:043417:133", 
            public string id { get; set; } // "id": "26:5:43417:133"
        }

        public class pkk5_json_extent
        {
            public string xmax { get; set; }
            public string xmin { get; set; }
            public string ymax { get; set; }
            public string ymin { get; set; }
        }
        public class pkk5_json_center
        {
            public string x { get; set; }
            public string y { get; set; }
        }
        //Класс, описывающий один объект
        public class pkk5_json_Fullresponse
        {
            public pkk5_json_Ffeature feature { get; set; }
        }

        public class pkk5_json_Ffeature
        {
            public pkk5_json_Fattrs attrs { get; set; }
        }

        public class pkk5_json_Fattrs
        {
            public string address { get; set; }
            public string cn { get; set; }
            public string area_type { get; set; } //": "009", 
            public string area_unit { get; set; } //": "055", 
            public string area_value { get; set; } //": 1748.0, 
            public string util_by_doc { get; set; }
            public pkk5_json_cad_eng_data cad_eng_data { get; set; }
            public pkk5_json_extent extent { get; set; }
            public string AreaType2Str(string area_type)
            {
                if (area_type == "001") return "Площадь застройки";
                if (area_type == "002") return "Общая площадь";
                if (area_type == "003") return "Общая площадь без лоджии";
                if (area_type == "004") return "Общая площадь с лоджией";
                if (area_type == "005") return "Жилая площадь";
                if (area_type == "007") return "Основная площадь";
                if (area_type == "008") return "Декларированная площадь";
                if (area_type == "009") return "Уточненная площадь";
                if (area_type == "010") return "Фактическая площадь";
                if (area_type == "011") return "Вспомогательная площадь";
                if (area_type == "012") return "Площадь помещений общего пользования без лоджии";
                if (area_type == "013") return "Площадь помещений общего пользования с лоджией";
                if (area_type == "014") return "Прочие технические помещения без лоджии";
                if (area_type == "015") return "Прочие технические помещения с лоджией";
                if (area_type == "020") return "Застроенная площадь";
                if (area_type == "021") return "Незастроенная площадь";
                if (area_type == "022") return "Значение площади отсутствует";
                return "";
            }
            public string Unit2Str(string area_unit)
            {
                if (area_unit == "003") return "мм";
                if (area_unit == "004") return "см";
                if (area_unit == "005") return "дм";
                if (area_unit == "006") return "м";
                if (area_unit == "008") return "км";
                if (area_unit == "009") return "Мм";
                if (area_unit == "047") return "морск. м.";
                if (area_unit == "050") return "кв. мм";
                if (area_unit == "051") return "кв. см";
                if (area_unit == "053") return "кв. дм.";
                if (area_unit == "055") return "кв. м";
                if (area_unit == "058") return "тыс. кв. м";
                if (area_unit == "059") return "га";
                if (area_unit == "061") return "кв. км";
                if (area_unit == "109") return "а";
                if (area_unit == "359") return "сут";
                if (area_unit == "360") return "Нед.";
                if (area_unit == "361") return "дек.";
                if (area_unit == "362") return "мес.";
                if (area_unit == "364") return "кварт.";
                if (area_unit == "365") return "полугод.";
                if (area_unit == "366") return "кв. км";
                if (area_unit == "383") return "руб.";
                if (area_unit == "384") return "тыс. руб.";
                if (area_unit == "385") return "млн. руб.";
                if (area_unit == "386") return "млрд. руб.";
                if (area_unit == "1000") return "неопр.";
                if (area_unit == "1001") return "отсуств.";
                if (area_unit == "1002") return "руб. за кв. км";
                if (area_unit == "1003") return "руб. за а";
                if (area_unit == "1004") return "руб. за га";
                if (area_unit == "1005") return "Иные";
                return "";
            }

        }
        public class pkk5_json_cad_eng_data
        {
            public string actual_date { get; set; } //": "Thu, 08 Sep 2016 00:00:00 GMT", 
            public string ci_first { get; set; } //": "\u041e\u043b\u0435\u0433", 
            public string ci_n_certificate { get; set; } //": "26-11-115", 
            public string ci_patronymic { get; set; } //": "\u0412\u043b\u0430\u0434\u0438\u043c\u0438\u0440\u043e\u0432\u0438\u0447", 
            public string ci_surname { get; set; } //": "\u0420\u0438\u043c\u0448\u0430", 
            public string lastmodified { get; set; } //": "Thu, 08 Sep 2016 09:28:07 GMT", 
            public string rc_type { get; set; } //": 0
        }

        public enum pkk5_Types
        {
            Parcel =1,
            Block = 2,
            OKS =5,
            TerrZone = 6
        }

    
        /// <summary>
        /// Класс сервисов Росреестра (api, arccgis)
        /// </summary>
        public class pkk5_Rosreestr_ru
        {
            public const string url_api      = "http://pkk5.rosreestr.ru/api/features/";
            public string url_arcgis_export  = "http://pkk5.rosreestr.ru/arcgis/rest/services/Cadastre/CadastreOriginal/MapServer/export?bbox=";
            public string url_arcgis_exportZ ="http://pkk5.rosreestr.ru/arcgis/rest/services/Cadastre/ZONES/MapServer/export?bbox=";
            public pkk5_json_response jsonResponse; //Ответ сервера, краткий
            public pkk5_json_Fullresponse jsonFResponse;// Ответ полный, на запрос по id
			public string TODO_TEst_URL;
            private int fImage_Width;
            private int fImage_Height;
            public int Image_Width {
                get { return this.fImage_Width; }
            }
            public int Image_Height {
                get { return this.fImage_Height; }
            }
            public int mapScale;
            public string dpi;
            public int Timeout;
            public int ElapsedTime;
            public System.Drawing.Image Image;
            public System.Diagnostics.Stopwatch watch;
            public List<TreeNode> Nodes;
            /// <summary>
            /// Конструктор для объекта pkk5.rosreestr.ru
            /// </summary>
            /// <param name="pic_width">Ширина экспортной картинки</param>
            /// <param name="pic_h">Высота экспортной картинки</param>
            public pkk5_Rosreestr_ru(int pic_width, int pic_h)
            {

                this.fImage_Height = pic_h;
                this.fImage_Width = pic_width;
                this.mapScale = 1000;// default
                this.Timeout = 8000;// default 8sec
                this.Nodes = new List<TreeNode>();
                this.dpi = "96"; //Default
                this.watch = new System.Diagnostics.Stopwatch();
            }
     
         
            /// <summary>
            /// Запрос к pkk5 серверу
            /// </summary>
            /// <param name="CN">Кадастровый номер поиска</param>
            /// <param name="ObjectType">если 2- ОКС, 1-  ЗУ, 0  - в Кварталах</param>
            /// <returns></returns>
            public bool Get_WebOnline_th(string CN, pkk5_Types ObjectType)
            {
                if (CN == null) return false;
                this.watch.Reset();
                this.watch.Start();
                this.Nodes.Clear();
                this.Image = null;
                try
                {
                    //TreeNode PWebNode = treeView_Web.Nodes.Add(Parcel.CN);
                    TreeNode PWebNode = new TreeNode(CN);
                    this.Nodes.Add(PWebNode);

                    WebRequest wrGETURL = null;
					//Запрос по кадастровому номеру, возвращает массив (сокращенные атрибуты):
					wrGETURL = WebRequest.Create(pkk5_Rosreestr_ru.url_api + ((int)ObjectType).ToString() +"?text="+ CN);
                    wrGETURL.Proxy = WebProxy.GetDefaultProxy();
                    wrGETURL.Timeout = this.Timeout;
                    Stream objStream;
                    WebResponse wr = wrGETURL.GetResponse();
                    objStream = wr.GetResponseStream();
                    if (objStream != null)
                    {
                        StreamReader objReader = new StreamReader(objStream);
                        string jsonResult = objReader.ReadToEnd();
                        objReader.Close();
                        //Понадобилась ссылка на System.Web.Extensions
                        System.Web.Script.Serialization.JavaScriptSerializer sr = new System.Web.Script.Serialization.JavaScriptSerializer();
                         jsonResponse = sr.Deserialize<pkk5_json_response>(jsonResult);
                        if (jsonResponse != null)
                            if (jsonResponse.features != null)
                             if (jsonResponse.features.Count >0) // на количество тоже надо проверять - бывают "пустые ответы", но со статусом 200 , т.е. ОК
                            {
                                PWebNode.Nodes.Add(jsonResponse.features[0].attrs.address).Expand();
                                //Запрос по конкретному id:
                                wrGETURL = WebRequest.Create(pkk5_Rosreestr_ru.url_api + ((int)ObjectType).ToString() + "/" + jsonResponse.features[0].attrs.id);
                                wrGETURL.Timeout = this.Timeout;
                                WebResponse wrF = wrGETURL.GetResponse();
                                objStream = wrF.GetResponseStream();
                                if (objStream != null)
                                {
                                    StreamReader objFReader = new StreamReader(objStream);
                                    string jsonFResult = objFReader.ReadToEnd();
                                    objFReader.Close();
                                    jsonFResponse = sr.Deserialize<pkk5_json_Fullresponse>(jsonFResult);
                                    if (jsonFResponse != null)
                                        if (jsonFResponse.feature != null)
                                        {
                                            //PWebNode.Nodes.Add(jsonFResponse.feature.attrs.util_by_doc);
                                            //PWebNode.Nodes.Add(jsonFResponse.feature.attrs.AreaType2Str(jsonFResponse.feature.attrs.area_type)).Nodes.Add(jsonFResponse.feature.attrs.area_value +
                                            //    " " + jsonFResponse.feature.attrs.Unit2Str(jsonFResponse.feature.attrs.area_unit));
                                           // PWebNode.ExpandAll();

                                            /*
                                            if (jsonFResponse.feature.attrs.cad_eng_data != null)
                                            {
                                                TreeNode PWebNodec = PWebNode.Nodes.Add("Документы для ГКУ подготовлены");
                                                PWebNodec.Nodes.Add(jsonFResponse.feature.attrs.cad_eng_data.ci_surname + " " +
                                                    jsonFResponse.feature.attrs.cad_eng_data.ci_first + " " +
                                                    jsonFResponse.feature.attrs.cad_eng_data.ci_patronymic + " " +
                                                    jsonFResponse.feature.attrs.cad_eng_data.ci_n_certificate);
                                                PWebNodec.Nodes.Add("Дата обновления атрибутов : " + jsonFResponse.feature.attrs.cad_eng_data.actual_date);
                                                PWebNodec.Nodes.Add("lastmodified:" + jsonFResponse.feature.attrs.cad_eng_data.lastmodified);
                                            }
                                            */
                                        }
                                }

                                // если есть ОИПД:
                                if (jsonResponse.features[0].extent != null)
                                {
                                    TreeNode PWebNodeExt = PWebNode.Nodes.Add("Экстент (ПД)");
                                    TreeNode PWebNodebbox = PWebNodeExt.Nodes.Add("bbox");
                                    PWebNodebbox.ToolTipText = "Extent (bounding box) of the exported image";
                                    TreeNode PWebNodebboxV = PWebNodebbox.Nodes.Add(jsonResponse.features[0].extent.xmin.ToString() + "," +
                                                                                   jsonResponse.features[0].extent.ymin.ToString() + "," +
                                                                                   jsonResponse.features[0].extent.xmax.ToString() + "," +
                                                                                   jsonResponse.features[0].extent.ymax.ToString());
                                    PWebNodebboxV.Tag = 256; // признак bbox value node;
                                    PWebNodebboxV.ToolTipText = "Строка xmin,ymin,xmax,ymax. Для вызова pkk5/MapServer";

                                    TreeNode PWebNodeCenter = PWebNodeExt.Nodes.Add("center");
                                    PWebNodeCenter.Nodes.Add(jsonResponse.features[0].center.x.ToString()).ToolTipText = "x";
                                    PWebNodeCenter.Nodes.Add(jsonResponse.features[0].center.y.ToString()).ToolTipText = "y";
                                    TreeNode PWebNodeCenterV = PWebNodeCenter.Nodes.Add("#x=" + jsonResponse.features[0].center.x.ToString() +
                                                             "&y=" + jsonResponse.features[0].center.y.ToString() + "&z=20&app=search&opened=1");
                                    PWebNodeCenterV.ToolTipText = "Для вызова pkk5 direct";
                                    PWebNodeCenterV.Tag = 255;

                                    string sURLpkk5_jpeg = null;
                                    // Запрос изображения в jpeg по bbox:
                                    if (ObjectType == pkk5_Types.TerrZone)
                                        sURLpkk5_jpeg = this.url_arcgis_exportZ; // Для Зон Другой сервер однако
                                    else
                                        sURLpkk5_jpeg = this.url_arcgis_export;

                                             sURLpkk5_jpeg += jsonResponse.features[0].extent.xmin + "%2C" +
                                                              jsonResponse.features[0].extent.ymin + "%2C" +
                                                              jsonResponse.features[0].extent.xmax + "%2C" +
                                                              jsonResponse.features[0].extent.ymax + "%2C" +
                                                                        "&bboxSR=&layers=&layerDefs="+
                                                                        "&size=" +
                                                              this.Image_Width.ToString() + "%2C" +
                                                              this.Image_Height.ToString()+ "%2C"+
                                                              "&imageSR=&format=jpg&transparent=true"+
                                                              "&dpi="+this.dpi+"%2C"+
                                                              "&time=&layerTimeOptions=&dynamicLayers=&gdbVersion=" +
                                                              "&mapScale=" + this.mapScale.ToString() +
                                                              "&f=image";
                                    // PWebNode.Nodes.Add("mapScale").Nodes.Add(this.mapScale.ToString());
                                    // PWebNode.Nodes.Add("url jpeg").Nodes.Add(sURLpkk5_jpeg);
                                    wrGETURL = WebRequest.Create(sURLpkk5_jpeg);
                                    wrGETURL.Timeout = this.Timeout;
                                    WebResponse wrJpeg = wrGETURL.GetResponse();
                                    objStream = wrJpeg.GetResponseStream();
                                    if (objStream != null)
                                        this.Image = System.Drawing.Bitmap.FromStream(objStream);
                                    this.watch.Stop();
                                    return true;
                                }
                            }
                        this.watch.Stop();
                        return false;
                    }
                    this.watch.Stop();
                    return false;
                }

                catch (IOException ex)
                {
                    MessageBox.Show(ex.ToString());
                    this.watch.Stop();
                    return false;
                }
            }


        }


        /*
             public class JSON_TypeRes : System.Web.Script.Serialization.JavaScriptTypeResolver
             {
                 public override Type ResolveType(string id)
                 {
                     return Type.GetType(id);
                 }

                 public override string ResolveTypeId(Type type)
                 {
                     if (type == null)
                     {
                         throw new ArgumentNullException("type");
                     }

                     return type.Name;
                 }
             }
             */

    }
}