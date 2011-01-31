using System;
using System.Collections.Generic;

namespace NetLib
{
	public class NetPackage
	{
		UInt32 header;
		List<byte[]> body;
		bool complete;
		
		public NetPackage ()
		{
			complete = false;
			header = 0;
			body = new List<byte[]>();
		}
		
		public void Recieve(byte[] incoming)
		{
			//we only intend to recieve 4 bytes at a time
			
			if(header == 0)
			{
				//no header has been created. We naievely assume the data read is a header
				header = BitConverter.ToUInt32(incoming,0);
			}
			else
			{
				body.Add(incoming);
			}
			
			
		}
		
		public bool IsComplete()
		{
			return complete;
		}
	}
}

