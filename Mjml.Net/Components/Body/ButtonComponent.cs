namespace Mjml.Net.Components.Body
{
    public partial struct ButtonProps
    {
        [Bind("align")]
        public string? Align = "center";

        [Bind("background-color", BindType.Color)]
        public string? BackgroundColor = "#414141";

        [Bind("border")]
        public string? Border = "none";

        [Bind("border-bottom", BindType.Pixels)]
        public string? BorderBottom;

        [Bind("border-left", BindType.Pixels)]
        public string? BorderLeft;

        [Bind("border-radius", BindType.Pixels)]
        public string? BorderRadius = "3px";

        [Bind("border-right", BindType.Pixels)]
        public string? BorderRight;

        [Bind("border-top", BindType.Pixels)]
        public string? BorderTop;

        [Bind("color", BindType.Color)]
        public string? Color = "#FFFFFF";

        [Bind("container-background-color", BindType.Color)]
        public string? ContainerBackgroundColor;

        [Bind("font-family")]
        public string? FontFamily = "Ubuntu, Helvetica, Arial, sans-serif";

        [Bind("font-size", BindType.Pixels)]
        public string? FontSize = "13px";

        [Bind("font-style")]
        public string? FontStyle;

        [Bind("font-weight")]
        public string? FontWeight = "normal";

        [Bind("height", BindType.PixelsOrPercent)]
        public string? Height;

        [Bind("href")]
        public string? Href;

        [Bind("inner-padding", BindType.FourPixelsOrPercent)]
        public string? InnerPadding = "10px 25px";

        [Bind("letter-spacing", BindType.Pixels)]
        public string? LetterSpacing;

        [Bind("line-height", BindType.PixelsOrPercent)]
        public string? LineHeight = "120%";

        [Bind("name")]
        public string? Name;

        [Bind("padding", BindType.FourPixelsOrPercent)]
        public string? Padding = "10px 25px";

        [Bind("padding-bottom", BindType.PixelsOrPercent)]
        public string? PaddingBottom;

        [Bind("padding-left", BindType.PixelsOrPercent)]
        public string? PaddingLeft;

        [Bind("padding-right", BindType.PixelsOrPercent)]
        public string? PaddingRight;

        [Bind("padding-top", BindType.PixelsOrPercent)]
        public string? PaddingTop;

        [Bind("rel")]
        public string? Rel;

        [Bind("target")]
        public string? Target = "_blank";

        [Bind("text-align")]
        public string? TextAlign;

        [Bind("text-decoration")]
        public string? TextDecoration = "none";

        [Bind("text-transform")]
        public string? TextTransform = "none";

        [Bind("vertical-align")]
        public string? VerticalAlign = "middle";

        [Bind("width", BindType.PixelsOrPercent)]
        public string? Width;
    }

    public sealed class ButtonComponent : BodyComponentBase<ButtonProps>
    {
        public override string ComponentName => "mj-button";

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var props = new ButtonProps(node);

            var backgroundColor = props.BackgroundColor;
            var borderRadius = props.BorderRadius;
            var fontStyle = props.FontStyle;
            var height = props.Height;
            var href = props.Href;
            var innerPadding = props.InnerPadding;
            var textAlign = props.TextAlign;
            var width = props.Width;

            var buttonHtmlTag = !string.IsNullOrEmpty(href) ? "a" : "p";

            renderer.ElementStart("table")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("border-collapse", "separate")
                .Style("width", width)
                .Style("line-height", "100%");

            renderer.ElementStart("tr");
            renderer.ElementStart("td")
                .Attr("align", "center")
                .Attr("bgcolor", backgroundColor)
                .Attr("role", "presentation")
                .Attr("valign", props.VerticalAlign)
                .Style("border", props.Border)
                .Style("border-bottom", props.BorderBottom)
                .Style("border-left", props.BorderLeft)
                .Style("border-right", props.BorderRight)
                .Style("border-top", props.BorderTop)
                .Style("border-radius", borderRadius)
                .Style("cursor", "auto")
                .Style("font-style", fontStyle)
                .Style("height", height)
                .Style("mso-padding-alt", innerPadding)
                .Style("text-align", textAlign)
                .Style("background", backgroundColor);

            renderer.ElementStart(buttonHtmlTag)
                .Attr("href", props.Href)
                .Attr("rel", props.Rel)
                .Attr("name", props.Name)
                .Attr("target", !string.IsNullOrEmpty(href) ? props.Target : null)
                .Style("display", "inline-block")
                .Style("width", CalculateButtonWidth(ref props))
                .Style("background", backgroundColor)
                .Style("color", props.Color)
                .Style("font-family", props.FontFamily)
                .Style("font-style", fontStyle)
                .Style("font-size", props.FontSize)
                .Style("font-weight", props.FontWeight)
                .Style("line-height", props.LineHeight)
                .Style("letter-spacing", props.LetterSpacing)
                .Style("margin", "0")
                .Style("text-decoration", props.TextDecoration)
                .Style("text-transform", props.TextTransform)
                .Style("letter-spacing", props.LetterSpacing)
                .Style("padding", innerPadding)
                .Style("mso-padding-alt", "0px")
                .Style("text-align", textAlign)
                .Style("border-radius", borderRadius);

            renderer.Content(node.GetContent());

            renderer.ElementEnd(buttonHtmlTag);
            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");
        }

        private static string? CalculateButtonWidth(ref ButtonProps props)
        {
            var width = props.Width;

            if (string.IsNullOrEmpty(width) || !width.Contains("px", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var widthParsed = UnitParser.Parse(props.Width).Value;

            var borders =
                UnitParser.Parse(props.BorderLeft).Value +
                UnitParser.Parse(props.BorderRight).Value;

            var innerPadding =
                UnitParser.Parse(props.PaddingLeft).Value +
                UnitParser.Parse(props.PaddingRight).Value;

            return $"{widthParsed - innerPadding - borders}px";
        }
    }
}
