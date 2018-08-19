namespace XMLReaderCS
{
    public partial class SchemaKPTMainForm
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node1");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Node0", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Итем 1",
            "Субитем 1-1"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "Итем 1",
            "Субитем 1-1"}, -1);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchemaKPTMainForm));
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Узел0");
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.DocName_comboBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_DateReg = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_NumberReG = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label3_GUID = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_NameIssueOrgan = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.ParcelSchema_In_Block_textBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox1_CSName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TV_Parcels = new System.Windows.Forms.TreeView();
            this.contextMenu_Parcels = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.добавитьЗУToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader_Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_X = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_Y = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_Mt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuOIPD = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.импортКоординатToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mifфайлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.текстовыйФайлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.listView_Properties = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button2 = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.pkk5Viewer1 = new RRTypes.pkk5.pkk5Viewer();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.cXmlTreeView1 = new XMLReaderCS.CXmlTreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenu_Parcels.SuspendLayout();
            this.contextMenuOIPD.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pkk5Viewer1)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(943, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem1,
            this.toolStripMenuItem3,
            this.выходToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(140, 22);
            this.toolStripMenuItem2.Text = "Создать";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(140, 22);
            this.toolStripMenuItem1.Text = "Открыть";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(140, 22);
            this.toolStripMenuItem3.Text = "Сохранить";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 49);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(943, 527);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBox1);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.DocName_comboBox);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.textBox_DateReg);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.textBox_NumberReG);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label3_GUID);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.textBox_NameIssueOrgan);
            this.tabPage2.Controls.Add(this.button3);
            this.tabPage2.Controls.Add(this.ParcelSchema_In_Block_textBox);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.comboBox1_CSName);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(935, 501);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Сведения";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(454, 361);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(459, 121);
            this.textBox1.TabIndex = 38;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(458, 344);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(216, 26);
            this.label9.TabIndex = 37;
            this.label9.Text = "Подпись";
            // 
            // DocName_comboBox
            // 
            this.DocName_comboBox.FormattingEnabled = true;
            this.DocName_comboBox.Items.AddRange(new object[] {
            "Решение",
            "Постановление",
            "Акт"});
            this.DocName_comboBox.Location = new System.Drawing.Point(249, 100);
            this.DocName_comboBox.Name = "DocName_comboBox";
            this.DocName_comboBox.Size = new System.Drawing.Size(387, 24);
            this.DocName_comboBox.TabIndex = 36;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(8, 103);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(219, 18);
            this.label8.TabIndex = 35;
            this.label8.Text = "Наименование вида документа";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(9, 76);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(315, 27);
            this.label7.TabIndex = 34;
            this.label7.Text = "Сведения об утверждении Схемы ЗУ на КПТ";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(9, 17);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 27);
            this.label5.TabIndex = 33;
            this.label5.Text = "GUID пакета";
            // 
            // button1
            // 
            this.button1.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.button1.Location = new System.Drawing.Point(398, 190);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 26);
            this.button1.TabIndex = 32;
            this.button1.Text = "Сегодня";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox_DateReg
            // 
            this.textBox_DateReg.Location = new System.Drawing.Point(249, 194);
            this.textBox_DateReg.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_DateReg.Name = "textBox_DateReg";
            this.textBox_DateReg.Size = new System.Drawing.Size(142, 23);
            this.textBox_DateReg.TabIndex = 31;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(121, 194);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 20);
            this.label4.TabIndex = 30;
            this.label4.Text = "Дата документа";
            // 
            // textBox_NumberReG
            // 
            this.textBox_NumberReG.Location = new System.Drawing.Point(249, 166);
            this.textBox_NumberReG.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_NumberReG.Name = "textBox_NumberReG";
            this.textBox_NumberReG.Size = new System.Drawing.Size(147, 23);
            this.textBox_NumberReG.TabIndex = 29;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(77, 162);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 27);
            this.label3.TabIndex = 28;
            this.label3.Text = "Номер документа";
            // 
            // label3_GUID
            // 
            this.label3_GUID.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label3_GUID.Location = new System.Drawing.Point(180, 17);
            this.label3_GUID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3_GUID.Name = "label3_GUID";
            this.label3_GUID.Size = new System.Drawing.Size(275, 27);
            this.label3_GUID.TabIndex = 27;
            this.label3_GUID.Text = "d87c91c3-f349-456c-8d4e-0f1cfd1508a3";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 128);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(213, 18);
            this.label1.TabIndex = 26;
            this.label1.Text = "Наименование органа";
            // 
            // textBox_NameIssueOrgan
            // 
            this.textBox_NameIssueOrgan.Location = new System.Drawing.Point(249, 128);
            this.textBox_NameIssueOrgan.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_NameIssueOrgan.Name = "textBox_NameIssueOrgan";
            this.textBox_NameIssueOrgan.Size = new System.Drawing.Size(664, 23);
            this.textBox_NameIssueOrgan.TabIndex = 25;
            // 
            // button3
            // 
            this.button3.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.button3.Location = new System.Drawing.Point(725, 300);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(30, 26);
            this.button3.TabIndex = 10;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // ParcelSchema_In_Block_textBox
            // 
            this.ParcelSchema_In_Block_textBox.Location = new System.Drawing.Point(0, 300);
            this.ParcelSchema_In_Block_textBox.Margin = new System.Windows.Forms.Padding(4);
            this.ParcelSchema_In_Block_textBox.Name = "ParcelSchema_In_Block_textBox";
            this.ParcelSchema_In_Block_textBox.Size = new System.Drawing.Size(708, 23);
            this.ParcelSchema_In_Block_textBox.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(5, 264);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(219, 20);
            this.label6.TabIndex = 8;
            this.label6.Text = "Изображение ЗУ";
            // 
            // comboBox1_CSName
            // 
            this.comboBox1_CSName.FormattingEnabled = true;
            this.comboBox1_CSName.Items.AddRange(new object[] {
            "МСК-26 от СК-95, зона 1",
            "МСК-26 от СК-95",
            "МСК 09-95",
            "МСК 23, зона 1",
            "МСК 23, зона 2"});
            this.comboBox1_CSName.Location = new System.Drawing.Point(163, 361);
            this.comboBox1_CSName.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox1_CSName.Name = "comboBox1_CSName";
            this.comboBox1_CSName.Size = new System.Drawing.Size(273, 24);
            this.comboBox1_CSName.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 361);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 27);
            this.label2.TabIndex = 3;
            this.label2.Text = "Система координат";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(935, 501);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Образуемые ЗУ";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(4, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.TV_Parcels);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listView1);
            this.splitContainer1.Panel2.Controls.Add(this.splitter1);
            this.splitContainer1.Panel2.Controls.Add(this.listView_Properties);
            this.splitContainer1.Panel2MinSize = 15;
            this.splitContainer1.Size = new System.Drawing.Size(927, 493);
            this.splitContainer1.SplitterDistance = 527;
            this.splitContainer1.TabIndex = 9;
            // 
            // TV_Parcels
            // 
            this.TV_Parcels.ContextMenuStrip = this.contextMenu_Parcels;
            this.TV_Parcels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TV_Parcels.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TV_Parcels.Location = new System.Drawing.Point(0, 0);
            this.TV_Parcels.Name = "TV_Parcels";
            treeNode1.Name = "Child";
            treeNode1.Text = "Node1";
            treeNode2.Checked = true;
            treeNode2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            treeNode2.ImageIndex = -2;
            treeNode2.Name = "Root";
            treeNode2.Text = "Node0";
            this.TV_Parcels.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.TV_Parcels.Size = new System.Drawing.Size(527, 493);
            this.TV_Parcels.TabIndex = 2;
            // 
            // contextMenu_Parcels
            // 
            this.contextMenu_Parcels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.добавитьЗУToolStripMenuItem});
            this.contextMenu_Parcels.Name = "contextMenu_Parcels";
            this.contextMenu_Parcels.Size = new System.Drawing.Size(152, 26);
            // 
            // добавитьЗУToolStripMenuItem
            // 
            this.добавитьЗУToolStripMenuItem.Name = "добавитьЗУToolStripMenuItem";
            this.добавитьЗУToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.добавитьЗУToolStripMenuItem.Text = "Добавить ЗУ";
            this.добавитьЗУToolStripMenuItem.Click += new System.EventHandler(this.добавитьЗУToolStripMenuItem_Click);
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.SystemColors.Info;
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_Name,
            this.columnHeader_X,
            this.columnHeader_Y,
            this.columnHeader_Mt,
            this.columnHeader3});
            this.listView1.ContextMenuStrip = this.contextMenuOIPD;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.HideSelection = false;
            listViewItem1.StateImageIndex = 0;
            listViewItem2.StateImageIndex = 0;
            listViewItem3.StateImageIndex = 0;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3});
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(396, 291);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader_Name
            // 
            this.columnHeader_Name.Text = "Имя";
            // 
            // columnHeader_X
            // 
            this.columnHeader_X.Text = "x, м.";
            this.columnHeader_X.Width = 100;
            // 
            // columnHeader_Y
            // 
            this.columnHeader_Y.Text = "y, м.";
            this.columnHeader_Y.Width = 100;
            // 
            // columnHeader_Mt
            // 
            this.columnHeader_Mt.Text = "Mt, м.";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Описание";
            // 
            // contextMenuOIPD
            // 
            this.contextMenuOIPD.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.импортКоординатToolStripMenuItem});
            this.contextMenuOIPD.Name = "contextMenuOIPD";
            this.contextMenuOIPD.Size = new System.Drawing.Size(181, 26);
            // 
            // импортКоординатToolStripMenuItem
            // 
            this.импортКоординатToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mifфайлToolStripMenuItem,
            this.текстовыйФайлToolStripMenuItem});
            this.импортКоординатToolStripMenuItem.Name = "импортКоординатToolStripMenuItem";
            this.импортКоординатToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.импортКоординатToolStripMenuItem.Text = "Импорт координат";
            // 
            // mifфайлToolStripMenuItem
            // 
            this.mifфайлToolStripMenuItem.Name = "mifфайлToolStripMenuItem";
            this.mifфайлToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.mifфайлToolStripMenuItem.Text = "mif-файл";
            // 
            // текстовыйФайлToolStripMenuItem
            // 
            this.текстовыйФайлToolStripMenuItem.Name = "текстовыйФайлToolStripMenuItem";
            this.текстовыйФайлToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.текстовыйФайлToolStripMenuItem.Text = "Текстовый файл";
            // 
            // splitter1
            // 
            this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter1.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 291);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(396, 10);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // listView_Properties
            // 
            this.listView_Properties.BackColor = System.Drawing.SystemColors.Window;
            this.listView_Properties.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView_Properties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader8});
            this.listView_Properties.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listView_Properties.FullRowSelect = true;
            this.listView_Properties.GridLines = true;
            this.listView_Properties.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView_Properties.HideSelection = false;
            this.listView_Properties.ImeMode = System.Windows.Forms.ImeMode.Off;
            listViewItem4.StateImageIndex = 0;
            listViewItem5.StateImageIndex = 0;
            listViewItem6.StateImageIndex = 0;
            this.listView_Properties.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem4,
            listViewItem5,
            listViewItem6});
            this.listView_Properties.Location = new System.Drawing.Point(0, 301);
            this.listView_Properties.MultiSelect = false;
            this.listView_Properties.Name = "listView_Properties";
            this.listView_Properties.Size = new System.Drawing.Size(396, 192);
            this.listView_Properties.TabIndex = 1;
            this.listView_Properties.UseCompatibleStateImageBehavior = false;
            this.listView_Properties.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Характеристика";
            this.columnHeader1.Width = 171;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Значение";
            this.columnHeader2.Width = 130;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "";
            this.columnHeader8.Width = 45;
            // 
            // button2
            // 
            this.button2.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.button2.Location = new System.Drawing.Point(846, 252);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(33, 26);
            this.button2.TabIndex = 8;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.pkk5Viewer1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(935, 501);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "pkk5";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // pkk5Viewer1
            // 
            this.pkk5Viewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pkk5Viewer1.Image = ((System.Drawing.Image)(resources.GetObject("pkk5Viewer1.Image")));
            this.pkk5Viewer1.Location = new System.Drawing.Point(3, 3);
            this.pkk5Viewer1.Name = "pkk5Viewer1";
            this.pkk5Viewer1.QueryObjectType = RRTypes.pkk5.pkk5_Types.Block;
            this.pkk5Viewer1.QueryValue = null;
            this.pkk5Viewer1.Size = new System.Drawing.Size(929, 495);
            this.pkk5Viewer1.TabIndex = 0;
            this.pkk5Viewer1.TabStop = false;
            this.pkk5Viewer1.QuerySuccefull += new System.EventHandler(this.pkk5Viewer1_QuerySuccefull);
            this.pkk5Viewer1.QueryStart += new System.EventHandler(this.pkk5Viewer1_QueryStart);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.cXmlTreeView1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(935, 501);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "XML body";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // cXmlTreeView1
            // 
            this.cXmlTreeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cXmlTreeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cXmlTreeView1.Location = new System.Drawing.Point(3, 3);
            this.cXmlTreeView1.Name = "cXmlTreeView1";
            treeNode3.Name = "Узел0";
            treeNode3.Text = "Узел0";
            this.cXmlTreeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3});
            this.cXmlTreeView1.Size = new System.Drawing.Size(929, 495);
            this.cXmlTreeView1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.toolStripSeparator1,
            this.toolStripButton4,
            this.toolStripButton1,
            this.toolStripSeparator2,
            this.toolStripButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(943, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "Выход";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AutoSize = false;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(15, 25);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton4.Text = "toolStripButton4";
            this.toolStripButton4.ToolTipText = "Создать новый документ";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Открыть файл";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AutoSize = false;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(25, 25);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "О про";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "xml|*.xml";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Схема расположения на КПТ|*.xml";
            // 
            // SchemaKPTMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 576);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SchemaKPTMainForm";
            this.Text = "Схема ЗУ на КПТ";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenu_Parcels.ResumeLayout(false);
            this.contextMenuOIPD.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pkk5Viewer1)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ComboBox comboBox1_CSName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox ParcelSchema_In_Block_textBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ComboBox DocName_comboBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox_DateReg;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_NumberReG;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label3_GUID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_NameIssueOrgan;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView TV_Parcels;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader_Name;
        private System.Windows.Forms.ColumnHeader columnHeader_X;
        private System.Windows.Forms.ColumnHeader columnHeader_Y;
        private System.Windows.Forms.ColumnHeader columnHeader_Mt;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ListView listView_Properties;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ContextMenuStrip contextMenuOIPD;
        private System.Windows.Forms.ToolStripMenuItem импортКоординатToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mifфайлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem текстовыйФайлToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenu_Parcels;
        private System.Windows.Forms.ToolStripMenuItem добавитьЗУToolStripMenuItem;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabPage tabPage3;
        private RRTypes.pkk5.pkk5Viewer pkk5Viewer1;
        private System.Windows.Forms.TabPage tabPage4;
        private CXmlTreeView cXmlTreeView1;
    }
}

