namespace Mjml.Net.Components.Body
{
    public partial class HeroProps
    {
        [Bind("align", BindType.Align)]
        public string? Align;

        [Bind("background-color")]
        public string? BackgroundColor = "#ffffff";

        [Bind("background-height", BindType.PixelsOrPercent)]
        public string? BackgroundHeight;

        [Bind("background-position")]
        public string? BackgroundPosition = "center center";

        [Bind("background-url")]
        public string? BackgroundUrl;

        [Bind("background-width", BindType.PixelsOrPercent)]
        public string? BackgroundWidth;

        [Bind("border-radius")]
        public string? BorderRadius;

        [Bind("container-background-color", BindType.Color)]
        public string? ContainerBackgroundColor;

        [Bind("css-class")]
        public string? CssClass;

        [Bind("height", BindType.PixelsOrPercent)]
        public string? Height = "0px";

        [Bind("inner-background-color", BindType.Color)]
        public string? InnerBackgroundColor;

        [Bind("inner-padding", BindType.FourPixelsOrPercent)]
        public string? InnerPadding;

        [Bind("inner-padding-bottom", BindType.PixelsOrPercent)]
        public string? InnerPaddingBottom;

        [Bind("inner-padding-left", BindType.PixelsOrPercent)]
        public string? InnerPaddingLeft;

        [Bind("inner-padding-right", BindType.PixelsOrPercent)]
        public string? InnerPaddingRight;

        [Bind("inner-padding-top", BindType.PixelsOrPercent)]
        public string? InnerPaddingTop;

        [Bind("mode")]
        public string? Mode = "fixed-height";

        [Bind("padding", BindType.FourPixelsOrPercent)]
        public string? Padding = "0px";

        [Bind("padding-bottom", BindType.PixelsOrPercent)]
        public string? PaddingBottom;

        [Bind("padding-left", BindType.PixelsOrPercent)]
        public string? PaddingLeft;

        [Bind("padding-right", BindType.PixelsOrPercent)]
        public string? PaddingRight;

        [Bind("padding-top", BindType.PixelsOrPercent)]
        public string? PaddingTop;

        [Bind("vertical-align", BindType.VerticalAlign)]
        public string? VerticalAlign = "top";

        [Bind("width", BindType.Pixels)]
        public string? Width;
    }

    public sealed class HeroComponent : BodyComponentBase<HeroProps>
    {
        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            var containerWidth = renderer.GetContainerWidth();

            var parsedBackgroundHeight = UnitParser.Parse(Props.BackgroundHeight);
            var parsedBackgroundWidth = UnitParser.Parse(Props.BackgroundWidth);
            var backgroundString = Props.BackgroundColor;
            if (Props.BackgroundUrl != null)
            {
                backgroundString = $"{backgroundString} url({Props.BackgroundUrl}) no-repeat {Props.BackgroundPosition} / cover";
            }

            var backgroundRatioValue = Math.Round(100 *
                parsedBackgroundHeight.Value /
                parsedBackgroundWidth.Value);
            var backgroundRatio = $"{backgroundRatioValue}px";

            var widthValue = parsedBackgroundWidth.Value;
            if (widthValue <= 0)
            {
                widthValue = containerWidth.Value;
            }

            var width = $"{widthValue}px";

            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementStart("table") // Style: outlook-table
                .Attr("align", "center")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Attr("width", containerWidth.String)
                .Style("width", containerWidth.StringWithUnit);

            renderer.ElementStart("tr");

            renderer.ElementStart("td")
                .Style("line-height", "0")
                .Style("font-size", "0")
                .Style("mso-line-height-rule", "exactly"); // Style: outlook-td

            renderer.ElementStart("v:image") // Style: outlook-image
                .Attr("src", Props.BackgroundUrl)
                .Attr("xmlns:v", "urn:schemas-microsoft-com:vml")
                .Style("border", "0")
                .Style("height", Props.BackgroundHeight)
                .Style("mso-position-horizontal", "center")
                .Style("position", "absolute")
                .Style("top", "0")
                .Style("width", width)
                .Style("z-index", "-3");

            renderer.Content("<![endif]-->");

            renderer.ElementStart("div") // Style div
                .Attr("align", Props.Align)
                .Attr("class", Props.CssClass)
                .Style("margin", "0 auto")
                .Style("max-width", containerWidth.StringWithUnit);

            renderer.ElementStart("table") // Style table
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("width", "100%");

            renderer.ElementStart("tbody");

            renderer.ElementStart("tr")
                .Style("vertical-align", "top"); // Style tr

            if (Props.Mode == "fluid-height")
            {
                static void MagicId(IHtmlRenderer renderer, string backgroundRatio)
                {
                    renderer.ElementStart("td") // Style td-fluid
                        .Style("mso-padding-bottom-alt", "0")
                        .Style("padding-bottom", backgroundRatio)
                        .Style("width", "0.01%");
                    renderer.ElementEnd("td");
                }

                MagicId(renderer, backgroundRatio);

                renderer.ElementStart("td") // Style: hero
                    .Attr("background", Props.BackgroundUrl)
                    .Style("background", backgroundString)
                    .Style("background-position", Props.BackgroundPosition)
                    .Style("background-repeat", "no-repeat")
                    .Style("border-radius", Props.BorderRadius)
                    .Style("padding", Props.Padding)
                    .Style("padding-bottom", Props.PaddingBottom)
                    .Style("padding-left", Props.PaddingLeft)
                    .Style("padding-right", Props.PaddingRight)
                    .Style("padding-top", Props.PaddingTop)
                    .Style("vertical-align", Props.VerticalAlign);

                RenderContent(renderer, containerWidth, context);

                renderer.ElementEnd("td");

                MagicId(renderer, backgroundRatio);
            }
            else
            {
                var height =
                    UnitParser.Parse(Props.Height).Value -
                    UnitParser.Parse(Props.PaddingTop).Value +
                    UnitParser.Parse(Props.PaddingBottom).Value;

                renderer.ElementStart("td") // Style: hero
                    .Attr("background", Props.BackgroundUrl)
                    .Attr("height", height.ToInvariantString())
                    .Style("background", backgroundString)
                    .Style("background-position", Props.BackgroundPosition)
                    .Style("background-repeat", "no-repeat")
                    .Style("border-radius", Props.BorderRadius)
                    .Style("padding", Props.Padding)
                    .Style("padding-bottom", Props.PaddingBottom)
                    .Style("padding-left", Props.PaddingLeft)
                    .Style("padding-right", Props.PaddingRight)
                    .Style("padding-top", Props.PaddingTop)
                    .Style("vertical-align", Props.VerticalAlign);

                RenderContent(renderer, containerWidth, context);

                renderer.ElementEnd("td");
            }

            renderer.ElementEnd("tr");
            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
            renderer.ElementEnd("div");

            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");

            renderer.Content("<![endif]-->");
        }

        private void RenderContent(IHtmlRenderer renderer, ContainerWidth containerWidth, GlobalContext context)
        {
            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementStart("table") // Style: outlook-inner-table
                .Attr("align", Props.Align)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("width", containerWidth.String)
                .Style("width", containerWidth.StringWithUnit);

            renderer.ElementStart("tr");

            renderer.ElementStart("td") // Style: outlook-inner-td
                .Style("background-color", Props.InnerBackgroundColor)
                .Style("inner-padding", Props.InnerPadding)
                .Style("inner-padding-bottom", Props.InnerPaddingBottom)
                .Style("inner-padding-left", Props.InnerPaddingLeft)
                .Style("inner-padding-right", Props.InnerPaddingRight)
                .Style("inner-padding-top", Props.InnerPaddingTop);

            renderer.Content("<![endif]-->");

            renderer.ElementStart("div") // Style: inner-div
                .Attr("align", Props.Align)
                .Class("mj-hero-content")
                .Style("background-color", Props.InnerBackgroundColor)
                .Style("float", Props.Align)
                .Style("margin", "0px auto")
                .Style("width", Props.Width);

            renderer.ElementStart("table") // Style: inner-table
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("width", "100%")
                .Style("margin", "0px");

            renderer.ElementStart("tbody");
            renderer.ElementStart("tr");
            renderer.ElementStart("td");

            renderer.ElementStart("table") // Style: inner-table
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("width", "100%")
                .Style("margin", "0px");

            renderer.ElementStart("tbody");

            var innerWidth =
                containerWidth.Value -
                UnitParser.Parse(Props.PaddingTop).Value +
                UnitParser.Parse(Props.PaddingBottom).Value;

            context.Push();
            context.SetContainerWidth(innerWidth);

            foreach (var child in ChildNodes)
            {
                if (child.Type == ComponentType.Raw)
                {
                    child.Render(renderer, context);
                }
                else
                {
                    var backgroundColor = child.Node.GetAttribute("container-background-color");

                    renderer.ElementStart("tr");

                    renderer.ElementStart("td")
                        .Attr("align", child.Node.GetAttribute("align"))
                        .Attr("background", backgroundColor)
                        .Attr("class", child.Node.GetAttribute("css-class"))
                        .Style("background", backgroundColor)
                        .Style("font-size", "0px")
                        .Style("padding", child.Node.GetAttribute("padding"))
                        .Style("padding-bottom", child.Node.GetAttribute("padding-bottom"))
                        .Style("padding-left", child.Node.GetAttribute("padding-left"))
                        .Style("padding-right", child.Node.GetAttribute("padding-right"))
                        .Style("padding-top", child.Node.GetAttribute("padding-top"))
                        .Style("word-break", "break-word");

                    child.Render(renderer, context);

                    renderer.ElementEnd("td");
                    renderer.ElementEnd("tr");
                }
            }

            context.Pop();

            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
            renderer.ElementEnd("div");

            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");

            renderer.Content("<![endif]-->");
        }
    }
}
