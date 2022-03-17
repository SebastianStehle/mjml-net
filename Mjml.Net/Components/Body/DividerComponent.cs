namespace Mjml.Net.Components.Body
{
    public partial struct DividerProps
    {
        [Bind("align", BindType.Align)]
        public string Align = "center";

        [Bind("border-color", BindType.Color)]
        public string BorderColor = "#000000";

        [Bind("border-style")]
        public string BorderStyle = "solid";

        [Bind("border-width", BindType.Pixels)]
        public string BorderWidth = "4px";

        [Bind("container-background-color", BindType.Color)]
        public string? ContainerBackgroundColor;

        [Bind("padding", BindType.FourPixelsOrPercent)]
        public string Padding = "10px 25px";

        [Bind("padding-bottom", BindType.PixelsOrPercent)]
        public string? PaddingBottom;

        [Bind("padding-left", BindType.PixelsOrPercent)]
        public string? PaddingLeft;

        [Bind("padding-right", BindType.PixelsOrPercent)]
        public string? PaddingRight;

        [Bind("padding-top", BindType.PixelsOrPercent)]
        public string? PaddingTop;

        [Bind("width", BindType.PixelsOrPercent)]
        public string Width = "100%";
    }

    public sealed class DividerComponent : BodyComponentBase<DividerProps>
    {
        public override string ComponentName => "mj-divider";

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var props = new DividerProps(node);

            var borderSetting = $"{props.BorderStyle} {props.BorderWidth} {props.BorderColor}";

            var margin = GetMargin(props.Align);

            renderer.ElementStart("p")
                .Style("border-top", borderSetting)
                .Style("font-size", "1px")
                .Style("margin", margin)
                .Style("width", props.Width);

            renderer.ElementEnd("p");

            var outlookWidth = GetOutlookWidth(renderer, ref props);

            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementStart("table")
                .Attr("align", props.Align)
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

        private static string GetOutlookWidth(IHtmlRenderer renderer, ref DividerProps props)
        {
            var containerWidth = renderer.GetContainerWidth().Value;

            var paddingSize =
                UnitParser.Parse(props.PaddingLeft).Value +
                UnitParser.Parse(props.PaddingRight).Value;

            var width = props.Width!;

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
