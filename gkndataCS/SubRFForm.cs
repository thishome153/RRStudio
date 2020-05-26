using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GKNData
{
    public partial class SubRFForm : Form
    {

        DataTable data = new DataTable();
        private MySqlDataAdapter da;
        private MySqlCommandBuilder cb;
        MySqlConnection conn;
        public Byte subrf_id;
        public string subrf_Name;
        public string subrf_kn;




        public SubRFForm(MySqlConnection connCt)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            conn = connCt;

        }

        private void SubRFForm_Shown(object sender, EventArgs e)
        {
            if (this.conn != null)
              if (this.conn.State == ConnectionState.Open)
            {
                MySqlDataReader reader;
                this.myDataGridView1.Columns.Clear();
                this.myDataGridView1.Columns.Add("subrf_id", "id");
                this.myDataGridView1.Columns["subrf_id"].Width = 30;
                this.myDataGridView1.Columns.Add("subrf_kn", "КН");
                this.myDataGridView1.Columns["subrf_kn"].Width = 50;
                this.myDataGridView1.Columns.Add("subrf_Name", "Наименование");
                this.myDataGridView1.Columns["subrf_Name"].Width = 250;

                MySqlCommand command = new MySqlCommand();
                string commandString = "select * from subrf;";
                command.CommandText = commandString;
                command.Connection = conn;

                try
                {
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        
                        myDataGridView1.Rows.Add(reader["subrf_id"].ToString(),
                                                 reader["subrf_kn"].ToString(), 
                                                 reader["subrf_Name"].ToString());
                    }
                    reader.Close();


                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error: \r\n{0}", ex.ToString());
                }
                finally
                {
                    //command.Connection.Close();
                }

            }
        }
        private void ChangeGridItem()
        {
            if (myDataGridView1.SelectedRows.Count == 1)
            {
                this.subrf_id = Convert.ToByte(myDataGridView1.SelectedRows[0].Cells[0].Value);
                this.subrf_kn = myDataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                this.subrf_Name = myDataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            }
        }

        private void myDataGridView1_Click(object sender, EventArgs e)
        {
            ChangeGridItem();
        }

        private void myDataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            ChangeGridItem();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChangeGridItem();
        }

        private void myDataGridView1_DoubleClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }
    } 
             
}
