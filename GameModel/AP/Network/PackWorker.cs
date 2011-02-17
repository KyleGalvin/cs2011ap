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

        public String HandleText(NetPackage pack)
        {
            return "";
        }

        public void HandleCreate(NetPackage pack)
        {
        }

        public void HandleRequest(NetPackage pack)
        {
        }

        public void HandleDescribe(NetPackage pack)
        {
        }
    }
}
