using Mjml.Net.Types;

namespace Mjml.Net.Components.Body
{
    public sealed class SocialComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-social";

        public override AllowedAttributes? AllowedAttributes { get; } =
            new AllowedAttributes
            {
                ["align"] = AttributeTypes.Align,
                ["border-radius"] = AttributeTypes.String,
                ["color"] = AttributeTypes.String,
                ["container-background-color"] = AttributeTypes.Color,
                ["font-family"] = AttributeTypes.String,
                ["font-size"] = AttributeTypes.Pixels,
                ["font-style"] = AttributeTypes.String,
                ["font-weight"] = AttributeTypes.String,
                ["icon-height"] = AttributeTypes.PixelsOrPercent,
                ["icon-padding"] = AttributeTypes.FourPixelsOrPercent,
                ["icon-size"] = AttributeTypes.PixelsOrPercent,
                ["inner-padding"] = AttributeTypes.FourPixelsOrPercent,
                ["line-height"] = AttributeTypes.PixelsOrPercent,
                ["mode"] = new EnumType("horizontal", "vertical"),
                ["padding"] = AttributeTypes.FourPixelsOrPercent,
                ["padding-bottom"] = AttributeTypes.PixelsOrPercent,
                ["padding-left"] = AttributeTypes.PixelsOrPercent,
                ["padding-right"] = AttributeTypes.PixelsOrPercent,
                ["padding-top"] = AttributeTypes.PixelsOrPercent,
                ["table-layout"] = new EnumType("auto", "fixed"),
                ["text-decoration"] = AttributeTypes.String,
                ["text-padding"] = AttributeTypes.FourPixelsOrPercent,
                ["vertical-align"] = AttributeTypes.VerticalAlign
            };

        public override Attributes? DefaultAttributes { get; } =
            new Attributes
            {
                ["align"] = "center",
                ["border-radius"] = "3px",
                ["color"] = "#333333",
                ["font-family"] = "Ubuntu, Helvetica, Arial, sans-serif",
                ["font-size"] = "13px",
                ["icon-size"] = "20px",
                ["line-height"] = "22px",
                ["mode"] = "horizontal",
                ["padding"] = "10px 25px",
                ["text-decoration"] = "none"
            };

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var mode = node.GetAttribute("mode");

            if (mode == "horizontal")
            {
                RenderHorizontal(renderer, node);
            }
            else
            {
                RenderVertical(renderer, node);
            }
        }

        private static void RenderHorizontal(IHtmlRenderer renderer, INode node)
        {
            var align = node.GetAttribute("align");

            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementStart("table")
                .Attr("align", align)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("margin", "0px");

            renderer.ElementStart("tr");

            renderer.Content("<![endif]-->");

            renderer.RenderChildren(new ChildOptions
            {
                Renderer = child =>
                {
                    if (child.Node.Component.Raw)
                    {
                        child.Render();
                    }
                    else
                    {
                        renderer.Content("<!--[if mso | IE]>");

                        renderer.ElementStart("td");

                        renderer.ElementStart("tr");

                        renderer.ElementStart("table")
                            .Attr("align", child.Node.GetAttribute("align"))
                            .Attr("border", "0")
                            .Attr("cellpadding", "0")
                            .Attr("cellspacing", "0")
                            .Attr("role", "presentation")
                            .Style("display", "inline-table")
                            .Style("float", "none");

                        renderer.ElementStart("tbody");

                        child.Render();

                        renderer.ElementEnd("tbody");

                        renderer.Content("<!--[if mso | IE]>");
                        renderer.ElementEnd("td");
                        renderer.Content("<![endif]-->");
                    }
                }
            });

            renderer.Content("<!--[if mso | IE]>");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");
            renderer.Content("<![endif]-->");
        }

        private static void RenderVertical(IHtmlRenderer renderer, INode node)
        {
            renderer.ElementStart("table") // Table-vertical
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("margin", "0px");

            renderer.ElementStart("tbody");

            renderer.RenderChildren();

            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
        }
    }
}
