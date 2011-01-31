using System;
using System.Collections.Generic;

namespace NetLib
{
	public class NetPackage
	{
		public UInt32 header;
		public UInt32 size;
		public UInt32 count;
		public List<byte[]> body;
		public bool complete;
		
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
				UInt32 type = (header & 0x0F000000)>>24;
				size = GetSize(type);
				count = (header & 0x00FF0000)>>16;
			}
			else
			{
				body.Add(incoming);
				if(body.Count == size*count)
				{
					complete = true;
				}
			}
			
			
		}
		
		public bool IsComplete()
		{
			return complete;
		}
		
		public UInt32 GetSize(UInt32 type)
		{
			switch (type)
			{
			case ((UInt32)Type.Player)://player
				return 8;
			case (UInt32)Type.Enemy://AI
				return 8;
			case 2://building
				return 8;;
			case (UInt32)Type.Circle://circle
				return 6;
			case 4://explosion
				return 8;
			case 5://power-up
				return 8;
			default:
				return 0;
			}
		}
	}
}

