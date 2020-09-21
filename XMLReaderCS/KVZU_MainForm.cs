
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
using netFteo.Cadaster;
using netFteo.Windows;
using RRTypes.pkk5;



namespace XMLReaderCS

{

    public partial class KVZU_Form : Form
    { 
        IntPtr Ptr;
        string TextDefault; // Текст заголовока по умолчанию

        MyWindowEx ESwindow;
        netFteo.EntityViewer ViewWindow; // xaml WPF control

        /// <summary>
        /// Current file properies
        /// </summary>
        public netFteo.IO.FileInfo DocInfo = new netFteo.IO.FileInfo();
        ZipFile zip;
        //netFteo.IO.MRU MRU = new netFteo.IO.MRU(RecentFilesMenuItem, FixosoftKey + "\\MRU", mru);		//public string FileName;
        //public string FilePath;        
        string pathToHtmlFile;
        string hrefToXSLT;
        string AppConfiguration;
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
        /// <summary>
        /// Классификатор видов использования земель
        /// </summary>
        netFteo.XML.XSDFile dutilizations_v01;
        /// <summary>
        /// Классификатор видов разрешенного использования земельных участков
        /// </summary>
        netFteo.XML.XSDFile dAllowedUse_v02;
        netFteo.XML.XSDFile dRegionsRF_v01;
        netFteo.XML.XSDFile dCategories_v01;
        netFteo.XML.XSDFile dStates_v01;
        netFteo.XML.XSDFile dWall_v01;
        netFteo.XML.XSDFile dLocationLevel1_v01;//.xsd;
        string dLocationLevel2_v01;
        public string MP_06_schema;

        const string FixosoftKey = "HKEY_CURRENT_USER\\Software\\Fixosoft\\GKNData\\NET";

        //Импорт библиотеки
        [DllImport("ESViewer_lib_mcvc.dll")]
        public static extern void Function1(int id);

        // c++ Fteo 6 library
        [DllImport("ESChecker.dll")]
        unsafe public static extern void* Func2(int id);

        [DllImport("ESChecker.dll")]
        unsafe public static extern int TestLibrary();

        /*
        // c++ CodeBlocks library
        [DllImport("ESlib.dll")]
        public static extern void Function2(int id);
        */

        //Конструктор:
        public KVZU_Form()
        {
            InitializeComponent();
            this.Tag = 1; // "Как приложение"
            ClearControls();
            this.Folder_AppStart = Application.StartupPath;
            this.Folder_XSD = Folder_AppStart + "\\Schema";
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

            const string keyName_LastDir = FixosoftKey + "\\Recent";
            string test_LastDir = (string)Microsoft.Win32.Registry.GetValue(keyName_LastDir, "LastDir", 0);
            //if (Microsoft.Win32.Registry.GetValue(keyName, "LastDir", 0) == )
            ESwindow = new MyWindowEx(); //System.Windows.Window();// Окно визуализации ПД
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
        private void DXFStateUpdater(object Sender, netDxf.DXFParsingEventArgs e)
        {
            if (e.Max < Int32.MaxValue)
            {
                //	toolStripProgressBar1.Maximum = Convert.ToInt32(e.Max);
                if (e.Process < toolStripProgressBar1.Maximum)
                    toolStripProgressBar1.Value = Convert.ToInt32(e.Process);
            }
            else
            {
                //	toolStripProgressBar1.Maximum = Convert.ToInt32(e.Max/1024);
                if (e.Process < toolStripProgressBar1.Maximum)
                    toolStripProgressBar1.Value = Convert.ToInt32(e.Process / 1024);
            }
            //toolStripLabel_Counts.Text = toolStripProgressBar1.Value.ToString() + "/" + e.CurrentSection;
            Application.DoEvents();
            this.Update();
        }

        //Обработчик события On...Parsing
        private void XMLStateUpdater(object Sender, ESCheckingEventArgs e)
        {
            int currProc = e.Process;
            toolStripProgressBar1.Maximum = e.Max;
            if (e.Process < e.Max)
                toolStripProgressBar1.Value = e.Process;
            this.Update();
        }



        private void XMLStartUpdater(object Sender, ESCheckingEventArgs e)
        {
            toolStripProgressBar1.Maximum = e.Max;
            this.Update();
        }


        #region Открываем Файл (слава http://code.google.com/p/xsd-to-classes/wiki/Usage ):
        /// <summary>
        /// Открыть файл xml
        /// </summary>
        private void OpenFile(int FilterIndex)
        {
            openFileDialog1.Filter = "Сведения ЕГРН, ТехПлан, Межевой план|*.xml;*.zip;" +
                "|Про$транственные данные|*.dxf;*.mif;*.txt" +
                "|Технокад|*.csv" +
                "|Файл подписи|*.sig" +
                "|Mapinfo table file|*.tab" +
                "|XML schema|*.xsd";
            openFileDialog1.FilterIndex = FilterIndex;
            openFileDialog1.FileName = XMLReaderCS.Properties.Settings.Default.Recent0;
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                Read(openFileDialog1.FileName, true);

        }



        private void ClearControls()
        {
            richTextBox1.Clear();
            listView1.Controls.Clear();
            listView1.Items.Clear();
            listView_Properties.Items.Clear();
            contextMenuStrip_SaveAs.Enabled = false;
            listView_Contractors.Items.Clear();
            cXmlTreeView2.Clear();
            TV_Parcels.ImageIndex = imList_dStates.Images.Count;
            if (this.DocInfo == null)
                this.DocInfo = new netFteo.IO.FileInfo();

            else
            {
                this.DocInfo.FileName = null;
                this.DocInfo.FilePath = null;
                this.DocInfo.Comments = null;
                this.DocInfo.Version = null;
                this.DocInfo.Number = null;
                this.DocInfo.Date = null;
                this.DocInfo.MyBlocks.Blocks.Clear();
            }
            TV_Parcels.Nodes.Clear();

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
            textBox_DocNum.Image = XMLReaderCS.Properties.Resources.calendar_view_day;
            PreloaderMenuItem.LoadingCircleControl.Active = false;
            PreloaderMenuItem.Visible = false;
            PreloaderMenuItem.BackColor = Color.Transparent;
            документToolStripMenuItem.Enabled = false;
            pathToHtmlFile = null;
            this.Text = TextDefault;
            toolStripProgressBar1.Value = 0;
            Gen_id.Reset();

#if (DEBUG)
            debugToolStripMenuItem.Enabled = true;

#else
            debugToolStripMenuItem.Enabled = false;
         //   проверкаГеометрииToolStripMenuItem.Enabled = false;
#endif
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
                toolStripStatusLabel1.Text = "not exist:" + Path.GetFileName(FileName);
                return;
            }

            ClearControls();
            PreloaderMenuItem.LoadingCircleControl.Active = true;
            PreloaderMenuItem.Visible = true;
            Application.DoEvents();
            SaveLastDir(Path.GetDirectoryName(FileName));
            if (File.Exists(FileName + "~.html")) File.Delete(FileName + "~.html"); // если есть предыдущий сеанс
            label_FileSize.Text = FileSizeAdapter.FileSizeToString(FileName);
            this.DocInfo.FileName = Path.GetFileName(FileName);
            this.DocInfo.FilePath = Path.GetFullPath(FileName);
            this.DocInfo.FileSize = FileSizeAdapter.FileSize(FileName)/1024; // kBytes
            this.Text = DocInfo.FileName;
            tabPage5.Text = DocInfo.FileName;
            linkLabel_FileName.Text = DocInfo.FileName;
            toolStripStatusLabel1.Text = DocInfo.FileName;

            // got mif file:
            if (Path.GetExtension(FileName).ToUpper().Equals(".MIF"))
            {
                netFteo.IO.MIFReader mifreader = new netFteo.IO.MIFReader(FileName);
                RRTypes.CommonParsers.Doc2Type parser = new RRTypes.CommonParsers.Doc2Type();// (dutilizations_v01, dAllowedUse_v02);
                mifreader.OnParsing += XMLStateUpdater;
                this.DocInfo = parser.ParseMIF(this.DocInfo, mifreader);
            }

            if (Path.GetExtension(FileName).ToUpper().Equals(".TAB"))
            {
                OpenMiTab(FileName);
            }

            // got AutoCad Drawing Exchange Format -  dxf file:
            if (Path.GetExtension(FileName).ToUpper().Equals(".DXF"))
            {
                string Body = "";
                try
                {
                    netFteo.IO.DXFReader dxfreader = new netFteo.IO.DXFReader(FileName);
                    Body = dxfreader.Body;
                    this.DocInfo.Number = "Encoding  " + dxfreader.BodyEncoding.EncodingName;
                    toolStripProgressBar1.Maximum = dxfreader.BodyLinesCount;
                    toolStripProgressBar1.Minimum = 0;
                    toolStripProgressBar1.Value = 0;
                    dxfreader.dxfFile.OnReaderRead += DXFStateUpdater;
                    RRTypes.CommonParsers.Doc2Type parser = new RRTypes.CommonParsers.Doc2Type(this.dutilizations_v01, dAllowedUse_v02);
                    this.DocInfo = parser.ParseDXF(this.DocInfo, dxfreader);
                }

                catch (ArgumentException err)
                {
                    ClearControls();
                    this.DocInfo.DocTypeNick = "dxf";
                    this.DocInfo.CommentsType = "DXF error...";
                    this.DocInfo.Comments = "\r" + FileName + " contain errors:\r" + err.Message + "\r DXF file body:\r\r" + Body;
                    this.DocInfo.DocType = "dxf";
                }
            }

            if (Path.GetExtension(FileName).ToUpper().Equals(".TXT"))
            {
                netFteo.IO.TextReader mifreader = new netFteo.IO.TextReader(FileName);
                DocInfo = mifreader.ImportTxtFile(FileName);
   
                 //   DocInfo.MyBlocks.ParsedSpatial.Clear();
                   // DocInfo.MyBlocks.ParsedSpatial = polyfromMIF;// not Add, need assume to update Layers

                if (mifreader.isNikonRaw(FileName))
                {
                    //this.DocInfo.DocTypeNick = "Nikon RAW data format V2.00";
                    //call Traverser:
                    Traverser.TraverserMainForm frmTraverser = new Traverser.TraverserMainForm();
                    frmTraverser.ReadRawFile(FileName);
                    frmTraverser.ShowDialog(this);
                }
            }


            if (Path.GetExtension(FileName).ToUpper().Equals(".CSV"))
            {
                netFteo.IO.TextReader CSVReader = new netFteo.IO.TextReader(FileName);
                CSVReader.OnParsing += XMLStateUpdater;
                TEntitySpatial ES_from_CSV = CSVReader.ImportCSVFile();

                if (ES_from_CSV != null)
                {
                    DocInfo.MyBlocks.ParsedSpatial.Clear();
                    DocInfo.MyBlocks.ParsedSpatial = ES_from_CSV;
                    this.DocInfo.DocTypeNick = "Текстовый файл";
                    this.DocInfo.CommentsType = "CSV";
                    this.DocInfo.Comments = CSVReader.Body;
                    this.DocInfo.Encoding = CSVReader.BodyEncoding.EncodingName;
                    this.DocInfo.Number = "Текстовый файл CSV ,  " + CSVReader.BodyEncoding.EncodingName;
                }
            }

            if (Path.GetExtension(FileName).Equals(".xml"))
            {

                Stream ms = new MemoryStream(File.ReadAllBytes(FileName));
                Read(ms);
                ms.Dispose();
                DocInfo.FileName = FileName;

#if (DEBUG)
                //LV_SchemaDisAssembly.Visible =  TODO...;                      
#else
                //LV_SchemaDisAssembly.Visible = false;                      
#endif
                this.DocInfo.MyBlocks.SpatialData.Definition = Path.GetFileName(FileName);
                saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(netFteo.StringUtils.ReplaceComma(this.DocInfo.MyBlocks.SpatialData.Definition));
                //На пся крев просидел два дня....  SaveOpenedFileInfo(DocInfo, FileName);
            }

            // Got XSD schema file
            if (Path.GetExtension(FileName).Equals(".xsd"))
            {
                netFteo.XML.XSDFile xsdenum = new netFteo.XML.XSDFile(FileName);
            }

            // We recieve archive package GKUOKS, GKUZU:
            if (Path.GetExtension(FileName).Equals(".zip"))
                /*if ((FileName.Contains("GKU"))  ||
                    (FileName.Contains("SchemaParcels"))) */
                {
                    ClearFiles();
                    BackgroundWorker w1 = new BackgroundWorker();
                    w1.WorkerSupportsCancellation = false;
                    w1.WorkerReportsProgress = true;
                    w1.DoWork += this.UnZipit;
                    w1.RunWorkerCompleted += this.UnZipComplete;
                    w1.RunWorkerAsync(FileName);
                }



            // file is signature
#if DEBUG
            if (Path.GetExtension(FileName).Equals(".sig"))
            {
                /*
                cspUtils.CadesWrapper cwrp = new cspUtils.CadesWrapper();
                cwrp.DisplaySig(FileName, this.Handle);
                */
            }
#endif
            //Если есть парная ЭЦП:
            if (File.Exists(FileName + ".sig"))
            {
                frmCertificates certfrm = new frmCertificates();
                List<string> sigs = certfrm.ParseSignature(FileName + ".sig");
                if (sigs != null)
                    foreach (string sig in sigs)
                        textBox_FIO.Text += "\n ЭЦП= " + sig;
            }

            if (NeedListing)
            {
                ListMyCoolections(this.DocInfo.MyBlocks);
                ListFileInfo(DocInfo);
            }
            PreloaderMenuItem.LoadingCircleControl.Active = false;
            PreloaderMenuItem.Visible = false;
            документToolStripMenuItem.Enabled = true;
        }


        /// <summary>
        ///   Read, construct XMLDocument  and parse them of all types of xml documents
        /// </summary>
        /// <param name="xmldoc">XML Document</param>
        public void Read(Stream fs)
        {

            fs.Seek(0, 0);

            //DataSet dsXmlFile = new DataSet();
            //fs.Seek(0, 0);
            //dsXmlFile.ReadXml(fs); //Dataset consumpt memory at 2 times than XMLDocument

            /*
            webBrowser1.Navigate(DocInfo.FilePath);
            webBrowser1.Visible = true;
            */

            // First show xml, before parsing... :)
            /*
            if (DocInfo.FileSize < 32768)
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(fs);
                cXmlTreeView2.LoadXML(DocInfo.FileName, fs); // Загрузим тело в дерево XMlTreeView - собственный клас/компонент, умеющий показывать XmlDocument
                xmldoc = null;
            }
            */
            cXmlTreeView2.LoadXML(DocInfo.FileName, fs); // Загрузим тело в дерево XMlTreeView - собственный клас/компонент, умеющий показывать XmlDocument
            fs.Seek(0, 0);
            RRTypes.CommonParsers.ParserCommon.dAllowedUse_v02 = dAllowedUse_v02;
            RRTypes.CommonParsers.ParserCommon.dutilizations_v01 = dutilizations_v01;
            DocInfo = RRTypes.CommonParsers.ParserCommon.ParseXMLDocument(fs);
            fs.Dispose();
            GC.Collect();
        }
    

        private void InitSchemas(string folder_xsd)
        {
            dutilizations_v01 = new netFteo.XML.XSDFile(folder_xsd + "\\SchemaCommon" + "\\dUtilizations_v01.xsd");
            dAllowedUse_v02 = new netFteo.XML.XSDFile(folder_xsd + "\\SchemaCommon" + "\\dAllowedUse_v02.xsd");
            dRegionsRF_v01 = new netFteo.XML.XSDFile(folder_xsd + "\\SchemaCommon" + "\\dRegionsRF_v01.xsd");
            dCategories_v01 = new netFteo.XML.XSDFile(folder_xsd + "\\SchemaCommon" + "\\dCategories_v01.xsd");
            dStates_v01 = new netFteo.XML.XSDFile(folder_xsd + "\\SchemaCommon" + "\\dStates_v01.xsd");
            dWall_v01 = new netFteo.XML.XSDFile(folder_xsd + "\\SchemaCommon" + "\\dWall_v01.xsd");
            dLocationLevel1_v01 = new netFteo.XML.XSDFile(folder_xsd + "\\SchemaCommon" + "\\dLocationLevel1_v01.xsd");
            dLocationLevel2_v01 = folder_xsd + "\\SchemaCommon" + "\\dLocationLevel2_v01.xsd";
            MP_06_schema = folder_xsd + "\\V06_MP" + "\\MP_v06.xsd";
        }


        //Zip utils 
        public void ZipExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Object, ExtractProgressEventArgs>(ZipExtractProgress), new Object[] { sender, e });
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
                        this.toolStripStatusLabel3.Text = "[" + e.EntriesTotal.ToString() + "] " + e.CurrentEntry.FileName;
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
                   //ZipFile  zip_Test = ZipFile.Read((string)e.Argument);
                    //zip_Test.AlternateEncoding

                    ReadOptions ro = new ReadOptions();
                    // if under Win:
                    Encoding win1251 = Encoding.GetEncoding("windows-1251");
                    Encoding dos866 = Encoding.GetEncoding(866);
                    ro.Encoding = dos866;
                    
                    //ro.ReadProgress += ZipReadProgress;



                    zip = ZipFile.Read((string)e.Argument, ro);
                   // zip.AlternateEncoding = dos866;

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
                    if (firstFileName.Contains("SchemaParcels"))
                    {
                        //BugReport_SchemaParcels(ArchiveFolder);
                    }
                    if (firstFileName.Contains("GKUZU"))
                    {
                        BugReport_MP06_II(ArchiveFolder);
                    }
                    //else
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
                this.label1.Text = "Exception: " + ex1.ToString();
                //delay = 4000;
            }

            /*
            var timer1 =  new System.Timers.Timer(delay);
            timer1.Enabled = true;
            timer1.AutoReset = false;
            timer1.Elapsed += this.OnTimerEvent; */
        }



        //------------Запись xml-файла-спутника  pinfo_......xml---------------------------------------------------------------
        private void SaveOpenedFileInfo(netFteo.IO.FileInfo Doc, string FileName)
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

        /*

       private void ParseTPolygon(TPolygon xmlPolygon)
       {
           TCadastralBlock Bl = new TCadastralBlock();
           if (xmlPolygon.Definition != null)
               Bl.CN = xmlPolygon.Definition;
           else Bl.CN = "Polygon";
           TParcel MainObj = Bl.Parcels.AddParcel(new TParcel(xmlPolygon.Definition, "nefteo::TPolygon"));
           MainObj.EntSpat.Add(xmlPolygon);
           this.DocInfo.MyBlocks.Blocks.Add(Bl);
           ListMyCoolections(this.DocInfo.MyBlocks);
       }


       private void ParseTPolygon(TPolygonCollection xmlPolygons)
       {
           TMyCadastralBlock Bl = new TMyCadastralBlock();
           Bl.CN = xmlPolygons.Defintion;// "imported xml polygons";

           for (int i = 0; i <= xmlPolygons.Count - 1; i++)
           {
               TMyParcel MainObj = Bl.Parcels.AddParcel(new TMyParcel(xmlPolygons[i].Definition, "nefteo::TPolygon"));
               MainObj.EntitySpatial = xmlPolygons[i];
           }
           this.DocInfo.MyBlocks.Blocks.Add(Bl);
           ListMyCoolections(this.DocInfo.MyBlocks);
       }
       */

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
                    TPolygon NewCont = MainObj.Contours.AddPolygon(RRTypes.KPZU_v05Utils.AddEntSpatKPZU05(kp.Parcel.CadastralNumber + "(" +
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
                        TPolygon SlEs = RRTypes.KPZU_v05Utils.AddEntSpatKPZU05(kp.Parcel.SubParcels[i].NumberRecord, kp.Parcel.SubParcels[i].EntitySpatial);
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
                TCadastralBlock Bl = new TCadastralBlock(kv.Realty.Building.CadastralBlocks[0].ToString());
                TRealEstate Bld = new TRealEstate(kv.Realty.Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                Bld.Building.AssignationBuilding = kv.Realty.Building.AssignationBuilding.ToString();
                Bld.Name = kv.Realty.Building.Name;
                //Constructions.Address = KPT_v09Utils.AddrKPT09(kv.Realty.Construction.Address);
                Bld.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(kv.Realty.Building.CadastralNumber, kv.Realty.Building.EntitySpatial);
                Bl.AddOKS(Bld);
                //MifOKSPolygons.AddPolygon((TPolygon) Constructions.ES);
                this.DocInfo.MyBlocks.Blocks.Add(Bl);
            }



            if (kv.Realty.Construction != null)
            {
                TCadastralBlock Bl = new TCadastralBlock(kv.Realty.Construction.CadastralBlocks[0].ToString());
                TRealEstate Constructions = new TRealEstate(kv.Realty.Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);

                Constructions.Construction.AssignationName = kv.Realty.Construction.AssignationName;
                Constructions.Name = kv.Realty.Construction.Name;
                Constructions.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(kv.Realty.Construction.CadastralNumber, kv.Realty.Construction.EntitySpatial);
                foreach (RRTypes.kvoks_v02.tOldNumber n in kv.Realty.Construction.OldNumbers)
                    Constructions.Construction.OldNumbers.Add(new TKeyParameter() { Type = n.Type.ToString(), Value = n.Number });
                Bl.AddOKS(Constructions);
                this.DocInfo.MyBlocks.Blocks.Add(Bl);
            }

            ListMyCoolections(this.DocInfo.MyBlocks);
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
                TCadastralBlock Bl = new TCadastralBlock(kv.Realty.Building.CadastralBlocks[0].ToString());
                TRealEstate Bld = new TRealEstate(kv.Realty.Building.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Здание);
                Bld.Building.AssignationBuilding = netFteo.Rosreestr.dAssBuildingv01.ItemToName(kv.Realty.Building.AssignationBuilding.ToString());
                Bld.Name = kv.Realty.Building.Name;
                Bld.Location.Address = RRTypes.CommonCast.CasterOKS.CastAddress(kv.Realty.Building.Address);
                Bld.Area = kv.Realty.Building.Area;
                Bld.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(kv.Realty.Building.CadastralNumber, kv.Realty.Building.EntitySpatial);
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
                //MifOKSPolygons.AddPolygon((TPolygon) Constructions.ES);
                this.DocInfo.MyBlocks.Blocks.Add(Bl);
            }



            if (kv.Realty.Construction != null)
            {
                TCadastralBlock Bl = new TCadastralBlock(kv.Realty.Construction.CadastralBlocks[0].ToString());
                TRealEstate Constructions = new TRealEstate(kv.Realty.Construction.CadastralNumber, netFteo.Rosreestr.dRealty_v03.Сооружение);
                Constructions.Construction.AssignationName = kv.Realty.Construction.AssignationName;
                //Constructions.Address = KPT_v09Utils.AddrKPT09(kv.Realty.Construction.Address);
                Constructions.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(kv.Realty.Construction.CadastralNumber, kv.Realty.Construction.EntitySpatial);
                Constructions.ObjectType = RRTypes.CommonCast.CasterOKS.ObjectTypeToStr(kv.Realty.Construction.ObjectType);
                Bl.AddOKS(Constructions);
                //MifOKSPolygons.AddPolygon((TPolygon) Constructions.ES);
                this.DocInfo.MyBlocks.Blocks.Add(Bl);
            }

            ListMyCoolections(this.DocInfo.MyBlocks);
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

                if (TP.Construction.Package.New_Construction.Count > 0)
                {
                    TCadastralBlock Bl = new TCadastralBlock();
                    TRealEstate Constructions = new TRealEstate(TP.Construction.Package.New_Construction[0].Name, netFteo.Rosreestr.dRealty_v03.Сооружение);
                    Constructions.Construction.AssignationName = TP.Construction.Package.New_Construction[0].Assignation_Name;
                    Constructions.Location.Address.Note = TP.Construction.Package.New_Construction[0].Location.Note;
                    Constructions.EntSpat = RRTypes.CommonCast.CasterOKS.ES_OKS2(TP.Construction.Package.New_Construction[0].Assignation_Name,
                                                                                     TP.Construction.Package.New_Construction[0].Entity_Spatial);
                    Bl.AddOKS(Constructions);
                    this.DocInfo.MyBlocks.SpatialData.AddRange(Constructions.EntSpat);
                    this.DocInfo.MyBlocks.Blocks.Add(Bl);
                }

                ListMyCoolections(this.DocInfo.MyBlocks);

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

        #region Отображение в TreeView Коллекций ЗУ и полигонов (из КВЗУ и КПТ)
        private void ListFileInfo(netFteo.IO.FileInfo fileinfo)
        {
            label_DocType.Text = fileinfo.DocType + " " + fileinfo.Version;// "КПТ + 10";;
            textBox_DocNum.Text = fileinfo.Number;
            if (fileinfo.Date != null)
                textBox_DocDate.Text = fileinfo.Date.ToString();
            textBox_OrgName.Text = fileinfo.Cert_Doc_Organization;
            textBox_Appointment.Text = fileinfo.Appointment;
            textBox_FIO.Text = fileinfo.AppointmentFIO;
            toolStripStatusLabel3.Text = fileinfo.Namespace;
            toolStripStatusLabel2.Text = "<" + fileinfo.DocRootName + "> " + label_FileSize.Text;
            linkLabel_tns.Text = DocInfo.Namespace;

            tabPage1.Text = fileinfo.DocTypeNick + " " + fileinfo.Version;// "КПТ + 10";
            tabPage3.Text = fileinfo.CommentsType;// "Conclusion/Notes";
            linkLabel_Recipient.Text = fileinfo.ReceivName + " " + fileinfo.ReceivAdress;
            linkLabel_Request.Text = fileinfo.RequeryNumber;
            if (fileinfo.Comments != null)
                richTextBox1.AppendText(fileinfo.Comments.Replace("<br>", "\n"));


            foreach (netFteo.Rosreestr.TEngineerOut eng in fileinfo.Contractors)
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

            /*
			if (fileinfo.dxfVAriables != null)
			{
				TV_Parcels.BeginUpdate();
				TreeNode TopNode_ = TV_Parcels.Nodes.Add("dxfVarsNode", "dxf Variables");
				foreach (var keyValue in fileinfo.dxfVAriables.variables)
					TopNode_.Nodes.Add("dxfVariable", keyValue.Key + " = " + keyValue.Value);
				TV_Parcels.EndUpdate();
			}
			*/
        }

        /// </summary>
        /// <param name="kpt09"></param>
        private void ListMyCoolections(TCadastralDistrict BlockList)
        {
            //TreeNode TopNode_ = TV_Parcels.Nodes.Add("TopNode", DocInfo.DocRootName);
            TreeNode TopNode_ = null;
            TV_Parcels.BeginUpdate();
            for (int bc = 0; bc <= BlockList.Blocks.Count - 1; bc++)
            {
                TopNode_ = TV_Parcels.Nodes.Add("TopNode", BlockList.Blocks[bc].CN);
                if (BlockList.Blocks.Count == 1)
                {
                    this.Text = BlockList.Blocks[bc].CN;

                    if (BlockList.Blocks[bc].Parcels.Count == 1)
                    {
                        this.Text = BlockList.Blocks[bc].Parcels[0].CN;
                    }

                    if (BlockList.Blocks[bc].ObjectRealtys.Count == 1)
                    {
                        this.Text = BlockList.Blocks[bc].ObjectRealtys[0].CN;
                    }
                }// DocInfo.DocRootName);}

                //	else { TopNode_ = TV_Parcels.Nodes.Add("TopNode", BlockList.Blocks[bc].CN; }

                if ((BlockList.Blocks[bc].Parcels != null) && (BlockList.Blocks[bc].Parcels.Count > 0))
                {
                    TreeNode ParcelsNode_ = TopNode_.Nodes.Add("ParcelsNode", "Земельные участки");
                    foreach (TParcel Parcel in BlockList.Blocks[bc].Parcels)
                    {
                        TreeNode TnP = ListMyParcel(ParcelsNode_, Parcel);
                        if (BlockList.Blocks[bc].Parcels.Count == 1) TnP.Expand();

                        if (BlockList.Blocks[bc].Parcels.Count == 1)
                        {
                            pkk5Viewer1.Start(BlockList.Blocks[bc].Parcels[0].CN, pkk5_Types.Parcel);
                            ParcelsNode_.Expand();
                        }
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
                if ((BlockList.OMSPoints.AsPointList.Count) > 0)
                {
                    TreeNode OMSNode = TopNode_.Nodes.Add("OMSPoints", "Пункты ОМС");
                    OMSNode.SelectedImageIndex = 5;
                    OMSNode.ImageIndex = 5;
                    ListPointList(OMSNode, BlockList.OMSPoints.AsPointList, 0);
                }

                //ОИПД Квартала
                if (BlockList.Blocks[bc].Entity_Spatial.PointCount != 0)
                {
                    //TreeNode KvEntitytNode = TopNode_.Nodes.Add("OMSPoints", "Границы квартала");
                    //TreeNode KvEntityItemNode = TopNode_.Nodes.Add("SPElem." + BlockList.Blocks[bc].Entity_Spatial.Layer_id, "Границы " + BlockList.Blocks[bc].CN);
                    //netFteo.ObjectLister.ListEntSpat(KvEntityItemNode, BlockList.Blocks[bc].Entity_Spatial);
                    netFteo.ObjectLister.ListEntSpat(TopNode_, BlockList.Blocks[bc].Entity_Spatial, "SPElem.", "Границы ", 6);
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


            } // block

            //ОИПД если нет кварталов - не было импорта ЕГРН, значит только пространственный файл mif, dxf
            if (BlockList.Blocks.Count == 0)
            {
                if (BlockList.ParsedSpatial != null)
                    netFteo.ObjectLister.ListEntSpat(TV_Parcels.Nodes.Add("TopNode"), BlockList.ParsedSpatial);
            }

            if (TopNode_ != null) TopNode_.Expand();
            TV_Parcels.EndUpdate();
            contextMenuStrip_SaveAs.Enabled = true;
        }

        //-------------------------------------------------------------------------------------------------------------------
        private TreeNode ListMyParcel(TreeNode Node, TParcel Parcel)
        {
            if (Parcel.SpecialNote != null)
            {
                tabPage3.Text = "Особые отметки";
                richTextBox1.AppendText(Parcel.SpecialNote);
            }
            else tabPage3.Hide();

            TreeNode PNode = new TreeNode();

            if (Parcel.CN != "")
            {
                PNode = Node.Nodes.Add("PNode" + Parcel.id.ToString(), Parcel.CN);
            }
            else

            if (Parcel.Definition != "")
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

            if (Parcel.CompozitionEZ != null)
            {
                if (Parcel.CompozitionEZ.Count() > 0)
                {
                    TreeNode EntrysNode = PNode.Nodes.Add("EntrysNode" + Parcel.id.ToString(), "Входящие в ЕЗП (" + Parcel.CompozitionEZ.Count().ToString() + ")");
                    for (int i = 0; i <= Parcel.CompozitionEZ.Count - 1; i++)
                    {
                        // TreeNode EntrSNode = PNode.Nodes.Add("EntryNode." + Parcel.CompozitionEZ[i].Layer_id.ToString(), 
                        //                                                   Parcel.CompozitionEZ[i].Definition); 
                        IGeometry spatialofEntry = Parcel.GetEs(Parcel.CompozitionEZ[i].Spatial_ID);
                        netFteo.ObjectLister.ListEntSpat(EntrysNode, (TPolygon)spatialofEntry, "SPElem.", Parcel.CompozitionEZ[i].CN, Parcel.CompozitionEZ[i].State);
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

            else
                netFteo.ObjectLister.ListEntSpat(PNode, Parcel.EntSpat);


            if (Parcel.SubParcels != null)
                if (Parcel.SubParcels.Count > 0)
                {
                    TreeNode Slotsnode = PNode.Nodes.Add("SlotsNode", "Части земельного участка");

                    for (int i = 0; i <= Parcel.SubParcels.Count - 1; i++)
                    {
                        TreeNode SlotNode = Slotsnode.Nodes.Add("SlotNode" + Parcel.SubParcels[i].id, "Часть " + Parcel.SubParcels[i].NumberRecord);
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
                                                                 "Границы", 6);
                            }

                        if (Parcel.SubParcels[i].Contours != null)
                            if (Parcel.SubParcels[i].Contours.Count > 0)
                            {
                                netFteo.ObjectLister.ListEntSpat(SlotNode, Parcel.SubParcels[i].Contours);
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
                        TreeNode InnerCNNode = InnerCNsNode.Nodes.Add("oNode" + i.ToString(), Parcel.PrevCadastralNumbers[i]);
                    }
                }

            ListRights(PNode, Parcel.Rights, Parcel.id, "Права", "Rights");
            ListRights(PNode, Parcel.EGRN, Parcel.id, "ЕГРН", "EGRNRight"); // и права из "приписочки /KPZU/ReestrExtract"
            ListEncums(PNode, Parcel.Encumbrances);
            return PNode;
        }


        private void ListAdress(TreeNode Node, netFteo.Rosreestr.TAddress Address, long id)
        {
            if (Address == null) return;
            if (Address.Empty) return;

            TreeNode Adrs = Node.Nodes.Add("Adrss" + id.ToString(), "Адрес");

            if (Address.Region != null)
                if (dRegionsRF_v01 != null)
                    if (Address.Region != "99") //prevent fake value
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
                TreeNode an = Adrs.Nodes.Add("AdrNote", "Неформализованное описание");
                an.ToolTipText = "Неформализованное описание";
                an.Nodes.Add(Address.Note.Replace("Российская федерация", "РФ.."));
            }


        }

        private void ListOldNumbers(TreeNode node, TKeyParameters oldnumbers)
        {
            if ((oldnumbers != null) &&
                       (oldnumbers.Count > 0))
            {
                TreeNode oldNumsNode = node.Nodes.Add("OldNumsNodes", "Ранее присвоенные номера");
                foreach (TKeyParameter s in oldnumbers)
                    oldNumsNode.Nodes.Add("Number", s.Type).Nodes.Add(s.Value);
                oldNumsNode.ExpandAll();
            }
        }

        private void ListMyOKS(TreeNode Node, TRealEstate oks)
        {
            TreeNode PNode = Node.Nodes.Add("PNode" + oks.id.ToString(), oks.CN);

            if (oks.Building != null)
            {
                PNode.ImageIndex = 2;
                PNode.SelectedImageIndex = 2;

                if (oks.Building.Flats.Count > 0)
                {
                    TreeNode flatsnodes = PNode.Nodes.Add("Flats" + oks.id.ToString(), "Помещения (" + oks.Building.Flats.Count.ToString() + ")");
                    foreach (TFlat s in oks.Building.Flats)
                        flatsnodes.Nodes.Add("FlatItem" + s.id.ToString(), s.CN);
                }

                ListOldNumbers(PNode, oks.Building.OldNumbers);

            }

            if (oks.Construction != null) //.Type == "Сооружение")
            {
                ListOldNumbers(PNode, oks.Construction.OldNumbers);



            }

            if (oks.Uncompleted != null) //.Type == "Сооружение")
            {
                ListOldNumbers(PNode, oks.Uncompleted.OldNumbers);

            }

            if ((oks.Location != null) &&
                (oks.Location.Address != null))
            {
                ListAdress(PNode, oks.Location.Address, oks.id);
            }
            //oks.KeyParameters
            if (oks.Notes != null)
                PNode.Nodes.Add("SpecNotes", "Особые отметки").Nodes.Add("Note", oks.Notes);

            if ((oks.ParentCadastralNumbers != null) && (oks.ParentCadastralNumbers.Count > 0))
            {
                TreeNode flatsnodes = PNode.Nodes.Add("ParentCadastralNumbers" + oks.id.ToString(), "Земельные участки");
                foreach (string s in oks.ParentCadastralNumbers)
                    flatsnodes.Nodes.Add(s, s);
            }

            netFteo.ObjectLister.ListEntSpat(PNode, oks.EntSpat);
            ListRights(PNode, oks.Rights, oks.id, "Права", "Rights");
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
                    netFteo.ObjectLister.ListEntSpat(PNode, Parcel.EntitySpatial, "SPElem.", "Границы", 6);
                }
        }

        private void ListZone(TreeNode Node, TZone Parcel)
        {
            netFteo.ObjectLister.ListZone(Node, Parcel);
        }

        /*
	
		*/

        /*
	// Листинг точек окружностней в ListView
	private ListViewItem CircleToListView(ListView LV, TCircle Circle)
	{
		if (Circle == null) return null;
		LV.Items.Clear();
		LV.Tag = Circle.id;
		LV.Items.Clear();
		LV.Controls.Clear();
		LV.Columns[0].Text = "Имя";
		LV.Columns[1].Text = "x, м.";
		LV.Columns[2].Text = "y, м.";
		LV.Columns[3].Text = "Mt, м.";
		LV.Columns[4].Text = "R";
		LV.Columns[5].Text = "-";
		LV.Columns[6].Text = "-";
		LV.View = View.Details;

		string BName = Circle.Pref + Circle.NumGeopointA + Circle.OrdIdent;
		ListViewItem LVi = new ListViewItem();
		LVi.Text = BName;
		LVi.Tag = Circle.id;
		LVi.SubItems.Add(Circle.x_s);
		LVi.SubItems.Add(Circle.y_s);
		LVi.SubItems.Add(Circle.Mt_s);
		LVi.SubItems.Add(Circle.R.ToString());
		LVi.SubItems.Add(Circle.Description);
		if (Circle.Pref == "н")
			LVi.ForeColor = Color.Red;
		else LVi.ForeColor = Color.Black;
		if (Circle.Status == 6)
			LVi.ForeColor = Color.Blue;

		LV.Items.Add(LVi);
		return LVi;
	}
*/
        /*
		 * 
		 * 	// Листинг отрезков границ в ListView
		private void PointList_asBordersToListView(ListView LV, netFteo.Spatial.TPolygon PList)
		{
			if (PList.Count == 0) return;
			string BName;
			LV.Items.Clear();
			LV.Tag = PList.id;
			for (int i = 0; i <= PList.Count - 2; i++)
			{
				BName = PList[i].Pref + PList[i].NumGeopointA + " - " +
						PList[i + 1].Pref + PList[i + 1].NumGeopointA;
				ListViewItem LVi = new ListViewItem();
				LVi.Text = BName;
				/* LVi.SubItems.Add(PList.Points[i].x_s);
				 LVi.SubItems.Add(PList.Points[i].y_s);
				 LVi.SubItems.Add(PList.Points[i].Mt_s);
				 LVi.SubItems.Add(PList.Points[i].Description);
				 * */
        /*
	   if (PList[i].Pref == "н")
		   LVi.ForeColor = Color.Red;
	   else LVi.ForeColor = Color.Black;
	   LV.Items.Add(LVi);
   }
}
private ListViewItem PointListToListView(ListView LV, PointList PList, bool SetTag)
{
if (PList.Count == 0) return null;
string BName;
LV.BeginUpdate();
//LV.Items.Clear();
//LV.Tag = PList.Parent_Id;
if (SetTag) LV.Tag = PList.id;
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
   if (i == 0) res = LV.Items.Add(LVi);
   else
	   LV.Items.Add(LVi);
}
LV.EndUpdate();
return res; // вернем первую строчку Items
}

// Листинг точек TPolygon в ListView

private ListViewItem PointListToListView(ListView LV, netFteo.Spatial.TPolygon PList)
{
if (PList.Count == 0) return null;
LV.Items.Clear();
LV.Tag = PList.id;
ListViewItem res = PointListToListView(LV, (PointList)PList, true);

for (int ic = 0; ic <= PList.Childs.Count - 1; ic++)
{  //Пустая строчка - разделитель
   ListViewItem LViEmpty_ch = new ListViewItem();
   LViEmpty_ch.Text = "";
   LV.Items.Add(LViEmpty_ch);
   PointListToListView(LV, PList.Childs[ic], false);
}

ListViewItem LViEmpty = new ListViewItem();
LViEmpty.Text = "";
LV.Items.Add(LViEmpty);
return res;
}
*/

        //Листинг точек ПД
        /// <summary>
        /// Draw geometry on ES (EntityViewer )
        /// </summary>
        /// <param name="LV"></param>
        /// <param name="Feature"></param>
        /// <returns></returns>
        private ListViewItem GeometryToSpatialView(ListView LV, IGeometry Feature)
        {
            if (Feature == null)
            {
                if (ViewWindow != null) ViewWindow.Spatial = null; // сотрем картинку (последнюю)
                return null;
            }

            // Visualizer check:
            if (toolStripMI_ShowES.Checked)
            {
                ViewWindow.Spatial = Feature;
                //ViewWindow.label2.Content = poly.Definition;
                ViewWindow.BringIntoView();
                ViewWindow.CreateView(Feature);
            }

            ToolTip tt = new ToolTip();
            // Adding Controls:
            // if (parent_id > 0) // до момента наладки с parent_id будем проверять его наличие
            if (LV.Items.Count > 3)    // если что-то было отображено
            {
                ListViewItem LVi_Commands = null;
                LinkLabel pkk5Label = new LinkLabel();
                pkk5Label.Click += new System.EventHandler(OnPKK5LabelActionClick);
                pkk5Label.Tag = Feature.id; //parent_id; //CN
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
                return LVi_Commands;
            }
            return null;
        }

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
            LV.Columns[3].Text = "Дата рег.";
            LV.Columns[4].Text = "Доля в праве";
            LV.Columns[5].Text = "Особые отметки";
            LV.Columns[6].Text = "Обременения";
            ListToListView(LV, list);
        }

        private void PermittedUsesToListView(ListView LV, List<string> list, string Caption)
        {
            if (list.Count == 0) return;
            LV.Columns[0].Text = "ВРИ " + Caption;
            LV.Columns[1].Text = "-";
            LV.Columns[2].Text = "-";
            LV.Columns[3].Text = "-";
            LV.Columns[4].Text = "-";
            LV.Columns[5].Text = "-";
            List<string> res = new List<string>();
            //foreach (string s in list)
            //	res.Add(this.dutilizations_v01.Item2Annotation(s));
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





        private void LongTextToListView(ListView LV, string Text, string TextTitle)
        {
            LV.Columns[0].Text = TextTitle;
            LV.Controls.Clear();
            CRichTextBox rt = new CRichTextBox(Text);
            rt.Dock = DockStyle.Fill;
            rt.ReadOnly = true;
            LV.Controls.Add(rt);
        }


        private void OMSPointsToListView(ListView LV, PointList list)
        {
            if (list.PointCount == 0) return;
            LV.BeginUpdate();
            LV.Items.Clear();
            LV.Columns[0].Text = "#";
            LV.Columns[1].Text = "Номер";
            LV.Columns[2].Text = "х, м.";
            LV.Columns[3].Text = "y, м.";
            LV.Columns[4].Text = "Тип";
            LV.Columns[5].Text = "Класс";


            List<string> lst = new List<string>();
            for (int i = 0; i <= list.PointCount - 1; i++)
            {
                string flat_string = (i + 1).ToString();
                flat_string += "\t" + list[i].Definition + "\t" +
                                      list[i].x_s + "\t" +
                                      list[i].y_s + "\t" +
                                      list[i].Description + "\t" +
                                      list[i].Code;
                lst.Add(flat_string);
            }
            ListToListView(LV, lst);
            LV.EndUpdate();
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
                string flat_string = "";
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
                    LVi.SubItems.Add(fl.CN + "/  " + fl.PositionInObject.Levels[0].Position.NumberOnPlan);
                }
                else
                {
                    LVi.BackColor = System.Drawing.Color.LightGray;
                    LVi.SubItems.Add("");
                    LVi.SubItems.Add("");
                    LVi.SubItems.Add(fl.CN);
                }
                LVi.SubItems.Add(fl.Area.ToString());
                LVi.SubItems.Add(fl.Location.Address.AsString());//Adress
                if (fl.Location.Address.Other != null)
                    LVi.SubItems.Add(fl.Location.Address.Other);
                if (fl.Location.Address.Note != null)
                    LVi.SubItems.Add(fl.Location.Address.Note);
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
            if (Address == null) return;
            if (Address.Empty) return;
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
                LViAssgn2.SubItems.Add(this.dRegionsRF_v01.Item2Annotation(Address.Region) + " " + Address.AsString());
                LV.Items.Add(LViAssgn2);
                //  }
            }
        }
        private void KeyParametersToListView(ListView LV, TKeyParameters ps)
        {
            if (ps == null) return;
            foreach (TKeyParameter param in ps)
            {
                ListViewItem LViAssgn = new ListViewItem();
                LViAssgn.Text = "Осн. характеристика/ Параметры";
                LViAssgn.SubItems.Add(param.Type + " " + param.Value);
                LV.Items.Add(LViAssgn);
            }
        }

        /// <summary>
        /// Show properties of geometry (agregate and summarize data)
        /// </summary>
        /// <param name="LV"></param>
        /// <param name="Obj"></param>
        private void PropertiesToListView(ListView LV, object Obj)
        {

            if (Obj == null)
                LV.Items.Clear();
            else
            {
                LV.Controls.Clear();
                LV.BeginUpdate();
                if (Obj.ToString() == "netFteo.Cadaster.TParcel")
                {
                    TParcel P = (TParcel)Obj;
                    LV.Items.Clear();
                    LV.Controls.Clear();

                    /*
					//Отрисуем и отлистаем ПД (not for contours):
					GeometryToListView(listView1, P.EntitySpatial);
					*/
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
                    LVNa.SubItems.Add(P.ParcelName);
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


                    if (!P.AreaGKN.Contains("-1"))
                    {
                        ListViewItem LVip = new ListViewItem();
                        LVip.Text = "Площадь ГКН";
                        LVip.SubItems.Add(P.AreaGKN);
                        LVip.SubItems.Add("кв.м.");
                        LV.Items.Add(LVip);
                    }

                    if (P.Category != null)
                    {
                        ListViewItem LViCat = new ListViewItem();
                        LViCat.Text = "Категория";
                        LViCat.SubItems.Add(this.dCategories_v01.Item2Annotation(P.Category));
                        LV.Items.Add(LViCat);
                    }

                    if (P.Utilization.UtilbyDoc != null)
                    {
                        ListViewItem LViPurpDoc = new ListViewItem();
                        LViPurpDoc.Text = "Разр. использование (док)";
                        LViPurpDoc.SubItems.Add(P.Utilization.UtilbyDoc);
                        LV.Items.Add(LViPurpDoc);
                    }

                    if (P.Utilization.UtilizationSpecified)
                    {
                        ListViewItem LViPurp = new ListViewItem();
                        LViPurp.Text = "Разр. использование (кл)";
                        LViPurp.SubItems.Add(this.dutilizations_v01.Item2Annotation(P.Utilization.Untilization));
                        LV.Items.Add(LViPurp);
                    }
                    if (P.CadastralCost > 0)
                    {
                        ListViewItem LViPurp = new ListViewItem();
                        LViPurp.Text = "Кадастровая. стоимость";
                        LViPurp.SubItems.Add(P.CadastralCost.ToString());
                        LV.Items.Add(LViPurp);
                    }

                }

                if (Obj.ToString() == "netFteo.Spatial.TEntitySpatial")
                {
                    TEntitySpatial ES = (TEntitySpatial)Obj;
                    if (ES.PolyArea != -1)
                    {
                        ListViewItem LVip = new ListViewItem();
                        LVip.Text = "Площадь полигонов [" + ES.PolyCount + "]";
                        LVip.SubItems.Add(ES.PolyArea.ToString());
                        LVip.SubItems.Add("кв.м.");
                        LV.Items.Add(LVip);
                    }
                }

                if (Obj.ToString() == "netFteo.Spatial.TBuilding")
                {
                    TBuilding bld = (TBuilding)Obj;
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

                
                if (Obj.ToString() == "netFteo.Cadaster.TRealEstate")
                {
                    TRealEstate P = (TRealEstate)Obj;
                    LV.Items.Clear();

                    ListViewItem LViCN = new ListViewItem();
                    LViCN.Text = "Кадастровый номер";
                    LViCN.SubItems.Add(P.CN);
                    LViCN.SubItems.Add(P.DateCreated);
                    LV.Items.Add(LViCN);
                    AdressToListView(LV, P.Location.Address);

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

                        //TFlats:
                        if (P.Building.Flats != null)
                            if (P.Building.Flats.Count > 0)
                            {
                                //LV.Items.Clear();
                                ListViewItem LViFlats = new ListViewItem();
                                LViFlats.Text = "Помещения (" + P.Building.Flats.Count.ToString() + ")";
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
                    }

                    if (P.Area != 0)
                    {
                        ListViewItem LViAr = new ListViewItem();
                        LViAr.Text = "Площадь";
                        LViAr.SubItems.Add(P.Area.ToString());
                        LV.Items.Add(LViAr);
                    }

                    if (P.ElementsConstruct.Count() == 1)
                    {
                        ListViewItem LViAr = new ListViewItem();
                        LViAr.Text = "Конструкт. элем. - материал стен";
                        LViAr.SubItems.Add(this.dWall_v01.Item2Annotation(P.ElementsConstruct[0].WallMaterial));
                        LV.Items.Add(LViAr);
                    }

                    if (P.Floors != null)
                    {
                        ListViewItem LViFloors = new ListViewItem();
                        LViFloors.Text = "Количество этажей";
                        LViFloors.SubItems.Add(P.Floors);
                        if (P.UndergroundFloors != null)
                            LViFloors.SubItems.Add(" подземных " + P.UndergroundFloors);
                        LV.Items.Add(LViFloors);
                    }


                    GeometryToSpatialView(listView1, P.EntSpat);
                    KeyParametersToListView(LV, P.KeyParameters);

                    if (P.CadastralCost > 0)
                    {
                        ListViewItem LViPurp = new ListViewItem();
                        LViPurp.Text = "Кадастровая. стоимость";
                        LViPurp.SubItems.Add(P.CadastralCost.ToString());
                        LV.Items.Add(LViPurp);
                    }
                }

                //  Если это часть: 
                
                if (Obj.ToString() == "netFteo.Cadaster.TSlot")
                {
                    TSlot P = (TSlot)Obj;
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

                if (Obj.ToString() == "netFteo.Spatial.TPolygon")
                {
                    TPolygon Poly = (TPolygon)Obj;
                    LV.Items.Clear();
                    ListViewItem LVip = new ListViewItem();
                    LVip.Text = "Площадь граф. [1.." + Poly.PointCount.ToString() + "]";
                    LVip.SubItems.Add(Poly.AreaSpatialFmt("#,0.00"));
                    LVip.SubItems.Add("кв.м.");
                    LV.Items.Add(LVip);

                    if (Poly.AreaValue != -1)
                    {
                        ListViewItem LVipG = new ListViewItem();
                        LVipG.Text = "Площадь";
                        LVipG.SubItems.Add(Poly.AreaValue.ToString());
                        LVipG.SubItems.Add("кв.м.");
                        LV.Items.Add(LVipG);
                    }
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

                if (Obj.ToString() == "netFteo.Spatial.TPolyLine")
                {
                    TPolyLine Poly = (TPolyLine)Obj;
                    LV.Items.Clear();
                    ListViewItem LVipP = new ListViewItem();
                    LVipP.Text = "Длина";
                    LVipP.SubItems.Add(Poly.Length.ToString("#,0.00"));
                    LVipP.SubItems.Add("м.");
                    LV.Items.Add(LVipP);
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
                        LVip.SubItems.Add(cEZ.Count.ToString());
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

                /*
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
						/*
											ListViewItem LVip2 = new ListViewItem();
											LVip2.Text = "Площадь";
											LVip2.SubItems.Add(cEZ.AreaSpatialFmt("0.00", true));
											LV.Items.Add(LVip2);

											ListViewItem LVip3 = new ListViewItem();
											LVip3.Text = "Площадь сем.";
											LVip3.SubItems.Add(cEZ.AreaSpecifiedFmt("0.00", true));
											LV.Items.Add(LVip3);

											ListViewItem LVip3d = new ListViewItem();
											LVip3d.Text = "Δ";
											LVip3d.SubItems.Add(cEZ.AreaVariance.ToString("0.00"));
											LVip3d.SubItems.Add("кв.м");
											LV.Items.Add(LVip3d);
											*/
                /*
ListViewItem LVipP = new ListViewItem();
LVipP.Text = "Периметр";
LVipP.SubItems.Add(cEZ.TotalPerimeter.ToString("0.00"));
LVipP.SubItems.Add("м.");
LV.Items.Add(LVipP);

}
}
*/

                if (Obj.ToString() == "netFteo.Cadaster.TZone")
                {
                    LV.Items.Clear();
                    TZone Zn = (TZone)Obj;
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
                    BName = PList[i].Definition;
                }
                else BName = PList[i].Definition + "." + Convert.ToString(InternalNumber);
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
            //clear items in any case:
            listView1.Items.Clear();
            listView1.Controls.Clear();
            listView1.View = View.Details;

            listView_Properties.Items.Clear();
            listView_Properties.Controls.Clear();
            GeometryToSpatialView(listView1, null);

            if (STrN.Name.Contains("ES."))
            {

                IGeometry Entity = (IGeometry)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(STrN.Name.Substring(3)));
                if (Entity != null)
                {
                    GeometryToSpatialView(listView1, Entity);
                    Entity.ShowasListItems(listView1, true);
                    PropertiesToListView(listView_Properties, Entity);
                }
            }

            //Show features by whole layer 
            if (STrN.Name.Contains("Layer."))
            {
                IGeometry Entity = (IGeometry)this.DocInfo.MyBlocks.ParsedSpatial.GetFeatures(STrN.Name.Substring(6));
                if (Entity != null)
                {
                    GeometryToSpatialView(listView1, Entity);
                    Entity.ShowasListItems(listView1, true);
                    PropertiesToListView(listView_Properties, Entity);
                }
            }

            if (STrN.Name.Contains("SPElem."))
            {
                IGeometry Entity = (IGeometry)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(STrN.Name.Substring(7)));
                if (Entity != null)
                {
                    GeometryToSpatialView(listView1, Entity);
                    Entity.ShowasListItems(listView1, true);
                    PropertiesToListView(listView_Properties, Entity);
                }
            }

            if (STrN.Name.Contains("TPolyLine."))
            {
                IGeometry Entity = (IGeometry)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(STrN.Name.Substring(10)));
                if (Entity != null)
                {
                    GeometryToSpatialView(listView1, Entity);
                    Entity.ShowasListItems(listView1, true);
                    PropertiesToListView(listView_Properties, Entity);
                }
            }

            if (STrN.Name.Contains("PointList."))
            {
                IGeometry Entity = (IGeometry)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(STrN.Name.Substring(10)));
                if (Entity != null)
                {
                    GeometryToSpatialView(listView1, Entity);
                    Entity.ShowasListItems(listView1, true);
                    PropertiesToListView(listView_Properties, Entity);
                }
            }

            if (STrN.Name.Contains("Circle.") || STrN.Name.Contains("TPoint."))
            {
                IGeometry Entity = (IGeometry)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(STrN.Name.Substring(7)));
                if (Entity != null)
                {
                    GeometryToSpatialView(listView1, Entity);
                    Entity.ShowasListItems(listView1, true);
                }
            }

            if (STrN.Name.Contains("PNode")) // this is Parcel - list all about stuf
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(5));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                
                if (O.GetType().ToString().Equals("netFteo.Cadaster.TParcel"))
                {
                    TParcel parcel = (TParcel)O;
                    GeometryToSpatialView(listView1, parcel.EntSpat);
                    parcel.EntSpat.ShowasListItems(listView1, true);
                }

                
                if (O.GetType().ToString().Equals("netFteo.Cadaster.TRealEstate"))
                {
                    TRealEstate parcel = (TRealEstate)O;
                    GeometryToSpatialView(listView1, parcel.EntSpat);
                    parcel.EntSpat.ShowasListItems(listView1, true);
                }

                PropertiesToListView(listView_Properties, O);
            }

            if (STrN.Name.Contains("ParcelsNode"))
            {
                IGeometry Entity = (IGeometry)this.DocInfo.MyBlocks.GetParcelsEs();
                if (Entity != null)
                {
                    GeometryToSpatialView(listView1, Entity);
                    Entity.ShowasListItems(listView1, true);
                    PropertiesToListView(listView_Properties, Entity);
                }
            }

            if (STrN.Name.Contains("OKSsNode"))
            {
                IGeometry Entity = (IGeometry)this.DocInfo.MyBlocks.GetRealtyEs();
                if (Entity != null)
                {
                    GeometryToSpatialView(listView1, Entity);
                    Entity.ShowasListItems(listView1, true);
                    PropertiesToListView(listView_Properties, Entity);
                }
            }

            if (STrN.Name.Contains("TopNode"))
            {
                TEntitySpatial TopES = new TEntitySpatial();
                TopES.AddRange(this.DocInfo.MyBlocks.GetRealtyEs());
                TopES.AddRange(this.DocInfo.MyBlocks.GetParcelsEs());
                if (TopES != null)
                {
                    GeometryToSpatialView(listView1, TopES);
                    TopES.ShowasListItems(listView1, true);
                    PropertiesToListView(listView_Properties, TopES);
                }
            }


            if (STrN.Name.Contains("EntrysNode"))
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(10));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                
                if (O.ToString() == "netFteo.Cadaster.TParcel")
                {
                    TParcel P = (TParcel)O;
                    //TODO : EZPEntryListToListView(listView1, P.CompozitionEZ.AsList());
                    GeometryToSpatialView(listView1, P.EntSpat);
                    P.EntSpat.ShowasListItems(listView1, true);
                    PropertiesToListView(listView_Properties, P.CompozitionEZ);
                }
            }

            if (STrN.Name.Contains("Contours"))
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(8));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                if (O != null)
                    if (O.ToString() == "netFteo.Spatial.TPolygonCollection")
                    {
                        netFteo.Spatial.TPolygonCollection P = (netFteo.Spatial.TPolygonCollection)O;
                        EZPEntryListToListView(listView1, P.AsList());
                        PropertiesToListView(listView_Properties, P);
                    }
            }

            if (STrN.Name.Contains("OMSPoints"))
            {
                OMSPointsToListView(listView1, this.DocInfo.MyBlocks.OMSPoints.AsPointList);
            }

            if (STrN.Name.Contains("ZNode"))
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(5));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                if (((TZone)O).TypeName == "Территориальная зона")
                {
                    PermittedUsesToListView(listView1, ((TZone)O).PermittedUses, ((TZone)O).AccountNumber);
                }
                else
                    LongTextToListView(listView1, ((TZone)O).ContentRestrictions, "Ограничения");
                PropertiesToListView(listView_Properties, O);
            }

            if ((STrN.Name.Contains("SpecNotes")) ||
                (STrN.Name.Contains("AdrNote"))
                )

            {
                LongTextToListView(listView1, STrN.Nodes[0].Text, "Особые отметки");
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
                if (O.ToString() == "netFteo.Cadaster.TParcel")
                {
                    TParcel P = (TParcel)O;
                    PropertiesToListView(listView_Properties, P.Rights);
                    if (P.Rights != null) RightsToListView(listView1, P.Rights.AsList());
                }
            }

            if (STrN.Name.Contains("EGRNRight"))
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(9));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                if (O.ToString() == "netFteo.Cadaster.TParcel")
                {
                    TParcel P = (TParcel)O;
                    PropertiesToListView(listView_Properties, P.EGRN);
                    if (P.EGRN != null) RightsToListView(listView1, P.EGRN.AsList());
                }
            }


            if (STrN.Name.Contains("Flats"))
            {
                Int32 id = Convert.ToInt32(STrN.Name.Substring(5));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                
                if (O.ToString() == "netFteo.Cadaster.TRealEstate")
                {
                    TRealEstate P = (TRealEstate)O;
                    if (P.Building != null)
                        if (P.Building.Flats.Count > 0)
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
                
                if (O.ToString() == "netFteo.Cadaster.TFlat")
                {
                    TFlat P = (TFlat)O;
                    if (P != null)
                    {
                        PropertiesToListView(listView_Properties, P);
                        FlatToListView(listView1, P);
                    }
                }
            }
        }


        private enum NodeGeometryTypes
        {
            ES = 0,
            Layer = 1
        }


        public static int GetNodeGeometryID(string GeometryTagID)
        {
            try
            {
                string[] Node = GeometryTagID.Split('.');
                return Convert.ToInt32(Node[1]);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private IGeometry GetNodeGeometry(string GeometryTagID)
        {
            IGeometry Entity = null;
            int id = GetNodeGeometryID(GeometryTagID);
            if (id != -1)
            {
                Entity = (IGeometry)this.DocInfo.MyBlocks.GetEs(id);
                return Entity;
            }
            else return null;
        }

        private bool RemoveGeometryNode(string GeometryTagID)
        {
            IGeometry Entity = GetNodeGeometry(GeometryTagID);

            if (Entity != null)
            {
                return this.DocInfo.MyBlocks.RemoveGeometry(Entity.id);
            }
            return false;
        }

        private void ListRights(TreeNode PNode, netFteo.Rosreestr.TMyRights Rights, long ownerid, string Name, string Nodename)
        {
            if (Rights == null) return;
            if (Rights.Count > 0)
            {
                TreeNode Rnode = PNode.Nodes.Add(Nodename + ownerid.ToString(), Name);

                for (int i = 0; i <= Rights.Count - 1; i++)
                {
                    TreeNode RNameNode = Rnode.Nodes.Add("RightItemNode", Rights[i].Name);
                    if (Rights[i].Type != null)
                        RNameNode.Nodes.Add(Rights[i].Type);
                    if (Rights[i].RegNumber != null)
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

        private void ListEncum(TreeNode Rnode, netFteo.Rosreestr.TMyEncumbrance Encums)
        {
            if (Encums == null) return;
            string EncType = Encums.Type;
            //if (Encums.Type == null) EncType = "Обременение";

            TreeNode RNameNode = Rnode.Nodes.Add("RencNode", (Encums.Type != null ? Encums.Type : "Обременение"));

            if (Encums.Name != null)
                RNameNode.Nodes.Add(Encums.Name);
            if (Encums.Document.DocName != null)
            {
                TreeNode RDocNode = RNameNode.Nodes.Add("Документ: " + Encums.Document.DocName);
                RDocNode.Nodes.Add(Encums.Document.Date);
                RDocNode.Nodes.Add(Encums.Document.Number);
            }

            if (Encums.AccountNumber != null)
                RNameNode.Nodes.Add("Учетный номер " + Encums.AccountNumber);
            if (Encums.RegNumber != null)
            {
                TreeNode RNameRegNode = RNameNode.Nodes.Add("Государственная регистрация");
                RNameRegNode.Nodes.Add(Encums.RegNumber);
                RNameRegNode.Nodes.Add(Encums.RegDate);
            }

            if (Encums.Owners.Count > 0)
            {
                TreeNode RNameOwnNode = RNameNode.Nodes.Add("В пользу");
                for (int io = 0; io <= Encums.Owners.Count - 1; io++)
                    RNameOwnNode.Nodes.Add(Encums.Owners[io].OwnerName);
            }
            if (Encums.DurationStarted != null)
            {
                TreeNode RDurStrN = RNameNode.Nodes.Add("Дата возникновения");
                RDurStrN.Nodes.Add(Encums.DurationStarted);
            }
            if (Encums.DurationStopped != null)
            {
                TreeNode RDurStopN = RNameNode.Nodes.Add("Дата прекращения");
                RDurStopN.Nodes.Add(Encums.DurationStopped);
            }
            if (Encums.DurationTerm != null)
            {
                TreeNode RDurTerm = RNameNode.Nodes.Add("Продолжительность");
                RDurTerm.Nodes.Add(Encums.DurationTerm);
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
                                           Rights[i].RegDate + "\t" +
                                           Rights[i].Desc + "\t" +
                                           Rights[i].ShareText + "\t");

                    for (int io = 0; io <= Rights[i].Owners.Count - 1; io++)
                        writer.WriteLine(Rights[i].Owners[io].OwnerName);

                }

                writer.Close();
            }

        }

        #region Запись в DXF, MIF, TXT 


        private void списокТочекФайлNikonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            //Для отдельного ОИПД выгружаем:
            netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
            saveFileDialog1.FilterIndex = 2; // txt

            TEntitySpatial ES = GetES(TV_Parcels.SelectedNode.Name);

            if (TV_Parcels.SelectedNode.Name.Contains("PointList"))
            {

            }

            if (TV_Parcels.SelectedNode.Name.Contains("SPElem"))
            {
                TPolygon Pl = (TPolygon)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(7)));
                if (Pl != null)
                {
                    saveFileDialog1.FileName = netFteo.StringUtils.ReplaceSlash(Pl.Definition);
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {


                        TR.SaveAsNikon(saveFileDialog1.FileName, Pl.AsPointList());
                    }

                }
            }
            */
            SaveAs("NikonXY", TV_Parcels.SelectedNode.Name);
        }



        //------------------------------------------------------------------------------------------
        private void SaveAs(string Format, string ItemName, int scale = 1000)
        {
            //1. Get ES: 
            TEntitySpatial ES = GetES(ItemName);

            //2. Set Format and write out file
            if (ES != null)

            {
                switch (Format)
                {
                    case "MIF":
                        {
                            netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                            saveFileDialog1.FilterIndex = 1; // mif
                            ES.RemoveParentCN(DocInfo.MyBlocks.SingleCN);
                            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                                TR.SaveAsmif(saveFileDialog1.FileName, ES);
                            break;
                        }

                    case "CSV":
                        {
                            saveFileDialog1.FilterIndex = 4;
                            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                            {
                                netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                                TR.SaveAsCSV(saveFileDialog1.FileName, ES);
                            }
                            break;
                        }

                    case "TXT":
                        {
                            saveFileDialog1.FilterIndex = 2;
                            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                            {
                                netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                                TR.SaveAsFixosoftTXT2018(saveFileDialog1.FileName, ES, Encoding.Unicode);
                            }
                            break;
                        }

                    case "NikonXY":
                        {
                            saveFileDialog1.FilterIndex = 2;
                            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                            {
                                netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                                TR.SaveAsNikon(saveFileDialog1.FileName, ES);
                            }
                            break;
                        }


                    case "DXF":
                        {
                            double ScaleRaduis;
                            switch (scale)
                            {
                                case 33333:
                                    ScaleRaduis = 22.5; //
                                    break;
                                case 10000:
                                    ScaleRaduis = 7.5;
                                    break;
                                case 5000:
                                    ScaleRaduis = 3.75;
                                    break;
                                case 1000:
                                    ScaleRaduis = 0.75;
                                    break;
                                case 500:
                                    ScaleRaduis = 0.375;
                                    break;
                                default:
                                    ScaleRaduis = 0.75; // aka 1:1000 is default scale
                                    break;
                            }
                            netFteo.IO.DXFWriter wr = new netFteo.IO.DXFWriter();
                            saveFileDialog1.FilterIndex = 3;
                            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                            {
                                ES.RemoveParentCN(DocInfo.MyBlocks.SingleCN);
                                wr.SaveAsDxfScale(saveFileDialog1.FileName, ES, ScaleRaduis);
                            }
                            break;
                        }
                }
            }

        }

        private TEntitySpatial GetES(string NodeName)
        {
            if (NodeName == "TopNode")
                return this.DocInfo.MyBlocks.SpatialData;

            if (NodeName.Contains("Contours"))
            {
                TPolygonCollection Pl = (TPolygonCollection)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(NodeName.Substring(8)));
                if (Pl != null)
                {
                    TEntitySpatial res = new TEntitySpatial();
                    res.AddRange(Pl);
                    return res;
                }
            }

            if (NodeName.Contains("EntrysNode"))
            {
                TParcel Lot = (TParcel)this.DocInfo.MyBlocks.GetObject(Convert.ToInt32(NodeName.Substring(10)));
                if (Lot != null)
                {
                    TEntitySpatial res = new TEntitySpatial();
                    res.AddRange(Lot.EntSpat);//  Lot.CompozitionEZ);
                    return res;
                }
            }


            if (NodeName.Contains("SPElem."))
            {
                TPolygon Pl = (TPolygon)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(NodeName.Substring(7)));
                if (Pl != null)
                {
                    TEntitySpatial PC = new TEntitySpatial();
                    PC.Add(Pl);
                    return PC;
                }
            }

            if (NodeName.Contains("TPolyLine."))
            {
                TPolyLine Pl = (TPolyLine)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(NodeName.Substring(10)));
                if (Pl != null)
                {
                    TEntitySpatial PC = new TEntitySpatial();
                    PC.Add(Pl);
                    return PC;

                }
            }
            
            if (NodeName.Contains("PointList."))
            {
                PointList Pl = (PointList)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(NodeName.Substring(10)));
                if (Pl != null)
                {
                    TEntitySpatial PC = new TEntitySpatial();
                    PC.Add(Pl);
                    return PC;
                }
            }

            //Не Все зоны - только территориальные, ==(1) ??
            if (NodeName.Contains("ZonesNode"))
            {
                TPolygonCollection Plc = this.DocInfo.MyBlocks.GetZonesEs(1);
                if (Plc != null)
                {
                    TEntitySpatial PC = new TEntitySpatial();
                    PC.AddRange(Plc);
                    return PC;
                }
            }

            if (NodeName.Contains("OKSsNode"))
            {
                return this.DocInfo.MyBlocks.GetRealtyEs(); // TODO All data
            }

            if (NodeName.Contains("ParcelsNode"))
            {
                return this.DocInfo.MyBlocks.GetParcelsEs(); // TODO All data
            }

            if (NodeName.Contains("ES."))
            {
                var es = this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(NodeName.Substring(3)));
                return (TEntitySpatial)es;
            }



            if (NodeName.Contains("Layer."))
            {
                return (TEntitySpatial)this.DocInfo.MyBlocks.GetEs(NodeName.Substring(6));

            }

            if (NodeName.Contains("OMSPoints"))
            {
                return this.DocInfo.MyBlocks.OMSPoints;
            }
            return null;
        }


        #endregion



        #region Some funcs..

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

            if (!Directory.Exists(this.Folder_XSD))
            {
                Directory.CreateDirectory(Folder_XSD);
            }

            if (!Directory.Exists(this.Folder_XSD + "\\SchemaCommon"))
            {
                Directory.CreateDirectory(Folder_XSD + "\\SchemaCommon");
            }

        }

 
        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        // ********************************************** mif ********************************************
        private void mifКПТToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs("MIF", TV_Parcels.SelectedNode.Name, 500);
            /*
            netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
            saveFileDialog1.FilterIndex = 1; // mif
            {

                if (TV_Parcels.SelectedNode.Name == "TopNode")
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        TR.SaveAsmif(saveFileDialog1.FileName, this.DocInfo.MyBlocks.SpatialData);
                        string test = Path.GetDirectoryName(saveFileDialog1.FileName) + "\\OKS_" + Path.GetFileName(saveFileDialog1.FileName);
						/* TODO
                        TR.SaveAsmif(Path.GetDirectoryName(saveFileDialog1.FileName) + "\\OKS_" + Path.GetFileName(saveFileDialog1.FileName),
                                                      this.DocInfo.MifOKSPolygons);
													  */

            /*
						}
			 * 
					if (TV_Parcels.SelectedNode.Name == "OMSPoints")
					{
						saveFileDialog1.FileName = "ОМС.mif";// +this.DocInfo.MyBlocks.Blocks[0].CN;
						if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
						{

							SaveAsmifOMS(saveFileDialog1.FileName, this.DocInfo.MyBlocks.OMSPoints.AsPointList);

						}
					}

					//Для отдельного ОИПД выгружаем:
					if (TV_Parcels.SelectedNode.Name.Contains("SPElem"))
					{
						TPolygon Pl = (TPolygon)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(7)));
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
				*/
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
            OpenFile(1);// OpenXML_KVZUTyped();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void typedClassesXSD2ClassessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile(1);// OpenXML_KVZUTyped();
        }

        public void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFile(1);// OpenXML_KVZUTyped();
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
            /*
            if (((MouseEventArgs)e).Button == MouseButtons.Left)
              ListSelectedNode(TV_Parcels.SelectedNode);
			*/
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
            Guid testGUID;
            bool validGuid = Guid.TryParse(textBox_DocNum.Text, out testGUID);
            if (validGuid)
                textBox_DocNum.Image = XMLReaderCS.Properties.Resources.tick;
            else
                textBox_DocNum.Image = XMLReaderCS.Properties.Resources.cross;
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
                    builder.AppendLine(sub.Text + "\t");

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
            /*
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
                TPolygon Pl = (TPolygon) this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(7)));
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


		
			if (TV_Parcels.SelectedNode.Name == "TopNode")
                if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                {
                    netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                    TR.SaveAsFixosoftTXT2018(saveFileDialog1.FileName, this.DocInfo.MyBlocks.SpatialData, Encoding.Unicode);
                }

            //"OKSsNode"
            if (TV_Parcels.SelectedNode.Name.Contains("OKSsNode"))
            {
                TEntitySpatial Plc = this.DocInfo.MyBlocks.GetRealtyEs();
                if (Plc != null)
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {

                        netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                        TR.SaveAsFixosoftTXT2018(saveFileDialog1.FileName, Plc, Encoding.Unicode);
                    }
            }
			*/

            SaveAs("TXT", TV_Parcels.SelectedNode.Name);
            if (TV_Parcels.SelectedNode.Name.Contains("Rights"))
            {
                Int32 id = Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(6));
                object O = this.DocInfo.MyBlocks.GetObject(id);
                
                if (O.ToString() == "netFteo.Cadaster.TParcel")
                {
                    TParcel P = (TParcel)O;
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
                if (O.ToString() == "netFteo.Cadaster.TParcel")
                {
                    TParcel P = (TParcel)O;
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
            SaveAs("DXF", TV_Parcels.SelectedNode.Name, 500);
        }
        #endregion



        private void m11000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs("DXF", TV_Parcels.SelectedNode.Name, 1000);
        }

        private void listView_Properties_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void показатьПДToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
            {
                ESwindow.Hide();
                toolStripMI_ShowES.Checked = false;
                toolStripButton_VisualizerToggle.Checked = false;
                просмотрToolStripMenuItem.Checked = false;
                return;
            }


            ESwindow.Title = "Визуализация ПД (WPF)";
            ViewWindow = new netFteo.EntityViewer();
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
            unsafe
            {
                int resTest = TestLibrary();

                //Example for send string
                //[MarshalAs(UnmanagedType.LPStr)]     string filename,
            }
        }



        private void пересеченияMifToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void mSVCESCheckerFunc2Int1975ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unsafe { Func2(1975); }
        }

        private void csvStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs("CSV", TV_Parcels.SelectedNode.Name);
        }

        private void m110000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs("DXF", TV_Parcels.SelectedNode.Name, 10000);
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



            if (File.Exists(DocInfo.FilePath))
            {
                netFteo.XML.XSLWriter XSLwr = new netFteo.XML.XSLWriter();
                pathToHtmlFile = XSLwr.TransformXMLToHTML(DocInfo.FilePath);
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
            Clipboard.SetText(label_DocType.Text + " №" + textBox_DocNum.Text + " от " + textBox_DocDate.Text);
        }


        private int ConvertToInt32(string str)
        {
            if (str.Length < 5) return -1;
            if (str.Substring(1, 4) == "Node") // PNode, ZNode, 
                return
                  Convert.ToInt32(str.Substring(5));
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

            string cn = null;
            //if (TV_Parcels.SelectedNode.Name.Contains("PNode"))
            //{
            Int32 id = ConvertToInt32(TV_Parcels.SelectedNode.Name); // PNode CNNode
            object O = this.DocInfo.MyBlocks.GetObject(id);
            if (O != null)
            {
                if (O.ToString() == "netFteo.Cadaster.TParcel") //Пока только для ЗУ, ПКК5 пока все равно не обрабатывает оксы
                {
                    cn = ((TParcel)O).CN;
                    pkk5Viewer1.Start(cn, pkk5_Types.Parcel);
                    //Here instances for debug without threading:
                    pkk5_Rosreestr_ru srvPKK5 = new pkk5_Rosreestr_ru(256, 256);
                    srvPKK5.Get_WebOnline_th("26:5:23409:50", pkk5_Types.Parcel);

                    RRTypes.FIR.FIR_Server_ru srvFIR = new RRTypes.FIR.FIR_Server_ru();
                    srvFIR.GET_WebOnline_th(cn);


                }
                
                if (O.ToString() == "netFteo.Cadaster.TRealEstate") //далее - добавим ОКС. 
                {
                    cn = ((TRealEstate)O).CN;
                    pkk5Viewer1.Start(cn, pkk5_Types.OKS);//oks
                }

                if (O.ToString() == "netFteo.Cadaster.TFlat") //далее - ОКС. 
                {
                    cn = ((TFlat)O).CN;
                    pkk5Viewer1.Start(cn, pkk5_Types.OKS);//oks
                }

                if (O.ToString() == "netFteo.Cadaster.TZone") //
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
            /*
						  if (TV_Parcels.SelectedNode.Name.Contains("CN")) // исходные зу
						  {
							  cn = TV_Parcels.SelectedNode.Name;
							  pkk5Viewer1.Start(TV_Parcels.SelectedNode.Text, pkk5_Types.Parcel);//Parent
						  }
			*/
            if (TV_Parcels.SelectedNode.Name.Contains("TopNode")) // Квартал
            {
                //Int32 id = Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(5));
                if (this.DocInfo.MyBlocks.Blocks.Count == 1)
                {
                    cn = this.DocInfo.MyBlocks.Blocks[0].CN;
                    pkk5Viewer1.Start(cn, pkk5_Types.Block);
                }
            }

            // otherwise, just string like cadastral numer 
            if (netFteo.StringUtils.isCadastralNumber(TV_Parcels.SelectedNode.Text))
                pkk5Viewer1.Start(TV_Parcels.SelectedNode.Text, pkk5_Types.Parcel);
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
                
                if (O.ToString() == "netFteo.Cadaster.TParcel")
                {
                    GotoPKK5(((TParcel)O).CN);
                }
                
                if (O.ToString() == "netFteo.Cadaster.TRealEstate")
                {
                    GotoPKK5(((TRealEstate)O).CN);
                }

            }
        }

        private void m5000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs("DXF", TV_Parcels.SelectedNode.Name, 5000);
        }

        // Save xml. Точнее сериализовать в XML 
        private void xmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FilterIndex = 5; // xml


            if ((TV_Parcels.SelectedNode.Name == "TopNode") && (this.DocInfo.MyBlocks.SpatialData != null))
                if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                {
                    //Заменить в полигонах CN квартала
                    netFteo.StringUtils.RemoveParentCN(this.DocInfo.MyBlocks.SingleCN, this.DocInfo.MyBlocks.SpatialData);
                    XmlSerializer serializer = new XmlSerializer(typeof(TPolygonCollection));
                    TextWriter writer = new StreamWriter(saveFileDialog1.FileName);
                    serializer.Serialize(writer, this.DocInfo.MyBlocks.SpatialData);
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
                TPolygon Pl = (TPolygon)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(7)));
                if (Pl != null)
                {
                    saveFileDialog1.FileName = netFteo.StringUtils.ReplaceSlash(Pl.Definition);
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(TPolygon));
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
                
                if (O.ToString() == "netFteo.Cadaster.TParcel")
                {
                    TParcel P = (TParcel)O;
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

        private void MRU_MenuItem_Click(object obj, EventArgs evt)
        {
            FileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.InitialDirectory = Environment.CurrentDirectory;
            if (openFileDlg.ShowDialog() != DialogResult.OK)
                return;
            string openedFile = openFileDlg.FileName;

            //Now give it to the MRUManager
            //this.MRU.AddUsedFile(openedFile);

            //do something with the file here
            MessageBox.Show("Through the 'Open' menu item, you opened: " + openedFile);
        }

        private void MRU__recentFileGotClicked_handler(object obj, EventArgs evt)
        {
            string fName = (obj as ToolStripItem).Text;
            if (!File.Exists(fName))
            {
                if (MessageBox.Show(string.Format("{0} doesn't exist. Remove from recent " +
                         "workspaces?", fName), "File not found",
                         MessageBoxButtons.YesNo) == DialogResult.Yes)
                    //	this.MRU.DeleteUsedFile(fName);
                    return;
            }

            //do something with the file here
            MessageBox.Show(string.Format("Through the 'Recent Files' menu item, you opened: {0}", fName));
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
            ToolStripItem rc0 = RecentFilesMenuItem.DropDownItems.Add(XMLReaderCS.Properties.Settings.Default.Recent0);

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

        private void BugReport_MP06(string ArciveFileName)
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
                var Exept = ex1;
            }
            //Read(openFileDialog1.FileName);            

            //  netFteo.IO.TextReader TR = new netFteo.IO.TextReader();

            /*
            if (TV_Parcels.SelectedNode.Name.Contains("SPElem."))
            {
                TPolygon Pl = (TPolygon)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(TV_Parcels.SelectedNode.Name.Substring(7)));
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
                    if (O.ToString() == "netFteo.Cadaster.TParcel")
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
                //Read(ESChecker_MP06frm.MP_v06);
                ESChecker_MP06frm.ShowDialog();
                //ListMyCoolections(this.DocInfo.MyBlocks, this.DocInfo.MifPolygons);
                //ListFileInfo(DocInfo);
            }

            catch (System.Exception ex1)
            {
                DocInfo.CommentsType = "Exception";
                DocInfo.Comments = ex1.Message;
                ListFileInfo(DocInfo);
            }



        }






        private void копироватьToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            List<string> items = new List<string>();
            Control parent = ((ContextMenuStrip)(((ToolStripMenuItem)sender).Owner)).SourceControl;

            foreach (ListViewItem lvit in ((ListView)parent).Items)
            {
                string sub = "";
                //   if (lvit.Text != "")
                {
                    foreach (ListViewItem.ListViewSubItem lv in lvit.SubItems)
                    {
                        if (lv.Text != lvit.Text) // бывает что суб повторяет саму ноду
                            sub += "\t" + lv.Text;
                    }

                    items.Add(lvit.Text + sub);
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
            Clipboard.SetText(linkLabel_Recipient.Text);
        }

        private void linkLabel_Request_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(linkLabel_Request.Text);
        }

        private void textBox_DocDate_Click(object sender, EventArgs e)
        {
            if (textBox_DocDate.Text != "")
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
            toolStripStatusLabel1.Width = (int)Math.Round(this.Width / 2.76);
            toolStripStatusLabel3.Width = (int)Math.Round(this.Width / 2.594);
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOptions frmoptions = new frmOptions();
            frmoptions.dutilizations_v01 = this.dutilizations_v01;
            frmoptions.dAllowedUse_v02 = this.dAllowedUse_v02;
            frmoptions.dRegionsRF_v01 = this.dRegionsRF_v01;
            frmoptions.dCategories_v01 = this.dCategories_v01;
            frmoptions.dStates_v01 = this.dStates_v01;
            frmoptions.dWall_v01 = this.dWall_v01;
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
            SaveAs("DXF", TV_Parcels.SelectedNode.Name, 33333);
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
                    ESwindow.Left = this.Left - 13 + this.Width;
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
                FindNode(TV_Parcels.Nodes[0], searchtbox.Text.ToUpper(), false);
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
        /// Проверка топокорректности Пространственных данных and set spinned point
        /// </summary>
        private void TopoCheck(TreeNode STrN)
        {
             TPoint test = new netFteo.Spatial.TPoint();

            openFileDialog1.Filter = "Про$транственные данные|*.mif";
            openFileDialog1.FileName = XMLReaderCS.Properties.Settings.Default.Recent0;

            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (STrN.Name.Contains("EntrysNode"))
                {
                    Int32 id = Convert.ToInt32(STrN.Name.Substring(10));
                    object O = this.DocInfo.MyBlocks.GetObject(id);
                    if (O.ToString() == "netFteo.Cadaster.TParcel")
                    {
                        TParcel P = (TParcel)O;
                        //подключим обработчик события
                        //TODO P.CompozitionEZ.OnChecking += new ESCheckingHandler(ESCheckerStateUpdater);

                        netFteo.IO.TextReader mifreader = new netFteo.IO.TextReader(openFileDialog1.FileName);
                        // TPolygonCollection polyfromMIF =  mifreader.ImportMIF(openFileDialog1.FileName);

                        //  toolStripProgressBar1.Maximum = P.CompozitionEZ.Count *  polyfromMIF.Count;
                        toolStripProgressBar1.Minimum = 0;
                        toolStripProgressBar1.Value = 0;

                        PointList res = new PointList();

                        // res.AppendPoints(P.CompozitionEZ.CheckESs(polyfromMIF));

                        //Если пересечения не найдены - то общие точки:
                        if (res.PointCount == 0)
                        {
                            PointList resCommon = new PointList();
                            // resCommon.AppendPoints(P.CompozitionEZ.CheckCommon(polyfromMIF));
                            //Если есть общие точки - возможны накрытия через узлы ! 
                        }

                        if (res.PointCount > 0)
                        {

                            TEntitySpatial ES = new TEntitySpatial();
                            ES.AddRange(res);

                            netFteo.IO.TextWriter TR = new netFteo.IO.TextWriter();
                            saveFileDialog1.FilterIndex = 1; // mif
                            {
                                if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                                {
                                    TR.SaveAsmif(saveFileDialog1.FileName, ES);
                                }
                            }


                            if (toolStripMI_ShowES.Checked)
                            {
                                if (res.PointCount == 0)
                                    ViewWindow.Spatial = null;
                                else
                                    ViewWindow.Spatial = ES;

                                //TODO : repair ViewWindow.label2.Content = res.Parent_Id.ToString();
                                ViewWindow.BringIntoView();
                                ViewWindow.CreateView(ES);
                            }
                        }
                    }
                }

                /*
                TPolygon Poly1 = new TPolygon("One");
            Poly1.AddPoint(new Point(   0, 0, "11"));
            Poly1.AddPoint(new Point(1000, 0, "12"));
            Poly1.AddPoint(new Point(1000, 1000, "13"));
            Poly1.AddPoint(new Point(   0, 1000, "14"));

            TPolygon Poly2 = new TPolygon("Second");
            Poly2.AddPoint(new Point( 500, 500, "21"));
            Poly2.AddPoint(new Point(1500, 500, "22"));
            Poly2.AddPoint(new Point(1500, 1500, "23"));
            Poly2.AddPoint(new Point( 500, 1500, "24"));

            TPolygon res = new TPolygon("Sect test unit #1");
            res.AppendPoints( Poly1.FindSectES(Poly2));

            int cchk = res.PointCount;
            */
                TopoCheckSpin(STrN, openFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Проверка Пространственных данных - "Установка ГКН точек"
        /// </summary>
        private void TopoCheckSpin(TreeNode STrN, string FileName)
        {
            if (STrN.Name.Contains("ES."))
            {
                IGeometry Entity = (IGeometry)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(STrN.Name.Substring(3)));
                if (Entity != null)
                    if (Entity.TypeName == "netFteo.Spatial.TEntitySpatial")
                    {
                        //подключим обработчик события
                        netFteo.IO.MIFReader mifreader = new netFteo.IO.MIFReader(FileName);
                        mifreader.OnParsing += XMLStateUpdater;
                        TEntitySpatial polyfromMIF = mifreader.ParseMIF();
                        TEntitySpatial SourceES = (TEntitySpatial)Entity;
                        SourceES.DetectSpins(polyfromMIF);
                        PointList res = new PointList();
                    }
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
        public void OpenMiTab(string FileName)
        {
            TEntitySpatial ES = new TEntitySpatial();
            toolStripStatusLabel1.Text = FileName;
            Ptr = MiApi.mitab_c_open(FileName);
            string CorrdsSys = EBop.MapObjects.MapInfo.MiApi.mitab_c_get_mif_coordsys(Ptr);
            int feature_count = MiApi.mitab_c_get_feature_count(Ptr);
            int field_count = MiApi.mitab_c_get_field_count(Ptr);

            richTextBox1.AppendText("\nПоля данных: " + Convert.ToString(field_count) + "\n");
            for (int i = 0; i <= field_count - 1; i++)
            {
                richTextBox1.AppendText(EBop.MapObjects.MapInfo.MiApi.mitab_c_get_field_name(Ptr, i) + " ");
            }


            richTextBox1.AppendText("\n deature_count:" + Convert.ToString(feature_count) + "\r\n");

            for (int i = 1; i <= EBop.MapObjects.MapInfo.MiApi.mitab_c_get_feature_count(Ptr); i++)
            {
                IntPtr FeaturePtr = EBop.MapObjects.MapInfo.MiApi.mitab_c_read_feature(Ptr, i);
                FeatureType Featuretype = MiApi.mitab_c_get_type(FeaturePtr);
                if (Featuretype == FeatureType.TABFC_Region)
                {
                    TPolygon Poly = new TPolygon(MiApi.mitab_c_get_field_as_string(FeaturePtr, 0));

                    int RingCount = MiApi.mitab_c_get_parts(FeaturePtr);

                    for (int iRing = 0; iRing <= RingCount - 1; iRing++)
                    {
                        int vertexCount = MiApi.mitab_c_get_vertex_count(FeaturePtr, iRing);
                        if (iRing == 0)
                            for (int iVertex = 0; iVertex <= vertexCount - 1; iVertex++)
                            {
                                TPoint MIF_Point = new TPoint(MiApi.mitab_c_get_vertex_y(FeaturePtr, iRing, iVertex), MiApi.mitab_c_get_vertex_x(FeaturePtr, iRing, iVertex));
                                Poly.AddPoint(MIF_Point);
                            }
                        else
                        {
                            TRing child = new TRing();
                            for (int iVertex = 0; iVertex <= vertexCount - 1; iVertex++)
                            {
                                TPoint MIF_Point = new TPoint(MiApi.mitab_c_get_vertex_y(FeaturePtr, iRing, iVertex), MiApi.mitab_c_get_vertex_x(FeaturePtr, iRing, iVertex));
                                child.AddPoint(MIF_Point);
                            }
                            Poly.Childs.Add(child);
                        }
                    }
                    ES.Add(Poly);
                }

                if (Featuretype == FeatureType.TABFC_Polyline)
                {
                    TPolyLine poly = new TPolyLine();
                    poly.Definition = MiApi.mitab_c_get_field_as_string(FeaturePtr, 0);
                    int RingCount = MiApi.mitab_c_get_parts(FeaturePtr);
                    int vertexCount = MiApi.mitab_c_get_vertex_count(FeaturePtr, 0);


                    for (int iVertex = 0; iVertex <= vertexCount - 1; iVertex++)
                    {
                        TPoint MIF_Point = new TPoint(MiApi.mitab_c_get_vertex_y(FeaturePtr, 0, iVertex),
                                                      MiApi.mitab_c_get_vertex_x(FeaturePtr, 0, iVertex));
                        poly.AddPoint(MIF_Point);
                    }
                    ES.Add(poly);
                }

                if (Featuretype == FeatureType.TABFC_Ellipse)
                {
                    TPolygon Poly = new TPolygon(MiApi.mitab_c_get_field_as_string(FeaturePtr, 0));
                    Poly.Name = "TABFC_Ellipse";
                    int vertexCount = MiApi.mitab_c_get_vertex_count(FeaturePtr, 0);
                    for (int iVertex = 0; iVertex <= vertexCount - 1; iVertex++)
                    {
                        TPoint MIF_Point = new TPoint(MiApi.mitab_c_get_vertex_y(FeaturePtr, 0, iVertex), MiApi.mitab_c_get_vertex_x(FeaturePtr, 0, iVertex));
                        Poly.AddPoint(MIF_Point);
                    }
                    ES.Add(Poly);
                }


                if (Featuretype == FeatureType.TABFC_Point)
                {

                }

                richTextBox1.AppendText("Feature " + FeaturePtr.ToString() + "\t");
                for (int ifd = 0; ifd <= field_count - 1; ifd++)
                    richTextBox1.AppendText(MiApi.mitab_c_get_field_as_string(FeaturePtr, ifd) + "\t");
                richTextBox1.AppendText("\r\n");
            }

            this.DocInfo.MyBlocks.ParsedSpatial = ES;
            this.DocInfo.DocTypeNick = "Mapinfo tab";
            this.DocInfo.CommentsType = "TAB";
            //res.Comments = mifreader.Body;//.GetType().ToString() + " file info \r Blocked LWPOLYLINE.Count = " + mifreader.PolygonsCount().ToString() + " \rFileBody:\r" + mifreader.Body;
            //res.Encoding = mifreader.BodyEncoding.ToString();
            //res.Number = "Encoding  " + mifreader.BodyEncoding;
            this.DocInfo.DocType = "Mapinfo tab";
            this.DocInfo.Version = MiApi.mitab_c_getlibversion().ToString();
            //ListMyCoolections(this.DocInfo.MyBlocks);
            //ListFileInfo(DocInfo);
        }
        #endregion


        private void KVZU_Form_Load(object sender, EventArgs e)
        {
#if (DEBUG)
            this.AppConfiguration = "DEBUG";
            this.Text += "/DEBUG {2019}";
            debugToolStripMenuItem.Visible = true;
            картапланToolStripMenuItem.Visible = true;
            сКПТToolStripMenuItem.Visible = true;
            fteoImage.Visible = true;
            pkk5Viewer1.Start("26:05:043433", pkk5_Types.Block);
            //TMyPoints test = new TMyPoints();
            //test.PoininTest();
#else
			this.AppConfiguration = "";
            debugToolStripMenuItem.Visible = false;
            картапланToolStripMenuItem.Visible = false;
            сКПТToolStripMenuItem.Visible = false;
			fteoImage.Visible = false;
#endif

            if ((int)this.Tag == 3) // load from NET application
            {
                this.Text = "XMl Reader в составе приложения";
                this.ShowInTaskbar = true;
                LogStarttoWebServer("XMl Reader Slaveform");

            }
            else
            {  // load as standalone application with args in cli:
                this.Text = "XMl Reader для файлов Росреестра @2015 Fixosoft";
                LogStarttoWebServer("XMl Reader Desktop");
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
            ListMyCoolections(this.DocInfo.MyBlocks);
            ListFileInfo(DocInfo);
            this.TextDefault = this.Text;
            ClearFiles();
        }


        private void LogStarttoWebServer(string AppTypeName)
        {
            if (netFteo.NetWork.NetWrapper.HostIP != "10.66.77.150") //main admin/developer machine
            {
                netFteo.IO.LogServer srv = new netFteo.IO.LogServer("82.119.136.82",
                    new netFteo.IO.LogServer_response()
                    {
                        ApplicationType = AppTypeName + " " + this.AppConfiguration,
                        AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                        state = "ste",
                        Client = netFteo.NetWork.NetWrapper.UserName
                    });
            }
            //	srv.Get_WebOnline_th("");
        }

        private void TV_Parcels_DoubleClick(object sender, EventArgs e)
        {
            if (TV_Parcels.SelectedNode != null &&
                TV_Parcels.SelectedNode.Name.Contains("SPElem"))
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

            foreach (string filename in DraggedFiles)
            {
                try
                {
                    Read(filename, true);
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    ClearControls();
                    TreeNode errNodePatr = TV_Parcels.Nodes.Add("Error in " + Path.GetFileName(filename));
                    errNodePatr.Nodes.Add(ex.Message);

                    if (TV_Parcels.TopNode != null) TV_Parcels.TopNode.Expand();
                    else
                        errNodePatr.ExpandAll();
                    return;
                }
            }
        }

    
        // Format points for cadastral usage
        private void округлитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IPointList Feature = (IPointList)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(listView1.Tag));
            if (Feature != null)
            {
                Feature.Fraq("0.00");
                Feature.SetMt(0.1); // Also set Mt= 0.1
                Feature.ResetOrdinates();
                Feature.ResetStatus("н");
                Feature.ShowasListItems(listView1, true);
            }
        }

        private void перенумероватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IPointList Feature = (IPointList)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(listView1.Tag));
            if (Feature != null)
            {
                Feature.ReorderPoints(1);
                Feature.ShowasListItems(listView1, true);
            }
        }

        /// <summary>
        /// Only for polyline. Close polyline`ll convert it to polygon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void замкнутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IPointList Feature = (IPointList)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(listView1.Tag));

            if ((Feature != null) &&
                (Feature.TypeName == "netFteo.Spatial.TPolyLine")
                )
            {
                TPolygon NewFeature = new TPolygon((TPolyLine)Feature);
                NewFeature.LayerHandle = Feature.LayerHandle;
                NewFeature.Definition = Feature.Definition + "*";
                this.DocInfo.MyBlocks.ParsedSpatial.Add(NewFeature);
                //Insert new node:
                netFteo.ObjectLister.ListEntSpat(TV_Parcels.SelectedNode.Parent, NewFeature, "SPElem.", NewFeature.Definition, NewFeature.State);
                //ListMyCoolections(this.DocInfo.MyBlocks);
                NewFeature.ShowasListItems(listView1, true);
            }



            if ((Feature != null) &&
                    (Feature.TypeName == "netFteo.Spatial.PointList")
                    )
            {
                TPolygon NewFeature = new TPolygon((PointList)Feature);
                NewFeature.LayerHandle = Feature.LayerHandle;
                NewFeature.Definition = Feature.Definition + "*";
                this.DocInfo.MyBlocks.ParsedSpatial.Add(NewFeature);
                //Insert new node:
                netFteo.ObjectLister.ListEntSpat(TV_Parcels.SelectedNode.Parent, NewFeature, "SPElem.", NewFeature.Definition, NewFeature.State);
                //ListMyCoolections(this.DocInfo.MyBlocks);
                NewFeature.ShowasListItems(listView1, true);
            }

            if ((Feature != null) &&
                (Feature.TypeName == "netFteo.Spatial.TPolygon")
                )
            {
                TPolygon NewFeature = (TPolygon)Feature;
                NewFeature.Close();
                NewFeature.ShowasListItems(listView1, true);
            }

        }

        private void ChangeXYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IPointList Feature = (IPointList)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(listView1.Tag));
            if (Feature != null)
            {
                Feature.ExchangeOrdinates();
                Feature.ShowasListItems(listView1, true);
            }
        }

        private void обратныйПорядокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IPointList Feature = (IPointList)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(listView1.Tag));
            if (Feature != null)
            {
                Feature.ReversePoints();
                Feature.ShowasListItems(listView1, true);
            }


            /*
			TPolygon Pl = (TPolygon)this.DocInfo.MyBlocks.GetEs(Convert.ToInt32(listView1.Tag));
			if (Pl != null)
			{
				Pl.Reverse_Points();
				PointListToListView(listView1, Pl);
			}
			*/
        }






        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            frmCertificates frmcertificates = new frmCertificates();
            frmcertificates.ShowDialog();
        }

        private void toolStripButton4_Click_1(object sender, EventArgs e)
        {
            OpenFile(2);// OpenXML_KVZUTyped();
        }

        private void writeDXFSingleEntitydxfPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            netFteo.IO.DXFWriter wr = new netFteo.IO.DXFWriter();
            saveFileDialog1.FilterIndex = 3;
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                netFteo.Spatial.TEntitySpatial ES = new TEntitySpatial();
                TPoint Pt = new TPoint(1000, 1000);
                ES.Add(Pt);
                ES.RemoveParentCN(DocInfo.MyBlocks.SingleCN);
                wr.SaveAsDxfScale(saveFileDialog1.FileName, ES, 1);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void удалитьГеометриюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (sender is TreeView)
            {
                //	TreeView tv = (TreeView)sender;
                if (RemoveGeometryNode(TV_Parcels.SelectedNode.Name))
                {
                    TV_Parcels.Nodes.Remove(TV_Parcels.SelectedNode);
                }
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (ListView_ItemSelected((ListView)sender, out string tag))
            {
                toolStripStatusLabel2.Text = tag;
            }
        }


        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (ListView_ItemSelected((ListView)sender, out string tag))
            {
                toolStripStatusLabel2.Text = tag;
            }
        }



        private bool ListView_ItemSelected(ListView lv, out string ItemTag)
        {
            if (lv.SelectedItems.Count == 1)
            {
                if (lv.SelectedItems[0].Tag != null)
                {
                    ItemTag = lv.SelectedItems[0].Tag.ToString();
                    return true;
                }
            }
            ItemTag = null;
            return false;
        }

        //Удалить точку
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control parent = ((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl;
            if (ListView_ItemSelected((ListView)parent, out string tag))
            {
                if (RemoveGeometryNode(tag))
                {
                    ((ListView)parent).Items.Remove(((ListView)parent).SelectedItems[0]);
                }
            }
        }

        //Удалить току/геометрию
        private void удалитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (RemoveGeometryNode(TV_Parcels.SelectedNode.Name))
            {
                TV_Parcels.Nodes.Remove(TV_Parcels.SelectedNode);
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            OpenFile(3);// OpenXML_KVZUTyped();
        }

        private void TraverserToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Traverser.TraverserMainForm frmTraverser = new Traverser.TraverserMainForm();
            frmTraverser.ShowDialog(this);

        }


        private void EditGeometryNode(ListView parent, string Tag)
        {
            IGeometry Feature = GetNodeGeometry(Tag);
            if (Feature != null)
            {
                if (Feature.TypeName == "netFteo.Spatial.TPoint")
                {
                    TPoint pt = (TPoint)Feature;
                    frmPointEditor Editor = new frmPointEditor(pt);
                    Editor.StartPosition = FormStartPosition.CenterParent;
                    if (Editor.ShowDialog(this) == DialogResult.OK)
                    {
                        parent.SelectedItems[0].Text = pt.Pref + pt.Definition;
                        parent.SelectedItems[0].SubItems[1].Text = pt.x_s;
                        parent.SelectedItems[0].SubItems[2].Text = pt.y_s;
                        parent.SelectedItems[0].SubItems[3].Text = pt.z_s;
                        parent.SelectedItems[0].SubItems[4].Text = pt.Mt_s;
                        if (pt.Pref == "н")
                            parent.SelectedItems[0].ForeColor = Color.Red;
                        else parent.SelectedItems[0].ForeColor = Color.Black;

                    }
                }
            }
        }

        private void ИзменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control parent = ((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl;
            if (ListView_ItemSelected((ListView)parent, out string tag))
            {
                EditGeometryNode((ListView)parent, tag);
            }
        }



        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (((ListView)sender).SelectedItems.Count == 1)
            {
                EditGeometryNode((ListView)sender, (string)((ListView)sender).SelectedItems[0].Tag);
            }
        }

        private void ОткрытьДополнительноеОкноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KVZU_Form frm2 = new KVZU_Form();
            frm2.Show(this);
        }

        private void ПроверкаГеометрииустановкаЕГРНТочекToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopoCheck(TV_Parcels.SelectedNode);
        }

        private void contractorEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmContractor Cntr = new frmContractor();
            Cntr.Show();
        }
    }
}

