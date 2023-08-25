namespace Mjml.Net.Components.Body;

public partial class BodyComponent : Component
{
    private static readonly AllowedParents Parents = new AllowedParents
    {
        "mjml"
    };

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

        renderer.StartBuffer();

        renderer.StartElement("div")
            .Class(CssClass)
            .Style("background-color", BackgroundColor);

        RenderChildren(renderer, context);

        renderer.EndElement("div");

        context.AddGlobalData(new BodyBuffer(renderer.EndBuffer()));
    }
}
