using Mjml.Net.Helpers;
using Mjml.Net.Types;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Mjml.Net.Components;

public sealed partial class IncludeComponent : Component
{
    private static readonly (Type, string) ContextKey = (typeof(object), "FileContext");

    public override string ComponentName => "mj-include";

    public override bool Raw => true;

    [Bind("path", BindType.RequiredString)]
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

        if (context.Options.FileLoader == null)
        {
            return;
        }

        context.GlobalData.TryGetValue(ContextKey, out var parentContext);
        var includedFileInfo = new IncludedFileInfo(actualPath, (parentContext as FileContext)?.FileInfo);

        var content = context.Options.FileLoader.LoadText(includedFileInfo);

        if (!string.IsNullOrWhiteSpace(content))
        {
            context.GlobalData[ContextKey] = new FileContext(includedFileInfo);

            mjmlReader.ReadFragment(content, actualPath, Parent!);

            // Restore the previous context.
            if (parentContext != null)
            {
                context.GlobalData[ContextKey] = parentContext;
            }
            else
            {
                context.GlobalData.Remove(ContextKey);
            }
        }
    }

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        if (ActualType == IncludeType.Mjml || string.IsNullOrWhiteSpace(Path))
        {
            // Render the children that have been added in the bind method.
            RenderChildren(renderer, context);
            return;
        }

        if (context.Options.FileLoader == null)
        {
            return;
        }

        context.GlobalData.TryGetValue(ContextKey, out var parentContext);
        var includedFileInfo = new IncludedFileInfo(Path, (parentContext as FileContext)?.FileInfo);
        var content = context.Options.FileLoader.LoadText(includedFileInfo);

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
            // Allow multiple styles and render them later.
            context.AddGlobalData(Style.Static(new InnerTextOrHtml(content)));
        }
    }

    internal sealed class TypeValidator : EnumType
    {
        public TypeValidator()
            : base(false, "mjml", "html", "css")
        {
        }
    }

    /// <summary>
    /// Represents information about included file.
    /// </summary>
    public sealed record FileContext(IncludedFileInfo FileInfo) : GlobalData;
}
