using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Mjml.Net.Benchmarking;

public static class BenchmarkComparison
{
    public const string Marker = "<!-- mjml-net-benchmark -->";

    private sealed record Result(string Method, double MeanNanoseconds, double ErrorNanoseconds, long AllocatedBytes);

    public static void Write(string baseFile, string headFile, string outputFile)
    {
        var baseResults = Read(baseFile).ToDictionary(x => x.Method, StringComparer.Ordinal);
        var headResults = Read(headFile);

        var sb = new StringBuilder();
        sb.AppendLine(Marker);
        sb.AppendLine("## Benchmark");
        sb.AppendLine();
        sb.AppendLine("Comparison of this pull request with the base branch. Each operation renders all benchmark templates.");
        sb.AppendLine();
        sb.AppendLine("| Benchmark | Time (base) | Time (PR) | Time | Allocated (base) | Allocated (PR) | Allocated |");
        sb.AppendLine("|---|---:|---:|---:|---:|---:|---:|");

        foreach (var head in headResults)
        {
            if (!baseResults.TryGetValue(head.Method, out var baseResult))
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"| {head.Method} | – | {FormatTime(head)} | – | – | {FormatBytes(head.AllocatedBytes)} | – |");
                continue;
            }

            sb.AppendLine(CultureInfo.InvariantCulture,
                $"| {head.Method} | {FormatTime(baseResult)} | {FormatTime(head)} | {FormatChange(baseResult.MeanNanoseconds, head.MeanNanoseconds)} | {FormatBytes(baseResult.AllocatedBytes)} | {FormatBytes(head.AllocatedBytes)} | {FormatChange(baseResult.AllocatedBytes, head.AllocatedBytes)} |");
        }

        sb.AppendLine();
        sb.AppendLine("Measured on a shared GitHub runner, so time differences of up to ~10% can be noise. Allocations are stable.");

        File.WriteAllText(outputFile, sb.ToString());
    }

    private static List<Result> Read(string file)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(file));

        var results = new List<Result>();

        foreach (var benchmark in document.RootElement.GetProperty("Benchmarks").EnumerateArray())
        {
            var statistics = benchmark.GetProperty("Statistics");

            results.Add(new Result(
                benchmark.GetProperty("Method").GetString()!,
                statistics.GetProperty("Mean").GetDouble(),
                statistics.GetProperty("StandardError").GetDouble(),
                benchmark.GetProperty("Memory").GetProperty("BytesAllocatedPerOperation").GetInt64()));
        }

        return results;
    }

    private static string FormatTime(Result result)
    {
        return string.Create(CultureInfo.InvariantCulture, $"{result.MeanNanoseconds / 1_000_000:0.00} ms ± {result.ErrorNanoseconds / 1_000_000:0.00}");
    }

    private static string FormatBytes(long bytes)
    {
        return string.Create(CultureInfo.InvariantCulture, $"{bytes / 1024.0 / 1024.0:0.00} MB");
    }

    private static string FormatChange(double baseValue, double headValue)
    {
        if (baseValue == 0)
        {
            return "–";
        }

        var change = (headValue - baseValue) / baseValue * 100;

        return string.Create(CultureInfo.InvariantCulture, $"{change:+0.0;-0.0;0.0}%");
    }
}
