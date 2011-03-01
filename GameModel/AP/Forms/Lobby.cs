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
    /// <summary>
    /// The main lobby form players see when they have connected to a server.
    /// </summary>
    public partial class Lobby : Form
    {
		#region Fields (2) 

        private ClientManager clientManager;
        private LobbyManager lobbyManager;

		#endregion Fields 

		#region Constructors (2) 

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

		#endregion Constructors 

		#region Methods (3) 

		// Private Methods (3) 

        /// <summary>
        /// Handles the Click event of the btn_Close control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btn_Close_Click(object sender, EventArgs e)
        {
            //lobbyManager.listener.Close();
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btn_Start control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btn_Start_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the listBox1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

		#endregion Methods 
    }
}
