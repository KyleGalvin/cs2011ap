using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using OpenTK;

namespace AP
{
    /// <summary>
    /// This is the server that the clients connect to.
    /// Contributors: Scott Herman, Gage Patterson, Kyle Galvin, Todd Burton
    /// Revision: 294
    /// </summary>
    class ServerProgram
    {
		#region Fields (17) 

        public static int bulletID = 0;
        public static CollisionAI collisionAI;
        private int currentLevel = 1;
        private bool enemySpawned = false;
        private  GameState gameState;
        private System.Timers.Timer gameTime = new System.Timers.Timer(50);
        List<int> heightSquares = new List<int>();
        CreateLevel level;
        NetManager net;
        List<EnemySpawn> spawns = new List<EnemySpawn>();
        List<int> widthSquares = new List<int>();
        List<int> xPosSpawn = new List<int>();
        List<int> xPosSquares = new List<int>();
        List<int> yPosSpawn = new List<int>();
        List<int> yPosSquares = new List<int>();
        private int zombieCount = 0;
        private int zombieIterator = 0;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerProgram"/> class.
        /// </summary>
        public ServerProgram()
        {
            setUpLevel();
            
            // Set up the spawn locations for enemies
            setSpawns();
            gameState = new GameState();
            net = new HostManager(9999, ref gameState);
            net.setRole("server");
            while (!net.Connected) { }
            Console.WriteLine("Connected!");
            Console.ReadLine();
            gameTime.Elapsed += new ElapsedEventHandler(gameLoop);
            gameTime.Enabled = true;
        }

		#endregion Constructors 

		#region Methods (3) 

		// Private Methods (3) 

        /// <summary>
        /// Main game loop used to control the game state
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void gameLoop(object sender, ElapsedEventArgs e)
        {
            List<Bullet> bulletDelete = new List<Bullet>();
            foreach (Bullet bullet in gameState.Bullets)
            {
                //if( bullet.timestamp > 0)
                    //bullet.move();
                

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
                if (bullet.killProjectile())
                    bullet.timestamp = -1;
            }

            net.SyncStateOutgoing();
        }

        /// <summary>
        /// Sets the spawns.
        /// </summary>
        private void setSpawns()
        {
            spawns.Clear();
            if (xPosSpawn.Count > 0)
            {
                spawns.Add(new EnemySpawn(xPosSpawn[0], yPosSpawn[0]));
            }
            if (xPosSpawn.Count > 1)
            {
                spawns.Add(new EnemySpawn(xPosSpawn[1], yPosSpawn[1]));
            }
            if (xPosSpawn.Count > 2)
            {
                spawns.Add(new EnemySpawn(xPosSpawn[2], yPosSpawn[2]));
            }
            if (xPosSpawn.Count > 3)
            {
                spawns.Add(new EnemySpawn(xPosSpawn[3], yPosSpawn[3]));
            }
        }

        /// <summary>
        /// Sets up level.
        /// </summary>
        private void setUpLevel()
        {
            level = new CreateLevel(currentLevel);
            level.parseFile(ref xPosSquares, ref yPosSquares, ref heightSquares, ref widthSquares, ref xPosSpawn, ref yPosSpawn);
            
            collisionAI = new CollisionAI(ref xPosSquares, ref yPosSquares, ref widthSquares, ref heightSquares);

            setSpawns();
        }

		#endregion Methods 
    }
}
