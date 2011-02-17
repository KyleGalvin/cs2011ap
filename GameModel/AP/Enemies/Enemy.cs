using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AP
{
    public abstract class Enemy : Position
    {       
        protected bool alive = true;
        protected enum Damage { None = 0, Low = 1, Medium = 3, High = 5 }
        protected enum Life { Zombie = 1, Harder = 2, Hardest = 3, Boss = 100 }

        public abstract void move(float x, float y);
        public abstract void moveTowards(Player target);
        public abstract void attack(Player player);
        public abstract void draw();

        public void hasDied()
        {
            alive = false;
        }

        public void setPosition(float x, float y)
        {
            xPos = x;
            yPos = y;
        }
    }
}
