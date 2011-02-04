using System;
namespace NetLib
{
    public enum Action
    {
        Create = 0x60000000,
        Update = 0x10000000,
        Delete = 0x20000000
    }

    public enum Type
    {
        Player = 0x00000000,
        Enemy = 0x01000000,
        Circle = 0x03000000
    }

	public class PackageInterpreter
	{
		//Interpreter seems best suited to place TypeSize information
		public int GetTypeSize(Type type){
			switch (type)//these values need to be sorted out when the protocol is more sound
			{
			case Type.Player:
				return 1;
			case Type.Enemy:
				return 2;
			case Type.Circle:
				return 3;
			default:
				return 0;
			}
							
		}
		
		public PackageInterpreter ()
		{
		}
	}
}

