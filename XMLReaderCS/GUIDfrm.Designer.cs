﻿namespace XMLReaderCS
{
    partial class GUIDfrm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.linkLabel_GKUOKS = new System.Windows.Forms.LinkLabel();
            this.linkLabel_GKUZU = new System.Windows.Forms.LinkLabel();
            this.linkLabel_GUID = new System.Windows.Forms.LinkLabel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = global::XMLReaderCS.Properties.Resources.Refresh;
            this.pictureBox1.Location = new System.Drawing.Point(452, 44);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(69, 96);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // linkLabel_GKUOKS
            // 
            this.linkLabel_GKUOKS.BackColor = System.Drawing.SystemColors.Control;
            this.linkLabel_GKUOKS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.linkLabel_GKUOKS.Image = global::XMLReaderCS.Properties.Resources.building;
            this.linkLabel_GKUOKS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel_GKUOKS.Location = new System.Drawing.Point(23, 115);
            this.linkLabel_GKUOKS.Name = "linkLabel_GKUOKS";
            this.linkLabel_GKUOKS.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.linkLabel_GKUOKS.Size = new System.Drawing.Size(423, 25);
            this.linkLabel_GKUOKS.TabIndex = 20;
            this.linkLabel_GKUOKS.TabStop = true;
            this.linkLabel_GKUOKS.Text = "GKUOKS_4e80f696-d870-4234-8e52-fb613ddd0266.xml";
            this.linkLabel_GKUOKS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel_GKUOKS.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_GKUOKS_LinkClicked);
            // 
            // linkLabel_GKUZU
            // 
            this.linkLabel_GKUZU.BackColor = System.Drawing.SystemColors.Control;
            this.linkLabel_GKUZU.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.linkLabel_GKUZU.Image = global::XMLReaderCS.Properties.Resources.ЗУ_1;
            this.linkLabel_GKUZU.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel_GKUZU.Location = new System.Drawing.Point(15, 79);
            this.linkLabel_GKUZU.Name = "linkLabel_GKUZU";
            this.linkLabel_GKUZU.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.linkLabel_GKUZU.Size = new System.Drawing.Size(431, 25);
            this.linkLabel_GKUZU.TabIndex = 19;
            this.linkLabel_GKUZU.TabStop = true;
            this.linkLabel_GKUZU.Text = "GKUZU_E5BE58A2-0B57-4C9B-8255-0A92E923FA98.xml";
            this.linkLabel_GKUZU.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel_GKUZU.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_GKUZU_LinkClicked);
            // 
            // linkLabel_GUID
            // 
            this.linkLabel_GUID.BackColor = System.Drawing.SystemColors.Control;
            this.linkLabel_GUID.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.linkLabel_GUID.Image = global::XMLReaderCS.Properties.Resources.page_white_code;
            this.linkLabel_GUID.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel_GUID.Location = new System.Drawing.Point(91, 44);
            this.linkLabel_GUID.Name = "linkLabel_GUID";
            this.linkLabel_GUID.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.linkLabel_GUID.Size = new System.Drawing.Size(320, 25);
            this.linkLabel_GUID.TabIndex = 18;
            this.linkLabel_GUID.TabStop = true;
            this.linkLabel_GUID.Text = "4e80f696-d870-4234-8e52-fb613ddd0266";
            this.linkLabel_GUID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel_GUID.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_GUID_LinkClicked);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(417, 12);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(79, 17);
            this.checkBox1.TabIndex = 22;
            this.checkBox1.Text = "UpperCase";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Click += new System.EventHandler(this.checkBox1_Click);
            // 
            // GUIDfrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 171);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.linkLabel_GKUOKS);
            this.Controls.Add(this.linkLabel_GKUZU);
            this.Controls.Add(this.linkLabel_GUID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GUIDfrm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Генератор GUID ";
            this.Load += new System.EventHandler(this.GUIDfrm_Load);
            this.Shown += new System.EventHandler(this.GUIDfrm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel_GUID;
        private System.Windows.Forms.LinkLabel linkLabel_GKUZU;
        private System.Windows.Forms.LinkLabel linkLabel_GKUOKS;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}