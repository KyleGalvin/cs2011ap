using System;
using System.Collections.Generic;
namespace NetLib
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class PackWorker
    {
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
           // for(int i=0;  i< pack.)pack.sizeofobj
          return new List<AP.Position>();
        }
    }
}
