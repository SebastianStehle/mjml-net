#pragma warning disable SA1119 // Statement should not use unnecessary parenthesis


namespace Mjml.Net.Components.Body
{
    public partial class DividerComponent : BodyComponentBase
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mj-column",
            "mj-group"
        };

        public override AllowedParents? AllowedParents => Parents;

        public override string ComponentName => "mj-divider";

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

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            var borderSetting = FormattableString.Invariant($"{BorderStyle} {BorderWidth} {BorderColor}");

            var margin = GetMargin(Align);

            renderer.StartElement("p")
                .Style("border-top", borderSetting)
                .Style("font-size", "1px")
                .Style("margin", margin)
                .Style("width", Width);

            renderer.EndElement("p");

            var outlookWidth = GetOutlookWidth();

            renderer.StartConditional("<!--[if mso | IE]>");
            {
                renderer.StartElement("table")
                    .Attr("align", Align)
                    .Attr("border", "0")
                    .Attr("cellpadding", "0")
                    .Attr("cellspacing", "0")
                    .Attr("role", "presentation")
                    .Attr("width", $"{outlookWidth}px")
                    .Style("border-top", borderSetting)
                    .Style("font-size", "1px")
                    .Style("margin", margin)
                    .Style("width", $"{outlookWidth}px");

                renderer.StartElement("tr");

                renderer.StartElement("td")
                    .Attr("style", "height:0; line-height:0;");

                renderer.Content("&nbsp;");

                renderer.EndElement("td");
                renderer.EndElement("tr");
                renderer.EndElement("table");
            }
            renderer.EndConditional("<![endif]-->");
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

        private double GetOutlookWidth()
        {
            var paddingSize =
                UnitParser.Parse(PaddingLeft).Value +
                UnitParser.Parse(PaddingRight).Value;

            var (width, unit) = UnitParser.Parse(Width);

            switch (unit)
            {
                case Unit.Pixels:
                    return width;
                case Unit.Percent:
                    return (ActualWidth - paddingSize) * (width / 100);
                default:
                    return (ActualWidth - paddingSize);
            }
        }
    }
}
