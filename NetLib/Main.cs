using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace NetTest
{
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
		
		//communication is handled differently in the lobby/client children classes
		protected abstract void HandleIncomingComm(Object remoteEnd);
		
	}
	
	class Lobby : NetManager
	{
		public Lobby(int port):base(port){;
			Console.WriteLine("Opening socket...");
			IPEndPoint lep = new IPEndPoint(IPAddress.Any,port);
			
			Console.WriteLine("Listening for incoming connections...");
			tcpListener = new TcpListener(lep);
			listenThread = new Thread(new ThreadStart(Listen));
			listenThread.Start();
		}
		
		private void Respond(UInt32 data)
		{
			Console.WriteLine("Responding to incoming communication...");
			Console.WriteLine("Locking Connections resource - incoming connections must wait");
			
			byte[] rawData = BitConverter.GetBytes(data);//break down into bytes to send over socket
			
			lock(this)
			{
				foreach(TcpClient client in myConnections)
				{
					//we will eventually need to do something other than echo back.
					Console.WriteLine("echoing response: {1}",data,client.Client.RemoteEndPoint);
					int val = client.Client.Send(rawData);
				}
			}
			Console.WriteLine("Freeing resource lock - now accepting incomming connections");
		}
		
		protected void HandleOutgoingComm(object client)
		{
		}
		
		protected override void HandleIncomingComm(object client)
		{
		
			TcpClient tcpClient = (TcpClient)client;
			NetworkStream clientStream = tcpClient.GetStream();
			Console.WriteLine("client {0} has connected.", tcpClient.Client.RemoteEndPoint);
				
			byte[] rawMessage = new byte[4];
			UInt32 message;
			
			List<UInt32> data = new List<UInt32>();
			
			//The current 32 bit data unit coming in
			int bytesRead;
			
			//the header data unit indicates how many data units are expected to follow. This value includes the header data
			UInt32 packageSize;
			
			System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();
			
			bytesRead = 0;
			
			while (true)
			{
				
			
			    try
			    {
					//blocks until a client sends a message
					bytesRead += clientStream.Read(rawMessage, 0, 4);
					message = BitConverter.ToUInt32(rawMessage,0);
					data.Add(message);
					
					//the number of 32 bit reads to follow plus the 32 bit header
					packageSize = GetExpectedPackageSize(message);
					

			    }
			    catch
			    {
					//a socket error has occured
					break;
			    }
			
			    if (bytesRead == 0)//nothing was read from socket
				{
					Console.WriteLine("Client IP: {0}, Client Data: {1}",tcpClient.Client.RemoteEndPoint,encoding.GetString(rawMessage));
					Respond(message);
					//the client has disconnected from the server
					Console.WriteLine("Client {0} has disconnected.",tcpClient.Client.RemoteEndPoint);
					break;
				}
				if(bytesRead == packageSize)//we've accumulated the amount of data our header predicts
				{
					Console.WriteLine("Now we are able to test if we are picking up the right data here");
					bytesRead = 0;
				}
				
			}
			
			tcpClient.Close();
		}
		
		private UInt32 GetExpectedPackageSize(UInt32 header)
		{
			//byte action = ((byte)header[0] >> 4);
			UInt32 type = header & 0x000F;
			UInt32 typesize=0;
			UInt32 count = (header & 0x0F00) >> 8;//take only our count byte
			
			switch (type)
			{
			case 0x0://player
				Console.WriteLine("00 = client player; 8 floats");
				typesize=8;
				break;
			case 0x1://AI
				Console.WriteLine("01 = enemy AI; 8 floats");
				typesize=8;
				break;
			case 0x2://building
				Console.WriteLine("02 = building; 8 floats");
				typesize=8;
				break;
			case 0x4://bullet
				Console.WriteLine("03 = bullet; 7 floats");
				typesize=7;
				break;
			case 0x5://explosion
				Console.WriteLine("04 = explosion; 8 floats");
				typesize=8;
				break;
			case 0x6://power-up
				Console.WriteLine("05 = power-up; 8 floats");
				typesize=8;
				break;
			default:
				Console.WriteLine("Invalid Command!");
				break;
			}
			
			return typesize*count;
		}
		
	}
	
	class Client : NetManager
	{
		
		public Client(int port):base(port)
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
			
			//Send to server at our leisure
			while(true){
				string Message = Console.ReadLine();
				while(Message.Length % 4 !=0){
					Message += "\0";//insert nulls to fill the rest of the 32 bit packet
				}
				clientStream.Write(encoding.GetBytes(Message), 0 , Message.Length);
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
	
	class MainClass
	{	
		public static void Main (string[] args)
		{		
			int DefaultPort = 9999;
			
			while(true){
				Console.WriteLine("[c]reate Lobby, [j]oin Lobby, [q]uit");
				string input = Console.ReadLine();

				if (input[0]=='c'){
					Lobby myLobby = new Lobby(DefaultPort);
					break;
				}else if(input[0]=='j'){
					Client myClient = new Client(DefaultPort);
				}else if(input[0]=='q'){
						break;
				}else{
				}
			}
		}
	}		
}		

