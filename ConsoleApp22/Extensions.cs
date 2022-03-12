namespace ConsoleApp22
{
    internal static class Extensions
    {
        public static void InlineStyle(this IHtmlRenderer renderer, string? value)
        {
            renderer.StartElement("style")
                .Attr("type", "text/css")
                .Done();

            renderer.Plain(value);

            renderer.EndElement("style");
        }

        public static void Style(this IHtmlRenderer renderer, string? value)
        {
            renderer.StartElement("link")
                .Attr("href", value)
                .Attr("type", "text/css")
                .Attr("rel", "stylesheet")
                .Done();

            renderer.EndElement("link");
        }
    }
}
