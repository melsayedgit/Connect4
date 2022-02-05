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

namespace Client
{
    /// <summary>
    ///  Responsible for all Connect4 Game client logic 
    /// </summary>

    public static class GameManger
    {

        static string ip;
        static int port;
        static bool connStatues;
        static string UserName;

      static  IPAddress ServerIP;
      static  TcpClient server;
      static  NetworkStream ConnectionStream;
      static  BinaryReader br;
      static  BinaryWriter bwr;
      static  Thread updatingThread;

     public static List<Player> playerslist;
     public static List<Room> Rommslist;

     public static Player CurrentPlayer;
     public static Room CurrentRoom;

        static GameManger()
           
        {
            ip = "127,0,0,1";
            port = 1997;
            connStatues = false;
            ServerIP =  IPAddress.Parse(ip);
           
        }

        /// <summary>
        ///  Connect the user to the server and return and return The result of the attempt 
        /// </summary>
   
        public static bool Login(string userName)
        {
            bool isconnected = false;


            try
            {
                TcpClient server = new TcpClient();
                server.Connect(ServerIP, port);
                ConnectionStream = server.GetStream();
                br = new BinaryReader(ConnectionStream);
                bwr = new BinaryWriter(ConnectionStream);

                isconnected = true;
                connStatues = true;
                SendServerRequest(Flag.sendLoginInfo,userName);
                UserName = userName;
            }
            catch (Exception e)
            {
                connStatues = isconnected;

            }

            return isconnected;



        }
        /// <summary>
        ///  This Method used for communicating with the game server
        /// </summary>
        public static void SendServerRequest(Flag flag,params string[] data)
        {
            var f = (int)Flag.getPlayers;
            string msg = f.ToString();
            

            if (data.Length >0)
            {
                foreach (var item in data)
                {
                    msg += "," + item;

                }


            }
            bwr.Write(msg);

        }

        public static void ReceiveServerRequest()
        {
            var msg = br.ReadString();
            var msgArray = msg.Split(',');
            Flag flag = (Flag)int.Parse(msgArray[0]);
            var data = msgArray.ToList();
            data.RemoveAt(0);
            switch (flag)
            {
                case Flag.getPlayers:
                  playerslist = Getplayers(data);
                    break;
                case Flag.getRooms:
                    Rommslist = GetRooms(data);
                    break;
                case Flag.createRoom:
                    break;
                case Flag.joinRoom:
                    break;
                case Flag.SendMove:
                    break;
                case Flag.updateBoard:
                    break;
                default:
                    break;
            }


        }

        /// <summary>
        ///  Get all the players on the server and return them as an List of Players
        /// </summary>

        public static List<Player> Getplayers(List<string> data)
        {
            var players = new List<Player>();

            foreach (var item in data)
            {
                players.Add(new Player(item));
            }
            return players;
        }

        /// <summary>
        ///  Get all the players on the server and return them as an List of Players
        /// </summary>

        public static List<Room> GetRooms(List<string> data)
        {   
            var rooms = new List<Room>();


            return rooms;
        }

        /// <summary>
        ///  Update the current Player INFO
        /// </summary>

        public static void UpdatePlayer(string colorName)
        {
            var currPlayer= new Player(UserName);

            currPlayer.PlayerColor = Color.FromName(colorName);

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
     SendMove = 410,
     updateBoard =420

    }
}
