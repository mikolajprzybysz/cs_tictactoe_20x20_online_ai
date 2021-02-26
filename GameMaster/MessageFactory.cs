using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMaster
{
    public class MessageFactory
    {
        //<!--
        //an error message which may appear from both sides as a response 
        //-->
        public static String error(String error)
        {
            //<message type="error">
            //[String with error message]
            //</message>
            message msg = new message();
            msg.type = "error";
            msg.Text[0] = error;
            String str = XmlParser.ToXML(msg);
            // MessageBox.Show(str);
            return (str);
        }
        //<!-- login request message from new game master -->        
        public static String gameMasterLogin(String id, String gameType, int playersMin, int playersMax)
        {
            //<message type="gameMasterLogin">
            //<gameMasterLogin id="[string]" gameType="[string]" playersMin="[int>1]" playersMax="[int>=playersMin]"/>
            //</message>
            message msg = new message();
            msg.type = "gameMasterLogin";
            msg.gameMasterLogin = new messageGameMasterLogin();
            msg.gameMasterLogin.id = id;
            msg.gameMasterLogin.gameType = gameType;
            msg.gameMasterLogin.playersMin = playersMin;
            msg.gameMasterLogin.playersMinSpecified = true;
            msg.gameMasterLogin.playersMax = playersMax;
            msg.gameMasterLogin.playersMaxSpecified = true;
            String str = XmlParser.ToXML(msg);
            return str;

        }
        //<!--
        //game state message sent from the game master to the server and then by the server to all players in the game
        //-->
        public static String gameState(String gameId, String[,] player, int LastMoveX, int LastMoveY)
        {
            //<message type="gameState">
            //<gameId id="[string]"/>
            //<!-- one tag of the two below appears in message -->
            //<nextPlayer nick="[string]"/>
            //<gameOver>
            //<!-- this tag appears repeatedly for all the players -->
            //<player nick="[string]" result="loser/winner"/>
            //</gameOver>
            //<!--
            //this tag will always appear. Not read by the server.
            //-->
            //<gameState>
            //<!--
            //this tag contains game-specific information in XML format
            //-->
            //</gameState>
            //</message>
            message msg = new message();
            msg.type = "gameState";

            msg.gameId = new messageGameId();
            msg.gameId.id = gameId;

            msg.gameOver = new messagePlayer1[player.Length];
            for (int i = 0; i < player.Length; i++)
            {
                msg.gameOver[i].nick = player[i, 0];
                msg.gameOver[i].result = player[i, 1];
            }

            msg.gameState = new messageGameState();
            msg.gameState.tac = new messageGameStateTac();
            msg.gameState.tac.x = LastMoveX;
            msg.gameState.tac.y = LastMoveY;

            String str = XmlParser.ToXML(msg);
            return str;
        }
        public static String gameState(String gameId, String[,] player, messageGameStateTac tac)
        {
            //<message type="gameState">
            //<gameId id="[string]"/>
            //<!-- one tag of the two below appears in message -->
            //<nextPlayer nick="[string]"/>
            //<gameOver>
            //<!-- this tag appears repeatedly for all the players -->
            //<player nick="[string]" result="loser/winner"/>
            //</gameOver>
            //<!--
            //this tag will always appear. Not read by the server.
            //-->
            //<gameState>
            //<!--
            //this tag contains game-specific information in XML format
            //-->
            //</gameState>
            //</message>
            message msg = new message();
            msg.type = "gameState";

            msg.gameId = new messageGameId();
            msg.gameId.id = gameId;

            msg.gameOver = new messagePlayer1[player.Length/2];
            for (int i = 0; i < player.Length/2; i++)
            {
                msg.gameOver[i] = new messagePlayer1();
                msg.gameOver[i].nick = player[i, 0];
                msg.gameOver[i].result = player[i, 1];
            }

            msg.gameState = new messageGameState();
            msg.gameState.tac = tac;

            String str = XmlParser.ToXML(msg);
            return str;
        }

        public static String gameState(String gameId, String nextPlayer, int x, int y)
        {
            //<message type="gameState">
            //<gameId id="[string]"/>
            //<!-- one tag of the two below appears in message -->
            //<nextPlayer nick="[string]"/>
            //<gameOver>
            //<!-- this tag appears repeatedly for all the players -->
            //<player nick="[string]" result="loser/winner"/>
            //</gameOver>
            //<!--
            //this tag will always appear. Not read by the server.
            //-->
            //<gameState>
            //<!--
            //this tag contains game-specific information in XML format
            //-->
            //</gameState>
            //</message>
            message msg = new message();
            msg.type = "gameState";

            msg.gameId = new messageGameId();
            msg.gameId.id = gameId;

            msg.nextPlayer = new messageNextPlayer();
            msg.nextPlayer.nick = nextPlayer;

            msg.gameState = new messageGameState();
            msg.gameState.tac = new messageGameStateTac();
            msg.gameState.tac.x = x;
            msg.gameState.tac.y = y;
            
            String str = XmlParser.ToXML(msg);
            return str;
        }
        public static String gameState(String gameId, String nextPlayer, messageGameStateTac tac)
        {
            //<message type="gameState">
            //<gameId id="[string]"/>
            //<!-- one tag of the two below appears in message -->
            //<nextPlayer nick="[string]"/>
            //<gameOver>
            //<!-- this tag appears repeatedly for all the players -->
            //<player nick="[string]" result="loser/winner"/>
            //</gameOver>
            //<!--
            //this tag will always appear. Not read by the server.
            //-->
            //<gameState>
            //<!--
            //this tag contains game-specific information in XML format
            //-->
            //</gameState>
            //</message>
            message msg = new message();
            msg.type = "gameState";

            msg.gameId = new messageGameId();
            msg.gameId.id = gameId;

            msg.nextPlayer = new messageNextPlayer();
            msg.nextPlayer.nick = nextPlayer;

            msg.gameState = new messageGameState();
            if (tac != null)
                msg.gameState.tac = tac;

            String str = XmlParser.ToXML(msg);
            return str;
        }


    }
}
