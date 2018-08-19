namespace XMLReaderCS
{
    partial class frmOptions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
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
            this.contextMenuStrip_CopyPasteAll = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.копироватьToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listView_Schemas = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LV_SchemaDisAssembly = new System.Windows.Forms.ListView();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip_CopyPasteAll.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip_CopyPasteAll
            // 
            this.contextMenuStrip_CopyPasteAll.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.копироватьToolStripMenuItem2});
            this.contextMenuStrip_CopyPasteAll.Name = "contextMenuStrip_OIPD";
            this.contextMenuStrip_CopyPasteAll.Size = new System.Drawing.Size(161, 26);
            // 
            // копироватьToolStripMenuItem2
            // 
            this.копироватьToolStripMenuItem2.Name = "копироватьToolStripMenuItem2";
            this.копироватьToolStripMenuItem2.Size = new System.Drawing.Size(160, 22);
            this.копироватьToolStripMenuItem2.Text = "Копировать все";
            this.копироватьToolStripMenuItem2.Click += new System.EventHandler(this.копироватьToolStripMenuItem2_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 514);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(978, 56);
            this.panel1.TabIndex = 10;
            // 
            // button1
            // 
            this.button1.BackgroundImage = global::XMLReaderCS.Properties.Resources.thatch;
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(579, 11);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(146, 41);
            this.button1.TabIndex = 9;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listView_Schemas);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.LV_SchemaDisAssembly);
            this.splitContainer1.Size = new System.Drawing.Size(978, 514);
            this.splitContainer1.SplitterDistance = 575;
            this.splitContainer1.TabIndex = 12;
            // 
            // listView_Schemas
            // 
            this.listView_Schemas.BackColor = System.Drawing.SystemColors.Info;
            this.listView_Schemas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView_Schemas.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView_Schemas.ContextMenuStrip = this.contextMenuStrip_CopyPasteAll;
            this.listView_Schemas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_Schemas.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listView_Schemas.FullRowSelect = true;
            this.listView_Schemas.GridLines = true;
            this.listView_Schemas.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView_Schemas.HideSelection = false;
            listViewItem1.StateImageIndex = 0;
            listViewItem2.StateImageIndex = 0;
            listViewItem3.StateImageIndex = 0;
            this.listView_Schemas.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3});
            this.listView_Schemas.Location = new System.Drawing.Point(0, 0);
            this.listView_Schemas.Name = "listView_Schemas";
            this.listView_Schemas.Size = new System.Drawing.Size(575, 514);
            this.listView_Schemas.TabIndex = 12;
            this.listView_Schemas.UseCompatibleStateImageBehavior = false;
            this.listView_Schemas.View = System.Windows.Forms.View.Details;
            this.listView_Schemas.Click += new System.EventHandler(this.listView_Schemas_Click);
            this.listView_Schemas.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_Schemas_KeyUp);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Root element name";
            this.columnHeader1.Width = 61;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "TargetNameSpace";
            this.columnHeader2.Width = 150;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Путь";
            this.columnHeader3.Width = 300;
            // 
            // LV_SchemaDisAssembly
            // 
            this.LV_SchemaDisAssembly.BackColor = System.Drawing.SystemColors.Info;
            this.LV_SchemaDisAssembly.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LV_SchemaDisAssembly.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader15,
            this.columnHeader16});
            this.LV_SchemaDisAssembly.ContextMenuStrip = this.contextMenuStrip_CopyPasteAll;
            this.LV_SchemaDisAssembly.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV_SchemaDisAssembly.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LV_SchemaDisAssembly.FullRowSelect = true;
            this.LV_SchemaDisAssembly.GridLines = true;
            this.LV_SchemaDisAssembly.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LV_SchemaDisAssembly.HideSelection = false;
            listViewItem4.StateImageIndex = 0;
            listViewItem5.StateImageIndex = 0;
            listViewItem6.StateImageIndex = 0;
            this.LV_SchemaDisAssembly.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem4,
            listViewItem5,
            listViewItem6});
            this.LV_SchemaDisAssembly.Location = new System.Drawing.Point(0, 0);
            this.LV_SchemaDisAssembly.Name = "LV_SchemaDisAssembly";
            this.LV_SchemaDisAssembly.Size = new System.Drawing.Size(399, 514);
            this.LV_SchemaDisAssembly.TabIndex = 10;
            this.LV_SchemaDisAssembly.UseCompatibleStateImageBehavior = false;
            this.LV_SchemaDisAssembly.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Имя";
            this.columnHeader10.Width = 200;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "Тип";
            this.columnHeader15.Width = 151;
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "Hash";
            this.columnHeader16.Width = 300;
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(978, 570);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOptions";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки";
            this.Load += new System.EventHandler(this.frmOptions_Load);
            this.contextMenuStrip_CopyPasteAll.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_CopyPasteAll;
        private System.Windows.Forms.ToolStripMenuItem копироватьToolStripMenuItem2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listView_Schemas;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ListView LV_SchemaDisAssembly;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.ColumnHeader columnHeader16;


    }
}