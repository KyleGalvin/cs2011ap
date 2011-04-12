using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace AP
{
    /// <summary>
    /// Controls the spawning of enemies
    /// Contributors: Scott Herman, Gage Patterson
    /// Revision: 248
    /// </summary>
    class EnemySpawn : Position
    {
		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemySpawn"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public EnemySpawn(float x, float y)
        {
            xPos = x;
            yPos = y;
        }

		#endregion Constructors 

		#region Methods (2) 

		// Public Methods (2) 

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public void draw()
        {
            // OpenGL Calls
            float colorR = 1.0f;
            float colorG = 1.0f;
            float colorB = 0.0f;
            float radius = 0.1f;
            GL.Begin(BeginMode.Polygon);

            GL.Color3(colorR, colorG, colorB);
            GL.Vertex3(xPos - radius, yPos, 4.0f);
            GL.Vertex3(xPos - radius * 0.7, yPos + radius * 0.7, 4.0f);
            GL.Vertex3(xPos, yPos + radius, 4.0f);
            GL.Vertex3(xPos + radius * 0.7, yPos + radius * 0.7, 4.0f);
            GL.Vertex3(xPos + radius, yPos, 4.0f);
            GL.Vertex3(xPos + radius * 0.7, yPos - radius * 0.7, 4.0f);
            GL.Vertex3(xPos, yPos - radius, 4.0f);
            GL.Vertex3(xPos - radius * 0.7, yPos - radius * 0.7, 4.0f);

            GL.End();
        }

        /// <summary>
        /// Spawns the enemy.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public Enemy spawnEnemy(int ID)
        {
            Enemy enemy = new Zombie(ID);
            
            enemy.setPosition(xPos, yPos);
            //enemy.draw();

            return enemy;
        }

		#endregion Methods 
    }
}
