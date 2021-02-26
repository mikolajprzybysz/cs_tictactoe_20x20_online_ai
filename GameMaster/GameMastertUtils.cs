using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Collections;
using NLog;

delegate void AddMessage(string sNewMessage);

namespace GameMaster
{
    class GameMasterUtils
    {
        private Socket serverSock;
        private byte[] m_byBuff = new byte[1024];
        private event AddMessage m_AddMessage;
        //ListBox sendListBox, recListBox;
        GameMaster Parent;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // todo
        private Hashtable handlers;

        public bool isSocketNull()
        {
            if (serverSock == null)
                return true;
            else
                return false;
        }
        public bool isConnected()
        {
            return serverSock.Connected;
        }
        public int send(byte[] buffer, int size, SocketFlags socketFlag)
        {
            return serverSock.Send(buffer, size, socketFlag);
        }
        // constructor
        public GameMasterUtils()
        {
            serverSock = null;
        }
        public GameMasterUtils(int alPort, String szIPSelected, ListBox sendLB, ListBox recLB, GameMaster Parent)
        {
            //recListBox = recLB;
            //sendListBox = sendLB;
            this.Parent = Parent;

            #region Setting up handling functions
            handlers = new Hashtable();
            handlers.Add("error", new ProtocolParser.handlingFunction(Parent.error));
            handlers.Add("loginResponse", new ProtocolParser.handlingFunction(Parent.loginResponse));
            handlers.Add("beginGame", new ProtocolParser.handlingFunction(Parent.beginGame));
            handlers.Add("move", new ProtocolParser.handlingFunction(Parent.move));
            handlers.Add("playerLeftGame", new ProtocolParser.handlingFunction(Parent.playerLeftGame));
            handlers.Add("serverShutdown", new ProtocolParser.handlingFunction(Parent.serverShutdown));
            #endregion

            try
            {
                if (serverSock != null && serverSock.Connected)
                {
                    serverSock.Shutdown(SocketShutdown.Both);
                    System.Threading.Thread.Sleep(10);
                    serverSock.Close();
                }
                serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Parse(szIPSelected);
                System.Net.IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, alPort);


                // Connect to server non-Blocking method
                serverSock.Blocking = false;
                AsyncCallback onconnect = new AsyncCallback(OnConnect);
                serverSock.BeginConnect(remoteEndPoint, onconnect, serverSock);
            }
            catch (SocketException)
            {
                Console.Write("Unable to connect to server.");
            }
        }
        //public void sendMessage(string msg, ListBox sendLB)
        public void sendMessage(string msg)
        {
            Byte[] byteDateLine = Encoding.ASCII.GetBytes(msg.ToCharArray());
            send(byteDateLine, byteDateLine.Length, 0);
            //m_AddMessage = new AddMessage(OnSendMessage);
            logger.Info("(Send) : " + msg);
            //sendListBox.Invoke(m_AddMessage, new string[] { msg });
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
                    MessageBox.Show("Unable to connect to remote machine", "Connect Failed!");
            }
            catch (Exception)
            {
                MessageBox.Show("Unusual error during Connect!");
            }
        }
        public void SetupReceiveCallback(Socket sock)
        {
            try
            {
                AsyncCallback receiveData = new AsyncCallback(OnReceivedData);
                sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None, receiveData, sock);
            }
            catch (Exception)
            {
                MessageBox.Show("Setup Receive Callback failed!");
            }
        }
        public void OnReceivedData(IAsyncResult ar)
        {
            // Socket was the passed in object
            Socket sock = (Socket)ar.AsyncState;

            // Check if we got any data
            try
            {
                int nBytesRec = sock.EndReceive(ar);
                if (nBytesRec > 0)
                {


                    // Wrote the data to the List
                    string sReceived = Encoding.ASCII.GetString(m_byBuff, 0, nBytesRec);

                    // wrote data to log
                    logger.Debug("(Received) " + sReceived);
                    //Console.WriteLine(sReceived);
                    m_AddMessage = new AddMessage(OnRecMessage);
                    //recListBox.Invoke(m_AddMessage, new string[] { sReceived });

                    ProtocolParser.MessageProcessor msgProc = new ProtocolParser.MessageProcessor(handlers);
                    message p = XmlParser.parseXml(sReceived);
                    msgProc.handle(p);

                    // If the connection is still usable restablish the callback
                    if (serverSock.Connected) SetupReceiveCallback(sock);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unusual error druing Receive! [" + ex.TargetSite + " ]: " + ex.Message);
            }
        }
        public void OnRecMessage(string sMessage)
        {
            //recListBox.Items.Add(sMessage);
        }
        public void OnSendMessage(string sMessage)
        {
            //sendListBox.Items.Add(sMessage);
        }

    }
}
