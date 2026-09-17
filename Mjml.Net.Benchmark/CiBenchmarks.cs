using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Mjml.Net.Validators;

namespace Mjml.Net.Benchmarking;

[Config(typeof(Config))]
[MemoryDiagnoser]
public class CiBenchmarks
{
    private static readonly MjmlOptions Default = new MjmlOptions { Beautify = true };
    private static readonly MjmlOptions WithValidator = new MjmlOptions { Beautify = true, Validator = SoftValidator.Instance };
    private static readonly MjmlOptions WithPostProcessors = new MjmlOptions { Beautify = true, PostProcessors = [AngleSharpPostProcessor.Default] };
    private readonly MjmlRenderer MjmlRenderer = new();
    private string[] templates = [];
    private string mjClassTemplate = string.Empty;

    public class Config : ManualConfig
    {
        public Config()
        {
            // Run in process, because the default toolchain would build the benchmark again against the source of the current branch.
            AddJob(Job.Default
                .WithToolchain(InProcessEmitToolchain.Instance)
                .WithWarmupCount(5)
                .WithIterationCount(15)
                .WithId("CI"));
        }
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        templates = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Templates"), "*.mjml").Select(File.ReadAllText).ToArray();

        // Not part of the templates, so that the results of Render_Templates stay comparable.
        mjClassTemplate = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Scenarios", "MjClass.mjml"));
    }

    [Benchmark]
    public int Render_Templates()
    {
        var length = 0;

        foreach (var template in templates)
        {
            length += MjmlRenderer.Render(template, Default).Html.Length;
        }

        return length;
    }

    [Benchmark]
    public int Render_MjClass()
    {
        // Most templates use no or only a few mj-class attributes, so they would not show the costs of resolving them.
        return MjmlRenderer.Render(mjClassTemplate, Default).Html.Length;
    }

    [Benchmark]
    public int Render_Templates_Validator()
    {
        var length = 0;

        foreach (var template in templates)
        {
            length += MjmlRenderer.Render(template, WithValidator).Html.Length;
        }

        return length;
    }

    [Benchmark]
    public async Task<int> Render_Templates_PostProcessors()
    {
        var length = 0;

        foreach (var template in templates)
        {
            length += (await MjmlRenderer.RenderAsync(template, WithPostProcessors)).Html.Length;
        }

        return length;
    }
}
