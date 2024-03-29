﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace AP
{
    /// <summary>
    /// Collection of objects that represent the game.
    /// Contributors: Scott Herman, Kyle Galvin
    /// Revision: 236
    /// </summary>
    public class GameState
    {
		#region Fields (4) 

        public List<AP.Bullet> Bullets;
        public List<AP.Enemy> Enemies;
        private int enemyIDs = 4;
        public List<AP.Player> Players;
        public List<AP.Crate> Crates;
        public int myUID;
		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see cref="GameState"/> class.
        /// </summary>
        public GameState()
        {
            Players = new List<AP.Player>();
            Enemies = new List<AP.Enemy>();
            Bullets = new List<AP.Bullet>();
            Crates = new List<AP.Crate>();
        }

		#endregion Constructors

		#region Methods (2)

		// Public Methods (2)

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return Players.Count + Enemies.Count + Bullets.Count;
        }

        /// <summary>
        /// Gets the enemy UID.
        /// </summary>
        /// <returns></returns>
        public int getEnemyUID()
        {
            return (enemyIDs++);
        }

        /// <summary>
        /// Timestamps the specified obj.
        /// </summary>
        /// <param name="Obj">The obj.</param>
        public void Timestamp(Position Obj)
        {
            DateTime t = DateTime.Now;
            Obj.timestamp = t.Ticks;
        }

		#endregion Methods 
    }
}
