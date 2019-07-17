using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel; // BackgroundWorker
using System.Windows.Forms;
using System.Drawing;
using netFteo.Windows;
namespace RRTypes

{
    namespace pkk5
    {
     

 
        /// <summary>
        /// Viewer картинок , присылаемых pkk5.rosreestr.ru
        /// </summary>
        public class pkk5Viewer : System.Windows.Forms.PictureBox
        {
            ContextMenuStrip contextMenu_pkk5;
            TMyLabel label_MapScale;
            TMyLabel label_CI;
            TMyLabel label_CI_date;
            ProgressBar progressBar1;
            ComboBox ComboBox_Dpi;
            ComboBox ComboBox_SizeMode;
			RadioButton ModeSelector;
            BackgroundWorker backgroundWorker1;
            BackgroundWorker backgroundWorkerCounter;
            public pkk5_Rosreestr_ru Server;
            public bool NeedRecall;

			private ServiceMode fmode;

			/// <summary>
			/// Mode fir/pkk5
			/// </summary>
			[Category("Rosreestr")]
			[Browsable(true)]
			public ServiceMode Mode
			{
				get { return this.fmode; }
				set
				{
					this.fmode = value;
					this.NeedRecall = true; //сбрасываем флаг
				}
			}

			private string fQueryValue;
			/// <summary>
			/// Значение , КН например
			/// </summary>
			[Category("Rosreestr")]
			[Browsable(true)]/// 
			public string QueryValue
            {
                get { return this.fQueryValue; }
                set { this.fQueryValue = value;
                      this.NeedRecall = true; //сбрасываем флаг
                   }
            }

            public pkk5_json_Fattrs Result_Full;

            private pkk5_Types fQueryObjectType;
			/// <summary>
			/// Type of Parcel, OKS, Block..etcS
			/// </summary>
			[Category("Rosreestr")]
			[Browsable(true)]
			public pkk5_Types QueryObjectType
        {
            get { return this.fQueryObjectType;}
            set { this.fQueryObjectType = value; }
        }
            // Own events:
            /// <summary>
            /// Возникает при успешном вызове сервиса pkk5
            /// </summary>
            public event EventHandler QuerySuccefull; // Событие без данных, просто EventHandler
            public event EventHandler QueryStart; // Событие без данных, просто EventHandler
            protected virtual void OnQueryStart(EventArgs e)
            {
                EventHandler handler = this.QueryStart;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            protected virtual void OnQuerySuccefull(EventArgs e)
            {
                EventHandler handler = this.QuerySuccefull;
                if (handler != null)
                {
                    handler(this, e);
                }
            }


            public pkk5Viewer()
            {
                this.DoubleBuffered = true;
                this.SizeMode = PictureBoxSizeMode.Normal;//default;
                this.QueryObjectType = pkk5_Types.Block;//default
                this.NeedRecall = true; // надо перевызыватьи
                this.Server = new pkk5_Rosreestr_ru(800, 420);//default //this.Width, this.Height);
                this.backgroundWorker1 = new BackgroundWorker();
                this.backgroundWorkerCounter = new BackgroundWorker();
                this.MouseEnter += pictureBox_MouseEnter;
                this.MouseWheel += pictureBox_OnMouseWheel;
                this.SizeChanged += pictureBox_SizeChanged;
                //this.Paint += pictureBox_Paint;
                this.MouseHover += pictureBox_MouseHover;
                //StartBt = new Button();
                //StartBt.Location = new System.Drawing.Point(5,330 );
                //StartBt.Text = "Start pkk5";
                //StartBt.Click += Button_Start_Click;

                ComboBox_Dpi = new ComboBox();
                ComboBox_Dpi.Location = new System.Drawing.Point(5, 240);
                ComboBox_Dpi.Items.AddRange(new string[] { "96", "150", "200", "300", "400", "600" });
                ComboBox_Dpi.SelectedIndex = 0;
                //ComboBox_Dpi.Enabled = false;
                ComboBox_Dpi.Width = 50;
                ComboBox_Dpi.SelectionChangeCommitted += ComboBox_DpiChange;

                ComboBox_SizeMode = new ComboBox();
                ComboBox_SizeMode.Location = new System.Drawing.Point(5, 270);
                ComboBox_SizeMode.DataSource = Enum.GetValues(typeof(PictureBoxSizeMode));
                ComboBox_SizeMode.Width = 70;
                ComboBox_SizeMode.SelectionChangeCommitted += ComboBox_SizeModeChange;

				ModeSelector = new RadioButton();
				ModeSelector.Location = new System.Drawing.Point(20, 20);
				ModeSelector.Text = this.Mode.ToString();
				ModeSelector.Select();

                backgroundWorker1.DoWork += backgroundWorker_DoWork;
                backgroundWorker1.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.WorkerSupportsCancellation = true;
                backgroundWorkerCounter.DoWork += backgroundWorkerCounter_DoWork;
                backgroundWorkerCounter.ProgressChanged += backgroundWorkerCounter_ProgressChanged;
                backgroundWorkerCounter.WorkerSupportsCancellation = true;
                backgroundWorkerCounter.WorkerReportsProgress = true;

                this.progressBar1 = new ProgressBar();
                this.progressBar1.Location = new System.Drawing.Point(5, 4);
                this.progressBar1.Name = "progressBar1";
                this.progressBar1.Size = new System.Drawing.Size(300, 5);
                this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
                this.progressBar1.TabIndex = 9;
                this.progressBar1.Value = 0;
                this.progressBar1.Maximum = this.Server.Timeout/1000; // 5sec = 5000 msec

                

                label_MapScale = new TMyLabel();
                this.label_MapScale.Location = new System.Drawing.Point(5, 10);
                //label_MapScale.Resize += label_MapScale_Resize;

                //label_CN = new TMyLabel();
                //this.label_CN.Location = new System.Drawing.Point(3, 35);

                label_CI = new TMyLabel();
                this.label_CI.Location = new System.Drawing.Point(5, 60);
                label_CI_date = new TMyLabel();
                this.label_CI_date.Location = new System.Drawing.Point(5, 85);
                this.Controls.Add(label_MapScale);
                //this.Controls.Add(StartBt);
                this.Controls.Add(label_CI);
                this.Controls.Add(label_CI_date);
                this.Controls.Add(ComboBox_Dpi);
                this.Controls.Add(ComboBox_SizeMode);
                this.Controls.Add(progressBar1);
                contextMenu_pkk5 = new ContextMenuStrip();
                this.ContextMenuStrip = contextMenu_pkk5;
                ToolStripItem pkjpegItem = contextMenu_pkk5.Items.Add("Сохранить снимок как....");
                pkjpegItem.Click += ScreenShoot_Click;
                this.Result_Full = new pkk5_json_Fattrs();
            }

            // В тени  -загрузка он-лайна ПКК
            public void Start(string cn, pkk5_Types queryobjecttype)
            {
                this.QueryValue = cn;
                this.QueryObjectType = queryobjecttype;
                this.NeedRecall = true; //сбрасываем флаг
                if (this.QueryValue != null)
                   if (!this.backgroundWorker1.IsBusy)
                      backgroundWorker1.RunWorkerAsync();
            }

            private void Button_Start_Click(object sender, EventArgs e)
            {
                Start(this.QueryValue, this.QueryObjectType);
            }

            ///Make sure that the PicBox have the focus, otherwise it doesn´t receive 
            /// mousewheel events !.
            private void pictureBox_MouseEnter(object sender, EventArgs e)
            {
                if (this.Focused == false)
                {
                    this.Focus();
                }
            }

            //Обработчика крутилки мыши пришлось вручную  добавлять в wzParcel.Designer.cs
            // в раздел про обрабочики: 
            //      this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_OnMouseWheel);
            private void pictureBox_OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
            {
               if (!backgroundWorker1.IsBusy)
                {
                    if (e.Delta > 0)
                    {
                        if (this.Server.mapScale > 0)
                            this.Server.mapScale -= 250;
                    }
                    else
                        if (this.Server.mapScale < 10000)
                            this.Server.mapScale += 250;
                    this.NeedRecall = true; //сбрасываем флаг
                    label_MapScale.SetTextInThread("    M*1:" + this.Server.mapScale.ToString() + "         * ");
                }
               // if (!backgroundWorkerCounter.IsBusy)
                 //  backgroundWorkerCounter.CancelAsync();

            }

            private void pictureBox_MouseHover(object sender, EventArgs e)
            {
                if (!backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
            }

            private void ComboBox_SizeModeChange(object sender, EventArgs e)
            {
                this.SizeMode =(PictureBoxSizeMode) ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex];
                this.NeedRecall = true; //сбрасываем флаг
                if (!backgroundWorker1.IsBusy)
                {
                       backgroundWorker1.RunWorkerAsync();
                }
            }

            private void ComboBox_DpiChange(object sender, EventArgs e)
            {
                this.Server.dpi = (string)((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex];
                // this.Server.picture_Height = this.Height;
                // this.Server.picture_Width = this.Width;
                this.NeedRecall = true; //сбрасываем флаг
                if (!backgroundWorker1.IsBusy)
                    backgroundWorker1.RunWorkerAsync();
            }
         
            private void pictureBox_SizeChanged(object sender, EventArgs e)
            {
                this.NeedRecall = true; //сбрасываем флаг
                if (!backgroundWorker1.IsBusy)
                {
                    //this.Server.picture_Height = ((System.Windows.Forms.PictureBox)sender).Height;
                    //this.Server.picture_Width = ((System.Windows.Forms.PictureBox)sender).Width;
                    //label_MapScale.Text = "    M 1:" + this.Server.mapScale.ToString() + "   <> .... ";
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            
            private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
            {
               if (this.NeedRecall)
                {
                    this.OnQueryStart(new EventArgs());
                    label_MapScale.SetTextInThread("    M 1:" + this.Server.mapScale.ToString() + "        Work started: ");
                    if (!backgroundWorkerCounter.IsBusy) { backgroundWorkerCounter.CancelAsync(); backgroundWorkerCounter.RunWorkerAsync(); }
                    this.Server.Get_WebOnline_th(this.QueryValue, this.QueryObjectType); 
                }
            }

            private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                label_MapScale.SetTextInThread("    M 1:" + this.Server.mapScale.ToString() + "        Work finish: {0} sec."+Server.watch.Elapsed.Seconds.ToString());
                Font myFont = new Font("Arial", 12);
                Font font2 = new Font("Arial", 8);
                //this.Image = this.Server.Image; // так как-то напрямую
                this.Image = new Bitmap(this.Server.Image_Width, this.Server.Image_Height); // а так разрешение картинки ???
                Graphics ge = Graphics.FromImage(this.Image); // this.Image - на чем рисуем

                if (this.Server.jsonFResponse != null)
                {
                    this.Result_Full = this.Server.jsonFResponse.feature.attrs;
                    if (this.Server.jsonFResponse.feature.attrs.cad_eng_data != null)
                    {
                        label_CI.SetTextInThread(this.Server.jsonFResponse.feature.attrs.cad_eng_data.ci_surname + " " +
                                                this.Server.jsonFResponse.feature.attrs.cad_eng_data.ci_first + " " +
                                                this.Server.jsonFResponse.feature.attrs.cad_eng_data.ci_patronymic + " " +
                                                this.Server.jsonFResponse.feature.attrs.cad_eng_data.ci_n_certificate);
                        label_CI_date.SetTextInThread("Дата обновления атрибутов : " + this.Server.jsonFResponse.feature.attrs.cad_eng_data.actual_date + " " +
                                                                    "lastmodified: " + this.Server.jsonFResponse.feature.attrs.cad_eng_data.lastmodified);
                    }
                    else
                    {
                        label_CI.SetTextInThread("--");
                        label_CI_date.SetTextInThread("--");
                    }

                
                    
                if (this.Server.Image != null)
                {
                      Rectangle SourceSize = new Rectangle(3, 3, this.Server.Image_Width-3, this.Server.Image_Height-3);
                      ge.DrawImage(this.Server.Image, SourceSize);//new System.Drawing.Point(0, 0)); // копируем резyльтат pkk5
                      label_MapScale.SetTextInThread("    M 1: "+ this.Server.mapScale.ToString()+"  :) "+ Server.watch.Elapsed.Seconds.ToString()+ " sec");
                      this.NeedRecall = false; // картинка получена, больше не спрашиваем
                }
                else
                {
                    label_MapScale.SetTextInThread("  :(  ");
                    if (this.Image != null)
                    {
                   //     this.Image.Dispose();// = null;
                    //    this.Image = null;
                    }
                 }
                    // Дадим событие, что ответ получен%
                  this.OnQuerySuccefull(new EventArgs()); // что же мы в событие отправим то ,,
                }
                // А теперь тексты:
                Rectangle BourderSize = new Rectangle(2, 2, this.Server.Image_Width-2, this.Server.Image_Height-2);
                ge.DrawRectangle(new Pen(Brushes.Gray,2), BourderSize);
                ge.DrawString(this.QueryValue+  "  , S= " + this.Result_Full.area_value, myFont, Brushes.Green, new Point(2, this.Image.Height - 80));
                if (this.Result_Full.address != null)
                    ge.DrawString(this.Result_Full.address, myFont, Brushes.Green, new Point(2, this.Image.Height - 60));
                if (this.Result_Full.util_by_doc != null)
                    ge.DrawString(this.Result_Full.util_by_doc, myFont, Brushes.Green, new Point(2, this.Image.Height - 40));
                ge.DrawString(this.Server.dpi + " dpi M 1:" + this.Server.mapScale.ToString(), myFont, Brushes.Green, new Point(2, this.Image.Height - 20));
                ge.DrawString("2019@Fixosoft pkk5 Viewer " + this.ProductVersion, font2, Brushes.Black, this.Image.Width - 200, 2);
                ge.DrawString(pkk5_Rosreestr_ru.url_api, font2, Brushes.Black, this.Image.Width - 450, 2);
                ge.DrawString(DateTime.Now.ToString(), font2, Brushes.Black, this.Image.Width - 120, 18); // this.Image.Width не равен this.Width
				ge.DrawString(this.Server.TODO_TEst_URL, font2, Brushes.Black, this.Image.Width - 120, 101);


				/*
                if (this.Server.jsonResponse != null)
                    if (this.Server.jsonResponse.features.Count > 0) // на количество тоже надо проверять - бывают "пустые ответы", но со статусом 200 , т.е. ОК
                {
                    this.Result_Address = this.Server.jsonResponse.features[0].attrs.address;
                }
               */
				if (backgroundWorkerCounter.IsBusy)
                {
                  //  backgroundWorkerCounter.CancelAsync();
                    //backgroundWorkerCounter.ReportProgress();
                }
                
            }

            delegate void SetProgressCallback(int progress);
            private void backgroundWorkerCounter_ProgressChanged(object sender, ProgressChangedEventArgs e)
            {
                this.SetProgress(e.ProgressPercentage);
            }

            private void SetProgress(int value)
            {
                // InvokeRequired required compares the thread ID of the
                // calling thread to the thread ID of the creating thread.
                // If these threads are different, it returns true.
                if (this.progressBar1.InvokeRequired)
                {
                    SetProgressCallback d = new SetProgressCallback(SetProgress);

                    this.Invoke(d, new object[] { value });
                }
                else
                {
                    this.progressBar1.Value = value;
                }
            }

            private void backgroundWorkerCounter_DoWork(object sender, DoWorkEventArgs e)
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                worker.ReportProgress(0);
                for (int i = 1; i <= this.Server.Timeout/1000; i++)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        // Perform a time consuming operation and report progress.
                        System.Threading.Thread.Sleep(1000);
                        worker.ReportProgress(i);
                    }
                }
            }

            private void backgroundWorkerCounter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
               // BackgroundWorker worker = sender as BackgroundWorker;
              //  worker.ReportProgress(0);
            }
            
          

            private void ScreenShoot_Click(object sender, EventArgs e)
            {
                if (this.Image != null)
                {
                    
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    //System.Drawing.Bitmap sv = new System.Drawing.Bitmap(this.Image);
                    saveFileDialog1.DefaultExt = "*.jpeg";
                    saveFileDialog1.FileName = "pkk5-map-" + netFteo.StringUtils.ReplaceSlash(this.QueryValue) + ".jpeg";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                       this.Image.Save(saveFileDialog1.FileName);
                    }
                }

            }

        }
    }
}
