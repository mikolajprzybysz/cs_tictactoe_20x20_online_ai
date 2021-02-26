using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Utils;
using System.Diagnostics;

namespace GameServer
{
    static class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string [] args)
        {
            Arguments CommandLine = new Arguments(args);

            string championship_id = null;
            int no_of_players_in_championship = 0;
            int port = 0;
            try {
                if (CommandLine["championship"] != null) {
                    int tmp = CommandLine["championship"].IndexOf(":");
                    championship_id = CommandLine["championship"].Substring(0, tmp);
                    no_of_players_in_championship = Convert.ToInt32(CommandLine["championship"].Substring(tmp + 1));
                }
                if (CommandLine["port"] != null) {
                    port = Convert.ToInt32(CommandLine["port"]);
                } else {
                    Usage(Process.GetCurrentProcess().MainModule.ModuleName);
                    Console.ReadKey();
                   return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if(CommandLine["championship"]!=null){
                    Application.Run(new GameServer(championship_id,no_of_players_in_championship,port));
                }else{
                    Application.Run(new GameServer(port));
                }
            } catch (Exception e) {
                logger.Debug(e.Message + "\n" + e.Source + "\n" + e.StackTrace);
                //Console.ReadKey();
            }
            
        }
        static void Usage(string program) {
            Console.WriteLine("USAGE: {0} --championship [game id]:[no_of_players] --port [PORT_NO]", program);
            Console.WriteLine("OR");
            Console.WriteLine("USAGE: {0} --port [PORT_NO]", program);
        }
    }
}
