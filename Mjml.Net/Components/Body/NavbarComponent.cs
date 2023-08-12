using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Body;

public partial class NavbarComponent : BodyComponentBase
{
    private static readonly AllowedParents Parents = new AllowedParents
    {
        "mj-column",
        "mj-hero"
    };

    public override AllowedParents? AllowedParents => Parents;

    public override string ComponentName => "mj-navbar";

    [Bind("align", BindType.Align)]
    public string Align = "center";

    [Bind("base-url", BindType.String)]
    public string? BaseUrl;

    [Bind("hamburger", BindType.String)]
    public string? Hamburger;

    [Bind("ico-align", BindType.Align)]
    public string IcoAlign = "center";

    [Bind("ico-close", BindType.String)]
    public string IcoClose = "&#8855;";

    [Bind("ico-color", BindType.Color)]
    public string IcoColor = "#000000";

    [Bind("ico-font-family", BindType.String)]
    public string IcoFontFamily = "Ubuntu, Helvetica, Arial, sans-serif";

    [Bind("ico-font-size", BindType.PixelsOrPercent)]
    public string IcoFontSize = "30px";

    [Bind("ico-line-height", BindType.PixelsOrPercentOrNone)]
    public string IcoLineHeight = "30px";

    [Bind("ico-open", BindType.String)]
    public string IcoOpen = "&#9776;";

    [Bind("ico-padding", BindType.FourPixelsOrPercent)]
    public string IcoPadding = "10px";

    [Bind("ico-padding-bottom", BindType.PixelsOrPercent)]
    public string? IcoPaddingBottom;

    [Bind("ico-padding-left", BindType.PixelsOrPercent)]
    public string? IcoPaddingLeft;

    [Bind("ico-padding-right", BindType.PixelsOrPercent)]
    public string? IcoPaddingRight;

    [Bind("ico-padding-top", BindType.PixelsOrPercent)]
    public string? IcoPaddingTop;

    [Bind("ico-text-decoration", BindType.String)]
    public string IcoTextDecoration = "none";

    [Bind("ico-text-transform", BindType.String)]
    public string IcoTextTransform = "uppercase";

    [Bind("padding", BindType.FourPixelsOrPercent)]
    public string? Padding;

    [Bind("padding-bottom", BindType.PixelsOrPercent)]
    public string? PaddingBottom;

    [Bind("padding-left", BindType.PixelsOrPercent)]
    public string? PaddingLeft;

    [Bind("padding-right", BindType.PixelsOrPercent)]
    public string? PaddingRight;

    [Bind("padding-top", BindType.PixelsOrPercent)]
    public string? PaddingTop;

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        context.SetGlobalData(ComponentName, new Style(HeadStyle));

        if (Hamburger == "hamburger")
        {
            RenderHamburger(renderer, context);
        }

        renderer.StartElement("div")
            .Class("mj-inline-links");

        renderer.StartConditional("<!--[if mso | IE]>");
        {
            renderer.StartElement("table")
                .Attr("align", Align)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation");
            renderer.StartElement("tr");
        }
        renderer.EndConditional("<![endif]-->");

        foreach (var child in ChildNodes)
        {
            child.Render(renderer, context);
        }

        renderer.StartConditional("<!--[if mso | IE]>");
        {
            renderer.EndElement("tr");
            renderer.EndElement("table");
        }
        renderer.EndConditional("<![endif]-->");

        renderer.EndElement("div");
    }

    private void RenderHamburger(IHtmlRenderer renderer, GlobalContext context)
    {
        var key = context.Options.IdGenerator.Next();

        renderer.StartConditional("<!--[if !mso><!-->");
        {
            renderer.StartElement("input", true)
                .Attr("id", key)
                .Attr("type", "checkbox")
                .Class("mj-menu-checkbox")
                .Style("display", "none !important")
                .Style("max-height", "0")
                .Style("visibility", "hidden");
        }
        renderer.EndConditional("<!--<![endif]-->");

        renderer.StartElement("div") // Style trigger
            .Class("mj-menu-trigger")
            .Style("display", "none")
            .Style("font-size", "0px")
            .Style("max-height", "0px")
            .Style("max-width", "0px")
            .Style("overflow", "hidden");

        renderer.StartElement("label") // Style label
            .Attr("align", IcoAlign)
            .Attr("for", key)
            .Class("mj-menu-label")
            .Style("color", IcoColor)
            .Style("cursor", "pointer")
            .Style("display", "block")
            .Style("font-family", IcoFontFamily)
            .Style("font-size", IcoFontSize)
            .Style("line-height", IcoLineHeight)
            .Style("-moz-user-select", "none")
            .Style("mso-hide", "all")
            .Style("padding", IcoPadding)
            .Style("padding-bottom", IcoPaddingBottom)
            .Style("padding-left", IcoPaddingLeft)
            .Style("padding-right", IcoPaddingRight)
            .Style("padding-top", IcoPaddingTop)
            .Style("text-decoration", IcoTextDecoration)
            .Style("text-transform", IcoTextTransform)
            .Style("user-select", "none");

        renderer.StartElement("span") // Style icoOpen
            .Class("mj-menu-icon-open")
            .Style("mso-hide", "all");

        renderer.Content(IcoOpen);

        renderer.EndElement("span");

        renderer.StartElement("span") // Style icoClose
            .Class("mj-menu-icon-close")
            .Style("display", "none")
            .Style("mso-hide", "all");

        renderer.Content(IcoClose);

        renderer.EndElement("span");

        renderer.EndElement("label");
        renderer.EndElement("div");
    }

    private static void HeadStyle(IHtmlRenderer renderer, GlobalContext context)
    {
        renderer.Content("noinput.mj-menu-checkbox { display:block!important; max-height:none!important; visibility:visible!important; }");
        renderer.Content(" ");
        renderer.Content($"@media only screen and (max-width:{BindingHelper.MakeLowerEndpoint(context.Options.Breakpoint)}) {{");
        renderer.Content("  .mj-menu-checkbox[type=\"checkbox\"] ~ .mj-inline-links { display:none!important; }");
        renderer.Content("  .mj-menu-checkbox[type=\"checkbox\"]:checked ~ .mj-inline-links,");
        renderer.Content("  .mj-menu-checkbox[type=\"checkbox\"] ~ .mj-menu-trigger { display:block!important; max-width:none!important; max-height:none!important; font-size:inherit!important; }");
        renderer.Content("  .mj-menu-checkbox[type=\"checkbox\"] ~ .mj-inline-links > a { display:block!important; }");
        renderer.Content("  .mj-menu-checkbox[type=\"checkbox\"]:checked ~ .mj-menu-trigger .mj-menu-icon-close { display:block!important; }");
        renderer.Content("  .mj-menu-checkbox[type=\"checkbox\"]:checked ~ .mj-menu-trigger .mj-menu-icon-open { display:none!important; }");
        renderer.Content("}");
    }

    public override string? GetInheritingAttribute(string name)
    {
        switch (name)
        {
            case "navbar-base-url":
                return BaseUrl;
        }

        return null;
    }
}
