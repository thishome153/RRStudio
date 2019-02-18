
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBox_UserName = new System.Windows.Forms.ComboBox();
			this.comboBox_server = new System.Windows.Forms.ComboBox();
			this.textBox_pswrd = new System.Windows.Forms.TextBox();
			this.comboBox_CharSet = new System.Windows.Forms.ComboBox();
			this.comboBox_Database = new System.Windows.Forms.ComboBox();
			this.textBox_Port = new System.Windows.Forms.TextBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(684, 362);
			this.tabControl1.TabIndex = 9;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.label7);
			this.tabPage1.Controls.Add(this.label6);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this.comboBox_UserName);
			this.tabPage1.Controls.Add(this.comboBox_server);
			this.tabPage1.Controls.Add(this.textBox_pswrd);
			this.tabPage1.Controls.Add(this.comboBox_CharSet);
			this.tabPage1.Controls.Add(this.comboBox_Database);
			this.tabPage1.Controls.Add(this.textBox_Port);
			this.tabPage1.Location = new System.Drawing.Point(4, 25);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(676, 333);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Настройки базы данных";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(20, 84);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(131, 17);
			this.label7.TabIndex = 22;
			this.label7.Text = "Имя пользователя";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(188, 84);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(57, 17);
			this.label6.TabIndex = 21;
			this.label6.Text = "Пароль";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(274, 85);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(79, 17);
			this.label5.TabIndex = 20;
			this.label5.Text = "Кодировка";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(288, 23);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(124, 17);
			this.label4.TabIndex = 19;
			this.label4.Text = "Имя базы данных";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(188, 23);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(41, 17);
			this.label3.TabIndex = 18;
			this.label3.Text = "Порт";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(28, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 17);
			this.label1.TabIndex = 17;
			this.label1.Text = "Сервер";
			// 
			// comboBox_UserName
			// 
			this.comboBox_UserName.FormattingEnabled = true;
			this.comboBox_UserName.Items.AddRange(new object[] {
            "gkndata_u1",
            "gd_u1"});
			this.comboBox_UserName.Location = new System.Drawing.Point(23, 105);
			this.comboBox_UserName.Margin = new System.Windows.Forms.Padding(4);
			this.comboBox_UserName.Name = "comboBox_UserName";
			this.comboBox_UserName.Size = new System.Drawing.Size(160, 24);
			this.comboBox_UserName.TabIndex = 14;
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
			this.comboBox_server.Location = new System.Drawing.Point(23, 44);
			this.comboBox_server.Margin = new System.Windows.Forms.Padding(4);
			this.comboBox_server.Name = "comboBox_server";
			this.comboBox_server.Size = new System.Drawing.Size(160, 24);
			this.comboBox_server.TabIndex = 13;
			this.comboBox_server.Text = "10.66.77.4";
			// 
			// textBox_pswrd
			// 
			this.textBox_pswrd.Location = new System.Drawing.Point(191, 106);
			this.textBox_pswrd.Margin = new System.Windows.Forms.Padding(4);
			this.textBox_pswrd.Name = "textBox_pswrd";
			this.textBox_pswrd.Size = new System.Drawing.Size(75, 23);
			this.textBox_pswrd.TabIndex = 12;
			this.textBox_pswrd.Text = "123456";
			// 
			// comboBox_CharSet
			// 
			this.comboBox_CharSet.FormattingEnabled = true;
			this.comboBox_CharSet.Items.AddRange(new object[] {
            "UTF8"});
			this.comboBox_CharSet.Location = new System.Drawing.Point(277, 106);
			this.comboBox_CharSet.Margin = new System.Windows.Forms.Padding(4);
			this.comboBox_CharSet.Name = "comboBox_CharSet";
			this.comboBox_CharSet.Size = new System.Drawing.Size(160, 24);
			this.comboBox_CharSet.TabIndex = 11;
			this.comboBox_CharSet.Text = "UTF8";
			// 
			// comboBox_Database
			// 
			this.comboBox_Database.FormattingEnabled = true;
			this.comboBox_Database.Items.AddRange(new object[] {
            "gkn_test",
            "gkn_shadow",
            "gkndatabase"});
			this.comboBox_Database.Location = new System.Drawing.Point(277, 44);
			this.comboBox_Database.Margin = new System.Windows.Forms.Padding(4);
			this.comboBox_Database.Name = "comboBox_Database";
			this.comboBox_Database.Size = new System.Drawing.Size(160, 24);
			this.comboBox_Database.TabIndex = 10;
			this.comboBox_Database.Text = "gkn_test";
			// 
			// textBox_Port
			// 
			this.textBox_Port.Location = new System.Drawing.Point(191, 44);
			this.textBox_Port.Margin = new System.Windows.Forms.Padding(4);
			this.textBox_Port.Name = "textBox_Port";
			this.textBox_Port.Size = new System.Drawing.Size(75, 23);
			this.textBox_Port.TabIndex = 9;
			this.textBox_Port.Text = "3306";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.label2);
			this.tabPage2.Location = new System.Drawing.Point(4, 25);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(676, 333);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Общие";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 17);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 17);
			this.label2.TabIndex = 18;
			this.label2.Text = "Сервер";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 307);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(684, 55);
			this.panel1.TabIndex = 10;
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(295, 9);
			this.button2.Margin = new System.Windows.Forms.Padding(4);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(128, 39);
			this.button2.TabIndex = 18;
			this.button2.Text = "Отмена";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button1.Location = new System.Drawing.Point(157, 8);
			this.button1.Margin = new System.Windows.Forms.Padding(4);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(132, 39);
			this.button1.TabIndex = 17;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// ConnectorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(684, 362);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.tabControl1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(800, 600);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(700, 400);
			this.Name = "ConnectorForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Настройки";
			this.Shown += new System.EventHandler(this.ConnectorForm_Shown);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_UserName;
        private System.Windows.Forms.ComboBox comboBox_server;
        private System.Windows.Forms.TextBox textBox_pswrd;
        private System.Windows.Forms.ComboBox comboBox_CharSet;
        private System.Windows.Forms.ComboBox comboBox_Database;
        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
    }
}