using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace AP
{
    public abstract class Position
    {
        public float xPos { get; set; }
        public float yPos { get; set; }
        public Vector3 position {get; set;}
        public Vector3 velocity { get; set; }
        public float life { get; set; }
        protected float speed { get; set; }
        public float xVel { get; set; }
        public float yVel { get; set; }
        public int UID { get; set; }
        // enum starts at 0
        // set any of these values to anything you want
        // i.e. none=0, slow=3, normal=5, fast=7
        //protected enum Speed { None=0, Slow, Normal, Fast };
    }
}
