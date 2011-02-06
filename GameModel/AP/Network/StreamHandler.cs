using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace NetLib
{
    public class StreamHandler
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

        public void WritePackage(NetPackage pack)
        {
            myStream.Write(BitConverter.GetBytes(pack.header), 0, 4);

            foreach (byte[] chunk32 in pack.body)
            {
                Console.WriteLine("Writing to stream: {0}", BitConverter.ToString(chunk32));
                myStream.Write(chunk32, 0, 4);
            }
            myStream.Flush();
            Console.WriteLine("Write multi data to socket here");
        }

        public void Write(List<byte[]> data)
        {
            foreach (byte[] chunk32 in data)
            {
                Console.WriteLine("Writing to stream: {0}", BitConverter.ToString(chunk32));
                myStream.Write(chunk32, 0, 4);
            }
            myStream.Flush();
            Console.WriteLine("Write multi data to socket here");
        }

        public void Write(byte[] data)
        {
            //we assume 32 bits of data only
            myStream.Write(data, 0, 4);
            myStream.Flush();
            Console.WriteLine("Write single data to socket here");
        }

        public NetPackage ReadPackage()
        {


            //keep reading from stream until pack.iscomplete is true
            byte[] rawMessage = new byte[4];

            //segment number
            int i = 0;
            int bytesRead = 0;
            Console.WriteLine("Waiting for incoming data from...");
            bytesRead += myStream.Read(rawMessage, 0, 4);//read header
            myPackage.Recieve(rawMessage);

            while (myPackage.IsComplete() == false)
            {//recieve body
                bytesRead += myStream.Read(rawMessage, 0, 4);
                myPackage.Recieve(rawMessage);
            }

            return myPackage;
        }
    }
}