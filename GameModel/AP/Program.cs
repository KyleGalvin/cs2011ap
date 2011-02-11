using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

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
        List<Enemy> enemyList = new List<Enemy>();
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
        EnemySpawn spawn1;
        EnemySpawn spawn2;
        EnemySpawn spawn3;
        EnemySpawn spawn4;
        CreateLevel currentLevel;
        Random randNum = new Random();
        Bullet bullet1;
        static LoadedObjects loadedObjects = new LoadedObjects();

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
            //base.OnLoad(e);
            player = new Player();//(defaultPosition, 0);
            currentLevel = new CreateLevel(1);
            currentLevel.parseFile(ref xPosSquares, ref yPosSquares, ref heightSquares, ref widthSquares, ref xPosSpawn, ref yPosSpawn);

            if ( xPosSpawn.Count > 0 )
            {
                spawn1 = new EnemySpawn(xPosSpawn[0], yPosSpawn[0]);
                spawns.Add(spawn1);
            }
            if ( xPosSpawn.Count > 1 )
            {
                Console.WriteLine(" 2 spawns ");
                spawn2 = new EnemySpawn(xPosSpawn[1], yPosSpawn[1]);
                spawns.Add(spawn2);
            }
            if ( xPosSpawn.Count > 2 )
            {
                Console.WriteLine(" 2 spawns ");
                spawn3 = new EnemySpawn(xPosSpawn[2], yPosSpawn[2]);
                spawns.Add(spawn3);
            }
            if ( xPosSpawn.Count > 3 )
            {
                spawn4 = new EnemySpawn(xPosSpawn[3], yPosSpawn[3]);
                spawns.Add(spawn4);
            }

            GL.ClearColor(0.0f, 0.2f, 0.2f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.IndexArray);
            
            //loading a cube... so easy
            loadedObjects.LoadObject("Objects//UnitCube.obj", "Objects//cube.png");
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
            base.OnUpdateFrame(e);
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

            if (Mouse[OpenTK.Input.MouseButton.Left] == true)
            {
                
                if (player.canShoot())
                {
                    Console.WriteLine("shoot");
                    bullet1 = new Bullet(player.position, defaultVelocity, ref bullet1);
                    bullet1.setDirectionByMouse(Mouse.X, Mouse.Y, screenX, screenY, ref player);
                }
            }
            player.updateBulletCooldown();

            if (bullet1 != null)
            {
                bullet1.move();
                if (bullet1.killProjectile())
                    bullet1 = null;
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

        private void assignPlayerID()
        {
            //player = server request for ID
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
            if (bullet1 != null)
            {
                bullet1.draw();
            }


            zombieIterator++;
            foreach (var spawn in spawns)
            {
                spawn.draw();
                if (zombieIterator == 60)
                {
                    enemyList.Add(spawn.spawnEnemy());
                    //Console.WriteLine("spawn");
                    enemySpawned = true;
                }
            }
            if (enemySpawned)
            {
                zombieIterator = 0;
                enemySpawned = false;
            }
            
            
            foreach (var member in enemyList)
            {
                member.move(1, 0);
                member.draw();
            }


            i = 0;
            foreach (var x in xPosSquares)
            {
                if (widthSquares[i] > 1)
                {
                    for (int idx = 0; idx < widthSquares[i] * 2; idx += 2)
                    {
                        GL.LoadMatrix(ref camera);
                        GL.Translate(x + idx, yPosSquares[i], 4);
                        loadedObjects.DrawObject(0);
                    }
                }

                if (heightSquares[i] > 1)
                {
                    for (int idx = 0; idx < heightSquares[i] * 2; idx += 2)
                    {
                        GL.LoadMatrix(ref camera);
                        GL.Translate(x, yPosSquares[i] + idx, 4);
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
