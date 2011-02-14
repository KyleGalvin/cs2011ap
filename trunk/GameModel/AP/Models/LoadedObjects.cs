using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
//using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace AP
{
    class LoadedObjects : Loader
    {
        List<MeshData> m = new List<MeshData>();
        uint dataBuffer;
        uint indexBuffer;
        List<uint> tex = new List<uint>();

        int vertOffset, normOffset, texcoordOffset, inds;

        public LoadedObjects()
        {
        }

        public int LoadObject(String objPath, String texPath, float scale)
        {
            m.Add(LoadFile(objPath, scale));
            tex.Add(LoadTex(texPath));

            return m.Count - 1;
        }

        public void UnloadObject(int objNumber)
        {
            m.RemoveAt(objNumber);
            tex.RemoveAt(objNumber);
        }

        public void DrawObject(int objNumber)
        {
            LoadBuffers(objNumber);
            DrawBuffer(objNumber);
        }

        void LoadBuffers(int objNumber)
        {
            float[] verts, norms, texcoords;
            uint[] indices;
            m[objNumber].OpenGLArrays(out verts, out norms, out texcoords, out indices);
            inds = indices.Length;
            GL.GenBuffers(1, out dataBuffer);
            GL.GenBuffers(1, out indexBuffer);


            // Set up data for VBO.
            // We're going to use one VBO for all geometry, and stick it in 
            // in (VVVVNNNNCCCC) order.  Non interleaved.
            int buffersize = (verts.Length + norms.Length + texcoords.Length);
            float[] bufferdata = new float[buffersize];
            vertOffset = 0;
            normOffset = verts.Length;
            texcoordOffset = (verts.Length + norms.Length);

            verts.CopyTo(bufferdata, vertOffset);
            norms.CopyTo(bufferdata, normOffset);
            texcoords.CopyTo(bufferdata, texcoordOffset);

            for (int i = texcoordOffset; i < bufferdata.Length; i++)
            {
                if (i % 2 == 1) bufferdata[i] = 1 - bufferdata[i];
            }

            // Load geometry data
            GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(buffersize * sizeof(float)), bufferdata,
                          BufferUsageHint.StaticDraw);

            // Load index data
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer,
                          (IntPtr)(inds * sizeof(uint)), indices, BufferUsageHint.StaticDraw);
        }

        void DrawBuffer(int objNumber)
        {
            // Push current Array Buffer state so we can restore it later
            GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

            GL.ClientActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex[objNumber]);

            GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer);
            // Normal buffer
            GL.NormalPointer(NormalPointerType.Float, 0, (IntPtr)(normOffset * sizeof(float)));

            // TexCoord buffer
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)(texcoordOffset * sizeof(float)));

            // Vertex buffer
            GL.VertexPointer(3, VertexPointerType.Float, 0, (IntPtr)(vertOffset * sizeof(float)));

            // Index array
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            //GL.DrawElements(BeginMode.Triangles, m.Tris.Length * 3, DrawElementsType.UnsignedInt, 0);
            GL.DrawElements(BeginMode.Triangles, inds, DrawElementsType.UnsignedInt, 0);

            // Restore the state
            GL.PopClientAttrib();
        }
    }
}
