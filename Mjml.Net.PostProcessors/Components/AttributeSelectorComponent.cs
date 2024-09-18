namespace Mjml.Net;

public partial class SelectorComponent : Component
{
    private static readonly AllowedParents Parents =
    [
        "mj-html-attributes"
    ];

    public override AllowedParents? AllowedParents => Parents;

    public override ContentType ContentType => ContentType.Complex;

    public override string ComponentName => "mj-selector";

    [Bind("path", BindType.RequiredString)]
    public string Path;

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        if (!context.Async || !context.Options.HasProcessor<AttributesPostProcessor>())
        {
            return;
        }

        renderer.StartElement(ComponentName).Attr("path", Path);
        RenderChildren(renderer, context);
        renderer.EndElement(ComponentName);
    }
}
