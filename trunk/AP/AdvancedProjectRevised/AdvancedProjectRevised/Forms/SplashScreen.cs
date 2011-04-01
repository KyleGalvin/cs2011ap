using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace AP
{
    public partial class SplashScreen : Form
    {
        bool loaded = false;
        //camera related things
        OpenTK.Vector3d up = new OpenTK.Vector3d(0.0, 0.0, -1.0);
        OpenTK.Vector3d viewDirection = new OpenTK.Vector3d(0.0, 1.0, -0.3);
        double viewDist = 30.0;
        float angle1 = 0.0f;
        float angle2 = 0.0f;

        float zombieWalk = 0.0f;

        bool incWalk = true;
        float legAngle = 0.0f;

        bool incWalk2 = true;
        float legAngle2 = 0.0f;

        LoadedObjects loadedObjects = new LoadedObjects();
        public SplashScreen()
        {
            InitializeComponent();
        }

        private void btn_Client_Click(object sender, EventArgs e)
        {
            btn_Multiplayer.Visible = true;
            btn_Singleplayer.Visible = true;
            btn_Client.Visible = false;
            btn_Server.Visible = false;
        }

        private void btn_Server_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            ServerProgram server = new ServerProgram();
            
        }

        private void btn_Singleplayer_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            using (ClientProgram client = new ClientProgram(false))
            {
                client.Run(28.0);
            }
            Application.Exit();
        }

        private void btn_Multiplayer_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            using (ClientProgram client = new ClientProgram(true))
            {
                client.Run(28.0);
            }
            Application.Exit();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            loaded = true;
            GL.ClearColor(0.5f, 0.5f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.IndexArray);

            loadedObjects.LoadObject("Objects//PlayerBody.obj", "Objects//texture256.png",1.0f);
            loadedObjects.LoadObject("Objects//PlayerLeftLeg.obj", "Objects//texture256.png",1.0f);
            loadedObjects.LoadObject("Objects//PlayerRightLeg.obj", "Objects//texture256.png",1.0f);
            loadedObjects.LoadObject("Objects//PlayerLeftArm.obj", "Objects//texture256.png",1.0f);
            loadedObjects.LoadObject("Objects//PlayerRightArm.obj", "Objects//texture256.png",1.0f);

            loadedObjects.LoadObject("Objects//PlayerBody.obj", "Objects//zombie.png", 1.0f);
            loadedObjects.LoadObject("Objects//PlayerLeftLeg.obj", "Objects//zombie.png", 1.0f);
            loadedObjects.LoadObject("Objects//PlayerRightLeg.obj", "Objects//zombie.png", 1.0f);
            loadedObjects.LoadObject("Objects//PlayerLeftArm.obj", "Objects//zombie.png", 1.0f);
            loadedObjects.LoadObject("Objects//PlayerRightArm.obj", "Objects//zombie.png", 1.0f);

            loadedObjects.LoadObject("Objects//2DSquare.obj", "Objects//Background.png", 1.0f);
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;
            GL.Viewport(0, 0, Width, Height);

            double aspect_ratio = Width / (double)Height;

            OpenTK.Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 0.1f, 64000f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded)
                return;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);

            GL.LoadIdentity();
            Matrix4d camera = Matrix4d.LookAt(OpenTK.Vector3d.Multiply(viewDirection, viewDist),
                                                OpenTK.Vector3d.Zero, up);

            GL.LoadMatrix(ref camera);
            GL.Rotate(angle1 + zombieWalk + 45, 0, 0, 1.0);
            GL.Rotate(angle2, 0, 1.0, 0);
            GL.Translate(10, 0, 0);
            loadedObjects.DrawObject(0); //body

            GL.LoadMatrix(ref camera);
            GL.Rotate(angle1 + zombieWalk + 45, 0, 0, 1.0);
            GL.Rotate(angle2, 0, 1.0, 0);
            GL.Rotate(legAngle, 1.0, 0, 0);
            GL.Translate(10, 0, 0);
            GL.Translate(0, legAngle / 20, 0);
            loadedObjects.DrawObject(2); //right leg

            GL.LoadMatrix(ref camera);
            GL.Rotate(angle1 + zombieWalk + 45, 0, 0, 1.0);
            GL.Rotate(angle2, 0, 1.0, 0);
            GL.Rotate(-legAngle, 1.0, 0, 0);
            GL.Translate(10, 0, 0);
            GL.Translate(1, 0, 0);
            GL.Translate(0, -legAngle / 20, 0);
            loadedObjects.DrawObject(1); //left leg

            GL.LoadMatrix(ref camera);
            GL.Rotate(angle1 + zombieWalk + 45, 0, 0, 1.0);
            GL.Rotate(angle2, 0, 1.0, 0);
            GL.Rotate(legAngle, 1.0, 0, 0);
            GL.Translate(10, 0, 0);
            loadedObjects.DrawObject(3); //left arm

            GL.LoadMatrix(ref camera);
            GL.Rotate(angle1 + zombieWalk + 45, 0, 0, 1.0);
            GL.Rotate(angle2, 0, 1.0, 0);
            GL.Rotate(-legAngle, 1.0, 0, 0);
            GL.Translate(10, 0, 0);
            loadedObjects.DrawObject(4); //right arm


            GL.LoadMatrix(ref camera);
            GL.Rotate(angle1 + zombieWalk, 0, 0, 1.0);
            GL.Rotate(angle2, 0, 1.0, 0);
            GL.Translate(10, 0, 0);
            loadedObjects.DrawObject(5); //body

            GL.LoadMatrix(ref camera);
            GL.Rotate(angle1 + zombieWalk, 0, 0, 1.0);
            GL.Rotate(angle2, 0, 1.0, 0);
            GL.Rotate(legAngle2, 1.0, 0, 0);
            GL.Translate(10, 0, 0);
            GL.Translate(0, legAngle2 / 20, 0);
            loadedObjects.DrawObject(7); //right leg

            GL.LoadMatrix(ref camera);
            GL.Rotate(angle1 + zombieWalk, 0, 0, 1.0);
            GL.Rotate(angle2, 0, 1.0, 0);
            GL.Rotate(-legAngle2, 1.0, 0, 0);
            GL.Translate(1, 0, 0);
            GL.Translate(10, 0, 0);
            GL.Translate(0, -legAngle2 / 20, 0);
            loadedObjects.DrawObject(6); //left leg

            GL.LoadMatrix(ref camera);
            GL.Rotate(angle1 + zombieWalk, 0, 0, 1.0);
            GL.Rotate(angle2, 0, 1.0, 0);
            GL.Rotate(legAngle2 - 90, 1.0, 0, 0);
            GL.Translate(10, 0, 0);
            loadedObjects.DrawObject(8); //left arm

            GL.LoadMatrix(ref camera);
            GL.Rotate(angle1 + zombieWalk, 0, 0, 1.0);
            GL.Rotate(angle2, 0, 1.0, 0);
            GL.Rotate(-legAngle2 - 90, 1.0, 0, 0);
            GL.Translate(10, 0, 0);
            loadedObjects.DrawObject(9); //right arm

            GL.LoadMatrix(ref camera);
            GL.Translate(0, -17, 8);
            GL.Rotate(72, 1, 0, 0);
            GL.Rotate(180, 0, 0, 1);
            GL.Rotate(180, 0, 1, 0);
            loadedObjects.DrawObject(10); //background



            glControl1.SwapBuffers();
        }
    }
}
