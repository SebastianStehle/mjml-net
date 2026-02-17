using Mjml.Net.Extensions;

namespace Mjml.Net.Components.Body;

public partial class SectionComponent : BodyComponentBase
{
    private static readonly AllowedParents Parents =
    [
        "mj-body",
        "mj-wrapper"
    ];

    public override AllowedParents? AllowedParents => Parents;

    public override string ComponentName => "mj-section";

    [Bind("background-color", BindType.Color)]
    public string? BackgroundColor;

    [Bind("background-position")]
    public string BackgroundPosition = "top center";

    [Bind("background-position-x")]
    public string? BackgroundPositionX;

    [Bind("background-position-y")]
    public string? BackgroundPositionY;

    [Bind("background-repeat")]
    public string BackgroundRepeat = "repeat";

    [Bind("background-size")]
    public string BackgroundSize = "auto";

    [Bind("background-url")]
    public string? BackgroundUrl;

    [Bind("border")]
    public string? Border;

    [Bind("border-bottom")]
    public string? BorderBottom;

    [Bind("border-left")]
    public string? BorderLeft;

    [Bind("border-radius", BindType.PixelsOrPercent)]
    public string? BorderRadius;

    [Bind("border-right")]
    public string? BorderRight;

    [Bind("border-top")]
    public string? BorderTop;

    [Bind("direction")]
    public string Direction = "ltr";

    [Bind("full-width")]
    public string? FullWidth;

    [Bind("padding", BindType.FourPixelsOrPercent)]
    public string Padding = "20px 0";

    [Bind("padding-bottom", BindType.PixelsOrPercent)]
    public string? PaddingBottom;

    [Bind("padding-left", BindType.PixelsOrPercent)]
    public string? PaddingLeft;

    [Bind("padding-right", BindType.PixelsOrPercent)]
    public string? PaddingRight;

    [Bind("padding-top", BindType.PixelsOrPercent)]
    public string? PaddingTop;

    [Bind("text-align", BindType.TextAlign)]
    public string TextAlign = "center";

    [Bind("text-padding", BindType.FourPixelsOrPercent)]
    public string TextPadding = "4px 4px 4px 0";

    protected override void AfterBind(GlobalContext context)
    {
        base.AfterBind(context);
    }

    public override void Measure(GlobalContext context, double parentWidth, int numSiblings, int numNonRawSiblings)
    {
        ActualWidth = parentWidth;

        var innerWidth =
            ActualWidth -
            UnitParser.Parse(BorderLeft).Value -
            UnitParser.Parse(BorderRight).Value -
            UnitParser.Parse(PaddingLeft).Value -
            UnitParser.Parse(PaddingRight).Value;

        MeasureChildren(context, innerWidth);
    }

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        if (FullWidth == "full-width")
        {
            RenderFullWidth(renderer, context);
        }
        else
        {
            RenderSimple(renderer, context);
        }
    }

    private void RenderFullWidth(IHtmlRenderer renderer, GlobalContext context)
    {
        var hasBackground = HasBackground();
        var hasBorderRadius = !string.IsNullOrWhiteSpace(BorderRadius);

        var tableElement = renderer.StartElement("table")
            .Attr("align", "center")
            .Attr("background", BackgroundUrl)
            .Attr("border", "0")
            .Attr("cellpadding", "0")
            .Attr("cellspacing", "0")
            .Attr("role", "presentation")
            .Class(CssClass)
            .Style("width", "100%")
            .Style("border-radius", BorderRadius)
            .StyleIf("border-collapse", hasBorderRadius, "separate");

        if (hasBackground)
        {
            tableElement
                .Style("background", GetBackground())
                .Style("background-position", BackgroundPosition)
                .Style("background-repeat", BackgroundRepeat)
                .Style("background-size", BackgroundSize);
        }
        else
        {
            tableElement
                .Style("background", BackgroundColor)
                .Style("background-color", BackgroundColor);
        }

        renderer.StartElement("tbody");
        renderer.StartElement("tr");
        renderer.StartElement("td");

        if (hasBackground)
        {
            RenderSectionWithBackground(renderer, context, true);
        }
        else
        {
            RenderSectionStart(renderer);
            RenderSection(renderer, context, true);
            RenderSectionEnd(renderer);
        }

        renderer.EndElement("td");
        renderer.EndElement("tr");
        renderer.EndElement("tbody");
        renderer.EndElement("table");
    }

    private void RenderSimple(IHtmlRenderer renderer, GlobalContext context)
    {
        RenderSectionStart(renderer);

        if (HasBackground())
        {
            RenderSectionWithBackground(renderer, context, false);
        }
        else
        {
            RenderSection(renderer, context, false);
        }

        RenderSectionEnd(renderer);
    }

    private void RenderSectionStart(IHtmlRenderer renderer)
    {
        renderer.StartConditional("<!--[if mso | IE]>");
        {
            renderer.StartElement("table")
                .Attr("align", "center")
                .Attr("bgcolor", BackgroundColor)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Attr("width", $"{ActualWidth}")
                .Classes(CssClass, "outlook")
                .Style("width", $"{ActualWidth}px");

            renderer.StartElement("tr");
            renderer.StartElement("td")
                .Style("font-size", "0px")
                .Style("line-height", "0px")
                .Style("mso-line-height-rule", "exactly");
        }
        renderer.EndConditional("<![endif]-->");
    }

    private void RenderSection(IHtmlRenderer renderer, GlobalContext context, bool fullWidth)
    {
        var hasBackground = HasBackground();
        var background = hasBackground ? GetBackground() : null;
        var hasBorderRadius = !string.IsNullOrWhiteSpace(BorderRadius);

        var divElement = renderer.StartElement("div") // Style div
            .Class(fullWidth ? null : CssClass)
            .Style("border-radius", BorderRadius)
            .Style("margin", "0px auto")
            .Style("max-width", $"{ActualWidth}px")
            .StyleIf("overflow", !string.IsNullOrWhiteSpace(BorderRadius), "hidden");

        if (!fullWidth)
        {
            if (hasBackground)
            {
                divElement // Style background
                    .Style("background", background)
                    .Style("background-position", BackgroundPosition)
                    .Style("background-repeat", BackgroundRepeat)
                    .Style("background-size", BackgroundSize);
            }
            else
            {
                divElement // Style background
                    .Style("background", BackgroundColor)
                    .Style("background-color", BackgroundColor);
            }
        }

        if (hasBackground)
        {
            renderer.StartElement("div") // Style innerDiv
                .Style("line-height", "0")
                .Style("font-size", "0");
        }

        var tableElement = renderer.StartElement("table") // Style table
            .Attr("align", "center")
            .Attr("background", fullWidth ? null : BackgroundUrl)
            .Attr("border", "0")
            .Attr("cellpadding", "0")
            .Attr("cellspacing", "0")
            .Attr("role", "presentation")
            .Style("width", "100%")
            .StyleIf("border-collapse", hasBorderRadius, "separate");

        if (!fullWidth)
        {
            if (hasBackground)
            {
                tableElement // Style background
                    .Style("background", background)
                    .Style("background-position", BackgroundPosition)
                    .Style("background-repeat", BackgroundRepeat)
                    .Style("background-size", BackgroundSize);
            }
            else
            {
                tableElement // Style background
                    .Style("background", BackgroundColor)
                    .Style("background-color", BackgroundColor);
            }
        }

        renderer.StartElement("tbody");
        renderer.StartElement("tr");
        renderer.StartElement("td") // Style td
            .Style("border", Border)
            .Style("border-bottom", BorderBottom)
            .Style("border-left", BorderLeft)
            .Style("border-right", BorderRight)
            .Style("border-top", BorderTop)
            .Style("border-radius", BorderRadius)
            .Style("direction", Direction)
            .Style("font-size", "0px")
            .Style("padding", Padding)
            .Style("padding-bottom", PaddingBottom)
            .Style("padding-left", PaddingLeft)
            .Style("padding-right", PaddingRight)
            .Style("padding-top", PaddingTop)
            .Style("text-align", TextAlign);

        renderer.StartConditional("<!--[if mso | IE]>");
        {
            renderer.StartElement("table")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation");
        }
        renderer.EndConditional("<![endif]-->");

        RenderWrappedChildren(renderer, context);

        renderer.StartConditional("<!--[if mso | IE]>");
        {
            renderer.EndElement("table");
        }
        renderer.EndConditional("<![endif]-->");

        renderer.EndElement("td");
        renderer.EndElement("tr");
        renderer.EndElement("tbody");
        renderer.EndElement("table");

        if (hasBackground)
        {
            renderer.EndElement("div");
        }

        renderer.EndElement("div");
    }

    private static void RenderSectionEnd(IHtmlRenderer renderer)
    {
        renderer.StartConditional("<!--[if mso | IE]>");
        {
            renderer.EndElement("td");
            renderer.EndElement("tr");
            renderer.EndElement("table");
        }
        renderer.EndConditional("<![endif]-->");
    }

    private void RenderSectionWithBackground(IHtmlRenderer renderer, GlobalContext context, bool fullWidth)
    {
        var (x, y) = ParseBackgroundPosition();
        var (xPercent, yPercent) = GetBackgroundPositionAsPercentage(x, y);

        var (xOrigin, xPosition) = GetOriginBasedForAxis(true, xPercent, yPercent);
        var (yOrigin, yPosition) = GetOriginBasedForAxis(false, xPercent, yPercent);

        var isBackgroundSizeAuto = BackgroundSize.Equals("auto", StringComparison.OrdinalIgnoreCase);
        var isBackgroundSizeCover = BackgroundSize.Equals("cover", StringComparison.OrdinalIgnoreCase);
        var isBackgroundSizeContain = BackgroundSize.Equals("contain", StringComparison.OrdinalIgnoreCase);
        var isBackgroundRepeatNoRepeat = BackgroundRepeat.Equals("no-repeat", StringComparison.OrdinalIgnoreCase);

        string? vmlSize = null;
        string? vmlAspect = null;
        string? vmlType = isBackgroundRepeatNoRepeat ? "frame" : "tile";

        if (isBackgroundSizeCover || isBackgroundSizeContain)
        {
            vmlSize = "1,1";
            vmlAspect = isBackgroundSizeCover ? "atleast" : "atmost";
        }
        else if (!isBackgroundSizeAuto)
        {
#if NET8_0_OR_GREATER
            // Optimize: Use stackalloc for small expected arrays (max 2 parts) on .NET 8+
            Span<Range> ranges = stackalloc Range[2];
            var count = BackgroundSize.AsSpan().Split(ranges, ' ');

            if (count == 1)
            {
                vmlSize = BackgroundSize[ranges[0]];
                vmlAspect = "atmost";
            }
            else if (count >= 2)
            {
                vmlSize = $"{BackgroundSize[ranges[0]]},{BackgroundSize[ranges[1]]}";
            }
#else
            // For .NET 6/7, use standard Split
            var positions = BackgroundSize.Split(' ');

            if (positions.Length == 1)
            {
                vmlSize = positions[0];
                vmlAspect = "atmost";
            }
            else if (positions.Length >= 2)
            {
                vmlSize = $"{positions[0]},{positions[1]}";
            }
#endif
        }

        if (isBackgroundSizeAuto)
        {
            vmlType = "tile";
            xOrigin = "0.5";
            yOrigin = "0";
            xPosition = "0.5";
            yPosition = "0";
        }

        renderer.StartConditional("<!--[if mso | IE]>");
        {
            var rectElement = renderer.StartElement("v:rect")
                .Attr("xmlns:v", "urn:schemas-microsoft-com:vml")
                .Attr("fill", "true")
                .Attr("stroke", "false");

            if (fullWidth)
            {
                rectElement.Style("mso-width-percent", "1000");
            }
            else
            {
                rectElement.Style("width", $"{ActualWidth}px");
            }

            renderer.StartElement("v:fill", true)
                .Attr("origin", $"{xOrigin}, {yOrigin}")
                .Attr("position", $"{xPosition}, {yPosition}")
                .Attr("src", BackgroundUrl)
                .Attr("color", BackgroundColor)
                .Attr("type", vmlType)
                .Attr("size", vmlSize)
                .Attr("aspect", vmlAspect);

            renderer.StartElement("v:textbox")
                .Attr("inset", "0,0,0,0")
                .Style("mso-fit-shape-to-text", "true");
        }
        renderer.EndConditional("<![endif]-->");

        if (fullWidth)
        {
            RenderSectionStart(renderer);
            RenderSection(renderer, context, fullWidth);
            RenderSectionEnd(renderer);
        }
        else
        {
            RenderSection(renderer, context, fullWidth);
        }

        renderer.StartConditional("<!--[if mso | IE]>");
        {
            renderer.EndElement("v:textbox");
            renderer.EndElement("v:rect");
        }
        renderer.EndConditional("<![endif]-->");
    }

    protected virtual void RenderWrappedChildren(IHtmlRenderer renderer, GlobalContext context)
    {
        renderer.StartConditional("<!--[if mso | IE]>");
        {
            renderer.StartElement("tr");
        }
        renderer.EndConditional("<![endif]-->");

        foreach (var child in ChildNodes)
        {
            if (child.Raw)
            {
                child.Render(renderer, context);
            }
            else
            {
                renderer.StartConditional("<!--[if mso | IE]>");
                {
                    renderer.StartElement("td")
                        .Attr("align", child.GetAttribute("align"))
                        .Classes(child.GetAttribute("css-class"), "outlook")
                        .Style("vertical-align", child.GetAttribute("vertical-align"))
                        .Style("width", $"{child.ActualWidth}px");
                }
                renderer.EndConditional("<![endif]-->");

                child.Render(renderer, context);

                renderer.StartConditional("<!--[if mso | IE]>");
                {
                    renderer.EndElement("td");
                }
                renderer.EndConditional("<![endif]-->");
            }
        }

        renderer.StartConditional("<!--[if mso | IE]>");
        {
            renderer.EndElement("tr");
        }
        renderer.EndConditional("<![endif]-->");
    }

    private string? GetBackground()
    {
        if (!HasBackground())
        {
            return BackgroundColor;
        }

        return FormattableString.Invariant($"{BackgroundColor} url('{BackgroundUrl}') {GetBackgroundPositionString()} / {BackgroundSize} {BackgroundRepeat}");
    }

    private string? GetBackgroundPositionString()
    {
        var (parsedX, parsedY) = ParseBackgroundPosition();

        var x = string.IsNullOrEmpty(BackgroundPositionX) ? parsedX : BackgroundPositionX;
        var y = string.IsNullOrEmpty(BackgroundPositionY) ? parsedY : BackgroundPositionY;

        return FormattableString.Invariant($"{x} {y}");
    }

    private (string X, string Y) ParseBackgroundPosition()
    {
#if NET8_0_OR_GREATER
        // Optimize: Use stackalloc to avoid heap allocation (max 2 parts) on .NET 8+
        Span<Range> ranges = stackalloc Range[2];
        var count = BackgroundPosition.AsSpan().Split(ranges, ' ');

        static bool IsTopOrBottom(ReadOnlySpan<char> axis)
        {
            return axis.Equals("top", StringComparison.OrdinalIgnoreCase) ||
                   axis.Equals("bottom", StringComparison.OrdinalIgnoreCase);
        }

        switch (count)
        {
            case 1:
                var pos0 = BackgroundPosition.AsSpan(ranges[0]);
                if (IsTopOrBottom(pos0))
                {
                    return ("center", BackgroundPosition[ranges[0]]);
                }

                return (BackgroundPosition[ranges[0]], "center");

            case >= 2:
                var pos0_2 = BackgroundPosition.AsSpan(ranges[0]);
                var pos1 = BackgroundPosition.AsSpan(ranges[1]);

                if (IsTopOrBottom(pos0_2) || (pos0_2.Equals("center", StringComparison.OrdinalIgnoreCase) && IsTopOrBottom(pos1)))
                {
                    return (BackgroundPosition[ranges[1]], BackgroundPosition[ranges[0]]);
                }

                return (BackgroundPosition[ranges[0]], BackgroundPosition[ranges[1]]);

            default:
                return ("center", "top");
        }
#else
        // For .NET 6/7, use standard Split
        var positions = BackgroundPosition.Split(' ');

        static bool IsTopOrBottom(string axis)
        {
            return axis.Equals("top", StringComparison.OrdinalIgnoreCase) ||
                   axis.Equals("bottom", StringComparison.OrdinalIgnoreCase);
        }

        switch (positions.Length)
        {
            case 1:
                if (IsTopOrBottom(positions[0]))
                {
                    return ("center", positions[0]);
                }

                return (positions[0], "center");

            case >= 2:
                if (IsTopOrBottom(positions[0]) || (positions[0].Equals("center", StringComparison.OrdinalIgnoreCase) && IsTopOrBottom(positions[1])))
                {
                    return (positions[1], positions[0]);
                }

                return (positions[0], positions[1]);

            default:
                return ("center", "top");
        }
#endif
    }

    private static (string XPercent, string YPercent) GetBackgroundPositionAsPercentage(string backgroundPositionX, string backgroundPositionY)
    {
        var xPercent = backgroundPositionX;
        var yPercent = backgroundPositionY;

        switch (backgroundPositionX.ToLowerInvariant())
        {
            case "left":
                xPercent = "0%";
                break;

            case "center":
                xPercent = "50%";
                break;

            case "right":
                xPercent = "100%";
                break;

            default:
                if (!backgroundPositionX.Contains('%', StringComparison.OrdinalIgnoreCase))
                {
                    xPercent = "50%";
                }

                break;
        }

        switch (backgroundPositionY.ToLowerInvariant())
        {
            case "top":
                yPercent = "0%";
                break;

            case "center":
                yPercent = "50%";
                break;

            case "bottom":
                yPercent = "100%";
                break;

            default:
                if (!backgroundPositionY.Contains('%', StringComparison.OrdinalIgnoreCase))
                {
                    xPercent = "0%";
                }

                break;
        }

        return (xPercent, yPercent);
    }

    private (string Origin, string Position) GetOriginBasedForAxis(bool isXAxis, string positionX, string positionY)
    {
        var position = isXAxis ? positionX : positionY;

        if (position.Contains('%', StringComparison.OrdinalIgnoreCase))
        {
            var positionUnit = UnitParser.Parse(position);
            var positionUnitDouble = positionUnit.Value / 100.0;

            if (BackgroundRepeat == "repeat")
            {
                var temp = positionUnitDouble.ToInvariantString();

                return (temp, temp);
            }
            else
            {
                var temp = (((positionUnitDouble * 100) - 50.0) / 100).ToInvariantString();

                return (temp, temp);
            }
        }
        else if (BackgroundRepeat == "repeat")
        {
            var temp = isXAxis ? "0.5" : "0";

            return (temp, temp);
        }
        else
        {
            var temp = isXAxis ? "0" : "-0.5";

            return (temp, temp);
        }
    }

    private bool HasBackground()
    {
        return BackgroundUrl != null;
    }
}
