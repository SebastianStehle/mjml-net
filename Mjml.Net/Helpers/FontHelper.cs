namespace Mjml.Net.Helpers
{
    public sealed class FontHelper : IHelper
    {
        public void Render(IHtmlRenderer renderer, GlobalData data)
        {
            foreach (var (type, values) in data)
            {
                if (type == "font" && values is Font font && font.Href != null)
                {
                    var href = font.Href;

                    renderer.StartElement("style")
                        .Attr("type", "text/css")
                        .Done();

                    renderer.Plain($"@import url({href});");

                    renderer.EndElement("style");

                    renderer.StartElement("link")
                        .Attr("href", href)
                        .Attr("type", "text/css")
                        .Attr("rel", "stylesheet")
                        .Done();

                    renderer.EndElement("link");
                }
            }
        }
    }
}
