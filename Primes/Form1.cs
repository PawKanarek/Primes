using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Primes
{
    public partial class Form1 : Form
    {
        // 1000/60 = 16.6
        private const int ScreenWidth = 2560;
        private const int ScreenHeight = 1440;
        private const int framerateMs = 17;

        private Graphics graphics;
        private readonly Brush blackBrush;
        private bool[] primes;
        private bool? arePrimesCounted = null;
        private CancellationTokenSource cancellationTokenSource;
        private bool isResizing;
        private bool isDrawing;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public Form1()
        {
            this.InitializeComponent();
            this.ResizeRedraw = true;
            this.blackBrush = Brushes.Black;
            this.Paint += this.Form1_Paint;
            this.ResizeBegin += this.Form1_ResizeBegin;
            this.ResizeEnd += this.Form1_ResizeEnd;
            this.Resize += this.Form1_Resize;
            this.MouseMove += this.Form1_MouseMove;
            this.graphics = this.CreateGraphics();
            //this.graphics.BeginContainer(this.graphics.VisibleClipBounds, newRect, GraphicsUnit.Pixel);
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
            this.graphics.Dispose();
            this.graphics = this.CreateGraphics();
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
            this.DrawCoordinates(e);
            this.GetPrimesViaEratosthenesSieve(e);
            await Task.Run(() => this.DrawPrimes(e, this.cancellationTokenSource.Token)); ;
        }

        private void DrawPrimes(PaintEventArgs e, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                if (!CanDraw(e))
                {
                    return;
                }

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                this.isDrawing = true;
                for (var i = 0; i < this.primes.Length; i++)
                {
                    if (this.primes[i])
                    {
                        var x = i % e.ClipRectangle.Width;
                        var y = i / e.ClipRectangle.Width;
                        this.graphics.FillRectangle(this.blackBrush, x, y, 1, 1);

                        if (y > e.ClipRectangle.Height)
                        {
                            break;
                        }
                        token.ThrowIfCancellationRequested();
                    }
                }
                this.isDrawing = false;
                stopwatch.Stop();
                Debug.WriteLine($"Drawing took: {stopwatch.ElapsedMilliseconds}");

            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine($"Drawing cancelled from");
            }
            finally
            {
            }

            bool CanDraw(PaintEventArgs eventArgs)
            {
                return this.arePrimesCounted.HasValue && this.arePrimesCounted.Value && eventArgs.ClipRectangle.Width > 0 && e.ClipRectangle.Height > 0;
            }
        }

        private void GetPrimesViaEratosthenesSieve(PaintEventArgs e)
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
            Debug.WriteLine($"Prime numbers finding limit: {limit}");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            this.primes = new bool[limit];
            for (var i = 0; i < limit; i++)
            {
                this.primes[i] = true;
            }
            Debug.WriteLine($"first step took: {stopwatch.ElapsedMilliseconds}ms");

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
            Debug.WriteLine($"second step took: {stopwatch.ElapsedMilliseconds}ms");
            this.arePrimesCounted = true;
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
