using System;
using System.Collections.Generic;
using AP;
using System.Linq;
using OpenTK;

    /// <summary>
    /// Executes the packet commands
    /// </summary>
    public class PackWorker
    {
		#region Fields (1) 
        protected GameState State;
        //private List<AP.Position> GameState;
        protected PackageInterpreter myInterpreter = new PackageInterpreter();

		#endregion Fields 

		#region Constructors (1) 

        public PackWorker(ref GameState s)
        {
            State = s;
        }

		#endregion Constructors 


        #region Methods (8)

        // Public Methods (8) 

        /// <summary>
        /// Creates the AI.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public AP.Enemy CreateAI(List<byte[]> data)
        {
            return new AP.Zombie(BitConverter.ToInt32(data[0], 0));
        }

        /// <summary>
        /// Creates the bullet.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public AP.Bullet CreateBullet(List<byte[]> data)
        {   
            Bullet b = new AP.Bullet( new OpenTK.Vector3(BitConverter.ToSingle(data[1], 0), BitConverter.ToSingle(data[2], 0),0), new OpenTK.Vector2(BitConverter.ToSingle(data[3], 0), BitConverter.ToSingle(data[4], 0)));
            b.setID(BitConverter.ToInt32( data[0],0));
            return b;
        }

        /// <summary>
        /// Creates the player.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public AP.Player CreatePlayer(List<byte[]> data)
        {
            return new AP.Player(new OpenTK.Vector3(BitConverter.ToSingle( data[1],0),BitConverter.ToSingle( data[2],0), 0), BitConverter.ToInt32(data[0], 0));
        }

        public void HandleDelete(NetPackage pack)
        {
            Int32 myTypeSize = (Int32)myInterpreter.GetTypeSize((Type)(pack.typeofobj));

            for (int i = 0; i < pack.count; i++)
            {
                UInt32 t = pack.typeofobj;
                if ((Type)t == Type.Bullet)
                {
                    Int32 ID = BitConverter.ToInt32(pack.body[(i*myTypeSize)+1], 0);
                    State.Bullets.Remove(State.Bullets.Where(y => y.UID == ID).First()) ;
                }
            }
        }

        /// <summary>
        /// Handles the create.
        /// </summary>
        /// <param name="pack">The pack.</param>
        public void HandleCreate(NetPackage pack)
        {
            UInt32 myTypeSize = myInterpreter.GetTypeSize((Type)(pack.typeofobj));

            //i=1 initially since the header is not data
            for (int i = 0; i < pack.count; i++)
            {
                UInt32 t = pack.typeofobj;
                if ((Type)t == Type.AI)
                {
                    List<AP.Enemy> result = new List<AP.Enemy>();
                    result.Add(CreateAI(pack.body.GetRange((int)(i * myTypeSize) + 1, 5)));
                    State.Enemies.AddRange(result);
                    Console.WriteLine("Created {0} AI objects from remote network command!", result.Count);
                }
                else if ((Type)t == Type.Player)
                {
                    List<AP.Player> result = new List<AP.Player>();
                    result.Add(CreatePlayer(pack.body.GetRange((int)(i * myTypeSize) + 1, 5)));
                    result.Last().modelNumber = ClientProgram.loadedObjectPlayer;
                    State.Players.AddRange(result);
                    Console.WriteLine("Created {0} Player objects from remote network command!", result.Count);
                }
                else if ((Type)t == Type.Bullet)
                {
                    List<AP.Bullet> result = new List<AP.Bullet>();
                    result.Add(CreateBullet(pack.body.GetRange((int)(i * myTypeSize) + 1, 5)));
                    State.Bullets.AddRange(result);
                    Console.WriteLine("Created {0} Bullet objects from remote network command!", result.Count);
                }
            }

        }

        /// <summary>
        /// Handles the describe.
        /// </summary>
        /// <param name="pack">The pack.</param>
        public void HandleDescribe(NetPackage pack)
        {
            BitConverter.ToInt32(pack.body[0], 0);
        }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="pack">The pack.</param>
        public void HandleRequest(NetPackage pack, Connection conn)
        {
            UInt32 myTypeSize = myInterpreter.GetTypeSize((Type)(pack.typeofobj));
            
                UInt32 t = pack.typeofobj;
                if ((Type) t == Type.Move)
                {
                    Console.WriteLine("Handle request");
                    Console.WriteLine("Player before handling move request: xPos: " + State.Players.Where(y => y.playerId == conn.playerUID).First().xPos + " yPos: " + State.Players.Where(y => y.playerId == conn.playerUID).First().yPos);
                    State.Players.Where(y => y.playerId == conn.playerUID).First().move(BitConverter.ToInt32(pack.body[1], 0), BitConverter.ToInt32(pack.body[2], 0));
                    Console.WriteLine("Player after handling move request: xPos: " + State.Players.Where(y => y.playerId == conn.playerUID).First().xPos + " yPos: " + State.Players.Where(y => y.playerId == conn.playerUID).First().yPos);
                }
                if ((Type)t == Type.Bullet)
                {
                    for (int i = 0; i < pack.count; i++)
                    {
                        State.Players.Where(y => y.playerId == conn.playerUID).First().weapons.shoot(ref State.Bullets, new Vector3(BitConverter.ToSingle(pack.body[(int)(i * myTypeSize) + 2], 0), BitConverter.ToSingle(pack.body[(int)(i * myTypeSize) + 3], 0), 0), new Vector2(800, 800), new Vector2(BitConverter.ToSingle(pack.body[(int)(i * myTypeSize) + 4], 0), BitConverter.ToSingle(pack.body[(int)(i * myTypeSize) + 5], 0)));
                        Console.WriteLine("Player before handling move request: xPos: " + State.Bullets.Last().xPos + " yPos: " + State.Bullets.Last().yPos);
                    }
                }
            
        }

        public int HandleIdentify(NetPackage pack)
        {
            State.myUID = BitConverter.ToInt32(pack.body[1], 0);
            Console.WriteLine("MY UID is set to " + State.myUID);
            Console.WriteLine("PACKAGE " + BitConverter.ToInt32(pack.body[1], 0));
            return State.myUID;
        }


        /// <summary>
        /// Handles the text.
        /// </summary>
        /// <param name="pack">The pack.</param>
        /// <returns></returns>
        public String HandleText(NetPackage pack)
        {
            string result = "";
            for (int i = 0; i < pack.sizeofobj; )
            {
                result += BitConverter.ToString(pack.body[i]);
            }
            return result;
        }

        /// <summary>
        /// Handles the update.
        /// </summary>
        /// <param name="pack">The pack.</param>
        /// <returns></returns>
        public void HandleUpdate(NetPackage pack)
        {
            List<AP.Position> result = new List<AP.Position>();

            UInt32 myTypeSize = myInterpreter.GetTypeSize((Type)(pack.typeofobj));
            //i=1 initially since the header is not data
            for (int i = 0; i < pack.count; i++)
            {
                var UID = BitConverter.ToUInt32(pack.body[(i * (int)myTypeSize) + 1], 0);
                UInt32 t = pack.typeofobj;
                if ((Type)t == Type.AI)
                {
                    State.Enemies.Where(y => y.UID == UID).First().Update(
                        pack.body[(i * (int)myTypeSize) + 1], pack.body[(i * (int)myTypeSize) + 2], pack.body[(i * (int)myTypeSize) + 3], pack.body[(i * (int)myTypeSize) + 4]);
                }
                else if ((Type)t == Type.Player)
                {
                    State.Players.Where(y => y.playerId == UID).First().Update(
                        pack.body[(i * (int)myTypeSize) + 2], pack.body[(i * (int)myTypeSize) + 3]
                        , pack.body[(i * (int)myTypeSize) + 4], pack.body[(i * (int)myTypeSize) + 5]);
                    //Console.WriteLine("HANDLE UPDATE: " + (float)BitConverter.ToSingle(pack.body[0], 0) + " " + (float)BitConverter.ToSingle(pack.body[1], 0) + " " + (float)BitConverter.ToSingle(pack.body[2], 0) + " " + (float)BitConverter.ToSingle(pack.body[3], 0) + " " + BitConverter.ToSingle(pack.body[4], 0));
                }
            }
            Console.WriteLine("Created {0} objects from remote network command!", result.Count);
            //return result;

        }


		#endregion Methods 
    }

