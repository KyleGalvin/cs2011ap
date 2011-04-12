using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AP
{
    /// <summary>
    /// Called everytime a map is loaded to generate a list of tiles used in collision and path finding.
    /// Contributors: Gage Patterson
    /// Revision: 293
    /// </summary>
    public class Tiles
    {
		#region Fields (6) 

        public float maxX;
        public float maxY;
        public float minX;
        public float minY;
        Random random = new Random();
        public Tile[,] tileList;

		#endregion Fields 

		#region Constructors (2) 

        /// <summary>
        /// Initializes a new instance of the <see cref="Tiles"/> class.
        /// </summary>
        /// <param name="_minX">The _min X.</param>
        /// <param name="_maxX">The _max X.</param>
        /// <param name="_minY">The _min Y.</param>
        /// <param name="_maxY">The _max Y.</param>
        public Tiles(float _minX, float _maxX, float _minY, float _maxY)
        {
            minX = _minX;
            maxX = _maxX;
            minY = _minY;
            maxY = _maxY;
            //calculateTiles();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tiles"/> class.
        /// </summary>
        /// <param name="walls">The walls.</param>
        public Tiles(List<Wall> walls)
        {
            minX = walls.Min(y => y.xPos) - 2;
            maxX = walls.Max(y => y.xPos) + 2;
            minY = walls.Min(y => y.yPos) - 2;
            maxY = walls.Max(y => y.yPos) + 2;
            calculateTiles(walls);
        }

		#endregion Constructors 

		#region Methods (7) 

		// Public Methods (5) 

        /// <summary>
        /// Returns a bytelist representation of the tilelist. 1 is open 0 is a wall.
        /// </summary>
        /// <returns></returns>
        public byte[,] byteList()
        {
            var returnList = new byte[tileList.GetUpperBound(0) + 1, tileList.GetUpperBound(1) + 1];
            for (int index00 = 0; index00 <= returnList.GetUpperBound(0); index00++)
                for (int index01 = 0; index01 <= returnList.GetUpperBound(1); index01++)
                {
                    returnList[index00, index01]=1;
                }
            for (int i0 = 0; i0 <= tileList.GetUpperBound(0); i0++)
                for (int i1 = 0; i1 <= tileList.GetUpperBound(1); i1++)
                {
                    if (tileList[i0, i1].isWall)
                    {
                        returnList[i0, i1] = 0;
                        
                    }
                    else
                    {
                        if (returnList[i0, i1] != 0)
                        {
                            returnList[i0, i1] = 1;
                        }
                    }
                }
            return returnList;
        }

        /// <summary>
        /// Determines whether the specified position is in a wall.
        /// </summary>
        /// <param name="xPos">The x pos.</param>
        /// <param name="yPos">The y pos.</param>
        /// <returns>
        ///   <c>true</c> if the specified position is in a wall; otherwise, <c>false</c>.
        /// </returns>
        public bool isWall(float xPos, float yPos)
        {
            for (int i0 = 0; i0 <= tileList.GetUpperBound(0); i0++)
                for (int i1 = 0; i1 <= tileList.GetUpperBound(1); i1++)
                {
                    var x = tileList[i0, i1];
                    if (xPos >= x.X && xPos < x.X + 1 && yPos >= x.Y - 1 && yPos < x.Y)
                    {
                        if (tileList[i0, i1].isWall)
                        {
                            return true;
                        }
                    }
                }
            return false;
        }

        /// <summary>
        /// Returns the coordinates of the tile.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="j">The j.</param>
        /// <returns></returns>
        public List<float> returnCoords(int i, int j)
        {
            var returnList = new List<float>();
            returnList.Add(minX + i +0.5f );
            returnList.Add(minY + j -0.5f);
            return returnList;
        }

        /// <summary>
        /// Returns the tile position.
        /// </summary>
        /// <param name="p">The position.</param>
        /// <returns></returns>
        public List<int> returnTilePos(Position p)
        {
            var returnList = new List<int>();
            for (int i0 = 0; i0 <= tileList.GetUpperBound(0); i0++)
                for (int i1 = 0; i1 <= tileList.GetUpperBound(1); i1++)
                {
                    var x = tileList[i0, i1];
                    if (p.xPos >= x.X && p.xPos < x.X + 1 && p.yPos >= x.Y - 1 && p.yPos < x.Y)
                    {
                        returnList.Add(i0);
                        returnList.Add(i1);
                        return returnList;
                    }
                }
            return null;
        }

        /// <summary>
        /// Determines a location to spawn a crate. Used in multiplayer.
        /// </summary>
        /// <returns></returns>
        public List<float> SpawnCrate()
        {
            while (true)
            {
                var x = RandomNumber(0, (int)maxX + (int)Math.Abs(minX));
                var y = RandomNumber(0, (int)maxY + (int)Math.Abs(minY));
                if (!tileList[x, y].isWall && tileList[x, y].X < maxX-2 && tileList[x, y].Y < maxY-2 && tileList[x, y].X > minX+2 && tileList[x, y].Y > minY+1)
                {
                    var returnList = new List<float>();
                    
                    returnList.Add(tileList[x,y].X-0.5f);
                    returnList.Add(tileList[x, y].Y + 0.5f);
                    return returnList;
                }
            }
        }
		// Private Methods (2) 

        /// <summary>
        /// Calculates the tiles.
        /// </summary>
        /// <param name="walls">The walls.</param>
        private void calculateTiles(List<Wall> walls)
        {
            var width = Int32.Parse((maxX - minX).ToString());
            var height = Int32.Parse((maxY - minY).ToString());
            tileList = new Tile[width, height];
            //create the tiles with two for loops
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //Sets the x and y for each tile
                    tileList[i, j] = new Tile(minX + i, minY + j, i + j);
                    if (walls.Where(y => (y.yPos <= tileList[i, j].Y) && ((y.yPos + y.height) > tileList[i, j].Y) && (y.xPos <= tileList[i, j].X) && ((y.xPos + y.width) > tileList[i, j].X)).Count() > 0)
                    {
                        tileList[i, j].isWall = true;
                    }
                }
            }
            //Go through each loop and check to see if the space is occuppied by the walls or not.

        }

        /// <summary>
        /// Returns a random number
        /// </summary>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns></returns>
        private int RandomNumber(int min, int max)
        {            
            return random.Next(min, max);
        }

		#endregion Methods 
    }
}
