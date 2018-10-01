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
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Node0");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Node4");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Node1", new System.Windows.Forms.TreeNode[] {
            treeNode16});
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Node2");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Node3");
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.SearchTextBox = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.treeViewSearchable1 = new netFteo.TreeViewSearchable();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(239, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(165, 59);
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
            this.treeView1.Location = new System.Drawing.Point(12, 109);
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
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode14});
            this.treeView1.Size = new System.Drawing.Size(395, 330);
            this.treeView1.TabIndex = 1;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // SearchTextBox
            // 
            this.SearchTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.SearchTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SearchTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SearchTextBox.ForeColor = System.Drawing.Color.DarkRed;
            this.SearchTextBox.Location = new System.Drawing.Point(273, 77);
            this.SearchTextBox.Name = "SearchTextBox";
            this.SearchTextBox.Size = new System.Drawing.Size(134, 26);
            this.SearchTextBox.TabIndex = 5;
            this.SearchTextBox.WordWrap = false;
            this.SearchTextBox.TextChanged += new System.EventHandler(this.SearchTextBox_TextChanged);
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
            this.treeViewSearchable1.Location = new System.Drawing.Point(423, 209);
            this.treeViewSearchable1.Name = "treeViewSearchable1";
            treeNode15.Name = "Node0";
            treeNode15.Text = "Node0";
            treeNode16.Name = "Node4";
            treeNode16.Text = "Node4";
            treeNode17.Name = "Node1";
            treeNode17.Text = "Node1";
            treeNode18.Name = "Node2";
            treeNode18.Text = "Node2";
            treeNode19.Name = "Node3";
            treeNode19.Text = "Node3";
            this.treeViewSearchable1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode15,
            treeNode17,
            treeNode18,
            treeNode19});
            this.treeViewSearchable1.Size = new System.Drawing.Size(316, 157);
            this.treeViewSearchable1.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 451);
            this.Controls.Add(this.treeViewSearchable1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.SearchTextBox);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "TExpress disasssmebler";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox SearchTextBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TreeView treeView1;
        private netFteo.TreeViewSearchable treeViewSearchable1;
    }
}

