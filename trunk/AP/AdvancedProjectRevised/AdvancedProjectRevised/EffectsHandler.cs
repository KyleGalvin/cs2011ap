using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace AP
{
    /// <summary>
    /// Handles the effects
    /// Contributors: Adam Humeniuk, Todd Burton, Mike Rioux
    /// Revision: 214
    /// </summary>
    public class EffectsHandler
    {
		#region Fields (5) 

        List<int> bloodTextureNumber = new List<int>();
        List<int> bloodTimeLeft = new List<int>();
        List<float> bloodXPos = new List<float>();
        List<float> bloodYPos = new List<float>();
        Random rand = new Random();

		#endregion Fields 

		#region Constructors (1) 

        public EffectsHandler()
        {
        }

		#endregion Constructors 

		#region Methods (3) 

		// Public Methods (3) 

        /// <summary>
        /// Adds the blood.
        /// </summary>
        /// <param name="xPos">The x pos.</param>
        /// <param name="yPos">The y pos.</param>
        public void addBlood(float xPos, float yPos)
        {
            bloodXPos.Add(xPos);
            bloodYPos.Add(yPos);
            bloodTimeLeft.Add(100);
            bloodTextureNumber.Add(ClientProgram.loadedBloodTexture + rand.Next(0, 4));
        }

        /// <summary>
        /// Draws the effects.
        /// </summary>
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

        /// <summary>
        /// Updates the effects.
        /// </summary>
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

		#endregion Methods 
    }
}
