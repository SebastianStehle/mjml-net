using Mjml.Net.Extensions;

namespace Mjml.Net.Components.Body;

public partial class NavbarLinkComponent : BodyComponentBase
{
    private static readonly AllowedParents Parents = new AllowedParents
    {
        "mj-navbar"
    };

    public override AllowedParents? AllowedParents => Parents;

    public override ContentType ContentType => ContentType.Text;

    public override string ComponentName => "mj-navbar-link";

    [Bind("color", BindType.Color)]
    public string Color = "#000000";

    [Bind("font-family", BindType.String)]
    public string FontFamily = "Ubuntu, Helvetica, Arial, sans-serif";

    [Bind("font-size", BindType.Pixels)]
    public string FontSize = "13px";

    [Bind("font-style", BindType.String)]
    public string? FontStyle;

    [Bind("font-weight", BindType.String)]
    public string FontWeight = "normal";

    [Bind("href", BindType.String)]
    public string? Href;

    [Bind("letter-spacing", BindType.PixelsOrEm)]
    public string? LetterSpacing;

    [Bind("line-height", BindType.PixelsOrPercentOrNone)]
    public string LineHeight = "22px";

    [Bind("name", BindType.String)]
    public string? Name;

    [Bind("padding", BindType.FourPixelsOrPercent)]
    public string Padding = "15px 10px";

    [Bind("padding-bottom", BindType.PixelsOrPercent)]
    public string? PaddingBottom;

    [Bind("padding-left", BindType.PixelsOrPercent)]
    public string? PaddingLeft;

    [Bind("padding-right", BindType.PixelsOrPercent)]
    public string? PaddingRight;

    [Bind("padding-top", BindType.PixelsOrPercent)]
    public string? PaddingTop;

    [Bind("rel", BindType.String)]
    public string? Rel;

    [Bind("target", BindType.String)]
    public string Target = "_blank";

    [Bind("text-decoration", BindType.String)]
    public string TextDecoration = "none";

    [Bind("text-transform", BindType.String)]
    public string TextTransform = "uppercase";

    [Bind("navbar-base-url")]
    public string? NavbarBaseUrl;

    [BindText]
    public string Text;

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        var link = Href;

        if (NavbarBaseUrl != null)
        {
            link = FormattableString.Invariant($"{NavbarBaseUrl}{link}");
        }

        renderer.StartConditional("<!--[if mso | IE]>");
        {
            renderer.StartElement("td") // Style td
                .Classes(CssClass, "outlook")
                .Style("padding", Padding)
                .Style("padding-bottom", PaddingBottom)
                .Style("padding-left", PaddingLeft)
                .Style("padding-right", PaddingRight)
                .Style("padding-top", PaddingTop);
        }
        renderer.EndConditional("<![endif]-->");

        renderer.StartElement("a") // Style a
            .Attr("href", link)
            .Attr("name", Name)
            .Attr("rel", Rel)
            .Attr("target", Target)
            .Class($"mj-link{CssClass}")
            .Style("color", Color)
            .Style("display", "inline-block")
            .Style("font-family", FontFamily)
            .Style("font-size", FontSize)
            .Style("font-style", FontStyle)
            .Style("font-weight", FontWeight)
            .Style("letter-spacing", LetterSpacing)
            .Style("line-height", LineHeight)
            .Style("padding", Padding)
            .Style("padding-bottom", PaddingBottom)
            .Style("padding-left", PaddingLeft)
            .Style("padding-right", PaddingRight)
            .Style("padding-top", PaddingTop)
            .Style("text-decoration", TextDecoration)
            .Style("text-transform", TextTransform);

        renderer.Content(Text);

        renderer.EndElement("a");

        renderer.StartConditional("<!--[if mso | IE]>");
        {
            renderer.EndElement("td");
        }
        renderer.EndConditional("<![endif]-->");
    }
}
