namespace Mjml.Net.Components.Body
{
    public partial class ButtonComponent : BodyComponentBase
    {
        public override ContentType ContentType => ContentType.Raw;

        public override string ComponentName => "mj-button";

        [Bind("align")]
        public string Align = "center";

        [Bind("background-color", BindType.Color)]
        public string BackgroundColor = "#414141";

        [Bind("border")]
        public string Border = "none";

        [Bind("border-bottom", BindType.Pixels)]
        public string? BorderBottom;

        [Bind("border-left", BindType.Pixels)]
        public string? BorderLeft;

        [Bind("border-radius", BindType.Pixels)]
        public string BorderRadius = "3px";

        [Bind("border-right", BindType.Pixels)]
        public string? BorderRight;

        [Bind("border-top", BindType.Pixels)]
        public string? BorderTop;

        [Bind("color", BindType.Color)]
        public string Color = "#FFFFFF";

        [Bind("container-background-color", BindType.Color)]
        public string? ContainerBackgroundColor;

        [Bind("font-family")]
        public string FontFamily = "Ubuntu, Helvetica, Arial, sans-serif";

        [Bind("font-size", BindType.Pixels)]
        public string FontSize = "13px";

        [Bind("font-style")]
        public string? FontStyle;

        [Bind("font-weight")]
        public string FontWeight = "normal";

        [Bind("height", BindType.PixelsOrPercent)]
        public string? Height;

        [Bind("href")]
        public string? Href;

        [Bind("inner-padding", BindType.FourPixelsOrPercent)]
        public string InnerPadding = "10px 25px";

        [Bind("letter-spacing", BindType.Pixels)]
        public string? LetterSpacing;

        [Bind("line-height", BindType.PixelsOrPercent)]
        public string LineHeight = "120%";

        [Bind("name")]
        public string? Name;

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

        [Bind("rel")]
        public string? Rel;

        [Bind("target")]
        public string Target = "_blank";

        [Bind("text-align")]
        public string? TextAlign;

        [Bind("text-decoration")]
        public string TextDecoration = "none";

        [Bind("text-transform")]
        public string TextTransform = "none";

        [Bind("vertical-align")]
        public string VerticalAlign = "middle";

        [Bind("width", BindType.PixelsOrPercent)]
        public string? Width;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            var buttonHtmlTag = !string.IsNullOrEmpty(Href) ? "a" : "p";

            renderer.StartElement("table")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("border-collapse", "separate")
                .Style("width", Width)
                .Style("line-height", "100%");

            renderer.StartElement("tr");
            renderer.StartElement("td")
                .Attr("align", "center")
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

            renderer.StartElement(buttonHtmlTag)
                .Attr("href", Href)
                .Attr("rel", Rel)
                .Attr("name", Name)
                .Attr("target", !string.IsNullOrEmpty(Href) ? Target : null)
                .Style("display", "inline-block")
                .Style("width", CalculateButtonWidth(), "px")
                .Style("background", BackgroundColor)
                .Style("color", Color)
                .Style("font-family", FontFamily)
                .Style("font-style", FontStyle)
                .Style("font-size", FontSize)
                .Style("font-weight", FontWeight)
                .Style("line-height", LineHeight)
                .Style("letter-spacing", LetterSpacing)
                .Style("margin", "0")
                .Style("text-decoration", TextDecoration)
                .Style("text-transform", TextTransform)
                .Style("letter-spacing", LetterSpacing)
                .Style("padding", InnerPadding)
                .Style("mso-padding-alt", "0px")
                .Style("text-align", TextAlign)
                .Style("border-radius", BorderRadius);

            RenderRaw(renderer);

            renderer.EndElement(buttonHtmlTag);
            renderer.EndElement("td");
            renderer.EndElement("tr");
            renderer.EndElement("table");
        }

        private double CalculateButtonWidth()
        {
            var widthParsed = UnitParser.Parse(Width);

            if (widthParsed.Value <= 0 || widthParsed.Unit != Unit.Pixels)
            {
                return double.NaN;
            }

            var borders =
                UnitParser.Parse(BorderLeft).Value +
                UnitParser.Parse(BorderRight).Value;

            var innerPadding =
                UnitParser.Parse(PaddingLeft).Value +
                UnitParser.Parse(PaddingRight).Value;

            return widthParsed.Value - innerPadding - borders;
        }
    }
}
