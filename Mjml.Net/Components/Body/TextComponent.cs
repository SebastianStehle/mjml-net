using Mjml.Net.Extensions;

namespace Mjml.Net.Components.Body
{
    public sealed class TextComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-text";

        public override AllowedAttributes? AllowedAttributes { get; } =
            new AllowedAttributes
            {
                ["align"] = AttributeTypes.Align,
                ["color"] = AttributeTypes.Color,
                ["container-background-color"] = AttributeTypes.Color,
                ["css-class"] = AttributeTypes.String,
                ["font-family"] = AttributeTypes.String,
                ["font-size"] = AttributeTypes.Pixels,
                ["font-style"] = AttributeTypes.String,
                ["font-weight"] = AttributeTypes.String,
                ["height"] = AttributeTypes.Pixels,
                ["letter-spacing"] = AttributeTypes.Pixels,
                ["line-height"] = AttributeTypes.Pixels,
                ["padding"] = AttributeTypes.FourPixelsOrPercent,
                ["padding-bottom"] = AttributeTypes.PixelsOrPercent,
                ["padding-left"] = AttributeTypes.PixelsOrPercent,
                ["padding-right"] = AttributeTypes.PixelsOrPercent,
                ["padding-top"] = AttributeTypes.PixelsOrPercent,
                ["text-decoration"] = AttributeTypes.String,
                ["text-transform"] = AttributeTypes.String,
            };

        public override Attributes? DefaultAttributes { get; } =
            new Attributes
            {
                ["align"] = "left",
                ["color"] = "#000000",
                ["font-family"] = "Ubuntu, Helvetica, Arial, sans-serif",
                ["font-size"] = "13px",
                ["font-style"] = "normal",
                ["letter-spacing"] = "none",
                ["line-height"] = "1",
                ["padding"] = "10px 25px",
            };

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var height = node.GetAttribute("height");

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

        private static void RenderTextContent(IHtmlRenderer renderer, INode node)
        {
            renderer.ElementStart("div")
                .Style("font-family", node.GetAttribute("font-family"))
                .Style("font-size", node.GetAttribute("font-size"))
                .Style("font-style", node.GetAttribute("font-style"))
                .Style("font-weight", node.GetAttribute("font-weight"))
                .Style("letter-spacing", node.GetAttribute("letter-spacing"))
                .Style("line-height", node.GetAttribute("line-height"))
                .Style("text-align", node.GetAttribute("align"))
                .Style("text-decoration", node.GetAttribute("text-decoration"))
                .Style("text-transform", node.GetAttribute("text-transform"))
                .Style("color", node.GetAttribute("color"))
                .Style("height", node.GetAttribute("height"));

            var rawContent = node.GetContentRaw();

            renderer.Content(string.IsNullOrEmpty(rawContent) ? node.GetContent() : rawContent);

            renderer.ElementEnd("div");
        }
    }
}
