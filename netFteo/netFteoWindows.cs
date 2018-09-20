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
}
   

    