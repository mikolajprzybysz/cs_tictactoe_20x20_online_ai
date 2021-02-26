using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace ProtocolParser
{
    public class GameServerMsg
    {
        public class MessageHandle
        {
            //<!--
            //an error message which may appear from both sides as a response 
            //-->
            public static void error(protocol message)
            {
            }
            //<!-- login request message from new player -->
            public static string playerLogin(protocol data)
            {
                String s = ((protocolMessagePlayerLogin)(data.message[0].Items[0])).nick + " " + ((protocolMessagePlayerLogin)(data.message[0].Items[0])).gameType;
              return GameServerMsg.MessageCreate.loginResponse("no",3);

                //MessageBox.Show(GameServerMsg.MessageCreate.loginResponse("yes", 0));
            }
            //<!-- login request message from new game master -->        
            public static string gameMasterLogin(protocol data)
            {
                //<message type="gameMasterLogin">
                //<gameMasterLogin id="[string]" gameType="[string]" playersMin="[int>1]" playersMax="[int>=playersMin]"/>
                //</message>
                String s = ((protocolMessageGameMasterLogin)(data.message[0].Items[0])).id + " " + ((protocolMessageGameMasterLogin)(data.message[0].Items[0])).gameType + " " + ((protocolMessageGameMasterLogin)(data.message[0].Items[0])).playersMin + " " + ((protocolMessageGameMasterLogin)(data.message[0].Items[0])).playersMax;
                return GameServerMsg.MessageCreate.loginResponse("no",1);
                //MessageBox.Show(s);
            }
            //<!--
            //Server message with player list sent to game initiator. As a response initiator sends gameState message with nextPlayer set to a desired first player to do a move.
            //-->
            //<!--
            //game state message sent from the game master to the server and then by the server to all players in the game
            //-->
            public static void gameState(protocol message)
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
            //"leaving game" request message sent by player to server
            //-->
            public static void leaveGame(protocol message)
            {
            }
            //<!--
            //by this message server and game master exchange information about player that left a game
            //-->
            public static void playerLeftGame(protocol message)
            {
            }
            //<!-- logout request from player -->
            public static void logout(protocol message)
            {
            }
        }
        public class MessageCreate
        {

            /// <summary>
            ///
            /// an error message which may appear from both sides as a response 
            ///
            /// </summary>
            /// <param name="message">Error Message</param>
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
            //<!-- login request response sent by server -->
            public static String loginResponse(String accept, int error)
            {

                //<message type="loginResponse">
                //<response accept="yes/no"/>
                //−
                //<!--

                //tag present only when accept="no"
                //Error ids:
                //1 - wrong nick
                //2 - improper game type
                //3 - players pool overflow
                //-->
                //<error id="[int]"/>
                //</message>

                messageResponse msgResponse = new messageResponse();
                msgResponse.accept = accept;

                messageError msgError = new messageError();
                msgError.id = error;

                message msg = new message();
                msg.type = "loginResponse";
                msg.response = msgResponse;
                msg.error = msgError;

                return XmlParser.ToXML(msg);
            }
            public static String loginResponse(String accept)
            {

                //<message type="loginResponse">
                //<response accept="yes/no"/>
                //−
                //<!--

                //tag present only when accept="no"
                //Error ids:
                //1 - wrong nick
                //2 - improper game type
                //3 - players pool overflow
                //-->
                //<error id="[int]"/>
                //</message>

                messageResponse msgResponse = new messageResponse();
                msgResponse.accept = accept;

                message msg = new message();
                msg.type = "loginResponse";
                msg.response = msgResponse;

                return XmlParser.ToXML(msg);
            }
            //<!--
            //Server message with player list sent to game initiator. As a response initiator sends gameState message with nextPlayer set to a desired first player to do a move.
            //-->
            public static String beginGame(String gameId, String[] nicks)
            {
                //<message type="beginGame">
                //<!-- unique game identifier generated by server-->
                //<gameId id="[string]"/>
                //−
                //<!--
                //this tag appears repeatedly for all players selected by a server
                //-->
                //<player nick="[string]"/>
                //</message>
                message msg = new message();
                msg.type = "beginGame";

                messageGameId msgGameId = new messageGameId();
                msgGameId.id = gameId;

                
                for (int i = 0; i < nicks.Length; i++)
                {
                    messagePlayer msgPlayer = new messagePlayer();
                    msgPlayer.nick = nicks[i];
                    msg.player[i] = msgPlayer;
                }
             
                return XmlParser.ToXML(msg);
            }
            //<!--
            //game state message sent from the game master to the server and then by the server to all players in the game
            //-->
            public static String gameState(String gameId, String nextPlayer)
            {
                //<message type="gameState">
                //<gameId id="[string]"/>
                //<!-- one tag of the two below appears in message -->
                //<nextPlayer nick="[string]"/>
                //−
                //<gameOver>
                //<!-- this tag appears repeatedly for all the players -->
                //<player nick="[string]" result="loser/winner"/>
                //</gameOver>
                //−
                //<!--
                //this tag will always appear. Not read by the server.
                //-->
                //−
                //<gameState>
                //−
                //<!--
                //this tag contains game-specific information in XML format
                //-->
                //</gameState>
                //</message>

                messageNextPlayer msgNextPlayer = new messageNextPlayer();
                msgNextPlayer.nick = nextPlayer;

                messageGameId msgGameId = new messageGameId();
                msgGameId.id = gameId;

                message msg = new message();
                msg.type = "gameState";
                msg.gameId = msgGameId;
                msg.nextPlayer = msgNextPlayer;

                return XmlParser.ToXML(msg);
            }
            public static void gameState(String gameId, String[,] gameOver)
            {
                //<message type="gameState">
                //<gameId id="[string]"/>
                //<!-- one tag of the two below appears in message -->
                //<nextPlayer nick="[string]"/>
                //−
                //<gameOver>
                //<!-- this tag appears repeatedly for all the players -->
                //<player nick="[string]" result="loser/winner"/>
                //</gameOver>
                //−
                //<!--
                //this tag will always appear. Not read by the server.
                //-->
                //−
                //<gameState>
                //−
                //<!--
                //this tag contains game-specific information in XML format
                //-->
                //</gameState>
                //</message>
                //message msg = new message();
                //msg.type = "gameState";

                //messageGameId msgGameId = new messageGameId();
                //msgGameId.id = gameId;

                //messageGameOve
                //for (int i = 0; i < gameOver.Length; i++)
                //{
                //    messagePlayer1 msgPlyr = new messagePlayer1();
                //    msgPlyr.nick = gameOver[i, 0];
                //    msgPlyr.result = gameOver[i, 1];
                //    msg
                //}

                
                //msg.gameId = msgGameId;
                //msg.nextPlayer = msgNextPlayer;

                //return XmlParser.ToXML(msg);
                //return null;
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
                //−
                //<move>
                //−
                //<!--

                //this tag contains rule-specific information in XML format
                //-->
                //</move>
                //</message>
            }
            //<!--
            //by this message server and game master exchange information about player that left a game
            //-->
            public static void playerLeftGame(protocol message)
            {
                //<message type="playerLeftGame">
                //<player nick="[string]"/>
                //<gameId id="[string]"/>
                //</message>
            }
            //<!--
            //message sent before shutting down server to all registered players and game master
            //-->
            public static void serverShutdown()
            {
                //<message type="serverShutdown"/>
            }
            //<!--
            //server message with championship winners sent to all participants of this championship. A player may expect this kind of message anytime when not playing any game. This list is sorted according to won and then lost values
            //-->
            // Object[String,int,int] - Nick, Won, Lost
            public static void championsList(Object[,,] players)
            {
                //<message type="championsList">
                //−
                //<!--
                //this tag appears repeatedly for all registers players
                //-->
                //<player nick="[string]" won="[int]" lost="[int]"/>
                //</message>
            }           
            
            
            
        }
    }
}
