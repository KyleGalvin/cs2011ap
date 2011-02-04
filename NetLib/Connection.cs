using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace NetLib
{
	public class Connection
	{
		private PackageReader myNetReader;
		private PackageWriter myNetWriter;
		private TcpClient myClient;
		
		public Connection(object client)
		{			
			myClient = (TcpClient)client;
			myNetReader = new PackageReader(myClient.GetStream());
			myNetWriter = new PackageWriter(myClient.GetStream());
		}
		
		public NetPackage Read()
		{
			return myNetReader.Read();
		}
		
		public void Write(List<byte[]> data)
		{
			myNetWriter.Write(data);
		}
		
		public void Write(byte[] data)
		{
			myNetWriter.Write(data);
		}
		
		public TcpClient GetClient()
		{
			return myClient;
		}
	}
}

