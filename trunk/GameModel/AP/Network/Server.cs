using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AP.Network
{
    public class Server
    {
        public string Name;
        public IPAddress ServerIP;
        public Server(string _Name, IPAddress _ServerIP)
        {
            Name = _Name;
            ServerIP = _ServerIP;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
