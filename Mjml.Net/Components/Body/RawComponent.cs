namespace Mjml.Net.Components.Body;

public partial class RawComponent : BodyComponentBase
{
    private static readonly AllowedParents Parents =
    [
        "mj-accordion",
        "mj-accordion-element",
        "mj-body",
        "mj-column",
        "mj-group",
        "mj-head",
        "mj-hero",
        "mjml",
        "mj-navbar",
        "mj-section",
        "mj-social",
        "mj-wrapper"
    ];

    public override AllowedParents? AllowedParents => Parents;

    public override ContentType ContentType => ContentType.Raw;

    public override string ComponentName => "mj-raw";

    public override bool Raw => true;

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        RenderRaw(renderer);
    }
}
