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
    /// v 1.5
    /// </summary>
    public static class TreeViewFinder
    {
        //Search next node. Jump to parent if last node
        public static TreeNode SearchNextNode(TreeNode StartNode)
        {
            if (StartNode.NextNode != null)
                StartNode = StartNode.NextNode;  // далее в том же уровне, следующая нода
            else
            {
                //check if root level , last node
                if (StartNode.Level == 0) return null;
                //otherwise, go up to parent:
                if (StartNode.Parent != null)
                    return SearchNextNode(StartNode.Parent); //recursive ascent 
            }
            return StartNode;
        }


        // Full scanning of Node, next Nodes and childs:
        public static TreeNode SearchNodes(TreeNode StartNode, string SearchText)
        {
            if (SearchText == "") return null;

            while (StartNode != null)
            {
                if (StartNode.Text.ToUpper().Contains(SearchText))
                {
                    return StartNode;  // выходим по первому сопадению
                }

                // recursive to childs:
                if (StartNode.Nodes.Count != 0)
                {
                    return SearchNodes(StartNode.Nodes[0], SearchText);//Recursive Search 
                };

                StartNode = SearchNextNode(StartNode);
            };
            return null;
        }


        //Strong seek to desired node
        public static TreeNode SeekNode(TreeNode StartNode, string SearchText)
        {
            if (SearchText == "") return null;

            while (StartNode != null)
            {
                if (StartNode.Text.ToUpper().Equals(SearchText)) // strong equality needed
                {
                    return StartNode;  // выходим по первому сопадению
                }

                // recursive to childs:
                if (StartNode.Nodes.Count != 0)
                {
                    return SeekNode(StartNode.Nodes[0], SearchText);//Recursive Search 
                };

                StartNode = SearchNextNode(StartNode);
            };

            return null;
        }





    }
    
    /// <summary>
    /// Map class- viewPort for netfteo application
    /// </summary>
    public class Map :  UserControl
    {
        string _FileName;

        /// <summary>
        /// Gets or sets filename of viewed file
        /// </summary>
        public string FileName
        {
            get
            {
              return  this._FileName;
            }

            set
            {
                this._FileName = value;
            }
        }
    }
}


