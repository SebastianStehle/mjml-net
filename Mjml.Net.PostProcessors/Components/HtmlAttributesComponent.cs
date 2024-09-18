namespace Mjml.Net.Components;

public partial class HtmlAttributesComponent : Component
{
    private static readonly AllowedParents Parents =
    [
        "mj-head"
    ];

    public override AllowedParents? AllowedParents => Parents;

    public override ContentType ContentType => ContentType.Complex;

    public override string ComponentName => "mj-html-attributes";

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        if (!context.Async || !context.Options.HasProcessor<AttributesPostProcessor>())
        {
            return;
        }

        renderer.StartElement(ComponentName);
        RenderChildren(renderer, context);
        renderer.EndElement(ComponentName);
    }
}
