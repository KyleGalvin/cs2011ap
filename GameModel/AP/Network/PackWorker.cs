using System;
using System.Collections.Generic;
namespace NetLib
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class PackWorker
    {
        private List<AP.Position> GameState;
        PackageInterpreter myInterpreter = new PackageInterpreter();

        public PackWorker(ref List<AP.Position> StatePtr)
        {
            GameState = StatePtr;
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
                Type t = (Type)pack.typeofobj;
                if (t == Type.AI)
                {
                    result.Add(CreateAI(pack.body.GetRange(i,5)));
                }
                else if (t == Type.Player)
                {

                }
            }
            Console.WriteLine("Created {0} objects from remote network command!");
          return result;
        }

        public List<AP.Position> HandleUpdate(NetPackage pack)
        {
            return new List<AP.Position>();
        }

        public AP.Enemy CreateAI(List<byte[]> data)
        {
            return new AP.Zombie(BitConverter.ToInt32(data[0],0));
        }
    }
}
