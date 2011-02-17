using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace NetLib
{
	public class Connection
	{
		//private PackageReader myNetReader;
		//private PackageWriter myNetWriter;
        public StreamHandler myStream;
		private TcpClient myClient;
        public int playerUID;
        public string playerName;
		
		public Connection(object client)
		{
			myClient = (TcpClient)client;
			myStream = new StreamHandler(myClient.GetStream());
		}
		
		public NetPackage ReadPackage()
		{
			return myStream.ReadPackage();
		}
		
		public void Write(List<byte[]> data)
		{
			myStream.Write(data);
		}
		
		public void Write(byte[] data)
		{
			myStream.Write(data);
		}
		
		public TcpClient GetClient()
		{
			return myClient;
		}
	}
}

