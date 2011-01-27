using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace NetTest
{
	
	class Lobby
	{
		private List<TcpClient> myClients;
		private Thread listenthread;
		private Thread respondthread;
		private TcpListener tcplistener;
		
		public Lobby(int port){
			myClients = new List<TcpClient>();
			Console.WriteLine("Opening socket...");
			IPEndPoint lep = new IPEndPoint(IPAddress.Any,port);
			
			Console.WriteLine("Listening for incoming connections...");
			this.tcplistener = new TcpListener(lep);
			this.listenthread = new Thread(new ThreadStart(ListenForClients));
			this.listenthread.Start();
			
			this.respondthread = new Thread(new ThreadStart(RespondToClients));
			this.respondthread.Start();
		}
		
		private void RespondToClients()
		{
			bool sendmessage = true;
			string message = "Server: I see you connected";
			while(true)
			{
				if(sendmessage)
				{
					//This loop needs to lock myClients so nothing modifies its data while we loop
					//foreach(TcpClient Client in myClients)
					//{
						
					//}
				}
				
			}
		}
		
		private void ListenForClients()
		{
		  tcplistener.Start();
		
		  while (true)
		  {
		    //wait for new connection and ad it to the end of our clients list
			myClients.Add(tcplistener.AcceptTcpClient());
			
		
		    //create a thread to handle communication
		    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
		    clientThread.Start(myClients[myClients.Count -1]);//start the thread using the last added client
		  }
			
		}
		
		private void HandleClientComm(object client)
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
	
	class Client
	{
		public Client(int port)
		{
			//The client's first order of business is to connect to a lobby
			Console.WriteLine("Please enter Lobby IP:");
			String IP = Console.ReadLine();
			TcpClient client = new TcpClient();

			IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
			Console.WriteLine("Connecting to server...");
			client.Connect(serverEndPoint);
			Console.WriteLine("Connected to {0}",client.Client.RemoteEndPoint);
			
			NetworkStream clientStream = client.GetStream();
			System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();

			//create listener to pick up on server responses
			Thread clientThread = new Thread(new ParameterizedThreadStart(HandleServerComm));
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
		
		private void HandleServerComm(object server)
		{
		}
	}
	
	class MainClass
	{	
		public static void Main (string[] args)
		{		
			int DefaultPort = 3000;
			
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

