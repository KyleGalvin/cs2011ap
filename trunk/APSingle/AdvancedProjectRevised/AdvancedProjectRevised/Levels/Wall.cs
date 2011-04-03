using System;
using System.Collections.Generic;
using System.Text;

namespace AP
{
    public class Wall
    {
        public float xPos;
        public float yPos;
        public int height;
        public int width;
        public Wall(float _x, float _y, int _height, int _width)
        {
            xPos = _x;
            yPos = _y;
            height = _height;
            width = _width;
        }
    }
}