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

    public bool ShouldProcess(string html)
    {
        return HasInlineStyle(html);
    }

    public ValueTask ProcessAsync(IDocument document, MjmlOptions options,
        CancellationToken ct)
    {
        // Like mjml, only inline when there are inline styles at all.
        var inlineStyles = document.QuerySelectorAll(TagNames.Style).Where(IsInline).ToList();
        if (inlineStyles.Count == 0)
        {
            return default;
        }

        // Disable the other style sheets, so that only the inline styles are applied.
        foreach (var style in document.QuerySelectorAll(TagNames.Style).Where(x => !IsInline(x)).ToList())
        {
            RenameTag(style, FallbackStyle, document);
        }

        var styles = GetStyles(document);
        if (styles != null)
        {
            // Like juice in mjml, only elements that are matched by an inline rule get styles.
            // Inherited properties are not copied and all other style attributes are left untouched.
            foreach (var element in GetMatchedElements(document, styles))
            {
                InlineStyle(element, styles);
            }
        }

        foreach (var style in document.QuerySelectorAll(FallbackStyle).ToList())
        {
            RenameTag(style, TagNames.Style, document);
        }

        foreach (var style in inlineStyles)
        {
            style.Remove();
        }

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

    private static List<IElement> GetMatchedElements(IDocument document, IStyleCollection styles)
    {
        var matched = new HashSet<IElement>();

        foreach (var rule in styles)
        {
            IHtmlCollection<IElement> elements;
            try
            {
                elements = document.QuerySelectorAll(rule.SelectorText);
            }
            catch (DomException)
            {
                // Selectors that cannot be queried cannot be inlined anyway.
                continue;
            }

            foreach (var element in elements)
            {
                matched.Add(element);
            }
        }

        // Keep the document order to be deterministic.
        return document.All.Where(matched.Contains).ToList();
    }

    private static void InlineStyle(IElement element, IStyleCollection styles)
    {
        var currentStyle = styles.ComputeExplicitStyle(element);
        if (currentStyle.Any())
        {
            var css = currentStyle.ToCss();

            element.SetAttribute(TagNames.Style, css);
        }
    }

    internal static bool HasInlineStyle(string html)
    {
        var span = html.AsSpan();

        while (true)
        {
            var start = span.IndexOf("<style", StringComparison.OrdinalIgnoreCase);
            if (start < 0)
            {
                return false;
            }

            span = span[(start + 6)..];

            var end = span.IndexOf('>');
            if (end < 0)
            {
                return false;
            }

            if (span[..end].Contains("inline", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            span = span[end..];
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
