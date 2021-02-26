using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ProtocolParser
{
    class GameMaster
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
            //Server message with player list sent to game initiator. As a response initiator sends gameState message with nextPlayer set to a desired first player to do a move.
            //-->
            public static void beginGame(protocol message)
            {
            }
            //<!--
            //move message sent by player to server and by server to the game master as
            //a response to gameState message with nick pointing to that player
            //-->
            public static void move(protocol message)
            {
            }
            //<!--
            //by this message server and game master exchange information about player that left a game
            //-->
            public static void playerLeftGame(protocol message)
            {
            }
            //<!--
            //message sent before shutting down server to all registered players and game master
            //-->
            public static void serverShutdown(protocol message)
            {
            }
        }
        class MessageCreate
        {
            //<!--
            //an error message which may appear from both sides as a response 
            //-->
            public static String error(String message)
            {
                //<message type="error">
                //[String with error message]
                //</message>
                return null;
            }
            //<!-- login request message from new game master -->        
            public static String gameMasterLogin(String id, String gameType, int playersMin, int playersMax)
            {
                //<message type="gameMasterLogin">
                //<gameMasterLogin id="[string]" gameType="[string]" playersMin="[int>1]" playersMax="[int>=playersMin]"/>
                //</message>
                protocolMessageGameMasterLogin msg = new protocolMessageGameMasterLogin();
                msg.id = id;
                msg.gameType = gameType;
                msg.playersMin = playersMin;
                msg.playersMax = playersMax;
                return msg.ToString();

            }
            //<!--
            //game state message sent from the game master to the server and then by the server to all players in the game
            //-->
            public static String gameState(String gameId, String[,] nick, XmlDocument gameState)
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
                return null;
            }

        }
    }
}
