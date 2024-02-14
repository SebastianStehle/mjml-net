using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

namespace Mjml.Net.Benchmarking;

[Config(typeof(Config))]
[MemoryDiagnoser]
public class TemplateBenchmarks
{
    private static readonly MjmlOptions WithBeautify = new MjmlOptions { Beautify = true };
    private static readonly MjmlOptions WithMinify = new MjmlOptions { Minify = true };
    private readonly MjmlRenderer MjmlRenderer;
 
    [ParamsSource(nameof(MjmlTemplates))]
    public string MjmlTemplateFilePath { get; set; }

    public static IEnumerable<string> MjmlTemplates => Directory.GetFiles("./Templates/", "*.mjml");

    public string MjmlTemplate { get; set; }

    public class Config : ManualConfig
    {
        public Config()
        {
            var baseJob = Job.ShortRun;

            AddJob(baseJob
                .WithId("Dev").WithBaseline(true));

            AddJob(baseJob.WithCustomBuildConfiguration("V1_24")
                .WithId("1.24.0"));

            AddJob(baseJob.WithCustomBuildConfiguration("V2_0")
                .WithId("2.0.0"));

            AddJob(baseJob.WithCustomBuildConfiguration("V2_1")
                .WithId("2.1.0"));

            AddJob(baseJob.WithCustomBuildConfiguration("V3_8")
                .WithId("3.8.0"));

            AddExporter(MarkdownExporter.GitHub);
        }
    }

    public TemplateBenchmarks()
    {
        MjmlRenderer = new();
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        MjmlTemplate = File.ReadAllText(MjmlTemplateFilePath);
    }

    [Benchmark()]
    public string Render_Template_Beautify()
    {
        return MjmlRenderer.Render(MjmlTemplate, WithBeautify).Html;
    }

    [Benchmark]
    public string Render_Template_Minify()
    {
        return MjmlRenderer.Render(MjmlTemplate, WithMinify).Html;
    }
}
