﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;



namespace netFteo
{

	public static class StringUtils
	{
		public static bool isCadastralNumber(string QueryString, char Delimiter = ':')
		{
			//Format ??
			string[] SplittedString = QueryString.Split(Delimiter);
			if (SplittedString.Length > 3)
				return true;
			else
				return false;
		}




		// -----Функция замены ',' на '.' в случае работы с разделитем Windows Locale ID <> .
		public static string ReplaceComma(string Texts)
		{


			string Result = Texts;
			/* 
			FindPosition = pos(',',Result);
		   if  (FindPosition != 0) 
			 Result[FindPosition] = '.'; // заменияем там, где нашли запятую
			*/
			return Result;
        }

		/// <summary>
		/// Convert string to double with checking dot and comma
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
        public static double TryDouble(string src)
        {
			if ((src.Length > 0) && (!src.Contains("-")))
			{
				string Convsrc = src.Replace(',','.');
				try
				{
					double res = Convert.ToDouble(Convsrc);
					return res;
				}
				catch (Exception ex)
                {
					string msg = ex.Message;
					throw new Exception("Exception ToDouble convert:" + msg);
				}
			}
			else throw new FormatException("Invalid input string while ToDouble convert");
        }

		public static void RemoveParentCN(string ParentCN, Spatial.TEntitySpatial Target)
		{
			if (ParentCN == null) return;
			foreach (Spatial.IGeometry poly in Target)
			{
				if (poly.Definition != null &&
					poly.Definition.Contains(ParentCN))
					if (poly.Definition.Substring(0, ParentCN.Length) == ParentCN)
						poly.Definition = poly.Definition.Substring(ParentCN.Length);
			}
		}

		//-----------------------------------------------------------------------------
		public static string ReplaceSlash(string LayerName)
		{
			if (LayerName == null) return null;
			char[] chars = LayerName.ToCharArray();
			for (int scan = 0; scan <= LayerName.Length - 1; scan++)
			{

				if (LayerName[scan].ToString() == "/" |
				LayerName[scan].ToString() == "\n\\n" |
				LayerName[scan].ToString() == "(" |
				LayerName[scan].ToString() == ")" |
				LayerName[scan].ToString() == "$" |
				LayerName[scan].ToString() == "%" |
				LayerName[scan].ToString() == "^" |
				LayerName[scan].ToString() == "*" |
				LayerName[scan].ToString() == "@" |
				LayerName[scan].ToString() == "#" |
				LayerName[scan].ToString() == " " |
				LayerName[scan].ToString() == "-" |
				LayerName[scan].ToString() == ":")
					chars[scan] = '_';

			}

			return new string(chars);
		}

	}
    public static class ObjectLister
    {
		public static void ListZone(TreeNode Node, Cadaster.TZone Zone)
		{
			string CN = Zone.Description;
			TreeNode PNode = Node.Nodes.Add("ZNode" + Zone.id, Zone.AccountNumber);
			if (Zone.TypeName == "Территориальная зона")
			{
				PNode.ImageIndex = 1;
				PNode.SelectedImageIndex = 1;
			}
			else
			{
				Node.ImageIndex = 6;
				PNode.SelectedImageIndex = 6;
			}

			//  
			if (Zone.PermittedUses != null)
			{
				TreeNode PuNode = PNode.Nodes.Add("ZonePuNode", "ВРИ");
				foreach (string item in Zone.PermittedUses)
				{
					PuNode.Nodes.Add(item);
				}
				//PNode.ImageIndex = 7;
				//PNode.SelectedImageIndex = 7;
			}


			if (Zone.Documents.Count > 0)
			{
				TreeNode PuNode = PNode.Nodes.Add("ZonePuNode", "Документы-основания");
				foreach (Spatial.TDocument doc in Zone.Documents)
				{

					if (doc.Name != null)
					{
						TreeNode PutNode = PuNode.Nodes.Add(doc.Name);

						if (doc.Number != null) PutNode.Nodes.Add(doc.Number);
						if (doc.Doc_Date != null) PutNode.Nodes.Add(doc.Doc_Date);
						if (doc.IssueOrgan != null) PutNode.Nodes.Add(doc.IssueOrgan);
						if (doc.Serial != null) PutNode.Nodes.Add(doc.Serial);
					}
				}
			}
			//list both spatial fileds. For Further deprecations
				ListEntSpat(PNode, Zone.SpatialElement, "SPElem.", "Границы", 0);
				ListES(PNode, Zone.EntitySpatial);
		}

        public static void ListEntSpat(TreeNode NodeTo, Spatial.TPolygon ES, string NodeName, string Definition, int Status)
        {
            if (ES == null) return;
            TreeNode Node = /* NodeTo; */ NodeTo.Nodes.Add(NodeName + ES.id.ToString(), Definition); 
            //Node.Name = NodeName + ES.id.ToString();
            Node.ToolTipText = Spatial.TMyState.StateToString(Status) + ES.HasChanges;
            Node.ForeColor = Spatial.TMyColors.StatusToColor(Status);// Rosreestr.System.Drawing.Color.DarkSeaGreen;
                                                                     //redifine status color, if happend changes:
            if (ES.HasChangesBool)
                Node.ForeColor = Spatial.TMyColors.StatusToColor(0);
            Node.ImageIndex = 3;
            Node.SelectedImageIndex = 3;
            Node.Tag = ES.id;//
        }

		public static void ListEntSpat(TreeNode NodeTo, Spatial.TPolyLines ES, string NodeName, string Definition, int Status)
		{
			if (ES == null) return;
			TreeNode Node = NodeTo.Nodes.Add("TPLines." + ES.ParentID.ToString(), Definition);
			Node.ToolTipText = Spatial.TMyState.StateToString(Status);
			Node.ForeColor = Spatial.TMyColors.StatusToColor(Status);// Rosreestr.System.Drawing.Color.DarkSeaGreen;

			for (int i = 0; i <= ES.Count - 1; i++)
			{
				ListEntSpat(Node, ES[i], "", "Отрезок " + (i + 1).ToString(), Status);
			}

		}

		public static void ListEntSpat(TreeNode NodeTo, Spatial.TPolyLine ES, string NodeName, string Definition, int Status)
		{
			if (ES == null) return;
			TreeNode Node = NodeTo.Nodes.Add("TPolyLine." + ES.id.ToString(), Definition);
			Node.ToolTipText = Spatial.TMyState.StateToString(Status);
			Node.ForeColor = Spatial.TMyColors.StatusToColor(Status);// Rosreestr.System.Drawing.Color.DarkSeaGreen;
			Node.ImageIndex = 13;
			Node.SelectedImageIndex = 13;
			Node.Tag = ES.id;
		}

        public static void ListEntSpat(TreeNode NodeTo, Spatial.PointList ES, string NodeName, string Definition, int Status)
        {
            if (ES == null) return;
            TreeNode Node = NodeTo.Nodes.Add("PointList." + ES.id.ToString(), Definition);
            Node.ToolTipText = Spatial.TMyState.StateToString(Status);
            Node.ForeColor = Spatial.TMyColors.StatusToColor(Status);// Rosreestr.System.Drawing.Color.DarkSeaGreen;
            Node.ImageIndex = 13;
            Node.SelectedImageIndex = 13;
            Node.Tag = ES.id;
        }

        public static void ListEntSpat(TreeNode NodeTo, Spatial.TCircle ES, string Definition, int Status)
		{
			if (ES == null) return;
			TreeNode Node = NodeTo.Nodes.Add("Circle." + ES.id.ToString(), Definition);
			Node.ToolTipText = Spatial.TMyState.StateToString(Status);
			Node.ForeColor = Spatial.TMyColors.StatusToColor(Status);// Rosreestr.System.Drawing.Color.DarkSeaGreen;
			Node.ImageIndex = 11;
			Node.SelectedImageIndex = 11;
			Node.Tag = ES.id;
		}

		public static void ListEntSpat(TreeNode NodeTo, Spatial.TPoint ES, string Definition, int Status)
		{
			if (ES == null) return;
			TreeNode Node = NodeTo.Nodes.Add("TPoint." + ES.id.ToString(), Definition);
			Node.ToolTipText = Spatial.TMyState.StateToString(Status);
			Node.ForeColor = Spatial.TMyColors.StatusToColor(Status);// Rosreestr.System.Drawing.Color.DarkSeaGreen;
			Node.ImageIndex = 10;
			Node.SelectedImageIndex = 10;
			Node.Tag = ES.id;
		}

		
		/// <summary>
		/// List spatial collection in given TreeNode
		/// </summary>
		/// <param name="NodeES">Target node</param>
		/// <param name="ES"></param>
		public static void  ListES(TreeNode NodeES, Spatial.TEntitySpatial ES)
		{
			if ((ES == null) || (ES.Count == 0)) return;

			TreeNode NodeTo = NodeES.Nodes.Add("ES." + ES.id.ToString(), "Слои");
			NodeTo.Tag = ES.id;
			//Show layers of ES
			if (ES.Layers.Count == 1)
			{
				NodeTo.Text = "Границы";// + ES.Count().ToString();
				if (ES.State == 200) //parsed, complete spatial
				{
					NodeTo.ImageIndex = 3;
					NodeTo.SelectedImageIndex = 3;
				}

				if (ES.State == 14) //raw spatial
				{
					NodeTo.ImageIndex = 14; //raw spatial
					NodeTo.SelectedImageIndex = 14; //raw spatial
					NodeTo.ToolTipText = "Raw spatial data, need parsed";
				}

				ListESbyLayer(ES, NodeTo, ES.Layers[0].LayerHandle);
			}

			else
				foreach (netFteo.Spatial.TLayer layer in ES.Layers)
				{
					TreeNode LayerNode = NodeTo.Nodes.Add("Layer." + layer.LayerHandle, layer.Name);
					//TODO Filter features by Inumerable selects : 
					//Spatial.TEntitySpatial LayerFeatures = ES.Select<Spatial.TEntitySpatial, Spatial.TEntitySpatial>(res = new Spatial.Geometry); //xd => xd.LayerHandle == layer.LayerHandle);
					ListESbyLayer(ES, LayerNode, layer.LayerHandle);
				}
		}
		private static void ListESbyLayer(Spatial.TEntitySpatial ES, TreeNode NodeTo, string LayerHandle)
		{
			// TreeNode Node = NodeTo;
			//if (ES.Count >1 ) { NodeTo = NodeTo.Nodes.Add(NodeTo.Text); }
			foreach (Spatial.IGeometry feature in ES)
			{
				if (feature.LayerHandle == LayerHandle)
				{
					if (feature.TypeName == "netFteo.Spatial.TPolygon")
					{
						if (((Spatial.TPolygon)feature).PointCount > 0)
							netFteo.ObjectLister.ListEntSpat(NodeTo, (Spatial.TPolygon)feature, "SPElem.", ((Spatial.TPolygon)feature).Definition, 6);
					}

					if (feature.TypeName == "netFteo.Spatial.TPolyLine")
					{
						if (((Spatial.TPolyLine)feature).PointCount > 0)
							netFteo.ObjectLister.ListEntSpat(NodeTo, (Spatial.TPolyLine)feature, "SPElem.", ((Spatial.TPolyLine)feature).Definition, 6);
					}

					if (feature.TypeName == "netFteo.Spatial.PointList")
					{
						if (((Spatial.PointList)feature).PointCount > 0)
							netFteo.ObjectLister.ListEntSpat(NodeTo, (Spatial.PointList)feature, "SPElem.", ((Spatial.PointList)feature).Definition, 6);
					}

					if (feature.TypeName == "netFteo.Spatial.TCircle")
					{
						netFteo.ObjectLister.ListEntSpat(NodeTo, (Spatial.TCircle)feature, ((Spatial.TCircle)feature).Definition, 6);
					}

					if (feature.TypeName == "netFteo.Spatial.TPoint")
					{
						netFteo.ObjectLister.ListEntSpat(NodeTo, (Spatial.TPoint)feature, ((Spatial.TPoint)feature).Definition, 6);
					}

				}
			}
		}
				
		public static ListView.ListViewItemCollection EStoListViewCollection(ListView owner, Spatial.TPolygon ES)
		{

			if (ES == null) return null;

			ListView.ListViewItemCollection res = new ListView.ListViewItemCollection(owner);
			res.Add("");
			res.Add("Отрезки границ:");
			Spatial.BrdList Borders = new Spatial.BrdList();
			Borders.AddItems("", ES);
			int OutCount = Borders.Count;
			owner.BeginUpdate();
			for (int i = 0; i <= Borders.Count - 1; i++)
			{
				ListViewItem LVb = new ListViewItem(Borders[i].PointNames);
				LVb.Tag = NetFteoTypes.Border;
				ListViewItem.ListViewSubItem subLen = LVb.SubItems.Add(Borders[i].Length.ToString("0.00"));
				subLen.Tag = NetFteoTypes.BorderLength;

				LVb.SubItems.Add(Borders[i].Definition);
				if (Borders[i].Length == 0)
				{

					LVb.UseItemStyleForSubItems = false;
					subLen.BackColor = System.Drawing.Color.LightPink;
					subLen.Font = new System.Drawing.Font(
						  "Arial", 10, System.Drawing.FontStyle.Italic);
				}
				res.Add(LVb);
			}


			for (int ic = 0; ic <= ES.Childs.Count - 1; ic++)
			{
				res.Add("");
				Borders.AddItems((ic + 1).ToString() + ".", ES.Childs[ic]);

				for (int i = OutCount; i <= Borders.Count - 1; i++)
				{
					ListViewItem LVb = new ListViewItem(Borders[i].PointNames);
					LVb.SubItems.Add(Borders[i].Length.ToString("0.00"));
					LVb.SubItems.Add(Borders[i].Definition);
					res.Add(LVb);
				}
				OutCount = OutCount + ES.Childs[ic].Count - 1;
			}

			owner.EndUpdate();
			return res;
		}

		public static ListView.ListViewItemCollection EStoListViewCollection(ListView owner, Spatial.TPolyLine ES)
		{
			if (ES == null) return null;
			ListView.ListViewItemCollection res = new ListView.ListViewItemCollection(owner);
			res.Add("");
			res.Add("Отрезки границ:");
			netFteo.Spatial.BrdList Borders = new Spatial.BrdList();
			Borders.AddItems("", ES);
			int OutCount = Borders.Count;
			owner.BeginUpdate();
			for (int i = 0; i <= Borders.Count - 1; i++)
			{
				ListViewItem LVb = new ListViewItem(Borders[i].PointNames);
				LVb.SubItems.Add(Borders[i].Length.ToString("0.00"));
				LVb.SubItems.Add(Borders[i].Definition);
				res.Add(LVb);
			}


			owner.EndUpdate();
			return res;
		}
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

        public static string Win1251ToUTF8(string source)
        {
            Encoding utf8 = Encoding.GetEncoding("utf-8");
            Encoding win1251 = Encoding.GetEncoding("windows-1251");
            byte[] utf8Bytes = win1251.GetBytes(source);
            byte[] win1251Bytes = Encoding.Convert(win1251, utf8, utf8Bytes);
            source = win1251.GetString(win1251Bytes);
            return source;
        }
    }
	public static class GUID
	{
		/// <summary>
		/// Create (compile) the value of two GUIDs. 
		/// </summary>
		/// <param name="ToUpperCase">UpperCase for literals</param>
		/// <returns>GUID as string value</returns>
		public static string CompileGUID(bool ToUpperCase)
		{
			Guid g;
			// Create and display the value of two GUIDs.
			g = Guid.NewGuid();
			if (ToUpperCase)
				return g.ToString().ToUpper();
			else
				return g.ToString();
		}

		public static bool Valide(string guid)
		{
			return false;
		}

	}

}
