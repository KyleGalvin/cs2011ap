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
		
		protected List<TcpClient> myConnectionsIn;
		protected List<TcpClient> myConnectionsOut;
		protected List<byte[]> myData;
		protected String myRole;
		
		public NetManager(int newPort)
		{
			myConnectionsIn = new List<TcpClient>();
			myConnectionsOut = new List<TcpClient>();
			port = newPort;
		}
		
		//communication is handled differently in the lobby/client children classes
		protected abstract void HandleIncomingComm(Object remoteEnd);
		
		protected abstract void SendOutgoingComm(Object remoteEnd, byte[] data);
		
		//The server will respond to every incoming connection request with an outgoing connection request
		//if both sides did this, we'd connect-loop infinitely. Therefore, two implimentations
		protected abstract void Listen();
		
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
		
		//Listen for any requests directed at our IP and Port
		//respond by accepting connection and requesting one of our own for outgoing data
		protected override void Listen()
		{
			tcpListener.Start();
			TcpClient client = new TcpClient();
			
			while (true)
			{
				//wait for new connection and ad it to the end of our clients list
				client = tcpListener.AcceptTcpClient();
				myConnectionsIn.Add(client);

				IPEndPoint clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
				client.Connect(clientEndPoint);
				myConnectionsOut.Add(client);
						
				//create a thread to handle communication
				Thread clientThread = new Thread(new ParameterizedThreadStart(HandleIncomingComm));
				clientThread.Start(myConnectionsIn[myConnectionsIn.Count -1]);//start the thread using the last added client
			}
		}
		
		private List<byte[]> GetResponse(List<byte[]> data, TcpClient tcpClient)
		{
			//we will eventually need to do something other than echo back.
			return data;
		}
		
		//for responding to a single client
		private void RespondToClient(List<byte[]> response, TcpClient tcpClient)
		{
		
		}
		
		//for responding to all clients
		private void RespondToClients(List<byte[]> response)
		{
			foreach(TcpClient client in myConnectionsOut)
			{
			}
		}
		
		protected override void HandleIncomingComm(object client)
		{
		
			TcpClient tcpClient = (TcpClient)client;
			NetworkStream clientStream = tcpClient.GetStream();
			Console.WriteLine("client {0} has connected.", tcpClient.Client.RemoteEndPoint);
				
			byte[] message = new byte[4];
			
			List<byte[]> data = new List<byte[]>();
			int bytesRead;
			
			while (true)
			{
				bytesRead = 0;
			
			    try
			    {
					//blocks until a client sends a message
					bytesRead = clientStream.Read(message, 0, 4);
					data.Add(message);
					RespondToClients(GetResponse(data,tcpClient));
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
				Console.WriteLine("Client IP: {0} Client Data: {1}",tcpClient.Client.RemoteEndPoint,encoding.GetString(message));
			}
			
			tcpClient.Close();
		}
		
		protected override void SendOutgoingComm(Object remoteEnd, byte[] data)
		{
		}
		
	}
	
	class Client : NetManager
	{
		
		public Client(int port):base(port)
		{
			String IP = "";
			TcpClient client = new TcpClient();
			IPEndPoint broadcastEP = new IPEndPoint(IPAddress.Broadcast,port);
	
			Console.WriteLine("Opening socket...");
			IPEndPoint lep = new IPEndPoint(IPAddress.Any,port);
			
			Console.WriteLine("Listening for incoming connections...");
			tcpListener = new TcpListener(lep);
			listenThread = new Thread(new ThreadStart(Listen));
			listenThread.Start();
			
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
			
			while(myConnectionsIn.Count == 0){
				//wait for a server to establish a connection (need a timeout)
			}
			
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
		
		//Listen for any requests directed at our IP and Port
		//respond by accepting connection and requesting one of our own for outgoing data
		protected override void Listen()
		{
			tcpListener.Start();
			TcpClient client = new TcpClient();
			
			while (true)
			{
				//wait for new connection and ad it to the end of our clients list
				client = tcpListener.AcceptTcpClient();
				myConnectionsIn.Add(client);
						
				//create a thread to handle communication
				Thread clientThread = new Thread(new ParameterizedThreadStart(HandleIncomingComm));
				clientThread.Start(myConnectionsIn[myConnectionsIn.Count -1]);//start the thread using the last added client
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
		
		protected override void HandleIncomingComm(object server)
		{
			TcpClient tcpClient = (TcpClient)server;
			NetworkStream clientStream = tcpClient.GetStream();
			Console.WriteLine("Waiting for incoming messages from {0}.", tcpClient.Client.RemoteEndPoint);
				
			byte[] message = new byte[4];
			
			List<byte[]> data = new List<byte[]>();
			int bytesRead;
			
			while (true)
			{
				bytesRead = 0;
			
			    try
			    {
					//blocks until a client sends a message
					bytesRead = clientStream.Read(message, 0, 4);
					data.Add(message);
			    }
			    catch
			    {
					//a socket error has occured
					break;
			    }
			
			    if (bytesRead == 0)
				{
					//the client has disconnected from the server
					Console.WriteLine("Disconnected from {0}.",tcpClient.Client.RemoteEndPoint);
					break;
				}
				
				//we read 32 bits at a time. This is a single float, a Uint32, or 4 chars
				System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();
				Console.WriteLine("Read data: {0}",tcpClient.Client.RemoteEndPoint,encoding.GetString(message));
			}
			
			tcpClient.Close();
		}
		
		protected override void SendOutgoingComm(Object remoteEnd, byte[] data)
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

