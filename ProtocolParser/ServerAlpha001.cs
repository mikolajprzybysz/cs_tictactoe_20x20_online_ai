using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace ConnectionUtils
{
    class ServerAlpha001
    {
        Socket Listener;
        Socket Client;
        byte[] buffer;
        ServerAlpha001()
        {
            string port_number = "12345";
            Listener = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, Convert.ToInt16(port_number, 10));

            Listener.Bind(ipLocal);

            Listener.Listen(10);

            Listener.BeginAccept(new AsyncCallback(OnClientConnect), null);
        }
        private void OnClientConnect(IAsyncResult asyn)
        {
            Client = Listener.EndAccept(asyn);

            buffer = new byte[100];

            WaitForData();
        }



        private void WaitForData()
        {
            Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnDataReceived), null);
        }

        private void OnDataReceived(IAsyncResult asyn)
        {

            string szData;

            int iRx = 0 ;

            iRx = Client.EndReceive(asyn);

            char[] chars = new char[iRx + 1];

            System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();

            int charLen = d.GetChars(buffer, 0, iRx, chars, 0);

            szData = new string(chars);

            Console.WriteLine(szData);

            // Do whatever one desires with the szData string which represents the received data
            WaitForData();
        }
    }
}
