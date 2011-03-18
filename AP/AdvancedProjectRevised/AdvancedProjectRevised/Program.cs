using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.Net;

namespace AP
{
    class Program : GameWindow
    {
		#region Fields (23) 

        public static CollisionAI collisionAI;
        public static int spritenum;
        CreateLevel currentLevel;
        // Default position and velocity
        Vector3 defaultPosition = new Vector3(0, 0, 0);
        Vector3 defaultVelocity = new Vector3(0, 0, 0);
        private int enemyIdentifier = 0;
        private bool enemySpawned = false;
        public GameState gameState;
        List<int> heightSquares = new List<int>();
        public static LoadedObjects loadedObjects = new LoadedObjects();
        public Player player;
        // Screen dimensions
        const int screenX = 800;
        const int screenY = 800;
        //List<Bullet> bulletList = new List<Bullet>();
        //List<Enemy> enemyList = new List<Enemy>();
        //List<Player> playerList = new List<Player>();
        List<EnemySpawn> spawns = new List<EnemySpawn>();
        //camera related things
        Vector3d up = new Vector3d(0.0, 1.0, 0.0);
        Vector3d viewDirection = new Vector3d(0.0, 0.0, 1.0);
        private double viewDist = 23.0;
        List<int> widthSquares = new List<int>();
        List<int> xPosSpawn = new List<int>();
        List<int> xPosSquares = new List<int>();
        List<int> yPosSpawn = new List<int>();
        List<int> yPosSquares = new List<int>();
        private int zombieCount = 0;
        private int zombieIterator = 0;
        NetManager net;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>Creates a window with the specified title.</summary>
        public Program()
            : base(screenX, screenY, OpenTK.Graphics.GraphicsMode.Default, "ROFLPEWPEW")
        {
            VSync = VSyncMode.On;
        }

		#endregion Constructors 

		#region Methods (7) 

		// Public Methods (1) 

        /// <summary>
        /// Dirties the net hack.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public NetManager StartNetwork(ref GameState s)
        {
            //create client and/or server
            Console.WriteLine("[s]erver or [c]lient");
            string val = Console.ReadLine();
            NetManager manager;

            if (val == "s")
            {
                player = new Player();
                player.assignPlayerID(0);
                
                manager = new HostManager(9999, ref s);
                manager.State.Players.Add(player);
                manager.State.myUID = player.playerId;
                manager.setRole("server");
                return manager;
            }
            else
            {
                Server serv = new Server("Serv",IPAddress.Parse("192.168.105.127"));
                manager = new ClientManager(9999, ref s,serv);
                manager.setRole("client");
                while (manager.myConnections.Count == 0){}
                manager.Connected = true;
                return manager;
            }
        }
		// Protected Methods (4) 

        /// <summary>
        /// Load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            // Create 
            gameState = new GameState();
            currentLevel = new CreateLevel(1);
            currentLevel.parseFile(ref xPosSquares, ref yPosSquares, ref heightSquares, ref widthSquares, ref xPosSpawn, ref yPosSpawn);
            collisionAI = new CollisionAI(ref xPosSquares, ref yPosSquares, ref widthSquares, ref heightSquares);
            if ( xPosSpawn.Count > 0 )
            {
                spawns.Add(new EnemySpawn(xPosSpawn[0], yPosSpawn[0]));
            }
            if ( xPosSpawn.Count > 1 )
            {
                spawns.Add(new EnemySpawn(xPosSpawn[1], yPosSpawn[1]));
            }
            if ( xPosSpawn.Count > 2 )
            {
                spawns.Add(new EnemySpawn(xPosSpawn[2], yPosSpawn[2]));
            }
            if ( xPosSpawn.Count > 3 )
            {
                spawns.Add(new EnemySpawn(xPosSpawn[3], yPosSpawn[3]));
            }

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.IndexArray);
            
            //loading a cube... so easy
            loadedObjects.LoadObject("Objects//UnitCube.obj", "Objects//cube.png", 1.0f);            
            loadedObjects.LoadObject("Objects//groundTile.obj", "Objects//grass2.png", 5);

            Zombie.drawNumber = loadedObjects.LoadObject("Objects//zombie.obj", "Objects//Zomble.png", 0.08f);


            net = StartNetwork(ref gameState);
            while (!net.Connected) { }
            Console.WriteLine("Connected!");
            //player = new Player();
            //gameState.Players.Add(player);
            spritenum = loadedObjects.LoadObject("Objects//Player.obj", "Objects//Player.png", 0.08f);
            if (net.getRole() == "server")
            {
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (gameState.Players.Count > 0)
            {
                //Console.WriteLine(gameState.Players.Count);
                base.OnRenderFrame(e);
                int i = 0;
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                Matrix4d camera = Matrix4d.LookAt(OpenTK.Vector3d.Multiply(viewDirection, viewDist),
                                                  OpenTK.Vector3d.Zero, up);
                GL.LoadMatrix(ref camera);

                //player.draw();

                //GL.Translate(-gameState.Players.Where(y=>y.playerId==gameState.myUID).First().xPos, -gameState.Players.Where(y=>y.playerId==gameState.myUID).First().yPos, 0);
                /*lock (gameState)
                {

                    foreach (Player p in gameState.Players)
                    {
                        if (p.UID == net.myConnections[0].playerUID)
                        {
                            GL.LoadMatrix(ref camera);
                            p.draw();
                        }
                        else
                        {
                            GL.Translate(-player.xPos, -player.yPos, 0);
                            p.draw();
                        }
                    }
                }*/
                lock (gameState)
                {
                    foreach (Bullet bullet in gameState.Bullets)
                    {
                        bullet.draw();
                    }
                }

                //foreach (var x in gameState.Players)
                //{
                //    x.draw();
                //}
                for (var g = 0; g < gameState.Players.Count; g++)
                {
                    gameState.Players[g].draw();
                }
                GL.Color3(1.0f, 1.0f, 1.0f);//resets the colors so the textures don't end up red
                lock (gameState)
                {
                    foreach (var member in gameState.Enemies)
                    {
                        //member.Update(ref playerList,ref enemyList, ref xPosSquares,ref yPosSquares);
                        member.draw();
                    }
                }

                zombieIterator++;
                if (zombieCount < 20)
                {
                    foreach (var spawn in spawns)
                    {
                        spawn.draw();
                        if (zombieIterator == 60)
                        {
                            lock (gameState)
                            {
                                //need to ping server for a UID
                                //gameState.Enemies.Add(spawn.spawnEnemy(0));
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

                GL.Color3(1.0f, 1.0f, 1.0f);//resets the colors so the textures don't end up red
                //change this to be the same way as you do the walls
                for (int x = 0; x < 5; x++)
                    for (int y = 0; y < 5; y++)
                    {
                        GL.PushMatrix();
                        GL.Translate(-10 + x * 5.75, -10 + y * 5.75, 0);
                        loadedObjects.DrawObject(1); //grassssssssssssss
                        GL.PopMatrix();
                    }

                i = 0;
                foreach (var x in xPosSquares)
                {
                    if (widthSquares[i] > 1)
                    {
                        for (int idx = 0; idx < widthSquares[i]; idx++)
                        {
                            GL.LoadMatrix(ref camera);
                            //GL.Translate(-player.xPos, -player.yPos, 0);
                            GL.Translate(x + idx - 0.5f, yPosSquares[i] - 0.5f, 0.5f);
                            loadedObjects.DrawObject(0);
                        }
                    }

                    if (heightSquares[i] > 1)
                    {
                        for (int idx = 0; idx < heightSquares[i]; idx++)
                        {
                            GL.LoadMatrix(ref camera);
                            //GL.Translate(-player.xPos, -player.yPos, 0);
                            GL.Translate(x - 0.5f, yPosSquares[i] + idx - 0.5f, 0.5f);
                            loadedObjects.DrawObject(0);
                        }
                    }

                    i++;
                }

                if (net.getRole() == "server")
                {
                    //this updates our game state to the most current version
                    net.SyncState();
                }

                SwapBuffers();
            }
        }

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e)
        {
            //base.OnResize(e);
            GL.Viewport(0, 0, screenX, screenY);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void  OnUpdateFrame(FrameEventArgs e)
        {

            if (Keyboard[Key.W] && Keyboard[Key.D])
            {
                //player.move(1, 1);
             
                net.SendObjs<int>(Action.Request, new List<int>() { 1, 1 }, Type.Move);
            }
            else if (Keyboard[Key.W] && Keyboard[Key.A])
            {
                net.SendObjs<int>(Action.Request, new List<int>() { -1, 1 }, Type.Move);
            }
            else if (Keyboard[Key.S] && Keyboard[Key.D])
            {
                net.SendObjs<int>(Action.Request, new List<int>() { 1, -1 }, Type.Move);
            }
            else if (Keyboard[Key.S] && Keyboard[Key.A])
                net.SendObjs<int>(Action.Request, new List<int>() { -1, -1 }, Type.Move);
            else if (Keyboard[Key.W])
                net.SendObjs<int>(Action.Request, new List<int>() { 0, 1 }, Type.Move);
            else if (Keyboard[Key.S])
            {
                var temp=new List<int>() { 0, -1 };
                net.SendObjs<int>(Action.Request, temp, Type.Move);
            }
            else if (Keyboard[Key.A])
                net.SendObjs<int>(Action.Request, new List<int>() { -1, 0 }, Type.Move);
            else if (Keyboard[Key.D])
                net.SendObjs<int>(Action.Request, new List<int>() { 1, 0 }, Type.Move);
            else if (Keyboard[Key.Escape])
                Exit();

                if (Keyboard[OpenTK.Input.Key.Up])
                {
                    viewDist *= 1.1f;
                    Console.WriteLine("View distance: {0}", viewDist);
                }
                else if (Keyboard[OpenTK.Input.Key.Down])
                {
                    viewDist *= 0.9f;
                    Console.WriteLine("View distance: {0}", viewDist);
                }

                if (Keyboard[Key.Number1])
                {
                    player.weapons.equipPistol();
                }
                if (Keyboard[Key.Number2])
                {
                    player.weapons.equipRifle();
                }
                if (Keyboard[Key.Number3])
                {
                    player.weapons.equipShotgun();
                }
                if (Keyboard[Key.Number4])
                {
                    player.weapons.equipRocket();
                }

                float wheelD = Mouse.WheelDelta;
                viewDirection.Y += wheelD / 10;
                viewDirection.Z += wheelD / 10;

                collisionAI.updateState(ref gameState.Enemies);
                foreach (var member in gameState.Enemies)
                {
                    member.moveTowards(player);
                }

                if (Mouse[OpenTK.Input.MouseButton.Left] == true)
                {
                    //if (player.weapons.canShoot())
                    //{
                        //player.weapons.shoot(ref gameState.Bullets, ref player, screenX, screenY, Mouse.X, Mouse.Y);
                    //}
                }
                // player.weapons.updateBulletCooldown();

                List<Bullet> tmpBullet = new List<Bullet>();
                foreach (Bullet bullet in gameState.Bullets)
                {
                    bullet.move();
                    if (bullet.killProjectile())
                        tmpBullet.Add(bullet);
                    float moveX;
                    float moveY;
                    Enemy enemyHit;
                    bool hit = collisionAI.checkForCollision(bullet, out moveX, out moveY, out enemyHit);
                    if (hit)
                    {

                        if (enemyHit.decreaseHealth())
                            gameState.Enemies.Remove(enemyHit);
                        GC.Collect();
                        tmpBullet.Add(bullet);
                    }
                }

                foreach (Bullet bullet in tmpBullet)
                {
                    gameState.Bullets.Remove(bullet);
                }
                gameState = net.State;

                GC.Collect();
            
        }
		// Private Methods (2) 

        /// <summary>
        /// Assign passed enemy object a unique enemyIdentifier
        /// </summary>
        /// <param name="enemy">Passed enemy object.</param>
        /// <output>None.</output>
        private void assignEnemyID( Enemy enemy )
        {
            enemy.enemyID = enemyIdentifier++;
        }

        static void Main(string[] args)
        {
            //start the form for log in screen
            // if client
            // - create player object and send to server
            // if server
            // - get client info
            //Form1 form = new Form1();

            using( Program game = new Program() )
            {
                game.Run(28.0);
            }
        }

		#endregion Methods 
    }
}
