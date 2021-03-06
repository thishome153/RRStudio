﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using netFteo.Runtime;
namespace XMLReaderCS
{
    partial class AboutBox1 : Form
    {
        
        public AboutBox1()
        {
            InitializeComponent();
            
#if (DEBUG)
            this.labelProductName.Text = AssemblyProduct + " /DEBUG version ";
#else
            this.labelProductName.Text = AssemblyProduct;
#endif
            this.Text = String.Format("О {0}", AssemblyTitle);
			this.labelProductName.Text = AssemblyDescription + " " + String.Format(" v{0}", AssemblyVersion) +
				"\r\n" + AssemblyCompany + " " + AssemblyCopyright;
            this.textBoxDescription.Text = "IDE :";
            this.textBoxDescription.AppendText("\r\n Begin design:   MSVC 2010.Express Edition");
            this.textBoxDescription.AppendText("\r\n Current time (: Visual Studio 2019 Community");
            this.textBoxDescription.AppendText("\r\n Assemblys:");
            foreach (string ass in Assemblys)
            {
                this.textBoxDescription.AppendText("\r\n "+ass);
            }
			this.textBoxDescription.AppendText("\r\n Operation System Information:");
			this.textBoxDescription.AppendText("\r\n"+ OSInfo.Name+" " + OSInfo.Edition + " " +OSInfo.ServicePack);
			this.textBoxDescription.AppendText("\r\n Version " + OSInfo.VersionString +" " +  OSInfo.Bits + "bit");
		}

        #region Методы доступа к атрибутам сборки

        public List<string> Assemblys
    
        {
            get
            {

                List<string> res = new List<string>();
                foreach (System.Reflection.AssemblyName an in System.Reflection.Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                {
                    res.Add(an.Name + " v "+an.Version.ToString());
                }
                return res;
            }


        }

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }


        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}
