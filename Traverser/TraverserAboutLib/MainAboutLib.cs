using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TraverserAboutLib
{
    public class MainAboutLib
    {
        public void ShowAboutDialog()
        {
            AboutLibDlg abtdlg = new AboutLibDlg();
            abtdlg.ShowDialog();
          }

    }
}
