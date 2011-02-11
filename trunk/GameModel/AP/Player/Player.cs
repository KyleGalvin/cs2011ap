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
        private int playerId;
        private int bulletCooldown;

        public Player( Vector3 position, int ID )
        {
            this.position = position;
            life = 100;
            //client assigns passed ID from server.
            playerId = ID;
            speed = 0.1f;
            bulletCooldown = 0;
        }

        public Player()
        {
            this.position = position;
            life = 100;
            //client assigns passed ID from server.
            xPos = 0;
            yPos = 0;
            speed = 0.1f;
            bulletCooldown = 0;
        }

        public void assignPlayerID( int ID )
        {
            playerId = ID;
        }

        public void updateBulletCooldown()
        {
            bulletCooldown--;
        }

        public bool canShoot()
        {
            if (bulletCooldown <= 0)
            {
                bulletCooldown = 10;
                return true;
            }
            return false;
        }

        public void move( int x, int y)
        {
            Vector3 newPosition;
            xPos += x * this.speed;
            yPos += y * this.speed;
            //Console.WriteLine("player x: " + xPos);
            //Console.WriteLine("player y: " + yPos);
            position = new Vector3(xPos, yPos,0);
            //Console.WriteLine( "xPos =: " + ((float)xPos).ToString());
            //Console.WriteLine( "yPos =: " + ((float)yPos).ToString());
        }

        public void loseHealth( float damage )
        {
            life -= damage;
            //-update damage bar
            //-send update health bar packet
        }

        public void draw()
        {
            float colorR = 1.0f;
            float colorG = 1.0f;
            float colorB = 0.0f;
            float radius = 0.1f;

            GL.Begin( BeginMode.Polygon);

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
