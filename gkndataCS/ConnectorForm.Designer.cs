
namespace GKNData
{
    partial class ConnectorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectorForm));
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.comboBox_Database = new System.Windows.Forms.ComboBox();
            this.comboBox_CharSet = new System.Windows.Forms.ComboBox();
            this.textBox_pswrd = new System.Windows.Forms.TextBox();
            this.comboBox_server = new System.Windows.Forms.ComboBox();
            this.comboBox_UserName = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox_Port
            // 
            this.textBox_Port.Location = new System.Drawing.Point(186, 32);
            this.textBox_Port.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_Port.Name = "textBox_Port";
            this.textBox_Port.Size = new System.Drawing.Size(75, 23);
            this.textBox_Port.TabIndex = 0;
            this.textBox_Port.Text = "3306";
            // 
            // comboBox_Database
            // 
            this.comboBox_Database.FormattingEnabled = true;
            this.comboBox_Database.Items.AddRange(new object[] {
            "gkn_test"});
            this.comboBox_Database.Location = new System.Drawing.Point(272, 32);
            this.comboBox_Database.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox_Database.Name = "comboBox_Database";
            this.comboBox_Database.Size = new System.Drawing.Size(160, 24);
            this.comboBox_Database.TabIndex = 1;
            this.comboBox_Database.Text = "gkn_test";
            // 
            // comboBox_CharSet
            // 
            this.comboBox_CharSet.FormattingEnabled = true;
            this.comboBox_CharSet.Items.AddRange(new object[] {
            "UTF8"});
            this.comboBox_CharSet.Location = new System.Drawing.Point(272, 81);
            this.comboBox_CharSet.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox_CharSet.Name = "comboBox_CharSet";
            this.comboBox_CharSet.Size = new System.Drawing.Size(160, 24);
            this.comboBox_CharSet.TabIndex = 2;
            this.comboBox_CharSet.Text = "UTF8";
            // 
            // textBox_pswrd
            // 
            this.textBox_pswrd.Location = new System.Drawing.Point(186, 81);
            this.textBox_pswrd.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_pswrd.Name = "textBox_pswrd";
            this.textBox_pswrd.Size = new System.Drawing.Size(75, 23);
            this.textBox_pswrd.TabIndex = 3;
            this.textBox_pswrd.Text = "123456";
            // 
            // comboBox_server
            // 
            this.comboBox_server.FormattingEnabled = true;
            this.comboBox_server.Items.AddRange(new object[] {
            "10.66.77.4",
            "ns.geocom.lan",
            "10.66.77.47",
            "c7.lan.geo-complex.com",
            "localhost",
            "127.0.0.1"});
            this.comboBox_server.Location = new System.Drawing.Point(18, 32);
            this.comboBox_server.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox_server.Name = "comboBox_server";
            this.comboBox_server.Size = new System.Drawing.Size(160, 24);
            this.comboBox_server.TabIndex = 4;
            this.comboBox_server.Text = "10.66.77.4";
            // 
            // comboBox_UserName
            // 
            this.comboBox_UserName.FormattingEnabled = true;
            this.comboBox_UserName.Items.AddRange(new object[] {
            "gkndata_u1",
            "gd_u1"});
            this.comboBox_UserName.Location = new System.Drawing.Point(18, 80);
            this.comboBox_UserName.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox_UserName.Name = "comboBox_UserName";
            this.comboBox_UserName.Size = new System.Drawing.Size(160, 24);
            this.comboBox_UserName.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.button1.Image = global::GKNData.Properties.Resources.MySQL_24х24;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(97, 128);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(132, 39);
            this.button1.TabIndex = 6;
            this.button1.Text = "Подключить";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(230, 128);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(128, 39);
            this.button2.TabIndex = 7;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Сервер";
            // 
            // ConnectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 180);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox_UserName);
            this.Controls.Add(this.comboBox_server);
            this.Controls.Add(this.textBox_pswrd);
            this.Controls.Add(this.comboBox_CharSet);
            this.Controls.Add(this.comboBox_Database);
            this.Controls.Add(this.textBox_Port);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ConnectorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Выбор базы данных";
            this.Shown += new System.EventHandler(this.ConnectorForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion




        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.ComboBox comboBox_Database;
        private System.Windows.Forms.ComboBox comboBox_CharSet;
        private System.Windows.Forms.TextBox textBox_pswrd;
        private System.Windows.Forms.ComboBox comboBox_server;
        private System.Windows.Forms.ComboBox comboBox_UserName;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
    }
}