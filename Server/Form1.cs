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
    public partial class Server : Form
    {
        #region server variables
        //server variables
        byte[] bt = new byte[] { 127, 0, 0, 1 };
        IPAddress add;
        TcpListener server;
        Socket connection;

        //players in lobby counter
        static int counter = 0;


        //players and rooms list
        static List<player> Allplayers = new List<player>();
        static List<room> allRooms = new List<room>();


        //requests enum
        public enum request
        {
            //receiveLoginInfo = 100,
            //getPlayers = 210,
            //getRooms = 220,
            //createRoom = 310,
            //joinRoom = 320,

            //askToplay = 400,
            //waitToPlay = 405,
            SendMove = 410,
            updateBoard = 420,
            gameEnded = 500
        }
        #endregion

        public Server()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        //start server button and its task function
        private void startServerBtn_Click(object sender, EventArgs e)
        {
            Task acceptingClients = new Task(NewClientListener);
            acceptingClients.Start();
        }
        public void NewClientListener()
        {
            add = new IPAddress(bt);
            server = new TcpListener(add, 2222);
            server.Start();
            MessageBox.Show("server started");
            while (true)
            {
                //start connection
                connection = server.AcceptSocket();
                //create new player
                player tempPlayer = new player();
                


                //assign the player member variables for stream
                tempPlayer.nstream = new NetworkStream(connection);
                tempPlayer.br = new BinaryReader(tempPlayer.nstream);
                tempPlayer.bw = new BinaryWriter(tempPlayer.nstream);

                //launch the thread of the new player
                tempPlayer.playerThread = new Task(tempPlayer.playerHandling);
                tempPlayer.playerThread.Start();


            }
        }

        //login name request
        public static void checkName(player tempPlayer, string requestedName)
        {
            bool exists = false;
            foreach (player p in Allplayers)
            {
                if(p.name == requestedName)
                {
                    exists = true;
                }
            }
            if (!exists)
            {
                tempPlayer.name = requestedName;
                //adding the player to the players list
                Allplayers.Add(tempPlayer);
                tempPlayer.bw.Write("100,1");
            }
            else
            {
                tempPlayer.bw.Write("100,0");
            }
        }

        //create a room request
        public static void createRoom(player roomOwner, string color)
        {
            room tempRoom = new room(roomOwner, 6, 6);
            roomOwner.myRoom = tempRoom;
            roomOwner.color = color;
            allRooms.Add(tempRoom);
            //updatelist(); //can't call it here as it needs to be static and when its static the roomlist control can't be accessed
        }

        //updating the roomList
        public void UpdateList()
        {
            roomList.Items.Clear();
            foreach (room r in allRooms)
            {
                roomList.Items.Add(r.roomName);
                foreach (player p in r.roomPlayers)
                {
                    roomList.Items.Add("    " + p.name);
                }
            }
            playerList.Items.Clear();
            foreach (player p in Allplayers)
            {
                playerList.Items.Add(p.name + " entered");
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateList();
        }

        //joining the room
        public static int joinRoom(player askingPlayer, int roomID)
        {
            int retVal = -1;
            room requestedRoom = null;
            bool isFound = false;
            for (var i = 0; i < allRooms.Count && !isFound; i++)
            {
                if (allRooms[i].roomID == roomID)
                {
                    requestedRoom = allRooms[i];
                    isFound = true;
                }
            }
            //the requested room is found 
            if (isFound == true)
            {
                //check if the player is gonna be playing or watching
                if (requestedRoom.roomPlayers.Count == 1)
                {
                    //the player will play
                    askingPlayer.isPlaying = true;
                    requestedRoom.roomPlayers.Add(askingPlayer);
                    requestedRoom.roomPlayers[0].bw.Write(askingPlayer.name + " has joined");
                    //requestedRoom.roomThread = new Task(requestedRoom.roomHandling);
                    //requestedRoom.roomThread.Start();
                    askingPlayer.myRoom = requestedRoom;
                    retVal = 1;
                }
                else
                {
                    //the player will join as a spectator
                    requestedRoom.roomPlayers.Add(askingPlayer);
                    //LOOOP ON ARRAY AND SEND IT [0,0,0,1],[0,0,1,0]
                    ///askingPlayer.bw.Write(requestedRoom.board.ToString());
                    retVal = 2;
                }
            }
            return retVal;
        }

        //send the online players and rooms to the connected client
        //get players
        public static string getPlayer()
        {
            string lobbyinfo = "210";
            foreach (player p in Allplayers)
            {
                lobbyinfo += "," + p.name + "+" + p.isPlaying;
            }
            return lobbyinfo;
        }

        //get rooms
        public static string getRooms()
        {
            string roomsData = "220";
            foreach (room r in allRooms)
            {
                roomsData += "," + r.roomID + "+" + r.roomName;
                foreach (player p in r.roomPlayers)
                {
                    roomsData += "+" + p.name + "-" + p.isPlaying + "-" + p.color;
                }
            }
            return roomsData;
        }

        //ask to play
        public static void askToPlay(player askingPlayer, string color)
        {
            room currentRoom = askingPlayer.myRoom;
            player roomOwner = currentRoom.roomPlayers[0];
            askingPlayer.color = color;
            string askingstr = "400," + askingPlayer.name + "+" + askingPlayer.color;
            roomOwner.bw.Write(askingstr);
            //askingPlayer.name = "asking to play";
            MessageBox.Show(roomOwner+ ": " + askingstr);
        }
     
        public static int waitToPlay(player roomOwner, int response)
        {
            room currentRoom = roomOwner.myRoom;
            player askingPlayer = currentRoom.roomPlayers[1];
            int retVal = -1;

            if (response == 1)
            {
                //if the room owner accepted
                askingPlayer.bw.Write("405,1");
                retVal = 1;
            }else
            {
                //the room owner refused 
                askingPlayer.bw.Write("405,0");
                //remove this player from the room
                currentRoom.roomPlayers.RemoveAt(1);
            }


            return retVal;
        }

        //update board
        public static void updateBoared(room currentRoom)
        {
            string updateStr = "";
            for (int row = 0; row < currentRoom.rows; row++)
            {
                updateStr += "[";
                for (int col = 0; col < currentRoom.cols; col++)
                {
                    if(col < currentRoom.cols - 1)
                        updateStr += currentRoom.board[row, col] + ",";
                    else
                        updateStr += currentRoom.board[row, col] ;
                }
                if(row < currentRoom.rows -1)
                    updateStr += "],";
                else
                    updateStr += "]";
            }
            foreach (player p in currentRoom.roomPlayers)
            {
                p.bw.Write(updateStr);
            }
            MessageBox.Show("boared updated!\n" + updateStr);
        }
        //End game
        public static void endGame(player winner)
        {
            room currentRoom = winner.myRoom;
            int winnerIndex = currentRoom.roomPlayers.IndexOf(winner);
            player loser;
            //assigning the loser player
            if (winnerIndex == 0)
            {
                loser = currentRoom.roomPlayers[1];
            }
            else
            {
                loser = currentRoom.roomPlayers[0];
            }
            //sending the end game response 
            for (var i =0; i < currentRoom.roomPlayers.Count; i++)
            {
                player currentPlayer = currentRoom.roomPlayers[i];
                if(currentPlayer.name == winner.name)
                {
                    winner.bw.Write("500,1");
                }
                else if(currentPlayer.name == loser.name)
                {
                    winner.bw.Write("500,0");
                }
                else
                {
                    winner.bw.Write("500,-1");
                }
            }
        }
        //send move
        public static void sendMove(player moveSender, int x, int y)
        {
            room currentRoom = moveSender.myRoom;
            currentRoom.board[x, y] = 1;
            updateBoared(currentRoom);
            bool isWinner = checkWinner(moveSender);
            if (isWinner)
            {
                endGame(moveSender);
            }
        }
        public static bool checkWinner(player moveSender)
        {
            bool isWinner = false;
            return isWinner;
        }
    }
    public class player
    {
        public string name;
        public NetworkStream nstream;
        public BinaryReader br;
        public BinaryWriter bw;
        public Task playerThread;
        public bool isPlaying = false;
        public room myRoom = null;
        public string color;

        public void playerHandling()
        {
            //MessageBox.Show("player handling thread entered");
            while (true)
            {
                string clientRequest = this.br.ReadString();
                //parsing the incoming request
                string[] arr = ReadFromClient(clientRequest);

                string request = arr[0];
                string color;

                //MessageBox.Show("request sent = " + request);
                switch (request)
                {
                    case "100":
                        string name = arr[1];
                        Server.checkName(this, name);
                        break;
                    case "310":
                        //MessageBox.Show("create room!!!!!!!");
                        //int row, col;
                        color = arr[1];
                        Server.createRoom(this, color);
                        break;
                    case "320":
                        //MessageBox.Show("join room");
                        string roomID = arr[1];
                        int isJoined = Server.joinRoom(this, int.Parse(roomID));
                        if (isJoined == 1)
                        {
                            //the player has joined the room as a player
                            bw.Write("320,1");
                        }
                        else if (isJoined == 2)
                        {
                            //the player has joined the room as a spectator
                            bw.Write("320,2");
                        }
                        else
                        {
                            //the player hasn't joined the room
                            bw.Write("320,-1");
                        }
                        break;
                    case "210":
                        string lobbyinfo = Server.getPlayer();
                        bw.Write(lobbyinfo);
                        // MessageBox.Show(lobbyinfo);
                        break;
                    case "220":
                        // MessageBox.Show("show rooms");
                        string s = Server.getRooms();
                        // MessageBox.Show(s);
                        this.bw.Write(s);
                        break;
                    case "400":
                        color = arr[1];
                        Server.askToPlay(this, color);
                        break;
                    case "405":
                        int responseToPlay = int.Parse(arr[1]);
                        int response = Server.waitToPlay(this, responseToPlay);
                        if (response == 1)
                        {
                            MessageBox.Show("game started");
                        }
                        else
                        {
                            MessageBox.Show("player 1 still waiting");
                        }
                        break;
                    case "410":
                        Server.sendMove(this, int.Parse(arr[1]), int.Parse(arr[2]));
                        break;
                    case "600":
                        //Server.playAgain(this)


                        break;
                }
                
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string[] ReadFromClient(string request)
        {
            //when the flag sytax is assigned
            string[] sArr = request.Split(',');
            //MessageBox.Show(sArr[0]);
            return sArr;
        }

    }
    public class room
    {
        static int counter = 0;
        public int roomID;
        public string roomName;
        public List<player> roomPlayers = new List<player>();
        public int rows; 
        public int cols;
        public int[,] board;

        public BinaryReader br;
        public BinaryWriter bw;
        public Task roomThread;


        public room(player roomOwner, int r, int c)
        {
            counter++;
            roomID = counter;
            roomName = "room number " + roomID;
            rows = r;
            cols = c;
            board = new int[r, c];
            roomOwner.isPlaying = true;
            roomPlayers.Add(roomOwner);
        }
        //public void roomHandling()
        //{
        //    while (true)
        //    {
        //        string roomRequest = this.br.ReadString();
        //        if (roomRequest == "310")
        //        {
        //            MessageBox.Show("create room!!!!!!! from room class");
        //            //int row, col;
        //            //Server.createRoom(this);
        //        }

        //    }
        //}
    }
}
