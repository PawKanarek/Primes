using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Primes
{ 
    public class Scene
    {
        private readonly Graphics graphics;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public Scene(Graphics graphics)
        {
            this.graphics = graphics;
        }

        public Point Location { get; set; }
        public Point Center { get; set; }
        public Rectangle ActualRect { get; set; }
        public Rectangle WholeRect { get; set; }

        public void DrawRect(int x, int y)
        {
            this.graphics.FillRectangle(Brushes.Black, x, y, 1, 1);
        }

        public void DrawPrimesUlam(CancellationToken token)
        {
            try
            {
                this.semaphore.Wait();

                if (this.Center == Point.Empty)
                {
                    this.Center = new Point(this.ActualRect.Width / 2, this.ActualRect.Height / 2);
                }

                Point coords = this.Center;
                var totalRadius = 2;
                var currentRadius = totalRadius;
                var canIncrementRadius = false;
                Direction direction = Direction.Right;

                // go in lenght of *curreentRadius* in *direction*, move 1 pixel at time *x++ || y-- || x-- || y++*
                // change *driection* and reset *currentRadius* if current *currentRadius == 1* (reched corner)
                // also every two times *if canIncrementRadius* increment *totalRadius++* to make spiral 
                for (var i = 1; i < Primes.primes.Length; i++)
                {
                    token.ThrowIfCancellationRequested();
                    if (Primes.primes[i])
                    {
                        this.DrawRect(coords.X, coords.Y);
                    }

                    if (coords.X > this.ActualRect.Width && coords.Y > this.ActualRect.Height)
                    {
                        break;
                    }

                    currentRadius--;
                    var x = coords.X;
                    var y = coords.Y;
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
                    coords = new Point(x, y);

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
            }
            finally
            {
                this.semaphore.Release();
            }
        }



    }

    public enum Direction
    {
        Left = 0,
        Up,
        Right,
        Down
    }
}
