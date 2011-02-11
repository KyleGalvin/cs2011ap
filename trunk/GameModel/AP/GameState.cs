using System;
using System.Collections.Generic;
using System.Text;

namespace AP
{
    public class GameState
    {
        public GameState()
        {
            Players = new List<AP.Player>();
            Enemies = new List<AP.Enemy>();
            Bullets = new List<AP.Bullet>();
        }

        public int Count()
        {
            return Players.Count + Enemies.Count + Bullets.Count;
        }
        public List<AP.Player> Players;
        public List<AP.Enemy> Enemies;
        public List<AP.Bullet> Bullets;
        private int enemyIDs = 4;
        public int getEnemyUID()
        {
            return (enemyIDs++);
        }
    }
}
