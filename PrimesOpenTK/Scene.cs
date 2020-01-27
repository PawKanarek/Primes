using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimesOpenTK
{
    /// <summary>
    /// Based on https://github.com/opentk/LearnOpenTK/blob/master/Chapter%201/2%20-%20Hello%20Triangle/Window.cs 
    /// </summary>
    public class Scene : GameWindow
    {

        private Shader shader;
        private int vertexBufferObject;
        private int vertexArrayObject;
        private int elementBufferObject;

        // x, y, z
        private readonly float[] verticles =
        {
            .5f,  .5f, .0f,  // Top-Right Vortex
            .5f, -.5f, .0f,  // Bottom-Right Vortex
           -.5f, -.5f, .0f,  // Bottom-Left Vortex
           -.5f,  .5f, .0f,  // Top-Left Vortex
        };

        private readonly uint[] indices = {  // note that we start from 0!
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
        };

        public Scene(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            this.vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, this.verticles.Length * sizeof(float), this.verticles, BufferUsageHint.StaticDraw);

            this.elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, this.indices.Length * sizeof(uint), this.indices, BufferUsageHint.StaticDraw);

            this.shader = new Shader("shader.vert", "shader.frag");
            this.shader.Use();

            this.vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(this.vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.elementBufferObject);

            GL.VertexAttribPointer(this.shader.GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            this.shader.Use();
            GL.BindVertexArray(this.vertexArrayObject);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            this.Context.SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);

            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(this.vertexBufferObject);
            GL.DeleteVertexArray(this.vertexArrayObject);
            this.shader.Dispose();

            base.OnUnload(e);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState inputState = Keyboard.GetState();
            if (inputState.IsKeyDown(Key.Escape))
            {
                this.Exit();
            }

            base.OnUpdateFrame(e);
        }
    }
}
