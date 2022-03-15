using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Body
{
    public partial struct ImageProps
    {
        [Bind("align", BindType.Align)]
        public string? Align = "center";

        [Bind("alt")]
        public string? Alt;

        [Bind("border")]
        public string? Border = "0";

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
        public string? FontSize = "13px";

        [Bind("height", BindType.PixelsOrAuto)]
        public string? Height = "auto";

        [Bind("href")]
        public string? Href;

        [Bind("max-height", BindType.PixelsOrPercent)]
        public string? MaxHeight;

        [Bind("name")]
        public string? Name;

        [Bind("padding", BindType.FourPixelsOrPercent)]
        public string? Padding = "10px 25px";

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
        public string? Target = "_blank";

        [Bind("title")]
        public string? Title;

        [Bind("usemap")]
        public string? Usemap;

        [Bind("width", BindType.Pixels)]
        public string? Width;
    }

    public sealed class ImageComponent : BodyComponentBase<ImageProps>
    {
        public override string ComponentName => "mj-image";

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            renderer.SetGlobalData("mj-image", new DynamicStyle(HeadStyle));

            var props = new ImageProps(node);

            var isFluid = props.FluidOnMobile == "true";
            var isFullWidth = Equals(renderer.Get("full-width"), true);

            var widthConfigured = UnitParser.Parse(props.Width).Value;
            var widthAvailable =
                renderer.GetContainerWidth().Value -
                UnitParser.Parse(props.BorderLeft).Value -
                UnitParser.Parse(props.BorderRight).Value -
                UnitParser.Parse(props.PaddingLeft).Value -
                UnitParser.Parse(props.PaddingRight).Value;

            var widthMin = Math.Min(widthConfigured, widthAvailable);

            var href = props.Href;

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
                .Style("width", isFullWidth ? null : $"{widthMin}px")
                .Class(isFluid ? "mj-full-width-mobile" : null);

            if (!string.IsNullOrWhiteSpace(href))
            {
                renderer.ElementStart("a")
                  .Attr("href", props.Href)
                  .Attr("name", props.Name)
                  .Attr("rel", props.Rel)
                  .Attr("target", props.Target)
                  .Attr("title", props.Title);

                RenderImage(renderer, widthMin, isFullWidth, ref props);

                renderer.ElementEnd("a");
            }
            else
            {
                RenderImage(renderer, widthMin, isFullWidth, ref props);
            }

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
        }

        private static string HeadStyle(IHtmlRenderer renderer)
        {
            var breakpoint = renderer.GlobalData.Values.OfType<Breakpoint>().First();

            return $@"
@media only screen and (max-width:${breakpoint.Value}) {{
  table.mj-full-width-mobile {{ width: 100% !important; }}
  td.mj-full-width-mobile {{ width: auto !important; }}
}}";
        }

        private static void RenderImage(IHtmlRenderer renderer, double width, bool fullWidth, ref ImageProps props)
        {
            renderer.ElementStart("img", true)
                .Attr("alt", props.Alt)
                .Attr("height", props.Height.GetNumberOrAuto())
                .Attr("sizes", props.Sizes)
                .Attr("src", props.Src)
                .Attr("srcset", props.Srcset)
                .Attr("title", props.Title)
                .Attr("usemap", props.Usemap)
                .Attr("width", width.ToInvariantString())
                .Style("border", props.Border)
                .Style("border-bottom", props.BorderBottom)
                .Style("border-left", props.BorderLeft)
                .Style("border-radius", props.BorderRadius)
                .Style("border-right", props.BorderRight)
                .Style("border-top", props.BorderTop)
                .Style("display", "block")
                .Style("font-size", props.FontSize)
                .Style("height", props.Height)
                .Style("max-height", props.MaxHeight)
                .Style("max-width", fullWidth ? "100%" : null)
                .Style("min-width", fullWidth ? "100%" : null)
                .Style("outline", "none")
                .Style("text-decoration", "none")
                .Style("width", "100%");
        }
    }
}
