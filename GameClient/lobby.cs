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
        public Form board;
        public static lobby mainlobby;
        public static GameBoard seegamebaord;
        public static waiting wait;
     


        public lobby()  
        {
            InitializeComponent();
            mainlobby = this;
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
            try
            {
                GameManger.SendServerRequest(Flag.disconnect, "");
            }
            catch (Exception)
            {


            }
        
            Application.Exit();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var cons = flowLayoutPanel2.Controls;
            var selected = from RoomControl con in cons
                           where con.BackColor == Color.Silver
                           select con.TabIndex;
           
            if (selected.Count() == 0) //no room selected
            {
                MessageBox.Show("please select a room before joining");
            }
            else
            {
                if (GameManger.Rommslist[selected.ElementAt(0)].challenger == null)//&& !GameManger.Rommslist[selected.ElementAt(0)].occupied  
                {
                    GameManger.SendServerRequest(Flag.joinRoom, GameManger.Rommslist[selected.ElementAt(0)].Name);
                    join_game = new choosecolor();
                    var dg = join_game.ShowDialog();
                    if (dg == DialogResult.OK)
                    {
                        GameManger.SendServerRequest(Flag.asktoplay, GameManger.CurrentPlayer.Name ,GameManger.CurrentPlayer.PlayerColor.ToArgb().ToString());
                         wait = new waiting();
                        wait.Show();

                    }
                    //GameManger.Rommslist[selected.ElementAt(0)].occupied = true;
                }
                else
                {
                    //GameManger.SendServerRequest(Flag.joinRoom, GameManger.Rommslist[selected.ElementAt(0)].Name);
                    //MessageBox.Show(selected.ElementAt(0).ToString() + "you are inspectator");
                    join_spectate = new spectate();
                    var dg = join_spectate.ShowDialog();
                    join_spectate.Show();
                    if (dg == DialogResult.OK)
                    {
                        GameManger.SendServerRequest(Flag.joinRoom, GameManger.Rommslist[selected.ElementAt(0)].Name);

                    }
                }



            }

            

        }


        private void Button2_Click(object sender, EventArgs e)
        {
            create_room = new createRoom();
            var dg = create_room.ShowDialog();
            
            if (dg == DialogResult.OK)
            {

                Roomname_new = create_room.Roomname_new;
                board_width = create_room.board_width;
                board_hight = create_room.board_hight;
           
                GameManger.SendServerRequest(Flag.createRoom,
                    GameManger.CurrentPlayer.Name+"+"+GameManger.CurrentPlayer.PlayerColor.ToArgb().ToString(),
                     Roomname_new, board_width.ToString() +"+"+ board_hight.ToString()
                    );
                GameBoard.rows = board_width;
                GameBoard.columns = board_hight;
                GameBoard.HostColor = create_room.Selected_color1;
                GameBoard.ChallangerColor = Color.Purple;
                GameBoard.turn = 1;
                GameBoard.playerTurn = 1;

                board = new GameBoard();
                board.Show();

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
