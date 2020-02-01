using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace PrimesOpenTK
{
    /// <summary>
    /// Based on https://github.com/opentk/LearnOpenTK/blob/master/Chapter%201/2%20-%20Hello%20Triangle/Window.cs 
    /// </summary>
    public class Window : GameWindow
    {
        private Camera camera;
        private bool firstMove = true;
        private Vector2 lastPos;
        private double time;
        private readonly Cube cube = new Cube();
        private readonly List<Vector3> primesCoordinates = new List<Vector3>();

        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            this.cube.CreateVao();
            this.camera = new Camera(Vector3.UnitZ * 3, this.Width / (float)this.Height);
            Primes.GetPrimesViaEratosthenesSieve();
            this.GetUlamSpiralCoordinates();
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            this.time += 160 * e.Time;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            this.cube.RenderVao(this.camera, this.time, this.primesCoordinates);
            this.Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            this.camera.Fov -= e.DeltaPrecise;
            base.OnMouseWheel(e);
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

            const float cameraSpeed = 150f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Key.W))
            {
                this.camera.Position += this.camera.Front * cameraSpeed * (float)e.Time; // Forward 
            }

            if (input.IsKeyDown(Key.S))
            {
                this.camera.Position -= this.camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }

            if (input.IsKeyDown(Key.A))
            {
                this.camera.Position -= this.camera.Right * cameraSpeed * (float)e.Time; // Left
            }

            if (input.IsKeyDown(Key.D))
            {
                this.camera.Position += this.camera.Right * cameraSpeed * (float)e.Time; // Right
            }

            if (input.IsKeyDown(Key.Space))
            {
                this.camera.Position += this.camera.Up * cameraSpeed * (float)e.Time; // Up 
            }

            if (input.IsKeyDown(Key.LShift))
            {
                this.camera.Position -= this.camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            MouseState mouse = Mouse.GetState();

            if (input.IsKeyDown(Key.ControlLeft))
            {
                if (this.firstMove)
                {
                    this.lastPos = new Vector2(mouse.X, mouse.Y);
                    this.firstMove = false;
                }
                else
                {
                    var deltaX = mouse.X - this.lastPos.X;
                    var deltaY = mouse.Y - this.lastPos.Y;
                    this.lastPos = new Vector2(mouse.X, mouse.Y);

                    this.camera.Yaw += deltaX * sensitivity;
                    this.camera.Pitch -= deltaY * sensitivity;
                }
            }

            base.OnUpdateFrame(e);
        }

        private void GetUlamSpiralCoordinates()
        {
            if (this.primesCoordinates.Count > 0)
            {
                return;
            }

            var x = 200;
            var y = 200;
            var totalRadius = 2;
            var currentRadius = totalRadius;
            var canIncrementRadius = false;
            Direction direction = Direction.Right;
            var performance = new Performance();
            int i;
            // go in lenght of "curreentRadius" in "direction", move 1 pixel at time (x++ || y-- || x-- || y++)
            // change "driection" and reset "currentRadius" if current "currentRadius == 1" (reched corner) and every two times "if canIncrementRadius" increment "totalRadius++"
            for (i = 1; i < Primes.primes.Length; i++)
            {
                if (Primes.primes[i])
                {
                    this.primesCoordinates.Add(new Vector3(x, y, 0));
                    if (y > 400 && x > 400)
                    {
                        break;
                    }
                }

                currentRadius--;

                if (direction == Direction.Right)
                {
                    x++;
                }
                else if (direction == Direction.Up)
                {
                    y--;
                }
                else if (direction == Direction.Left)
                {
                    x--;
                }
                else if (direction == Direction.Down)
                {
                    y++;
                }

                if (currentRadius == 1)
                {
                    if (direction == Direction.Right)
                    {
                        direction = Direction.Up;
                    }
                    else if (direction == Direction.Up)
                    {
                        direction = Direction.Left;
                    }
                    else if (direction == Direction.Left)
                    {
                        direction = Direction.Down;
                    }
                    else if (direction == Direction.Down)
                    {
                        direction = Direction.Right;
                    }

                    if (canIncrementRadius)
                    {
                        totalRadius++;
                    }
                    currentRadius = totalRadius;
                    canIncrementRadius = !canIncrementRadius;
                }
            }
            performance.Stop($"Created {this.primesCoordinates.Count} coordinates for tiles in {i} iterations");
        }

        private enum Direction
        {
            Left = 0,
            Up,
            Right,
            Down
        }
    }
}
