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
            gameEnded = 500,

            disconnectPlayer = 700
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
                tempPlayer.Nstream = new NetworkStream(connection);
                tempPlayer.Br = new BinaryReader(tempPlayer.Nstream);
                tempPlayer.Bw = new BinaryWriter(tempPlayer.Nstream);

                //assigning the cancellation token
                tempPlayer.ct = tempPlayer.tokenSource.Token;
                

                //launch the thread of the new player
                tempPlayer.PlayerThread = new Task(tempPlayer.playerHandling, tempPlayer.tokenSource.Token);
                tempPlayer.PlayerThread.Start();


            }
        }

        //login Name request
        public static void checkName(player tempPlayer, string requestedName)
        {
            bool exists = false;
            foreach (player p in Allplayers)
            {
                if(p.Name == requestedName)
                {
                    exists = true;
                }
            }
            if (!exists)
            {
                tempPlayer.Name = requestedName;
                //adding the player to the players list
                Allplayers.Add(tempPlayer);
                tempPlayer.Bw.Write("100,1");
            }
            else
            {
                tempPlayer.Bw.Write("100,0");
            }
        }

        //create a room request
        public static void createRoom(player roomOwner, string Color)
        {
            room tempRoom = new room(roomOwner, 6, 6);
            roomOwner.MyRoom = tempRoom;
            roomOwner.Color = Color;
            allRooms.Add(tempRoom);
            //updatelist(); //can't call it here as it needs to be static and when its static the roomlist control can't be accessed
        }

        //updating the roomList
        public void UpdateList()
        {
            roomList.Items.Clear();
            for(int i=0; i< allRooms.Count; i++)
            {
                //if there are players in the room
                if (allRooms[i].RoomPlayers.Count != 0)
                {
                    roomList.Items.Add(allRooms[i].RoomName);
                    foreach (player p in allRooms[i].RoomPlayers)
                    {
                        roomList.Items.Add("    " + p.Name);
                    }
                }
                else
                {
                    roomList.Items.Remove(allRooms[i]);
                }
            }
            playerList.Items.Clear();
            foreach (player p in Allplayers)
            {
                playerList.Items.Add(p.Name + " entered");
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateList();
        }

        //joining the room
        public static int joinRoom(player askingPlayer, int RoomID)
        {
            int retVal = -1;
            room requestedRoom = null;
            bool isFound = false;
            for (var i = 0; i < allRooms.Count && !isFound; i++)
            {
                if (allRooms[i].RoomID == RoomID)
                {
                    requestedRoom = allRooms[i];
                    isFound = true;
                }
            }
            //the requested room is found 
            if (isFound == true)
            {
                //check if the player is gonna be playing or watching
                if (requestedRoom.RoomPlayers.Count == 1)
                {
                    //the player will play
                    askingPlayer.IsPlaying = true;
                    requestedRoom.RoomPlayers.Add(askingPlayer);
                    requestedRoom.RoomPlayers[0].Bw.Write(askingPlayer.Name + " has joined");
                    //requestedRoom.roomThread = new Task(requestedRoom.roomHandling);
                    //requestedRoom.roomThread.Start();
                    askingPlayer.MyRoom = requestedRoom;
                    retVal = 1;
                }
                else
                {
                    //the player will join as a spectator
                    requestedRoom.RoomPlayers.Add(askingPlayer);
                    //LOOOP ON ARRAY AND SEND IT [0,0,0,1],[0,0,1,0]
                    ///askingPlayer.Bw.Write(requestedRoom.Board.ToString());
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
                lobbyinfo += "," + p.Name + "+" + p.IsPlaying;
            }
            return lobbyinfo;
        }

        //get rooms
        public static string getRooms()
        {
            string roomsData = "220";
            foreach (room r in allRooms)
            {
                roomsData += "," + r.RoomID + "+" + r.RoomName;
                foreach (player p in r.RoomPlayers)
                {
                    roomsData += "+" + p.Name + "-" + p.IsPlaying + "-" + p.Color;
                }
            }
            return roomsData;
        }

        //ask to play
        public static void askToPlay(player askingPlayer, string Color)
        {
            room currentRoom = askingPlayer.MyRoom;
            player roomOwner = currentRoom.RoomPlayers[0];
            askingPlayer.Color = Color;
            string askingstr = "400," + askingPlayer.Name + "+" + askingPlayer.Color;
            roomOwner.Bw.Write(askingstr);
            //askingPlayer.Name = "asking to play";
            MessageBox.Show(roomOwner+ ": " + askingstr);
        }
     
        public static int waitToPlay(player roomOwner, int response)
        {
            room currentRoom = roomOwner.MyRoom;
            player askingPlayer = currentRoom.RoomPlayers[1];
            int retVal = -1;

            if (response == 1)
            {
                //if the room owner accepted
                askingPlayer.Bw.Write("405,1");
                retVal = 1;
            }else
            {
                //the room owner refused 
                askingPlayer.Bw.Write("405,0");
                //remove this player from the room
                currentRoom.RoomPlayers.RemoveAt(1);
                //remove the room refrence from the player
                askingPlayer.MyRoom = null;
                //remove all audience
                for (int i = 2; i <currentRoom.RoomPlayers.Count; i++)
                {
                    currentRoom.RoomPlayers.RemoveAt(i);
                }
            }


            return retVal;
        }

        //update Board
        public static void updateBoared(room currentRoom)
        {
            string updateStr = "";
            for (int row = 0; row < currentRoom.Rows; row++)
            {
                updateStr += "[";
                for (int col = 0; col < currentRoom.Cols; col++)
                {
                    if(col < currentRoom.Cols - 1)
                        updateStr += currentRoom.Board[row, col] + ",";
                    else
                        updateStr += currentRoom.Board[row, col] ;
                }
                if(row < currentRoom.Rows -1)
                    updateStr += "],";
                else
                    updateStr += "]";
            }
            foreach (player p in currentRoom.RoomPlayers)
            {
                p.Bw.Write(updateStr);
            }
            MessageBox.Show("boared updated!\n" + updateStr);
        }
        //End game
        public static void endGame(player winner)
        {
            room currentRoom = winner.MyRoom;
            int winnerIndex = currentRoom.RoomPlayers.IndexOf(winner);
            player loser;
            //assigning the loser player
            if (winnerIndex == 0)
            {
                loser = currentRoom.RoomPlayers[1];
            }
            else
            {
                loser = currentRoom.RoomPlayers[0];
            }
            //sending the end game response 
            for (var i =0; i < currentRoom.RoomPlayers.Count; i++)
            {
                player currentPlayer = currentRoom.RoomPlayers[i];
                if(currentPlayer.Name == winner.Name)
                {
                    winner.Bw.Write("500,1");
                }
                else if(currentPlayer.Name == loser.Name)
                {
                    winner.Bw.Write("500,0");
                }
                else
                {
                    winner.Bw.Write("500,-1");
                }
            }
        }
        //send move
        public static void sendMove(player moveSender, int x, int y)
        {
            room currentRoom = moveSender.MyRoom;
            currentRoom.Board[x, y] = 1;
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

        //play again after one has win or lose
        public static void playAgain(player moveSender, int PlayAgain)
        {
            //600,1 sending player wants to play again
            room currentRoom = moveSender.MyRoom;
            //int winnerIndex = currentRoom.RoomPlayers.IndexOf(winner);
            if (currentRoom.GameEnded == 0)//the other player havn't responded yet
            {
                //check the response of the move sender
                currentRoom.GameEnded++;
                if (PlayAgain == 1)
                {
                    moveSender.PlayAgain = true;
                }
            }
            else //the other player has already sent the response
            {
                if (PlayAgain == 1)
                {
                    moveSender.PlayAgain = true;
                }
                if (currentRoom.RoomPlayers[0].Name == moveSender.Name)//the move sender is the room owner
                {
                    player guestPlayer = currentRoom.RoomPlayers[1];
                    if (moveSender.PlayAgain)//the room owner wants to play again
                    {
                        //check the guest
                        if (guestPlayer.PlayAgain)//the guest wants to play also
                        {
                            waitToPlay(moveSender, 1);
                        }
                        else//the guest doesn't want to play so kick him out
                        {
                            waitToPlay(moveSender, 0);
                        }
                    }
                    else//the room owner doesn't want to play again
                    {
                        waitToPlay(moveSender, 0);
                    }

                }
                else //the second responder is the guest
                {
                    player roomOwner = currentRoom.RoomPlayers[0];
                    if (moveSender.PlayAgain)//the guest wants to play again
                    {
                        //check the guest
                        if (roomOwner.PlayAgain)//the room owner wants to play also
                        {
                            waitToPlay(moveSender, 1);
                        }
                        else//the room owner doesn't want to play so kick the guest out
                        {
                            waitToPlay(moveSender, 0);
                        }
                    }
                    else
                    {
                        waitToPlay(moveSender, 0);
                    }
                }
                //restore the default settings
                currentRoom.GameEnded = 0;
                currentRoom.RoomPlayers[0].PlayAgain = false;
                currentRoom.RoomPlayers[1].PlayAgain = false;
                currentRoom.Board = new int[currentRoom.Rows, currentRoom.Cols];
            }
        }

        internal static void disconnectPlayer(player player)
        {
            room playerRoom = player.MyRoom;
            //close all the player's streams, writer and reader
            player.Br.Close();
            player.Bw.Close();
            player.Nstream.Close();
            //dispose the player's thread
            player.tokenSource.Cancel();
            //player.PlayerThread.Dispose();
            //remove the player from the players list 
            Allplayers.Remove(player);
            playerRoom.RoomPlayers.Remove(player);
        }
    }
    public class player
    {
        string name;
        string color;
        bool isPlaying = false;
        bool playAgain = false;
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
        public room MyRoom { get { return myRoom; } set { myRoom = value; } }
        public NetworkStream Nstream { get { return nstream; } set { nstream = value; } }
        public BinaryReader Br { get { return br; } set { br = value; } }
        public BinaryWriter Bw { get { return bw; } set { bw = value; } }
        public Task PlayerThread { get { return playerThread; } set { playerThread = value; } }



        public void playerHandling()
        {
            if (ct.IsCancellationRequested)
            {
                MessageBox.Show("canceled");
            }
            //MessageBox.Show("player handling thread entered");
            while (true && !ct.IsCancellationRequested)
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
                        string RoomID = arr[1];
                        int isJoined = Server.joinRoom(this, int.Parse(RoomID));
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
                        int playAgain = int.Parse( arr[1]);
                        Server.playAgain(this, playAgain);
                        break;
                    case "700":
                        Server.disconnectPlayer(this);
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
        int roomID;
        string roomName;
        static int counter = 0; //to specify room name
        //room specifications
        int rows; 
        int cols;
        int[,] board;
        int gameEnded = 0;
        //room players list
        List<player> roomPlayers = new List<player>();
        
        //class getters and setters
        public int RoomID { get { return roomID; } set { roomID = value; } }
        public string RoomName { get { return roomName; } set { roomName = value; } }
        public int Rows { get { return rows; } set { rows = value; } }
        public int Cols { get { return cols; } set { cols = value; } }
        public int[,] Board { get { return board; } set { board = value; }  }
        public List<player> RoomPlayers { get { return roomPlayers; } }
        public int GameEnded { get { return gameEnded; } set { gameEnded = value; } }

        public room(player roomOwner, int r, int c)
        {
            counter++;
            roomID = counter;
            roomName = "room number " + roomID;
            rows = r;
            cols = c;
            board = new int[r, c];
            roomOwner.IsPlaying = true;
            roomPlayers.Add(roomOwner);
        }
    }
}
