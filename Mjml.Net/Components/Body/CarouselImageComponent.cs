using Mjml.Net.Extensions;
using Mjml.Net.Helpers;

#pragma warning disable IDE0059 // Unnecessary assignment of a value

namespace Mjml.Net.Components.Body
{
    public partial class CarouselImageComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-carousel-image";

        [Bind("alt", BindType.String)]
        public string? Alt;

        [Bind("border-radius", BindType.PixelsOrPercent)]
        public string? BorderRadius;

        [Bind("href", BindType.String)]
        public string? Href;

        [Bind("rel", BindType.String)]
        public string? Rel;

        [Bind("src", BindType.String)]
        public string? Src;

        [Bind("target", BindType.String)]
        public string Target = "_blank";

        [Bind("tb-border", BindType.String)]
        public string? TbBorder;

        [Bind("tb-border-radius", BindType.PixelsOrPercent)]
        public string? TbBorderRadius;

        [Bind("tb-width", BindType.Pixels)]
        public string? TbWidth;

        [Bind("thumbnails-src", BindType.String)]
        public string? ThumbnailsSrc;

        [Bind("title", BindType.String)]
        public string? Title;

        public string CarouselID { get; set; }

        public int CarouselImageIndex { get; set; }

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            var divElement = renderer.StartElement("div")
                .Class("mj-carousel-image")
                .Class($"mj-carousel-image-{CarouselImageIndex + 1}")
                .Class(CssClass);

            if (CarouselImageIndex != 0)
            {
                divElement // Style images.otherImageDiv
                    .Style("display", "none")
                    .Style("mso-hide", "all");
            }

            if (Href != null)
            {
                renderer.StartElement("a")
                    .Attr("href", Href)
                    .Attr("rel", Rel)
                    .Attr("target", "_blank");

                RenderImage(renderer, context);

                renderer.EndElement("a");
            }
            else
            {
                RenderImage(renderer, context);
            }

            renderer.EndElement("div");
        }

        private void RenderImage(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.StartElement("img", true) // Style images.img
                .Attr("title", Title)
                .Attr("src", Src)
                .Attr("alt", Alt)
                .Attr("border", "0")
                .Attr("width", $"{ActualWidth}")
                .Style("border-radius", BorderRadius)
                .Style("display", "block")
                .Style("width", $"{ActualWidth}px")
                .Style("max-width", "100%")
                .Style("height", "auto");
        }

        internal void RenderThumbnail(IHtmlRenderer renderer, GlobalContext context)
        {
            var (widthParsed, _) = UnitParser.Parse(TbWidth);
            var imgIndex = CarouselImageIndex + 1;

            renderer.StartElement("a") // Style thumbnails.a
                .Attr("href", $"#{imgIndex}")
                .Attr("target", Target)
                .Class("mj-carousel-thumbnail")
                .Class($"mj-carousel-{CarouselID}-thumbnail")
                .Class($"mj-carousel-{CarouselID}-thumbnail-{imgIndex}")
                .Classes(CssClass, "thumbnail")
                .Style("border", TbBorder)
                .Style("border-radius", TbBorderRadius)
                .Style("display", "inline-block")
                .Style("overflow", "hidden")
                .Style("width", TbWidth);

            renderer.StartElement("label")
                .Attr("for", $"mj-carousel-{CarouselID}-radio-{imgIndex}");

            renderer.StartElement("img", true) // Style thumbnails.img
                .Attr("src", !string.IsNullOrEmpty(ThumbnailsSrc) ? ThumbnailsSrc : Src)
                .Attr("alt", Alt)
                .Attr("width", $"{widthParsed}")
                .Style("display", "block")
                .Style("width", "100%")
                .Style("height", "auto");

            renderer.EndElement("label");
            renderer.EndElement("a");
        }

        internal void RenderRadio(IHtmlRenderer renderer, object context)
        {
            renderer.StartElement("input", true) // Style radio.input
                .Attr("type", "radio")
                .Attr("name", $"mj-carousel-radio-{CarouselID}")
                .Attr("id", $"mj-carousel-{CarouselID}-radio-{CarouselImageIndex + 1}")
                .Attr("checked", CarouselImageIndex == 0 ? "checked" : null)
                .Class("mj-carousel-radio")
                .Class($"mj-carousel-{CarouselID}-radio")
                .Class($"mj-carousel-{CarouselID}-radio-{CarouselImageIndex + 1}")
                .Style("display", "none")
                .Style("mso-hide", "all");
        }
    }
}
