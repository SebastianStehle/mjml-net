using Mjml.Net.Extensions;

namespace Mjml.Net.Components.Body
{
    public partial class TableComponent : BodyComponentBase
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mj-column",
            "mj-hero"
        };

        public override AllowedParents? AllowedParents => Parents;

        public override ContentType ContentType => ContentType.Raw;

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

        [Bind("line-height", BindType.PixelsOrPercentOrNone)]
        public string LineHeight = "22px";

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

        [Bind("role", BindType.String)]
        public string? Role;

        [Bind("table-layout", BindType.String)]
        public string TableLayout = "auto";

        [Bind("vertical-align", BindType.VerticalAlign)]
        public string? VerticalAlign;

        [Bind("width", BindType.PixelsOrPercent)]
        public string Width = "100%";

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            var (widthValue, widthUnit) = UnitParser.Parse(Width);

            renderer.StartElement("table") // Style table
                .Attr("border", "0")
                .Attr("cellpadding", CellPadding)
                .Attr("cellspacing", CellSpacing)
                .Attr("role", Role)
                .Attr("width", widthUnit == Unit.Percent ? Width : widthValue.ToInvariantString())
                .Style("border", Border)
                .Style("color", Color)
                .Style("font-family", FontFamily)
                .Style("font-size", FontSize)
                .Style("line-height", LineHeight)
                .Style("table-layout", TableLayout)
                .Style("width", Width);

            RenderRaw(renderer);

            renderer.EndElement("table");
        }
    }
}
