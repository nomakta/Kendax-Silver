using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Kendax
{

    public partial class Form1 : Form
    {
       
        public Form1()
        {
          
            InitializeComponent();
            Title.Start();
            Title3.Start();
            Title4.Start();
            MaximizeBox = false;

        }

        private void AvatarPctbx_Click(object sender, EventArgs e)
        {

        }

        private void StatusTxt_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.label3.Text = "Kendax \\ [Public]";
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.label3.Text = "Kendax  = [Public]";
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            this.label3.Text = "Kendax // [Public]";
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            this.label3.Text = "Kendax ~~> [Public]";
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void RoomPasswordTxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void NavigateBtn_Click(object sender, EventArgs e)
        {

        }

        private void NavigateBtn_Click_1(object sender, EventArgs e)
        {

        }

        private void RoomPasswordTxt_TextChanged_1(object sender, EventArgs e)
        {

        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                string myConnection = "datasource=sql3.freesqldatabase.com;port=3306;username=sql372869;password=dR5%iM7%";
                MySqlConnection myConn = new MySqlConnection(myConnection);

                MySqlCommand SelectCommand = new MySqlCommand("SELECT * FROM sql372869.accounts WHERE username='" + this.username_txt.Text + "' AND password='" + this.password_txt.Text + "' ;", myConn);

                MySqlDataReader myReader;
                myConn.Open();
                myReader = SelectCommand.ExecuteReader();
                int count = 0;
                int active = 0;
                while (myReader.Read())
                    count = count + 1;
                   active = active + 9;
                if (count == 1)
                {
                    Main OpenThis = new
                    Main();
                    OpenThis.Show();
                    this.Close();
                }
                       
                    else if (count > 1)

                        MessageBox.Show("Duplicate username and password. Access is denied.");
                    else
                        MessageBox.Show("Username and password is incorrect. Please try again.");
                myConn.Close();
                
               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
