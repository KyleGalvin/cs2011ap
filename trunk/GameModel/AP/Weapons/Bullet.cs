﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace AP
{
    public class Bullet: Position
    {
        private int lifeTime = 30;
        private Bullet prevBullet = null;
        float radius = 0.1f;
        private float direction = 1.0f;
        float xVel = 0;
        float yVel = 0;

        public Bullet(Vector3 position, Vector3 velocity, ref Bullet prevBullet)
        {
            this.position = position;
            xPos = position.X;
            yPos = position.Y;
            this.velocity = velocity;
            speed = 4.5f;
            this.prevBullet = prevBullet;
        }
        public void move()
        {
            xPos += xVel * speed;
            yPos += yVel * speed;


            if (prevBullet != null)
                prevBullet.move();

            if (lifeTime <= 0)
                prevBullet = null;
            lifeTime--;
        }

        public void draw()
        {
            GL.Begin(BeginMode.Polygon);
            
            GL.Color3(1.0f, 1.0f, 0.0f);
            GL.Vertex3(xPos - radius, yPos, 4.0f);
            GL.Vertex3(xPos - radius * 0.7, yPos + radius * 0.7, 4.0f);
            GL.Vertex3(xPos, yPos + radius, 4.0f);
            GL.Vertex3(xPos + radius * 0.7, yPos + radius * 0.7, 4.0f);
            GL.Vertex3(xPos + radius, yPos, 4.0f);
            GL.Vertex3(xPos + radius * 0.7, yPos - radius * 0.7, 4.0f);
            GL.Vertex3(xPos, yPos - radius, 4.0f);
            GL.Vertex3(xPos - radius * 0.7, yPos - radius * 0.7, 4.0f); 
            GL.End();

            if (prevBullet != null)
                prevBullet.draw();
        }

        public bool killProjectile()
        {
            if (lifeTime <= 0)
                return true;
            return false;
        }

        public void setDirectionByMouse(float x, float y, int screenX, int screenY, ref Player player)
        {
            /*if (x <= screenX / 2)
                x *= -1;
            if (y >= screenY / 2)
                y *= -1;*/

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
    }
}