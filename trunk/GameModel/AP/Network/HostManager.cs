using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using AP;

namespace NetLib
{
    public class HostManager : PlayerManager
    {
        public HostManager(int port, ref GameState State): base(port, ref State)
        {
            new Thread(new ThreadStart(this.Listen)).Start();//Collect clients in our connection pool
            bool startGame = false;
        }

        protected override void HandleIncomingComm(object connection)
        {
        }

        protected void SyncState()
        {
                SendObjs<AP.Player>(State.Players);
        }
        //ConnectGame(IPEndPoint host)
        //{
        //}
    }
}
