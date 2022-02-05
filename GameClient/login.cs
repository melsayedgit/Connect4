using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class login : Form
    {
        lobby start_lobby;
        public string playername;
        public login()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            playername = textBox1.Text;
            start_lobby = new lobby();
            start_lobby.Show();
            this.Hide();
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                playername = textBox1.Text;
                start_lobby = new lobby();
                start_lobby.Show();
                this.Hide();
            }
        }
    }
}
