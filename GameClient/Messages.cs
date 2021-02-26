using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameClient
{
    class Messages
    {
        class Handle
        {
            //<!--
            //an error message which may appear from both sides as a response 
            //-->
            public static void error(message msg)
            {
            }
            //<!-- login request response sent by server -->
            public static void loginResponse(message msg)
            {
            }
            //<!--
            //game state message sent from the game master to the server and then by the server to all players in the game
            //-->
            public static void gameState(message msg)
            {
            }
            //<!--
            //message sent before shutting down server to all registered players and game master
            //-->
            public static void serverShutdown(message msg)
            {
            }
            //<!--
            //server message with championship winners sent to all participants of this championship. A player may expect this kind of message anytime when not playing any game. This list is sorted according to won and then lost values
            //-->
            public static void championsList(message msg)
            {
            }
        }
        class Create
        {
            //<!--
            //an error message which may appear from both sides as a response 
            //-->        
            public static string error(string errorMsg)
            {
                //<message type="error">
                //[String with error message]
                //</message>
                message msg = new message();
                msg.type = "error";
                msg.Text[0] = errorMsg;

                return XmlParser.ToXML(msg);
            }
            //<!-- login request message from new player -->
            public static string playerLogin(String nick, String gameType)
            {
                //<message type="playerLogin">
                //<playerLogin nick="[string]" gameType="[string]"/>
                //</message>
                messagePlayerLogin msgPlayerLogin = new messagePlayerLogin();
                msgPlayerLogin.nick = nick;
                msgPlayerLogin.gameType = gameType;

                message msg = new message();
                msg.type = "playerLogin";
                msg.playerLogin = msgPlayerLogin;

                return XmlParser.ToXML(msg);

            }
            //<!--
            //move message sent by player to server and by server to the game master as
            //a response to gameState message with nick pointing to that player
            //-->           
            public static string moveTic(int x, int y, string gameId)
            {
                //<message type="move">
                //<gameId id="[string]"/>
                //<!-- not read by server -->
                //<move>
                //<!--
                //this tag contains rule-specific information in XML format
                //-->
                //</move>
                //</message>               
                messageMoveTic msgMoveTic = new messageMoveTic();
                msgMoveTic.x = x;
                msgMoveTic.y = y;

                messageMove msgMove = new messageMove();
                msgMove.tic = msgMoveTic;

                messageGameId msgGameId = new messageGameId();
                msgGameId.id = gameId;

                message msg = new message();
                msg.type = "move";
                msg.gameId = msgGameId;
                msg.move = msgMove;

                return XmlParser.ToXML(msg);

            }
            //<!--
            //"leaving game" request message sent by player to server
            //-->
            public static string leaveGame(string gameId)
            {
                //<message type="leaveGame">
                //<gameId id="[string]"/>
                //</message>
                //messageGameId
                messageGameId msgGameId = new messageGameId();
                msgGameId.id = gameId;

                message msg = new message();
                msg.type = "leaveGame";
                msg.gameId = msgGameId;

                return XmlParser.ToXML(msg);

            }
            //<!-- logout request from player -->
            public static string logout()
            {
                //<message type="logout"/>
                message msg = new message();
                msg.type = "logout";

                return XmlParser.ToXML(msg);
            }
        }
    }
}
