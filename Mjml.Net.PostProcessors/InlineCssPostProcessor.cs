using System.Collections;
using AngleSharp;
using AngleSharp.Css;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;

namespace Mjml.Net;

public sealed class InlineCssPostProcessor : IAngleSharpPostProcessor
{
    private const string FallbackStyle = "non_inline_style";

    public static readonly IPostProcessor Instance = new AngleSharpPostProcessor(new InlineCssPostProcessor());

    public ValueTask ProcessAsync(IDocument document, MjmlOptions options,
        CancellationToken ct)
    {
        Traverse(document, a => RenameNonInline(a, document));

        var styles = GetStyles(document);
        if (styles != null)
        {
            Traverse(document, a => InlineStyle(a, styles));
        }

        Traverse(document, a => RestoreNonInline(a, document));
        return default;
    }

    private static IStyleCollection? GetStyles(IDocument document)
    {
        var device = document.Context.GetService<IRenderDevice>();
        if (device == null)
        {
            return null;
        }

        var view = document.DefaultView;
        if (view == null)
        {
            return null;
        }

        return new CachedStyleCollection(view.GetStyleCollection(device));
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

    private static void InlineStyle(IElement element, IStyleCollection styles)
    {
        var currentStyle = styles.GetDeclarations(element);
        if (currentStyle.Any())
        {
            var css = currentStyle.ToCss();

            element.SetAttribute(TagNames.Style, css);
        }
    }

    private static void RenameNonInline(IElement element, IDocument document)
    {
        if (string.Equals(element.TagName, TagNames.Style, StringComparison.OrdinalIgnoreCase) && !IsInline(element))
        {
            RenameTag(element, FallbackStyle, document);
        }
    }

    private static void RestoreNonInline(IElement element, IDocument document)
    {
        if (string.Equals(element.TagName, FallbackStyle, StringComparison.OrdinalIgnoreCase))
        {
            RenameTag(element, TagNames.Style, document);
        }

        if (string.Equals(element.TagName, TagNames.Style, StringComparison.OrdinalIgnoreCase) && IsInline(element))
        {
            element.Remove();
        }
    }

    private static bool IsInline(IElement element)
    {
        return element.HasAttribute("inline");
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

    private sealed class CachedStyleCollection(IStyleCollection inner) : IStyleCollection
    {
        // The default style collection enumerates all style sheets and rules again for every element (and its ancestors).
        // Inlining does not change the style sheets, so the rules can be collected once per document.
        private readonly List<ICssStyleRule> rules = inner.ToList();

        public IRenderDevice Device => inner.Device;

        public IEnumerator<ICssStyleRule> GetEnumerator()
        {
            return rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return rules.GetEnumerator();
        }
    }
}
