using MySql.Data.MySqlClient;
using System.Data;
namespace GKNData

{
    partial class MainGKNForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainGKNForm));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Top", 4);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Sub #1", 0);
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node1");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Node0", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel_SubRf_CN = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel_AllMessages = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel_CurrentItem = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel_DBName = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Connect = new System.Windows.Forms.ToolStripMenuItem();
            this.сбросПодключенияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.импортToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сервисToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сменитьСубъектToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.правкаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.добавитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.открытьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.удалитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.помощьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.оПрограммеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.свойстваToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.онлайнToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.поискToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.копироватьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Tree_imageList = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_Connect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_Import = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_Property = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.loadingCircleToolStripMenuItem1 = new MRG.Controls.UI.LoadingCircleToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.SearchTextBox = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.button_Favorites = new System.Windows.Forms.Button();
            this.button_History = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.Button_Exit = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.Explorer_listView = new System.Windows.Forms.ListView();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.contextMenu1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel_SubRf_CN,
            this.StatusLabel_AllMessages,
            this.StatusLabel_CurrentItem,
            this.StatusLabel_DBName,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 574);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(941, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLabel_SubRf_CN
            // 
            this.StatusLabel_SubRf_CN.AutoSize = false;
            this.StatusLabel_SubRf_CN.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusLabel_SubRf_CN.Name = "StatusLabel_SubRf_CN";
            this.StatusLabel_SubRf_CN.Size = new System.Drawing.Size(180, 17);
            this.StatusLabel_SubRf_CN.Text = "::";
            // 
            // StatusLabel_AllMessages
            // 
            this.StatusLabel_AllMessages.AutoSize = false;
            this.StatusLabel_AllMessages.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusLabel_AllMessages.Name = "StatusLabel_AllMessages";
            this.StatusLabel_AllMessages.Size = new System.Drawing.Size(150, 17);
            this.StatusLabel_AllMessages.Text = "::";
            // 
            // StatusLabel_CurrentItem
            // 
            this.StatusLabel_CurrentItem.AutoSize = false;
            this.StatusLabel_CurrentItem.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusLabel_CurrentItem.Name = "StatusLabel_CurrentItem";
            this.StatusLabel_CurrentItem.Size = new System.Drawing.Size(70, 17);
            this.StatusLabel_CurrentItem.Text = "-";
            // 
            // StatusLabel_DBName
            // 
            this.StatusLabel_DBName.AutoSize = false;
            this.StatusLabel_DBName.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusLabel_DBName.Name = "StatusLabel_DBName";
            this.StatusLabel_DBName.Size = new System.Drawing.Size(180, 17);
            this.StatusLabel_DBName.Text = "::";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.BackColor = System.Drawing.Color.Maroon;
            this.toolStripProgressBar1.ForeColor = System.Drawing.Color.Maroon;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.toolStripProgressBar1.Value = 50;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.сервисToolStripMenuItem,
            this.правкаToolStripMenuItem,
            this.помощьToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(941, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_Connect,
            this.сбросПодключенияToolStripMenuItem,
            this.toolStripSeparator1,
            this.импортToolStripMenuItem,
            this.toolStripSeparator2,
            this.выходToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // MenuItem_Connect
            // 
            this.MenuItem_Connect.Image = global::GKNData.Properties.Resources.MySQL_24х24;
            this.MenuItem_Connect.Name = "MenuItem_Connect";
            this.MenuItem_Connect.Size = new System.Drawing.Size(188, 22);
            this.MenuItem_Connect.Text = "Подключить";
            this.MenuItem_Connect.Click += new System.EventHandler(this.MenuItem_Connect_Click);
            // 
            // сбросПодключенияToolStripMenuItem
            // 
            this.сбросПодключенияToolStripMenuItem.Name = "сбросПодключенияToolStripMenuItem";
            this.сбросПодключенияToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.сбросПодключенияToolStripMenuItem.Text = "Сброс подключения";
            this.сбросПодключенияToolStripMenuItem.Click += new System.EventHandler(this.сбросПодключенияToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(185, 6);
            // 
            // импортToolStripMenuItem
            // 
            this.импортToolStripMenuItem.Image = global::GKNData.Properties.Resources.xml_import16x16;
            this.импортToolStripMenuItem.Name = "импортToolStripMenuItem";
            this.импортToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.импортToolStripMenuItem.Text = "Импорт....";
            this.импортToolStripMenuItem.Click += new System.EventHandler(this.импортToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(185, 6);
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Image = global::GKNData.Properties.Resources.cross1;
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // сервисToolStripMenuItem
            // 
            this.сервисToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.сменитьСубъектToolStripMenuItem,
            this.OptionsToolStripMenuItem});
            this.сервисToolStripMenuItem.Name = "сервисToolStripMenuItem";
            this.сервисToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.сервисToolStripMenuItem.Text = "Сервис";
            // 
            // сменитьСубъектToolStripMenuItem
            // 
            this.сменитьСубъектToolStripMenuItem.Image = global::GKNData.Properties.Resources.ВыборСубъектаРФ;
            this.сменитьСубъектToolStripMenuItem.Name = "сменитьСубъектToolStripMenuItem";
            this.сменитьСубъектToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.сменитьСубъектToolStripMenuItem.Text = "Сменить субъект";
            this.сменитьСубъектToolStripMenuItem.Click += new System.EventHandler(this.сменитьСубъектToolStripMenuItem_Click);
            // 
            // OptionsToolStripMenuItem
            // 
            this.OptionsToolStripMenuItem.Image = global::GKNData.Properties.Resources.page_white_code;
            this.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            this.OptionsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.OptionsToolStripMenuItem.Text = "Настройки";
            this.OptionsToolStripMenuItem.Click += new System.EventHandler(this.настройкиToolStripMenuItem_Click);
            // 
            // правкаToolStripMenuItem
            // 
            this.правкаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.добавитьToolStripMenuItem,
            this.открытьToolStripMenuItem,
            this.удалитьToolStripMenuItem});
            this.правкаToolStripMenuItem.Name = "правкаToolStripMenuItem";
            this.правкаToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.правкаToolStripMenuItem.Text = "Правка";
            // 
            // добавитьToolStripMenuItem
            // 
            this.добавитьToolStripMenuItem.Image = global::GKNData.Properties.Resources.add;
            this.добавитьToolStripMenuItem.Name = "добавитьToolStripMenuItem";
            this.добавитьToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.добавитьToolStripMenuItem.Text = "Добавить";
            this.добавитьToolStripMenuItem.Click += new System.EventHandler(this.ДобавитьToolStripMenuItem_Click);
            // 
            // открытьToolStripMenuItem
            // 
            this.открытьToolStripMenuItem.Image = global::GKNData.Properties.Resources.Свойства;
            this.открытьToolStripMenuItem.Name = "открытьToolStripMenuItem";
            this.открытьToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.открытьToolStripMenuItem.Text = "Свойства";
            this.открытьToolStripMenuItem.Click += new System.EventHandler(this.открытьToolStripMenuItem_Click);
            // 
            // удалитьToolStripMenuItem
            // 
            this.удалитьToolStripMenuItem.Image = global::GKNData.Properties.Resources.cross;
            this.удалитьToolStripMenuItem.Name = "удалитьToolStripMenuItem";
            this.удалитьToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.удалитьToolStripMenuItem.Text = "Удалить";
            this.удалитьToolStripMenuItem.Click += new System.EventHandler(this.УдалитьToolStripMenuItem_Click);
            // 
            // помощьToolStripMenuItem
            // 
            this.помощьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.оПрограммеToolStripMenuItem});
            this.помощьToolStripMenuItem.Name = "помощьToolStripMenuItem";
            this.помощьToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.помощьToolStripMenuItem.Text = "Помощь";
            // 
            // оПрограммеToolStripMenuItem
            // 
            this.оПрограммеToolStripMenuItem.Name = "оПрограммеToolStripMenuItem";
            this.оПрограммеToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.оПрограммеToolStripMenuItem.Text = "О программе";
            this.оПрограммеToolStripMenuItem.Click += new System.EventHandler(this.оПрограммеToolStripMenuItem_Click);
            // 
            // contextMenu1
            // 
            this.contextMenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.свойстваToolStripMenuItem,
            this.онлайнToolStripMenuItem,
            this.поискToolStripMenuItem,
            this.копироватьToolStripMenuItem});
            this.contextMenu1.Name = "contextMenu1";
            this.contextMenu1.Size = new System.Drawing.Size(150, 92);
            // 
            // свойстваToolStripMenuItem
            // 
            this.свойстваToolStripMenuItem.Image = global::GKNData.Properties.Resources.Свойства;
            this.свойстваToolStripMenuItem.Name = "свойстваToolStripMenuItem";
            this.свойстваToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.свойстваToolStripMenuItem.Text = "Свойства";
            this.свойстваToolStripMenuItem.Click += new System.EventHandler(this.свойстваToolStripMenuItem_Click);
            // 
            // онлайнToolStripMenuItem
            // 
            this.онлайнToolStripMenuItem.Name = "онлайнToolStripMenuItem";
            this.онлайнToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.онлайнToolStripMenuItem.Text = "Он-лайн";
            this.онлайнToolStripMenuItem.Click += new System.EventHandler(this.онлайнToolStripMenuItem_Click);
            // 
            // поискToolStripMenuItem
            // 
            this.поискToolStripMenuItem.Name = "поискToolStripMenuItem";
            this.поискToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.поискToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.поискToolStripMenuItem.Text = "Поиск";
            this.поискToolStripMenuItem.Click += new System.EventHandler(this.поискToolStripMenuItem_Click);
            // 
            // копироватьToolStripMenuItem
            // 
            this.копироватьToolStripMenuItem.Name = "копироватьToolStripMenuItem";
            this.копироватьToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.копироватьToolStripMenuItem.Text = "Копировать";
            this.копироватьToolStripMenuItem.Click += new System.EventHandler(this.КопироватьToolStripMenuItem_Click);
            // 
            // Tree_imageList
            // 
            this.Tree_imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("Tree_imageList.ImageStream")));
            this.Tree_imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.Tree_imageList.Images.SetKeyName(0, "Кварталы_2.bmp");
            this.Tree_imageList.Images.SetKeyName(1, "ЗУ_1.bmp");
            this.Tree_imageList.Images.SetKeyName(2, "ЗУ.bmp");
            this.Tree_imageList.Images.SetKeyName(3, "script_code_red.png");
            this.Tree_imageList.Images.SetKeyName(4, "home_green16_h.bmp");
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_Connect,
            this.toolStripSeparator3,
            this.Button_Import,
            this.toolStripSeparator4,
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripSeparator5,
            this.Button_Property,
            this.toolStripSeparator6,
            this.loadingCircleToolStripMenuItem1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(941, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_Connect
            // 
            this.toolStripButton_Connect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Connect.Image = global::GKNData.Properties.Resources.MySQL_24х24;
            this.toolStripButton_Connect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Connect.Name = "toolStripButton_Connect";
            this.toolStripButton_Connect.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_Connect.Text = "toolStripButton1";
            this.toolStripButton_Connect.Click += new System.EventHandler(this.ToolStripButton1_Click_1);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.AutoSize = false;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(70, 25);
            // 
            // Button_Import
            // 
            this.Button_Import.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Button_Import.Image = global::GKNData.Properties.Resources.xml_import16x16;
            this.Button_Import.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Button_Import.Name = "Button_Import";
            this.Button_Import.Size = new System.Drawing.Size(23, 22);
            this.Button_Import.Text = "toolStripButton1";
            this.Button_Import.CheckStateChanged += new System.EventHandler(this.Button_Import_CheckStateChanged);
            this.Button_Import.Click += new System.EventHandler(this.Button_Import_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.AutoSize = false;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(45, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::GKNData.Properties.Resources.arrow_left;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.ToolStripButton1_Click_2);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::GKNData.Properties.Resources.arrow_right;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.AutoSize = false;
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(45, 25);
            // 
            // Button_Property
            // 
            this.Button_Property.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Button_Property.Image = global::GKNData.Properties.Resources.Свойства;
            this.Button_Property.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Button_Property.Name = "Button_Property";
            this.Button_Property.Size = new System.Drawing.Size(23, 22);
            this.Button_Property.Text = "toolStripButton1";
            this.Button_Property.ToolTipText = "Свойства";
            this.Button_Property.Click += new System.EventHandler(this.Button_Property_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.AutoSize = false;
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(150, 25);
            // 
            // loadingCircleToolStripMenuItem1
            // 
            this.loadingCircleToolStripMenuItem1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            // 
            // loadingCircleToolStripMenuItem1
            // 
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.AccessibleName = "loadingCircleToolStripMenuItem1";
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.Active = false;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.Color = System.Drawing.Color.RoyalBlue;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.InnerCircleRadius = 5;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.Location = new System.Drawing.Point(434, 1);
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.Name = "loadingCircleToolStripMenuItem1";
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.NumberSpoke = 12;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.OuterCircleRadius = 11;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.RotationSpeed = 100;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.Size = new System.Drawing.Size(26, 22);
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.SpokeThickness = 2;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.TabIndex = 1;
            this.loadingCircleToolStripMenuItem1.LoadingCircleControl.Text = "loadingCircleToolStripMenuItem1";
            this.loadingCircleToolStripMenuItem1.Name = "loadingCircleToolStripMenuItem1";
            this.loadingCircleToolStripMenuItem1.Size = new System.Drawing.Size(26, 22);
            this.loadingCircleToolStripMenuItem1.Text = "loadingCircleToolStripMenuItem1";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.SearchTextBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(168, 49);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(773, 35);
            this.panel1.TabIndex = 10;
            this.panel1.Visible = false;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Right;
            this.button2.Image = global::GKNData.Properties.Resources.pictures;
            this.button2.Location = new System.Drawing.Point(732, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(39, 33);
            this.button2.TabIndex = 10;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // SearchTextBox
            // 
            this.SearchTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SearchTextBox.Location = new System.Drawing.Point(12, 6);
            this.SearchTextBox.Name = "SearchTextBox";
            this.SearchTextBox.Size = new System.Drawing.Size(171, 23);
            this.SearchTextBox.TabIndex = 11;
            this.SearchTextBox.TextChanged += new System.EventHandler(this.SearchTextBox_TextChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.panel2.Controls.Add(this.button4);
            this.panel2.Controls.Add(this.button_Favorites);
            this.panel2.Controls.Add(this.button_History);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.Button_Exit);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 49);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(168, 525);
            this.panel2.TabIndex = 12;
            // 
            // button4
            // 
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button4.Image = global::GKNData.Properties.Resources.photos;
            this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button4.Location = new System.Drawing.Point(2, 7);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(165, 40);
            this.button4.TabIndex = 1;
            this.button4.Text = "Проводник";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.Button4_Click);
            // 
            // button_Favorites
            // 
            this.button_Favorites.FlatAppearance.BorderSize = 0;
            this.button_Favorites.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Favorites.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Favorites.Image = global::GKNData.Properties.Resources.heart;
            this.button_Favorites.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_Favorites.Location = new System.Drawing.Point(1, 152);
            this.button_Favorites.Name = "button_Favorites";
            this.button_Favorites.Size = new System.Drawing.Size(165, 40);
            this.button_Favorites.TabIndex = 0;
            this.button_Favorites.Text = "Избранное";
            this.button_Favorites.UseVisualStyleBackColor = true;
            this.button_Favorites.Click += new System.EventHandler(this.button_Favorites_Click);
            // 
            // button_History
            // 
            this.button_History.FlatAppearance.BorderSize = 0;
            this.button_History.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_History.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_History.Image = global::GKNData.Properties.Resources.table_refresh;
            this.button_History.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_History.Location = new System.Drawing.Point(1, 106);
            this.button_History.Name = "button_History";
            this.button_History.Size = new System.Drawing.Size(165, 40);
            this.button_History.TabIndex = 0;
            this.button_History.Text = "История";
            this.button_History.UseVisualStyleBackColor = true;
            this.button_History.Click += new System.EventHandler(this.button_History_Click);
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Image = global::GKNData.Properties.Resources.pictures;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(1, 60);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(165, 40);
            this.button1.TabIndex = 0;
            this.button1.Text = "Кварталы";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Button_Exit
            // 
            this.Button_Exit.FlatAppearance.BorderSize = 0;
            this.Button_Exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Exit.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Button_Exit.Image = global::GKNData.Properties.Resources.cross;
            this.Button_Exit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Button_Exit.Location = new System.Drawing.Point(1, 672);
            this.Button_Exit.Name = "Button_Exit";
            this.Button_Exit.Size = new System.Drawing.Size(167, 40);
            this.Button_Exit.TabIndex = 0;
            this.Button_Exit.Text = "Выход";
            this.Button_Exit.UseVisualStyleBackColor = true;
            this.Button_Exit.Click += new System.EventHandler(this.Button_Exit_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.Explorer_listView);
            this.panel3.Controls.Add(this.treeView1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(168, 84);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(773, 490);
            this.panel3.TabIndex = 13;
            // 
            // Explorer_listView
            // 
            this.Explorer_listView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.Explorer_listView.HideSelection = false;
            this.Explorer_listView.HotTracking = true;
            this.Explorer_listView.HoverSelection = true;
            this.Explorer_listView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.Explorer_listView.LargeImageList = this.Tree_imageList;
            this.Explorer_listView.Location = new System.Drawing.Point(430, 25);
            this.Explorer_listView.Name = "Explorer_listView";
            this.Explorer_listView.Size = new System.Drawing.Size(331, 265);
            this.Explorer_listView.TabIndex = 4;
            this.Explorer_listView.UseCompatibleStateImageBehavior = false;
            this.Explorer_listView.DoubleClick += new System.EventHandler(this.Explorer_listView_DoubleClick);
            this.Explorer_listView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Explorer_listView_KeyUp);
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.SystemColors.Control;
            this.treeView1.ContextMenuStrip = this.contextMenu1;
            this.treeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.Tree_imageList;
            this.treeView1.Location = new System.Drawing.Point(13, 25);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Node1";
            treeNode1.Text = "Node1";
            treeNode2.Name = "Node0";
            treeNode2.Text = "Node0";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(411, 265);
            this.treeView1.TabIndex = 3;
            this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.TreeView1_DrawNode);
            this.treeView1.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeView1_BeforeSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.Enter += new System.EventHandler(this.TreeView1_Enter);
            this.treeView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyUp);
            this.treeView1.Leave += new System.EventHandler(this.TreeView1_Leave);
            // 
            // MainGKNForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(941, 596);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(650, 400);
            this.Name = "MainGKNForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "GKN Data .NET";
            this.Load += new System.EventHandler(this.MainGKNForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainGKNForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainGKNForm_DragEnter);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenu1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сервисToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem правкаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem помощьToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton Button_Property;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_SubRf_CN;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_AllMessages;
        private System.Windows.Forms.ToolStripButton Button_Import;
        private System.Windows.Forms.ToolStripMenuItem импортToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сменитьСубъектToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem открытьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сбросПодключенияToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Connect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_DBName;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_CurrentItem;
        private System.Windows.Forms.ToolStripMenuItem оПрограммеToolStripMenuItem;
        private System.Windows.Forms.ImageList Tree_imageList;
        private System.Windows.Forms.ContextMenuStrip contextMenu1;
        private System.Windows.Forms.ToolStripMenuItem онлайнToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem поискToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox SearchTextBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private MRG.Controls.UI.LoadingCircleToolStripMenuItem loadingCircleToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem свойстваToolStripMenuItem;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Button Button_Exit;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button_History;
		private System.Windows.Forms.Button button_Favorites;
        private System.Windows.Forms.ToolStripButton toolStripButton_Connect;
        private System.Windows.Forms.ToolStripMenuItem копироватьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem добавитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem удалитьToolStripMenuItem;
        private System.Windows.Forms.ListView Explorer_listView;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
    }
}

