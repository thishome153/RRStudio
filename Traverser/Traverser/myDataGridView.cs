using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Traverser
{
    /// <summary>
    /// Обертка для  DataGridView с установленным DoubleBuffered = true; 
    /// дабы избежать глюков C# дебаггером под winXP
    /// </summary>
    class myDataGridView: System.Windows.Forms.DataGridView
    {
        public myDataGridView() 
        { 
            DoubleBuffered = true; 
        }
    }
}
    
