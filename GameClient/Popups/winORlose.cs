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
        public static  int result;
        public static string winner;
        public winORlose()
        {

            InitializeComponent();
            ShowWinner();
        }
        public void ShowWinner()
        {
            if (result == 1)
            {
                label1.Text = "Congratulations you Won the Game";
                pictureBox1.Image = global::Client.Properties.Resources.win;
            }
            else if (result==0)
            {
                label1.Text = "   Game Over you Lose the Game";
                pictureBox1.Image = global::Client.Properties.Resources.lose;

            }

            else if (result == -1)
            {
                label1.Text = $"Game Over and {winner} was the Winner";
                pictureBox1.Image = global::Client.Properties.Resources.win;
                button1.Text = "Watch Next Game"; 
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            if (result == -1)
            {
                //(this.Parent as GameBoard).Close();
                GameBoard.currntGameboard.Close();
              
            }
            else
            {
                GameManger.SendServerRequest(Flag.playAgain, "0");

                DialogResult = DialogResult.Cancel;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (result == -1)
            {
                this.Close();
            }
            else
            {
                GameManger.SendServerRequest(Flag.playAgain, "1");
                DialogResult = DialogResult.OK;
            }
        }
    }
}
