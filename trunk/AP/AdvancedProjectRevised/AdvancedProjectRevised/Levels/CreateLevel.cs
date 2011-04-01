using System;
using System.Collections.Generic;
using System.Xml;

namespace AP
{
    /// <summary>
    /// Handles the level creation
    /// </summary>
    public class CreateLevel
    {
		#region Fields (3) 

        // Read a document
        XmlDocument doc = new XmlDocument();
         XmlNodeList nodes;
        XmlElement root;

		#endregion Fields 

		#region Enums (1) 

        private enum objTypes { wall };

		#endregion Enums 

		#region Constructors (1) 

         public CreateLevel( int levelNum )
        {
            switch( levelNum )
            {
                case 1:
                    doc.Load("level1.xml");
                    break;
                case 2:
                    doc.Load("level2.xml");
                    break;
                default:
                    break;
            }
        }

		#endregion Constructors 

		#region Methods (1) 

		// Public Methods (1) 

         /// <summary>
         /// Parses the file.
         /// </summary>
         /// <param name="x">The x.</param>
         /// <param name="y">The y.</param>
         /// <param name="h">The height.</param>
         /// <param name="w">The width.</param>
         /// <param name="xSpawn">The x spawn.</param>
         /// <param name="ySpawn">The y spawn.</param>
        public void parseFile(ref List<int> x, ref List<int> y, ref List<int> h, ref List<int> w, ref List<int> xSpawn, ref List<int> ySpawn, ref List<int> xPlayerSpawn, ref List<int> yPlayerSpawn, ref List<int> playerSpawnID)
        {
            Console.WriteLine("Creating level...");
            root = doc.DocumentElement;

            nodes = root.SelectNodes("objects/spawnObject");
            foreach (XmlNode node in nodes)
            {
                xSpawn.Add(Convert.ToInt32(node["spawn"].GetAttribute("x")));
                ySpawn.Add(Convert.ToInt32(node["spawn"].GetAttribute("y")));
            }

            nodes = root.SelectNodes("objects/playerSpawnObject");
            foreach (XmlNode node in nodes)
            {
                xPlayerSpawn.Add(Convert.ToInt32(node["playerSpawn"].GetAttribute("x")));
                yPlayerSpawn.Add(Convert.ToInt32(node["playerSpawn"].GetAttribute("y")));
                playerSpawnID.Add(Convert.ToInt32(node["playerSpawn"].GetAttribute("id")));
            }

            nodes = root.SelectNodes("objects/wallObject");
            foreach (XmlNode node in nodes)
            {
                x.Add(Convert.ToInt32(node["wall"].GetAttribute("x")));
                y.Add(Convert.ToInt32(node["wall"].GetAttribute("y")));
                h.Add(Convert.ToInt32(node["wall"].GetAttribute("height")));
                w.Add(Convert.ToInt32(node["wall"].GetAttribute("width")));
            }
        }

		#endregion Methods 
    }
}
