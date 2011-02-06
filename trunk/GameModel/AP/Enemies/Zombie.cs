using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace AP
{
    public class Zombie : Enemy
    {
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
            speed = (float)0.01;
        }

        public Zombie(int ID, int xPosInit, int yPosInit, int xVelInit, int yVelInit)
        {
            life = (int)Life.Zombie;
            UID = ID;
            speed = (float)0.01;
            xPos = xPosInit;
            yPos = yPosInit;
            xVel = xVelInit;
            yVel = yVelInit;
        }
        
        /// <summary>
        /// Moves the zombie enemy using the passed x and y positions as
        /// determined from the AI calculation. The zombie has a speed of slow.
        /// </summary>
        /// <param name="x">Passed x position determined from AI calulation.</param>
        /// <param name="y">Passed y position determined from AI calulation.</param>
        /// <output>None.</output>
        public override void move( int x, int y )
        {
            //kind of unknown what we are gonna do here right now.
            xPos += x * speed;
            yPos += y * speed;
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
            // OpenGL Calls
            float colorR = 1.0f;
            float colorG = 0.0f;
            float colorB = 0.0f;
            float radius = 0.1f;

            GL.Begin(BeginMode.Polygon);

            GL.Color3(colorR, colorG, colorB);
            GL.Vertex3(xPos - radius, yPos, 4.0f);
            GL.Vertex3(xPos - radius * 0.7, yPos + radius * 0.7, 4.0f);
            GL.Vertex3(xPos, yPos + radius, 4.0f);
            GL.Vertex3(xPos + radius * 0.7, yPos + radius * 0.7, 4.0f);
            GL.Vertex3(xPos + radius, yPos, 4.0f);
            GL.Vertex3(xPos + radius * 0.7, yPos - radius * 0.7, 4.0f);
            GL.Vertex3(xPos, yPos - radius, 4.0f);
            GL.Vertex3(xPos - radius * 0.7, yPos - radius * 0.7, 4.0f);

            GL.End();
        }
    }
}
