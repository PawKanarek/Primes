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
        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;

        private double _time;
        private Matrix4 _view;
        private Matrix4 _projection;
        private Texture texture0;
        private Texture texture1;
        private Shader shader;
        private int vertexBufferObject;
        private int vertexArrayObject;
        //private int elementBufferObject;
        private readonly Vector3 VAO2pos = new Vector3(2f, 0, 0);
        // x, y, z
        float[] verticles = {
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f, //1
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f, //2
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f, //3
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f, //4
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f, //5 
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f, //6

            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        };

        //private readonly float[] verticles =
        //{
        //    // x, y, z          Texture coordinates
        //    -0.1f,  0.1f, 0.0f, 1.0f, 1.0f, // Left-Top Vortex
        //     0.1f,  0.1f, 0.0f, 1.0f, 0.0f, // Right-Top Vortex
        //     0.1f, -0.1f, 0.0f, 0.0f, 0.0f, // Right-Bottom Vortex
        //    -0.1f, -0.1f, 0.0f, 0.0f, 1.0f // Left-Bottom Vortex
        //};

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
            GL.Enable(EnableCap.DepthTest);

            this.vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, this.verticles.Length * sizeof(float), this.verticles, BufferUsageHint.StaticDraw);

            // uncomment when using 2 d an indices
            //this.elementBufferObject = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.elementBufferObject);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, this.indices.Length * sizeof(uint), this.indices, BufferUsageHint.StaticDraw);

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

            var vertexLocation = this.shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);


            // uncomment when using 2 d an indices
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.elementBufferObject);



            var texCoordLocation = this.shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Width / (float)Height, 0.1f, 100.0f);


            _camera = new Camera(Vector3.UnitZ * 3, Width / (float)Height);
            //CursorVisible = false;

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {

            _time += 160 * e.Time;


            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            texture0.Use(TextureUnit.Texture0);
            texture1.Use(TextureUnit.Texture1);
            this.shader.Use();

            var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time));

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", _camera.GetViewMatrix());
            shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            GL.BindVertexArray(this.vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            this.shader.Use();

            var model2 = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time)) * Matrix4.CreateTranslation(VAO2pos);

            shader.SetMatrix4("model", model2);
            shader.SetMatrix4("view", _camera.GetViewMatrix());
            shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            GL.BindVertexArray(this.vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            this.shader.Use();


            //GL.DrawElements(PrimitiveType.Triangles, this.indices.Length, DrawElementsType.UnsignedInt, 0);
            this.Context.SwapBuffers();

            base.OnRenderFrame(e);
        }

        // In the mouse wheel function we manage all the zooming of the camera
        // this is simply done by changing the FOV of the camera
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _camera.Fov -= e.DeltaPrecise;
            base.OnMouseWheel(e);
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            //if (Focused) // check to see if the window is focused
            //{
            //    Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            //}

            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
            _camera.AspectRatio = Width / (float)Height;
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
            this.texture1.Dispose();

            base.OnUnload(e);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!Focused) // check to see if the window is focused
            {
                return;
            }

            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Key.W))
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward 
            if (input.IsKeyDown(Key.S))
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            if (input.IsKeyDown(Key.A))
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            if (input.IsKeyDown(Key.D))
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            if (input.IsKeyDown(Key.Space))
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up 
            if (input.IsKeyDown(Key.LShift))
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down

            // Get the mouse state
            var mouse = Mouse.GetState();

            if (input.IsKeyDown(Key.ControlLeft))
            {
                if (_firstMove) // this bool variable is initially set to true
                {
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                    _firstMove = false;
                }
                else
                {
                    // Calculate the offset of the mouse position
                    var deltaX = mouse.X - _lastPos.X;
                    var deltaY = mouse.Y - _lastPos.Y;
                    _lastPos = new Vector2(mouse.X, mouse.Y);

                    // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                    _camera.Yaw += deltaX * sensitivity;
                    _camera.Pitch -= deltaY * sensitivity; // reversed since y-coordinates range from bottom to top
                }
            }

            base.OnUpdateFrame(e);

            base.OnUpdateFrame(e);
        }
    }
}
