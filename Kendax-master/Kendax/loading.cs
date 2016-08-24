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
    public partial class loading : Form
    {
        public loading()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
           
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.label3.Text = "Kendax // Loading [Public]";
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            this.label3.Text = "Kendax \\  Loading [Public]";
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            this.label3.Text = "Kendax = Loading [Public]";
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            this.label3.Text = "Kendax ~~> Loading [Public]";
        }

        private void timer6_Tick(object sender, EventArgs e)
        {
            Form1 OpenThis = new
            Form1();
            OpenThis.Show();
            this.Close();
        }

        private void loading_Load(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            timer3.Start();
            timer4.Start();
            timer5.Start();
            timer6.Start();
         
        }
    }
}
