using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace AP
{
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

        public void setAngle()
        {
            prevXPos = xPos - prevXPos;
            prevYPos = yPos - prevYPos;
            angle = (int)(Math.Atan2(prevYPos, prevXPos) * RAD_TO_DEG);
        }

        /*public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }*/
    }
}
