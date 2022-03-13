namespace Mjml.Net.Components.Body
{
    public sealed class SectionComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-section";

        public override AllowedAttributes? AllowedAttributes { get; } =
          new AllowedAttributes
          {
              ["background-color"] = AttributeTypes.Color,
              ["background-position"] = AttributeTypes.String,
              ["background-position-x"] = AttributeTypes.String,
              ["background-position-y"] = AttributeTypes.String,
              ["background-repeat"] = AttributeTypes.String,
              ["background-size"] = AttributeTypes.String,
              ["background-url"] = AttributeTypes.String,
              ["border"] = AttributeTypes.String,
              ["border-bottom"] = AttributeTypes.String,
              ["border-left"] = AttributeTypes.String,
              ["border-radius"] = AttributeTypes.FourPixelsOrPercent,
              ["border-right"] = AttributeTypes.String,
              ["border-top"] = AttributeTypes.String,
              ["css-class"] = AttributeTypes.String,
              ["direction"] = AttributeTypes.String,
              ["full-width"] = AttributeTypes.String,
              ["padding"] = AttributeTypes.FourPixelsOrPercent,
              ["padding-bottom"] = AttributeTypes.PixelsOrPercent,
              ["padding-left"] = AttributeTypes.PixelsOrPercent,
              ["padding-right"] = AttributeTypes.PixelsOrPercent,
              ["padding-top"] = AttributeTypes.PixelsOrPercent,
              ["text-align"] = AttributeTypes.Align
          };

        public override Attributes? DefaultAttributes { get; } =
            new Attributes
            {
                ["background-position"] = "top center",
                ["background-repeat"] = "repeat",
                ["background-size"] = "auto",
                ["border"] = "none",
                ["direction"] = "ltr",
                ["padding"] = "20px 0",
                ["text-align"] = "center"
            };

        public override void Render(IHtmlRenderer renderer, INode node)
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
            RenderSectionStart(renderer, node);

            if (HasBackground(node))
            {
                RenderSectionWithBackground(renderer, node);
            }
            else
            {
                RenderSection(renderer, node);
            }

            RenderSectionEnd(renderer, node);
        }

        private static void RenderSectionStart(IHtmlRenderer renderer, INode node)
        {
            // This may need to be changed to get the full width of the container opposed to the box width.
            var width = renderer.GetContext("width") as string;

            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementStart("table")
                .Attr("align", "center")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("width", width)
                .Class("outlook")
                .Class(node.GetAttribute("css-class"))
                .Style("width", width);

            renderer.ElementStart("tr");
            renderer.ElementStart("td")
                .Style("line-height", "0px")
                .Style("font-size", "0px")
                .Style("mso-line-height-rule", "exactly");

            renderer.Content("<![endif]-->");
        }

        private static void RenderSection(IHtmlRenderer renderer, INode node)
        {
            var isFullWidth = IsFullWidth(node);
            var containerOuterwidth = renderer.GetContext("width") as string;

            renderer.ElementStart("div")
                           .Attr("class", node.GetAttribute("css-class"))
                           .Style("margin", "0px auto")
                           .Style("border-radius", node.GetAttribute("border-radius"))
                           .Style("max-width", containerOuterwidth);

            renderer.ElementStart("table")
                .Attr("align", "center")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("width", "100%")
                .Style("border-radius", node.GetAttribute("border-radius"));

            renderer.ElementStart("tbody");
            renderer.ElementStart("tr");
            renderer.ElementStart("td")
                .Style("border", node.GetAttribute("border"))
                .Style("border-bottom", node.GetAttribute("border-bottom"))
                .Style("border-left", node.GetAttribute("border-left"))
                .Style("border-right", node.GetAttribute("border-right"))
                .Style("border-top", node.GetAttribute("border-top"))
                .Style("direction", node.GetAttribute("direction"))
                .Style("font-size", "0px")
                .Style("padding", node.GetAttribute("padding"))
                .Style("padding-bottom", node.GetAttribute("padding-bottom"))
                .Style("padding-left", node.GetAttribute("padding-left"))
                .Style("padding-right", node.GetAttribute("padding-right"))
                .Style("padding-top", node.GetAttribute("padding-top"))
                .Style("text-align", node.GetAttribute("text-align"));

            renderer.Content("<!--[if mso | IE]>");
            renderer.ElementStart("table")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation");
            renderer.Content("<![endif]-->");

            RenderChildren(renderer, node);

            renderer.Content("<!--[if mso | IE]>");
            renderer.ElementEnd("table");
            renderer.Content("<![endif]-->");

            renderer.ElementEnd("table");
            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
            renderer.ElementEnd("div");
        }

        private static void RenderSectionEnd(IHtmlRenderer renderer, INode node)
        {
            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");

            renderer.Content("<![endif]-->");
        }

        private static void RenderSectionWithBackground(IHtmlRenderer renderer, INode node)
        {
            throw new NotImplementedException();
        }

        private static void RenderChildren(IHtmlRenderer renderer, INode node)
        {
            renderer.Content("<!--[if mso | IE]>");
            renderer.ElementStart("tr");
            renderer.Content("<![endif]-->");

            renderer.RenderChildren(new ChildOptions
            {
                Renderer = child =>
                {
                    //renderer.Content("<!--[if mso | IE]>");
                    //renderer.ElementStart("td")
                    //    .Attr("align", node.GetAttribute("align"))
                    //    .Class("outlook")
                    //    .Style("");
                    //renderer.Content("<![endif]-->");

                    child.Render();
                }
            });

            renderer.Content("<!--[if mso | IE]>");
            renderer.ElementEnd("tr");
            renderer.Content("<![endif]-->");
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
