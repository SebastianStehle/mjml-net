using HtmlPerformanceKit;
using HtmlReaderImpl = HtmlPerformanceKit.HtmlReader;

namespace Mjml.Net.Internal;

internal class HtmlReaderWrapper : IHtmlReader
{
    private readonly HtmlReaderImpl impl;

    public int LineNumber => impl.LineNumber;

    public int LinePosition => impl.LinePosition;

    public int AttributeCount => impl.AttributeCount;

    public string Name => impl.Name;

    public string Text => impl.Text;

    public bool SelfClosingElement => impl.SelfClosingElement;

    public HtmlTokenKind TokenKind => impl.TokenKind;

    public HtmlReaderImpl Impl => impl;

    public HtmlReaderWrapper(HtmlReaderImpl inner)
    {
        this.impl = inner;
    }

    public HtmlReaderWrapper(string input)
    {
        impl = new HtmlReaderImpl(new StringReader(input));
    }

    public string GetAttribute(string name)
    {
        return impl.GetAttribute(name);
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
        return impl.Read();
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
