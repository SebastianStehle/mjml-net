using System.Diagnostics.CodeAnalysis;
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

    ReadOnlySpan<char> NameAsSpan { get; }

    ReadOnlySpan<char> TextAsSpan { get; }

    bool SelfClosingElement { get; }

    HtmlTokenKind TokenKind { get; }

    bool Read();

    string GetAttribute(string name);

    bool TryGetAttribute(string name, [NotNullWhen(true)] out string? value);

    string GetAttribute(int index);

    string GetAttributeName(int index);

    ReadOnlySpan<char> GetAttributeNameAsSpan(int index);

    InnerTextOrHtml ReadInnerHtml();

    InnerTextOrHtml ReadInnerText();

    IHtmlReader ReadSubtree();
}
