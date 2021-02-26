using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolParser
{
    class GameClient
    {
        class MessageHandle
        {
            //<!--
            //an error message which may appear from both sides as a response 
            //-->
            public static void error(protocol message)
            {
            }
            //<!-- login request response sent by server -->
            public static void loginResponse(protocol message)
            {
            }
            //<!--
            //game state message sent from the game master to the server and then by the server to all players in the game
            //-->
            public static void gameState(protocol message)
            {
            }
            //<!--
            //message sent before shutting down server to all registered players and game master
            //-->
            public static void serverShutdown(protocol message)
            {
            }
            //<!--
            //server message with championship winners sent to all participants of this championship. A player may expect this kind of message anytime when not playing any game. This list is sorted according to won and then lost values
            //-->
            public static void championsList(protocol message)
            {
            }
        }
        class MessageCreate
        {
            //<!--
            //an error message which may appear from both sides as a response 
            //-->
            public static void error(protocol message)
            {
                //<message type="error">
                //[String with error message]
                //</message>
            }
            //<!-- login request message from new player -->
            public static String playerLogin(String nick, String gameType)
            {
                //<message type="playerLogin">
                //<playerLogin nick="[string]" gameType="[string]"/>
                //</message>
                protocolMessagePlayerLogin msg = new protocolMessagePlayerLogin();
                msg.nick = nick;
                msg.gameType = gameType;
                return msg.ToString();
                
            }
            //<!--
            //move message sent by player to server and by server to the game master as
            //a response to gameState message with nick pointing to that player
            //-->
            public static void move(protocol message)
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
            }
            //<!--
            //"leaving game" request message sent by player to server
            //-->
            public static void leaveGame(protocol message)
            {
                //<message type="leaveGame">
                //<gameId id="[string]"/>
                //</message>
            }
            //<!-- logout request from player -->
            public static void logout(protocol message)
            {
                //<message type="logout"/>
            }
        }
    }
}
