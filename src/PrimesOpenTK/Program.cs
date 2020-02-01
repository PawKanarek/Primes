namespace PrimesOpenTK
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var game = new Window(800, 800, "Primes"))
            {
                game.Run(60.0);
            }
        }
    }
}
