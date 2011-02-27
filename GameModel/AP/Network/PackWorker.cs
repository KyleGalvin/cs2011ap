using System;
using System.Collections.Generic;
using AP;
namespace NetLib
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public abstract class PackWorker
    {
		#region Fields (1) 

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

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="pack">The pack.</param>
        public virtual void HandleRequest(NetPackage pack)
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
}
