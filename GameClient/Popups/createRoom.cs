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
    public partial class createRoom : Form
    {
        public string Roomname_new;
        public int board_hight;
        public int board_width;
        public Color Selected_color1;

        public createRoom()
        {
            InitializeComponent();
        }

        private void Button5_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Selected_color1 = button5.BackColor;
            }
        }

        private void Button2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Selected_color1 = button2.BackColor;
            }
        }

        private void Button3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Selected_color1 = button3.BackColor;
            }
        }

        private void Button4_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Selected_color1 = button4.BackColor;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text=="")
            {
                MessageBox.Show("Please Fill Information");
            }
            else
            {
                Roomname_new = textBox1.Text;
                board_hight = int.Parse(comboBox1.Text == "H" ? "6" : comboBox1.Text) ;
                board_width = int.Parse(comboBox2.Text == "W" ? "7" : comboBox2.Text);
                GameManger.UpdatePlayer(Selected_color1);
                this.DialogResult = DialogResult.OK;

            }
           
    
        }


        private void Button6_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        //drag the borderless form 
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Label4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }

        }
    }
}
