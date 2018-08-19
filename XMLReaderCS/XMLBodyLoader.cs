using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace XMLReaderCS
{
    // Класс, наследник Treeview. C# как-то сам добавил в Toolbox.
    public class CXmlTreeView : TreeView
    {
        ContextMenuStrip contextMenu_XMLBoby;
        public string Namespace;
        public string RootName;
        public CXmlTreeView()
        {
            BeforeExpand += OnItemexpanding;

            contextMenu_XMLBoby = new ContextMenuStrip();
            this.ContextMenuStrip = contextMenu_XMLBoby;
            ToolStripItem XMLnodeItem = contextMenu_XMLBoby.Items.Add("Копировать");
            XMLnodeItem.Click += BodyMenu_Click;

        }


        private void BodyMenu_Click(object sender, EventArgs e)
        {
            if (this.SelectedNode != null)
            {
                Clipboard.SetText(this.SelectedNode.Text);
            }
        }

        public bool loadXML(XmlDocument dom)
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

            TreeNode rNode = this.insertItem(Root, Root.Name, TreeRoot);
            populateAttributes(Root, rNode);
            rNode.Expand();
            return false;
        }

        TreeNode insertItem(XmlNode node, string nodeName, TreeNode hParent)
        {
            TreeNode nn = hParent.Nodes.Add(nodeName);
            nn.Tag = node;
            if (node.HasChildNodes)
            {
                TreeNode hChildItem = nn.Nodes.Add("");
                hChildItem.Tag = null;

            }
            return nn;
        }

        // Заполнение всех siblings of the [in] node.
        private bool populateNode(XmlNode inXmlNode, TreeNode inTreeNode)
        {
            string nodeType;
            string nodeName;
            TreeNode popuToNode;
            // что в xml ноде:
            nodeType = inXmlNode.NodeType.ToString();
            if (nodeType == "Element")
            {
                TreeNode hItem = insertItem((XmlElement)inXmlNode, inXmlNode.Name, inTreeNode);
                populateAttributes(inXmlNode, hItem);
            }
            else
                if (nodeType == "Text")
            {
                insertItem((XmlText)inXmlNode, inXmlNode.Value, inTreeNode);
            }
            else // Иначе все другие типы:
                if (nodeType == "Comment")
            {
                TreeNode hItem = insertItem((XmlComment)inXmlNode,"<!-- " + inXmlNode.Value+ " -->", inTreeNode);
                populateAttributes(inXmlNode, hItem);
            }
            else // Иначе все другие типы:
            {
                insertItem((XmlElement)inXmlNode, inXmlNode.Name, inTreeNode);
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
                    insertItem((XmlNode)namedNodeMap.Item(i), namedNodeMap.Item(i).Name + "=" + namedNodeMap.Item(i).Value, inTreeNode);
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
