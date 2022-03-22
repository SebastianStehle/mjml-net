using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Body
{
    public partial class AccordionComponent : BodyComponentBase
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mj-column",
            "mj-hero"
        };

        public override AllowedParents? AllowedParents => Parents;

        public override string ComponentName => "mj-accordion";

        [Bind("border", BindType.String)]
        public string Border = "2px solid black";

        [Bind("container-background-color", BindType.Color)]
        public string? ContainerBackgroundColor;

        [Bind("font-family", BindType.String)]
        public string FontFamily = "Ubuntu, Helvetica, Arial, sans-serif";

        [Bind("icon-align", BindType.VerticalAlign)]
        public string IconAlign = "middle";

        [Bind("icon-height", BindType.PixelsOrPercent)]
        public string IconHeight = "32px";

        [Bind("icon-position", BindType.LeftRight)]
        public string IconPosition = "right";

        [Bind("icon-unwrapped-alt", BindType.String)]
        public string IconUnwrappedAlt = "-";

        [Bind("icon-unwrapped-url", BindType.String)]
        public string IconUnwrappedUrl = "https://i.imgur.com/w4uTygT.png";

        [Bind("icon-width", BindType.PixelsOrPercent)]
        public string IconWidth = "32px";

        [Bind("icon-wrapped-alt", BindType.String)]
        public string IconWrappedAlt = "+";

        [Bind("icon-wrapped-url", BindType.String)]
        public string IconWrappedUrl = "https://i.imgur.com/bIXv1bk.png";

        [Bind("padding", BindType.PixelsOrPercent)]
        public string Padding = "10px 25px";

        [Bind("padding-bottom", BindType.PixelsOrPercent)]
        public string? PaddingBottom;

        [Bind("padding-left", BindType.PixelsOrPercent)]
        public string? PaddingLeft;

        [Bind("padding-right", BindType.PixelsOrPercent)]
        public string? PaddingRight;

        [Bind("padding-top", BindType.PixelsOrPercent)]
        public string? PaddingTop;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            context.SetGlobalData(ComponentName, new Style(HeadStyle));

            renderer.StartElement("table") // Style table
                .Attr("cell-spacing", "0")
                .Attr("cell-padding", "0")
                .Class("mj-accordion")
                .Style("border", Border)
                .Style("border-bottom", "none")
                .Style("border-collapse", "collapse")
                .Style("font-family", FontFamily)
                .Style("width", "100%");

            renderer.StartElement("tbody");

            foreach (var child in ChildNodes)
            {
                child.Render(renderer, context);
            }

            renderer.EndElement("tbody");
            renderer.EndElement("table");
        }

        private void HeadStyle(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.Content("noinput.mj-accordion-checkbox { display:block!important; }");
            renderer.Content("@media yahoo, only screen and (min-width:0) {");
            renderer.Content("	.mj-accordion-element { display:block; }");
            renderer.Content("	input.mj-accordion-checkbox, .mj-accordion-less { display:none!important; }");
            renderer.Content("	input.mj-accordion-checkbox + * .mj-accordion-title { cursor:pointer; touch-action:manipulation; -webkit-user-select:none; -moz-user-select:none; user-select:none; }");
            renderer.Content("	input.mj-accordion-checkbox + * .mj-accordion-content { overflow:hidden; display:none; }");
            renderer.Content("	input.mj-accordion-checkbox + * .mj-accordion-more { display:block!important; }");
            renderer.Content("	input.mj-accordion-checkbox:checked + * .mj-accordion-content { display:block; }");
            renderer.Content("	input.mj-accordion-checkbox:checked + * .mj-accordion-more { display:none!important; }");
            renderer.Content("	input.mj-accordion-checkbox:checked + * .mj-accordion-less { display:block!important; }");
            renderer.Content(".moz-text-html input.mj-accordion-checkbox + * .mj-accordion-title { cursor: auto; touch-action: auto; -webkit-user-select: auto; -moz-user-select: auto; user-select: auto; }");
            renderer.Content(".moz-text-html input.mj-accordion-checkbox + * .mj-accordion-content { overflow: hidden; display: block; }");
            renderer.Content(".moz-text-html input.mj-accordion-checkbox + * .mj-accordion-ico { display: none; }");
            renderer.Content("@goodbye { @gmail }");
        }

        public override string? GetInheritingAttribute(string name)
        {
            switch (name)
            {
                case "border":
                    return Border;
                case "icon-align":
                    return IconAlign;
                case "icon-height":
                    return IconHeight;
                case "icon-position":
                    return IconPosition;
                case "icon-width":
                    return IconWidth;
                case "icon-unwrapped-url":
                    return IconUnwrappedUrl;
                case "icon-unwrapped-alt":
                    return IconUnwrappedAlt;
                case "icon-wrapped-url":
                    return IconWrappedUrl;
                case "icon-wrapped-alt":
                    return IconWrappedAlt;
            }

            return null;
        }
    }
}
