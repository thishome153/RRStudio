#region Вся используемые NameSpaces
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
//using System.IO.Compression; //zip, only net since 4.5 ... coming soon
using Ionic.Zip;
using System.Net; // http???
using EBop.MapObjects.MapInfo;


using System.Runtime.InteropServices;


using netFteo.Spatial;
using RRTypes.pkk5;

#endregion

namespace XMLReaderCS
          
{
   
  public  partial class KVZU_Form : Form
    { // Глобальные объекты:
        IntPtr Ptr;
        string TextDefault; // Текст заголовока по учмолчанию

        RRTypes.kvoks_v02.KVOKS KVoks02 = new RRTypes.kvoks_v02.KVOKS();
        RRTypes.kpoks_v03.KPOKS KPoks03 = new RRTypes.kpoks_v03.KPOKS();
        RRTypes.MP_V05.MP MPV05;
        //System.Windows.Window ESwindow;
        netFteo.MyWindowEx ESwindow;
        EntityViewer ViewWindow; // xaml WPF control

        /// <summary>
        /// Current file properies
        /// </summary>
        public netFteo.XML.FileInfo DocInfo = new netFteo.XML.FileInfo(); 
        ZipFile zip;

        //public string FileName;
        //public string FilePath;        
        string pathToHtmlFile;
        string hrefToXSLT;
        string[] args; //Аргументы коммандной строки

        /// <summary>
        /// List of files, dragged on to form
        /// </summary>
        string[] DraggedFiles; 

        /// <summary>
        /// Default WorkingDirectory for temporary deflating archives
        /// </summary>
        string Folder_Unzip;

        string ArchiveFolder; //Текущий файл архива
        /// <summary>
        /// Default WorkingDirectory 
        /// </summary>
        string Folder_AppStart;
        string Folder_XSD;
        netFteo.XML.XSDFile dutilizations_v01;
        netFteo.XML.XSDFile dRegionsRF_v01;
        netFteo.XML.XSDFile dStates_v01;
        netFteo.XML.XSDFile dLocationLevel1_v01;//.xsd;
        string dLocationLevel2_v01;
        public string MP_06_schema;

        const string FixosoftKey = "HKEY_CURRENT_USER\\Software\\Fixosoft\\GKNData\\NET";      

        //Импорт библиотеки
        [DllImport("ESViewer_lib_mcvc.dll")]
        public static extern void Function1(int id);

        // c++ Fteo 6 library
        [DllImport("ESChecker.dll")]
        unsafe  public static extern void *Func2(int id);

       // c++ CodeBlocks library
        [DllImport("ESlib.dll")]
        public static extern void Function2(int id);

            //Конструктор:
        public KVZU_Form()
        {
            InitializeComponent();
            this.Tag = 1; // "Как приложение"
            ClearControls();
            this.Folder_AppStart = Application.StartupPath;
            this.Folder_XSD = Folder_AppStart+"\\Schema";
            this.Folder_Unzip = Folder_AppStart + "\\~tmp.zip";
            InitSchemas(this.Folder_XSD); 

            /// Позиция запуска
            ///HKEY_CURRENT_USER\Software\Fixosoft\GKNData\FormState\MainForm 
            const string keyName = FixosoftKey + "\\FormStartPosition";
            //Проверим, есть ли запись:
            if (Microsoft.Win32.Registry.GetValue(keyName, "ActivePosition", 0) != null)
                if ((int)Microsoft.Win32.Registry.GetValue(keyName, "ActivePosition", 0) == 1)
                {
                    this.StartPosition = FormStartPosition.Manual;
                    this.Top = (int)Microsoft.Win32.Registry.GetValue(keyName, "Top", this.Top) + 20;
                    this.Left = (int)Microsoft.Win32.Registry.GetValue(keyName, "Left", this.Left) + 20;
                }
                else this.StartPosition = FormStartPosition.WindowsDefaultLocation;

            const string keyName_LastDir = FixosoftKey+ "\\Recent";
            string test_LastDir = (string)Microsoft.Win32.Registry.GetValue(keyName_LastDir, "LastDir", 0);
            //if (Microsoft.Win32.Registry.GetValue(keyName, "LastDir", 0) == )
            ESwindow = new netFteo.MyWindowEx(); //System.Windows.Window();// Окно визуализации ПД
        }

        private void SaveRegistry(string lastdir)
        {
            const string subKey_LastDir = FixosoftKey + "\\Recent";
            const string keyName_LastDir = "" + subKey_LastDir;
            Microsoft.Win32.Registry.SetValue(keyName_LastDir, "LastDir", lastdir);
        }

        private void SaveLastDir(string lastdir)
        {
            XMLReaderCS.Properties.Settings.Default.LastDir = lastdir;
            SaveRegistry(lastdir);
        }


        private static string GetXPathToNode(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Attribute)
            {
                // attributes have an OwnerElement, not a ParentNode; also they have
                // to be matched by name, not found by position
                return String.Format(
                    "{0}/@{1}",
                    GetXPathToNode(((XmlAttribute)node).OwnerElement),
                    node.Name
                    );
            }
            if (node.ParentNode == null)
            {
                // the only node with no parent is the root node, which has no path
                return "";
            }
            //get the index
            int iIndex = 1;
            XmlNode xnIndex = node;
            while (xnIndex.PreviousSibling != null) { iIndex++; xnIndex = xnIndex.PreviousSibling; }
            // the path to a node is the path to its parent, plus "/node()[n]", where 
            // n is its position among its siblings.
            return String.Format(
                "{0}/node()[{1}]",
                GetXPathToNode(node.ParentNode),
                iIndex
                );
        }

        //Обработчик события OnDXFParsing
        private void DXFStateUpdater(object Sender, ESCheckingEventArgs e)
        {
            int currProc = e.Process;
            /*
            if (e.Definition == "26:01:071402:12")
            {
                int here100 = 1000;
            }
            */
            if (e.Process < toolStripProgressBar1.Maximum)
                toolStripProgressBar1.Value = e.Process;
            // toolStripStatusLabel3.Text = e.Definition;
            this.Update();
        }

        private void XMLStartUpdater(object Sender, ESCheckingEventArgs e)
        {
                toolStripProgressBar1.Maximum = e.Process;
            this.Update();
        }


        #region Открываем Файл (слава http://code.google.com/p/xsd-to-classes/wiki/Usage ):
        /// <summary>
        /// Открыть файл xml
        /// </summary>
        private void OpenFile()
        {
            openFileDialog1.Filter = "Сведения ЕГРН, ТехПлан, Межевой план|*.xml;*.zip;*.xsd;"+
                "|Про$транственные данные|*.dxf;*.mif;*.txt";
            openFileDialog1.FileName = XMLReaderCS.Properties.Settings.Default.Recent0;
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                Read(openFileDialog1.FileName, true);

        }
        /// <summary>
        ///   Читать документ из объекта(instance)
        /// </summary>
        /// <param name="xmldoc">Объект типа XMLDocument</param>
        public void Read(XmlDocument xmldoc)
        {
            if (xmldoc == null)
            {
                toolStripStatusLabel1.Text = "document null" ;
                return;
            }
            DocInfo.DocRootName = xmldoc.DocumentElement.Name;
            DocInfo.Namespace = xmldoc.DocumentElement.NamespaceURI;  // "urn://x-artefacts-rosreestr-ru/outgoing/kpt/10.0.1"
                                                                      // "urn://x-artefacts-rosreestr-ru/outgoing/kpt/9.0.3"
            if (xmldoc.DocumentElement.Attributes.GetNamedItem("Version") != null) // Для MP версия в корне
                DocInfo.Version = xmldoc.DocumentElement.Attributes.GetNamedItem("Version").Value;
            toolStripStatusLabel2.Text = "<" + DocInfo.DocRootName + "> " + label_FileSize.Text;
            toolStripStatusLabel3.Text = DocInfo.Namespace;
            linkLabel_tns.Text = DocInfo.Namespace;
            документToolStripMenuItem.Enabled = true;
            // Вначале отобразим xml, вдруг далее парсеры слажают... :)
            cXmlTreeView2.RootName = DocInfo.FileName;
            tabPage5.Text = this.DocInfo.FileName;
            cXmlTreeView2.LoadXML(xmldoc); // Загрузим тело в дерево XMlTreeView - собственный клас/компонент, умеющий показывать XmlDocument


            Stream stream = new MemoryStream(); 
            xmldoc.Save(stream);
            stream.Seek(0, 0);
            RRTypes.CommonParsers.Doc2Type parser = new RRTypes.CommonParsers.Doc2Type(); //prepare parser

            if (DocInfo.DocRootName == "TMyPolygon")
            {
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                tabPage1.Text = "netfteo::TMyPolygon";
                {
                    toolStripStatusLabel3.Text = "netfteo::";
                    XmlSerializer serializer = new XmlSerializer(typeof(TMyPolygon));
                    TMyPolygon xmlPolygon = (TMyPolygon)serializer.Deserialize(stream);
                    ParseTMyPolygon(xmlPolygon);
                }
            }

            if (DocInfo.DocRootName == "TPolygonCollection")
            {
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                tabPage1.Text = "PolygonCollection";
                {
                    toolStripStatusLabel3.Text = "netfteo::";
                    XmlSerializer serializer = new XmlSerializer(typeof(TPolygonCollection));
                    TPolygonCollection xmlPolygons = (TPolygonCollection)serializer.Deserialize(stream);
                    ParseTMyPolygon(xmlPolygons);
                }
            }
            // EZP hard editing:
            if (DocInfo.DocRootName == "tExistEZEntryParcelCollection")
            {
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                tabPage1.Text = "tExistEZEntryParcelCollection";
                {
                    toolStripStatusLabel3.Text = "tExistEZEntryParcelCollection";
                    XmlSerializer serializer = new XmlSerializer(typeof(RRTypes.MP_V06.tExistEZEntryParcelCollection));
                    RRTypes.MP_V06.tExistEZEntryParcelCollection xmlPolygons = (RRTypes.MP_V06.tExistEZEntryParcelCollection)serializer.Deserialize(stream);
                }
            }

            //Если это КВЗУ V04/V05
            if (DocInfo.DocRootName == "Region_Cadastr_Vidimus_KV")
            {
                  toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                string ver_Vidimus="";
                if ((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument") != null) &&
                        (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version") != null))       
                    ver_Vidimus = xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version").Value;

                if ( ver_Vidimus.Equals("04")) // Проверим
                {
                    this.DocInfo = parser.ParseKVZU04(this.DocInfo, xmldoc);
                }
                // Проверим
                if ((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name).Attributes.GetNamedItem("Version") != null) &&
                    (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name).Attributes.GetNamedItem("Version").Value.Equals("05")))
                {
                    this.DocInfo = parser.ParseKVZU05(this.DocInfo, xmldoc);
                }
            }

            if ((DocInfo.DocRootName == "KVZU") & (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kvzu/6.0.9"))
            {
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                tabPage1.Text = "Кадастровая выписка 6";
                this.DocInfo = parser.ParseKVZU06(this.DocInfo, xmldoc);
            }


            if ((DocInfo.DocRootName == "KVZU") & (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kvzu/7.0.1"))
            {
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                this.DocInfo = parser.ParseKVZU07(this.DocInfo, xmldoc);
            }

            if (DocInfo.DocRootName == "KPZU")
            {
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                if (DocInfo.Namespace != "urn://x-artefacts-rosreestr-ru/outgoing/kpzu/6.0.1")
                {
                      this.DocInfo = parser.ParseKPZU508(this.DocInfo, xmldoc);
                }

                // KPZU_V6 01  - ЕГРН
                if (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kpzu/6.0.1")
                {
                    this.DocInfo = parser.ParseKPZU(this.DocInfo, xmldoc);
                }
            }
            
            //Выписка ЕГРП, блядь есть и такое
            if (xmldoc.DocumentElement.Name == "Extract")
            {
                this.DocInfo = parser.ParseEGRP(this.DocInfo, xmldoc);

            }


            if (xmldoc.DocumentElement.Name == "Users")
            {
                XmlElement xRoot = xmldoc.DocumentElement;

                // выбор всех дочерних узлов
                //XmlNodeList childnodes = xRoot.SelectNodes("*");
                //Выберем все узлы <user>:
                XmlNodeList childnodes = xRoot.SelectNodes("//user/company");
            }



            if (DocInfo.DocRootName == "KVOKS")
            {
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                if (DocInfo.Namespace != "urn://x-artefacts-rosreestr-ru/outgoing/kvoks/3.0.1")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RRTypes.kvoks_v02.KVOKS));
                    KVoks02 = (RRTypes.kvoks_v02.KVOKS)serializer.Deserialize(stream);
                    ParseKVOKS(KVoks02);
                }
             
                //Под этим urn urn://x-artefacts-rosreestr-ru/outgoing/kvoks/3.0.1 как бы выписка версии KVOKS_V07
                if (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kvoks/3.0.1")
                {
                    this.DocInfo = parser.ParseKVOKS07(this.DocInfo, xmldoc);
                }
            }

            if (DocInfo.DocRootName == "KPOKS")
            {
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;

                if (DocInfo.Namespace != "urn://x-artefacts-rosreestr-ru/outgoing/kpoks/4.0.1")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RRTypes.kpoks_v03.KPOKS));
                    KPoks03 = (RRTypes.kpoks_v03.KPOKS)serializer.Deserialize(stream);
                    ParseKPOKS(KPoks03);
                }

                // KPOKS_V4
                if (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kpoks/4.0.1")
                {
                    this.DocInfo = parser.ParseKPOKS(this.DocInfo, xmldoc);
                }
            }


            if ((DocInfo.DocRootName == "Region_Cadastr"))
            {
                DocInfo.DocTypeNick = "КПТ";
                DocInfo.DocType = "Кадастровый план территории";
                //Не КПТ v07 ли это?   
                if (((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument") != null) &&
                  (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version") != null) &&
                    (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name + "/eDocument").Attributes.GetNamedItem("Version").Value.Equals("07"))))
                {
                    DocInfo.Version = "07..coming soon";
                }

                //Не КПТ v08 ли это?            
                if ((xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name).Attributes.GetNamedItem("Version") != null) &&
                (xmldoc.SelectSingleNode(xmldoc.DocumentElement.Name).Attributes.GetNamedItem("Version").Value.Equals("08")))
                {
                    toolStripProgressBar1.Minimum = 0;
                    toolStripProgressBar1.Value = 0;
                    parser.OnParsing += DXFStateUpdater;
                    parser.OnStartParsing += XMLStartUpdater;
                    DocInfo.Version = "08";
                    this.DocInfo = parser.ParseKPT08(this.DocInfo, xmldoc);
                }
            }
            
            //Не КПТ v09 ли это?            
            if ((DocInfo.DocRootName == "KPT") && (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kpt/9.0.3"))
            {
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                this.DocInfo = parser.ParseKPT09(this.DocInfo, xmldoc);
            }

            //Не КПТ v10 ли это?
            if ((DocInfo.DocRootName == "KPT") && (DocInfo.Namespace == "urn://x-artefacts-rosreestr-ru/outgoing/kpt/10.0.1"))
            {
                 toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                this.DocInfo =  parser.ParseKPT10(this.DocInfo, xmldoc);
            }



            /*
                if (DocInfo.DocRootName == "MapPlan")
                {
                    toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                    XmlSerializer serializerMP = new XmlSerializer(typeof(RRTypes.MapPlanV01.MapPlan));
                    //TextReader readerMP = new StreamReader(FileName);
                    MapPlanEditor.MapPlanForm MPF = new MapPlanEditor.MapPlanForm();
                    MPF.Top = this.Top - 5; MPF.Left = this.Left + 5;
                    MPF.MPv01 = (RRTypes.MapPlanV01.MapPlan)serializerMP.Deserialize(stream);
                    this.Hide();
                    MPF.ShowDialog();
                    //readerMP.Close();
                    this.Close();
                }



                if (DocInfo.DocRootName == "ZoneToGKN")
                {
                    toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.arrow_switch;
                    XmlSerializer serializerZN = new XmlSerializer(typeof(RRTypes.ZoneV03.ZoneToGKN));
                    //TextReader readerMP = new StreamReader(FileName);
                    MapPlanEditor.frmZoneV03Editor ZNf = new MapPlanEditor.frmZoneV03Editor();
                    ZNf.Zone03 = (RRTypes.ZoneV03.ZoneToGKN)serializerZN.Deserialize(stream);
                    //readerMP.Close();
                    ZNf.Top = this.Top - 5; ZNf.Left = this.Left + 5;
                    this.Hide();
                    ZNf.ShowDialog();
                    this.Close();

                }
             * */
            
              if (DocInfo.DocRootName == "SchemaParcels")
                {
                    toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.misc28;
                    SchemaKPTMainForm frm = new SchemaKPTMainForm();
                    frm.Top = this.Top; frm.Left = this.Left ;
                    frm.StartPosition = FormStartPosition.Manual;
                    frm.OpenXML( xmldoc);
                    this.Visible = false;
                    frm.ShowDialog();
                    this.Close();
                }
                

            if (DocInfo.DocRootName == "STD_MP")
            {
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                /*
                XmlSerializer serializerMP = new XmlSerializer(typeof(RRTypes.STD_MPV04.STD_MP));
                RRTypes.STD_MPV04.STD_MP MP = (RRTypes.STD_MPV04.STD_MP)serializerMP.Deserialize(stream);
                ParseSTDMPV04(MP);
                */
                this.DocInfo = parser.ParseMPV04(this.DocInfo, xmldoc);

            }


            if ((DocInfo.DocRootName == "MP") && (DocInfo.Version == "05"))
            {

                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                XmlSerializer serializerMP = new XmlSerializer(typeof(RRTypes.MP_V05.MP));
                MPV05 = (RRTypes.MP_V05.MP)serializerMP.Deserialize(stream);
                ParseMPV05(MPV05);
            }

            // Типы MP Версия 06 - без XSD to clasess. напрямую XSD.exe
            if ((DocInfo.DocRootName == "MP") && (DocInfo.Version == "06"))
            {
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.cross;
                this.DocInfo = parser.ParseMPV06(this.DocInfo, xmldoc);
            }


            if (DocInfo.DocRootName == "STD_TP")
            {
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                XmlSerializer serializerTP = new XmlSerializer(typeof(RRTypes.STD_TPV02.STD_TP));
                RRTypes.STD_TPV02.STD_TP TP = (RRTypes.STD_TPV02.STD_TP)serializerTP.Deserialize(stream);
                ParseSTDTPV02(TP);
            }

            //TP
            if (DocInfo.DocRootName == "TP")
            {
                toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.asterisk_orange;
                this.DocInfo = parser.ParseTP_V03(this.DocInfo, xmldoc);
            }

        }


        /// <summary>
        /// Читать документ из файла
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="NeedListing">Надо ли отображать коллекции после чтения</param>
        public void Read(string FileName, bool NeedListing)
        {
            if (!File.Exists(FileName))
            {
                toolStripStatusLabel1.Text = "not exist:"+ Path.GetFileName(FileName);
                return;
            }

            ClearControls();
            PreloaderMenuItem.LoadingCircleControl.Active = true;
            PreloaderMenuItem.Visible = true;
            Application.DoEvents();
            SaveLastDir(Path.GetDirectoryName(FileName));
            if (File.Exists(FileName + "~.html")) File.Delete(FileName + "~.html"); // если есть предыдущий сеанс
            linkLabel_FileName.Text = Path.GetFileName(FileName);
            toolStripStatusLabel1.Text = Path.GetFileName(FileName);
            label_FileSize.Text = FileSizeAdapter.FileSize(FileName);
            // got mif file:
            if (Path.GetExtension(FileName).ToUpper().Equals(".MIF"))
            {
                netFteo.IO.TextReader mifreader = new netFteo.IO.TextReader();
                TPolygonCollection polyfromMIF = mifreader.ImportMIF(FileName);

                // Virtual Parcel with contours:
                TMyCadastralBlock Bl = new TMyCadastralBlock();
                Bl.CN = "Полигоны MIF";
                
                TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(Path.GetFileNameWithoutExtension(FileName), "Item05"));
                if (polyfromMIF.Count == 1)
                {
                    MainObj.Name = netFteo.Rosreestr.dParcelsv01.ItemToName("Item01"); 
                    MainObj.EntitySpatial = polyfromMIF[0];
                }
                else
                    MainObj.Contours.AddPolygons(polyfromMIF);

                this.DocInfo.MyBlocks.Blocks.Clear();
                this.DocInfo.MyBlocks.Blocks.Add(Bl);
                this.DocInfo.DocTypeNick = "Mapinfo mif";
                this.DocInfo.CommentsType = "MIF";
                this.DocInfo.Comments = mifreader.Body;
                this.DocInfo.Encoding = mifreader.BodyEncoding;
                this.DocInfo.Number = "Mapinfo mif,  " + mifreader.BodyEncoding;
              }

            // got AutoCad Drawing Exchange Format -  dxf file:
            if (Path.GetExtension(FileName).ToUpper().Equals(".DXF"))
            {

                try
                {
                    netFteo.IO.DXFReader mifreader = new netFteo.IO.DXFReader(FileName);
                    toolStripProgressBar1.Maximum = mifreader.PolygonsCount();  //TODO: number of items ???
                    toolStripProgressBar1.Minimum = 0;
                    toolStripProgressBar1.Value = 0;
                    mifreader.OnParsing += DXFStateUpdater;
                    RRTypes.CommonParsers.Doc2Type parser = new RRTypes.CommonParsers.Doc2Type();
                    this.DocInfo = parser.ParseDXF(this.DocInfo, mifreader);
                }

                catch (ArgumentException err)
                {
                    ClearControls();
                    this.DocInfo.DocTypeNick = "dxf";
                    this.DocInfo.CommentsType = "DXF error...";
                    this.DocInfo.Comments = err.Message;
                }


            }

                if (Path.GetExtension(FileName).ToUpper().Equals(".TXT"))
            {
                netFteo.IO.TextReader mifreader = new netFteo.IO.TextReader();
                TPolygonCollection polyfromMIF = mifreader.ImportTxtFile(FileName);
                if (polyfromMIF != null)
                {

                    // Virtual Parcel with contours:
                    TMyCadastralBlock Bl = new TMyCadastralBlock();
                    Bl.CN = "Полигоны txt";
                    TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(Path.GetFileNameWithoutExtension(FileName), "Item05"));

                    if (polyfromMIF.Count == 1)
                    {
                        MainObj.Name = netFteo.Rosreestr.dParcelsv01.ItemToName("Item01");
                        MainObj.EntitySpatial = polyfromMIF[0];
                    }
                    else
                        MainObj.Contours.AddPolygons(polyfromMIF);

                    this.DocInfo.MyBlocks.Blocks.Clear();
                    this.DocInfo.MyBlocks.Blocks.Add(Bl);
                }

                this.DocInfo.DocTypeNick = "Текстовый файл";
                this.DocInfo.CommentsType = "TXT";
                this.DocInfo.Comments = mifreader.Body;
                this.DocInfo.Encoding = mifreader.BodyEncoding;
                this.DocInfo.Number = "Текстовый файл,  " + mifreader.BodyEncoding;
            }


            if (Path.GetExtension(FileName).Equals(".xml"))
            {
               this.DocInfo.FileName = Path.GetFileName(FileName);
               this.DocInfo.FilePath = Path.GetFullPath(FileName);
               TextReader reader = new StreamReader(FileName);
               XmlDocument XMLDocFromFile = new XmlDocument();
               XMLDocFromFile.Load(reader);
               reader.Close();
                Read(XMLDocFromFile);

#if (DEBUG)
               //LV_SchemaDisAssembly.Visible =  TODO...;                      
#else
                //LV_SchemaDisAssembly.Visible = false;                      
#endif
                this.DocInfo.MifPolygons.Defintion = Path.GetFileName(FileName);
                saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(this.DocInfo.MifPolygons.Defintion) + "_mif";
                //На пся крев просидел два дня....  SaveOpenedFileInfo(DocInfo, FileName);
            }

            // Got XSD schema file
            if (Path.GetExtension(FileName).Equals(".xsd"))
            {
                netFteo.XML.XSDFile xsdenum = new netFteo.XML.XSDFile(FileName);
            }

            // We recieve archive package GKUOKS, GKUZU:
            if (Path.GetExtension(FileName).Equals(".zip"))
                if (FileName.Contains("GKU"))
                {
                    ClearFiles();
                    BackgroundWorker w1 = new BackgroundWorker();
                    w1.WorkerSupportsCancellation = false;
                    w1.WorkerReportsProgress = true;
                    w1.DoWork += this.UnZipit;
                    w1.RunWorkerCompleted += this.UnZipComplete;
                    w1.RunWorkerAsync(FileName);
                }

            //Если есть парная ЭЦП:
            if (File.Exists(FileName + ".sig"))
            {
                frmCertificates certfrm = new frmCertificates();
                List<string> sigs = certfrm.ParseSignature(FileName+".sig");
                if (sigs != null)
                    foreach (string sig in sigs)
                        textBox_FIO.Text += "\n ЭЦП= " + sig;

            }

            if (NeedListing)
            {
                ListMyCoolections(this.DocInfo.MyBlocks, this.DocInfo.MifPolygons);
                ListFileInfo(DocInfo);
            }
            PreloaderMenuItem.LoadingCircleControl.Active = false;
            PreloaderMenuItem.Visible = false;
        }


        private void InitSchemas(string folder_xsd)
        {
            dutilizations_v01 = new netFteo.XML.XSDFile(folder_xsd + "\\SchemaCommon" + "\\dUtilizations_v01.xsd");
            dRegionsRF_v01 = new netFteo.XML.XSDFile(folder_xsd + "\\SchemaCommon" + "\\dRegionsRF_v01.xsd");
            dStates_v01 = new netFteo.XML.XSDFile(folder_xsd + "\\SchemaCommon" + "\\dStates_v01.xsd");
            dLocationLevel1_v01 = new netFteo.XML.XSDFile(folder_xsd + "\\SchemaCommon" + "\\dLocationLevel1_v01.xsd");
            dLocationLevel2_v01 = folder_xsd + "\\SchemaCommon" + "\\dLocationLevel2_v01.xsd";
            MP_06_schema = folder_xsd + "\\V06_MP" + "\\MP_v06.xsd";
        }

        

      //Zip utils 
        public void ZipExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Object, ExtractProgressEventArgs>(ZipExtractProgress),  new Object[] { sender, e });
            }
            else
            {
                switch (e.EventType)
                {
                        /*
                    case ZipProgressEventType.Extracting_BeforeExtractAll:
                        //Console.WriteLine("pb max {0}", e.EntriesTotal);
                        this.progressBar1.Maximum = e.EntriesTotal;
                        this.progressBar1.Value = 0;
                        this.progressBar1.Minimum = 0;
                        this.progressBar1.Step = 1;
                        break;
                        */
                    case ZipProgressEventType.Extracting_BeforeExtractEntry:
                        //Console.WriteLine("entry {0}", e.CurrentEntry.FileName);
                        this.toolStripStatusLabel3.Text ="["+e.EntriesTotal.ToString()+"] " + e.CurrentEntry.FileName;
                        break;

                    case ZipProgressEventType.Extracting_AfterExtractEntry:
                        this.toolStripProgressBar1.PerformStep();
                        break;
                }
                this.Update();
                Application.DoEvents();
            }
        }

      /*
        public void ZipReadProgress(object sender, ReadProgressEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Object, ReadProgressEventArgs>(ZipReadProgress), new Object[] { sender, e });
            }
            else
            {
                switch (e.EventType)
                {
                    case ZipProgressEventType.Reading_Completed:
                        //Console.WriteLine("pb max {0}", e.EntriesTotal);
                        this.progressBar1.Maximum = e.EntriesTotal;
                        this.progressBar1.Value = 0;
                        this.progressBar1.Minimum = 0;
                        this.progressBar1.Step = 1;
                        break;
                }
                this.Update();
                Application.DoEvents();
            }
        }
      */
        private void UnZipit(object sender, DoWorkEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Object, DoWorkEventArgs>(UnZipit), new Object[] { sender, e });
            }
            else
            {
                try
                {
                    ReadOptions ro = new ReadOptions() { Encoding = Encoding.ASCII };
                    //ro.ReadProgress += ZipReadProgress;

                    zip = ZipFile.Read((string)e.Argument, ro);
                    this.toolStripProgressBar1.Maximum = zip.Count();
                    this.toolStripProgressBar1.Value = 0;
                    this.toolStripProgressBar1.Minimum = 0;
                    this.toolStripProgressBar1.Step = 1;

                    zip.ExtractProgress += this.ZipExtractProgress;
                    ArchiveFolder = this.Folder_Unzip +
                              "\\" + Path.GetFileNameWithoutExtension((string)e.Argument);
                    /*
                     */
                    //if (zip.EntryFileNames.Contains(".xml")) // wrong ???? not working .Contains !!!
                    {
                        zip.ExtractAll(ArchiveFolder);

                    }

                }
                catch (Exception ex1)
                {
                    this.richTextBox1.Text = "Zip Exception: " + ex1.ToString();
                }
            }
        }

      //Распаковка окончена, читаем результаты:
        private void UnZipComplete(object sender, RunWorkerCompletedEventArgs e)
        {
           // string ArchiveFolder = this.Folder_Unzip +
             //                 "\\" + Path.GetFileNameWithoutExtension((string)e.Argument);
            //string ArchiveFolder = (string)e.Result;
            if (Directory.Exists(ArchiveFolder))
            {
                DirectoryInfo di = new DirectoryInfo(ArchiveFolder);
                string firstFileName = di.GetFiles().Select(fi => fi.Name).FirstOrDefault(name => name != "*.xml");

                if (File.Exists(ArchiveFolder + "\\" + firstFileName))
                {
                    DocInfo.FileName = firstFileName;
                    //until unzipping, start checking MP for bugs :
                    if (firstFileName.Contains("GKUZU"))
                    {
                        BugReport_MP06_II(ArchiveFolder);
                    }
                    else

                        Read(ArchiveFolder + "\\" + firstFileName, true); // теперь загружаем xml
                }

            }



        }

        private void Zipit(object sender, DoWorkEventArgs e)
        {
            //int delay = 1200; // ms to keep form open after completion
            try
            {
                using (var zip = new ZipFile())
                {
                    /*
                    zip.AddSelectedFiles(selectionCriteria, ".", "", true);
                    zip.SaveProgress += this.SaveProgress;
                    zip.Save(zipfileName); */
                }
            }
            catch (Exception ex1)
            {
                this.label1.Text = "Exception: " +  ex1.ToString();
                //delay = 4000;
            }

            /*
            var timer1 =  new System.Timers.Timer(delay);
            timer1.Enabled = true;
            timer1.AutoReset = false;
            timer1.Elapsed += this.OnTimerEvent; */
        }



        //------------Запись xml-файла-спутника  pinfo_......xml---------------------------------------------------------------
        private void SaveOpenedFileInfo(netFteo.XML.FileInfo Doc, string FileName)
        {
            /*          Doc.Number = textBox_DocNum.Text;
                    Doc.Date = textBox_DocDate.Text;
                    Doc.FIO = textBox_FIO.Text;
                    Doc.FileName = FileName;
                    Doc.Parser = "XMLReaderCS";
                    Doc.ParsedDate = DateTime.Now.ToString();// DateTime.Today.ToString();

                    Doc.ParcelsCount = MyPacels.Parcels.Count.ToString();
                    Doc.BlocksCount = "1";

                    XSD_Schemes.tBlock Bl = new XSD_Schemes.tBlock();

            
                    for (int i = 0; i <= MyPacels.Parcels.Count-1; i++)
                    {
                        Bl.Parcels = new XSD_Schemes.tParcelCollection();
                        XSD_Schemes.tParcel Pl = new XSD_Schemes.tParcel();
                        Pl.ParcelCadastalNumber = MyPacels.Parcels[i].CN;
                        Bl.BlockCadastalNumber  = MyPacels.Parcels[i].CadastralBlock;
                        Bl.Parcels.Add(Pl);
                    }

                    Doc.Block.Add(Bl);
                    XmlSerializer serializer = new XmlSerializer(typeof(XSD_Schemes.XMLDocumentInfo));
                    TextWriter writer = new StreamWriter(Path.GetDirectoryName(FileName)+"\\pinfo_"+Path.GetFileNameWithoutExtension(FileName)+".xml");
                    serializer.Serialize(writer, Doc);
                    writer.Close();
             * */
        }
        #endregion


        private void Parse_Polygon(TMyPolygon xmlPolygon)
        {
       
        }

        private void ParseTMyPolygon(TMyPolygon xmlPolygon)
        {
            TMyCadastralBlock Bl = new TMyCadastralBlock();
            if (xmlPolygon.Definition != null)
                Bl.CN = xmlPolygon.Definition;
            else Bl.CN = "Polygon";
            TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(xmlPolygon.Definition, "nefteo::TMyPolygon"));
            MainObj.EntitySpatial = xmlPolygon;
            this.DocInfo.MyBlocks.Blocks.Add(Bl);
            ListMyCoolections(this.DocInfo.MyBlocks, this.DocInfo.MifPolygons);
        }

        private void ParseTMyPolygon(TPolygonCollection xmlPolygons)
        {
            TMyCadastralBlock Bl = new TMyCadastralBlock();
            Bl.CN = xmlPolygons.Defintion;// "imported xml polygons";

            for (int i = 0; i <= xmlPolygons.Count - 1; i++)
            {
                TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(xmlPolygons[i].Definition, "nefteo::TMyPolygon"));
                MainObj.EntitySpatial = xmlPolygons[i];    
            }
            this.DocInfo.MyBlocks.Blocks.Add(Bl);
            ListMyCoolections(this.DocInfo.MyBlocks, this.DocInfo.MifPolygons);
        }

   
        #region разбор Кадастрового паспорта  KPZU V05
        /*
        private void ParseKPZU(RRTypes.kpzu.KPZU kp)
        {
            TMyCadastralBlock Bl = new TMyCadastralBlock();
            label_DocType.Text = "Кадастровый паспорт";
            tabPage1.Text = "Земельные участки";
            textBox_DocNum.Text = kp.CertificationDoc.Number;
            textBox_DocDate.Text = kp.CertificationDoc.Date.ToString();
            if (kp.CertificationDoc.Official != null)
            {
                textBox_Appointment.Text = kp.CertificationDoc.Official.Appointment;
                textBox_Appointment.Text = kp.CertificationDoc.Official.FamilyName + " " + kp.CertificationDoc.Official.FirstName + " " + kp.CertificationDoc.Official.Patronymic;
            }

            textBox_OrgName.Text = kp.CertificationDoc.Organization;


            for (int i = 0; i <= kp.CoordSystems.Count - 1; i++)
            {
                
              this.DocInfo.MyBlocks.CSs.Add(new TCoordSystem(kp.CoordSystems[i].Name, kp.CoordSystems[i].CsId));

            }

            for (int i = 0; i <= kp.Contractors.Count - 1; i++)
            {
                ListViewItem LVi = new ListViewItem();
                LVi.Text = kp.Contractors[i].Date.ToString();
                LVi.SubItems.Add(kp.Contractors[i].FamilyName + " " + kp.Contractors[i].FirstName + " " + kp.Contractors[i].Patronymic);
                LVi.SubItems.Add(kp.Contractors[i].NCertificate);

                if (kp.Contractors[i].Organization != null)
                    LVi.SubItems.Add(kp.Contractors[i].Organization.Name);
                else LVi.SubItems.Add("-");
                listView_Contractors.Items.Add(LVi);
            }

            TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(kp.Parcel.CadastralNumber, kp.Parcel.Name.ToString()));
            MainObj.CadastralBlock = kp.Parcel.CadastralBlock;
            Bl.CN = kp.Parcel.CadastralBlock;
            MainObj.SpecialNote = kp.Parcel.SpecialNote;
            MainObj.Utilization.UtilbyDoc = kp.Parcel.Utilization.ByDoc;
            MainObj.Category = netFteo.Rosreestr.dCategoriesv01.ItemToName(kp.Parcel.Category.ToString());
             MainObj.Location = RRTypes.CommonCast.CasterZU.CastLocation(kp.Parcel.Location);
           // MainObj.Rights = KVZU_v06Utils.KVZURightstoFteorights(kp.Parcel.Rights);
            MainObj.Encumbrances = KPZU_v05Utils.KPZUEncumstoFteoEncums(kp.Parcel.Encumbrances);
            MainObj.AreaGKN = kp.Parcel.Area.Area;
            //OIPD:
            //Землепользование
            
            if (kp.Parcel.EntitySpatial != null)
                if (kp.Parcel.EntitySpatial.SpatialElement.Count > 0)
                {
                    MainObj.EntitySpatial = RRTypes.KPZU_v05Utils.AddEntSpatKPZU05(kp.Parcel.CadastralNumber,kp.Parcel.EntitySpatial);
                    this.DocInfo.MifPolygons.Add(RRTypes.KPZU_v05Utils.AddEntSpatKPZU05(kp.Parcel.CadastralNumber, kp.Parcel.EntitySpatial));
                }
            //Многоконтурный
            if (kp.Parcel.Contours != null)
            {

                for (int ic = 0; ic <= kp.Parcel.Contours.Count - 1; ic++)
                {
                    this.DocInfo.MifPolygons.Add(RRTypes.KPZU_v05Utils.AddEntSpatKPZU05(kp.Parcel.CadastralNumber + "(" +
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
                                                   kp.Parcel.CompositionEZ[i].Area.Area,6,
                                                    RRTypes.KPZU_v05Utils.AddEntSpatKPZU05(kp.Parcel.CompositionEZ[i].CadastralNumber,
                                                                     kp.Parcel.CompositionEZ[i].EntitySpatial));


                    this.DocInfo.MifPolygons.Add(RRTypes.KPZU_v05Utils.AddEntSpatKPZU05(kp.Parcel.CompositionEZ[i].CadastralNumber,
                                                           kp.Parcel.CompositionEZ[i].EntitySpatial));

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
                        this.DocInfo.MifPolygons.Add(SlEs);
                    }

                }
            }



            this.DocInfo.MyBlocks.Blocks.Add(Bl);
          //ListMyCoolections(this.DocInfo.MyBlocks, this.DocInfo.MifPolygons);
        }


  */
      

        #endregion



 

        #region разбор КВ на ОКС. Сооружение.  KPZU V02
        // С Наступающим 2016!
        private void ParseKVOKS(RRTypes.kvoks_v02.KVOKS kv)
        {

            label_DocType.Text = "Кадастровая выписка";
            tabPage1.Text = "ОКС";
            textBox_DocNum.Text = kv.CertificationDoc.Number;
            textBox_DocDate.Text = kv.CertificationDoc.Date.ToString("dd/MM/yyyy");
            if (kv.CertificationDoc.Official != null)
            {
                textBox_Appointment.Text = kv.CertificationDoc.Official.Appointment;
                textBox_Appointment.Text = kv.CertificationDoc.Official.FamilyName + " " + kv.CertificationDoc.Official.FirstName + " " + kv.CertificationDoc.Official.Patronymic;
            }

            textBox_OrgName.Text = kv.CertificationDoc.Organization;


            for (int i = 0; i <= kv.CoordSystems.Count - 1; i++)
            {
                this.DocInfo.MyBlocks.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));

            }

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

            if (kv.Realty.Building != null)
            {
                TMyCadastralBlock Bl = new TMyCadastralBlock(kv.Realty.Building.CadastralBlocks[0].ToString());
                TMyRealty Bld = new TMyRealty(kv.Realty.Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                Bld.Building.AssignationBuilding = kv.Realty.Building.AssignationBuilding.ToString();
                Bld.Name = kv.Realty.Building.Name;
                //Constructions.Address = KPT_v09Utils.AddrKPT09(kv.Realty.Construction.Address);
                Bld.Building.ES = RRTypes.CommonCast.CasterOKS.ES_OKS(kv.Realty.Building.CadastralNumber, kv.Realty.Building.EntitySpatial);
                Bl.AddOKS(Bld);
                //MifOKSPolygons.AddPolygon((TMyPolygon) Constructions.ES);
                this.DocInfo.MyBlocks.Blocks.Add(Bl);
            }



            if (kv.Realty.Construction != null)
            {
                TMyCadastralBlock Bl = new TMyCadastralBlock(kv.Realty.Construction.CadastralBlocks[0].ToString());
                TMyRealty  Constructions = new TMyRealty(kv.Realty.Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);

                Constructions.Construction.AssignationName = kv.Realty.Construction.AssignationName;
                Constructions.Name = kv.Realty.Construction.Name;
                Constructions.Construction.ES = RRTypes.CommonCast.CasterOKS.ES_OKS(kv.Realty.Construction.CadastralNumber, kv.Realty.Construction.EntitySpatial);
               foreach (RRTypes.kvoks_v02.tOldNumber n in kv.Realty.Construction.OldNumbers)
                      Constructions.Construction.OldNumbers.Add(n.Number);
                Bl.AddOKS(Constructions);
                this.DocInfo.MyBlocks.Blocks.Add(Bl);
            }

            ListMyCoolections(this.DocInfo.MyBlocks, this.DocInfo.MifPolygons);
        }

   

        //  Привет Росреесстру... а в это время здесь http://centsrv.geocom.lan/eclipse/hw_eclipse/gd_getBlocks.php  идет стройка 
        private void ParseKPOKS(RRTypes.kpoks_v03.KPOKS kv)
        {

            label_DocType.Text = "Кадастровый паспорт";
            tabPage1.Text = "ОКС";
            textBox_DocNum.Text = kv.CertificationDoc.Number;
            textBox_DocDate.Text = kv.CertificationDoc.Date.ToString("dd/MM/yyyy");
            if (kv.CertificationDoc.Official != null)
            {
                textBox_Appointment.Text = kv.CertificationDoc.Official.Appointment;
                textBox_Appointment.Text = kv.CertificationDoc.Official.FamilyName + " " + kv.CertificationDoc.Official.FirstName + " " + kv.CertificationDoc.Official.Patronymic;
            }

            textBox_OrgName.Text = kv.CertificationDoc.Organization;


            for (int i = 0; i <= kv.CoordSystems.Count - 1; i++)
            {
                this.DocInfo.MyBlocks.CSs.Add(new TCoordSystem(kv.CoordSystems[i].Name, kv.CoordSystems[i].CsId));

            }

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

            if (kv.Realty.Building != null)
            {
                TMyCadastralBlock Bl = new TMyCadastralBlock(kv.Realty.Building.CadastralBlocks[0].ToString());
                TMyRealty Bld = new TMyRealty(kv.Realty.Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                Bld.Building.AssignationBuilding = netFteo.Rosreestr.dAssBuildingv01.ItemToName(kv.Realty.Building.AssignationBuilding.ToString());
                Bld.Name = kv.Realty.Building.Name;
                Bld.Address = RRTypes.CommonCast.CasterOKS.CastAddress(kv.Realty.Building.Address);
                Bld.Area = kv.Realty.Building.Area;
                Bld.Building.ES = RRTypes.CommonCast.CasterOKS.ES_OKS(kv.Realty.Building.CadastralNumber, kv.Realty.Building.EntitySpatial);
                Bld.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Building.ObjectType);
                if (kv.Realty.Building.CadastralNumbersFlats != null)
                    if (kv.Realty.Building.CadastralNumbersFlats.Count() > 0)
                    {
                        foreach (string s in kv.Realty.Building.CadastralNumbersFlats)
                        {
                            TFlat flat = new TFlat(s);
                            Bld.Building.Flats.Add(flat);
                        }
                    }
                 
                Bl.AddOKS(Bld);
                //MifOKSPolygons.AddPolygon((TMyPolygon) Constructions.ES);
                this.DocInfo.MyBlocks.Blocks.Add(Bl);
            }



            if (kv.Realty.Construction != null)
            {
                TMyCadastralBlock Bl = new TMyCadastralBlock(kv.Realty.Construction.CadastralBlocks[0].ToString());
                TMyRealty Constructions = new TMyRealty(kv.Realty.Construction.CadastralNumber,netFteo.Rosreestr.dRealty_v03.Сооружение);
                Constructions.Construction.AssignationName = kv.Realty.Construction.AssignationName;
                //Constructions.Address = KPT_v09Utils.AddrKPT09(kv.Realty.Construction.Address);
                Constructions.Construction.ES = RRTypes.CommonCast.CasterOKS.ES_OKS(kv.Realty.Construction.CadastralNumber, kv.Realty.Construction.EntitySpatial);
                Constructions.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Construction.ObjectType);
                Bl.AddOKS(Constructions);
                //MifOKSPolygons.AddPolygon((TMyPolygon) Constructions.ES);
                this.DocInfo.MyBlocks.Blocks.Add(Bl);
            }

            ListMyCoolections(this.DocInfo.MyBlocks, this.DocInfo.MifPolygons);
        }


        #endregion
  
        #region  Разбор STD_TP ТехПлан

        private void ParseSTDTPV02(RRTypes.STD_TPV02.STD_TP TP)
        {
            label_DocType.Text = "Технический план";
            tabPage1.Text = "ОКС";
            if (TP.Construction != null)
            {
                
                richTextBox1.AppendText(TP.Construction.Conclusion);
                this.DocInfo.MyBlocks.CSs.Add(new TCoordSystem(TP.Construction.Coord_Systems[0].Name, TP.Construction.Coord_Systems[0].Cs_Id));
                ListViewItem LVi = new ListViewItem();
                LVi.Text = TP.Construction.Contractor.Date.ToString();
                LVi.SubItems.Add(TP.Construction.Contractor.Cadastral_Engineer.FIO.Surname + " " + TP.Construction.Contractor.Cadastral_Engineer.FIO.First +
                                     " " + TP.Construction.Contractor.Cadastral_Engineer.FIO.Patronymic);
                LVi.SubItems.Add(TP.Construction.Contractor.Cadastral_Engineer.N_Certificate);
                listView_Contractors.Items.Add(LVi);
                textBox_Appointment.Text = TP.Construction.Contractor.Cadastral_Engineer.FIO.Surname + " " + TP.Construction.Contractor.Cadastral_Engineer.FIO.First +
                                 " " + TP.Construction.Contractor.Cadastral_Engineer.FIO.Patronymic + ";  " + TP.Construction.Contractor.Cadastral_Engineer.E_mail;
                textBox_DocDate.Text = TP.Construction.Contractor.Date.ToString();
                if (TP.Construction.Contractor.Cadastral_Organization != null)
                textBox_OrgName.Text = TP.Construction.Contractor.Cadastral_Organization.Name;
                textBox_Appointment.Text = TP.Construction.Contractor.Cadastral_Engineer.N_Certificate;
                textBox_DocNum.Text = TP.GUID;

                if (TP.Construction.Package.New_Construction.Count> 0)
                {
                    TMyCadastralBlock Bl = new TMyCadastralBlock();
                    TMyRealty Constructions = new TMyRealty(TP.Construction.Package.New_Construction[0].Name, netFteo.Rosreestr.dRealty_v03.Сооружение);
                    Constructions.Construction.AssignationName = TP.Construction.Package.New_Construction[0].Assignation_Name;
                    Constructions.Address.Note = TP.Construction.Package.New_Construction[0].Location.Note;
                    Constructions.Construction.ES = RRTypes.CommonCast.CasterOKS.ES_OKS(TP.Construction.Package.New_Construction[0].Assignation_Name,
                                                                                     TP.Construction.Package.New_Construction[0].Entity_Spatial);
                    Bl.AddOKS(Constructions);
                    this.DocInfo.MifOKSPolygons.AddPolygon((TMyPolygon)Constructions.Construction.ES);
                    this.DocInfo.MyBlocks.Blocks.Add(Bl);
                }
                
                ListMyCoolections(this.DocInfo.MyBlocks, this.DocInfo.MifPolygons);

            }
        }
        private void ParseCoordSystems(RRTypes.V03_TP.tCoordSystems CS, string Conclusion)
        {
        }

        private void ParseGeneralCadastralWorks(RRTypes.V03_TP.tGeneralCadastralWorks GW, string Conclusion)
        {
            richTextBox1.AppendText("\n----------------------------------------------------------------Технический план v3" +
                                          "----------------------------------------------------------------\n Подготовлен " +
                            GW.DateCadastral.ToString().Replace("0:00:00", "") +
                            " в результате выполнения кадастровых работ в связи с\n" +
                            GW.Reason);
            richTextBox1.AppendText("\n------------------------------------------------   Заключение кадастрового инженера" +
                                       "   ------------------------------------------------\n");
            richTextBox1.AppendText(Conclusion + "\n ");
            richTextBox1.AppendText("\n----------------------------------------------------------------" +
                                       "----------------------------------------------------------------\n" +
                                       GW.Contractor.FamilyName +
                                 " " + GW.Contractor.FirstName +
                                 " " + GW.Contractor.Patronymic + "\n " +
                                 " " + GW.DateCadastral.ToString().Replace("0:00:00", ""));

            /*
            ListViewItem LVi = new ListViewItem();
            LVi.Text = GW.DateCadastral.ToString().Replace("0:00:00", "");
            LVi.SubItems.Add(GW.Contractor.FamilyName+
                                 " " + GW.Contractor.FirstName+
                                 " " + GW.Contractor.Patronymic);
            LVi.SubItems.Add(GW.Contractor.NCertificate);
            if (GW.Contractor.Organization != null)
            LVi.SubItems.Add(GW.Contractor.Organization.Name);

            listView_Contractors.Items.Add(LVi);
            */
            listView_Contractors.Visible = false;
            label4.Text = "Сведения о кадастровом инженере";
            textBox_FIO.Text = GW.Contractor.FamilyName +
                                 " " + GW.Contractor.FirstName +
                                 " " + GW.Contractor.Patronymic +
                                 "\n" + GW.Contractor.NCertificate;
            textBox_DocDate.Text = GW.DateCadastral.ToString().Replace("0:00:00", "");
            textBox_Appointment.Text = GW.Contractor.Email;
            if (GW.Contractor.Organization != null)
                textBox_OrgName.Text = GW.Contractor.Organization.Name + " \n" +
                                       GW.Contractor.Organization.AddressOrganization;
            else textBox_OrgName.Text = GW.Contractor.Address;
            label2.Text = "Заказчик работ";
            if (GW.Clients.Count == 1)
            {
                if (GW.Clients[0].Organization != null)
                {
                    linkLabel_Recipient.Text = GW.Clients[0].Organization.Name;
                    linkLabel_Request.Text = GW.Clients[0].Organization.INN;
                }

                if (GW.Clients[0].Person != null)
                {
                    linkLabel_Recipient.Text = GW.Clients[0].Person.FamilyName + " "
                        + GW.Clients[0].Person.FirstName + " " + GW.Clients[0].Person.Patronymic;
                    if (GW.Clients[0].Person.SNILS != null)
                        linkLabel_Request.Text = "СНИЛС " + GW.Clients[0].Person.SNILS;
                    else linkLabel_Request.Text = "";
                }
            }
        }
        
  
#endregion

        
        #region  Разбор Межевого Плана V05
        
      private void ParseMPV05(RRTypes.MP_V05.MP MP)
        {
            label_DocType.Text = "Межевой план";
                richTextBox1.AppendText("\n");
                richTextBox1.AppendText("\n____________________________________ТИТУЛЬНЫЙ ЛИСТ___________________________________");
                richTextBox1.AppendText("\nМежевой план подготовлен в результате выполнения кадастровых работ в связи с:");
                richTextBox1.AppendText("\n");
                richTextBox1.AppendText(MP.GeneralCadastralWorks.Reason);

            if (MP.Conclusion != null)
            {
                richTextBox1.AppendText("\n");
                richTextBox1.AppendText("\n______________________________________ЗАКЛЮЧЕНИЕ_____________________________________");
                richTextBox1.AppendText("\n");
                richTextBox1.AppendText(MP.Conclusion);
                richTextBox1.AppendText("\n");
            }
            //this.DocInfo.MyBlocks.CSs.Add(new TCoordSystem(MP.Coord_Systems.Coord_System.Name, MP.Coord_Systems.Coord_System.Cs_Id));
            ListViewItem LVi = new ListViewItem();
            ListViewItem LViadd = new ListViewItem();
            ListViewItem LViorg = new ListViewItem();
            ListViewItem LViTel = new ListViewItem();

            LVi.Text = MP.GeneralCadastralWorks.DateCadastral.ToString();
            LVi.SubItems.Add(MP.GeneralCadastralWorks.Contractor.FamilyName + " " + MP.GeneralCadastralWorks.Contractor.FirstName +
                                 " " + MP.GeneralCadastralWorks.Contractor.Patronymic);
            LVi.SubItems.Add(MP.GeneralCadastralWorks.Contractor.NCertificate);

            if (MP.GeneralCadastralWorks.Contractor.Organization != null)
            {
                LVi.SubItems.Add(MP.GeneralCadastralWorks.Contractor.Organization.Name);
            }


            LViadd.SubItems.Add(MP.GeneralCadastralWorks.Contractor.Address);
            LViadd.SubItems.Add("-");
            if (MP.GeneralCadastralWorks.Contractor.Organization != null)
            {
                LViadd.SubItems.Add(MP.GeneralCadastralWorks.Contractor.Organization.AddressOrganization);
            }

            LViorg.SubItems.Add(MP.GeneralCadastralWorks.Contractor.Email);
            LViTel.SubItems.Add(MP.GeneralCadastralWorks.Contractor.Telephone);
            listView_Contractors.Items.Add(LVi);
            listView_Contractors.Items.Add(LViadd);
            listView_Contractors.Items.Add(LViorg);
            listView_Contractors.Items.Add(LViTel);
            textBox_DocNum.Text = MP.GUID;
            textBox_DocDate.Text = MP.GeneralCadastralWorks.DateCadastral.ToString("dd/MM/yyyy");

            /*
            textBox_FIO.Text = MP.Title.Contractor.FIO.Surname + " " + MP.Title.Contractor.FIO.First +
                                 " " + MP.Title.Contractor.FIO.Patronymic + ";  " + MP.Title.Contractor.E_mail; ;

            textBox_OrgName.Text = MP.Title.Contractor.Organization;
            textBox_Appointment.Text = MP.Title.Contractor.N_Certificate;
            */
            TMyCadastralBlock Bl = new TMyCadastralBlock();
            string ParcelName;
            if (MP.Package.FormParcels != null)
            {
                for (int i = 0; i <= MP.Package.FormParcels.NewParcel.Count - 1; i++)
                {
                   if (MP.Package.FormParcels.NewParcel[i].Contours != null & MP.Package.FormParcels.NewParcel[i].Contours.Count > 0)
                        ParcelName = "Item05";
                    else
                        ParcelName = "Item01";
                    TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(MP.Package.FormParcels.NewParcel[i].Definition, ParcelName));
                      MainObj.AreaGKN = MP.Package.FormParcels.NewParcel[i].Area.Area;//Вычисленную??
                      MainObj.Category = netFteo.Rosreestr.dCategoriesv01.ItemToName(MP.Package.FormParcels.NewParcel[i].Category.Category.ToString());
                      MainObj.Utilization.UtilbyDoc = MP.Package.FormParcels.NewParcel[i].Utilization.ByDoc;
                      MainObj.Utilization.Untilization = MP.Package.FormParcels.NewParcel[i].Utilization.Utilization.ToString();
                      if (MP.Package.FormParcels.NewParcel[i].EntitySpatial != null)
                      {
                          MainObj.EntitySpatial.ImportPolygon(RRTypes.CommonCast.CasterZU.AddEntSpatMP5("",MP.Package.FormParcels.NewParcel[i].EntitySpatial));
                      }
                    if (MP.Package.FormParcels.NewParcel[i].Contours != null)
                        for (int ic = 0; ic <= MP.Package.FormParcels.NewParcel[i].Contours.Count - 1; ic++)
                        {
                            TMyPolygon NewCont = MainObj.Contours.AddPolygon(RRTypes.CommonCast.CasterZU.AddEntSpatMP5(MP.Package.FormParcels.NewParcel[i].Contours[ic].Definition,
                                                                            MP.Package.FormParcels.NewParcel[i].Contours[ic].EntitySpatial));
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
                    MainObj.AreaGKN = MP.Package.SpecifyParcel.ExistParcel.Area.Area;//Вычисленную??
                    if (MP.Package.SpecifyParcel.ExistParcel.Contours != null)
                    {
                        for (int ic = 0; ic <= MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour.Count - 1; ic++)
                        {
                            TMyPolygon NewCont = MainObj.Contours.AddPolygon(RRTypes.CommonCast.CasterZU.AddEntSpatMP5(MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour[ic].Definition,
                                                                            MP.Package.SpecifyParcel.ExistParcel.Contours.NewContour[ic].EntitySpatial));
                        }
                        for (int ic = 0; ic <= MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour.Count - 1; ic++)
                        {
                            TMyPolygon NewCont = MainObj.Contours.AddPolygon(RRTypes.CommonCast.CasterZU.AddEntSpatMP5(MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour[ic].NumberRecord,
                                                                            MP.Package.SpecifyParcel.ExistParcel.Contours.ExistContour[ic].EntitySpatial));
                        }
                    }

                  if (MP.Package.SpecifyParcel.ExistParcel.EntitySpatial != null)
                      MainObj.EntitySpatial.ImportPolygon(RRTypes.CommonCast.CasterZU.AddEntSpatMP5("",MP.Package.SpecifyParcel.ExistParcel.EntitySpatial));

            }
            // Уточнение ЕЗП
                if (MP.Package.SpecifyParcel.ExistEZ != null)
                {
                    ParcelName = "Item02"; // 02 = ЕЗП RRCommon.cs,  там есть public static class dParcelsv01
                    TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(MP.Package.SpecifyParcel.ExistEZ.ExistEZParcels.CadastralNumber, ParcelName));
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
                        Sl.Encumbrances.Add(new netFteo.Rosreestr.TMyEncumbrance() { Name = MP.Package.SubParcels.NewSubParcel[ii].Encumbrance.Name });
                        Sl.AreaGKN = MP.Package.SubParcels.NewSubParcel[ii].Area.Area;
                         if (MP.Package.SubParcels.NewSubParcel[ii].EntitySpatial != null) //Если одноконтурная чзу
                               Sl.EntSpat.ImportPolygon(RRTypes.CommonCast.CasterZU.AddEntSpatMP5(MP.Package.SubParcels.NewSubParcel[ii].Definition, MP.Package.SubParcels.NewSubParcel[ii].EntitySpatial));
                         if (MP.Package.SubParcels.NewSubParcel[ii].Contours != null)
                             Sl.Contours = RRTypes.CommonCast.CasterZU.AddContoursMP5(MP.Package.SubParcels.NewSubParcel[ii].Definition,MP.Package.SubParcels.NewSubParcel[ii].Contours);
                                                

                        MainObj.SubParcels.Add(Sl);
                    }



            }
            this.DocInfo.MyBlocks.Blocks.Add(Bl);
            ListMyCoolections(this.DocInfo.MyBlocks, this.DocInfo.MifPolygons);
        }


  
      // TODO:
      private void CreateSignature(string filename, string SubjectName)
      {
           
      }

  
      
        #endregion

        


        #region Отображение в TreeView Коллекций ЗУ и полигонов (из КВЗУ и КПТ)
        private void ListFileInfo(netFteo.XML.FileInfo fileinfo)
        {
            label_DocType.Text = fileinfo.DocType + " " + fileinfo.Version;// "КПТ + 10";;
            textBox_DocNum.Text = fileinfo.Number;
            if (fileinfo.Date != null)
            textBox_DocDate.Text = fileinfo.Date.ToString();
            textBox_OrgName.Text = fileinfo.Cert_Doc_Organization;
            textBox_Appointment.Text = fileinfo.Appointment;
            textBox_FIO.Text = fileinfo.AppointmentFIO;
            tabPage1.Text = fileinfo.DocTypeNick +" "+ fileinfo.Version;// "КПТ + 10";
            tabPage3.Text = fileinfo.CommentsType;// "Conclusion/Notes";
            linkLabel_Recipient.Text = fileinfo.ReceivName + " " + fileinfo.ReceivAdress;
            linkLabel_Request.Text = fileinfo.RequeryNumber;
            if (fileinfo.Comments != null)
            richTextBox1.AppendText(fileinfo.Comments.Replace("<br>", "\n"));

            
            foreach(netFteo.Rosreestr.TEngineerOut eng in fileinfo.Contractors)
            {
                ListViewItem LVi = new ListViewItem();
                LVi.Text = eng.Date;
                LVi.SubItems.Add(eng.FamilyName + " " + eng.FirstName + " " + eng.Patronymic);
                LVi.SubItems.Add(eng.NCertificate);

                if (eng.Organization_Name != null)
                    LVi.SubItems.Add(eng.Organization_Name);
                else LVi.SubItems.Add("-");

                listView_Contractors.Items.Add(LVi);
            }
            
        }

        /// </summary>
        /// <param name="kpt09"></param>
        private void ListMyCoolections(TMyBlockCollection BlockList, TPolygonCollection mifPolygons)
        {
            //TreeNode TopNode_ = TV_Parcels.Nodes.Add("TopNode", DocInfo.DocRootName);
            TreeNode TopNode_ = null;
            TV_Parcels.BeginUpdate();
            for (int bc = 0; bc <= BlockList.Blocks.Count - 1; bc++)
            {

                if (BlockList.Blocks.Count == 1)
                {

                    TopNode_ = TV_Parcels.Nodes.Add("TopNode", BlockList.Blocks[bc].CN);
                    this.Text = BlockList.Blocks[bc].CN;
                    if (BlockList.Blocks[bc].Parcels.Parcels.Count == 1)
                    {
                        this.Text = BlockList.Blocks[bc].Parcels.Parcels[0].CN;
                    }

                    if (BlockList.Blocks[bc].ObjectRealtys.Count == 1)
                    {
                        this.Text = BlockList.Blocks[bc].ObjectRealtys[0].CN;
                    }
                }// DocInfo.DocRootName);}

                else { TopNode_ = TV_Parcels.Nodes.Add("TopNode", DocInfo.DocRootName); }

                TMyParcelCollection ObjList = BlockList.Blocks[bc].Parcels;

                if (ObjList.Parcels.Count > 0)
                {
                    TreeNode ParcelsNode_ = TopNode_.Nodes.Add("ParcelsNode", "Земельные участки");
                    for (int i = 0; i <= ObjList.Parcels.Count - 1; i++)
                    {
                        TreeNode TnP = ListMyParcel(ParcelsNode_, ObjList.Parcels[i]);
                        if (ObjList.Parcels.Count == 1) TnP.Expand();
                    }

                    if (ObjList.Parcels.Count == 1)
                    {
                        pkk5Viewer1.Start(ObjList.Parcels[0].CN, pkk5_Types.Parcel);
                        ParcelsNode_.Expand();
                    }

                }

                if (BlockList.Blocks[bc].ObjectRealtys.Count > 0)
                {
                    TreeNode OKSsNode_ = TopNode_.Nodes.Add("OKSsNode", "ОКС");
                    for (int i = 0; i <= BlockList.Blocks[bc].ObjectRealtys.Count - 1; i++)
                    {
                        ListMyOKS(OKSsNode_, BlockList.Blocks[bc].ObjectRealtys[i]);
                    }
                }

                if (BlockList.Blocks[bc].GKNBounds.Count > 0)
                {
                    TreeNode BNDsNode_ = TopNode_.Nodes.Add("BNDsNode", "Границы ГКН");
                    for (int i = 0; i <= BlockList.Blocks[bc].GKNBounds.Count - 1; i++)
                    {
                        ListBound(BNDsNode_, BlockList.Blocks[bc].GKNBounds[i]);
                    }
                }

                if (BlockList.Blocks[bc].GKNZones.Count > 0)
                {
                    TreeNode BNDsNode_ = TopNode_.Nodes.Add("ZonesNode", "Объекты землеустройства-зоны");
                    for (int i = 0; i <= BlockList.Blocks[bc].GKNZones.Count - 1; i++)
                    {
                        ListZone(BNDsNode_, BlockList.Blocks[bc].GKNZones[i]);
                    }
                }

                //ОМС: все  в один,,,.
                if ((BlockList.OMSPoints.PointCount) != 0)
                {
                    TreeNode OMSNode = TopNode_.Nodes.Add("OMSPoints", "Пункты ОМС");
                    OMSNode.SelectedImageIndex = 5;
                    OMSNode.ImageIndex = 5;
                    // for (int i = 0; i <= BlockList.OMSPoints.PointCount - 1; i++)
                    ListPointList(OMSNode, BlockList.OMSPoints, 0);
                }

                //ОИПД Квартала
                if (BlockList.Blocks[bc].Entity_Spatial.PointCount != 0)
                {
                    //TreeNode KvEntitytNode = TopNode_.Nodes.Add("OMSPoints", "Границы квартала");
                    //TreeNode KvEntityItemNode = TopNode_.Nodes.Add("SPElem." + BlockList.Blocks[bc].Entity_Spatial.Layer_id, "Границы " + BlockList.Blocks[bc].CN);
                    //netFteo.ObjectLister.ListEntSpat(KvEntityItemNode, BlockList.Blocks[bc].Entity_Spatial);
                    netFteo.ObjectLister.ListEntSpat(TopNode_, BlockList.Blocks[bc].Entity_Spatial, "SPElem.", "Границы ",6);
                }

                //СК Квартала
                if (BlockList.CSs.Count != 0)
                {
                    //TreeNode KvEntitytNode = TopNode_.Nodes.Add("OMSPoints", "Границы квартала");
                    TreeNode CSSNode = TopNode_.Nodes.Add("CSS", "Системы координат ");
                    CSSNode.SelectedImageIndex = 4;
                    CSSNode.ImageIndex = 4;
                    for (int i = 0; i <= BlockList.CSs.Count - 1; i++)
                    {
                        TreeNode CSSItemNode = CSSNode.Nodes.Add("CSSItem", BlockList.CSs[i].Name);
                        CSSItemNode.ImageIndex = 4; CSSItemNode.SelectedImageIndex = 4;
                        TreeNode CSSidItemNode = CSSItemNode.Nodes.Add("CSSid", BlockList.CSs[i].CSid);
                    }
                }


            } // block block
            if (TopNode_ != null) TopNode_.Expand();
            TV_Parcels.EndUpdate();
            contextMenuStrip_SaveAs.Enabled = true;
        }

        //-------------------------------------------------------------------------------------------------------------------
        private TreeNode ListMyParcel(TreeNode Node, TMyParcel Parcel)
        {
            if (Parcel.SpecialNote != null)
            {
                tabPage3.Text = "Особые отметки";
                richTextBox1.AppendText(Parcel.SpecialNote);
            }
            else tabPage3.Hide();

            TreeNode PNode;
            if (Parcel.CN != null)
            {
                PNode = Node.Nodes.Add("PNode" + Parcel.id.ToString(), Parcel.CN);
            }
            else
            {
                PNode = Node.Nodes.Add("PNode" + Parcel.id.ToString(), Parcel.Definition);
            }
            PNode.ImageIndex = 1;
            PNode.SelectedImageIndex = 1;
            PNode.ForeColor = Color.Green;
            if (Parcel.ParentCN != null)
            { TreeNode ParNode = PNode.Nodes.Add("ParNode", "В составе " + Parcel.ParentCN); }
            if (Parcel.Location != null)
            {
                if (Parcel.Location.Elaboration.AsString() != null)
                {
                    TreeNode LocNameNode = PNode.Nodes.Add("LocNodes" + Parcel.id.ToString(), "Уточнение местоположения").Nodes.Add("LocNode", Parcel.Location.AsString());
                }
                ListAdress(PNode, Parcel.Location.Address, Parcel.id);
            }
           /*
            if (Parcel.Purpose != null)
            { TreeNode LPurposeNode = PNode.Nodes.Add("LPurposeNode", Parcel.Purpose);  }
            if (Parcel.UtilbyDoc != null)
            { TreeNode LUtildocNode = PNode.Nodes.Add("LUtildocNode", Parcel.UtilbyDoc); }
            */
            if (Parcel.State != null)
            {
                PNode.ToolTipText = dStates_v01.Item2Annotation(Parcel.State);
                PNode.ForeColor = RRTypes.KVZU_v06Utils.State2Color(Parcel.State.ToString());
            }

            if (Parcel.EntitySpatial != null)
                if (Parcel.EntitySpatial.Count > 0)
                {
                    //TreeNode ESNode = PNode.Nodes.Add("SPElem." + .ToString(), );
                    
                    netFteo.ObjectLister.ListEntSpat(PNode,
                                                     Parcel.EntitySpatial,
                                                     "SPElem.","Границы",6);
                    
                }

            if (Parcel.CompozitionEZ != null)
            {
                if (Parcel.CompozitionEZ.Count() > 0)
                {
                    TreeNode EntrysNode = PNode.Nodes.Add("EntrysNode" + Parcel.id.ToString(), "Входящие в ЕЗП (" + Parcel.CompozitionEZ.Count().ToString() + ")");
                    for (int i = 0; i <= Parcel.CompozitionEZ.Count - 1; i++)
                    {
                        // TreeNode EntrSNode = PNode.Nodes.Add("EntryNode." + Parcel.CompozitionEZ[i].Layer_id.ToString(), 
                        //                                                   Parcel.CompozitionEZ[i].Definition); 
                        netFteo.ObjectLister.ListEntSpat(EntrysNode,
                                                           Parcel.CompozitionEZ[i],
                                                           "SPElem.", Parcel.CompozitionEZ[i].Definition, Parcel.CompozitionEZ[i].State);
                    }
                }
                if (Parcel.CompozitionEZ.DeleteEntryParcels.Count() > 0)
                {
                    TreeNode EntrysNode = PNode.Nodes.Add("EntrysNode" + Parcel.id.ToString(), "Исключаемые из ЕЗП (" + Parcel.CompozitionEZ.DeleteEntryParcels.Count().ToString() + ")");
                    for (int i = 0; i <= Parcel.CompozitionEZ.DeleteEntryParcels.Count - 1; i++)
                    {
                        EntrysNode.Nodes.Add(Parcel.CompozitionEZ.DeleteEntryParcels[i]);
                    }
                }

                if (Parcel.CompozitionEZ.TransformationEntryParcel.Count() > 0)
                {
                    TreeNode EntrysNode = PNode.Nodes.Add("EntrysNode" + Parcel.id.ToString(), "Измененные в ЕЗП (" + Parcel.CompozitionEZ.TransformationEntryParcel.Count().ToString() + ")");
                    for (int i = 0; i <= Parcel.CompozitionEZ.TransformationEntryParcel.Count - 1; i++)
                    {
                        EntrysNode.Nodes.Add(Parcel.CompozitionEZ.TransformationEntryParcel[i]);
                    }
                }

            }

            if //(Parcel.Name == "Многоконтурный участок") && 
                (Parcel.Contours != null)
                if (Parcel.Contours.Count > 0 )
                {
                    TreeNode ContNode = PNode.Nodes.Add("Contours" + Parcel.Contours.id.ToString(), Parcel.Name);
                    ContNode.Text = "Контура [" + Parcel.Contours.Count.ToString() + "]";
                    for (int i = 0; i <= Parcel.Contours.Count - 1; i++)
                    {
                        netFteo.ObjectLister.ListEntSpat(ContNode,
                                                         Parcel.Contours[i],
                                                        "SPElem.", 
                                                        Parcel.Contours[i].Definition, Parcel.Contours[i].State);
                    }
                }
            if (Parcel.SubParcels != null)
            if (Parcel.SubParcels.Count > 0)
            {
                TreeNode Slotsnode = PNode.Nodes.Add("SlotsNode", "Части земельного участка");

                for (int i = 0; i <= Parcel.SubParcels.Count - 1; i++)
                {
                    TreeNode SlotNode = Slotsnode.Nodes.Add("SlotNode" + Parcel.SubParcels[i].id, "Часть "+Parcel.SubParcels[i].NumberRecord);
                        if (Parcel.SubParcels[i].Encumbrances != null)
                        {
                            ListEncums(SlotNode, Parcel.SubParcels[i].Encumbrances);
                        }
                    if (Parcel.SubParcels[i].EntSpat != null)
                        if (Parcel.SubParcels[i].EntSpat.PointCount > 0)
                        {
                            //TreeNode SlotESNode = SlotNode.Nodes.Add("SPElem." + Parcel.SubParcels[i].EntSpat.Layer_id.ToString(), "Границы");
                           // netFteo.ObjectLister.ListEntSpat(SlotESNode, Parcel.SubParcels[i].EntSpat);

                            netFteo.ObjectLister.ListEntSpat(SlotNode,
                                                             Parcel.SubParcels[i].EntSpat, 
                                                             "SPElem.", 
                                                             "Границы",6);
                        }

                    if (Parcel.SubParcels[i].Contours != null)
                        if (Parcel.SubParcels[i].Contours.Count > 0)
                        {
                            for (int ic = 0; ic <= Parcel.SubParcels[i].Contours.Count - 1; ic++)
                            
                            netFteo.ObjectLister.ListEntSpat(SlotNode,
                                                             Parcel.SubParcels[i].Contours[ic],
                                                             "SPElem.",
                                                             Parcel.SubParcels[i].Contours[ic].Definition, 6);
                        }
                }
            }
            // 
            if (Parcel.AllOffspringParcel != null)
            if (Parcel.AllOffspringParcel.Count > 0)
            {
                TreeNode OffSpringsNode = PNode.Nodes.Add("Все образованные земельные участки");
                for (int i = 0; i <= Parcel.AllOffspringParcel.Count - 1; i++)
                {

                    TreeNode OffSpringNode = OffSpringsNode.Nodes.Add("CN",
                                                                       Parcel.AllOffspringParcel[i]);
                }

            }
            if (Parcel.InnerCadastralNumbers != null)
            if (Parcel.InnerCadastralNumbers.Count > 0)
            {
                TreeNode InnerCNsNode = PNode.Nodes.Add("ОКС на земельном участке");
                for (int i = 0; i <= Parcel.InnerCadastralNumbers.Count - 1; i++)
                {
                    TreeNode InnerCNNode = InnerCNsNode.Nodes.Add(Parcel.InnerCadastralNumbers[i]);
                }

            }

            if (Parcel.PrevCadastralNumbers != null)
                if (Parcel.PrevCadastralNumbers.Count > 0)
                {
                    TreeNode InnerCNsNode = PNode.Nodes.Add("Предыдущие номера");
                    for (int i = 0; i <= Parcel.PrevCadastralNumbers.Count - 1; i++)
                    {
                        TreeNode InnerCNNode = InnerCNsNode.Nodes.Add("oNode"+i.ToString(), Parcel.PrevCadastralNumbers[i]);
                    }

                }
            ListRights(PNode, Parcel.Rights, Parcel.id, "Права", "Rights");
            ListRights(PNode, Parcel.EGRN, Parcel.id, "ЕГРН", "EGRNRight"); // и права из "приписочки /KPZU/ReestrExtract"
            ListEncums(PNode, Parcel.Encumbrances);
            return PNode;
        }


        private void ListAdress(TreeNode Node, netFteo.Rosreestr.TAddress Address, int id)
        {
            if (Address != null)
            {
                TreeNode Adrs = Node.Nodes.Add("Adrss" + id.ToString(), "Адрес");

               if (Address.Region != null)
                   if (dRegionsRF_v01 != null)
                    Adrs.Nodes.Add("Adr", "Регион").Nodes.Add(dRegionsRF_v01.Item2Annotation(Address.Region));

                if (Address.District != null)
                    Adrs.Nodes.Add("Adr", "Район").Nodes.Add(Address.District);
                if (Address.City != null)
                    Adrs.Nodes.Add("Adr", "Муниципальное образование").Nodes.Add(Address.City);

                if (Address.Locality != null)
                    Adrs.Nodes.Add("Adr", "Населённый пункт").Nodes.Add(Address.Locality);
                if (Address.Street != null)
                    Adrs.Nodes.Add("Adr", "Улица").Nodes.Add(Address.Street);
                if (Address.Level1 != null)
                    Adrs.Nodes.Add("Adr", "Дом").Nodes.Add(Address.Level1);
                if (Address.Level2 != null)
                    Adrs.Nodes.Add("Adr", "Стр.").Nodes.Add(Address.Level2);
                if (Address.Apartment != null)
                    Adrs.Nodes.Add("Adr", "Квартира").Nodes.Add(Address.Apartment);

                if (Address.Note != null)
                {
                    TreeNode an = Adrs.Nodes.Add("Adr", Address.Note.Replace("Российская федерация", "РФ.."));
                    an.ToolTipText = "Неформализованное описание";
                }

            }
        }
        
    private void ListMyOKS(TreeNode Node, TMyRealty oks)
        {
            TreeNode PNode = Node.Nodes.Add("PNode" + oks.id.ToString(), oks.CN);

            if (oks.Building != null ) 
            {
                PNode.ImageIndex = 2;
                PNode.SelectedImageIndex = 2;

                if (oks.Building.Flats.Count >0)
                {
                    TreeNode flatsnodes = PNode.Nodes.Add("Flats" + oks.id.ToString(), "Помещения (" + oks.Building.Flats.Count.ToString()+")");
                   foreach (TFlat s in oks.Building.Flats)
                       flatsnodes.Nodes.Add("FlatItem" + s.id.ToString(), s.CN);
                }

                if (oks.Building.OldNumbers.Count > 0)
                {
                    foreach (string s in oks.Building.OldNumbers)
                        PNode.Nodes.Add("OldNumsNode", "Ранее присвоенные номера").Nodes.Add("Number", s);
                }

                if (oks.Building.ES != null)
                {
                    string test = oks.Building.ES.GetType().Name;
                    if (oks.Building.ES.GetType().Name == "TMyPolygon")
                    {
                        if (((TMyPolygon)oks.Building.ES).PointCount > 0)
                            netFteo.ObjectLister.ListEntSpat(PNode, (TMyPolygon)oks.Building.ES, "SPElem.", "Границы", 6);
                    }

                    if (oks.Building.ES.GetType().Name == "TPolyLines")
                    {
                        netFteo.ObjectLister.ListEntSpat(PNode, (TPolyLines)oks.Building.ES, "SPElem.", "ПолиЛинии", 6);
                    }
                }
            }

            if (oks.Construction != null) //.Type == "Сооружение")
            {

                if (oks.Construction.ES != null)
                {


                    if (oks.Construction.ES.GetType().Name == "TMyPolygon")
                    {
                        if (((TMyPolygon)oks.Construction.ES).PointCount > 0)
                            netFteo.ObjectLister.ListEntSpat(PNode, (TMyPolygon)oks.Construction.ES, "SPElem.", "Границы", 6);
                    }

                    if (oks.Construction.ES.GetType().Name == "TPolyLines")
                    {
                        netFteo.ObjectLister.ListEntSpat(PNode, (TPolyLines)oks.Construction.ES, "SPElem.", "ПолиЛинии", 6);
                    }
                }
            }
                
            
            /*
            if (oks.Name != null)
            {
                TreeNode OksNameNode = PNode.Nodes.Add("OksNameNode","Наименование").Nodes.Add("Name", oks.Name);
            }
            */
            if (oks.Address != null)
            {
                ListAdress(PNode, oks.Address, oks.id);
            }
                //oks.KeyParameters
            if (oks.Notes != null)
                PNode.Nodes.Add("OksNotes", "Особые отметки").Nodes.Add("Note", oks.Notes);
            if (oks.ParentCadastralNumbers.Count > 0)
            {
                TreeNode flatsnodes = PNode.Nodes.Add("ParentCadastralNumbers" + oks.id.ToString(), "Земельные участки");
                foreach (string s in oks.ParentCadastralNumbers)
                    flatsnodes.Nodes.Add("CN" + s, s);
            }
            ListRights(PNode, oks.Rights, oks.id, "Права","Rights");
            ListRights(PNode, oks.EGRN, oks.id, "ЕГРН", "EGRNRight"); // и права из "приписочки /..../ReestrExtract"
        }

        private void ListBound(TreeNode Node, TBound Parcel)
        {
            string CN = Parcel.Description;
            TreeNode PNode = Node.Nodes.Add("BNDNode" + Parcel.id, Parcel.Description);
            TreeNode AddressNode = PNode.Nodes.Add("BNDTypeNode", Parcel.TypeName);
            if (Parcel.EntitySpatial != null)
                if (Parcel.EntitySpatial.Count > 0)
                {
                    //TreeNode ESNode = PNode.Nodes.Add("SPElem." + Parcel.EntitySpatial.Layer_id.ToString(), "Границы");
                   // netFteo.ObjectLister.ListEntSpat(ESNode, Parcel.EntitySpatial);
                    netFteo.ObjectLister.ListEntSpat(PNode, Parcel.EntitySpatial, "SPElem.", "Границы",6);
                }
        }
        private void ListZone(TreeNode Node, TZone Parcel)
        {
        
            netFteo.ObjectLister.ListZone(Node, Parcel);
        }


       
        // Листинг отрезков границ в ListView
        private void PointList_asBordersToListView(ListView LV, netFteo.Spatial.TMyPolygon PList)
        {
            if (PList.Count == 0) return;
            string BName;
            LV.Items.Clear();
            LV.Tag = PList.Layer_id;
            for (int i = 0; i <= PList.Count - 2; i++)
            {
                BName = PList[i].Pref + PList[i].NumGeopointA+" - "+
                        PList[i+1].Pref + PList[i+1].NumGeopointA;
                ListViewItem LVi = new ListViewItem();
                LVi.Text = BName;
               /* LVi.SubItems.Add(PList.Points[i].x_s);
                LVi.SubItems.Add(PList.Points[i].y_s);
                LVi.SubItems.Add(PList.Points[i].Mt_s);
                LVi.SubItems.Add(PList.Points[i].Description);
                * */
                if (PList[i].Pref == "н")
                    LVi.ForeColor = Color.Red;
                else LVi.ForeColor = Color.Black;
                LV.Items.Add(LVi);
            }
        }

        // Листинг точек в ListView
        private ListViewItem PointListToListView(ListView LV, netFteo.Spatial.TMyPolygon PList)
        {
            if (PList.Count == 0) return null;
            string BName;
            LV.Items.Clear();
            LV.Tag = PList.Layer_id;
            ListViewItem res = PointListToListView(LV, (PointList) PList);

            for (int ic = 0; ic <= PList.Childs.Count - 1; ic++)
            {  //Пустая строчка - разделитель
                ListViewItem LViEmpty_ch = new ListViewItem();
                LViEmpty_ch.Text = "";
                LV.Items.Add(LViEmpty_ch);
                PointListToListView(LV, PList.Childs[ic]);
            }
            ListViewItem LViEmpty = new ListViewItem();
            LViEmpty.Text = "";
            LV.Items.Add(LViEmpty);
            return res;
        }

        private ListViewItem PointListToListView(ListView LV, netFteo.Spatial.PointList PList)
        {
            if (PList.Count == 0) return null;
            string BName;
            LV.BeginUpdate();
            //LV.Items.Clear();
            //LV.Tag = PList.Parent_Id;
            ListViewItem res = null; ;
            for (int i = 0; i <= PList.Count - 1; i++)
            {
                BName = PList[i].Pref + PList[i].NumGeopointA + PList[i].OrdIdent;
                ListViewItem LVi = new ListViewItem();
                LVi.Text = BName;
                LVi.Tag = PList[i].id;
                LVi.SubItems.Add(PList[i].x_s);
                LVi.SubItems.Add(PList[i].y_s);
                LVi.SubItems.Add(PList[i].Mt_s);
                LVi.SubItems.Add(PList[i].Description);
                if (PList[i].Pref == "н")
                    LVi.ForeColor = Color.Red;
                else LVi.ForeColor = Color.Black;
                if (PList[i].Status == 6)
                    LVi.ForeColor = Color.Blue;
                if (i ==0) res = LV.Items.Add(LVi);
                else
                LV.Items.Add(LVi);
            }
            LV.EndUpdate();
            return res; // вернем первую строчку Items
        }

        //Листинг ПД 
        private void EsToListView(ListView LV, object ES, int parent_id)
        {
            if (ES == null)
            {
                if (ViewWindow != null) ViewWindow.Spatial = null; // сотрем картинку (последнюю)
                return;
            }
            ListViewItem LVi_Commands = null;
            LV.Columns[0].Text = "Имя";
            LV.Columns[1].Text = "x, м.";
            LV.Columns[2].Text = "y, м.";
            LV.Columns[3].Text = "Mt, м.";
            LV.Columns[4].Text = "-";
            LV.Columns[5].Text = "-";
            LV.Columns[6].Text = "-";
            LV.View = System.Windows.Forms.View.Details;
            if (ES.ToString() == "netFteo.Spatial.TPolygonCollection")
            {
                TPolygonCollection Contours = (TPolygonCollection)ES;
                if (Contours.Count == 0) return;
                string BName;
                for (int i = 0; i <= Contours.Count - 1; i++)
                {
                    ListViewItem LVi = new ListViewItem();
                    LVi.Text = Contours[i].Definition;
                    LVi_Commands = PointListToListView(LV, Contours[i]);
                    LV.Items.Add(LVi);
                }
            }

            if (ES.ToString() == "netFteo.Spatial.TMyPolygon")
            {
                TMyPolygon poly = (TMyPolygon)ES;
              LVi_Commands =  PointListToListView(LV, poly);

              if (toolStripMI_ShowES.Checked)
              {
                  if (poly.PointCount == 0)
                      ViewWindow.Spatial = null;
                  else
                      ViewWindow.Spatial = ES;
                  ViewWindow.label2.Content = poly.Definition;
                  ViewWindow.BringIntoView();
                  ViewWindow.CreateView(ES);
              }
            }
            
            //ListViewItem LVi_Commands = new ListViewItem();
            //LVi_Commands.Text = "**";
            //LV.Items.Add(LVi_Commands);
            ToolTip tt = new ToolTip();

           // if (parent_id > 0) // до момента наладки с parent_id будем проверять его наличие
             if (LV.Items.Count > 3)    // если что-то было отображено
            {
                LinkLabel pkk5Label = new LinkLabel();
                pkk5Label.Click += new System.EventHandler(OnPKK5LabelActionClick);
                pkk5Label.Tag = parent_id; //CN
                //pkk5Label.Text = "ПКК5 :)";
                pkk5Label.Image = XMLReaderCS.Properties.Resources.Rosreestr;
                pkk5Label.ImageAlign = ContentAlignment.MiddleLeft;
                pkk5Label.Cursor = Cursors.Hand;
                tt.SetToolTip(pkk5Label, "Найти на ПКК5 (Онлайн)");
                if (LVi_Commands == null) LVi_Commands = new ListViewItem("Полигон...не полигон?");
                pkk5Label.SetBounds(400, LVi_Commands.Position.Y, 32, 32);
                LV.Controls.Add(pkk5Label);

                Button SaveButt = new Button();
                //SaveButt.Text = "Сохранить";
                SaveButt.Image = XMLReaderCS.Properties.Resources.disk_multiple;
                SaveButt.Click += new System.EventHandler(OnPKK5ButtonActionClick);
                SaveButt.ImageAlign = ContentAlignment.MiddleCenter;
                SaveButt.FlatStyle = FlatStyle.Flat;
                SaveButt.SetBounds(435, LVi_Commands.Position.Y, 20, 20);
                tt.SetToolTip(SaveButt, "Сохранить как...");
                LV.Controls.Add(SaveButt);
            }
        }

      /// <summary>
      /// Вывод списка строк в ListView
      /// Вот порнография по  листингу строчки: split . Требует полной переработки!!! Позор 
      /// </summary>
      /// <param name="LV">Listview для отображения</param>
      /// <param name="list">список строк с разделителем "\t"</param>
        private void ListToListView(ListView LV, List<string> list)
        {
            if (list.Count == 0) return;
            LV.BeginUpdate();
            LV.View = System.Windows.Forms.View.Details;
            for (int i = 0; i <= list.Count - 1; i++)
            {
                if (list[i].Split('\t').Count() > 0)
                {
                    ListViewItem LVi = new ListViewItem();
                    LVi.Text = list[i].Split('\t')[0];

                    for (int ii = 1; ii <= list[i].Split('\t').Count() - 1; ii++)
                        LVi.SubItems.Add(list[i].Split('\t')[ii]);
                    LV.Items.Add(LVi);
                }
            }

            LV.EndUpdate();
        }

        private void EZPEntryListToListView(ListView LV, List<string> list)
        {
            if (list.Count == 0) return;

            LV.Columns[0].Text = "Номер";
            LV.Columns[1].Text = "Площадь граф.";
            LV.Columns[2].Text = "Площадь сем.";
            LV.Columns[3].Text = "Δ"; LV.Columns[3].TextAlign = HorizontalAlignment.Center;
            LV.Columns[4].Text = "ΔP";
            LV.Columns[5].Text = "изм.";
            LV.Columns[6].Text = "-";
            LV.Columns[7].Text = "-";
            ListToListView(LV, list);

        }

  

        private void RightsToListView(ListView LV, List<string> list)
        {
            if (list.Count == 0) return;
            LV.Columns[0].Text = "Субъект";
            LV.Columns[1].Text = "Право";
            LV.Columns[2].Text = "Рег. номер";
            LV.Columns[3].Text = "Доля в праве";
            LV.Columns[4].Text = "Особые отметки";
            LV.Columns[5].Text = "Обременения";
            ListToListView(LV, list);
        }


        /// <summary>
        /// Для установки IconSpacing нагуглил код через SendMessage (как в детстве):
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="wMsg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern Int32 SendMessage(IntPtr hwnd, Int32 wMsg, Int32 wParam, Int32 lParam);

        const int LVM_FIRST = 0x1000;
        const int LVM_SETICONSPACING = LVM_FIRST + 53;

        public void SetControlSpacing(Control control, Int16 x, Int16 y)
        {
            SendMessage(control.Handle, LVM_SETICONSPACING, 0, x * 65536 + y);
            control.Refresh();
        }


       

        private void ZoneRestrictsToListView(ListView LV, string Text)
        {
            LV.Columns[0].Text = "Ограничения";
            LV.Controls.Clear();
            CRichTextBox rt = new CRichTextBox(Text);
            rt.Dock = DockStyle.Fill;
            rt.ReadOnly = true;
            LV.Controls.Add(rt);
        }


        private void OMSPointsToListView(ListView LV, OMSPoints list)
        {
            if (list.PointCount == 0) return;
            LV.Columns[0].Text = "#";
            LV.Columns[1].Text = "Номер";
            LV.Columns[2].Text = "х, м.";
            LV.Columns[3].Text = "y, м.";
            LV.Columns[4].Text = "Тип";
            LV.Columns[5].Text = "Описание";


            List<string> lst = new List<string>();
             for (int i = 0; i <= list.PointCount - 1; i++)
            {
                 string flat_string = (i+1).ToString();
                 flat_string += "\t" + list[i].NumGeopointA + "\t" +
                                       list[i].x_s + "\t" +
                                       list[i].y_s + "\t" +
                                       list[i].Description + "\t"+
                                       list[i].Code;
                lst.Add(flat_string);
            }
            ListToListView(LV, lst);
        }

        private void FlatsToListView(ListView LV, TFlats list)
        {
            if (list.Count == 0) return;
            LV.Columns[0].Text = "#";
            LV.Columns[1].Text = "Назначение,вид";
            LV.Columns[2].Text = "Этаж";
            LV.Columns[3].Text = "Номер этажа";
            LV.Columns[4].Text = "КН / Обозначение на плане";
            LV.Columns[5].Text = "Площадь";
            LV.Columns[6].Text = "Адрес";
            LV.Columns[7].Text = "Адрес нестр.";
            LV.BeginUpdate();
            LV.View = System.Windows.Forms.View.Details;

            List<string> lst = new List<string>();
            int cnt = 1;
            foreach (TFlat fl in list)
            {
                string flat_string = "" ;  
                if (fl.AssignationType == "")
                    flat_string += fl.AssignationCode;
                else
                    flat_string += fl.AssignationCode + "/" + fl.AssignationType;

                ListViewItem LVi = new ListViewItem();
                LVi.Text = cnt++.ToString();
                LVi.SubItems.Add(flat_string);

                if (fl.PositionInObject.Levels.Count == 1)
                {
                    LVi.SubItems.Add(fl.PositionInObject.Levels[0].Type);
                    LVi.SubItems.Add(fl.PositionInObject.Levels[0].Number);
                    LVi.SubItems.Add(fl.CN + "/  "+ fl.PositionInObject.Levels[0].Position.NumberOnPlan);
                }
                else
                {
                    LVi.BackColor = System.Drawing.Color.LightGray;
                    LVi.SubItems.Add("");
                    LVi.SubItems.Add("");
                    LVi.SubItems.Add(fl.CN);
                }
                LVi.SubItems.Add(fl.Area.ToString());
                LVi.SubItems.Add(fl.Address.AsString());//Adress
                if ( fl.Address.Other != null)
                LVi.SubItems.Add(fl.Address.Other);
                if ( fl.Address.Note != null )
                    LVi.SubItems.Add(fl.Address.Note); 
                LV.Items.Add(LVi);

                if (fl.PositionInObject.Levels.Count > 1)
                    foreach (TLevel lv in fl.PositionInObject.Levels)
                {
                    ListViewItem LVip = new ListViewItem();
                    LVip.Text = ""; //пропустим
                    LVip.SubItems.Add(flat_string);
                    LVip.SubItems.Add(lv.Type); //Тип этажа
                    LVip.SubItems.Add(lv.Number); // Номер этажа
                    LVip.SubItems.Add(lv.Position.NumberOnPlan);
                    LV.Items.Add(LVip);
                }
                

            }

            LV.EndUpdate();
        }





        private void FlatToListView(ListView LV, TFlat flat)
        {
            if (flat == null) return;
            LV.BeginUpdate();
            LV.Items.Clear();
            LV.View = System.Windows.Forms.View.LargeIcon;
            ImageList jpegs = new ImageList()
            {
                ImageSize = new Size(256, 182) // 1.4
            };
            LV.LargeImageList = jpegs;
            int imgindex = 0;
            foreach (TLevel lev in flat.PositionInObject.Levels)
            {
                foreach (string jpegname in lev.Position.Plans_Plan_JPEG)
                {
                    ListViewItem LVi = new ListViewItem()
                    {
                        Text = jpegname,
                        ImageIndex = imgindex++
                    };
                    LV.Items.Add(LVi);
                }
            }
            LV.EndUpdate();

   //populate items with pictures
            foreach (TLevel lev in flat.PositionInObject.Levels)
            {
                foreach (string jpegname in lev.Position.Plans_Plan_JPEG)
                {
                    string testPath = Folder_Unzip + "\\" + jpegname;
                    System.Drawing.Image FileJpeg = null;
                    System.Drawing.Image FileJpegSmall = null;
                    /*
                    if (File.Exists(testPath))
                        FileJpeg = new Bitmap(testPath); //in deflated zip folder
                    */
                    string testPath2 = Path.GetDirectoryName(this.DocInfo.FilePath) + "\\" + jpegname;
                    if (File.Exists(testPath2))
                    {
                        FileJpeg = new Bitmap(testPath2); // in folder
                        netFteo.Drawing.FteoJpeg fj = new netFteo.Drawing.FteoJpeg();
                        FileJpegSmall = fj.resizeImage(FileJpeg, 256, 182); //1.4
                        FileJpeg.Dispose(); // free memory
                        FileJpeg = null;    // this not freed, just point to null
                        if (FileJpegSmall != null)
                            jpegs.Images.Add(FileJpegSmall);
                    }
                }
            }
   
            //jpegs.Dispose(); // memory not freed
        }

        private void AdressToListView(ListView LV, netFteo.Rosreestr.TAddress Address)
        {
            if (Address != null)
            {
                if (Address.Note != null)
                {
                    ListViewItem LViAssgn = new ListViewItem();
                    LViAssgn.Text = "Адрес (местоположение)";
                    LViAssgn.SubItems.Add(Address.Note.Replace("Российская федерация", "РФ.."));
                    LV.Items.Add(LViAssgn);
                }
                //Есть структ. адресс ?:
               // if (Address.Locality != null)
                //    if (Address.Locality.Length > 2)
                  //  {
                        ListViewItem LViAssgn2 = new ListViewItem();
                        LViAssgn2.Text = "Адрес (структ.)";
                        LViAssgn2.SubItems.Add(this.dRegionsRF_v01.Item2Annotation(Address.Region)+" "+ Address.AsString());
                        LV.Items.Add(LViAssgn2);
                  //  }
            }
        }
        private void KeyParametersToListView(ListView LV, netFteo.Spatial.TKeyParameters ps)
        {
            if (ps == null) return;
            foreach (netFteo.Spatial.TKeyParameter param in ps)
            {
                ListViewItem LViAssgn = new ListViewItem();
                LViAssgn.Text = param.Type;
                LViAssgn.SubItems.Add(param.Value);
                LV.Items.Add(LViAssgn);
            }
        }
        
      private void PropertiesToListView(ListView LV, object Obj)
        {
            
            if (Obj == null)
                LV.Items.Clear();
            else
            {
                    LV.Controls.Clear();
                    LV.BeginUpdate();
                if (Obj.ToString() == "netFteo.Spatial.TMyParcel")
                {
                    TMyParcel P = (TMyParcel)Obj;
                    LV.Items.Clear();
                    LV.Controls.Clear();

                    //Отрисуем и отлистаем ПД (not for contours):
                    EsToListView(listView1, P.EntitySpatial, P.id);

                    ListViewItem LViCN = new ListViewItem();
                    if (P.CN != null)
                    {
                        LViCN.Text = "Кадастровый номер";
                        LViCN.SubItems.Add(P.CN);
                        LViCN.SubItems.Add(P.DateCreated);
                    }
                    if (P.Definition != null)
                    {
                        LViCN.Text = "Обозначение";
                        LViCN.SubItems.Add(P.Definition);
                    }
                    LV.Items.Add(LViCN);

                    AdressToListView(LV, P.Location.Address);
                    ListViewItem LVNa = new ListViewItem();
                    LVNa.Text = "Тип";
                    LVNa.SubItems.Add(P.Name);
                    LV.Items.Add(LVNa);

                    if (P.ParentCN != null)
                    {
                        ListViewItem LVCN = new ListViewItem();
                        LVCN.Text = "Входит в состав";
                        LVCN.SubItems.Add(P.ParentCN);
                        LV.Items.Add(LVCN);

                    }

                    if (P.Area_float > 0)
                    {
                        ListViewItem LVipg = new ListViewItem();
                        LVipg.Text = "Площадь (коорд.)";
                        LVipg.SubItems.Add(P.Area("#0.00"));
                        LVipg.SubItems.Add("кв.м.");
                        ListViewItem addedItem = LV.Items.Add(LVipg);
                    }
                    else LV.Controls.Clear();

                    if (P.AreaValue != null)
                    {
                        ListViewItem LVipgv = new ListViewItem();
                        LVipgv.Text = "Площадь";
                        LVipgv.SubItems.Add(P.AreaValue);
                        LVipgv.SubItems.Add("кв.м.");
                        LV.Items.Add(LVipgv);

                    }

                    //отобразим отклонение в площади выичсленной и указанной
                    try
                    {
                        ListViewItem LVip3d = new ListViewItem();
                        double area = Convert.ToDouble(P.AreaValue);
                        LVip3d.Text = "Δ";
                        LVip3d.SubItems.Add((P.Area_float - area).ToString("0.00"));
                        LVip3d.SubItems.Add("кв.м");
                        LV.Items.Add(LVip3d);
                    }
                    catch
                    {

                    }



                    ListViewItem LVip = new ListViewItem();
                    LVip.Text = "Площадь ГКН";
                    LVip.SubItems.Add(P.AreaGKN);
                    LVip.SubItems.Add("кв.м.");
                    LV.Items.Add(LVip);

                    ListViewItem LViCat = new ListViewItem();
                    LViCat.Text = "Категория";
                    LViCat.SubItems.Add(P.Category);
                    LV.Items.Add(LViCat);

                    ListViewItem LViPurpDoc = new ListViewItem();
                    LViPurpDoc.Text = "Разр. использование (док)";
                    LViPurpDoc.SubItems.Add(P.Utilization.UtilbyDoc);
                    LV.Items.Add(LViPurpDoc);

                    if (P.Utilization.UtilizationSpecified)
                    {
                        ListViewItem LViPurp = new ListViewItem();
                        LViPurp.Text = "Разр. использование (кл)";
                        LViPurp.SubItems.Add(this.dutilizations_v01.Item2Annotation(P.Utilization.Untilization));
                        LV.Items.Add(LViPurp);
                    }
                }


                if (Obj.ToString() == "netFteo.Spatial.TBuilding")
                {
                    netFteo.Spatial.TBuilding bld = (netFteo.Spatial.TBuilding) Obj;
                    if (bld.Flats != null)
                    {
                        if (bld.Flats.Count > 0)
                        {
                            ListViewItem LViPurp = new ListViewItem();
                            LViPurp.Text = "Помещения";
                            LViPurp.SubItems.Add(bld.Flats.Count.ToString());
                            LV.Items.Add(LViPurp);
                        }
                        if (bld.Flats.TotalArea > 0)
                        {
                            ListViewItem LViFlats = new ListViewItem();
                            LViFlats.Text = "Площадь помещений";
                            LViFlats.SubItems.Add(bld.Flats.TotalArea.ToString());
                            LViFlats.SubItems.Add(" кв.м.");
                            LV.Items.Add(LViFlats);
                        }

                        if (bld.Flats.AreabyCode("Жилое помещение") > 0)
                        {
                            ListViewItem LViFlatsLive = new ListViewItem();
                            LViFlatsLive.Text = "Жилых помещений " + bld.Flats.CountbyCode("Жилое помещение").ToString();
                            LViFlatsLive.SubItems.Add(bld.Flats.AreabyCode("Жилое помещение").ToString());
                            LViFlatsLive.SubItems.Add(" кв.м.");
                            LV.Items.Add(LViFlatsLive);
                        }

                        if (bld.Flats.AreabyCode("Нежилое помещение") > 0)
                        {
                            ListViewItem LViFlatsLive = new ListViewItem();
                            LViFlatsLive.Text = "Нежилых помещений " + bld.Flats.CountbyCode("Нежилое помещение").ToString();
                            LViFlatsLive.SubItems.Add(bld.Flats.AreabyCode("Нежилое помещение").ToString());
                            LViFlatsLive.SubItems.Add(" кв.м.");
                            LV.Items.Add(LViFlatsLive);
                        }
                    }


                }

                if (Obj.ToString() == "netFteo.Spatial.TMyRealty")
                {
                    TMyRealty P = (TMyRealty)Obj;
                    LV.Items.Clear();
                  
                    ListViewItem LViCN = new ListViewItem();
                    LViCN.Text = "Кадастровый номер";
                    LViCN.SubItems.Add(P.CN);
                    LViCN.SubItems.Add(P.DateCreated);
                    LV.Items.Add(LViCN);

                    /*
                    if (P.Address != null)
                    {
                        if (P.Address.Note != null)
                        {
                            ListViewItem LViAssgn = new ListViewItem();
                            LViAssgn.Text = "Адрес (местоположение)";
                            LViAssgn.SubItems.Add(P.Address.Note.Replace("Российская федерация", "РФ.."));
                            LV.Items.Add(LViAssgn);
                        }
                        //Есть структ. адресс ?:
                        if (P.Address.Locality != null)
                            if (P.Address.Locality.Length > 2)
                            {
                                ListViewItem LViAssgn2 = new ListViewItem();
                                LViAssgn2.Text = "Адрес (структ.)";
                                LViAssgn2.SubItems.Add(P.Address.District + " " + P.Address.Locality + " " +
                                                       P.Address.Street + " " + P.Address.Level1);
                                LV.Items.Add(LViAssgn2);
                            }
                    }
                    */
                    AdressToListView(LV, P.Address);

                    if (P.Name != null)
                    {
                        ListViewItem LViAssgn = new ListViewItem();
                        LViAssgn.Text = "Наименование";
                        LViAssgn.SubItems.Add(P.Name);
                        LV.Items.Add(LViAssgn);
                    }

                    ListViewItem LViType = new ListViewItem();
                    LViType.Text = "Вид объекта недвижимости";
                    LViType.SubItems.Add(P.ObjectType);
                    LV.Items.Add(LViType);


                    if (P.Building != null)
                    {
                        ListViewItem LViAssgn = new ListViewItem();
                        LViAssgn.Text = "Назначение";
                        LViAssgn.SubItems.Add(P.Building.AssignationBuilding);
                        LV.Items.Add(LViAssgn);
                        EsToListView(listView1, P.Building.ES, P.id);

                        //TFlats:
                        if (P.Building.Flats != null)
                            if (P.Building.Flats.Count >0)
                        {
                            //LV.Items.Clear();
                            ListViewItem LViFlats = new ListViewItem();
                            LViFlats.Text = "Помещения (" + P.Building.Flats.Count.ToString()+ ")";
                            if (P.Building.Flats.Area > 0)
                            {
                                LViFlats.SubItems.Add(P.Building.Flats.Area.ToString());
                                LViFlats.SubItems.Add(" кв.м.");
                            }
                            LV.Items.Add(LViFlats);
                        }

                        if (P.Building.Area != 0)
                        {
                            ListViewItem LViAr = new ListViewItem();
                            LViAr.Text = "Площадь";
                            LViAr.SubItems.Add(P.Building.Area.ToString());
                            LViAr.SubItems.Add(" кв.м.");
                            LV.Items.Add(LViAr);
                        }
                    }
                    
                    //{netFteo.Spatial.TFlat}
                    if (P.Flat != null)
                    {
                        TFlats flats = new TFlats();
                        flats.AddFlat(P.Flat);
                        FlatsToListView(listView1, flats);
                        /*
                        if (P.Flat.PositionInObject != null)
                        {

                            ListViewItem LViAssgn = new ListViewItem();
                            LViAssgn.Text = "Расположение в пределах объекта недвижимости";
                            string numOnString = "";
                            foreach (TLevel level in P.Flat.PositionInObject.Levels)
                            {
                                numOnString += level.Position.NumberOnPlan;
                            }
                            LViAssgn.SubItems.Add(numOnString);

                            LV.Items.Add(LViAssgn);
                        }
                        */
                       // AdressToListView(LV, P.Address);
                    }



                    if (P.Construction != null)
                    {
                        ListViewItem LViAssgn = new ListViewItem();
                        LViAssgn.Text = "Назначение";
                        LViAssgn.SubItems.Add(P.Construction.AssignationName);
                        LV.Items.Add(LViAssgn);
                        EsToListView(listView1, P.Construction.ES, P.id);
                    }

                    if (P.Uncompleted != null)
                    {
                        ListViewItem LViAssgn = new ListViewItem();
                        LViAssgn.Text = "Проектируемое назначение";
                        LViAssgn.SubItems.Add(P.Uncompleted.AssignationName);
                        LV.Items.Add(LViAssgn);
                        ListViewItem LViReady = new ListViewItem();
                        LViReady.Text = "Степень готовности";
                        LViReady.SubItems.Add(P.Uncompleted.DegreeReadiness + "%");
                        LV.Items.Add(LViReady);
                        EsToListView(listView1, P.Uncompleted.ES, P.id);
                    }

                    if (P.Area != 0)
                    {
                        ListViewItem LViAr = new ListViewItem();
                        LViAr.Text = "Площадь";
                        LViAr.SubItems.Add(P.Area.ToString());
                        LV.Items.Add(LViAr);
                    }

                    KeyParametersToListView(LV, P.KeyParameters);
                }

   
                //  Если это часть: 
                if (Obj.ToString() == "netFteo.Spatial.TmySlot")
                {
                    TmySlot P = (TmySlot)Obj;
                    LV.Items.Clear();
                    ListViewItem LViCN = new ListViewItem();
                    LViCN.Text = "Учетный номер части";
                    LViCN.SubItems.Add(P.NumberRecord);
                    LV.Items.Add(LViCN);

                    ListViewItem LVipg = new ListViewItem();
                    LVipg.Text = "Площадь (коорд.)";
                    LVipg.SubItems.Add(P.EntSpat.AreaSpatialFmt("#0.00"));
                    LVipg.SubItems.Add("кв.м.");
                    LV.Items.Add(LVipg);

                    ListViewItem LVip = new ListViewItem();
                    LVip.Text = "Площадь ГКН";
                    LVip.SubItems.Add(P.AreaGKN);
                    LVip.SubItems.Add("кв.м.");
                    LV.Items.Add(LVip);

                    if (P.Encumbrances.Count == 1)
                    {
                        ListViewItem LViCat = new ListViewItem();
                        LViCat.Text = "Характеристика";
                        LViCat.SubItems.Add(P.Encumbrances[0].Name);
                        LV.Items.Add(LViCat);

                        ListViewItem LViCatE = new ListViewItem();
                        LViCatE.Text = "Тип";
                        LViCatE.SubItems.Add(P.Encumbrances[0].Type);
                        LV.Items.Add(LViCatE);

                        ListViewItem LViDoc = new ListViewItem();
                        LViDoc.Text = "Документ";
                        LViDoc.SubItems.Add(P.Encumbrances[0].Document.DocName);
                        LViDoc.SubItems.Add(P.Encumbrances[0].Document.Number);
                        LViDoc.SubItems.Add(P.Encumbrances[0].Document.Date);
                        LV.Items.Add(LViDoc);
                    }
                }

                if (Obj.ToString() == "netFteo.Spatial.TMyPolygon")
                {
                    TMyPolygon Poly = (TMyPolygon)Obj;
                    LV.Items.Clear();
                    ListViewItem LVip = new ListViewItem();
                    LVip.Text = "Площадь [1.." + Poly.PointCount.ToString() + "]";
                    LVip.SubItems.Add(Poly.AreaSpatialFmt("#,0.00"));
                    LVip.SubItems.Add("кв.м.");
                    LV.Items.Add(LVip);

                    ListViewItem LVipG = new ListViewItem();
                    LVipG.Text = "Площадь ГКН";
                    LVipG.SubItems.Add(Poly.AreaValue.ToString());
                    LVipG.SubItems.Add("кв.м.");
                    LV.Items.Add(LVipG);

                    ListViewItem LVipP = new ListViewItem();
                    LVipP.Text = "Периметр";
                    LVipP.SubItems.Add(Poly.PerymethrFmt("#,0.00"));
                    LVipP.SubItems.Add("м.");
                    LV.Items.Add(LVipP);

                    if (Poly.Childs.Count > 0)
                    {
                        ListViewItem LVipIB = new ListViewItem();
                        LVipIB.Text = "Внутренние границы";
                        LVipIB.SubItems.Add(Poly.Childs.Count.ToString());
                        LV.Items.Add(LVipIB);
                    }

                    // list borders:
                    netFteo.ObjectLister.EStoListViewCollection(LV, Poly);
                }

                if (Obj.ToString() == "netFteo.Rosreestr.TMyRights")
                {
                    LV.Items.Clear();
                    netFteo.Rosreestr.TMyRights R = (netFteo.Rosreestr.TMyRights)Obj;
                    if (R.Count > 0)
                    {
                        ListViewItem LVip = new ListViewItem();
                        LVip.Text = "Права";
                        LVip.SubItems.Add(R.Count.ToString());
                        LV.Items.Add(LVip);
                    }
                }

                //string tst = Obj.GetType().ToString();
                if (Obj.ToString() == "netFteo.Spatial.TPolygonCollection")

                {
                    LV.Items.Clear();
                    netFteo.Spatial.TPolygonCollection cEZ = (netFteo.Spatial.TPolygonCollection)Obj;
                    if (cEZ.Count > 0)
                    {
                        ListViewItem LVip = new ListViewItem();
                        LVip.Text = "Состав";
                        LVip.SubItems.Add( cEZ.Count.ToString());
                        LV.Items.Add(LVip);

                        ListViewItem LVip2 = new ListViewItem();
                        LVip2.Text = "Площадь";
                        LVip2.SubItems.Add(cEZ.AreaSpatialFmt("0.00", true));
                        LV.Items.Add(LVip2);

                        ListViewItem LVip3 = new ListViewItem();
                        LVip3.Text = "Площадь сем.";
                        LVip3.SubItems.Add(cEZ.AreaSpecifiedFmt("0.00", true));
                        LV.Items.Add(LVip3);


                    }
                }

                
                if (Obj.ToString() == "netFteo.Spatial.TCompozitionEZ")

                {
                    LV.Items.Clear();
                    netFteo.Spatial.TCompozitionEZ cEZ = (netFteo.Spatial.TCompozitionEZ)Obj;
                    if (cEZ.Count > 0)
                    {
                        ListViewItem LVip = new ListViewItem();
                        LVip.Text = "Состав";
                        LVip.SubItems.Add(cEZ.Count.ToString());
                        LV.Items.Add(LVip);

                        ListViewItem LVip2 = new ListViewItem();
                        LVip2.Text = "Площадь";
                        LVip2.SubItems.Add(cEZ.AreaSpatialFmt("0.00", true));
                        LV.Items.Add(LVip2);

                        ListViewItem LVip3 = new ListViewItem();
                        LVip3.Text = "Площадь сем.";
                        LVip3.SubItems.Add(cEZ.AreaSpecifiedFmt ("0.00", true));
                        LV.Items.Add(LVip3);

                        ListViewItem LVip3d = new ListViewItem();
                        LVip3d.Text = "Δ";
                        LVip3d.SubItems.Add(cEZ.AreaVariance.ToString("0.00"));
                        LVip3d.SubItems.Add("кв.м");
                        LV.Items.Add(LVip3d);

                        ListViewItem LVipP = new ListViewItem();
                        LVipP.Text = "Периметр";
                        LVipP.SubItems.Add(cEZ.TotalPerimeter.ToString("0.00"));
                        LVipP.SubItems.Add("м.");
                        LV.Items.Add(LVipP);

                    }
                }

                if (Obj.ToString() == "netFteo.Spatial.TZone")
                {
                    LV.Items.Clear();
                    netFteo.Spatial.TZone Zn = (netFteo.Spatial.TZone)Obj;
                    ListViewItem LVip = new ListViewItem();
                    LVip.Text = "Идентификационный реестровый номер";
                    LVip.SubItems.Add(Zn.AccountNumber);
                    LV.Items.Add(LVip);

                    ListViewItem LVip3 = new ListViewItem();
                    LVip3.Text = "Тип";
                    LVip3.SubItems.Add(Zn.TypeName);
                    LV.Items.Add(LVip3);

                    ListViewItem LVip2 = new ListViewItem();
                    LVip2.Text = "Описание";
                    LVip2.SubItems.Add(Zn.Description);
                    LV.Items.Add(LVip2);
                }
                LV.EndUpdate();
            }

        }

        //--------------Листинг ОМС --------------
        private void ListPointList(TreeNode Node, netFteo.Spatial.PointList PList, int InternalNumber)
        {
            if (PList.Count == 0) return;
            string BName;
            for (int i = 0; i <= PList.Count - 1; i++)
            {
                if (InternalNumber == 0)
                {
                    BName = PList[i].NumGeopointA;
                }
                else BName = PList[i].NumGeopointA + "." + Convert.ToString(InternalNumber);
                TreeNode PNode = Node.Nodes.Add("PointNode", BName);
                PNode.ToolTipText = "Номер пункта опорной межевой сети на плане";
                PNode.Nodes.Add("Ordinate", PList[i].x_s);
                PNode.Nodes.Add("Ordinate", PList[i].y_s);
                if (PList[i].Mt_s != null)
                    if (PList[i].Mt != 0) PNode.Nodes.Add("Ordinate", "Mt = " + PList[i].Mt_s);
                if (PList[i].Description != null) PNode.Nodes.Add("Ordinate", PList[i].Description);
                if (PList[i].Code != null) { PNode.Nodes.Add("Code", PList[i].Code).ToolTipText = "Номер, тип пункта опорной межевой сети"; }
            }

        }


        //--------------Проверим ноду--------------
        private void ListSelectedNode(TreeNode STrN)
        {
            if (STrN == null) return;
            toolStripStatusLabel2.Text = STrN.Name;
            listView1.Items.Clear();
            listView1.Controls.Clear();
            listView_Properties.Items.Clear();
            listView_Properties.Controls.Clear();

            if (STrN.Name.Contains("SPElem."))
            {
                int chek_id = Convert.ToInt32(STrN.Name.Substring(7));
                /*
                object parent = this.DocInfo.MyBlocks.GetObject(Convert.ToInt32(STrN.Name.Substring(7)));
                if (parent != null)
                {
                    object Mparent = this.DocInfo.MyBlocks.GetObject(((TMyPolygon)parent).Parent_Id);
                }
                */
                TMyPolygon Pl = (TMyPolygon)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(STrN.Name.Substring(7)));
                
                if (Pl != null)
                {
                    if (Pl.Parent_Id > 0)
                        EsToListView(listView1, Pl, Pl.Parent_Id);
                    else
                        EsToListView(listView1, Pl, (int)STrN.Tag);
                } 
                PropertiesToListView(listView_Properties, Pl);
            }

            //Стереть предыдыущее изображение
            /*
                   if (toolStripMI_ShowES.Checked)
                   {
                       ViewWindow.Spatial = Pl;
                       ViewWindow.label2.Content = Pl.Definition;
                       ViewWindow.BringIntoView();
                       ViewWindow.CreateView(Pl);

                   }
                   */

            if (STrN.Name.Contains("TPolyLine."))
            {
                int chek_id = Convert.ToInt32(STrN.Name.Substring(10));
                Object Entity = this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(STrN.Name.Substring(10)));
                if (Entity != null)
                {
                    PointListToListView(listView1, (PointList) Entity);
                }
              }

            if (STrN.Name.Contains("PNode"))
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(5));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                PropertiesToListView(listView_Properties, O);
                
                if (O.GetType().ToString().Equals("netFteo.Spatial.TMyParcel"))
                    {
                    TMyParcel parcel = (TMyParcel)O;
                    if (parcel.Contours!= null)
                    EZPEntryListToListView(listView1, parcel.Contours.AsList());
                }
            }

            if (STrN.Name.Contains("ZNode"))
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(5));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                ZoneRestrictsToListView(listView1, ((TZone)O).ContentRestrictions);
                PropertiesToListView(listView_Properties, O);
            }


            if (STrN.Name.Contains("SlotNode"))
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(8));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                PropertiesToListView(listView_Properties, O);
            }


            if (STrN.Name.Contains("Rights"))
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(6));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                if (O.ToString() == "netFteo.Spatial.TMyParcel")
                {
                    TMyParcel P = (TMyParcel)O;
                    PropertiesToListView(listView_Properties, P.Rights);
                    // PropertiesToListView(listView_Properties, P.EGRN);
                    if (P.Rights != null) RightsToListView(listView1, P.Rights.AsList());
                    //  if (P.EGRN != null)    RightsToListView(listView1, P.EGRN.AsList());
                }
            }

           if (STrN.Name.Contains("EGRNRight"))
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(9));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                if (O.ToString() == "netFteo.Spatial.TMyParcel")
                {
                    TMyParcel P = (TMyParcel)O;
                    //PropertiesToListView(listView_Properties, P.Rights);
                    PropertiesToListView(listView_Properties, P.EGRN);
                    //if (P.Rights != null)  RightsToListView(listView1, P.Rights.AsList());
                    if (P.EGRN != null)    RightsToListView(listView1, P.EGRN.AsList());
                }

            }

            
            if (STrN.Name.Contains("Flats"))
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(5));
                object O = this.DocInfo.MyBlocks.GetObject(id);

                if (O.ToString() == "netFteo.Spatial.TMyRealty")
                {
                    TMyRealty P = (TMyRealty)O;
                    if (P.Building != null )
                        if (P.Building.Flats.Count >0)
                    {
                        FlatsToListView(listView1, P.Building.Flats);
                        PropertiesToListView(listView_Properties, P.Building);

                    }
                }
            }

            if (STrN.Name.Contains("FlatItem"))
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(8));
                object O = this.DocInfo.MyBlocks.GetObject(id);

                if (O.ToString() == "netFteo.Spatial.TFlat")
                {
                    TFlat P = (TFlat)O;
                    if (P != null)
                        {
                            PropertiesToListView(listView_Properties, P);
                            FlatToListView(listView1, P);
                        }
                }
            }


            if (STrN.Name.Contains("EntrysNode"))
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(10));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                if (O.ToString() == "netFteo.Spatial.TMyParcel")
                {
                    TMyParcel P = (TMyParcel)O;
                    EZPEntryListToListView(listView1, P.CompozitionEZ.AsList());
                    PropertiesToListView(listView_Properties, P.CompozitionEZ);
                }
            }

            if (STrN.Name.Contains("Contours"))
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(8));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                if (O != null )
                    if (O.ToString() == "netFteo.Spatial.TPolygonCollection")
                    {
                        netFteo.Spatial.TPolygonCollection P = (netFteo.Spatial.TPolygonCollection)O;
                        EZPEntryListToListView(listView1, P.AsList());
                        PropertiesToListView(listView_Properties, P);
                    }
                
            }

            if (STrN.Name.Contains("OMSPoints"))
            {
                OMSPointsToListView(listView1, this.DocInfo.MyBlocks.OMSPoints);
            }
        }


        private void ListRights(TreeNode PNode, netFteo.Rosreestr.TMyRights Rights, int ownerid, string Name, string Nodename)
        {
            if (Rights == null) return;
            if (Rights.Count > 0)
            {
                TreeNode Rnode = PNode.Nodes.Add(Nodename + ownerid.ToString(), Name);

                for (int i = 0; i <= Rights.Count - 1; i++)
                {
                    TreeNode RNameNode = Rnode.Nodes.Add("RightItemNode", Rights[i].Name);
                    RNameNode.Nodes.Add(Rights[i].Type);
                    RNameNode.Nodes.Add(Rights[i].RegNumber + " " + Rights[i].RegDate);
                    if (Rights[i].Owners.Count > 0)
                    {
                        TreeNode ROwnersNode = RNameNode.Nodes.Add("Правообладатели");
                        ROwnersNode.SelectedImageIndex = 9;
                        ROwnersNode.ImageIndex = 9;
                    for (int io = 0; io <= Rights[i].Owners.Count - 1; io++)
                    {
                        TreeNode ROwnerNode = ROwnersNode.Nodes.Add(Rights[i].Owners[io].OwnerName);
                        if (Rights[i].Owners[io].ContactOwner != null)
                        {
                            TreeNode ROwnerContactsNode = ROwnerNode.Nodes.Add(Rights[i].Owners[io].ContactOwner);
                            ROwnerContactsNode.ToolTipText = "Связь с правообладателем";
                            ROwnerContactsNode.ForeColor = Color.Red;
                            ROwnerContactsNode.SelectedImageIndex = 8;
                            ROwnerContactsNode.ImageIndex = 8;
                        }
                    }
                    }

                    if (Rights[i].ShareText != null)
                        RNameNode.Nodes.Add(Rights[i].ShareText).ToolTipText = "Доля в праве (текстом)";

                    foreach (netFteo.Rosreestr.TMyEncumbrance enc in Rights[i].Encumbrances)
                    {
                        ListEncum(RNameNode, enc);
                    }

                }
            }
        }
        private void ListEncum(TreeNode Rnode, netFteo.Rosreestr.TMyEncumbrance Ens)
        {
            if (Ens == null) return;
            string EncType = Ens.Type;
            if (Ens.Type == null) EncType = "Обременение";
            TreeNode RNameNode = Rnode.Nodes.Add("RencNode", Ens.Type);
            if (Ens.Name != null)
                RNameNode.Nodes.Add(Ens.Name);
            if (Ens.Document.DocName != null)
            {
                TreeNode RDocNode = RNameNode.Nodes.Add("Документ: " + Ens.Document.DocName);
                     RDocNode.Nodes.Add(Ens.Document.Date);
                     RDocNode.Nodes.Add(Ens.Document.Number);
            }

            if (Ens.AccountNumber != null)
            RNameNode.Nodes.Add("Учетный номер " + Ens.AccountNumber);
            if (Ens.RegNumber != null)
            {
                TreeNode RNameRegNode = RNameNode.Nodes.Add("Государственная регистрация");
                RNameRegNode.Nodes.Add(Ens.RegNumber);
                RNameRegNode.Nodes.Add(Ens.RegDate);
            }

            if (Ens.Owners.Count > 0)
            {
                TreeNode RNameOwnNode = RNameNode.Nodes.Add("В пользу");
                for (int io = 0; io <= Ens.Owners.Count - 1; io++)
                    RNameOwnNode.Nodes.Add(Ens.Owners[io].OwnerName);
            }
            if (Ens.DurationStarted != null)
            {
                TreeNode RDurStrN = RNameNode.Nodes.Add("Дата возникновения");
                RDurStrN.Nodes.Add(Ens.DurationStarted);
            }
            if (Ens.DurationStopped != null)
            {
                TreeNode RDurStopN = RNameNode.Nodes.Add("Дата прекращения");
                RDurStopN.Nodes.Add(Ens.DurationStopped);
            }
            if (Ens.DurationTerm != null)
            {
                TreeNode RDurTerm = RNameNode.Nodes.Add("Продолжительность");
                RDurTerm.Nodes.Add(Ens.DurationTerm);
            }
        }
        private void ListEncums(TreeNode PNode, netFteo.Rosreestr.TMyEncumbrances Ens)
        {
            if (Ens == null) return;
            TreeNode PEnsNode = new TreeNode();
            if (Ens.Count > 0)
            {
                PEnsNode = PNode.Nodes.Add("EncNode", "Обременения");
                for (int i = 0; i <= Ens.Count - 1; i++)
                {
                    ListEncum(PEnsNode, Ens[i]);
                }
            }
        }


        /*
         /// <summary>
         /// Он-лайн запрос сведений по кварталу
         /// </summary>
         /// <param name="treeView_Web">"Дерево для отображения он-лайн сведений"</param>
         /// <param name="Block">"Кадастровый квартал"</param>
         public System.Drawing.Image GetCadastralBlockWebOnline(TreeView treeView_Web, TMyCadastralBlock Block)
         {
             treeView_Web.Visible = true;
             treeView_Web.Nodes.Clear();

             try
             {
                 TreeNode PWebNode = treeView_Web.Nodes.Add(Block.CN);
                 WebRequest wrGETURL;
                 //Запрос кварталов по кадастровому номеру, возвращает массив (сокращенные атрибуты):
                 wrGETURL = WebRequest.Create("http://pkk5.rosreestr.ru/api/features/2?text=" + Block.CN);
                 wrGETURL.Proxy = WebProxy.GetDefaultProxy();
                 wrGETURL.Timeout = 1000;
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
                     pkk5_json_response jsonResponse = sr.Deserialize<pkk5_json_response>(jsonResult);
                     if (jsonResponse != null)
                         if (jsonResponse.features != null)
                             if (jsonResponse.features.Count > 0)
                         {
                             PWebNode.Nodes.Add(jsonResponse.features[0].attrs.address).Expand();
                             //Запрос по конкретному id:
                             wrGETURL = WebRequest.Create("http://pkk5.rosreestr.ru/api/features/2/" + jsonResponse.features[0].attrs.id);
                             wrGETURL.Timeout = 10000;
                             WebResponse wrF = wrGETURL.GetResponse();
                             objStream = wrF.GetResponseStream();
                             if (objStream != null)
                             {
                                 StreamReader objFReader = new StreamReader(objStream);
                                 string jsonFResult = objFReader.ReadToEnd();
                                 objFReader.Close();
                                 pkk5_json_Fullresponse jsonFResponse = sr.Deserialize<pkk5_json_Fullresponse>(jsonFResult);
                                 if (jsonFResponse != null)
                                     if (jsonFResponse.feature != null)
                                     {
                                        // PWebNode.Nodes.Add(jsonFResponse.feature.attrs.util_by_doc);
                                         //PWebNode.Nodes.Add(jsonFResponse.feature.attrs.AreaType2Str(jsonFResponse.feature.attrs.area_type)).Nodes.Add(jsonFResponse.feature.attrs.area_value +
                                        //     " " + jsonFResponse.feature.attrs.Unit2Str(jsonFResponse.feature.attrs.area_unit));
                                       //  PWebNode.ExpandAll();

                                       //  if (jsonFResponse.feature.attrs.cad_eng_data != null)
                                         {
                                           //  TreeNode PWebNodec = PWebNode.Nodes.Add("Документы для ГКУ подготовлены");
                                          //   PWebNodec.Nodes.Add(jsonFResponse.feature.attrs.cad_eng_data.ci_surname + " " +
                                          //       jsonFResponse.feature.attrs.cad_eng_data.ci_first + " " +
                                          //       jsonFResponse.feature.attrs.cad_eng_data.ci_patronymic + " " +
                                          //       jsonFResponse.feature.attrs.cad_eng_data.ci_n_certificate);
                                         //    PWebNodec.Nodes.Add("Дата обновления атрибутов : " + jsonFResponse.feature.attrs.cad_eng_data.actual_date);
                                         //    PWebNodec.Nodes.Add("lastmodified:" + jsonFResponse.feature.attrs.cad_eng_data.lastmodified);
                                         }
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


                                 // Запрос изображения в jpeg по bbox:
                                 string sURLpkk5_jpeg = "http://pkk5.rosreestr.ru/arcgis/rest/services/Cadastre/Cadastre/MapServer/export?bbox=" +
                                                           jsonResponse.features[0].extent.xmin + "%2C" +
                                                           jsonResponse.features[0].extent.ymin + "%2C" +
                                                           jsonResponse.features[0].extent.xmax + "%2C" +
                                                           jsonResponse.features[0].extent.ymax + "%2C" +
                                                                     "&bboxSR=&layers=&layerDefs=&size=" +
                                                           pictureBox1.Size.Width.ToString() + "%2C" +
                                                           pictureBox1.Size.Height.ToString() +
                                                           "&imageSR=&format=jpg&transparent=true&dpi=&time=&layerTimeOptions=&dynamicLayers=&gdbVersion=" +
                                                           "&f=image";
                                                            //"&mapScale=1000&f=image";
                                 wrGETURL = WebRequest.Create(sURLpkk5_jpeg);
                                 wrGETURL.Timeout = 10000;
                                 WebResponse wrJpeg = wrGETURL.GetResponse();
                                 objStream = wrJpeg.GetResponseStream();
                                 if (objStream != null)
                                     //pictureBox1.Image = Bitmap.FromStream(objStream);
                                  return Bitmap.FromStream(objStream);
                             }
                         }
                 }
             }

             catch (IOException ex)
             {
                 //  MessageBox.Show(ex.ToString());
                 toolStripStatusLabel1.Text = ex.ToString();
                 return null;
             }
             return null;
         }
         */

        #endregion

        private void WriteRights(string FileName, netFteo.Rosreestr.TMyRights Rights)
        {
            if (Rights == null) return;
            if (Rights.Count > 0)
            {
                TextWriter writer = new StreamWriter(FileName, true, Encoding.Unicode);
                for (int i = 0; i <= Rights.Count - 1; i++)
                {
                    writer.Write(Rights[i].Name + "\t" +
                                           Rights[i].Type + "\t" +
                                           Rights[i].RegNumber + "\t" +
                                           Rights[i].RegDate + "\t"+
                                           Rights[i].Desc + "\t"+
                                           Rights[i].ShareText + "\t");

                    for (int io = 0; io <= Rights[i].Owners.Count - 1; io++)
                        writer.WriteLine(Rights[i].Owners[io].OwnerName);

                }

                writer.Close();
            }

        }
       
        private void SaveAsmifOMS(string FileName, PointList OMS)
        {
            if (OMS == null) return;
            if (OMS.PointCount == 0) return;
            TextWriter writer = new StreamWriter(FileName);
            TextWriter writerMIDA = new StreamWriter(Path.GetFileNameWithoutExtension(FileName) + ".mid", false, Encoding.GetEncoding("Windows-1251"));
            writer.WriteLine("Version 450");
            writer.WriteLine("Charset \"WindowsCyrillic\"");
            writer.WriteLine("Delimiter \"$\"");
            writer.WriteLine("CoordSys NonEarth Units \"m\" Bounds (" +
                             OMS.Bounds.MinY.ToString() + "," + OMS.Bounds.MinX.ToString() + ")  (" +
                             OMS.Bounds.MaxY.ToString() + "," + OMS.Bounds.MaxX.ToString() + ")");
            writer.WriteLine("Columns 3");
            writer.WriteLine("    Point_Name Char(127)");
            writer.WriteLine("    Net_Klass  Char(127)");
            writer.WriteLine("    Number Char(127)");
            writer.WriteLine("Data");
            writer.WriteLine("");
            netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
            TR.WriteMifPoints(writer, writerMIDA, OMS);
            writer.Close();
            writerMIDA.Close();
        }
       
        #region Запись в DXF



    /*
        private void CreateDxf3dPolygon(DxfDocument dxfDoc, netDxf.Tables.Layer LayerPoints, netDxf.Tables.Layer LayerText, netDxf.Tables.Layer LayerPoly, netFteo.Spatial.PointList Points)
        {
            List<PolylineVertex> PlVertexLst = new List<PolylineVertex>();  //Список Vertexов (вершин) полилинии:

            for (int i = 0; i <= Points.Count - 1; i++)
            {

                PlVertexLst.Add(new PolylineVertex(Points[i].y, Points[i].x, Points[i].z));
                netDxf.Entities.Point Pt = new Point(Points[i].y, Points[i].x, Points[i].z);
                Pt.Layer = LayerPoints;
                dxfDoc.AddEntity(Pt);
                netDxf.Entities.Text PointName = new Text();
                if (Points[i].NumGeopointA == null)
                    PointName.Value = i.ToString();
                else PointName.Value = Points[i].NumGeopointA;
                PointName.Height = 5;
                PointName.Position = new Vector3(Points[i].y, Points[i].x, Points[i].z);
                PointName.Layer = LayerText;
                dxfDoc.AddEntity(PointName);
            }

            //3Д Полилиния
            Polyline PLine = new Polyline(PlVertexLst, true); //Сама полилиния, замкнутая true:
            PLine.Layer = LayerPoly;
            dxfDoc.AddEntity(PLine);        //Вгоняем в dxf:
        }
       
        */
        //------------------------------------------------------------------------------------------
        private void SaveAsDxf(int Scale)
        {
            double ScaleRaduis;
            switch (Scale)
            {
                case 33333: ScaleRaduis = 22.5; //
                    break;
                case 10000: ScaleRaduis = 7.5;
                    break;
                case 5000: ScaleRaduis = 3.75;
                    break;
                case 1000: ScaleRaduis = 0.75;
                    break;
                case  500: ScaleRaduis = 0.375;
                    break;
                default:   ScaleRaduis = 0.75; // aka 1:1000 is default scale
                    break;
            }
            netFteo.IO.DXFWriter wr = new netFteo.IO.DXFWriter();
            saveFileDialog1.FilterIndex = 3;
            if (TV_Parcels.SelectedNode.Name == "TopNode")
                if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                {
                    //Заменить в полигонах CN квартала
                    netFteo.StringUtils.RemoveParentCN(this.DocInfo.MyBlocks.SingleCN(), this.DocInfo.MifPolygons);
                    wr.SaveAsDxfScale(saveFileDialog1.FileName, this.DocInfo.MifPolygons, ScaleRaduis);
                }

            if (TV_Parcels.SelectedNode.Name.Contains("Contours"))
            {
                TPolygonCollection Pl = (TPolygonCollection)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(8)));
                if (Pl != null)
                {
                    saveFileDialog1.FileName = "contours_" + Pl.Defintion;
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {

                        wr.SaveAsDxfScale(saveFileDialog1.FileName, Pl,ScaleRaduis);
                    }
                }
            }
            
            if (TV_Parcels.SelectedNode.Name.Contains("EntrysNode"))
            {
                TMyParcel Lot = (TMyParcel)this.DocInfo.MyBlocks.GetObject(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(10)));
                if (Lot != null)
                {
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {

                        wr.SaveAsDxfScale(saveFileDialog1.FileName, Lot.CompozitionEZ, ScaleRaduis);
                    }
                }
            }
 
            if (TV_Parcels.SelectedNode.Name.Contains("SPElem."))
            {
                TMyPolygon Pl = (TMyPolygon) this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(7)));
                if (Pl != null)
                {
                    TPolygonCollection PC = new TPolygonCollection();
                    PC.AddPolygon(Pl);
                    saveFileDialog1.FileName = netFteo.StringUtils.ReplaceSlash(Pl.Definition);
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {

                        wr.SaveAsDxfScale(saveFileDialog1.FileName, PC, ScaleRaduis);
                    }
                }
            }
          

            if (TV_Parcels.SelectedNode.Name.Contains("TPLines."))  
            {
                TPolyLines Plist = (TPolyLines) this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(8)));
                if (Plist != null)
                {
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {

                        wr.SaveAsDxfScale(saveFileDialog1.FileName, Plist, ScaleRaduis);
                    }
                }

            }

            //Не Все зоны - только территориальные, ==(1) ??
            if (TV_Parcels.SelectedNode.Name.Contains("ZonesNode"))
            {
                TPolygonCollection Plc = this.DocInfo.MyBlocks.GetZonesEs(1);
                if (Plc != null)
                {
                    saveFileDialog1.FileName = "ZonesNode";
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {

                        wr.SaveAsDxfScale(saveFileDialog1.FileName, Plc, ScaleRaduis);
                    }
                }
            }

            //"OKSsNode"
            if (TV_Parcels.SelectedNode.Name.Contains("OKSsNode"))
            {
                TPolygonCollection Plc = this.DocInfo.MyBlocks.GetRealtyEs();
                if (Plc != null)
                {
                    saveFileDialog1.FileName = "OKSsNode";
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {

                        wr.SaveAsDxfScale(saveFileDialog1.FileName, Plc, ScaleRaduis);
                    }
                }
            }


            if (TV_Parcels.SelectedNode.Name.Contains("OMSPoints"))
            {
                if (this.DocInfo.MyBlocks.OMSPoints.PointCount > 0)
                {
                    saveFileDialog1.FileName = "omsPoints";
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {

                        wr.SaveAsDxfScale(saveFileDialog1.FileName, this.DocInfo.MyBlocks.OMSPoints, ScaleRaduis);
                    }
                }
            }
        }

  
   


        #endregion

  

        #region Херня всякая, не каждодневная

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 AB1 = new AboutBox1();
            AB1.ShowDialog(this);
        }
      /// <summary>
      /// Очистка временных файлов
      /// </summary>
        private void ClearFiles()
        {
            
            if (Directory.Exists(this.Folder_Unzip))
            {
                Directory.Delete(this.Folder_Unzip, true);
            }

            if (! Directory.Exists(this.Folder_XSD))
            {
                Directory.CreateDirectory(Folder_XSD);
            }

            if (!Directory.Exists(this.Folder_XSD+"\\SchemaCommon"))
            {
                Directory.CreateDirectory(Folder_XSD + "\\SchemaCommon");
            }

        }

        private void ClearControls()
        {
            richTextBox1.Clear();
            listView1.Controls.Clear();
            listView1.Items.Clear();
            listView_Properties.Items.Clear();
             contextMenuStrip_SaveAs.Enabled = false;
            listView_Contractors.Items.Clear();
            TV_Parcels.Nodes.Clear();
            TV_Parcels.ImageIndex = imList_dStates.Images.Count;
            if (this.DocInfo == null)
                this.DocInfo = new netFteo.XML.FileInfo();

            else
            {
                this.DocInfo.FileName = null;
                this.DocInfo.FilePath = null;
                this.DocInfo.Comments = null;
                this.DocInfo.Version = null;
                this.DocInfo.Number = null;
                this.DocInfo.Date = null;
            }
            cXmlTreeView2.Clear();

            textBox_Appointment.Text = "";
            textBox_DocDate.Text = "";
            textBox_DocNum.Text = ""; linkLabel_Recipient.Text = ""; linkLabel_Request.Text = ""; 
            textBox_Appointment.Text = "";
            textBox_FIO.Text = "";
            textBox_OrgName.Text = "";
            toolStripStatusLabel1.Text = "";
            tabPage3.Show();
            tabPage3.Text = "-";
            tabPage5.Text = "XML";
            label_FileSize.Text = "";
            label2.Text = "Получатель";
            toolStripStatusLabel2.Image = XMLReaderCS.Properties.Resources.exclamation;
            PreloaderMenuItem.LoadingCircleControl.Active = false;
            PreloaderMenuItem.Visible = false;
            PreloaderMenuItem.BackColor = Color.Transparent;
            документToolStripMenuItem.Enabled = false;
            pathToHtmlFile = null;
            this.Text = TextDefault;
            toolStripProgressBar1.Value = 0;
            this.DocInfo.MyBlocks.Blocks.Clear();
            Gen_id.Reset();
           
#if (DEBUG) 
            debugToolStripMenuItem.Enabled = true;

#else
            debugToolStripMenuItem.Enabled = false;
         //   проверкаГеометрииToolStripMenuItem.Enabled = false;
#endif
        }
       

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        // ********************************************** mif ********************************************
        private void mifКПТToolStripMenuItem_Click(object sender, EventArgs e)
        {

            netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
            saveFileDialog1.FilterIndex = 1; // mif
            {

                if (TV_Parcels.SelectedNode.Name == "TopNode")
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        TR.SaveAsmif(saveFileDialog1.FileName, this.DocInfo.MifPolygons);
                        string test = Path.GetDirectoryName(saveFileDialog1.FileName) + "\\OKS_" + Path.GetFileName(saveFileDialog1.FileName);
                        TR.SaveAsmif(Path.GetDirectoryName(saveFileDialog1.FileName) + "\\OKS_" + Path.GetFileName(saveFileDialog1.FileName),
                                                      this.DocInfo.MifOKSPolygons);
                    }


                if (TV_Parcels.SelectedNode.Name == "OMSPoints")
                {
                    saveFileDialog1.FileName = "ОМС.mif";// +this.DocInfo.MyBlocks.Blocks[0].CN;
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {

                        SaveAsmifOMS(saveFileDialog1.FileName, this.DocInfo.MyBlocks.OMSPoints);

                    }
                }

                //Для отдельного ОИПД выгружаем:
                if (TV_Parcels.SelectedNode.Name.Contains("SPElem"))
                {
                    TMyPolygon Pl = (TMyPolygon)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(7)));
                    if (Pl != null)
                    {
                        saveFileDialog1.FileName = netFteo.StringUtils.ReplaceSlash(Pl.Definition);
                        if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                        {
                            TR.SaveAsmif(saveFileDialog1.FileName, Pl);
                        }

                    }
                }

                if (TV_Parcels.SelectedNode.Name.Contains("EntrysNode"))
                {
                    TMyParcel Lot = (TMyParcel)this.DocInfo.MyBlocks.GetObject(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(10)));
                    if (Lot != null)
                    {
                        if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                        {
                            //saveFileDialog1.FileName = "EZP_" + netFteo.StringUtils.ReplaceSlash(Lot.CN);
                            TR.SaveAsmif(saveFileDialog1.FileName, Lot.CompozitionEZ);
                        }
                    }
                }

                if (TV_Parcels.SelectedNode.Name.Contains("Contours"))
                {
                    TPolygonCollection Pl = (TPolygonCollection)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(8)));
                    if (Pl != null)
                    {
                        saveFileDialog1.FileName = "Contours_" + netFteo.StringUtils.ReplaceSlash(Pl.Defintion);
                        if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                        {
                            TR.SaveAsmif(saveFileDialog1.FileName, Pl);
                        }
                    }
                }

            }
        }




        private void TV_Parcels_KeyUp(object sender, KeyEventArgs e)
        {
            //  ListSelectedNode(TV_Parcels.SelectedNode);
        }

        private void бибиотекаKVZUV609ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void открытьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();// OpenXML_KVZUTyped();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void typedClassesXSD2ClassessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();// OpenXML_KVZUTyped();
        }

        public void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFile();// OpenXML_KVZUTyped();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            AboutBox1 AB1 = new AboutBox1();
            AB1.ShowDialog(this);
        }



        private void TV_Parcels_MouseClick(object sender, MouseEventArgs e)
        {
            // ListSelectedNode(TV_Parcels.SelectedNode);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //XMLBodyLoader.XML2TreeView(XMLDoc, treeView_XMLBody);// Загрузим тело в дерево
        }

        private void TV_Parcels_Click_1(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left)
              ListSelectedNode(TV_Parcels.SelectedNode);
        }

        private void TV_Parcels_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {

        }

        private void TV_Parcels_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ListSelectedNode(TV_Parcels.SelectedNode);
        }

        private void картапланToolStripMenuItem_Click(object sender, EventArgs e)
        {
         /*
           MapPlanEditor.frmZoneV03Editor MF = new MapPlanEditor.frmZoneV03Editor();
           if (MPV05 != null) MF.ParseMPV05(MPV05);
           MF.ShowDialog();
          * */
        }






        private void textBox_DocNum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(textBox_DocNum.Text);
        }

        private void contextMenuStrip_SaveAs_Opening(object sender, CancelEventArgs e)
        {

        }

        private void списокПравToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            /*
            if (TV_Parcels.SelectedNode.Name.Contains("Rights"))
            {
                Int32 id = Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(6));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                if (O.ToString() == "netFteo.Spatial.TMyParcel")
                {
                    TMyParcel P = (TMyParcel)O;
                    saveFileDialog1.FilterIndex = 2;
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        WriteRights(saveFileDialog1.FileName, P.Rights);

                    }
                }
            }
            */

        }



        private void listView_Properties_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender != listView_Properties) return;

            if (e.Control && e.KeyCode == Keys.C)
                CopySelectedValuesToClipboard(listView_Properties);
        }

        private void CopySelectedValuesToClipboard(ListView LV)
        {
            var builder = new StringBuilder();
            foreach (ListViewItem item in LV.SelectedItems)
                foreach (ListViewItem.ListViewSubItem sub in item.SubItems)
                 builder.AppendLine(sub.Text+"\t");

            Clipboard.SetText(builder.ToString());
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (TV_Parcels.SelectedNode != null)
            {
                Clipboard.SetText(TV_Parcels.SelectedNode.Text);
            }

        }

        private void TV_Parcels_KeyDown(object sender, KeyEventArgs e)
        {
            //copy:
            if (e.KeyData == (Keys.Control | Keys.C))
            {
                if (TV_Parcels.SelectedNode != null)
                {
                    Clipboard.SetText(TV_Parcels.SelectedNode.Text);
                }
                e.SuppressKeyPress = true;
            }
   
            
        }
        
        
        #region SaveAs Text (FixosoftTXT2018, Rights)
        private void toolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            saveFileDialog1.FilterIndex = 2;
            if (TV_Parcels.SelectedNode.Name.Contains("Contours"))
            {
                  TPolygonCollection Pl =  (TPolygonCollection) this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(8)));
                if (Pl != null)
                {
                    saveFileDialog1.FileName = "Contours_" + Pl.Defintion;
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                        TR.SaveAsFixosoftTXT2018(saveFileDialog1.FileName, Pl, Encoding.Unicode);
                        
                    }
                }
            }

            if (TV_Parcels.SelectedNode.Name.Contains("EntrysNode"))
            {
                TMyParcel Lot = (TMyParcel)this.DocInfo.MyBlocks.GetObject(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(10)));
                if (Lot != null)
                {
                    saveFileDialog1.FileName = "EZP_" + netFteo.StringUtils.ReplaceSlash(Lot.CN);
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                        //TR.SaveAsFixosoftTXT2016(saveFileDialog1.FileName, Lot.CompozitionEZ);
                        TR.SaveAsFixosoftTXT2018(saveFileDialog1.FileName, Lot.CompozitionEZ, Encoding.Unicode);

                    }
                }
            }
             
          if (TV_Parcels.SelectedNode.Name.Contains("SPElem."))
            {
                TMyPolygon Pl = (TMyPolygon) this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(7)));
                if (Pl != null)
                {
                    saveFileDialog1.FileName = netFteo.StringUtils.ReplaceSlash(Pl.Definition);
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                        //TR.SaveAsFixosoftTXT2016(saveFileDialog1.FileName, Pl);
                        TR.SaveAsFixosoftTXT2018(saveFileDialog1.FileName, Pl, Encoding.Unicode);
                    }
                }
            }

          //если полилиния
          if (TV_Parcels.SelectedNode.Name.Contains("TPLines."))
          {
              TPolyLines Plist = (TPolyLines)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(8)));
              if (Plist != null)
                  saveFileDialog1.FileName = netFteo.StringUtils.ReplaceSlash(TV_Parcels.SelectedNode.Name);
              if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
              {
                  {
                      netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                      TR.SaveAsFixosoftTXT2017(saveFileDialog1.FileName, Plist);
                  }
              }

          }



          if (TV_Parcels.SelectedNode.Name == "OMSPoints")
          {
              if (this.DocInfo.MyBlocks.OMSPoints[0].NumGeopointA != null)
                  saveFileDialog1.FileName = "ОМС-" + this.DocInfo.MyBlocks.OMSPoints[0].NumGeopointA;
              else saveFileDialog1.FileName = "ОМС";
              if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
              {
                  netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                  TR.SaveAsOMSTXT(this.DocInfo.MyBlocks.SingleCN(), saveFileDialog1.FileName, this.DocInfo.MyBlocks.OMSPoints);
              }
          }

            if (TV_Parcels.SelectedNode.Name == "TopNode")
                if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                {
                    netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                    TR.SaveAsFixosoftTXT2018(saveFileDialog1.FileName, this.DocInfo.MifPolygons, Encoding.Unicode);
                    TR.SaveAsFixosoftTXT2018(Path.GetDirectoryName(saveFileDialog1.FileName) + 
                                             "\\OKS_" + Path.GetFileName(saveFileDialog1.FileName), this.DocInfo.MifOKSPolygons, Encoding.Unicode);
                }

            //"OKSsNode"
            if (TV_Parcels.SelectedNode.Name.Contains("OKSsNode"))
            {
                TPolygonCollection Plc = this.DocInfo.MyBlocks.GetRealtyEs();
                if (Plc != null)
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {

                        netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                        TR.SaveAsFixosoftTXT2018(saveFileDialog1.FileName, Plc, Encoding.Unicode);
                    }
            }

            if (TV_Parcels.SelectedNode.Name.Contains("Rights"))
            {
                Int32 id = Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(6));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                if (O.ToString() == "netFteo.Spatial.TMyParcel")
                {
                    TMyParcel P = (TMyParcel)O;
                    saveFileDialog1.FileName = "Права_ГКН_" + netFteo.StringUtils.ReplaceSlash(P.CN);
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        WriteRights(saveFileDialog1.FileName, P.Rights);
                    }
                }
            }

            if (TV_Parcels.SelectedNode.Name.Contains("EGRNRight"))
            {
                Int32 id = Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(9));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                if (O.ToString() == "netFteo.Spatial.TMyParcel")
                {
                    TMyParcel P = (TMyParcel)O;
                    saveFileDialog1.FileName = "Права_ГРП_" + netFteo.StringUtils.ReplaceSlash(P.CN);
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        WriteRights(saveFileDialog1.FileName, P.EGRN);
                    }
                }
            }

        }
        #endregion

        private void m1500ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsDxf(500);
        }
        #endregion

        private void ChangeXYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TMyPolygon Pl = (TMyPolygon) this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(listView1.Tag));// Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(7)));
            if (Pl != null)
            {
                Pl.ExchangeXY();
                PointListToListView(listView1, Pl);
            }

        }

        private void m11000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsDxf(1000);
        }

        private void listView_Properties_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void показатьПДToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void eSlibdllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Function2(1975);
        }

        private void toolStripMI_ShowES_Click(object sender, EventArgs e)
        {
            Toggle_Visualizer();
        }
        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            Toggle_Visualizer();
        }        
        private void Toggle_Visualizer()
        {

            if (toolStripMI_ShowES.Checked) 
            { ESwindow.Hide(); 
                toolStripMI_ShowES.Checked = false;
                toolStripButton_VisualizerToggle.Checked = false;
                просмотрToolStripMenuItem.Checked = false;
                return; }

            
                ESwindow.Title = "Визуализация ПД (WPF)";
                ViewWindow = new EntityViewer();
                ViewWindow.Definition = "Viewer Created ok";
                ESwindow.Content = ViewWindow;
                ESwindow.MinHeight = 300; ESwindow.MinWidth = 500;
                ESwindow.Height = this.Height;  //576 ? just added to have a smaller control (Window)
                ESwindow.Width = 810;
            
                ESwindow.Top = this.Top; ESwindow.Left = this.Left + 1 + this.Width;
                ESwindow.Show();// ShowDialog();
                                // checkonClick = true тогда не нужно это: 
                toolStripMI_ShowES.Checked = true;
                toolStripButton_VisualizerToggle.Checked = true;
                просмотрToolStripMenuItem.Checked = true;

            if (TV_Parcels.SelectedNode != null)
                if (TV_Parcels.SelectedNode.Name.Contains("SPElem."))
                {
                    ListSelectedNode(TV_Parcels.SelectedNode);
                }
        }

        private void eSViewerlibmcvcdllF1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Function1(1975);
        }



        private void пересеченияMifToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void mSVCESCheckerFunc2Int1975ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unsafe {  Func2(1975); }
        }

        private void csvStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FilterIndex = 4;
            if (TV_Parcels.SelectedNode.Name.Contains("SPElem."))
            {
                TMyPolygon Pl = (TMyPolygon) this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(7)));
                if (Pl != null)
                {
                    saveFileDialog1.FileName = netFteo.StringUtils.ReplaceSlash(Pl.Definition);
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                        TR.SaveAsTexnoCADCSV (saveFileDialog1.FileName, Pl);
                    }
                }
            }

            if (TV_Parcels.SelectedNode.Name.Contains("EntrysNode"))
            {
                TMyParcel Lot = (TMyParcel)this.DocInfo.MyBlocks.GetObject(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(10)));
                if (Lot != null)
                {
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                        TR.SaveAsTexnoCADCSV(saveFileDialog1.FileName, Lot.CompozitionEZ);
                    }
                }
            }

            if (TV_Parcels.SelectedNode.Name == "TopNode")
                if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                {
                    netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                    TR.SaveAsTexnoCADCSV(saveFileDialog1.FileName, this.DocInfo.MifPolygons);

                }
        }

        private void m110000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsDxf(10000);
        }

        private void label_FileSize_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel_FileName_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(linkLabel_FileName.Text);
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            /*
            cXmlTreeView1.BeginUpdate();
            cXmlTreeView1.ExpandAll();
            cXmlTreeView1.EndUpdate();
            */
        }

        //Открыть в бровзере человекочитаемый формат xml через xsl
        private void документToolStripMenuItem_Click(object sender, EventArgs e)
        {



            if (File.Exists( DocInfo.FilePath))
            {
                netFteo.XML.XSLWriter XSLwr = new netFteo.XML.XSLWriter();
                pathToHtmlFile= XSLwr.TransformXMLToHTML(DocInfo.FilePath);
                hrefToXSLT = XSLwr.hrefToXSLTServer;
                if ((hrefToXSLT != "") && (pathToHtmlFile != ""))
                   System.Diagnostics.Process.Start(pathToHtmlFile);
            }
            else документToolStripMenuItem.Enabled = false;
            /*
            if (File.Exists(ArchiveFolder + DocInfo.FileName))
            {
                XSLWriter XSLwr = new XSLWriter();
                pathToHtmlFile = XSLwr.TransformXMLToHTML(ArchiveFolder + DocInfo.FileName);
                hrefToXSLT = XSLwr.hrefToXSLTServer;
                if (hrefToXSLT != null)
                    System.Diagnostics.Process.Start(pathToHtmlFile);
                документToolStripMenuItem.Enabled = true;
            }
            */
            
        }


      

        //Показать в форме 
        private void tabPage4_Enter(object sender, EventArgs e)
        {
            /*
            if (! File.Exists(pathToHtmlFile))
                if (File.Exists(DocInfo.FileName))
                {
                    XSLWriter XSLwr = new XSLWriter();
                    pathToHtmlFile = XSLwr.TransformXMLToHTML(DocInfo.FileName);
                    hrefToXSLT = XSLwr.hrefToXSLTServer;
                    
                }
            
                 
            if (pathToHtmlFile != null)
            {
                string HtmlPath = "file://" + Path.GetDirectoryName(Path.GetFullPath(DocInfo.FileName)) + "\\" + pathToHtmlFile;
                webBrowser1.Navigate(HtmlPath);
            }
            */
        }


        private void KVZU_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (File.Exists(pathToHtmlFile)) File.Delete(pathToHtmlFile); // если был сеанс
            XMLReaderCS.Properties.Settings.Default.Recent0 = DocInfo.FilePath;
            XMLReaderCS.Properties.Settings.Default.Save();
            ESwindow.Close();
        }

        private void RecentFile0MenuItem_Click(object sender, EventArgs e)
        {
            this.Read(XMLReaderCS.Properties.Settings.Default.Recent0, true);
        }

        private void label_DocType_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(label_DocType.Text+" №"+ textBox_DocNum.Text+ " от "+textBox_DocDate.Text);
        }


        private int ConvertToInt32(string str)
        {
            if (str.Length < 5) return -1;
            if (str.Substring(1,4) == "Node") // PNode, ZNode, 
                return
                  Convert.ToInt32( str.Substring(5));
            if (str.Length > 8)
            if (str.Substring(0, 8) == "FlatItem") // FlatItem node
                return
                  Convert.ToInt32(str.Substring(8));
            if (str.Substring(0, 5) == "Flats") // Flat node
                return
                  Convert.ToInt32(str.Substring(5));

            if (str.Substring(0, 4) == "Flat") // Flat node
                return
                  Convert.ToInt32(str.Substring(4));

            return -1;
        }

// Найти pkk5 server viewer:
        private void онлайнЗапросToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TV_Parcels.SelectedNode == null) return;

            string cn =null;
              //if (TV_Parcels.SelectedNode.Name.Contains("PNode"))
              //{
                  Int32 id = ConvertToInt32(TV_Parcels.SelectedNode.Name); // PNode CNNode

                  object O = this.DocInfo.MyBlocks.GetObject(id);
                  if (O != null)
                  {
                      if (O.ToString() == "netFteo.Spatial.TMyParcel") //Пока только для ЗУ, ПКК5 пока все равно не обрабатывает оксы
                      {
                          cn = ((TMyParcel)O).CN;
                          pkk5Viewer1.Start(cn, pkk5_Types.Parcel);
                      }

                      if (O.ToString() == "netFteo.Spatial.TMyRealty") //далее - добавим ОКС. 
                      {
                          cn = ((TMyRealty)O).CN;
                          pkk5Viewer1.Start(cn, pkk5_Types.OKS);//oks
                      }

                      if (O.ToString() == "netFteo.Spatial.TFlat") //далее - ОКС. 
                      {
                          cn = ((TFlat)O).CN;
                          pkk5Viewer1.Start(cn, pkk5_Types.OKS);//oks
                      }

                      if (O.ToString() == "netFteo.Spatial.TZone") //
                      {
                          cn = ((TZone)O).AccountNumber;
                          pkk5Viewer1.Start(cn, pkk5_Types.TerrZone);// Terr Zone!!!
                      }
                  }
        

              if (TV_Parcels.SelectedNode.Name.Contains("oNode")) // исходные зу
              {
                  cn = TV_Parcels.SelectedNode.Text;
                  pkk5Viewer1.Start(TV_Parcels.SelectedNode.Text, pkk5_Types.Parcel);//oks

              }

              if (TV_Parcels.SelectedNode.Name.Contains("CN")) // исходные зу
              {
                  cn = TV_Parcels.SelectedNode.Name;
                  pkk5Viewer1.Start(TV_Parcels.SelectedNode.Text, pkk5_Types.Parcel);//Parent

              }

              if (TV_Parcels.SelectedNode.Name.Contains("TopNode"))
              {
                  //Int32 id = Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(5));
                  if (this.DocInfo.MyBlocks.Blocks.Count == 1)
                  {
                      cn = this.DocInfo.MyBlocks.Blocks[0].CN;
                      pkk5Viewer1.Start(cn, pkk5_Types.Block);
                  }
              }

              if (cn != null)
              {
                  tabControl1.SelectedIndex = 3;
              }
        }



// Обработчик динамической Гиперметки pkk5:)
        private void OnPKK5LabelActionClick(object sender, EventArgs e) 
        {

        }


        // Обработчик динамической Кнопки "Сохранить как..."
        private void OnPKK5ButtonActionClick(object sender, EventArgs e)
        {

            Button btnSender = (Button)sender;
            System.Drawing.Point ptLowerLeft = new System.Drawing.Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            contextMenuStrip_SaveAs.Show(ptLowerLeft);
        }

        private void GotoPKK5(string CN)
        {
            //http://pkk5.rosreestr.ru/#x=&y=&z=&type=1&zoomTo=1&app=search&opened=1&text=26:6:130701:53
            // https://rosreestr.ru/wps/portal/p/cc_ib_portal_services/online_request

            string pkk5RequestURL = "http://pkk5.rosreestr.ru/#x=&y=&z=&type=1&zoomTo=1&app=search&opened=1&text=" + CN;
                System.Diagnostics.Process.Start(pkk5RequestURL);
        }


        private void найтиНаПККбровзерToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TV_Parcels.SelectedNode == null) return;
            if (TV_Parcels.SelectedNode.Name.Contains("PNode"))
            {
                Int32 id = Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(5));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                if (O.ToString() == "netFteo.Spatial.TMyParcel")
                {
                    GotoPKK5(((TMyParcel)O).CN);
                }

                if (O.ToString() == "netFteo.Spatial.TMyOKS") 
                {
                    GotoPKK5(((TMyRealty)O).CN);
                }

            }
        }

        private void m5000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsDxf(5000);
        }

// Save xml. Точнее сериализовать в XML 
        private void xmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FilterIndex = 5; // xml


            if ((TV_Parcels.SelectedNode.Name == "TopNode") && (this.DocInfo.MifPolygons != null))
                if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                {
                    //Заменить в полигонах CN квартала
                    //netFteo.StringUtils.RemoveParentCN(this.DocInfo.MyBlocks.SingleCN(), this.DocInfo.MifPolygons);
                    XmlSerializer serializer = new XmlSerializer(typeof(TPolygonCollection));
                    TextWriter writer = new StreamWriter(saveFileDialog1.FileName);
                    serializer.Serialize(writer, this.DocInfo.MifPolygons);
                    writer.Close();
                }
            


            if (TV_Parcels.SelectedNode.Name.Contains("Contours"))
            {
                TPolygonCollection Pl = (TPolygonCollection)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(8)));
                if (Pl != null)
                {
                    saveFileDialog1.FileName = "Contours_" + Pl.Defintion;
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(TPolygonCollection));
                        TextWriter writer = new StreamWriter(saveFileDialog1.FileName);
                        serializer.Serialize(writer, Pl);
                        writer.Close();
                    }
                }
            }


            if (TV_Parcels.SelectedNode.Name.Contains("SPElem."))
            {
                TMyPolygon Pl = (TMyPolygon)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(7)));
                if (Pl != null)
                {
                    saveFileDialog1.FileName = netFteo.StringUtils.ReplaceSlash(Pl.Definition);
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(TMyPolygon));
                        TextWriter writer = new StreamWriter(saveFileDialog1.FileName);
                        serializer.Serialize(writer, Pl);
                        writer.Close();
                    }
                }
            }


            // Сохраним ЕЗП из кадастровой выписки/Межевого плана как часть МП_V06
            // Тухлый Technokad Express сожрал чижика на 100 входящих в ЕЗП....
            if (TV_Parcels.SelectedNode.Name.Contains("EntrysNode"))
            {
                Int32 id = Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(10));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                if (O.ToString() == "netFteo.Spatial.TMyParcel")
                {
                    TMyParcel P = (TMyParcel)O;
                    // Выписываем явное проебразование:
                    // cast from netfteo.CompozitionEZ to MP_V06.tExistEZEntryParcelCollection :
                    RRTypes.MP_V06.tExistEZEntryParcelCollection compoz = RRTypes.MP_V06.CasterEZPEntrys.CastEZP(P.CompozitionEZ);//new RRTypes.MP_V06.tExistEZEntryParcelCollection();

                    saveFileDialog1.FileName = "CompositionEZ_" + netFteo.StringUtils.ReplaceSlash(P.CN);
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(RRTypes.MP_V06.tExistEZEntryParcelCollection));
                        TextWriter writer = new StreamWriter(saveFileDialog1.FileName);
                        serializer.Serialize(writer, compoz);
                        writer.Close();
                    }
                }

       
            }

        }

        


        private void копироватьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (TV_Parcels.SelectedNode != null)
            {
               // Clipboard.SetText(treeView_OnLine.SelectedNode.Text);
            }

        }
        /*
        private void pkk5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView_OnLine.SelectedNode.Tag != null)
            if (treeView_OnLine.SelectedNode.Tag.ToString() == "256")
            {
                string pkk5RequestURL = "http://pkk5.rosreestr.ru/arcgis/rest/services/Cadastre/Cadastre/MapServer/export?bbox=" +
                    treeView_OnLine.SelectedNode.Text +
                    "& bboxSR = &layers = &layerDefs = &size = &imageSR = &format = png & transparent = false & dpi = &time = &layerTimeOptions = &dynamicLayers = &gdbVersion = &mapScale = 10000 & f = html";
                System.Diagnostics.Process.Start(pkk5RequestURL);
            }
        }

        private void поискНаПККToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (treeView_OnLine.SelectedNode.Tag != null)
                if (treeView_OnLine.SelectedNode.Tag.ToString() == "255")
                {
                    string pkk5RequestURL = "http://pkk5.rosreestr.ru/" +
                        treeView_OnLine.SelectedNode.Text ;
                    System.Diagnostics.Process.Start(pkk5RequestURL);
                }
        }

        */
      

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void pkk5Viewer1_Click(object sender, EventArgs e)
        {

        }

      // возникает при первом отображении формы
        private void KVZU_Form_Shown(object sender, EventArgs e)
        {
            RecentFilesMenuItem.DropDownItems.Clear();
            ToolStripItem rc0 =   RecentFilesMenuItem.DropDownItems.Add(XMLReaderCS.Properties.Settings.Default.Recent0);
            rc0.Click += RecentFile0MenuItem_Click; // handler for sub menu
            openFileDialog1.InitialDirectory = XMLReaderCS.Properties.Settings.Default.LastDir;
          //  ListMyCoolections(DocInfo.MyBlocks, DocInfo.MifPolygons);
          //  ListFileInfo(DocInfo);
        }

        private void fteoImage1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            BugReport_MP06();
        }

        
      private RRTypes.MP_V06.MP BugReport_MP06()
        {
               openFileDialog1.Filter = "Межевой план 06|*.zip";
              openFileDialog1.FilterIndex = 1; openFileDialog1.FileName = "";
              if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
              {
                  BugReport_MP06(openFileDialog1.FileName);
                return null;
              }
              else return null;
        }

        //-----------------------------------Проверка пересечений MP06---------------------------
        // - проверяемый файл - zip межевого плана  с форматом имени GKUZU_.... Рассматриваем версию 6
        // - данные проверки - данные КПТ/КВ/КП/КВЕГРН (или как там-ее)
        //  в общем то, что у нас загружено в this.DocInfo.MyBlocks.

        private void BugReport_MP06 (string ArciveFileName)
        {
            try
                {
                    // using (ZipArchive archive = System.IO.Compression.ZipFile.Open(zipPath, ZipArchiveMode.Update))
                    var options = new ReadOptions { StatusMessageWriter = System.Console.Out };
                     ZipFile zip = ZipFile.Read(ArciveFileName, options);
                /*
                    if (zip.EntryFileNames.Contains("GKUZU"))
                        zip.ExtractAll(Application.StartupPath);

                */
                    ESChecker_MP06Form ESChecker_MP06frm = new ESChecker_MP06Form();
                    ESChecker_MP06frm.MP06ZiptoCheck = zip;
                    ESChecker_MP06frm.ShowDialog();
                }

                catch (System.Exception ex1)
                {
                    //   System.Console.Error.WriteLine("exception: " + ex1);

                }
                //Read(openFileDialog1.FileName);            
            
          //  netFteo.IO.TextReader TR = new netFteo.IO.TextReader();
         
            /*
            if (TV_Parcels.SelectedNode.Name.Contains("SPElem."))
            {
                TMyPolygon Pl = (TMyPolygon)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(7)));
                if (Pl != null)
                {
                    if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        TPolygonCollection TestES = TR.ImportTxtFile(openFileDialog1.FileName);
                        if (TestES != null)
                        {
                            TMyPoints EsCheckres = Pl.FindSectsES(TestES);
                            if (EsCheckres.PointCount > 0) { MessageBox.Show("Найдено " + EsCheckres.PointCount.ToString() + " пересечений"); }
                            if (EsCheckres.PointCount == 0) { MessageBox.Show("Пересечений не обнаружено"); }
                        }
                    }
                }
            }


            if (TV_Parcels.SelectedNode.Name.Contains("PNode"))
            {

                if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                {
                    Int32 id = Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(5));
                    object O = this.DocInfo.MyBlocks.GetObject(id);
                    if (O.ToString() == "netFteo.Spatial.TMyParcel")
                    {
                        TMyParcel P = (TMyParcel)O;

                        TPolygonCollection TestES = TR.ImportTxtFile(openFileDialog1.FileName);
                        if (TestES != null)
                        {
                            TMyPoints EsCheckres = P.CheckEs(TestES);
                            if (EsCheckres.PointCount > 0) { MessageBox.Show("Найдено " + EsCheckres.PointCount.ToString() + " пересечений"); }
                            if (EsCheckres.PointCount == 0) { MessageBox.Show("Пересечений не обнаружено"); }
                        }
                    }
                }
            }
            */


        }
        private void BugReport_MP06_II(string archiveFolder)
        {
            try
            {
              
                ESChecker_MP06Form ESChecker_MP06frm = new ESChecker_MP06Form();
                ESChecker_MP06frm.MP06UnZiptoCheck = archiveFolder;
                Read(ESChecker_MP06frm.MP_v06);
                ESChecker_MP06frm.ShowDialog();
                ListMyCoolections(this.DocInfo.MyBlocks, this.DocInfo.MifPolygons);
                ListFileInfo(DocInfo);
            }

            catch (System.Exception ex1)
            {
               DocInfo.CommentsType = "Exception";
               DocInfo.Comments = ex1.Message;
               ListFileInfo(DocInfo);
            }
 


        }


        private void списокТочекФайлNikonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Для отдельного ОИПД выгружаем:
            netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
            saveFileDialog1.FilterIndex = 2; // txt

            if (TV_Parcels.SelectedNode.Name.Contains("SPElem"))
            {
                TMyPolygon Pl = (TMyPolygon)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(7)));
                if (Pl != null)
                {
                    saveFileDialog1.FileName = netFteo.StringUtils.ReplaceSlash(Pl.Definition);
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {


                        TR.SaveAsNikon(saveFileDialog1.FileName, Pl.AsPointList());
                    }

                }
            }
        }



        private void копироватьToolStripMenuItem2_Click(object sender, EventArgs e)
        {
           List<string> items = new List<string>();
           Control parent = ((ContextMenuStrip)(((ToolStripMenuItem)sender).Owner)).SourceControl;

            foreach(ListViewItem lvit in ((ListView)parent).Items)
            {
              string sub = "";
             //   if (lvit.Text != "")
                {
                    foreach (ListViewItem.ListViewSubItem lv in lvit.SubItems)
                    {
                      if (lv.Text != lvit.Text) // бывает что суб повторяет саму ноду
                         sub += "\t" + lv.Text;
                    }


                    items.Add(lvit.Text+sub);
                }
            }

            if (items.Count > 0)
            {
                //LINQ:
                Clipboard.SetText(string.Join(Environment.NewLine, items.Cast<string>().Select(o => o.ToString()).ToArray()));  
            }
        }

        private void linkLabel_Recipient_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(linkLabel_Recipient .Text);
        }

        private void linkLabel_Request_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(linkLabel_Request.Text);
        }

        private void textBox_DocDate_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox_DocDate.Text);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void textBox_Appointment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(textBox_Appointment.Text);
        }

        private void textBox_OrgName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(textBox_OrgName.Text);
        }

        private void textBox_FIO_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(textBox_FIO.Text);
        }

        private void KVZU_Form_InputLanguageChanging(object sender, InputLanguageChangingEventArgs e)
        {

        }

        private void KVZU_Form_Resize(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Width = (int) Math.Round(this.Width / 2.76);
            toolStripStatusLabel3.Width = (int)Math.Round(this.Width / 2.594);
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOptions frmoptions = new frmOptions();
            frmoptions.dutilizations_v01 = this.dutilizations_v01;
            frmoptions.dRegionsRF_v01 = this.dRegionsRF_v01;
            frmoptions.dStates_v01 = this.dStates_v01;
            frmoptions.dLocationLevel1_v01 = this.dLocationLevel1_v01;
            frmoptions.dLocationLevel2_v01 = this.dLocationLevel2_v01;
            frmoptions.MP_06_schema = this.MP_06_schema;
            frmoptions.ShowDialog();
        }

        private void сертификатыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCertificates frmcertificates = new frmCertificates();
            frmcertificates.ShowDialog();

        }

        private void m133333ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsDxf(33333);
        }

        private void RecentFilesMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip_OIPD_Opening(object sender, CancelEventArgs e)
        {

        }

        private void KVZU_Form_LocationChanged(object sender, EventArgs e)
        {
            if (ESwindow != null)
                if (ESwindow.Visibility == System.Windows.Visibility.Visible)
                {
                    ESwindow.Top = this.Top; 
                    ESwindow.Left = this.Left - 13  + this.Width;
                }
        }

        private void межевойПланToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BugReport_MP06();
        }

        private void генераторGUIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GUIDfrm GUIDGenfrm = new GUIDfrm();
            GUIDGenfrm.ShowDialog();
        }

        private void просмотрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Toggle_Visualizer();
        }

        private void KVZU_Form_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void KVZU_Form_KeyPress(object sender, KeyPressEventArgs e)
        {
 
        }

        private void tabControl1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }





    

            private void TV_Parcels_KeyUp_1(object sender, KeyEventArgs e)
        {
            //search Ctrl+F  : trough menu item shortcut Ctrl+F;
            /*
            if ((e.Control) && (e.KeyValue == 70))
            {
              if (SearchTextBoxSwith(SearchTextBox))
                TV_Parcels.Focus();
                e.SuppressKeyPress = true;
            }  */
        }

        
        // Случай #1 для поиска  ноды

        /// <summary>
        /// Поиск по дереву по тексту Node
        /// </summary>
        /// <param name="srcNodes"></param>
        /// <param name="searchstring"></param>
        /// <param name="foundFirst"></param>
        private void FindNode(TreeNode srcNodes, string searchstring, bool foundFirst)
        {
            if (searchstring == "") return;
            Boolean selectedfound = foundFirst;
            foreach (TreeNode tn in srcNodes.Nodes)
            {
                if (tn.Text.ToUpper().Contains(searchstring) && !selectedfound)
                {
                    TV_Parcels.SelectedNode = tn;
                    TV_Parcels.SelectedNode.EnsureVisible();
                    selectedfound = true;
                    TV_Parcels.Focus();
                    TV_Parcels.Select();
                    return;
                }
                //in childs:
                FindNode(tn, searchstring, selectedfound);
            }
        }

        
                private void SearchTextBox_TextChanged(object sender, EventArgs e)
                {
                    TextBox searchtbox = (TextBox)sender;
                    if (searchtbox.Visible)
                    {   // начинаем с высшей ноды:
                        TV_Parcels.BeginUpdate();
                        FindNode(TV_Parcels.Nodes[0], searchtbox.Text.ToUpper(),false);
                        SearchTextBox.Focus();
                        TV_Parcels.EndUpdate();
                    }

                }
        
        
        /*
          // Случай #2 для поиска сразу ноды
        private TreeNode FindNode2(TreeNode srcNodes, string searchstring, bool foundFirst)
        {
            if (searchstring == "") return null;
            Boolean selectedfound = foundFirst;
            foreach (TreeNode tn in srcNodes.Nodes)
            {
                if (tn.Text.ToUpper().Contains(searchstring) && !selectedfound)
                {
                    selectedfound = true;
                    return tn;
                }
                //in childs:
              return  FindNode2(tn, searchstring, selectedfound);
            }
            return null;
        }
        
        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox searchtbox = (TextBox)sender;
            if (searchtbox.Visible)
            {   // начинаем с высшей ноды:
                TV_Parcels.BeginUpdate();

                TreeNode res = FindNode2(TV_Parcels.Nodes[0], searchtbox.Text.ToUpper(), false);
                if (res != null)
                {

                    TV_Parcels.SelectedNode = res;
                    TV_Parcels.SelectedNode.EnsureVisible();
                }
                SearchTextBox.Focus();
                TV_Parcels.EndUpdate();
            }
        }
        
            */

        // Возникает только если текст не пустой
        /*
          private void SearchTextBox_TextChanged(object sender, EventArgs e)
          {
              TextBox searchtbox = (TextBox)sender;
              if (searchtbox.Visible)
              {   // начинаем с высшей ноды:
                  TV_Parcels.BeginUpdate();
                  int res = netFteo.TreeViewFinder.SearchNodes(TV_Parcels.Nodes[0], searchtbox.Text.ToUpper());

                  if (res != -1)
                  {
                      TV_Parcels.CollapseAll();
                      TV_Parcels.SelectedNode = TV_Parcels.Nodes[res];
                      TV_Parcels.SelectedNode.Expand();
                      TV_Parcels.SelectedNode.EnsureVisible();

                  }
                  else
                  {
                      TV_Parcels.SelectedNode = TV_Parcels.Nodes[0];
                      TV_Parcels.SelectedNode.EnsureVisible();
                      TV_Parcels.CollapseAll();
                  }

                  SearchTextBox.Focus();
                  TV_Parcels.EndUpdate();

              }

          }

          */

        private bool SearchTextBox_Toggle(TextBox sender)
        {
            if (!sender.Visible)
            {
                sender.Visible = true;
                sender.Clear();
                sender.Focus();
                return true;
            }
            else
            {
                sender.Visible = false;
                return false;
            }
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox searchtbox = (TextBox)sender;
            //Прячем контрол:
            /*
            if ((e.Control) && (e.KeyValue == 70))
            {
               if (Toggle_SearchTextBox(searchtbox)) TV_Parcels.Focus();
                e.SuppressKeyPress = true;
            }
            */
            //Ищем текст:
            if (searchtbox.Text == "")
                TV_Parcels.SelectedNode = TV_Parcels.Nodes[0]; // если пусто, возвращаем в начало
        }

    

    

        private void поискToolStripMenuItem_Click(object sender, EventArgs e)
        {
            поискToolStripMenuItem.Checked = SearchTextBox_Toggle(SearchTextBox);
        }

        private void validateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DocInfo.FilePath != null)
            {
                frmValidator frmvldtr = new frmValidator();

                frmvldtr.xmlToValide = DocInfo.FilePath; //DocInfo.FileName;
                frmvldtr.ShowDialog(this);
            }
        }

        //Обработчик события OnChecking
        private void ESCheckerStateUpdater(object Sender, ESCheckingEventArgs e)
        {
            int currProc = e.Process;
            /*
            if (e.Definition == "26:01:071402:12")
            {
                int here100 = 1000;
            }
            */
            if (e.Process < toolStripProgressBar1.Maximum)
             toolStripProgressBar1.Value = e.Process;
           // toolStripStatusLabel3.Text = e.Definition;
            this.Update();
        }

        /// <summary>
        /// Проверка топокорректности Пространственных данных 
        /// </summary>
        private void TopoCheck(TreeNode STrN)
        {
            netFteo.Spatial.Point test = new netFteo.Spatial.Point();

            openFileDialog1.Filter = "Про$транственные данные|*.mif";
            openFileDialog1.FileName = XMLReaderCS.Properties.Settings.Default.Recent0;

            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (STrN.Name.Contains("EntrysNode"))
                {
                    Int32 id = Convert.ToInt32(STrN.Name.Substring(10));
                    object O = this.DocInfo.MyBlocks.GetObject(id);
                    if (O.ToString() == "netFteo.Spatial.TMyParcel")
                    {
                        TMyParcel P = (TMyParcel)O;
                        //подключим обработчик события
                        P.CompozitionEZ.OnChecking += new ESCheckingHandler(ESCheckerStateUpdater);

                        netFteo.IO.TextReader mifreader = new netFteo.IO.TextReader();
                        TPolygonCollection polyfromMIF =  mifreader.ImportMIF(openFileDialog1.FileName);

                        toolStripProgressBar1.Maximum = P.CompozitionEZ.Count *  polyfromMIF.Count;
                        toolStripProgressBar1.Minimum = 0;
                        toolStripProgressBar1.Value = 0;

                        PointList res = new PointList();
                      
                        res.AppendPoints(P.CompozitionEZ.CheckESs(polyfromMIF));

                        //Если пересечения не найдены - то общие точки:
                        if (res.PointCount == 0)
                        { PointList resCommon = new PointList();
                           // resCommon.AppendPoints(P.CompozitionEZ.CheckCommon(polyfromMIF));
                            //Если есть общие точки - возможны накрытия через узлы ! 
                        }

                        if (res.PointCount > 0)
                        {

                            TMyPolygon poly = new TMyPolygon("intersections");
                            poly.AppendPoints(res);

                            netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                            saveFileDialog1.FilterIndex = 1; // mif
                            {
                                if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                                {
                                    TR.SaveAsmif(saveFileDialog1.FileName, poly);
                                }
                            }


                            if (toolStripMI_ShowES.Checked)
                            {
                                if (res.PointCount == 0)
                                    ViewWindow.Spatial = null;
                                else
                                    ViewWindow.Spatial = poly;

                                ViewWindow.label2.Content = res.Parent_Id.ToString();
                                ViewWindow.BringIntoView();
                                ViewWindow.CreateView(poly);
                            }
                        }
                        


                    }
                }





                /*
                TMyPolygon Poly1 = new TMyPolygon("One");
            Poly1.AddPoint(new Point(   0, 0, "11"));
            Poly1.AddPoint(new Point(1000, 0, "12"));
            Poly1.AddPoint(new Point(1000, 1000, "13"));
            Poly1.AddPoint(new Point(   0, 1000, "14"));

            TMyPolygon Poly2 = new TMyPolygon("Second");
            Poly2.AddPoint(new Point( 500, 500, "21"));
            Poly2.AddPoint(new Point(1500, 500, "22"));
            Poly2.AddPoint(new Point(1500, 1500, "23"));
            Poly2.AddPoint(new Point( 500, 1500, "24"));

            TMyPolygon res = new TMyPolygon("Sect test unit #1");
            res.AppendPoints( Poly1.FindSectES(Poly2));

            int cchk = res.PointCount;
            */

            }
        }

        private void проверкаГеометрииToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
            TopoCheck(TV_Parcels.SelectedNode);
            
        }

        private void toolStripButton2_Click_2(object sender, EventArgs e)
        {
            TopoCheck(TV_Parcels.SelectedNode);
        }

        #region Пример использования mitab.dll (из mitab)
        public void OpenMif(string FileName)
        {
            toolStripStatusLabel1.Text = FileName;
            Ptr = EBop.MapObjects.MapInfo.MiApi.mitab_c_open(FileName);
            string CorrdsSys = EBop.MapObjects.MapInfo.MiApi.mitab_c_get_mif_coordsys(Ptr);
            int feature_count = EBop.MapObjects.MapInfo.MiApi.mitab_c_get_feature_count(Ptr);
            int field_count = EBop.MapObjects.MapInfo.MiApi.mitab_c_get_field_count(Ptr);

            richTextBox1.AppendText("\nПоля данных: " + Convert.ToString(field_count) + "\n");
            for (int i = 0; i <= field_count - 1; i++)
            {
                //    richTextBox1.AppendText(EBop.MapObjects.MapInfo.MiApi.mitab_c_get_field_name(Ptr, i) + " ");
                //    comboBox1.Items.Add(EBop.MapObjects.MapInfo.MiApi.mitab_c_get_field_name(Ptr, i));
            }


            richTextBox1.AppendText("\n Feature_count:" + Convert.ToString(feature_count) + "\n");

            for (int i = 1; i <= EBop.MapObjects.MapInfo.MiApi.mitab_c_get_feature_count(Ptr); i++)
            {
                //  IntPtr Feature = EBop.MapObjects.MapInfo.MiApi.mitab_c_read_feature(Ptr, i);
                //  FeatureslistBox.Items.Add(EBop.MapObjects.MapInfo.MiApi.mitab_c_get_field_as_string(Feature, 0));

            }

        }
        #endregion


        private void KVZU_Form_Load(object sender, EventArgs e)
        {
            if ((int)this.Tag == 3) // load from NET application
            {
                this.Text = "XMl Reader в составе приложения";
                this.ShowInTaskbar = true;
            }
            else
            {  // load as standalone application with args in cli:
                this.Text = "XMl Reader для файлов Росреестра @2015 Fixosoft";
                this.ShowInTaskbar = true;
                args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                {
                    //string Test = Path.GetDirectoryName(args[0]) + "\\" + args[2];
                    toolStripStatusLabel3.Text = args[1];
                    string TestFileName = args[1];
                    //if (args[2] == "open")
                    if (File.Exists(TestFileName))
                        Read(TestFileName, false);
                }
                //No command line args[]
                else toolStripStatusLabel3.Text = "Нет аргументов";
            }
            //anyway - MyBlocks must be exist at this point:
            ListMyCoolections(this.DocInfo.MyBlocks, this.DocInfo.MifPolygons);
            ListFileInfo(DocInfo);
#if (DEBUG)
            this.Text += "/DEBUG {2018}";

            debugToolStripMenuItem.Visible = true;
            картапланToolStripMenuItem.Visible = true;
            сКПТToolStripMenuItem.Visible = true;
            //TMyPoints test = new TMyPoints();
            //test.PoininTest();
#else
            debugToolStripMenuItem.Visible = false;
            картапланToolStripMenuItem.Visible = false;
            сКПТToolStripMenuItem.Visible = false;
#endif
            this.TextDefault = this.Text;
            ClearFiles();
        }



        private void TV_Parcels_DoubleClick(object sender, EventArgs e)
        {
            if (TV_Parcels.SelectedNode.Name.Contains("SPElem"))
            {
                Toggle_Visualizer();
            }
        }

        private void KVZU_Form_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void KVZU_Form_DragDrop(object sender, DragEventArgs e)
        {
            this.DraggedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            
            foreach(string filename in DraggedFiles)
              {
                try
                {
                    Read(filename, true);
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    ClearControls();
                   TreeNode errNode=  TV_Parcels.Nodes.Add("Error in " + Path.GetFileName(filename));
                    errNode.Nodes.Add(ex.Message);
                    TV_Parcels.TopNode.Expand();
                    return;
                }
              }
        }

        private void ававааToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void SetMT_MenuItem_Click(object sender, EventArgs e)
        {
            TMyPolygon Pl = (TMyPolygon)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(listView1.Tag));
            if (Pl != null)
            {
                Pl.SetMT(0.1); 
                PointListToListView(listView1, Pl);
            }
        }

        private void округлитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TMyPolygon Pl = (TMyPolygon)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(listView1.Tag));
            if (Pl != null)
            {
                Pl.Fraq("0.00");
                PointListToListView(listView1, Pl);
            }
        }

        private void перенумероватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TMyPolygon Pl = (TMyPolygon)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(listView1.Tag));
            if (Pl != null)
            {
                Pl.ReorderPoints(1);
                PointListToListView(listView1, Pl);
            }
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            var test = (ListView)sender;
            if (test.SelectedItems.Count == 1)
            {
                
                if (test.SelectedItems[0].Tag != null)
                toolStripStatusLabel2.Text = test.SelectedItems[0].Tag.ToString();
            }

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            frmCertificates frmcertificates = new frmCertificates();
            frmcertificates.ShowDialog();
        }
    }
}

