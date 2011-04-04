using System;
using System.Collections.Generic;
using System.Linq;

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
        public List<byte[]> newbody;

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
            body.Add(incoming.ToList().ToArray());
			if(body.Count == 1)
			{
				//no header has been created. We naievely assume the data read is a header
				
				header = BitConverter.ToUInt32(incoming,0);
                typeofobj = (header & 0x0F000000) ;
				action = (header & 0xF0000000);
                if (this.isLobby)
                {
                    sizeofobj = 1;
                }
                else
                {
                    sizeofobj = myInterpreter.GetTypeSize((Type)(typeofobj));
                }
                count = myInterpreter.GetCount(header);
                Console.WriteLine("RECIEVED PACKET HEADER -- Size of object: {0} Type: {1} Count: {2} Action: {3}", sizeofobj, ((Type)(typeofobj)).ToString(), count, ((Action)action).ToString());
			}
			else
			{
   
				if(body.Count == ((myInterpreter.GetCount(header)*sizeofobj))+1)
				{
                    Console.WriteLine("Packet complete: " + body.Count + "\n");
					complete = true;
				}
			}
			
			
		}

		#endregion Methods 
	}

