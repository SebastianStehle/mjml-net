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

    public ReadOnlySpan<char> GetAttributeNameAsSpan(int index)
    {
        return impl.GetAttributeNameAsMemory(index).Span;
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
        var sb = DefaultPools.StringBuilders.Get();
        try
        {
            // Whitespace-only text at the end is not rendered, so the content ends after the last other token.
            var contentEnd = -1;

            var subTree = ReadSubtree();

            while (subTree.Read())
            {
                switch (TokenKind)
                {
                    case HtmlTokenKind.Text:
                        var text = impl.TextAsMemory.Span;

                        sb.Append(text);

                        if (contentEnd < 0 || !text.IsWhiteSpace())
                        {
                            contentEnd = sb.Length;
                        }

                        break;
                    case HtmlTokenKind.Tag:
                        sb.Append('<');
                        sb.Append(impl.NameAsMemory.Span);

                        for (var i = 0; i < impl.AttributeCount; i++)
                        {
                            sb.Append(' ');
                            sb.Append(impl.GetAttributeNameAsMemory(i).Span);
                            sb.Append("=\"");
                            sb.Append(impl.GetAttributeAsMemory(i).Span);
                            sb.Append('"');
                        }

                        sb.Append(impl.SelfClosingElement ? "/>" : ">");
                        contentEnd = sb.Length;
                        break;
                    case HtmlTokenKind.Comment:
                        sb.Append("<!-- ");
                        sb.Append(impl.TextAsMemory.Span);
                        sb.Append(" -->");
                        contentEnd = sb.Length;
                        break;
                    case HtmlTokenKind.EndTag:
                        sb.Append("</");
                        sb.Append(impl.NameAsMemory.Span);
                        sb.Append('>');
                        contentEnd = sb.Length;
                        break;
                }
            }

            return new InnerTextOrHtml(contentEnd < 0 ? string.Empty : sb.ToString(0, contentEnd));
        }
        finally
        {
            DefaultPools.StringBuilders.Return(sb);
        }
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
