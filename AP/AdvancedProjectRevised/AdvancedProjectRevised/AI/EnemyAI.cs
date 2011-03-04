using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AP.AI
{
    public class EnemyAI : Position
    {
        private double playerAngle;
        public void Update(ref List<Player> players, ref List<Enemy> enemies, ref List<int> xWalls, ref List<int> yWalls)
        {
            int vert, hor;
            //Find closest player
            Player closest = FindClosestPlayer(ref players);
            //Move towards that player
            if (this.xPos - closest.xPos > 0) hor = -1; else hor = 1;
            if (this.yPos - closest.yPos > 0) vert = -1; else vert = 1;
            move(hor, vert, ref enemies, ref closest,ref xWalls,ref yWalls);

            //Pathfind();
        }
        /// <summary>
        /// Moves the zombie enemy using the passed x and y positions as
        /// determined from the AI calculation. The zombie has a speed of slow.
        /// </summary>
        /// <param name="x">Passed x position determined from AI calulation.</param>
        /// <param name="y">Passed y position determined from AI calulation.</param>
        /// <output>None.</output>
        public void move(int x, int y, ref List<Enemy> enemies, ref Player closest, ref List<int> xWalls, ref List<int> yWalls)
        {
            //Maybe add some wander effect
            float p_x = CalculateX(x);
            float p_y = CalculateY(y);
            //Determine if that move colides with another enemy
            //Todo better detect colisions. Need to know the size of the enemy??
            float radius = (0.1f * 0.7f) * 2.5f;
            //if (enemies.Where(j => p_y <(radius + j.yPos) &&  p_y > (j.yPos - radius) && p_x < (radius + j.xPos) && p_x > (j.xPos - radius) &&j.UID!=this.UID ).Any())
            //{
            //    xPos = p_x;
            //    yPos = p_y;
            //}
            //if (((p_y < (radius + g.yPos) && p_y > (g.yPos - radius)) && (p_x < (radius + g.xPos) && p_x > (g.xPos - radius))))
            //{
            //    nogo = true;
            //}

            if (checkPosition(p_x, p_y, ref enemies, ref closest, ref xWalls, ref yWalls))
            {
                xPos = p_x;
                yPos = p_y;
                return;
            }
            else
            {   //This is if the player is above or below
                if ((playerAngle > 45 && playerAngle <= 135) || (playerAngle > 225 && playerAngle <= 315))
                {
                    p_x = CalculateX(x * -1);
                    if (checkPosition(p_x, p_y, ref enemies, ref closest, ref xWalls, ref yWalls))
                    {
                        xPos = p_x;
                        yPos = p_y; return;
                    }
                    else
                    {
                        p_x = CalculateX(0);
                        if (checkPosition(p_x, p_y, ref enemies, ref closest, ref xWalls, ref yWalls))
                        {
                            xPos = p_x;
                            yPos = p_y; return;
                        }
                    }
                }
                //this is if the player is side to side
                else if ((playerAngle <= 225 && playerAngle > 135) || (playerAngle <= 45) || playerAngle > 315)
                {
                    p_y = CalculateY(x * -1);
                    if (checkPosition(p_x, p_y, ref enemies, ref closest, ref xWalls, ref yWalls))
                    {
                        xPos = p_x;
                        yPos = p_y; return;
                    }
                    else
                    {
                        p_y = CalculateY(0);
                        if (checkPosition(p_x, p_y, ref enemies, ref closest, ref xWalls, ref yWalls))
                        {
                            xPos = p_x;
                            yPos = p_y; return;
                        }
                    }
                }
            }
            int hor, vert;
            if (this.xPos - closest.xPos > 0) hor = 1; else hor = -1;
            if (this.yPos - closest.yPos > 0) vert = 1; else vert = -1;
            if ((this.yPos - closest.yPos) < 0.7 && (this.yPos - closest.yPos) > -0.7)
            {
                hor = 0;
            }
            if ((this.xPos - closest.xPos) < 0.7 && (this.xPos - closest.xPos) > -0.7)
            {
                vert = 0;
            }

            p_x = CalculateX(hor);
            p_y = CalculateY(vert);
            if (checkPosition(p_x, p_y, ref enemies, ref closest, ref xWalls, ref yWalls))
            {
                xPos = p_x;
                yPos = p_y;
            }
        }

        private float CalculateX(int x)
        {
            Random randy = new Random();
            float wander = ((float)randy.NextDouble()) / 100;
            return xPos + (x * speed) + wander;
        }
        private float CalculateY(int y)
        {
            return yPos + (y * speed);
        }
        private bool checkPosition(float p_x, float p_y, ref List<Enemy> enemies, ref Player closest, ref List<int> xWalls, ref List<int> yWalls)
        {
            if ((Math.Sqrt(Math.Pow((closest.xPos - p_x), 2) + Math.Pow((closest.yPos - p_y), 2)) < ((0.1 * 0.7 * 2.0) + 0.1)))
                return false;
            for (int i = 0; i < xWalls.Count(); i++)
            {
                
                if (Math.Sqrt(Math.Pow((xWalls[i] - p_x), 2) + Math.Pow((yWalls[i] - p_y), 2)) < ((0.1 * 0.7) +1) + 0.1)
                {
                    //var tempAngle = Angle(p_x, p_y, g.xPos, g.yPos);
                    //var temp = tempAngle - playerAngle;
                    //if (temp >= -45 && temp <=45) 
                    return false;
                }
            }
            foreach (var g in enemies.Where(j => j.enemyID != this.enemyID))
            {
                if (Math.Sqrt(Math.Pow((g.xPos - p_x), 2) + Math.Pow((g.yPos - p_y), 2)) < ((0.1 * 0.7 * 2.0) + 0.1))
                {
                    //var tempAngle = Angle(p_x, p_y, g.xPos, g.yPos);
                    //var temp = tempAngle - playerAngle;
                    //if (temp >= -45 && temp <=45) 
                    return false;
                }
            }
            return true;
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
            playerAngle = Angle(this.xPos, this.yPos, result.xPos, result.yPos);
            return result;
        }
        public double Angle(double px1, double py1, double px2, double py2)
        {
            // Negate X and Y values
            double pxRes = px2 - px1;
            double pyRes = py2 - py1;
            double angle = 0.0;
            // Calculate the angle
            if (pxRes == 0.0)
            {
                if (pxRes == 0.0)
                    angle = 0.0;
                else if (pyRes > 0.0) angle = System.Math.PI / 2.0;
                else
                    angle = System.Math.PI * 3.0 / 2.0;
            }
            else if (pyRes == 0.0)
            {
                if (pxRes > 0.0)
                    angle = 0.0;
                else
                    angle = System.Math.PI;
            }
            else
            {
                if (pxRes < 0.0)
                    angle = System.Math.Atan(pyRes / pxRes) + System.Math.PI;
                else if (pyRes < 0.0) angle = System.Math.Atan(pyRes / pxRes) + (2 * System.Math.PI);
                else
                    angle = System.Math.Atan(pyRes / pxRes);
            }
            // Convert to degrees
            angle = angle * 180 / System.Math.PI; return angle;

        }
    }
}