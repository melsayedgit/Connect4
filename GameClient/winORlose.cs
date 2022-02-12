using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
   

    public partial class winORlose : Form
    {
        public int result =0;
        public winORlose()
        {
            InitializeComponent();
            ShowWinner();
        }
        public void ShowWinner()
        {
            if (result==1)
            {
                label1.Text = "Congratulations you Won the Game";
                pictureBox1.Image = global::Client.Properties.Resources.win;
            }
            else if (result==0)
            {
                label1.Text = "   Game Over you Lose the Game";
                pictureBox1.Image = global::Client.Properties.Resources.lose;

            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
