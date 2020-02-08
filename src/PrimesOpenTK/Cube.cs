using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrimesOpenTK
{
    public class Cube : IDisposable
    {
        private int vertexBufferObject;
        private int vertexArrayObject;
        private Texture texture0;
        private Shader shader;

        //// x, y, z
        //private static readonly float[] verticles = {
        //    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
        //     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
        //     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        //     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        //    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
        //    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

        //    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
        //     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
        //     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
        //     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
        //    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
        //    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

        //    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
        //    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        //    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
        //    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
        //    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
        //    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

        //     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
        //     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        //     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
        //     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
        //     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
        //     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

        //    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
        //     0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
        //     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
        //     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
        //    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
        //    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

        //    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
        //     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        //     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
        //     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
        //    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
        //    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        //};

        public static readonly float[] verticles = {
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f,
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f,
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f,
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
        };

        public List<float> mesh = new List<float>(verticles);

        public void CreateVao()
        {
            this.vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.Count* sizeof(float), mesh.ToArray(), BufferUsageHint.StaticDraw);

            this.texture0 = new Texture("Resources/tile.png");
            this.texture0.Use();

            this.shader = new Shader("shader.vert", "shader.frag");
            this.shader.Use();
            this.shader.SetInt("texture0", 0);

            this.vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(this.vertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexArrayObject);

            var vertexLocation = this.shader.GetAttribLocation("vertexCoord");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = this.shader.GetAttribLocation("textureCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        }

        public void RenderVao(Camera camera, double time, List<Vector3> primesCoordinates)
        {
            this.shader.Use();
            this.texture0.Use(TextureUnit.Texture0);

            //foreach (Vector3 item in primesCoordinates)
            //{
                Matrix4 model = Matrix4.Identity /** Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(time))*/ /** Matrix4.CreateTranslation(item)*/;
                this.shader.SetMatrix4("model", model);
                this.shader.SetMatrix4("view", camera.GetViewMatrix());
                this.shader.SetMatrix4("projection", camera.GetProjectionMatrix());
                GL.BindVertexArray(this.vertexArrayObject);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            //}

            //GL.DrawElements(BeginMode.TriangleStrip, primesCoordinates.Count, DrawElementsType.UnsignedInt, 0);
        }

        private bool disposed = false;
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.Cleanup();
            this.disposed = true;
        }

        protected void Cleanup()
        {
            GL.DeleteBuffer(this.vertexBufferObject);
            GL.DeleteVertexArray(this.vertexArrayObject);

            this.shader.Dispose();
            this.texture0.Dispose();
        }
    }
}
