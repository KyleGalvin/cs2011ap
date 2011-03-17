using System;
using System.Collections.Generic;
using AP;
using OpenTK;

    /// <summary>
    /// Executes the packet commands
    /// </summary>
    public abstract class PackWorker
    {
		#region Fields (1) 
        protected GameState State;
        //private List<AP.Position> GameState;
        protected PackageInterpreter myInterpreter = new PackageInterpreter();

		#endregion Fields 

		#region Constructors (1) 

        public PackWorker()
        {
        }

		#endregion Constructors 

		#region Methods (4) 

		// Public Methods (4) 

        /// <summary>
        /// Handles the create.
        /// </summary>
        /// <param name="pack">The pack.</param>
        public virtual void HandleCreate(NetPackage pack)
        {
            
        }

        /// <summary>
        /// Handles the describe.
        /// </summary>
        /// <param name="pack">The pack.</param>
        public virtual void HandleDescribe(NetPackage pack)
        {
        }

        public virtual void HandleIdentify(NetPackage pack)
        {

        }
        public virtual void HandleUpdate(NetPackage pack)
        {

        }
        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="pack">The pack.</param>
        public virtual void HandleRequest(NetPackage pack, Connection conn)
        {
            
        }

        /// <summary>
        /// Handles the text.
        /// </summary>
        /// <param name="pack">The pack.</param>
        /// <returns></returns>
        public virtual String HandleText(NetPackage pack)
        {
            return String.Empty;
        }

		#endregion Methods 
    }

