﻿using System;
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
        private Random rand = new Random();

        public static LoadedObjects loadedObjects = new LoadedObjects();
        public static int loadedObjectWall;
        public static int loadedObjectGrass;
        public static int loadedObjectPlayer;
        public static int loadedObjectZombie;
        public static int loadedBloodTexture;
        public static int loadedObjectBullet;
        public static int loadedObjectCrate;
        public static int loadedCrackedGroundTexture;
        public static int loadedBossMouth;
        public static int loadedBossEye;
        public EffectsHandler effectsHandler = new EffectsHandler();

        public static SoundHandler soundHandler;
        ImageHandler imageHandler;
        TextHandler textHandler;
        private int imageLifeBarBG;
        private int imageLifeBar;
        private int imageGameOver;
        private int imageYouWin;
        private int imageBenson;
        private int imageSoundOn;
        private int imageSoundOff;
        private int imageLevelOne;
        private int imageLevelTwo;

        private int imagePistolSelected;
        private int imagePistolAvailable;
        private int imageRifleSelected;
        private int imageRifleAvailable;
        private int imageRifleUnavailable;
        private int imageShotgunSelected;
        private int imageShotgunAvailable;
        private int imageShotgunUnavailable;

        CreateLevel level;

        // Screen dimensions
        private const int screenX = 700;
        private const int screenY = 700;

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

        public int bossSpawnCooldown = 2;
        public int bossSpawnWaveCooldown = 50;
        public int bossWaveCount = 1;
        public static bool bossKilled = false;

        public int timeToShowLevel = 48;

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
            gameState = new GameState();
            level = new CreateLevel(currentLevel);
            level.parseFile(ref xPosSquares, ref yPosSquares, ref heightSquares, ref widthSquares, ref xPosSpawn, ref yPosSpawn);

            soundHandler = new SoundHandler();
            textHandler = new TextHandler("../../Images/mybitmapfont.png");
            imageHandler = new ImageHandler();
            effectsHandler = new EffectsHandler();

            soundHandler.playSong(SoundHandler.BACKGROUND);
            soundHandler.play(SoundHandler.OMGHERETHECOME);

            imageLifeBarBG = imageHandler.loadImage("../../Images/LifeBarBg.png");
            imageLifeBar = imageHandler.loadImage("../../Images/LifeBar.png");

            imagePistolSelected = imageHandler.loadImage("Objects//Guns//Pistol_selected.png");
            imagePistolAvailable = imageHandler.loadImage("Objects//Guns//Pistol_available.png");
            imageRifleSelected = imageHandler.loadImage("Objects//Guns//Rifle_selected.png");
            imageRifleAvailable = imageHandler.loadImage("Objects//Guns//Rifle_available.png");
            imageRifleUnavailable = imageHandler.loadImage("Objects//Guns//Rifle_unavailable.png");
            imageShotgunSelected = imageHandler.loadImage("Objects//Guns//Shotgun_selected.png");
            imageShotgunAvailable = imageHandler.loadImage("Objects//Guns//Shotgun_available.png");
            imageShotgunUnavailable = imageHandler.loadImage("Objects//Guns//Shotgun_unavailable.png");

            imageLevelOne= imageHandler.loadImage("Objects//level1.png"); 
            imageLevelTwo = imageHandler.loadImage("Objects//level2.png");
            imageGameOver = imageHandler.loadImage("Objects//GameOver.png");
            imageYouWin = imageHandler.loadImage("Objects//YouWin.png");
            imageBenson = imageHandler.loadImage("Objects//BensonHead.png");
            imageSoundOn = imageHandler.loadImage("Objects//soundON.png");
            imageSoundOff = imageHandler.loadImage("Objects//soundOFF.png");

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.IndexArray);

            //Load Mesh Data into a buffer to be referenced in the future.
            loadedObjectWall = loadedObjects.LoadObject("Objects//UnitCube.obj", "Objects//stoneWall.png", 1.0f);
            loadedObjectGrass = loadedObjects.LoadObject("Objects//groundTile.obj", "Objects//dry_grass.png", 5);
            loadedObjectBullet = loadedObjects.LoadObject("Objects//bullet.obj", "Objects//bullet.png", 0.04f);
            loadedObjectCrate = loadedObjects.LoadObject("Objects//guns//crate.obj", "Objects//guns//rifleCrate.png", 0.5f);
                loadedObjects.LoadObject("Objects//guns//crate.obj", "Objects//guns//shotgunCrate.png", 0.5f);


            loadedObjectPlayer = loadedObjects.LoadObject("Objects//PlayerBody.obj", "Objects//Player.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerLeftLeg.obj", "Objects//Player.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerRightLeg.obj", "Objects//Player.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerLeftArm.obj", "Objects//Player.png", 0.08f);
            loadedObjects.LoadObject("Objects//PlayerRightArm.obj", "Objects//Player.png", 0.08f);

            //normal zombie
            loadedObjectZombie = loadedObjects.LoadObject("Objects//PlayerBody.obj", "Objects//zombie.png", Zombie.normalScale);
            loadedObjects.LoadObject("Objects//PlayerLeftLeg.obj", "Objects//zombie.png", Zombie.normalScale);
            loadedObjects.LoadObject("Objects//PlayerRightLeg.obj", "Objects//zombie.png", Zombie.normalScale);
            loadedObjects.LoadObject("Objects//PlayerLeftArm.obj", "Objects//zombie.png", Zombie.normalScale);
            loadedObjects.LoadObject("Objects//PlayerRightArm.obj", "Objects//zombie.png", Zombie.normalScale);

            //tank
            loadedObjects.LoadObject("Objects//PlayerBody.obj", "Objects//TankZombie.png", Zombie.tankScale);
            loadedObjects.LoadObject("Objects//PlayerLeftLeg.obj", "Objects//TankZombie.png", Zombie.tankScale);
            loadedObjects.LoadObject("Objects//PlayerRightLeg.obj", "Objects//TankZombie.png", Zombie.tankScale);
            loadedObjects.LoadObject("Objects//PlayerLeftArm.obj", "Objects//TankZombie.png", Zombie.tankScale);
            loadedObjects.LoadObject("Objects//PlayerRightArm.obj", "Objects//TankZombie.png", Zombie.tankScale);

            //fast
            loadedObjects.LoadObject("Objects//PlayerBody.obj", "Objects//FastZombie.png", Zombie.fastScale);
            loadedObjects.LoadObject("Objects//PlayerLeftLeg.obj", "Objects//FastZombie.png", Zombie.fastScale);
            loadedObjects.LoadObject("Objects//PlayerRightLeg.obj", "Objects//FastZombie.png", Zombie.fastScale);
            loadedObjects.LoadObject("Objects//PlayerLeftArm.obj", "Objects//FastZombie.png", Zombie.fastScale);
            loadedObjects.LoadObject("Objects//PlayerRightArm.obj", "Objects//FastZombie.png", Zombie.fastScale);

            //boss
            loadedObjects.LoadObject("Objects//PlayerBody.obj", "Objects//BOSS.png", Zombie.bossScale);
            loadedObjects.LoadObject("Objects//PlayerLeftLeg.obj", "Objects//BOSS.png", Zombie.bossScale);
            loadedObjects.LoadObject("Objects//PlayerRightLeg.obj", "Objects//BOSS.png", Zombie.bossScale);
            loadedObjects.LoadObject("Objects//BossLeftArm.obj", "Objects//BOSS.png", Zombie.bossScale);
            loadedObjects.LoadObject("Objects//BossRightArm.obj", "Objects//BOSS.png", Zombie.bossScale);


            loadedBloodTexture = loadedObjects.LoadObject("Objects//square.obj", "Objects//BloodSplatters//Blood1.png", 0.4f);
            loadedObjects.LoadObject("Objects//square.obj", "Objects//BloodSplatters//Blood2.png", 0.4f);
            loadedObjects.LoadObject("Objects//square.obj", "Objects//BloodSplatters//Blood3.png", 0.4f);
            loadedObjects.LoadObject("Objects//square.obj", "Objects//BloodSplatters//Blood4.png", 0.4f);

            loadedCrackedGroundTexture = loadedObjects.LoadObject("Objects//square.obj", "Objects//CrackedGround.png", 3.0f);

            loadedBossMouth = loadedObjects.LoadObject("Objects//square.obj", "Objects//mouth1.png", 1.0f);
            loadedObjects.LoadObject("Objects//square.obj", "Objects//mouth2.png", 1.0f);
            loadedObjects.LoadObject("Objects//square.obj", "Objects//mouth3.png", 1.0f);
            loadedObjects.LoadObject("Objects//square.obj", "Objects//mouth4.png", 1.0f);
            loadedObjects.LoadObject("Objects//square.obj", "Objects//mouth5.png", 1.0f);

            loadedBossEye = loadedObjects.LoadObject("Objects//square.obj", "Objects//eye.png", 0.3f);

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

        private void loadNewLevel()
        {
            gameState = new GameState();
            level = new CreateLevel(currentLevel);
            effectsHandler = new EffectsHandler();
            xPosSquares.Clear();
            yPosSquares.Clear();
            heightSquares.Clear();
            widthSquares.Clear();
            xPosSpawn.Clear();
            yPosSpawn.Clear();
            level.parseFile(ref xPosSquares, ref yPosSquares, ref heightSquares, ref widthSquares, ref xPosSpawn, ref yPosSpawn);
            gameState.Players.Add(player);
            player.xPos = 0;
            player.yPos = 0;
            setUpLevel();
            zombieCount = 0;

            if (currentLevel == 2)
            {
                collisionAI.posXBound = 9;
                collisionAI.posYBound = 1;
                collisionAI.negXBound = -9;
                collisionAI.negYBound = -14;
                loadedObjectGrass = loadedObjects.LoadObject("Objects//groundTile.obj", "Objects//grass.png", 5);
                loadedObjectWall = loadedObjects.LoadObject("Objects//UnitCube.obj", "Objects//cube.png", 1.0f);
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

            /*System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap("Objects//Guns//Rifle_available.png");
            System.Windows.Forms.Cursor myCursor = new System.Windows.Forms.Cursor(bitmap.GetHicon());
            System.Windows.Forms.Cursor.Current = myCursor; */ 

            GL.Color3(1.0f, 1.0f, 1.0f); //reset colors
           if (gameState.Players.Count > 0)
           {
                 base.OnRenderFrame(e);
                 int i = 0;
                 GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                 GL.MatrixMode(MatrixMode.Modelview);
                 GL.LoadIdentity();

                 Matrix4d camera = Matrix4d.LookAt(OpenTK.Vector3d.Multiply(viewDirection, viewDist), OpenTK.Vector3d.Zero, up);
                 GL.LoadMatrix(ref camera);

                 effectsHandler.shakeTheScreen();                 

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

                     foreach (Crate crate in gameState.Crates)
                     {
                         GL.PushMatrix();
                         crate.draw();
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
                         GL.PushMatrix();
                         if (member.type == Zombie.BOSS)
                             effectsHandler.translateForBoss();
                         if (player.health > 0)
                             member.draw();
                         else
                             member.drawVictory();
                         GL.PopMatrix();
                     }
                 }

                 

                 GL.Color3(1.0f, 1.0f, 1.0f);//resets the colors so the textures don't end up red
                 //change this to be the same way as you do the walls
                 for (int x = 0; x < 10; x++)
                     for (int y = 0; y < 10; y++)
                     {
                         GL.PushMatrix();
                         GL.Translate(-20 + x * 5.75, -20 + y * 5.75, 0);
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
                    if (p.playerId == gameState.myUID)
                    {
                        //life bar
                        TexUtil.InitTexturing();
                        GL.Color3(1.0f, 0.0f, 0.0f);
                        imageHandler.drawImage(imageLifeBar, 0.9f, 97.2f, 0.68f, 1.89f * p.health * 0.01f);
                        GL.Color3(1.0f, 1.0f, 1.0f);
                        imageHandler.drawImage(imageLifeBarBG, 0, 96, 0.68f, 1.0f);

                        //text stuff
                        GL.Color3(1.0f, 0.0f, 0.0f);
                        textHandler.writeText("Player " + (p.playerId + 1), 2, 55.0f, 98.1f, 0);
                        textHandler.writeText("Score: " + p.score, 2, 87.0f, 98.1f, 0);
                        
                        GL.Color3(1.0f, 1.0f, 1.0f);
                        //sound off/on images
                        if( !soundHandler.soundsOn )
                            imageHandler.drawImage(imageSoundOff, 92.0f, 92.0f, 0.6f, 1.0f);
                        else
                            imageHandler.drawImage(imageSoundOn, 92.0f, 92.0f, 0.6f, 1.0f);
                        
                        //gun images
                        
                        if (p.weapons.pistolEquipped)
                            imageHandler.drawImage(imagePistolSelected, 0.7f, 89.0f, 1.0f, 1.0f);
                        else
                            imageHandler.drawImage(imagePistolAvailable, 0.7f, 89.0f, 1.0f, 1.0f);
                        if (p.weapons.rifleEquipped)
                            imageHandler.drawImage(imageRifleSelected, 8.0f, 89.0f, 1.0f, 1.0f);
                        else if(p.weapons.rifleAmmo <= 0)
                            imageHandler.drawImage(imageRifleUnavailable, 8.0f, 89.0f, 1.0f, 1.0f);
                        else
                            imageHandler.drawImage(imageRifleAvailable, 8.0f, 89.0f, 1.0f, 1.0f);
                        textHandler.writeText(p.weapons.rifleAmmo.ToString(), 2, 13.0f, 85.0f, 0);
                        if (p.weapons.shotgunEquipped)
                            imageHandler.drawImage(imageShotgunSelected, 21.5f, 89.0f, 1.0f, 1.0f);
                        else if (p.weapons.shotgunAmmo <= 0)
                            imageHandler.drawImage(imageShotgunUnavailable, 21.5f, 89.0f, 1.0f, 1.0f);
                        else
                            imageHandler.drawImage(imageShotgunAvailable, 21.5f, 89.0f, 1.0f, 1.0f);
                        textHandler.writeText(p.weapons.shotgunAmmo.ToString(), 2, 28.0f, 85.0f, 0);
                    }
                }

                if (bossKilled)
                {
                    imageHandler.drawImage(imageYouWin, 12.0f, 60.0f, 3.0f, 1.0f);
                }
                else if (player.health <= 0)
                {
                    imageHandler.drawImage(imageGameOver, 12.0f, 60.0f, 3.0f, 1.0f);
                    imageHandler.drawImageRotate(imageBenson, 50.0f, 30.0f, 2.0f, 1.0f, imageHandler.bensonRotate);
                    imageHandler.bensonRotate += 5;
                }
                else if (timeToShowLevel > 0)
                {
                    if (currentLevel == 1)
                        imageHandler.drawImage(imageLevelOne, 38.0f, 60.0f, 2.0f, 1.0f);
                    else if (currentLevel == 2)
                        imageHandler.drawImage(imageLevelTwo, 38.0f, 60.0f, 2.0f, 1.0f);
                }

               //ADAM
               //move this into the for loop as an else     
               /*
                int horizontalInc = 0;
                GL.Color3(0.0f, 0.0f, 1.0f);
                imageHandler.drawImage(imageLifeBar, 0.7f + horizontalInc, 0.84f, 0.5f, 1.89f * 100 * 0.01f);
                GL.Color3(1.0f, 1.0f, 1.0f);
                imageHandler.drawImage(imageLifeBarBG, 0 + horizontalInc, 0, 0.5f, 1.0f);
                GL.Color3(0.0f, 0.0f, 1.0f);
                textHandler.writeText("Player " + 2, 2, 12.0f + horizontalInc, 6.0f, 0);
                textHandler.writeText("Score: " + 100, 2, 12.0f + horizontalInc, 4.0f, 0);

                horizontalInc += 37;
                GL.Color3(0.0f, 1.0f, 0.0f);
                imageHandler.drawImage(imageLifeBar, 0.7f + horizontalInc, 0.84f, 0.5f, 1.89f * 100 * 0.01f);
                GL.Color3(1.0f, 1.0f, 1.0f);
                imageHandler.drawImage(imageLifeBarBG, 0 + horizontalInc, 0, 0.5f, 1.0f);
                GL.Color3(0.0f, 1.0f, 0.0f);
                textHandler.writeText("Player " + 3, 2, 12.0f + horizontalInc, 6.0f, 0);
                textHandler.writeText("Score: " + 100, 2, 12.0f + horizontalInc, 4.0f, 0);

                horizontalInc += 37;
                GL.Color3(0.9f, 0.9f, 0.2f);
                imageHandler.drawImage(imageLifeBar, 0.7f + horizontalInc, 0.84f, 0.5f, 1.89f * 100 * 0.01f);
                GL.Color3(1.0f, 1.0f, 1.0f);
                imageHandler.drawImage(imageLifeBarBG, 0 + horizontalInc, 0, 0.5f, 1.0f);
                GL.Color3(0.9f, 0.9f, 0.2f);
                textHandler.writeText("Player " + 4, 2, 12.0f + horizontalInc, 6.0f, 0);
                textHandler.writeText("Score: " + 100, 2, 12.0f + horizontalInc, 4.0f, 0);*/

                if (effectsHandler.bossSpawned)
                {
                    Zombie boss = (Zombie)gameState.Enemies.Where(y => y.type == Zombie.BOSS).FirstOrDefault();
                    if (boss != null)
                    {
                        GL.Color3(0.7f, 0.0f, 0.7f);
                        textHandler.writeText("Big Scary Purple Zomble", 3, 50, 7, 0);
                        imageHandler.drawImage(imageLifeBar, 2.7f, 1.65f, 1.95f, 1.89f * (boss.health / (float)Enemy.Life.Boss) * 0.01f * 100, 0.5f);
                        GL.Color3(1.0f, 1.0f, 1.0f);
                        imageHandler.drawImage(imageLifeBarBG, 0.1f, 0, 1.95f, 1.0f, 0.5f);
                    }
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
            
            if (Keyboard[Key.P])
            {
            }
            else
            {
                if (multiplayer)
                {
                }
                else
                    player.walking = false;

                effectsHandler.updateEffects();
                effectsHandler.updateBossEyes(player.xPos);
                timeToShowLevel--;

                if (player.health > 0)
                {
                    if (Keyboard[Key.W] && Keyboard[Key.D])
                    {
                        if (multiplayer)
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

                    if (Mouse[OpenTK.Input.MouseButton.Left] == true)
                    {
                        if (multiplayer)
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
                                player.weapons.shoot(ref gameState.Bullets, new Vector3(player.xPos, player.yPos, 0), new Vector2(screenX, screenY), new Vector2(Mouse.X, Mouse.Y), ref player);
                            }
                        }

                    }
                }
                
                if (Keyboard[Key.Escape])
                    Exit();

                if (Keyboard[Key.F1] && !soundHandler.pressingF1)
                {
                    if (soundHandler.getSoundState())
                    {
                        soundHandler.setSoundState(false);
                        soundHandler.stopSong();
                    }
                    else
                    {
                        soundHandler.setSoundState(true);
                        //soundHandler.playSong(SoundHandler.BACKGROUND);
                        soundHandler.continueSong();
                    }
                    soundHandler.pressingF1 = true;
                }
                else
                    soundHandler.pressingF1 = false;

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

                

                float wheelD = Mouse.WheelDelta;
                viewDirection.Y += wheelD / 10;
                viewDirection.Z += wheelD / 10;

                if (Keyboard[Key.Left])
                    viewDirection.Y += 0.1f;
                if (Keyboard[Key.Right])
                    viewDirection.Y -= 0.1f;


                if (!multiplayer)
                {
                    collisionAI.updateState(ref gameState.Enemies);


                    foreach (var member in gameState.Enemies)
                    {
                        member.moveTowards(player);
                    }
                }

                //ADAM
                //move to serverrrrr
                //logic for picking up ammo crates
                if (!multiplayer)
                {
                    List<Crate> cratesToRemove = new List<Crate>();
                    foreach (Crate crate in gameState.Crates)
                    {
                        float diffX = player.xPos - crate.xPos;
                        float diffY = player.yPos - crate.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= player.radius + crate.radius)
                        {
                            if (crate.crateType == 0)
                                player.weapons.rifleAmmo += 30;
                            else if (crate.crateType == 1)
                                player.weapons.shotgunAmmo += 5;
                            cratesToRemove.Add(crate);
                            soundHandler.play(SoundHandler.RELOAD);
                        }
                    }
                    foreach (Crate crate in cratesToRemove)
                    {
                        gameState.Crates.Remove(crate);
                    }
                }

                
                if (!multiplayer)
                {
                    player.weapons.updateBulletCooldown();
                    if (player.weapons.rifleBurstCooldown != 0)
                    {
                        if (player.weapons.canShoot())
                        {
                            player.weapons.shoot(ref gameState.Bullets, new Vector3(player.xPos, player.yPos, 0), new Vector2(screenX, screenY), new Vector2(Mouse.X, Mouse.Y), ref player);
                        }
                    }
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
                    zombieIterator++;
                    if (zombieCount < 100)
                    {
                        foreach (var spawn in spawns)
                        {
                            if (zombieIterator >= 40)
                            {
                                lock (gameState)
                                {
                                    //need to ping server for a UID
                                    gameState.Enemies.Add(spawn.spawnEnemy(0));
                                    if (rand.Next(0, 10) == 0) //make a special zombie
                                        if (rand.Next(0, 2) == 0) //make a tank!
                                            gameState.Enemies.Last().changeSubtype(Zombie.TANK);
                                        else //make a fast zombles
                                            gameState.Enemies.Last().changeSubtype(Zombie.FAST);
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
                    else
                    {
                        if (gameState.Enemies.Count == 0 && effectsHandler.bossSpawned == false && currentLevel == 2)
                        { //SPAWN THE BOSS!!!
                            soundHandler.stopSong();
                            soundHandler.playSong(SoundHandler.BOSSBACKGROUND);
                            soundHandler.play(SoundHandler.SMASH);
                            effectsHandler.bossSpawned = true;
                            Enemy enemy = new Zombie(100000); //get this to have it's own ID?
                            enemy.setPosition(0, 3);

                            gameState.Enemies.Add(enemy);
                            gameState.Enemies.Last().changeSubtype(Zombie.BOSS);

                            viewDist = 13;
                            viewDirection.Y = -1.5;
                        }
                        else if (effectsHandler.bossSpawned == true && currentLevel == 2)
                        {
                            if ((Zombie)gameState.Enemies.Where(y => y.type == Zombie.BOSS).FirstOrDefault() == null)
                            {
                                if (!soundHandler.playingVictory)
                                {
                                    soundHandler.playingVictory = true;
                                    soundHandler.stopSong();
                                    soundHandler.playSong(SoundHandler.VICTORY);
                                }
                                bossKilled = true;
                                gameState.Enemies.Clear();
                            }
                            else
                            {
                                if (bossSpawnWaveCooldown <= 0)
                                {
                                    bossSpawnWaveCooldown++;
                                    bossSpawnCooldown--;
                                    if (bossSpawnWaveCooldown == -1)
                                    {
                                        bossSpawnWaveCooldown = 200;
                                        bossWaveCount = (bossWaveCount + 1) % 3 + 1;
                                    }
                                }
                                else
                                {
                                    bossSpawnWaveCooldown--;
                                    if (bossSpawnWaveCooldown == 1)
                                        bossSpawnWaveCooldown = -200;
                                }

                                if (bossSpawnCooldown <= 0)
                                {
                                    Enemy enemy = new Zombie(100001); //get this to have it's own ID?
                                    enemy.setPosition(0, 3);

                                    gameState.Enemies.Add(enemy);
                                    if (bossWaveCount == 2)
                                    {
                                        gameState.Enemies.Last().changeSubtype(Zombie.FAST);
                                        bossSpawnCooldown = 16;
                                    }
                                    else if (bossWaveCount == 3)
                                    {
                                        gameState.Enemies.Last().changeSubtype(Zombie.TANK);
                                        bossSpawnCooldown = 24;
                                    }
                                    else
                                        bossSpawnCooldown = 8;
                                }
                            }
                        }
                        else if (gameState.Enemies.Count == 0 && currentLevel == 1)
                        {
                            timeToShowLevel = 48;
                            currentLevel = 2;
                            soundHandler.stopSong();
                            soundHandler.playSong(SoundHandler.BACKGROUND2);
                            loadNewLevel();
                        }
                    }
                }


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
                            enemyHit.health--;
                            tmpBullet.Add(bullet);
                            bullet.timestamp = -1;
                            soundHandler.zombieScreamCooldown--;
                            if (soundHandler.zombieScreamCooldown <= 0)
                            {
                                soundHandler.zombieScreamCooldown = rand.Next(0, 8);

                                if (enemyHit.type == Zombie.NORMAL || enemyHit.type == Zombie.FAST)
                                {
                                    soundHandler.play(soundHandler.randomZombieSound());
                                }
                                else if (enemyHit.type == Zombie.TANK)
                                {
                                    soundHandler.play(soundHandler.randomTankSound());
                                }
                                else if (enemyHit.type == Zombie.BOSS)
                                {
                                    soundHandler.play(soundHandler.randomBossSound());
                                }
                            }
                            if (enemyHit.health <= 0)
                            {
                                //ADAM
                                effectsHandler.addBlood(moveX, moveY); //needs to be on the server too
                                gameState.Enemies.Remove(enemyHit);
                                GC.Collect();
                                if (rand.Next(0, 10) < 1) //new ammo crate
                                    gameState.Crates.Add(new Crate(new Vector2(enemyHit.xPos, enemyHit.yPos)));
                                player.score += 100; //ADAM need to complicate this on the server
                            }
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
                        if (player.health > 0)
                        {
                            player.health--;
                            if (ClientProgram.soundHandler.injuredSoundCooldown <= 0)
                            {
                                ClientProgram.soundHandler.play(SoundHandler.INJURED);
                                ClientProgram.soundHandler.injuredSoundCooldown = 14;
                            }
                        }
                        else
                        {
                            if (!ClientProgram.soundHandler.playedDeadSound)
                            {
                                ClientProgram.soundHandler.playedDeadSound = true;
                                ClientProgram.soundHandler.play(SoundHandler.DEAD);
                            }
                        }
                        effectsHandler.addBlood(player.xPos, player.yPos);
                    }
                    ClientProgram.soundHandler.injuredSoundCooldown--;
                }
            }
            GC.Collect();
        }

    }
}
