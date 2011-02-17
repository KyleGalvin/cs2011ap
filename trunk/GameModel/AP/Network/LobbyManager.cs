using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using System.Collections.Generic;
using AP;
using System.Windows.Forms;

namespace NetLib
{
	public class LobbyManager : NetManager
	{
        public string LobbyName;
        public LobbyManager(int port, ref ListBox list): base(port)
        {
            IsLobby = true;
            worker = (PackWorker)new LobbyPackWorker(ref list);
            Connected = true;//Server has nothing to connect to
            Console.WriteLine("Listening for incoming connections...");
            new Thread(new ThreadStart(this.BroadcastListener)).Start();//Alert clients of our presence
            new Thread(new ThreadStart(this.Listen)).Start();//Collect clients in our connection pool
            System.Timers.Timer timer = new System.Timers.Timer(5000);
            timer.Enabled = true;
            
        }

        public void RespondToClients(int port, IPEndPoint broadcastEP)
        {
            
            //Needs to broadcast itself on the network
            //broadcast
            Socket BC = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            try
            {
                BC.SendTo(encoding.GetBytes("S"+LobbyName),broadcastEP);
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
			
			while (true)
			{
                Console.WriteLine("Size of game state: {0}", State.Enemies.Count+State.Players.Count);
                try
                {
                    //read package data
                    Console.WriteLine("attempt to read pack:");
                    pack = myConnection.ReadPackage();
                    Console.WriteLine("Package recieved!");
                }
                catch
                {
                    //a socket error has occured
                    break;
                }
			
			    //if (bytesRead == 0)//nothing was read from socket
				//{
				//	Console.WriteLine("Client {0} has disconnected.",client.Client.RemoteEndPoint);
				//	break;
				//}
                packetSwitcher(pack);
			}
			
			lock(this)
			{
				myConnections.Remove(myConnection);
			}
		}

	}
}

