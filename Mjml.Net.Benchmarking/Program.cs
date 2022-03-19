using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Mjml.Net.Benchmarking
{
    public static class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<TemplateBenchmarks>();
        }
    }
}
