namespace Mjml.Net.Helpers
{
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

                    renderer.ElementStart("style")
                        .Attr("type", "text/css");

                    renderer.Content($"@import url({href});");

                    renderer.ElementEnd("style");

                    renderer.ElementStart("link")
                        .Attr("href", href)
                        .Attr("type", "text/css")
                        .Attr("rel", "stylesheet");

                    renderer.ElementEnd("link");
                }
            }
        }
    }
}
