using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using OpenTK;
using AP;

    /// <summary>
    /// Network manager specific to the game host.
    /// </summary>
    public class HostManager : PlayerManager
    {
		#region Fields (1) 

        //HACK todo
        int i = 0;
		#endregion Fields 

		#region Constructors (1) 

        public HostManager(int port, ref GameState State): base(port, ref State)
        {
            new Thread(new ThreadStart(this.Listen)).Start();//Collect clients in our connection pool
            bool startGame = false;
        }

		#endregion Constructors 

		#region Methods (2) 
		// Protected Methods (1) 

        /// <summary>
        /// Handles the incoming comm.
        /// </summary>
        /// <param name="connection">The connection.</param>
        protected override void HandleIncomingComm(object connection)
        {
            Connection myConnection = (Connection)connection;
            NetPackage pack = new NetPackage();

            client = myConnection.GetClient();
            Player p = new Player(new Vector3(0.5f, 0.5f, 0), State.Players.Count);
            p.timestamp = 0;
            State.Players.Add( p );
           
            Console.WriteLine("client {0} has connected.", client.Client.RemoteEndPoint);

            while (true)
            {
                Console.WriteLine("Size of game state: {0}", State.Enemies.Count + State.Players.Count);
                try
                {
                    //read package data
                    Console.WriteLine("attempt to read pack:");
                    pack = myConnection.ReadPackage();
                    Console.WriteLine("Package recieved!");
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                //if (bytesRead == 0)//nothing was read from socket
                //{
                //	Console.WriteLine("Client {0} has disconnected.",client.Client.RemoteEndPoint);
                //	break;
                //}
                packetSwitcher(pack);
            }

            lock (this)
            {
                myConnections.Remove(myConnection);
            }

        }

		#endregion Methods 

        //ConnectGame(IPEndPoint host)
        //{
        //}
    }

