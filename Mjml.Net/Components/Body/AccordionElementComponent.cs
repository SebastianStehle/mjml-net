using Mjml.Net.Internal;

namespace Mjml.Net.Components.Body;

public partial class AccordionElementComponent : BodyComponentBase
{
    private static readonly AllowedParents Parents = new AllowedParents
    {
        "mj-accordion"
    };

    public override AllowedParents? AllowedParents => Parents;

    public override string ComponentName => "mj-accordion-element";

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

    protected override void BeforeBind(GlobalContext context)
    {
        Binder binder;

        if (!ChildNodes.Any(x => x is AccordionTitleComponent))
        {
            var child = new AccordionTitleComponent();

            binder = DefaultPools.Binders.Get();
            binder.Setup(context, this, child.ComponentName);

            child.SetBinder(binder);

            InsertChild(child, 0);
        }

        if (!ChildNodes.Any(x => x is AccordionTextComponent))
        {
            var child = new AccordionTextComponent();

            binder = DefaultPools.Binders.Get();
            binder.Setup(context, this, child.ComponentName);

            child.SetBinder(binder);

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
