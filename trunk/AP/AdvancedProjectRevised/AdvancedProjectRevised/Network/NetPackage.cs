using System;
using System.Collections.Generic;

namespace NetLib
{
    /// <summary>
    /// Representation of a packet
    /// </summary>
	public class NetPackage
	{
		#region Fields (10) 

		public UInt32 action;
		public List<byte[]> body;
		public bool complete;
        public UInt32 count;
		public UInt32 header;
        public bool isLobby;
        public PackageInterpreter myInterpreter;
		public UInt32 sizeofobj;
		public UInt32 totalSize;
		public UInt32 typeofobj;

		#endregion Fields 

		#region Constructors (1) 

		public NetPackage ()
		{
            myInterpreter = new PackageInterpreter();
			complete = false;
			header = 0;
			body = new List<byte[]>();
		}

		#endregion Constructors 

		#region Methods (2) 

		// Public Methods (2) 

        /// <summary>
        /// Determines whether this instance is complete.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is complete; otherwise, <c>false</c>.
        /// </returns>
		public bool IsComplete()
		{
			return complete;
		}

        /// <summary>
        /// Recieves the specified incoming.
        /// </summary>
        /// <param name="incoming">The incoming.</param>
		public void Recieve(byte[] incoming)
		{
			//we only intend to recieve 4 bytes at a time
			body.Add(incoming);
			
			if(body.Count == 1)
			{
				//no header has been created. We naievely assume the data read is a header
				
				header = BitConverter.ToUInt32(incoming,0);
                typeofobj = (header & 0x0F000000) >> 24;
				action = (header & 0xF0000000);
                Console.WriteLine("action: {0}", (Action)action);
                if (this.isLobby)
                {
                    sizeofobj = 1;
                }
                else
                {
                    sizeofobj = myInterpreter.GetTypeSize((Type)(0x01000000));
                }
                Console.WriteLine("action: {0}", (Action)action);
                count = myInterpreter.GetCount(header);
                Console.WriteLine("action: {0}", (Type)typeofobj);
				//Console.WriteLine("GetSize(type): {0}",GetSize(typeofobj));
				Console.WriteLine("RECIEVED HEADER-- size of typeobj: {0} type {1} count: {2} action: {3}",sizeofobj,typeofobj,count,action);
                Console.WriteLine(header);
			}
			else
			{
                Console.WriteLine("Data {0} of {1}: {2}", body.Count, ((myInterpreter.GetCount(header)*sizeofobj) + 1),BitConverter.ToString(body[body.Count-1]));
				if(body.Count == (myInterpreter.GetCount(header)*sizeofobj))
				{
					complete = true;
				}
			}
			
			
		}

		#endregion Methods 
	}
}

