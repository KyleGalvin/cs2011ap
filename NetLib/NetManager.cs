using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace NetLib
{
	
	enum Action{
		Create = 0x00000000,
		Update = 0x10000000,
		Delete = 0x20000000
	}
	
	abstract class NetManager
	{
		protected Thread listenThread;
		protected Thread respondThread;
		protected TcpListener tcpListener;
		protected int port;
		
		protected List<TcpClient> myConnections;
		protected List<byte[]> myData;
		protected String myRole;
		
		public NetManager(int newPort)
		{
			myConnections = new List<TcpClient>();
			port = newPort;
		}
		
		//Listen for any requests directed at our IP and Port
		//respond by accepting connection and requesting one of our own for outgoing data
		protected void Listen()
		{
			tcpListener.Start();
			TcpClient client = new TcpClient();
			
			while (true)
			{
				//wait for new incoming connection
				client = tcpListener.AcceptTcpClient();
				
				//We cannot be sending out on our connections while we add a new one
				//since the list length cannot change while we iterate through the list
				lock(this)
				{
					myConnections.Add(client);
				}
				
				Console.WriteLine("Starting the {0}th connection",myConnections.Count);
				//create a thread to handle communication
				Thread clientThread = new Thread(new ParameterizedThreadStart(HandleIncomingComm));
				clientThread.Start(myConnections[(myConnections.Count -1)]);
			}
		}
		
		public void Send(Dictionary<String,List<GameObj>> outgoing, UInt32 action)
		{
			
			List<byte[]> buffer = new List<byte[]>();
			//fill the buffer with our outgoing data
			foreach(KeyValuePair<String,List<GameObj>> p in outgoing)
			{
				
				List<GameObj> ObjSet = p.Value;
				//craft packet header
				UInt32 size = ObjSet[0].netsize;
				UInt32 count = (UInt32)ObjSet.Count;
				count = count<<16;
				UInt32 header = count^size^action;
				Console.WriteLine("Sending Header: {0}",header);
				buffer.Add(BitConverter.GetBytes(header));
				
				foreach(GameObj O in ObjSet)
				{
					buffer.Add(O.Export());
				}
				
			}
			
			//write buffer to all clients
			foreach(TcpClient client in myConnections)
			{
				NetworkStream clientStream = client.GetStream();
				foreach(byte[] b in buffer)
				{
					Console.WriteLine("Writing: {0}",b);
					clientStream.Write(b,0,4);
				}
				clientStream.Flush();
			}

		}
		
		//communication is handled differently in the lobby/client children classes
		protected abstract void HandleIncomingComm(Object remoteEnd);
		
	}
	
}

