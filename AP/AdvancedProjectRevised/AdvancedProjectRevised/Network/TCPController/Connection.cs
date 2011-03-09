using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

    /// <summary>
    /// Holds the client connection and everyone connected to it. If your a client you will have one if your a server you will have up to 4
    /// </summary>
	public class Connection
	{
		#region Fields (4) 

		private TcpClient myClient;
        public StreamHandler myStream;
        public string playerName;
        public int playerUID;

		#endregion Fields 

		#region Constructors (1) 

		public Connection(object client)
		{
			myClient = (TcpClient)client;
			myStream = new StreamHandler(myClient.GetStream());
		}

		#endregion Constructors 

		#region Methods (4) 

		// Public Methods (4) 

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <returns></returns>
		public TcpClient GetClient()
		{
			return myClient;
		}

        /// <summary>
        /// Reads a full communication data packet.
        /// </summary>
        /// <returns></returns>
		public NetPackage ReadPackage()
		{
			return myStream.ReadPackage();
		}

        /// <summary>
        /// Writes the specified data to a communication packet.
        /// </summary>
        /// <param name="data">The data.</param>
		public void Write(List<byte[]> data)
		{
			myStream.Write(data);
		}

        /// <summary>
        /// Writes the specified data to a communication packet.
        /// </summary>
        /// <param name="data">The data.</param>
		public void Write(byte[] data)
		{
			myStream.Write(data);
		}

		#endregion Methods 
	}


