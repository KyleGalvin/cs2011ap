using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace NetLib
{
	
	abstract class NetManager
	{
		protected Thread respondThread;
		protected int port;
		
		protected List<Connection> myConnections;
		
		protected List<byte[]> myData;
		protected String myRole;

        protected PackageInterpreter myProtocol;
		
		public NetManager(int newPort)
		{
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
		
		//communication is handled differently in the lobby/client children classes
		protected abstract void HandleIncomingComm(Object remoteEnd);
	}
	
}

