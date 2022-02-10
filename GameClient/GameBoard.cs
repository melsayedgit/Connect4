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
    public partial class GameBoard : Form
    {
        // component
        //1)
        private Rectangle[] boardcolumns;
        //2)
        private int[,] board;
        int x;
        int y;
        //3)
        private int turn;
        //4)

        //
        string player;
        //5)

        //SolidBrush player1;
        //SolidBrush player2;

        // Server Connecting members
         public static int rows;//6
         public static int columns; //7


        public GameBoard()
        {
            InitializeComponent();
            //1)
            this.boardcolumns = new Rectangle[columns];
            //2) 6 rows  by 7 colum 
            // width , Heigth
            this.board = new int[rows, columns];//x,y
            //3)
            this.turn = 1;
            //4)
            // winner = this.winnerplayer(this.turn);
            //5) player*color
            //player1 = (SolidBrush)Brushes.Red;
            //player2 = (SolidBrush)Brushes.DarkGreen;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Blue, 24, 24, columns * 48,  rows * 48);//
            for (int i = 0; i < rows; i++)//x
            {
                for (int j = 0; j < columns; j++)//y
                {
                    if (i == 0)
                    {
                        this.boardcolumns[j] = new Rectangle(32 + 48 * j, 24, 32, rows* 48);
                    }
                    e.Graphics.FillEllipse(Brushes.White, (32 + 48 * j), (32 + 48 * i), 32, 32);

                }
            }
        }

        //Event of Mouseclick
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int columnIndex = this.columNumber(e.Location);
            //validate to add
            if (columnIndex != -1)
            {
                int rowindex = this.EmptyRow(columnIndex);
                if (rowindex != -1)
                {
                    this.board[rowindex, columnIndex] = this.turn;
                    if (this.turn == 1)
                    {
                        //User One use Red Color 
                        Graphics g = this.CreateGraphics();
                        g.FillEllipse(Brushes.Red, 32 + 48 * columnIndex, 32 + 48 * rowindex, 32, 32);
                    }
                    else if (this.turn == 2)
                    {
                        //User two use DarkGreen Color 
                        Graphics g = this.CreateGraphics();
                        g.FillEllipse(Brushes.DarkGreen, 32 + 48 * columnIndex, 32 + 48 * rowindex, 32, 32);
                    }

                    //***************Winner***********
                    int winner = this.winnerplayer(this.turn);

                    if (winner != -1)//There is a winning player
                    {

                        if (winner == 1)
                        {
                            player = "red";
                        }
                        else
                        { player = "DarkGreen"; }
                        MessageBox.Show(player + "player has win");
                    }

                    // change 1=>2 && 2=>1
                    if (this.turn == 1)
                    {
                        this.turn = 2;
                    }
                    else
                    {
                        this.turn = 1;
                    }
                }
            }
        }
        // Winner conditions:
        private int winnerplayer(int Checkplayer)
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
        //function to check all number is checked 
        private bool AllNumber(int tocheck, params int[] numbers)
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

        //function 

        private int columNumber(Point mouse)
        {
            for (int i = 0; i < this.boardcolumns.Length; i++)
            {
                //check Mouse location X ,Y
                if ((mouse.X >= this.boardcolumns[i].X) && (mouse.Y >= this.boardcolumns[i].Y))
                {
                    if ((mouse.X <= this.boardcolumns[i].X + this.boardcolumns[i].Width) && (mouse.Y <= this.boardcolumns[i].Y + this.boardcolumns[i].Height))
                    {
                        return i;
                    }
                }

            }
            return -1;
        }
        // ***************fill empty row*****************
        private int EmptyRow(int col)
        {
            // check if valid to add or no
            for (int i = rows-1; i >= 0; i--) //(start add from lowest one to highest one)
            {
                if (this.board[i, col] == 0)
                {
                    return i;
                }
            }

            return -1;

        }

    }
}
