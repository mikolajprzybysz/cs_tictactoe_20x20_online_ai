namespace GameMaster
{
    partial class GameMaster
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sendTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.connectButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.playersMaxTextBox = new System.Windows.Forms.TextBox();
            this.playersMinTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gameTypetextBox = new System.Windows.Forms.TextBox();
            this.IPTextBox = new System.Windows.Forms.TextBox();
            this.IDtextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.receivedListBox = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.sendListBox = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sendTextBox
            // 
            this.sendTextBox.Location = new System.Drawing.Point(31, 231);
            this.sendTextBox.Name = "sendTextBox";
            this.sendTextBox.Size = new System.Drawing.Size(200, 20);
            this.sendTextBox.TabIndex = 9;
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(80, 254);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(100, 23);
            this.sendButton.TabIndex = 8;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(62, 171);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(100, 23);
            this.connectButton.TabIndex = 4;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.playersMaxTextBox);
            this.panel1.Controls.Add(this.connectButton);
            this.panel1.Controls.Add(this.playersMinTextBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.gameTypetextBox);
            this.panel1.Controls.Add(this.IPTextBox);
            this.panel1.Controls.Add(this.IDtextBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.portTextBox);
            this.panel1.Location = new System.Drawing.Point(17, 11);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(250, 199);
            this.panel1.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(58, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "ID:";
            // 
            // playersMaxTextBox
            // 
            this.playersMaxTextBox.Location = new System.Drawing.Point(155, 135);
            this.playersMaxTextBox.Name = "playersMaxTextBox";
            this.playersMaxTextBox.Size = new System.Drawing.Size(30, 20);
            this.playersMaxTextBox.TabIndex = 14;
            // 
            // playersMinTextBox
            // 
            this.playersMinTextBox.Location = new System.Drawing.Point(85, 135);
            this.playersMinTextBox.Name = "playersMinTextBox";
            this.playersMinTextBox.Size = new System.Drawing.Size(30, 20);
            this.playersMinTextBox.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gameTypetextBox
            // 
            this.gameTypetextBox.Location = new System.Drawing.Point(85, 82);
            this.gameTypetextBox.Name = "gameTypetextBox";
            this.gameTypetextBox.Size = new System.Drawing.Size(100, 20);
            this.gameTypetextBox.TabIndex = 12;
            // 
            // IPTextBox
            // 
            this.IPTextBox.Location = new System.Drawing.Point(85, 3);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new System.Drawing.Size(100, 20);
            this.IPTextBox.TabIndex = 0;
            // 
            // IDtextBox
            // 
            this.IDtextBox.Location = new System.Drawing.Point(85, 56);
            this.IDtextBox.Name = "IDtextBox";
            this.IDtextBox.Size = new System.Drawing.Size(100, 20);
            this.IDtextBox.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(85, 30);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(100, 20);
            this.portTextBox.TabIndex = 1;
            // 
            // receivedListBox
            // 
            this.receivedListBox.ForeColor = System.Drawing.Color.Maroon;
            this.receivedListBox.FormattingEnabled = true;
            this.receivedListBox.HorizontalScrollbar = true;
            this.receivedListBox.Location = new System.Drawing.Point(529, 11);
            this.receivedListBox.Name = "receivedListBox";
            this.receivedListBox.Size = new System.Drawing.Size(250, 199);
            this.receivedListBox.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Game type:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(35, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Players:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(85, 119);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Min";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(158, 119);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Max";
            // 
            // sendListBox
            // 
            this.sendListBox.ForeColor = System.Drawing.Color.Green;
            this.sendListBox.FormattingEnabled = true;
            this.sendListBox.HorizontalScrollbar = true;
            this.sendListBox.Location = new System.Drawing.Point(273, 11);
            this.sendListBox.Name = "sendListBox";
            this.sendListBox.Size = new System.Drawing.Size(250, 199);
            this.sendListBox.TabIndex = 11;
            // 
            // GameMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 282);
            this.Controls.Add(this.sendListBox);
            this.Controls.Add(this.sendTextBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.receivedListBox);
            this.Name = "GameMaster";
            this.Text = "GameMaster";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox sendTextBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox IPTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox portTextBox;
        public System.Windows.Forms.ListBox receivedListBox;
        private System.Windows.Forms.TextBox gameTypetextBox;
        private System.Windows.Forms.TextBox IDtextBox;
        private System.Windows.Forms.TextBox playersMinTextBox;
        private System.Windows.Forms.TextBox playersMaxTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.ListBox sendListBox;
    }
}

