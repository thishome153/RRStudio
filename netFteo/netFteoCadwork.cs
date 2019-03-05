using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netFteo
{
   class TCadwork
    {

       public Spatial.PointList Points;
       public Spatial.TMyParcelCollection Parcels;
       public NikonRaw.TNikonRaw RawData;
       public TCadwork()
       {
           this.Points = new Spatial.PointList();
           this.RawData = new NikonRaw.TNikonRaw();
           this.Parcels = new Spatial.TMyParcelCollection();
       }
    }
}
