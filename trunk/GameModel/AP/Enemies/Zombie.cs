﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace AP
{
    public class Zombie : Enemy
    {
        public static int drawNumber; //not sure how this should properly be inheirited so just putting it here for now
        /// <summary>
        /// Initialize the creation of a zombie enemy.
        /// -Zombie's life: 1 hit.
        /// </summary>
        /// <param name="x">Specified spawning x position.</param>
        /// <param name="y">Specified spawning y position.</param>
        /// <output>None.</output>
        public Zombie( int ID )
        {
            life = (int)Life.Zombie;
            enemyID = ID;
            speed = (float)0.05;
            radius = 0.08f;
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
        }

        public override void moveTowards(Player target)
        {
            float x = target.xPos - xPos;
            float y = target.yPos - yPos;
            prevXPos = xPos;
            prevYPos = yPos;

            float len = (float)Math.Sqrt(x * x + y * y);
            float moveX;
            float moveY;
            if (!Program.collisionAI.checkForMovementCollision(this, out moveX, out moveY))
                move(x / len, y / len); //free to move where you want
            else
            { //standing in something, move away from it
                x = moveX - xPos;
                y = moveY - yPos;

                len = (float)Math.Sqrt(x * x + y * y);
                move(-x / len, -y / len);
            }
            setAngle();
        }

        /// <summary>
        /// Reduces passed player's life.
        /// </summary>
        /// <param name="player">Passed played that was hit from a zombie attack.</param>
        /// <output>None.</output>
        public override void attack( Player player )
        {
            player.loseHealth( (float)Damage.Low );
        }

        public override void draw()
        {
            GL.PushMatrix();
            GL.Translate(xPos, yPos, 0.2f);
            GL.Rotate(angle - 115, 0, 0, 1);
            GL.Rotate(90, 1.0, 0, 0);
            Program.loadedObjects.DrawObject(drawNumber);
            GL.PopMatrix();
        }
    }
}
