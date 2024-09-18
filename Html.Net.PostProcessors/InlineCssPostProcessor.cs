using AngleSharp;
using AngleSharp.Css;
using AngleSharp.Dom;
using Mjml.Net;

namespace Html.Net;

public sealed class InlineCssPostProcessor : IPostProcessor
{
    public static readonly InlineCssPostProcessor Instance = new InlineCssPostProcessor();

    private InlineCssPostProcessor()
    {
    }

    public async ValueTask<string> PostProcessAsync(string html, MjmlOptions options,
        CancellationToken ct)
    {
        var config = Configuration.Default.WithCss().Without<ICssDefaultStyleSheetProvider>();
        var context = BrowsingContext.New(config);

        var document = await context.OpenAsync(req => req.Content(html), ct);

        Traverse(document, a => RenameNonInline(a, document));
        Traverse(document, InlineStyle);
        Traverse(document, a => RestoreNonInline(a, document));

        var result = document.ToHtml();

        return result;
    }

    private static void Traverse(INode node, Action<IElement> action)
    {
        foreach (var child in node.ChildNodes.ToList())
        {
            Traverse(child, action);
        }

        if (node is IElement element)
        {
            action(element);
        }
    }

    private static void InlineStyle(IElement element)
    {
        var currentStyle = element.ComputeCurrentStyle();

        if (currentStyle.Any())
        {
            var css = currentStyle.ToCss();

            element.SetAttribute("style", css);
        }
    }

    private static void RenameNonInline(IElement element, IDocument document)
    {
        if (string.Equals(element.TagName, "style", StringComparison.OrdinalIgnoreCase) && !element.HasAttribute("inline"))
        {
            RenameTag(element, "non_inline_style", document);
        }
    }

    private static void RestoreNonInline(IElement element, IDocument document)
    {
        if (string.Equals(element.TagName, "non_inline_style", StringComparison.OrdinalIgnoreCase))
        {
            RenameTag(element, "style", document);
        }

        if (string.Equals(element.TagName, "style", StringComparison.OrdinalIgnoreCase) && element.HasAttribute("inline"))
        {
            element.Remove();
        }
    }

    private static void RenameTag(IElement node, string tagName, IDocument document)
    {
        var clone = document.CreateElement(tagName);

        foreach (var attribute in node.Attributes)
        {
            clone.SetAttribute(attribute.NamespaceUri, attribute.Name, attribute.Value);
        }

        var parent = node.Parent!;

        clone.InnerHtml = node.InnerHtml;

        parent.InsertBefore(clone, node);
        parent.RemoveChild(node);
    }
}
