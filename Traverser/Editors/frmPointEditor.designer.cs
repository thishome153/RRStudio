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
            System.Windows.Forms.Label oldYLabel;
            System.Windows.Forms.Label yLabel;
            System.Windows.Forms.Label zLabel;
            System.Windows.Forms.Label descriptionLabel;
            System.Windows.Forms.Label definitionLabel;
            System.Windows.Forms.Label placeLabel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPointEditor));
            this.textBox_x = new System.Windows.Forms.TextBox();
            this.textBox_z = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_Mt = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_Name = new System.Windows.Forms.TextBox();
            this.label_Name = new System.Windows.Forms.Label();
            this.tPointBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tPointBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.oldYTextBox = new System.Windows.Forms.TextBox();
            this.yTextBox = new System.Windows.Forms.TextBox();
            this.zTextBox = new System.Windows.Forms.TextBox();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.definitionTextBox = new System.Windows.Forms.TextBox();
            this.placeTextBox = new System.Windows.Forms.TextBox();
            oldYLabel = new System.Windows.Forms.Label();
            yLabel = new System.Windows.Forms.Label();
            zLabel = new System.Windows.Forms.Label();
            descriptionLabel = new System.Windows.Forms.Label();
            definitionLabel = new System.Windows.Forms.Label();
            placeLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tPointBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tPointBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox_x
            // 
            this.textBox_x.Location = new System.Drawing.Point(184, 50);
            this.textBox_x.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_x.Name = "textBox_x";
            this.textBox_x.Size = new System.Drawing.Size(163, 23);
            this.textBox_x.TabIndex = 2;
            // 
            // textBox_z
            // 
            this.textBox_z.Location = new System.Drawing.Point(184, 114);
            this.textBox_z.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_z.Name = "textBox_z";
            this.textBox_z.Size = new System.Drawing.Size(132, 23);
            this.textBox_z.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(138, 53);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "X, м.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(239, 15);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Существующие";
            // 
            // textBox4
            // 
            this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource1, "oldX", true));
            this.textBox4.Location = new System.Drawing.Point(358, 50);
            this.textBox4.Margin = new System.Windows.Forms.Padding(4);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(163, 23);
            this.textBox4.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(355, 15);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 17);
            this.label3.TabIndex = 13;
            this.label3.Text = "Новые";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(138, 119);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 17);
            this.label5.TabIndex = 15;
            this.label5.Text = "Z, м.";
            // 
            // textBox_Mt
            // 
            this.textBox_Mt.Location = new System.Drawing.Point(184, 148);
            this.textBox_Mt.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_Mt.Name = "textBox_Mt";
            this.textBox_Mt.Size = new System.Drawing.Size(101, 23);
            this.textBox_Mt.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(132, 150);
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
            this.panel1.Location = new System.Drawing.Point(0, 236);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(553, 70);
            this.panel1.TabIndex = 19;
            // 
            // button2
            // 
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
            this.textBox_Name.Location = new System.Drawing.Point(59, 15);
            this.textBox_Name.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_Name.Name = "textBox_Name";
            this.textBox_Name.Size = new System.Drawing.Size(95, 23);
            this.textBox_Name.TabIndex = 20;
            // 
            // label_Name
            // 
            this.label_Name.AutoSize = true;
            this.label_Name.Location = new System.Drawing.Point(13, 15);
            this.label_Name.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Name.Name = "label_Name";
            this.label_Name.Size = new System.Drawing.Size(35, 17);
            this.label_Name.TabIndex = 21;
            this.label_Name.Text = "Имя";
            // 
            // tPointBindingSource
            // 
            this.tPointBindingSource.DataSource = typeof(netFteo.Spatial.TPoint);
            // 
            // tPointBindingSource1
            // 
            this.tPointBindingSource1.DataSource = typeof(netFteo.Spatial.TPoint);
            // 
            // oldYLabel
            // 
            oldYLabel.AutoSize = true;
            oldYLabel.Location = new System.Drawing.Point(134, 87);
            oldYLabel.Name = "oldYLabel";
            oldYLabel.Size = new System.Drawing.Size(44, 17);
            oldYLabel.TabIndex = 21;
            oldYLabel.Text = "old Y:";
            // 
            // oldYTextBox
            // 
            this.oldYTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource1, "oldY", true));
            this.oldYTextBox.Location = new System.Drawing.Point(184, 84);
            this.oldYTextBox.Name = "oldYTextBox";
            this.oldYTextBox.Size = new System.Drawing.Size(163, 23);
            this.oldYTextBox.TabIndex = 22;
            // 
            // yLabel
            // 
            yLabel.AutoSize = true;
            yLabel.Location = new System.Drawing.Point(362, 84);
            yLabel.Name = "yLabel";
            yLabel.Size = new System.Drawing.Size(19, 17);
            yLabel.TabIndex = 22;
            yLabel.Text = "y:";
            // 
            // yTextBox
            // 
            this.yTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource1, "y", true));
            this.yTextBox.Location = new System.Drawing.Point(387, 81);
            this.yTextBox.Name = "yTextBox";
            this.yTextBox.Size = new System.Drawing.Size(134, 23);
            this.yTextBox.TabIndex = 23;
            // 
            // zLabel
            // 
            zLabel.AutoSize = true;
            zLabel.Location = new System.Drawing.Point(396, 122);
            zLabel.Name = "zLabel";
            zLabel.Size = new System.Drawing.Size(19, 17);
            zLabel.TabIndex = 23;
            zLabel.Text = "z:";
            // 
            // zTextBox
            // 
            this.zTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource1, "z", true));
            this.zTextBox.Location = new System.Drawing.Point(421, 119);
            this.zTextBox.Name = "zTextBox";
            this.zTextBox.Size = new System.Drawing.Size(100, 23);
            this.zTextBox.TabIndex = 24;
            // 
            // descriptionLabel
            // 
            descriptionLabel.AutoSize = true;
            descriptionLabel.Location = new System.Drawing.Point(52, 184);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Size = new System.Drawing.Size(83, 17);
            descriptionLabel.TabIndex = 24;
            descriptionLabel.Text = "Description:";
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource1, "Description", true));
            this.descriptionTextBox.Location = new System.Drawing.Point(141, 181);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(144, 23);
            this.descriptionTextBox.TabIndex = 25;
            // 
            // definitionLabel
            // 
            definitionLabel.AutoSize = true;
            definitionLabel.Location = new System.Drawing.Point(310, 153);
            definitionLabel.Name = "definitionLabel";
            definitionLabel.Size = new System.Drawing.Size(71, 17);
            definitionLabel.TabIndex = 25;
            definitionLabel.Text = "Definition:";
            // 
            // definitionTextBox
            // 
            this.definitionTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource1, "Definition", true));
            this.definitionTextBox.Location = new System.Drawing.Point(387, 150);
            this.definitionTextBox.Name = "definitionTextBox";
            this.definitionTextBox.Size = new System.Drawing.Size(100, 23);
            this.definitionTextBox.TabIndex = 26;
            // 
            // placeLabel
            // 
            placeLabel.AutoSize = true;
            placeLabel.Location = new System.Drawing.Point(343, 196);
            placeLabel.Name = "placeLabel";
            placeLabel.Size = new System.Drawing.Size(47, 17);
            placeLabel.TabIndex = 26;
            placeLabel.Text = "Place:";
            // 
            // placeTextBox
            // 
            this.placeTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tPointBindingSource1, "Place", true));
            this.placeTextBox.Location = new System.Drawing.Point(396, 193);
            this.placeTextBox.Name = "placeTextBox";
            this.placeTextBox.Size = new System.Drawing.Size(100, 23);
            this.placeTextBox.TabIndex = 27;
            // 
            // frmPointEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 306);
            this.Controls.Add(placeLabel);
            this.Controls.Add(this.placeTextBox);
            this.Controls.Add(definitionLabel);
            this.Controls.Add(this.definitionTextBox);
            this.Controls.Add(descriptionLabel);
            this.Controls.Add(this.descriptionTextBox);
            this.Controls.Add(zLabel);
            this.Controls.Add(this.zTextBox);
            this.Controls.Add(yLabel);
            this.Controls.Add(this.yTextBox);
            this.Controls.Add(oldYLabel);
            this.Controls.Add(this.oldYTextBox);
            this.Controls.Add(this.label_Name);
            this.Controls.Add(this.textBox_Name);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox_Mt);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
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
            ((System.ComponentModel.ISupportInitialize)(this.tPointBindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox_x;
        private System.Windows.Forms.TextBox textBox_z;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_Mt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox_Name;
        private System.Windows.Forms.Label label_Name;
        private System.Windows.Forms.BindingSource tPointBindingSource1;
        private System.Windows.Forms.BindingSource tPointBindingSource;
        private System.Windows.Forms.TextBox oldYTextBox;
        private System.Windows.Forms.TextBox yTextBox;
        private System.Windows.Forms.TextBox zTextBox;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.TextBox definitionTextBox;
        private System.Windows.Forms.TextBox placeTextBox;
    }
}