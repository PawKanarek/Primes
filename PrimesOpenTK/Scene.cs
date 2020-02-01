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
        private readonly Cube cube = new Cube();

        public Scene(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            this.cube.CreateVao();
            this._camera = new Camera(Vector3.UnitZ * 3, this.Width / (float)this.Height);
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            this._time += 160 * e.Time;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            this.cube.RenderVao(this._camera, this._time);
            this.Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            this._camera.Fov -= e.DeltaPrecise;
            base.OnMouseWheel(e);
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
            this._camera.AspectRatio = this.Width / (float)this.Height;
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            this.cube.Dispose();
            base.OnUnload(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!this.Focused) // check to see if the window is focused
            {
                return;
            }

            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                this.Exit();
            }

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Key.W))
            {
                this._camera.Position += this._camera.Front * cameraSpeed * (float)e.Time; // Forward 
            }

            if (input.IsKeyDown(Key.S))
            {
                this._camera.Position -= this._camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }

            if (input.IsKeyDown(Key.A))
            {
                this._camera.Position -= this._camera.Right * cameraSpeed * (float)e.Time; // Left
            }

            if (input.IsKeyDown(Key.D))
            {
                this._camera.Position += this._camera.Right * cameraSpeed * (float)e.Time; // Right
            }

            if (input.IsKeyDown(Key.Space))
            {
                this._camera.Position += this._camera.Up * cameraSpeed * (float)e.Time; // Up 
            }

            if (input.IsKeyDown(Key.LShift))
            {
                this._camera.Position -= this._camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            MouseState mouse = Mouse.GetState();

            if (input.IsKeyDown(Key.ControlLeft))
            {
                if (this._firstMove)
                {
                    this._lastPos = new Vector2(mouse.X, mouse.Y);
                    this._firstMove = false;
                }
                else
                {
                    var deltaX = mouse.X - this._lastPos.X;
                    var deltaY = mouse.Y - this._lastPos.Y;
                    this._lastPos = new Vector2(mouse.X, mouse.Y);

                    this._camera.Yaw += deltaX * sensitivity;
                    this._camera.Pitch -= deltaY * sensitivity;
                }
            }

            base.OnUpdateFrame(e);
        }
    }
}
