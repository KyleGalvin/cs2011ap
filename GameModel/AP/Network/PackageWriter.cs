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


	}
}

