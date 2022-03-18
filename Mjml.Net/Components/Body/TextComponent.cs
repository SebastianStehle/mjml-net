using Mjml.Net.Extensions;

namespace Mjml.Net.Components.Body
{
    public partial class TextComponent : BodyComponentBase
    {
        public override ContentType ContentType => ContentType.Raw;

        public override string ComponentName => "mj-text";

        [Bind("align", BindType.Align)]
        public string Align = "left";

        [Bind("color", BindType.Color)]
        public string Color = "#000000";

        [Bind("container-background-color", BindType.Color)]
        public string? ContainerBackgroundColor;

        [Bind("css-class")]
        public string? CssClass;

        [Bind("font-family")]
        public string FontFamily = "Ubuntu, Helvetica, Arial, sans-serif";

        [Bind("font-size", BindType.Pixels)]
        public string FontSize = "13px";

        [Bind("font-style")]
        public string FontStyle = "normal";

        [Bind("font-weight")]
        public string? FontWeight;

        [Bind("height", BindType.Pixels)]
        public string? Height;

        [Bind("letter-spacing", BindType.Pixels)]
        public string LetterSpacing = "none";

        [Bind("line-height", BindType.Pixels)]
        public string LineHeight = "1";

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

        [Bind("text-decoration")]
        public string? TextDecoration;

        [Bind("text-transform")]
        public string? TextTransform;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            var height = Height;

            if (string.IsNullOrEmpty(height))
            {
                RenderTextContent(renderer);
            }
            else
            {
                renderer.StartConditionalTag();
                renderer.StartElement("table")
                    .Attr("role", "presentation")
                    .Attr("border", "0")
                    .Attr("cellpadding", "0")
                    .Attr("cellspacing", "0");

                renderer.StartElement("tr");
                renderer.StartElement("td")
                    .Attr("height", height)
                    .Style("vertical-align", "top")
                    .Style("height", height);
                renderer.EndConditionalTag();

                RenderTextContent(renderer);

                renderer.StartConditionalTag();
                renderer.EndElement("td");
                renderer.EndElement("tr");
                renderer.EndElement("table");
                renderer.EndConditionalTag();
            }
        }

        private void RenderTextContent(IHtmlRenderer renderer)
        {
            renderer.StartElement("div")
                .Style("font-family", FontFamily)
                .Style("font-size", FontSize)
                .Style("font-style", FontStyle)
                .Style("font-weight", FontWeight)
                .Style("letter-spacing", LetterSpacing)
                .Style("line-height", LineHeight)
                .Style("text-align", Align)
                .Style("text-decoration", TextDecoration)
                .Style("text-transform", TextTransform)
                .Style("color", Color)
                .Style("height", Height);

            RenderRaw(renderer);

            renderer.EndElement("div");
        }
    }
}
