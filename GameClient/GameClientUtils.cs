using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Utils;
using NLog;
using System.Collections;
using System.Drawing;

delegate void AddMessage(string sNewMessage);

namespace GameClient
{
    class GameClientUtils
    {
        private Socket serverSock;
        private byte[] m_byBuff = new byte[1024];
        private bool first = false;
        private bool gameStarted = false;
        private bool logged = false;
        public Logger logger = LogManager.GetCurrentClassLogger();
        private Hashtable messageHandlers = new Hashtable();
        message p = null;
        private string nick;
        private string gameID;
        private RichTextBox textBox;
        private static GameClient gameClient;
        string lastPlayer = "";
        private bool canReceive = true;


        private TicTacToe game;

        public bool isSocketNull()
        {
            if (serverSock == null)
                return true;
            else
                return false;
        }

        public bool isConnected()
        {
            if (serverSock != null)
                return serverSock.Connected;
            else
                return false;
        }

        public void sendMessage(string msg)
        {
            Byte[] byteDateLine = Encoding.ASCII.GetBytes(msg.ToCharArray());
            send(byteDateLine, byteDateLine.Length, 0);
            logger.Debug(" --- Sended --- " + Environment.NewLine + msg);
            //gameClient.writeToBox(" --- Sended --- " + Environment.NewLine + msg, Color.Green);
        }

        public void SendMove()
        {
            game.nextMove();
            string s = ProtocolParser.MessageBuilder.msgMove(gameID, game.x, game.y);
            sendMessage(s);
            //game.move(game.x, game.y, mark);
        }
        public bool sendThankYou()
        {
            string s = ProtocolParser.MessageBuilder.msgThankYou(gameID);
            sendMessage(s);
            return true;
        }

        public int send(byte[] buffer, int size, SocketFlags socketFlag)
        {
            canReceive = true;
            return serverSock.Send(buffer, size, socketFlag);
        }

        // constructors
        public GameClientUtils()
        {
            serverSock = null;
        }
        public GameClientUtils(GameClient gm, int selectedPort, String selectedIP, RichTextBox rtb, string playerLogin, string gameType)
        {
            gameClient = gm;
            textBox = rtb;
            nick = playerLogin;
            try
            {
                if (serverSock != null && serverSock.Connected)
                {
                    serverSock.Shutdown(SocketShutdown.Both);
                    System.Threading.Thread.Sleep(10);
                    serverSock.Close();
                }
                serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Parse(selectedIP);
                System.Net.IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, selectedPort);

                // Connect to server non-Blocking method
                serverSock.Blocking = false;
                AsyncCallback onconnect = new AsyncCallback(OnConnect);
                serverSock.BeginConnect(remoteEndPoint, onconnect, serverSock);
                // sleep 1 second then try to send message
                System.Threading.Thread.Sleep(1000);
                if (serverSock.Connected)
                {
                    logger.Info("Connected successfuly to: " + selectedIP + ":" + selectedPort.ToString()
                    + " Your nick: " + playerLogin);
                    gameClient.writeToBox("Connected successfuly to: " + selectedIP + ":" + selectedPort.ToString() +
                        " Your nick: " + playerLogin, Color.Green);
                    string msg = ProtocolParser.MessageBuilder.msgplayerLogin(gameType, playerLogin);
                    sendMessage(msg);
                }

            }
            catch (SocketException s)
            {
                logger.Error("Error: Unable to connect to server." + Environment.NewLine + s.Message.ToString());
                logger.Trace("Error: Unable to connect to server." + Environment.NewLine + s.Message.ToString()
                    + Environment.NewLine + s.StackTrace.ToString());
                Console.Write("Error: Unable to connect to server." + Environment.NewLine + s.Message.ToString());
            }
        }
        public void OnConnect(IAsyncResult ar)
        {
            // Socket was the passed in object
            Socket sock = (Socket)ar.AsyncState;

            // Check if we were sucessfull
            try
            {
                //sock.EndConnect( ar );
                if (sock.Connected)
                    SetupReceiveCallback(sock);
                else
                {
                    logger.Error("Error: Unable to connect to remote machine", "Connect Failed!");
                    MessageBox.Show("Error: Unable to connect to remote machine", "Connect Failed!");
                }
            }
            catch (Exception e)
            {
                logger.Error("Error: Unusual error during Connect!" + Environment.NewLine + e.Message.ToString());
                logger.Trace("Error: Unusual error during Connect!" + Environment.NewLine + e.Message.ToString()
                    + Environment.NewLine + e.StackTrace.ToString());
                Console.Write("Error: Unusual error during Connect!" + Environment.NewLine + e.Message.ToString());
            }
        }
        public void Disconnect()
        {
            string s = ProtocolParser.MessageBuilder.msgLogout();
            sendMessage(s);
            System.Threading.Thread.Sleep(1000);
            if (serverSock.Connected)
            {
                serverSock.Shutdown(SocketShutdown.Both);
                System.Threading.Thread.Sleep(10);
                serverSock.Close();
            }
            logger.Info("Disconnected.");
            gameClient.writeToBox("Disconnected.", Color.Red);
        }
        public void SetupReceiveCallback(Socket sock)
        {
            try
            {
                AsyncCallback receiveData = new AsyncCallback(OnReceivedData);
                sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None, receiveData, sock);
            }
            catch (Exception e)
            {
                logger.Error("Error: Setup Receive Callback failed!" + Environment.NewLine + e.Message.ToString());
                logger.Trace("Error: Setup Receive Callback failed!" + Environment.NewLine + e.Message.ToString()
                    + Environment.NewLine + e.StackTrace.ToString());
                Console.Write("Error: Setup Receive Callback failed!" + Environment.NewLine + e.Message.ToString());
            }
        }
        public void OnReceivedData(IAsyncResult ar)
        {
            // Socket was the passed in object
            Socket sock = (Socket)ar.AsyncState;
            string recvMessage = "";
            System.Threading.Thread.Sleep(200);

            // Check if we got any data
            try
            {
                int nBytesRec = sock.EndReceive(ar);
                if (nBytesRec > 0)
                {
                    // Wrote the data to the List
                    //  string sReceived = Encoding.ASCII.GetString(m_byBuff, 0, nBytesRec);
                    recvMessage = Encoding.ASCII.GetString(m_byBuff, 0, nBytesRec);
                    do
                    {
                        int len = (recvMessage.IndexOf("</message>") + "</message>".Length);
                        if (len == 9)
                            len = (recvMessage.IndexOf("/>") + "/>".Length);
                        String sReceived = recvMessage.Substring(0, len);
                        recvMessage = recvMessage.Remove(0, len);

                        // write data to log
                        logger.Debug(" --- Received --- " + Environment.NewLine + sReceived);

                        ProtocolParser.MessageProcessor msgProc = new ProtocolParser.MessageProcessor(messageHandlers);
                        p = XmlParser.parseXml(sReceived);
                        msgProc.handle(p);

                        #region message type
                        switch (p.type)
                        {
                            case "serverShutdown":
                                #region serverShutdown
                                {
                                    if (serverSock.Connected)
                                    {
                                        serverSock.Shutdown(SocketShutdown.Both);
                                        System.Threading.Thread.Sleep(10);
                                        serverSock.Close();
                                    }
                                    logger.Info("Server shutdown.");
                                    gameClient.writeToBox("Server shutdown.", Color.Red);
                                    break;
                                }
                                #endregion
                            case "loginResponse":
                                #region loginResponse
                                {
                                    if (p.response.accept == "yes")
                                    {
                                        logger.Info("Login accepted. Waiting for game...");
                                        gameClient.writeToBox("Login accepted. Waiting for game...", Color.Green);
                                        logged = true;
                                    }
                                    else
                                    {
                                        if (p.error.id == 1)
                                        {
                                            logger.Info("Login not accepted. Disconnecting.");
                                            logger.Info("Change your nick and connect again.");
                                            gameClient.writeToBox("Login not accepted. Disconnecting.", Color.Red);
                                            gameClient.writeToBox("Change your nick and connect again.", Color.Red);
                                        }
                                        else if (p.error.id == 2)
                                        {
                                            logger.Info("Improper game type. Disconnecting.");
                                            logger.Info("Change game type and connect again.");
                                            gameClient.writeToBox("Improper game type. Disconnecting.", Color.Red);
                                            gameClient.writeToBox("Change game type and connect again.", Color.Red);
                                        }
                                        else if (p.error.id == 3)
                                        {
                                            logger.Info("Players poll overflow. Disconnecting.");
                                            logger.Info("Please try again later.");
                                            gameClient.writeToBox("Players poll overflow. Disconnecting.", Color.Red);
                                            gameClient.writeToBox("Please try again later.", Color.Red);
                                        }
                                        Disconnect();
                                    }
                                    break;
                                }
                                #endregion
                            case "gameState":
                                #region gameState
                                {
                                    #region newGame
                                    if (!gameStarted)
                                    {
                                        game = new TicTacToe();
                                        gameClient.clearBoard();
                                        gameID = p.gameId.id;
                                        if (p.nextPlayer.nick == nick)
                                        {
                                            first = true;
                                            gameClient.setPiece(2);
                                            SendMove();
                                        }
                                        else
                                            gameClient.setPiece(1);
                                        gameStarted = true;
                                        lastPlayer = p.nextPlayer.nick;
                                        gameClient.writeToBox("-- Game Started -- " + "With X's starts: " + lastPlayer, Color.Blue);
                                        break;
                                    }
                                    #endregion
                                    #region gameOver
                                    else if (p.gameOver != null)
                                    {
                                        gameClient.writeToBox(" --- Game over ---", Color.Chocolate);
                                        logger.Info(" --- Game over ---");
                                        foreach (var player in p.gameOver)
                                        {
                                            logger.Info(player.nick + ": " + player.result);
                                            gameClient.writeToBox(player.nick + ": " + player.result, Color.DarkGreen);
                                        }
                                        while (!sendThankYou())
                                            canReceive = false;
                                        gameStarted = false;
                                        first = false;
                                        //return;
                                        break;
                                    }
                                    #endregion
                                    else
                                    {
                                        int me = first ? 2 : 1;
                                        int opponent = first ? 1 : 2;
                                        //moja kolej
                                        if (p.nextPlayer.nick == nick)
                                        {
                                            // wstawiam ruch przeciwnika
                                            game.move(
                                                    p.gameState.tac.x,
                                                    p.gameState.tac.y,
                                                    opponent
                                                    );
                                            gameClient.writeToBox("Move (" + lastPlayer + "): x=" +
                                              p.gameState.tac.x + " y=" + p.gameState.tac.y, Color.Black);
                                            gameClient.fillBoard(
                                                p.gameState.tac.x,
                                                p.gameState.tac.y,
                                                opponent);
                                            //wysylam swoj ruch
                                            SendMove();
                                            lastPlayer = p.nextPlayer.nick;
                                        }
                                        //nie moja kolej
                                        else
                                        {
                                            //bylem pierwszy wiec swoj ruch oznaczam X
                                            game.move(
                                                    p.gameState.tac.x,
                                                    p.gameState.tac.y,
                                                    me
                                                    );
                                            gameClient.writeToBox("Move (" + lastPlayer + "): x=" +
                                              p.gameState.tac.x + " y=" + p.gameState.tac.y, Color.Black);
                                            gameClient.fillBoard(
                                                p.gameState.tac.x,
                                                p.gameState.tac.y,
                                                me);
                                            lastPlayer = p.nextPlayer.nick;
                                        }
                                        break;
                                    }
                                    //logger.Trace(Environment.NewLine + game.printBoard());
                                    //break;
                                }
                                #endregion
                            case "championsList":
                                #region championsList
                                {
                                    gameClient.writeToBox(" --- Champions list ---", Color.Black);
                                    logger.Info(" --- Champions list ---");
                                    foreach (var player in p.player)
                                    {
                                        logger.Info(player.nick + ":  | Wons:" + player.won + " Loses: " + player.lost);
                                        gameClient.writeToBox(player.nick + ":  | Wons:" + player.won + " Loses: " + player.lost, Color.Blue);
                                    }
                                    break;
                                }
                                #endregion
                        }
                        #endregion

                        // If the connection is still usable restablish the callback
                        if (serverSock.Connected)
                        {
                            System.Threading.Thread.Sleep(10);
                            SetupReceiveCallback(sock);
                        }
                    } while (recvMessage.Length > 0);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error: Receiving data error" + Environment.NewLine + e.Message.ToString());
                logger.Trace("Error: Receiving data error" + Environment.NewLine + e.Message.ToString()
                    + Environment.NewLine + e.StackTrace.ToString());
                Console.Write("Error: Receiving data error" + Environment.NewLine + e.Message.ToString());
            }
        }
    }
}
