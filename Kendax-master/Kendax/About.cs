using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kendax
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            timer1.Start();
            

        }

      

        private void About_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer4.Stop();
            this.label3.Text = "Mika";
            timer2.Start();
            
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            this.label3.Text = "Nomakta";
            timer3.Start();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            timer3.Stop();
            this.label3.Text = "Ghbsys.net ";
            timer1.Start();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            this.label3.Text = " Arachis";  
            timer4.Start();
        }
    }
}
