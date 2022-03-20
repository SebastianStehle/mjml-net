namespace Mjml.Net.Components.Body
{
    public partial class TableComponent : Component
    {
        public override string ComponentName => "mj-table";

        [Bind("align", BindType.Align)]
        public string Align = "left";

        [Bind("border", BindType.String)]
        public string Border = "none";

        [Bind("cellpadding", BindType.String)]
        public string CellPadding = "0";

        [Bind("cellspacing", BindType.String)]
        public string CellSpacing = "0";

        [Bind("color", BindType.Color)]
        public string Color = "#000000";

        [Bind("container-background-color", BindType.Color)]
        public string? ContainerBackgroundColor;

        [Bind("font-family", BindType.String)]
        public string FontFamily = "Ubuntu, Helvetica, Arial, sans-serif";

        [Bind("font-size", BindType.Pixels)]
        public string FontSize = "13px";

        [Bind("font-weight", BindType.String)]
        public string? FontWeight;

        [Bind("line-height", BindType.PixelsOrPercent)]
        public string LineHeight = "22px";

        [Bind("padding", BindType.PixelsOrPercent)]
        public string Padding = "10px 25px";

        [Bind("padding-bottom", BindType.PixelsOrPercent)]
        public string? PaddingBottom;

        [Bind("padding-left", BindType.PixelsOrPercent)]
        public string? PaddingLeft;

        [Bind("padding-right", BindType.PixelsOrPercent)]
        public string? PaddingRight;

        [Bind("padding-top", BindType.PixelsOrPercent)]
        public string? PaddingTop;

        [Bind("role", BindType.String)]
        public string? Role;

        [Bind("table-layout", BindType.SocialTableLayout)]
        public string TableLayout = "auto";

        [Bind("vertical-align", BindType.VerticalAlign)]
        public string? VerticalAlign;

        [Bind("width", BindType.PixelsOrPercent)]
        public string Width = "100%";

        public (double Value, Unit Unit, string WidthString, double InnerWidth) CurrentWidth;

        public override void Measure(int parentWidth, int numSiblings, int numNonRawSiblings)
        {
            var widthValue = 0d;
            var widthUnit = Unit.Pixels;
            var widthString = string.Empty;

            if (Width != null)
            {
                (widthValue, widthUnit) = UnitParser.Parse(Width);
                widthString = Width;
            }
            else
            {
                widthValue = 100d;
                widthUnit = Unit.Percent;
                widthString = $"{widthValue}%";
            }

            if (widthUnit != Unit.Pixels)
            {
                ActualWidth = (int)(parentWidth * widthValue / 100);
            }
            else
            {
                ActualWidth = (int)widthValue;
            }

            CurrentWidth = (widthValue, widthUnit, widthString, ActualWidth);
        }

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.StartElement("table") // Style table
                .Attr("border", "0")
                .Attr("cellpadding", CellPadding)
                .Attr("cellspacing", CellSpacing)
                .Attr("role", Role)
                .Attr("width", $"{CurrentWidth.Value}")
                .Style("border", Border)
                .Style("color", Color)
                .Style("font-family", FontFamily)
                .Style("font-size", FontSize)
                .Style("line-height", LineHeight)
                .Style("table-layout", TableLayout)
                .Style("width", CurrentWidth.WidthString);

            RenderRaw(renderer);

            renderer.EndElement("table");
        }
    }
}
