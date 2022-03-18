namespace Mjml.Net.Components.Body
{
    public partial class HeroComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-hero";

        [Bind("align", BindType.Align)]
        public string? Align;

        [Bind("background-color")]
        public string BackgroundColor = "#ffffff";

        [Bind("background-height", BindType.PixelsOrPercent)]
        public string? BackgroundHeight;

        [Bind("background-position")]
        public string BackgroundPosition = "center center";

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
        public string Height = "0px";

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
        public string Mode = "fixed-height";

        [Bind("padding", BindType.FourPixelsOrPercent)]
        public string Padding = "0px";

        [Bind("padding-bottom", BindType.PixelsOrPercent)]
        public string? PaddingBottom;

        [Bind("padding-left", BindType.PixelsOrPercent)]
        public string? PaddingLeft;

        [Bind("padding-right", BindType.PixelsOrPercent)]
        public string? PaddingRight;

        [Bind("padding-top", BindType.PixelsOrPercent)]
        public string? PaddingTop;

        [Bind("vertical-align", BindType.VerticalAlign)]
        public string VerticalAlign = "top";

        [Bind("width", BindType.Pixels)]
        public string? Width;

        public ContainerWidth ContainerWidth;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            ContainerWidth = context.GetContainerWidth();

            var backgroundHeight = UnitParser.Parse(BackgroundHeight);
            var backgroundWidth = UnitParser.Parse(BackgroundWidth);
            var backgroundString = BackgroundColor;

            if (BackgroundUrl != null)
            {
                backgroundString = $"{backgroundString} url({BackgroundUrl}) no-repeat {BackgroundPosition} / cover";
            }

            var backgroundRatioValue = Math.Round(100 *
                backgroundHeight.Value /
                backgroundWidth.Value);
            var backgroundRatio = $"{backgroundRatioValue}px";

            var widthValue = backgroundWidth.Value;
            if (widthValue <= 0)
            {
                widthValue = ContainerWidth.Value;
            }

            var width = $"{widthValue}px";

            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementStart("table") // Style: outlook-table
                .Attr("align", "center")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Attr("width", ContainerWidth.String)
                .Style("width", ContainerWidth.StringWithUnit);

            renderer.ElementStart("tr");

            renderer.ElementStart("td")
                .Style("line-height", "0")
                .Style("font-size", "0")
                .Style("mso-line-height-rule", "exactly"); // Style: outlook-td

            renderer.ElementStart("v:image") // Style: outlook-image
                .Attr("src", BackgroundUrl)
                .Attr("xmlns:v", "urn:schemas-microsoft-com:vml")
                .Style("border", "0")
                .Style("height", BackgroundHeight)
                .Style("mso-position-horizontal", "center")
                .Style("position", "absolute")
                .Style("top", "0")
                .Style("width", width)
                .Style("z-index", "-3");

            renderer.Content("<![endif]-->");

            renderer.ElementStart("div") // Style div
                .Attr("align", Align)
                .Attr("class", CssClass)
                .Style("margin", "0 auto")
                .Style("max-width", ContainerWidth.StringWithUnit);

            renderer.ElementStart("table") // Style table
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("width", "100%");

            renderer.ElementStart("tbody");

            renderer.ElementStart("tr")
                .Style("vertical-align", "top"); // Style tr

            if (Mode == "fluid-height")
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
                    .Attr("background", BackgroundUrl)
                    .Style("background", backgroundString)
                    .Style("background-position", BackgroundPosition)
                    .Style("background-repeat", "no-repeat")
                    .Style("border-radius", BorderRadius)
                    .Style("padding", Padding)
                    .Style("padding-bottom", PaddingBottom)
                    .Style("padding-left", PaddingLeft)
                    .Style("padding-right", PaddingRight)
                    .Style("padding-top", PaddingTop)
                    .Style("vertical-align", VerticalAlign);

                RenderContent(renderer, context);

                renderer.ElementEnd("td");

                MagicId(renderer, backgroundRatio);
            }
            else
            {
                var height =
                    UnitParser.Parse(Height).Value -
                    UnitParser.Parse(PaddingTop).Value +
                    UnitParser.Parse(PaddingBottom).Value;

                renderer.ElementStart("td") // Style: hero
                    .Attr("background", BackgroundUrl)
                    .Attr("height", height.ToInvariantString())
                    .Style("background", backgroundString)
                    .Style("background-position", BackgroundPosition)
                    .Style("background-repeat", "no-repeat")
                    .Style("border-radius", BorderRadius)
                    .Style("padding", Padding)
                    .Style("padding-bottom", PaddingBottom)
                    .Style("padding-left", PaddingLeft)
                    .Style("padding-right", PaddingRight)
                    .Style("padding-top", PaddingTop)
                    .Style("vertical-align", VerticalAlign);

                RenderContent(renderer, context);

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

        private void RenderContent(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementStart("table") // Style: outlook-inner-table
                .Attr("align", Align)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("width", ContainerWidth.String)
                .Style("width", ContainerWidth.StringWithUnit);

            renderer.ElementStart("tr");

            renderer.ElementStart("td") // Style: outlook-inner-td
                .Style("background-color", InnerBackgroundColor)
                .Style("inner-padding", InnerPadding)
                .Style("inner-padding-bottom", InnerPaddingBottom)
                .Style("inner-padding-left", InnerPaddingLeft)
                .Style("inner-padding-right", InnerPaddingRight)
                .Style("inner-padding-top", InnerPaddingTop);

            renderer.Content("<![endif]-->");

            renderer.ElementStart("div") // Style: inner-div
                .Attr("align", Align)
                .Class("mj-hero-content")
                .Style("background-color", InnerBackgroundColor)
                .Style("float", Align)
                .Style("margin", "0px auto")
                .Style("width", Width);

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
                ContainerWidth.Value -
                UnitParser.Parse(PaddingTop).Value +
                UnitParser.Parse(PaddingBottom).Value;

            context.Push();
            context.SetContainerWidth(innerWidth);

            foreach (var child in ChildNodes)
            {
                if (child.Raw)
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
