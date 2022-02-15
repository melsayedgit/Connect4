using Client.Popups;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    /// <summary>
    ///  Responsible for all Connect4 Game client logic 
    /// </summary>

    public static class GameManger
    {

        static string ip;
        static int port;
        public static bool connStatues;
        static string UserName;


      static  IPAddress ServerIP;
      static  TcpClient server;
      static  NetworkStream ConnectionStream;
      static  BinaryReader br;
      static  BinaryWriter bwr;
      public static Task recieve;

     public static List<Player> playerslist;
     public static List<Room> Rommslist;

     public static Player CurrentPlayer;
     public static Room CurrentRoom;

     

        static GameManger()
           
        {
            ip = "127.0.0.1";
            port = 2222;
            connStatues = false;
            ServerIP =  IPAddress.Parse(ip);

          playerslist = new List<Player>();
          Rommslist = new List<Room>();

    }

        /// <summary>
        ///  Connect the user to the server and return and return The result of the attempt 
        /// </summary>
   
        public static void Login(string userName)
        {
    

            try
            {
                TcpClient server = new TcpClient();
                server.Connect(ServerIP, port);
                ConnectionStream = server.GetStream();
                br = new BinaryReader(ConnectionStream);
                bwr = new BinaryWriter(ConnectionStream);

                SendServerRequest(Flag.sendLoginInfo, userName);
                UserName = userName;

                //recieve = new Task(ReceiveServerRequest);
                //recieve.Start();
                //SendServerRequest(Flag.getPlayers);
                //SendServerRequest(Flag.getRooms);




            }
            catch (Exception e)
            {
            
                throw e;
            }

           



        }
        /// <summary>
        ///  This Method used for communicating with the game server
        /// </summary>
        public static void SendServerRequest(Flag flag,params string[] data)
        {
            var f = (int)flag;
            string msg = f.ToString();
           

            if (data.Length >0)
            {
                foreach (var item in data)
                {
                    msg += "," + item;

                }


            }
           
            bwr.Write(msg);
            //MessageBox.Show(msg);

        }

        public static bool isloginSuc(string userName)
        {
            
            var msg = br.ReadString();
            var msgArray = msg.Split(',');
            Flag flag = (Flag)int.Parse(msgArray[0]);
            var data = msgArray.ToList();
           // MessageBox.Show(data.ElementAt(0) + "," + data.ElementAt(1));
            data.RemoveAt(0);
            if (data.ElementAt(0) == "1")
            {
                UserName = userName;


                connStatues = true;

                return true;
            }
            else
            {
                MessageBox.Show("Name already taken");
     
                return false;
            }
        }


        public static void ReceiveServerRequest()
        {
            var msg = br.ReadString();
           // MessageBox.Show(msg);
 
            var msgArray = msg.Split(',');
            Flag flag = (Flag)int.Parse(msgArray[0]);
            var data = msgArray.ToList();
            data.RemoveAt(0);

            switch (flag)
            {
                case Flag.getPlayers:
                     playerslist = Getplayers(data);
                    lobby.mainlobby.Invoke(new MethodInvoker(delegate ()
                    {
                        lobby.mainlobby.showplayer();
                    }));
                    break;

                case Flag.getRooms:
                    Rommslist = GetRooms(data);
                    lobby.mainlobby.Invoke(new MethodInvoker(delegate ()
                    {
                        lobby.mainlobby.showroom();
                    }));
                    break;

                case Flag.waittopaly:
                    //care if the owner refused he return 405,0 so it throws exception 
                    playgame(data.ElementAt(0), data.ElementAt(1), data.ElementAt(2)); // if 405,1: hide or open gamebaord     else 405,0:close choose color host didnt accpet
                    break;
                case Flag.createRoom:
                    break;
                case Flag.joinRoom:
                    if (data.ElementAt(0) == "2")
                    {
                        joinASspectator(data.ElementAt(1), data.ElementAt(2), data.ElementAt(3));
                    }
                   
                    break;
                case Flag.SendMove:
                    GameBoard.turn = int.Parse(data.ElementAt(0));
                    data.RemoveAt(0);
                    updateBoard(data);
                    break;
                case Flag.updateBoard:
                    break;
                case Flag.asktoplay:
                    //"400, askingPlayer.Name + askingPlayer.Color"
                    acceptTheChallenger(data[0]);
                    GameBoard.ChallangerColor = Color.FromArgb(Int32.Parse(data[0].Split('+')[1]));
                    GameBoard.ChallangerBrush = new SolidBrush(GameBoard.ChallangerColor);
                    // MessageBox.Show("the color is:" + GameBoard.ChallangerColor);
                    break;
                case Flag.gameResult:
                    showWinningMesg(data);
                        break;
                default:
                    break;
            }
          //  MessageBox.Show(msg);
            ReceiveServerRequest();

        }

        private static void joinASspectator(string hostColor,string ChallngerColor,string size)
        {
            var sizear = size.Split('+');
            lobby.mainlobby.Invoke(new MethodInvoker(delegate ()
            {
                //lobby.mainlobby.join_spectate.Close();
              
                message ms = new message();
                ms.msg = "YOU ARE NOW SPECTATING \n THE GAME";
                DialogResult res = ms.ShowDialog();
                GameBoard.columns = int.Parse(sizear[1]);
                GameBoard.rows = int.Parse(sizear[0]);
                GameBoard.HostColor = Color.FromArgb(Int32.Parse(hostColor));
                GameBoard.ChallangerColor = Color.FromArgb(Int32.Parse(ChallngerColor));
                GameBoard.turn = 0;
                GameBoard.playerTurn = 3;

                lobby.seegamebaord = new GameBoard();
                lobby.seegamebaord.Show();
            }));
        }

        private static void showWinningMesg(List<string> data)
        {

            switch (data[0])
            {
                case "0":
                    GameBoard.currntGameboard.Invoke(new MethodInvoker(delegate ()
                    {
                        winORlose.result = 0;
                        GameBoard.winandlose = new winORlose();
                        GameBoard.winandlose.ShowDialog();

                        
                    }));
                    break;
                case "1":
                    GameBoard.currntGameboard.Invoke(new MethodInvoker(delegate ()
                    {
                        winORlose.result = 1;
                        GameBoard.winandlose = new winORlose();
                        GameBoard.winandlose.ShowDialog();
                       
                    }));
                    break;
                case "-1":
                    GameBoard.currntGameboard.Invoke(new MethodInvoker(delegate ()
                    {
                        winORlose.result = -1;
                        winORlose.winner = data[1];
                        GameBoard.winandlose = new winORlose();
                        GameBoard.winandlose.ShowDialog();
                    }));
                    break;
                default:
                    break;
            }
        }

        private static void acceptTheChallenger(string data)
        {
            //pop up a menu asking the room owner if he wants the challenger to play or not
            
            acceptTheChallenger dlg = new acceptTheChallenger();
            string[] arr = data.Split('+');
            dlg.challengerLabel = $"{arr[0]} wants to challange you ";
            DialogResult ownerResponse = dlg.ShowDialog();
            //if the owner accepts
            if (ownerResponse == DialogResult.OK)
            {
                SendServerRequest(Flag.waittopaly, "1");
                lobby.mainlobby.Invoke(new MethodInvoker(delegate ()
                {
                    message ms = new message();
                    ms.msg = $"  You are currently playing against: {arr[0]}";
                    lobby.wait.Close();
                    lobby.mainlobby.board.Show();

                }));
            }
            else
            {
                SendServerRequest(Flag.waittopaly, "0");
            }
        }

        private static void playgame(string response, string size,string hostcolor)
        {
            if (int.Parse(response) == 1)
            {
                var sizear = size.Split('+');
                lobby.mainlobby.Invoke(new MethodInvoker(delegate ()
                {
                    lobby.wait.Close();
                    //lobby.mainlobby.Hide();
                    message ms = new message();
                    ms.msg = "the owner has accepted you!";
                    DialogResult res = ms.ShowDialog();
                    GameBoard.columns = int.Parse(sizear[1]);
                    GameBoard.rows = int.Parse(sizear[0]);
                    GameBoard.HostColor = Color.FromArgb(Int32.Parse(hostcolor));
                    GameBoard.ChallangerColor = CurrentPlayer.PlayerColor;
                    GameBoard.turn = 1;
                    GameBoard.playerTurn = 2;

                    lobby.seegamebaord = new GameBoard();
                    lobby.seegamebaord.Show();
                }));

       

            }

            else
            {
                lobby.mainlobby.Invoke(new MethodInvoker(delegate ()
                {

                    lobby.seegamebaord.Close();
                  MessageBox.Show("am dead");
                }));

            }
        }
       private static void updateBoard(List<string> data)
       {
            //int[,] tempbaord = new int[GameBoard.rows, GameBoard.columns];

            for (int i = 0; i < data.Count; i++)
            {
                var rowstring = data.ElementAt(i);
                var row = rowstring.Split('+');
                for (int j = 0; j < row.Length; j++)
                {

                    GameBoard.currntGameboard.board[i, j] = int.Parse(row[j]);
                    //GameBoard.currntGameboard.board[i, j] = int.Parse(row[j]);

                }
             
            }
            //check if the game board is shown so it is updated,
            //as when a player send that he doesnot want to play again
            //and the game board panel is closed it doesnot throw an exception
            if (GameBoard.currntGameboard.Visible)
            {
                GameBoard.currntGameboard.BeginInvoke(new MethodInvoker(delegate () {

                    GameBoard.currntGameboard.repaintBord();
                }));

            }

            //string updateStr = "";
            //for (int row = 0; row < rows; row++)
            //{
            //    updateStr += "[";
            //    for (int col = 0; col <columns; col++)
            //    {
            //        if (col < columns - 1)
            //            updateStr += board[row, col] + ",";
            //        else
            //            updateStr += board[row, col];
            //    }
            //    if (row < rows - 1)
            //        updateStr += "],";
            //    else
            //        updateStr += "]";
            //}

        }

        /// <summary>
        ///  Get all the players on the server and return them as an List of Players
        /// </summary>

        public static List<Player> Getplayers(List<string> data)
        {
            var players = new List<Player>();

            foreach (var item in data)
            {
                var name = item.Split('+')[0];
                players.Add(new Player(name));
            }
            return players;
        }

        /// <summary>
        ///  Get all the players on the server and return them as an List of Players
        /// </summary>

        public static List<Room> GetRooms(List<string> data)
        {   
            var rooms = new List<Room>();

            foreach (var item in data)
            {
                var rom = item.Split('+');
                var roomName = rom[0];
                var host = rom[1].Split('-');
                var addedRoom = new Room(roomName, new Player(host[0]));
                if (rom.Length == 3)
                {
                    addedRoom.challenger = new Player(rom[2].Split('-')[0]);
                }

                rooms.Add(addedRoom);
    
            }
            return rooms;
        }

        /// <summary>
        ///  Update the current Player INFO
        /// </summary>

        public static void UpdatePlayer(Color colorName)
        {
            var currPlayer= new Player(UserName);

            currPlayer.PlayerColor = colorName;

            CurrentPlayer = currPlayer;
        }

        /// <summary>
        ///  Update the current Room INFO
        /// </summary>

        public static Room UpdateRoom(List<string> data)
        {
            
            var currroom = new Room(data[0],new Player(data[1]));
         

            return currroom;
        }


    }

    public class Player
    {
        public string Name { get;}
        public Color PlayerColor { get; set; }

      public  Player(string name)
      {
            Name = name;

      }       
    }


    public class Room
    {
        public string Name { get; set; }
        public Player Host { get; set; }

        public Player challenger { get; set; }
        public Player[] inspectors { get; set; }
        public  bool occupied = false;

        public Room(string name ,Player host)
        {
            Name = name;
            Host = host;
        }

    }
   public enum Flag
    {
     sendLoginInfo = 100,
     getPlayers = 210,
     getRooms = 220,
     createRoom = 310,
     joinRoom = 320,
     asktoplay = 400,
     waittopaly = 405,
     SendMove = 410,
     updateBoard = 420,
     gameResult =500,
     playAgain =600,
     leaveRoom = 650,
     disconnect = 700

    }
}
