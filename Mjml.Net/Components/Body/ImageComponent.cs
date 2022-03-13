using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Body
{
    public sealed class ImageComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-image";

        public override AllowedAttributes? AllowedAttributes { get; } =
            new AllowedAttributes
            {
                ["align"] = AttributeTypes.Align,
                ["alt"] = AttributeTypes.String,
                ["border"] = AttributeTypes.String,
                ["border-bottom"] = AttributeTypes.String,
                ["border-left"] = AttributeTypes.String,
                ["border-radius"] = AttributeTypes.FourPixelsOrPercent,
                ["border-right"] = AttributeTypes.String,
                ["border-top"] = AttributeTypes.String,
                ["container-background-color"] = AttributeTypes.Color,
                ["fluid-on-mobile"] = AttributeTypes.Boolean,
                ["font-size"] = AttributeTypes.Pixels,
                ["height"] = AttributeTypes.PixelsOrAuto,
                ["height"] = AttributeTypes.PixelsOrPercent,
                ["href"] = AttributeTypes.String,
                ["max-height"] = AttributeTypes.PixelsOrPercent,
                ["name"] = AttributeTypes.String,
                ["padding"] = AttributeTypes.FourPixelsOrPercent,
                ["padding-bottom"] = AttributeTypes.PixelsOrPercent,
                ["padding-left"] = AttributeTypes.PixelsOrPercent,
                ["padding-right"] = AttributeTypes.PixelsOrPercent,
                ["padding-top"] = AttributeTypes.PixelsOrPercent,
                ["rel"] = AttributeTypes.String,
                ["sizes"] = AttributeTypes.String,
                ["src"] = AttributeTypes.String,
                ["srcset"] = AttributeTypes.String,
                ["target"] = AttributeTypes.String,
                ["title"] = AttributeTypes.String,
                ["usemap"] = AttributeTypes.String,
                ["width"] = AttributeTypes.Pixels,
            };

        public override Attributes? DefaultAttributes { get; } =
            new Attributes
            {
                ["align"] = "center",
                ["border"] = "0",
                ["font-size"] = "13px",
                ["height"] = "auto",
                ["padding"] = "10px 25px",
                ["target"] = "_blank"
            };

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            renderer.SetGlobalData("mj-image", new DynamicStyle(HeadStyle));

            var fluidOnMobile = node.GetAttribute("fluid-on-mobile") == "true";
            var fullWidth = Equals(renderer.GetContext("full-width"), true);

            var widthConfigured = node.GetAttributeNumber("width");
            var widthAvailable = node.GetBoxWidths(renderer).Box;
            var widthMin = Math.Min(widthConfigured, widthAvailable);

            var href = node.GetAttribute("href");

            renderer.ElementStart("table")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Class(fluidOnMobile ? "mj-full-width-mobile" : null)
                .Style("border-collapse", "collapse")
                .Style("border-spacing", "0px")
                .Style("max-width", fullWidth ? "100%" : null)
                .Style("min-width", fullWidth ? "100%" : null)
                .Style("width", fullWidth ? $"{widthMin}px" : null);

            renderer.ElementStart("tbody");
            renderer.ElementStart("tr");

            renderer.ElementStart("td")
                .Style("width", fullWidth ? null : $"{widthMin}px")
                .Class(fluidOnMobile ? "mj-full-width-mobile" : null);

            if (!string.IsNullOrWhiteSpace(href))
            {
                renderer.ElementStart("a")
                  .Attr("href", node.GetAttribute("href"))
                  .Attr("name", node.GetAttribute("name"))
                  .Attr("rel", node.GetAttribute("rel"))
                  .Attr("target", node.GetAttribute("target"))
                  .Attr("title", node.GetAttribute("title"));

                RenderImage(renderer, node, widthMin, fullWidth);

                renderer.ElementEnd("a");
            }
            else
            {
                RenderImage(renderer, node, widthMin, fullWidth);
            }

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
        }

        private static string HeadStyle(IHtmlRenderer renderer)
        {
            var breakpoint = renderer.GlobalData.Values.OfType<Breakpoint>().First();

            return $@"
@media only screen and (max-width:${breakpoint.Value}) {{
  table.mj-full-width-mobile {{ width: 100% !important; }}
  td.mj-full-width-mobile {{ width: auto !important; }}
}}";
        }

        private static void RenderImage(IHtmlRenderer renderer, INode node, double width, bool fullWidth)
        {
            renderer.ElementStart("img", true)
                .Attr("alt", node.GetAttribute("alt"))
                .Attr("height", node.GetAttributeNumberOrAuto("height"))
                .Attr("sizes", node.GetAttribute("sizes"))
                .Attr("src", node.GetAttribute("src"))
                .Attr("srcset", node.GetAttribute("srcset"))
                .Attr("title", node.GetAttribute("title"))
                .Attr("usemap", node.GetAttribute("usemap"))
                .Attr("width", width.ToInvariantString())
                .Style("border", node.GetAttribute("border"))
                .Style("border-bottom", node.GetAttribute("border-bottom"))
                .Style("border-left", node.GetAttribute("border-left"))
                .Style("border-radius", node.GetAttribute("border-radius"))
                .Style("border-right", node.GetAttribute("border-right"))
                .Style("border-top", node.GetAttribute("border-top"))
                .Style("display", "block")
                .Style("font-size", node.GetAttribute("font-size"))
                .Style("height", node.GetAttribute("height"))
                .Style("max-height", node.GetAttribute("max-height"))
                .Style("max-width", fullWidth ? "100%" : null)
                .Style("min-width", fullWidth ? "100%" : null)
                .Style("outline", "none")
                .Style("text-decoration", "none")
                .Style("width", "100%");
        }
    }
}
