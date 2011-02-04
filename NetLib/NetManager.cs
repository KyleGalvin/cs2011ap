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
		
		public NetManager(int newPort)
		{
			myConnections = new List<Connection>();
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
		
		public void Send(Dictionary<String,List<GameObj>> outgoing, UInt32 action)
		{
			//Create buffer to fill and send over network
			List<byte[]> buffer = new List<byte[]>();
			
			//fill the buffer with our outgoing data
			foreach(KeyValuePair<String,List<GameObj>> p in outgoing)
			{
				
				List<GameObj> ObjSet = p.Value;
				//craft packet header
				UInt32 count = (UInt32)ObjSet.Count;
				count = count<<16;
				UInt32 header = count^action^ObjSet[0].type;
				Console.WriteLine("Sending Header: {0} Count: {1} Action: {2} Type: {3}",header,count>>16,action>>28,ObjSet[0].type>>24);
				buffer.Add(BitConverter.GetBytes(header));
				
				foreach(GameObj O in ObjSet)
				{
					List<byte[]> oNetData = O.Export();
					foreach(byte[] b32 in oNetData)
					{
						Console.WriteLine("Data!");
						buffer.Add(b32);
					}
					
				}
				
			}
			
			Console.WriteLine("Buffer Size: {0}",buffer.Count);
			
			//write buffer to all clients
			foreach(Connection c in myConnections)
			{
				c.Write(buffer);
			}

		}
		
		//communication is handled differently in the lobby/client children classes
		protected abstract void HandleIncomingComm(Object remoteEnd);
	}
	
}

