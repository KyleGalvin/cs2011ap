using System;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using AP;

namespace NetLib
{
	public class ClientManager : PlayerManager
	{



        public ClientManager(int port, ref GameState State): base(port, ref State)
		{
			//set up variables

            String IP = "192.168.105.123";
            TcpClient client = new TcpClient();
            //IPAddress broadcast = IPAddress.Parse("192.168.105.255");
            //IPEndPoint broadcastEP = new IPEndPoint(broadcast,port);
            //IPEndPoint lep = new IPEndPoint(IPAddress.Any,port);
			
            ////Try to connect to server via broadcast
             //serverEndPoint = FindServer(port,broadcastEP);
			
            //if (serverEndPoint == null){
               //server failed to respond. Our trickery failed. Ask for manual intervention
                //Console.WriteLine("Please enter Lobby IP:");
             IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
            //}
			
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
            //List<AP.Position> myEnemies = new List<AP.Position>();
            //Model.Add("Enemies",myEnemies);
			
            ////create an enemy object to test communications with
            //AP.Zombie baddie1 = new AP.Zombie(1);
            //AP.Zombie baddie2 = new AP.Zombie(2);
            //myEnemies.Add(baddie1);
            //myEnemies.Add(baddie2);


			
			Console.WriteLine("Sending model to server. Action = Create for all 2 objects");
			//Send(Model,(UInt32)Action.Create);
            //
            //foreach (Connection c in myConnections)
            //{
            //    //Console.WriteLine("Writing model to stream: {0}",BitConverter.ToString(data[0],0)  );
            //    c.Write(data);
            //}
		}

		protected override void HandleIncomingComm(object stream)
		{
			NetworkStream clientStream = (NetworkStream)stream;

			Console.WriteLine("Waiting for incoming messages");
				
			byte[] message = new byte[4];
			
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
				var  encoding=new UTF8Encoding();
				Console.WriteLine("Message: {0}",encoding.GetString(message));
			}
			
		}
		
	}
}

