using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Timer = System.Threading.Timer;
using AP;
namespace NetLib
{
	public class ClientManager : NetManager
	{
        private Thread broadcastThread;
	    private bool TimesUp;
	    private List<IPAddress> ServerIps= new List<IPAddress>();
        public ClientManager(int port, ref GameState State)
            : base(port, ref State)
		{
			//set up variables

			String IP = "";
			TcpClient client = new TcpClient();
            IPAddress broadcast = IPAddress.Parse("192.168.105.255");
			IPEndPoint broadcastEP = new IPEndPoint(broadcast,port);
			IPEndPoint lep = new IPEndPoint(IPAddress.Any,port);
			
			//Try to connect to server via broadcast
			IPEndPoint serverEndPoint = FindServer(port,broadcastEP);
			
			if (serverEndPoint == null){
				//server failed to respond. Our trickery failed. Ask for manual intervention
				Console.WriteLine("Please enter Lobby IP:");
				IP = Console.ReadLine();
				serverEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
			}
			
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
			List<AP.Position> myEnemies = new List<AP.Position>();
			Model.Add("Enemies",myEnemies);
			
			//create an enemy object to test communications with
			AP.Zombie baddie1 = new AP.Zombie(1);
			AP.Zombie baddie2 = new AP.Zombie(2);
            myEnemies.Add(baddie1);
			myEnemies.Add(baddie2);


			
			Console.WriteLine("Sending model to server. Action = Create for all 2 objects");
			//Send(Model,(UInt32)Action.Create);
            List<byte[]> data = myProtocol.encode(Action.Create,Type.AI,myEnemies);
            foreach (Connection c in myConnections)
            {
                Console.WriteLine("Writing model to stream: {0}",BitConverter.ToString(data[0],0)  );
                c.Write(data);
            }
		}
		
		//automatically find server on subnet
        private IPEndPoint FindServer(int port, IPEndPoint broadcastEP)
        {
            //broadcast
            SendBroadcast(broadcastEP);
            //listen

            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);
            // Create a timer with a ten second interval.
            System.Timers.Timer aTimer = new System.Timers.Timer(10000);
            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Enabled = true;
            aTimer.AutoReset = false;
            aTimer.Start();
            try
            {
                    broadcastThread = new Thread(new ThreadStart(ListenforBroadCast));
                    broadcastThread.Start();
                    
                    Console.WriteLine("Starting broadcast thread");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            while (!TimesUp)
            {
                //Place holder need to wait until the timer is done
            }
            return ServerIps.Count==0?null:new IPEndPoint(ServerIps.First(),port);
        }

	    private void SendBroadcast(IPEndPoint broadcastEP)
	    {
	        Socket BC = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

	        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

	        try
	        {
	            BC.SendTo(encoding.GetBytes("Client Broadcast"), broadcastEP);
	            Console.WriteLine("Broadcast sent");
	        }
	        catch
	        {
	            Console.WriteLine("Could not broadcast request for server");
	            Console.WriteLine("Broadcast May be disabled...");
	        }
	    }

	    private void ListenforBroadCast()
	    {
	        bool once=false;
            IPAddress broadcast = IPAddress.Parse("192.168.105.255");
            IPEndPoint broadcastEP = new IPEndPoint(broadcast, port);
            string msg = String.Empty;
            UdpClient listener = new UdpClient(port);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);
            while (!TimesUp)
            {
                if (!once)
                {
                    SendBroadcast(broadcastEP);
                    once = true;
                } 
                Console.WriteLine("Waiting for broadcast again {0}",groupEP.ToString());
                byte[] bytes = listener.Receive(ref groupEP);
                msg = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                if (msg == "Server Broadcast")
                {
                    Console.WriteLine("Adding " + groupEP.Address.ToString() + " to the list of clients");
                    ServerIps.Add(groupEP.Address);
                    TimesUp = true;
                }
            }
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (broadcastThread.IsAlive)
            {
                broadcastThread.Suspend();
            }
            Console.WriteLine("Listening Stopped (Timeout Reached)");
            TimesUp = true;
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

