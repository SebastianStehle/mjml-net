namespace Mjml.Net.Helpers
{
    public sealed class FontHelper : IHelper
    {
        public void Render(IHtmlRenderer renderer, GlobalData data)
        {
            foreach (var (_, value) in data)
            {
                if (value is Font font)
                {
                    var href = font.Href;

                    renderer.StartElement("style")
                        .Attr("type", "text/css");

                    renderer.Content($"@import url({href});");

                    renderer.EndElement("style");

                    renderer.StartElement("link")
                        .Attr("href", href)
                        .Attr("type", "text/css")
                        .Attr("rel", "stylesheet");

                    renderer.EndElement("link");
                }
            }
        }
    }
}
