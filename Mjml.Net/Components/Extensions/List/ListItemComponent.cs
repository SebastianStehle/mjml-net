namespace Mjml.Net.Components.Body;

public partial class ListItemComponent : BodyComponentBase
{
    private static readonly AllowedParents Parents = new AllowedParents
    {
        "mj-list"
    };

    public override AllowedParents? AllowedParents => Parents;

    public override ContentType ContentType => ContentType.Raw;

    public override string ComponentName => "mj-li";

    [Bind("background-color", BindType.Color)]
    public string? BackgroundColor;

    [Bind("bullet-color", BindType.Color)]
    public string? BulletColor;

    [Bind("color", BindType.Color)]
    public string? Color;

    [Bind("font-family", BindType.String)]
    public string FontFamily = "Ubuntu, Helvetica, Arial, sans-serif";

    [Bind("font-size", BindType.Pixels)]
    public string FontSize = "13px";

    [Bind("font-style", BindType.String)]
    public string? FontStyle;

    [Bind("font-weight", BindType.String)]
    public string? FontWeight;

    [Bind("gutter", BindType.Pixels)]
    public string Gutter = "3px";

    [Bind("letter-spacing", BindType.PixelsOrEm)]
    public string? LetterSpacing;

    [Bind("line-height", BindType.PixelsOrPercentOrNone)]
    public string? LineHeight;

    [Bind("padding-left", BindType.PixelsOrPercent)]
    public string? PaddingLeft;

    [Bind("padding-right", BindType.PixelsOrPercent)]
    public string? PaddingRight;

    [Bind("text-align", BindType.TextAlign)]
    public string TextAlign = "left";

    [Bind("text-color", BindType.Color)]
    public string? TextColor;

    [Bind("text-decoration", BindType.String)]
    public string? TextDecoration;

    [Bind("text-transform", BindType.String)]
    public string? TextTransform;

    [Bind("vertical-align", BindType.VerticalAlign)]
    public string VerticalAlign = "top";

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        renderer.StartElement("ul") // Style ulStyle
            .Attr("align", TextAlign)
            .Attr("role", "presentation")
            .Attr("type", "disc")
            .Class("list-item-wrap")
            .Style("backround-color", BackgroundColor)
            .Style("color", Color)
            .Style("font-family", FontFamily)
            .Style("font-size", FontSize)
            .Style("font-style", FontStyle)
            .Style("font-weight", FontWeight)
            .Style("letter-spacing", LetterSpacing)
            .Style("line-height", LineHeight)
            .Style("margin-bottom", "0")
            .Style("margin-left", PaddingLeft)
            .Style("margin-right", PaddingRight)
            .Style("margin-top", "0")
            .Style("mso-line-height-rule", "exactly")
            .Style("padding", "0")
            .Style("text-align", TextAlign)
            .Style("text-decoration", TextDecoration)
            .Style("text-transform", TextTransform);

        renderer.StartElement("li") // Style liStyle
            .Attr("role", "list-item")
            .Class("list-item")
            .Class(CssClass)
            .Style("color", BulletColor)
            .Style("margin", "0")
            .Style("padding", "0")
            .Style("padding-left", Gutter)
            .Style("text-align", TextAlign)
            .Style("text-decoration", TextDecoration)
            .Style("text-transform", TextTransform);

        renderer.StartElement("span") // Style textWrap
            .Class("list-item__text")
            .Style("color", TextColor)
            .Style("letter-spacing", LetterSpacing)
            .Style("text-align", TextAlign)
            .Style("text-decoration", TextDecoration)
            .Style("text-transform", TextTransform);

        RenderRaw(renderer);

        renderer.EndElement("span");
        renderer.EndElement("li");
        renderer.EndElement("ul");
    }
}
