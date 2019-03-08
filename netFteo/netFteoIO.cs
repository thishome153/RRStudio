/*
 * Intput output classes. Unusable higher .NET 3.5
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;

//using System.Runtime.InteropServices;
//using System.Drawing; // inter sect with netfteo Point
using netFteo.Spatial;
//changes 3 to 11 ( и Sync)

namespace netFteo.IO
{
	/// <summary>
	///Класс для операций с файлами Fteo, mif, dxf
	/// </summary>
	public class TextReader
	{
		public const string TabDelimiter = "\t";  // tab
		public const string CommaDelimiter = ";";  // tab
		public string FileType = "";  // tab
		private String fBody;
		public string Body
		{
			get
			{
				return this.fBody;
			}
			set
			{
				this.fBody = value;
				//   this.BodyEncoding = System.Text.Encoding.GetEncoding(fBody).EncodingName;
			}
		}

		public string BodyEncoding;
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
			System.IO.TextReader readFile = new StreamReader(Fname);
			BodyLoad(Fname);
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
			TMyPolygon res = new TMyPolygon("REGION " + ringCount.ToString());
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
		///  Detect Encoding name, encoding Body field
		/// </summary>
		/// <param name="fname"></param>
		protected void BodyLoad(string fname)
		{
			using (var reader = new StreamReader(fname, Encoding.Default, true))
			{
				if (reader.Peek() >= 0) // you need this!
					reader.Read();
				this.BodyEncoding = reader.CurrentEncoding.EncodingName;
			}

			//"Кириллица (Windows)"
			if (this.BodyEncoding.Equals("Кириллица (Windows)"))
			{
				byte[] bodyWithEncode = File.ReadAllBytes(fname);
				Body = Encoding.Default.GetString(bodyWithEncode);
			}
			else
				Body = File.ReadAllText(fname);
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


			// TODO Encoding for mif:
			BodyLoad(baseFileName + ".mif");
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
								MIF_Points.AddPoint(MIF_ParseOrdinate(line.Substring(6), "POINT"));
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
								if (SplittedStr.Count() > 6)
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
		/*
				public void SaveAsFixosoftTXT2016(string FileName, TMyPolygon ES)
				{
					TPolygonCollection pl = new TPolygonCollection(ES.Parent_Id);
					pl.AddPolygon(ES);
					SaveAsFixosoftTXT2016(FileName, pl);
				}

		*/
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
			System.IO.TextWriter writer = new StreamWriter(FileName, false, encoding);
			writer.WriteLine(FixosoftFileSign5);
			writer.WriteLine("# Created " + DateTime.Now.ToString() + ". " +
							 "Library: netfteo " +
								String.Format(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));  //"Версия {0}", 
			writer.WriteLine("# Разделители полей tab. Кодировка " + encoding.EncodingName + ".  Поля файла: ");
			writer.WriteLine("# Номер;  Старый X;   Старый Y;   Новый X;    Новый Y;    Погрешность;    Описание закрепления");
			writer.WriteLine("# Полигонов " + ES.Count.ToString());
			for (int ic = 0; ic <= ES.Count - 1; ic++)
			{
				writer.WriteLine("Polygon" + "\t" + (ES[ic].Definition));
				for (int i = 0; i <= ES[ic].PointCount - 1; i++)
				{

					writer.WriteLine(((ES[ic][i].Status == 0) ? "н" + ES[ic][i].NumGeopointA : ES[ic][i].NumGeopointA) + "\t" +
									 ES[ic][i].oldX_s + "\t" +
									 ES[ic][i].oldY_s + "\t" +
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
						writer.WriteLine(
							((ES[ic].Childs[ich][ici].Status == 0) ? "н" + ES[ic].Childs[ich][ici].NumGeopointA : ES[ic].Childs[ich][ici].NumGeopointA) + "\t" +
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

		/*
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
*/

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

		public bool ByteArrayToFile(string fileName, byte[] byteArray)
		{
			try
			{
				using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					fs.Write(byteArray, 0, byteArray.Length);
					return true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception caught in process: {0}", ex);
				return false;
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



	//Класс, описывающий ответ сервера в виде массива объектов (при поиске типа /1?text=26...)
	public class LogServer_response
	{
		public string Service { get; set; }
		public string Version { get; set; }
		public string Client { get; set; }
		public string ApplicationType { get; set; }
		public string AppVersion { get; set; }
		public string Type { get; set; }
		public string Time { get; set; }
		public string state { get; set; }
		public string stateText { get; set; }
	}


	/*
	{
   "Service": "nodeapi",
   "Version": "1.0.0.22",
   "Client ": "10.66.77.150 login log",
   "ApplicationType ": "AppType expected",
   "AppVersion": "NC",
   "Type": "Login log",
   "Time ": "2019-03-04T08:30:16.780Z",
   "state": 200,
   "stateText": "Server ok"
}
*/

	

	public class LogServer
	{
		public const string Protocol = "http://";
		private const string ServiceURL = "/node/log";
		private string fServiceHost;
		private string url_api;
		public string App_type;
		public string App_Version;
		public LogServer_response jsonResponse; //Ответ сервера, краткий
		public string TODO_TEst_URL;
		public int Timeout;
		public int ElapsedTime;
		public System.Diagnostics.Stopwatch watch;
		public event EventHandler QueryStart; // Событие без данных, просто EventHandler
		System.ComponentModel.BackgroundWorker BackgroundThread;
		
		public LogServer(string ServiceHost, LogServer_response LogData)
		{
			this.fServiceHost = ServiceHost;
			this.url_api = Protocol + fServiceHost + ServiceURL;
			this.App_type = LogData.ApplicationType;
			this.App_Version = LogData.AppVersion;
			this.Timeout = 1500;//System.Threading.Timeout.Infinite;
			this.watch = new System.Diagnostics.Stopwatch();
			this.BackgroundThread = new System.ComponentModel.BackgroundWorker();
			BackgroundThread.DoWork += backgroundWorker_DoWork;
			BackgroundThread.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
			BackgroundThread.WorkerReportsProgress = true;
			BackgroundThread.WorkerSupportsCancellation = true;
			BackgroundThread.RunWorkerAsync();
		}


		

		// Own events:
		public event EventHandler QuerySuccefull; // Событие без данных, просто EventHandler

		protected virtual void OnQueryStart(EventArgs e)
		{
			EventHandler handler = this.QueryStart;
			if (handler != null)
			{
				handler(this, e);
			}
		}
		protected virtual void OnQuerySuccefull(EventArgs e)
		{
			EventHandler handler = this.QuerySuccefull;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		/// Work start
		private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			this.OnQueryStart(new EventArgs());
			Get_WebOnline_th(this.App_type);
		}

		private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			if (this.jsonResponse != null)
			{
				// Дадим событие, что ответ получен%
				this.OnQuerySuccefull(new EventArgs()); // что же мы в событие отправим то ,,
			}
		}

		public bool Get_WebOnline_th(string query)
		{
			if (query == null) return false;
			this.watch.Reset();
			this.watch.Start();
			try
			{
				WebRequest wrGETURL = null;
				//Запрос по кадастровому номеру, возвращает массив (сокращенные атрибуты):
				wrGETURL = WebRequest.Create(url_api + "?AppType=" + query+
											"&AppVer=" + this.App_Version+
											"&UserName=" + netFteo.NetWork.NetWrapper.UserName+
											"&Host=" + netFteo.NetWork.NetWrapper.Host);
				wrGETURL.Proxy = WebProxy.GetDefaultProxy();
				wrGETURL.Timeout = this.Timeout;
				Stream objStream;
				WebResponse wr = wrGETURL.GetResponse();
				objStream = wr.GetResponseStream();
				if (objStream != null)
				{
					StreamReader objReader = new StreamReader(objStream);
					string jsonResult = objReader.ReadToEnd();
					objReader.Close();
					//Понадобилась ссылка на System.Web.Extensions
					System.Web.Script.Serialization.JavaScriptSerializer sr = new System.Web.Script.Serialization.JavaScriptSerializer();

					jsonResponse = sr.Deserialize<LogServer_response>(jsonResult);
					if (jsonResponse != null)
							{
								this.watch.Stop();
								return true;
							}
				}
				this.watch.Stop();
				return false;
			}

			catch (IOException ex)
			{
				// MessageBox.Show(ex.ToString());
				string g= ex.Message;
				this.watch.Stop();
				return false;
			}
		}


	}
}

