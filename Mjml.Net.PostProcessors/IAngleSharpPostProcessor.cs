using AngleSharp.Dom;

namespace Mjml.Net;

public interface IAngleSharpPostProcessor
{
    bool ShouldProcess(string html)
    {
        return true;
    }

    ValueTask ProcessAsync(IDocument document, MjmlOptions options,
        CancellationToken ct);
}
