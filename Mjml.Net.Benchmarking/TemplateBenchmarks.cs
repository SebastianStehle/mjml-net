using BenchmarkDotNet.Attributes;

namespace Mjml.Net.Benchmarking
{
    public class TemplateBenchmarks
    {
        private readonly MjmlRenderer MjmlRenderer;      
        public string TemplateArturia { get; set; }
        public string TemplateAustin { get; set; }
        public string TemplateBlackFriday { get; set; }
        public string TemplateCard { get; set; }
        public string TemplateChristmas { get; set; }
        public string TemplateHappyNewYear { get; set; }
        public string TemplateOnePage { get; set; }
        public string TemplateProof { get; set; }
        public string TemplateRacoon { get; set; }
        public string TemplateReactivation { get; set; }
        public string TemplateRealEstate { get; set; }
        public string TemplateRecast { get; set; }
        public string TemplateReceipt { get; set; }
        public string TemplateReferral { get; set; }
        public string TemplateSpheroDroids { get; set; }
        public string TemplateSpheroMini { get; set; }
        public string TemplateUggRoyale { get; set; }
        public string TemplateWelcome { get; set; }
        public string TemplateWorldly { get; set; }

        public TemplateBenchmarks()
        {
            MjmlRenderer = new();

            TemplateArturia = File.ReadAllText("../../../../Templates/Arturia.mjml");
            TemplateAustin = File.ReadAllText("../../../../Templates/Austin.mjml");
            TemplateBlackFriday = File.ReadAllText("../../../../Templates/BlackFriday.mjml");
            TemplateCard = File.ReadAllText("../../../../Templates/Card.mjml");
            TemplateChristmas = File.ReadAllText("../../../../Templates/Christmas.mjml");
            TemplateHappyNewYear = File.ReadAllText("../../../../Templates/HappyNewYear.mjml");
            TemplateOnePage = File.ReadAllText("../../../../Templates/OnePage.mjml");
            TemplateProof = File.ReadAllText("../../../../Templates/Proof.mjml");
            TemplateRacoon = File.ReadAllText("../../../../Templates/Racoon.mjml");
            TemplateReactivation = File.ReadAllText("../../../../Templates/Reactivation.mjml");
            TemplateRealEstate = File.ReadAllText("../../../../Templates/RealEstate.mjml");
            TemplateReceipt = File.ReadAllText("../../../../Templates/Receipt.mjml");
            TemplateReferral = File.ReadAllText("../../../../Templates/Referral.mjml");
            TemplateSpheroDroids = File.ReadAllText("../../../../Templates/SpheroDroids.mjml");
            TemplateSpheroMini = File.ReadAllText("../../../../Templates/SpheroMini.mjml");
            TemplateUggRoyale = File.ReadAllText("../../../../Templates/UGGRoyale.mjml");
            TemplateWelcome = File.ReadAllText("../../../../Templates/Welcome.mjml");
            TemplateWorldly = File.ReadAllText("../../../../Templates/Worldly.mjml");
        }

        #region Beautified
        [Benchmark]
        public string Render_Template_Arturia_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateArturia, new MjmlOptions
            {
                Beautify = true
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
        public string Render_Template_BlackFriday_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateBlackFriday, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Card_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateCard, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Christmas_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateChristmas, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_HappyNewYEar_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateHappyNewYear, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_OnePage_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateOnePage, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Proof_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateProof, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Racoon_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateRacoon, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Reactivation_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateReactivation, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_RealEstate_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateRealEstate, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Recast_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateRecast, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Receipt_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateReceipt, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Referral_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateReferral, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_SpheroDroids_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateSpheroDroids, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_SpheroMini_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateSpheroMini, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_UggRoyale_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateUggRoyale, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Welcome_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateWelcome, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Worldly_Beautified()
        {
            var html = MjmlRenderer.Render(TemplateWorldly, new MjmlOptions
            {
                Beautify = true
            }).Html;

            return html;
        }
        #endregion

        #region Minified
        [Benchmark]
        public string Render_Template_Arturia_Minified()
        {
            var html = MjmlRenderer.Render(TemplateArturia, new MjmlOptions
            {
                Minify = true
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
        public string Render_Template_BlackFriday_Minified()
        {
            var html = MjmlRenderer.Render(TemplateBlackFriday, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Card_Minified()
        {
            var html = MjmlRenderer.Render(TemplateCard, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Christmas_Minified()
        {
            var html = MjmlRenderer.Render(TemplateChristmas, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_HappyNewYEar_Minified()
        {
            var html = MjmlRenderer.Render(TemplateHappyNewYear, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_OnePage_Minified()
        {
            var html = MjmlRenderer.Render(TemplateOnePage, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Proof_Minified()
        {
            var html = MjmlRenderer.Render(TemplateProof, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Racoon_Minified()
        {
            var html = MjmlRenderer.Render(TemplateRacoon, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Reactivation_Minified()
        {
            var html = MjmlRenderer.Render(TemplateReactivation, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_RealEstate_Minified()
        {
            var html = MjmlRenderer.Render(TemplateRealEstate, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Recast_Minified()
        {
            var html = MjmlRenderer.Render(TemplateRecast, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Receipt_Minified()
        {
            var html = MjmlRenderer.Render(TemplateReceipt, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Referral_Minified()
        {
            var html = MjmlRenderer.Render(TemplateReferral, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_SpheroDroids_Minified()
        {
            var html = MjmlRenderer.Render(TemplateSpheroDroids, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_SpheroMini_Minified()
        {
            var html = MjmlRenderer.Render(TemplateSpheroMini, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_UggRoyale_Minified()
        {
            var html = MjmlRenderer.Render(TemplateUggRoyale, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Welcome_Minified()
        {
            var html = MjmlRenderer.Render(TemplateWelcome, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }

        [Benchmark]
        public string Render_Template_Worldly_Minified()
        {
            var html = MjmlRenderer.Render(TemplateWorldly, new MjmlOptions
            {
                Minify = true
            }).Html;

            return html;
        }
        #endregion
    }
}
