using AngleSharp;
using AngleSharp.Dom;
using Mjml.Net;

namespace Html.Net;

public sealed class InlineProcessor : IPostProcessor
{
    public static readonly InlineProcessor Instance = new InlineProcessor();

    private InlineProcessor()
    {
    }

    public string PostProcess(string html, MjmlOptions options)
    {
        var config = Configuration.Default.WithCss();
        var context = BrowsingContext.New(config);

        var document = context.OpenAsync(req => req.Content(html)).Result;

        Traverse(document, a => RenameNonInline(a, document));
        Traverse(document, InlineStyle);
        Traverse(document, a => RestoreNonInline(a, document));

        return document.ToString()!;
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
            RenameTag(element, "non_inline_style", document);
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
