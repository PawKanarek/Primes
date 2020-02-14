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
    public class Window : GameWindow
    {
        private Camera camera;
        private double time;
        private readonly Cube cube = new Cube();
        private FpsInfo fpsInfo;

        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            Primes.GetPrimesViaEratosthenesSieve();
            this.cube.GetUlamSpiralCoordinates();
            this.cube.CreateVao();
            this.camera = new Camera(Vector3.UnitZ * 3, this.Width / (float)this.Height);
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            this.time += 160 * e.Time;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            this.cube.RenderVao(this.camera, this.time);
            this.Context.SwapBuffers();

            // display fps
            if (this.fpsInfo.Update(e.Time))
            {
                this.Title = this.fpsInfo.GetFpsInfo();
            }

            base.OnRenderFrame(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            this.camera.ChangeSpeed(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
            this.camera.AspectRatio = this.Width / (float)this.Height;
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            this.cube.Dispose();

            Preferences.Instance.Model.WindowLeft = this.X;
            Preferences.Instance.Model.WindowTop = this.Y;
            Preferences.Instance.Model.WindowWidth = this.Width;
            Preferences.Instance.Model.WindowHeight = this.Height;
            Preferences.Instance.Save();

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

            this.camera.HandeMovement(e, input);
            base.OnUpdateFrame(e);
        }
    }
}
