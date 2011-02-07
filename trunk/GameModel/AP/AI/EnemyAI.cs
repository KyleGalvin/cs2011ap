using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AP.AI
{
    public class EnemyAI : Position
    {
        public void Update(ref List<Player> players, ref List<Enemy> enemies)
        {
            int vert, hor;
            //Find closest player
            Player closest = FindClosestPlayer(ref players);
            //Move towards that player
            if (this.xPos - closest.xPos > 0) hor = -1; else hor = 1;
            if (this.yPos - closest.yPos > 0) vert = -1; else vert = 1;
            move(hor,vert, ref enemies);
        }
        /// <summary>
        /// Moves the zombie enemy using the passed x and y positions as
        /// determined from the AI calculation. The zombie has a speed of slow.
        /// </summary>
        /// <param name="x">Passed x position determined from AI calulation.</param>
        /// <param name="y">Passed y position determined from AI calulation.</param>
        /// <output>None.</output>
        public void move(int x, int y, ref List<Enemy> enemies)
        {
            //Maybe add some wander effect
            Random randy=new Random();
            float wander = ((float)randy.NextDouble())/100;
            float p_x = xPos + (x*speed)+wander;
            float p_y = yPos + (y * speed);
            //Determine if that move colides with another enemy
            //Todo better detect colisions. Need to know the size of the enemy??
            if(enemies.Where(j=>j.yPos==p_y && j.xPos==p_x).Count()==0)
            {
                xPos = p_x;
                yPos = p_y;
            }
        }
        private Player FindClosestPlayer(ref List<Player> players)
        {
            Player result = null;
            var dist = Double.MaxValue;
            foreach (var p in players)
            {
                var temp = Math.Sqrt(Math.Pow((p.xPos - this.xPos), 2) + Math.Pow((p.yPos - this.yPos), 2));
                if (temp < dist)
                {
                    result = p;
                    dist = temp;
                }
            }
            return result;
        }
    }
}
