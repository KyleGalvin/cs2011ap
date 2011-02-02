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
        const int screenX = 600;
        const int screenY = 600;

        //assign amount of enemies per level
        // first row index = EnemyObject
        // second row index = EnemyID
        List<Enemy>[,] enemyList;
        public Player player;
        public Controls controls;
        private int enemyIdentifier = 0;

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
            GL.ClearColor(0.0f, 0.2f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
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
                player.move(1,0);
            if (Keyboard[Key.D])
                player.move(-1,0);
            if (Keyboard[Key.Escape])
                Exit();

            //base.OnUpdateFrame(e);

            Console.WriteLine("Running");

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
            //base.OnRenderFrame(e);
            
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            player.draw();

            SwapBuffers();
        }

        static void Main(string[] args)
        {
            //start the form for log in screen
            // if client
            // - create player object and send to server
            // if server
            // - get client info
            using( Program game = new Program() )
            {
                game.Run(28.0);
            }
        }
    }
}
