namespace TExpress.DisAss
{
    partial class Form1
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("26:01:02");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("26:01:020304:888");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("26:01:020304:1000");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("26:01:020304:777");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("26:01:020304", new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode3,
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("26:01");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("26:01:020305");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("26:01:020301");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("26:01:020302");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("26:01:020303");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("26:01:020309");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("26:01:020310");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("26:01:020405");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("26", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11,
            treeNode12,
            treeNode13});
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("26312:090905:222");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("26312:090905", new System.Windows.Forms.TreeNode[] {
            treeNode15});
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("26312", new System.Windows.Forms.TreeNode[] {
            treeNode16});
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("263");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("comm");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("28:000400:222", new System.Windows.Forms.TreeNode[] {
            treeNode19});
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("28:000400", new System.Windows.Forms.TreeNode[] {
            treeNode20});
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("28", new System.Windows.Forms.TreeNode[] {
            treeNode21});
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("261");
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("2631");
            System.Windows.Forms.TreeNode treeNode25 = new System.Windows.Forms.TreeNode("271");
            System.Windows.Forms.TreeNode treeNode26 = new System.Windows.Forms.TreeNode("Node0");
            System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("Node4");
            System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("Node1", new System.Windows.Forms.TreeNode[] {
            treeNode27});
            System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("Node2");
            System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("Node3");
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.treeViewSearchable1 = new netFteo.Windows.TreeViewSearchable();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SearchTextBox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(720, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 37);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start pkcs11";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // treeView1
            // 
            this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(12, 194);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Node2";
            treeNode1.Text = "26:01:02";
            treeNode2.Name = "Node0";
            treeNode2.Text = "26:01:020304:888";
            treeNode3.Name = "Node5";
            treeNode3.Text = "26:01:020304:1000";
            treeNode4.Name = "Node2";
            treeNode4.Text = "26:01:020304:777";
            treeNode5.Name = "Node3";
            treeNode5.Text = "26:01:020304";
            treeNode6.Name = "Node1";
            treeNode6.Text = "26:01";
            treeNode7.Name = "Node4";
            treeNode7.Text = "26:01:020305";
            treeNode8.Name = "Node6";
            treeNode8.Text = "26:01:020301";
            treeNode9.Name = "Node7";
            treeNode9.Text = "26:01:020302";
            treeNode10.Name = "Node8";
            treeNode10.Text = "26:01:020303";
            treeNode11.Name = "Node3";
            treeNode11.Text = "26:01:020309";
            treeNode12.Name = "Node4";
            treeNode12.Text = "26:01:020310";
            treeNode13.Name = "Node5";
            treeNode13.Text = "26:01:020405";
            treeNode14.Name = "Node0";
            treeNode14.Text = "26";
            treeNode15.Name = "Node2";
            treeNode15.Text = "26312:090905:222";
            treeNode16.Name = "Node5";
            treeNode16.Text = "26312:090905";
            treeNode17.Name = "Node1";
            treeNode17.Text = "26312";
            treeNode18.Name = "Node0";
            treeNode18.Text = "263";
            treeNode19.Name = "Node3";
            treeNode19.Text = "comm";
            treeNode20.Name = "Node1";
            treeNode20.Text = "28:000400:222";
            treeNode21.Name = "Node4";
            treeNode21.Text = "28:000400";
            treeNode22.Name = "Node0";
            treeNode22.Text = "28";
            treeNode23.Name = "Node2";
            treeNode23.Text = "261";
            treeNode24.Name = "Node0";
            treeNode24.Text = "2631";
            treeNode25.Name = "Node2";
            treeNode25.Text = "271";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode14,
            treeNode17,
            treeNode18,
            treeNode22,
            treeNode23,
            treeNode24,
            treeNode25});
            this.treeView1.Size = new System.Drawing.Size(395, 245);
            this.treeView1.TabIndex = 1;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(413, 109);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(397, 79);
            this.textBox1.TabIndex = 6;
            // 
            // treeViewSearchable1
            // 
            this.treeViewSearchable1.BackColor = System.Drawing.Color.Lime;
            this.treeViewSearchable1.Location = new System.Drawing.Point(413, 194);
            this.treeViewSearchable1.Name = "treeViewSearchable1";
            treeNode26.Name = "Node0";
            treeNode26.Text = "Node0";
            treeNode27.Name = "Node4";
            treeNode27.Text = "Node4";
            treeNode28.Name = "Node1";
            treeNode28.Text = "Node1";
            treeNode29.Name = "Node2";
            treeNode29.Text = "Node2";
            treeNode30.Name = "Node3";
            treeNode30.Text = "Node3";
            this.treeViewSearchable1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode26,
            treeNode28,
            treeNode29,
            treeNode30});
            this.treeViewSearchable1.Size = new System.Drawing.Size(397, 245);
            this.treeViewSearchable1.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.SearchTextBox);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Location = new System.Drawing.Point(186, 150);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(221, 38);
            this.panel1.TabIndex = 9;
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
            this.button2.Image = global::TExpress.DisAss.Properties.Resources.arrow_right;
            this.button2.Location = new System.Drawing.Point(180, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(39, 36);
            this.button2.TabIndex = 10;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 451);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.treeViewSearchable1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "TExpress disasssmebler";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TreeView treeView1;
        private netFteo.Windows.TreeViewSearchable treeViewSearchable1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox SearchTextBox;
    }
}

