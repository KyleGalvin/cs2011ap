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
        List<uint> dataBuffer = new List<uint>();
        List<uint> indexBuffer = new List<uint>();
        List<uint> tex = new List<uint>();

        List<int> vertOffset = new List<int>();
        List<int> normOffset = new List<int>();
        List<int> texcoordOffset = new List<int>();
        List<int> inds = new List<int>();

        public LoadedObjects()
        {
        }

        public int LoadObject(String objPath, String texPath, float scale)
        {
            m.Add(LoadFile(objPath, scale));
            tex.Add(LoadTex(texPath));
            LoadBuffers(m.Count - 1);
            return m.Count - 1;
        }

        public void UnloadObject(int objNumber)
        {
            m.RemoveAt(objNumber);
            tex.RemoveAt(objNumber);
            dataBuffer.RemoveAt(objNumber);
            indexBuffer.RemoveAt(objNumber);
            vertOffset.RemoveAt(objNumber);
            normOffset.RemoveAt(objNumber);
            texcoordOffset.RemoveAt(objNumber);
            inds.RemoveAt(objNumber);
        }

        public void DrawObject(int objNumber)
        {
            DrawBuffer(objNumber);
        }

        void LoadBuffers(int objNumber)
        {
            float[] verts, norms, texcoords;
            uint[] indices;
            m[objNumber].OpenGLArrays(out verts, out norms, out texcoords, out indices);
            inds.Add(indices.Length);
            uint outDataBuffer, outIndexBuffer;
            GL.GenBuffers(1, out outDataBuffer);
            GL.GenBuffers(1, out outIndexBuffer);
            dataBuffer.Add(outDataBuffer);
            indexBuffer.Add(outIndexBuffer);

            // Set up data for VBO.
            // We're going to use one VBO for all geometry, and stick it in 
            // in (VVVVNNNNCCCC) order.  Non interleaved.
            int buffersize = (verts.Length + norms.Length + texcoords.Length);
            float[] bufferdata = new float[buffersize];
            vertOffset.Add(0);
            normOffset.Add(verts.Length);
            texcoordOffset.Add(verts.Length + norms.Length);

            verts.CopyTo(bufferdata, vertOffset[objNumber]);
            norms.CopyTo(bufferdata, normOffset[objNumber]);
            texcoords.CopyTo(bufferdata, texcoordOffset[objNumber]);

            for (int i = texcoordOffset[objNumber]; i < bufferdata.Length; i++)
            {
                if (i % 2 == 1) bufferdata[i] = 1 - bufferdata[i];
            }

            // Load geometry data
            GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer[objNumber]);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(buffersize * sizeof(float)), bufferdata,
                          BufferUsageHint.StaticDraw);

            // Load index data
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer[objNumber]);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer,
                          (IntPtr)(inds[objNumber] * sizeof(uint)), indices, BufferUsageHint.StaticDraw);
        }

        void DrawBuffer(int objNumber)
        {
            // Push current Array Buffer state so we can restore it later
            GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

            GL.ClientActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex[objNumber]);

            GL.BindBuffer(BufferTarget.ArrayBuffer, dataBuffer[objNumber]);
            // Normal buffer
            GL.NormalPointer(NormalPointerType.Float, 0, (IntPtr)(normOffset[objNumber] * sizeof(float)));

            // TexCoord buffer
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)(texcoordOffset[objNumber] * sizeof(float)));

            // Vertex buffer
            GL.VertexPointer(3, VertexPointerType.Float, 0, (IntPtr)(vertOffset[objNumber] * sizeof(float)));

            // Index array
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer[objNumber]);
            //GL.DrawElements(BeginMode.Triangles, m.Tris.Length * 3, DrawElementsType.UnsignedInt, 0);
            GL.DrawElements(BeginMode.Triangles, inds[objNumber], DrawElementsType.UnsignedInt, 0);

            // Restore the state
            GL.PopClientAttrib();
        }
    }
}
