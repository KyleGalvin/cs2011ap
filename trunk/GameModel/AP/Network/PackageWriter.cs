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
		
		public void Write(List<byte[]> data)
		{
			Console.WriteLine("Write multi data to socket here");
		}
		
		public void Write(byte[] data)
		{
			Console.WriteLine("Write single data to socket here");
		}
	}
}

