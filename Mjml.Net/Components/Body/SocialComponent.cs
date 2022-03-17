namespace Mjml.Net.Components.Body
{
    public partial class SocialComponentProps
    {
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
    }

    public sealed class SocialComponent : BodyComponentBase<SocialComponentProps>
    {
        public override string Name => "mj-social";

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            if (Props.Mode == "horizontal")
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

            renderer.ElementStart("table")
                .Attr("align", Props.Align)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation");

            renderer.ElementStart("tr");

            foreach (var child in ChildNodes)
            {
                if (child.Type == ComponentType.Raw)
                {
                    renderer.Content("<![endif]-->");

                    child.Render(renderer, context);

                    renderer.Content("<!--[if mso | IE]>");
                }
                else
                {
                    renderer.ElementStart("td");
                    renderer.Content("<![endif]-->");

                    renderer.ElementStart("table")
                        .Attr("align", child.Node.GetAttribute("align"))
                        .Attr("border", "0")
                        .Attr("cellpadding", "0")
                        .Attr("cellspacing", "0")
                        .Attr("role", "presentation")
                        .Style("display", "inline-table")
                        .Style("float", "none");

                    renderer.ElementStart("tbody");

                    child.Render(renderer, context);

                    renderer.ElementEnd("tbody");
                    renderer.ElementEnd("table");

                    renderer.Content("<!--[if mso | IE]>");
                    renderer.ElementEnd("td");
                }
            }

            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");
            renderer.Content("<![endif]-->");
        }

        private void RenderVertical(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.ElementStart("table") // Table-vertical
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("margin", "0px");

            renderer.ElementStart("tbody");

            RenderChildren(renderer, context);

            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
        }

        public override string? GetInheritingAttribute(string name)
        {
            switch (name)
            {
                case "border-radius":
                    return Props.BorderRadius;
                case "color":
                    return Props.Color;
                case "font-family":
                    return Props.FontFamily;
                case "font-size":
                    return Props.FontSize;
                case "font-style":
                    return Props.FontStyle;
                case "icon-height":
                    return Props.IconHeight;
                case "icon-padding":
                    return Props.IconPadding;
                case "icon-size":
                    return Props.IconSize;
                case "line-height":
                    return Props.IconHeight;
                case "text-padding":
                    return Props.TextPadding;
                case "text-decoration":
                    return Props.TextDecoration;
                default:
                    return null;
            }
        }
    }
}
