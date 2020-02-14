using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;
using System.Text;

namespace PrimesOpenTK
{
    public class Shader : IDisposable
    {
        private readonly int handle;

        public Shader(string vertexPath, string fragmentPath)
        {
            int vertexShader;
            int fragmentShader;

            string vertexShaderSource;
            using (var reader = new StreamReader(vertexPath, Encoding.UTF8))
            {
                vertexShaderSource = reader.ReadToEnd();
            }

            string fragmentShaderSource;
            using (var reader = new StreamReader(fragmentPath, Encoding.UTF8))
            {
                fragmentShaderSource = reader.ReadToEnd();
            }

            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);

            GL.CompileShader(vertexShader);
            this.PrintInfo(GL.GetShaderInfoLog(vertexShader));

            GL.CompileShader(fragmentShader);
            this.PrintInfo(GL.GetShaderInfoLog(fragmentShader));

            this.handle = GL.CreateProgram();
            GL.AttachShader(this.handle, vertexShader);
            GL.AttachShader(this.handle, fragmentShader);
            GL.LinkProgram(this.handle);

            // Cleanup - shaders are now compiled & attached to the program 
            GL.DetachShader(this.handle, vertexShader);
            GL.DetachShader(this.handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            GL.UseProgram(this.handle);
        }

        public void SetInt(string name, int value)
        {
            var location = GL.GetUniformLocation(this.handle, name);

            GL.Uniform1(location, value);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            var location = GL.GetUniformLocation(this.handle, name);

            GL.UniformMatrix4(location, true, ref data);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(this.handle, attribName);
        }

        private void PrintInfo(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine(message);
            }
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

        private void Cleanup()
        {
            GL.DeleteProgram(this.handle);
        }
    }
}
