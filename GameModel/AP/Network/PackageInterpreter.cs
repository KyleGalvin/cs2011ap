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
        Text = 0x50000000
    }

    public enum Type
    {
        Player = 0x00000000,
        AI = 0x01000000,
        Building = 0x03000000,
        Bullet = 0x04000000,
        Explosion = 0x05000000,
        Powerup = 0x06000000
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
			default:
				return 0x0;
			}				
		}

        //sends a string over the network
        public List<byte[]> encodeText(String s)
        {
            List<byte[]> result = new List<byte[]>();
            result.Add(BitConverter.GetBytes((UInt32)(s.Length<<16) ^ (UInt32)Action.Text));

            while (s.Length % 4 != 0)
            {
                s += " ";
            }

            char[] carray = s.ToCharArray();
            for(int i = 0; i < carray.Length;i=i+4)
            {
                UInt32 segment = (UInt32)(carray[i]) ^ (UInt32)(carray[i + 1] << 8) ^ (UInt32)(carray[i + 2] << 16) ^ (UInt32)(carray[i + 3] << 24);
                result.Add( BitConverter.GetBytes(segment));

            }

            return result;
        }

        //turns a list of objects into a serialized network stream
        public List<byte[]> encode(Action a, Type t, List<AP.Position> objs)
        {
            List<byte[]> result = new List<byte[]>();
            UInt32 count = (UInt32)objs.Count;
            count = count << 16;
            UInt32 header = (UInt32)a ^ (UInt32)t ^ count;
            result.Add(BitConverter.GetBytes(header));

            foreach (AP.Position obj in objs)
            {
                result.AddRange(serialize(t, obj));
            }
            return result;
        }

        private List<byte[]> serialize(Type t, AP.Position obj)
        {
            Console.WriteLine(obj.GetType());
            List<byte[]> result = new List<byte[]>();

            //each type of object has a different composure.
            //here we define the structure of all possible types
            switch (t)
            {
                case Type.AI:
                    result.Add(BitConverter.GetBytes((int)obj.UID));
                    result.Add(BitConverter.GetBytes((int)obj.xPos));
                    result.Add(BitConverter.GetBytes((int)obj.yPos));
                    result.Add(BitConverter.GetBytes((int)obj.xVel));
                    result.Add(BitConverter.GetBytes((int)obj.yVel));
                    break;
                case Type.Building:
                    break;
                case Type.Bullet:
                    break;
                case Type.Explosion:
                    break;
                case Type.Player:
                    result.Add(BitConverter.GetBytes((int)obj.UID));
                    result.Add(BitConverter.GetBytes((int)obj.xPos));
                    result.Add(BitConverter.GetBytes((int)obj.yPos));
                    result.Add(BitConverter.GetBytes((int)obj.xVel));
                    result.Add(BitConverter.GetBytes((int)obj.yVel));
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

