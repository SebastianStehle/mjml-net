using Mjml.Net.Helpers;
using Mjml.Net.Types;

namespace Mjml.Net.Components;

public sealed partial class IncludeComponent : Component
{
    public override string ComponentName => "mj-include";

    public override bool Raw => true;

    [Bind("path", typeof(PathValidator))]
    public string Path;

    [Bind("type", typeof(TypeValidator))]
    public string Type;

    public IncludeType ActualType { get; private set; }

    public override void Read(IHtmlReader htmlReader, IMjmlReader mjmlReader, GlobalContext context)
    {
        var actualPath = Binder.GetAttribute("path");
        var actualType = Binder.GetAttribute("type");

        switch (actualType)
        {
            case "html":
                ActualType = IncludeType.Html;
                break;
            case "css":
                ActualType = IncludeType.Css;
                break;
            case "mjml":
                ActualType = IncludeType.Mjml;
                break;
            default:
                if (actualPath?.EndsWith(".html", StringComparison.OrdinalIgnoreCase) == true)
                {
                    ActualType = IncludeType.Html;
                }
                else if (actualPath?.EndsWith(".css", StringComparison.OrdinalIgnoreCase) == true)
                {
                    ActualType = IncludeType.Css;
                }
                else
                {
                    ActualType = IncludeType.Mjml;
                }

                break;
        }

        if (ActualType != IncludeType.Mjml || string.IsNullOrWhiteSpace(actualPath))
        {
            return;
        }

        var content = context.Options.FileLoader?.LoadText(actualPath);

        if (content == null)
        {
            return;
        }

        mjmlReader.ReadFragment(content, actualPath, Parent!);
    }

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        if (ActualType == IncludeType.Mjml || string.IsNullOrWhiteSpace(Path))
        {
            // Render the children that have been added in the bind method.
            RenderChildren(renderer, context);
            return;
        }

        var content = context.Options.FileLoader?.LoadText(Path);

        if (content == null)
        {
            return;
        }

        if (ActualType == IncludeType.Html)
        {
            // Allow pretty formatting and indentation.
            renderer.Content(content);
        }
        else if (ActualType == IncludeType.Css)
        {
            var style = Style.Static(new InnerTextOrHtml(content));

            // Allow multiple styles and render them later.
            context.AddGlobalData(style);
        }
    }

    public sealed class PathValidator : IType
    {
        public bool Validate(string value, ref ValidationContext context)
        {
            return !string.IsNullOrWhiteSpace(value) && context.Options?.FileLoader?.ContainsFile(value) == true;
        }
    }

    internal sealed class TypeValidator : EnumType
    {
        public TypeValidator()
            : base(false, "mjml", "html", "css")
        {
        }
    }
}
