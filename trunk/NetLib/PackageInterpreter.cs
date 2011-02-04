using System;
namespace NetLib
{
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

