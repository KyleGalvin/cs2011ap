using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace AP
{
    /// <summary>
    /// Used to store the crate type and position for ammo pickups
    /// Contributors: Adam Humeniuk
    /// Revision: 278
    /// </summary>
    public class Crate : Position
    {
        public int crateType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Crate"/> class.
        /// </summary>
        /// <param name="spawnPos">The spawn pos.</param>
        /// <param name="UID">The UID.</param>
        public Crate(Vector2 spawnPos, int UID)
        {
            xPos = spawnPos.X;
            yPos = spawnPos.Y;

            radius = 0.35f;
            Random rand = new Random();
            crateType = rand.Next(0, 2);
            this.UID = UID;
        }

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public void draw()
        {
            GL.PushMatrix();
            GL.Translate(xPos, yPos, 0.5f);
            ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedObjectCrate + crateType); //body 
            GL.PopMatrix();
        }
    }
}
