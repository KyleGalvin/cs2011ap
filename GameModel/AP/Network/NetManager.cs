using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using AP;

namespace NetLib
{
	public abstract class NetManager
	{
        protected TcpClient client;
        static public GameState State;
		protected Thread respondThread;
		protected int port;
        protected bool IsLobby;
		public List<Connection> myConnections;
        public Queue<NetPackage> myOutgoing;
		
		protected List<byte[]> myData;
		protected String myRole;

        protected PackageInterpreter myProtocol;

        protected PackWorker worker;

        public bool Connected = false;
		
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

        //protected NetManager(int newPort)
        //{
        //    //throw new NotImplementedException();
        //    myConnections = new List<Connection>();
        //    myProtocol = new PackageInterpreter();
        //    port = newPort;
        //}

	    //Listen for any requests directed at our IP and Port
		//respond by accepting connection and requesting one of our own for outgoing data
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

        public void SendObjs<T>(Action a,List<T> Objs)
        {
            List<byte[]> data = myProtocol.encodeObjs(a, Type.Player, Objs);
            foreach (Connection c in myConnections)
            {
                //Console.WriteLine("Writing model to stream: {0}",BitConverter.ToString(data[0],0)  );
                c.Write(data);
            }
        }
	
		//communication is handled differently in the lobby/client children classes
		protected abstract void HandleIncomingComm(Object remoteEnd);

	}
	
}

