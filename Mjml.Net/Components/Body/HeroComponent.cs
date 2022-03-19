#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
using Mjml.Net.Extensions;

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

            var backgroundRatio = Math.Round(100 *
                backgroundHeight.Value /
                backgroundWidth.Value);

            var width = backgroundWidth.Value;
            if (width <= 0)
            {
                width = ContainerWidth.Value;
            }

            renderer.Content("<!--[if mso | IE]>");

            renderer.StartElement("table") // Style: outlook-table
                .Attr("align", "center")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Attr("width", ContainerWidth.String)
                .Style("width", ContainerWidth.StringWithUnit);

            renderer.StartElement("tr");

            renderer.StartElement("td")
                .Style("line-height", "0")
                .Style("font-size", "0")
                .Style("mso-line-height-rule", "exactly"); // Style: outlook-td

            renderer.StartElement("v:image") // Style: outlook-image
                .Attr("src", BackgroundUrl)
                .Attr("xmlns:v", "urn:schemas-microsoft-com:vml")
                .Style("border", "0")
                .Style("height", BackgroundHeight)
                .Style("mso-position-horizontal", "center")
                .Style("position", "absolute")
                .Style("top", "0")
                .Style("width", width, "px")
                .Style("z-index", "-3");

            renderer.Content("<![endif]-->");

            renderer.StartElement("div") // Style div
                .Attr("align", Align)
                .Attr("class", CssClass)
                .Style("margin", "0 auto")
                .Style("max-width", ContainerWidth.StringWithUnit);

            renderer.StartElement("table") // Style table
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("width", "100%");

            renderer.StartElement("tbody");

            renderer.StartElement("tr")
                .Style("vertical-align", "top"); // Style tr

            if (Mode == "fluid-height")
            {
                static void MagicId(IHtmlRenderer renderer, double backgroundRatio)
                {
                    renderer.StartElement("td") // Style td-fluid
                        .Style("mso-padding-bottom-alt", "0")
                        .Style("padding-bottom", backgroundRatio, "px")
                        .Style("width", "0.01%");
                    renderer.EndElement("td");
                }

                MagicId(renderer, backgroundRatio);

                renderer.StartElement("td") // Style: hero
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

                renderer.EndElement("td");

                MagicId(renderer, backgroundRatio);
            }
            else
            {
                var height =
                    UnitParser.Parse(Height).Value -
                    UnitParser.Parse(PaddingTop).Value +
                    UnitParser.Parse(PaddingBottom).Value;

                renderer.StartElement("td") // Style: hero
                    .Attr("background", BackgroundUrl)
                    .Attr("height", height)
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

                renderer.EndElement("td");
            }

            renderer.EndElement("tr");
            renderer.EndElement("tbody");
            renderer.EndElement("table");
            renderer.EndElement("div");

            renderer.Content("<!--[if mso | IE]>");

            renderer.EndElement("td");
            renderer.EndElement("tr");
            renderer.EndElement("table");

            renderer.Content("<![endif]-->");
        }

        private void RenderContent(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.Content("<!--[if mso | IE]>");

            renderer.StartElement("table") // Style: outlook-inner-table
                .Attr("align", Align)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("width", ContainerWidth.String)
                .Style("width", ContainerWidth.StringWithUnit);

            renderer.StartElement("tr");

            renderer.StartElement("td") // Style: outlook-inner-td
                .Style("background-color", InnerBackgroundColor)
                .Style("inner-padding", InnerPadding)
                .Style("inner-padding-bottom", InnerPaddingBottom)
                .Style("inner-padding-left", InnerPaddingLeft)
                .Style("inner-padding-right", InnerPaddingRight)
                .Style("inner-padding-top", InnerPaddingTop);

            renderer.Content("<![endif]-->");

            renderer.StartElement("div") // Style: inner-div
                .Attr("align", Align)
                .Class("mj-hero-content")
                .Style("background-color", InnerBackgroundColor)
                .Style("float", Align)
                .Style("margin", "0px auto")
                .Style("width", Width);

            renderer.StartElement("table") // Style: inner-table
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("width", "100%")
                .Style("margin", "0px");

            renderer.StartElement("tbody");
            renderer.StartElement("tr");
            renderer.StartElement("td");

            renderer.StartElement("table") // Style: inner-table
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("width", "100%")
                .Style("margin", "0px");

            renderer.StartElement("tbody");

            var innerWidth =
                ContainerWidth.Value -
                UnitParser.Parse(PaddingTop).Value -
                UnitParser.Parse(PaddingBottom).Value;

            if (innerWidth != ContainerWidth.Value)
            {
                context.Push();
                context.SetContainerWidth(innerWidth);
            }

            foreach (var child in ChildNodes)
            {
                if (child.Raw)
                {
                    child.Render(renderer, context);
                }
                else
                {
                    var backgroundColor = child.Node.GetAttribute("container-background-color");

                    renderer.StartElement("tr");

                    renderer.StartElement("td")
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

                    renderer.EndElement("td");
                    renderer.EndElement("tr");
                }
            }

            if (innerWidth != ContainerWidth.Value)
            {
                context.Pop();
            }

            renderer.EndElement("tbody");
            renderer.EndElement("table");
            renderer.EndElement("td");
            renderer.EndElement("tr");
            renderer.EndElement("tbody");
            renderer.EndElement("table");
            renderer.EndElement("div");

            renderer.Content("<!--[if mso | IE]>");

            renderer.EndElement("td");
            renderer.EndElement("tr");
            renderer.EndElement("table");

            renderer.Content("<![endif]-->");
        }
    }
}
