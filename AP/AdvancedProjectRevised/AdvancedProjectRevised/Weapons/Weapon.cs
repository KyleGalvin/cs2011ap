using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace AP
{
    /// <summary>
    /// The class that will keep track of weapons and ammunition for a player.
    /// Each player will have it's own instance of this class.
    /// </summary>
    public class Weapon : Position
    {
		#region Fields (14) 

        private int bulletCooldown;
        Vector3 defaultVelocity = new Vector3(0, 0, 0);
        private bool grenadeAvailable = false;
        private UInt16 grenadeCount = 3;
        private bool pistolEquipped = false;
        private bool rifleAvailable = false;
        private UInt16 rifleBulletCount = 0;
        private bool rifleEquipped = false;
        private bool rocketAvailable = false;
        private UInt16 rocketBulletCount = 0;
        private bool rocketEquipped = false;
        private bool shotgunAvailable = false;
        private UInt16 shotgunBulletCount = 0;
        private bool shotgunEquipped = false;

		#endregion Fields 

		#region Constructors (1) 

        public Weapon()
        {
            pistolEquipped = true;
            bulletCooldown = 0;
        }

		#endregion Constructors 

		#region Methods (11) 

		// Public Methods (11) 

        public void updateTimeStamp()
        {
            timestamp = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Determines whether this instance can shoot.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can shoot; otherwise, <c>false</c>.
        /// </returns>
        public bool canShoot()
        {
            if (bulletCooldown <= 0)
            {
                bulletCooldown = 10;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Equips the pistol.
        /// </summary>
        public void equipPistol()
        {
            rifleEquipped = false;
            pistolEquipped = true;
            rocketEquipped = false;
            shotgunEquipped = false;
        }

        /// <summary>
        /// Equips the rifle.
        /// </summary>
        public void equipRifle()
        {
            rifleEquipped = true;
            pistolEquipped = false;
            rocketEquipped = false;
            shotgunEquipped = false;
        }

        /// <summary>
        /// Equips the rocket.
        /// </summary>
        public void equipRocket()
        {
            rifleEquipped = false;
            pistolEquipped = false;
            rocketEquipped = true;
            shotgunEquipped = false;
        }

        /// <summary>
        /// Equips the shotgun.
        /// </summary>
        public void equipShotgun()
        {
            rifleEquipped = false;
            pistolEquipped = false;
            rocketEquipped = false;
            shotgunEquipped = true;
        }

        /// <summary>
        /// Called when grenades are attained
        /// </summary>
        public void grenadeAttained()
        {
            if (grenadeCount != 3)
            {
                grenadeCount++;
                grenadeAvailable = true;
            }

        }

        /// <summary>
        /// Called when rifle is attained
        /// </summary>
        public void rifleAttained()
        {
            if (rifleBulletCount != 100)
            {
                rifleBulletCount += 30;
                rifleAvailable = true;
            }
        }

        /// <summary>
        /// Called when rocket is attained
        /// </summary>
        public void rocketAttained()
        {
            if (rocketBulletCount != 3)
            {
                rocketBulletCount++;
                rocketAvailable = true;
            }
        }

        /// <summary>
        /// Shoots the specified bullet.
        /// </summary>
        /// <param name="bulletList">The bullet list.</param>
        /// <param name="player">The player.</param>
        /// <param name="screenX">The screen X.</param>
        /// <param name="screenY">The screen Y.</param>
        /// <param name="mouseX">The mouse X.</param>
        /// <param name="mouseY">The mouse Y.</param>
        public void shoot(ref List<Bullet> bulletList, ref Player player, int screenX, int screenY, float mouseX, float mouseY)
        {
            if(pistolEquipped)
            {
                bulletList.Add(new Bullet(player.position, defaultVelocity));
                bulletList.Last().setDirectionByMouse(mouseX, mouseY, screenX, screenY, ref player);
            } else if( rifleEquipped ) {
                bulletList.Add(new Bullet(player.position, defaultVelocity));
                bulletList.Last().setDirectionByMouse(mouseX, mouseY, screenX, screenY, ref player);
            } else if ( shotgunEquipped ) {
                bulletList.Add(new Bullet(player.position, defaultVelocity));
                bulletList.Last().setDirectionByMouse(mouseX, mouseY, screenX, screenY, ref player);
                bulletList.Add(new Bullet(player.position, defaultVelocity));
                bulletList.Last().setDirectionByMouse(mouseX, mouseY * 1.15f, screenX, screenY, ref player);
                bulletList.Add(new Bullet(player.position, defaultVelocity));
                bulletList.Last().setDirectionByMouse(mouseX, mouseY * 0.85f, screenX, screenY, ref player);
            } else if (rocketEquipped ) {
                bulletList.Add(new Bullet(player.position, defaultVelocity));
                bulletList.Last().setDirectionByMouse(mouseX, mouseY, screenX, screenY, ref player);
            }
        }

        /// <summary>
        /// Called when shotgun is attained
        /// </summary>
        public void shotgunAttained()
        {
            if (shotgunBulletCount != 36)
            {
                shotgunBulletCount = 12;
                shotgunAvailable = true;
            }
        }

        /// <summary>
        /// Updates the bullet cooldown.
        /// </summary>
        public void updateBulletCooldown()
        {
            bulletCooldown--;
        }

		#endregion Methods 
    }
}
