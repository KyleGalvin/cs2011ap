using System;
using System.Collections.Generic;
using AP;
namespace NetLib
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class PackWorker
    {
        //private List<AP.Position> GameState;
        PackageInterpreter myInterpreter = new PackageInterpreter();
        GameState State;

        public PackWorker(ref GameState s)
        {
            State = s;
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
        public void HandleCreate(NetPackage pack)
        {
            

            UInt32 myTypeSize = myInterpreter.GetTypeSize((Type)(pack.typeofobj<<24));

            //i=1 initially since the header is not data
            for (int i = 0; i < pack.count; i++)
            {
                UInt32 t = pack.typeofobj <<24;
                if ((Type)t == Type.AI)
                {
                    List<AP.Enemy> result = new List<AP.Enemy>();
                    result.Add(CreateAI(pack.body.GetRange((int)(i*myTypeSize),5)));
                    State.Enemies.AddRange(result);
                    Console.WriteLine("Created {0} AI objects from remote network command!", result.Count);
                }
                else if ((Type)t == Type.Player)
                {
                    List<AP.Player> result = new List<AP.Player>();
                    result.Add(CreatePlayer(pack.body.GetRange((int)(i * myTypeSize), 5)));
                    State.Players.AddRange(result);
                    Console.WriteLine("Created {0} Player objects from remote network command!", result.Count);
                }
            }
            
        }

        public List<AP.Position> HandleUpdate(NetPackage pack)
        {
            List<AP.Position> result = new List<AP.Position>();

            UInt32 myTypeSize = myInterpreter.GetTypeSize((Type)(pack.typeofobj << 24));

            //i=1 initially since the header is not data
            for (int i = 0; i < pack.count; i++)
            {
                UInt32 t = pack.typeofobj << 24;
                if ((Type)t == Type.AI)
                {
                    result.Add(CreateAI(pack.body.GetRange((int)(i * myTypeSize), 5)));
                }
                else if ((Type)t == Type.Player)
                {
                    result.Add(CreatePlayer(pack.body.GetRange((int)(i * myTypeSize), 5)));
                }
            }
            Console.WriteLine("Created {0} objects from remote network command!", result.Count);
            return result;
        }

        public AP.Enemy CreateAI(List<byte[]> data)
        {
            return new AP.Zombie(BitConverter.ToInt32(data[0],0));
        }

        public AP.Player CreatePlayer(List<byte[]> data)
        {
            return new AP.Player(new OpenTK.Vector3(0,0,0),BitConverter.ToInt32(data[0],0));
        }
    }
}
