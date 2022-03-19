using Mjml.Net.Extensions;
using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Body
{
    public partial class ImageComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-image";

        [Bind("align", BindType.Align)]
        public string Align = "center";

        [Bind("alt")]
        public string? Alt;

        [Bind("border")]
        public string Border = "0";

        [Bind("border-bottom")]
        public string? BorderBottom;

        [Bind("border-left")]
        public string? BorderLeft;

        [Bind("border-radius", BindType.FourPixelsOrPercent)]
        public string? BorderRadius;

        [Bind("border-right")]
        public string? BorderRight;

        [Bind("border-top")]
        public string? BorderTop;

        [Bind("container-background-color", BindType.Color)]
        public string? ContainerBackgroundColor;

        [Bind("fluid-on-mobile", BindType.Boolean)]
        public string? FluidOnMobile;

        [Bind("font-size", BindType.Pixels)]
        public string FontSize = "13px";

        [Bind("height", BindType.PixelsOrAuto)]
        public string Height = "auto";

        [Bind("href")]
        public string? Href;

        [Bind("max-height", BindType.PixelsOrPercent)]
        public string? MaxHeight;

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

        [Bind("sizes")]
        public string? Sizes;

        [Bind("src")]
        public string? Src;

        [Bind("srcset")]
        public string? Srcset;

        [Bind("target")]
        public string Target = "_blank";

        [Bind("title")]
        public string? Title;

        [Bind("usemap")]
        public string? Usemap;

        [Bind("width", BindType.Pixels)]
        public string? Width;

        public ContainerWidth ContainerWidth;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            ContainerWidth = context.GetContainerWidth();

            context.SetGlobalData("mj-image", new Style(HeadStyle));

            var isFluid = FluidOnMobile == "true";
            var isFullWidth = Equals(context.Get("full-width"), true);

            var width =
                ContainerWidth.Value -
                UnitParser.Parse(BorderLeft).Value -
                UnitParser.Parse(BorderRight).Value -
                UnitParser.Parse(PaddingLeft).Value -
                UnitParser.Parse(PaddingRight).Value;

            if (Width != null)
            {
                var parsedWidth = UnitParser.Parse(Width);

                if (parsedWidth.Value > 0)
                {
                    width = Math.Min(width, parsedWidth.Value);
                }
            }

            var href = Href;

            renderer.StartElement("table")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Class(isFluid ? "mj-full-width-mobile" : null)
                .Style("border-collapse", "collapse")
                .Style("border-spacing", "0px")
                .StyleIf("max-width", isFullWidth, "100%")
                .StyleIf("min-width", isFullWidth, "100%")
                .StyleIf("width", isFullWidth, width, "px");

            renderer.StartElement("tbody");
            renderer.StartElement("tr");

            renderer.StartElement("td")
                .Class(isFluid ? "mj-full-width-mobile" : null)
                .StyleIf("width", !isFullWidth, width, "px");

            if (!string.IsNullOrEmpty(href))
            {
                renderer.StartElement("a")
                  .Attr("href", Href)
                  .Attr("name", Name)
                  .Attr("rel", Rel)
                  .Attr("target", Target)
                  .Attr("title", Title);

                RenderImage(renderer, width, isFullWidth);

                renderer.EndElement("a");
            }
            else
            {
                RenderImage(renderer, width, isFullWidth);
            }

            renderer.EndElement("td");
            renderer.EndElement("tr");
            renderer.EndElement("tbody");
            renderer.EndElement("table");
        }

        private static void HeadStyle(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.Content($"@media only screen and (max-width:{context.Options.Breakpoint}) {{");
            renderer.Content("  table.mj-full-width-mobile {");
            renderer.Content("    width: 100% !important; }");
            renderer.Content("  }");
            renderer.Content("  td.mj-full-width-mobile {");
            renderer.Content("    width: auto !important;");
            renderer.Content("  }");
            renderer.Content("}");
        }

        private void RenderImage(IHtmlRenderer renderer, double width, bool fullWidth)
        {
            renderer.StartElement("img", true)
                .Attr("alt", Alt)
                .Attr("height", Height.GetNumberOrAuto())
                .Attr("sizes", Sizes)
                .Attr("src", Src)
                .Attr("srcset", Srcset)
                .Attr("title", Title)
                .Attr("usemap", Usemap)
                .Attr("width", width)
                .Style("border", Border)
                .Style("border-bottom", BorderBottom)
                .Style("border-left", BorderLeft)
                .Style("border-radius", BorderRadius)
                .Style("border-right", BorderRight)
                .Style("border-top", BorderTop)
                .Style("display", "block")
                .Style("font-size", FontSize)
                .Style("height", Height)
                .Style("max-height", MaxHeight)
                .StyleIf("max-width", fullWidth, "100%")
                .StyleIf("min-width", fullWidth, "100%")
                .Style("outline", "none")
                .Style("text-decoration", "none")
                .Style("width", "100%");
        }
    }
}
