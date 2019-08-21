namespace XMLReaderCS
{
    partial class frmValidator
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
			this.button1 = new System.Windows.Forms.Button();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.button2 = new System.Windows.Forms.Button();
			this.radioButton_XMLValReader = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(574, 12);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(173, 58);
			this.button1.TabIndex = 0;
			this.button1.Text = "Open schema";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// richTextBox1
			// 
			this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Left;
			this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.richTextBox1.Location = new System.Drawing.Point(0, 0);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(556, 532);
			this.richTextBox1.TabIndex = 2;
			this.richTextBox1.Text = "";
			// 
			// button2
			// 
			this.button2.Enabled = false;
			this.button2.Location = new System.Drawing.Point(574, 158);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(173, 58);
			this.button2.TabIndex = 3;
			this.button2.Text = "Valide";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click_1);
			// 
			// radioButton_XMLValReader
			// 
			this.radioButton_XMLValReader.AutoSize = true;
			this.radioButton_XMLValReader.Checked = true;
			this.radioButton_XMLValReader.Location = new System.Drawing.Point(574, 85);
			this.radioButton_XMLValReader.Name = "radioButton_XMLValReader";
			this.radioButton_XMLValReader.Size = new System.Drawing.Size(123, 17);
			this.radioButton_XMLValReader.TabIndex = 4;
			this.radioButton_XMLValReader.Text = "XmlValidatingReader";
			this.radioButton_XMLValReader.UseVisualStyleBackColor = true;
			// 
			// radioButton2
			// 
			this.radioButton2.AutoSize = true;
			this.radioButton2.Location = new System.Drawing.Point(574, 108);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(137, 17);
			this.radioButton2.TabIndex = 5;
			this.radioButton2.Text = "XmlReader / Document";
			this.radioButton2.UseVisualStyleBackColor = true;
			// 
			// frmValidator
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(755, 532);
			this.Controls.Add(this.radioButton2);
			this.Controls.Add(this.radioButton_XMLValReader);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.button1);
			this.MinimumSize = new System.Drawing.Size(771, 362);
			this.Name = "frmValidator";
			this.Text = "frmValidator";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.RadioButton radioButton_XMLValReader;
		private System.Windows.Forms.RadioButton radioButton2;
	}
}