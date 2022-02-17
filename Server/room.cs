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
    public class room
    {
        int roomID;
        string roomName;
        static int counter = 0; //to specify room ID
        //room specifications
        int rows;
        int cols;
        int[,] board;
        int playerTurn = 1;
        int gameEnded = 0;
        //room players list
        List<player> roomPlayers = new List<player>();

        //class getters and setters
        public int RoomID { get { return roomID; } set { roomID = value; } }
        public string RoomName { get { return roomName; } set { roomName = value; } }
        public int Rows { get { return rows; } set { rows = value; } }
        public int Cols { get { return cols; } set { cols = value; } }
        public int[,] Board { get { return board; } set { board = value; } }
        public int PlayerTurn { get { return playerTurn; } set { playerTurn = value; } }
        public int GameEnded { get { return gameEnded; } set { gameEnded = value; } }
        public List<player> RoomPlayers { get { return roomPlayers; } }

        public room(player roomOwner, string name, int r, int c)
        {
            counter++;
            roomID = counter;
            roomName = name;
            rows = r;
            cols = c;
            board = new int[r, c];
            roomOwner.IsPlaying = true;
            roomPlayers.Add(roomOwner);
        }


        //chech if the given player has won the game 
        public int checkWin(int Checkplayer)
        {
            //1)Vertical
            for (int row = 0; row < this.board.GetLength(0) - 3; row++)
            {
                for (int colum = 0; colum < this.board.GetLength(1); colum++)
                {
                    //check if the winner get 4 point vertically 
                    if (this.AllNumber(Checkplayer, this.board[row, colum], this.board[row + 1, colum], this.board[row + 2, colum], this.board[row + 3, colum]))
                    {
                        //if True
                        return Checkplayer;
                    }
                }
            }
            //2)Horizontal
            for (int row = 0; row < this.board.GetLength(0); row++)
            {
                for (int colum = 0; colum < this.board.GetLength(1) - 3; colum++)
                {
                    //check if the winner get 4 point Horizontal 
                    if (this.AllNumber(Checkplayer, this.board[row, colum], this.board[row, colum + 1], this.board[row, colum + 2], this.board[row, colum + 3]))
                    {
                        //if True
                        return Checkplayer;
                    }
                }
            }
            //3)top-left diagonal(\)
            for (int row = 0; row < this.board.GetLength(0) - 3; row++)
            {
                for (int colum = 0; colum < this.board.GetLength(1) - 3; colum++)
                {
                    //check if the winner get 4 point Horizontal 
                    if (this.AllNumber(Checkplayer, this.board[row, colum], this.board[row + 1, colum + 1], this.board[row + 2, colum + 2], this.board[row + 3, colum + 3]))
                    {
                        //if True
                        return Checkplayer;
                    }
                }
            }
            //4)top-right diagonal(/)
            for (int row = 0; row < this.board.GetLength(0) - 3; row++)
            {
                for (int colum = 3; colum < this.board.GetLength(1); colum++)
                {
                    //check if the winner get 4 point Horizontal 
                    if (this.AllNumber(Checkplayer, this.board[row, colum], this.board[row + 1, colum - 1], this.board[row + 2, colum - 2], this.board[row + 3, colum - 3]))
                    {
                        //if True
                        return Checkplayer;
                    }
                }
            }

            return -1;
        }
        public bool AllNumber(int tocheck, params int[] numbers)
        {
            foreach (int num in numbers)
            {
                if (num != tocheck) //check if the player get 4 point 
                {
                    return false;
                }
            }
            return true;
        }

    }
}
