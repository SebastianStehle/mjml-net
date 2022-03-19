#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Mjml.Net.Helpers
{
    public sealed record Style(Action<IHtmlRenderer, GlobalContext> Renderer)
    {
        public static Style Static(string text)
        {
            return new Style((renderer, _) => renderer.Content(text));
        }
    }

    public sealed record MediaQuery(string ClassName, string Rule)
    {
        public static MediaQuery Width(string className, string width)
        {
            return new MediaQuery(className, $"{{\r\nwidth:{width} !important;\r\nmax-width: {width};\r\n}}");
        }
    }

    public sealed class StyleHelper : IHelper
    {
        public void Render(IHtmlRenderer renderer, HelperTarget target, GlobalContext context)
        {
            if (target != HelperTarget.HeadEnd)
            {
                return;
            }

            WriteMediaQueries(renderer, context);
            WriteMediaQueriesThunderbird(renderer, context);
            WriteMediaQueriesOWA(renderer, context);
            WriteStyles(renderer, context);
        }

        private static void WriteMediaQueries(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.StartElement("style")
                .Attr("type", "text/css");

            renderer.Content($"@media only screen and (min-width:{context.Options.Breakpoint}) {{");

            foreach (var mediaQuery in context.GlobalData.Values.OfType<MediaQuery>())
            {
                renderer.Content($"  .{mediaQuery.ClassName} {mediaQuery.Rule}");
            }

            renderer.Content("}");

            renderer.EndElement("style");
        }

        private static void WriteMediaQueriesThunderbird(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.StartElement("style")
                .Attr("type", "text/css").Attr("media", $"screen and (min-width:{context.Options.Breakpoint})");

            foreach (var mediaQuery in context.GlobalData.Values.OfType<MediaQuery>())
            {
                renderer.Content($"  .moz-text-html .{mediaQuery.ClassName} {mediaQuery.Rule}");
            }

            renderer.EndElement("style");
        }

        private static void WriteStyles(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.StartElement("style")
                .Attr("type", "text/css");

            foreach (var style in context.GlobalData.Values.OfType<Style>())
            {
                style.Renderer(renderer, context);
            }

            renderer.EndElement("style");
        }

        private static void WriteMediaQueriesOWA(IHtmlRenderer renderer, GlobalContext context)
        {
            if (!context.Options.ForceOWAQueries)
            {
                return;
            }

            renderer.StartElement("style")
                .Attr("type", "text/css");

            foreach (var mediaQuery in context.GlobalData.Values.OfType<MediaQuery>())
            {
                renderer.Content($"  [owa] {mediaQuery.Rule}");
            }

            renderer.EndElement("style");
        }
    }
}
