using Mjml.Net.Internal;
using System.Xml;

namespace Mjml.Net.Components.Body
{
    public partial class AccordionElementComponent : BodyComponentBase
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mj-accordion"
        };

        public override string ComponentName => "mj-accordion-element";

        public override AllowedParents? AllowedAsChild => Parents;

        [Bind("background-color", BindType.Color)]
        public string? BackgroundColor;

        [Bind("border", BindType.String)]
        public string? Border;

        [Bind("font-family", BindType.String)]
        public string? FontFamily;

        [Bind("icon-align", BindType.VerticalAlign)]
        public string? IconAlign;

        [Bind("icon-height", BindType.PixelsOrPercent)]
        public string? IconHeight;

        [Bind("icon-position", BindType.LeftRight)]
        public string? IconPosition;

        [Bind("icon-unwrapped-alt", BindType.String)]
        public string? IconUnwrappedAlt;

        [Bind("icon-unwrapped-url", BindType.String)]
        public string? IconUnwrappedUrl;

        [Bind("icon-width", BindType.PixelsOrPercent)]
        public string? IconWidth;

        [Bind("icon-wrapped-alt", BindType.String)]
        public string? IconWrappedAlt;

        [Bind("icon-wrapped-url", BindType.String)]
        public string? IconWrappedUrl;

        public override void AfterBind(GlobalContext context, XmlReader reader)
        {
            Binder? binder = null;

            if (!ChildNodes.Any(x => x is AccordionTitleComponent))
            {
                binder ??= new Binder(context);
                binder.Clear(this, "mj-accordion-title");

                var child = new AccordionTitleComponent();

                child.Bind(binder, context, reader);

                InsertChild(child, 0);
            }

            if (!ChildNodes.Any(x => x is AccordionTextComponent))
            {
                binder ??= new Binder(context);
                binder.Clear(this, "mj-accordion-text");

                var child = new AccordionTextComponent();

                child.Bind(binder, context, reader);

                AddChild(child);
            }
        }

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.StartElement("tr")
                .Class(CssClass);

            renderer.StartElement("td") // Style td
                .Style("background-color", BackgroundColor)
                .Style("padding", "0px");

            renderer.StartElement("label") // Style label
                .Class("mj-accordion-element")
                .Style("font-family", FontFamily)
                .Style("font-size", "13px");

            renderer.StartConditional("<!--[if !mso | IE]><!-->");
            {
                renderer.StartElement("input", true) // Style input
                    .Attr("type", "checkbox")
                    .Class("mj-accordion-checkbox")
                    .Style("display", "none");
            }
            renderer.EndConditional("<!--<![endif]-->");

            renderer.StartElement("div");

            foreach (var child in ChildNodes)
            {
                child.Render(renderer, context);
            }

            renderer.EndElement("div");
            renderer.EndElement("label");
            renderer.EndElement("td");
            renderer.EndElement("tr");
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
