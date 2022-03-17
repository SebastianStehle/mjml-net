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

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            context.SetGlobalData("mj-image", new DynamicStyle(HeadStyle));

            var isFluid = FluidOnMobile == "true";
            var isFullWidth = Equals(renderer.Get("full-width"), true);

            var widthConfigured = UnitParser.Parse(Width).Value;
            var widthAvailable =
                renderer.GetContainerWidth().Value -
                UnitParser.Parse(BorderLeft).Value -
                UnitParser.Parse(BorderRight).Value -
                UnitParser.Parse(PaddingLeft).Value -
                UnitParser.Parse(PaddingRight).Value;

            var widthMin = Math.Min(widthConfigured, widthAvailable);

            var href = Href;

            renderer.ElementStart("table")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Class(isFluid ? "mj-full-width-mobile" : null)
                .Style("border-collapse", "collapse")
                .Style("border-spacing", "0px")
                .Style("max-width", isFullWidth ? "100%" : null)
                .Style("min-width", isFullWidth ? "100%" : null)
                .Style("width", isFullWidth ? $"{widthMin}px" : null);

            renderer.ElementStart("tbody");
            renderer.ElementStart("tr");

            renderer.ElementStart("td")
                .Class(isFluid ? "mj-full-width-mobile" : null)
                .Style("width", isFullWidth ? null : $"{widthMin}px");

            if (!string.IsNullOrWhiteSpace(href))
            {
                renderer.ElementStart("a")
                  .Attr("href", Href)
                  .Attr("name", Name)
                  .Attr("rel", Rel)
                  .Attr("target", Target)
                  .Attr("title", Title);

                RenderImage(renderer, widthMin, isFullWidth);

                renderer.ElementEnd("a");
            }
            else
            {
                RenderImage(renderer, widthMin, isFullWidth);
            }

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
        }

        private static string HeadStyle(GlobalContext context)
        {
            var breakpoint = context.GlobalData.Values.OfType<Breakpoint>().First();

            return $@"
@media only screen and (max-width:${breakpoint.Value}) {{
  table.mj-full-width-mobile {{ width: 100% !important; }}
  td.mj-full-width-mobile {{ width: auto !important; }}
}}";
        }

        private void RenderImage(IHtmlRenderer renderer, double width, bool fullWidth)
        {
            renderer.ElementStart("img", true)
                .Attr("alt", Alt)
                .Attr("height", Height.GetNumberOrAuto())
                .Attr("sizes", Sizes)
                .Attr("src", Src)
                .Attr("srcset", Srcset)
                .Attr("title", Title)
                .Attr("usemap", Usemap)
                .Attr("width", width.ToInvariantString())
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
                .Style("max-width", fullWidth ? "100%" : null)
                .Style("min-width", fullWidth ? "100%" : null)
                .Style("outline", "none")
                .Style("text-decoration", "none")
                .Style("width", "100%");
        }
    }
}
