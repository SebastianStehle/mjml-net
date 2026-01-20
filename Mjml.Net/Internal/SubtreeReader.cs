using HtmlPerformanceKit;

namespace Mjml.Net.Internal;

internal sealed class SubtreeReader(HtmlReaderWrapper inner) : HtmlReaderWrapper(inner.Impl)
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
    private int depth = 1;

    public override bool Read()
    {
        if (depth == 0)
        {
            return false;
        }

        var hasRead = inner.Read();

        if (hasRead)
        {
            if (TokenKind == HtmlTokenKind.Tag && !VoidTags.Contains(inner.Name) && !inner.SelfClosingElement)
            {
                depth++;
            }
            else if (TokenKind == HtmlTokenKind.EndTag && !VoidTags.Contains(inner.Name))
            {
                depth--;
            }
        }
        else
        {
            depth = 0;
        }

        return depth > 0;
    }
}
