using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace ConnectionUtils
{
    class ClientAlpha001
    {
        ClientAlpha001()
        {
            Socket m_socClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            string szIPSelected = "192.168.1.1";

            string szPort = "12345";

            int alPort = System.Convert.ToInt16 (szPort,10);



            System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Parse(szIPSelected);

            System.Net.IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, alPort);

            m_socClient.Connect(remoteEndPoint);

            string szData = "Hello there. This is the text that will be sent.";

            byte[] byData = System.Text.Encoding.ASCII.GetBytes(szData);

            m_socClient.Send(byData);



        }
    }
}
