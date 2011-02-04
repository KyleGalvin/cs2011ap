using System;
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
		public int GetTypeSize(Type type){
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

        public int GetCount(UInt32 header)
        {
            return (int)(header & 0x00110000)>>16;
        }

		public PackageInterpreter ()
		{
		}
	}
}

