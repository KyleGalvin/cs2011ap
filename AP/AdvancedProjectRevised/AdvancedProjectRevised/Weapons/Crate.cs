using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace AP
{
    public class Crate : Position
    {
        public int crateType;

        public Crate(Vector2 spawnPos)
        {
            xPos = spawnPos.X;
            yPos = spawnPos.Y;

            radius = 0.35f;
            Random rand = new Random();
            crateType = rand.Next(0, 2);
        }

        public void draw()
        {
            GL.PushMatrix();
            GL.Translate(xPos, yPos, 0.5f);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectCrate + crateType); //body 
            GL.PopMatrix();
        }
    }
}
