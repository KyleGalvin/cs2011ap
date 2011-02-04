using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace NetLib
{
	class ClientManager : NetManager
	{
		
		public ClientManager(int port):base(port)
		{
			//set up variables
			String IP = "";
			TcpClient client = new TcpClient();
			IPEndPoint broadcastEP = new IPEndPoint(IPAddress.Broadcast,port);
			IPEndPoint lep = new IPEndPoint(IPAddress.Any,port);
			
			//Try to connect to server via broadcast
			IPEndPoint serverEndPoint = FindServer(port,broadcastEP);
			
			if (serverEndPoint == broadcastEP){
				//server failed to respond. Our trickery failed. Ask for manual intervention
				Console.WriteLine("Please enter Lobby IP:");
				IP = Console.ReadLine();
				serverEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
			}
			
			Console.WriteLine("Waiting for connections...");
			client.Connect(serverEndPoint);
			
			lock(this){
				myConnections.Add(new Connection(client));
				Console.WriteLine("Connected to {0}",client.Client.RemoteEndPoint);
			}
			
			NetworkStream clientStream = client.GetStream();
			
			Console.WriteLine("Creating listener thread for reading server communications...");
			new Thread(new ThreadStart(this.Listen)).Start();
			
			Console.WriteLine("Creating new game model containing 2 enemies...");
			Dictionary<String,List<AP.Position>> Model = new Dictionary<String,List<AP.Position>>();
			List<AP.Position> Enemies = new List<AP.Position>();
			Model.Add("Enemies",Enemies);
			
			//create an enemy object to test communications with
			AP.Zombie baddie1 = new AP.Zombie(100,100,1);
			AP.Zombie baddie2 = new AP.Zombie(300,100,2);
			Enemies.Add(baddie1);
			Enemies.Add(baddie2);
			
			Console.WriteLine("Sending model to server. Action = Create for all 2 objects");
			//Send(Model,(UInt32)Action.Create);
		}
		
		//automatically find server on subnet
		private IPEndPoint FindServer(int port,IPEndPoint broadcastEP){
			//broadcast
			Socket BC = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);

			System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();
			
			try
			{
				BC.SendTo(encoding.GetBytes("Client Broadcast"),broadcastEP);
			}
			catch
			{
				Console.WriteLine("Could not broadcast request for server");
				Console.WriteLine("Broadcast May be disabled...");
			}
			
			//listen

			//get tcpclient for all
			
			//set server end point
				
			return broadcastEP;
		}
		
		protected override void HandleIncomingComm(object stream)
		{
			NetworkStream clientStream = (NetworkStream)stream;

			Console.WriteLine("Waiting for incoming messages");
				
			byte[] message = new byte[4];
			byte[] temp;
			
			List<byte[]> data = new List<byte[]>();
			int bytesRead;
			
			while (true)
			{
				bytesRead = 0;
			
			    try
			    {
					//blocks until a client sends a message
					bytesRead += clientStream.Read(message, 0, 4);
					
			    }
			    catch
			    {
					//a socket error has occured
					break;
			    }
			
			    if (bytesRead == 0)
				{
					//the client has disconnected from the server
					Console.WriteLine("Disconnected.");
					break;
				}
				data.Add(message);
				
				//we read 32 bits at a time. This is a single float, a Uint32, or 4 chars
				System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();
				Console.WriteLine("Message: {0}",encoding.GetString(message));
			}
			
		}
		
	}
}

