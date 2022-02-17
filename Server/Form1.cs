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
            receiveLoginInfo = 100,
            getPlayers = 210,
            getRooms = 220,
            createRoom = 310,
            joinRoom = 320,
            askToplay = 400,
            waitToPlay = 405,
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
                if (p.Name == requestedName)
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
                foreach (var player in Allplayers)
                {
                    player.Bw.Write(getPlayer());
                }
            }
            else
            {
                tempPlayer.Bw.Write("100,0");
            }
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
                roomsData += "," + r.RoomName;
                foreach (player p in r.RoomPlayers)
                {
                    roomsData += "+" + p.Name + "-" + p.IsPlaying + "-" + p.Color;
                }
            }
            return roomsData;
        }

        //create a room request
        public static void createRoom(player roomOwner, string Color, string roomName, int row, int col)
        {
            room tempRoom = new room(roomOwner, roomName, row, col);
            roomOwner.MyRoom = tempRoom;
            roomOwner.Color = Color;
            allRooms.Add(tempRoom);

            foreach (var player in Allplayers)
            {
                player.Bw.Write(getRooms());
            }
            foreach (var player in Allplayers)
            {
                player.Bw.Write(getPlayer());
            }
        }

        //joining the room
        public static int joinRoom(player askingPlayer, string roomName)
        {
            int retVal = -1;
            room requestedRoom = null;
            bool isFound = false;
            for (var i = 0; i < allRooms.Count && !isFound; i++)
            {
                if (allRooms[i].RoomName == roomName)
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
                    //the player has joined the room as a player
                    //the player will play
                    askingPlayer.IsPlaying = true;
                    requestedRoom.RoomPlayers.Add(askingPlayer);
                    //requestedRoom.RoomPlayers[0].Bw.Write(askingPlayer.Name + " has joined");
                    askingPlayer.MyRoom = requestedRoom;
                    askingPlayer.Bw.Write("320,1");
                }
                else
                {
                    //the player will join as a spectator
                    requestedRoom.RoomPlayers.Add(askingPlayer);
                    askingPlayer.MyRoom = requestedRoom;
                    retVal = 2;
                    askingPlayer.Bw.Write($"320,2,{requestedRoom.RoomPlayers[0].Color},{requestedRoom.RoomPlayers[1].Color},{requestedRoom.Rows}+{requestedRoom.Cols}");
                }
                foreach (var player in Allplayers)
                {
                    player.Bw.Write(getRooms());
                }
                foreach (var player in Allplayers)
                {
                    player.Bw.Write(getPlayer());
                }
            }
            return retVal;
        }


        //ask the room owner to play
        public static void askToPlay(player askingPlayer, string Color)
        {
            room currentRoom = askingPlayer.MyRoom;
            player roomOwner = currentRoom.RoomPlayers[0];
            askingPlayer.Color = Color;
            string askingstr = "400," + askingPlayer.Name + "+" + askingPlayer.Color;
            roomOwner.Bw.Write(askingstr);
            //askingPlayer.Name = "asking to play";
            //MessageBox.Show(roomOwner+ ": " + askingstr);
        }
        //responding to the player asing to play
        public static int waitToPlay(player roomOwner, int response)
        {
            room currentRoom = roomOwner.MyRoom;
            player askingPlayer = currentRoom.RoomPlayers[1];
            int retVal = -1;

            if (response == 1)
            {
                //if the room owner accepted
                askingPlayer.Bw.Write($"405,1,{currentRoom.Rows}+{currentRoom.Cols},{roomOwner.Color}");
                retVal = 1;
            }
            else
            {
                //the room owner refused 
                askingPlayer.Bw.Write("405,0,0,0");
                //remove the room refrence from the player
                askingPlayer.MyRoom = null;
                //restore the player's score to 0 and is playing to false
                askingPlayer.Score = 0;
                askingPlayer.IsPlaying = false;
                //remove this player from the room
                currentRoom.RoomPlayers.Remove(askingPlayer);
                //return the room turn to the owner if the challenger is kicked out
                currentRoom.PlayerTurn = 1;
                //remove all audience
                int roomPlayersCount = currentRoom.RoomPlayers.Count;
                for (int i = 1; i < roomPlayersCount; i++)
                {
                    currentRoom.RoomPlayers[i].MyRoom = null;
                }
                for (int i = 1; i < roomPlayersCount; i++)
                {
                    currentRoom.RoomPlayers.RemoveAt(1);
                }
                foreach (var player in Allplayers)
                {
                    player.Bw.Write(getRooms());
                }
                foreach (var player in Allplayers)
                {
                    player.Bw.Write(getPlayer());
                }
            }


            return retVal;
        }


        //send the move from one player to all the other in the room and check if this move win
        //send move
        public static void sendMove(player moveSender, int x, int y)
        {
            room currentRoom = moveSender.MyRoom;
            //change the room player turn & change the fill board according to the player turn
            currentRoom.Board[x, y] = (currentRoom.PlayerTurn == 1) ? 1 : 2;
            int winnerPlayer = currentRoom.checkWin(currentRoom.PlayerTurn);
            currentRoom.PlayerTurn = (currentRoom.PlayerTurn == 1) ? 2 : 1;
            //update the room board and send it to all the room players
            updateBoared(currentRoom);
            if (winnerPlayer == 1 || winnerPlayer == 2)
            {
                endGame(moveSender);
                //MessageBox.Show($"{moveSender.Name} has won the game");
            }
            updateBoared(currentRoom);
        }
        //update the Board in all the room members 
        public static void updateBoared(room currentRoom)
        {
            //parse the room board and sent it to all the room players
            //410,2,0+1+0+2+0,0+1+0+2+0
            string updateStr = $"410,{currentRoom.PlayerTurn},";
            for (int row = 0; row < currentRoom.Rows; row++)
            {
                for (int col = 0; col < currentRoom.Cols; col++)
                {
                    if (col < currentRoom.Cols - 1)
                        updateStr += currentRoom.Board[row, col] + "+";
                    else
                        updateStr += currentRoom.Board[row, col];
                }
                if (row < currentRoom.Rows - 1)
                    updateStr += ",";
            }
            foreach (player p in currentRoom.RoomPlayers)
            {
                p.Bw.Write(updateStr);
            }
           // MessageBox.Show("boared updated!\n" + updateStr);
        }
        //End game if a player has won in this move
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
            for (var i = 0; i < currentRoom.RoomPlayers.Count; i++)
            {
                player currentPlayer = currentRoom.RoomPlayers[i];
                if (currentPlayer.Name == winner.Name)
                {
                    currentPlayer.Bw.Write("500,1");
                    currentPlayer.Score++;
                }
                else if (currentPlayer.Name == loser.Name)
                {
                    currentPlayer.Bw.Write("500,0");
                }
                else
                {
                    currentPlayer.Bw.Write("500,-1," + winner.Name);
                }
            }
            //clearing the room board
            for(int i=0;i<currentRoom.Board.GetLength(0);i++)
            {
                for (int j = 0; j < currentRoom.Board.GetLength(1); j++)
                {
                    currentRoom.Board[i, j] = 0;
                }
            }
            //return the turn to the winning player if they played again
            currentRoom.PlayerTurn = (currentRoom.PlayerTurn == 1) ? 2 : 1;
        }


        //play again after one has win or lose and save the score to the server text file
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
                            //waitToPlay(moveSender, 1);
                            Server.updateBoared(currentRoom);
                        }
                        else//the guest doesn't want to play so kick him out
                        {
                            currentRoom.RoomPlayers[1].PlayAgain = false;
                            saveGameToFile(currentRoom);
                            waitToPlay(moveSender, 0);
                        }
                    }
                    else//the room owner doesn't want to play again
                    {
                        currentRoom.RoomPlayers[1].PlayAgain = false;
                        saveGameToFile(currentRoom);
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
                            //waitToPlay(moveSender, 1);
                            Server.updateBoared(currentRoom);
                        }
                        else//the room owner doesn't want to play so kick the guest out
                        {
                            currentRoom.RoomPlayers[1].PlayAgain = false;
                            saveGameToFile(currentRoom);
                            waitToPlay(moveSender, 0);
                        }
                    }
                    else
                    {
                        currentRoom.RoomPlayers[1].PlayAgain = false;
                        saveGameToFile(currentRoom);
                        waitToPlay(moveSender, 0);
                    }
                }
                //restore the default settings
                currentRoom.GameEnded = 0;
                currentRoom.RoomPlayers[0].PlayAgain = false;
                currentRoom.Board = new int[currentRoom.Rows, currentRoom.Cols];
            }
        }
        public static void saveGameToFile(room room)
        {
            //Player2 name “value”, Player2 name “value” date of the game
            string path = @"C:\Users\Blu-Ray\OneDrive\Desktop\iti\visual C# .NET\project\scoreSheet.txt";

            StreamWriter Sw = File.AppendText(path);
            Sw.WriteLine($"player1: {room.RoomPlayers[0].Name}, Score: {room.RoomPlayers[0].Score}, player2: {room.RoomPlayers[1].Name}, score: {room.RoomPlayers[1].Score}, Date: {DateTime.Now.ToString()}");
            Sw.Close();
        }


        //if the room owner wants to leave the room and go to the lobby again
        public static void leaveRoom(player player)
        {
            if (player.MyRoom != null && player.MyRoom.RoomPlayers.Count<2)
            {
                if (player.MyRoom.RoomPlayers[0].Name == player.Name)
                {
                    room currentRoom = player.MyRoom;
                    player.MyRoom = null;
                    player.Score = 0;
                    player.IsPlaying = false;
                    currentRoom.RoomPlayers.Remove(player);
                    allRooms.Remove(currentRoom);
                    foreach (var p in Allplayers)
                    {
                        p.Bw.Write(getRooms());
                    }
                    foreach (var p in Allplayers)
                    {
                        p.Bw.Write(getPlayer());
                    }
                }
            }
        }
        //when the player wants to logout of the game
        public static void disconnectPlayer(player player)
        {
            //close all the player's streams, writer and reader
            player.Br.Close();
            player.Bw.Close();
            player.Nstream.Close();
            //we need to close the connection socket also and use .shutdown()
            //dispose the player's thread
            player.tokenSource.Cancel();
            //player.PlayerThread.Dispose();
            //remove the player from the players list 
            Allplayers.Remove(player);
            foreach (var p in Allplayers)
            {
                p.Bw.Write(getPlayer());
            }
        }
        

        //updating the roomList in the server GUI
        public void UpdateList()
        {
            roomList.Items.Clear();
            for (int i = 0; i < allRooms.Count; i++)
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

    }
    
    
}
