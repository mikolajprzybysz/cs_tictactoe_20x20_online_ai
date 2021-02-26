using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer {
    public class GameMasterInfo {
        public string id { get; set; }
        public string gameType { get; set; }
        public int playersMin { get; set; }
        public int playersMax { get; set; }
        public System.Collections.ArrayList gameIds {get;set;}
        public int noOfPlayers { get; set; }
        public GameMasterInfo(string id, string gameType, int playersMin, int playersMax) {
            gameIds = new System.Collections.ArrayList();
            this.id = id;
            this.gameType = gameType;
            this.playersMin = playersMin;
            this.playersMax = playersMax;
            this.noOfPlayers = 0;
        }


    }
}
