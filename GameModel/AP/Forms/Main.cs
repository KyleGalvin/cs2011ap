using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using AP.Network;
using NetLib;

namespace AP.Forms
{
    public partial class Main : Form
    {
        private List<Server> ServerIps = new List<Server>();
        private Thread broadcastThread;
        private Thread broadcastThread2;
        private bool TimesUp;
        private int port = 9999;
        private bool checkServers = false;
        IPAddress broadcast = IPAddress.Parse("192.168.105.255");
        private IPEndPoint broadcastEP;
        private UdpClient listener;
        private IPEndPoint groupEP;
        private bool listen;
        public Main()
        {
            InitializeComponent();
            //These need to be initialized out here otherwise it thinks the socket is already open
            lst_Servers.DataSource = ServerIps;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            if (checkServers) return;

            if (!listen)
            {
                //These need to be set for the server check to run correctly
                broadcastEP = new IPEndPoint(broadcast, port);
                listener = new UdpClient(port);
                groupEP = new IPEndPoint(IPAddress.Any, port);
                listen = true;
            }
            //This makes sure the thread only runs one at a time.
            checkServers = true;
            broadcastThread2 = new Thread(FetchServers);
            broadcastThread2.Start();
        }

        private void FetchServers()
        {
            while (checkServers)
            {
                //Try to connect to server via broadcast
                FindServer(port, broadcastEP);
                //Once the thread stops we check this flag to allow it to check again
                checkServers = false;
                
            }
        }
        /// <summary>
        /// This handles the display of the lobby windows when hosting a game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Host_Click(object sender, EventArgs e)
        {
            if(listener!=null) listener.Close();
            DialogResult dr;
            Lobby_Dialog lobbyDialog = new Lobby_Dialog();
            dr = lobbyDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                Lobby lobby = new Lobby(lobbyDialog.Name);
                DialogResult dr2;
                Hide();
                dr2 = lobby.ShowDialog();
                if (dr2 == DialogResult.Cancel)
                {
                    Show();
                }
                else if(dr2==DialogResult.OK)
                {//Starts the game
                    //todo Maybe it should keep the window open in case we want to come back
                    Hide();
                    using (Program game = new Program())
                    {
                        game.Run(28.0);
                    }
                }
            }
        }
        private void btn_Join_Click(object sender, EventArgs e)
        {
            if (lst_Servers.SelectedIndex > 0)
            {
                MessageBox.Show("Select a game");
                return;
            }
            joinLobby();
        }
        /// <summary>
        /// Called when the user selects a lobby to join
        /// </summary>
        private void joinLobby()
        {
            //Open the lobby with the clients view. Use a different initializer.

        }

        private void lst_Servers_DoubleClick(object sender, EventArgs e)
        {
            if (lst_Servers.SelectedIndex > 0)
            {
                MessageBox.Show("Select a game");
                return;
            }
            joinLobby();
        }
        #region "Broadcast Functions"
        /// <summary>
        /// automatically find server on subnet
        /// </summary>
        /// <param name="port"></param>
        /// <param name="broadcastEP"></param>
        /// <returns></returns>
        public List<Server> FindServer(int port, IPEndPoint broadcastEP)
        {
            //broadcast
            SendBroadcast(broadcastEP);
            //listen
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);
            // Create a timer with a ten second interval.
            System.Timers.Timer aTimer = new System.Timers.Timer(10000);
            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;
            aTimer.AutoReset = false;
            aTimer.Start();
            try
            {
                broadcastThread = new Thread(ListenforBroadCast);
                broadcastThread.Start();

                Console.WriteLine("Starting broadcast thread");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            while (!TimesUp)
            {
                //Place holder need to wait until the timer is done
            }
            return ServerIps;
        }

        private void SendBroadcast(IPEndPoint broadcastEP)
        {
            Socket BC = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            UTF8Encoding encoding = new UTF8Encoding();

            try
            {
                BC.SendTo(encoding.GetBytes("Client Broadcast"), broadcastEP);
                Console.WriteLine("Broadcast sent");
            }
            catch
            {
                Console.WriteLine("Could not broadcast request for server");
                Console.WriteLine("Broadcast May be disabled...");
            }
        }

        private void ListenforBroadCast()
        {
            bool once = false;
            string msg = String.Empty;
            while (!TimesUp)
            {
                if (!once)
                {
                    SendBroadcast(broadcastEP);
                    once = true;
                }
                Console.WriteLine("Waiting for broadcast again {0}", groupEP.ToString());
                byte[] bytes = listener.Receive(ref groupEP);
                msg = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                if (msg[0] == 'S')
                {
                    Console.WriteLine("Adding " + groupEP.Address.ToString() + " to the list of clients");
                    ServerIps.Add(new Server(msg.Substring(1), groupEP.Address));
                    TimesUp = true;
                }
            }
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Listening Stopped (Timeout Reached)");
            TimesUp = true;
            if (broadcastThread.IsAlive)
            {
                broadcastThread.Suspend();
            }
        }
        #endregion
    }
}
