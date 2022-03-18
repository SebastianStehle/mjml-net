using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Mjml.Net.Benchmarking
{
    public class MjmlDotNetBenchmark
    {
        private readonly MjmlRenderer MjmlRenderer;
        public string TemplateAmario { get; set; }
        public string TemplateAustin { get; set; }
        public string TemplateSphero { get; set; }
        public string TemplateManyHeroes { get; set; }

        public MjmlDotNetBenchmark()
        {
            MjmlRenderer = new();

            TemplateAmario = File.ReadAllText($"../../../../Templates/Amario.mjml");
            TemplateAustin = File.ReadAllText($"../../../../Templates/Austin.mjml");
            TemplateSphero = File.ReadAllText($"../../../../Templates/Sphero.mjml");
        }

        [Benchmark]
        public string Render_Template_Amario_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateAmario, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Amario_Minified()
        {
            var html = MjmlRenderer.Render(TemplateAmario, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Austin_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateAustin, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Austin_Minified()
        {
            var html = MjmlRenderer.Render(TemplateAustin, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Sphero_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateSphero, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Sphero_Minified()
        {
            var html = MjmlRenderer.Render(TemplateSphero, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MjmlDotNetBenchmark>();
        }
    }
}
