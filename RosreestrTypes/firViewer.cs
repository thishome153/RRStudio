using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RRTypes.FIR
{
	public class firViewer : System.Windows.Forms.PictureBox
	{
		public FIR_Server_ru Server;

		public firViewer()
		{
			this.DoubleBuffered = true;
		}
		/*
		public firViewer(IContainer container)
		{
			container.Add(this);

			//InitializeComponent();
		}
		*/
	}
}
