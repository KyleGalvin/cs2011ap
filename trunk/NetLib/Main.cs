using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace NetTest
{
	
	class Lobby
	{
		private Thread listenthread;
		private TcpListener tcplistener;
		public Lobby(int port){
			
			Console.WriteLine("Opening socket...");
			IPEndPoint lep = new IPEndPoint(IPAddress.Any,port);
			
			Console.WriteLine("Listening for incoming connections...");
			this.tcplistener = new TcpListener(lep);
			this.listenthread = new Thread(new ThreadStart(ListenForClients));
			this.listenthread.Start();
		}
		
		private void ListenForClients()
		{
		  this.tcplistener.Start();
		
		  while (true)
		  {
		    //blocks until a client has connected to the server
		    TcpClient client = this.tcplistener.AcceptTcpClient();
		
		    //create a thread to handle communication
		    //with connected client
		    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
		    clientThread.Start(client);
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
				//Header Packet - 32 bits
			    //message[0] contains 'action' and 'generic type' flags
				//message[1] contains 'count' flags
				//messages[2-3] are picked up by our reader but we dont currently have a use for these bits in the header.
				
				//all other packets are 32 bit datatypes such as (but not entirely limited to) floats
				Console.WriteLine("Read Value: {0},{1},{2},{3}",message[0],message[1],message[2],message[3]);
			  }
			
			  tcpClient.Close();
		}
	}
	
	class Client
	{
		public byte[] NetWrite = new byte[4]{3,12,9,6};//should we make a class for this 32 bit reader? A class for an array of 32 bit reads?
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
			
			clientStream.Write(NetWrite, 0 , NetWrite.Length);
			clientStream.Flush();
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

