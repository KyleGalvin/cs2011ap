using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace AP
{
    /// <summary>
    /// The class that will keep track of a players state and handle the movement and actions of the player.
    /// </summary>
    public class Player : Position
    {
		#region Fields (4) 

        public int modelNumber = 3;
        //todo make this private again
        public int playerId = -1;
        public string playerName;
        public Weapon weapons = new Weapon();

		#endregion Fields 

		#region Constructors (2) 
        /// <summary>
        /// The constructor for a player. It will set all the player parameters to default values.
        /// </summary>
        /// <param name="position">The starting position of the player.</param>
        /// <param name="ID">The player ID</param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
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

        public void updateTimeStamp()
        {

            //Console.WriteLine(timestamp);
            timestamp = DateTime.Now.Ticks;
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
            GL.Translate(xPos, yPos, 0.4f);
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
        /// Draws this instance.
        /// </summary>
        public void drawOtherPlayer()
        {
            float colorR = 0.0f;
            float colorG = 0.0f;
            float colorB = 1.0f;
            float radius = 0.1f;
            GL.PushMatrix();
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
        /// A function that moves the player to a new position relative to his current position and speed.
        /// </summary>
        /// <param name="x">The x direction that the player is trying to move.</param>
        /// <param name="y">The y direction that the player is trying to move.</param>
        /// <output>
        /// No output, but the players position is updated.
        ///   </output>
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
        /// The move function is called when a movement key is pressed.
        /// It will check to see if the player is able to move and if so call makeMove().
        /// If it see that the player has collided with a zombie it will move him to the middle.
        /// </summary>
        /// <param name="x">The x direction that the player is trying to move.</param>
        /// <param name="y">The y direction that the player is trying to move.</param>
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
