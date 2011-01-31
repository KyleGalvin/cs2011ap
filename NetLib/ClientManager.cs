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
			//we try some auto-connection trickery first
			String IP = "";
			TcpClient client = new TcpClient();
			IPEndPoint broadcastEP = new IPEndPoint(IPAddress.Broadcast,port);
	
			Console.WriteLine("Opening socket...");
			IPEndPoint lep = new IPEndPoint(IPAddress.Any,port);
			
			//Broadcast our address and protocol in hopes that a server will respond
			IPEndPoint serverEndPoint = FindServer(port,broadcastEP);
			
			if (serverEndPoint == broadcastEP){
				//server failed to respond. Our trickery failed. Ask for manual intervention
				Console.WriteLine("Please enter Lobby IP:");
				IP = Console.ReadLine();
				serverEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
			}
			
			Console.WriteLine("Connecting to server...");
			client.Connect(serverEndPoint);
			Console.WriteLine("Connected to {0}",client.Client.RemoteEndPoint);
			
			NetworkStream clientStream = client.GetStream();
			System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();
			
			Console.WriteLine("Listening for incoming data...");
			tcpListener = new TcpListener(lep);
			listenThread = new Thread(new ParameterizedThreadStart(HandleIncomingComm));
			listenThread.Start(clientStream);
			
			//create an enemy object to test communications with
			Enemy baddie = new Enemy(100,100,5,5,20);
			byte count = 1;
			
			//Send to server at our leisure
			while(true){
				string Message = Console.ReadLine();
				
				//test hack
				byte[] testpackage = {0,0,count,(byte)Enemy.netObjType};
				clientStream.Write(testpackage,0,4);
				
				//while(Message.Length % 4 !=0){
				//	Message += "\0";//insert nulls to fill the rest of the 32 bit packet
				//}
				//clientStream.Write(encoding.GetBytes(Message), 0 , Message.Length);
				clientStream.Flush();
			}
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

