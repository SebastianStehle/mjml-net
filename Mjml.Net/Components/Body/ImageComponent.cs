using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Body
{
    public partial class ImageProps
    {
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
    }

    public sealed class ImageComponent : BodyComponentBase<ImageProps>
    {
        public override string Name => "mj-image";

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            context.SetGlobalData("mj-image", new DynamicStyle(HeadStyle));

            var isFluid = Props.FluidOnMobile == "true";
            var isFullWidth = Equals(renderer.Get("full-width"), true);

            var widthConfigured = UnitParser.Parse(Props.Width).Value;
            var widthAvailable =
                renderer.GetContainerWidth().Value -
                UnitParser.Parse(Props.BorderLeft).Value -
                UnitParser.Parse(Props.BorderRight).Value -
                UnitParser.Parse(Props.PaddingLeft).Value -
                UnitParser.Parse(Props.PaddingRight).Value;

            var widthMin = Math.Min(widthConfigured, widthAvailable);

            var href = Props.Href;

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
                  .Attr("href", Props.Href)
                  .Attr("name", Props.Name)
                  .Attr("rel", Props.Rel)
                  .Attr("target", Props.Target)
                  .Attr("title", Props.Title);

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
                .Attr("alt", Props.Alt)
                .Attr("height", Props.Height.GetNumberOrAuto())
                .Attr("sizes", Props.Sizes)
                .Attr("src", Props.Src)
                .Attr("srcset", Props.Srcset)
                .Attr("title", Props.Title)
                .Attr("usemap", Props.Usemap)
                .Attr("width", width.ToInvariantString())
                .Style("border", Props.Border)
                .Style("border-bottom", Props.BorderBottom)
                .Style("border-left", Props.BorderLeft)
                .Style("border-radius", Props.BorderRadius)
                .Style("border-right", Props.BorderRight)
                .Style("border-top", Props.BorderTop)
                .Style("display", "block")
                .Style("font-size", Props.FontSize)
                .Style("height", Props.Height)
                .Style("max-height", Props.MaxHeight)
                .Style("max-width", fullWidth ? "100%" : null)
                .Style("min-width", fullWidth ? "100%" : null)
                .Style("outline", "none")
                .Style("text-decoration", "none")
                .Style("width", "100%");
        }
    }
}
