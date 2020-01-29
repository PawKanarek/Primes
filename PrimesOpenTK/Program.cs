using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimesOpenTK
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new Scene(800, 800, "Primes"))
            {
                game.Run(60.0);
            }
        }
    }
}
