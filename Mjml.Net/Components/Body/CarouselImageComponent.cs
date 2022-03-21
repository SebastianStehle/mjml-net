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

        [Bind("tb-width", BindType.PixelsOrPercent)]
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
                // .Attr("width", containerWidth) NOT SURE
                .Style("border-radius", BorderRadius)
                .Style("display", "block")
                // .Style("width", containerWidth) NOT SURE
                .Style("max-width", "100%")
                .Style("height", "auto");
        }

        internal void RenderThumbnail(IHtmlRenderer renderer, GlobalContext context)
        {
            throw new NotImplementedException();
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
