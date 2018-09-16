using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using netFteo.Spatial;

namespace netFteo.IO
{   
    // Сводка:
    // Класс для операций с файлами M @ p in fo
    // Используются файлы mif..... 
  public  class MifReader
    {
        MifReader()
        {
           this.Layers = new List<TMyPolygon>();
        }
        public List<TMyPolygon> Layers;
        public void ReadFile(string FileName)
        { 
        }
        public void Clear()
        {
            this.Layers.Clear();
        } 

    }
    class netFteoMifWriter
    { 
        public string FileName;
        netFteoMifWriter()
        { 
            this.FileName ="";
        }
    public void SaveToFile(string Filename, TMyOutLayer Layer)
    {

    }
    public void SaveToFile(string Filename, TMyPolygon Polygon)
    {

    }

    }
}
