using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Primes
{
    public partial class Form1 : Form
    {
        private Point lastCursorPositionDuriningCapture = Point.Empty;
        private CancellationTokenSource cancellationTokenSource;
        private bool isResizing;
        private bool isDrawing;
        private bool isCapturingMouseMovement;
        private readonly Scene scene;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public Form1()
        {
            this.InitializeComponent();
            this.ResizeRedraw = true;
            this.Paint += this.Form1_Paint;
            this.ResizeBegin += this.Form1_ResizeBegin;
            this.ResizeEnd += this.Form1_ResizeEnd;
            this.Resize += this.Form1_Resize;
            this.MouseMove += this.Form1_MouseMove;
            this.MouseDown += this.Form1_MouseDown;
            this.MouseUp += this.Form1_MouseUp;
            this.scene = new Scene(this.CreateGraphics());
            //this.graphics.BeginContainer(this.graphics.VisibleClipBounds, newRect, GraphicsUnit.Pixel);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.isCapturingMouseMovement)
            {
                this.cancellationTokenSource?.Cancel();
                this.MouseMove -= this.MouseMove_WhenCapturing;
                this.cancellationTokenSource?.Cancel();

                this.isCapturingMouseMovement = false;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            this.lastCursorPositionDuriningCapture = this.scene.Center;
            this.MouseMove += this.MouseMove_WhenCapturing;
            this.isCapturingMouseMovement = true;
        }

        private async void MouseMove_WhenCapturing(object sender, MouseEventArgs e)
        {
            var delta = new Point(this.lastCursorPositionDuriningCapture.X - e.X, this.lastCursorPositionDuriningCapture.Y - e.Y);

            Debug.WriteLine($"e.X {e.X}, e.Y {e.Y}, {delta}");
            this.scene.Center = new Point(this.scene.Center.X + delta.X, this.scene.Center.Y + delta.Y);
            this.lastCursorPositionDuriningCapture = e.Location;
            //Debug.WriteLine($"{this.scene.center}");
            this.cancellationTokenSource?.Cancel();
            this.cancellationTokenSource = new CancellationTokenSource();
            await Task.Run(() => this.DrawPrimes(this.cancellationTokenSource.Token)); ;
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
            //this.cancellationTokenSource?.Cancel();
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

        private async void Form1_Paint(object sender, PaintEventArgs e)
        {
            this.cancellationTokenSource?.Cancel();
            this.cancellationTokenSource = new CancellationTokenSource();
            
            if (this.scene.ActualRect == Rectangle.Empty)
            {
                this.scene.ActualRect = e.ClipRectangle;
            }
            this.scene.WholeRect = e.ClipRectangle;

            this.DrawCoordinates(e);
            Primes.GetPrimesViaEratosthenesSieve(e);
            await Task.Run(() => this.DrawPrimes(this.cancellationTokenSource.Token)); ;
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

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                this.isDrawing = true;
                //this.DrawPrimesRectPattern(e, token);
                this.scene.DrawPrimesUlam(token);
                this.isDrawing = false;
                stopwatch.Stop();
                Debug.WriteLine($"Drawing took: {stopwatch.ElapsedMilliseconds}");
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine($"Drawing cancelled from {ex.StackTrace}");
            }
            finally
            {
            }

            bool CanDraw()
            {
                return Primes.arePrimesCounted.HasValue && Primes.arePrimesCounted.Value && this.scene.ActualRect.Width > 0 && this.scene.ActualRect.Height > 0;
            }
        }

        //private void DrawPrimesUlam(CancellationToken token)
        //{
        //    try
        //    {
        //        this.semaphore.Wait();
        //        var e = this.paintEventArgs;

        //        if (this.center == Point.Empty)
        //        {
        //            this.center = new Point(e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2);
        //        }

        //        Point coords = this.center;
        //        var totalRadius = 2;
        //        var currentRadius = totalRadius;
        //        var canIncrementRadius = false;
        //        Direction direction = Direction.Right;

        //        // go in lenght of *curreentRadius* in *direction*, move 1 pixel at time *x++ || y-- || x-- || y++*
        //        // change *driection* and reset *currentRadius* if current *currentRadius == 1* (reched corner) and every two times *if canIncrementRadius* increment *totalRadius++* to make spiral 
        //        for (var i = 1; i < this.primes.Length; i++)
        //        {
        //            token.ThrowIfCancellationRequested();
        //            if (this.primes[i])
        //            {
        //                this.scene.DrawRect(coords.X, coords.Y);
        //            }

        //            if (coords.X > e.ClipRectangle.Width && coords.Y > e.ClipRectangle.Height)
        //            {
        //                break;
        //            }

        //            currentRadius--;
        //            var x = coords.X;
        //            var y = coords.Y;
        //            if (direction == Direction.Right)
        //            {
        //                x++;
        //            }
        //            else if (direction == Direction.Up)
        //            {
        //                y--;
        //            }
        //            else if (direction == Direction.Left)
        //            {
        //                x--;
        //            }
        //            else if (direction == Direction.Down)
        //            {
        //                y++;
        //            }
        //            coords = new Point(x, y);

        //            if (currentRadius == 1)
        //            {
        //                if (direction == Direction.Right)
        //                {
        //                    direction = Direction.Up;
        //                }
        //                else if (direction == Direction.Up)
        //                {
        //                    direction = Direction.Left;
        //                }
        //                else if (direction == Direction.Left)
        //                {
        //                    direction = Direction.Down;
        //                }
        //                else if (direction == Direction.Down)
        //                {
        //                    direction = Direction.Right;
        //                }

        //                if (canIncrementRadius)
        //                {
        //                    totalRadius++;
        //                }

        //                currentRadius = totalRadius;
        //                canIncrementRadius = !canIncrementRadius;
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        this.semaphore.Release();
        //    }
        //}

        private void DrawPrimesRectPattern(PaintEventArgs e, CancellationToken token)
        {
            for (var i = 0; i < Primes.primes.Length; i++)
            {
                if (Primes.primes[i])
                {
                    var x = i % e.ClipRectangle.Width;
                    var y = i / e.ClipRectangle.Width;
                    this.scene.DrawRect(x, y);

                    if (y > e.ClipRectangle.Height)
                    {
                        break;
                    }
                    token.ThrowIfCancellationRequested();
                }
            }
        }



        private void DrawCoordinates(PaintEventArgs e)
        {
            //this.graphics.FillRectangle(this.blackBrush,
            //    x: e.ClipRectangle.Width / 2,
            //    y: 0,
            //    width: 1,
            //    height: e.ClipRectangle.Height);

            //this.graphics.FillRectangle(this.blackBrush,
            //    x: 0,
            //    y: e.ClipRectangle.Height / 2,
            //    width: e.ClipRectangle.Width,
            //    height: 1);
        }
    }
}
