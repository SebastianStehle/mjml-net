namespace Mjml.Net.Components.Body;

public partial class MsoButtonComponent : ButtonComponent
{
    public override string ComponentName => "mj-msobutton";

    [Bind("mso-proof", BindType.Boolean)]
    public string? MsoProof = "false";

    [Bind("mso-height", BindType.PixelsOrPercent)]
    public string? MsoHeight;

    [Bind("mso-width", BindType.PixelsOrPercent)]
    public string? MsoWidth;

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        var isMsoProof = string.Equals(MsoProof, "true", StringComparison.OrdinalIgnoreCase);

        if (isMsoProof)
        {
            RenderMso(renderer);

            renderer.Content("<!--[if !mso]><!-->");
            {
                base.Render(renderer, context);
            }
            renderer.Content("<!--<![endif]-->");
        }
        else
        {
            base.Render(renderer, context);
        }
    }

    private void RenderMso(IHtmlRenderer renderer)
    {
        var borderWeight = "0pt";
        var borderStyle = "Solid";
        var borderColor = "#000000";
        var stroked = Border != "none";
        var hasBackgroundColor = !string.IsNullOrWhiteSpace(BackgroundColor) && BackgroundColor != "none";

        if (stroked)
        {
#if NET8_0_OR_GREATER
            // Optimize: Use stackalloc to avoid heap allocation (max 3 parts) on .NET 8+
            Span<Range> ranges = stackalloc Range[3];
            var count = Border.AsSpan().Split(ranges, ' ');

            borderWeight = count > 0 ? Border[ranges[0]] : borderWeight;
            borderStyle = count > 1 ? AdaptBorderStyle(Border[ranges[1]]) : borderStyle;
            borderColor = count >= 3 ? Border[ranges[2]] : borderColor;
#else
            // For .NET 6/7, use standard Split
            var border = Border.Split(" ");

            borderWeight = border.Length > 0 ? border[0] : borderWeight;
            borderStyle = border.Length > 1 ? AdaptBorderStyle(border[1]) : borderStyle;
            borderColor = border.Length >= 3 ? border[2] : borderColor;
#endif
        }

        renderer.Content("<!--[if mso]>");
        {
            renderer.StartElement("div");
            renderer.StartElement("v:roundrect")
                .Attr("xmlns:v", "urn:schemas-microsoft-com:vml")
                .Attr("xmlns:w", "urn:schemas-microsoft-com:office:word")
                .Attr("fill", hasBackgroundColor ? "t" : "f")
                .Attr("strokeweight", borderWeight)
                .Attr("strokecolor", borderColor)
                .Attr("stroked", stroked ? "t" : "f")
                .Attr("arcsize", CalculateArcsize())
                .Attr("href", Href)
                .Style("height", MsoHeight ?? Height)
                .Style("width", MsoWidth ?? Width)
                .Style("padding", Padding)
                .Style("v-text-anchor", "middle");

            if (stroked)
            {
                renderer.StartElement("v:stroke", true)
                    .Attr("dashstyle", borderStyle);
            }

            if (hasBackgroundColor)
            {
                renderer.StartElement("v:fill", true)
                    .Attr("color", BackgroundColor);
            }

            renderer.StartElement("w:anchorlock", true);
            renderer.StartElement("center");

            RenderButton(renderer);

            renderer.EndElement("center");
            renderer.EndElement("v:roundrect");
            renderer.EndElement("div");
        }
        renderer.Content("<![endif]-->");
    }

    private string CalculateArcsize()
    {
        var radius = UnitParser.Parse(BorderRadius);

        if (radius.Value <= 0)
        {
            return string.Empty;
        }

        if (radius.Unit == Unit.Percent)
        {
            return $"{radius.Value}%";
        }

        if (radius.Unit == Unit.Pixels)
        {
            var height = UnitParser.Parse(MsoHeight ?? Height);
            var arcsize = ConvertBorderRadiusToArcsize(height.Value, radius.Value);

            return $"{arcsize}%";
        }

        return string.Empty;
    }

    private static string AdaptBorderStyle(string cssBorderStyle)
    {
        return cssBorderStyle switch
        {
            "dotted" => "Dot",
            "dashed" => "Dash",
            _ => "Solid"
        };
    }

    private static double ConvertBorderRadiusToArcsize(double boxHeight, double borderRadius)
    {
        const double defaultArcsize = 8;

        return borderRadius > boxHeight
            ? defaultArcsize
            : Math.Round(borderRadius / boxHeight * 100, MidpointRounding.AwayFromZero);
    }
}
