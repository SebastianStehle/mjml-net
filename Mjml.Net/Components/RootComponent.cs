using System.Text;
using Mjml.Net.Helpers;
using Mjml.Net.Properties;

namespace Mjml.Net.Components;

public sealed class RootComponent : Component
{
    private static readonly string DefaultMeta = Resources.DefaultMeta;
    private static readonly string DefaultStyles = Resources.DefaultStyles;
    private static readonly string DefaultComments = Resources.DefaultComments;

    public override string ComponentName => "mjml";

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        RenderChildren(renderer, context);

        renderer.Content("<!doctype html>");
        renderer.Content("<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">");
        renderer.Content(string.Empty);

        RenderHead(renderer, context);

        renderer.Content(string.Empty);

        RenderBody(renderer, context);

        renderer.Content(string.Empty);
        renderer.Content("</html>");
    }

    private static void RenderHead(IHtmlRenderer renderer, GlobalContext context)
    {
        renderer.StartElement("head");

        renderer.RenderHelpers(HelperTarget.HeadStart);

        // Add default meta tags.
        renderer.Content(DefaultMeta);

        renderer.StartElement("style").Attr("type", "text/css");
        renderer.Content(DefaultStyles);
        renderer.EndElement("style");

        renderer.Content(DefaultComments);

        // Already formatted properly.
        if (context.GlobalData.TryGetValue((typeof(StringBuilder), "head"), out var head) && head is StringBuilder sb)
        {
            renderer.Plain(sb);

            renderer.ReturnStringBuilder(sb);
        }

        renderer.RenderHelpers(HelperTarget.HeadEnd);

        renderer.EndElement("head");
    }

    private static void RenderBody(IHtmlRenderer renderer, GlobalContext context)
    {
        renderer.StartElement("body")
            .Style("background-color", context.GlobalData.Values.OfType<Background>().FirstOrDefault()?.Color)
            .Style("word-spacing", "normal");

        renderer.RenderHelpers(HelperTarget.BodyStart);

        // Already formatted properly.
        if (context.GlobalData.TryGetValue((typeof(StringBuilder), "body"), out var body) && body is StringBuilder sb)
        {
            renderer.Plain(sb);

            renderer.ReturnStringBuilder(sb);
        }

        renderer.RenderHelpers(HelperTarget.BodyEnd);

        renderer.EndElement("body");
    }
}
