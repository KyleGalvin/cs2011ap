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
        public UInt32 count;
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
			
			if(body.Count == 1)
			{
				//no header has been created. We naievely assume the data read is a header
				
				header = BitConverter.ToUInt32(incoming,0);
				typeofobj = (header & 0x0F000000)>>24;
				action = (header & 0xF0000000);
                Console.WriteLine("action: {0}", action);
                sizeofobj = myInterpreter.GetTypeSize((Type)(header & 0x01000000));
                count = myInterpreter.GetCount(header);
				//Console.WriteLine("GetSize(type): {0}",GetSize(typeofobj));
				Console.WriteLine("RECIEVED HEADER-- size of typeobj: {0} type {1} count: {2} action: {3}",sizeofobj,typeofobj,count,action);
			}
			else
			{
                Console.WriteLine("Data {0} of {1}: {2}", body.Count, ((myInterpreter.GetCount(header)*sizeofobj) + 1),BitConverter.ToString(body[body.Count-1]));
				if(body.Count == (myInterpreter.GetCount(header)*sizeofobj)+1)
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

