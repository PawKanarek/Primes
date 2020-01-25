using SkiaSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Primes
{
    public partial class Form1 : Form
    {
        private const int ScreenWidth = 2560;
        private const int ScreenHeight = 2560;
        private const int framerateMs = 17;

        private readonly Preferences preferences = new Preferences();
        private bool[] primes;
        private bool? arePrimesCounted = null;
        private CancellationTokenSource cancellationTokenSource;
        private bool isResizing;
        private PictureBox picturebox;
        private SKImageInfo imageInfo;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public Form1()
        {
            this.InitializeComponent();
            this.ResizeBegin += this.Form1_ResizeBegin;
            this.ResizeEnd += this.Form1_ResizeEnd;
            this.Resize += this.Form1_Resize;
            this.MouseMove += this.Form1_MouseMove;
            this.FormClosing += this.Form1_FormClosing;
            this.Load += this.Form1_Load;
            this.RestoreLastWindowPosition();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.RestoreLastWindowPosition();
            this.picturebox = new PictureBox();
            this.picturebox.Size = new Size(this.Width, this.Height);
            this.picturebox.Name = "CanvasPictureBox";
            this.imageInfo = new SKImageInfo(this.Width, this.Height);
            this.GetPrimesViaEratosthenesSieve();
            this.cancellationTokenSource?.Cancel();
            this.cancellationTokenSource = new CancellationTokenSource();
            this.DrawPrimes(this.cancellationTokenSource.Token);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.preferences.Model.WindowHeight = this.Height;
            this.preferences.Model.WindowWidth = this.Width;
            this.preferences.Model.WindowTop = this.Top;
            this.preferences.Model.WindowLeft = this.Left;
            this.preferences.Model.WindowState = this.WindowState;

            this.preferences.Save();
        }

        private void RestoreLastWindowPosition()
        {
            this.Height = this.preferences.Model.WindowHeight;
            this.Width = this.preferences.Model.WindowWidth;
            this.Top = this.preferences.Model.WindowTop;
            this.Left = this.preferences.Model.WindowLeft;
            this.WindowState = this.preferences.Model.WindowState;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isResizing)
            {
                this.cancellationTokenSource?.Cancel();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.cancellationTokenSource?.Cancel();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            this.isResizing = false;
        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            this.isResizing = true;
            this.cancellationTokenSource?.Cancel();
        }

        private void DrawPrimes(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                if (!CanDraw())
                {
                    return;
                }

                IPerformance performance = new Performance();
                //this.DrawPrimesRectPattern(e, token);
                this.DrawPrimesUlam(token);
                performance.Stop();

            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"Drawing cancelled from");
            }
            finally
            {
            }

            bool CanDraw()
            {
                return this.arePrimesCounted.HasValue && this.arePrimesCounted.Value && this.Width > 0 && this.Height > 0;
            }
        }

        private void DrawPrimesUlam(CancellationToken token)
        {
            var startX = this.Width / 2;
            var startY = this.Height / 2;

            var x = startX;
            var y = startY;
            var totalRadius = 2;
            var currentRadius = totalRadius;
            var canIncrementRadius = false;
            Direction direction = Direction.Right;

            using var surface = SKSurface.Create(this.imageInfo);
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Red);

            // go in lenght of "curreentRadius" in "direction", move 1 pixel at time (x++ || y-- || x-- || y++)
            // change "driection" and reset "currentRadius" if current "currentRadius == 1" (reched corner) and every two times "if canIncrementRadius" increment "totalRadius++"
            for (var i = 1; i < this.primes.Length; i++)
            {
                token.ThrowIfCancellationRequested();

                if (this.primes[i])
                {
                    var color = new SKColor(0, 120, 243);
                    canvas.DrawPoint(x, y, color);

                    if (y > this.Height && x > this.Width)
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

            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var memoryStream = new MemoryStream(data.ToArray());
            var bitmap = new Bitmap(memoryStream, false);

            this.picturebox.Image = bitmap;

            this.Controls.Add(this.picturebox);
        }

        private void GetPrimesViaEratosthenesSieve()
        {
            if (this.arePrimesCounted.HasValue)
            {
                return;
            }
            this.arePrimesCounted = false;
            // From pseudocode https://en.wikipedia.org/wiki/Sieve_of_Eratosthenes
            // Credits to Eratosthenes

            //var limit = BigInteger.Pow(Int64.MaxValue, 3);
            //var limit = (int.MaxValue / 2) - 1;
            var limit = ScreenWidth * ScreenHeight; //Good for tests on my PC

            var performance = new Performance();
            this.primes = new bool[limit];
            for (var i = 0; i < limit; i++)
            {
                this.primes[i] = true;
            }
            this.primes[0] = false;
            this.primes[1] = false;
            performance.Step();
            for (var i = 2; i < Math.Sqrt(limit); i++)
            {
                if (this.primes[i])
                {
                    for (var j = 2 * i; j < limit; j += i)
                    {
                        this.primes[j] = false;
                    }
                }
            }
            performance.Stop();
            this.arePrimesCounted = true;
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
