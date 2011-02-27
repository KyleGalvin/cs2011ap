using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace AP
{
    public class Player : Position
    {
		#region Fields (4) 

        public int modelNumber;
        private int playerId;
        public string playerName;
        public Weapon weapons = new Weapon();

		#endregion Fields 

		#region Constructors (2) 

        public Player( Vector3 position, int ID )
        {
            this.position = position;
            xPos = position.X;
            yPos = position.Y;
            life = 100;
            //client assigns passed ID from server.
            playerId = ID;
            speed = 0.1f;
            radius = 0.1f;
        }

        public Player()
        {
            this.position = position;
            life = 100;
            xPos = 0;
            yPos = 0;
            speed = 0.1f;
            radius = 0.1f;
        }

		#endregion Constructors 

		#region Methods (5) 

		// Public Methods (5) 

        /// <summary>
        /// Assigns the player ID.
        /// </summary>
        /// <param name="ID">The ID.</param>
        public void assignPlayerID( int ID )
        {
            playerId = ID;
        }

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public void draw()
        {
            float colorR = 0.0f;
            float colorG = 0.0f;
            float colorB = 1.0f;
            float radius = 0.1f;
            GL.PushMatrix();
            GL.Translate(0, 0, 0.4f);
            GL.Rotate(angle - 90, 0, 0, 1);
            GL.Rotate(180, 0, 1.0f, 0);            
            Program.loadedObjects.DrawObject(modelNumber);
            GL.PopMatrix();
            /*GL.Begin( BeginMode.Polygon);

            GL.Color3(colorR, colorG, colorB);
            GL.Vertex3(-radius, 0, 0.01f);
            GL.Vertex3(-radius * 0.7, radius * 0.7, 0.01f);
            GL.Vertex3(0, radius, 0.01f);
            GL.Vertex3(radius * 0.7, radius * 0.7, 0.01f);
            GL.Vertex3(radius, 0, 0.01f);
            GL.Vertex3(radius * 0.7, -radius * 0.7, 0.01f);
            GL.Vertex3(0, -radius, 0.01f);
            GL.Vertex3(-radius * 0.7, -radius * 0.7, 0.01f);

            GL.End();*/
        }

        /// <summary>
        /// Loses the health.
        /// </summary>
        /// <param name="damage">The damage.</param>
        public void loseHealth( float damage )
        {
            life -= damage;
            //-update damage bar
            //-send update health bar packet
        }

        /// <summary>
        /// Makes the move.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void makeMove( int x, int y)
        {
            prevXPos = xPos;
            prevYPos = yPos;            
            xPos += x * this.speed;
            yPos += y * this.speed;
            setAngle();
            position = new Vector3(xPos, yPos,0);
        }

        /// <summary>
        /// Moves the specified x and y.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void move(int x, int y)
        {
            float len = (float)Math.Sqrt(x * x + y * y);
            float moveX;
            float moveY;
            if (xPos + x * this.speed < 7 && xPos + x * this.speed > -8 && yPos + y * this.speed < 6 && yPos + y * this.speed > -7) //wallCheck
            {
                if (!Program.collisionAI.checkForMovementCollision(this, out moveX, out moveY))
                    makeMove(x, y);
                else
                {
                    //move player to middle if touching a zombie
                    //dont get hit by a zombie in the middle or you get stuck
                    //change this later to damage I guess
                    xPos = 0;
                    yPos = 0;
                }
            }
        }

		#endregion Methods 
    }
}
