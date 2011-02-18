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
            IsLobby = false;
            client = new TcpClient();

            IPEndPoint serverEndPoint = new IPEndPoint(serv.ServerIP, port);

            Console.WriteLine("Port: {0} IP: {1}",port,serv.ServerIP);
			Console.WriteLine("Waiting for connections...");
			client.Connect(serverEndPoint);
			
			lock(this){
				myConnections.Add(new Connection(client));
				Console.WriteLine("Connected to {0}",client.Client.RemoteEndPoint);
			}
			
			NetworkStream clientStream = client.GetStream();
			
			Console.WriteLine("Creating listener thread for reading server communications...");
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleIncomingComm));
            clientThread.Start(myConnections[(myConnections.Count - 1)]);
            JoinGame(serv.Name);
		}

        public override void SyncState()
        {
            SendObjs<AP.Player>(State.Players);
        }

		protected override void HandleIncomingComm(object conn)
		{

            Connection myConnection = (Connection)conn;
            NetPackage pack = new NetPackage();

            client = myConnection.GetClient();
            Console.WriteLine("client {0} has connected.", client.Client.RemoteEndPoint);

            while (true)
            {
                Console.WriteLine("Size of game state: {0}", State.Enemies.Count + State.Players.Count);
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

            lock (this)
            {
                myConnections.Remove(myConnection);
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
            List<byte[]> data = myProtocol.encodeComm(Action.Request, Type.Building, GameName);
            foreach (Connection c in myConnections)
            {
                    Console.WriteLine("Writing model to stream: {0}",BitConverter.ToString(data[0],0)  );
                    c.Write(data);

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

