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

        /*public override void SyncState(GameState s)
        {
            List<Enemy> enemyUpdateList = new List<Enemy>();
            List<Enemy> enemyAddList = new List<Enemy>();
            List<Enemy> enemyDeleteList = new List<Enemy>();
            List<Bullet> bulletUpdateList = new List<Bullet>();
            List<Bullet> bulletAddList = new List<Bullet>();
            List<Bullet> bulletDeleteList = new List<Bullet>();
            List<Player> playerUpdateList = new List<Player>();
            List<Player> playerAddList = new List<Player>();
            List<Player> playerDeleteList = new List<Player>();

            foreach (Bullet b in s.Bullets)
            {
                if (b.timestamp > lastFrameTime.Ticks)
                {
                    bulletUpdateList.Add(b);
                    this.SendObjs<Bullet>(Action.Update, bulletUpdateList, Type.Bullet);
                }
                else if (b.timestamp == 0)
                {
                    bulletAddList.Add(b);
                    this.SendObjs<Bullet>(Action.Create, bulletAddList, Type.Bullet);
                }
                else if (b.timestamp == -1)
                {
                    bulletDeleteList.Add(b);
                    this.SendObjs<Bullet>(Action.Delete, bulletDeleteList, Type.Bullet);
                }
                b.timestamp = DateTime.Now.Ticks;
            }
            foreach (Player p in s.Players)
            {
                if (p.timestamp > lastFrameTime.Ticks)
                {
                    playerUpdateList.Add(p);
                    this.SendObjs<Player>(Action.Update, playerUpdateList, Type.Player);
                }
                else if (p.timestamp == 0)
                {
                    playerAddList.Add(p);
                    this.SendObjs<Player>(Action.Create, playerAddList, Type.Player);
                }
                else if (p.timestamp == -1)
                {
                    playerDeleteList.Add(p);
                    this.SendObjs<Player>(Action.Delete, playerDeleteList, Type.Player);
                }
                p.timestamp = DateTime.Now.Ticks;
            }
            foreach (Enemy e in s.Enemies)
            {
                if (e.timestamp > lastFrameTime.Ticks)
                {
                    enemyUpdateList.Add(e);
                }
                else if (e.timestamp == 0)
                {
                    enemyAddList.Add(e);
                }
                else if (e.timestamp == -1)
                {
                    enemyDeleteList.Add(e);
                }
                e.timestamp = DateTime.Now.Ticks;
            }
            this.SendObjs<Enemy>(Action.Update, enemyUpdateList, Type.AI);
            this.SendObjs<Player>(Action.Create, playerUpdateList, Type.Player);
            this.SendObjs<Bullet>(Action.Delete, bulletUpdateList, Type.Bullet);
            this.SendObjs<Enemy>(Action.Update, enemyAddList, Type.AI);
            this.SendObjs<Player>(Action.Create, playerAddList, Type.Player);
            this.SendObjs<Bullet>(Action.Delete, bulletAddList, Type.Bullet);
            this.SendObjs<Enemy>(Action.Update, enemyDeleteList, Type.AI);
            this.SendObjs<Player>(Action.Create, playerDeleteList, Type.Player);
            this.SendObjs<Bullet>(Action.Delete, bulletDeleteList, Type.Bullet);
            lastFrameTime = DateTime.Now;
        }*/

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
                Console.WriteLine("Size of game state: {0}", State.Enemies.Count + State.Players.Count);
                try
                {
                    //read package data
                    Console.WriteLine("attempt to read pack:");
                    pack = myConnection.ReadPackage();
                    Console.WriteLine("Package recieved!");
                    Console.WriteLine("incoming x:" + pack.body[1] + " incoming y:" + pack.body[2]);
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

