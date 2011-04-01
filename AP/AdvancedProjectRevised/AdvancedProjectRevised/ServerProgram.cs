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
        public static int enemyID = 0;
        public static CollisionAI collisionAI;
        private int currentLevel = 1;
        private bool enemySpawned = false;
        private  GameState gameState;
        private System.Timers.Timer gameTime = new System.Timers.Timer(50f);
        List<int> heightSquares = new List<int>();
        CreateLevel level;
        private PathFinder mPathFinder;
        NetManager net;
        public static bool status;
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
            status = true;
            
            for (int i = 0; i < xPosSquares.Count; i++)
            {
                walls.Add(new Wall(xPosSquares[i], yPosSquares[i], heightSquares[i], widthSquares[i]));
            }
            tiles = new Tiles(walls);
            mPathFinder = new PathFinder(tiles.byteList());
            collisionAI.wallTiles = tiles;
            collisionAI.playerList=gameState.Players;
            for (var x = 0; x < gameState.Players.Count;x++ )
            {
                gameState.Players[x].tiles = tiles;
            }
            
            gameTime.Elapsed += new ElapsedEventHandler(gameLoop);
            gameTime.Enabled = true;
        }

		#endregion Constructors 

		#region Methods (3) 

		// Private Methods (3) 
        /// <summary>
        /// Handles the spawning.
        /// </summary>
        private void HandleSpawning()
        {
            zombieIterator++;
            if (zombieCount < 100)
            {
                foreach (var spawn in spawns)
                {
                    if (zombieIterator == 40)
                    {
                        lock (gameState)
                        {
                            //need to ping server for a UID
                            gameState.Enemies.Add(spawn.spawnEnemy(enemyID++));
                            enemySpawned = true;
                            zombieCount++;
                        }

                    }
                }
                if (enemySpawned)
                {
                    zombieIterator = 0;
                    enemySpawned = false;
                }
            }
        }
        /// <summary>
        /// Handles the pathing.
        /// </summary>
        private void HandlePathing()
        {
            for (int index = gameState.Enemies.Count-1; index >=0 ; index--)
                {
                    var zombie = gameState.Enemies[index];
                    var len4 = 999.0f;
                    var player = new Player();
                    //Find closest player
                    for (var i = 0; i < gameState.Players.Count; i++)
                    {
                        float x12 = gameState.Players[i].xPos - zombie.xPos;
                        float y12 = gameState.Players[i].yPos - zombie.yPos;
                        var len12 = (float)Math.Sqrt(x12 * x12 + y12 * y12);
                        if (len12 < len4)
                        {
                            len4 = len12;
                            player = gameState.Players[i];
                        }
                    }
                    var playerPos = tiles.returnTilePos(player);
                    var enemyPos = tiles.returnTilePos(zombie);
                    //Check to see how close the zombie is to the player
                    float x1 = player.xPos - zombie.xPos;
                    float y1 = player.yPos - zombie.yPos;
                    float len1 = (float)Math.Sqrt(x1 * x1 + y1 * y1);
                    if (len1 <= 1.1)
                    {
                        zombie.moveTowards(player);
                    }
                    //Find a path
                    else if (enemyPos != null)
                    {
                        if (playerPos != null)
                        {

                            List<PathFinderNode> path = mPathFinder.FindPath(enemyPos[0], enemyPos[1], playerPos[0],
                                                                             playerPos[1]);
                            //Give next X Y Coords
                            if (path != null && path.Count > 1)
                            {
                                var nextMove = tiles.returnCoords(path[1].X, path[1].Y);
                                //Move towards them
                                //Calculates the len between the moves
                                float x = nextMove[0] - zombie.xPos;
                                float y = nextMove[1] - zombie.yPos;
                                float len = (float)Math.Sqrt(x * x + y * y);
                                if (len < 1 && path.Count > 2)
                                {
                                    nextMove = tiles.returnCoords(path[2].X, path[2].Y);
                                    zombie.moveTowards(nextMove[0], nextMove[1]);
                                }
                                else
                                {
                                    zombie.moveTowards(nextMove[0], nextMove[1]);
                                }
                            }
                        }
                    }
                
            }
        }

        /// <summary>
        /// The main game loop
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void gameLoop(object sender, ElapsedEventArgs e)
        {
            List<Bullet> bulletDelete = new List<Bullet>();
            HandleSpawning();
            //HandlePathing();
            foreach (Bullet bullet in gameState.Bullets)
            {
                if (bullet.killProjectile())
                    bullet.timestamp = -1;
                if (tiles.isWall(bullet.xPos, bullet.yPos))
                {
                    bullet.timestamp = -1;
                }
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
