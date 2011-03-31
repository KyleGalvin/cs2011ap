using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using AP;
using Timer = System.Threading.Timer;
using OpenTK;
using System.Linq;
using System.Timers;
using System.Text;

    /// <summary>
    /// The network driver. Basically the network interface
    /// </summary>
	public abstract class NetManager
	{
		#region Fields (12) 

        protected TcpClient client;
        public bool Connected = false;
        protected bool IsLobby;
		public List<Connection> myConnections;
		protected List<byte[]> myData;
        public Queue<NetPackage> myOutgoing;
        protected PackageInterpreter myProtocol;
		public static String myRole;
		protected int port;
		protected Thread respondThread;
        protected PackWorker worker;
        public  GameState State;
        private Thread broadcastThread;
        private List<IPAddress> ServerIps = new List<IPAddress>();
        protected DateTime lastFrameTime;
        private bool TimesUp;
        private bool done = false;
        private bool change = false;
        private int playerUID = -1;

		#endregion Fields 

		#region Constructors (2) 

		public NetManager(int newPort,  ref GameState StateRef)
		{
            myOutgoing = new Queue<NetPackage>();
			myConnections = new List<Connection>();
            myProtocol = new PackageInterpreter();
			port = newPort;
            worker = new PackWorker(ref StateRef);
            State = StateRef;
            lastFrameTime = DateTime.Now;
		}

	    protected NetManager()
	    {
	        //throw new NotImplementedException();
	    }

		#endregion Constructors 

		#region Methods (4) 


        private IPEndPoint FindServer(int port, IPEndPoint broadcastEP)
        {
            //broadcast
            SendBroadcast(broadcastEP);
            //listen

            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);
            // Create a timer with a ten second interval.
            System.Timers.Timer aTimer = new System.Timers.Timer(10000);
            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Enabled = true;
            aTimer.AutoReset = false;
            aTimer.Start();
            try
            {
                broadcastThread = new Thread(new ThreadStart(ListenforBroadCast));
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
            return ServerIps.Count == 0 ? null : new IPEndPoint(ServerIps.First(), port);
        }

        /// <summary>
        /// Listenfors the broad cast.
        /// </summary>
        private void ListenforBroadCast()
        {
            bool once = false;
            IPAddress broadcast = IPAddress.Parse("192.168.105.211");
            IPEndPoint broadcastEP = new IPEndPoint(broadcast, port);
            string msg = String.Empty;
            UdpClient listener = new UdpClient(port);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);
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
                if (msg == "Server Broadcast")
                {
                    Console.WriteLine("Adding " + groupEP.Address.ToString() + " to the list of clients");
                    ServerIps.Add(groupEP.Address);
                    TimesUp = true;
                }
            }
        }

        /// <summary>
        /// Called when [timed event].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (broadcastThread.IsAlive)
            {
                broadcastThread.Suspend();
            }
            Console.WriteLine("Listening Stopped (Timeout Reached)");
            TimesUp = true;
        }

        private void SendBroadcast(IPEndPoint broadcastEP)
        {
            Socket BC = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

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

        public void SyncState()
        {
            SyncStateOutgoing();//send relevent data out to connections
        }

        public void SyncStateOutgoing()
        {
            GameState s = State;
            List<Enemy> enemyUpdateList = new List<Enemy>();
            List<Enemy> enemyAddList = new List<Enemy>();
            List<Enemy> enemyDeleteList = new List<Enemy>();
            List<Bullet> bulletUpdateList = new List<Bullet>();
            List<Bullet> bulletAddList = new List<Bullet>();
            List<Bullet> bulletDeleteList = new List<Bullet>();
            List<Player> playerUpdateList = new List<Player>();
            List<Player> playerAddList = new List<Player>();
            List<Player> playerDeleteList = new List<Player>();

            foreach (Bullet b in s.Bullets.OrderBy(y=>y.timestamp))
            {
                if (b.timestamp == -1)
                {
                    bulletDeleteList.Add(b);
                    State.Bullets.Remove(b);
                }
                else if (b.timestamp == 0)
                {
                    bulletAddList.Add(b);
                }
                else if (b.timestamp >= lastFrameTime.Ticks)
                {
                    bulletUpdateList.Add(b);
                    b.timestamp = -2; 
                }
                
                if( b.timestamp >= 0 )
                    b.timestamp = DateTime.Now.Ticks;
            }

            for (int x = 0; x < s.Players.Count; x++)
            {
                if (s.Players[x].timestamp > 0)
                    s.Players[x].updateTimeStamp();
            }

            foreach (Player p in s.Players.OrderBy(y=>y.timestamp))
            {
                if (p.timestamp >= lastFrameTime.Ticks)
                {
                    //Console.WriteLine("PLAYERMANAGER UPDATE for player" +p.playerId +" timestamp is : " +p.timestamp + " Last ticks:" + lastFrameTime.Ticks);
                    if (p.prevXPos != p.xPos || p.prevYPos != p.yPos)
                    {
                        playerUpdateList.Add(p);
                        p.prevYPos = p.yPos;
                        p.prevXPos = p.xPos;
                    }
                }
                else if (p.timestamp == 0)
                {
                    //Console.WriteLine("PLAYERMANAGER CREATEfor player" + p.playerId + " timestamp is : " + p.timestamp + " Last ticks:" + lastFrameTime.Ticks);
                    playerAddList.Add(p);
                    //State.Players.Add(p);
                    p.updateTimeStamp();

                }
                else if (p.timestamp == -1)
                {
                    playerDeleteList.Add(p);
                }
            }
            /*foreach (Enemy e in s.Enemies)
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
            }*/
            if (playerAddList.Count > 0)
                this.SendObjs<Player>(Action.Create, playerAddList, Type.Player);
            if (playerUpdateList.Count > 0)
                this.SendObjs<Player>(Action.Update, playerUpdateList, Type.Player);
            if (bulletDeleteList.Count > 0)
                this.SendObjs<Bullet>(Action.Delete, bulletDeleteList, Type.Bullet);
            if (bulletAddList.Count > 0)
                this.SendObjs<Bullet>(Action.Create, bulletAddList, Type.Bullet);
            if (bulletUpdateList.Count > 0)
                this.SendObjs<Bullet>(Action.Update, bulletUpdateList, Type.Bullet);

            lastFrameTime = DateTime.Now;
        }
        public void setRole(String role)
        {
            myRole = role;
        }

        public string getRole()
        {
            return myRole;
        }

		// Public Methods (3) 

	    //Listen for any requests directed at our IP and Port
		//respond by accepting connection and requesting one of our own for outgoing data
        /// <summary>
        /// Listens this instance.
        /// </summary>
        public void Listen()
        {

            IPEndPoint lep = new IPEndPoint(IPAddress.Any, port);

            TcpListener myListener = new TcpListener(lep);

            myListener.Start();
            bool createdIncomingCommThread = false;

            while (true)
            {
                Player p;
                List<Player> tempPlayerList = new List<Player>();
                //wait for new incoming connection
                client = myListener.AcceptTcpClient();
                Connected = true;
                Console.WriteLine("COUNT: " + myConnections.Count);
                //We cannot be sending out on our connections while we add a new one
                //since the list length cannot change while we iterate through the list
                lock (this)
                {
                    myConnections.Add(new Connection(client));

                }

                Connection lastCon = myConnections[myConnections.Count - 1];
                lastCon.myStream.myPackage.isLobby = IsLobby;

                Console.WriteLine("Starting the {0}th connection", myConnections.Count);
                //create a thread to handle communication
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleIncomingComm));
                    clientThread.Start(myConnections[(myConnections.Count - 1)]);

                if (Connected && String.Compare(myRole, "server") == 0)
                {
                    for (int i = 0; i < myConnections.Count; i++)
                    {
                        if (myConnections[i].playerUID < 0)
                        {
                            myConnections[i].playerUID = i;
                            p = new Player(new Vector3(0.0f, 0.0f, 0), i);
                            p.timestamp = 0;
                            State.Players.Add(p);
                            tempPlayerList.Add(p);
                        }
                    }
                    this.SendObjs<Player>(Action.Identify, tempPlayerList, Type.Connection, myConnections[myConnections.Count - 1]);
                }


            }
        }

        /// <summary>
        /// Switches the packet.
        /// </summary>
        /// <param name="pack">The pack.</param>
        public void packetSwitcher(NetPackage pack, Connection callerConnection)
        {
            Console.WriteLine("PACKET SWITCHED");
            if (pack.IsComplete())//we've accumulated the amount of data our header predicts
            {
                if (pack.action == (UInt32)Action.Delete)
                {
                    worker.HandleDelete(pack);
                }
                if (pack.action == (UInt32)Action.Text)
                {
                    string message = worker.HandleText(pack);
                    Console.WriteLine(message);
                }
                if (pack.action == (UInt32)Action.Create)
                {
                    worker.HandleCreate(pack);
                 
                    Console.WriteLine("Create command triggered by incoming packet header");
                }
                if (pack.action == (UInt32)Action.Update)
                {
                    worker.HandleUpdate(pack);
                }
                if (pack.action == (UInt32)Action.Request)
                {
                    worker.HandleRequest(pack, callerConnection);
                }
                if (pack.action == (UInt32)Action.Describe)
                {
                    worker.HandleDescribe(pack);
                }
                if (pack.action == (UInt32)Action.Identify)
                {
                    myConnections[0].playerUID = worker.HandleIdentify(pack);
                }

            }
        }

        /// <summary>
        /// Sends the objs. (server side specific)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a">A.</param>
        /// <param name="Objs">The objs.</param>
        public void SendObjs<T>(Action a, List<T> Objs, Type objType)
        {
            List<byte[]> data = myProtocol.encodeObjs(a, objType, Objs);
            for (int i = 0; i < myConnections.Count; i++)
            {
                myConnections[i].Write(data);
            }
        }
        /// <summary>
        /// Sends the objs. (client side specific)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a">A.</param>
        /// <param name="Objs">The objs.</param>
        public void SendObjs<T>(Action a, List<T> Objs, Type objType, Connection myConnection)
        {
            List<byte[]> data = myProtocol.encodeObjs(a, objType, Objs);
            myConnection.Write(data);
        }
		// Protected Methods (1) 

		//communication is handled differently in the lobby/client children classes
		protected abstract void HandleIncomingComm(Object remoteEnd);

		#endregion Methods 
	}
	


