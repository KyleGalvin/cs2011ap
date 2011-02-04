using System;
using System.Net;
using System.Net.Sockets;

namespace NetLib
{
    public abstract class StreamHandler
    {
        protected NetworkStream myStream;
        protected NetPackage myPackage;
        protected PackageInterpreter myInterpreter;
        //StateWorker myWorker;

        public StreamHandler(NetworkStream stream)
        {
            myStream = stream;
            myPackage = new NetPackage();
            myInterpreter = new PackageInterpreter();
        }
    }
}