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

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {

        }

        internal void RenderThumbnail(IHtmlRenderer renderer, GlobalContext context)
        {
            throw new NotImplementedException();
        }

        internal void RenderRadio(IHtmlRenderer renderer, object context)
        {
            throw new NotImplementedException();
        }
    }
}
