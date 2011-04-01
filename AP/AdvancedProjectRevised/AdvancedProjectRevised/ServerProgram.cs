using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using OpenTK;

namespace AP
{
    class ServerProgram
    {
		#region Fields (23) 

        public static int bulletID = 0;
        public static CollisionAI collisionAI;
        private int currentLevel = 1;
        private bool enemySpawned = false;
        private  GameState gameState;
        private System.Timers.Timer gameTime = new System.Timers.Timer(50);
        List<int> heightSquares = new List<int>();
        CreateLevel level;
        private PathFinder mPathFinder;
        NetManager net;
        public static List<int> playerSpawnID = new List<int>();
        List<EnemySpawn> spawns = new List<EnemySpawn>();
        public Tiles tiles;
        private List<Wall> walls = new List<Wall>();
        List<int> widthSquares = new List<int>();
        public static List<int> xPosPlayerSpawn = new List<int>();
        List<int> xPosSpawn = new List<int>();
        List<int> xPosSquares = new List<int>();
        public static List<int> yPosPlayerSpawn = new List<int>();
        List<int> yPosSpawn = new List<int>();
        List<int> yPosSquares = new List<int>();
        private int zombieCount = 0;
        private int zombieIterator = 0;

		#endregion Fields 

		#region Constructors (1) 

        public ServerProgram()
        {
            // Set up the spawn locations for enemies
            setSpawns();
            gameState = new GameState();
            net = new HostManager(9999, ref gameState);
            net.setRole("server");
            setUpLevel();
            while (!net.Connected) { }
            Console.WriteLine("Connected!");
            Console.ReadLine();

            foreach (var x in gameState.Players)
            {
                x.tiles = tiles;
            }

            gameTime.Elapsed += new ElapsedEventHandler(gameLoop);
            gameTime.Enabled = true;
        }

		#endregion Constructors 

		#region Methods (3) 

		// Private Methods (3) 

        /// <summary>
        /// The main game loop
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void gameLoop(object sender, ElapsedEventArgs e)
        {
            List<Bullet> bulletDelete = new List<Bullet>();
            foreach (Bullet bullet in gameState.Bullets)
            {
                if (bullet.killProjectile())
                    bullet.timestamp = -1;

                if( bullet.timestamp > 0)
                    bullet.move();
                

                /*float moveX;
                float moveY;
                Enemy enemyHit;
                bool hit = collisionAI.checkForCollision(bullet, out moveX, out moveY, out enemyHit);
                if (hit)
                {
                    if (enemyHit.decreaseHealth())
                        gameState.Enemies.Remove(enemyHit);
                    GC.Collect();
                    bullet.timestamp = -1;
                }*/
            }

            net.SyncStateOutgoing();
        }

        /// <summary>
        /// Sets the spawns.
        /// </summary>
        private void setSpawns()
        {
            spawns.Clear();
            for (int index = 0; index < xPosSpawn.Count; index++)
            {
                spawns.Add(new EnemySpawn(xPosSpawn[index], yPosSpawn[index]));
            }
        }

        /// <summary>
        /// Sets up level.
        /// </summary>
        private void setUpLevel()
        {
            level = new CreateLevel(currentLevel);
            level.parseFile(ref xPosSquares, ref yPosSquares, ref heightSquares, ref widthSquares, ref xPosSpawn, ref yPosSpawn, ref xPosPlayerSpawn, ref yPosPlayerSpawn, ref playerSpawnID);
            
            collisionAI = new CollisionAI(ref xPosSquares, ref yPosSquares, ref widthSquares, ref heightSquares);
            for (int i = 0; i < xPosSquares.Count; i++)
            {
                walls.Add(new Wall(xPosSquares[i], yPosSquares[i], heightSquares[i], widthSquares[i]));
            }
            tiles = new Tiles(walls);
            mPathFinder = new PathFinder(tiles.byteList());
            
            
            setSpawns();
        }

		#endregion Methods 
    }
}
