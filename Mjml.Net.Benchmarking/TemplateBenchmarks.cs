using BenchmarkDotNet.Attributes;

namespace Mjml.Net.Benchmarking
{
    public class TemplateBenchmarks
    {
        private readonly MjmlRenderer MjmlRenderer;
     
        [ParamsSource(nameof(MjmlTemplates))]
        public string MjmlTemplateFileName { get; set; }

        public IEnumerable<string> MjmlTemplates => new[] {
            "Arturia",
            "Austin",
            "BlackFriday",
            "Card",
            "Christmas",
            "HappyNewYear",
            "OnePage",
            "Proof",
            "Racoon",
            "Reactivation",
            "RealEstate",
            "Receipt",
            "Referral",
            "SpheroDroids",
            "SpheroMini",
            "UGGRoyale",
            "Welcome",
            "Worldly",
        };

        public string MjmlTemplate { get; set; }

        public TemplateBenchmarks()
        {
            MjmlRenderer = new();
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            MjmlTemplate = File.ReadAllText($"../../../../Templates/{MjmlTemplateFileName}.mjml");
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
