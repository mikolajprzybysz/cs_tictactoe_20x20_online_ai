using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProtocolParser {
    public class MessageBuilder {
        public static string msgplayerLogin(string gameType, string nick) {
        
            message msg = new message();
            msg.type = "playerLogin";
            msg.playerLogin = new messagePlayerLogin();
            msg.playerLogin.gameType = gameType;
            msg.playerLogin.nick = nick;
            string str = XmlParser.ToXML(msg);
           // MessageBox.Show(str);
            return (str);
        }
        public static string msgloginResponse(string response)
        {
            message msg = new message();
            msg.type = "loginResponse";
            msg.response = new messageResponse();
            ((messageResponse)msg.response).accept = response;
            string str = XmlParser.ToXML(msg);
           // MessageBox.Show(str);
            return (str);

        }

        public static string msgloginResponse(string response, int errorId)
        {
            message msg = new message();
            msg.type = "loginResponse";
            msg.response = new messageResponse();
            ((messageResponse)msg.response).accept = response;
            msg.error = new messageError();
            ((messageError)msg.error).id = errorId;
            string str = XmlParser.ToXML(msg);
            //MessageBox.Show(str);
            return (str);
        }


      
        public static string msgPlayerLeftGame(string playerNick, string gameId) 
        {
            //<message type="playerLeftGame">
            //<player nick="[string]"/>
            //<gameId id="[string]"/>
            //</message>
            message msg = new message();
            msg.type = "playerLeftGame";

            messagePlayer msgPlayer = new messagePlayer();
            msgPlayer.nick = playerNick;

            messageGameId msgGameId = new messageGameId();
            msgGameId.id = gameId;

            return XmlParser.ToXML(msg);

        }


        public static string msgBeginGame(string[] playerNicks, string gameId)       
        {
            //<message type="playerLeftGame">
            //<player nick="[string]"/>
            //<gameId id="[string]"/>
            //</message>
            message msg = new message();
            msg.type = "beginGame";

            msg.gameId = new messageGameId();
            msg.gameId.id = gameId;

            msg.player = new messagePlayer[playerNicks.Length];
            int i=0;
            foreach (string nick in playerNicks)
            {
                msg.player[i] = new messagePlayer();
                msg.player[i].nick = nick;
                i++;
            }
            
            return XmlParser.ToXML(msg);
        }
        public static string msgShutdown()
        {
            message msg = new message();
            msg.type = "serverShutdown";
            return XmlParser.ToXML(msg);
        }
        public static string msgLogout()
        {
            message msg = new message();
            msg.type = "logout";
            return XmlParser.ToXML(msg);
        }
        public static string msgMove(string gameID, int x, int y)
        {
            message p = new message();
            p.type = "move"; 
            p.move = new messageMove();
            p.move.tic = new messageMoveTic();
            p.move.tic.x = x;
            p.move.tic.y = y;
            p.gameId = new messageGameId();
            p.gameId.id = gameID;
            return XmlParser.ToXML(p);
        }

        public static string msgThankYou(string gameId)
        {
            message msg = new message();
            msg.type = "thank you";
            msg.gameId = new messageGameId();
            msg.gameId.id = gameId;

            return XmlParser.ToXML(msg);
        }

        public static string msgChampionsList(messagePlayer[] players)
        {
            message msg = new message();
            msg.type = "championsList";
            msg.player = new messagePlayer[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                msg.player[i] = players[i];
            }

            return XmlParser.ToXML(msg);

        }

        public static string msgChampionsList(string[,] players)
        {
            message msg = new message();
            msg.type = "championsList";
            // dziele przez 3 bo nick, won, lost;
            msg.player = new messagePlayer[players.Length/3];
            for (int i = 0; i < players.Length; i++)
            {
                msg.player[i] = new messagePlayer();
                msg.player[i].nick = players[i, 0];
                msg.player[i].won = int.Parse(players[i, 1]);
                msg.player[i].lost = int.Parse(players[i, 2]);
            }

            return XmlParser.ToXML(msg);

        }
    }
}
