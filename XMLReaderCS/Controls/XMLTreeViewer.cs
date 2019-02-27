using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using netFteo.Windows;

namespace XMLReaderCS
{

    /// <summary>
    /// Компонент XML TreeView, наследник Treeview. 
    /// Представляет дерево, способное отображать ветви xml
    /// </summary>
    /// <remarks>
    /// C# как-то сам добавил в Toolbox. ....
    /// </remarks>
    public class CXmlTreeView : TreeView
    {
        ContextMenuStrip contextMenu_XMLBoby;
        TextBox SearchTextBox;
        ToolStripMenuItem ItemSearch;
        public string Namespace;
        public string RootName;
        public CXmlTreeView()
        {
            BeforeExpand += OnItemexpanding;
            contextMenu_XMLBoby = new ContextMenuStrip();
            SearchTextBox = new TextBox();
            this.ContextMenuStrip = contextMenu_XMLBoby;
            ToolStripItem ItemCopy = contextMenu_XMLBoby.Items.Add("Копировать");
            ToolStripItem ItemCopyXPath = contextMenu_XMLBoby.Items.Add("Копировать XPath");
            ItemSearch = (ToolStripMenuItem)contextMenu_XMLBoby.Items.Add("Поиск");
            ItemCopy.Click += ItemCopy_Click;
            ItemCopyXPath.Click += ItemCopyXpath_Click;
            ItemSearch.Click += ItemSearch_Click;
            SearchTextBox.Visible = false;// true;
            SearchTextBox.Left = 200;
            SearchTextBox.Top = 200;
            SearchTextBox.Width = 150;
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            this.Controls.Add(SearchTextBox);
        }


        private void ItemCopyXpath_Click(object sender, EventArgs e)
        {
            Control parent = ((ContextMenuStrip)(((ToolStripMenuItem)sender).Owner)).SourceControl;
            TreeView tv = (TreeView)parent;
            if (tv.SelectedNode != null)
            {
                if (tv.SelectedNode.Tag != null)
                    if
                     (tv.SelectedNode.Tag.GetType().ToString() == "System.Xml.XmlAttribute")
                    {
                        XmlElement item = (XmlElement)tv.SelectedNode.Parent.Tag;
                        Clipboard.SetText(netFteo.XML.XMLWrapper.GetXPath_UsingPreviousSiblings(item)+"/@"+
                                     ((System.Xml.XmlAttribute)tv.SelectedNode.Tag).Name);
                    }

                if (tv.SelectedNode.Tag.GetType().ToString() == "System.Xml.XmlText")
                {
                    XmlElement item = (XmlElement)tv.SelectedNode.Parent.Tag;
                    Clipboard.SetText(netFteo.XML.XMLWrapper.GetXPath_UsingPreviousSiblings(item) + "/" +
                                 ((System.Xml.XmlText)tv.SelectedNode.Tag).Name);
                }

                    if  (tv.SelectedNode.Tag.GetType().ToString() == "System.Xml.XmlElement")
                {
                        XmlElement item = (XmlElement)this.SelectedNode.Tag;
                        Clipboard.SetText(netFteo.XML.XMLWrapper.GetXPath_UsingPreviousSiblings(item));
                    }
            }
        }

        private void ItemCopy_Click(object sender, EventArgs e)
        {
            Control parent = ((ContextMenuStrip)(((ToolStripMenuItem)sender).Owner)).SourceControl;

            if (((TreeView)parent).SelectedNode != null)
            {
                Clipboard.SetText(this.SelectedNode.Text);
            }
        }

        private void ItemSearch_Click(object sender, EventArgs e)
        {
            //TODO 
            //Find:.....
            ItemSearch.Checked = Toggle_SearchTextBox(SearchTextBox);
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.TopNode == null) return;
            TextBox searchtbox = (TextBox)sender;
            if (searchtbox.Visible)
            {   // начинаем с высшей ноды:
                this.BeginUpdate();

                if (searchtbox.Text == "")
                {
                    this.SelectedNode = this.Nodes[0]; // hi root node, seek to begin
                    this.CollapseAll();
                }

                TreeNode res = TreeViewFinder.SearchNodes(this.Nodes[0], searchtbox.Text.ToUpper());

                if (res != null)
                {
                    this.SelectedNode = res;
                    //TODO:  
                    //В случае поиска до раскрытия нод, для которых еще недогружены дочерние
                    //PrepareNode(treeView1.SelectedNode, res.Tag);
                    this.SelectedNode.EnsureVisible();
                }
                SearchTextBox.Focus();
                this.EndUpdate();
            }

        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Control) && (e.KeyCode == Keys.D))
                Toggle_SearchTextBox((TextBox)sender);

        }

        private bool Toggle_SearchTextBox(TextBox sender)
        {
            if (!sender.Visible)
            {
                SearchTextBox.Visible = true;
                sender.Visible = true;
                sender.Clear();
                sender.Focus();
                return true;
            }
            else

            {
                SearchTextBox.Visible = false;
                sender.Visible = false;
                return false;
            }
        }

        public bool LoadXML(XmlDocument dom)
        {
            this.Nodes.Clear();
            string href = "";
            this.Namespace = dom.DocumentElement.NamespaceURI;
            XmlElement Root = dom.DocumentElement;
            TreeNode TreeRoot = this.Nodes.Add(RootName);
            TreeRoot.Tag = Root;


            //insert xml prolog (aka Declaration):
            if (dom.FirstChild is XmlDeclaration)
            {
                XmlDeclaration decl = (XmlDeclaration)dom.FirstChild;
                TreeNode hrefNode = TreeRoot.Nodes.Add("xml");
                hrefNode.Nodes.Add(decl.Value);
                hrefNode.Tag = decl;
            }

            //insert xslt info:
            XmlNode styleNode = dom.SelectSingleNode("//processing-instruction(\"xml-stylesheet\")");
            if (styleNode is XmlProcessingInstruction)
            {
                XmlProcessingInstruction instruction = (XmlProcessingInstruction)styleNode;
                string tst = instruction.Value;
                int i = tst.IndexOf("href=\"") + 6;
                href = tst.Substring(i, tst.IndexOf('\"', i) - i);
                TreeNode hrefNode = TreeRoot.Nodes.Add("xml-stylesheet");
                hrefNode.Nodes.Add(instruction.Value).ToolTipText = "instruction value";
                hrefNode.Nodes.Add(href).ToolTipText = "instruction href only";
                hrefNode.Tag = styleNode;
            }

            TreeNode rNode = this.InsertItem(Root, Root.Name, TreeRoot);
            populateAttributes(Root, rNode);
            rNode.Expand();

            //TotalItemsPopulating(rNode);
            return false;
        }
        
        /// <summary>
        /// Clear treeview (delete nodes)
        /// </summary>
        public void Clear()
        {
            this.Nodes.Clear();
        }

        TreeNode InsertItem(XmlNode xmlnode, string nodeName, TreeNode hParent)
        {
            TreeNode nn = hParent.Nodes.Add(nodeName);
            nn.Tag = xmlnode;
            // populateAttributes(xmlnode, nn);
            // add emptys with Tag = null for nodes only

            if ((xmlnode.HasChildNodes)
                &&
                (xmlnode.NodeType.ToString() != "Attribute"))
            {
                TreeNode hChildItem = nn.Nodes.Add("");
                hChildItem.Tag = null;  // fake node for further using - "OnItemExpanding" will be replaced with actually child
            }

            return nn;
        }

        // Заполнение всех siblings of the [in] node.
        private bool populateNode(XmlNode inXmlNode, TreeNode inTreeNode)
        {
            string nodeType;
            //string nodeName;
            //TreeNode popuToNode;
            if (inXmlNode == null) return false;
            // что в xml ноде:
            nodeType = inXmlNode.NodeType.ToString();
            if (nodeType == "Element")
            {
                TreeNode hItem = InsertItem((XmlElement)inXmlNode, inXmlNode.Name, inTreeNode);
                populateAttributes(inXmlNode, hItem);
            }
            /*
            else
            if (nodeType == "Attribute")
            {
                TreeNode hItem = InsertItem((XmlAttribute)inXmlNode, inXmlNode.Name, inTreeNode);
                populateAttributes(inXmlNode, hItem);
            }
            */
            else
                if (nodeType == "Text")
            {
                InsertItem((XmlText)inXmlNode, inXmlNode.Value, inTreeNode);
            }
            else // Иначе все другие типы:
                if (nodeType == "Comment")
            {
                TreeNode hItem = InsertItem((XmlComment)inXmlNode, "<!-- " + inXmlNode.Value + " -->", inTreeNode);
                populateAttributes(inXmlNode, hItem);
            }
            else // Иначе все другие типы:
            {
                // insertItem((XmlElement)inXmlNode, inXmlNode.Name, inTreeNode);
            }


            XmlNode nextSibling = null;
            nextSibling = inXmlNode.NextSibling;
            if (nextSibling != null)
                populateNode((XmlNode)nextSibling, inTreeNode);

            return true;
        }

        // Заполнение всех атрибутов
        private void populateAttributes(XmlNode inXmlNode, TreeNode inTreeNode)
        {

            XmlAttributeCollection namedNodeMap = inXmlNode.Attributes;
            if (namedNodeMap != null)
            {
                int listLength = namedNodeMap.Count;
                for (int i = 0; i < listLength; i++)
                {
                    InsertItem((XmlNode)namedNodeMap.Item(i), namedNodeMap.Item(i).Name + "=" + namedNodeMap.Item(i).Value, inTreeNode);
                }
            }

        }

        //Обработчик события по раскрытию ноды
        private void OnItemexpanding(object sender, TreeViewCancelEventArgs e)
        {
            // CXmlTreeView Source = (CXmlTreeView)sender;
            // Текущая нода -      e.Node.Text  ....

            TreeNode hItem = e.Node;
            TreeNode hChildItem;
            XmlNode XMLSrc = (XmlNode)e.Node.Tag;

            hChildItem = hItem.FirstNode;
            if ((hChildItem) != null)
            {
                XmlNode childNode = (XmlNode)hItem.FirstNode.Tag;
                if (childNode == null)
                {
                    XmlNode firstChild = null;
                    firstChild = XMLSrc.FirstChild;
                    if (firstChild != null)
                    {
                        hItem.Nodes.Remove(hChildItem);
                        populateNode(firstChild, hItem);
                    }
                }
            }
            else //если child пустой, проверим xml- нет ли там child
            {
                XmlNode firstChild = null;
                firstChild = XMLSrc.FirstChild;
                if (firstChild != null)
                {
                    hItem.Nodes.Remove(hItem.FirstNode);
                    populateNode(firstChild, hItem);
                }

            }
        }



    }

    /// <summary>
    /// Доработанный класс с RichTextBox
    /// </summary>
    public class CRichTextBox : RichTextBox
    {
        ContextMenuStrip contextMenu_XMLBoby;
        public CRichTextBox(string text)
        {
            contextMenu_XMLBoby = new ContextMenuStrip();
            this.ContextMenuStrip = contextMenu_XMLBoby;
            ToolStripItem XMLnodeItem = contextMenu_XMLBoby.Items.Add("Копировать сю текст");
            XMLnodeItem.Click += CopyMenuItem_Click;
            this.Text = text;
        }

        /// <summary>
        /// Обработчик контекстнго меню "Копировать ограничения "
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Text != null)
            {
                Clipboard.SetText(this.Text);
            }
        }
    }
}
