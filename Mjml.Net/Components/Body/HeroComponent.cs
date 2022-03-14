namespace Mjml.Net.Components.Body
{
    public sealed class HeroComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-hero";

        public override AllowedAttributes? AllowedAttributes { get; } =
            new AllowedAttributes
            {
                ["background-color"] = AttributeTypes.String,
                ["background-height"] = AttributeTypes.PixelsOrPercent,
                ["background-position"] = AttributeTypes.String,
                ["background-url"] = AttributeTypes.String,
                ["background-width"] = AttributeTypes.PixelsOrPercent,
                ["border-radius"] = AttributeTypes.String,
                ["container-background-color"] = AttributeTypes.Color,
                ["css-class"] = AttributeTypes.String,
                ["height"] = AttributeTypes.PixelsOrPercent,
                ["inner-background-color"] = AttributeTypes.Color,
                ["inner-padding"] = AttributeTypes.FourPixelsOrPercent,
                ["inner-padding-bottom"] = AttributeTypes.PixelsOrPercent,
                ["inner-padding-left"] = AttributeTypes.PixelsOrPercent,
                ["inner-padding-right"] = AttributeTypes.PixelsOrPercent,
                ["inner-padding-top"] = AttributeTypes.PixelsOrPercent,
                ["mode"] = AttributeTypes.String,
                ["padding"] = AttributeTypes.FourPixelsOrPercent,
                ["padding-bottom"] = AttributeTypes.PixelsOrPercent,
                ["padding-left"] = AttributeTypes.PixelsOrPercent,
                ["padding-right"] = AttributeTypes.PixelsOrPercent,
                ["padding-top"] = AttributeTypes.PixelsOrPercent,
                ["vertical-align"] = AttributeTypes.VerticalAlign
            };

        public override Attributes? DefaultAttributes { get; } =
            new Attributes
            {
                ["background-color"] = "#ffffff",
                ["background-position"] = "center center",
                ["height"] = "0px",
                ["mode"] = "fixed-height",
                ["padding"] = "0px",
                ["vertical-align"] = "top"
            };

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var containerWidth = renderer.GetContainerWidth();

            var backgroundColor = node.GetAttribute("background-color");
            var backgroundHeight = node.GetAttribute("background-height");
            var backgroundPosition = node.GetAttribute("background-position");
            var backgroundUrl = node.GetAttribute("background-url");
            var backgroundWidth = node.GetAttribute("background-width");
            var parsedBackgroundHeight = UnitParser.Parse(backgroundHeight);
            var parsedBackgroundWidth = UnitParser.Parse(backgroundWidth);

            var backgroundString = backgroundColor;
            if (backgroundUrl != null)
            {
                backgroundString = $"{backgroundString} url({backgroundUrl}) no-repeat {backgroundPosition} / cover";
            }

            var backgroundRatioValue = Math.Round(100 *
                parsedBackgroundHeight.Value /
                parsedBackgroundWidth.Value);
            var backgroundRatio = $"{backgroundRatioValue}px";

            var widthValue = parsedBackgroundWidth.Value;
            if (widthValue <= 0)
            {
                widthValue = containerWidth.Value;
            }

            var width = $"{widthValue}px";

            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementStart("table") // Style: outlook-table
                .Attr("align", "center")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Attr("width", containerWidth.String)
                .Style("width", containerWidth.StringWithUnit);

            renderer.ElementStart("tr");

            renderer.ElementStart("td")
                .Style("line-height", "0")
                .Style("font-size", "0")
                .Style("mso-line-height-rule", "exactly"); // Style: outlook-td

            renderer.ElementStart("v:image") // Style: outlook-image
                .Attr("src", node.GetAttribute("background-url"))
                .Attr("xmlns:v", "urn:schemas-microsoft-com:vml")
                .Style("border", "0")
                .Style("height", backgroundHeight)
                .Style("mso-position-horizontal", "center")
                .Style("position", "absolute")
                .Style("top", "0")
                .Style("width", width)
                .Style("z-index", "-3");

            renderer.Content("<![endif]-->");

            renderer.ElementStart("div") // Style div
                .Attr("align", node.GetAttribute("align"))
                .Attr("class", node.GetAttribute("css-class"))
                .Style("margin", "0 auto")
                .Style("max-width", containerWidth.StringWithUnit);

            renderer.ElementStart("table") // Style table
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("width", "100%");

            renderer.ElementStart("tbody");

            renderer.ElementStart("tr")
                .Style("vertical-align", "top"); // Style tr

            if (node.GetAttribute("mode") == "fluid-height")
            {
                static void MagicId(IHtmlRenderer renderer, string backgroundRatio)
                {
                    renderer.ElementStart("td") // Style td-fluid
                        .Style("width", "0.01%")
                        .Style("padding-bottom", backgroundRatio)
                        .Style("mso-padding-bottom-alt", "0");
                    renderer.ElementEnd("td");
                }

                MagicId(renderer, backgroundRatio);

                renderer.ElementStart("td") // Style: hero
                    .Attr("background", backgroundUrl)
                    .Style("background", backgroundString)
                    .Style("background-position", backgroundPosition)
                    .Style("background-radius", node.GetAttribute("background-radius"))
                    .Style("background-repeat", "no-repeat")
                    .Style("padding", node.GetAttribute("padding"))
                    .Style("padding-bottom", node.GetAttribute("padding-bottom"))
                    .Style("padding-left", node.GetAttribute("padding-left"))
                    .Style("padding-right", node.GetAttribute("padding-right"))
                    .Style("padding-top", node.GetAttribute("padding-top"))
                    .Style("vertical-align", node.GetAttribute("vertical-align"));

                RenderContent(renderer, node, containerWidth);

                renderer.ElementEnd("td");

                MagicId(renderer, backgroundRatio);
            }
            else
            {
                var height =
                    UnitParser.Parse(node.GetAttribute("height")).Value -
                        node.GetShorthandAttributeValue("padding-top", "padding") -
                        node.GetShorthandAttributeValue("padding-bottom", "padding");

                renderer.ElementStart("td") // Style: hero
                    .Attr("background", backgroundUrl)
                    .Attr("height", height.ToInvariantString())
                    .Style("background", backgroundString)
                    .Style("background-position", backgroundPosition)
                    .Style("background-radius", node.GetAttribute("background-radius"))
                    .Style("background-repeat", "no-repeat")
                    .Style("padding", node.GetAttribute("padding"))
                    .Style("padding-bottom", node.GetAttribute("padding-bottom"))
                    .Style("padding-left", node.GetAttribute("padding-left"))
                    .Style("padding-right", node.GetAttribute("padding-right"))
                    .Style("padding-top", node.GetAttribute("padding-top"))
                    .Style("vertical-align", node.GetAttribute("vertical-align"));

                RenderContent(renderer, node, containerWidth);

                renderer.ElementEnd("td");
            }

            renderer.ElementEnd("tr");
            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
            renderer.ElementEnd("div");

            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");

            renderer.Content("<![endif]-->");
        }

        private static void RenderContent(IHtmlRenderer renderer, INode node, ContainerWidth containerWidth)
        {
            var innerBackgroundColor = node.GetAttribute("inner-background-color");

            var align = node.GetAttribute("align");

            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementStart("table") // Style: outlook-inner-table
                .Attr("align", align)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("width", containerWidth.String)
                .Style("width", containerWidth.StringWithUnit);

            renderer.ElementStart("tr");

            renderer.ElementStart("td") // Style: outlook-inner-td
                .Style("background-color", innerBackgroundColor)
                .Style("inner-padding", node.GetAttribute("inner-padding"))
                .Style("inner-padding-bottom", node.GetAttribute("inner-padding-bottom"))
                .Style("inner-padding-left", node.GetAttribute("inner-padding-left"))
                .Style("inner-padding-right", node.GetAttribute("inner-padding-right"))
                .Style("inner-padding-top", node.GetAttribute("inner-padding-top"));

            renderer.Content("<![endif]-->");

            renderer.ElementStart("div") // Style: inner-div
                .Attr("align", align)
                .Class("mj-hero-content")
                .Style("background-color", innerBackgroundColor)
                .Style("float", align)
                .Style("margin", "0px auto")
                .Style("width", node.GetAttribute("width"));

            renderer.ElementStart("table") // Style: inner-table
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("width", "100%")
                .Style("margin", "0px");

            renderer.ElementStart("tbody");
            renderer.ElementStart("tr");
            renderer.ElementStart("td");

            renderer.ElementStart("table") // Style: inner-table
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("width", "100%")
                .Style("margin", "0px");

            renderer.ElementStart("tbody");

            var innerWidth =
                containerWidth.Value -
                    node.GetShorthandAttributeValue("padding-left", "padding") -
                    node.GetShorthandAttributeValue("padding-right", "padding");

            renderer.RenderChildren(new ChildOptions
            {
                Renderer = child =>
                {
                    renderer.SetContainerWidth(innerWidth);

                    if (child.Node.Component.Raw)
                    {
                        child.Render();
                    }
                    else
                    {
                        var backgroundColor = child.Node.GetAttribute("container-background-color");

                        renderer.ElementStart("tr");

                        renderer.ElementStart("td")
                            .Attr("align", child.Node.GetAttribute("align"))
                            .Attr("background", backgroundColor)
                            .Attr("class", child.Node.GetAttribute("css-class"))
                            .Style("background", backgroundColor)
                            .Style("font-size", "0px")
                            .Style("padding", child.Node.GetAttribute("padding"))
                            .Style("padding-bottom", child.Node.GetAttribute("padding-bottom"))
                            .Style("padding-left", child.Node.GetAttribute("padding-left"))
                            .Style("padding-right", child.Node.GetAttribute("padding-right"))
                            .Style("padding-top", child.Node.GetAttribute("padding-top"))
                            .Style("word-break", child.Node.GetAttribute("break-word"));

                        child.Render();

                        renderer.ElementEnd("td");
                        renderer.ElementEnd("tr");
                    }
                }
            });

            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
            renderer.ElementEnd("div");

            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");

            renderer.Content("<![endif]-->");
        }
    }
}
