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
    public partial class DistrictForm : Form
    {

        DataTable data = new DataTable();
        private MySqlDataAdapter da;
        private MySqlCommandBuilder cb;
        MySqlConnection conn;
        public int subrf_id;
        public int district_id;
        public string district_Name;
        public string district_kn;

      



        public DistrictForm(MySqlConnection connCt, int sub_id)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            conn = connCt;
            subrf_id = sub_id;

        }

       

        private void DistrictForm_Shown(object sender, EventArgs e)
        {
            if (this.conn != null)
                if (this.conn.State == ConnectionState.Open)
            {
                MySqlDataReader reader;
                this.myDataGridView1.Columns.Clear();
                this.myDataGridView1.Columns.Add("district_id", "id");
                this.myDataGridView1.Columns["district_id"].Width = 40;
                this.myDataGridView1.Columns.Add("district_kn", "КН");
                this.myDataGridView1.Columns["district_kn"].Width = 50;
                this.myDataGridView1.Columns.Add("district_Name", "Наименование");
                this.myDataGridView1.Columns["district_Name"].Width = 250;

                MySqlCommand command = new MySqlCommand();
                string commandString = "SELECT * FROM districts where 	districts.subrf_id = " + subrf_id;
                command.CommandText = commandString;
                command.Connection = conn;

                try
                {
                    reader = command.ExecuteReader();
                    string test;
                    while (reader.Read())
                    {
                        
                        myDataGridView1.Rows.Add(reader["district_id"].ToString(),
                            reader["district_kn"].ToString(),
                            reader["district_Name"].ToString());
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
                this.district_id = Convert.ToInt32(myDataGridView1.SelectedRows[0].Cells[0].Value);
                this.district_kn = myDataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                this.district_Name = myDataGridView1.SelectedRows[0].Cells[2].Value.ToString();
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

        private void myDataGridView1_DoubleClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChangeGridItem();
        }

        private void MyDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    } 

}
