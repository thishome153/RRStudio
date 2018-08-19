using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKNData
{
   
        class myDataGridView : System.Windows.Forms.DataGridView
        {
            public myDataGridView()
            {
                DoubleBuffered = true;
                ReadOnly = true;
                ShowEditingIcon = false;
                AllowUserToAddRows = false;
                AllowUserToDeleteRows = false;
            }
        }
   
}
