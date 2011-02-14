using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AP
{
    class CollisionAI
    {
        private int Tiles = 50;
        //private float TileSize = 1;
        List<Enemy>[,] tileList;        

        public CollisionAI()
        {
            tileList = new List<Enemy>[Tiles, Tiles];
            for (int i = 0; i < Tiles; i++)
                for (int j = 0; j < Tiles; j++)
                    tileList[i, j] = new List<Enemy>();
        }

        public void updateState(ref List<Enemy> enemyList)
        {
            //clear the tiles from the last update
            for (int i = 0; i < Tiles - 1; i++)
                for (int j = 0; j < Tiles - 1; j++)
                    tileList[i, j].Clear();

            foreach (var member in enemyList)
            {
                int x = -1, y = -1;
                for (int i = 0; i < Tiles; i++)
                {
                    if (member.xPos <= i - Tiles / 2 && x == -1)
                    {
                        x = i;
                    }
                    if (member.yPos <= i - Tiles / 2 && y == -1)
                    {
                        y = i;
                    }
                }
                if (x != -1 && y != -1)
                {
                    tileList[x, y].Add(member);
                    //Console.WriteLine("Enemy x: " + member.xPos + " added to x tile: " + x);
                }
                else
                    Console.WriteLine("Failed to add an enemy to the tile list");
            }
            /*
            Circle tempCircle = circleList;
            while (tempCircle != null)
            {
                int x = -1, y = -1;
                for (int i = 0; i < Tiles; i++)
                {
                    numberOfTileChecks++;
                    if (tempCircle.posX + (Circle.distance / 10 * 8) / 2 <= ((i + 1) * (Circle.distance / 10 * 8) / Tiles) && x == -1)
                    {
                        x = i;
                    }
                    if (tempCircle.posY + (Circle.distance / 10 * 8) / 2 <= ((i + 1) * (Circle.distance / 10 * 8) / Tiles) && y == -1)
                    {
                        y = i;
                    }
                }

                if (x != -1 && y != -1)
                    tileList[x, y].Add(tempCircle);
                else
                    Console.WriteLine(tempCircle.posX);

                tempCircle = tempCircle.prevCircle;
            }*/


            /*
            tempCircle = circleList;
            while (tempCircle != null)
            {
                tempCircle.moveTowards(circleList);
                tempCircle = tempCircle.prevCircle;
            }

            if (Keyboard[Key.W])
                circleList.dirY = -0.01f;
            if (Keyboard[Key.S])
                circleList.dirY = 0.01f;
            if (!Keyboard[Key.W] && !Keyboard[Key.S])
                circleList.dirY = 0;

            if (Keyboard[Key.A])
                circleList.dirX = -0.01f;
            if (Keyboard[Key.D])
                circleList.dirX = 0.01f;
            if (!Keyboard[Key.A] && !Keyboard[Key.D])
                circleList.dirX = 0;

            //quick checks to keep the circles in the frame
            for (int i = 0; i < Tiles; i++)
                foreach (Circle a in tileList[0, i])
                {
                    if (a.posX < -(Circle.distance / 10 * 8) / 2)
                        a.dirX *= -1;
                }
            for (int i = 0; i < Tiles; i++)
                //these positive checks are strange.
                //setting the limit to the second last tile seems to avoid the NaNs
                foreach (Circle a in tileList[Tiles - 2, i])
                {
                    if (a.posX > 0)
                        a.dirX *= -1;
                }

            for (int i = 0; i < Tiles; i++)
                foreach (Circle a in tileList[i, 0])
                {
                    if (a.posY < -(Circle.distance / 10 * 8) / 2)
                        a.dirY *= -1;
                }
            for (int i = 0; i < Tiles; i++)
                foreach (Circle a in tileList[i, Tiles - 2])
                {
                    if (a.posY > 0)
                        a.dirY *= -1;
                }

            //and now the narrow phase checks
            for (int i = 0; i < Tiles; i++)
                for (int j = 0; j < Tiles; j++)
                    foreach (Circle a in tileList[i, j])
                    {
                        if (i < Tiles - 1 && j < Tiles - 1)
                            foreach (Circle b in tileList[i + 1, j + 1])
                            {
                                numberOfChecks++;
                                float diffX = a.posX - b.posX;
                                float diffY = a.posY - b.posY;
                                if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= a.radius + b.radius)
                                {
                                    numberOfCollisions++;
                                    a.collision(b.posX, b.posY);
                                }
                            }
                        if (i < Tiles - 1)
                            foreach (Circle b in tileList[i + 1, j])
                            {
                                numberOfChecks++;
                                float diffX = a.posX - b.posX;
                                float diffY = a.posY - b.posY;
                                if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= a.radius + b.radius)
                                {
                                    numberOfCollisions++;
                                    a.collision(b.posX, b.posY);
                                }
                            }
                        if (i < Tiles - 1 && j > 0)
                            foreach (Circle b in tileList[i + 1, j - 1])
                            {
                                numberOfChecks++;
                                float diffX = a.posX - b.posX;
                                float diffY = a.posY - b.posY;
                                if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= a.radius + b.radius)
                                {
                                    numberOfCollisions++;
                                    a.collision(b.posX, b.posY);
                                }
                            }
                        if (j < Tiles - 1)
                            foreach (Circle b in tileList[i, j + 1])
                            {
                                numberOfChecks++;
                                float diffX = a.posX - b.posX;
                                float diffY = a.posY - b.posY;
                                if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= a.radius + b.radius)
                                {
                                    numberOfCollisions++;
                                    a.collision(b.posX, b.posY);
                                }
                            }
                        foreach (Circle b in tileList[i, j])
                        {
                            numberOfChecks++;
                            float diffX = a.posX - b.posX;
                            float diffY = a.posY - b.posY;
                            if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= a.radius + b.radius && a != b)
                            {
                                numberOfCollisions++;
                                a.collision(b.posX, b.posY);
                            }
                        }
                        if (j > 0)
                            foreach (Circle b in tileList[i, j - 1])
                            {
                                numberOfChecks++;
                                float diffX = a.posX - b.posX;
                                float diffY = a.posY - b.posY;
                                if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= a.radius + b.radius)
                                {
                                    numberOfCollisions++;
                                    a.collision(b.posX, b.posY);
                                }
                            }
                        if (i > 0 && j < Tiles - 1)
                            foreach (Circle b in tileList[i - 1, j + 1])
                            {
                                numberOfChecks++;
                                float diffX = a.posX - b.posX;
                                float diffY = a.posY - b.posY;
                                if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= a.radius + b.radius)
                                {
                                    numberOfCollisions++;
                                    a.collision(b.posX, b.posY);
                                }
                            }
                        if (i > 0)
                            foreach (Circle b in tileList[i - 1, j])
                            {
                                numberOfChecks++;
                                float diffX = a.posX - b.posX;
                                float diffY = a.posY - b.posY;
                                if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= a.radius + b.radius)
                                {
                                    numberOfCollisions++;
                                    a.collision(b.posX, b.posY);
                                }
                            }
                        if (i > 0 && j > 0)
                            foreach (Circle b in tileList[i - 1, j - 1])
                            {
                                numberOfChecks++;
                                float diffX = a.posX - b.posX;
                                float diffY = a.posY - b.posY;
                                if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= a.radius + b.radius)
                                {
                                    numberOfCollisions++;
                                    a.collision(b.posX, b.posY);
                                }
                            }
                    }*/
        }

        public bool checkForCollision<T>(Position source, out T moveAwayFrom)
        {
            int i = -1;
            int j = -1;
            for (int l = 0; l < Tiles; l++)
            {
                if (source.xPos <= l - Tiles / 2 && i == -1)
                {
                    i = l;
                }
                if (source.yPos <= l - Tiles / 2 && j == -1)
                {
                    j = l;
                }
            }
            if (i != -1 && j != -1)
            {
                if (i < Tiles - 1 && j < Tiles - 1)
                    foreach (Enemy b in tileList[i + 1, j + 1])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            moveAwayFrom = (T)(object)b;
                            return true;
                        }
                    }
                if (i < Tiles - 1)
                    foreach (Enemy b in tileList[i + 1, j])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            moveAwayFrom = (T)(object)b;
                            return true;
                        }
                    }
                if (i < Tiles - 1 && j > 0)
                    foreach (Enemy b in tileList[i + 1, j - 1])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            moveAwayFrom = (T)(object)b;
                            return true;
                        }
                    }
                if (j < Tiles - 1)
                    foreach (Enemy b in tileList[i, j + 1])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            moveAwayFrom = (T)(object)b;
                            return true;
                        }
                    }
                foreach (Enemy b in tileList[i, j])
                {
                    float diffX = source.xPos - b.xPos;
                    float diffY = source.yPos - b.yPos;
                    if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius && source != b)
                    {
                        moveAwayFrom = (T)(object)b;
                        return true;
                    }
                }
                if (j > 0)
                    foreach (Enemy b in tileList[i, j - 1])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            moveAwayFrom = (T)(object)b;
                            return true;
                        }
                    }
                if (i > 0 && j < Tiles - 1)
                    foreach (Enemy b in tileList[i - 1, j + 1])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            moveAwayFrom = (T)(object)b;
                            return true;
                        }
                    }
                if (i > 0)
                    foreach (Enemy b in tileList[i - 1, j])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            moveAwayFrom = (T)(object)b;
                            return true;
                        }
                    }
                if (i > 0 && j > 0)
                    foreach (Enemy b in tileList[i - 1, j - 1])
                    {
                        float diffX = source.xPos - b.xPos;
                        float diffY = source.yPos - b.yPos;
                        if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius)
                        {
                            moveAwayFrom = (T)(object)b;
                            return true;
                        }
                    }
            }
                    moveAwayFrom = (T)(object)null;
            return false;
                    
        }
    }
}
