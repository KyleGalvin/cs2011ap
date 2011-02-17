using System;
using System.Net;
using AP;

namespace NetLib
{
    public class HostManager : PlayerManager
    {
        public HostManager(int port, ref GameState State): base(port, ref State)
        {
            bool startGame = false;
        }

        protected override void HandleIncomingComm(object connection)
        {
        }

        //ConnectGame(IPEndPoint host)
        //{
        //}
    }
}
