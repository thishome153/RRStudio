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
        public static int MatchNodeIndex;
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


        // Случай #1 (первый алгоритм )для поиска  ноды

        /// <summary>
        /// Поиск по дереву по тексту Node
        /// </summary>
        /// <param name="srcNode"></param>
        /// <param name="searchstring"></param>
        /// <param name="foundFirst"></param>
        private void FindNode(TreeNode srcNode, string searchstring, bool foundFirst)
        {
            if (searchstring == "") return;
            Boolean selectedfound = foundFirst;
            foreach (TreeNode tn in srcNode.Nodes)
            {
                if (tn.Text.ToUpper().Contains(searchstring) && !selectedfound)
                {
                    treeView1.SelectedNode = tn;
                    treeView1.SelectedNode.EnsureVisible();
                    selectedfound = true;
                    treeView1.Focus();
                    treeView1.Select();
                    return;
                }
                //in childs:
                FindNode(tn, searchstring, selectedfound);
            }
        }
        
    
     
        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox searchtbox = (TextBox)sender;
            if (searchtbox.Visible)
            {   // начинаем с высшей ноды:
                treeView1.BeginUpdate();

                if (searchtbox.Text == "")
                {
                    treeView1.SelectedNode = treeView1.Nodes[0]; // hi root node, seek to begin
                    treeView1.CollapseAll();
                }
                //FindNode не ходит далее одного root элемента:
                //FindNode(treeView1.Nodes[0], searchtbox.Text.ToUpper(), false);
                
                TreeNode res = netFteo.TreeViewFinder.SearchNodes(treeView1.Nodes[0], searchtbox.Text.ToUpper());
                  
                if (res != null)
                {
                    treeView1.SelectedNode = res;
                    treeView1.SelectedNode.EnsureVisible();
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


        private void button2_Click(object sender, EventArgs e)
        {
            if ((treeView1.SelectedNode != null) && (treeView1.SelectedNode.NextNode != null))
            {
                TreeNode res = netFteo.TreeViewFinder.SeekNode(netFteo.TreeViewFinder.SearchNextNode(treeView1.SelectedNode), SearchTextBox.Text.ToUpper());
                if (res != null)
                {
                    treeView1.SelectedNode = res;
                    treeView1.SelectedNode.EnsureVisible();
                }
            }
        }
    }
}
