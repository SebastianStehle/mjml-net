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

    private readonly HtmlReaderWrapper inner;
    private int depth = 1;

    public SubtreeReader(HtmlReaderWrapper inner)
        : base(inner.Impl)
    {
        this.inner = inner;
    }

    public override bool Read()
    {
        if (depth == 0)
        {
            return false;
        }

        var hasRead = inner.Read();

        if (hasRead)
        {
            if (!VoidTags.Contains(inner.Name))
            {
                if (TokenKind == HtmlTokenKind.Tag && !inner.SelfClosingElement)
                {
                    depth++;
                }
                else if (TokenKind == HtmlTokenKind.EndTag)
                {
                    depth--;
                }
            }
        }
        else
        {
            depth = 0;
        }

        return depth > 0;
    }
}
