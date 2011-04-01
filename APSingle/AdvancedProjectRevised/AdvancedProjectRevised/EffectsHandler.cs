using System;
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

        public EffectsHandler()
        {
        }

        public void addBlood(float xPos, float yPos)
        {
            bloodXPos.Add(xPos);
            bloodYPos.Add(yPos);
            bloodTimeLeft.Add(100);
            bloodTextureNumber.Add(ClientProgram.loadedBloodTexture + rand.Next(0, 4));
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
        }
    }
}
