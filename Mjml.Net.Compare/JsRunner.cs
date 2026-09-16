using System.Globalization;
using System.Text.Json;
using CliWrap;

namespace Mjml.Net.Compare;

public sealed record JsResults(string MjmlVersion, string NodeVersion, List<Measurement> Measurements);

public static class JsRunner
{
    private sealed record Output(string Version, string Node, List<Measurement> Results);

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<JsResults> RunAsync(ComparePaths paths, int warmup, int iterations)
    {
        await EnsureInstalledAsync(paths.Node);

        var outputFile = Path.Combine(Path.GetTempPath(), $"mjml-compare-{Guid.NewGuid():N}.json");
        try
        {
            await Cli.Wrap("node")
                .WithWorkingDirectory(paths.Node)
                .WithArguments(
                [
                    "--expose-gc",
                    "bench.js",
                    paths.Templates,
                    warmup.ToString(CultureInfo.InvariantCulture),
                    iterations.ToString(CultureInfo.InvariantCulture),
                    outputFile
                ])
                .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(Console.Error.WriteLine))
                .ExecuteAsync();

            var output = JsonSerializer.Deserialize<Output>(await File.ReadAllTextAsync(outputFile), JsonOptions)!;

            return new JsResults(output.Version, output.Node.TrimStart('v'), output.Results);
        }
        finally
        {
            File.Delete(outputFile);
        }
    }

    private static async Task EnsureInstalledAsync(string nodeDirectory)
    {
        if (Directory.Exists(Path.Combine(nodeDirectory, "node_modules", "mjml")))
        {
            return;
        }

        Console.WriteLine("Installing mjml (npm ci)...");

        // npm is a batch script on Windows, which cannot be started directly.
        var command = OperatingSystem.IsWindows()
            ? Cli.Wrap("cmd").WithArguments(["/c", "npm", "ci"])
            : Cli.Wrap("npm").WithArguments(["ci"]);

        await command
            .WithWorkingDirectory(nodeDirectory)
            .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(Console.Error.WriteLine))
            .ExecuteAsync();
    }
}
