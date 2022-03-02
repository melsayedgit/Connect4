﻿using Client.Popups;
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
        private bool firstlogin = true;

        public login()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text=="")
            {
                message ms = new message();
                ms.msg = "you need to Enter \n a user Name to login";
                DialogResult res = ms.ShowDialog();
            }
            else
            {
                playername = textBox1.Text;

                if (firstlogin)
                {
                    try
                    {
                        GameManger.Login(playername);
                        firstlogin = false;
                        //start_lobby = new lobby();
                        //start_lobby.Show();
                        //this.Hide();
                    }

                    catch (Exception)
                    {
                     
                        message ms = new message();
                        ms.msg = "The Server is Offline \n Please try again later";
                        DialogResult res = ms.ShowDialog();
                    }
                    try
                    {
                        if (GameManger.isloginSuc(playername))
                        {
                            start_lobby = new lobby();
                            start_lobby.Text += "- " + playername;
                            GameManger.recieve = new Task(GameManger.ReceiveServerRequest);
                            GameManger.recieve.Start();
                            GameManger.SendServerRequest(Flag.getPlayers);
                            GameManger.SendServerRequest(Flag.getRooms);

                            start_lobby.Show();
                            this.Hide();
                        }



                    }
                    catch (Exception)
                    {

                   
                    }


                }

                else
                {
                    GameManger.SendServerRequest(Flag.sendLoginInfo, playername);
                    if (GameManger.isloginSuc(playername))
                    {
                        start_lobby = new lobby();
                        start_lobby.Text += "- " + playername;
                        GameManger.recieve = new Task(GameManger.ReceiveServerRequest);
                        GameManger.recieve.Start();
                        GameManger.SendServerRequest(Flag.getPlayers);
                        GameManger.SendServerRequest(Flag.getRooms);

                        start_lobby.Show();
                        this.Hide();

                        //start_lobby = new lobby();
                        //start_lobby.Show();
                        //this.Hide();
                    }
                }
            }

            
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text == "")
                {
                    message ms = new message();
                    ms.msg = "you need to Enter \n a user Name to login";
                    DialogResult res = ms.ShowDialog();
                }
                else
                {
                    playername = textBox1.Text;

                    if (firstlogin)
                    {
                        try
                        {
                            GameManger.Login(playername);
                            firstlogin = false;
                            //start_lobby = new lobby();
                            //start_lobby.Show();
                            //this.Hide();
                        }

                        catch (Exception)
                        {

                            message ms = new message();
                            ms.msg = "The Server is Offline \n Please try again later";
                            DialogResult res = ms.ShowDialog();
                        }
                        try
                        {
                            if (GameManger.isloginSuc(playername))
                            {
                                start_lobby = new lobby();
                                start_lobby.Text += "- " + playername;
                                GameManger.recieve = new Task(GameManger.ReceiveServerRequest);
                                GameManger.recieve.Start();
                                GameManger.SendServerRequest(Flag.getPlayers);
                                GameManger.SendServerRequest(Flag.getRooms);

                                start_lobby.Show();
                                this.Hide();
                            }



                        }
                        catch (Exception)
                        {


                        }


                    }

                    else
                    {
                        GameManger.SendServerRequest(Flag.sendLoginInfo, playername);
                        if (GameManger.isloginSuc(playername))
                        {
                            start_lobby = new lobby();
                            start_lobby.Text += "- " + playername;
                            GameManger.recieve = new Task(GameManger.ReceiveServerRequest);
                            GameManger.recieve.Start();
                            GameManger.SendServerRequest(Flag.getPlayers);
                            GameManger.SendServerRequest(Flag.getRooms);

                            start_lobby.Show();
                            this.Hide();

                            //start_lobby = new lobby();
                            //start_lobby.Show();
                            //this.Hide();
                        }
                    }
                }

            }
        }




        //drag the borderless form 
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Login_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {


            try
            {
                if (GameManger.connStatues)
                {
                    GameManger.SendServerRequest(Flag.disconnect, "");
                }
            }
            catch (Exception)
            {


            }
        }
        // end code of dragable form
    }
}
