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

        public Player( float x, float y, int ID )
        {
            xPos = x;
            yPos = y;
            life = 100;
            //client assigns passed ID from server.
            playerId = ID;
            speed = (float)0.05;
        }

        public void assignPlayerID( int ID )
        {
            playerId = ID;
        }

        public void move( int x, int y)
        {
            //xPos += (float)0.01;
            //yPos += (float)0.01;
            xPos += x * this.speed;
            yPos += y * this.speed;
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

        public void loseHealth( float damage )
        {
            life -= damage;
            //-update damage bar
            //-send update health bar packet
        }

        public void draw()
        {
            float colorR = 0.0f;
            float colorG = 0.0f;
            float colorB = 1.0f;
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
