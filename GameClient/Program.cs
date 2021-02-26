using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Utils;
using System.Diagnostics;
using System.IO;

namespace GameClient
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string serverIP, serverPort, playerNick;
            serverIP = serverPort = playerNick = null;
            bool properIP = false;
            
            Arguments CommandLine = new Arguments(args);

            if (CommandLine["connect_to"] != null)
            {
                if (CommandLine["connect_to"].Contains(":"))
                {
                    properIP = true;
                    int tmp = CommandLine["connect_to"].IndexOf(":");
                    serverIP = CommandLine["connect_to"].Substring(0, tmp);
                    serverPort = CommandLine["connect_to"].Substring(tmp + 1);
                }
            }
            if (CommandLine["nick"] != null)
            {
                playerNick = CommandLine["nick"];
            }
            if (args.Count() == 0 || !properIP)
            {
                Application.Run(new GameClient(null, null, null, false));
                //usage();
            }
            else
                Application.Run(new GameClient(serverIP, serverPort, playerNick, false));
        }

        static void usage()
        {
            Console.WriteLine("USAGE: {0} --connect_to [server IP]:[server port] --nick [players nick]", Process.GetCurrentProcess().MainModule.ModuleName);
        }
    }
}
