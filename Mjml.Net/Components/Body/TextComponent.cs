using Mjml.Net.Extensions;

namespace Mjml.Net.Components.Body
{
    public partial struct TextProps
    {
        [Bind("align", BindType.Align)]
        public string? Align = "left";

        [Bind("color", BindType.Color)]
        public string? Color = "#000000";

        [Bind("container-background-color", BindType.Color)]
        public string? ContainerBackgroundColor;

        [Bind("css-class")]
        public string? CssClass;

        [Bind("font-family")]
        public string? FontFamily = "Ubuntu, Helvetica, Arial, sans-serif";

        [Bind("font-size", BindType.Pixels)]
        public string? FontSize = "13px";

        [Bind("font-style")]
        public string? FontStyle = "normal";

        [Bind("font-weight")]
        public string? FontWeight;

        [Bind("height", BindType.Pixels)]
        public string? Height;

        [Bind("letter-spacing", BindType.Pixels)]
        public string? LetterSpacing = "none";

        [Bind("line-height", BindType.Pixels)]
        public string? LineHeight = "1";

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

        [Bind("text-decoration")]
        public string? TextDecoration;

        [Bind("text-transform")]
        public string? TextTransform;
    }

    public sealed class TextComponent : BodyComponentBase<TextProps>
    {
        public override string ComponentName => "mj-text";

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var props = new TextProps(node);

            var height = props.Height;

            if (string.IsNullOrEmpty(height))
            {
                RenderTextContent(renderer, node);
            }
            else
            {
                renderer.StartConditionalTag();
                renderer.ElementStart("table")
                    .Attr("role", "presentation")
                    .Attr("border", "0")
                    .Attr("cellpadding", "0")
                    .Attr("cellspacing", "0");

                renderer.ElementStart("tr");
                renderer.ElementStart("td")
                    .Attr("height", height)
                    .Style("vertical-align", "top")
                    .Style("height", height);
                renderer.EndConditionalTag();

                RenderTextContent(renderer, node);

                renderer.StartConditionalTag();
                renderer.ElementEnd("td");
                renderer.ElementEnd("tr");
                renderer.ElementEnd("table");
                renderer.EndConditionalTag();
            }
        }

        private void RenderTextContent(IHtmlRenderer renderer, INode node)
        {
            renderer.ElementStart("div")
                .Style("font-family", props.FontFamily)
                .Style("font-size", props.FontSize)
                .Style("font-style", props.FontStyle)
                .Style("font-weight", props.FontWeight)
                .Style("letter-spacing", props.LetterSpacing)
                .Style("line-height", props.LineHeight)
                .Style("text-align", props.Align)
                .Style("text-decoration", props.TextDecoration)
                .Style("text-transform", props.TextTransform)
                .Style("color", props.Color)
                .Style("height", props.Height);

            var rawContent = node.GetContentRaw();

            renderer.Content(string.IsNullOrEmpty(rawContent) ? node.GetContent() : rawContent);

            renderer.ElementEnd("div");
        }
    }
}
