using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;
using OpenTK;
using System.Collections.Generic;
using AP;

    /// <summary>
    /// Network manager specific to the game host.
    /// </summary>
    public class HostManager : NetManager
    {


		#region Constructors (1) 

        public HostManager(int port, ref GameState State): base(port, ref State)
        {
            new Thread(new ThreadStart(this.Listen)).Start();//Collect clients in our connection pool
            bool startGame = false;
        }

		#endregion Constructors 

		#region Methods (2) 
		
        /// <summary>
        /// Handles the incoming comm.
        /// </summary>
        /// <param name="connection">The connection.</param>
        protected override void HandleIncomingComm(object connection)
        {
            Connection myConnection = (Connection)connection;

            client = myConnection.GetClient();

  

            while (true)
            {
                NetPackage pack = new NetPackage();
                //Console.WriteLine("Size of game state: {0}", State.Enemies.Count + State.Players.Count);
                try
                {
                    //read package data
                    pack = myConnection.ReadPackage();
                    packetSwitcher(pack, myConnection);
                }
                catch (Exception e)
                {
                    //a socket error has occured
                    Console.WriteLine(e.ToString());
                    MessageBox.Show("Error: " + e.Message);
                    Environment.Exit(0);
                    break;
                }
            }

            lock (this)
            {
                myConnections.Remove(myConnection);
            }

        }

		#endregion Methods 
    }

