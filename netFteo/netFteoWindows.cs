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
    /*
    public MyWindowEx()
        {
            InitializeComponent();
            Closed += new System.EventHandler(MyWindow_Closed);
        }

        private static MyWindowEx _instance;

        public static MyWindowEx Instance
        {
            if(_instance == null )
              {
                _instance = new Window();
        }
            return _instance();
        }

    void MyWindow_Closed(object sender, System.EventArgs e)
    
    {
        _instance = null;
    }

    */


    /// <summary>
    /// Класс для поиска по деревьям TreeView
    /// </summary>
    public static class TreeViewFinder
    {
        public static string results;
        //private List<TreeNode> CurrentNodeMatches;// = new List<TreeNode>();
        /// <summary>
        /// Поиск по дереву по тексту Node
        /// </summary>
        /// <param name="srcNodes"></param>
        /// <param name="searchstring"></param>
        /// <param name="foundFirst"></param>
        public static void FindNode(TreeView TV, TreeNode srcNodes, string searchstring, bool foundFirst)
        {
            if (searchstring == "") return;
            /*
        {
            TV_Parcels.SelectedNode = TV_Parcels.TopNode;
            TV_Parcels.Focus();
            return;
        }
            */

            Boolean selectedfound = foundFirst;

            foreach (TreeNode tn in srcNodes.Nodes )//srcNodes.Nodes)
            {
                if (tn.Text.ToUpper().Contains(searchstring) && !selectedfound)
                {
                    TV.SelectedNode = tn;
                    //TV_Parcels.TopNode = tn;
                    selectedfound = true;
                    TV.Focus();
                    TV.Select();
                    return;
                }
                //in childs:
                if (tn.Nodes.Count != 0)
                FindNode(TV, tn.Nodes[0], searchstring, selectedfound);
            }
        }


        // https://stackoverflow.com/questions/11530643/treeview-search
        public static string SearchNodes(TreeNode StartNode , string SearchText )
        {
            TreeNode node = null;
            while (StartNode != null)
            {
                if (StartNode.Text.ToLower().Contains(SearchText.ToLower()))
                {
                    results += (StartNode.Index+1).ToString() + ". "+ StartNode.Text+ "\n"; //  CurrentNodeMatches.Add(StartNode);
                };
                if (StartNode.Nodes.Count != 0)
                {
                    SearchNodes(StartNode.Nodes[0], SearchText);//Recursive Search 
                };
                StartNode = StartNode.NextNode;
            };
            return results;
        }
    }
}
   

    