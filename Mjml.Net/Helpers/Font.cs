namespace Mjml.Net.Helpers
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public sealed record Font(string Href);
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter

    public sealed class FontHelper : IHelper
    {
        public void Render(IHtmlRenderer renderer, HelperTarget target, GlobalContext context)
        {
            if (target != HelperTarget.HeadEnd)
            {
                return;
            }

            var hasFont = context.GlobalData.Values.Any(x => x is Font);

            if (!hasFont)
            {
                return;
            }

            renderer.Content("<!--[if !mso]><!-->");

            foreach (var font in context.GlobalData.Values.OfType<Font>())
            {
                renderer.StartElement("link", true)
                    .Attr("href", font.Href)
                    .Attr("rel", "stylesheet")
                    .Attr("type", "text/css");
            }

            renderer.StartElement("style")
                .Attr("type", "text/css");

            foreach (var font in context.GlobalData.Values.OfType<Font>())
            {
                renderer.Content($"@import url({font.Href});");
            }

            renderer.EndElement("style");

            renderer.Content("<!--<![endif]-->");
        }
    }
}
