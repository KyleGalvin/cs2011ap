using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace NetLib
{
	class LobbyManager : NetManager
	{
		public LobbyManager(int port):base(port){;
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
					Console.WriteLine("Expected {0} numerics", packageSize);
					

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
			UInt32 action = (header & 0xF0000000)>>28;
			UInt32 type = (header & 0x0F000000)>>24;
			UInt32 count = (header & 0x00FF0000)>>16;
			UInt32 typesize=0;		
			
			Console.WriteLine("Header: {0} Type: {1} Action: {2} Count: {3}", header, type, action, count);
			switch (type)
			{
			case 0://player
				Console.WriteLine("00 = client player; 8 floats");
				typesize=8;
				break;
			case 1://AI
				Console.WriteLine("01 = enemy AI; 8 floats");
				typesize=8;
				break;
			case 2://building
				Console.WriteLine("02 = building; 8 floats");
				typesize=8;
				break;
			case 3://bullet
				Console.WriteLine("03 = bullet; 7 floats");
				typesize=7;
				break;
			case 4://explosion
				Console.WriteLine("04 = explosion; 8 floats");
				typesize=8;
				break;
			case 5://power-up
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
}

