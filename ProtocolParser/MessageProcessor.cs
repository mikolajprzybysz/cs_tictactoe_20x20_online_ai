using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProtocolParser
{
    public class MessageProcessor
    {
        private Hashtable protocolTable;
        public MessageProcessor(Hashtable messageHandlers)
        {
            protocolTable = messageHandlers;
        }
        public string handle(message msg)
        {
            try {
                handlingFunction handler = (handlingFunction)protocolTable[msg.type];
                if (handler != null)
                    return handler(msg);
            } catch (Exception e) {
                MessageBox.Show("Inncorrect message: "+e.Message);
            }

            return null;
        }
    }
}
// Hashtable messageHandlers = new Hashtable();
// messageHandlers.Add("error", new f(ProtocolParser.GameServer.Handlers.error));
// messageHandlers.Add("gameId", new f(this.gameId));
// messageHandlers.Add("gameMasterLogin", new f(this.gameMasterLogin));
// messageHandlers.Add("gameOver", new f(this.gameOver));
// messageHandlers.Add("gameState", new f(this.gameState));
// messageHandlers.Add("move", new f(this.move));
// messageHandlers.Add("nextPlayer", new f(this.nextPlayer));
// messageHandlers.Add("player", new f(this.player));
// messageHandlers.Add("playerLogin", new f(this.playerLogin));
// messageHandlers.Add("response", new f(this.response));
// MessageProcessor smp = new MessageProcessor(messageHandlers);
// smp.handle(message);