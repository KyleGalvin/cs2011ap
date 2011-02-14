using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace AP
{
    class EnemySpawn : Position
    {
        public EnemySpawn(float x, float y,int ID)
        {
            xPos = x;
            yPos = y;
            enemyID = ID;
        }

        public Enemy spawnEnemy(int ID)
        {
            Enemy enemy = new Zombie(ID);
            
            //Console.WriteLine("Spawning");
            enemy.setPosition(xPos, yPos);
            enemy.draw();

            return enemy;
        }

        public void draw()
        {
            // OpenGL Calls
            float colorR = 1.0f;
            float colorG = 1.0f;
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
