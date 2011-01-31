using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace NetLib
{
	
	public enum Action {
		Create = 0x60000000,
		Update = 0x10000000,
		Delete = 0x20000000
	}
	
	public enum Type {
		Player = 0x00000000,
		Enemy = 0x01000000,
		Circle = 0x03000000
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
				UInt32 count = (UInt32)ObjSet.Count;
				count = count<<16;
				UInt32 header = count^action^ObjSet[0].type;
				Console.WriteLine("Sending Header: {0} Count: {1} Action: {2} Type: {3}",header,count,action,ObjSet[0].type);
				buffer.Add(BitConverter.GetBytes(header));
				
				foreach(GameObj O in ObjSet)
				{
					foreach(byte[] b32 in O.Export())
					{
						Console.WriteLine("Data!");
						buffer.Add(b32);
					}
					
				}
				
			}
			
			Console.WriteLine("Buffer Size: {0}",buffer.Count);
			
			//write buffer to all clients
			foreach(TcpClient client in myConnections)
			{
				Console.WriteLine("Connection Found!");
				NetworkStream clientStream = client.GetStream();
				foreach(byte[] b in buffer)
				{
					Console.WriteLine("Writing: {0}",BitConverter.ToInt32(b,0));
					clientStream.Write(b,0,4);
				}
				clientStream.Flush();
			}

		}
		
		//communication is handled differently in the lobby/client children classes
		protected abstract void HandleIncomingComm(Object remoteEnd);
		
		
		public UInt32 GetSize(UInt32 type)
		{
			switch (type)
			{
			case ((UInt32)Type.Player)://player
				return 8;
			case (UInt32)Type.Enemy://AI
				return 8;
			case 2://building
				return 8;;
			case 3://circle
				return 6;
			case 4://explosion
				return 8;
			case 5://power-up
				return 8;
			default:
				return 0;
			}
		}
	}
	
}

