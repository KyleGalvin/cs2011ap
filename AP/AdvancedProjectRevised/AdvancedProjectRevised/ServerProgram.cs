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
        private int ammoCounter=0;
        private int crateCounter = 0;
        private System.Timers.Timer bulletTime = new System.Timers.Timer(35f);
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
        /// Handles the crates.
        /// </summary>
        private void HandleCrates()
        {
            foreach (var p in gameState.Players)
            {
                foreach (Crate crate in gameState.Crates)
                {
                    float diffX = p.xPos - crate.xPos;
                    float diffY = p.yPos - crate.yPos;
                    if ((float) Math.Sqrt(diffX*diffX + diffY*diffY) <= p.radius + crate.radius)
                    {
                        if (crate.crateType == 0)
                            p.weapons.rifleAmmo += 25;
                        else if (crate.crateType == 1)
                            p.weapons.shotgunAmmo += 5;
                        crate.timestamp = -1;
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
            HandleSpawning();
            for (var i =0;i< gameState.Bullets.Count ; i++)
            {

                for (int j = 0; j < gameState.Players.Count; j++)
                {
                    if(gameState.Bullets[i].playerID==gameState.Players[j].playerId) continue;
                    float x1 = gameState.Players[j].xPos - gameState.Bullets[i].xPos;
                    float y1 = gameState.Players[j].yPos - gameState.Bullets[i].yPos;
                    float len1 = (float)Math.Sqrt(x1 * x1 + y1 * y1);
                    if (len1 <= gameState.Players[j].radius + 0.5f)
                    {
                        //gameState.Bullets[i].timestamp = -1;
                        gameState.Players[j].prevHealth = gameState.Players[j].health;
                        gameState.Players[j].health -= 2;
                        Console.WriteLine(gameState.Players[j].playerId + " Health: " + gameState.Players[j].health);
                    }
                }
                Bullet bullet = gameState.Bullets[i];
                if (bullet.killProjectile())
                    bullet.timestamp = -1;
                if (tiles.isWall(bullet.xPos, bullet.yPos) && bullet.timestamp > 0)
                {
                    bullet.timestamp = -1;
                }
                if (bullet.timestamp > 0)
                    bullet.move();

            }
            
            for (var i = 0; i <gameState.Crates.Count; i++)
            {
                for (int j = 0; j < gameState.Players.Count; j++)
                {
                    float x1 = gameState.Players[j].xPos - gameState.Crates[i].xPos;
                    float y1 = gameState.Players[j].yPos - gameState.Crates[i].yPos;
                    float len1 = (float)Math.Sqrt(x1 * x1 + y1 * y1);
                    if (len1 <= gameState.Players[j].radius + 0.5 && gameState.Crates[i].timestamp>=0)
                    {
                        gameState.Crates[i].enemyID = gameState.Players[j].playerId;
                        gameState.Crates[i].timestamp = -1;
                    }
                }

            }
            if (ammoCounter >= 100)
            {
                var x = tiles.SpawnCrate();
                //spawn ammo
                if (gameState.Crates.Where(y => y.xPos == x[0] && y.yPos == x[1]).Count() <= 0)
                {
                    Crate c = new Crate(new Vector2(x[0], x[1]), crateCounter++);
                    c.timestamp = 0;
                    gameState.Crates.Add(c);
                    ammoCounter = 0;
                }

            }
            else
            {
                ammoCounter++;
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
