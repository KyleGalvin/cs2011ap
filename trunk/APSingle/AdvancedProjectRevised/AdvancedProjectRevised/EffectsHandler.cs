﻿using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace AP
{
    public class EffectsHandler
    {
        List<float> bloodXPos = new List<float>();
        List<float> bloodYPos = new List<float>();
        List<int> bloodTextureNumber = new List<int>();
        List<int> bloodTimeLeft = new List<int>();
        Random rand = new Random();

        public bool shakeScreen = false;
        private bool incXShake = true;
        private float xShake = 0;
        private bool incYShake = true;
        private float yShake = 0;

        public bool bossSpawned = false;
        private float bossHeight = 0;

        private bool closingMouth = true;
        private int mouthVal = 0;
        private float eyeRotation = 0;
        Zombie boss; //zombie class used to store where the boss is and access the draw method for lowering on death

        public EffectsHandler()
        {
            boss = new Zombie(23490);
            //set to the boss'sesess's's's's position
            boss.changeSubtype(Zombie.BOSS);
            boss.xPos = 0;
            boss.yPos = 10;
        }

        public void updateBossEyes(float playerX)
        {
            float hold = playerX;

            playerX = Math.Abs(playerX);
            eyeRotation = playerX * 5 + 5;
            
            if (hold < 0)
                eyeRotation *= -1;
        }

        public void raiseBoss()
        {
            if (bossSpawned)
            if (!ClientProgram.bossKilled)
            {
                if (bossHeight < 6)
                {
                    shakeScreen = true;
                    bossHeight += 0.05f;
                }
                else
                    shakeScreen = false;
            }
            else
            {
                if (bossHeight > -2)
                {
                    shakeScreen = true;
                    bossHeight -= 0.05f;
                }
                else
                    shakeScreen = false;
            }

        }

        public void translateForBoss()
        {
            GL.Translate(0, 0, bossHeight - 9);
        }

        public void shakeTheScreen()
        {
            if (shakeScreen)
            {
                if (incXShake)
                {
                    xShake += 0.2f;
                    if (xShake >= 0.4f)
                        incXShake = false;
                }
                else
                {
                    xShake -= 0.2f;
                    if (xShake <= -0.4f)
                        incXShake = true;
                }

                if (incYShake)
                {
                    yShake += 0.15f;
                    if (yShake >= 0.4f)
                        incYShake = false;
                }
                else
                {
                    yShake -= 0.15f;
                    if (yShake <= -0.4f)
                        incYShake = true;
                }
                GL.Translate(xShake, yShake, 0);
            }
        }

        public void addBlood(float xPos, float yPos)
        {
            bloodXPos.Add(xPos);
            bloodYPos.Add(yPos);
            bloodTimeLeft.Add(100);
            bloodTextureNumber.Add(ClientProgram.loadedBloodTexture + rand.Next(0, 8));
        }

        public void updateEffects()
        {
            int bloodToRemove = -1;
            for (int i = 0; i < bloodTimeLeft.Count; i++)
            {
                bloodTimeLeft[i]--;
                if (bloodTimeLeft[i] <= 0)
                    bloodToRemove = i;
            }
            if (bloodToRemove != -1)
            {
                bloodXPos.RemoveAt(bloodToRemove);
                bloodYPos.RemoveAt(bloodToRemove);
                bloodTextureNumber.RemoveAt(bloodToRemove);
                bloodTimeLeft.RemoveAt(bloodToRemove);
            }

            raiseBoss();

            if (closingMouth)
            {
                mouthVal++;
                if (mouthVal == 4)
                    closingMouth = false;
            }
            else
            {
                mouthVal--;
                if (mouthVal == 0)
                    closingMouth = true;
            }
        }

        public void drawEffects()
        {
            for (int i = 0; i < bloodXPos.Count; i++)
            {
                GL.Color4(1.0f, 1.0f, 1.0f, ((float)bloodTimeLeft[i] / 100.0f));
                GL.PushMatrix();
                GL.Translate(bloodXPos[i], bloodYPos[i], 0.01f);
                ClientProgram.loadedObjects.DrawObject(bloodTextureNumber[i]);
                GL.PopMatrix();
            }

            if (ClientProgram.bossKilled)
            {
                GL.PushMatrix();
                translateForBoss();
                boss.draw();
                GL.PopMatrix();
            }

            if (bossSpawned)
            {
                GL.Color3(1.0f, 1.0f, 1.0f);
                GL.PushMatrix();
                GL.Translate(0, 10, 0.01f);
                ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedCrackedGroundTexture);                
                GL.Translate(0, 0, 6);
                GL.Translate(0, -1.1f, 0);
                translateForBoss();
                GL.PushMatrix();
                GL.Rotate(90, 1.0f, 0, 0);
                ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedBossMouth + mouthVal);
                GL.PopMatrix();

                GL.PushMatrix();
                GL.Translate(-1, 0, 2);
                GL.Rotate(90, 1.0f, 0, 0);
                GL.Rotate(eyeRotation, 0, 0, 1.0f);
                ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedBossEye);
                GL.PopMatrix();

                GL.PushMatrix();
                GL.Translate(1, 0, 2);
                GL.Rotate(90, 1.0f, 0, 0);
                GL.Rotate(eyeRotation, 0, 0, 1.0f);
                ClientProgram.loadedObjects.DrawObject(ClientProgram.loadedBossEye);
                GL.PopMatrix();

                GL.PopMatrix();
            }
        }
    }
}