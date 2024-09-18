namespace Mjml.Net;

public partial class HtmlAttributeComponent : Component
{
    private static readonly AllowedParents Parents =
    [
        "mj-selector"
    ];

    public override AllowedParents? AllowedParents => Parents;

    public override ContentType ContentType => ContentType.Text;

    public override string ComponentName => "mj-html-attribute";

    [Bind("name", BindType.RequiredString)]
    public string Name;

    [BindText]
    public InnerTextOrHtml? Text;

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        if (!context.Async || !context.Options.HasProcessor<AttributesPostProcessor>())
        {
            return;
        }

        renderer.StartElement(ComponentName).Attr("name", Name);
        renderer.Content(Text);
        renderer.EndElement(ComponentName);
    }
}
