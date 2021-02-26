using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer {
    public class Player {
        String nick;
        String gameId;
        public bool isPlaying {
            set;
            get;
        }

        public Player() {
            isPlaying = false;
        }
        public Player(String nck) {
            this.nick = nck;
        }
        public String getNick(){
            return nick;
        }
        public void setNick(String nck) {
            nick = nck;
        }
        public String getGameId() {
            return gameId;
        }
        public void setGameId(String id) {
            gameId = id;
        }
        
    }
}
