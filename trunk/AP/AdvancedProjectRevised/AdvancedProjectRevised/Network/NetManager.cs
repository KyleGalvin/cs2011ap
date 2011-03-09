using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using AP;

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
		protected String myRole;
		protected int port;
		protected Thread respondThread;
        static public GameState State;
        protected PackWorker worker;

		#endregion Fields 

		#region Constructors (2) 

		public NetManager(int newPort)
		{
            myOutgoing = new Queue<NetPackage>();
			myConnections = new List<Connection>();
            myProtocol = new PackageInterpreter();
			port = newPort;
		}

	    protected NetManager()
	    {
	        //throw new NotImplementedException();
	    }

		#endregion Constructors 

		#region Methods (4) 

		// Public Methods (3) 

        //protected NetManager(int newPort)
        //{
        //    //throw new NotImplementedException();
        //    myConnections = new List<Connection>();
        //    myProtocol = new PackageInterpreter();
        //    port = newPort;
        //}
	    //Listen for any requests directed at our IP and Port
		//respond by accepting connection and requesting one of our own for outgoing data
        /// <summary>
        /// Listens this instance.
        /// </summary>
		public void Listen()
		{
			
			IPEndPoint lep = new IPEndPoint(IPAddress.Any,port);
			
			TcpListener myListener = new TcpListener(lep);
			
			myListener.Start();
			
			
			while (true)
			{
				
				//wait for new incoming connection
				client = myListener.AcceptTcpClient();
                Connected = true;
				
				//We cannot be sending out on our connections while we add a new one
				//since the list length cannot change while we iterate through the list
				lock(this)
				{
					myConnections.Add(new Connection(client));
				}
                
                Connection lastCon = myConnections[myConnections.Count - 1];
                lastCon.myStream.myPackage.isLobby = IsLobby;


				
				Console.WriteLine("Starting the {0}th connection",myConnections.Count);
				//create a thread to handle communication
				Thread clientThread = new Thread(new ParameterizedThreadStart(HandleIncomingComm));
				clientThread.Start(myConnections[(myConnections.Count -1)]);
			}
		}

        /// <summary>
        /// Switches the packet.
        /// </summary>
        /// <param name="pack">The pack.</param>
        public void packetSwitcher(NetPackage pack)
        {
            Console.WriteLine("PACKET SWITCHED");
            if (pack.IsComplete())//we've accumulated the amount of data our header predicts
            {
                if (pack.action == (UInt32)Action.Text)
                {
                    string Message = worker.HandleText(pack);
                    Console.WriteLine(Message);
                }
                if (pack.action == (UInt32)Action.Create)
                {
                    worker.HandleCreate(pack);
                    Console.WriteLine("Create command triggered by incoming packet header");
                }
                if (pack.action == (UInt32)Action.Update)
                {

                }
                if (pack.action == (UInt32)Action.Request)
                {
                    worker.HandleRequest(pack);
                }
                if (pack.action == (UInt32)Action.Describe)
                {
                    worker.HandleDescribe(pack);
                }

            }
        }

        /// <summary>
        /// Sends the objs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a">A.</param>
        /// <param name="Objs">The objs.</param>
        public void SendObjs<T>(Action a,List<T> Objs)
        {
            List<byte[]> data = myProtocol.encodeObjs(a, Type.Player, Objs);
            foreach (Connection c in myConnections)
            {
                //Console.WriteLine("Writing model to stream: {0}",BitConverter.ToString(data[0],0)  );
                c.Write(data);
            }
        }
		// Protected Methods (1) 

		//communication is handled differently in the lobby/client children classes
		protected abstract void HandleIncomingComm(Object remoteEnd);

		#endregion Methods 
	}
	


