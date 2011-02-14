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

namespace AP
{
    class Program : GameWindow
    {
        //camera related things
        Vector3d up = new Vector3d(0.0, 1.0, 0.0);
        Vector3d viewDirection = new Vector3d(0.0, 0.0, 1.0);
        Vector3 defaultPosition = new Vector3(0, 0, 0);
        Vector3 defaultVelocity = new Vector3(0, 0, 0);
        private double viewDist = 23.0;
        const int screenX = 800;
        const int screenY = 800;
        private int zombieIterator = 0;
        private bool enemySpawned = false;
        //assign amount of enemies per level
        // first row index = EnemyObject
        // second row index = EnemyID
        List<Bullet> bulletList = new List<Bullet>();
        List<Enemy> enemyList = new List<Enemy>();
        List<Player> playerList = new List<Player>();
        List<EnemySpawn> spawns = new List<EnemySpawn>();
        public Player player;
        //public Controls controls;
        private int enemyIdentifier = 0;
        //EnemySpawn spawn;
        List<int> xPosSquares = new List<int>();
        List<int> yPosSquares = new List<int>();
        List<int> widthSquares = new List<int>();
        List<int> heightSquares = new List<int>();
        List<int> xPosSpawn = new List<int>();
        List<int> yPosSpawn = new List<int>();
        CreateLevel currentLevel;
        Random randNum = new Random();
        public static LoadedObjects loadedObjects = new LoadedObjects();
        public static CollisionAI collisionAI = new CollisionAI();

        /// <summary>Creates a window with the specified title.</summary>
        public Program()
            : base(screenX, screenY, OpenTK.Graphics.GraphicsMode.Default, "ROFLPEWPEW")
        {
            VSync = VSyncMode.On;
        }
        
        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            player = new Player();
            currentLevel = new CreateLevel(1);
            currentLevel.parseFile(ref xPosSquares, ref yPosSquares, ref heightSquares, ref widthSquares, ref xPosSpawn, ref yPosSpawn);
            playerList.Add(player);
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
            loadedObjects.LoadObject("Objects//UnitCube.obj", "Objects//Bricks.png", 1);
            loadedObjects.LoadObject("Objects//groundTile.obj", "Objects//grass2.png", 5);
            Zombie.drawNumber = loadedObjects.LoadObject("Objects//zombie.obj", "Objects//Zomble.png", 0.15f);
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
            if (Keyboard[Key.W])
                player.move(0,1);
            if (Keyboard[Key.S])
                player.move(0,-1);
            if (Keyboard[Key.A])
                player.move(-1,0);
            if (Keyboard[Key.D])
                player.move(1,0);
            if (Keyboard[Key.Escape])
                Exit();

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

            collisionAI.updateState(ref enemyList);
            foreach (var member in enemyList)
            {
                member.moveTowards(player);
            }

            if (Mouse[OpenTK.Input.MouseButton.Left] == true)
            {
                if (player.weapons.canShoot())
                {
                    player.weapons.shoot(ref bulletList, ref player, screenX, screenY, Mouse.X, Mouse.Y);
                }
            }
            player.weapons.updateBulletCooldown();

            List<Bullet> tmpBullet = new List<Bullet>();
            foreach (Bullet bullet in bulletList)
            {
                bullet.move();
                if (bullet.killProjectile())
                    tmpBullet.Add(bullet);
                Enemy hit;
                collisionAI.checkForCollision<Enemy>(bullet, out hit);
                if (hit != null)
                {
                    enemyList.Remove(hit);
                    tmpBullet.Add(bullet);
                }
            }

            foreach (Bullet bullet in tmpBullet)
            {
                bulletList.Remove(bullet);
            }

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
        }

        /// <summary>
        /// Assign passed enemy object a unique enemyIdentifier
        /// </summary>
        /// <param name="enemy">Passed enemy object.</param>
        /// <output>None.</output>
        private void assignEnemyID( Enemy enemy )
        {
            enemy.enemyID = enemyIdentifier++;
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            int i = 0;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            Matrix4d camera = Matrix4d.LookAt(OpenTK.Vector3d.Multiply(viewDirection, viewDist),
                                              OpenTK.Vector3d.Zero, up);
            GL.LoadMatrix(ref camera);

            player.draw();
            GL.Translate(-player.xPos, -player.yPos, 0);

            foreach (Bullet bullet in bulletList)
            {
                bullet.draw();
            }


            GL.Color3(1.0f, 1.0f, 1.0f);//resets the colors so the textures don't end up red
            foreach (var member in enemyList)
            {
                //member.Update(ref playerList,ref enemyList, ref xPosSquares,ref yPosSquares);
                member.draw();
            }

            zombieIterator++;

            foreach (var spawn in spawns)
            {
                spawn.draw();
                if (zombieIterator == 60)
                {
                    //need to ping server for a UID
                    enemyList.Add(spawn.spawnEnemy(0));
                    enemySpawned = true;
                }
            }
            if (enemySpawned)
            {
                zombieIterator = 0;
                enemySpawned = false;
            }


            

            GL.Color3(1.0f, 1.0f, 1.0f);//resets the colors so the textures don't end up red
            //change this to be the same way as you do the walls
            for(int x = 0; x < 5; x++)
                for (int y = 0; y < 5; y++)
                {
                    GL.PushMatrix();
                    GL.Translate(-10 + x * 5, -10 + y * 5, 0);
                    loadedObjects.DrawObject(1); //grassssssssssssss
                    GL.PopMatrix();
                }

            i = 0;
            foreach (var x in xPosSquares)
            {
                if (widthSquares[i] > 1)
                {
                    for (int idx = 0; idx < widthSquares[i] * 2; idx += 2)
                    {
                        GL.LoadMatrix(ref camera);
                        GL.Translate(-player.xPos, -player.yPos, 0);
                        GL.Translate(x + idx, yPosSquares[i], 1);
                        loadedObjects.DrawObject(0);
                    }
                }

                if (heightSquares[i] > 1)
                {
                    for (int idx = 0; idx < heightSquares[i] * 2; idx += 2)
                    {
                        GL.LoadMatrix(ref camera);
                        GL.Translate(-player.xPos, -player.yPos, 0);
                        GL.Translate(x, yPosSquares[i] + idx, 1);
                        loadedObjects.DrawObject(0);
                    }
                }

                i++;
            }
            
            SwapBuffers();
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
    }
}
