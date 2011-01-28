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
		
		public NetManager(int newPort)
		{
			myConnections = new List<TcpClient>();
			port = newPort;
		}
		
		//Listen for any requests directed at our IP and Port;
		protected void Listen()
		{
			tcpListener.Start();
		
			while (true)
			{
				//wait for new connection and ad it to the end of our clients list
				myConnections.Add(tcpListener.AcceptTcpClient());
			
		
				//create a thread to handle communication
				Thread clientThread = new Thread(new ParameterizedThreadStart(HandleIncomingComm));
				clientThread.Start(myConnections[myConnections.Count -1]);//start the thread using the last added client
			}
		}
		
		//communication is handled differently in the lobby/client children classes
		protected abstract void HandleIncomingComm(Object RemoteEnd);
		
		private void ReadFromConnection(TcpClient connection){}
		private void AcceptConnection(){}
		
	}
	
	class Lobby : NetManager
	{
		
		public Lobby(int port):base(port){
			myConnections = new List<TcpClient>();
			Console.WriteLine("Opening socket...");
			IPEndPoint lep = new IPEndPoint(IPAddress.Any,port);
			
			Console.WriteLine("Listening for incoming connections...");
			tcpListener = new TcpListener(lep);
			listenThread = new Thread(new ThreadStart(Listen));
			listenThread.Start();
			
			respondThread = new Thread(new ThreadStart(RespondToClients));
			respondThread.Start();
		}
		
		private void RespondToClients()
		{
			bool sendmessage = true;
			string message = "Server: I see you connected";
			while(true)
			{
				if(sendmessage)
				{
					//foreach(TcpClient Client in myConnections)
					//{
					//This loop needs to lock myClients so nothing modifies its data while we loop
						
					//}
				}
				
			}
		}
		
		protected override void HandleIncomingComm(object client)
		{
		
			  TcpClient tcpClient = (TcpClient)client;
			  NetworkStream clientStream = tcpClient.GetStream();
			  Console.WriteLine("client {0} has connected.", tcpClient.Client.RemoteEndPoint);
				
			  byte[] message = new byte[4];
			  int bytesRead;
			
			  while (true)
			  {
			    bytesRead = 0;
			
			    try
			    {
			      //blocks until a client sends a message
			      bytesRead = clientStream.Read(message, 0, 4);
			    }
			    catch
			    {
			      //a socket error has occured
			      break;
			    }
			
			    if (bytesRead == 0)
			    {
			      //the client has disconnected from the server
				Console.WriteLine("Client {0} has disconnected.",tcpClient.Client.RemoteEndPoint);
			      break;
			    }

				//we read 32 bits at a time. This is a single float, a Uint32, or 4 chars
				System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();
				Console.WriteLine("Client: {0}",encoding.GetString(message));
			  }
			
			  tcpClient.Close();
		}
	}
	
	class Client : NetManager
	{
		
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
		
		public Client(int port):base(port)
		{
			String IP = "";
			TcpClient client = new TcpClient();
			IPEndPoint broadcastEP = new IPEndPoint(IPAddress.Broadcast,port);
			
			//Broadcast our address and protocol in hopes that a server will respond
			IPEndPoint serverEndPoint = FindServer(port,broadcastEP);
			
			if (serverEndPoint == broadcastEP){
				//server failed to respond. Ask for manual intervention
				Console.WriteLine("Please enter Lobby IP:");
				IP = Console.ReadLine();
				serverEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
			}
			
			Console.WriteLine("Connecting to server...");
			client.Connect(serverEndPoint);
			Console.WriteLine("Connected to {0}",client.Client.RemoteEndPoint);
			
			NetworkStream clientStream = client.GetStream();
			System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();

			//create listener to pick up on server responses
			Thread clientThread = new Thread(new ParameterizedThreadStart(HandleIncomingComm));
			clientThread.Start();
			
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
		
		protected override void HandleIncomingComm(object server)
		{
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

