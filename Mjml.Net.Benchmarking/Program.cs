using BenchmarkDotNet.Running;
using CommandLine;

namespace Mjml.Net.Benchmarking
{
    public static class Program
    {
        class Options
        {
            [Option('p', "profiler", Required = false, HelpText = "Runs the test runenr logic.")]
            public bool TestRunner { get; set; }
        }

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                     .WithParsed(o =>
                     {
                         if (o.TestRunner)
                         {
                             TestRunner.Run();
                         } else
                         {
                             BenchmarkRunner.Run<TemplateBenchmarks>();
                         }
                     });
        }
    }
}
