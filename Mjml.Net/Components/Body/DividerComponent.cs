namespace Mjml.Net.Components.Body
{
    public sealed class DividerComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-divider";

        public override AllowedAttributes? AllowedAttributes { get; } =
            new AllowedAttributes
            {
                ["align"] = AttributeTypes.Align,
                ["border-color"] = AttributeTypes.Color,
                ["border-style"] = AttributeTypes.String,
                ["border-width"] = AttributeTypes.Pixels,
                ["container-background-color"] = AttributeTypes.Color,
                ["padding"] = AttributeTypes.FourPixelsOrPercent,
                ["padding-bottom"] = AttributeTypes.PixelsOrPercent,
                ["padding-left"] = AttributeTypes.PixelsOrPercent,
                ["padding-right"] = AttributeTypes.PixelsOrPercent,
                ["padding-top"] = AttributeTypes.PixelsOrPercent,
                ["width"] = AttributeTypes.PixelsOrPercent,
            };

        public override Attributes? DefaultAttributes { get; } =
            new Attributes
            {
                ["align"] = "center",
                ["border-color"] = "#000000",
                ["border-style"] = "solid",
                ["border-width"] = "4px",
                ["padding"] = "10px 25px",
                ["width"] = "100%",
            };

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var align = node.GetAttribute("align");
            var borderStyle = node.GetAttribute("border-style");
            var borderWidth = node.GetAttribute("border-width");
            var borderColor = node.GetAttribute("border-color");
            var borderSetting = $"{borderStyle} {borderWidth} {borderColor}";
            var margin = GetMargin(node.GetAttribute("align"));

            renderer.ElementStart("p")
                .Style("border-top", borderSetting)
                .Style("font-size", "1px")
                .Style("margin", margin)
                .Style("width", node.GetAttribute("width"));

            renderer.ElementEnd("p");

            var outlookWidth = GetOutlookWidth(renderer, node);

            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementStart("table")
                .Attr("align", align)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Attr("width", outlookWidth)
                .Style("border-top", borderSetting)
                .Style("font-size", "1px")
                .Style("margin", margin)
                .Style("width", outlookWidth);

            renderer.ElementStart("tr");

            renderer.ElementStart("td")
                .Attr("style", "height:0; line-height:0;");

            renderer.Content("&nbsp;");

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");

            renderer.Content("<![endif]-->");
        }

        private static string GetMargin(string? align)
        {
            switch (align)
            {
                case "left":
                    return "0px";
                case "right":
                    return "0px 0px 0px auto";
                default:
                    return "0px auto";
            }
        }

        private static string GetOutlookWidth(IHtmlRenderer renderer, INode node)
        {
            var containerWidth = renderer.GetContainerWidth().Value;

            var paddingSize =
                node.GetShorthandAttributeValue("padding-left", "padding") +
                node.GetShorthandAttributeValue("padding-right", "padding");

            var width = node.GetAttribute("width")!;

            var (parsedWidth, unit) = UnitParser.Parse(width);

            switch (unit)
            {
                case Unit.Percent:
                    return $"{(containerWidth - paddingSize) * (parsedWidth / 100)}px";
                case Unit.Pixels:
                    return width;
                default:
                    return $"{containerWidth - paddingSize}px";
            }
        }
    }
}
