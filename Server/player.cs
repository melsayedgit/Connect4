using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace serverAppConnect4
{
    public class player
    {
        string name;
        string color;
        bool isPlaying = false;
        bool playAgain = false;
        int score = 0;
        room myRoom = null;
        //player network and streams
        NetworkStream nstream;
        BinaryReader br;
        BinaryWriter bw;
        //player specific task to handle each player requests
        Task playerThread;

        //assign a cancellation token for the task
        public CancellationTokenSource tokenSource = new CancellationTokenSource();
        public CancellationToken ct; //this.tokenSource.Token;


        //class getters and setters
        public string Name { get { return name; } set { name = value; } }
        public string Color { get { return color; } set { color = value; } }
        public bool IsPlaying { get { return isPlaying; } set { isPlaying = value; } }
        public bool PlayAgain { get { return playAgain; } set { playAgain = value; } }
        public int Score { get { return score; } set { score = value; } }
        public room MyRoom { get { return myRoom; } set { myRoom = value; } }
        public NetworkStream Nstream { get { return nstream; } set { nstream = value; } }
        public BinaryReader Br { get { return br; } set { br = value; } }
        public BinaryWriter Bw { get { return bw; } set { bw = value; } }
        public Task PlayerThread { get { return playerThread; } set { playerThread = value; } }


        //handling each player according to the received requests
        public void playerHandling()
        {
            if (ct.IsCancellationRequested)
            {
                //MessageBox.Show("canceled");
            }
            ////MessageBox.Show("player handling thread entered");
            while (true && !ct.IsCancellationRequested)
            {
                string clientRequest = this.br.ReadString();
                //parsing the incoming request
                string[] arr = ReadFromClient(clientRequest);

                string request = arr[0];
                string color;

                ////MessageBox.Show("request sent = " + request);
                switch (request)
                {
                    case "100":
                        string name = arr[1];
                        Server.checkName(this, name);
                        break;
                    case "210":
                        string lobbyinfo = Server.getPlayer();
                        bw.Write(lobbyinfo);
                        // //MessageBox.Show(lobbyinfo);
                        break;
                    case "220":
                        // //MessageBox.Show("show rooms");
                        string s = Server.getRooms();
                        // //MessageBox.Show(s);
                        this.bw.Write(s);
                        break;
                    case "310":
                        ////MessageBox.Show("create room!!!!!!!");
                        //310,playername+color,roomname,row+col
                        int row, col;
                        string roomName = arr[3];
                        color = arr[2];
                        row = int.Parse(arr[4]);
                        col = int.Parse(arr[5]);
                        Server.createRoom(this, color, roomName, row, col);
                        break;
                    case "320":
                        ////MessageBox.Show("join room");
                        //320,roomname
                        string RoomName = arr[1];
                        int isJoined = Server.joinRoom(this, RoomName);
                        if (isJoined == -1)
                        {
                            //the player hasn't joined the room
                            bw.Write("320,-1");
                        }
                        break;
                    case "400":
                        color = arr[2];
                        Server.askToPlay(this, color);
                        break;
                    case "405":
                        int responseToPlay = int.Parse(arr[1]);
                        int response = Server.waitToPlay(this, responseToPlay);
                        //if (response == 1)
                        //{
                        //    //MessageBox.Show("game started");
                        //}
                        //else
                        //{
                        //    //MessageBox.Show("player 1 still waiting");
                        //}
                        break;
                    case "410":
                        Server.sendMove(this, int.Parse(arr[1]), int.Parse(arr[2]));
                        break;
                    case "600":
                        int playAgain = int.Parse(arr[1]);
                        Server.playAgain(this, playAgain);
                        break;
                    case "650":
                        Server.leaveRoom(this);
                        break;
                    case "700":
                        Server.disconnectPlayer(this);
                        break;
                }

            }
        }

        /// <summary>
        /// a function to parse the incoming requests from client
        /// </summary>
        /// <param incoming request="request"></param>
        /// <returns>an array of strings that represents the request</returns>
        public string[] ReadFromClient(string request)
        {
            //when the flag sytax is assigned
            string[] sArr = request.Split(',', '+');
            return sArr;
        }

    }
}
