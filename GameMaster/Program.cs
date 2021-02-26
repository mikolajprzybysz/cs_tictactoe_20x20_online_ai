using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using System.Diagnostics;
using System.Windows.Forms;

namespace GameMaster
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string serverIp = null;
            string serverPort = null;
            Arguments CommandLine = new Arguments(args);

            if (CommandLine["connect_to"] != null)
            {
                int tmp = CommandLine["connect_to"].IndexOf(":");
                serverIp = CommandLine["connect_to"].Substring(0, tmp);
                serverPort = CommandLine["connect_to"].Substring(tmp + 1);


                Application.Run(new GameMaster(serverIp, serverPort));
            }
            else
            {
                Usage(Process.GetCurrentProcess().MainModule.ModuleName);
                Application.Run(new GameMaster());
            }

           
        }

        static void Usage(string program)
        {
            Console.WriteLine("USAGE: {0} --connect_to IP:PORT", program);
            Console.WriteLine("USAGE: {0} -connect_to:IP:PORT", program);
        }
    }
}
