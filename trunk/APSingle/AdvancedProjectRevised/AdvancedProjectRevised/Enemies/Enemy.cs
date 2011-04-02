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
               public int type = 0;

		#endregion Fields 

		#region Enums (2) 

        protected enum Damage { None = 0, Low = 1, Medium = 3, High = 5 }
        public enum Life { Zombie = 1, Tank = 10, Fast = 2, Hardest = 3, Boss = 100 }

		#endregion Enums 

		#region Methods (6) 

		// Public Methods (6) 

        /// <summary>
        /// Attacks the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        public abstract void attack(Player player);

        /// <summary>
        /// Become a subtype for the subclass.
        /// </summary>
        /// <param name="player">The player.</param>
        public abstract void changeSubtype(int newType);

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public abstract void draw();

        /// <summary>
        /// Draws this instance in victory dance.
        /// </summary>
        public abstract void drawVictory();

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

        public void updateTimeStamp()
        {
            if( timestamp != -1 )
                timestamp = DateTime.Now.Ticks;
        }
        public bool decreaseHealth()
        {
            health--;
            if (health <= 0)
            {
                timestamp = -1;
                return true;
            }
            return false;
        }
        public void destroyTimeStamp()
        {
            timestamp = -1;
        }

		#endregion Methods 
    }
}
