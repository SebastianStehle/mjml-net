using Mjml.Net.Extensions;
using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Body;

public partial class CarouselComponent : BodyComponentBase
{
    private static readonly AllowedParents Parents =
    [
        "mj-column",
        "mj-hero"
    ];

    public override AllowedParents? AllowedParents => Parents;

    public override string ComponentName => "mj-carousel";

    [Bind("align", BindType.Align)]
    public string Align = "center";

    [Bind("border-radius", BindType.PixelsOrPercent)]
    public string BorderRadius = "6px";

    [Bind("container-background-color", BindType.Color)]
    public string? ContainerBackgroundColor;

    [Bind("icon-width", BindType.PixelsOrPercent)]
    public string IconWidth = "44px";

    [Bind("left-icon", BindType.String)]
    public string LeftIcon = "https://i.imgur.com/xTh3hln.png";

    [Bind("padding", BindType.FourPixelsOrPercent)]
    public string? Padding;

    [Bind("padding-bottom", BindType.PixelsOrPercent)]
    public string? PaddingBottom;

    [Bind("padding-left", BindType.PixelsOrPercent)]
    public string? PaddingLeft;

    [Bind("padding-right", BindType.PixelsOrPercent)]
    public string? PaddingRight;

    [Bind("padding-top", BindType.PixelsOrPercent)]
    public string? PaddingTop;

    [Bind("right-icon", BindType.String)]
    public string RightIcon = "https://i.imgur.com/os7o9kz.png";

    [Bind("tb-border", BindType.String)]
    public string TbBorder = "2px solid transparent";

    [Bind("tb-border-radius", BindType.PixelsOrPercent)]
    public string TbBorderRadius = "6px";

    [Bind("tb-hover-border-color", BindType.Color)]
    public string TbHoverBorderColor = "#fead0d";

    [Bind("tb-selected-border-color", BindType.Color)]
    public string TbSelectedBorderColor = "#ccc";

    [Bind("tb-width", BindType.PixelsOrPercent)]
    public string? TbWidth;

    [Bind("thumbnails", BindType.String)]
    public string Thumbnails = "visible";

    private string CarouselID { get; set; }

    public IEnumerable<CarouselImageComponent> CarouselImages { get; private set; }

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        CarouselID = context.Options.IdGenerator.Next();

        CarouselImages = ChildNodes
            .Where(c => c is CarouselImageComponent)
            .Cast<CarouselImageComponent>();

        context.SetGlobalData("mj-carousel", new Style(HeadStyle));

        renderer.StartConditional("<!--[if !mso]><!-->");
        {
            renderer.StartElement("div")
                .Class("mj-carousel");

            GenerateRadios(renderer);

            renderer.StartElement("div") // Style carousel.div
                .Class($"mj-carousel-content mj-carousel-{CarouselID}-content")
                .Style("display", "table")
                .Style("font-size", "0px")
                .Style("table-layout", "fixed")
                .Style("text-align", "center")
                .Style("width", "100%");

            GenerateThumbnails(renderer);
            GenerateCarousel(renderer, context);

            renderer.EndElement("div");
            renderer.EndElement("div");
        }
        renderer.EndConditional("<!--<![endif]-->");

        RenderFallback(renderer, context);
    }

    private void GenerateRadios(IHtmlRenderer renderer)
    {
        for (int i = 0; i < CarouselImages.Count(); i++)
        {
            var carouselImage = CarouselImages.ElementAt(i);

            if (carouselImage != null)
            {
                carouselImage.CarouselID = CarouselID;
                carouselImage.CarouselImageIndex = i;
                carouselImage.RenderRadio(renderer);
            }
        }
    }

    private void GenerateThumbnails(IHtmlRenderer renderer)
    {
        if (Thumbnails != "visible")
        {
            return;
        }

        for (int i = 0; i < CarouselImages.Count(); i++)
        {
            var carouselImage = CarouselImages.ElementAt(i);

            if (carouselImage != null)
            {
                carouselImage.CarouselID = CarouselID;
                carouselImage.CarouselImageIndex = i;
                carouselImage.TbBorder = TbBorder;
                carouselImage.TbBorderRadius = TbBorderRadius;
                carouselImage.TbWidth = GetThumbnailsWidth();
                carouselImage.RenderThumbnail(renderer);
            }
        }
    }

    private void GenerateCarousel(IHtmlRenderer renderer, GlobalContext context)
    {
        renderer.StartElement("table") // Style carousel.table
            .Attr("border", "0")
            .Attr("cell-padding", "0")
            .Attr("cell-spacing", "0")
            .Attr("role", "presentation")
            .Attr("width", "100%")
            .Class("mj-carousel-main")
            .Style("caption-side", "top")
            .Style("display", "table-caption")
            .Style("table-layout", "fixed")
            .Style("width", "100%");

        renderer.StartElement("tbody");
        renderer.StartElement("tr");

        GenerateControls(renderer, "previous", LeftIcon);
        GenerateImages(renderer, context);
        GenerateControls(renderer, "next", RightIcon);

        renderer.EndElement("tr");
        renderer.EndElement("tbody");
        renderer.EndElement("table");
    }

    private void GenerateControls(IHtmlRenderer renderer, string direction, string icon)
    {
        var (iconWidth, _) = UnitParser.Parse(IconWidth);

        renderer.StartElement("td") // Style controls.td
            .Class($"mj-carousel-{CarouselID}-icons-cell")
            .Style("display", "none")
            .Style("font-size", "0px")
            .Style("mso-hide", "all")
            .Style("padding", "0px");

        renderer.StartElement("div") // Style controls.div
            .Class($"mj-carousel-{direction}-icons")
            .Style("display", "none")
            .Style("mso-hide", "all");

        for (int i = 0; i < CarouselImages.Count(); i++)
        {
            renderer.StartElement("label")
                .Attr("for", $"mj-carousel-{CarouselID}-radio-{i + 1}")
                .Class($"mj-carousel-{direction}")
                .Class($"mj-carousel-{direction}-{i + 1}");

            renderer.StartElement("img", true) // Style controls.img
                .Attr("alt", direction)
                .Attr("src", icon)
                .Attr("width", $"{iconWidth}")
                .Style("display", "block")
                .Style("height", "auto")
                .Style("width", IconWidth);

            renderer.EndElement("label");
        }

        renderer.EndElement("div");
        renderer.EndElement("td");
    }

    private void GenerateImages(IHtmlRenderer renderer, GlobalContext context)
    {
        renderer.StartElement("td") // Style images.td
            .Style("padding", "0px");

        renderer.StartElement("div")
            .Class("mj-carousel-images");

        for (int i = 0; i < CarouselImages.Count(); i++)
        {
            var image = CarouselImages.ElementAt(i);

            image.CarouselID = CarouselID;
            image.CarouselImageIndex = i;
            image.BorderRadius = BorderRadius;
            image.Render(renderer, context);
        }

        renderer.EndElement("div");
        renderer.EndElement("td");
    }

    private void RenderFallback(IHtmlRenderer renderer, GlobalContext context)
    {
        var firstImage = CarouselImages.FirstOrDefault();

        if (firstImage != null)
        {
            renderer.Plain("<!--[if mso]>"); // I can't use StartConditional here as it's bugged.
            {
                firstImage.BorderRadius = BorderRadius;
                firstImage.Render(renderer, context);
            }
            renderer.Plain("<![endif]-->"); // I can't use EndConditional here as it's bugged.
        }
    }

    private void HeadStyle(IHtmlRenderer renderer, GlobalContext context)
    {
        if (!CarouselImages.Any())
        {
            return;
        }

        renderer.Content(".mj-carousel {");
        renderer.Content("  -webkit-user-select: none;");
        renderer.Content("  -moz-user-select: none;");
        renderer.Content("  user-select: none;");
        renderer.Content("}");

        renderer.Content($".mj-carousel-{CarouselID}-icons-cell {{");
        renderer.Content("  display: table-cell !important;");
        renderer.Content($" width: {IconWidth} !important;");
        renderer.Content("}");

        renderer.Content(".mj-carousel-radio,");
        renderer.Content(".mj-carousel-next,");
        renderer.Content(".mj-carousel-previous {");
        renderer.Content("  display: none !important;");
        renderer.Content("}");

        renderer.Content(".mj-carousel-thumbnail,");
        renderer.Content(".mj-carousel-next,");
        renderer.Content(".mj-carousel-previous {");
        renderer.Content("  touch-action: manipulation;");
        renderer.Content("}");

        var length = CarouselImages.Count();

        // https://github.com/mjmlio/mjml/blob/a5812ac1ad7cdf7ef9ae71fcf5808c49ba8ac5cb/packages/mjml-carousel/src/Carousel.js#L78-L88
        for (int i = 0; i < length; i++)
        {
            var image = CarouselImages.ElementAt(i);

            var selectorSibilings = BuildSelectorSiblings(i);

            if (image == CarouselImages.LastOrDefault())
            {
                renderer.Content($".mj-carousel-{CarouselID}-radio:checked {selectorSibilings}+ .mj-carousel-content .mj-carousel-image {{");
            }
            else
            {
                renderer.Content($".mj-carousel-{CarouselID}-radio:checked {selectorSibilings}+ .mj-carousel-content .mj-carousel-image, ");
            }
        }
        renderer.Content("  display: none !important;");
        renderer.Content("}");

        // https://github.com/mjmlio/mjml/blob/a5812ac1ad7cdf7ef9ae71fcf5808c49ba8ac5cb/packages/mjml-carousel/src/Carousel.js#L90-L100
        for (int i = 0; i < length; i++)
        {
            var image = CarouselImages.ElementAt(i);

            var selectorSibilings = BuildSelectorSiblings(length - i - 1);

            if (image == CarouselImages.LastOrDefault())
            {
                renderer.Content($".mj-carousel-{CarouselID}-radio-{i + 1}:checked {selectorSibilings}+ .mj-carousel-content .mj-carousel-image-{i + 1} {{");
            }
            else
            {
                renderer.Content($".mj-carousel-{CarouselID}-radio-{i + 1}:checked {selectorSibilings}+ .mj-carousel-content .mj-carousel-image-{i + 1}, ");
            }
        }
        renderer.Content("  display: block  !important;");
        renderer.Content("}");

        // https://github.com/mjmlio/mjml/blob/a5812ac1ad7cdf7ef9ae71fcf5808c49ba8ac5cb/packages/mjml-carousel/src/Carousel.js#L102-L112
        renderer.Content(".mj-carousel-previous-icons,");
        renderer.Content(".mj-carousel-next-icons,");
        for (int i = 0; i < length; i++)
        {
            var selectorSibilings = BuildSelectorSiblings(length - i - 1);

            renderer.Content($".mj-carousel-{CarouselID}-radio-{i + 1}:checked {selectorSibilings}+ .mj-carousel-content .mj-carousel-next-{((i + (1 % length) + length) % length) + 1}, ");
        }

        // https://github.com/mjmlio/mjml/blob/a5812ac1ad7cdf7ef9ae71fcf5808c49ba8ac5cb/packages/mjml-carousel/src/Carousel.js#L113-L121
        for (int i = 0; i < length; i++)
        {
            var image = CarouselImages.ElementAt(i);

            var selectorSibilings = BuildSelectorSiblings(length - i - 1);

            if (image == CarouselImages.LastOrDefault())
            {
                renderer.Content($".mj-carousel-{CarouselID}-radio-{i + 1}:checked {selectorSibilings}+ .mj-carousel-content .mj-carousel-previous-{((i - (1 % length) + length) % length) + 1} {{");
            }
            else
            {
                renderer.Content($".mj-carousel-{CarouselID}-radio-{i + 1}:checked {selectorSibilings}+ .mj-carousel-content .mj-carousel-previous-{((i - (1 % length) + length) % length) + 1}, ");
            }
        }
        renderer.Content("  display: block  !important;");
        renderer.Content("}");

        // https://github.com/mjmlio/mjml/blob/a5812ac1ad7cdf7ef9ae71fcf5808c49ba8ac5cb/packages/mjml-carousel/src/Carousel.js#L125-L137
        for (int i = 0; i < length; i++)
        {
            var image = CarouselImages.ElementAt(i);

            var selectorSibilings = BuildSelectorSiblings(length - i - 1);

            if (image == CarouselImages.LastOrDefault())
            {
                renderer.Content($".mj-carousel-{CarouselID}-radio-{i + 1}:checked {selectorSibilings}+ .mj-carousel-content .mj-carousel-{CarouselID}-thumbnail-{i + 1} {{");
            }
            else
            {
                renderer.Content($".mj-carousel-{CarouselID}-radio-{i + 1}:checked {selectorSibilings}+ .mj-carousel-content .mj-carousel-{CarouselID}-thumbnail-{i + 1}, ");
            }
        }
        renderer.Content($"  border-color: {TbSelectedBorderColor}  !important;");
        renderer.Content("}");

        renderer.Content(".mj-carousel-image img + div,");
        renderer.Content(".mj-carousel-thumbnail img + div {");
        renderer.Content("  display: none !important;");
        renderer.Content("}");

        // https://github.com/mjmlio/mjml/blob/a5812ac1ad7cdf7ef9ae71fcf5808c49ba8ac5cb/packages/mjml-carousel/src/Carousel.js#L144-L154
        for (int i = 0; i < length; i++)
        {
            var image = CarouselImages.ElementAt(i);

            var selectorSibilings = BuildSelectorSiblings(length - i - 1);

            if (image == CarouselImages.LastOrDefault())
            {
                renderer.Content($".mj-carousel-{CarouselID}-thumbnail:hover {selectorSibilings}+ .mj-carousel-main .mj-carousel-image {{");
            }
            else
            {
                renderer.Content($".mj-carousel-{CarouselID}-thumbnail:hover {selectorSibilings}+ .mj-carousel-main .mj-carousel-image, ");
            }
        }
        renderer.Content("  display: none !important;");
        renderer.Content("}");

        renderer.Content(".mj-carousel-thumbnail:hover {");
        renderer.Content($"  border-color: {TbHoverBorderColor} !important;");
        renderer.Content("}");

        // https://github.com/mjmlio/mjml/blob/a5812ac1ad7cdf7ef9ae71fcf5808c49ba8ac5cb/packages/mjml-carousel/src/Carousel.js#L160-L171
        for (int i = 0; i < length; i++)
        {
            var image = CarouselImages.ElementAt(i);

            var selectorSibilings = BuildSelectorSiblings(length - i - 1);

            if (image == CarouselImages.LastOrDefault())
            {
                renderer.Content($".mj-carousel-{CarouselID}-thumbnail-{i + 1}:hover {selectorSibilings}+ .mj-carousel-main .mj-carousel-image-{i + 1} {{");
            }
            else
            {
                renderer.Content($".mj-carousel-{CarouselID}-thumbnail-{i + 1}:hover {selectorSibilings}+ .mj-carousel-main .mj-carousel-image-{i + 1}, ");
            }
        }
        renderer.Content("  display: block !important;");
        renderer.Content("}");

        // Fallback
        renderer.Content(".mj-carousel noinput { display:block !important; }");
        renderer.Content(".mj-carousel noinput .mj-carousel-image-1 { display: block !important;  }");
        renderer.Content(".mj-carousel noinput .mj-carousel-arrows,");
        renderer.Content(".mj-carousel noinput .mj-carousel-thumbnails { display: none !important; }");

        renderer.Content("[owa] .mj-carousel-thumbnail { display: none !important; }");

        renderer.Content("@media screen yahoo {");
        renderer.Content($".mj-carousel-{CarouselID}-icons-cell,");
        renderer.Content(".mj-carousel-previous-icons,");
        renderer.Content(".mj-carousel-next-icons {");
        renderer.Content("  display: none !important;");
        renderer.Content("}");

        // https://github.com/mjmlio/mjml/blob/a5812ac1ad7cdf7ef9ae71fcf5808c49ba8ac5cb/packages/mjml-carousel/src/Carousel.js#L188-L193
        var selectorSibilingsFallback = BuildSelectorSiblings(length - 1);

        renderer.Content($".mj-carousel-{CarouselID}-radio-1:checked {selectorSibilingsFallback}+ .mj-carousel-content .mj-carousel-{CarouselID}-thumbnail-1 {{");
        renderer.Content("  border-color: transparent;");
        renderer.Content("}");
    }

    // Optimize: Helper to build selector siblings string efficiently
    private static string BuildSelectorSiblings(int count)
    {
        if (count == 0)
        {
            return string.Empty;
        }

        // Use StringBuilder to avoid LINQ allocations
        var sb = DefaultPools.StringBuilders.Get();
        try
        {
            for (int i = 0; i < count; i++)
            {
                sb.Append("+ * ");
            }

            return sb.ToString();
        }
        finally
        {
            DefaultPools.StringBuilders.Return(sb);
        }
    }

    private string GetThumbnailsWidth()
    {
        if (!CarouselImages.Any())
        {
            return "0";
        }

        if (!string.IsNullOrEmpty(TbWidth))
        {
            return TbWidth;
        }

        return Math.Min(ActualWidth / CarouselImages.Count(), 110d).ToInvariantString();
    }
}
