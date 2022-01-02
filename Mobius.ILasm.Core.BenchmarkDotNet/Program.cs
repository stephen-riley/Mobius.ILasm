using BenchmarkDotNet.Running;

namespace Mobius.ILasm.Core.BenchmarkDotNet
{
    class Program
    {
        static void Main(string[] _)
        {
            RunBenchmark();
        }

        private static void RunBenchmark()
        {
            BenchmarkRunner.Run<BasicBenchmark>();
        }
    }
}
