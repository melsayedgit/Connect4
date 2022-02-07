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
    public partial class lobby : Form
    {
        public choosecolor join_game;
        public spectate join_spectate;
        public createRoom create_room;
        public RoomControl prev_select;
        //create room data 
        public string Roomname_new;
        public int board_hight;
        public int board_width;
        public lobby() 
        {
            InitializeComponent();
           
        }
        ~lobby()
        {
           
            (this.Parent as login).Close();
        }
        private void Lobby_Load(object sender, EventArgs e)
        {
            showplayer();
            showroom();
        }

        public void showplayer()
        {
            flowLayoutPanel1.Controls.Clear();
            player[] playerlist = new player[GameManger.playerslist.Count];
            for (int i = 0; i < playerlist.Length; i++)
            {
                playerlist[i] = new player();
                playerlist[i].playername = GameManger.playerslist[i].Name;
                flowLayoutPanel1.Controls.Add(playerlist[i]);
         
                //if (flowLayoutPanel1.Controls.Count>0)
                //{
                //flowLayoutPanel1.Controls.Clear();
                //}
                //else

            }
        }


        public void showroom()
        {
            flowLayoutPanel2.Controls.Clear();

            RoomControl[] roomlist = new RoomControl[GameManger.Rommslist.Count];
            for (int i = 0; i < roomlist.Length; i++)
            {
                roomlist[i] = new RoomControl();
                roomlist[i].roomname = GameManger.Rommslist[i].Name;
                flowLayoutPanel2.Controls.Add(roomlist[i]);


                //if (flowLayoutPanel1.Controls.Count>0)
                //{
                //flowLayoutPanel1.Controls.Clear();
                //}
                //else

            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {

        }

        private void Button3_MouseEnter(object sender, EventArgs e)
        {
            button3.BackColor = Color.DarkRed;
        }

        private void Button3_MouseLeave(object sender, EventArgs e)
        {
            button3.BackColor = Color.FromArgb(217,83,79);
        }

        private void Button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.Green;
        }

        private void Button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(51, 178, 73);
        }

        private void Button2_MouseEnter(object sender, EventArgs e)
        {
            button2.BackColor = Color.Blue;

        }
        private void Button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackColor = Color.FromArgb(87, 131, 219);

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            showplayer();
            showroom();
        }

        private void Lobby_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            if (true) //no room selected
            {
                join_spectate = new spectate();
                join_spectate.Show();
            }
            else
            {
                join_game= new choosecolor();
                join_game.Show();

            }

            

        }


        private void Button2_Click(object sender, EventArgs e)
        {
            create_room = new createRoom();
            var dg = create_room.ShowDialog();
            
            if (dg ==DialogResult.OK)
            {

                Roomname_new = create_room.Roomname_new;
                board_width = create_room.board_width;
                board_hight = create_room.board_hight;
           
                GameManger.SendServerRequest(Flag.createRoom,
                    GameManger.CurrentPlayer.Name+GameManger.CurrentPlayer.PlayerColor.ToString(),
                     Roomname_new, board_width.ToString() + board_hight.ToString()
                    );

            }
            

        }





        //drag the borderless form 
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Lobby_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }



        // end code of dragable form
    }
}
