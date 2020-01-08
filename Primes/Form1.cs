using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Primes
{
    public partial class Form1 : Form
    {
        // 1000/60 = 16.6
        private const int framerateMs = 17;
        private Graphics graphics;
        private readonly Brush blackBrush;
        private bool[] primes;
        private bool? arePrimesCounted = null;
        private CancellationTokenSource cancellationTokenSource;
        private bool isResizing;
        private int lastMouseX;
        private int lastMouseY;
        private bool isMoving;
        private int lastDelta;
        private System.Timers.Timer timerMouseCapture;
        private bool isDrawing;

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
            this.MouseCaptureChanged += this.Form1_MouseCaptureChanged;
        }

        private async void Form1_MouseCaptureChanged(object sender, EventArgs e)
        {
            var lastMouseXCopy = this.lastMouseX;
            var lastMouseYCopy = this.lastMouseY;
            var lastDeltaCopy = this.lastDelta;
            Debug.WriteLine($"Form1_MouseCaptureChanged {e}");
            await Task.Delay(framerateMs);
            this.lastDelta = Math.Abs(lastMouseXCopy - this.lastMouseX) + Math.Abs(lastMouseYCopy - this.lastMouseY);
            this.isMoving = lastDeltaCopy - this.lastDelta != 0;
            Debug.WriteLine(this.isMoving);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            this.lastMouseX = e.X;
            this.lastMouseY = e.Y;
            this.cancellationTokenSource?.Cancel();
            Debug.WriteLine($"Form1_MouseMove {this.lastMouseX}, {this.lastMouseY}");
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //Debug.WriteLine($"Form1_Resize");
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            //Debug.WriteLine($"Form1_ResizeEnd");

            this.isResizing = false;
        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            //Debug.WriteLine($"Form1_ResizeBegin");
            this.isResizing = true;
            this.cancellationTokenSource?.Cancel();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Debug.WriteLine($"OnPaint 0");
            base.OnPaint(e);
            //Debug.WriteLine($"OnPaint 1");

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Debug.WriteLine($"Form1_Paint");
            if (this.isResizing)
            {
                this.timerMouseCapture = new System.Timers.Timer();
                this.timerMouseCapture.Interval = 16;
                this.timerMouseCapture.Elapsed += this.TimerMouseCapture_Elapsed;
                this.timerMouseCapture.Start();
            }

            this.graphics = e.Graphics;
            this.cancellationTokenSource?.Cancel();
            this.cancellationTokenSource = new CancellationTokenSource();
            this.DrawCoordinates(e);
            this.GetPrimesViaEratosthenesSieve(e);
            this.DrawPrimes(e, this.cancellationTokenSource.Token);
        }

        private void TimerMouseCapture_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.isDrawing)
            {
                var lastDeltaCopy = this.lastDelta;
                this.lastDelta = Math.Abs(this.lastMouseX - Cursor.Position.X) - Math.Abs(this.lastMouseY - Cursor.Position.Y);
                this.isMoving = lastDeltaCopy - this.lastDelta != 0;
                Debug.WriteLine($"isMoving? {isMoving}, lastDeleta:{lastDelta}; {Math.Abs(this.lastMouseX - Cursor.Position.X)}, { Math.Abs(this.lastMouseY - Cursor.Position.Y)}");
            }
        }

        private void DrawPrimes(PaintEventArgs e, CancellationToken token)
        {

            token.ThrowIfCancellationRequested();
            if (!canDraw(e))
            {
                return;
            }
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.WriteLine($"start framerate");
            var frameRate = new Stopwatch();
            frameRate.Start();

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
                    //Debug.WriteLine($"{frameRate.ElapsedMilliseconds / framerateMs}, {(frameRate.ElapsedMilliseconds / framerateMs) > 1}, elapsed SoFar{frameRate.ElapsedMilliseconds}");
                    if ((frameRate.ElapsedMilliseconds / framerateMs) > 0)
                    {
                        Debug.WriteLine($"reset framerate");
                        this.timerMouseCapture?.Dispose();
                        frameRate = new Stopwatch();
                        frameRate.Start();
                        this.cancellationTokenSource.Cancel();
                    }
                }
            }
            this.isDrawing = false;
            stopwatch.Stop();
            Debug.WriteLine($"Drawing took: {stopwatch.ElapsedMilliseconds}");

            bool canDraw(PaintEventArgs eventArgs)
            {
                return this.arePrimesCounted.HasValue && this.arePrimesCounted.Value && eventArgs.ClipRectangle.Width > 0 && e.ClipRectangle.Height > 0;
            }
        }

        private void DrawPixel(int x, int y)
        {

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
            var limit = 2560 * 1440; //Good for tests on my PC
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
