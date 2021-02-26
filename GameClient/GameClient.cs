using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace GameClient
{
    
    public partial class GameClient : Form
    {
        private int port;
        private string ip;
        private GameClientUtils gameClientUtils = new GameClientUtils();
        public Button[,] TicTacToeBoard;

        // adding buttons to list and to table of Buttons
        private void prepareBoard()
        {
            TicTacToeBoard = new Button[20, 20];
            Hashtable buttons = new Hashtable();
            foreach (Control c in tableLayoutPanel2.Controls)
            {
                if (c is Button)
                {
                    buttons.Add(c.Name, c);
                }
            }
            int counter = 1;
            for (int i=0; i < 20; i++)
                for (int j=0; j < 20; j++)
                {
                    string s = "button" + counter;
                    TicTacToeBoard[j, i] = (Button)buttons[s];
                    counter++;
                }
            #region prepare buttons (board) color, style, size...
            foreach (Button b in TicTacToeBoard)
            {
                b.Text = "";
                b.BackColor = System.Drawing.Color.White;
                b.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
                b.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
                b.Dock = System.Windows.Forms.DockStyle.Fill;
                b.Enabled = true;
                b.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
                b.FlatAppearance.BorderSize = 0;
                b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                b.Margin = new System.Windows.Forms.Padding(0);
                b.Size = new System.Drawing.Size(25, 25);
                b.UseVisualStyleBackColor = false;
                b.ImageAlign = ContentAlignment.MiddleCenter;
            }
            pieceButton.Text = "";
            pieceButton.BackColor = System.Drawing.Color.White;
            pieceButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            pieceButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            pieceButton.Enabled = true;
            pieceButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            pieceButton.FlatAppearance.BorderSize = 0;
            pieceButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            pieceButton.Margin = new System.Windows.Forms.Padding(0);
            pieceButton.Size = new System.Drawing.Size(25, 25);
            pieceButton.UseVisualStyleBackColor = false;
            pieceButton.ImageAlign = ContentAlignment.MiddleCenter;
            pieceButton.Image = null;
            #endregion
        }
        // reset all images on the board
        public void clearBoard()
        {
            foreach (Button b in TicTacToeBoard)
            {
                b.Image = null;
            }
            pieceButton.Image = null;
        }
        // set image with specified piece on the board
        public void fillBoard(int x, int y, int who)
        {
            switch (who)
            {
                case 1:
                    TicTacToeBoard[x - 1, y - 1].Image = global::GameClient.Properties.Resources.kolko1;
                    return;
                case 2:
                    TicTacToeBoard[x - 1, y - 1].Image = global::GameClient.Properties.Resources.krzyzyk1;
                    return;
            }
        }
        // set button with X or O which means which we play
        public void setPiece(int who)
        {
            switch (who)
            {
                case 1:
                    pieceButton.Image = global::GameClient.Properties.Resources.kolko1;
                    return;
                case 2:
                    pieceButton.Image = global::GameClient.Properties.Resources.krzyzyk1;
                    return;
            }
        }
        // main window with board
        public GameClient(string serverIP, string serverPort, string playersNick, bool connectAutomatically)
        {
            InitializeComponent();
            if (serverIP == null) IPTextBox.Text = "192.168.1.101"; //"194.29.178.51";
            else IPTextBox.Text = serverIP;
            if (serverPort == null) portTextBox.Text = "399";
            else portTextBox.Text = serverPort;
            if (playersNick == null) loginTextBox.Text = "Zdanek";
            else loginTextBox.Text = playersNick;
            gameTypeTextBox.Text = "tic tac toe";
            gameTypeTextBox.ReadOnly = true;
            timer1.Enabled = true;
            timer1.Interval = 1000;
            prepareBoard();

            if (connectAutomatically)
                connect();
        }
        // writing info about gameplay to tex box
        public void writeToBox(string msg, Color textColor)
        {
            if (textEventLog.InvokeRequired)
            {
                textEventLog.BeginInvoke(new Action(delegate
                {
                    writeToBox(msg, textColor);
                }));
                return;
            }

            string nDateTime = DateTime.Now.ToString("hh:mm:ss tt") + " - ";

            // color text.
            textEventLog.SelectionStart = textEventLog.Text.Length;
            textEventLog.SelectionColor = textColor;

            // newline if first line, append if else.
            if (textEventLog.Lines.Length == 0)
            {
                textEventLog.AppendText(nDateTime + msg);
                textEventLog.ScrollToCaret();
                textEventLog.AppendText(System.Environment.NewLine);
            }
            else
            {
                textEventLog.AppendText(nDateTime + msg + System.Environment.NewLine);
                textEventLog.ScrollToCaret();
            }
        }
        // function that changes Connect button
        public void changes(string mark)
        {
            switch (mark)
            {
                case "connected":
                    this.IPTextBox.ReadOnly = true;
                    this.portTextBox.ReadOnly = true;
                    this.loginTextBox.ReadOnly = true;
                    this.ConnectedToolStripStatusLabel.ForeColor = Color.Green;
                    this.ConnectedToolStripStatusLabel.Text = "Connected";
                    this.connectButton.Text = "Disconnect";
                    this.Cursor = Cursors.Default;
                    break;
                case "disconnected":
                    this.IPTextBox.ReadOnly = false;
                    this.portTextBox.ReadOnly = false;
                    this.loginTextBox.ReadOnly = false;
                    this.ConnectedToolStripStatusLabel.ForeColor = Color.Red;
                    this.ConnectedToolStripStatusLabel.Text = "Disconnected";
                    this.connectButton.Text = "Connect";
                    this.Cursor = Cursors.Default;
                    this.clearBoard();
                    break;
                case "connecting":
                    this.IPTextBox.ReadOnly = true;
                    this.portTextBox.ReadOnly = true;
                    this.loginTextBox.ReadOnly = true;
                    this.ConnectedToolStripStatusLabel.ForeColor = Color.Orange;
                    this.ConnectedToolStripStatusLabel.Text = "Connecting";
                    this.connectButton.Text = "Cancel";
                    this.Cursor = Cursors.WaitCursor;
                    break;
            }
        }
        // connect function create new GameClientUtils object
        private void connect()
        {
            port = Int32.Parse(portTextBox.Text);
            ip = IPTextBox.Text;
            gameClientUtils = new GameClientUtils(this, port, ip, textEventLog, loginTextBox.Text, gameTypeTextBox.Text);
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            // Check we are connected
            switch (connectButton.Text)
            {
                case "Connect":
                {
                    connect();
                    while (!gameClientUtils.isConnected() && gameClientUtils.isSocketNull())
                        changes("connecting");
                    changes("connected");
                    //connect();
                    break;
                }
                case "Disconnect":
                {
                    gameClientUtils.Disconnect();
                    break;
                }
                case "Cancel":
                {
                    gameClientUtils = null;
                    break;
                }
            }
        }

        private void GameClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (gameClientUtils.isConnected())
            {
                string msg = ProtocolParser.MessageBuilder.msgLogout();
                gameClientUtils.sendMessage(msg);
                gameClientUtils.logger.Info(" --- Sended --- " + Environment.NewLine + msg);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (gameClientUtils != null)
                    changes("connecting");
                if (gameClientUtils.isSocketNull() == true || gameClientUtils.isConnected() == false)
                    changes("disconnected");
                else
                    changes("connected");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
