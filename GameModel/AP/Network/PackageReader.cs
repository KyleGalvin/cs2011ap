using System;
using System.Net;
using System.Net.Sockets;

namespace NetLib
{
	public class PackageReader: StreamHandler
	{
		NetworkStream myStream;
		
		public PackageReader(NetworkStream stream):base(stream)
		{
			myStream = stream;
		}
		
		public NetPackage Read()
		{
			//keep reading from stream until pack.iscomplete is true
			NetPackage pack = new NetPackage();
			byte[] rawMessage = new byte[4];
			
			//segment number
			int i = 0;
			int bytesRead = 0;
			while(pack.IsComplete()==false){//recieve body
				bytesRead += myStream.Read(rawMessage, 0, 4);
				Console.WriteLine("Reading Segment {0} of {1}... Value: {2}",i,"(broken)", BitConverter.ToInt32(rawMessage,0));
				pack.Recieve(rawMessage);
				i++;
			}	
			Console.WriteLine("Packet complete!");
			
			return pack;
		}
	}
}

