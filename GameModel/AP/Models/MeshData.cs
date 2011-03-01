using System;
using System.Collections.Generic;
using System.Text;


namespace AP
{
    /// <summary>
    /// A class containing all the necessary data for a mesh: Points, normal vectors, UV coordinates,
    /// and indices into each.
    /// Regardless of how the mesh file represents geometry, this is what we load it into,
    /// because this is most similar to how OpenGL represents geometry.
    /// Sources: http://www.opentk.com/files/ObjMeshLoader.cs, OOGL (MS3D), Icarus (Colladia)
    /// </summary>
    public class MeshData
    {
		#region Fields (4) 

        public Vector33[] Normals;
        public Vector22[] TexCoords;
        public Tri[] Tris;
        public Vector33[] Vertices;

		#endregion Fields 

		#region Constructors (1) 

        public MeshData(Vector33[] vert, Vector33[] norm, Vector22[] tex, Tri[] tri)
        {
            Vertices = vert;
            TexCoords = tex;
            Normals = norm;
            Tris = tri;

            Verify();
        }

		#endregion Constructors 

		#region Methods (8) 

		// Public Methods (7) 

        // XXX: Might technically be incorrect, since a (malformed) file could have vertices
        // that aren't actually in any face.
        // XXX: Don't take the names of the out parameters too literally...
        public void Dimensions(out double width, out double length, out double height)
        {
            double maxx, minx, maxy, miny, maxz, minz;
            maxx = maxy = maxz = minx = miny = minz = 0;
            foreach (Vector33 vert in Vertices)
            {
                if (vert.X > maxx) maxx = vert.X;
                if (vert.Y > maxy) maxy = vert.Y;
                if (vert.Z > maxz) maxz = vert.Z;
                if (vert.X < minx) minx = vert.X;
                if (vert.Y < miny) miny = vert.Y;
                if (vert.Z < minz) minz = vert.Z;
            }
            width = maxx - minx;
            length = maxy - miny;
            height = maxz - minz;
        }

        public double[] NormalArray()
        {
            double[] norms = new double[Normals.Length * 3];
            for (int i = 0; i < Normals.Length; i++)
            {
                norms[i * 3] = Normals[i].X;
                norms[i * 3 + 1] = Normals[i].Y;
                norms[i * 3 + 2] = Normals[i].Z;
            }

            return norms;
        }

        // OpenGL's vertex buffers use the same index to refer to vertices, normals and floats,
        // and just duplicate data as necessary.  So, we do the same.
        // XXX: This... may or may not be correct, and is certainly not efficient.
        // But when in doubt, use brute force.
        public void OpenGLArrays(out float[] verts, out float[] norms, out float[] texcoords, out uint[] indices)
        {
            Point[] points = Points();
            verts = new float[points.Length * 3];
            norms = new float[points.Length * 3];
            texcoords = new float[points.Length * 2];
            indices = new uint[points.Length];

            for (uint i = 0; i < points.Length; i++)
            {
                Point p = points[i];
                verts[i * 3] = (float)Vertices[p.Vertex].X;
                verts[i * 3 + 1] = (float)Vertices[p.Vertex].Y;
                verts[i * 3 + 2] = (float)Vertices[p.Vertex].Z;

                norms[i * 3] = (float)Normals[p.Normal].X;
                norms[i * 3 + 1] = (float)Normals[p.Normal].Y;
                norms[i * 3 + 2] = (float)Normals[p.Normal].Z;

                texcoords[i * 2] = (float)TexCoords[p.TexCoord].X;
                texcoords[i * 2 + 1] = (float)TexCoords[p.TexCoord].Y;

                indices[i] = i;
            }
        }

        public double[] TexcoordArray()
        {
            double[] tcs = new double[TexCoords.Length * 2];
            for (int i = 0; i < TexCoords.Length; i++)
            {
                tcs[i * 3] = TexCoords[i].X;
                tcs[i * 3 + 1] = TexCoords[i].Y;
            }

            return tcs;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("Vertices:");
            foreach (Vector33 v in Vertices)
            {
                s.AppendLine(v.ToString());
            }

            s.AppendLine("Normals:");
            foreach (Vector33 n in Normals)
            {
                s.AppendLine(n.ToString());
            }
            s.AppendLine("TexCoords:");
            foreach (Vector22 t in TexCoords)
            {
                s.AppendLine(t.ToString());
            }
            s.AppendLine("Tris:");
            foreach (Tri t in Tris)
            {
                s.AppendLine(t.ToString());
            }
            return s.ToString();
        }

        public void Verify()
        {
            foreach (Tri t in Tris)
            {
                foreach (Point p in t.Points())
                {
                    if (p.Vertex >= Vertices.Length)
                    {
                        string message = String.Format("Vertex {0} >= length of vertices {1}", p.Vertex, Vertices.Length);
                        throw new IndexOutOfRangeException(message);
                    }
                    if (p.Normal >= Normals.Length)
                    {
                        string message = String.Format("Normal {0} >= number of normals {1}", p.Normal, Normals.Length);
                        throw new IndexOutOfRangeException(message);
                    }
                    if (p.TexCoord > TexCoords.Length)
                    {
                        string message = String.Format("TexCoord {0} > number of texcoords {1}", p.TexCoord, TexCoords.Length);
                        throw new IndexOutOfRangeException(message);
                    }
                }
            }
        }

        public double[] VertexArray()
        {
            double[] verts = new double[Vertices.Length * 3];
            for (int i = 0; i < Vertices.Length; i++)
            {
                verts[i * 3] = Vertices[i].X;
                verts[i * 3 + 1] = Vertices[i].Y;
                verts[i * 3 + 2] = Vertices[i].Z;
            }

            return verts;
        }
		// Private Methods (1) 

        /*
        public void IndexArrays(out int[] verts, out int[] norms, out int[] texcoords) {
            List<int> v = new List<int>();
            List<int> n = new List<int>();
            List<int> t = new List<int>();
            foreach(Face f in Faces) {
                foreach(Point p in f.Points) {
                    v.Add(p.Vertex);
                    n.Add(p.Normal);
                    t.Add(p.TexCoord);
                }
            }
            verts = v.ToArray();
            norms = n.ToArray();
            texcoords = t.ToArray();
        }
        */
        private Point[] Points()
        {
            List<Point> points = new List<Point>();
            foreach (Tri t in Tris)
            {
                points.Add(t.P1);
                points.Add(t.P2);
                points.Add(t.P3);
            }
            return points.ToArray();
        }

		#endregion Methods 
    }
    public struct Vector22
    {
		#region Data Members (2) 

        public double X;
        public double Y;

		#endregion Data Members 

		#region Methods (2) 

        public override string ToString() { return String.Format("<{0},{1}>", X, Y); }

        public Vector22(double x, double y)
        {
            X = x;
            Y = y;
        }

		#endregion Methods 
    }
    public struct Vector33
    {
		#region Data Members (3) 

        public double X;
        public double Y;
        public double Z;

		#endregion Data Members 

		#region Methods (2) 

        public override string ToString() { return String.Format("<{0},{1},{2}>", X, Y, Z); }

        public Vector33(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

		#endregion Methods 
    }
    public struct Point
    {
		#region Data Members (3) 

        public int Normal;
        public int TexCoord;
        public int Vertex;

		#endregion Data Members 

		#region Methods (2) 

        public Point(int v, int n, int t)
        {
            Vertex = v;
            Normal = n;
            TexCoord = t;
        }

        public override string ToString() { return String.Format("Point: {0},{1},{2}", Vertex, Normal, TexCoord); }

		#endregion Methods 
    }

    public class Tri
    {
        public Point P1, P2, P3;
        public Tri()
        {
            P1 = new Point();
            P2 = new Point();
            P3 = new Point();
        }
        public Tri(Point a, Point b, Point c)
        {
            P1 = a;
            P2 = b;
            P3 = c;
        }

        public Point[] Points()
        {
            return new Point[3] { P1, P2, P3 };
        }

        public override string ToString() { return String.Format("Tri: {0}, {1}, {2}", P1, P2, P3); }
    }
}