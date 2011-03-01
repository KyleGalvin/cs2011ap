using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace AP
{
    /// <summary>
    /// The superclass for all things that will move around our map (players, enemies and bullets).
    /// </summary>
    public class Position// : IDisposable
    {
        public float xPos { get; set; }
        public float yPos { get; set; }
        public float prevXPos;
        public float prevYPos;
        public int angle = 0;
        public Vector3 position {get; set;}
        public Vector3 velocity { get; set; }
        public int UID { get; set; }
        public float life { get; set; }
        protected float speed { get; set; }
        public float xVel { get; set; }
        public float yVel { get; set; }
        public int enemyID;
        public float radius{ get; set; }

        //radian constants for angle based calculations
        const float PI = 3.1415926f;
        protected const float RAD_TO_DEG = 180.0f / PI;

        /// <summary>
        /// Sets the angle.
        /// </summary>
        public void setAngle()
        {
            prevXPos = xPos - prevXPos;
            prevYPos = yPos - prevYPos;
            angle = (int)(Math.Atan2(prevYPos, prevXPos) * RAD_TO_DEG);
        }
        /// <summary>
        /// Updates the specified _x pos.
        /// </summary>
        /// <param name="_xPos">The _x pos.</param>
        /// <param name="_yPos">The _y pos.</param>
        /// <param name="_xVel">The _x vel.</param>
        /// <param name="_yVel">The _y vel.</param>
        public void Update(byte[] _xPos, byte[] _yPos, byte[] _xVel, byte[] _yVel)
        {
            xPos = (float)BitConverter.ToDouble(_xPos, 0);
            yPos = (float)BitConverter.ToDouble(_yPos, 0);
            xVel = (float)BitConverter.ToDouble(_xVel, 0);
            yVel = (float)BitConverter.ToDouble(_yVel, 0);
        }
        /*public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }*/
    }
}
