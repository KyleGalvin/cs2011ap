using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using AP.Forms;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.Net;

namespace AP
{
    class Program
    {
        static void Main(string[] args)
        {
            //start the form for log in screen
            // if client
            // - create player object and send to server
            // if server
            // - get client info
            //Form1 form = new Form1();

            Console.WriteLine("[s]erver or [c]lient");
            string val = Console.ReadLine();
            if (val == "s")
            {
                ServerProgram server = new ServerProgram();
            }
            else if (val == "c")
            {
                using (ClientProgram client = new ClientProgram())
                {
                    client.Run(28.0);
                }
            }
        }
    }
}
