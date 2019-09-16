using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using netFteo.NikonRaw;
using netFteo.Spatial;

namespace Traverser
{
   public class TraverserProject   // Что будем сохранять
    {
        public string ProjectName,
                      Operator,
                      CreateDatetime,
                      SavingDateTime,
                      HostName, HostIP,
                      UserDomainName;
        public TNikonRaw Raw;
        public TTraverse Travers;
        public PointList Points;
        public TMyPolygon Polygon; // Для отладки, тестовый полигон, в чтение #Fixosoft NumXYZD data format V201
        public TEntitySpatial ES; // Для отладки, тестовый полигон, в чтение #Fixosoft NumXYZD data format V201
        public TraverserProject()
        {
            this.Raw = new TNikonRaw();
            this.Travers = new TTraverse();
            this.Points = new PointList();
            this.Polygon = new TMyPolygon();
            this.ES = new TEntitySpatial();
            this.CreateDatetime = DateTime.Now.ToString();
        }
    }  
}
