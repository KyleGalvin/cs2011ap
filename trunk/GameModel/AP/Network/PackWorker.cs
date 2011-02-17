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
        //private List<AP.Position> GameState;
        protected PackageInterpreter myInterpreter = new PackageInterpreter();

        public PackWorker()
        {
        }

        public virtual String HandleText(NetPackage pack)
        {
            return "";
        }

        public virtual void HandleCreate(NetPackage pack)
        {
        }

        public virtual void HandleRequest(NetPackage pack)
        {
            
        }

        public virtual void HandleDescribe(NetPackage pack)
        {
        }
    }
}
