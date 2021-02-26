using System;
using System.Threading;								// Sleeping
using System.Net;									// Used to local machine info
using System.Net.Sockets;		// Socket namespace
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using NLog;
using System.Diagnostics;				
delegate void AddMessage(string sNewMessage);

namespace GameServer {
    /// <summary>
	/// Main class from which all objects are created
	/// </summary>
	
    public class GameServerUtil {

        #region Variables, constants etc.
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public Boolean isStop = false; // zmienić na private i dodać get set
        public Int16 isStopMonitor = new Int16();
        private ListBox logListBox = null;
        private Hashtable messageHandlers;
        private int port = 0;
        private int min_players = 0;
        private string championship_id = "";
        private bool isChampionship = false;
        private Boolean isChampionshipStarted = false;
        private bool [,] table;
        /// <summary>
        /// Key: SocketClient
        /// Value: gameType
        /// </summary>
        private System.Collections.Generic.Dictionary<SocketClient, string> m_aryClients = new Dictionary<SocketClient, string>();
        
        /// <summary>
        /// Key: GameMaster ID
        /// Value: SocketGameMaster
        /// </summary>
        private Dictionary<string, SocketGameMaster> m_aryGameMasters = new Dictionary<string, SocketGameMaster>();
        
        private event AddMessage m_AddMessage;
        
        #endregion Variables, constants etc.

        #region Constructors
        
        public GameServerUtil(ListBox logListBox) {
            this.logListBox = logListBox;
            m_AddMessage = new AddMessage(OnLogMessage);
      
        }
        public GameServerUtil(ListBox logListBox, int port) {
            this.logListBox = logListBox;
            m_AddMessage = new AddMessage(OnLogMessage);
            this.port = port;

        }
        public GameServerUtil(ListBox logListBox,string championship_id, int min_players, int port) {
            this.isChampionship = true;
            this.logListBox = logListBox;
            m_AddMessage = new AddMessage(OnLogMessage);
            this.championship_id = championship_id;
            this.port = port;
            this.min_players = min_players;

        }

        #endregion Constructors
       
        #region Logging
        private void serverLogging(string[] logMessage) {
            foreach(string str in logMessage){
                if (m_AddMessage != null) logListBox.Invoke(m_AddMessage, str );
                logger.Info(str);
            }

        }
        private void serverLogging(string logMessage) {
                if (m_AddMessage != null) logListBox.Invoke(m_AddMessage, logMessage);
                logger.Info(logMessage);
        }

        public void OnLogMessage(string sMessage) {
            if (logListBox != null) {
                logListBox.Items.Add(sMessage);
            }
        }
        #endregion Logging

        #region Server starting part
        /// <summary>
		/// Application starts here. Create an instance of this class and use it
		/// as the main object.
		/// </summary>
		/// <param name="args"></param>
		public void start()
		{
             messageHandlers = new Hashtable();
          
			
            serverLogging(new string[] { "*** Server Started " + DateTime.Now.ToString("G")+ " ***" });
		
			//const int nPortListen = 399;
			IPAddress [] aryLocalAddr = null;
			String strHostName = "";
			try
			{
				strHostName = Dns.GetHostName();
				IPHostEntry ipEntry = Dns.GetHostByName( strHostName );
				aryLocalAddr = ipEntry.AddressList;
			}
			catch( Exception ex )
			{
				serverLogging ("Error trying to get local address "+ ex.Message );
			}
	
			// Verify we got an IP address. Tell the user if we did
			if( aryLocalAddr == null || aryLocalAddr.Length < 1 )
			{
				serverLogging( "Unable to get local address" );
				return;
			}
			serverLogging( "Listening on : ["+ strHostName+ "] "+ aryLocalAddr[0].ToString()+" :"+port.ToString() );

			// Create the listener socket in this machines IP address
			Socket listener = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			listener.Bind( new IPEndPoint( aryLocalAddr[0], port ) );
			//listener.Bind( new IPEndPoint( IPAddress.Loopback, 399 ) );	// For use with localhost 127.0.0.1
			listener.Listen( 10 );

			// Setup a callback to be notified of connection requests
			listener.BeginAccept( new AsyncCallback( this.OnConnectRequest ), listener );
			// Clean up before we go home
            //Monitor.Enter(isStop);
            while (!isStop) { }
            byte[] arrByte = Encoding.ASCII.GetBytes(ProtocolParser.MessageBuilder.msgShutdown());
            foreach (KeyValuePair<string,SocketGameMaster> gm in m_aryGameMasters)
            {
                
               logger.Info("Server send shutdown message to Game Master id: " + gm.Value.gameMaster.id);
                gm.Value.getSocket.Send(arrByte);
            }
            
            foreach (KeyValuePair<SocketClient, string> pl in m_aryClients)
            {
                logger.Info("Server send shutdown message to Client nick: " + pl.Key.getPlayer.getNick());
                pl.Key.getSocket.Send(arrByte);
            }

            if (listener.Connected)
            {
                listener.Shutdown(SocketShutdown.Both);
                Thread.Sleep(10);
                listener.Close();
            }
			GC.Collect();
			GC.WaitForPendingFinalizers();
        }
        #endregion Server starting part

     

        #region Connection Callbacks

        /// <summary>
		/// Callback used when a client requests a connection. 
		/// Accpet the connection, adding it to our list and setup to 
		/// accept more connections.
		/// </summary>
		/// <param name="ar"></param>
		public void OnConnectRequest( IAsyncResult ar )
		{
			Socket listener = (Socket)ar.AsyncState;
            if (!isStop) {
                NewConnection(listener.EndAccept(ar));
                listener.BeginAccept(new AsyncCallback(OnConnectRequest), listener);
            }
            
		}

		public void NewConnection( Socket sockClient )
		{
			// Program blocks on Accept() until a client connects.
			SocketBase client = new SocketBase( sockClient );
            serverLogging("Client "+ client.getSocket.RemoteEndPoint +" joined");
			client.SetupRecieveCallback( this );
		}
   
        /// <summary>
        /// Method checking correctness of logins
        /// </summary>
        /// <param name="str"> String specifing type of login</param>
        /// <param name="msg"> Message object containing login</param>
        /// <returns>true if login is correct (uniqe) and false otherwise </returns>
        public bool checkLogin(string str , message msg){
            if (str.CompareTo("GameMaster") == 0) {
                string id = msg.gameMasterLogin.id;
                lock (m_aryGameMasters) {
                    return !m_aryGameMasters.Any(gm => gm.Key.CompareTo(id) == 0);
                }
            } else if (str.CompareTo("Player") == 0) {
                string nick = msg.playerLogin.nick;
                lock (m_aryClients) {
                    return !m_aryClients.Any(cl => cl.Key.getPlayer.getNick().CompareTo(nick) == 0);
                }
            }
            return false;   
        }

        /// <summary>
        /// Method used to find free game master
        /// </summary>
        /// <param name="gameType">Type of game of game master you are looking for</param>
        /// <returns></returns>
        private SocketGameMaster getFreeGM(string gameType) {
            lock (m_aryGameMasters) {
                IEnumerable<KeyValuePair<string, SocketGameMaster>> freeGM =
                                m_aryGameMasters.Where(
                                (gm, index) => gm.Value.gameMaster.gameType.CompareTo(gameType) == 0
                                             && (gm.Value.gameMaster.noOfPlayers < gm.Value.gameMaster.playersMax)
                    //&& index == 0
                                             );

                IEnumerator<KeyValuePair<string, SocketGameMaster>> itr = freeGM.GetEnumerator();
                if (freeGM.ToArray().Length == 0) return null;
                SocketGameMaster gameMaster = null;

                while (itr.MoveNext()) {
                    gameMaster = itr.Current.Value;
                    if (gameMaster != null) return gameMaster;
                }
            }
            return null;
        }

        /// <summary>
        /// Method used to find free player
        /// </summary>
        /// <param name="gameType">Type of game of player you are looking for</param>
        /// <returns></returns>
        private SocketClient getFreePlayer(string gameType,SocketClient sck) {
            lock (m_aryClients) {
                IEnumerable<KeyValuePair<SocketClient, string>> notAssignPlayers =
                            m_aryClients.Where(
                                               (pl, index) => !pl.Key.getPlayer.isPlaying
                                               && ((string)pl.Value).CompareTo(gameType) == 0
                                               && pl.Key != sck
                    //&&index==0
                                               );
                if (notAssignPlayers.ToArray().Length == 0) return null;
                SocketClient sckc = null;
                IEnumerator<KeyValuePair<SocketClient, string>> itr = notAssignPlayers.GetEnumerator();
                while (itr.MoveNext()) {
                    sckc = itr.Current.Key;
                    if (sckc != null) return sckc;
                }
            }
            return null;
        }

        private byte[] prepareBeginGame(SocketGameMaster gameM, SocketClient player1, SocketClient player2) {
            
                string gid = "";
                lock (m_aryGameMasters) {
                    gameM.gameMaster.noOfPlayers = gameM.gameMaster.noOfPlayers + 2;
                    gid = generateGameID();
                    gameM.gameMaster.gameIds.Add(gid);
                }
                lock (m_aryClients) {
                    player1.getPlayer.isPlaying = true;
                    player1.getPlayer.setGameId(gid);
                    player2.getPlayer.isPlaying = true;
                    player2.getPlayer.setGameId(gid);
                }
                byte[] arrByte1 = Encoding.ASCII.GetBytes(ProtocolParser.MessageBuilder.msgBeginGame(
                    new string[] { player2.getPlayer.getNick(), player1.getPlayer.getNick() }, gid));

                serverLogging("Message beginGame with players nicks: [1] "
                        + player2.getPlayer.getNick()
                        + "; [2] "
                        + player1.getPlayer.getNick()
                        + "; sent to Game Master id: "
                        + gameM.gameMaster.id
                        + " with game id: "
                        + gid.ToString() + "\n" + Encoding.ASCII.GetString(arrByte1));
                return arrByte1;
            
        }

        public void OnRecievedData( IAsyncResult ar ) {

            #region Preparing message to handle
            message p = null;
            string recvMessage = "";
            
            byte[] aryRet=null;
                SocketBase client = (SocketBase) ar.AsyncState;
                aryRet = client.GetRecievedData(ar);
                

                // If no data was recieved then the connection is probably dead
                if (aryRet.Length < 1) {
                    if (client.getSocket.Connected) {
                        serverLogging("Client " + client.getSocket.RemoteEndPoint + " disconnected");
                        client.getSocket.Close();
                    }
                    if(client is SocketGameMaster)
                        lock (m_aryGameMasters) {
                            m_aryGameMasters.Remove(((SocketGameMaster)client).gameMaster.id);
                        }
                    else if(client is SocketClient)
                        lock (m_aryClients) {
                            m_aryClients.Remove((SocketClient)client);
                        }
                    return;
                }

                recvMessage = Encoding.ASCII.GetString(aryRet, 0, aryRet.Length);

                do {
                int len = (recvMessage.IndexOf("</message>") + "</message>".Length);
                if (len == 9)
                    len = (recvMessage.IndexOf("/>") + "/>".Length);

                String str = recvMessage.Substring(0, len);
                recvMessage = recvMessage.Remove(0, len);


                ProtocolParser.MessageProcessor msgProc = new ProtocolParser.MessageProcessor(messageHandlers);
                 p = XmlParser.parseXml(str);
                
                if (p == null) {
                    client.SetupRecieveCallback(this);
                    return;
                }
                
                msgProc.handle(p);
                serverLogging(str);
            #endregion Preparing message to handle

            #region Server Logic
            switch (p.type) {
           
                    #region case playerLogin
            case "playerLogin":
                SocketClient cl = new SocketClient(((SocketBase) ar.AsyncState).getSocket);
                //SocketGameMaster sck = new SocketGameMaster(  ((SocketBase)ar.AsyncState).getSocket);

                #region playerLogin message definition
                /*
		        <message type="playerLogin">
		        <playerLogin nick="[string]" gameType="[string]"/>
		        </message>
		        */
                #endregion

                if (isChampionshipStarted) {
                    int errorId = 3;
                    byte[] arrByte = Encoding.ASCII.GetBytes(ProtocolParser.MessageBuilder.msgloginResponse("no", errorId).ToCharArray());
                    serverLogging(Convert.ToString(arrByte));
                    cl.getSocket.Send(arrByte);
                    cl.SetupRecieveCallback(this);
                }

                if (checkLogin("Player",p)==true) {
                    #region loginResponse message definiton
                    //send loginResponse
                    /*
			        <message type="loginResponse">
			        <response accept="yes"/>
			        </message>
			        */
                    #endregion
                    Player player = new Player(p.playerLogin.nick);
                    cl.setPlayer(player);

                    lock (m_aryClients) {
                        m_aryClients.Add(cl, p.playerLogin.gameType);
                    }

                    byte[] arrByte = Encoding.ASCII.GetBytes(ProtocolParser.MessageBuilder.msgloginResponse("yes").ToCharArray());
                    serverLogging(System.Text.ASCIIEncoding.ASCII.GetString(arrByte));
                    cl.getSocket.Send(arrByte);
                    cl.SetupRecieveCallback(this);
                    if (!isChampionship) {
                        #region Combining clients in pairs


                        SocketGameMaster gameMaster = getFreeGM(p.playerLogin.gameType);

                        SocketClient sckc = getFreePlayer(p.playerLogin.gameType, cl);

                        if (sckc != null && gameMaster != null) {
                            gameMaster.getSocket.Send(prepareBeginGame(gameMaster, sckc, cl));
                            break;
                        }

                        break;

                        #endregion Combining clients in pairs
                    } else {
                        lock (m_aryClients) {
                            if (m_aryClients.ToArray().Length < min_players) break;
                        }
                        makeTable();
                        initChampionship();
                        isChampionshipStarted = true;

                    }
                    
		        }else {
                    #region loginResponse message definiton
                    //send loginResponse
                    /*
                    Error ids:
                    1 - wrong nick
                    2 - improper game type
                    3 - players pool overflow
    			
                    <message type="loginResponse">
                    <response accept="no"/>
                    <error id="[int]"/>
                    </message>
                    */
                    #endregion
                    int errorId = 1;
                    byte[] arrByte = Encoding.ASCII.GetBytes(ProtocolParser.MessageBuilder.msgloginResponse("no",errorId).ToCharArray());
                    serverLogging(Convert.ToString(arrByte));
                    cl.getSocket.Send(arrByte);
                    cl.SetupRecieveCallback(this);

		        }
	    break;
            #endregion playerLogin
           
                    #region case gameMasterLogin
            case "gameMasterLogin":
     
                #region gameMasterLogin message definiton
        /*
		    <message type="gameMasterLogin">
		    <gameMasterLogin id="[string]" gameType="[string]" playersMin="[int>1]" playersMax="[int>=playersMin]" />
		    </message>
		    */
      
                #endregion
       
        if (checkLogin("GameMaster",p)==true) {
            #region loginResponse message definiton
            //send loginResponse
            /*
            <message type="loginResponse">
            <response accept="yes"/>
            </message>
            */
            #endregion
            byte[] arrByte;
            SocketGameMaster sck = new SocketGameMaster(  ((SocketBase)ar.AsyncState).getSocket);
            
            if (isChampionshipStarted) {
                int errorId = 3;
                this.serverLogging("Game Master id: " + p.gameMasterLogin.id + " rejected");
                arrByte = Encoding.ASCII.GetBytes(ProtocolParser.MessageBuilder.msgloginResponse("no", errorId).ToCharArray());
                sck.getSocket.Send(arrByte);
                sck.SetupRecieveCallback(this);
            }

                sck.gameMaster = new GameMasterInfo(p.gameMasterLogin.id,p.gameMasterLogin.gameType,p.gameMasterLogin.playersMin,p.gameMasterLogin.playersMax);
                lock (m_aryGameMasters) {
                    m_aryGameMasters.Add(p.gameMasterLogin.id, sck);
                }
                this.serverLogging("Add new Game Master id: "+sck.gameMaster.id);
                 arrByte = Encoding.ASCII.GetBytes(ProtocolParser.MessageBuilder.msgloginResponse("yes").ToCharArray());
                sck.getSocket.Send(arrByte);
                sck.SetupRecieveCallback(this);

                if (!isChampionship) {
                    #region Combining clients in pairs

                    IEnumerable<KeyValuePair<SocketClient, string>> notAssignPlayers = null;

                    lock (m_aryClients) {
                        while ((notAssignPlayers = m_aryClients.Where(
                                               (pl, index) => !pl.Key.getPlayer.isPlaying
                                               && ((string)pl.Value).CompareTo(p.gameMasterLogin.gameType) == 0)).ToArray().Length >= 2) {
                            IEnumerable<KeyValuePair<SocketClient, string>> playerPair = notAssignPlayers.Where((pl, index) =>
                                                                                                          index == 0
                                                                                                          || index == 1);
                            string[] nicks = new string[2];

                            Debug.Assert(playerPair.ToArray().Length == 2);
                            string gid = generateGameID();
                            int i = 0;
                            foreach (KeyValuePair<SocketClient, string> kvp in playerPair) {
                                nicks[i] = kvp.Key.getPlayer.getNick();
                                kvp.Key.getPlayer.isPlaying = true;
                                kvp.Key.getPlayer.setGameId(gid);
                                i++;
                            }

                            lock (m_aryGameMasters) {
                                sck.gameMaster.noOfPlayers += 2;
                                sck.gameMaster.gameIds.Add(gid);
                            }

                            byte[] arrByte1 = Encoding.ASCII.GetBytes(ProtocolParser.MessageBuilder.msgBeginGame(new string[] { nicks[0], nicks[1] }, gid));
                            serverLogging("Message beginGame with players nicks: [1] "
                                            + nicks[0]
                                            + "; [2] "
                                            + nicks[1]
                                            + "; sent to Game Master id: "
                                            + sck.gameMaster.id
                                            + " with game id: "
                                            + gid.ToString() + "\n" + Encoding.ASCII.GetString(arrByte1));
                            sck.getSocket.Send(arrByte1);
                        }
                    }

                    #endregion Combining clients in pairs
                } else {
                    lock (m_aryClients) {
                        if (m_aryClients.ToArray().Length < min_players) break;
                    }
                    initChampionship();
                    isChampionshipStarted = true;
                }

		    }else {
            #region loginResponse message definiton
            //send loginResponse
            /*
            Error ids:
            1 - wrong nick
            2 - improper game type
            3 - players pool overflow
    			
            <message type="loginResponse">
            <response accept="no"/>
            <error id="[int]"/>
            </message>
            */
            #endregion
            SocketGameMaster sck =  (SocketGameMaster)ar.AsyncState;
                int errorId = 1;
                 this.serverLogging("Game Master id: "+ p.gameMasterLogin.id+ " rejected");
                 byte[] arrByte = Encoding.ASCII.GetBytes(ProtocolParser.MessageBuilder.msgloginResponse("no",errorId).ToCharArray());
                sck.getSocket.Send(arrByte);
                sck.SetupRecieveCallback(this);
            }
	    break;
        #endregion case gameMasterLogin

                    #region case gameState
            case "gameState":
        #region gameState message definiton
        /*
		    <message type="gameState">
		    <gameId id="[string]"/>
		    <!-- one tag of the two below appears in message -->
		    <nextPlayer nick="[string]"/>
		    <gameOver>
		    <!-- this tag appears repeatedly for all the players -->
		    <player nick="[string]" result="loser/winner"/>
		    </gameOver>
		    <!-- this tag will always appear. Not read by the server.-->
		    <gameState>
		    <!--this tag contains game-specific information in XML format-->
		    </gameState>
		    </message>
		    */
        #endregion
        //resend to all clients in the game (found by gameId)
                        //TODO: dodać obsługę jak gameover czy aby napewno ?

        IEnumerable<KeyValuePair<SocketClient, string>> playersInTheGame;
        lock (m_aryClients) {
            playersInTheGame = m_aryClients.Where(pl => pl.Key.getPlayer.getGameId()!=null && pl.Key.getPlayer.getGameId().CompareTo(p.gameId.id) == 0);
        }

        foreach (KeyValuePair<SocketClient, string> kvp in playersInTheGame) {
            kvp.Key.getSocket.Send(aryRet);
            kvp.Key.SetupRecieveCallback(this);
        }
                    
    		
	    break;
        #endregion case gameState

                    #region case move
            case "move":
        #region move message definiton
        /*
		    <message type="move">
		    <gameId id="[string]"/>
		    <!-- not read by server -->
		    <move>
		    <!--
		    this tag contains rule-specific information in XML format
		    -->
		    </move>
		    </message>
		    */
        #endregion
        //send to proper GameMaster specified by gameId
        //line used to secure from sending move when player is not playing
        if (!(cl = (SocketClient)ar.AsyncState).getPlayer.isPlaying) break;
        IEnumerable<KeyValuePair<string, SocketGameMaster>> responsibleGameMaster = null;
        
        lock (m_aryGameMasters) {
            responsibleGameMaster = m_aryGameMasters.Where(gm => gm.Value.gameMaster.gameIds.Contains(p.gameId.id));
        }

        if (responsibleGameMaster.ToArray().Length > 0) {
            Debug.Assert(responsibleGameMaster.ToArray().Length == 1);
            foreach (KeyValuePair<string, SocketGameMaster> kvp in responsibleGameMaster) {
             //   serverLogging(Encoding.ASCII.GetString(aryRet));
                kvp.Value.getSocket.Send(aryRet);
                kvp.Value.SetupRecieveCallback(this);
            }

        }
    		
	    break;
            #endregion case move

                    #region case thank you
            case "thank you":
         #region thank you message definition
                    //<!--thank you message sent by all players to
                    //    the server after a game has finished. It
                    //    may happen that after a game a players 
                    //    sends error instead of this message.-->
                    //<message type="thank you">
                    //    <gameId id="[string]"/>
                    //</message>
        #endregion thank you message definition
                    SocketClient cl2 = (SocketClient)ar.AsyncState;
                    cl2.getPlayer.isPlaying = false;
                    cl2.getPlayer.setGameId("");
                    responsibleGameMaster = null;
                    lock (m_aryGameMasters) {
                        responsibleGameMaster = m_aryGameMasters.Where(gm => gm.Value.gameMaster.gameIds.Contains(p.gameId.id));

                        if (responsibleGameMaster.ToArray().Length == 1) {
                            SocketGameMaster sgm = ((KeyValuePair<string, SocketGameMaster>)responsibleGameMaster.ToArray().GetValue(0)).Value;
                            if (responsibleGameMaster.ToArray().Length != 0) {
                                sgm.gameMaster.gameIds.Remove(p.gameId.id);
                                //minus 2 tutaj bo zakładam że jeśli dostałem jedno thank you tzn że drugi player też już nie gra
                                sgm.gameMaster.noOfPlayers -= 2;
                            }
                        }
                    }
                    if (!isChampionship) {
                        #region Combaining in pairs
                        SocketClient sckClient = getFreePlayer(m_aryClients[cl2], cl2);

                        SocketGameMaster gameM = getFreeGM(m_aryClients[cl2]);

                        if (sckClient != null && gameM != null) {
                            gameM.getSocket.Send(prepareBeginGame(gameM, sckClient, cl2));
                            gameM.SetupRecieveCallback(this);
                            break;
                        }

                        #endregion Combaining in pairs
                    } else {
                        if (checkForGames() > 0) {
                            cl2.SetupRecieveCallback(this);
                            initChampionship();
                        } else {
                            //end of championship
                            //send championshiplist
                            MessageBox.Show("End of championship");
                        }
                    }

                    break;
            #endregion case thank you

                    #region case leaveGame
            case "leaveGame":
        #region leaveGame message definiton
        /*
		    <!--"leaving game" request message sent by player to server-->
		    <message type="leaveGame">
		    <gameId id="[string]"/>
		    </message>
		    */
        #endregion
        //send playerLeftGame to Game Master
                cl2 = (SocketClient) ar.AsyncState;
                string nick = cl2.getPlayer.getNick();
                responsibleGameMaster = m_aryGameMasters.Where(gm => gm.Value.gameMaster.gameIds.Contains(p.gameId.id));

                Debug.Assert(responsibleGameMaster.ToArray().Length==1);
                foreach (KeyValuePair<string, SocketGameMaster> kvp in responsibleGameMaster) {
                    lock (m_aryGameMasters) {
                        kvp.Value.gameMaster.gameIds.Remove(p.gameId.id);
                    }
                    byte[] arrByte = Encoding.ASCII.GetBytes(ProtocolParser.MessageBuilder.msgPlayerLeftGame(nick,p.gameId.id));
                    lock (m_aryClients) {
                        cl2.getPlayer.isPlaying = false;
                    }

                    if (isChampionship) {
                        //update table
                    }

                    kvp.Value.getSocket.Send(arrByte);
                    kvp.Value.SetupRecieveCallback(this);
                }

                                #region playerLeftGame message definiton
                /*
		    <message type="playerLeftGame">
		    <player nick="[string]"/>
		    <gameId id="[string]"/>
		    </message>
		    */
                #endregion
                break;
            #endregion case leaveGame

                    #region case logout
            case "logout":
        SocketClient cl1 = (SocketClient)ar.AsyncState;
        #region logout message definiton
        //<message type="logout"/>
        #endregion

        lock (m_aryClients) {
            cl1.getPlayer.isPlaying = false;
            if (cl1.getSocket.Connected) {
                cl1.getSocket.Shutdown(SocketShutdown.Both);
                System.Threading.Thread.Sleep(10);
                cl1.getSocket.Close();
            }
            if (isChampionship) {
                //update table
            }
            m_aryClients.Remove(cl1);
        }
	    break;
            #endregion case logout

                    #region case error
            case "error":
        #region error message definiton
        /*
		    <message type="error">
		    [String with error message]
		    </message>
		    Error handling routine
		    */
        #endregion
        break;
            #endregion case error
            }
            
            }while(recvMessage.Length>0);
            #endregion Server Logic
        }
        /// <summary>
        /// printing table of played games in championship
        /// </summary>
        private void printTable() {
            lock (table) {
                serverLogging("  | 1 | 2 | 3 | 4 ");
                string str = "";
                for (int i = 0; i < min_players; i++) {
                    str += i.ToString() + " | ";
                    for (int j = 0; j < min_players; j++) {
                        str += table[j, i] ? "X" : " ";
                        str += " | ";
                    }
                    serverLogging(str);
                    str = "";
                }
            }
        }
        /// <summary>
        /// Method which combines in pairs players during championship. Each one plays with each.
        /// </summary>
        private void initChampionship() {
            SocketClient sck = null;
            SocketGameMaster gm = null;
            int index1 = -1;
            int index2 = -1;
            //lock(m_aryClients){
                foreach (KeyValuePair<SocketClient, string> kvp in m_aryClients) {
                    if (kvp.Key.getPlayer.isPlaying) continue;
                    printTable();
                        do {
                            sck = getFreePlayer(m_aryClients[kvp.Key], kvp.Key);
                            if (sck == null) return;
                            index1 = m_aryClients.ToList().IndexOf(kvp);
                            index2 = m_aryClients.ToList().IndexOf(new KeyValuePair<SocketClient, string>(sck, m_aryClients[sck]));
                        } while (table[index1, index2]);
                        
                        lock (table) {
                        table[index1, index2] = true;
                        table[index2, index1] = true;
                        gm=getFreeGM(m_aryClients[kvp.Key]);
                        
                        if (gm == null) {
                            table[index1, index2] = false;
                            table[index2, index1] = false;
                            return;
                        }
                        
                        Byte [] arr =prepareBeginGame(gm, kvp.Key, sck);
                        serverLogging(Encoding.ASCII.GetString(arr));
                        gm.getSocket.Send(arr);
                        gm.SetupRecieveCallback(this);
                    }
               // }
            }
        }

        
        /// <summary>
        /// initialize table of championship to false
        /// </summary>
        private void makeTable() {
            table = new bool[min_players,min_players];
            for (int i = 0; i < min_players;i++ ) {
                for (int j = 0; j < min_players; j++) {
                    table[i, j] = false;
                }
            }
        }

        /// <summary>
        /// chceck if there are games to play in championship
        /// </summary>
        /// <returns></returns>
        private int checkForGames() {
            int counter = 0;
            for (int i = 0; i < min_players; i++) {
                for (int j = 0; j < min_players; j++) {
                   if(table[i, j]==false) counter++;
                }
            }
            return counter-min_players;// bo nie liczy się przekątnej w tablicy
        }

        private string generateGameID() {
            
            return Guid.NewGuid().ToString();
        }
        #endregion Connection Callbacks


    }
    #region Sockets classes

    internal class SocketBase {
        private Socket m_sock;						// Connection to the client
        private byte[] m_byBuff = new byte[1024];		// Receive data buffer

        public SocketBase() {
        }

        public SocketBase( Socket sock )
		{
			m_sock = sock;
		}

		// Readonly access
		public Socket getSocket
		{
			get{ return m_sock; }
		}
       
        /// <summary>
        /// Setup the callback for recieved data and loss of conneciton
        /// </summary>
        /// <param name="app"></param>
        public void SetupRecieveCallback(GameServerUtil app) {
            try {
                AsyncCallback recieveData = new AsyncCallback(app.OnRecievedData);
                //m_sock.BeginReceive( m_byBuff, 0, m_byBuff.Length, SocketFlags.None, recieveData, this );
                m_sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None, recieveData, this);

            } catch (Exception ex) {
                Console.WriteLine("Recieve callback setup failed! {0}", ex.Message);
            }
        }
        
        /// <summary>
        /// Data has been recieved so we shall put it in an array and
        /// return it.
        /// </summary>
        /// <param name="ar"></param>
        /// <returns>Array of bytes containing the received data</returns>
        public byte[] GetRecievedData(IAsyncResult ar) {
            int nBytesRec = 0;
            try {
                nBytesRec = m_sock.EndReceive(ar);
            } catch { }
            byte[] byReturn = new byte[nBytesRec];
            Array.Copy(m_byBuff, byReturn, nBytesRec);

            /*
            // Check for any remaining data and display it
            // This will improve performance for large packets 
            // but adds nothing to readability and is not essential
            int nToBeRead = m_sock.Available;
            if( nToBeRead > 0 )
            {
                byte [] byData = new byte[nToBeRead];
                m_sock.Receive( byData );
                // Append byData to byReturn here
            }
            */
            return byReturn;
        }
    }

    /// <summary>
	/// Class holding information and buffers for the Client socket connection
	/// </summary>
	internal class SocketClient : SocketBase
	{
        private Player player = new Player();   //Player associated with this Socket

        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sock">client socket conneciton this object represents</param>
		public SocketClient( Socket sock ): base(sock)
		{
            
		}

        public Player getPlayer {
            get { return player; }
        }
        public void setPlayer(Player pl) {
            player = pl;
        }
       
	}
    
    internal class SocketGameMaster : SocketBase{
        public GameMasterInfo gameMaster { get; set; } //Player associated with this Socket
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sock">client socket conneciton this object represents</param>
        public SocketGameMaster(Socket sock): base(sock) {
        }
    }
    
    #endregion Sockets classes
}

    


