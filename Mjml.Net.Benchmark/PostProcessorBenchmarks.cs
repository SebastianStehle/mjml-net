using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

namespace Mjml.Net.Benchmarking;

[Config(typeof(Config))]
[MemoryDiagnoser]
public class PostProcessorBenchmarks
{
    private static readonly MjmlOptions WithPostProcessors = new MjmlOptions { Beautify = true, PostProcessors = [AngleSharpPostProcessor.Default] };
    private readonly MjmlRenderer MjmlRenderer = new();

    [ParamsSource(nameof(MjmlTemplates))]
    public string MjmlTemplateFilePath { get; set; }

    public static IEnumerable<string> MjmlTemplates => Directory.GetFiles("./Templates/", "*.mjml");

    public string MjmlTemplate { get; set; }

    public class Config : ManualConfig
    {
        public Config()
        {
            AddJob(Job.ShortRun.WithId("Dev"));

            AddExporter(MarkdownExporter.GitHub);
        }
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        MjmlTemplate = File.ReadAllText(MjmlTemplateFilePath);
    }

    [Benchmark]
    public async Task<string> Render_Template_PostProcessors()
    {
        return (await MjmlRenderer.RenderAsync(MjmlTemplate, WithPostProcessors)).Html;
    }
}
