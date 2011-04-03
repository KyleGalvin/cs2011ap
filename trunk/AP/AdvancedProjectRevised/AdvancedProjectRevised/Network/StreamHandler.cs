using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

    /// <summary>
    /// Handles the incoming/out-going data
    /// </summary>
    public class StreamHandler
    {
		#region Fields (3) 

        protected PackageInterpreter myInterpreter;
        public NetPackage myPackage;
        protected NetworkStream myStream;

		#endregion Fields 

		#region Constructors (1) 

        //StateWorker myWorker;
        public StreamHandler(NetworkStream stream)
        {
            myStream = stream;
            myPackage = new NetPackage();
            myInterpreter = new PackageInterpreter();
        }

		#endregion Constructors 

		#region Methods (4) 

		// Public Methods (4) 

        /// <summary>
        /// Reads a full communication layer packet.
        /// </summary>
        /// <returns></returns>
        public NetPackage ReadPackage()
        {
            
            myPackage=new NetPackage();
            //keep reading from stream until pack.iscomplete is true
            byte[] rawMessage = new byte[4];

            //segment number
            int i = 0;
            int bytesRead = 0;

            while (myPackage.IsComplete() == false)
            {//recieve body
                bytesRead += myStream.Read(rawMessage, 0, 4);
                myPackage.Recieve(rawMessage);
            }

            return myPackage;
        }

        /// <summary>
        /// Blindly writes the specified data. It is up to the NetManager (or appropriate child) to ensure this is a properly formed packet.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(List<byte[]> data)
        {
            foreach (byte[] chunk32 in data)
            {
                //Console.WriteLine("Writing to stream: {0}", BitConverter.ToString(chunk32));
                myStream.Write(chunk32, 0, 4);//write the data in 32 bit blocks
            }
            myStream.Flush();
        }

        /// <summary>
        /// Blindly writes the specified data. 
        /// It is up to the NetManager (or appropriate child) to ensure this is a properly formed packet.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(byte[] data)
        {
            //we assume 32 bits of data only
            myStream.Write(data, 0, 4);
            myStream.Flush();
            Console.WriteLine("Write single data to socket here");
        }

        /// <summary>
        /// Writes the packet. Since we have the packet data structure, we can ensure it is well-formed.
        /// </summary>
        /// <param name="pack">The pack.</param>
        public void WritePackage(NetPackage pack)
        {
            myStream.Write(BitConverter.GetBytes(pack.header), 0, 4);

            foreach (byte[] chunk32 in pack.body)
            {
                Console.WriteLine("Writing to stream: {0}", BitConverter.ToString(chunk32));
                myStream.Write(chunk32, 0, 4);
            }
            myStream.Flush();
            Console.WriteLine("Write multi data to socket here");
        }

		#endregion Methods 
    }
