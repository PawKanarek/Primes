using OpenTK;
using OpenTK.Input;
using System;

namespace PrimesOpenTK
{
    // This is the camera class as it could be set up after the tutorials on the website
    // It is important to note there are a few ways you could have set up this camera, for example
    // you could have also managed the player input inside the camera class, and a lot of the properties could have
    // been made into functions.
    public class Camera
    {
        private float cameraSpeed = 150f;

        private Vector3 front = -Vector3.UnitZ;
        private Vector3 up = Vector3.UnitY;
        private Vector3 right = Vector3.UnitX;
        private Vector2 lastPos = Vector2.Zero;
        private const float cameraSensitivy = 0.2f;

        // Rotation around the X axis (radians)
        private float pitch;
        // Rotation around the Y axis (radians)
        private float yaw = -MathHelper.PiOver2; // Without this you would be started rotated 90 degrees right
        // The field of view of the camera (radians)
        private float fov = MathHelper.PiOver2;

        public Camera(Vector3 position, float aspectRatio)
        {
            this.Position = position;
            this.AspectRatio = aspectRatio;
        }

        public Vector3 Position { get; set; }
        public float AspectRatio { private get; set; }

        // We convert from degrees to radians as soon as the property is set to improve performance
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(this.pitch);
            set
            {
                // We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
                // of weird "bugs" when you are using euler angles for rotation.
                // If you want to read more about this you can try researching a topic called gimbal lock
                var angle = MathHelper.Clamp(value, -89f, 89f);
                this.pitch = MathHelper.DegreesToRadians(angle);
                this.UpdateVectors();
            }
        }

        // We convert from degrees to radians as soon as the property is set to improve performance
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(this.yaw);
            set
            {
                this.yaw = MathHelper.DegreesToRadians(value);
                this.UpdateVectors();
            }
        }

        // The field of view (FOV) is the vertical angle of the camera view, this has been discussed more in depth in a
        // previous tutorial, but in this tutorial you have also learned how we can use this to simulate a zoom feature.
        // We convert from degrees to radians as soon as the property is set to improve performance
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(this.fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 45f);
                this.fov = MathHelper.DegreesToRadians(angle);
            }
        }

        public void ChangeSpeed(MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                this.cameraSpeed -= 30;
            }
            else
            {
                this.cameraSpeed += 30;
            }
        }

        public void HandeMovement(FrameEventArgs e, OpenTK.Input.KeyboardState input)
        {

            if (input.IsKeyDown(Key.W))
            {
                this.Position += this.front * this.cameraSpeed * (float)e.Time; // Forward 
            }

            if (input.IsKeyDown(Key.S))
            {
                this.Position -= this.front * this.cameraSpeed * (float)e.Time; // Backwards
            }

            if (input.IsKeyDown(Key.A))
            {
                this.Position -= this.right * this.cameraSpeed * (float)e.Time; // Left
            }

            if (input.IsKeyDown(Key.D))
            {
                this.Position += this.right * this.cameraSpeed * (float)e.Time; // Right
            }

            if (input.IsKeyDown(Key.Space))
            {
                this.Position += this.up * this.cameraSpeed * (float)e.Time; // Up 
            }

            if (input.IsKeyDown(Key.LShift))
            {
                this.Position -= this.up * this.cameraSpeed * (float)e.Time; // Down
            }

            MouseState mouse = Mouse.GetState();
            if (mouse.IsButtonUp(MouseButton.Left) && this.lastPos != Vector2.Zero)
            {
                this.lastPos = Vector2.Zero;
            }

            if (mouse.IsButtonDown(MouseButton.Left))
            {
                if (this.lastPos == Vector2.Zero)
                {
                    this.lastPos = new Vector2(mouse.X, mouse.Y);
                }

                var deltaX = mouse.X - this.lastPos.X;
                var deltaY = mouse.Y - this.lastPos.Y;
                this.lastPos = new Vector2(mouse.X, mouse.Y);
                this.Yaw += deltaX * cameraSensitivy;
                this.Pitch -= deltaY * cameraSensitivy;
            }
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(this.Position, this.Position + this.front, this.up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(this.fov, this.AspectRatio, 0.01f, float.MaxValue);
        }

        private void UpdateVectors()
        {
            this.front.X = (float)Math.Cos(this.pitch) * (float)Math.Cos(this.yaw);
            this.front.Y = (float)Math.Sin(this.pitch);
            this.front.Z = (float)Math.Cos(this.pitch) * (float)Math.Sin(this.yaw);

            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results
            this.front = Vector3.Normalize(this.front);

            // Calculate both the right and the up vector using cross product
            // Note that we are calculating the right from the global up, this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera
            this.right = Vector3.Normalize(Vector3.Cross(this.front, Vector3.UnitY));
            this.up = Vector3.Normalize(Vector3.Cross(this.right, this.front));
        }
    }
}