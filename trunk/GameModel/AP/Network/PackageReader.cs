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

			while(myPackage.IsComplete()==false){//recieve body
				bytesRead += myStream.Read(rawMessage, 0, 4);
				Console.WriteLine("Reading Segment {0} of {1}... Value: {2}",i,"(broken)", BitConverter.ToInt32(rawMessage,0));
				myPackage.Recieve(rawMessage);
				i++;
			}	
			Console.WriteLine("Packet complete!");
            return myPackage;
		}
	}
}

