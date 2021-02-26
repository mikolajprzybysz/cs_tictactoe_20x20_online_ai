using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GameMaster
{
    public class Game
    {
        string gameId;
        string[] players; /* o-0 x-1*/
        char[,] board;
        string nextPlayer;
        string currentPlayer;
        char lastMove;
        messageMoveTic lastTic;
        GameMaster myGameMaster;

        public Game(string gameId, string[] players, GameMaster gameMaster)
        {
            this.gameId = gameId;
            this.players = new string[2];
            this.myGameMaster = gameMaster;

            int nextPlayah =0;
            Random rndm = new Random();
            nextPlayah = rndm.Next(0, 1);
            if(nextPlayah==0)
            {
                this.players[0] = players[0];
                this.players[1] = players[1];
            }
            else
            {
                this.players[0] = players[1];
                this.players[1] = players[0];
            }

            board = new char[21,21];

            currentPlayer = this.players[1];
            nextPlayer = this.players[0];
            lastMove = 'o';

            //send message
            sendGameState(null);
        }
        public void sendGameState(messageGameStateTac tac)
        {
            myGameMaster.SendMessage(MessageFactory.gameState(this.gameId, this.nextPlayer, tac));
        }
        public void sendError(string txt)
        {
            myGameMaster.SendMessage(MessageFactory.error(txt));
        }
        public void ValidateMove(messageMoveTic tic)
        {
            // error - no move;
            if (null == tic)
            {
                sendError("No move have been recieved in game '" + gameId + "' from player '" + nextPlayer + "'");
            }
            lastTic = tic;
            int moveX = tic.x;
            int moveY = tic.y;
            messageGameStateTac tac = new messageGameStateTac();
            tac.x = moveX;
            tac.y = moveY;
            // set up new current player
            string tmp = currentPlayer;            
            currentPlayer = nextPlayer;
            nextPlayer = tmp;
            
            // set up new char
            if (lastMove == 'x') 
            {
                // put o                                     
                lastMove = 'o';
            }
            else
            {
                // put x
                lastMove = 'x';
            }
            
            // check move inside the board
            if (moveX > 0 && moveX < 21 && moveY > 0 && moveY < 21)
            {
                if ('x' == board[moveX, moveY] || 'o' == board[moveX, moveY]) 
                {
                    string[,] p = new string[2, 2];

                    p[0, 0] = currentPlayer;
                    p[1, 0] = nextPlayer;

                    p[0, 1] = "loser";
                    p[1, 1] = "winner";

                    myGameMaster.SendMessage(MessageFactory.gameState(this.gameId, p, tac));
                    myGameMaster.GameFinished(gameId);
                }
                else
                {
                    board[moveX, moveY] = lastMove;
                    if (checkWinner(lastMove))
                    {
                        string[,] p = new string[2, 2];

                        p[0, 0] = currentPlayer;
                        p[1, 0] = nextPlayer;

                        p[0, 1] = "winner";
                        p[1, 1] = "loser";

                        myGameMaster.SendMessage(MessageFactory.gameState(this.gameId, p, tac));
                        myGameMaster.GameFinished(gameId);
                        
                    }
                    else if (checkFullBoard())
                    {
                        string[,] p = new string[2, 2];

                        p[0, 0] = currentPlayer;
                        p[1, 0] = nextPlayer;

                        p[0, 1] = "loser";
                        p[1, 1] = "loser";

                        myGameMaster.SendMessage(MessageFactory.gameState(this.gameId, p, tac));
                        myGameMaster.GameFinished(gameId);                       
                    }
                    else
                        sendGameState(tac);                    
                }
            }
        }
        public messageMoveTic getLastTic()
        {
            return lastTic;
        }
        public string[] getPlayers()
        {
            return players;
        }
        private bool checkFullBoard()
        {
            // looking for empty space if found return false;
            for (int i = 1; i < 21; i++)
                for (int j = 1; j < 21; j++)
                    if (board[i, j] != 'x' && board[i, j] != 'o') return false;
            return true;
        }
        private bool checkWinner(char t)
        {
            for(int i=0; i<21;i++)
            for(int j=0; j<21;j++)
                if (t==board[i, j])
                {
                    // check 5 in row to the right
                    if (FiveToTheRight(i, j, t)) return true;
                    // check 5 in row to the bottom right
                    if (FiveToTheBottomRight(i, j, t)) return true;
                    // check 5 in row to the bottom
                    if (FiveToTheBottom(i, j, t)) return true;
                    // check 5 in row to the bottom left
                    if (FiveToTheBottomLeft(i, j, t)) return true;
                }
            return false;
        }
        // x=16 y=20
        private bool FiveToTheRight(int x, int y, char t)
        {
            int i = 0;
            if (x < 17)
            {
                for (i=0; i < 5; i++)
                {
                    // t!= board[16+0, 20]
                    // t!= board[16+1, 20]
                    // t!= board[16+2, 20]
                    // t!= board[16+3, 20]
                    // t!= board[16+4, 20]
                    if(t != board[x+i, y])
                        break;
                }
                /*while ( && i < 5)
                // board[16+0,20] && 0 < 5
                // board[16+1,20] && 1 < 5
                // board[16+2,20] && 2 < 5
                // board[16+3,20] && 3 < 5
                // board[16+4,20] && 4 < 5
                // board[16+5,20] && 5 < 5
                {
                    i++;
                }
                 */
                if (i == 5) return true;
            }
            return false;
        }
        private bool FiveToTheBottomRight(int x, int y, char t)
        {
            int i = 0;
            if (x < 17 && y < 17)
            {
                for (; i < 5; i++)
                {
                    if (t != board[x + i, y+i])
                        break;
                }
                if (i == 4) return true;
            }
            return false;        
        }
        // 16 20 x
        private bool FiveToTheBottom(int x, int y, char t)
        {
            int i = 0;
            if (y < 17)
            {
                for (; i < 5; i++)
                {
                    if (t != board[x, y + i])
                        break;
                }               
                if (i == 4) return true;
            }
            return false;
        }
        private bool FiveToTheBottomLeft(int x, int y, char t)
        {
            int i = 0;
            if (x > 4 && x < 21 && y < 17 && y>0)
            {
                for (; i < 5; i++)
                {
                    if (t != board[x - i, y + i])
                        break;
                } 
                if (i == 4) return true;
            }
            return false;
        }
    }
}
