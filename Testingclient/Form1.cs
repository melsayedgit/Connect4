using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace client
{
    public partial class Form1 : Form
    {
        byte[] bt = new byte[] { 127, 0, 0, 1 };
        IPAddress add;
        NetworkStream nstream;
        BinaryReader br;
        BinaryWriter bw;

        public Form1()
        {
            InitializeComponent();
        }

        private void connect_Click(object sender, EventArgs e)
        {
            add = new IPAddress(bt);
            TcpClient client = new TcpClient();
            client.Connect(add, 2222);
            nstream = client.GetStream();
            //MessageBox.Show("connected");
            br = new BinaryReader(nstream);
            bw = new BinaryWriter(nstream);

            bw.Write("100," + nametextbox.Text);
        }

        private void sendRequest_Click(object sender, EventArgs e)
        {
            bw.Write(textBox1.Text);
            //MessageBox.Show("message sent");
            textBox1.Text = "";
        }

        private void receiveBtn_Click(object sender, EventArgs e)
        {
            //int[,] arr = new int[3, 4];
            //MessageBox.Show(arr.ToString());
            string request;
            request = br.ReadString();
            requestsList.Items.Add(request);
        }
    }
}
