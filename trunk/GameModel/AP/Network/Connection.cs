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
		
		public NetPackage ReadPackage()
		{
			return myNetReader.ReadPackage();
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

