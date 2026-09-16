using System.Diagnostics.CodeAnalysis;
using HtmlPerformanceKit;
using HtmlReaderImpl = HtmlPerformanceKit.HtmlReader;

namespace Mjml.Net.Internal;

internal class HtmlReaderWrapper : IHtmlReader
{
    private static readonly HtmlReaderOptions Options = new HtmlReaderOptions
    {
        DecodeHtmlCharacters = false
    };

    private readonly HtmlReaderImpl impl;
    private readonly HtmlReaderWrapper root;
    private int depth;

    public Action<HtmlError>? OnError { get; set; }

    protected int Depth => root.depth;

    public int LineNumber => impl.LineNumber;

    public int LinePosition => impl.LinePosition;

    public int AttributeCount => impl.AttributeCount;

    public string Name => impl.Name;

    public string Text => impl.Text;

    public ReadOnlySpan<char> NameAsSpan => impl.NameAsMemory.Span;

    public ReadOnlySpan<char> TextAsSpan => impl.TextAsMemory.Span;

    public bool SelfClosingElement => impl.SelfClosingElement;

    public HtmlTokenKind TokenKind => impl.TokenKind;

    protected HtmlReaderWrapper(HtmlReaderWrapper parent)
    {
        impl = parent.impl;
        root = parent.root;
    }

    public HtmlReaderWrapper(string input)
    {
        root = this;
        impl = new HtmlReaderImpl(new StringReader(input), Options);

        impl.ParseError += (sender, e) =>
        {
            OnError?.Invoke(new HtmlError(e.LineNumber, e.LinePosition, e.Message));
        };
    }

    public string GetAttribute(string name)
    {
        return impl.GetAttribute(name);
    }

    public bool TryGetAttribute(string name, [NotNullWhen(true)] out string? value)
    {
        return impl.TryGetAttribute(name, out value);
    }

    public string GetAttribute(int index)
    {
        return impl.GetAttribute(index);
    }

    public string GetAttributeName(int index)
    {
        return impl.GetAttributeName(index);
    }

    public virtual bool Read()
    {
        return root.ReadToken();
    }

    private bool ReadToken()
    {
        if (!impl.Read())
        {
            return false;
        }

        // Track the depth for all subtree readers at once, so that each token is only checked once.
        if (impl.TokenKind == HtmlTokenKind.Tag && !impl.SelfClosingElement && !IsVoidTag(impl.NameAsMemory.Span))
        {
            depth++;
        }
        else if (impl.TokenKind == HtmlTokenKind.EndTag && !IsVoidTag(impl.NameAsMemory.Span))
        {
            depth--;
        }

        return true;
    }

    private static bool IsVoidTag(ReadOnlySpan<char> name)
    {
        switch (name.Length)
        {
            case 2:
                return
                    name.Equals("br", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("hr", StringComparison.OrdinalIgnoreCase);
            case 3:
                return
                    name.Equals("col", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("img", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("wbr", StringComparison.OrdinalIgnoreCase);
            case 4:
                return
                    name.Equals("area", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("base", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("link", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("meta", StringComparison.OrdinalIgnoreCase);
            case 5:
                return
                    name.Equals("embed", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("input", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("param", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("track", StringComparison.OrdinalIgnoreCase);
            case 6:
                return name.Equals("source", StringComparison.OrdinalIgnoreCase);
            default:
                return false;
        }
    }

    public IHtmlReader ReadSubtree()
    {
        return new SubtreeReader(this);
    }

    public InnerTextOrHtml ReadInnerHtml()
    {
        var result = new InnerTextOrHtml();

        var subTree = ReadSubtree();

        while (subTree.Read())
        {
            switch (TokenKind)
            {
                case HtmlTokenKind.Text:
                    result.Add(subTree.Text);
                    break;
                case HtmlTokenKind.Tag:
                    result.Add("<");
                    result.Add(subTree.Name);

                    for (var i = 0; i < subTree.AttributeCount; i++)
                    {
                        var attributeName = subTree.GetAttributeName(i);
                        var attributeValue = subTree.GetAttribute(i);

                        result.Add(" ");
                        result.Add(attributeName);
                        result.Add("=");
                        result.Add("\"");
                        result.Add(attributeValue);
                        result.Add("\"");
                    }

                    if (subTree.SelfClosingElement)
                    {
                        result.Add("/>");
                    }
                    else
                    {
                        result.Add(">");
                    }
                    break;
                case HtmlTokenKind.Comment:
                    result.Add("<!-- ");
                    result.Add(subTree.Text);
                    result.Add(" -->");
                    break;
                case HtmlTokenKind.EndTag:
                    result.Add("</");
                    result.Add(subTree.Name);
                    result.Add(">");
                    break;
            }
        }

        return result;
    }

    public InnerTextOrHtml ReadInnerText()
    {
        var result = new InnerTextOrHtml();

        var subTree = ReadSubtree();

        while (subTree.Read())
        {
            switch (TokenKind)
            {
                case HtmlTokenKind.Text:
                    result.Add(subTree.Text);
                    break;
            }
        }

        return result;
    }
}
