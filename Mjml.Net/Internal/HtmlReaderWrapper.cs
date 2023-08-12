using HtmlPerformanceKit;
using HtmlReaderImpl = HtmlPerformanceKit.HtmlReader;

namespace Mjml.Net.Internal;

internal class HtmlReaderWrapper : IHtmlReader
{
    private static readonly char[] TrimChars = { ' ', '\n', '\r' };
    private readonly HtmlReaderImpl inner;

    public int LineNumber => inner.LineNumber;

    public int LinePosition => inner.LinePosition;

    public int AttributeCount => inner.AttributeCount;

    public string Name => inner.Name;

    public string Text => inner.Text;

    public bool SelfClosingElement => inner.SelfClosingElement;

    public HtmlTokenKind TokenKind => inner.TokenKind;

    public HtmlReaderWrapper(HtmlReaderImpl inner)
    {
        this.inner = inner;
    }

    public HtmlReaderWrapper(string input)
    {
        inner = new HtmlReaderImpl(new StringReader(input));
    }

    public string GetAttribute(string name)
    {
        return inner.GetAttribute(name);
    }

    public string GetAttribute(int index)
    {
        return inner.GetAttribute(index);
    }

    public string GetAttributeName(int index)
    {
        return inner.GetAttributeName(index);
    }

    public virtual bool Read()
    {
        return inner.Read();
    }

    public IHtmlReader ReadSubtree()
    {
        return new SubtreeReader(inner);
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
                    result.AddNonEmpty(subTree.Text);
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
                    result.AddNonEmpty(subTree.Text);
                    break;
            }
        }

        return result;
    }
}
