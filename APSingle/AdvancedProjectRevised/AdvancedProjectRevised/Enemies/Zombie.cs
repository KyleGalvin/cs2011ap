using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace AP
{
    /// <summary>
    /// Basic Zombie class.
    /// </summary>
    public class Zombie : Enemy
    {
		#region Fields (4) 
        public const int NORMAL = 0;
        public static float normalScale = 0.08f;
        public const int TANK = 1;
        public static float tankScale = 0.12f;
        public const int FAST = 2;
        public static float fastScale = 0.06f;
        public const int BOSS = 3;
        public static float bossScale = 1.0f;

        public static int drawNumber;
        bool incWalk = true;
        float legAngle = 0.0f;
        public bool walking = false;
        private float scale = 0;
        private float victoryJumpHeight = 0.0f;
        private bool ascending = true;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Initialize the creation of a zombie enemy.
        /// -Zombie's life: 1 hit.
        /// </summary>
        /// <param name="x">Specified spawning x position.</param>
        /// <param name="y">Specified spawning y position.</param>
        /// <output>None.</output>
        public Zombie( int ID )
        {
            enemyID = ID;
            updateTimeStamp();            
            changeSubtype(NORMAL);            
        }

		#endregion Constructors 

		#region Methods (4) 

		// Public Methods (4) 

        /// <summary>
        /// Reduces passed player's life.
        /// </summary>
        /// <param name="player">Passed played that was hit from a zombie attack.</param>
        /// <output>None.</output>
        public override void attack( Player player )
        {
            player.loseHealth( (float)Damage.Low );
        }

        public override void changeSubtype(int newType)
        {
            type = newType;
            if (type == NORMAL)
            {
                scale = normalScale;
                health = (int)Life.Zombie;                
                speed = (float)0.05;
                radius = 0.25f;           
            }
            else if (type == TANK)
            {
                scale = tankScale;
                health = (int)Life.Tank;
                speed = (float)0.035;
                radius = 0.25f * tankScale / normalScale;
            }
            else if (type == FAST)
            {
                scale = fastScale;
                health = (int)Life.Fast;
                speed = (float)0.12;
                radius = 0.25f * tankScale / normalScale;
            }
            else if (type == BOSS)
            {
                scale = bossScale;
                health = (int)Life.Boss;
                speed = 0;
                radius = 0.25f * bossScale / normalScale * 2;
                angle = -90;
            }
        }

        public override void drawVictory()
        {
            GL.PushMatrix();
            GL.Translate(xPos, yPos, 0);
            GL.Translate(0, 0, 0.4f / 0.08f * scale + victoryJumpHeight);
            GL.Rotate(angle - 90, 0, 0, 1);
            GL.Rotate(180, 0, 1.0f, 0);

            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie + type * 5); //body 

            GL.PushMatrix();
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie + 2 + type * 5); //right leg 
            GL.PopMatrix();

            GL.PushMatrix();
            GL.Translate(scale, 0, 0);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie + 1 + type * 5); //left leg 
            GL.PopMatrix();

            GL.PushMatrix();
            if (walking)
                GL.Rotate(180, 1.0, 0, 0);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie + 3 + type * 5); //left arm 
            GL.PopMatrix();

            GL.PushMatrix();
            if (walking)
                GL.Rotate(180, 1.0, 0, 0);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie + 4 + type * 5); //right arm 
            GL.PopMatrix();
            GL.PopMatrix();
        }

        public override void draw()
        {            
            GL.PushMatrix();
            GL.Translate(xPos, yPos, 0);
            GL.Translate(0, 0, 0.4f / 0.08f * scale);
            GL.Rotate(angle - 90, 0, 0, 1);
            GL.Rotate(180, 0, 1.0f, 0); 

            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie + type * 5); //body 

            GL.PushMatrix();
            if (walking)
            {
                GL.Rotate(legAngle, 1.0, 0, 0);
                GL.Translate(0, legAngle / 20 * scale, 0);
            }
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie + 2 + type * 5); //right leg 
            GL.PopMatrix();

            GL.PushMatrix();
            if (walking)
                GL.Rotate(-legAngle, 1.0, 0, 0);
            GL.Translate(scale, 0, 0);
            if (walking)
                GL.Translate(0, -legAngle / 20 * scale, 0);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie + 1 + type * 5); //left leg 
            GL.PopMatrix();

            GL.PushMatrix();
            if (walking)
                GL.Rotate(legAngle - 90, 1.0, 0, 0);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie + 3 + type * 5); //left arm 
            GL.PopMatrix();

            GL.PushMatrix();
            if (walking)
                GL.Rotate(-legAngle - 90, 1.0, 0, 0);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie + 4 + type * 5); //right arm 
            GL.PopMatrix();
            GL.PopMatrix();
        }

        

        /// <summary>
        /// Moves the zombie enemy using the passed x and y positions as
        /// determined from the AI calculation. The zombie has a speed of slow.
        /// </summary>
        /// <param name="x">Passed x position determined from AI calulation.</param>
        /// <param name="y">Passed y position determined from AI calulation.</param>
        /// <output>None.</output>
        public override void move( float x, float y )
        {            
            xPos += x * this.speed;
            yPos += y * this.speed;
            if (ascending)
            {
                victoryJumpHeight += 0.07f;
                if (victoryJumpHeight > 0.5f)
                    ascending = false;
            }
            else
            {
                victoryJumpHeight -= 0.07f;
                if (victoryJumpHeight <= 0)
                    ascending = true;
            }
        }

        /// <summary>
        /// Moves towards the player.
        /// </summary>
        /// <param name="target">The target.</param>
        public override void moveTowards(Player target)
        {
            float x = target.xPos - xPos;
            float y = target.yPos - yPos;
            prevXPos = xPos;
            prevYPos = yPos;

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

            if (type == BOSS)
                return; //boss doesnt move

            float len = (float)Math.Sqrt(x * x + y * y);
            float moveX;
            float moveY;
            if (ClientProgram.multiplayer)
            {
                if (!ServerProgram.collisionAI.checkForMovementCollision(this, out moveX, out moveY))
                    move(x / len, y / len); //free to move where you want
                else
                { //standing in something, move away from it
                    x = moveX - xPos;
                    y = moveY - yPos;

                    len = (float)Math.Sqrt(x * x + y * y);
                    move(-x / len, -y / len);
                }
            }
            else
            {
                move(x / len, y / len);
                setAngle();
                move(-x / len, -y / len);
                if (ClientProgram.collisionAI.checkForCollisionWithPlayers(this, out moveX, out moveY))
                { //standing in something, move away from it
                    x = moveX - xPos;
                    y = moveY - yPos;

                    len = (float)Math.Sqrt(x * x + y * y);
                    move(-x / len, -y / len);
                }
                else if (ClientProgram.collisionAI.checkForMovementCollision(this, out moveX, out moveY))
                { //standing in something, move away from it
                    x = moveX - xPos;
                    y = moveY - yPos;

                    len = (float)Math.Sqrt(x * x + y * y);
                    move(-x / len, -y / len);
                }
                else
                {
                    move(x / len, y / len); //free to move where you want
                }
            }


            updateTimeStamp();
        }
        /// <summary>
        /// Moves towards the player.
        /// </summary>
        /// <param name="target">The target.</param>
        public override void moveTowards(float _x, float _y)
        {
            float x = _x - xPos;
            float y = _y - yPos;
            prevXPos = xPos;
            prevYPos = yPos;

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

            if (type == BOSS)
                return; //boss doesnt move

            float len = (float)Math.Sqrt(x * x + y * y);
            float moveX;
            float moveY;
            if (ClientProgram.multiplayer)
            {
                if (!ServerProgram.collisionAI.checkForMovementCollision(this, out moveX, out moveY))
                    move(x / len, y / len); //free to move where you want
                else
                { //standing in something, move away from it
                    x = moveX - xPos;
                    y = moveY - yPos;

                    len = (float)Math.Sqrt(x * x + y * y);
                    move(-x / len, -y / len);
                }
            }
            else
            {
                move(x / len, y / len);
                setAngle();
                move(-x / len, -y / len);
                if (ClientProgram.collisionAI.checkForCollisionWithPlayers(this, out moveX, out moveY))
                { //standing in something, move away from it
                    x = moveX - xPos;
                    y = moveY - yPos;

                    len = (float)Math.Sqrt(x * x + y * y);
                    move(-x / len, -y / len);
                }
                else if (ClientProgram.collisionAI.checkForMovementCollision(this, out moveX, out moveY))
                { //standing in something, move away from it
                    x = moveX - xPos;
                    y = moveY - yPos;

                    len = (float)Math.Sqrt(x * x + y * y);
                    move(-x / len, -y / len);
                }
                else
                {
                    move(x / len, y / len); //free to move where you want
                }
            }
        }

		#endregion Methods 
    }
}
