using Microsoft.Graphics.Canvas.UI.Xaml;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Primes
{
    public class Skia
    {
        public CanvasControl picturebox;
        private SKImageInfo imageInfo;
        private SKRectI VisibleArea = SKRectI.Empty;
        private SKPointI startTracingPoint = SKPointI.Empty;
        private SKColor color = new SKColor(0, 120, 243);
        private bool startTracing;
        //private Dictionary<int, Point> primesCoordinates = new Dictionary<int, Point>();;
        private List<SKPointI> primesCoordinates = new List<SKPointI>();
        private SKSurface surface;

        public Skia(int Width, int Height)
        {
            this.picturebox = new CanvasControl();
            this.picturebox.Size = new Size(Width, Height);
            this.picturebox.Name = "CanvasPictureBox";
            this.picturebox.MouseMove += this.Picturebox_MouseMove;
            this.picturebox.MouseDown += this.Picturebox_MouseDown;
            this.picturebox.MouseUp += this.Picturebox_MouseUp;
            this.imageInfo = new SKImageInfo(Width, Height);
            this.VisibleArea = this.imageInfo.Rect;
        }

        private void Picturebox_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.startTracing)
            {
                this.Trasnlate(e);
                Helpers.Print(this, $"Location {this.VisibleArea}");
            }
        }

        private void Picturebox_MouseDown(object sender, MouseEventArgs e)
        {
            startTracingPoint = new SKPointI(e.X, e.Y);
            this.startTracing = true;
            Helpers.Print(this);
        }

        private void Picturebox_MouseUp(object sender, MouseEventArgs e)
        {
            this.startTracing = false;
            Helpers.Print(this);
        }
        private void Trasnlate(MouseEventArgs e)
        {
            this.VisibleArea.Location = new SKPointI(this.startTracingPoint.X - e.X, this.startTracingPoint.Y - e.Y);
            var performance = new Performance();
            
            performance.Step($"Start Drawing, iterations: {this.primesCoordinates.Count} Area: {this.VisibleArea.Width * this.VisibleArea.Height}");
            this.surface.Canvas.Clear(SKColors.Red.WithAlpha(10));
            foreach (var point in this.primesCoordinates)
            {
                var translatedPoint = point + this.VisibleArea.Location;
                this.surface.Canvas.DrawPoint(translatedPoint, this.color);
            }
            performance.Step("stop Drawing");
            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
            performance.Step();

            using var memoryStream = new MemoryStream(data.ToArray());
            performance.Step();

            var bitmap = new Bitmap(memoryStream, false);
            this.picturebox.Image?.Dispose();
            this.picturebox.Image = bitmap;
            performance.Stop();
        }

        private void DrawUlamSpiral()
        {
            var performance = new Performance();
            this.surface = SKSurface.Create(this.imageInfo);
            this.surface.Canvas.Clear(SKColors.Red.WithAlpha(10));

            foreach (var point in this.primesCoordinates)
            {
                this.surface.Canvas.DrawPoint(point, this.color);
            }

            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
            performance.Step();

            using var memoryStream = new MemoryStream(data.ToArray());
            performance.Step();

            var bitmap = new Bitmap(memoryStream, false);
            this.picturebox.Image?.Dispose();
            this.picturebox.Image = bitmap;
            performance.Stop();
        }

        public bool TryDraw(CancellationToken? token = null)
        {
            var completed = false;
            try
            {
                if (!Primes.arePrimesCounted.HasValue 
                    || !Primes.arePrimesCounted.Value 
                    || this.VisibleArea.Width <= 0 
                    || this.VisibleArea.Height <= 0)
                {
                    return completed;
                }

                var performance = new Performance();
                this.GetUlamSpiralCoordinates();
                DrawUlamSpiral();
                performance.Stop();
                completed = true;
            }
            catch (OperationCanceledException)
            {
                Helpers.Print(this, "OperationCancelledException");
            }

            return completed;
        }

        private void GetUlamSpiralCoordinates()
        { 
            if (this.primesCoordinates.Count > 0)
            {
                return;
            }

            var x = this.VisibleArea.Width / 2 - this.VisibleArea.Left;
            var y = this.VisibleArea.Height / 2 - this.VisibleArea.Top;
            var totalRadius = 2;
            var currentRadius = totalRadius;
            var canIncrementRadius = false;
            Direction direction = Direction.Right;

            var performance = new Performance(this.VisibleArea.ToString());
           
            performance.Step($"Getting coords, (x0,y0)=({x},{y})");
            var i = 1;
            // go in lenght of "curreentRadius" in "direction", move 1 pixel at time (x++ || y-- || x-- || y++)
            // change "driection" and reset "currentRadius" if current "currentRadius == 1" (reched corner) and every two times "if canIncrementRadius" increment "totalRadius++"
            for (i = 1; i < Primes.primes.Length; i++)
            {
                if (Primes.primes[i])
                {
                    this.primesCoordinates.Add(new SKPointI(x, y));
                    if (y > this.VisibleArea.Height && x > this.VisibleArea.Width)
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
            performance.Stop($"Getting coords done (xn,yn)=({x},{y}) iterationsCount: {i} ");
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
