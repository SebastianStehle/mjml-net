using HtmlPerformanceKit;

namespace Mjml.Net;

public interface IHtmlReader
{
    public Action<HtmlError>? OnError { get; set; }

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

    InnerTextOrHtml ReadInnerHtml();

    InnerTextOrHtml ReadInnerText();

    IHtmlReader ReadSubtree();
}
