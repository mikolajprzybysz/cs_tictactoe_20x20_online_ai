using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GameClient
{
    class TicTacToe
    {
        private int boardSize = 20;
        public static int[,] board; //1 - opponent  2 - ai
        public static int glebokosc = 3;
        public static int INFINITY = 100000;
        public int x, y;
        private int tmpX, tmpY;
        private bool first = true;

        public TicTacToe()
        {
            board = new int[boardSize, boardSize]; //1 - opponent  2 - ai
            for (int i = 0; i < boardSize; i++)
                for (int j = 0; j < boardSize; j++)
                    board[i, j] = 0;
            randomMove();
        }

        private void randomMove()
        {
            Random r = new Random();
            x = r.Next(1, 20);
            y = r.Next(1, 20);
        }

        private bool goDownLeft()
        {
            tmpX = x-1;
            tmpY = y+1;
            if (board[tmpX, tmpY] == 0)
            {
                x--;
                y++;
                return true;
            }
            else
                return false;
        }
        private bool goDownRight()
        {
            tmpX = x+1;
            tmpY = y+1;
            if (board[tmpX, tmpY] == 0)
            {
                x++;
                y++;
                return true;
            }
            else
                return false;
        }
        private bool goDown()
        {
            tmpX = x;
            tmpY = y+1;
            if (board[tmpX, tmpY] == 0)
            {
                y++;
                return true;
            }
            else
                return false;
        }
        private bool goRight()
        {
            tmpX = x+1;
            tmpY = y;
            if (board[tmpX, tmpY] == 0)
            {
                x++;
                return true;
            }
            else
                return false;
        }
        private bool goLeft()
        {
            tmpX = x-1;
            tmpY = y;
            if (board[tmpX, tmpY] == 0)
            {
                x--;
                return true;
            }
            else
                return false;
        }
        private bool goUpLeft()
        {
            tmpX = x-1;
            tmpY = y-1;
            if (board[tmpX, tmpY] == 0)
            {
                x--;
                y--;
                return true;
            }
            else
                return false;
        }
        private bool goUpRight()
        {
            tmpX = x+1;
            tmpY = y-1;
            if (board[tmpX, tmpY] == 0)
            {
                x++;
                y--;
                return true;
            }
            else
                return false;
        }
        private bool goUp()
        {
            tmpX = x;
            tmpY = y-1;
            if (board[tmpX, tmpY] == 0)
            {
                y--;
                return true;
            }
            else
                return false;
        }

        public void move(int i, int j, int piece)
        {
            board[i - 1, j - 1] = piece;
        }

        public void nextMove()
        {
            if (first)
            {
                randomMove();
                first = false;
            }
            else
            {
                if (x > 4 && y < 17)
                {
                    if (goDownLeft()) return;
                }
                else if (x < 17 && y < 17)
                {
                    if (goDownRight()) return;
                }
                else if (y < 17)
                {
                    if (goDown()) return;
                }
                else if (x < 17)
                {
                    if (goRight()) return;
                }
                else if (y > 5)
                {
                    if (goLeft()) return;
                }
                else if (x > 4 && y > 4)
                {
                    if (goUpLeft()) return;
                }
                else if (x < 17 && y > 4)
                {
                    if (goUpRight()) return;
                }
                else if (y > 4)
                {
                    if (goUp()) return;
                }
                else
                    randomMove();
            }
        }

        public string printBoard()
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                    str.Append(board[i, j]);
                str.AppendLine();
            }
            return str.ToString();
        }
    }
}
