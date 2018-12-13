namespace XMLReaderCS
{
    partial class ESChecker_MP06Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ESChecker_MP06Form));
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.imList_dStates = new System.Windows.Forms.ImageList(this.components);
            this.label_doc_GUID = new System.Windows.Forms.LinkLabel();
            this.Label_sig_Properties = new System.Windows.Forms.LinkLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.cXmlTreeView2 = new XMLReaderCS.CXmlTreeView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader2});
            this.listView1.Location = new System.Drawing.Point(0, 340);
            this.listView1.Margin = new System.Windows.Forms.Padding(4);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(967, 294);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Параметр";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Значение";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Результат";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 250;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 643);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(976, 55);
            this.panel1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(401, 10);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(164, 41);
            this.button1.TabIndex = 0;
            this.button1.Text = "ОК";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // imList_dStates
            // 
            this.imList_dStates.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imList_dStates.ImageStream")));
            this.imList_dStates.TransparentColor = System.Drawing.Color.Transparent;
            this.imList_dStates.Images.SetKeyName(0, "accept.png");
            this.imList_dStates.Images.SetKeyName(1, "ЗУ_1.bmp");
            this.imList_dStates.Images.SetKeyName(2, "house.png");
            this.imList_dStates.Images.SetKeyName(3, "golf1-16.bmp");
            this.imList_dStates.Images.SetKeyName(4, "globe.gif");
            this.imList_dStates.Images.SetKeyName(5, "ggs_16x16.gif");
            this.imList_dStates.Images.SetKeyName(6, "ОхрЗона16x16.bmp");
            this.imList_dStates.Images.SetKeyName(7, "ВнутренГраницы16x16.bmp");
            this.imList_dStates.Images.SetKeyName(8, "mail.bmp");
            this.imList_dStates.Images.SetKeyName(9, "users16_h.gif");
            this.imList_dStates.Images.SetKeyName(10, "sign 16x16.gif");
            this.imList_dStates.Images.SetKeyName(11, "favicons_orig.png");
            this.imList_dStates.Images.SetKeyName(12, "Acrobat 16x16.png");
            this.imList_dStates.Images.SetKeyName(13, "page_white_acrobat.png");
            this.imList_dStates.Images.SetKeyName(14, "rosette.png");
            // 
            // label_doc_GUID
            // 
            this.label_doc_GUID.BackColor = System.Drawing.SystemColors.Control;
            this.label_doc_GUID.Image = global::XMLReaderCS.Properties.Resources.tick;
            this.label_doc_GUID.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_doc_GUID.Location = new System.Drawing.Point(16, 11);
            this.label_doc_GUID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_doc_GUID.Name = "label_doc_GUID";
            this.label_doc_GUID.Padding = new System.Windows.Forms.Padding(33, 0, 0, 0);
            this.label_doc_GUID.Size = new System.Drawing.Size(410, 31);
            this.label_doc_GUID.TabIndex = 18;
            this.label_doc_GUID.TabStop = true;
            this.label_doc_GUID.Text = "Номер документа";
            this.label_doc_GUID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label_sig_Properties
            // 
            this.Label_sig_Properties.BackColor = System.Drawing.SystemColors.Control;
            this.Label_sig_Properties.Image = global::XMLReaderCS.Properties.Resources.calendar_view_day;
            this.Label_sig_Properties.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.Label_sig_Properties.Location = new System.Drawing.Point(480, 9);
            this.Label_sig_Properties.Name = "Label_sig_Properties";
            this.Label_sig_Properties.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.Label_sig_Properties.Size = new System.Drawing.Size(484, 44);
            this.Label_sig_Properties.TabIndex = 20;
            this.Label_sig_Properties.TabStop = true;
            this.Label_sig_Properties.Text = "Номер документа";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitContainer1.Location = new System.Drawing.Point(0, 45);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            this.splitContainer1.Panel1MinSize = 250;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cXmlTreeView2);
            this.splitContainer1.Size = new System.Drawing.Size(967, 288);
            this.splitContainer1.SplitterDistance = 480;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 22;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ImageKey = "accept.png";
            this.treeView1.ImageList = this.imList_dStates;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(480, 288);
            this.treeView1.TabIndex = 3;
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // cXmlTreeView2
            // 
            this.cXmlTreeView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cXmlTreeView2.Location = new System.Drawing.Point(0, 0);
            this.cXmlTreeView2.Name = "cXmlTreeView2";
            this.cXmlTreeView2.Size = new System.Drawing.Size(482, 288);
            this.cXmlTreeView2.TabIndex = 22;
            // 
            // ESChecker_MP06Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 698);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.Label_sig_Properties);
            this.Controls.Add(this.label_doc_GUID);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "ESChecker_MP06Form";
            this.Text = "Проверка межевого плана 06";
            this.Load += new System.EventHandler(this.ESChecker_MP06Form_Load);
            this.SizeChanged += new System.EventHandler(this.ESChecker_MP06Form_SizeChanged);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.LinkLabel label_doc_GUID;
        private System.Windows.Forms.ImageList imList_dStates;
        private System.Windows.Forms.LinkLabel Label_sig_Properties;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private CXmlTreeView cXmlTreeView2;
    }
}