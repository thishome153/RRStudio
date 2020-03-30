namespace GKNData
{
    partial class wzKPTProperty
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(wzKPTProperty));
            this.label_Filename = new System.Windows.Forms.Label();
            this.textBox_FileName = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_xmlns = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Number = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_Date = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_RequestNumber = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_Code = new System.Windows.Forms.TextBox();
            this.label_sizeXML = new System.Windows.Forms.Label();
            this.label_DocType = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_Filename
            // 
            this.label_Filename.AutoSize = true;
            this.label_Filename.Location = new System.Drawing.Point(4, 4);
            this.label_Filename.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Filename.Name = "label_Filename";
            this.label_Filename.Size = new System.Drawing.Size(82, 17);
            this.label_Filename.TabIndex = 0;
            this.label_Filename.Text = "Имя файла";
            // 
            // textBox_FileName
            // 
            this.textBox_FileName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_FileName.Location = new System.Drawing.Point(90, 4);
            this.textBox_FileName.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_FileName.Name = "textBox_FileName";
            this.textBox_FileName.Size = new System.Drawing.Size(290, 16);
            this.textBox_FileName.TabIndex = 1;
            this.textBox_FileName.Text = "kpt_846bc35f-9c6e-4791-9611-8d1938f8fc77.xml";
            this.textBox_FileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_FileName.TextChanged += new System.EventHandler(this.textBox_FileName_TextChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 203);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(389, 58);
            this.panel1.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(198, 11);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 37);
            this.button2.TabIndex = 3;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(80, 11);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 37);
            this.button1.TabIndex = 2;
            this.button1.Text = "Ок";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Размер, Кб";
            // 
            // textBox_xmlns
            // 
            this.textBox_xmlns.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_xmlns.Location = new System.Drawing.Point(4, 153);
            this.textBox_xmlns.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_xmlns.Multiline = true;
            this.textBox_xmlns.Name = "textBox_xmlns";
            this.textBox_xmlns.Size = new System.Drawing.Size(381, 39);
            this.textBox_xmlns.TabIndex = 6;
            this.textBox_xmlns.TextChanged += new System.EventHandler(this.textBox_xmlns_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 132);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Namespace";
            // 
            // textBox_Number
            // 
            this.textBox_Number.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_Number.Location = new System.Drawing.Point(103, 62);
            this.textBox_Number.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_Number.Name = "textBox_Number";
            this.textBox_Number.Size = new System.Drawing.Size(144, 16);
            this.textBox_Number.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 62);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Номер";
            // 
            // textBox_Date
            // 
            this.textBox_Date.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_Date.Location = new System.Drawing.Point(103, 86);
            this.textBox_Date.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_Date.Name = "textBox_Date";
            this.textBox_Date.Size = new System.Drawing.Size(144, 16);
            this.textBox_Date.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 86);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "Дата";
            // 
            // textBox_RequestNumber
            // 
            this.textBox_RequestNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_RequestNumber.Location = new System.Drawing.Point(130, 109);
            this.textBox_RequestNumber.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_RequestNumber.Name = "textBox_RequestNumber";
            this.textBox_RequestNumber.Size = new System.Drawing.Size(144, 16);
            this.textBox_RequestNumber.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 109);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 17);
            this.label5.TabIndex = 11;
            this.label5.Text = "Номер запроса";
            // 
            // textBox_Code
            // 
            this.textBox_Code.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_Code.Location = new System.Drawing.Point(282, 109);
            this.textBox_Code.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_Code.Name = "textBox_Code";
            this.textBox_Code.Size = new System.Drawing.Size(94, 16);
            this.textBox_Code.TabIndex = 13;
            // 
            // label_sizeXML
            // 
            this.label_sizeXML.BackColor = System.Drawing.SystemColors.Info;
            this.label_sizeXML.Location = new System.Drawing.Point(127, 40);
            this.label_sizeXML.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_sizeXML.Name = "label_sizeXML";
            this.label_sizeXML.Size = new System.Drawing.Size(120, 17);
            this.label_sizeXML.TabIndex = 14;
            this.label_sizeXML.Text = "::";
            this.label_sizeXML.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_DocType
            // 
            this.label_DocType.BackColor = System.Drawing.SystemColors.Info;
            this.label_DocType.Location = new System.Drawing.Point(265, 40);
            this.label_DocType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_DocType.Name = "label_DocType";
            this.label_DocType.Size = new System.Drawing.Size(120, 17);
            this.label_DocType.TabIndex = 15;
            this.label_DocType.Text = "::";
            this.label_DocType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // wzKPTProperty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(389, 261);
            this.Controls.Add(this.label_DocType);
            this.Controls.Add(this.label_sizeXML);
            this.Controls.Add(this.textBox_Code);
            this.Controls.Add(this.textBox_RequestNumber);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_Date);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_Number);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_xmlns);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBox_FileName);
            this.Controls.Add(this.label_Filename);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "wzKPTProperty";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Свойства файла КПТ";
            this.Shown += new System.EventHandler(this.wzKPTProperty_Shown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_Filename;
        private System.Windows.Forms.TextBox textBox_FileName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_xmlns;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Number;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_Date;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_RequestNumber;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_Code;
        private System.Windows.Forms.Label label_sizeXML;
        private System.Windows.Forms.Label label_DocType;
    }
}