namespace Mjml.Net.Components.Body
{
    public partial class AccordionTitleComponent : BodyComponentBase
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mj-accordion-title"
        };

        public override string ComponentName => "mj-accordion-title";

        public override ContentType ContentType => ContentType.Text;

        public override AllowedParents? AllowedAsChild => Parents;

        [Bind("background-color", BindType.Color)]
        public string? BackgroundColor;

        [Bind("border", BindType.String)]
        public string? Border;

        [Bind("color", BindType.Color)]
        public string? Color;

        [Bind("font-family", BindType.String)]
        public string? FontFamily;

        [Bind("font-size", BindType.Pixels)]
        public string FontSize = "13px";

        [Bind("icon-align", BindType.String)]
        public string IconAlign;

        [Bind("icon-height", BindType.PixelsOrPercent)]
        public string IconHeight;

        [Bind("icon-position", BindType.String)]
        public string IconPosition;

        [Bind("icon-unwrapped-alt", BindType.String)]
        public string? IconUnwrappedAlt;

        [Bind("icon-unwrapped-url", BindType.String)]
        public string? IconUnwrappedUrl;

        [Bind("icon-width", BindType.PixelsOrPercent)]
        public string IconWidth;

        [Bind("icon-wrapped-url", BindType.String)]
        public string IconWrappedUrl;

        [Bind("icon-wrapped-alt", BindType.String)]
        public string IconWrappedAlt;

        [Bind("padding", BindType.PixelsOrPercent)]
        public string Padding = "16px";

        [Bind("padding-bottom", BindType.PixelsOrPercent)]
        public string? PaddingBottom;

        [Bind("padding-left", BindType.PixelsOrPercent)]
        public string? PaddingLeft;

        [Bind("padding-right", BindType.PixelsOrPercent)]
        public string? PaddingRight;

        [Bind("padding-top", BindType.PixelsOrPercent)]
        public string? PaddingTop;

        [BindText]
        public string? Text;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.StartElement("div")
                .Class("mj-accordion-title");

            renderer.StartElement("table") // Style table
                .Attr("cell-spacing", "0")
                .Attr("cell-padding", "0")
                .Style("border-bottom", Border)
                .Style("width", "100%");

            renderer.StartElement("tbody");
            renderer.StartElement("tr");

            if (IconPosition == "right")
            {
                RenderTitle(renderer);
                RenderIcons(renderer);
            }
            else
            {
                RenderIcons(renderer);
                RenderTitle(renderer);
            }

            renderer.EndElement("tr");
            renderer.EndElement("tbody");
            renderer.EndElement("table");
            renderer.EndElement("div");
        }

        private void RenderTitle(IHtmlRenderer renderer)
        {
            renderer.StartElement("td") // Style td
                .Class(CssClass)
                .Style("background-color", BackgroundColor)
                .Style("color", Color)
                .Style("font-size", FontSize)
                .Style("font-size", FontFamily)
                .Style("padding", Padding)
                .Style("padding-bottom", PaddingBottom)
                .Style("padding-left", PaddingLeft)
                .Style("padding-right", PaddingRight)
                .Style("padding-top", PaddingTop)
                .Style("width", "100%");

            renderer.Content(Text);

            renderer.EndElement("td");
        }

        private void RenderIcons(IHtmlRenderer renderer)
        {
            renderer.StartConditional("<!--[if !mso | IE]><!-->");
            {
                renderer.StartElement("td") // Style td2
                    .Class("mj-accordion-ico")
                    .Style("background", BackgroundColor)
                    .Style("padding", "16px")
                    .Style("vertical-align", IconAlign);

                renderer.StartElement("img", true) // Style img
                    .Attr("src", IconWrappedUrl)
                    .Attr("alt", IconWrappedAlt)
                    .Class("mj-accordion-more")
                    .Style("display", "none")
                    .Style("height", IconHeight)
                    .Style("width", IconWidth);

                renderer.StartElement("img", true) // Style img
                    .Attr("src", IconUnwrappedUrl)
                    .Attr("alt", IconUnwrappedAlt)
                    .Class("mj-accordion-less")
                    .Style("display", "none")
                    .Style("height", IconHeight)
                    .Style("width", IconWidth);

                renderer.EndElement("td");
            }

            renderer.EndConditional("<!--<![endif]-->");
        }
    }
}
