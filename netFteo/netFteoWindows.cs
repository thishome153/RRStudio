using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Linq;
using System.Text;


namespace netFteo
{
    /// <summary>
    /// Модернизированный класс для Controls в WPF
    /// Умееет грамотно закрываться
    /// </summary>
    public class MyWindowEx :  Window
    {

        public MyWindowEx()
        {
            
            Closing += new System.ComponentModel.CancelEventHandler( Window_Closing);
            
           // Resize += new EventHandler(Window_Resize);
            //LocationChanged += new EventHandler (Window_LocationChanged);
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
            //this.Visible = false;
        }

        private void Window_Resize(object sender, EventArgs e)
        {


            this.Title = "on resize " + e.ToString();
        }

        private void Window_LocationChanged(object sender, EventHandler e)
        {
        //    e.Cancel = true;
          //  this.Visibility = Visibility.Hidden;

        }


        
    }


        public class TreeViewSearchable: TreeView
    {
        private System.Windows.Forms.TextBox SearchTextBox;
        public string results;
        public int MatchNodeIndex;

        public TreeViewSearchable()
        {
            this.SearchTextBox = new TextBox();
            this.Controls.Add(this.SearchTextBox);
            this.SearchTextBox.Top = this.Top- 25;
            this.SearchTextBox.Left = this.Left + 25;
            this.SearchTextBox.Text = "Begin me!!";
            this.SearchTextBox.Visible = true;
            this.SearchTextBox.BackColor = System.Drawing.Color.LightGreen;
            this.BackColor = System.Drawing.Color.Lime;
            this.SearchTextBox.TextChanged += SearchTextBox_TextChanged;
        }

        public int SearchNodes(TreeNode StartNode, string SearchText)
        {
            while (StartNode != null)
            {
                if (StartNode.Text.ToLower().Contains(SearchText.ToLower()))
                {
                    results += (StartNode.Index + 1).ToString() + ". " + StartNode.Text + "\n"; //  CurrentNodeMatches.Add(StartNode);
                    MatchNodeIndex = StartNode.Index; //treeView1.SelectedNode = StartNode;
                    this.SelectedNode = this.Nodes[ StartNode.Index];
                };
                if (StartNode.Nodes.Count != 0)
                {
                    SearchNodes(StartNode.Nodes[0], SearchText);//Recursive Search 
                };
                StartNode = StartNode.NextNode;
            };
            return MatchNodeIndex;
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {


        }

    }



    /// <summary>
    /// Класс для поиска по деревьям TreeView
    /// v 1.4
    /// </summary>
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
 


        public static TreeNode SearchNodesT(TreeNode StartNode, string SearchText, bool foundFirst)
        {
            if (SearchText == "") return null;
            Boolean selectedfound = foundFirst;

            while (StartNode != null)
            {
                if (StartNode.Text.ToUpper().Contains(SearchText))//&& !selectedfound)
                {
                    /*
                    if ((StartNode.Level == 1) && (StartNode.Parent != null))
                        return  StartNode.Parent;
                    else 
                        */
                    selectedfound = true;
                    return StartNode;  // выходим по первому сопадению
                    // child ??? - if them, get from parent:
                }

                if (StartNode.Nodes.Count != 0)
                {
                    return SearchNodesT(StartNode.Nodes[0], SearchText, selectedfound);//Recursive Search 
                }

                StartNode = StartNode.NextNode;
            }
            return null;
        }
    }

}


