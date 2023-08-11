using HtmlPerformanceKit;

namespace Mjml.Net.Internal;

internal sealed class SubtreeReader : HtmlReaderWrapper
{
    private static readonly HashSet<string> VoidTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "area",
        "base",
        "br",
        "col",
        "embed",
        "hr",
        "img",
        "input",
        "link",
        "meta",
        "param",
        "source",
        "track",
        "wbr",
    };

    private readonly HtmlReader inner;
    private int depth = 1;

    public SubtreeReader(HtmlReader inner)
        : base(inner)
    {
        this.inner = inner;
    }

    public override bool Read()
    {
        if (depth == 0)
        {
            return false;
        }

        var hasRead = base.Read();

        if (hasRead)
        {
            if (TokenKind == HtmlTokenKind.Tag && !inner.SelfClosingElement && !VoidTags.Contains(inner.Name))
            {
                depth++;
            }
            else if (TokenKind == HtmlTokenKind.EndTag)
            {
                depth--;
            }
        }

        return hasRead;
    }
}
