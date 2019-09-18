namespace XMLReaderCS
{
    partial class frmPointEditor
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
            System.Windows.Forms.Label placeLabel;
            System.Windows.Forms.Label descriptionLabel;
            System.Windows.Forms.Label codeLabel;
            System.Windows.Forms.Label oldXLabel;
            System.Windows.Forms.Label oldYLabel;
            System.Windows.Forms.Label prefLabel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPointEditor));
            this.textBox_x = new System.Windows.Forms.TextBox();
            this.textBox_z = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_Mt = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_Name = new System.Windows.Forms.TextBox();
            this.label_Name = new System.Windows.Forms.Label();
            this.placeTextBox = new System.Windows.Forms.TextBox();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.codeTextBox = new System.Windows.Forms.TextBox();
            this.yTextBox = new System.Windows.Forms.TextBox();
            this.oldXTextBox = new System.Windows.Forms.TextBox();
            this.oldYTextBox = new System.Windows.Forms.TextBox();
            this.prefTextBox = new System.Windows.Forms.TextBox();
            this.tPointBindingSource = new System.Windows.Forms.BindingSource(this.components);
            placeLabel = new System.Windows.Forms.Label();
            descriptionLabel = new System.Windows.Forms.Label();
            codeLabel = new System.Windows.Forms.Label();
            oldXLabel = new System.Windows.Forms.Label();
            oldYLabel = new System.Windows.Forms.Label();
            prefLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tPointBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // placeLabel
            // 
            placeLabel.AutoSize = true;
            placeLabel.Location = new System.Drawing.Point(17, 226);
            placeLabel.Name = "placeLabel";
            placeLabel.Size = new System.Drawing.Size(47, 17);
            placeLabel.TabIndex = 22;
            placeLabel.Text = "Place:";
            // 
            // descriptionLabel
            // 
            descriptionLabel.AutoSize = true;
            descriptionLabel.Location = new System.Drawing.Point(4, 192);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Size = new System.Drawing.Size(83, 17);
            descriptionLabel.TabIndex = 23;
            descriptionLabel.Text = "Description:";
            // 
            // codeLabel
            // 
            codeLabel.AutoSize = true;
            codeLabel.Location = new System.Drawing.Point(296, 192);
            codeLabel.Name = "codeLabel";
            codeLabel.Size = new System.Drawing.Size(45, 17);
            codeLabel.TabIndex = 25;
            codeLabel.Text = "Code:";
            // 
            // oldXLabel
            // 
            oldXLabel.AutoSize = true;
            oldXLabel.Location = new System.Drawing.Point(177, 69);
            oldXLabel.Name = "oldXLabel";
            oldXLabel.Size = new System.Drawing.Size(17, 17);
            oldXLabel.TabIndex = 27;
            oldXLabel.Text = "X";
            // 
            // oldYLabel
            // 
            oldYLabel.AutoSize = true;
            oldYLabel.Location = new System.Drawing.Point(177, 102);
            oldYLabel.Name = "oldYLabel";
            oldYLabel.Size = new System.Drawing.Size(17, 17);
            oldYLabel.TabIndex = 28;
            oldYLabel.Text = "Y";
            // 
            // prefLabel
            // 
            prefLabel.AutoSize = true;
            prefLabel.Location = new System.Drawing.Point(4, 13);
            prefLabel.Name = "prefLabel";
            prefLabel.Size = new System.Drawing.Size(67, 17);
            prefLabel.TabIndex = 29;
            prefLabel.Text = "Префикс";
            // 
            // textBox_x
            // 
            this.textBox_x.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "x", true));
            this.textBox_x.Location = new System.Drawing.Point(370, 66);
            this.textBox_x.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_x.Name = "textBox_x";
            this.textBox_x.Size = new System.Drawing.Size(153, 23);
            this.textBox_x.TabIndex = 2;
            // 
            // textBox_z
            // 
            this.textBox_z.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "z", true));
            this.textBox_z.Location = new System.Drawing.Point(370, 128);
            this.textBox_z.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_z.Name = "textBox_z";
            this.textBox_z.Size = new System.Drawing.Size(153, 23);
            this.textBox_z.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(233, 40);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Существующие";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(367, 40);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 17);
            this.label3.TabIndex = 13;
            this.label3.Text = "Новые";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(303, 129);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 17);
            this.label5.TabIndex = 15;
            this.label5.Text = "Z, м.";
            // 
            // textBox_Mt
            // 
            this.textBox_Mt.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "Mt", true));
            this.textBox_Mt.Location = new System.Drawing.Point(370, 159);
            this.textBox_Mt.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_Mt.Name = "textBox_Mt";
            this.textBox_Mt.Size = new System.Drawing.Size(153, 23);
            this.textBox_Mt.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(303, 160);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 17);
            this.label6.TabIndex = 17;
            this.label6.Text = "Mt, м.";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 269);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(534, 70);
            this.panel1.TabIndex = 19;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(278, 12);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(163, 43);
            this.button2.TabIndex = 10;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(107, 12);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(163, 43);
            this.button1.TabIndex = 9;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // textBox_Name
            // 
            this.textBox_Name.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "NumGeopointA", true));
            this.textBox_Name.Location = new System.Drawing.Point(182, 13);
            this.textBox_Name.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_Name.Name = "textBox_Name";
            this.textBox_Name.Size = new System.Drawing.Size(133, 23);
            this.textBox_Name.TabIndex = 20;
            // 
            // label_Name
            // 
            this.label_Name.AutoSize = true;
            this.label_Name.Location = new System.Drawing.Point(139, 13);
            this.label_Name.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Name.Name = "label_Name";
            this.label_Name.Size = new System.Drawing.Size(35, 17);
            this.label_Name.TabIndex = 21;
            this.label_Name.Text = "Имя";
            // 
            // placeTextBox
            // 
            this.placeTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "Place", true));
            this.placeTextBox.Location = new System.Drawing.Point(94, 226);
            this.placeTextBox.Name = "placeTextBox";
            this.placeTextBox.Size = new System.Drawing.Size(177, 23);
            this.placeTextBox.TabIndex = 23;
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "Description", true));
            this.descriptionTextBox.Location = new System.Drawing.Point(93, 192);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(177, 23);
            this.descriptionTextBox.TabIndex = 24;
            // 
            // codeTextBox
            // 
            this.codeTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "Code", true));
            this.codeTextBox.Location = new System.Drawing.Point(370, 190);
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.Size = new System.Drawing.Size(153, 23);
            this.codeTextBox.TabIndex = 26;
            // 
            // yTextBox
            // 
            this.yTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "y", true));
            this.yTextBox.Location = new System.Drawing.Point(370, 97);
            this.yTextBox.Name = "yTextBox";
            this.yTextBox.Size = new System.Drawing.Size(153, 23);
            this.yTextBox.TabIndex = 27;
            // 
            // oldXTextBox
            // 
            this.oldXTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "oldX", true));
            this.oldXTextBox.Location = new System.Drawing.Point(212, 66);
            this.oldXTextBox.Name = "oldXTextBox";
            this.oldXTextBox.Size = new System.Drawing.Size(153, 23);
            this.oldXTextBox.TabIndex = 28;
            // 
            // oldYTextBox
            // 
            this.oldYTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "oldY", true));
            this.oldYTextBox.Location = new System.Drawing.Point(212, 96);
            this.oldYTextBox.Name = "oldYTextBox";
            this.oldYTextBox.Size = new System.Drawing.Size(153, 23);
            this.oldYTextBox.TabIndex = 29;
            // 
            // prefTextBox
            // 
            this.prefTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "Pref", true));
            this.prefTextBox.Location = new System.Drawing.Point(77, 13);
            this.prefTextBox.Name = "prefTextBox";
            this.prefTextBox.Size = new System.Drawing.Size(46, 23);
            this.prefTextBox.TabIndex = 30;
            // 
            // tPointBindingSource
            // 
            this.tPointBindingSource.DataSource = typeof(netFteo.Spatial.TPoint);
            // 
            // frmPointEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 339);
            this.Controls.Add(prefLabel);
            this.Controls.Add(this.prefTextBox);
            this.Controls.Add(oldYLabel);
            this.Controls.Add(this.oldYTextBox);
            this.Controls.Add(oldXLabel);
            this.Controls.Add(this.oldXTextBox);
            this.Controls.Add(this.yTextBox);
            this.Controls.Add(codeLabel);
            this.Controls.Add(this.codeTextBox);
            this.Controls.Add(descriptionLabel);
            this.Controls.Add(this.descriptionTextBox);
            this.Controls.Add(placeLabel);
            this.Controls.Add(this.placeTextBox);
            this.Controls.Add(this.label_Name);
            this.Controls.Add(this.textBox_Name);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox_Mt);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_z);
            this.Controls.Add(this.textBox_x);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(550, 300);
            this.Name = "frmPointEditor";
            this.ShowInTaskbar = false;
            this.Text = "Редактор Точка";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tPointBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox_x;
        private System.Windows.Forms.TextBox textBox_z;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_Mt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox_Name;
        private System.Windows.Forms.Label label_Name;
        private System.Windows.Forms.BindingSource tPointBindingSource;
        private System.Windows.Forms.TextBox placeTextBox;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.TextBox codeTextBox;
        private System.Windows.Forms.TextBox yTextBox;
        private System.Windows.Forms.TextBox oldXTextBox;
        private System.Windows.Forms.TextBox oldYTextBox;
        private System.Windows.Forms.TextBox prefTextBox;
    }
}