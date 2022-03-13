using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mjml.Net.Components.Body
{
    public sealed class ButtonComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-button";

        public override AllowedAttributes? AllowedAttributes { get; } =
            new AllowedAttributes
            {
                ["name"] = AttributeTypes.String,
                ["href"] = AttributeTypes.String,
                ["height"] = AttributeTypes.PixelsOrPercent,
                ["width"] = AttributeTypes.PixelsOrPercent,
                ["rel"] = AttributeTypes.String,
                ["target"] = AttributeTypes.String,
                ["text-align"] = AttributeTypes.String,
                ["text-decoration"] = AttributeTypes.String,
                ["text-transform"] = AttributeTypes.String,
                ["vertical-align"] = AttributeTypes.String,
                ["align"] = AttributeTypes.String,
                ["color"] = AttributeTypes.Color,
                ["container-background-color"] = AttributeTypes.Color,
                ["background-color"] = AttributeTypes.Color,
                ["padding"] = AttributeTypes.PixelsOrPercent,
                ["padding-bottom"] = AttributeTypes.PixelsOrPercent,
                ["padding-left"] = AttributeTypes.PixelsOrPercent,
                ["padding-right"] = AttributeTypes.PixelsOrPercent,
                ["padding-top"] = AttributeTypes.PixelsOrPercent,
                ["border-top"] = AttributeTypes.Pixels,
                ["border-right"] = AttributeTypes.Pixels,
                ["border-bottom"] = AttributeTypes.Pixels,
                ["border-left"] = AttributeTypes.Pixels,
                ["border"] = AttributeTypes.String,
                ["border-radius"] = AttributeTypes.Pixels,
                ["inner-padding"] = AttributeTypes.String,
                ["font-family"] = AttributeTypes.String,
                ["font-size"] = AttributeTypes.Pixels,
                ["font-style"] = AttributeTypes.String,
                ["font-weight"] = AttributeTypes.String,
                ["letter-spacing"] = AttributeTypes.Pixels,
                ["line-height"] = AttributeTypes.PixelsOrPercent,
            };

        public override Attributes? DefaultAttributes { get; } =
            new Attributes
            {
                ["target"] = "_blank",
                ["text-align"] = "center",
                ["text-decoration"] = "none",
                ["text-transform"] = "none",
                ["vertical-align"] = "middle",
                ["align"] = "center",
                ["color"] = "#FFFFFF",
                ["background-color"] = "#414141",
                ["padding"] = "10px 25px",
                ["border"] = "none",
                ["border-radius"] = "3px",
                ["inner-padding"] = "10px 25px",
                ["font-family"] = "Ubuntu, Helvetica, Arial, sans-serif",
                ["font-size"] = "13px",
                ["font-weight"] = "normal",
                ["line-height"] = "120%",
            };

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            string? backgroundColor = node.GetAttribute("background-color");
            string? width = node.GetAttribute("width");
            string? height = node.GetAttribute("height");
            string? href = node.GetAttribute("href");
            string? fontStyle = node.GetAttribute("font-style");
            string? borderRadius = node.GetAttribute("border-radius");
            string? textAlign = node.GetAttribute("text-align");
            string? innerPadding = node.GetAttribute("inner-padding");

            string buttonHtmlTag = !string.IsNullOrEmpty(href) ? "a" : "p";

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

            RenderChildren(renderer, node);

            renderer.ElementEnd(buttonHtmlTag);
            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");
        }

        private static void RenderChildren(IHtmlRenderer renderer, INode node)
        {
            renderer.RenderChildren(new ChildOptions
            {
                Renderer = child =>
                {
                    child.Render();
                }
            });
        }

        private string? CalculateButtonWidth(INode node)
        {
            return node.GetAttribute("width");
        }
    }
}
