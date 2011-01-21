using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace NetTest
{
	class MainClass
	{	
		public byte[] NetRead = new byte[4]{0,0,0,0};
		public byte[] NetWrite = new byte[4]{3,12,9,6};
		private Thread listenthread;
		private TcpListener tcplistener;
		public static void Main (string[] args)
		{		
			MainClass MyMain = new MainClass();
			
			Console.WriteLine ("[s]erver, [c]lient, [t]est cases, [q]uit");
			string input = Console.ReadLine();
			if (input[0]=='s'){
				MyMain.Server();
			}else if(input[0]=='c'){
				MyMain.Client();
			}else if(input[0]=='t'){
				MyMain.Test();
			}else if(input[0]=='q'){
			}else{
			}
		}
		
		public void threadincoming(){
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
		
		public void Server()
		{
			int place = 0;
			int place2 = 0;
			int Count = 0;
			
			Console.WriteLine("Opening socket...");
			IPEndPoint lep = new IPEndPoint(IPAddress.Any,3000);//port 3000 is arbitrary, but needs to be consistent with client
			
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
			      break;
			    }
				//Header Packet - 32 bits
			    //message[0] contains 'action' and 'generic type' flags
				//message[1] contains 'count' flags
				//messages[2-3] are picked up by our reader but we dont currently have a use for these bits in the header.
				
				//all other packets are 32 bit datatypes such as(but not entirely limited to) floats
				Console.WriteLine("Read Value: {0},{1},{2},{3}",message[0],message[1],message[2],message[3]);
			  }
			
			  tcpClient.Close();
		}
		
		public void Client()
		{
			TcpClient client = new TcpClient();

			IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);
			
			client.Connect(serverEndPoint);
			
			NetworkStream clientStream = client.GetStream();
			
			clientStream.Write(NetWrite, 0 , NetWrite.Length);
			clientStream.Flush();
		}
		
		public void Test()
		{
		}
	}		
}		

