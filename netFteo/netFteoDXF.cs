using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using netFteo.Spatial;

namespace netFteo.IO
{

        /// <summary>
        /// DXF reader for dxf reading.
        /// Fixosoft wrapper for netdxf library code
        /// Targetting NET Framework 4.0 
        /// </summary>
    public class  DXFReader : TextReader
    {
        /// <summary>
        /// Импорт dxf-файлов
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public TPolygonCollection ImportDXF(string FileName)
        {
            TPolygonCollection res = new TPolygonCollection();
            System.IO.TextReader readFile = new StreamReader(FileName);
            BodyLoad(FileName);
            // TODO Encoding for dxf:
            netDxf.DxfDocument dxfFile = netDxf.DxfDocument.Load(FileName);

            if (dxfFile == null) return null;
            //if Block`s present:
            if (dxfFile.Blocks.Count > 0)
            {
                foreach (netDxf.Blocks.Block bl in dxfFile.Blocks)
                {
                    foreach (netDxf.Entities.EntityObject entity in bl.Entities) // in block may be placed inner borders (second, third ring)
                    {
                        if (entity.CodeName.Equals("LWPOLYLINE"))
                        {
                            TMyPolygon Polygon = DXF_ParseRegion(bl.Entities);
                            if (Polygon != null)
                            {
                                if ((bl.AttributeDefinitions.Count > 0) && (bl.AttributeDefinitions["Кад_номер"].Value != null))
                                    Polygon.Definition = (string)bl.AttributeDefinitions["Кад_номер"].Value;
                                res.AddPolygon(Polygon);
                            }
                        }
                    }
                }
            }


            if (dxfFile.LwPolylines.Count > 0)
                foreach (netDxf.Entities.LwPolyline polygon in dxfFile.LwPolylines)
                {
                    if (polygon.IsClosed)
                    {
                        TMyPolygon poly = new TMyPolygon();
                        poly.AppendPoints(DXF_ParseRing(polygon));
                        res.AddPolygon(poly);
                    }
                }

            return res;
        }

        private TMyOutLayer DXF_ParseRing(netDxf.Entities.LwPolyline poly)
        {
            if (!poly.IsClosed) return null;
            if (poly.Vertexes.Count < 3) return null;
            int ptNum = 0;
            TMyOutLayer res = new TMyOutLayer();
            try
            {
                foreach (netDxf.Entities.LwPolylineVertex vertex in poly.Vertexes)
                {
                    Point point = new Point(vertex.Location.Y, vertex.Location.X);
                    point.oldX = vertex.Location.Y;
                    point.oldY = vertex.Location.X;
                    point.NumGeopointA = "dxf" + (++ptNum).ToString();
                    res.AddPoint(point);
                }
            }

            catch (IOException ex)
            {
                return null;
                //  MessageBox.Show(ex.ToString());
            }
            return res;
        }

        private TMyPolygon DXF_ParseRegion(netDxf.Collections.EntityCollection polys)
        {

            TMyPolygon res = new TMyPolygon("dxfblock");
            try
            {
                if ((polys.Count > 0) && (polys[0].CodeName.Equals("LWPOLYLINE")))
                {
                    res.AppendPoints(DXF_ParseRing((netDxf.Entities.LwPolyline)polys[0]));
                }
                // childs:

                for (int i = 1; i <= polys.Count - 1; i++)
                {
                    if ((polys.Count > 0) && (polys[i].CodeName.Equals("LWPOLYLINE")))
                    {
                        res.AddChild(DXF_ParseRing((netDxf.Entities.LwPolyline)polys[i]));
                    }
                }

            }

            catch (IOException ex)
            {
                return null;
                //  MessageBox.Show(ex.ToString());
            }
            return res;
        }


    }
}
