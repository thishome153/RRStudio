using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
// CADES extentions CSP:
//using CAPICOM;

/// changes 
using netFteo.Spatial;
//changes 3 to 11 ( и Sync)
namespace netFteo.IO
{

    // Используются Текстовые файлы.....
    /// <summary>
    ///Класс для операций с файлами Fteo
    /// </summary>
    public class TextReader
    {
        public const string TabDelimiter = "\t";  // tab
        public const string CommaDelimiter = ";";  // tab
        public string FileType = "";  // tab
        public string Body;
        /// <summary>
        /// Чтение файлов  CSV (формата Технокад)
        /// </summary>
        /// <param name="Fname"></param>
        /// <returns></returns>
        public TPolygonCollection ImportCSVFile(string Fname)
        {
            TPolygonCollection resPolys = new TPolygonCollection();
            try
            {
                string line = null;
                int StrCounter = 0;
                System.IO.TextReader readFile = new StreamReader(Fname);

                while (readFile.Peek() != -1)
                {
                    line = readFile.ReadLine();

                    if (line != null) //Читаем строку
                    {      //по строке


                     
                        if (line.Contains(";;;;;;;;;;;")) //Комментарий в файлах, пропустим его
                        {
                            goto next;
                        };

                        if (line.Contains("["))
                        {
                            TMyPolygon resPoly = new TMyPolygon(line.Substring(7));
                            line = readFile.ReadLine();
                            while (!line.Contains(";;;;;;;;;;;"))
                            {
                                if (line.Contains("Child")) goto ChildsHere;
                                StrCounter++;
                                string[] SplittedStr = line.Split(CommaDelimiter.ToCharArray()); //Сплпиттер по ";" - default for CSV
                                Point FilePoint = new Point();
                                FilePoint.id = StrCounter;
                                FilePoint.NumGeopointA = SplittedStr[0].ToString();
                                FilePoint.x = Convert.ToDouble(SplittedStr[1].ToString());
                                FilePoint.y = Convert.ToDouble(SplittedStr[2].ToString());
                                if (!SplittedStr[3].Contains("-"))
                                    FilePoint.oldX = Convert.ToDouble(SplittedStr[3].ToString());
                                if (!SplittedStr[4].Contains("-"))
                                    FilePoint.oldY = Convert.ToDouble(SplittedStr[4].ToString());

                                //FilePoint.z = Convert.ToDouble(SplittedStr[5].ToString()); // here changes flag, * == TRUE

                                if (SplittedStr[5].ToString() == "*")
                                {
                                    FilePoint.Status = 0; // new, changed
                                }
                                else // ident old and new: just copy
                                {
                                    FilePoint.oldX = FilePoint.x;
                                    FilePoint.oldY = FilePoint.y;
                                    FilePoint.Status = 6; // exist, not changed
                                }
                                FilePoint.Mt = Convert.ToDouble(SplittedStr[4].ToString());
                                FilePoint.Description = SplittedStr[5].ToString();
                                resPoly.AddPoint(FilePoint);
                                //Внутренние границы
                                goto nextPolygonstring;

                            ChildsHere:

                                if (line.Contains("Child"))
                                {
                                    TMyOutLayer child = resPoly.AddChild();
                                    line = readFile.ReadLine();
                                    while (!line.Contains("EndChild"))
                                    {
                                        StrCounter++;
                                        string[] ChildStr = line.Split(TabDelimiter.ToCharArray()); //Сплпиттер по tab (\t)
                                        Point ChildPoint = new Point();
                                        ChildPoint.id = StrCounter;
                                        ChildPoint.NumGeopointA = ChildStr[0].ToString();
                                        ChildPoint.x = Convert.ToDouble(ChildStr[1].ToString());
                                        ChildPoint.y = Convert.ToDouble(ChildStr[2].ToString());
                                        ChildPoint.z = Convert.ToDouble(ChildStr[3].ToString());
                                        ChildPoint.Mt = Convert.ToDouble(ChildStr[4].ToString());
                                        ChildPoint.Description = ChildStr[5].ToString();
                                        child.AddPoint(ChildPoint);
                                        line = readFile.ReadLine();
                                    }
                                }
                            nextPolygonstring: line = readFile.ReadLine();
                            }
                            resPolys.AddPolygon(resPoly);
                        }
                    }
                next:;
                }
                readFile.Close();
                readFile = null;
                return resPolys;
            }
            catch (IOException ex)
            {
                //  MessageBox.Show(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Чтение текстовых файлов разных форматов (2014, 2015, 2016, pkzo)
        /// </summary>
        /// <param name="Fname"></param>
        /// <returns></returns>
        public TPolygonCollection ImportTxtFile(string Fname)
        {
            string line = null;
            // int StrCounter = 0;
            System.IO.TextReader readFile = new StreamReader(Fname);
            System.IO.TextReader readFileBody = new StreamReader(Fname);
            
            Body = readFileBody.ReadToEnd();
            readFileBody.Close();

                while (readFile.Peek() != -1)
            {
                line = readFile.ReadLine();

                if (line != null) //Читаем строку
                {      //по строке
                    FileType = line;
                    if (line.Contains("#Fixosoft NumXYZD data format V2014"))
                    {
                        TPolygonCollection rets = new TPolygonCollection();
                        rets.AddPolygon(ImportNXYZDFile2014(Fname));
                        return rets;
                    }
                    if (line.Contains("#Fixosoft NumXYZD data format V2015"))
                        return ImportNXYZDFile2015(Fname);

                    if (line.Contains("#Fixosoft NumXYZD data format V2016"))
                        return ImportNXYZDFile2016(Fname);

                    if (line.Equals("#Fixosoft spatial text file V2018"))
                        return ImportNXYZDFile2018(Fname);

                }
            }
            return null;
        }

        /// <summary>
        /// Импорт xml для типов netfeo::BaseClasses
        /// </summary>
        /// <param name="Fname">Имя файла</param>
        /// <returns>Коллекция полигонов</returns>
        public TPolygonCollection ImportXML(string Fname)
        {
            System.IO.TextReader reader = new System.IO.StreamReader(Fname);
            System.Xml.XmlDocument XMLDocFromFile = new System.Xml.XmlDocument();
            XMLDocFromFile.Load(reader);
            reader.Close();

            TPolygonCollection res = new TPolygonCollection();
            if (XMLDocFromFile.DocumentElement.Name == "TMyPolygon")
            {
                reader = new System.IO.StreamReader(Fname);
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TMyPolygon));
                TMyPolygon xmlPolygon = (TMyPolygon)serializer.Deserialize(reader);
                res.AddPolygon(xmlPolygon);
            }


            if (XMLDocFromFile.DocumentElement.Name == "TPolygonCollection")
            {
                reader = new System.IO.StreamReader(Fname);
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TPolygonCollection));
                res = (TPolygonCollection)serializer.Deserialize(reader);
            }

            return res;
        }



        private Point MIF_ParseOrdinate(string line, string pointName)
        {
            Point res = new Point();
            res.NumGeopointA = "p" + pointName;
            double Y = 0; double X = 0;
            if (Double.TryParse(line.Substring(0, line.IndexOf(' ')), out Y)) res.y = Y;

            string ss = line.Substring(line.IndexOf(' ') + 1, line.Length - line.IndexOf(' ') - 1);
            if (Double.TryParse(ss, out X)) res.x = X;
            return res;
        }
        /*
        private Point MIF_ParsePoint(System.IO.TextReader readFile)
        {
            Point res = MIF_ParseOrdinate(readFile.ReadLine(), "pt");
            return res;
        }
*/
        private TMyPolygon MIF_ParseRegion(System.IO.TextReader readFile, int ringCount)
        {
            string line;
            TMyPolygon res = new TMyPolygon("mif polygon");
            try
            {
                while (readFile.Peek() != -1)
                {
                    line = readFile.ReadLine();
                    if (line != null)
                    {
                        if (line.Length > 0)
                        {
                            //detect vertex count
                            int VertexCount = 0;
                            if (Int32.TryParse(line, out VertexCount))
                            {
                                for (int i = 0; i <= VertexCount - 1; i++)
                                {
                                    line = readFile.ReadLine();
                                    res.AddPoint(MIF_ParseOrdinate(line, i.ToString()));
                                }


                                //childs - inner rings:
                                for (int r = 1; r < ringCount; r++)
                                {

                                    line = readFile.ReadLine();
                                    //detect vertex count
                                    int VertexCount2 = 0;
                                    if (Int32.TryParse(line, out VertexCount2))
                                    {
                                        TMyOutLayer childRing = res.AddChild();
                                        for (int i = 0; i <= VertexCount2 - 1; i++)
                                        {
                                            line = readFile.ReadLine();
                                            childRing.AddPoint(MIF_ParseOrdinate(line, i.ToString()));
                                        }
                                    }
                                } // ----end for childs
                                return res;
                            }
                        }
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

        /// <summary>
        /// считывание записи из MID-файла Map-info
        /// </summary>
        /// <param name="Filename"></param>
        /// <param name="MIF_Options"></param>
        private string[] MID_ParseRow(System.IO.TextReader readFile, MifOptions MIF_Options)
        {
            string line;
            string[] mid_row = null;
            int StrCounter = 0;
            //while (readFile.Peek() != -1)
            //{
            line = readFile.ReadLine(); StrCounter++;
            if (line != null) //Читаем строку
            {
                //use spit for delimiter
                mid_row = line.Split(MIF_Options.Delimiter.ToCharArray());
                return mid_row;
            }
            else return null;
            //}
        }

        /// <summary>
        /// Импорт mif-файлов
        /// </summary>
        /// <param name="Filename"></param>
        /// <returns></returns>
        public TPolygonCollection ImportMIF(string FileName)
        {
            TPolygonCollection res = new TPolygonCollection();
            string baseFileName = Path.GetDirectoryName(FileName) + "\\" + Path.GetFileNameWithoutExtension(FileName);

            System.IO.TextReader readFile = new StreamReader(baseFileName + ".mif");
            System.IO.TextReader readMIDFile = new StreamReader(baseFileName + ".mid");

            System.IO.TextReader readFileBody = new StreamReader(baseFileName + ".mif", Encoding.ASCII); // default for mif            
            Body = readFileBody.ReadToEnd();
            readFileBody.Close();
            /* TODO Encoding for mif:
            byte[] ansiBytes = Encoding.GetEncoding(1251).GetBytes(Body);
            Encoding.Convert(Encoding.ASCII, Encoding.Unicode, ansiBytes);
            Body = Encoding.GetEncoding(1251).GetString(ansiBytes);
            */
            string line; string midline;
            int PolygonCount = 0;
            int StrCounter = 0;
            string delimiter = "";
            PointList MIF_Points = new PointList(); // single list for all points in mif-file
            // первый проход, читаем заголовок 
            try
            {

                while (readFile.Peek() != -1)
                {
                    line = readFile.ReadLine(); StrCounter++;
                    if (line != null) //Читаем строку
                    {

                        if (line.Length > 5)
                        {
                            if (line.Length > 12)
                            {
                                if (line.ToUpper().Substring(0, 9).Contains("DELIMITER"))
                                {
                                    res.MIF_Options.Delimiter = line.Substring(11, 1);
                                };
                            };


                            if (line.ToUpper().Substring(0, 7).Equals("COLUMNS")) //line present mapinfo polygon
                            {
                                Int16 columnsCount = 0;
                                if (Int16.TryParse(line.Substring(line.IndexOf(' ') + 1, line.Length - line.IndexOf(' ') - 1), out columnsCount))
                                {
                                    for (int i = 1; i <= columnsCount; i++)
                                    {
                                        res.MIF_Options.AddColumn(i.ToString(), readFile.ReadLine());
                                    }
                                }
                            };

                            //Everyone feature points to record in MID file
                            //and after reading (parsing) feature MID must be readed by line also

                            if (line.ToUpper().Substring(0, 5).Equals("POINT")) //line present mapinfo point
                            {
                                MIF_Points.AddPoint(MIF_ParseOrdinate(line.Substring(6), "mif-pt"));
                            }
                            if (line.ToUpper().Substring(0, 5).Equals("PLINE")) //line present mapinfo polyline
                            {
                                //   MIF_Points.AddPoint(MIF_ParseOrdinate(line.Substring(6), "mif-pt"));

                            }

                            if (line.ToUpper().Substring(0, 6).Equals("REGION")) //line present mapinfo polygon
                            {
                                PolygonCount++;
                                //detect count of ring of polygon "Region 1"
                                Int16 ringCount = 0;
                                if (Int16.TryParse(line.Substring(line.IndexOf(' ') + 1, line.Length - line.IndexOf(' ') - 1), out ringCount))
                                    res.AddPolygon(MIF_ParseRegion(readFile, ringCount));
                                midline = MID_ParseRow(readMIDFile, res.MIF_Options).ToString();
                            };
                        }
                        StrCounter++;
                    }
                }
                readFile.Close();
                readFile = null;
                TMyPolygon MIF_Points_POly = new TMyPolygon("mifPoints");
                MIF_Points_POly.AppendPoints(MIF_Points);
                res.AddPolygon(MIF_Points_POly);
            }

            catch (IOException ex)
            {
                return null;
                //  MessageBox.Show(ex.ToString());
            }
            res.Defintion = FileName;
            res.MIF_Options.Delimiter = delimiter;// ' " polygons " + PolygonCount.ToString();
            return res;
        }


        /// <summary>
        /// Стапый дорый PKZO (x tab y)
        /// </summary>
        /// <param name="Fname"></param>
        /// <returns></returns>
        private TMyPolygon ImportPKZO(string Fname)
        {
            try
            {
                TMyPolygon resPoly = new TMyPolygon(0, Fname);
                string line = null;
                int StrCounter = 0;
                System.IO.TextReader readFile = new StreamReader(Fname);

                while (readFile.Peek() != -1)
                {
                    line = readFile.ReadLine();
                    if (line != null) //Читаем строку
                    {      //по строке
                        if (line.Contains("CO,")) //Комментарий в файлах, пропустим его
                        {
                            return null;
                        };

                        if (line.Contains("#")) //Комментарий в файлах, пропустим его
                        {
                            return null;
                        };

                        StrCounter++;
                        string[] SplittedStr = line.Split(TabDelimiter.ToCharArray()); //Сплпиттер по tab (\t)
                        Point FilePoint = new Point();
                        FilePoint.id = StrCounter;
                        FilePoint.NumGeopointA = StrCounter.ToString();
                        FilePoint.x = Convert.ToDouble(SplittedStr[1].ToString());
                        FilePoint.y = Convert.ToDouble(SplittedStr[2].ToString());
                        resPoly.AddPoint(FilePoint);
                    }

                }
                readFile.Close();
                readFile = null;
                return resPoly;
            }
            catch (IOException ex)
            {
                return null;
                //  MessageBox.Show(ex.ToString());
            }
            return null;
        }
        private TMyPolygon ImportNXYZDFile2014(string Fname)
        {
            try
            {
                TMyPolygon resPoly = new TMyPolygon(0, Fname);
                // this.ClearItems();
                //this.Childs.RemoveAll(<TmySlot>);
                string line = null;
                int StrCounter = 0;
                System.IO.TextReader readFile = new StreamReader(Fname);

                while (readFile.Peek() != -1)
                {
                    line = readFile.ReadLine();

                    if (line != null) //Читаем строку
                    {      //по строке

                        if (line.Contains("CO,")) //Комментарий в файлах, пропустим его
                        {
                            goto next;
                        };

                        if (line.Contains("#")) //Комментарий в файлах, пропустим его
                        {
                            goto next;
                        };

                        StrCounter++;
                        string[] SplittedStr = line.Split(TabDelimiter.ToCharArray()); //Сплпиттер по tab (\t)
                        Point FilePoint = new Point();
                        FilePoint.id = StrCounter;
                        FilePoint.NumGeopointA = SplittedStr[0].ToString();
                        FilePoint.x = Convert.ToDouble(SplittedStr[1].ToString());
                        FilePoint.y = Convert.ToDouble(SplittedStr[2].ToString());
                        FilePoint.z = Convert.ToDouble(SplittedStr[3].ToString());
                        FilePoint.Description = SplittedStr[4].ToString();
                        resPoly.AddPoint(FilePoint);
                    }
                next:;
                }
                readFile.Close();
                readFile = null;
                return resPoly;
            }
            catch (IOException ex)
            {
                return null;
                //  MessageBox.Show(ex.ToString());
            }
            return null;
        }
        private TPolygonCollection ImportNXYZDFile2015(string Fname)
        {
            TPolygonCollection resPolys = new TPolygonCollection();
            try
            {
                string line = null;
                int StrCounter = 0;
                System.IO.TextReader readFile = new StreamReader(Fname);

                while (readFile.Peek() != -1)
                {
                    line = readFile.ReadLine();

                    if (line != null) //Читаем строку
                    {      //по строке

                        if (line.Contains("CO,")) //Комментарий в файлах, пропустим его
                        {
                            goto next;
                        };

                        if (line.Contains("#")) //Комментарий в файлах, пропустим его
                        {
                            goto next;
                        };

                        if (line.Contains("Polygon"))
                        {
                            TMyPolygon resPoly = new TMyPolygon(line.Substring(7));
                            line = readFile.ReadLine();
                            while (!line.Contains("EndPolygon"))
                            {
                                if (line.Contains("Child")) goto ChildsHere;
                                StrCounter++;
                                string[] SplittedStr = line.Split(TabDelimiter.ToCharArray()); //Сплпиттер по tab (\t)
                                Point FilePoint = new Point();
                                FilePoint.id = StrCounter;
                                FilePoint.NumGeopointA = SplittedStr[0].ToString();
                                FilePoint.x = Convert.ToDouble(SplittedStr[1].ToString());
                                FilePoint.y = Convert.ToDouble(SplittedStr[2].ToString());
                                FilePoint.z = Convert.ToDouble(SplittedStr[3].ToString());
                                FilePoint.Description = SplittedStr[4].ToString();
                                resPoly.AddPoint(FilePoint);
                                //Внутренние границы
                                goto nextPolygonstring;
                            ChildsHere:

                                if (line.Contains("Child"))
                                {
                                    TMyOutLayer child = resPoly.AddChild();
                                    line = readFile.ReadLine();
                                    while (!line.Contains("EndChild"))
                                    {
                                        StrCounter++;
                                        string[] ChildStr = line.Split(TabDelimiter.ToCharArray()); //Сплпиттер по tab (\t)
                                        Point ChildPoint = new Point();
                                        ChildPoint.id = StrCounter;
                                        ChildPoint.NumGeopointA = ChildStr[0].ToString();
                                        ChildPoint.x = Convert.ToDouble(ChildStr[1].ToString());
                                        ChildPoint.y = Convert.ToDouble(ChildStr[2].ToString());
                                        ChildPoint.z = Convert.ToDouble(ChildStr[3].ToString());
                                        ChildPoint.Description = ChildStr[4].ToString();
                                        child.AddPoint(ChildPoint);
                                        line = readFile.ReadLine();
                                    }
                                }
                            nextPolygonstring: line = readFile.ReadLine();
                            }
                            resPolys.AddPolygon(resPoly);
                        }
                    }
                next:;
                }
                readFile.Close();
                readFile = null;
                return resPolys;
            }
            catch (IOException ex)
            {
                //  MessageBox.Show(ex.ToString());
                return null;
            }
            return null;
        }
        private TPolygonCollection ImportNXYZDFile2016(string Fname)
        {
            TPolygonCollection resPolys = new TPolygonCollection();
            try
            {
                string line = null;
                int StrCounter = 0;
                System.IO.TextReader readFile = new StreamReader(Fname);

                while (readFile.Peek() != -1)
                {
                    line = readFile.ReadLine();

                    if (line != null) //Читаем строку
                    {      //по строке


                        if (line.Contains("CO,")) //Комментарий в файлах, пропустим его
                        {
                            goto next;
                        };

                        if (line.Contains("#")) //Комментарий в файлах, пропустим его
                        {
                            goto next;
                        };

                        if (line.Contains("Polygon"))
                        {
                            TMyPolygon resPoly = new TMyPolygon(line.Substring(7));
                            line = readFile.ReadLine();
                            while (!line.Contains("EndPolygon"))
                            {
                                if (line.Contains("Child")) goto ChildsHere;
                                StrCounter++;
                                string[] SplittedStr = line.Split(TabDelimiter.ToCharArray()); //Сплпиттер по tab (\t)
                                Point FilePoint = new Point();
                                FilePoint.id = StrCounter;
                                FilePoint.NumGeopointA = SplittedStr[0].ToString();
                                FilePoint.x = Convert.ToDouble(SplittedStr[1].ToString());
                                FilePoint.y = Convert.ToDouble(SplittedStr[2].ToString());
                                FilePoint.z = Convert.ToDouble(SplittedStr[3].ToString());
                                FilePoint.Mt = Convert.ToDouble(SplittedStr[4].ToString());
                                FilePoint.Description = SplittedStr[5].ToString();
                                resPoly.AddPoint(FilePoint);
                                //Внутренние границы
                                goto nextPolygonstring;
                            ChildsHere:

                                if (line.Contains("Child"))
                                {
                                    TMyOutLayer child = resPoly.AddChild();
                                    line = readFile.ReadLine();
                                    while (!line.Contains("EndChild"))
                                    {
                                        StrCounter++;
                                        string[] ChildStr = line.Split(TabDelimiter.ToCharArray()); //Сплпиттер по tab (\t)
                                        Point ChildPoint = new Point();
                                        ChildPoint.id = StrCounter;
                                        ChildPoint.NumGeopointA = ChildStr[0].ToString();
                                        ChildPoint.x = Convert.ToDouble(ChildStr[1].ToString());
                                        ChildPoint.y = Convert.ToDouble(ChildStr[2].ToString());
                                        ChildPoint.z = Convert.ToDouble(ChildStr[3].ToString());
                                        ChildPoint.Mt = Convert.ToDouble(ChildStr[4].ToString());
                                        ChildPoint.Description = ChildStr[5].ToString();
                                        child.AddPoint(ChildPoint);
                                        line = readFile.ReadLine();
                                    }
                                }
                            nextPolygonstring: line = readFile.ReadLine();
                            }
                            resPolys.AddPolygon(resPoly);
                        }
                    }
                next:;
                }
                readFile.Close();
                readFile = null;
                return resPolys;
            }
            catch (IOException ex)
            {
                //  MessageBox.Show(ex.ToString());
                return null;
            }
            return null;
        }

        private TPolygonCollection ImportNXYZDFile2018(string Fname)
        {
            TPolygonCollection resPolys = new TPolygonCollection();
            try
            {
                string line = null;
                int StrCounter = 0;
                System.IO.TextReader readFile = new StreamReader(Fname);

                while (readFile.Peek() != -1)
                {
                    line = readFile.ReadLine();

                    if (line != null) //Читаем строку
                    {      //по строке

                        if (line.Contains("#")) //Комментарий в файлах, пропустим его
                        {
                            goto next;
                        };

                        if (line.Contains("Polygon"))
                        {
                            string[] SplittedStr = line.Split(TabDelimiter.ToCharArray());
                            TMyPolygon resPoly = new TMyPolygon(SplittedStr[1]);//   line.Substring(7));

                            line = readFile.ReadLine();
                            while (!line.Contains("EndPolygon"))
                            {
                                if (line.Contains("Child")) goto ChildsHere;
                                StrCounter++;
                                SplittedStr = line.Split(TabDelimiter.ToCharArray()); //Сплпиттер по tab (\t)
                                Point FilePoint = new Point();
                                FilePoint.id = StrCounter;
                                FilePoint.NumGeopointA = SplittedStr[0].ToString();

                                if (!SplittedStr[1].Contains("-"))
                                {
                                    FilePoint.oldX = Convert.ToDouble(SplittedStr[1].ToString());
                                    FilePoint.oldY = Convert.ToDouble(SplittedStr[2].ToString());
                                    FilePoint.Status = 4;
                                }
                                else FilePoint.Status = 0; // нет старых координат
                                
                                // нет новых координат (z.B. точка ликвидируется)
                                if (!SplittedStr[3].Contains("-"))
                                {
                                    FilePoint.x = Convert.ToDouble(SplittedStr[3].ToString());
                                    FilePoint.y = Convert.ToDouble(SplittedStr[4].ToString());
                                }
                                
                                // нет Mt (z.B. точка ликвидируется)
                                if (!SplittedStr[5].Contains("-"))
                                {
                                    FilePoint.Mt = Convert.ToDouble(SplittedStr[5].ToString());
                                }

                                FilePoint.Description = SplittedStr[6].ToString();
                                resPoly.AddPoint(FilePoint);
                                //Внутренние границы
                                goto nextPolygonstring;
                            ChildsHere:

                                if (line.Contains("Child"))
                                {
                                    TMyOutLayer child = resPoly.AddChild();
                                    line = readFile.ReadLine();
                                    while (!line.Contains("EndChild"))
                                    {
                                        StrCounter++;
                                        string[] ChildStr = line.Split(TabDelimiter.ToCharArray()); //Сплпиттер по tab (\t)
                                        Point ChildPoint = new Point();
                                        ChildPoint.id = StrCounter;
                                        ChildPoint.NumGeopointA = ChildStr[0].ToString();
                                        if (!ChildStr[1].Contains("-"))
                                        {
                                            ChildPoint.oldX = Convert.ToDouble(ChildStr[1].ToString());
                                            ChildPoint.oldY = Convert.ToDouble(ChildStr[2].ToString());
                                            ChildPoint.Status = 4;
                                        }
                                        else ChildPoint.Status = 0;

                                        if (!ChildStr[3].Contains("-"))
                                        {
                                            ChildPoint.x = Convert.ToDouble(ChildStr[3].ToString());
                                            ChildPoint.y = Convert.ToDouble(ChildStr[4].ToString());
                                        }
                                        if (!ChildStr[5].Contains("-"))
                                        {
                                            ChildPoint.Mt = Convert.ToDouble(ChildStr[5].ToString());
                                        }
                                        ChildPoint.Description = ChildStr[6].ToString();
                                        child.AddPoint(ChildPoint);
                                        line = readFile.ReadLine();
                                    }
                                }
                            nextPolygonstring: line = readFile.ReadLine();
                            }
                            resPolys.AddPolygon(resPoly);
                        }
                    }
                next:;
                }
                readFile.Close();
                readFile = null;
                return resPolys;
            }
            catch (IOException ex)
            {
                //  MessageBox.Show(ex.ToString());
                return null;
            }
            return null;
        }


    }

    public class TextWriter
    {
        public const string FixosoftFileSignature = "#Fixosoft NumXYZD data format V2015";
        public const string FixosoftFileSignature2 = "#Fixosoft NumXYZD data format V2015.2";
        public const string FixosoftFileSignature3 = "#Fixosoft NumXYZD data format V2016";
        public const string FixosoftFileSignature4 = "#Fixosoft NumXYZD data format V2017";
        public const string FixosoftFileSign5 = "#Fixosoft spatial text file V2018";//"#Fixosoft Num oldx oldy XY MtD data format V2018";
        public void SaveAsFixosoftTXT2015(string FileName, TMyPolygon ES)
        {
            if (ES.PointCount == 0) return;
            System.IO.TextWriter writer = new StreamWriter(FileName);
            writer.WriteLine(FixosoftFileSignature);
            writer.WriteLine("#Producer: XMLReaderCS application. " + DateTime.Now.ToString());
            writer.WriteLine("# Файл формата FTEO - Разделители полей tab");
            writer.WriteLine("# Поля файла " + FileName + " :");
            writer.WriteLine("# ИмяТочки,X,Y,Z,Описание-Код.");
            //writer.WriteLine("Polygon " + (ES.Childs.Count + 1).ToString());
            writer.WriteLine("Polygon " + ES.Definition);
            for (int i = 0; i <= ES.PointCount - 1; i++)
            {
                writer.WriteLine(ES[i].NumGeopointA + "\t" +
                                 ES[i].x_s + "\t" +
                                 ES[i].y_s + "\t" +
                                 ES[i].z_s + "\t" +
                                 ES[i].Description);
            }

            for (int ic = 0; ic <= ES.Childs.Count - 1; ic++)
            {
                writer.WriteLine("Child" + (ic + 1).ToString());
                for (int ici = 0; ici <= ES.Childs[ic].PointCount - 1; ici++)
                    writer.WriteLine(ES.Childs[ic][ici].NumGeopointA + "\t" +
                                 ES.Childs[ic][ici].x_s + "\t" +
                                 ES.Childs[ic][ici].y_s + "\t" +
                                 ES.Childs[ic][ici].z_s + "\t" +
                                 ES.Childs[ic][ici].Description);
                writer.WriteLine("EndChild");
            }
            writer.WriteLine("EndPolygon");
            writer.Close();
        }
        public void SaveAsFixosoftTXT2015(string FileName, TPolygonCollection ES)
        {
            if (ES.Count == 0) return;
            System.IO.TextWriter writer = new StreamWriter(FileName);
            writer.WriteLine(FixosoftFileSignature);
            writer.WriteLine("#Producer: XMLReaderCS application. " + DateTime.Now.ToString());
            writer.WriteLine("# Файл формата FTEO - Разделители полей tab");
            writer.WriteLine("# Поля файла " + FileName + " :");
            writer.WriteLine("# ИмяТочки,X,Y,Z,Описание-Код.");
            writer.WriteLine("# Полигонов " + ES.Count.ToString() + " Parentd_id " + ES.Parent_id.ToString());
            for (int ic = 0; ic <= ES.Count - 1; ic++)
            {
                writer.WriteLine("Polygon " + (ES[ic].Definition));
                for (int i = 0; i <= ES[ic].PointCount - 1; i++)
                {
                    writer.WriteLine(ES[ic][i].NumGeopointA + "\t" +
                                     ES[ic][i].x_s + "\t" +
                                     ES[ic][i].y_s + "\t" +
                                     ES[ic][i].z_s + "\t" +
                                     ES[ic][i].Description);
                }

                for (int ich = 0; ich <= ES[ic].Childs.Count - 1; ich++)
                {
                    writer.WriteLine("Child" + (ich + 1).ToString());
                    for (int ici = 0; ici <= ES[ic].Childs[ich].PointCount - 1; ici++)
                        writer.WriteLine(ES[ic].Childs[ich][ici].NumGeopointA + "\t" +
                                     ES[ic].Childs[ich][ici].x_s + "\t" +
                                     ES[ic].Childs[ich][ici].y_s + "\t" +
                                     ES[ic].Childs[ich][ici].z_s + "\t" +
                                     ES[ic].Childs[ich][ici].Description);
                    writer.WriteLine("EndChild");
                }
                writer.WriteLine("EndPolygon");
            }
            writer.Close();
        }
        public void SaveAsFixosoftTXT2016(string FileName, TMyPolygon ES)
        {
            TPolygonCollection pl = new TPolygonCollection(ES.Parent_Id);
            pl.AddPolygon(ES);
            SaveAsFixosoftTXT2016(FileName, pl);
        }
        public void SaveAsFixosoftTXT2017(string FileName, TPolyLines ES)
        {
            if (ES.Count == 0) return;
            System.IO.TextWriter writer = new StreamWriter(FileName);
            writer.WriteLine(FixosoftFileSignature4);
            writer.WriteLine("# " + DateTime.Now.ToString());  //"Версия {0}", 
            writer.WriteLine("# Producer: netfteo " +
                                String.Format(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));  //"Версия {0}", 
            writer.WriteLine("# Разделители полей tab. Кодировка ANSI");
            writer.WriteLine("# Поля файла " + FileName + " :");
            writer.WriteLine("# ИмяТочки,x,y,z,Mt,Описание(Код).");
            writer.WriteLine("# Полилиний " + ES.Count.ToString());
            for (int ic = 0; ic <= ES.Count - 1; ic++)
            {
                writer.WriteLine("Polyline " + (ES[ic].Definition));
                for (int i = 0; i <= ES[ic].PointCount - 1; i++)
                {
                    string ptname;
                    if (ES[ic][i].Status == 0) ptname = "н" + ES[ic][i].NumGeopointA;
                    else ptname = ES[ic][i].NumGeopointA;

                    writer.WriteLine(ptname + "\t" +
                                     ES[ic][i].x_s + "\t" +
                                     ES[ic][i].y_s + "\t" +
                                     ES[ic][i].z_s + "\t" +
                                     ES[ic][i].Mt_s + "\t" +
                                     ES[ic][i].Description);
                }
                writer.WriteLine("EndPolyline");
            }
            writer.Close();
        }

        public void SaveAsFixosoftTXT2018(string FileName, TMyPolygon ES, Encoding encoding)
        {
            TPolygonCollection items = new TPolygonCollection();
            items.AddPolygon(ES);
            SaveAsFixosoftTXT2018(FileName, items, encoding);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="ES"></param>
        /// <param name="encoding"></param>
        public void SaveAsFixosoftTXT2018(string FileName, TPolygonCollection ES, Encoding encoding)
        {
            if (ES.Count == 0) return;
            System.IO.TextWriter writer = new StreamWriter(FileName);
            writer.WriteLine(FixosoftFileSign5);
            writer.WriteLine("# Created " + DateTime.Now.ToString()+". "+
                             "Library: netfteo " +
                                String.Format(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));  //"Версия {0}", 
            writer.WriteLine("# Разделители полей tab. Кодировка " + encoding.EncodingName + ".  Поля файла: ");
            writer.WriteLine("# Номер;  Старый X;   Старый Y;   Новый X;    Новый Y;    Погрешность;    Описание закрепления");
            writer.WriteLine("# Полигонов " + ES.Count.ToString() );
            for (int ic = 0; ic <= ES.Count - 1; ic++)
            {
                writer.WriteLine("Polygon"+ "\t" + (ES[ic].Definition));
                for (int i = 0; i <= ES[ic].PointCount - 1; i++)
                {
                    string ptname;
                    if (ES[ic][i].Status == 0) ptname = "н" + ES[ic][i].NumGeopointA;
                    else ptname = ES[ic][i].NumGeopointA;

                    writer.WriteLine(ptname + "\t" +
                                     ES[ic][i].oldX_s+ "\t" +
                                     ES[ic][i].oldY_s+ "\t" +
                                     ES[ic][i].x_s + "\t" +
                                     ES[ic][i].y_s + "\t" +
                                     ES[ic][i].Mt_s + "\t" +
                                     ES[ic][i].Description + "\t" +
                                     ES[ic][i].OrdIdent);
                }

                for (int ich = 0; ich <= ES[ic].Childs.Count - 1; ich++)
                {
                    writer.WriteLine("Child" + (ich + 1).ToString());
                    for (int ici = 0; ici <= ES[ic].Childs[ich].PointCount - 1; ici++)
                    {
                        string ptname;
                        if (ES[ic].Childs[ich][ici].Status == 0) ptname = "н" + ES[ic].Childs[ich][ici].NumGeopointA;
                        else ptname = ES[ic].Childs[ich][ici].NumGeopointA;

                        writer.WriteLine(ptname + "\t" +
                                     ES[ic].Childs[ich][ici].oldX_s + "\t" +
                                     ES[ic].Childs[ich][ici].oldY_s + "\t" +
                                     ES[ic].Childs[ich][ici].x_s + "\t" +
                                     ES[ic].Childs[ich][ici].y_s + "\t" +
                                     ES[ic].Childs[ich][ici].Mt_s + "\t" +
                                     ES[ic].Childs[ich][ici].Description + "\t" +
                                     ES[ic].Childs[ich][ici].OrdIdent);
                    }
                    writer.WriteLine("EndChild");
                }
                writer.WriteLine("EndPolygon");
            }
            writer.Close();
        }
        public void SaveAsFixosoftTXT2016(string FileName, TPolygonCollection ES)
        {
            if (ES.Count == 0) return;
            System.IO.TextWriter writer = new StreamWriter(FileName);
            writer.WriteLine(FixosoftFileSignature3);
            writer.WriteLine("# " + DateTime.Now.ToString());  //"Версия {0}", 
            writer.WriteLine("# Producer: netfteo " +
                                String.Format(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));  //"Версия {0}", 
            writer.WriteLine("# Разделители полей tab. Кодировка ANSI");
            writer.WriteLine("# Поля файла " + FileName + " :");
            writer.WriteLine("# ИмяТочки,x,y,z,Mt,Описание(Код).");
            writer.WriteLine("# Полигонов " + ES.Count.ToString() + " Parentd_id " + ES.Parent_id.ToString());
            for (int ic = 0; ic <= ES.Count - 1; ic++)
            {
                writer.WriteLine("Polygon " + (ES[ic].Definition));
                for (int i = 0; i <= ES[ic].PointCount - 1; i++)
                {
                    string ptname;
                    if (ES[ic][i].Status == 0) ptname = "н" + ES[ic][i].NumGeopointA;
                    else ptname = ES[ic][i].NumGeopointA;

                    writer.WriteLine(ptname + "\t" +
                                     ES[ic][i].x_s + "\t" +
                                     ES[ic][i].y_s + "\t" +
                                     ES[ic][i].z_s + "\t" +
                                     ES[ic][i].Mt_s + "\t" +
                                     ES[ic][i].Description);
                }

                for (int ich = 0; ich <= ES[ic].Childs.Count - 1; ich++)
                {
                    writer.WriteLine("Child" + (ich + 1).ToString());
                    for (int ici = 0; ici <= ES[ic].Childs[ich].PointCount - 1; ici++)
                    {
                        string ptname;
                        if (ES[ic].Childs[ich][ici].Status == 0) ptname = "н" + ES[ic].Childs[ich][ici].NumGeopointA;
                        else ptname = ES[ic].Childs[ich][ici].NumGeopointA;

                        writer.WriteLine(ptname + "\t" +
                                     ES[ic].Childs[ich][ici].x_s + "\t" +
                                     ES[ic].Childs[ich][ici].y_s + "\t" +
                                     ES[ic].Childs[ich][ici].z_s + "\t" +
                                     ES[ic].Childs[ich][ici].Mt_s + "\t" +
                                     ES[ic].Childs[ich][ici].Description);
                    }
                    writer.WriteLine("EndChild");
                }
                writer.WriteLine("EndPolygon");
            }
            writer.Close();
        }

      
        public void SaveAsOMSTXT(string CN, string FileName, PointList ES)
        {
            if (ES.Count == 0) return;
            System.IO.TextWriter writer = new StreamWriter(FileName);
            writer.WriteLine("# Каталог пунктов геодезических сетей");
            writer.WriteLine("# netfteo " +
                                String.Format(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString())
                                + " ANSI, " + DateTime.Now.ToString());  //"Версия {0}", 

            writer.WriteLine("# Номер, тип пункта опорной межевой сети \n Номер на плане,x,y, \n Класс сети");
            writer.WriteLine("# Записей " + ES.Count.ToString());
            writer.WriteLine("");
            writer.WriteLine("**************" + CN + "******************************");
            for (int i = 0; i <= ES.Count - 1; i++)
            {
                writer.WriteLine(ES[i].Code + ":");
                writer.WriteLine("Номер на плане \t" + ES[i].NumGeopointA);
                writer.WriteLine("\t x \t" + ES[i].x_s);
                writer.WriteLine("\t y \t" + ES[i].y_s);
                writer.WriteLine("Класс сети");
                writer.WriteLine("\t" + ES[i].Description);
                writer.WriteLine("*****************************************");
                writer.WriteLine("");
            }
            writer.Close();
        }
        public void SaveAsNikon(string FileName, PointList ES)
        {
            if (ES.Count == 0) return;
            System.IO.TextWriter writer = new StreamWriter(FileName);

            for (int i = 0; i <= ES.Count - 1; i++)
            {
                if (ES[i].Code != null)
                    writer.WriteLine(ES[i].NumGeopointA + "," + ES[i].x_s + "," + ES[i].y_s + "," + ES[i].z_s + "," + ES[i].Code);
                else
                    writer.WriteLine(ES[i].NumGeopointA + "," + ES[i].x_s + "," + ES[i].y_s + "," + ES[i].z_s);
            }
            writer.Close();
        }


        //Формат csv файла Техно Кад (наверное экспресс....замедленнный)
        /*Контур;Префикс номера;Номер;Старый X;Старый Y;Новый X;Новый Y;Метод определения;Формула;Радиус;Погрешность;Описание закрепления
        // [1];н;1;;;531645,96;1262377,82;;;;0,00;626003000000
                 ;;;;;;;;;;;
             [1.1];н;4;;;530684,92;1262376,38;;;;0,00;626003000000
             [1.1];н;5;;;530512,62;1262933,02;;;;0,00;626003000000
             [1.1];н;6;;;531640,58;1262945,68;;;;0,00;626003000000
             [1.1];н;4;;;530684,92;1262376,38;;;;0,00;626003000000
          */
        private void WriteEs2csv(System.IO.TextWriter writer, TMyPolygon ES, int EsNumber)
        {

            writer.WriteLine(";;;;;;;;;;;");
            for (int i = 0; i <= ES.PointCount - 1; i++)
            {
                writer.WriteLine("[" + EsNumber.ToString() + "];" +
                              ES[i].Pref + ";" + ES[i].NumGeopointA + ";" +
                              //oldx;oldx+
                              ES[i].oldX_s + ";" +
                              ES[i].oldY_s + ";" +
                              ES[i].x_s + ";" +
                              ES[i].y_s + ";" +
                                      ";;;" +
                              ES[i].Mt_s + ";" +
                              //Описание закрепления
                              //626001000000 Долговременный межевой знак // 626002000000 Временный межевой знак // 626003000000 Закрепление отсутствует
                              "626003000000");
            }

            for (int ich = 0; ich <= ES.Childs.Count - 1; ich++)
            {
                writer.WriteLine(";;;;;;;;;;;");
                for (int ici = 0; ici <= ES.Childs[ich].PointCount - 1; ici++)
                    writer.WriteLine("[" + EsNumber.ToString() + "." + (ich + 1).ToString() + "];" +
                             ES.Childs[ich][ici].Pref + ";" + ES.Childs[ich][ici].NumGeopointA + ";" +
                             //oldx;oldx+
                             ES.Childs[ich][ici].oldX_s + ";" +
                             ES.Childs[ich][ici].oldY_s + ";" +
                             ES.Childs[ich][ici].x_s + ";" +
                             ES.Childs[ich][ici].y_s + ";" +
                                      ";;;" +
                             ES.Childs[ich][ici].Mt_s + ";" + "626003000000");
            }
        }

        public void SaveAsTexnoCADCSV(string FileName, TMyPolygon ES)
        {
            if (ES.PointCount == 0) return;
            System.IO.Stream file = new System.IO.FileStream(FileName, FileMode.Create);
            System.IO.TextWriter writer = new System.IO.StreamWriter(file, Encoding.Unicode);
            writer.WriteLine("Контур;Префикс номера;Номер;Старый X;Старый Y;Новый X;Новый Y;Метод определения;Формула;Радиус;Погрешность;Описание закрепления");
            WriteEs2csv(writer, ES, 1);
            writer.Close();
        }

        public void SaveAsTexnoCADCSV(string FileName, TPolygonCollection ES)
        {
            if (ES.Count == 0) return;
            System.IO.Stream file = new System.IO.FileStream(FileName, FileMode.Create);
            System.IO.TextWriter writer = new System.IO.StreamWriter(file, Encoding.Unicode);
            writer.WriteLine("Контур;Префикс номера;Номер;Старый X;Старый Y;Новый X;Новый Y;Метод определения;Формула;Радиус;Погрешность;Описание закрепления");
            writer.WriteLine(";;;;;;;;;;;");
            for (int ic = 0; ic <= ES.Count - 1; ic++)
            {
                WriteEs2csv(writer, ES[ic], ic + 1);
            }
            writer.Close();
        }


        public void WriteMifPoints(System.IO.TextWriter writer, System.IO.TextWriter writerMIDA, PointList Points)
        {
            for (int i = 0; i <= Points.Count - 1; i++)
            {
                writer.WriteLine("Point " + Points[i].y_s + " " + Points[i].x_s);
                writer.WriteLine("    Symbol (34,16711680,6)");
                writerMIDA.WriteLine("\"" + Points[i].Code + "\"$" +
                                     "\"" + Points[i].Description + "\"$" +
                                     "\"" + Points[i].NumGeopointA + "\"$" +
                                            Points[i].x_s + "$" +
                                            Points[i].y_s
                                     );
            }
        }

        private void WriteMifRegion(System.IO.TextWriter writer, System.IO.TextWriter writerMIDA, TMyPolygon Poly)
        {
            writer.WriteLine("Region " + Convert.ToString(Poly.Childs.Count + 1));
            writer.WriteLine(Poly.Count.ToString());
            for (int ip = 0; ip <= Poly.Count - 1; ip++)
            {
                writer.Write(Poly[ip].y_s);
                writer.Write(" ");
                writer.WriteLine(Poly[ip].x_s);
            }

            //Внутр. границы
            for (int cc = 0; cc <= Poly.Childs.Count - 1; cc++)
            {
                writer.WriteLine(Poly.Childs[cc].Count.ToString());
                for (int ip = 0; ip <= Poly.Childs[cc].Count - 1; ip++)
                {
                    writer.Write(Poly.Childs[cc][ip].y_s);
                    writer.Write(" ");
                    writer.WriteLine(Poly.Childs[cc][ip].x_s);
                }
            }
            writer.WriteLine("Pen (1,2,0)");
            writer.WriteLine("Brush (1,0,16777215)");
            writerMIDA.WriteLine("\"" + Poly.Definition + "\"$");

        } // Polygon

        private void WriteMifPline(System.IO.TextWriter writer, PointList Vertex)   // PolyLine
        {
            writer.WriteLine("Pline " + Convert.ToString(Vertex.PointCount)); // ? +1 !!!

            for (int ip = 0; ip <= Vertex.PointCount - 1; ip++)
            {
                writer.Write(Vertex[ip].y_s);
                writer.Write(" ");
                writer.WriteLine(Vertex[ip].x_s);
            }
            writer.WriteLine("    Pen (1,2,0)");

        }

        /// <summary>
        /// Mapinfo mif-file export functions
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="Points"></param>
        public void SaveAsmif(string FileName, TMyPolygon ES)
        {
            TPolygonCollection pls = new TPolygonCollection(ES.Parent_Id);
            pls.AddPolygon(ES);
            SaveAsmif(FileName, pls);
        }

        /// <summary>
        /// Mapinfo mif-file export functions
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="writerMIDA"></param>
        /// <param name="Points"></param>
        public void SaveAsmif(string FileName, TPolygonCollection XmlPolygons)
        {
            {
                if (XmlPolygons == null) return;
                if (XmlPolygons.Count == 0) return;

                string baseFileName = Path.GetDirectoryName(FileName) + "\\" + Path.GetFileNameWithoutExtension(FileName);

                System.IO.TextWriter writer = new StreamWriter(baseFileName + ".mif"); //Только Точки
                System.IO.TextWriter writerMID = new StreamWriter(baseFileName + ".mid", false, Encoding.GetEncoding("Windows-1251"));
                System.IO.TextWriter writerP = new StreamWriter(baseFileName + "P.mif"); //Только полигоны // !!! Wrong in win7 !!!
                System.IO.TextWriter writerMIDP = new StreamWriter(baseFileName + "P.mid", false, Encoding.GetEncoding("Windows-1251"));


                // точки  и полинии  в отдельный файл:
                writer.WriteLine("Version 450");
                writer.WriteLine("Charset \"WindowsCyrillic\"");
                writer.WriteLine("Delimiter \"$\"");
                writer.WriteLine("CoordSys NonEarth Units \"m\" Bounds (" +
                                            XmlPolygons.Get_Bounds.MinY.ToString() + "," + XmlPolygons.Get_Bounds.MinX.ToString() + ")  (" +
                                            XmlPolygons.Get_Bounds.MaxY.ToString() + "," + XmlPolygons.Get_Bounds.MaxX.ToString() + ")");
                writer.WriteLine("Columns 5");
                writer.WriteLine("    Point_Name Char(127)");
                writer.WriteLine("    Net_Klass  Char(127)");
                writer.WriteLine("    Number Char(127)");
                writer.WriteLine("    X Float");
                writer.WriteLine("    Y Float");
                writer.WriteLine("Data");
                writer.WriteLine("");


                writerP.WriteLine("Version 450");
                writerP.WriteLine("Charset \"WindowsCyrillic\"");
                writerP.WriteLine("Delimiter \"$\"");
                writerP.WriteLine("CoordSys NonEarth Units \"m\"");
                writerP.WriteLine("Bounds (" + XmlPolygons.Get_Bounds.MinY.ToString() + "," + XmlPolygons.Get_Bounds.MinX.ToString() + ")  (" +
                                              XmlPolygons.Get_Bounds.MaxY.ToString() + "," + XmlPolygons.Get_Bounds.MaxX.ToString() + ")");
                writerP.WriteLine("Columns 3");
                writerP.WriteLine("CN Char(127)");
                writerP.WriteLine("BlockCN  Char(127)");
                writerP.WriteLine("LOT_ID Char(127)");
                writerP.WriteLine("Data");
                writerP.WriteLine("");


                for (int i = 0; i <= XmlPolygons.Count() - 1; i++)
                    if (XmlPolygons[i].PointCount > 0)
                    {
                        if ((XmlPolygons[i][0].x != XmlPolygons[i][XmlPolygons[i].PointCount - 1].x) &
                            (XmlPolygons[i][0].y != XmlPolygons[i][XmlPolygons[i].PointCount - 1].y))
                            WriteMifPline(writer, XmlPolygons[i]);

                        else // Polygons only file (layer):
                        {

                            WriteMifRegion(writerP, writerMIDP, XmlPolygons[i]);

                        }

                    }


                for (int i = 0; i <= XmlPolygons.Count - 1; i++)
                {
                    WriteMifPoints(writer, writerMID, XmlPolygons[i]);
                    // точки вгграниц:
                    for (int cc = 0; cc <= XmlPolygons[i].Childs.Count - 1; cc++)
                        WriteMifPoints(writer, writerMID, XmlPolygons[i].Childs[cc]);
                }
                writer.Close();
                writerMID.Close();
                writerP.Close();
                writerMIDP.Close();

            }
        }



    }


    /// <summary>
    /// Класс-обертка для Cryptography
    /// </summary>
    public static class CryptographyWrapper
    {
        private static string encrFolder = @"C:\Encrypt\";
        // Encrypt a file using a public key.
        public static void EncryptFile(string inFile, RSACryptoServiceProvider rsaPublicKey)
        {
            using (AesManaged aesManaged = new AesManaged())
            {
                // Create instance of AesManaged for
                // symetric encryption of the data.
                aesManaged.KeySize = 256;
                aesManaged.BlockSize = 128;
                aesManaged.Mode = CipherMode.CBC;
                using (ICryptoTransform transform = aesManaged.CreateEncryptor())
                {
                    RSAPKCS1KeyExchangeFormatter keyFormatter = new RSAPKCS1KeyExchangeFormatter(rsaPublicKey);
                    byte[] keyEncrypted = keyFormatter.CreateKeyExchange(aesManaged.Key, aesManaged.GetType());

                    // Create byte arrays to contain
                    // the length values of the key and IV.
                    byte[] LenK = new byte[4];
                    byte[] LenIV = new byte[4];

                    int lKey = keyEncrypted.Length;
                    LenK = BitConverter.GetBytes(lKey);
                    int lIV = aesManaged.IV.Length;
                    LenIV = BitConverter.GetBytes(lIV);

                    // Write the following to the FileStream
                    // for the encrypted file (outFs):
                    // - length of the key
                    // - length of the IV
                    // - ecrypted key
                    // - the IV
                    // - the encrypted cipher content

                    int startFileName = inFile.LastIndexOf("\\") + 1;
                    // Change the file's extension to ".enc"
                    string outFile = encrFolder + inFile.Substring(startFileName, inFile.LastIndexOf(".") - startFileName) + ".enc";
                    Directory.CreateDirectory(encrFolder);

                    using (FileStream outFs = new FileStream(outFile, FileMode.Create))
                    {

                        outFs.Write(LenK, 0, 4);
                        outFs.Write(LenIV, 0, 4);
                        outFs.Write(keyEncrypted, 0, lKey);
                        outFs.Write(aesManaged.IV, 0, lIV);

                        // Now write the cipher text using
                        // a CryptoStream for encrypting.
                        using (CryptoStream outStreamEncrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                        {

                            // By encrypting a chunk at
                            // a time, you can save memory
                            // and accommodate large files.
                            int count = 0;
                            int offset = 0;

                            // blockSizeBytes can be any arbitrary size.
                            int blockSizeBytes = aesManaged.BlockSize / 8;
                            byte[] data = new byte[blockSizeBytes];
                            int bytesRead = 0;

                            using (FileStream inFs = new FileStream(inFile, FileMode.Open))
                            {
                                do
                                {
                                    count = inFs.Read(data, 0, blockSizeBytes);
                                    offset += count;
                                    outStreamEncrypted.Write(data, 0, count);
                                    bytesRead += blockSizeBytes;
                                }
                                while (count > 0);
                                inFs.Close();
                            }
                            outStreamEncrypted.FlushFinalBlock();
                            outStreamEncrypted.Close();
                        }
                        outFs.Close();
                    }
                }
            }
        }
        public static byte[] Sign(string text, string certSubject)

        {

            // Access Personal (MY) certificate store of current user

            X509Store my = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            my.Open(OpenFlags.ReadOnly);


            // Find the certificate we'll use to sign

            RSACryptoServiceProvider csp = null;

            foreach (X509Certificate2 cert in my.Certificates)

            {

                if (cert.Subject.Contains(certSubject))

                {

                    // We found it.

                    // Get its associated CSP and private key

                    csp = (RSACryptoServiceProvider)cert.PrivateKey;

                }

            }

            if (csp == null)

            {

                throw new Exception("No valid cert was found");

            }


            // Hash the data

            SHA1Managed sha1 = new SHA1Managed();

            UnicodeEncoding encoding = new UnicodeEncoding();

            byte[] data = encoding.GetBytes(text);

            byte[] hash = sha1.ComputeHash(data);


            // Sign the hash

            return csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));

        }
        public static byte[] Sign(byte[] filebody, X509Certificate2 certificate)
        {

            // Access Personal (MY) certificate store of current user

            X509Store my = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            my.Open(OpenFlags.ReadOnly);
            RSACryptoServiceProvider csp = null;
            if (certificate.HasPrivateKey)
            {
                csp = (RSACryptoServiceProvider)certificate.PrivateKey;
                if (csp == null)
                {
                    throw new Exception("No valid CSP was found");
                }
            }
            //else
            //    csp = (RSACryptoServiceProvider)certificate.PublicKey.Key;


            // Hash the data

            SHA1Managed sha1 = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();



            byte[] hash = sha1.ComputeHash(filebody);

            //return  csp.SignData(filebody, new SHA512CryptoServiceProvider());
            // Sign the hash
            return csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
        }

        public static bool Verify(string text, byte[] signature, string certPath)

        {

            // Load the certificate we'll use to verify the signature from a file

            X509Certificate2 cert = new X509Certificate2(certPath);

            // Note:

            // If we want to use the client cert in an ASP.NET app, we may use something like this instead:

            // X509Certificate2 cert = new X509Certificate2(Request.ClientCertificate.Certificate);


            // Get its associated CSP and public key

            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;


            // Hash the data

            SHA1Managed sha1 = new SHA1Managed();

            UnicodeEncoding encoding = new UnicodeEncoding();

            byte[] data = encoding.GetBytes(text);

            byte[] hash = sha1.ComputeHash(data);


            // Verify the signature with the hash

            return csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), signature);

        }

        /// <summary>
        /// verifying the signature of PKCS #7 formatted messages
        /// </summary>
        /*
        public static bool VerifyP7s(byte[] signature, X509Certificate2 certificate)
        {
            if (signature == null)
                throw new ArgumentNullException("signature");


            // decode the signature

            System.Security.Cryptography.Pkcs.SignedCms verifyCms = new System.Security.Cryptography.Pkcs.SignedCms();
            verifyCms.Decode(signature);
            var test = verifyCms.ContentInfo;
            //verifyCms.CheckSignature(true);
            X509Certificate2Collection cmsCerts = verifyCms.Certificates;


            // verify it
            if (certificate == null)
                throw new ArgumentNullException("certificate");
            try
            {
                verifyCms.CheckSignature(new X509Certificate2Collection(certificate), false);
                return true;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }
          */
        public static List<string> DisplayCerts(byte[] signature)
        {
            if (signature == null)
                throw new ArgumentNullException("signature");
            List<string> res = new List<string>();
            try
            {
                // decode the signature

                System.Security.Cryptography.Pkcs.SignedCms verifyCms = new System.Security.Cryptography.Pkcs.SignedCms();
                verifyCms.Decode(signature);
                var test = verifyCms.ContentInfo;
                X509Certificate2Collection cmsCerts = verifyCms.Certificates;
                foreach (X509Certificate2 c in cmsCerts)
                    res.Add(c.GetNameInfo(X509NameType.SimpleName, false));

            }
            catch (CryptographicException)
            {
                return null;
            }
            return res;
        }

        /// <summary>
        /// Листинг сертификатов. Средствами только wyncrypt.
        /// </summary>
        /// <returns></returns>
        public static List<string> DisplayCerts()
        {
            List<string> res = new List<string>();
            X509Store store;
            /*
            store = new X509Store(StoreName.Root);
            store.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 c in store.Certificates)
                res.Add(c.SubjectName.Name);
            store.Close();
            */
            store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly);
            /* for .FindBySubjectDistinguishedName:
             foreach (X509Certificate2 c in store.Certificates)
                 res.Add(c.SubjectName.Name);
                 */
            // for .FindBySubjectCNName
            foreach (X509Certificate2 c in store.Certificates)
            {
                res.Add(c.GetNameInfo(X509NameType.SimpleName, false));
            }
            /*

             foreach (X509Certificate2 c in store.Certificates)
                 res.Add(c.GetName().ToString());
                 */
            store.Close();
            return res;
        }

        public static List<X509Certificate2> DisplayCerts(string storename)
        {
            List<X509Certificate2> res = new List<X509Certificate2>();
            X509Store store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 c in store.Certificates)
            {
                res.Add(c);
            }
            store.Close();
            return res;
        }

        /// <summary>
        /// Поиск сертификата по имени
        /// </summary>
        /// <param name="subject">X500DistinguishedName - string eq</param>
        /// <returns></returns>
        public static X509Certificate2 GetCertBySubject(string subject)
        {

            X509Store store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection listCerts = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, subject, false);
            if (listCerts.Count == 1)
            { return listCerts[0]; }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Поиск сертификата по CN-имени
        /// </summary>
        /// <param name="subject">CN name of subject</param>
        /// <returns></returns>
        public static X509Certificate2 GetCertBySubjectCN(string subjectCN)
        {

            X509Store store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection listCerts = store.Certificates.Find(X509FindType.FindBySubjectName, subjectCN, false);
            if (listCerts.Count == 1)
            { return listCerts[0]; }
            else
            {
                return null;
            }
        }

        public static X509Certificate2 GetCertBySerial(string serial)
        {

            X509Store store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection listCerts = store.Certificates.Find(X509FindType.FindBySerialNumber, serial, false);
            if (listCerts.Count == 1)
            { return listCerts[0]; }
            else
            {
                return null;
            }
        }
    }


    /// <summary>
    /// GOST CSP Provider wrapper class. Требует установленнoго CADESCOM (cadescom.dll)
    /// </summary>
    /*
    public static class CadesWrapper
    {
        /// <summary>
        /// Поиск сертификата по CN-имени. Из поля SubjectName выбирается cn=subjectCN 
        /// </summary>
        /// <param name="subject">CN name of subject</param>
        /// <returns></returns>
        public static CAdESCOM.CPCertificate Find(string subjectCN)
        {
            // Сразу ищем в Wyncrypt:
            X509Store store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection listCerts = store.Certificates.Find(X509FindType.FindBySubjectName, subjectCN, false);
            string serial = listCerts[0].SerialNumber; // Сохраняем серийник
            // Теперь выбираем в WinCryptEx (CADES) по серийнику:
            CAdESCOM.CPStore Cstore = new CAdESCOM.CPStore();
            Cstore.Open(CAPICOM_STORE_LOCATION.CAPICOM_CURRENT_USER_STORE,
                        "My",
                        CAPICOM_STORE_OPEN_MODE.CAPICOM_STORE_OPEN_READ_ONLY);

            foreach (CAdESCOM.CPCertificate crt in Cstore.Certificates)
            {
                if (crt.SerialNumber == serial)
                    return crt;
            }
            return null;
        }

        public static CAdESCOM.CPCertificate FindBySerial(string serial)
        {
            // Сразу ищем в Wyncrypt:
            //X509Store store = new X509Store(StoreName.My);
            //store.Open(OpenFlags.ReadOnly);
            //X509Certificate2Collection listCerts = store.Certificates.Find(X509FindType.FindBySerialNumber, serial, false);
            //string serial = listCerts[0].SerialNumber; // Сохраняем серийник
            // Теперь выбираем в WinCryptEx (CADES) по серийнику:
            CAdESCOM.CPStore Cstore = new CAdESCOM.CPStore();
            Cstore.Open(CAPICOM_STORE_LOCATION.CAPICOM_CURRENT_USER_STORE,
                        "My",
                        CAPICOM_STORE_OPEN_MODE.CAPICOM_STORE_OPEN_READ_ONLY);

            foreach (CAdESCOM.CPCertificate crt in Cstore.Certificates)
            {
                if (crt.SerialNumber == serial)
                    return crt;
            }
            return null;
        }

            
        
/// <summary>
/// Подписать файл отсоедниенной подписью
/// </summary>
/// <param name="filename">Имя файла</param>
/// <param name="subjectname">Владелец сертифката (Субьект)</param>
        public static void SignFile(string filename, string subjectname)
        {
            byte[] filebody = System.IO.File.ReadAllBytes(filename);
            byte[] file_sig = null;

            X509Certificate2 cert = CryptographyWrapper.GetCertBySubjectCN(subjectname);
            //Select CryptoProviderType by szOID:
            if (cert.SignatureAlgorithm.Value == "1.2.643.2.2.3") // szOID for CSP Crypto Pro (GOST 3411)
               file_sig =  Sign_GOST(filebody, Find(subjectname));

            if (cert.SignatureAlgorithm.Value == "1.2.840.113549.1.1.4") // szOID_RSA_MD5RSA   "1.2.840.113549.1.1.4"
                file_sig = Sign_CAPICOM(filebody, (CAPICOM.Certificate) cert);
            
            System.IO.File.WriteAllBytes(filename + ".sig", file_sig);
        }


        private static byte[] Sign_X509(byte[] filebody, string subjectname)
        {
                        // Access Personal (MY) certificate store of current user

            X509Store my = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            my.Open(OpenFlags.ReadOnly);


            // Find the certificate we'll use to sign

            RSACryptoServiceProvider csp = null;

            foreach (X509Certificate2 cert in my.Certificates)
            {

                if (cert.Subject.Contains(subjectname))
                {

                    // We found it.

                    // Get its associated CSP and private key

                    csp = (RSACryptoServiceProvider)cert.PrivateKey;

                }

            }

            if (csp == null)
            {

                throw new Exception("No valid cert (CSP) was found");

            }


            // Hash the data

            SHA1Managed sha1 = new SHA1Managed();

            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] hash = sha1.ComputeHash(filebody);
            // Sign the hash
            byte[] sig= csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
            return sig;
        }


        /// <summary>
        /// Подпись через COM-объекты  "CAPICOM"  - нужна dll (классы COM)
        /// </summary>
        /// <param name="_FileName"></param>
        /// <param name="cert"></param>
        private static byte[] Sign_CAPICOM(byte[] filebody, CAPICOM.Certificate cert)
        {
             CAPICOM.SignedData CSPdata = new CAPICOM.SignedData();
             CSPdata.Content = Convert.ToBase64String(filebody); 
            if (cert.HasPrivateKey())
             {

                // RSACryptoServiceProvider csp = 
                 // Hash the data
                // SHA1Managed sha1 = new SHA1Managed();
                // byte[] hash = sha1.ComputeHash(filebody);
                // byte[] sig = csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
             }

            CAPICOM.Signer Signer = new CAPICOM.Signer();
            Signer.Certificate = cert;
            Signer.Options = CAPICOM.CAPICOM_CERTIFICATE_INCLUDE_OPTION.CAPICOM_CERTIFICATE_INCLUDE_WHOLE_CHAIN;
            string signature = CSPdata.Sign(Signer, true, CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_ANY);
            byte[] sigMsg = Encoding.Default.GetBytes(signature);
            return sigMsg;
        }

        /// <summary>
        /// Подписание по ГОСТ 34.11-94 (CADESCOM - COM of CryptoPro), OID = 1.2.643.2.2.3
        /// </summary>
        /// <param name="filebody">Сообщение для подписания</param>
        /// <param name="cert">Сертификат</param>
        private static byte[] Sign_GOST(byte[] filebody, CAdESCOM.CPCertificate cert)
        {
            CAdESCOM.CadesSignedData CSPdata = new CAdESCOM.CadesSignedData();
            CSPdata.ContentEncoding = CAdESCOM.CADESCOM_CONTENT_ENCODING_TYPE.CADESCOM_BASE64_TO_BINARY; // Первым строкой - кодировку
            CSPdata.Content = Convert.ToBase64String(filebody);                 // иначе перекодирует дважды !!!!
            //Хэш-значение данных
            CAdESCOM.CPHashedData Hash = new CAdESCOM.CPHashedData();
            Hash.Algorithm = (CAPICOM.CAPICOM_HASH_ALGORITHM)CAdESCOM.CADESCOM_HASH_ALGORITHM.CADESCOM_HASH_ALGORITHM_CP_GOST_3411;
            Hash.DataEncoding = CAdESCOM.CADESCOM_CONTENT_ENCODING_TYPE.CADESCOM_BASE64_TO_BINARY;
            Hash.Hash(CSPdata.Content); // Создать хэш строки. Есть расширение - SetHashValue() - инициализация готовым хэш-значением
            
          
            CAdESCOM.CPSigner CSPSigner = new CAdESCOM.CPSigner();
            CSPSigner.Certificate = cert;
            //TSA адрес в какой-то момент требовался - никак не хотело работать, а потом перестало требоваться....блеатдь (я такой внимательный)
            //CSPSigner.TSAAddress = "http://www.cryptopro.ru/tsp/tsp.srf"; //  адрес службы штампов времени.
                                                                          //"http://testca.cryptopro.ru/tsp/";
            CSPSigner.Options = CAPICOM.CAPICOM_CERTIFICATE_INCLUDE_OPTION.CAPICOM_CERTIFICATE_INCLUDE_WHOLE_CHAIN;

            try
            {
                string resHashCades = CSPdata.SignHash((CAPICOM.HashedData)Hash, 
                                                       CSPSigner, 
                                                       CAdESCOM.CADESCOM_CADES_TYPE.CADESCOM_CADES_BES,
                                                       CAdESCOM.CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);
                return Encoding.Default.GetBytes(resHashCades);
                /*  // Это тоже дает верную подпись, без употребелния CAdESCOM.CPHashedData :
                string resSignCades = CSPdata.SignCades(CSPSigner, 
                 *                                      CAdESCOM.CADESCOM_CADES_TYPE.CADESCOM_CADES_BES, 
                 *                                      true,
                 *                                      CAdESCOM.CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);       
                   return Encoding.Default.GetBytes(resSignCades);
                
            }




            catch (System.Runtime.InteropServices.COMException ex)
            {

                return Encoding.Default.GetBytes("OID = 1.2.643.2.2.3 SCP Error: \r\n" +
                    ex.Message + "\r\n" +
                    " ErrorCode " + ex.ErrorCode.ToString() + "\r\n" +
                    " source " + ex.Source);
            }
         }

    }
    */
}
