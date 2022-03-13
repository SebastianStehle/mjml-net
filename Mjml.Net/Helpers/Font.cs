namespace Mjml.Net.Helpers
{
    public sealed record Font(string Href);

    public sealed class FontHelper : IHelper
    {
        public void Render(IHtmlRenderer renderer, HelperTarget target, GlobalData data)
        {
            if (target != HelperTarget.HeadEnd)
            {
                return;
            }

            foreach (var (_, value) in data)
            {
                if (value is Font font)
                {
                    var href = font.Href;

                    renderer.Content("<!--[if !mso]><!-->");

                    // Render Link (self closed).
                    renderer.ElementStart("link", true)
                        .Attr("href", href)
                        .Attr("rel", "stylesheet")
                        .Attr("type", "text/css");

                    // Render Style
                    renderer.ElementStart("style")
                        .Attr("type", "text/css");

                    renderer.Content($"@import url({href});");

                    renderer.ElementEnd("style");

                    renderer.Content("<!--<![endif]-->");
                }
            }
        }
    }
}
