using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace AP
{
    public class Bullet: Position
    {
		#region Fields (4) 

        //private Bullet prevBullet = null;
        private float direction = 1.0f;
        private int lifeTime = 30;
        float xVel = 0;
        float yVel = 0;

		#endregion Fields 

		#region Constructors (1) 

        public Bullet(Vector3 position, Vector3 velocity)
        {
            radius = 0.1f;
            this.position = position;
            xPos = position.X;
            yPos = position.Y;
            this.velocity = velocity;
            speed = 2.0f;
        }

		#endregion Constructors 

		#region Methods (4) 

		// Public Methods (4) 

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public void draw()
        {
            GL.Begin(BeginMode.Polygon);
            
            GL.Color3(0.0f, 0.0f, 0.0f);
            GL.Vertex3(xPos - radius, yPos, 0.1f);
            GL.Vertex3(xPos - radius * 0.7, yPos + radius * 0.7, 0.1f);
            GL.Vertex3(xPos, yPos + radius, 0.1f);
            GL.Vertex3(xPos + radius * 0.7, yPos + radius * 0.7, 0.1f);
            GL.Vertex3(xPos + radius, yPos, 0.1f);
            GL.Vertex3(xPos + radius * 0.7, yPos - radius * 0.7, 0.1f);
            GL.Vertex3(xPos, yPos - radius, 0.1f);
            GL.Vertex3(xPos - radius * 0.7, yPos - radius * 0.7, 0.1f); 
            GL.End();

            //if (prevBullet != null)
                //prevBullet.draw();
        }

        /// <summary>
        /// Kills the projectile.
        /// </summary>
        /// <returns></returns>
        public bool killProjectile()
        {
            if (lifeTime <= 0)
                return true;
            return false;
        }

        /// <summary>
        /// Moves this instance.
        /// </summary>
        public void move()
        {
            xPos += xVel * speed;
            yPos += yVel * speed;

            lifeTime--;
        }

        /// <summary>
        /// Sets the direction by mouse.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="screenX">The screen X.</param>
        /// <param name="screenY">The screen Y.</param>
        /// <param name="player">The player.</param>
        public void setDirectionByMouse(float x, float y, int screenX, int screenY, ref Player player)
        {
            float mx = (float)(x - screenX / 2) / (screenX * 0.3f);
            float my = (float)(y - screenY / 2) / (screenY * 0.3f);

            xVel = -mx;
            yVel = -my;

            float len = (float)Math.Sqrt(xVel * xVel + yVel * yVel);
            xVel /= len;
            yVel /= len;

            xVel /= 10;
            yVel /= 10;

            xVel *= -1;
            yVel *= 1;
        }

		#endregion Methods 
    }
}
