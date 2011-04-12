using System;
using System.Collections.Generic;
using System.Text;

namespace AP
{
    /// <summary>
    /// Used to store the information about the wall objects
    /// Contributors: Gage Patterson
    /// Revision: 215
    /// </summary>
    public class Wall
    {
        public float xPos;
        public float yPos;
        public int height;
        public int width;
        /// <summary>
        /// Initializes a new instance of the <see cref="Wall"/> class.
        /// </summary>
        /// <param name="_x">The _x.</param>
        /// <param name="_y">The _y.</param>
        /// <param name="_height">The _height.</param>
        /// <param name="_width">The _width.</param>
        public Wall(float _x, float _y, int _height, int _width)
        {
            xPos = _x;
            yPos = _y;
            height = _height;
            width = _width;
        }
    }
}
