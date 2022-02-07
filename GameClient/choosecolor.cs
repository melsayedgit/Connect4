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

    public partial class choosecolor : Form
    {
        public Color player_clr;
        public Color selected_color;
        public choosecolor()
        {
            InitializeComponent();
        }


        private void Button5_MouseClick(object sender, MouseEventArgs e)
        { 
            if (e.Button == MouseButtons.Left)
            {
                player_clr = button5.BackColor;

            }
        }

        private void Button2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                player_clr = button2.BackColor;

            }

        }

        private void Button3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                player_clr = button3.BackColor;

            }
        }

        private void Button4_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                player_clr = button4.BackColor;

            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            selected_color = player_clr;
            ///  code of start the gameeeeee (open game board ) player2 joined with selected color
        }



        //drag the borderless form 
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Player_color_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        // End code of dragable form

    }
}
