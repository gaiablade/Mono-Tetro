namespace MonoCustoms
{
    public class Timer
    {
        public double StartTime { get; set; }
        public double PausedTime { get; set; }
        public Timer(double startTime)
        {
            StartTime = startTime;
            PausedTime = 0;
        }
        public void AddPausedTime(double PausedTime)
        {
            this.PausedTime += PausedTime;
        }
        public double GetElapsedMilliseconds(double currentTime)
        {
            return currentTime - PausedTime - StartTime;
        }
        public void Restart(double currentTime)
        {
            StartTime = currentTime;
        }
    }
}
