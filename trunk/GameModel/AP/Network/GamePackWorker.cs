using System;
using System.Collections.Generic;
using System.Linq;
using AP;
namespace NetLib
{
    public class GamePackWorker : PackWorker
    {
        //private List<AP.Position> GameState;
        PackageInterpreter myInterpreter = new PackageInterpreter();
        GameState State;

        public GamePackWorker(ref GameState s)
        {
            State = s;
            //
            // TODO: Add constructor logic here
            //
        }
        public override String HandleText(NetPackage pack)
        {
            string result = "";
            for (int i = 0; i < pack.sizeofobj; )
            {
                result += BitConverter.ToString(pack.body[i]);
            }
            return result;
        }

        public override void HandleDescribe(NetPackage pack)
        {
            BitConverter.ToInt32(pack.body[0], 0);
        }

        public override void HandleCreate(NetPackage pack)
        {
            UInt32 myTypeSize = myInterpreter.GetTypeSize((Type)(pack.typeofobj << 24));

            //i=1 initially since the header is not data
            for (int i = 0; i < pack.count; i++)
            {
                UInt32 t = pack.typeofobj << 24;
                if ((Type)t == Type.AI)
                {
                    List<AP.Enemy> result = new List<AP.Enemy>();
                    result.Add(CreateAI(pack.body.GetRange((int)(i * myTypeSize), 5)));
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
                else if ((Type)t == Type.Bullet)
                {
                    List<AP.Player> result = new List<AP.Player>();
                    result.Add(CreatePlayer(pack.body.GetRange((int)(i * myTypeSize), 5)));
                    State.Players.AddRange(result);
                    Console.WriteLine("Created {0} Player objects from remote network command!", result.Count);
                }
            }

        }

        public override void HandleRequest(NetPackage pack)
        {
        }

        public List<AP.Position> HandleUpdate(NetPackage pack)
        {
            List<AP.Position> result = new List<AP.Position>();

            UInt32 myTypeSize = myInterpreter.GetTypeSize((Type)(pack.typeofobj << 24));
            //i=1 initially since the header is not data
            for (int i = 0; i < pack.count; i++)
            {
                var UID = BitConverter.ToUInt32(pack.body[0], 0);
                UInt32 t = pack.typeofobj << 24;
                if ((Type)t == Type.AI)
                {
                    State.Enemies.Where(y => y.UID == UID).First().Update(
                        pack.body[1], pack.body[2], pack.body[3], pack.body[4]); 
                }
                else if ((Type)t == Type.Player)
                {
                    //todo TEST!! This will probably break
                    State.Players.Where(y => y.UID == UID ).First().Update(
                        pack.body[1], pack.body[2], pack.body[3], pack.body[4]);
                }
                else if ((Type)t == Type.Bullet)
                {
                    //todo TEST!! This will probably break
                    State.Bullets.Where(y => y.UID == UID).First().Update(
                        pack.body[1], pack.body[2], pack.body[3], pack.body[4]);
                }
            }
            Console.WriteLine("Created {0} objects from remote network command!", result.Count);
            return result;
        
        }

        public AP.Enemy CreateAI(List<byte[]> data)
        {
            return new AP.Zombie(BitConverter.ToInt32(data[0], 0));
        }

        public AP.Player CreatePlayer(List<byte[]> data)
        {
            return new AP.Player(new OpenTK.Vector3(5,5, 0), BitConverter.ToInt32(data[0], 0));
        }
        public AP.Player CreateBullet(List<byte[]> data)
        {
            return new AP.Player(new OpenTK.Vector3(0, 0, 0), BitConverter.ToInt32(data[0], 0));
        }
    }
}
