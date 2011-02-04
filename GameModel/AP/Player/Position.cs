using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AP
{
    public class Position
    {
        protected float xPos { get; set; }
        protected float yPos { get; set; }
        public float life { get; set; }
        protected float speed { get; set; }
        protected float xVel { get; set; }
        protected float yVel { get; set; }
        // enum starts at 0
        // set any of these values to anything you want
        // i.e. none=0, slow=3, normal=5, fast=7
        //protected enum Speed { None=0, Slow, Normal, Fast };
    }
}
