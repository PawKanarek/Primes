using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PrimesOpenTK
{
    public class Cube : IDisposable
    {
        private int vertexBufferObject;
        private int vertexArrayObject;
        private Texture texture0;
        private Shader shader;
        private readonly List<Vector3> primesCoordinates = new List<Vector3>();
        private readonly Size spiralSize = new Size(1000, 1000);


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

        public void RenderVao(Camera camera, double time)
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
                GL.DrawArrays(PrimitiveType.Triangles, 0, this.mesh.Count);
            //}

            //GL.DrawElements(BeginMode.TriangleStrip, primesCoordinates.Count, DrawElementsType.UnsignedInt, 0);
        }

        public void GetUlamSpiralCoordinates()
        {
            if (this.primesCoordinates.Count > 0)
            {
                return;
            }

            var x = this.spiralSize.Width / 2;
            var y = this.spiralSize.Height / 2;
            var totalRadius = 2;
            var currentRadius = totalRadius;
            var canIncrementRadius = false;
            Direction direction = Direction.Right;
            var performance = new Performance();
            int i;
            // go in lenght of "curreentRadius" in "direction", move 1 pixel at time (x++ || y-- || x-- || y++)
            // change "driection" and reset "currentRadius" if current "currentRadius == 1" (reched corner) and every two times "if canIncrementRadius" increment "totalRadius++"
            var z = 0.01f;
            
            for (i = 1; i < Primes.primes.Length; i++)
            {
                z += 0.01f;
                if (Primes.primes[i])
                {
                    //this.primesCoordinates.Add(new Vector3(x, y, 0));
                    this.mesh.AddRange(new float[]
                    {
                        Cube.verticles[0] + x, Cube.verticles[1] + y, Cube.verticles[2] + z, Cube.verticles[3], Cube.verticles[4],
                        Cube.verticles[5] + x, Cube.verticles[6] + y, Cube.verticles[7] + z, Cube.verticles[8], Cube.verticles[9],
                        Cube.verticles[10] + x, Cube.verticles[11] + y, Cube.verticles[12] + z, Cube.verticles[13], Cube.verticles[14],
                        Cube.verticles[15] + x, Cube.verticles[16] + y, Cube.verticles[17] + z, Cube.verticles[18], Cube.verticles[19],
                        Cube.verticles[20] + x, Cube.verticles[21] + y, Cube.verticles[22] + z, Cube.verticles[23], Cube.verticles[24],
                        Cube.verticles[25] + x, Cube.verticles[26] + y, Cube.verticles[27] + z, Cube.verticles[28], Cube.verticles[29],
                    });

                    if (x > this.spiralSize.Width && y > this.spiralSize.Height)
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
            performance.Stop($"Created {this.mesh.Count / 5} coordinates for tiles in {i} iterations");
        }

        private enum Direction
        {
            Left = 0,
            Up,
            Right,
            Down
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
