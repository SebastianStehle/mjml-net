namespace Mjml.Net.Components.Body
{
    public partial class ButtonProps
    {
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
    }

    public sealed class ButtonComponent : BodyComponentBase<ButtonProps>
    {
        public override ComponentType Type => ComponentType.Raw;

        public override string Name => "mj-button";

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            var buttonHtmlTag = !string.IsNullOrEmpty(Props.Href) ? "a" : "p";

            renderer.ElementStart("table")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("border-collapse", "separate")
                .Style("width", Props.Width)
                .Style("line-height", "100%");

            renderer.ElementStart("tr");
            renderer.ElementStart("td")
                .Attr("align", "center")
                .Attr("bgcolor", Props.BackgroundColor)
                .Attr("role", "presentation")
                .Attr("valign", Props.VerticalAlign)
                .Style("border", Props.Border)
                .Style("border-bottom", Props.BorderBottom)
                .Style("border-left", Props.BorderLeft)
                .Style("border-right", Props.BorderRight)
                .Style("border-top", Props.BorderTop)
                .Style("border-radius", Props.BorderRadius)
                .Style("cursor", "auto")
                .Style("font-style", Props.FontStyle)
                .Style("height", Props.Height)
                .Style("mso-padding-alt", Props.InnerPadding)
                .Style("text-align", Props.TextAlign)
                .Style("background", Props.BackgroundColor);

            renderer.ElementStart(buttonHtmlTag)
                .Attr("href", Props.Href)
                .Attr("rel", Props.Rel)
                .Attr("name", Props.Name)
                .Attr("target", !string.IsNullOrEmpty(Props.Href) ? Props.Target : null)
                .Style("display", "inline-block")
                .Style("width", CalculateButtonWidth())
                .Style("background", Props.BackgroundColor)
                .Style("color", Props.Color)
                .Style("font-family", Props.FontFamily)
                .Style("font-style", Props.FontStyle)
                .Style("font-size", Props.FontSize)
                .Style("font-weight", Props.FontWeight)
                .Style("line-height", Props.LineHeight)
                .Style("letter-spacing", Props.LetterSpacing)
                .Style("margin", "0")
                .Style("text-decoration", Props.TextDecoration)
                .Style("text-transform", Props.TextTransform)
                .Style("letter-spacing", Props.LetterSpacing)
                .Style("padding", Props.InnerPadding)
                .Style("mso-padding-alt", "0px")
                .Style("text-align", Props.TextAlign)
                .Style("border-radius", Props.BorderRadius);

            RenderRaw(renderer);

            renderer.ElementEnd(buttonHtmlTag);
            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");
        }

        private string? CalculateButtonWidth()
        {
            var widthParsed = UnitParser.Parse(Props.Width);

            if (widthParsed.Value <= 0 || widthParsed.Unit != Unit.Pixels)
            {
                return null;
            }

            var borders =
                UnitParser.Parse(Props.BorderLeft).Value +
                UnitParser.Parse(Props.BorderRight).Value;

            var innerPadding =
                UnitParser.Parse(Props.PaddingLeft).Value +
                UnitParser.Parse(Props.PaddingRight).Value;

            return $"{widthParsed.Value - innerPadding - borders}px";
        }
    }
}
