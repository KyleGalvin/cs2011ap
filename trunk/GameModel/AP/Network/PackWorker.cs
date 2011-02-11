using System;
using System.Collections.Generic;
namespace NetLib
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class PackWorker
    {
        PackageInterpreter myInterpreter = new PackageInterpreter();
        public PackWorker()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public String HandleText(NetPackage pack)
        {
            string result = "";
            for (int i = 0; i < pack.sizeofobj; )
            {
                result += BitConverter.ToString(pack.body[i]);
            }
                return result;
        }
        public List<AP.Position> HandleCreate(NetPackage pack)
        {
            List<AP.Position> result = new List<AP.Position>();

            UInt32 myTypeSize = myInterpreter.GetTypeSize((Type)pack.typeofobj);

            //i=1 initially since the header is not data
            for (int i = 1; i < pack.count; i += (int)myTypeSize)
            {
                for (int j = 0; j < myTypeSize; j++)
                {
                    Type t = pack.typeofobj;
                    if (t == Type.AI)
                    {

                    }
                    else if (t == Type.Player)
                    {

                    }
                }
            }
          return new List<AP.Position>();
        }

        public List<AP.Position> HandleUpdate(NetPackage pack)
        {
            return new List<AP.Position>();
        }
    }
}
