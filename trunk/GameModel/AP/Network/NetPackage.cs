using System;
using System.Collections.Generic;

namespace NetLib
{
	public class NetPackage
	{
        public PackageInterpreter myInterpreter;
		public UInt32 header;
		public UInt32 sizeofobj;
		public UInt32 typeofobj;
		public UInt32 action;
		public UInt32 totalSize;
		public List<byte[]> body;
		public bool complete;
		
		public NetPackage ()
		{
            myInterpreter = new PackageInterpreter();
			complete = false;
			header = 0;
			body = new List<byte[]>();
		}
		
		public void Recieve(byte[] incoming)
		{
			//we only intend to recieve 4 bytes at a time
			body.Add(incoming);
			
			if(header == 0)
			{
				//no header has been created. We naievely assume the data read is a header
				
				header = BitConverter.ToUInt32(incoming,0);
				UInt32 typeofobj = (header & 0x0F000000)>>24;
				UInt32 action = (header & 0xF0000000)>>28;
				//sizeofobj = GetSize(typeofobj);
				totalSize = (header & 0x00FF0000)>>16;
				//Console.WriteLine("GetSize(type): {0}",GetSize(typeofobj));
				Console.WriteLine("RECIEVED HEADER-- size: {0} type {1} count: {2} action: {3}",sizeofobj,typeofobj,totalSize,action);
			}
			else
			{
				if(body.Count == myInterpreter.GetCount(header))
				{
					complete = true;
				}
			}
			
			
		}
		
		public bool IsComplete()
		{
			return complete;
		}
		

	}
}

