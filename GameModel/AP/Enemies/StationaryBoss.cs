using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AP
{
    public class StationaryBoss: Enemy
    {
        public StationaryBoss(int ID)
        {
            life = (int)Life.Boss;
            enemyID = ID;
        }

        public override void move(float x, float y)
        {
            //no need to move a stationary boss :)
        }

        public override void moveTowards(Player target)
        {
            //no need to move a stationary boss :)
        }

        public override void attack( Player player )
        {
            // insert weapon object attack method
            // i.e. pistolx2, rifle, rocket, etc
            player.loseHealth((float)Damage.High);
        }

        public override void draw()
        {
            // OpenGL Calls
        }
    }
}
