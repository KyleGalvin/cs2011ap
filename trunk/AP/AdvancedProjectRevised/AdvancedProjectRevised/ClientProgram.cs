using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using System.Net;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;


namespace AP
{
    class ClientProgram : GameWindow
    {
        public static LoadedObjects loadedObjects = new LoadedObjects();
        public static int loadedObjectWall;
        public static int loadedObjectGrass;
        public static int loadedObjectPlayer;
        public static int loadedObjectZombie;
        public static int loadedBloodTexture;
        public EffectsHandler effectsHandler = new EffectsHandler();

        public static SoundHandler soundHandler;
        ImageHandler imageHandler;
        TextHandler textHandler;
        private int imageLifeBarBG;
        private int imageLifeBar;

        CreateLevel level;

        // Screen dimensions
        private const int screenX = 800;
        private const int screenY = 800;

        //camera related things
        Vector3d up = new Vector3d(0.0, 1.0, 0.0);
        Vector3d viewDirection = new Vector3d(0.0, 0.0, 1.0);
        private double viewDist = 23.0;
        List<int> widthSquares = new List<int>();
        List<int> heightSquares = new List<int>();
        List<int> xPosSpawn = new List<int>();
        List<int> xPosSquares = new List<int>();
        List<int> yPosSpawn = new List<int>();
        List<int> yPosSquares = new List<int>();

        private int currentLevel = 1;
        public static CollisionAI collisionAI;
        private bool enemySpawned = false;

        List<EnemySpawn> spawns = new List<EnemySpawn>();
        private int zombieCount = 0;
        private int zombieIterator = 0;

        private NetManager net;
        private GameState gameState;
        public static bool multiplayer = false;
        private Player player;

        /// <summary>Creates a window with the specified title.</summary>
        public ClientProgram()
            : base(screenX, screenY, OpenTK.Graphics.GraphicsMode.Default, "ROFLPEWPEW")
        {
            Console.WriteLine("[s]ingle player or [m]ultiplayer");
            string val = Console.ReadLine();
            if (val == "m")
                multiplayer = true;


            VSync = VSyncMode.On;
        }

        /// <summary>
        /// Dirties the net hack.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public NetManager StartNetwork(ref GameState s)
        {
            //create client and/or server
            NetManager manager;

            Server serv = new Server("Serv", IPAddress.Parse("192.168.105.211"));
            manager = new ClientManager(9999, ref s, serv);
            manager.setRole("client");
            while (manager.myConnections.Count == 0) { }
            manager.Connected = true;
            return manager;
        }

        /// <summary>
        /// Load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            // Create 
            gameState = new GameState();
            level = new CreateLevel(1);
            level.parseFile(ref xPosSquares, ref yPosSquares, ref heightSquares, ref widthSquares, ref xPosSpawn, ref yPosSpawn);

            soundHandler = new SoundHandler();
            textHandler = new TextHandler("../../Images/mybitmapfont.png");
            imageHandler = new ImageHandler();
            effectsHandler = new EffectsHandler();

            imageLifeBarBG = imageHandler.loadImage("../../Images/LifeBarBg.png");
            imageLifeBar = imageHandler.loadImage("../../Images/LifeBar.png");

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.IndexArray);

            //Load Mesh Data into a buffer to be referenced in the future.
            loadedObjectWall = loadedObjects.LoadObject("Objects//UnitCube.obj", "Objects//cube.png", 1.0f);
            loadedObjectGrass = loadedObjects.LoadObject("Objects//groundTile.obj", "Objects//grass2.png", 5);
            //loadedObjectZombie = loadedObjects.LoadObject("Objects//zombie.obj", "Objects//Zomble.png", 0.08f);
            //loadedObjectPlayer = loadedObjects.LoadObject("Objects//Player.obj", "Objects//Player.png", 0.08f);

            loadedObjectPlayer = loadedObjects.LoadObject("Objects//PlayerBody.obj", "Objects//Player.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerLeftLeg.obj", "Objects//Player.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerRightLeg.obj", "Objects//Player.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerLeftArm.obj", "Objects//Player.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerRightArm.obj", "Objects//Player.png", 0.08f);

            loadedObjectZombie = loadedObjects.LoadObject("Objects//PlayerBody.obj", "Objects//zombie.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerLeftLeg.obj", "Objects//zombie.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerRightLeg.obj", "Objects//zombie.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerLeftArm.obj", "Objects//zombie.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerRightArm.obj", "Objects//zombie.png", 0.08f);

            loadedBloodTexture = loadedObjects.LoadObject("Objects//square.obj", "Objects//BloodSplatters//Blood1.png", 0.4f);
            loadedObjects.LoadObject("Objects//square.obj", "Objects//BloodSplatters//Blood2.png", 0.4f);
            loadedObjects.LoadObject("Objects//square.obj", "Objects//BloodSplatters//Blood3.png", 0.4f);
            loadedObjects.LoadObject("Objects//square.obj", "Objects//BloodSplatters//Blood4.png", 0.4f);

            if (multiplayer)
            {
                net = StartNetwork(ref gameState);
                while (!net.Connected) { }
                Console.WriteLine("Connected!");
                //add players to collisionAI here
            }
            else
            {
                player = new Player();
                player.assignPlayerID(0);
                gameState.Players.Add(player);
                setUpLevel();
                collisionAI.addToPlayerList(ref player);
            }
        }

        private void setUpLevel()
        {
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

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {

           if (gameState.Players.Count > 0)
           {
                 base.OnRenderFrame(e);
                 int i = 0;
                 GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                 GL.MatrixMode(MatrixMode.Modelview);
                 GL.LoadIdentity();

                 Matrix4d camera = Matrix4d.LookAt(OpenTK.Vector3d.Multiply(viewDirection, viewDist), OpenTK.Vector3d.Zero, up);
                 GL.LoadMatrix(ref camera);
                 if (multiplayer)
                 {
                     Player player = gameState.Players.Where(y => y.playerId == gameState.myUID).First();
                     lock (gameState)
                     {
                         player.draw();
                         GL.Translate(-player.xPos, -player.yPos, 0);

                         foreach (Player p in gameState.Players)
                         {

                             if (p.playerId != player.playerId)
                             {
                                 Console.WriteLine("x:" + p.xPos + " y:" + p.yPos);
                                 GL.PushMatrix();
                                 GL.Translate(p.xPos, p.yPos, 0);
                                 p.draw();
                                 GL.PopMatrix();
                             }
                         } 
                     }
                 }
                 else
                 {
                     player.draw();
                     GL.Translate(-player.xPos, -player.yPos, 0);
                 }

                 lock (gameState)
                 {
                     foreach (Bullet bullet in gameState.Bullets)
                     {
                         GL.PushMatrix();
                         bullet.draw();
                         GL.PopMatrix();
                     }
                 }
                 
                
                 //float tempX;
                 //float tempY;
                 //GL.Translate(-gameState.Players.Where(y=>y.playerId==gameState.myUID).First().xPos, -gameState.Players.Where(y=>y.playerId==gameState.myUID).First().yPos, 0);
                 //lock (gameState)
                 //{

                 //    foreach (Player p in gameState.Players)
                 //    {
                 //       if (p.UID == net.myConnections[0].playerUID)
                 ////        {
                 //              tempX = p.xPos;
                 //              tempY = p.yPos;
                 //            GL.LoadMatrix(ref camera);
                 //            p.draw();
                 //        }
                 //        else
                 //        {
                 //            GL.Translate(-player.xPos, -player.yPos, 0);
                 //            p.draw();
                 //        }
                 //    }
                 //}

                 GL.Color3(1.0f, 1.0f, 1.0f);//resets the colors so the textures don't end up red
                 lock (gameState)
                 {
                     foreach (var member in gameState.Enemies)
                     {
                         member.draw();
                     }
                 }

                 if (!multiplayer)
                 {
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
                                     gameState.Enemies.Add(spawn.spawnEnemy(0));
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

                 GL.Color3(1.0f, 1.0f, 1.0f);//resets the colors so the textures don't end up red
                 //change this to be the same way as you do the walls
                 for (int x = 0; x < 5; x++)
                     for (int y = 0; y < 5; y++)
                     {
                         GL.PushMatrix();
                         GL.Translate(-10 + x * 5.75, -10 + y * 5.75, 0);
                         loadedObjects.DrawObject(loadedObjectGrass); //grassssssssssssss
                         GL.PopMatrix();
                     }


                 i = 0;
                 foreach (var x in xPosSquares)
                 {
                     if (widthSquares[i] > 1)
                     {
                         for (int idx = 0; idx < widthSquares[i]; idx++)
                         {
                             GL.PushMatrix();
                             GL.Translate(x + idx - 0.5f, yPosSquares[i] - 0.5f, 0.5f);
                             loadedObjects.DrawObject(loadedObjectWall);
                             GL.PopMatrix();
                         }
                     }

                     if (heightSquares[i] > 1)
                     {
                         for (int idx = 0; idx < heightSquares[i]; idx++)
                         {
                             GL.PushMatrix();
                             GL.Translate(x - 0.5f, yPosSquares[i] + idx - 0.5f, 0.5f);
                             loadedObjects.DrawObject(loadedObjectWall);
                             GL.PopMatrix();
                         }
                     }

                     i++;
                 }

                 effectsHandler.drawEffects();

               // Lifebar
                //TexUtil.InitTexturing();
                //imageHandler.drawImage(imageLifeBar, 0.7f, 93.84f, 0.5f, 1.89f * player.health * 0.01f);
                //imageHandler.drawImage(imageLifeBarBG, 0, 93, 0.5f, 1.0f);

                //Text
                foreach (Player p in gameState.Players)
                {
                    TexUtil.InitTexturing();
                    imageHandler.drawImage(imageLifeBar, 0.7f, 93.84f, 0.5f, 1.89f * p.health * 0.01f);
                    imageHandler.drawImage(imageLifeBarBG, 0, 93, 0.5f, 1.0f);
                    textHandler.writeText("Player " + (p.playerId+1) + " Score: " + 10000, 2, 21.2f, 98.1f, 0);
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
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (multiplayer)
            {
            }
            else
                player.walking = false;

            effectsHandler.updateEffects();

             if (Keyboard[Key.W] && Keyboard[Key.D])
             {
                 if( multiplayer )
                     net.SendObjs<int>(Action.Request, new List<int>() { 1, 1 }, Type.Move);
                 else
                    player.move(1, 1);
             }
             else if (Keyboard[Key.W] && Keyboard[Key.A])
             {
                 if (multiplayer)
                     net.SendObjs<int>(Action.Request, new List<int>() { -1, 1 }, Type.Move);
                 else
                     player.move(-1, 1);
             }
             else if (Keyboard[Key.S] && Keyboard[Key.D])
             {
                 if (multiplayer)
                     net.SendObjs<int>(Action.Request, new List<int>() { 1, -1 }, Type.Move);
                 else
                     player.move(1, -1);
                 
             }
             else if (Keyboard[Key.S] && Keyboard[Key.A])
             {
                 if (multiplayer)
                     net.SendObjs<int>(Action.Request, new List<int>() { -1, -1 }, Type.Move);
                 else
                     player.move(-1, -1);
             }
             else if (Keyboard[Key.W])
             {
                 if (multiplayer)
                     net.SendObjs<int>(Action.Request, new List<int>() { 0, 1 }, Type.Move);
                 else
                     player.move(0, 1);
             }
             else if (Keyboard[Key.S])
             {
                 if (multiplayer)
                     net.SendObjs<int>(Action.Request, new List<int>() { 0, -1 }, Type.Move);
                 else
                     player.move(0, -1);
             }
             else if (Keyboard[Key.A])
             {
                 if (multiplayer)
                     net.SendObjs<int>(Action.Request, new List<int>() { -1, 0 }, Type.Move);
                 else
                     player.move(-1, 0);
             }
             else if (Keyboard[Key.D])
             {
                 if (multiplayer)
                     net.SendObjs<int>(Action.Request, new List<int>() { 1, 0 }, Type.Move);
                 else
                     player.move(1, 0);
             }
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

                 if (!multiplayer)
                 {
                     collisionAI.updateState(ref gameState.Enemies);
                 

                     foreach (var member in gameState.Enemies)
                     {
                         member.moveTowards(player);
                     }
                 }

                 if (Mouse[OpenTK.Input.MouseButton.Left] == true)
                 {
                     if(multiplayer)
                     {
                         //if (player.weapons.canShoot())
                         //{
                         //    soundHandler.play(SoundHandler.EXPLOSION);
                         //    player.weapons.shoot(ref gameState.Bullets, new Vector3(player.xPos, player.yPos, 0), new Vector2(800, 800), new Vector2(Mouse.X, Mouse.Y));
                         //}
                     }
                     else
                     {
                         if (player.weapons.canShoot())
                         {
                             soundHandler.play(SoundHandler.EXPLOSION);
                             player.weapons.shoot(ref gameState.Bullets, new Vector3(player.xPos, player.yPos, 0), new Vector2(800, 800), new Vector2(Mouse.X, Mouse.Y), ref player);
                         }
                     }
                     
                 }
                 if (!multiplayer)
                 {
                     player.weapons.updateBulletCooldown();
                 }
                /* List<Bullet> tmpBullet = new List<Bullet>();
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
                 }*/

                 
                 if (!multiplayer)
                 {
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
                             //ADAM
                             effectsHandler.addBlood(moveX, moveY); //needs to be on the server too
                             tmpBullet.Add(bullet);
                             if (enemyHit.decreaseHealth())
                                 gameState.Enemies.Remove(enemyHit);
                             GC.Collect();
                             bullet.timestamp = -1;
                             soundHandler.play(SoundHandler.ZOMBIE);
                         }
                     }
                 

                     foreach (Bullet bullet in tmpBullet)
                     {                         
                         gameState.Bullets.Remove(bullet);
                     }

                     //ADAM
                     /*
                      * MOVE THIS TO SERVER STUFFFFFFFFFFF
                      * 
                      */
                     float moveX2, moveY2;
                     if (collisionAI.checkForMovementCollision(player, out moveX2, out moveY2))
                     {
                         player.health--;
                         if (ClientProgram.soundHandler.injuredSoundCooldown <= 0)
                         {
                             ClientProgram.soundHandler.play(SoundHandler.INJURED);
                             ClientProgram.soundHandler.injuredSoundCooldown = 14;
                         }
                     }
                     ClientProgram.soundHandler.injuredSoundCooldown--;
                 }
            GC.Collect();
        }

    }
}
