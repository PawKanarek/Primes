using System;

namespace PrimesOpenTK
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var model = Preferences.Instance.Model;
            using (var game = new Window(model.WindowWidth, model.WindowHeight, "Primes"))
            {
                game.X = model.WindowLeft;
                game.Y = model.WindowTop;
                game.Run(60.0);
            }
        }
    }
}
