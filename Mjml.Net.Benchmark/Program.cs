using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Running;
using CommandLine;

namespace Mjml.Net.Benchmarking;

public static class Program
{
    private sealed class Options
    {
        [Option('p', "profiler", Required = false, HelpText = "Runs the test runner logic.")]
        public bool TestRunner { get; set; }

        [Option('i', "interations", Required = false, HelpText = "The number of iterations when using profiler mode.", Default = 20)]
        public int TestRunnerIterations { get; set; }

        [Option('a', "postprocessors", Required = false, HelpText = "Runs the benchmarks with the AngleSharp post processors.")]
        public bool PostProcessors { get; set; }

        [Option("ci", Required = false, HelpText = "Runs the CI benchmarks and writes the JSON report to the artifacts folder.")]
        public string? CiArtifacts { get; set; }

        [Option("compare", Required = false, Min = 3, Max = 3, HelpText = "Compares two JSON reports: <base.json> <head.json> <output.md>.")]
        public IEnumerable<string>? Compare { get; set; }
    }

    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                var compare = o.Compare?.ToArray();

                if (compare?.Length == 3)
                {
                    BenchmarkComparison.Write(compare[0], compare[1], compare[2]);
                }
                else if (o.TestRunner)
                {
                    TestRunner.Run(o.TestRunnerIterations);
                }
#if POST_PROCESSORS
                else if (o.CiArtifacts != null)
                {
                    BenchmarkRunner.Run<CiBenchmarks>(
                        DefaultConfig.Instance
                            .WithArtifactsPath(o.CiArtifacts)
                            .AddExporter(JsonExporter.Full));
                }
                else if (o.PostProcessors)
                {
                    BenchmarkRunner.Run<PostProcessorBenchmarks>();
                }
#endif
                else
                {
                    BenchmarkRunner.Run<TemplateBenchmarks>();
                }
            });
    }
}
