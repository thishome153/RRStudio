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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPointEditor));
            this.textBox_x = new System.Windows.Forms.TextBox();
            this.tPointBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.textBox_z = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_Mt = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.textBox_Name = new System.Windows.Forms.TextBox();
            this.label_Name = new System.Windows.Forms.Label();
            this.placeTextBox = new System.Windows.Forms.TextBox();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.codeTextBox = new System.Windows.Forms.TextBox();
            this.yTextBox = new System.Windows.Forms.TextBox();
            this.oldXTextBox = new System.Windows.Forms.TextBox();
            this.oldYTextBox = new System.Windows.Forms.TextBox();
            this.prefTextBox = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            placeLabel = new System.Windows.Forms.Label();
            descriptionLabel = new System.Windows.Forms.Label();
            codeLabel = new System.Windows.Forms.Label();
            oldXLabel = new System.Windows.Forms.Label();
            oldYLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tPointBindingSource)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // placeLabel
            // 
            placeLabel.AutoSize = true;
            placeLabel.Location = new System.Drawing.Point(17, 235);
            placeLabel.Name = "placeLabel";
            placeLabel.Size = new System.Drawing.Size(126, 17);
            placeLabel.TabIndex = 22;
            placeLabel.Text = "Местоположение:";
            // 
            // descriptionLabel
            // 
            descriptionLabel.AutoSize = true;
            descriptionLabel.Location = new System.Drawing.Point(5, 160);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Size = new System.Drawing.Size(78, 17);
            descriptionLabel.TabIndex = 23;
            descriptionLabel.Text = "Описание:";
            // 
            // codeLabel
            // 
            codeLabel.AutoSize = true;
            codeLabel.Location = new System.Drawing.Point(330, 235);
            codeLabel.Name = "codeLabel";
            codeLabel.Size = new System.Drawing.Size(37, 17);
            codeLabel.TabIndex = 25;
            codeLabel.Text = "Код:";
            // 
            // oldXLabel
            // 
            oldXLabel.AutoSize = true;
            oldXLabel.Location = new System.Drawing.Point(139, 69);
            oldXLabel.Name = "oldXLabel";
            oldXLabel.Size = new System.Drawing.Size(17, 17);
            oldXLabel.TabIndex = 27;
            oldXLabel.Text = "X";
            // 
            // oldYLabel
            // 
            oldYLabel.AutoSize = true;
            oldYLabel.Location = new System.Drawing.Point(139, 102);
            oldYLabel.Name = "oldYLabel";
            oldYLabel.Size = new System.Drawing.Size(17, 17);
            oldYLabel.TabIndex = 28;
            oldYLabel.Text = "Y";
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
            // tPointBindingSource
            // 
            this.tPointBindingSource.DataSource = typeof(netFteo.Spatial.TPoint);
            // 
            // textBox_z
            // 
            this.textBox_z.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "z", true));
            this.textBox_z.Location = new System.Drawing.Point(370, 202);
            this.textBox_z.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_z.Name = "textBox_z";
            this.textBox_z.Size = new System.Drawing.Size(153, 23);
            this.textBox_z.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(195, 40);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Существующие";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(370, 40);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 17);
            this.label3.TabIndex = 13;
            this.label3.Text = "Новые";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(326, 204);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 17);
            this.label5.TabIndex = 15;
            this.label5.Text = "Z, м.";
            // 
            // textBox_Mt
            // 
            this.textBox_Mt.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "Mt", true));
            this.textBox_Mt.Location = new System.Drawing.Point(370, 128);
            this.textBox_Mt.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_Mt.Name = "textBox_Mt";
            this.textBox_Mt.Size = new System.Drawing.Size(153, 23);
            this.textBox_Mt.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(326, 130);
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
            this.panel1.Controls.Add(this.button_OK);
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
            // button_OK
            // 
            this.button_OK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_OK.Location = new System.Drawing.Point(107, 12);
            this.button_OK.Margin = new System.Windows.Forms.Padding(4);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(163, 43);
            this.button_OK.TabIndex = 9;
            this.button_OK.Text = "Ok";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.Button_OK_Click);
            // 
            // textBox_Name
            // 
            this.textBox_Name.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "NumGeopointA", true));
            this.textBox_Name.Location = new System.Drawing.Point(309, 12);
            this.textBox_Name.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_Name.Name = "textBox_Name";
            this.textBox_Name.Size = new System.Drawing.Size(133, 23);
            this.textBox_Name.TabIndex = 20;
            // 
            // label_Name
            // 
            this.label_Name.AutoSize = true;
            this.label_Name.Location = new System.Drawing.Point(270, 12);
            this.label_Name.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Name.Name = "label_Name";
            this.label_Name.Size = new System.Drawing.Size(35, 17);
            this.label_Name.TabIndex = 21;
            this.label_Name.Text = "Имя";
            // 
            // placeTextBox
            // 
            this.placeTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "Place", true));
            this.placeTextBox.Location = new System.Drawing.Point(149, 232);
            this.placeTextBox.Name = "placeTextBox";
            this.placeTextBox.Size = new System.Drawing.Size(177, 23);
            this.placeTextBox.TabIndex = 23;
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "Description", true));
            this.descriptionTextBox.Location = new System.Drawing.Point(94, 158);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(400, 23);
            this.descriptionTextBox.TabIndex = 24;
            // 
            // codeTextBox
            // 
            this.codeTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "Code", true));
            this.codeTextBox.Location = new System.Drawing.Point(370, 232);
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.Size = new System.Drawing.Size(153, 23);
            this.codeTextBox.TabIndex = 26;
            // 
            // yTextBox
            // 
            this.yTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "y", true));
            this.yTextBox.Location = new System.Drawing.Point(370, 96);
            this.yTextBox.Name = "yTextBox";
            this.yTextBox.Size = new System.Drawing.Size(153, 23);
            this.yTextBox.TabIndex = 27;
            // 
            // oldXTextBox
            // 
            this.oldXTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "oldX", true));
            this.oldXTextBox.Location = new System.Drawing.Point(174, 66);
            this.oldXTextBox.Name = "oldXTextBox";
            this.oldXTextBox.Size = new System.Drawing.Size(153, 23);
            this.oldXTextBox.TabIndex = 28;
            // 
            // oldYTextBox
            // 
            this.oldYTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "oldY", true));
            this.oldYTextBox.Location = new System.Drawing.Point(174, 96);
            this.oldYTextBox.Name = "oldYTextBox";
            this.oldYTextBox.Size = new System.Drawing.Size(153, 23);
            this.oldYTextBox.TabIndex = 29;
            // 
            // prefTextBox
            // 
            this.prefTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource, "Pref", true));
            this.prefTextBox.Location = new System.Drawing.Point(213, 12);
            this.prefTextBox.Name = "prefTextBox";
            this.prefTextBox.Size = new System.Drawing.Size(46, 23);
            this.prefTextBox.TabIndex = 30;
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Image = global::Traverser.Properties.Resources.cross;
            this.button3.Location = new System.Drawing.Point(333, 66);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(23, 23);
            this.button3.TabIndex = 31;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // button4
            // 
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Image = global::Traverser.Properties.Resources.cross;
            this.button4.Location = new System.Drawing.Point(333, 96);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(23, 23);
            this.button4.TabIndex = 32;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.Button4_Click);
            // 
            // button5
            // 
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Location = new System.Drawing.Point(118, 12);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(85, 24);
            this.button5.TabIndex = 33;
            this.button5.Text = "Префикс";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.Button5_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 17);
            this.label1.TabIndex = 34;
            this.label1.Text = "Координаты";
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = global::Traverser.Properties.Resources.cross;
            this.button1.Location = new System.Drawing.Point(500, 158);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(23, 23);
            this.button1.TabIndex = 35;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // frmPointEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 339);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
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
            ((System.ComponentModel.ISupportInitialize)(this.tPointBindingSource)).EndInit();
            this.panel1.ResumeLayout(false);
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
        private System.Windows.Forms.Button button_OK;
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
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}