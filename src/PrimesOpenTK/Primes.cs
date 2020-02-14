using System;

namespace PrimesOpenTK
{
    public static class Primes
    {
        public static bool? arePrimesCounted = null;
        public static bool[] primes;

        public static void GetPrimesViaEratosthenesSieve()
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
            var limit = Constants.ScreenWidth * Constants.ScreenHeight * 3; //Good for tests on my PC

            var performance = new Performance();
            primes = new bool[limit];
            for (var i = 0; i < limit; i++)
            {
                primes[i] = true;
            }
            primes[0] = false;
            primes[1] = false;
            performance.Step();
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
            performance.Stop();
            arePrimesCounted = true;
        }
    }
}
