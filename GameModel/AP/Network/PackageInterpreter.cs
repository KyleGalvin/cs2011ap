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
        Describe = 0x40000000
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
				return 8;
			case Type.AI:
				return 8;
			case Type.Building:
				return 8;
			default:
				return 0;
			}				
		}

        public List<byte[]> encode(Action a, Type t, List<AP.Position> objs)
        {
            List<byte[]> result = new List<byte[]>();
            UInt32 count = (UInt32)objs.Count;
            count = count << 16;
            UInt32 header = (UInt32)a ^ (UInt32)t ^ count;
            result.Add(BitConverter.GetBytes(header));

            Console.WriteLine("count!: {0}",count);
            foreach (AP.Position obj in objs)
            {
                Console.WriteLine("Adding object to buffer. size: {0}",result.Count);
                result.AddRange(serialize(t, obj));
            }
            return result;
        }

        private List<byte[]> serialize(Type t, AP.Position obj)
        {
            Console.WriteLine(obj.GetType());
            List<byte[]> result = new List<byte[]>();
            switch (t)
            {
                case Type.AI:
                    Console.WriteLine("xPos:{0}",obj.xPos);
                    result.Add(BitConverter.GetBytes((int)obj.xPos));
                    result.Add(BitConverter.GetBytes((int)obj.yPos));
                    result.Add(BitConverter.GetBytes((int)obj.xVel));
                    result.Add(BitConverter.GetBytes((int)obj.yVel));
                        //result.Add();
                    break;
                case Type.Building:
                    break;
                case Type.Bullet:
                    break;
                case Type.Explosion:
                    break;
                case Type.Player:
                    break;
                case Type.Powerup:
                    break;
                default:
                    break;
            }

            return result;
        }

        public int GetCount(UInt32 header)
        {
            return (int)(header & 0x00110000)>>16;
        }

		public PackageInterpreter ()
		{
		}
	}
}

