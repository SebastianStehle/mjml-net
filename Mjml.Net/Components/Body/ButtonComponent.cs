﻿namespace Mjml.Net.Components.Body
{
    public sealed class ButtonComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-button";

        public override AllowedAttributes? AllowedAttributes { get; } =
            new AllowedAttributes
            {
                ["align"] = AttributeTypes.String,
                ["background-color"] = AttributeTypes.Color,
                ["border"] = AttributeTypes.String,
                ["border-bottom"] = AttributeTypes.Pixels,
                ["border-left"] = AttributeTypes.Pixels,
                ["border-radius"] = AttributeTypes.Pixels,
                ["border-right"] = AttributeTypes.Pixels,
                ["border-top"] = AttributeTypes.Pixels,
                ["color"] = AttributeTypes.Color,
                ["container-background-color"] = AttributeTypes.Color,
                ["font-family"] = AttributeTypes.String,
                ["font-size"] = AttributeTypes.Pixels,
                ["font-style"] = AttributeTypes.String,
                ["font-weight"] = AttributeTypes.String,
                ["height"] = AttributeTypes.PixelsOrPercent,
                ["href"] = AttributeTypes.String,
                ["inner-padding"] = AttributeTypes.FourPixelsOrPercent,
                ["letter-spacing"] = AttributeTypes.Pixels,
                ["line-height"] = AttributeTypes.PixelsOrPercent,
                ["name"] = AttributeTypes.String,
                ["padding"] = AttributeTypes.FourPixelsOrPercent,
                ["padding-bottom"] = AttributeTypes.PixelsOrPercent,
                ["padding-left"] = AttributeTypes.PixelsOrPercent,
                ["padding-right"] = AttributeTypes.PixelsOrPercent,
                ["padding-top"] = AttributeTypes.PixelsOrPercent,
                ["rel"] = AttributeTypes.String,
                ["target"] = AttributeTypes.String,
                ["text-align"] = AttributeTypes.String,
                ["text-decoration"] = AttributeTypes.String,
                ["text-transform"] = AttributeTypes.String,
                ["vertical-align"] = AttributeTypes.String,
                ["width"] = AttributeTypes.PixelsOrPercent,
            };

        public override Attributes? DefaultAttributes { get; } =
            new Attributes
            {
                ["align"] = "center",
                ["background-color"] = "#414141",
                ["border"] = "none",
                ["border-radius"] = "3px",
                ["color"] = "#FFFFFF",
                ["font-family"] = "Ubuntu, Helvetica, Arial, sans-serif",
                ["font-size"] = "13px",
                ["font-weight"] = "normal",
                ["inner-padding"] = "10px 25px",
                ["line-height"] = "120%",
                ["padding"] = "10px 25px",
                ["target"] = "_blank",
                ["text-decoration"] = "none",
                ["text-transform"] = "none",
                ["vertical-align"] = "middle",
            };

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var backgroundColor = node.GetAttribute("background-color");
            var borderRadius = node.GetAttribute("border-radius");
            var fontStyle = node.GetAttribute("font-style");
            var height = node.GetAttribute("height");
            var href = node.GetAttribute("href");
            var innerPadding = node.GetAttribute("inner-padding");
            var textAlign = node.GetAttribute("text-align");
            var width = node.GetAttribute("width");

            var buttonHtmlTag = !string.IsNullOrEmpty(href) ? "a" : "p";

            renderer.ElementStart("table")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("border-collapse", "separate")
                .Style("width", width)
                .Style("line-height", "100%");

            renderer.ElementStart("tr");
            renderer.ElementStart("td")
                .Attr("align", "center")
                .Attr("bgcolor", backgroundColor)
                .Attr("role", "presentation")
                .Attr("valign", node.GetAttribute("vertical-align"))
                .Style("border", node.GetAttribute("border"))
                .Style("border-bottom", node.GetAttribute("border-bottom"))
                .Style("border-left", node.GetAttribute("border-left"))
                .Style("border-right", node.GetAttribute("border-right"))
                .Style("border-top", node.GetAttribute("border-top"))
                .Style("border-radius", borderRadius)
                .Style("cursor", "auto")
                .Style("font-style", fontStyle)
                .Style("height", height)
                .Style("mso-padding-alt", innerPadding)
                .Style("text-align", textAlign)
                .Style("background", backgroundColor);

            renderer.ElementStart(buttonHtmlTag)
                .Attr("href", node.GetAttribute("href"))
                .Attr("rel", node.GetAttribute("rel"))
                .Attr("name", node.GetAttribute("name"))
                .Attr("target", !string.IsNullOrEmpty(href) ? node.GetAttribute("target") : null)
                .Style("display", "inline-block")
                .Style("width", CalculateButtonWidth(node))
                .Style("background", backgroundColor)
                .Style("color", node.GetAttribute("color"))
                .Style("font-family", node.GetAttribute("font-family"))
                .Style("font-style", fontStyle)
                .Style("font-size", node.GetAttribute("font-size"))
                .Style("font-weight", node.GetAttribute("font-weight"))
                .Style("line-height", node.GetAttribute("line-height"))
                .Style("letter-spacing", node.GetAttribute("letter-spacing"))
                .Style("margin", "0")
                .Style("text-decoration", node.GetAttribute("text-decoration"))
                .Style("text-transform", node.GetAttribute("text-transform"))
                .Style("letter-spacing", node.GetAttribute("letter-spacing"))
                .Style("padding", innerPadding)
                .Style("mso-padding-alt", "0px")
                .Style("text-align", textAlign)
                .Style("border-radius", borderRadius);

            renderer.Content(node.GetContent());

            renderer.ElementEnd(buttonHtmlTag);
            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");
        }

        private static string? CalculateButtonWidth(INode node)
        {
            var width = node.GetAttribute("width");

            if (string.IsNullOrEmpty(width) || !width.Contains("px", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var widthParsed = node.GetAttributeNumber("width");

            var borders =
                node.GetShorthandBorderValue("left") +
                node.GetShorthandBorderValue("right");

            var innerPadding =
                node.GetShorthandAttributeValue("inner-padding", "left") +
                node.GetShorthandAttributeValue("inner-padding", "right");

            return $"{widthParsed - innerPadding - borders}px";
        }
    }
}
