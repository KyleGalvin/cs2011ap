using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AP.Network
{
    /// <summary>
    /// Used to hold the information about the server.
    /// </summary>
    public class Server
    {
		#region Fields (2) 

        public string Name;
        public IPAddress ServerIP;

		#endregion Fields 

		#region Constructors (1) 

        public Server(string _Name, IPAddress _ServerIP)
        {
            Name = _Name;
            ServerIP = _ServerIP;
        }

		#endregion Constructors 

		#region Methods (1) 

		// Public Methods (1) 

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

		#endregion Methods 
    }
}
