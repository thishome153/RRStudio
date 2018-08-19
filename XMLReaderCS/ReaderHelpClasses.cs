using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLReaderCS
{
   public class TreeNodeUDdata
    {
       public int Parent_id;
       public int Item_id;
       public int ItemType;
       public int Item_Status;
       public string ItemName;
       public string ItemHint;
    }

   public static class MyEncoding
   {
       public static string Utf8ToWin1251(string Inpututf8)
       {
           var utf8bytes = Encoding.UTF8.GetBytes(Inpututf8);
           var win1252Bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("windows-1251"), utf8bytes);
           return win1252Bytes.ToString();
       }

       public static string Utf8To866(string Inpututf8)
       {
           Encoding ansiCyrillic = Encoding.GetEncoding(866);
           byte[] learnAlbanianBytes = ansiCyrillic.GetBytes(Inpututf8);
           string Text = ansiCyrillic.GetString(learnAlbanianBytes);
           return Text;
       }

       public static string Utf8ToWin1251_01(string Inpututf8)
       {
           byte[] byteArray = Encoding.UTF8.GetBytes(Inpututf8);
           byte[] asciiArray = Encoding.Convert(Encoding.UTF8, Encoding.ASCII, byteArray);
           string finalString = Encoding.ASCII.GetString(asciiArray);
           return finalString;
       }
       
       /// <summary>
       /// Юникод в однобайтный ASCII
       /// </summary>
       /// <param name="Inpututf8"></param>
       /// <returns></returns>
       public static string Utf8ToWinASCII(string Inpututf8)
       {
           ASCIIEncoding ascii = new ASCIIEncoding();
           byte[] byteArray = Encoding.UTF8.GetBytes(Inpututf8);
           byte[] asciiArray = Encoding.Convert(Encoding.UTF8, Encoding.ASCII, byteArray);
           string finalString = ascii.GetString(asciiArray);
           return finalString;
       }

       private static readonly ASCIIEncoding asciiEncoding = new ASCIIEncoding();

       public static string ToAscii(this string dirty)
       {
           byte[] bytes = asciiEncoding.GetBytes(dirty);
           string clean = asciiEncoding.GetString(bytes);
           return clean;
       }
       public static void FiletoAScii(string infile, string outfile)
       {
           System.IO.StreamReader sr = new System.IO.StreamReader(infile);
           System.IO.StreamWriter sw = new System.IO.StreamWriter(outfile, false, Encoding.ASCII); // or UTF-7, etc  

           sw.WriteLine(sr.ReadToEnd());

           sw.Close();
           sr.Close(); 
       }

   }
}
