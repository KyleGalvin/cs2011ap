using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using BeginMode = OpenTK.Graphics.OpenGL.BeginMode;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace AP
{
    /// <summary>
    /// This class is used to store the data for each tile generated from the map. Also holds the boolean used to determine if the tile is a wall or not.
    /// Contributors: Gage Patterson
    /// Revision: 215
    /// </summary>
    public class Tile
    {
		#region Fields (5) 

        private int _Value;
        public bool isWall;
        private int size = 1;
        public float X;
        public float Y;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="val">The val.</param>
        public Tile(float x, float y, int val)
        {
            this.X = x;
            this.Y = y;
            _Value = val;
        }

		#endregion Constructors 

		#region Methods (2) 

		// Public Methods (2) 

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public void draw()
        {
            if (isWall)
            {
                GL.Color3(1.0f, 0.0f, 0.0f); //RED
            }
            else
            {
                GL.Color3(0.0f, 1.0f, 0.0f);//GREEN
            }
            GL.Begin(BeginMode.Lines);						// Draw A Quad
            GL.Vertex3(X, Y, 0.0f);					// Top Left
            GL.Vertex3(X + size, Y, 0.0f);             //Top right
            GL.End();

            GL.Begin(BeginMode.Lines);
            GL.Vertex3(X + size, Y - size, 0.0f);					// Bottom Right
            GL.Vertex3(X, Y - size, 0.0f);					// Bottom Left
            GL.End();

            GL.Begin(BeginMode.Lines);
            GL.Vertex3(X, Y, 0.0f);					        // Top Left
            GL.Vertex3(X, Y - size, 0.0f);					// Bottom Left
            GL.End();

            GL.Begin(BeginMode.Lines);
            GL.Vertex3(X + size, Y, 0.0f);             //Top right
            GL.Vertex3(X + size, Y - size, 0.0f);					// Bottom Right
            GL.End();
        }

        /// <summary>
        /// Used to determine the value of the tile. Not used anymore.
        /// </summary>
        /// <returns></returns>
        public int Value()
        {
            if (isWall) return _Value * 999;
            return _Value;
        }

		#endregion Methods 
    }
}
