﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace netFteo.XML
{

    public class ComplexSchema
    {
        public string RootName;
        public XmlSchemaSet schemaSet; // Набор схем/подсхем
    }

    /// <summary>
    /// Wrapper class для System.XMl.XmlSchemaSet. Временный, надеюсь
    /// </summary>
    public class SchemaSet
    {
        public string SchemaName;
        public string SchemaDir;
        public XmlSchemaSet schemaSet; // Набор схем
        public SchemaSet()
        {
            this.schemaSet = new XmlSchemaSet();
        }
        public SchemaSet(string rootdir, string RootSchema)
        {
            this.SchemaName = RootSchema;
            this.SchemaDir = rootdir;
            this.schemaSet = new XmlSchemaSet();
             AddSchema(RootSchema);
        }

        public XmlSchema AddSchema(string filename)
        {
            if (System.IO.File.Exists(SchemaDir + filename))
            {
                XmlSchema schema = XmlSchema.Read(XmlReader.Create(SchemaDir + filename), new ValidationEventHandler(ValidationCallBack));
                this.schemaSet.Add(schema);
                schemaSet.Compile();
                return schema;
            }
            else return null;
        }
        public void CompileSet()
        {
            this.schemaSet.Compile();
        }
        public int FilesCount // Количество файлов в схеме, после компиляции естессно
        {
            get { return this.schemaSet.Count; }
        }

        public XmlSchema GetSchema(string targetnamespace)
        {
            if (this.schemaSet.Schemas(targetnamespace).Count == 1)
            { //немного криво, но GetEnumerator непобедимый однако:
                foreach (System.Xml.Schema.XmlSchema cs in this.schemaSet.Schemas(targetnamespace))
                {
                    return cs;
                }
            }
            return null;
        }
        public IEnumerable<string> SchemaSetNamespaces()
        {
            XmlUrlResolver xmlres = new  XmlUrlResolver();
            this.schemaSet.XmlResolver = xmlres;
            System.Uri uri = null;
            xmlres.ResolveUri(uri, "urn://x-artefacts-rosreestr-ru/commons/directories/regions/1.0.1");
                return null;

        }
        private void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            // If Document Validation Fails
            // isvalid = false;
            // MessageConsole.Text = "INVALID. Check message and datagridview table.";
            //  richTextBox1.Text = "The document is invalid: " + e.Message;
        }
    }


    /// <summary>
    /// Справочник XSD
    /// Любой файл XSD, Представляющий собой перечисления (enumeration)
    /// </summary>
    /// 
    public class XSDFile
    {
        public string XSDFileName;
        private bool fComplete; // флаг открытого файла
        private XmlSchema fschema;
        public string targetNamespace;

        public XmlSchema Schema
        {
            set
            {
                this.fschema = value;
                if (fschema == null)
                {
                    this.fComplete = false;
                    return;
                }
                targetNamespace = fschema.TargetNamespace;
                fSimpleTypeNames = fschema.Items.OfType<XmlSchemaSimpleType>()
                                                        .Where(selector => selector.Namespaces != null)
                                                        .Select<XmlSchemaSimpleType, string>(selector => selector.Name);

                fSimpleTypeNamesAnno = fschema.Items.OfType<XmlSchemaSimpleType>()
                                                        .Where(selector => selector.Namespaces != null)
                                                        .Select<XmlSchemaSimpleType, XmlSchemaAnnotation>(selector => selector.Annotation)
                                                        .Select<XmlSchemaAnnotation, string>(selAnno => ((XmlSchemaDocumentation)selAnno.Items[0]).Markup[0].Value);

                this.Facets = fschema.Items.OfType<XmlSchemaSimpleType>()
                              .Where(s => (s.Content is XmlSchemaSimpleTypeRestriction) && s.Name == fSimpleTypeNames.First())
                              .SelectMany<XmlSchemaSimpleType, XmlSchemaEnumerationFacet>
                                 (c => ((XmlSchemaSimpleTypeRestriction)c.Content).Facets.OfType<XmlSchemaEnumerationFacet>());

                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add(fschema);
                schemaSet.Compile();
                this.fComplete = true;
            }
            get
            {
                return this.fschema;
            }

        }
        public XSDFile()
        {
            this.fschema = null;
        }

        public XSDFile(XmlSchema schema)
        {
            this.Schema = schema;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="xsdfilename">Имя xsd файла схемы</param>
        public XSDFile(string xsdfilename) 
        {
            this.XSDFileName = xsdfilename;
            if (System.IO.File.Exists(this.XSDFileName))
            {
              this.Schema = XmlSchema.Read(XmlReader.Create(this.XSDFileName), new ValidationEventHandler(ValidationCallBack));

            }

            else
                this.fComplete = false;

        }

        IEnumerable<XmlSchemaEnumerationFacet> Facets;
        public IEnumerable<string> fSimpleTypeNames;//Список "Простых" типов (для <xs:enumeration)
        public IEnumerable<string> fSimpleTypeNamesAnno;//Список "Простых" типов (для <xs:enumeration)
        public IEnumerable<string> SimpleTypeNames
        {
            get
            {
                if (this.fSimpleTypeNames.Count() > 0)
                    return this.fSimpleTypeNames;
                else return null;
            }
            set
            {
                this.fSimpleTypeNames = value;
            }
        }
        public string SimpleTypeNamesSafeFirst
        {
            get
            {
                if (this.fSimpleTypeNames.Count() > 0)
                    return this.fSimpleTypeNames.First(); // predicate ? !!
                else return null;
            }
        }
        public string SimpleTypeNamesAnoSafeFirst
        {
            get
            {
                if (this.fSimpleTypeNamesAnno.Count() > 0)
                    return this.fSimpleTypeNamesAnno.First(); // predicate ? !!
                else return null;
            }
        }
        private void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            // If Document Validation Fails
            // isvalid = false;
            // MessageConsole.Text = "INVALID. Check message and datagridview table.";
            //  richTextBox1.Text = "The document is invalid: " + e.Message;
        }

        /// <summary>
        /// Проверка на наличие перечислений в файле XSD
        /// </summary>
        public bool EnumerationPresent
        {
            get
            {
                if (Facets == null) return false;
                if (Facets.Count() > 0) return true;
                    else return false;
                
            }
        }

        /// <summary>
        /// Получает список Аннотаций/Документации по указанному value. LINQ
        /// </summary>
        /// <param name="value">Значение перечисления</param>
        /// <param name="simpletypeName">Имя типа перечисления, например: 'dRegionsRF' </param>
        /// <returns>Результат - Список аннотаций</returns>
        public List<string> Item2Annotation (string value, string simpletypeName)
        {


            IEnumerable<string> Items2 = fschema.Items.OfType<XmlSchemaSimpleType>()
                                         .Where(s => (s.Content is XmlSchemaSimpleTypeRestriction) && s.Name == simpletypeName)

                                         .SelectMany<XmlSchemaSimpleType, XmlSchemaEnumerationFacet>
                                          (c => ((XmlSchemaSimpleTypeRestriction)c.Content).Facets.OfType<XmlSchemaEnumerationFacet>()
                                          .Where(dd => dd.Value.ToString().Contains(value)))
                                          .Select<XmlSchemaEnumerationFacet, XmlSchemaAnnotation>
                                             (selector_ => selector_.Annotation).Where(an => (an.Items.Count == 1) && (an.Items[0] != null))
                //.Select<XmlSchemaAnnotation, string>
                //  (selector_ => ((XmlSchemaDocumentation)selector_.Items[0]).Markup[0].Value)
                                           .Select<XmlSchemaAnnotation, XmlSchemaDocumentation>
                                             (selector => (XmlSchemaDocumentation)selector.Items[0])
                                             .Select<XmlSchemaDocumentation, XmlNode[]>
                                               (selector => (XmlNode[])selector.Markup).Where( d => d.Count() ==1)
                                               .Select<XmlNode[], string> (markup => markup[0].Value)   
                                          
                                          ;
                                                             

            List<string> resvalues = new List<string>();
            resvalues.AddRange(Items2);
            return resvalues;
        }
        /// <summary>
        /// Список значений
        /// </summary>
        /// <param name="value"></param>
        /// <param name="simpletypeName"></param>
        /// <returns></returns>
         public List<string> Item2Item (string value, string simpletypeName)
        {
            /*
            // Одним предложением
                    IEnumerable<string> FacetsValues = schema.Items.OfType<XmlSchemaSimpleType>()
                           .Where(s => (s.Content is XmlSchemaSimpleTypeRestriction) && s.Name == simpletypeName)

                           .SelectMany<XmlSchemaSimpleType, XmlSchemaEnumerationFacet>
                            (c => ((XmlSchemaSimpleTypeRestriction)c.Content).Facets.OfType<XmlSchemaEnumerationFacet>()
                            .Where(dd => dd.Value.ToString().Contains(value)))
                            .Select<XmlSchemaEnumerationFacet, string>
                            ( FacetSelector => FacetSelector.Value)
                            ;
            */


             // Двумя предложениями
            if (fSimpleTypeNames.Count() == 1)
                this.Facets = fschema.Items.OfType<XmlSchemaSimpleType>()
                                            .Where(s => (s.Content is XmlSchemaSimpleTypeRestriction) && s.Name == fSimpleTypeNames.First())
                                            .SelectMany<XmlSchemaSimpleType, XmlSchemaEnumerationFacet>
                                             (c => ((XmlSchemaSimpleTypeRestriction)c.Content).Facets.OfType<XmlSchemaEnumerationFacet>()
                                             .Where(dd => dd.Value.ToString().Contains(value)));

            IEnumerable<string> FacetsValues2 = this.Facets
                                                .Select<XmlSchemaEnumerationFacet, string>
                                                        (FacetSelector => FacetSelector.Value);

            List<string> resvalues = new List<string>();
            resvalues.AddRange(FacetsValues2);
            return resvalues;
        }

        /// <summary>
         /// Выбирает единственное значение Аннотации/Документации соответствующее value
        /// </summary>
         /// <param name="value">Значение для поиска Аннотации/Документации</param>
        /// <returns></returns>
         public string Item2Annotation(string value)
         {
             if (value == null) return "NULL";
             if (this.fComplete)
             {
                 if (value.Contains("Item"))
                     value = value.Substring(4);
                 System.Collections.Generic.List<string> res = this.Item2Annotation(value, SimpleTypeNamesSafeFirst);
                 if (res.Count == 1)
                     return res[0];
                 else return "-";
             }
             else return "файл " + System.IO.Path.GetFileName(this.XSDFileName) +" не найден";
         }
         


        /// <summary>
        /// Получает полный список перечислений . XPath
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> FullEnumList()
        {
            var xDoc = new XmlDocument();
            xDoc.Load(this.XSDFileName);// (assembly.GetManifestResourceStream("v1.xsd"));

            var xMan = new XmlNamespaceManager(xDoc.NameTable);
            xMan.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");

            var xNodeList = xDoc.SelectNodes("//xs:schema/xs:simpleType[@name='Type']/xs:restriction/xs:enumeration", xMan);

            return (IEnumerable<string>)xNodeList;
        }

        

    }
    public class FileInfo  
    {
        private string fComments;
        public string Version;
        public string DocRootName;
        public string FileName;
        public string FilePath;
        public string Namespace;  
        public string DocType;
        public string DocTypeNick;
        public string Number;
        public string RequeryNumber;
        public string ReceivName;
        public string ReceivAdress;
        public string Date;
        public string Appointment;
        public string AppointmentFIO;
        public string Cert_Doc_Organization;
        public string Comments // Conclusion, Notes
        {   
            get 
        {
            if (this.CommentsType != "-")
                return this.fComments;
            else return "-";
        }
        
           set
            {
                this.fComments = value;//
                if (value == null) this.CommentsType = "-";
            }
        }

        public string CommentsType; // Conclusion, Notes

        public netFteo.Spatial.TMyBlockCollection MyBlocks;
        public netFteo.Spatial.TPolygonCollection MifOKSPolygons;
        public netFteo.Spatial.TPolygonCollection MifPolygons;
        public FileInfo()
        {
            this.MifPolygons = new netFteo.Spatial.TPolygonCollection();
            this.MifOKSPolygons = new netFteo.Spatial.TPolygonCollection();
            this.MyBlocks = new netFteo.Spatial.TMyBlockCollection();
            this.DocType = "-";
            this.Version = "";
        }
    }
    public class XSLWriter
    {
        public string hrefToXSLTServer;
        //public static string ApiServer = "http://api.geo-complex.com/xsl";
        //public static string ApiServer = "http://10.66.77.47/xsl";
        public static string ApiServer = "http://www.geo-complex.com/xsl";
        //TODO: 
        // need replace  any https://portal.rosreestr.ru/xsl to ApiServer

        public string XSL_Vidimus06_pub = "https://portal.rosreestr.ru/xsl/GKN/Vidimus/06/common.xsl";
        public string XSL_Vidimus06_lan = ApiServer + "/GKN/Vidimus/06/common.xsl";

        public string XSL_KPT09_pub = "https://portal.rosreestr.ru/xsl/GKN/KPT/09/common.xsl";
        public string XSL_KPT09_lan = ApiServer + "/GKN/KPT/09/common.xsl";

        

        public string XSL_KPT10_b_pub = "https://portal.rosreestr.ru/xsl/GKN/KPT_b/10/common.xsl";
        public string XSL_KPT10_b_lan = ApiServer + "/GKN/KPT_b/10/common.xsl";

        public string XSL_KVOKS02_pub = "https://portal.rosreestr.ru/xsl/GKN/KVOKS/02/common.xsl";
        public string XSL_KVOKS02_lan = ApiServer + "/GKN/KVOKS/02/common.xsl";
        public string XSL_V03_TP = ApiServer + "/fixosoft/V03_TP/STD_TP03.xslt";
        public string XSL_V06_MP = ApiServer + "/fixosoft/V06_MP/MP_V06.xslt";
        //urn://x-artefacts-rosreestr-ru/outgoing/kpoks/4.0.1
        public string XSL_KPOKS0401_pub = "https://portal.rosreestr.ru/xsl/EGRP/Reestr_Extract_Big/ROOM/07/Common.xsl";
        public string XSL_KPOKS0401_lan = ApiServer + "/EGRP/Reestr_Extract_Big/ROOM/07/Common.xsl";

        public string TransformXMLToHTML(string inputXml)
        {
            if (inputXml == null) return null;
            XmlDocument doc = new XmlDocument();
            //TextReader reader = new StreamReader(inputXml);
            //doc.Load(reader);

            // there problem reading xlt. Need custom reader with settings
            XmlReaderSettings xmlopt = new XmlReaderSettings();
            xmlopt.DtdProcessing = DtdProcessing.Prohibit; // DTD disabled on rosreestr, select Prohibit

            XmlReader xmlr = XmlReader.Create(new System.IO.StreamReader(inputXml), xmlopt);
            doc.Load(xmlr);

            XmlNode styleNode = doc.SelectSingleNode("//processing-instruction(\"xml-stylesheet\")");
            if (styleNode is XmlProcessingInstruction)
            {
                XmlProcessingInstruction instruction = (XmlProcessingInstruction)styleNode;
                string tst = instruction.Value;
                int i = tst.IndexOf("href=\"") + 6;
                hrefToXSLTServer = tst.Substring(i, tst.IndexOf('\"', i) - i);
                // На время перестройки будет 
                if (hrefToXSLTServer == XSL_Vidimus06_pub)
                    hrefToXSLTServer = XSL_Vidimus06_lan;//заменить на строку

                if (hrefToXSLTServer == XSL_KPT09_pub)
                    hrefToXSLTServer = XSL_KPT09_lan;//заменить на строку

                if (hrefToXSLTServer == XSL_KPT10_b_pub)
                    hrefToXSLTServer = XSL_KPT10_b_lan;//заменить на строку

                if (hrefToXSLTServer == XSL_KVOKS02_pub)
                    hrefToXSLTServer = XSL_KVOKS02_lan;//заменить на строку

                if (hrefToXSLTServer == XSL_KPOKS0401_pub)
                    hrefToXSLTServer = XSL_KPOKS0401_lan;//заменить на строку

            }

            if (doc.DocumentElement.Name == "TP") // если получен ТехническийПлан, применим наш стиль
                hrefToXSLTServer = XSL_V03_TP;

            if (doc.DocumentElement.Name == "MP") // если получен ТехническийПлан, применим наш стиль
                hrefToXSLTServer = XSL_V06_MP;

            string OutName = inputXml + "~.html";
            if (hrefToXSLTServer != null)
            {
                try
                {

                    System.Xml.Xsl.XslTransform transform = new System.Xml.Xsl.XslTransform();
                    System.Xml.Xsl.XsltSettings xsopt = new System.Xml.Xsl.XsltSettings();
                    transform.Load(hrefToXSLTServer);
                    xmlr.Close();
                    transform.Transform(inputXml, OutName);
                    return OutName;
                }
                catch (Exception e)
                {
                    string s = e.Message;
                    return "";
                }


            }
            else return "";
        }
    }
}