using Mjml.Net.Properties;

namespace Mjml.Net.Components;

public partial class RootComponent : Component
{
    private static readonly string DefaultMeta = Resources.DefaultMeta;
    private static readonly string DefaultStyles = Resources.DefaultStyles;
    private static readonly string DefaultComments = Resources.DefaultComments;

    public override string ComponentName => "mjml";

    [Bind("lang", BindType.RequiredString)]
    public string Lang = "und";

    [Bind("dir", BindType.RequiredString)]
    public string Dir = "auto";

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        context.SetGlobalData("lang", new Language(Lang));
        context.SetGlobalData("dir", new Direction(Dir));

        RenderChildren(renderer, context);

        if (Parent != null)
        {
            return;
        }

        renderer.Content("<!doctype html>");
        renderer.Content($"<html lang=\"{Lang}\" dir=\"{Dir}\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">");
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

        foreach (var (_, value) in context.GlobalData)
        {
            if (value is HeadBuffer head && head.Buffer != null)
            {
                // Already formatted properly.
                renderer.Plain(head.Buffer);
            }
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

        foreach (var (_, value) in context.GlobalData)
        {
            if (value is BodyBuffer body && body.Buffer != null)
            {
                // Already formatted properly.
                renderer.Plain(body.Buffer);
            }
        }

        renderer.RenderHelpers(HelperTarget.BodyEnd);
        renderer.EndElement("body");
    }
}
