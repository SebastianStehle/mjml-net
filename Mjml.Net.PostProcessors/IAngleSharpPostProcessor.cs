using AngleSharp.Dom;

namespace Mjml.Net;

public interface IAngleSharpPostProcessor
{
    ValueTask ProcessAsync(IDocument document, MjmlOptions options,
        CancellationToken ct);
}
