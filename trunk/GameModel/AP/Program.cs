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
        OpenTK.Vector3d up = new OpenTK.Vector3d(0.0, 1.0, 0.0);
        OpenTK.Vector3d viewDirection = new OpenTK.Vector3d(0.0, 0.0, 1.0);
        double viewDist = 17.0;
        const int screenX = 800;
        const int screenY = 800;
        private int zombieIterator = 0;
        //assign amount of enemies per level
        // first row index = EnemyObject
        // second row index = EnemyID
        List<Enemy> enemyList = new List<Enemy>();
        List<EnemySpawn> spawns = new List<EnemySpawn>();
        public Player player;
        //public Controls controls;
        private int enemyIdentifier = 0;
        EnemySpawn spawn1;
        EnemySpawn spawn2;
        EnemySpawn spawn3;
        EnemySpawn spawn4;
        OpenLevel currentLevel;
        Random randNum = new Random();
        LoadedObjects loadedObjects = new LoadedObjects();

        /// <summary>Creates a window with the specified title.</summary>
        public Program()
            : base(screenX, screenY, OpenTK.Graphics.GraphicsMode.Default, "Shoot - em fuckers up!")
        {
            VSync = VSyncMode.On;
        }

        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            //base.OnLoad(e);
            player = new Player(0, 0, 0);
            spawn1 = new EnemySpawn(0, (float)1.6);
            spawn2 = new EnemySpawn((float)1.6, 0);
            spawn3 = new EnemySpawn(0, (float)-1.6);
            spawn4 = new EnemySpawn((float)-1.6, 0);
            currentLevel = new OpenLevel(1);
            currentLevel.parseFile();
            spawns.Add(spawn1);
            spawns.Add(spawn2);
            spawns.Add(spawn3);
            spawns.Add(spawn4);
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
            //base.OnUpdateFrame(e);

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

            //base.OnUpdateFrame(e);

            //Console.WriteLine("Running");

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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Matrix4d camera = Matrix4d.LookAt(OpenTK.Vector3d.Multiply(viewDirection, viewDist),
                                              OpenTK.Vector3d.Zero, up);
            GL.LoadMatrix(ref camera);

            player.draw();
            zombieIterator++;
            if (zombieIterator == 60)
            {
                foreach (var spawn in spawns)
                {
                    enemyList.Add(spawn.spawnEnemy());
                }
                //Console.WriteLine("spawn");
                zombieIterator = 0;
            }


            foreach (var member in enemyList)
            {
                member.move(1, 0);
                member.draw();
            }

            //GL.Rotate(180, 1, 1, 1);
            loadedObjects.DrawObject(0);
            
            SwapBuffers();
        }

        static void Main(string[] args)
        {

            Console.WriteLine("[0] Run Game");
            Console.WriteLine("[1] Adam's Tests");
            Console.WriteLine("[2] Gage's Tests");
            Console.WriteLine("[3] Kyle's Tests");
            Console.WriteLine("[4] Mike's Tests");
            Console.WriteLine("[5] Scott's Tests");
            Console.WriteLine("[6] Todd's Tests");
            String read = Console.ReadLine();
            switch (read)
            {
                case "0":
                    //start the form for log in screen
                    // if client
                    // - create player object and send to server
                    // if server
                    // - get client info
                    //Form1 form = new Form1();
                    using (Program game = new Program())
                    {
                        game.Run(28.0);
                    }
                    break;
                case "1":
                    break;
                case "2":
                    break;
                case "3":
                    int DefaultPort = 9999;
                    
                    while (true)
                    {
                        Console.WriteLine("[c]reate Lobby, [j]oin Lobby, [q]uit");
                        string input = Console.ReadLine();

                        if (input[0] == 'c')
                        {
                            NetLib.LobbyManager myLobby = new NetLib.LobbyManager(DefaultPort);
                            break;
                        }
                        else if (input[0] == 'j')
                        {
                            NetLib.ClientManager myClient = new NetLib.ClientManager(DefaultPort);
                        }
                        else if (input[0] == 'q')
                        {
                            break;
                        }
                        else
                        {
                        }
                    }
                    break;
                case "4":
                    break;
                case "5":
                    break;
                case "6":
                    break;
                default:
                    break;
            }
        }
    }
}
