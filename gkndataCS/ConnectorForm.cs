using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GKNData
{


    public partial class ConnectorForm : Form
    {
        public TAppCfgRecord Cfg;
        public MySqlConnection conn;
        public MySqlConnection conn2;
        public ConnectorForm()
        {
            InitializeComponent();
            Cfg = new TAppCfgRecord();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Cfg.ServerName = comboBox_server.Text;
            this.Cfg.ServerPort = textBox_Port.Text;
            this.Cfg.CharSet = comboBox_CharSet.Text;
            this.Cfg.DatabaseName = comboBox_Database.Text;
            this.Cfg.UserName = comboBox_UserName.Text;
            this.Cfg.UserPwrd = textBox_pswrd.Text;

            this.Cfg.CfgWrite();
            this.Cfg.Result = button1.DialogResult;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Cfg.Result = button2.DialogResult;
        }

        private void ConnectorForm_Shown(object sender, EventArgs e)
        {
            this.textBox_Port.Text = this.Cfg.ServerPort;
            this.comboBox_server.Text = this.Cfg.ServerName;
            this.comboBox_UserName.Text = this.Cfg.UserName;
            this.comboBox_Database.Text = this.Cfg.DatabaseName;
            this.comboBox_CharSet.Text = this.Cfg.CharSet;
            this.textBox_pswrd.Text = this.Cfg.UserPwrd;
			this.textBox_TimeOut.Text = this.Cfg.IddleTimeOut;
		}

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

    }

    /*
        public class TAppCfgReader
        {


            public bool CfgRead(TAppCfgRecord Cfgurator)
            {

                return true;
            }
        }
        */


    public class TCurrentItem
    {
        public string TypeName_Block = "netFteo.Spatial.TMyCadastralBlock";
        public string TypeName_Parcel = "netFteo.Spatial.TMyParcel";
        public string Item_TypeName;
        public int Item_id;
        public TCurrentItem()
        {
            this.Item_id = -1;
            this.Item_TypeName = "EMPTY";
        }
    }

    public class TAppCfgRecord
    {
        string Fixosoft_GKNDATA2 = "\\Software\\Fixosoft\\GKNData\\2.x.x.x";
        string Fixosoft_GKNDATA_NETApps = "\\Software\\Fixosoft\\GKNData\\NET";
        public TCurrentItem CurrentItem;
        public string DatabaseName;
        public string ServerName;
        public string ServerPort;
        public string CharSet;
        public string UserName;
        public string UserPwrd;
        public int Subrf_id;
        public string SubRF_KN;
        public string SubRF_Name;
        public int District_id;
        public string District_KN;
        public string District_Name;
		public string IddleTimeOut;
		/// <summary>
		/// Количество кварталов в районе
		/// </summary>
		public int BlockCount;
        public DialogResult Result;

        public TAppCfgRecord()
        {
            this.CurrentItem = new TCurrentItem();
        }

        /// <summary>
        /// Запись в реестр
        /// </summary>
        public void CfgRead()
        {
            Microsoft.Win32.RegistryKey rk;
            rk = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, Microsoft.Win32.RegistryView.Registry32);
            Microsoft.Win32.RegistryKey nk = rk.CreateSubKey("Software").CreateSubKey("Fixosoft").CreateSubKey("GKNData").CreateSubKey("2.x.x.x");
            if (nk == null) return;
            try
            {
                this.ServerName = (string)nk.GetValue("ServerName");
                this.DatabaseName = (string)nk.GetValue("DatabaseName");
                this.ServerPort = (string)nk.GetValue("ServerPort");
                this.CharSet = (string)nk.GetValue("CharSet");
                this.UserName = (string)nk.GetValue("SQLUserName");
                this.UserPwrd = (string)nk.GetValue("Password");
                this.Subrf_id = (int)nk.GetValue("subrf_ID");
                this.SubRF_KN = (string)nk.GetValue("SubRF_KN");
                this.SubRF_Name = (string)nk.GetValue("SubRF_Name");
                this.District_id = (int)nk.GetValue("district_ID");
                this.District_KN = (string)nk.GetValue("District_KN");
                this.District_Name = (string)nk.GetValue("District_Name");
				this.IddleTimeOut = (string)nk.GetValue("IddleTimeOut");
			}
            catch
            {
            }
        }

        /// <summary>
        /// Запись в реестр
        /// </summary>
        public void CfgWrite()
        {
            Microsoft.Win32.RegistryKey rk;
            rk = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, Microsoft.Win32.RegistryView.Registry32);
            Microsoft.Win32.RegistryKey nk = rk.CreateSubKey("Software").CreateSubKey("Fixosoft").CreateSubKey("GKNData").CreateSubKey("2.x.x.x");
            if (nk == null) return;
            try
            {

                nk.SetValue("ServerName", this.ServerName);
                nk.SetValue("DatabaseName", this.DatabaseName);
                nk.SetValue("ServerPort", this.ServerPort);
                nk.SetValue("CharSet", this.CharSet);
                nk.SetValue("SQLUserName", this.UserName);
                nk.SetValue("Password", this.UserPwrd);
                nk.SetValue("subrf_ID", this.Subrf_id);
                nk.SetValue("SubRF_KN", this.SubRF_KN);
                nk.SetValue("SubRF_Name", this.SubRF_Name);
                nk.SetValue("district_ID", this.District_id);
                nk.SetValue("District_KN", this.District_KN);
                nk.SetValue("District_Name", this.District_Name);
				if (this.IddleTimeOut != null)
				nk.SetValue("IddleTimeOut", this.IddleTimeOut);
				else
					nk.SetValue("IddleTimeOut", "10000");
			}
            catch
            {
            }
        }
    }
}
