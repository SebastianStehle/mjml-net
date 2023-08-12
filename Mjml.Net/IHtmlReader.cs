using HtmlPerformanceKit;

namespace Mjml.Net;

public interface IHtmlReader
{
    int LineNumber { get; }

    int LinePosition { get; }

    int AttributeCount { get; }

    string Name { get; }

    string Text { get; }

    bool SelfClosingElement { get; }

    HtmlTokenKind TokenKind { get; }

    bool Read();

    string GetAttribute(string name);

    string GetAttribute(int index);

    string GetAttributeName(int index);

    string ReadInnerHtml();

    string ReadInnerText();

    IHtmlReader ReadSubtree();
}
