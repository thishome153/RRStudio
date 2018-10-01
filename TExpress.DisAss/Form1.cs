using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TExpress.DisAss
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Net.Pkcs11Interop.HighLevelAPI.Pkcs11 sys = new Net.Pkcs11Interop.HighLevelAPI.Pkcs11("cades");
            //Net.Pkcs11Interop.HighLevelAPI.Slot slot = new Net.Pkcs11Interop.HighLevelAPI.Slot();
          //  Net.Pkcs11Interop.HighLevelAPI.Session ss = new Net.Pkcs11Interop.HighLevelAPI.Session(

        }

        
        /// <summary>
        /// Класс для поиска по деревьям TreeView
        /// v 1.2 in debug
        /// </summary>
        /*
        public static class TreeViewFinder
        {
            //public static string results;
            public static int MatchNodeIndex;
            //private List<TreeNode> CurrentNodeMatches;// = new List<TreeNode>();
            /// <summary>
            /// Поиск по дереву по тексту Node
            /// по следам https://stackoverflow.com/questions/11530643/treeview-search
            /// </summary>
            /// <param name="srcNodes"></param>
            /// <param name="searchstring"></param>
            /// <param name="foundFirst"></param>
            public static int SearchNodes(TreeNode StartNode, string SearchText)
            {
                if (SearchText == "") return -1;
                while (StartNode != null)
                {
                    if (StartNode.Text.ToLower().Contains(SearchText.ToLower()))
                    {
                            MatchNodeIndex = StartNode.Index;

                        // child ??? - if them, get from parent:
                        if ((StartNode.Level == 1) && (StartNode.Parent != null))
                            MatchNodeIndex = StartNode.Parent.Index;
                        return MatchNodeIndex;  // выходим по первому сопадению
                    }
                    else MatchNodeIndex = -1;

                    if (StartNode.Nodes.Count != 0)
                    {
                       MatchNodeIndex= SearchNodes(StartNode.Nodes[0], SearchText);//Recursive Search 
                        if (MatchNodeIndex != -1) return MatchNodeIndex;
                    };

                    StartNode = StartNode.NextNode;
                };
                return MatchNodeIndex;
            }
        }
        */
        /*
        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox searchtbox = (TextBox)sender;
            if (searchtbox.Visible)
            {   // начинаем с высшей ноды:
                treeView1.BeginUpdate();
                   int res = netFteo.TreeViewFinder.SearchNodes(treeView1.Nodes[0], searchtbox.Text.ToUpper());
                if (res != -1)
                {
                    treeView1.CollapseAll();
                    treeView1.SelectedNode = treeView1.Nodes[res];
                    treeView1.SelectedNode.Expand();
                    treeView1.SelectedNode.EnsureVisible();
                    textBox1.Text = "  MatchNodeIndex = " + res.ToString();

                }
                else
                {
                    treeView1.SelectedNode = treeView1.Nodes[0];
                    treeView1.SelectedNode.EnsureVisible();
                    treeView1.CollapseAll();
                }

                SearchTextBox.Focus();
                treeView1.EndUpdate();

            }
        }
        */

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox searchtbox = (TextBox)sender;
            if (searchtbox.Visible)
            {   // начинаем с высшей ноды:
                treeView1.BeginUpdate();
                TreeNode res = netFteo.TreeViewFinder.SearchNodesT(treeView1.Nodes[0], searchtbox.Text.ToUpper());
                if (res != null)
                {
                    treeView1.CollapseAll();
                    treeView1.SelectedNode = res;
                    treeView1.SelectedNode.Expand();
                    treeView1.SelectedNode.EnsureVisible();
                    textBox1.Text = "  MatchNodeIndex = " + res.ToString();

                }
                else
                {
                    treeView1.SelectedNode = treeView1.Nodes[0];
                    treeView1.SelectedNode.EnsureVisible();
                    treeView1.CollapseAll();
                }

                SearchTextBox.Focus();
                treeView1.EndUpdate();

            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            int selIndex = e.Node.Index;
            int stopme = e.Node.Level;
        }
    }
}
