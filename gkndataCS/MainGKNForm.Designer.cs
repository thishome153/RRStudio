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
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Node1");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Node0", new System.Windows.Forms.TreeNode[] {
            treeNode3});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainGKNForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel_SubRf_CN = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel_AllMessages = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel_CurrentItem = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel_DBName = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.открытьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.помощьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.оПрограммеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenu1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.онлайнToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.поискToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Tree_imageList = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_Exit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_ChangeSub = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.Button_Property = new System.Windows.Forms.ToolStripButton();
            this.Button_Import = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SearchTextBox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.contextMenu1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 383);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(710, 22);
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
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.сервисToolStripMenuItem,
            this.правкаToolStripMenuItem,
            this.помощьToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(710, 24);
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
            this.открытьToolStripMenuItem});
            this.правкаToolStripMenuItem.Name = "правкаToolStripMenuItem";
            this.правкаToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.правкаToolStripMenuItem.Text = "Правка";
            // 
            // открытьToolStripMenuItem
            // 
            this.открытьToolStripMenuItem.Image = global::GKNData.Properties.Resources.Свойства;
            this.открытьToolStripMenuItem.Name = "открытьToolStripMenuItem";
            this.открытьToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.открытьToolStripMenuItem.Text = "Свойства";
            this.открытьToolStripMenuItem.Click += new System.EventHandler(this.открытьToolStripMenuItem_Click);
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
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.treeView1.ContextMenuStrip = this.contextMenu1;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.Tree_imageList;
            this.treeView1.Location = new System.Drawing.Point(0, 49);
            this.treeView1.Name = "treeView1";
            treeNode3.Name = "Node1";
            treeNode3.Text = "Node1";
            treeNode4.Name = "Node0";
            treeNode4.Text = "Node0";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4});
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(710, 334);
            this.treeView1.TabIndex = 2;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.Click += new System.EventHandler(this.treeView1_Click);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            this.treeView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyUp);
            // 
            // contextMenu1
            // 
            this.contextMenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.онлайнToolStripMenuItem,
            this.поискToolStripMenuItem});
            this.contextMenu1.Name = "contextMenu1";
            this.contextMenu1.Size = new System.Drawing.Size(150, 48);
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
            this.toolStripButton_Exit,
            this.toolStripSeparator3,
            this.Button_ChangeSub,
            this.toolStripSeparator4,
            this.Button_Property,
            this.Button_Import});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(710, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_Exit
            // 
            this.toolStripButton_Exit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Exit.Name = "toolStripButton_Exit";
            this.toolStripButton_Exit.Size = new System.Drawing.Size(45, 22);
            this.toolStripButton_Exit.Text = "Выход";
            this.toolStripButton_Exit.ToolTipText = "Выход";
            this.toolStripButton_Exit.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.AutoSize = false;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(12, 25);
            // 
            // Button_ChangeSub
            // 
            this.Button_ChangeSub.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Button_ChangeSub.Image = global::GKNData.Properties.Resources.ВыборСубъектаРФ;
            this.Button_ChangeSub.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Button_ChangeSub.Name = "Button_ChangeSub";
            this.Button_ChangeSub.Size = new System.Drawing.Size(23, 22);
            this.Button_ChangeSub.Text = "toolStripButton1";
            this.Button_ChangeSub.ToolTipText = "Сменить Субъект";
            this.Button_ChangeSub.Click += new System.EventHandler(this.Button_ChangeSub_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
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
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.SearchTextBox);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Location = new System.Drawing.Point(249, 172);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(221, 38);
            this.panel1.TabIndex = 10;
            this.panel1.Visible = false;
            // 
            // SearchTextBox
            // 
            this.SearchTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SearchTextBox.Location = new System.Drawing.Point(3, 9);
            this.SearchTextBox.Name = "SearchTextBox";
            this.SearchTextBox.Size = new System.Drawing.Size(171, 23);
            this.SearchTextBox.TabIndex = 11;
            this.SearchTextBox.TextChanged += new System.EventHandler(this.SearchTextBox_TextChanged);
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Right;
            this.button2.Image = global::GKNData.Properties.Resources.pictures;
            this.button2.Location = new System.Drawing.Point(180, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(39, 36);
            this.button2.TabIndex = 10;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(50, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.toolStripProgressBar1.Value = 50;
            // 
            // MainGKNForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 405);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(650, 400);
            this.Name = "MainGKNForm";
            this.Text = "ГКН Дата";
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
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton Button_ChangeSub;
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
        private System.Windows.Forms.ToolStripButton toolStripButton_Exit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_DBName;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_CurrentItem;
        private System.Windows.Forms.ToolStripMenuItem оПрограммеToolStripMenuItem;
        private System.Windows.Forms.ImageList Tree_imageList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ContextMenuStrip contextMenu1;
        private System.Windows.Forms.ToolStripMenuItem онлайнToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem поискToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox SearchTextBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
    }
}

