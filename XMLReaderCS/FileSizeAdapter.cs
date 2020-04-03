using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XMLReaderCS
{
   public static class FileSizeAdapter
    {
       public  static string FileSizeToString(string FileName)
       { 
            FileInfo info = new FileInfo(FileName);
           if (info.Length < 1024)
               return info.Length.ToString() +" bytes";
           if (info.Length < 1048576)
           {
               long size = info.Length / 1024;
               return  size.ToString() + " Kb";
           }
           if (info.Length < 16777216)
           {
               long size = info.Length / 1048576;
               return size.ToString() + " Mb";
           }
           else return "";

       }

        public static long FileSize(string FileName)
        {
            return new FileInfo(FileName).Length;
        }

    }
   public class RecentFiles 
   {
       public List<string> t;
       public string[] Items;
       public RecentFiles()
       {
           this.Items =new string [10];
       }

       public void AddFile(string filename)
       {
           this.Items[0] = filename;// вверху самый последний
           //переставляем как в стеке
           for (int i = 9; i > 0; --i)
           {
               this.Items[i] = this.Items[i - 1];
           }

       }
       /*
       public void Save2Setting
       {
           //XMLReaderCS.Properties.
       }
        * */
   }
}
