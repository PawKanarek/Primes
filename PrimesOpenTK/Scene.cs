using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace PrimesOpenTK
{
    /// <summary>
    /// Based on https://github.com/opentk/LearnOpenTK/blob/master/Chapter%201/2%20-%20Hello%20Triangle/Window.cs 
    /// </summary>
    public class Scene : GameWindow
    {
        private Texture texture0;
        private Texture texture1;
        private Shader shader;
        private int vertexBufferObject;
        private int vertexArrayObject;
        private int elementBufferObject;

        private readonly float[] verticles =
        {
            // x, y, z          Texture coordinates
            -0.1f,  0.1f, 0.0f, 1.0f, 1.0f, // Left-Top Vortex
             0.1f,  0.1f, 0.0f, 1.0f, 0.0f, // Right-Top Vortex
             0.1f, -0.1f, 0.0f, 0.0f, 0.0f, // Right-Bottom Vortex
            -0.1f, -0.1f, 0.0f, 0.0f, 1.0f // Left-Bottom Vortex
        };

        //private readonly float[] verticles2 =
        //{
        //     .1f,  .1f, .0f, // Left-Top Vortex
        //     .2f,  .1f, .0f, // Right-Top Vortex
        //     .2f, -.1f, .0f, // Right-Bottom Vortex
        //    -.1f, -.1f, .0f, // Left-Bottom Vort
        //};

        private readonly uint[] indices = {
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

            this.texture0 = new Texture("Resources/container.png");
            this.texture0.Use();

            this.texture1 = new Texture("Resources/awesomeface.png");
            this.texture1.Use();

            this.shader.SetInt("texture0", 0);
            this.shader.SetInt("texture1", 1);

            this.vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(this.vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.elementBufferObject);

            var vertexLocation = this.shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = this.shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            var transform = Matrix4.Identity;
            
            transform *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(20f));

            transform *= Matrix4.CreateScale(1.1f);

            transform *= Matrix4.CreateTranslation(0.1f, 0.1f, 0.0f);


            texture0.Use(TextureUnit.Texture0);
            texture1.Use(TextureUnit.Texture1);
            this.shader.Use();
            this.shader.SetMatrix4("transform", transform);

            GL.BindVertexArray(this.vertexArrayObject);

            GL.DrawElements(PrimitiveType.Triangles, this.indices.Length, DrawElementsType.UnsignedInt, 0);
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
            this.texture0.Dispose();

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
