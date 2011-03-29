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
        private System.Timers.Timer gameTime = new System.Timers.Timer(50);
        public static CollisionAI collisionAI;
        public static int bulletID = 0;
        CreateLevel level;
        private int currentLevel = 1;

        private bool enemySpawned = false;

        List<int> heightSquares = new List<int>();
        List<int> widthSquares = new List<int>();
        List<int> xPosSpawn = new List<int>();
        List<int> xPosSquares = new List<int>();
        List<int> yPosSpawn = new List<int>();
        List<int> yPosSquares = new List<int>();
        List<EnemySpawn> spawns = new List<EnemySpawn>();
        private int zombieCount = 0;
        private int zombieIterator = 0;
        NetManager net;
        private  GameState gameState;
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

        private void setUpLevel()
        {
            level = new CreateLevel(currentLevel);
            level.parseFile(ref xPosSquares, ref yPosSquares, ref heightSquares, ref widthSquares, ref xPosSpawn, ref yPosSpawn);
            
            collisionAI = new CollisionAI(ref xPosSquares, ref yPosSquares, ref widthSquares, ref heightSquares);

            setSpawns();
        }

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
    }
}
