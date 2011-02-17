using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetLib;
using AP.Network;

namespace AP.Forms
{
    public partial class Lobby : Form
    {
        private LobbyManager lobbyManager;
        private ClientManager clientManager;
        /// <summary>
        /// This lobby constructor is used when joining the lobby
        /// </summary>
        public Lobby(Server s)
        {
            InitializeComponent();
            GameState state = new GameState();
            clientManager = new ClientManager(9999, ref state, s);
            btn_Start.Enabled = false;
        }
        /// <summary>
        /// This lobby constructor is used for creating a lobby
        /// </summary>
        /// <param name="_Name"></param>
        public Lobby(string _Name)
        {
            var port = 9999;
            InitializeComponent();
            lbl_Name.Text = _Name;
            lobbyManager = new LobbyManager(port, ref lst_Players) { LobbyName = _Name };
            
        }


        private void btn_Start_Click(object sender, EventArgs e)
        {
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            //lobbyManager.listener.Close();
            this.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
