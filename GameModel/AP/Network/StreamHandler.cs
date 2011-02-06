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
                myStream.Write(chunk32, 0, 4);
            }
            myStream.Flush();
            Console.WriteLine("Write multi data to socket here");
        }

        public void Write(List<byte[]> data)
        {
            foreach (byte[] chunk32 in data)
            {
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
            Console.WriteLine("Read header!");
            myPackage.Recieve(rawMessage);

            int typeSize = myInterpreter.GetTypeSize((Type)(myPackage.header & 0x01000000));
            int count = myInterpreter.GetCount(myPackage.header);
            myPackage.totalSize = (UInt32)(typeSize * count);

            while (myPackage.IsComplete() == false)
            {//recieve body
                bytesRead += myStream.Read(rawMessage, 0, 4);
                Console.WriteLine("Reading Segment {0} of {1}... Value: {2}", i, "(broken)", BitConverter.ToInt32(rawMessage, 0));
                myPackage.Recieve(rawMessage);
                i++;
            }
            Console.WriteLine("Packet complete!");
            return myPackage;
        }
    }
}