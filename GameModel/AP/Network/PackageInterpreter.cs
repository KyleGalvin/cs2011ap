using System;
using System.Collections.Generic;

namespace NetLib
{
    public enum Action
    {
        Create = 0x00000000,
        Delete = 0x10000000,
        Update = 0x20000000,
        Request = 0x30000000,
        Describe = 0x40000000,
        Text = 0x50000000,
    }

    public enum Type
    {
        Player = 0x00000000,
        AI = 0x01000000,
        Building = 0x03000000,
        Bullet = 0x04000000,
        Explosion = 0x05000000,
        Powerup = 0x06000000,
        Text = 0x07000000,
        Connection = 0x08000000
    }

	public class PackageInterpreter
	{
		//Interpreter seems best suited to place TypeSize information
		public UInt32 GetTypeSize(Type type){
            
			switch (type)//these values need to be sorted out when the protocol is more sound
			{
			case Type.Player:
				return 0x5;
			case Type.AI:
				return 0x5;
			case Type.Building:
				return 0x5;
            case Type.Text:
                return 0x1;
			default:
				return 0x0;
			}				
		}

        ///Lobby Communication Protocols
        public List<byte[]> encodeComm(Action a, Type t, String Comm)
        {
            Console.WriteLine("Encoding data:"+a.ToString()+" "+t.ToString()+" "+Comm);
            List<byte[]> result = new List<byte[]>();

            //how many 32 bit network numbers do we need to contain the string?
            UInt32 length = (UInt32)Comm.Length / 4;
            if (Comm.Length % 4 != 0)
            {
                length++;
            }

            //add packet header
            result.Add(BitConverter.GetBytes(((length) << 16) ^ ((UInt32)a) ^ ((UInt32)t) ));
            Console.WriteLine(BitConverter.ToString(result[0]));

            char[] carray = Comm.ToCharArray();

            UInt32 segment = 0;

            //add packet body
            for (int i = 0; i < carray.Length; i++)
            {
                int SegmentLen =0;
                while(SegmentLen !=4)
                {
                    if (i + SegmentLen > carray.Length)
                    {
                        segment ^= (UInt32)carray[i + SegmentLen] << (8*SegmentLen);
                    }
                    else
                    {
                    }

                    SegmentLen++;
                }
                Console.WriteLine("Segment:"+segment.ToString());
                result.Add(BitConverter.GetBytes(segment));
                segment = 0;
            }

            return result;
        }

        ///Game State Communication Protocols
        public List<byte[]> encodeObjs<T>(Action a, Type t, List<T> objs)
        {

            List<byte[]> result = new List<byte[]>();
            UInt32 count = (UInt32)objs.Count;
            count = count << 16;
            UInt32 header = (UInt32)a ^ (UInt32)t ^ count;
            result.Add(BitConverter.GetBytes(header));

            foreach (T obj in objs)
            {
                result.AddRange(serialize<T>(t, obj)); 
            }
            return result;
        }

        ///turns a list of objects into a serialized network stream
        private List<byte[]> serialize<T>(Type t, T obj)
        {

            Console.WriteLine(obj.GetType());
            List<byte[]> result = new List<byte[]>();
            //each type of object has a different composure.
            //here we define the structure of all possible types
            switch (t)
            {
                case Type.AI:
                    AP.Enemy e = (AP.Enemy)(object)obj;
                    result.Add(BitConverter.GetBytes((int)e.UID));
                    result.Add(BitConverter.GetBytes((int)e.xPos));
                    result.Add(BitConverter.GetBytes((int)e.yPos));
                    result.Add(BitConverter.GetBytes((int)e.xVel));
                    result.Add(BitConverter.GetBytes((int)e.yVel));
                    break;
                case Type.Building:
                    break;
                case Type.Bullet:
                    break;
                case Type.Explosion:
                    break;
                case Type.Player:
                    AP.Player p = (AP.Player)(object)obj;
                    result.Add(BitConverter.GetBytes((int)p.UID));
                    result.Add(BitConverter.GetBytes((int)p.xPos));
                    result.Add(BitConverter.GetBytes((int)p.yPos));
                    result.Add(BitConverter.GetBytes((int)p.xVel));
                    result.Add(BitConverter.GetBytes((int)p.yVel));
                    break;
                case Type.Powerup:
                    break;
                default:
                    break;
            }

            return result;
        }

        public UInt32 GetCount(UInt32 header)
        {
            return (header & 0x00FF0000) >> 16;
        }

		public PackageInterpreter ()
		{
		}
	}
}

