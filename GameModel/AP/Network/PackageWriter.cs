using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace NetLib
{
	public class PackageWriter: StreamHandler
	{
		public PackageWriter (NetworkStream stream):base(stream)
		{
			myStream = stream;
		}

        public void WritePackage(NetPackage pack)
        {
            myStream.Write(BitConverter.GetBytes(pack.header),0,4);

            foreach (byte[] chunk32 in pack.body)
            {
                myStream.Write(chunk32, 0, 4);
            }
            myStream.Flush();
            Console.WriteLine("Write multi data to socket here");
        }

		public void Write(List<byte[]> data)
		{
            foreach(byte[] chunk32 in data)
            {
                myStream.Write(chunk32,0,4);
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
	}
}

