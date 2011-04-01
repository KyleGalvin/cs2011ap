using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using OpenTK;
using System.Collections.Generic;
using AP;

    /// <summary>
    /// Network manager specific to the game host.
    /// </summary>
    public class HostManager : NetManager
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
		
        /// <summary>
        /// Handles the incoming comm.
        /// </summary>
        /// <param name="connection">The connection.</param>
        protected override void HandleIncomingComm(object connection)
        {
            Connection myConnection = (Connection)connection;

            client = myConnection.GetClient();

            Console.WriteLine("client {0} has connected.", client.Client.RemoteEndPoint);


            while (true)
            {
                NetPackage pack = new NetPackage();
                //Console.WriteLine("Size of game state: {0}", State.Enemies.Count + State.Players.Count);
                try
                {
                    //read package data
                    Console.WriteLine("attempt to read pack:");
                    pack = myConnection.ReadPackage();
                    packetSwitcher(pack, myConnection);
                    Console.WriteLine("Package recieved!");
                }
                catch (Exception e)
                {
                    //a socket error has occured
                    Console.WriteLine(e.ToString());
                    break;
                }

                //if (bytesRead == 0)//nothing was read from socket
                //{
                //	Console.WriteLine("Client {0} has disconnected.",client.Client.RemoteEndPoint);
                //	break;
                //}

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

