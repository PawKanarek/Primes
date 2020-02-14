using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PrimesOpenTK
{
    public class Cube : IDisposable
    {
        //// x, y, z
        private static readonly float[] verticles = {
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

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

        public static readonly float[] verticles2d = {
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f,
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f,
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f,
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
        };

        private readonly bool is2D = true;
        private int vertexBufferObject;
        private int vertexArrayObject;
        private Texture texture0;
        private Shader shader;
        private readonly List<Vector3> primesCoordinates = new List<Vector3>();
        private readonly Size spiralSize = new Size(1000, 1000);

        public List<float> mesh;

        public Cube()
        {
            if (this.is2D)
            {
                this.mesh = new List<float>(verticles2d);
            }
            else
            {
                this.mesh = new List<float>(verticles);
            }
        }

        public void CreateVao()
        {
            this.vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, this.mesh.Count * sizeof(float), this.mesh.ToArray(), BufferUsageHint.StaticDraw);

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
                    if (this.is2D)
                    {
                        this.mesh.AddRange(new float[]
                        {
                            Cube.verticles[0] + x, Cube.verticles[1] + y, Cube.verticles[2] + z, Cube.verticles[3], Cube.verticles[4],
                            Cube.verticles[5] + x, Cube.verticles[6] + y, Cube.verticles[7] + z, Cube.verticles[8], Cube.verticles[9],
                            Cube.verticles[10] + x, Cube.verticles[11] + y, Cube.verticles[12] + z, Cube.verticles[13], Cube.verticles[14],
                            Cube.verticles[15] + x, Cube.verticles[16] + y, Cube.verticles[17] + z, Cube.verticles[18], Cube.verticles[19],
                            Cube.verticles[20] + x, Cube.verticles[21] + y, Cube.verticles[22] + z, Cube.verticles[23], Cube.verticles[24],
                            Cube.verticles[25] + x, Cube.verticles[26] + y, Cube.verticles[27] + z, Cube.verticles[28], Cube.verticles[29],
                        });
                    }
                    else
                    {
                        this.mesh.AddRange(new float[]
                        {
                            Cube.verticles[0] + x, Cube.verticles[1] + y, Cube.verticles[2] + z, Cube.verticles[3], Cube.verticles[4],
                            Cube.verticles[5] + x, Cube.verticles[6] + y, Cube.verticles[7] + z, Cube.verticles[8], Cube.verticles[9],
                            Cube.verticles[10] + x, Cube.verticles[11] + y, Cube.verticles[12] + z, Cube.verticles[13], Cube.verticles[14],
                            Cube.verticles[15] + x, Cube.verticles[16] + y, Cube.verticles[17] + z, Cube.verticles[18], Cube.verticles[19],
                            Cube.verticles[20] + x, Cube.verticles[21] + y, Cube.verticles[22] + z, Cube.verticles[23], Cube.verticles[24],
                            Cube.verticles[25] + x, Cube.verticles[26] + y, Cube.verticles[27] + z, Cube.verticles[28], Cube.verticles[29], 

                            Cube.verticles[30] + x, Cube.verticles[31] + y, Cube.verticles[32] + z, Cube.verticles[33], Cube.verticles[34],
                            Cube.verticles[35] + x, Cube.verticles[36] + y, Cube.verticles[37] + z, Cube.verticles[38], Cube.verticles[39],
                            Cube.verticles[40] + x, Cube.verticles[41] + y, Cube.verticles[42] + z, Cube.verticles[43], Cube.verticles[44],
                            Cube.verticles[45] + x, Cube.verticles[46] + y, Cube.verticles[47] + z, Cube.verticles[48], Cube.verticles[49],
                            Cube.verticles[50] + x, Cube.verticles[51] + y, Cube.verticles[52] + z, Cube.verticles[53], Cube.verticles[54],
                            Cube.verticles[55] + x, Cube.verticles[56] + y, Cube.verticles[57] + z, Cube.verticles[58], Cube.verticles[59],

                            Cube.verticles[60] + x, Cube.verticles[61] + y, Cube.verticles[62] + z, Cube.verticles[63], Cube.verticles[64],
                            Cube.verticles[65] + x, Cube.verticles[66] + y, Cube.verticles[67] + z, Cube.verticles[68], Cube.verticles[69],
                            Cube.verticles[70] + x, Cube.verticles[71] + y, Cube.verticles[72] + z, Cube.verticles[73], Cube.verticles[74],
                            Cube.verticles[75] + x, Cube.verticles[76] + y, Cube.verticles[77] + z, Cube.verticles[78], Cube.verticles[79],
                            Cube.verticles[80] + x, Cube.verticles[81] + y, Cube.verticles[82] + z, Cube.verticles[83], Cube.verticles[84],
                            Cube.verticles[85] + x, Cube.verticles[86] + y, Cube.verticles[87] + z, Cube.verticles[88], Cube.verticles[89],

                            Cube.verticles[90] + x, Cube.verticles[91] + y, Cube.verticles[92] + z, Cube.verticles[93], Cube.verticles[94],
                            Cube.verticles[95] + x, Cube.verticles[96] + y, Cube.verticles[97] + z, Cube.verticles[98], Cube.verticles[99],
                            Cube.verticles[100] + x, Cube.verticles[101] + y, Cube.verticles[102] + z, Cube.verticles[103], Cube.verticles[104],
                            Cube.verticles[105] + x, Cube.verticles[106] + y, Cube.verticles[107] + z, Cube.verticles[108], Cube.verticles[109],
                            Cube.verticles[110] + x, Cube.verticles[111] + y, Cube.verticles[112] + z, Cube.verticles[113], Cube.verticles[114],
                            Cube.verticles[115] + x, Cube.verticles[116] + y, Cube.verticles[117] + z, Cube.verticles[118], Cube.verticles[109],

                            Cube.verticles[120] + x, Cube.verticles[121] + y, Cube.verticles[122] + z, Cube.verticles[123], Cube.verticles[124],
                            Cube.verticles[125] + x, Cube.verticles[126] + y, Cube.verticles[127] + z, Cube.verticles[128], Cube.verticles[129],
                            Cube.verticles[130] + x, Cube.verticles[131] + y, Cube.verticles[132] + z, Cube.verticles[133], Cube.verticles[134],
                            Cube.verticles[135] + x, Cube.verticles[136] + y, Cube.verticles[137] + z, Cube.verticles[138], Cube.verticles[139],
                            Cube.verticles[140] + x, Cube.verticles[141] + y, Cube.verticles[142] + z, Cube.verticles[143], Cube.verticles[144],
                            Cube.verticles[145] + x, Cube.verticles[146] + y, Cube.verticles[147] + z, Cube.verticles[148], Cube.verticles[149],

                            Cube.verticles[150] + x, Cube.verticles[151] + y, Cube.verticles[152] + z, Cube.verticles[153], Cube.verticles[154],
                            Cube.verticles[155] + x, Cube.verticles[156] + y, Cube.verticles[157] + z, Cube.verticles[158], Cube.verticles[159],
                            Cube.verticles[160] + x, Cube.verticles[161] + y, Cube.verticles[162] + z, Cube.verticles[163], Cube.verticles[164],
                            Cube.verticles[165] + x, Cube.verticles[166] + y, Cube.verticles[167] + z, Cube.verticles[168], Cube.verticles[169],
                            Cube.verticles[170] + x, Cube.verticles[171] + y, Cube.verticles[172] + z, Cube.verticles[173], Cube.verticles[174],
                            Cube.verticles[175] + x, Cube.verticles[176] + y, Cube.verticles[177] + z, Cube.verticles[178], Cube.verticles[179],
                        });
                    }
                    //this.primesCoordinates.Add(new Vector3(x, y, 0));

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
