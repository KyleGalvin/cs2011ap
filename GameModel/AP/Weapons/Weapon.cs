using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace AP
{
    public class Weapon : Position
    {
        private int bulletCooldown;
        private bool rifleAvailable = false;
        private bool rocketAvailable = false;
        private bool shotgunAvailable = false;
        private bool grenadeAvailable = false;
        private bool rifleEquipped = false;
        private bool rocketEquipped = false;
        private bool pistolEquipped = false;
        private bool shotgunEquipped = false;
        private UInt16 rifleBulletCount = 0;
        private UInt16 rocketBulletCount = 0;
        private UInt16 shotgunBulletCount = 0;
        private UInt16 grenadeCount = 3;
        Vector3 defaultVelocity = new Vector3(0, 0, 0);

        public Weapon()
        {
            pistolEquipped = true;
            bulletCooldown = 0;
        }

        public void updateBulletCooldown()
        {
            bulletCooldown--;
        }

        public bool canShoot()
        {
            if (bulletCooldown <= 0)
            {
                bulletCooldown = 10;
                return true;
            }
            return false;
        }

        public void equipRifle()
        {
            rifleEquipped = true;
            pistolEquipped = false;
            rocketEquipped = false;
            shotgunEquipped = false;
        }
        public void equipRocket()
        {
            rifleEquipped = false;
            pistolEquipped = false;
            rocketEquipped = true;
            shotgunEquipped = false;
        }
        public void equipPistol()
        {
            rifleEquipped = false;
            pistolEquipped = true;
            rocketEquipped = false;
            shotgunEquipped = false;
        }
        public void equipShotgun()
        {
            rifleEquipped = false;
            pistolEquipped = false;
            rocketEquipped = false;
            shotgunEquipped = true;
        }

        public void rifleAttained()
        {
            if (rifleBulletCount != 100)
            {
                rifleBulletCount += 30;
                rifleAvailable = true;
            }
        }
        public void rocketAttained()
        {
            if (rocketBulletCount != 3)
            {
                rocketBulletCount++;
                rocketAvailable = true;
            }
        }
        public void shotgunAttained()
        {
            if (shotgunBulletCount != 36)
            {
                shotgunBulletCount = 12;
                shotgunAvailable = true;
            }
        }
        public void grenadeAttained()
        {
            if (grenadeCount != 3)
            {
                grenadeCount++;
                grenadeAvailable = true;
            }

        }
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
    }
}
