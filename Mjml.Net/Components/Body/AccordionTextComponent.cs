namespace Mjml.Net.Components.Body
{
    public partial class AccordionTextComponent : BodyComponentBase
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mj-accordion-element"
        };

        public override string ComponentName => "mj-accordion-text";

        public override ContentType ContentType => ContentType.Raw;

        public override AllowedParents? AllowedAsChild => Parents;

        [Bind("background-color", BindType.Color)]
        public string? BackgroundColor;

        [Bind("border", BindType.String)]
        public string? Border;

        [Bind("color", BindType.Color)]
        public string? Color;

        [Bind("font-family", BindType.String)]
        public string? FontFamily;

        [Bind("font-size", BindType.Pixels)]
        public string FontSize = "13px";

        [Bind("font-weight", BindType.String)]
        public string? FontWeight;

        [Bind("letter-spacing", BindType.PixelsOrEm)]
        public string? LetterSpacing;

        [Bind("line-height", BindType.PixelsOrPercentOrNone)]
        public string LineHeight = "1";

        [Bind("padding", BindType.PixelsOrPercent)]
        public string Padding = "16px";

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
            renderer.StartElement("div")
                .Class("mj-accordion-content");

            renderer.StartElement("table") // Style table
                .Attr("cell-padding", "0")
                .Attr("cell-spacing", "0")
                .Style("border-bottom", Border)
                .Style("width", "100%");

            renderer.StartElement("tbody");
            renderer.StartElement("tr");

            renderer.StartElement("td") // Style td
                .Class(CssClass)
                .Style("background", BackgroundColor)
                .Style("font-family", FontFamily)
                .Style("font-size", FontSize)
                .Style("font-weight", FontWeight)
                .Style("letter-spacing", LetterSpacing)
                .Style("line-height", LineHeight)
                .Style("color", Color)
                .Style("padding", Padding)
                .Style("padding-bottom", PaddingBottom)
                .Style("padding-left", PaddingLeft)
                .Style("padding-right", PaddingRight)
                .Style("padding-top", PaddingTop);

            RenderRaw(renderer);

            renderer.EndElement("td");
            renderer.EndElement("tr");
            renderer.EndElement("tbody");
            renderer.EndElement("table");
            renderer.EndElement("div");
        }
    }
}
