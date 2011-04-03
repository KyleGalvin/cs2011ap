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
    public class Tile
    {
        public float X;
        public float Y;
        private int size = 1;
        public bool isWall;
        private int _Value;
        public Tile(float x, float y, int val)
        {
            this.X = x;
            this.Y = y;
            _Value = val;
        }

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
            GL.Begin(BeginMode.Lines);                                          // Draw A Quad
            GL.Vertex3(X, Y, 0.0f);                                     // Top Left
            GL.Vertex3(X + size, Y, 0.0f);             //Top right
            GL.End();

            GL.Begin(BeginMode.Lines);
            GL.Vertex3(X + size, Y - size, 0.0f);                                       // Bottom Right
            GL.Vertex3(X, Y - size, 0.0f);                                      // Bottom Left
            GL.End();

            GL.Begin(BeginMode.Lines);
            GL.Vertex3(X, Y, 0.0f);                                             // Top Left
            GL.Vertex3(X, Y - size, 0.0f);                                      // Bottom Left
            GL.End();

            GL.Begin(BeginMode.Lines);
            GL.Vertex3(X + size, Y, 0.0f);             //Top right
            GL.Vertex3(X + size, Y - size, 0.0f);                                       // Bottom Right
            GL.End();
        }

        public int Value()
        {
            if (isWall) return _Value * 999;
            return _Value;
        }
    }
}
