namespace PrimesOpenTK
{
    // A helper class, much like Shader, meant to simplify loading textures.
    public struct FpsInfo
    {
        private double timeTotal;
        private double time1Sec;
        private double time3Sec;
        private long frameCountTotal;
        private long frameCount1Sec;
        private long frameCount3Sec;

        private double preFpsIn1Sec;
        private double preFpsIn3Sec;

        public string GetFpsInfo()
        {
            return $"{this.preFpsIn1Sec:F2}(1s) {this.preFpsIn3Sec:F2}(3s) {this.frameCountTotal / this.timeTotal:F2}(total)";
        }

        public bool Update(double time)
        {
            var bUpdate = false;
            this.timeTotal += time;
            this.time1Sec += time;
            this.time3Sec += time;

            this.frameCountTotal++;
            this.frameCount1Sec++;
            this.frameCount3Sec++;

            if (1.0f <= this.time1Sec)
            {
                this.preFpsIn1Sec = this.frameCount1Sec / this.time1Sec;
                this.time1Sec = 0;
                this.frameCount1Sec = 0;
                bUpdate = true;
            }
            if (3.0f <= this.time3Sec)
            {
                this.preFpsIn3Sec = this.frameCount3Sec / this.time3Sec;
                this.time3Sec = 0;
                this.frameCount3Sec = 0;
                bUpdate = true;
            }
            return bUpdate;
        }
    }
}
