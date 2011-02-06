using System;
using System.Net;
using System.Net.Sockets;

namespace NetLib
{
	public class PackageReader: StreamHandler
	{		
		public PackageReader(NetworkStream stream):base(stream)
		{
			myStream = stream;
		}
		

	}
}

