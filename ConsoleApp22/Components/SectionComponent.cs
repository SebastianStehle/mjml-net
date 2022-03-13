namespace ConsoleApp22.Components
{
    internal class SectionComponent : IComponent
    {
        public string ComponentName => "mj-section";

        public AllowedAttributes? AllowedAttributes { get; } =
            new AllowedAttributes
            {
                ["background-color"] = AttributeType.Color,
                ["background-url"] = AttributeType.String,
                ["background-repeat"] = AttributeType.String,
                ["background-size"] = AttributeType.String,
                ["background-position"] = AttributeType.String,
                ["background-position-x'"] = AttributeType.String,
                ["background-position-y"] = AttributeType.String,
            };

        public void Render(IHtmlRenderer renderer, INode node)
        {
            if (IsFullWidth(node))
            {
                RenderFullWidth(renderer, node);
            }
            else
            {
                RenderSimple(renderer, node);
            }
        }

        private static void RenderSimple(IHtmlRenderer renderer, INode node)
        {
            RenderBefore(renderer, node);
        }

        private static void RenderBefore(IHtmlRenderer renderer, INode node)
        {
            var width = renderer.GetContext("width");

            renderer.Plain("<!--[if mso | IE]>");

            renderer.StartElement("div")
                .Class("outlook")
                .Class(node.GetAttribute("css-class"))
                .Attr("align", "center")
                .Attr("bgcolor", node.GetAttribute("background-color"))
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("width", width as string)
                .Done();

            renderer.StartElement("tr")
                .Done();

            renderer.StartElement("td")
                .Style("line-height", "0px")
                .Style("font-size", "0px")
                .Style("mso-line-height-rule", "exactly");

            renderer.Plain("<![endif]-->");
        }

        private static void RenderAfter(IHtmlRenderer renderer, INode node)
        {
            renderer.Plain("<!--[if mso | IE]>");

            renderer.EndElement("td");
            renderer.EndElement("tr");
            renderer.EndElement("table");

            renderer.Plain("<![endif]-->");
        }

        private static void RenderFullWidth(IHtmlRenderer renderer, INode node)
        {
        }

        private static void RenderWithBackground(IHtmlRenderer renderer, INode node, Action<IHtmlRenderer, INode> action)
        {
            var isFullWidh = IsFullWidth(node);

            var (backgroundX, backgroundY) = GetBackgroundPosition(node);

            switch (backgroundX)
            {
                case "left":
                    backgroundX = "0%";
                    break;
                case "center":
                    backgroundX = "50%";
                    break;
                case "right":
                    backgroundX = "100%";
                    break;
                default:
                    if (!backgroundX.EndsWith("%", StringComparison.OrdinalIgnoreCase))
                    {
                        backgroundX = "50%";
                    }

                    break;
            }

            switch (backgroundY)
            {
                case "top":
                    backgroundY = "0%";
                    break;
                case "center":
                    backgroundY = "50%";
                    break;
                case "bottom":
                    backgroundY = "100%";
                    break;
                default:
                    if (!backgroundY.EndsWith("%", StringComparison.OrdinalIgnoreCase))
                    {
                        backgroundY = "0%";
                    }

                    break;
            }

            var backgroundSize = node.GetAttribute("background-size");
            var backgroundRepeat = node.GetAttribute("background-repeat");

            var originX = 0;
            var originY = 0;
            var posX = 0;
            var posY = 0;

            if (backgroundSize != "auto")
            {

            }

            renderer.Plain("<!--[if mso | IE]>");

            var rect = renderer.StartElement("v:rect")
                .Style("mso-width-percent", isFullWidh ? "1000" : null)
                .Style("width", !isFullWidh ? renderer.GetContext("containerWidth")?.ToString() : null)
                .Attr("xmlns:v", "urn:schemas-microsoft-com:vml")
                .Attr("fill", "true")
                .Attr("stroke", "false");

            if (isFullWidh)
            {
                rect.Style("mso-width-percent", "1000");
            }
            else
            {
                rect.Style("width", renderer.GetContext("containerWidth")?.ToString());
            }

            rect.Done();

            var fill = renderer.StartElement("v:fill")
                .Attr("xmlns:v", "urn:schemas-microsoft-com:vml")
                .Attr("fill", "true")
                .Attr("stroke", "false");

            fill.Done();

            renderer.Plain("<![endif]-->");
        }

        private static bool HasBackground(INode node)
        {
            return node.GetAttribute("background-url") != null;
        }

        private static bool IsFullWidth(INode node)
        {
            return node.GetAttribute("full-width") == "full-width";
        }

        private static (string X, string Y) GetBackgroundPosition(INode node)
        {
            var (x, y) = ParseBackgroundPosition(node);

            return (
                node.GetAttribute("background-position-x") ?? x,
                node.GetAttribute("background-position-y") ?? y
            );
        }

        private static (string X, string Y) ParseBackgroundPosition(INode node)
        {
            // TODO: Allocation free
            var positions = node.GetAttribute("background-position")?.Split(' ');

            static bool IsTopOrBottom(string value)
            {
                return string.Equals(value, "top", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "bottom", StringComparison.OrdinalIgnoreCase);
            }

            static bool IsLeftOrRight(string value)
            {
                return string.Equals(value, "left", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "right", StringComparison.OrdinalIgnoreCase);
            }

            static bool IsCenter(string value)
            {
                return string.Equals(value, "center", StringComparison.OrdinalIgnoreCase);
            }

            if (positions?.Length == 1)
            {
                var value = positions[0];

                if (IsTopOrBottom(value))
                {
                    return ("center", value);
                }
                else
                {
                    return (value, "center");
                }
            }
            else if (positions?.Length == 2)
            {
                var value1 = positions[0];
                var value2 = positions[1];

                if (IsTopOrBottom(value1) || (IsCenter(value1) && IsLeftOrRight(value2)))
                {
                    return (value2, value1);
                }
                else
                {
                    return (value1, value2);
                }
            }

            return ("center", "top");
        }
    }
}
