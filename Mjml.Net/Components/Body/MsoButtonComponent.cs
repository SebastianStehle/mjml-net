namespace Mjml.Net.Components.Body
{
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
                var border = Border.Split(" ");

                borderWeight = border.Length > 0 ? border[0] : borderWeight;
                borderStyle = border.Length > 1 ? AdaptBorderStyle(border[1]) : borderStyle;
                borderColor = border.Length == 3 ? border[2] : borderColor;
            }

            renderer.Content("<!--[if mso]>");
            {
                renderer.StartElement("tr");

                renderer.StartElement("td")
                    .Attr("align", Align)
                    .Attr("bgcolor", BackgroundColor)
                    .Attr("role", "presentation")
                    .Attr("valign", VerticalAlign)
                    .Style("border", Border)
                    .Style("border-bottom", BorderBottom)
                    .Style("border-left", BorderLeft)
                    .Style("border-right", BorderRight)
                    .Style("border-top", BorderTop)
                    .Style("border-radius", BorderRadius)
                    .Style("cursor", "auto")
                    .Style("font-style", FontStyle)
                    .Style("height", Height)
                    .Style("mso-padding-alt", InnerPadding)
                    .Style("text-align", TextAlign)
                    .Style("background", BackgroundColor);

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

                renderer.EndElement("td");
                renderer.EndElement("tr");
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
}
