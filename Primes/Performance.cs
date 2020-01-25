using System.Diagnostics;

namespace Primes
{
    public class Performance
    {
        private readonly Stopwatch stopwatch;
        private int stepNumber;

        public Performance()
        {
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
            Helpers.Print(this, this.StepStepDesc, 2);
        }
        private string StepStepDesc => $"Step: {this.stepNumber++}. time: {this.stopwatch.ElapsedMilliseconds}ms.";
        private string StoptDesc => $"Stop: {this.stepNumber++}. time: {this.stopwatch.ElapsedMilliseconds}ms.";

        public void Stop()
        {
            Helpers.Print(this, this.StoptDesc, 2);
            this.stopwatch.Stop();
        }

        public void Step()
        {
            Helpers.Print(this, this.StepStepDesc, 2);
        }
    }
}
