namespace GKNData
{
    partial class wzDistrict
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_CN = new System.Windows.Forms.TextBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_FileName = new System.Windows.Forms.TextBox();
            this.label_Filename = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 153);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(635, 52);
            this.panel1.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(333, 12);
            this.button2.Margin = new System.Windows.Forms.Padding(5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 28);
            this.button2.TabIndex = 3;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(221, 12);
            this.button1.Margin = new System.Windows.Forms.Padding(5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 28);
            this.button1.TabIndex = 2;
            this.button1.Text = "Ок";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // textBox_CN
            // 
            this.textBox_CN.BackColor = System.Drawing.SystemColors.Info;
            this.textBox_CN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_CN.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource1, "Item_NameExt", true));
            this.textBox_CN.Location = new System.Drawing.Point(140, 28);
            this.textBox_CN.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_CN.Name = "textBox_CN";
            this.textBox_CN.Size = new System.Drawing.Size(75, 23);
            this.textBox_CN.TabIndex = 12;
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = typeof(GKNData.TCurrentItem);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 75);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "Наменование";
            // 
            // textBox_FileName
            // 
            this.textBox_FileName.BackColor = System.Drawing.SystemColors.Info;
            this.textBox_FileName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_FileName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource1, "Item_TypeName", true));
            this.textBox_FileName.Location = new System.Drawing.Point(140, 73);
            this.textBox_FileName.Margin = new System.Windows.Forms.Padding(5);
            this.textBox_FileName.Name = "textBox_FileName";
            this.textBox_FileName.Size = new System.Drawing.Size(478, 23);
            this.textBox_FileName.TabIndex = 10;
            this.textBox_FileName.Text = "kpt_846bc35f-9c6e-4791-9611-8d1938f8fc77.xml";
            // 
            // label_Filename
            // 
            this.label_Filename.AutoSize = true;
            this.label_Filename.Location = new System.Drawing.Point(70, 28);
            this.label_Filename.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label_Filename.Name = "label_Filename";
            this.label_Filename.Size = new System.Drawing.Size(51, 17);
            this.label_Filename.TabIndex = 9;
            this.label_Filename.Text = "Номер";
            // 
            // wzDistrict
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 205);
            this.Controls.Add(this.textBox_CN);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_FileName);
            this.Controls.Add(this.label_Filename);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "wzDistrict";
            this.Text = "Редактор кадастрового района";
            this.Shown += new System.EventHandler(this.WzDistrict_Shown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox_CN;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_FileName;
        private System.Windows.Forms.Label label_Filename;
        private System.Windows.Forms.BindingSource bindingSource1;
    }
}