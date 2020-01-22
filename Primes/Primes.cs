using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Primes
{ 
    public static class Primes
    {
        public static bool? arePrimesCounted = null;

        public static bool[] primes;

        public static void GetPrimesViaEratosthenesSieve(PaintEventArgs e)
        {
            if (arePrimesCounted.HasValue)
            {
                return;
            }
            arePrimesCounted = false;
            // From pseudocode https://en.wikipedia.org/wiki/Sieve_of_Eratosthenes
            // Credits to Eratosthenes

            //var limit = BigInteger.Pow(Int64.MaxValue, 3);
            //var limit = (int.MaxValue / 2) - 1;
            var limit = Constants.ScreenWidth * Constants.ScreenHeight; //Good for tests on my PC
            Debug.WriteLine($"Prime numbers finding limit: {limit}");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            primes = new bool[limit];
            for (var i = 0; i < limit; i++)
            {
                primes[i] = true;
            }
            primes[0] = false;
            primes[1] = false;
            Debug.WriteLine($"first step took: {stopwatch.ElapsedMilliseconds}ms");

            for (var i = 2; i < Math.Sqrt(limit); i++)
            {
                if (primes[i])
                {
                    for (var j = 2 * i; j < limit; j += i)
                    {
                        primes[j] = false;
                    }
                }
            }
            Debug.WriteLine($"second step took: {stopwatch.ElapsedMilliseconds}ms");
            arePrimesCounted = true;
        }
    }
}
