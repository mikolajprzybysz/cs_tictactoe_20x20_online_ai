using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ConnectionUtils;
using System.Threading;
using System.Diagnostics;

namespace GameServer
{
    public partial class GameServer : Form
    {
        ThreadStart ths;
        Thread th;
        GameServerUtil gameServer = null;
        public GameServer() {
            InitializeComponent();
            gameServer = new GameServerUtil(logListBox);
            ths = new ThreadStart(gameServer.start);
            th = new Thread(ths);
            th.Start();
        }
        public GameServer(int port)
        {
            InitializeComponent();
            this.buttonStart.Enabled = false;
            gameServer = new GameServerUtil(logListBox,port);
            ths = new ThreadStart(gameServer.start);
            th = new Thread(ths);
            th.Start();
        }
        public GameServer(string championship_id, int min_players, int port) {
            InitializeComponent();
            this.buttonStart.Enabled = false;

            gameServer = new GameServerUtil(logListBox,championship_id,min_players,port);
            ths = new ThreadStart(gameServer.start);
            th = new Thread(ths);
            th.Start();
        }
        
        private void startServerButton_Click(object sender, EventArgs e) {
            gameServer = new GameServerUtil(logListBox);
            ths = new ThreadStart(gameServer.start);
            th = new Thread(ths);
            th.Start();
           // gameServer = ;
            //ThreadStart ths = new ThreadStart(gameServer.start);

            //ProtocolParser.MessageBuilder.msgPlayerLogin("yes",0);
        }

        private void button1_Click(object sender, EventArgs e) {
            //Monitor.Enter(gameServer.isStopMonitor);
            gameServer.isStop = true;
            //Monitor.Exit(gameServer.isStopMonitor);
            th.Join();
            if(th.IsAlive)th.Suspend();
            Process.GetCurrentProcess().Kill();
           
        }
    }
}
