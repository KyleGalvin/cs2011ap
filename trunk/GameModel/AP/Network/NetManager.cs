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
        static public GameState State = new GameState();
		protected Thread respondThread;
		protected int port;
		
		public List<Connection> myConnections;
		
		protected List<byte[]> myData;
		protected String myRole;

        protected PackageInterpreter myProtocol;

        protected PackWorker worker;

        public bool Connected = false;
		
		public NetManager(int newPort,  ref GameState StateRef)
		{
            worker = new PackWorker(ref StateRef);
            State = StateRef;
			myConnections = new List<Connection>();
            myProtocol = new PackageInterpreter();
			port = newPort;
		}
		//Listen for any requests directed at our IP and Port
		//respond by accepting connection and requesting one of our own for outgoing data
		public void Listen()
		{
			
			IPEndPoint lep = new IPEndPoint(IPAddress.Any,port);
			
			TcpListener myListener = new TcpListener(lep);
			
			myListener.Start();
			TcpClient client = new TcpClient();
			
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
				
				Console.WriteLine("Starting the {0}th connection",myConnections.Count);
				//create a thread to handle communication
				Thread clientThread = new Thread(new ParameterizedThreadStart(HandleIncomingComm));
				clientThread.Start(myConnections[(myConnections.Count -1)]);
			}
		}

        public void Send<T>(List<T> Objs)
        {
            List<byte[]> data = myProtocol.encode(Action.Create, Type.AI, Objs);
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

