using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AP
{
    /// <summary>
    /// The superclass of game enemies. It contains the definitions of the attributes and functions used by
    /// all enemy types in the game. This includes things like damage and life with abstract functions like
    /// move and draw.
    /// </summary>
    public abstract class Enemy : Position
    {
		#region Fields (1) 

               protected bool alive = true;

		#endregion Fields 

		#region Enums (2) 

        protected enum Damage { None = 0, Low = 1, Medium = 3, High = 5 }
protected enum Life { Zombie = 1, Harder = 2, Hardest = 3, Boss = 100 }

		#endregion Enums 

		#region Methods (6) 

		// Public Methods (6) 

        /// <summary>
        /// Attacks the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        public abstract void attack(Player player);

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public abstract void draw();

        /// <summary>
        /// Determines whether this instance has died.
        /// </summary>
        public void hasDied()
        {
            alive = false;
        }

        /// <summary>
        /// Moves the specified x and y.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
public abstract void move(float x, float y);

        /// <summary>
        /// Moves towards the player.
        /// </summary>
        /// <param name="target">The target.</param>
        public abstract void moveTowards(Player target);

        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void setPosition(float x, float y)
        {
            xPos = x;
            yPos = y;
        }

		#endregion Methods 
    }
}
