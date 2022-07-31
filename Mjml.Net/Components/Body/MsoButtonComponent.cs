using Mjml.Net.Extensions;
using Mjml.Net.Types;

namespace Mjml.Net.Components.Body
{
    public partial class MsoButtonComponent : ButtonComponent
    {
        private Dictionary<string, string> BorderstyleAdapter = new()
        {
            { "solid", "Solid" },
            { "dotted", "Dot" },
            { "dashed", "Dash" },
            { "double", "Solid" },
            { "groove", "Solid" },
            { "ridge", "Solid" },
            { "inset", "Solid" },
            { "outset", "Solid" },
            { "none", "Solid" },
            { "hidden", "Solid" }
        };
        public override string ComponentName => "mj-msobutton";

        [Bind("mso-proof", BindType.Boolean)]
        public string? MsoProof = "false";

        [Bind("mso-height", BindType.PixelsOrPercent)]
        public string? MsoHeight;

        [Bind("mso-width", BindType.PixelsOrPercent)]
        public string? MsoWidth;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            var isMsoProof = MsoProof == "true";
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
            var borderAttributes = new[] { "0pt", "Solid", "#000000" };
            var stroked = Border != "none";
            var hasBackgroundColor = !string.IsNullOrEmpty(BackgroundColor) && BackgroundColor != "none";
            if (stroked)
            {
                var border = Border.Split(" ");
                borderAttributes = new[]
                {
                    border.Length > 0 ? border[0] : borderAttributes[0],
                    border.Length > 1 ? BorderstyleAdapter[border[1]] : borderAttributes[1],
                    border.Length == 3 ? border[2] : borderAttributes[2],
                };
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
                    .Attr("strokeweight", stroked ? borderAttributes[0] : "0pt")
                    .Attr("strokecolor", borderAttributes[2])
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
                        .Attr("dashstyle", borderAttributes[1]);
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
                const string defaultArcsize = "8%";
                var height = UnitParser.Parse(MsoHeight ?? Height);
                var percent = Math.Round(radius.Value / height.Value * 100, MidpointRounding.AwayFromZero);
                return radius.Value > height.Value ? defaultArcsize : $"{percent}%";
            }

            return string.Empty;
        }


    }
}
