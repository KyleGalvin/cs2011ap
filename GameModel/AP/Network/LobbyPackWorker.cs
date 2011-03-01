using System;
using AP;
using System.Windows.Forms;

namespace NetLib
{
    /// <summary>
    /// Executes the packet commands for the lobby
    /// </summary>
    public class LobbyPackWorker : PackWorker
    {
		#region Fields (1) 

        public ListBox list;

		#endregion Fields 

		#region Constructors (1) 

        public LobbyPackWorker(ref ListBox list)
        {
            this.list = list;
        }

		#endregion Constructors 

		#region Methods (3) 

		// Public Methods (3) 

        /// <summary>
        /// Handles the create.
        /// </summary>
        /// <param name="pack">The pack.</param>
        public override void HandleCreate(NetPackage pack)
        {
            //start game. no body
        }

        /// <summary>
        /// Handles the describe.
        /// </summary>
        /// <param name="pack">The pack.</param>
        public override void HandleDescribe(NetPackage pack)
        {
            if (pack.typeofobj == (UInt32)Type.Building)
            {
                //create game in lobby. pack.body contains game name
                list.Items.Add("dafdf");
            }
            else if (pack.typeofobj == (UInt32)Type.Player)
            {
                //Add player to connections. pack.body contains playername
            }
        }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="pack">The pack.</param>
        public override void HandleRequest(NetPackage pack)
        {
            Console.WriteLine("HERE");
            //join game. pack.body contains game name
            list.Items.Add("PlayerName");
        }

		#endregion Methods 
    }

}