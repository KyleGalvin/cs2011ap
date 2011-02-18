using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using AP;

namespace NetLib
{
    public class HostManager : PlayerManager
    {

        //HACK todo
        int i = 0;

        public HostManager(int port, ref GameState State): base(port, ref State)
        {
            new Thread(new ThreadStart(this.Listen)).Start();//Collect clients in our connection pool
            bool startGame = false;
        }

        protected override void HandleIncomingComm(object connection)
        {
            Connection myConnection = (Connection)connection;
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

        public override void SyncState()
        {
            if (i == 0)
            {
                SendObjs<AP.Player>(Action.Create, State.Players);
            }
            else
            {
                SendObjs<AP.Player>(Action.Update, State.Players);
            }
               
        }
        //ConnectGame(IPEndPoint host)
        //{
        //}
    }
}
