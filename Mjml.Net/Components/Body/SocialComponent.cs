namespace Mjml.Net.Components.Body
{
    public partial class SocialComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-social";

        [Bind("align", BindType.Align)]
        public string Align = "center";

        [Bind("border-radius")]
        public string BorderRadius = "3px";

        [Bind("color")]
        public string Color = "#333333";

        [Bind("container-background-color", BindType.Color)]
        public string ContainerBackgroundColor;

        [Bind("font-family")]
        public string FontFamily = "Ubuntu, Helvetica, Arial, sans-serif";

        [Bind("font-size", BindType.Pixels)]
        public string FontSize = "13px";

        [Bind("font-style")]
        public string? FontStyle;

        [Bind("font-weight")]
        public string? FontWeight;

        [Bind("icon-height", BindType.PixelsOrPercent)]
        public string? IconHeight;

        [Bind("icon-padding", BindType.FourPixelsOrPercent)]
        public string? IconPadding;

        [Bind("icon-size", BindType.PixelsOrPercent)]
        public string IconSize = "20px";

        [Bind("inner-padding", BindType.FourPixelsOrPercent)]
        public string? InnerPadding;

        [Bind("line-height", BindType.PixelsOrPercent)]
        public string LineHeight = "22px";

        [Bind("mode", BindType.SocialMode)]
        public string Mode = "horizontal";

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

        [Bind("table-layout", BindType.SocialTableLayout)]
        public string? TableLayout;

        [Bind("text-decoration")]
        public string TextDecoration = "none";

        [Bind("text-padding", BindType.FourPixelsOrPercent)]
        public string? TextPadding;

        [Bind("vertical-align", BindType.VerticalAlign)]
        public string? VerticalAlign;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            if (Mode == "horizontal")
            {
                RenderHorizontal(renderer, context);
            }
            else
            {
                RenderVertical(renderer, context);
            }
        }

        private void RenderHorizontal(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.Content("<!--[if mso | IE]>");

            renderer.StartElement("table")
                .Attr("align", Align)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation");

            renderer.StartElement("tr");

            foreach (var child in ChildNodes)
            {
                if (child.ContentType == ContentType.Raw)
                {
                    renderer.Content("<![endif]-->");
                    child.Render(renderer, context);
                    renderer.Content("<!--[if mso | IE]>");
                }
                else
                {
                    renderer.StartElement("td");
                    renderer.Content("<![endif]-->");

                    renderer.StartElement("table")
                        .Attr("align", Align)
                        .Attr("border", "0")
                        .Attr("cellpadding", "0")
                        .Attr("cellspacing", "0")
                        .Attr("role", "presentation")
                        .Style("display", "inline-table")
                        .Style("float", "none");

                    renderer.StartElement("tbody");

                    child.Render(renderer, context);

                    renderer.EndElement("tbody");
                    renderer.EndElement("table");

                    renderer.Content("<!--[if mso | IE]>");
                    renderer.EndElement("td");
                }
            }

            renderer.EndElement("tr");
            renderer.EndElement("table");
            renderer.Content("<![endif]-->");
        }

        private void RenderVertical(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.StartElement("table") // Table-vertical
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("margin", "0px");

            renderer.StartElement("tbody");

            RenderChildren(renderer, context);

            renderer.EndElement("tbody");
            renderer.EndElement("table");
        }

        public override string? GetInheritingAttribute(string name)
        {
            switch (name)
            {
                case "border-radius":
                    return BorderRadius;
                case "color":
                    return Color;
                case "font-family":
                    return FontFamily;
                case "font-size":
                    return FontSize;
                case "font-style":
                    return FontStyle;
                case "icon-height":
                    return IconHeight;
                case "icon-padding":
                    return IconPadding;
                case "icon-size":
                    return IconSize;
                case "line-height":
                    return IconHeight;
                case "text-padding":
                    return TextPadding;
                case "text-decoration":
                    return TextDecoration;
                default:
                    return null;
            }
        }
    }
}
