using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utils;

namespace GameMaster
{
    public partial class GameMaster : Form
    {
        private int port;
        private String ip;
        private GameMasterUtils gameMaster = new GameMasterUtils();
        private int PlayersMax = 2;
        private int PlayersMin = 2;
        private Hashtable GamePool;

        public GameMaster()
        {
            InitializeComponent();            
            GamePool = new Hashtable();            
        }
        public GameMaster(string serverIp, string serverPort)
        {            
            InitializeComponent();            
            Connect(serverIp, serverPort);
            GamePool = new Hashtable();
            Login(PlayersMin, PlayersMax);
        }
        public GameMaster(string serverIp, string serverPort, int MinPlayers, int MaxPlayers)
        {
            InitializeComponent();
            Connect(serverIp, serverPort);
            GamePool = new Hashtable();
            PlayersMin = MinPlayers;
            PlayersMax = MaxPlayers;
            Login(PlayersMin, PlayersMax);
        }
        private void Login(int pMin, int pMax)
        {
            #region Sending gameMasterLogin to server            
            string gameMasterId = Guid.NewGuid().ToString();
            String messageToBeSend = MessageFactory.gameMasterLogin(gameMasterId, "tic tac toe", PlayersMin, PlayersMax);
            // log message
            SendMessage(messageToBeSend);
            #endregion
        }
        private void Connect(string destIp, string destPort)
        {
            if (destIp != null && destPort != null)
            {
                ip = destIp;
                port = int.Parse(destPort);                
                if (gameMaster.isSocketNull() == true || gameMaster.isConnected() == false)
                {
                    port = int.Parse(destPort);
                    ip = destIp;
                    gameMaster = new GameMasterUtils(port, ip, sendListBox, receivedListBox, this);
                    
                }
                else
                {
                    MessageBox.Show("You are already connected");
                }
            }                       
            
        }
        private bool isNotConnected()
        {
            if (gameMaster.isSocketNull() == true || gameMaster.isConnected() == false)
                return true;
            return false;
        }
        public void SendMessage(string txt)
        {
            // Check we are connected
            if (isNotConnected())
            {
                MessageBox.Show(this, "Must be connected to Send a message");
                return;
            }

            // Read the message from the text box and send it
            try
            {
                gameMaster.sendMessage(txt);
                // Convert to byte array and send.
                //string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><message type=\"gameMasterLogin\"><gameMasterLogin id=\"MiKoLAj\" gameType=\"chess\" playersMin=\"1\" playersMax=\"10\"/></message>";
                //Byte[] byteDateLine = Encoding.ASCII.GetBytes(xml.ToCharArray());
                //Byte[] byteDateLine = Encoding.ASCII.GetBytes(sendTextBox.Text.ToCharArray());
                //gameMaster.send(byteDateLine, byteDateLine.Length, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Send Message Failed! {" + ex.Message + "}");
            }
        }
        private void connectButton_Click(object sender, EventArgs e)
        {
            Connect(IPTextBox.Text, portTextBox.Text);
        }
        private void sendButton_Click(object sender, EventArgs e)
        {
            SendMessage(sendTextBox.Text);   
        }      
        public void CreateGame( string gameId, messagePlayer[] players )
        {
            //if(players!=null)
            string[] nicks = new string[players.Length];
            for(int i =0; i < players.Length; i++)
                nicks[i] = players[i].nick;

            Game newGame = new Game(gameId, nicks, this);
            GamePool.Add(gameId, newGame);
        }
        // prawo | prawo dol | dol |lewo dol
        public void ValidateMove( string gameId, messageMoveTic move)
        {
            if(GamePool.ContainsKey(gameId))
            {
                ((Game)GamePool[gameId]).ValidateMove(move);
            }
        }
        private Game getGame(string gameId)
        {
            IDictionaryEnumerator i = GamePool.GetEnumerator();
            while (i.MoveNext())
            {
                if ((string)i.Key == gameId)
                {
                    return (Game)i.Value;
                }
            }
            return null;
        }
        public void StopGame(string gameId, string playerNick)
        {
            string[] players;            
            Game gameToBeStopped = getGame(gameId);
            if (gameToBeStopped != null)
            {
                players = gameToBeStopped.getPlayers();
                string[,] playersStatus = new string[players.Length, 2];
                for (int i = 0; i < players.Length; i++)
                {
                    playersStatus[i, 0] = players[i];
                    playersStatus[i, 1] = "winner";
                    if (players[i] == playerNick)
                        playersStatus[i, 1] = "loser";
                }
                messageMoveTic lastTic = gameToBeStopped.getLastTic();
                SendMessage(MessageFactory.gameState(gameId, playersStatus, lastTic.x, lastTic.y));
                GamePool.Remove(gameId);
            }
            else
            {
                SendMessage(MessageFactory.error("Game with id="+gameId+" does not exist"));
            }
        }

        #region HANDLING FUNCTIONS
        //<!--
        //an error message which may appear from both sides as a response 
        //-->
        public string error(message message)
        {
            return null;
        }
        //<!-- login request response sent by server -->
        public string loginResponse(message msg)
        {
            if (msg.response != null)
                if ("yes" == msg.response.accept)
                {
                    // announce connection have been established
                }
                else
                {
                    if (msg.error != null)
                        switch (msg.error.id)
                        {
                            case 2:
                                //announce improper game type have been set
                                break;
                        }
                }
            return null;
        }
        //<!--
        //Server message with player list sent to game initiator. As a response initiator sends gameState message with nextPlayer set to a desired first player to do a move.
        //-->       
        public string beginGame(message msg)
        {

            if (msg.gameId != null && msg.player != null && msg.player.Length == PlayersMax)
                CreateGame(msg.gameId.id, msg.player);
            //gameState'a wysyla CreateGame() jako nowy watek zajmuajcy sie nowa gra,
            return null;
        }
        //<!--
        //move message sent by player to server and by server to the game master as
        //a response to gameState message with nick pointing to that player
        //-->
        public string move(message msg)
        {
            bool ok = true;
            if (msg.gameId == null) ok = false;
            if (msg.move.tic == null) ok = false;
            if(ok)
                ValidateMove(msg.gameId.id, msg.move.tic);
            return null;
        }
        //<!--
        //by this message server and game master exchange information about player that left a game
        //-->
        public string playerLeftGame(message msg)
        {            
            if (msg.gameId == null)
            {
                SendMessage(MessageFactory.error("No gameId in last message"));
                return null;
            }
            if (msg.player == null)              
            {
                SendMessage(MessageFactory.error("No players in last message"));
                return null;
            }
            if (msg.player[0] == null)
            {
                SendMessage(MessageFactory.error("No player[0] in last message"));
                return null;
            }            
            StopGame(msg.gameId.id,msg.player[0].nick);
            return null;
        }
        //<!--
        //message sent before shutting down server to all registered players and game master
        //-->
        public string serverShutdown(message msg)
        {
            //exit();
            MessageBox.Show("Server terminated. Thank you for using this application.");
            Process.GetCurrentProcess().Kill();
            return null;
        }
        #endregion


        internal void GameFinished(string gameId)
        {
            GamePool.Remove(gameId);   
        }
    }
}
