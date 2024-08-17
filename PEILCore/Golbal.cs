using System.Diagnostics;

namespace PEILCore
{
    public class Golbal
    {
        public static Stopwatch passedtimewatch = new Stopwatch();
        public Golbal()
        {
            passedtimewatch.Start();
        }
        public string GetPassedTime()
        {
            return passedtimewatch.ElapsedMilliseconds.ToString();
        }
    }
}
