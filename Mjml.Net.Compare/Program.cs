using CommandLine;

namespace Mjml.Net.Compare;

// Compares the render performance of Mjml.Net with the official MJML renderer (JavaScript).
//
// Usage: dotnet run -c Release --project Mjml.Net.Compare -- [--iterations 100] [--warmup 20] [--no-readme]
public static class Program
{
    private sealed class Options
    {
        [Option("iterations", Required = false, HelpText = "The number of measured renders per template.", Default = 100)]
        public int Iterations { get; set; }

        [Option("warmup", Required = false, HelpText = "The number of warmup rounds over all templates.", Default = 20)]
        public int Warmup { get; set; }

        [Option("no-readme", Required = false, HelpText = "Do not update the summary in the README.md.")]
        public bool NoReadme { get; set; }
    }

    public static async Task<int> Main(string[] args)
    {
        var result = Parser.Default.ParseArguments<Options>(args);

        if (result is not Parsed<Options> parsed)
        {
            return 1;
        }

        await RunAsync(parsed.Value);
        return 0;
    }

    private static async Task RunAsync(Options options)
    {
#if DEBUG
        Console.WriteLine("WARNING: Running in Debug mode. Use '-c Release' for meaningful results.");
#endif

        var paths = ComparePaths.Resolve();
        var templates = Template.LoadAll(paths.Templates);

        Console.WriteLine($"Templates:  {paths.Templates}");
        Console.WriteLine($"Iterations: {options.Iterations} (warmup {options.Warmup})");

        Console.WriteLine("Rendering with Mjml.Net...");
        var netResults = NetRunner.Run(templates, options.Warmup, options.Iterations);

        Console.WriteLine("Rendering with MJML (JavaScript)...");
        var jsResults = await JsRunner.RunAsync(paths, options.Warmup, options.Iterations);

        var comparison = Comparison.Create(netResults, jsResults, MachineInfo.Detect(), options.Warmup, options.Iterations);

        await File.WriteAllTextAsync(paths.Benchmark, MarkdownReport.BuildDetails(comparison));
        Console.WriteLine($"Written {paths.Benchmark}");

        var summary = MarkdownReport.BuildSummary(comparison);

        if (!options.NoReadme)
        {
            await ReadmeUpdater.UpdateAsync(paths.Readme, summary);
            Console.WriteLine($"Updated {paths.Readme}");
        }

        Console.WriteLine();
        Console.WriteLine(summary);
    }
}
