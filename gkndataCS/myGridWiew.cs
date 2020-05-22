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
            RowTemplate.Height = 35;
            
            Columns.Add("district_id", "id");
            Columns["district_id"].Width = 40;
            Columns.Add("district_kn", "КН");
            Columns["district_kn"].Width = 50;
            Columns.Add("district_Name", "Наименование");
            Columns["district_Name"].Width = 250;
            RowsDefaultCellStyle.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.GraphicsUnit.Pixel);   
            Rows.Add("1", "26", "First row");
            Rows.Add("256", "61", "Rostov, Taganrog");
            
        }
        }
   
}
