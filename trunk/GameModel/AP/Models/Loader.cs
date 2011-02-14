using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace AP
{
    class Loader
    {       
        public MeshData LoadStream(Stream stream, float scale)
        {
            StreamReader reader = new StreamReader(stream);
            List<Vector33> points = new List<Vector33>();
            List<Vector33> normals = new List<Vector33>();
            List<Vector22> texCoords = new List<Vector22>();
            List<Tri> tris = new List<Tri>();
            string line;
            char[] splitChars = { ' ' };
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim(splitChars);
                line = line.Replace("  ", " ");

                string[] parameters = line.Split(splitChars);

                switch (parameters[0])
                {
                    case "p": // Point
                        break;

                    case "v": // Vertex
                        float x = float.Parse(parameters[1]) * scale;
                        float y = float.Parse(parameters[2]) * scale;
                        float z = float.Parse(parameters[3]) * scale;
                        points.Add(new Vector33(x, y, z));
                        break;

                    case "vt": // TexCoord
                        float u = float.Parse(parameters[1]);
                        float v = float.Parse(parameters[2]);
                        texCoords.Add(new Vector22(u, v));
                        break;

                    case "vn": // Normal
                        float nx = float.Parse(parameters[1]);
                        float ny = float.Parse(parameters[2]);
                        float nz = float.Parse(parameters[3]);
                        normals.Add(new Vector33(nx, ny, nz));
                        break;

                    case "f": // Face
                        tris.AddRange(parseFace(parameters));
                        break;
                }
            }

            Vector33[] p = points.ToArray();
            Vector22[] tc = texCoords.ToArray();
            Vector33[] n = normals.ToArray();
            Tri[] f = tris.ToArray();

            return new MeshData(p, n, tc, f);
        }

        public MeshData LoadFile(string file, float scale)
        {
            // Silly me, using() closes the file automatically.
            using (FileStream s = File.Open(file, FileMode.Open))
            {
                return LoadStream(s, scale);
            }
        }

        public uint LoadTex(string file)
        {
            Bitmap bitmap = new Bitmap(file);

            uint texture;
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return texture;
        }

        private static Tri[] parseFace(string[] indices)
        {
            Point[] p = new Point[indices.Length - 1];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = parsePoint(indices[i + 1]);
            }
            return Triangulate(p);
            //return new Face(p);
        }

        // Takes an array of points and returns an array of triangles.
        // The points form an arbitrary polygon.
        private static Tri[] Triangulate(Point[] ps)
        {
            List<Tri> ts = new List<Tri>();
            if (ps.Length < 3)
            {
                throw new Exception("Invalid shape!  Must have >2 points");
            }

            Point lastButOne = ps[1];
            Point lastButTwo = ps[0];
            for (int i = 2; i < ps.Length; i++)
            {
                Tri t = new Tri(lastButTwo, lastButOne, ps[i]);
                lastButOne = ps[i];
                lastButTwo = ps[i - 1];
                ts.Add(t);
            }
            return ts.ToArray();
        }

        private static Point parsePoint(string s)
        {
            char[] splitChars = { '/' };
            string[] parameters = s.Split(splitChars);
            int vert = int.Parse(parameters[0]) - 1;
            int tex = int.Parse(parameters[1]) - 1;
            int norm = int.Parse(parameters[2]) - 1;
            return new Point(vert, norm, tex);
        }
    }
}
