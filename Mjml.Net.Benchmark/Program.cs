using BenchmarkDotNet.Running;
using CommandLine;

namespace Mjml.Net.Benchmarking
{
    public static class Program
    {
        private sealed class Options
        {
            [Option('p', "profiler", Required = false, HelpText = "Runs the test runner logic.")]
            public bool TestRunner { get; set; }

            [Option('i', "interations", Required = false, HelpText = "The number of iterations when using profiler mode.", Default = 20)]
            public int TestRunnerIterations { get; set; }
        }

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    if (o.TestRunner)
                    {
                        TestRunner.Run(o.TestRunnerIterations);
                    } else
                    {
                        BenchmarkRunner.Run<TemplateBenchmarks>();
                    }
                });
        }
    }
}
