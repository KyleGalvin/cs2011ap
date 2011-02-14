using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace AP
{
    public class Position
    {
        public float xPos { get; set; }
        public float yPos { get; set; }
        public Vector3 position {get; set;}
        public Vector3 velocity { get; set; }
        public float life { get; set; }
        protected float speed { get; set; }
        public float xVel { get; set; }
        public float yVel { get; set; }
        public int enemyID;
        public float radius{ get; set; }
    }
}
