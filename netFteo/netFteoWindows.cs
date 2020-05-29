using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Text;




namespace netFteo
{
    /// <summary>
    /// Small object for pointing in treevew to objects
    /// </summary>
    public class TreeNodeTag
    {
        public long Item_id;
        public string Type;
        public string Name;
        public string NameExt;
        /// <summary>
        /// Create small Tag object for treenodes ops
        /// </summary>
        /// <param name="item_id"></param>
        /// <param name="item_type"></param>
        public TreeNodeTag(long item_id, string item_type)
        {
            this.Item_id = item_id;
            this.Type = item_type;
        }
    }
}


/// <summary>
/// Not An MS Windows, everyvere window, controls, forms etc.
/// </summary>
namespace netFteo.Windows
{
    /// <summary>
    /// Модернизированный класс для Controls в WPF
    /// Умееет грамотно закрываться
    /// </summary>
    public class MyWindowEx : Window
    {

        public MyWindowEx()
        {

            Closing += new System.ComponentModel.CancelEventHandler(Window_Closing);

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

    public class TreeViewSearchable : TreeView
    {
        private System.Windows.Forms.TextBox SearchTextBox;
        public string results;
        public int MatchNodeIndex;

        public TreeViewSearchable()
        {
            this.SearchTextBox = new TextBox();
            this.Controls.Add(this.SearchTextBox);
            this.SearchTextBox.Top = this.Top - 25;
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
                    this.SelectedNode = this.Nodes[StartNode.Index];
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

   //Модифицированный класс компонента для работы в потоках
    public class TMyLabel : Label
    {
        public TMyLabel()
        {
            this.BackColor = System.Drawing.Color.LightCyan;
            this.BorderStyle = BorderStyle.FixedSingle;
            //this.Height = 15;
            this.AutoSize = true;
        }

        delegate void SetProgressCallbackText(string value);

        public void SetTextInThread(string value)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.

            // InvokeRequired требует сравнения id вызывающего потока
            //с id данного. 
            if (this.InvokeRequired) // Они отличаются, это разные потоки - напрямую нельзя,
                                     // и требуется Invoke
            {
                SetProgressCallbackText d = new SetProgressCallbackText(SetTextInThread);

                this.Invoke(d, new object[] { value });
            }
            else  // это наш поток, и можно к объекту обратиться напрямую:
            {
                this.Text = value;
            }
        }

    }

    public class TmyTextBlock : System.Windows.Controls.TextBlock
    {
        public double BaseX;
        public double BaseY;
        public TmyTextBlock(double x, double y, string TextContent)
        {
            this.BaseX = x;
            this.BaseY = y;
            this.Text = TextContent;
            this.ToolTip = this.Text + " (" + BaseX.ToString() + ", " + BaseY.ToString() + ")";
            this.Cursor = System.Windows.Input.Cursors.Hand;

        }
    }
    public static class InputBox
    {
        public static DialogResult doInputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new System.Drawing.Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new System.Drawing.Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }



}


