using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .SetClass("outlook")
                .SetClass(node.GetAttribute("css-class"))
                .SetAttribute("align", "center")
                .SetAttribute("bgcolor", node.GetAttribute("background-color"))
                .SetAttribute("border", "0")
                .SetAttribute("cellpadding", "0")
                .SetAttribute("cellspacing", "0")
                .SetAttribute("role", "presentation")
                .SetStyle("width", width as string)
                .Done();

            renderer.StartElement("tr")
                .Done();

            renderer.StartElement("td")
                .SetStyle("line-height", "0px")
                .SetStyle("font-size", "0px")
                .SetStyle("mso-line-height-rule", "exactly");

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

        private static bool HasBackground(INode node)
        {
            return node.GetAttribute("background-url") != null;
        }

        private static bool IsFullWidth(INode node)
        {
            return node.GetAttribute("full-width") == "full-width";
        }
    }
}
