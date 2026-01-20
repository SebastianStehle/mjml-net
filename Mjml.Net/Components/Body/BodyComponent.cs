using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Body;

public partial class BodyComponent : Component
{
    private static readonly AllowedParents Parents =
    [
        "mjml"
    ];

    public override AllowedParents? AllowedParents => Parents;

    public override string ComponentName => "mj-body";

    [Bind("css-class")]
    public string? CssClass;

    [Bind("background-color")]
    public string? BackgroundColor;

    [Bind("width", BindType.Pixels)]
    public string Width = "600px";

    public override void Measure(GlobalContext context, double parentWidth, int numSiblings, int numNonRawSiblings)
    {
        ActualWidth = (int)UnitParser.Parse(Width).Value;

        MeasureChildren(context, ActualWidth);
    }

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        if (BackgroundColor != null)
        {
            context.SetGlobalData("default", new Background(BackgroundColor));
        }

        var title = context.GlobalData.Values.OfType<Title>().FirstOrDefault()?.Value;

        renderer.StartBuffer();

        renderer.StartElement("div")
            .Attr("aria-label", context.GlobalData.Values.OfType<Title>().FirstOrDefault()?.Value.SinglePart())
            .Attr("aria-roledescription", "email")
            .Attr("dir", context.GlobalData.Values.OfType<Direction>().FirstOrDefault()?.Value)
            .Attr("lang", context.GlobalData.Values.OfType<Language>().FirstOrDefault()?.Value)
            .Attr("role", "article")
            .Class(CssClass)
            .Style("background-color", BackgroundColor);

        RenderChildren(renderer, context);

        renderer.EndElement("div");

        context.AddGlobalData(new BodyBuffer(renderer.EndBuffer()));
    }
}
