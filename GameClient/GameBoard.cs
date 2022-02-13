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
        public int[,] board;
        int x;
        int y;
        //3)
        public static int turn; // on login define if Host or Challanger

        public static int playerTurn;
        //4)

        //
        string player;
        //5)

        //SolidBrush player1;
        //SolidBrush player2;

        // Server Connecting members
         public static int rows;//6
         public static int columns; //7

         public static Color HostColor;
         public static Color ChallangerColor; 
         public static Brush HostBrush;
         public static Brush ChallangerBrush;
         public static GameBoard currntGameboard;
         public static winORlose winandlose;


        public GameBoard()
        {
            InitializeComponent();
            //1)
            this.boardcolumns = new Rectangle[columns];
            //2) 6 rows  by 7 colum 
            // width , Heigth
            this.board = new int[rows, columns];//x,y
            //3)
          
            //4)
            // winner = this.winnerplayer(this.turn);
            //5) player*color
            //player1 = (SolidBrush)Brushes.Red;
            //player2 = (SolidBrush)Brushes.DarkGreen;

            HostBrush = new SolidBrush(HostColor);
            ChallangerBrush = new SolidBrush(ChallangerColor);
            currntGameboard = this;
        }

        public void repaintBord()
        {
            Graphics g = this.CreateGraphics();

            for (int i = 0; i <rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (board[i,j] == 1)
                    {
                        g.FillEllipse(HostBrush, 32 + 48 * j, 32 + 48 * i, 32, 32);
                    }
                    else if (board[i, j] == 2)
                    {
                        g.FillEllipse(ChallangerBrush, 32 + 48 * j, 32 + 48 * i, 32, 32);
                    }
                  
                }
            }
            

        }



        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.OrangeRed, 24, 24, columns * 48,  rows * 48);//

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
                    this.board[rowindex, columnIndex] = turn;  /// مهم اوي حلي بالك من السفر 
                

                    if (playerTurn == turn) //cuurnt player 
                    {
                     

                    GameManger.SendServerRequest(Flag.SendMove, rowindex.ToString(), columnIndex.ToString());
                       // MessageBox.Show(rowindex.ToString(), columnIndex.ToString());
                      //  repaintBord();


                        //switch (turn)
                        //{
                        //    case 1:
                        //        turn = 1;
                        //        break;
                        //    case 2:
                        //        turn = 1;
                        //        break;
                        //}

                    }
                    else
                    {
                        MessageBox.Show("That is not your turn please wait for the Other player Move");
                    }

                    //else if (turn == 2)
                    //{
                    //    //User two use DarkGreen Color 
                    //    //Graphics g = this.CreateGraphics();
                    //    //g.FillEllipse(ChallangerBrush, 32 + 48 * columnIndex, 32 + 48 * rowindex, 32, 32);
                    //    repaintBord();
                    //}

                    //***************Winner***********
                    //int winner = this.winnerplayer(turn);

                    //if (winner != -1)//There is a winning player
                    //{

                    //    if (winner == 1)
                    //    {
                    //        player = "red";
                    //    }
                    //    else
                    //    { player = "DarkGreen"; }
                    //    MessageBox.Show(player + "player has win");
                    //}

                    // change 1=>2 && 2=>1
                    //if (turn == 1)
                    //{
                    //    turn = 2;
                    //}
                    //else
                    //{
                    //    turn = 1;
                    //}
                }
            }
        }
        // Winner conditions:
        //private int winnerplayer(int Checkplayer)
        //{
        //    //1)Vertical
        //    for (int row = 0; row < this.board.GetLength(0) - 3; row++)
        //    {
        //        for (int colum = 0; colum < this.board.GetLength(1); colum++)
        //        {
        //            //check if the winner get 4 point vertically 
        //            if (this.AllNumber(Checkplayer, this.board[row, colum], this.board[row + 1, colum], this.board[row + 2, colum], this.board[row + 3, colum]))
        //            {
        //                //if True
        //                return Checkplayer;
        //            }
        //        }
        //    }
        //    //2)Horizontal
        //    for (int row = 0; row < this.board.GetLength(0); row++)
        //    {
        //        for (int colum = 0; colum < this.board.GetLength(1) - 3; colum++)
        //        {
        //            //check if the winner get 4 point Horizontal 
        //            if (this.AllNumber(Checkplayer, this.board[row, colum], this.board[row, colum + 1], this.board[row, colum + 2], this.board[row, colum + 3]))
        //            {
        //                //if True
        //                return Checkplayer;
        //            }
        //        }
        //    }
        //    //3)top-left diagonal(\)
        //    for (int row = 0; row < this.board.GetLength(0) - 3; row++)
        //    {
        //        for (int colum = 0; colum < this.board.GetLength(1) - 3; colum++)
        //        {
        //            //check if the winner get 4 point Horizontal 
        //            if (this.AllNumber(Checkplayer, this.board[row, colum], this.board[row + 1, colum + 1], this.board[row + 2, colum + 2], this.board[row + 3, colum + 3]))
        //            {
        //                //if True
        //                return Checkplayer;
        //            }
        //        }
        //    }
        //    //4)top-right diagonal(/)
        //    for (int row = 0; row < this.board.GetLength(0) - 3; row++)
        //    {
        //        for (int colum = 3; colum < this.board.GetLength(1); colum++)
        //        {
        //            //check if the winner get 4 point Horizontal 
        //            if (this.AllNumber(Checkplayer, this.board[row, colum], this.board[row + 1, colum - 1], this.board[row + 2, colum - 2], this.board[row + 3, colum - 3]))
        //            {
        //                //if True
        //                return Checkplayer;
        //            }
        //        }
        //    }

        //    return -1;
        //}
        ////function to check all number is checked 
        //private bool AllNumber(int tocheck, params int[] numbers)
        //{
        //    foreach (int num in numbers)
        //    {
        //        if (num != tocheck) //check if the player get 4 point 
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

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

        private void GameBoard_FormClosing(object sender, FormClosingEventArgs e)
        {
            lobby.mainlobby.Show();
        }
    }
}
