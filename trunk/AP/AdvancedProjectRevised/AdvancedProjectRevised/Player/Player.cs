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
    /// Contributors: Scott Herman, Gage Patterson, Kyle Galvin, Adam Humeniuk
    /// Revision: 295
    /// </summary>
    public class Player : Position
    {
		#region Fields (10) 

        bool incWalk = true;
        float legAngle = 0.0f;
        public int modelNumber = 3;
        //todo make this private again
        public int playerId = -1;
        public string playerName;
        public int prevHealth=100;
         public int score = 0;
        public Tiles tiles;
        public bool walking = false;
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
            prevXPos = xPos;
            prevYPos = yPos; 
            health = 100;
            //client assigns passed ID from server.
            playerId = ID;
            Console.WriteLine("Setting the playerUID for " + playerId);
            speed = 0.1f;
            radius = 0.1f;
        }

        /// <summary> 
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        public Player()
        {
            this.position = position;
            prevXPos = xPos;
            prevYPos = yPos;  
            health = 100;
            xPos = 0;
            yPos = 0;
            speed = 0.1f;
            radius = 0.1f;
        }

		#endregion Constructors 

		#region Methods (6) 

		// Public Methods (6) 

        /// <summary>
        /// Animates the player.
        /// </summary>
        public void AnimatePlayer()
        {
            walking = true;
            if (incWalk)
            {
                legAngle += 8;
                if (legAngle > 35)
                    incWalk = false;
            }
            else
            {
                legAngle -= 8;
                if (legAngle < -35)
                    incWalk = true;
            }            
        }

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
            /*
            GL.PushMatrix();
            GL.Translate(0, 0, 0.4f);
            GL.Rotate(angle - 90, 0, 0, 1);
            GL.Rotate(180, 0, 1.0f, 0);            
            ClientProgram.loadedObjects.DrawObject(modelNumber);
            GL.PopMatrix();*/

            GL.PushMatrix();
            if (health <= 0)
            {
                GL.Rotate(90, 0, 1.0f, 0);
                GL.Rotate(90, 0, 0, 1.0f);
            }
            else
            {
                GL.Translate(0, 0, 0.4f);
                GL.Rotate(angle - 90, 0, 0, 1);
            }

            GL.Rotate(180, 0, 1.0f, 0); 

            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectPlayer); //body 

            GL.PushMatrix();
            if (walking)
            {
                GL.Rotate(legAngle, 1.0, 0, 0);
                GL.Translate(0, legAngle / 20 * 0.08f, 0);
            }
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectPlayer + 2); //right leg 
            GL.PopMatrix();

            GL.PushMatrix();
            if (walking)
                GL.Rotate(-legAngle, 1.0, 0, 0);
            GL.Translate(0.08f, 0, 0);
            if (walking)
                GL.Translate(0, -legAngle / 20 * 0.08f, 0);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectPlayer + 1); //left leg 
            GL.PopMatrix();

            GL.PushMatrix();
            if (walking)
                GL.Rotate(legAngle, 1.0, 0, 0);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectPlayer + 3); //left arm 
            GL.PopMatrix();

            GL.PushMatrix();
            if (walking)
                GL.Rotate(-legAngle, 1.0, 0, 0);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectPlayer + 4); //right arm 
            GL.PopMatrix();
            GL.PopMatrix();
        }

        /// <summary>
        /// Loses the health.
        /// </summary>
        /// <param name="damage">The damage.</param>
        public void loseHealth( float damage )
        {
            health -= (int)damage;
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
            AnimatePlayer();
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
            if (!tiles.isWall(xPos + x * this.speed, yPos + y * this.speed))
            {
                float len = (float) Math.Sqrt(x*x + y*y);
                float moveX;
                float moveY;
                if (xPos + x * this.speed < tiles.maxX-1 && xPos + x * this.speed > tiles.minX+2 && yPos + y * this.speed < tiles.maxY-2 &&
                    yPos + y*this.speed > tiles.minY+1) //wallCheck
                {
                    if (ClientProgram.multiplayer)
                    {
                        //if (!ServerProgram.collisionAI.checkForMovementCollision(this, out moveX, out moveY))
                        makeMove(x, y);
                        //else
                        //{
                        //move player to middle if touching a zombie
                        //dont get hit by a zombie in the middle or you get stuck
                        //change this later to damage I guess
                        //xPos = 0;
                        //yPos = 0;
                        //}
                    }
                    else
                    {
                        makeMove(x, y);
                    }
                }
            }


            timestamp = DateTime.Now.Ticks;
        }

		#endregion Methods 
    }
}
