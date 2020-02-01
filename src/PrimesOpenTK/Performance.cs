using System.Diagnostics;

namespace PrimesOpenTK
{
    public class Performance
    {
        public readonly Stopwatch stopwatch;
        private string message;
        private int stepNumber;

        public Performance(string message = null)
        {
            this.message = message;
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
            Helpers.Print(this, this.StepStepDesc, 2);
        }
        private string StepStepDesc => $"Step: {this.stepNumber++}. {this.message} time: {this.stopwatch.ElapsedMilliseconds}ms.";
        private string StoptDesc => $"Stop: {this.stepNumber++}. {this.message} time: {this.stopwatch.ElapsedMilliseconds}ms.";

        public void Stop(string newMessage = null)
        {
            this.message = newMessage;
            Helpers.Print(this, this.StoptDesc, 2);
            this.stopwatch.Stop();
        }

        public void Step(string newMessage = null)
        {
            this.message = newMessage;
            Helpers.Print(this, this.StepStepDesc, 2);
        }
    }
}
