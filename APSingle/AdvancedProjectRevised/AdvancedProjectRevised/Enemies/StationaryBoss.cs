using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AP
{
    /// <summary>
    /// This class is used for the stationary boss
    /// Contributors: Scott Herman
    /// Revision: 215
    /// </summary>
    public class StationaryBoss: Enemy
    {
		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see cref="StationaryBoss"/> class.
        /// </summary>
        /// <param name="ID">The ID.</param>
        public StationaryBoss(int ID)
        {
            health = (int)Life.Boss;
            enemyID = ID;
        }

		#endregion Constructors 

		#region Methods (4) 

		// Public Methods (4) 

        /// <summary>
        /// Attacks the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        public override void attack( Player player )
        {
            // insert weapon object attack method
            // i.e. pistolx2, rifle, rocket, etc
            player.loseHealth((float)Damage.High);
        }

        public override void changeSubtype(int newType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public override void draw()
        {
            // OpenGL Calls
        }

        public override void drawVictory()
        {
            // OpenGL Calls
        }

        /// <summary>
        /// Moves the specified x and y.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public override void move(float x, float y)
        {
            //no need to move a stationary boss :)
        }

        /// <summary>
        /// Moves towards the player.
        /// </summary>
        /// <param name="target">The target.</param>
        public override void moveTowards(Player target)
        {
            //no need to move a stationary boss :)
        }

        public override void moveTowards(float _x, float _y)
        {
            //no need to move a stationary boss :)
        }

		#endregion Methods 
    }
}
