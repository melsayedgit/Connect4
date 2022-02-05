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
        public RoomControl prev_select;
        public lobby() 
        {
            InitializeComponent();
           
        }
        private void Lobby_Load(object sender, EventArgs e)
        {
            showplayer();
            showroom();
        }

        public void showplayer()
        {
            player[] playerlist = new player[20];
            for (int i = 0; i < playerlist.Length; i++)
            {
                playerlist[i] = new player();
                playerlist[i].playername = "PlayerName";
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
            RoomControl[] roomlist = new RoomControl[10];
            for (int i = 0; i < roomlist.Length; i++)
            {
                roomlist[i] = new RoomControl();
                roomlist[i].roomname = "Room Name"+i;
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
    }
}
