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

        public static int drawNumber;
        bool incWalk = true;
        float legAngle = 0.0f;
        public bool walking = false;

        #endregion Fields

        #region Constructors (1)

        /// <summary>
        /// Initialize the creation of a zombie enemy.
        /// -Zombie's life: 1 hit.
        /// </summary>
        /// <param name="x">Specified spawning x position.</param>
        /// <param name="y">Specified spawning y position.</param>
        /// <output>None.</output>
        public Zombie(int ID)
        {
            health = (int)Life.Zombie;
            enemyID = ID;
            UID = ID;
            speed = (float)0.05;
            radius = 0.25f;
            health = 1;
            updateTimeStamp();
        }

        #endregion Constructors

        #region Methods (4)

        // Public Methods (4) 

        /// <summary>
        /// Reduces passed player's life.
        /// </summary>
        /// <param name="player">Passed played that was hit from a zombie attack.</param>
        /// <output>None.</output>
        public override void attack(Player player)
        {
            player.loseHealth((float)Damage.Low);
        }

        public override void draw()
        {
            GL.PushMatrix();
            GL.Translate(xPos, yPos, 0);
            GL.Translate(0, 0, 0.4f);
            GL.Rotate(angle - 90, 0, 0, 1);
            GL.Rotate(180, 0, 1.0f, 0);

            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie); //body 

            GL.PushMatrix();
            if (walking)
            {
                GL.Rotate(legAngle, 1.0, 0, 0);
                GL.Translate(0, legAngle / 20 * 0.08f, 0);
            }
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie + 2); //right leg 
            GL.PopMatrix();

            GL.PushMatrix();
            if (walking)
                GL.Rotate(-legAngle, 1.0, 0, 0);
            GL.Translate(0.08f, 0, 0);
            if (walking)
                GL.Translate(0, -legAngle / 20 * 0.08f, 0);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie + 1); //left leg 
            GL.PopMatrix();

            GL.PushMatrix();
            if (walking)
                GL.Rotate(legAngle - 90, 1.0, 0, 0);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie + 3); //left arm 
            GL.PopMatrix();

            GL.PushMatrix();
            if (walking)
                GL.Rotate(-legAngle - 90, 1.0, 0, 0);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectZombie + 4); //right arm 
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
        public override void move(float x, float y)
        {
            xPos += x * this.speed;
            yPos += y * this.speed;
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
        #endregion Methods
    }
}
