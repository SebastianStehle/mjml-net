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

            foreach (var (_, value) in context.GlobalData)
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
