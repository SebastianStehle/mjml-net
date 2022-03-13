namespace ConsoleApp22.Components
{
    public sealed class ButtonComponent : IComponent
    {
        public string ComponentName => "mj-button";

        public void Render(IHtmlRenderer renderer, INode node)
        {
            var href = node.GetAttribute("href");

            var tag = !string.IsNullOrWhiteSpace(href) ? "a" : "p";

            var backgroundColor = node.GetAttribute("background-color");

            renderer.StartElement("table")
                .Style("border", "0")
                .Style("border-collapse", "separate")
                .Style("line-height", "100%")
                .Style("width", node.GetAttribute("width"))
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellpadding", "0")
                .Attr("role", "presentation")
                .Done();

            renderer.StartElement("tbody")
                .Done();

            renderer.StartElement("tr")
                .Done();

            var td = renderer.StartElement("td")
                .Style("background", node.GetAttribute("background-color"))
                .Style("border", node.GetAttribute("border"))
                .Style("border-bottom", node.GetAttribute("border-bottom"))
                .Style("border-left", node.GetAttribute("border-left"))
                .Style("border-radius", node.GetAttribute("border-radius"))
                .Style("border-right", node.GetAttribute("border-right"))
                .Style("border-top", node.GetAttribute("border-top"))
                .Style("cursor", "auto")
                .Style("font-style", node.GetAttribute("font-style"))
                .Style("font-weight", "node")
                .Style("height", node.GetAttribute("height"))
                .Style("mso-padding-alt", node.GetAttribute("inner-padding"))
                .Style("text -align", node.GetAttribute("text-align"))
                .Attr("align", "center")
                .Attr("role", "presentation")
                .Attr("valign", "vertical-align");

            if (backgroundColor != "none")
            {
                td.Attr("bgcolor", backgroundColor);
            }

            td.Done();

            var target = renderer.StartElement(tag)
                .Style("background", node.GetAttribute("background -color"))
                .Style("border-radius", node.GetAttribute("border-radius"))
                .Style("color", node.GetAttribute("color"))
                .Style("display", "inline-block")
                .Style("font-family", node.GetAttribute("font-family"))
                .Style("font-size", node.GetAttribute("font-size"))
                .Style("font-style", node.GetAttribute("font-style"))
                .Style("font-weight", node.GetAttribute("font-weight"))
                .Style("letter-spacing", node.GetAttribute("letter-spacing"))
                .Style("line-height", node.GetAttribute("line-height"))
                .Style("margin", "0")
                .Style("mso-padding-alt", "0px")
                .Style("padding", node.GetAttribute("inner-padding"))
                .Style("text-decoration", node.GetAttribute("text-decoration"))
                .Style("text-transform", node.GetAttribute("text-transform"))
                .Style("width", CalculateWidth(node.GetAttribute("width")))
                .Attr("href", href)
                .Attr("name", node.GetAttribute("name"))
                .Attr("rel", node.GetAttribute("rel"))
                .Attr("title", node.GetAttribute("title"));

            if (tag == "a")
            {
                target.Attr("target", node.GetAttribute("target"));
            }

            target.Done();

            renderer.EndElement(tag);
            renderer.EndElement("td");
            renderer.EndElement("tr");
            renderer.EndElement("tbody");
            renderer.EndElement("table");
        }

        private static string CalculateWidth(string? width)
        {
            return String.Empty;
        }
    }
}
