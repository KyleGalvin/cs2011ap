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
        bool[,] blockedByWall;

        public CollisionAI(ref List<int> xWalls, ref List<int> yWalls, ref List<int> xSize, ref List<int> ySize)
        {
            tileList = new List<Enemy>[Tiles, Tiles];
            blockedByWall = new bool[Tiles, Tiles];
            int wallCount = 0;
            for (int i = 0; i < Tiles; i++)
                for (int j = 0; j < Tiles; j++)
                {
                    tileList[i, j] = new List<Enemy>();
                    bool blocked = false;
                    for (int w = 0; w < xWalls.Count; w++)
                    {
                        int countX = xSize[w];
                        int countY = ySize[w];
                        for (int x = countX - 1; x >= 0; x--)
                            for (int y = countY - 1; y >= 0; y--)
                            {
                                if (xWalls[w] + x == i - Tiles / 2 && yWalls[w] + y == j - Tiles / 2 && !blocked)
                                {
                                    blocked = true;
                                    blockedByWall[i, j] = true;
                                    wallCount++;
                                    //Console.WriteLine("{0} {1}", i, j);
                                }
                            }
                    }
                    if (!blocked)
                        blockedByWall[i, j] = false;
                }
            Console.WriteLine("wall count{0}", wallCount);
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
        }

        public bool checkForMovementCollision(Position source, out float moveAwayFromX, out float moveAwayFromY)
        {
            Enemy enemyHit;
            //disabling the buggy wall check for now
            //if (checkForWallCollision(source, out moveAwayFromX, out moveAwayFromY))
                //return true;
            return checkForCollision(source, out moveAwayFromX, out moveAwayFromY, out enemyHit);
        }

        public bool checkForWallCollision(Position source, out float moveAwayFromX, out float moveAwayFromY)
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
                if (blockedByWall[i, j])
                {
                    moveAwayFromX = i - Tiles / 2;
                    moveAwayFromY = j - Tiles / 2;
                        int integerDropY = (int)source.yPos;
                        int integerDropX = (int)source.xPos;
                            if (source.yPos - integerDropY > -0.5f && source.yPos - integerDropY < 0.5f)
                            {
                                moveAwayFromX = source.xPos;
                                moveAwayFromY = source.yPos - 50;
                            }
                            if (source.xPos - integerDropX > -0.5f && source.xPos - integerDropX < 0.5f)
                            {
                                moveAwayFromX = source.xPos - 50;
                                moveAwayFromY = source.yPos;
                            }
                       
                        //Console.WriteLine("{0} {1} {2} {3}", source.xPos, source.yPos, source.xPos - integerDropX, source.yPos - integerDropY);
                        return true;
                }
                //Console.WriteLine("{0}", i);
                /*
                if (blockedByWall[i, j])
                {
                    moveAwayFromX = i + 0.5f;
                    moveAwayFromY = j + 0.5f;
                    return true;
                }*/
            }
            moveAwayFromX = 0;
            moveAwayFromY = 0;
            return false;
        }

        public bool checkForCollision(Position source, out float moveAwayFromX, out float moveAwayFromY, out Enemy enemyHit)
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
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
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
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
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
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
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
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
                            return true;
                        }
                    }
                foreach (Enemy b in tileList[i, j])
                {
                    float diffX = source.xPos - b.xPos;
                    float diffY = source.yPos - b.yPos;
                    if ((float)Math.Sqrt(diffX * diffX + diffY * diffY) <= source.radius + b.radius && source != b)
                    {
                        enemyHit = b;
                        moveAwayFromX = b.xPos;
                        moveAwayFromY = b.yPos;
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
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
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
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
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
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
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
                            enemyHit = b;
                            moveAwayFromX = b.xPos;
                            moveAwayFromY = b.yPos;
                            return true;
                        }
                    }
            }
            //these need to be assigned but arnt ever used
            enemyHit = null;
            moveAwayFromX = 0;
            moveAwayFromY = 0;
            return false;                    
        }
    }
}
