using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AP
{
    public class OpenLevel
    {
        // Read a document
        XmlTextReader textReader;
        public OpenLevel( int levelNum )
        {
            switch( levelNum )
            {
                case 1:
                    textReader = new XmlTextReader("level1.xml");
                    break;
                default:
                    break;
            }
        }

        public void parseFile()
        {
            while (textReader.Read())
            {
                XmlNodeType nType = textReader.NodeType;
                textReader.MoveToElement();
                Console.WriteLine("XmlTextReader Properties Test");
                Console.WriteLine("===================");
                // Read this element's properties and display them on console
                Console.WriteLine("Name:" + textReader.Name);
                Console.WriteLine("Base URI:" + textReader.BaseURI);
                Console.WriteLine("Local Name:" + textReader.LocalName);
                Console.WriteLine("Attribute Count:" + textReader.AttributeCount.ToString());
                Console.WriteLine("Depth:" + textReader.Depth.ToString());
                Console.WriteLine("Line Number:" + textReader.LineNumber.ToString());
                Console.WriteLine("Node Type:" + textReader.NodeType.ToString());
                Console.WriteLine("Attribute Count:" + textReader.Value.ToString());
            }
        }
    }
}
