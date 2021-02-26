using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Collections;

namespace ProtocolParser
{
    public delegate string handlingFunction(message msg);
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            Hashtable n = new Hashtable();
            //n.Add("playerLogin", new Form1(this.mojaf) );
            //fun();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "XML (*.xml)|*.xml";
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string file = openFileDialog1.FileName.ToString();
                message p = XmlParser.parseXml(file);
                Hashtable h = new Hashtable();
                //h.Add("playerLogin", new handlingFunction());
                MessageProcessor msgproc = new MessageProcessor(h);
                msgproc.handle(p);

                //String s = ((protocolMessagePlayerLogin)(p.message[0].Items[0])).nick + " " + ((protocolMessagePlayerLogin)(p.message[0].Items[0])).gameType;
                //MessageBox.Show(s);

            }
        }
    }
}
