using System;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using AP;
using AP.Network;

namespace NetLib
{
	public class ClientManager : PlayerManager
	{



        public ClientManager(int port, ref GameState State,Server serv): base(port, ref State)
		{
			//set up variables

            TcpClient client = new TcpClient();
            //IPAddress broadcast = IPAddress.Parse("192.168.105.255");
            //IPEndPoint broadcastEP = new IPEndPoint(broadcast,port);
            //IPEndPoint lep = new IPEndPoint(IPAddress.Any,port);
			
            ////Try to connect to server via broadcast
             //serverEndPoint = FindServer(port,broadcastEP);
			
            //if (serverEndPoint == null){
               //server failed to respond. Our trickery failed. Ask for manual intervention
                //Console.WriteLine("Please enter Lobby IP:");
             IPEndPoint serverEndPoint = new IPEndPoint(serv.ServerIP, port);
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

			Console.WriteLine("Sending model to server. Action = Create for all 2 objects");
            JoinGame(serv.Name);
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
        public void ServerHandshake(String Username)
        {
            List<byte[]> data = myProtocol.encodeComm(Action.Describe, Type.Player, Username);
            foreach (Connection c in myConnections)
            {
                foreach (NetPackage p in myOutgoing)
                {
                    //worker.

                    //Console.WriteLine("Writing model to stream: {0}",BitConverter.ToString(data[0],0)  );
                    c.Write(data);
                }
            }
        }

        public void BecomeHost(String GameName)
        {
            List<byte[]> data = myProtocol.encodeComm(Action.Describe, Type.Building, GameName);
            foreach (Connection c in myConnections)
            {
                foreach (NetPackage p in myOutgoing)
                {
                    //worker.

                    //Console.WriteLine("Writing model to stream: {0}",BitConverter.ToString(data[0],0)  );
                    c.Write(data);
                }
            }
        }

        public void JoinGame(String GameName)
        {
            List<byte[]> data = myProtocol.encodeComm(Action.Request, Type.Building,GameName);
            foreach (Connection c in myConnections)
            {
                foreach (NetPackage p in myOutgoing)
                {
                    //worker.

                    //Console.WriteLine("Writing model to stream: {0}",BitConverter.ToString(data[0],0)  );
                    c.Write(data);
                }
            }
        }

        public void StartGame()
        {
            List<byte[]> data = myProtocol.encodeComm(Action.Create, Type.Player, "");
            foreach (Connection c in myConnections)
            {
                foreach (NetPackage p in myOutgoing)
                {
                    //worker.

                    //Console.WriteLine("Writing model to stream: {0}",BitConverter.ToString(data[0],0)  );
                    c.Write(data);
                }
            }
        }
	}
}

