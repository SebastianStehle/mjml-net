using BenchmarkDotNet.Attributes;

namespace Mjml.Net.Benchmarking
{
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    [RyuJitX64Job]
    [IterationCount(20)]
    public class TemplateBenchmarks
    {
        private readonly MjmlRenderer MjmlRenderer;
     
        [ParamsSource(nameof(MjmlTemplates))]
        public string MjmlTemplateFilePath { get; set; }

        public IEnumerable<string> MjmlTemplates => Directory.GetFiles("./Templates/", "*.mjml");

        public string MjmlTemplate { get; set; }

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
            return MjmlRenderer.Render(MjmlTemplate, new MjmlOptions
            {
                Beautify = true
            }).Html;
        }

        [Benchmark]
        public string Render_Template_Minify()
        {
            return MjmlRenderer.Render(MjmlTemplate, new MjmlOptions
            {
                Beautify = true
            }).Html;
        }
    }
}
