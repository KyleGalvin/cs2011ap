using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace NetLib
{
	class LobbyManager : NetManager
	{

        public void RespondToClients(int port, IPEndPoint broadcastEP)
        {
            //Needs to broadcast itself on the network
            //broadcast
            Socket BC = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            try
            {
                BC.SendTo(encoding.GetBytes("Server Broadcast"),broadcastEP);
                Console.WriteLine("Sent Response to " +broadcastEP.ToString());
            }
            catch
            {
                Console.WriteLine("Could not broadcast request for server");
                Console.WriteLine("Broadcast May be disabled...");
            }
        }
        private void BroadcastListener()
        {
            //This is used to see the servers
            IPAddress broadcast = IPAddress.Parse("192.168.105.255");
            IPEndPoint broadcastEP = new IPEndPoint(broadcast, port);
            bool done = false;
            string msg = String.Empty;
            UdpClient listener = new UdpClient(port);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);
            //Send initial broadcast
            RespondToClients(port, broadcastEP);
            System.Text.UTF8Encoding Encoding = new System.Text.UTF8Encoding();

            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);
                    msg = Encoding.GetString(bytes, 0, bytes.Length);
                    if (msg == "Client Broadcast")
                    {
                        Console.WriteLine("Responding to: " + msg + " at " + groupEP.ToString());
                        RespondToClients(port, broadcastEP);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }

		public LobbyManager(int port):base(port){		
			Console.WriteLine("Listening for incoming connections...");
            new Thread(new ThreadStart(this.BroadcastListener)).Start();//Alert clients of our presence
			new Thread(new ThreadStart(this.Listen)).Start();//Collect clients in our connection pool
		}
		
		private void Respond(UInt32 data)
		{
			Console.WriteLine("Responding to incoming communication...");
			Console.WriteLine("Locking Connections resource - incoming connections must wait");
			
			byte[] rawData = BitConverter.GetBytes(data);//break down into bytes to send over socket
			
			lock(this)
			{
				foreach(Connection c in myConnections)
				{
					//we will eventually need to do something other than echo back.
					Console.WriteLine("echoing response: {0}",data);
					c.Write(rawData);
				}
			}
			Console.WriteLine("Freeing resource lock - now accepting incomming connections");
		}

		protected override void HandleIncomingComm(object connection)
		{
			
			Connection myConnection = (Connection)connection;
			NetPackage pack = new NetPackage();

			TcpClient client = myConnection.GetClient();
			Console.WriteLine("client {0} has connected.", client.Client.RemoteEndPoint);
			
			//The current 32 bit data unit coming in
			int bytesRead;
			
			bytesRead = 0;
			
			while (true)
			{
			
			    try
			    {
					//read package data
					pack = myConnection.ReadPackage();
			    }
			    catch
			    {
					//a socket error has occured
					break;
			    }
			
			    if (bytesRead == 0)//nothing was read from socket
				{
					Console.WriteLine("Client {0} has disconnected.",client.Client.RemoteEndPoint);
					break;
				}
				if(pack.IsComplete())//we've accumulated the amount of data our header predicts
				{
					if(pack.action == (UInt32)Action.Create)
					{
						Console.WriteLine("Create command triggered by incoming packet header");
					}
			
					bytesRead = 0;
				}
				
			}
			
			lock(this)
			{
				myConnections.Remove(myConnection);
			}
		}
		
		
		//MOVE THIS
		private UInt32 GetExpectedPackageSize(UInt32 header)
		{
			UInt32 action = (header & 0xF0000000)>>28;
			UInt32 type = (header & 0x0F000000)>>24;
			UInt32 count = (header & 0x00FF0000)>>16;
			
			Console.WriteLine("Header: {0} Type: {1} Action: {2} Count: {3}", header, type, action, count);

			
			return 0;
		}
		
	}
}

