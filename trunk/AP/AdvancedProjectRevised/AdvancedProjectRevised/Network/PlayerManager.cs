using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Timer = System.Threading.Timer;
using AP;

    /// <summary>
    /// Network manager specific to the player.
    /// </summary>
public abstract class PlayerManager : NetManager
{
		#region Fields (4) 

    private Thread broadcastThread;
    private List<IPAddress> ServerIps = new List<IPAddress>();
    protected DateTime lastFrameTime;
    private bool TimesUp;
    private bool done = false;
    private bool change = false;
    private int playerUID = 0;
		#endregion Fields 

		#region Constructors (1) 

	public PlayerManager(int newPort,  ref GameState StateRef): base(newPort)
	{
        worker = new GamePackWorker(ref StateRef);
        State = StateRef;
    }

		#endregion Constructors 

		#region Methods (5) 

		// Public Methods (1) 

    /// <summary>
    /// Syncs the state.
    /// </summary>

    /*public void modifyStateElement()
    {
        switch()
        {
            case enemyModification:
            {
	            var foundEnemy = 	from e in <enemyList>
						            select e
						            where e.id = $
						            select e;
						
	            using(foundEnemy)
	            {
		            e.xPos = <packetInfo>;
		            e.yPos = <packetInfo>;
		            e.xVel = <packetInfo>;
		            e.yVel = <packetInfo>;
		            e.life = <packetInfo>;
	            }
	            break;
            }
            case playerModification:
            {
	            var foundPlayer = 	from p in <playerList>
						            select p
						            where p.id = #
						            select p;
						
	            using(foundPlayer)
	            {
		            p.xPos = <packetInfo>;
		            p.yPos = <packetInfo>;
		            p.xVel = <packetInfo>;
		            p.yVel = <packetInfo>;
		            p.life = <packetInfo>;
	            }
	            break;
            }
            case bulletModification:
            {
	            var foundBullet = 	from b in <List>
						            select b
						            where b.id = #
						            select b;
						
	            using(foundBullet)
	            {
		            b.xPos = <packetInfo>;
		            b.yPos = <packetInfo>;
	            }
	            break;
            }
        }
    }*/
		// Private Methods (4) 

        //automatically find server on subnet
        /// <summary>
        /// Finds the server.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="broadcastEP">The broadcast EP.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Listenfors the broad cast.
        /// </summary>
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

        /// <summary>
        /// Called when [timed event].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (broadcastThread.IsAlive)
            {
                broadcastThread.Suspend();
            }
            Console.WriteLine("Listening Stopped (Timeout Reached)");
            TimesUp = true;
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

        public void SyncState()
        {
            SyncStateOutgoing();//send relevent data out to connections
        }

        public void SyncStateOutgoing()
        {
            GameState s = State;
            List<Enemy> enemyUpdateList = new List<Enemy>();
            List<Enemy> enemyAddList = new List<Enemy>();
            List<Enemy> enemyDeleteList = new List<Enemy>();
            List<Bullet> bulletUpdateList = new List<Bullet>();
            List<Bullet> bulletAddList = new List<Bullet>();
            List<Bullet> bulletDeleteList = new List<Bullet>();
            List<Player> playerUpdateList = new List<Player>();
            List<Player> playerAddList = new List<Player>();
            List<Player> playerDeleteList = new List<Player>();

            /*foreach (Bullet b in s.Bullets)
            {
                if (b.timestamp > lastFrameTime.Ticks)
                {
                    bulletUpdateList.Add(b);
                }
                else if (b.timestamp == 0)
                {
                    bulletAddList.Add(b);
                }
                else if (b.timestamp == -1)
                {
                    bulletDeleteList.Add(b);
                }
                b.timestamp = DateTime.Now.Ticks;
            }*/
            for (int x = 0; x < s.Players.Count; x++)
            {
                Player p = s.Players[x];
                if (p.timestamp > 0)
                    p.updateTimeStamp();
            }

            for (int x = 0; x < s.Players.Count; x++)
            {
                Player p = s.Players[x];
                if (p.timestamp >= lastFrameTime.Ticks)
                {
                    Console.WriteLine("PLAYERMANAGER UPDATE");
                    playerUpdateList.Add(p);
                }
                else if (p.timestamp == 0)
                {
                    p.playerId = playerUID;
                    playerUID++;
                    Console.WriteLine("PLAYERMANAGER CREATE");
                    playerAddList.Add(p);
                    //State.Players.Add(p);
                    p.updateTimeStamp();

                }
                else if (p.timestamp == -1)
                {
                    playerDeleteList.Add(p);
                }
            }
            /*foreach (Enemy e in s.Enemies)
            {
                if (e.timestamp > lastFrameTime.Ticks)
                {
                    enemyUpdateList.Add(e);
                }
                else if (e.timestamp == 0)
                {
                    enemyAddList.Add(e);
                }
                else if (e.timestamp == -1)
                {
                    enemyDeleteList.Add(e);
                }
                e.timestamp = DateTime.Now.Ticks;
            }*/
            if (playerAddList.Count > 0)
                this.SendObjs<Player>(Action.Create, playerAddList, Type.Player);
            //this.SendObjs<Enemy>(Action.Update, enemyUpdateList, Type.AI);
            if (playerUpdateList.Count > 0)
                this.SendObjs<Player>(Action.Update, playerUpdateList, Type.Player);
            //this.SendObjs<Bullet>(Action.Update, bulletUpdateList, Type.Bullet);
            //this.SendObjs<Enemy>(Action.Create, enemyAddList, Type.AI);
            //this.SendObjs<Bullet>(Action.Create, bulletAddList, Type.Bullet);
            //this.SendObjs<Enemy>(Action.Delete, enemyDeleteList, Type.AI);
            //this.SendObjs<Player>(Action.Delete, playerDeleteList, Type.Player);
            //this.SendObjs<Bullet>(Action.Delete, bulletDeleteList, Type.Bullet);
            /*if (!done)
            {
                playerAddList.Add(player);
                this.SendObjs<Player>(Action.Create, playerAddList, Type.Player);
                done = true;
            }
            else
            {
                if( change )
                {
                    player.move(0, 1);
                    player.move(0, 1);
                    player.move(0, 1);
                    player.move(0, 1);
                    player.move(0, 1);
                    change = false;
                }
                else
                {
                    player.move(1,0);
                    player.move(1, 0);
                    player.move(1, 0);
                    player.move(1, 0);
                    player.move(1, 0);
                    change = true;
                }
                playerUpdateList.Add(player);
                this.SendObjs<Player>(Action.Update, playerUpdateList, Type.Player);
            }*/

            lastFrameTime = DateTime.Now;
        }

		#endregion Methods 
	
        public string getRole()
        {
            return myRole;
        }
}